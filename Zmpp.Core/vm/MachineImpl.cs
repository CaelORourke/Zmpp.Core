/*
 * Created on 2005/10/03
 * Copyright (c) 2005-2010, Wei-ju Wu.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * Neither the name of Wei-ju Wu nor the names of its contributors may
 * be used to endorse or promote products derived from this software without
 * specific prior written permission.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

namespace org.zmpp.vm
{
    using Microsoft.Extensions.Logging;
    using org.zmpp.@base;
    using org.zmpp.encoding;
    using org.zmpp.iff;
    using org.zmpp.io;
    using org.zmpp.media;
    using org.zmpp.vmutil;
    using org.zmpp.windowing;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class implements the state and some services of a Z-machine, version 3.
    /// </summary>
    public class MachineImpl : IMachine, IDrawingArea
    {
        private const long serialVersionUID = -8497998738628466785L;
        private readonly ILogger LOG;

        /// <summary>
        /// Number of undo steps.
        /// </summary>
        private const int NUM_UNDO = 5;

        private MachineRunState runstate;
        private IRandomGenerator _random;
        private IStatusLine statusLine;
        private IScreenModel screenModel;
        private ISaveGameDataStore datastore;
        private RingBuffer<PortableGameState> undostates;
        private InputFunctions inputFunctions;
        private ISoundSystem soundSystem;
        private IPictureManager pictureManager;
        private ICpu cpu;
        private OutputImpl output;
        private InputImpl input;

        // Formerly GameData
        private IStoryFileHeader fileheader;
        private IMemory memory;
        private IDictionary dictionary;
        private IObjectTree objectTree;
        private IZsciiEncoding encoding;
        private IZCharDecoder decoder;
        private ZCharEncoder encoder;
        private IAlphabetTable alphabetTable;
        private IResources resources;
        private byte[] storyfileData;
        private int checksum;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">the logger</param>
        public MachineImpl(ILogger logger)
        {
            this.LOG = logger;
            this.inputFunctions = new InputFunctions(this);
        }

        #region Initialization

        public void initialize(byte[] data, IResources aResources)
        {
            this.storyfileData = data;
            this.resources = aResources;
            this._random = new UnpredictableRandomGenerator();
            this.undostates = new RingBuffer<PortableGameState>(NUM_UNDO);

            cpu = new CpuImpl(this.LOG, this);
            output = new OutputImpl(this);
            input = new InputImpl();

            IMediaCollection<ISoundEffect> sounds = null;
            IMediaCollection <IZmppImage > pictures = null;
            int resourceRelease = 0;

            if (resources != null)
            {
                sounds = resources.getSounds();
                pictures = resources.getImages();
                resourceRelease = resources.getRelease();
            }

            // TODO: Implement these!
            //soundSystem = new SoundSystemImpl(sounds);
            //pictureManager = new PictureManagerImpl(resourceRelease, this, pictures);

            resetState();
        }

        /// <summary>
        /// Resets the data.
        /// </summary>
        public void resetGameData()
        {
            // Make a copy and initialize from the copy
            byte[] data = new byte[storyfileData.Length];
            Array.Copy(storyfileData, 0, data, 0, storyfileData.Length);

            memory = new DefaultMemory(data);
            fileheader = new DefaultStoryFileHeader(memory);
            checksum = calculateChecksum();

            IDictionarySizes dictionarySizes = (fileheader.getVersion() <= 3) ?
                (IDictionarySizes)new DictionarySizesV1ToV3() : (IDictionarySizes)new DictionarySizesV4ToV8();
            // Install the whole character code system here
            initEncodingSystem(dictionarySizes);

            // The object tree and dictionaries depend on the code system
            if (fileheader.getVersion() <= 3)
            {
                objectTree = new ClassicObjectTree(memory,
                    memory.readUnsigned16(StoryFileHeaderBase.OBJECT_TABLE));
            }
            else
            {
                objectTree = new ModernObjectTree(memory,
                    memory.readUnsigned16(StoryFileHeaderBase.OBJECT_TABLE));
            }
            // CAUTION: the current implementation of DefaultDictionary reads in all
            // entries into a hash table, so it will break when moving this statement
            // to a different position
            dictionary = (IDictionary)new DefaultDictionary(memory,
                memory.readUnsigned16(StoryFileHeaderBase.DICTIONARY), decoder, encoder,
                                      dictionarySizes);
        }

        /// <summary>
        /// Initializes the encoding system.
        /// </summary>
        /// <param name="dictionarySizes">the DictionarySizes</param>
        private void initEncodingSystem(IDictionarySizes dictionarySizes)
        {
            IAccentTable accentTable = (fileheader.getCustomAccentTable() == 0) ?
                (IAccentTable)new DefaultAccentTable() :
                (IAccentTable)new CustomAccentTable(memory, fileheader.getCustomAccentTable());
            encoding = new ZsciiEncoding(accentTable);

            // Configure the alphabet table
            char customAlphabetTable =
                memory.readUnsigned16(StoryFileHeaderBase.CUSTOM_ALPHABET);
            if (customAlphabetTable == 0)
            {
                if (fileheader.getVersion() == 1)
                {
                    alphabetTable = new AlphabetTableV1();
                }
                else if (fileheader.getVersion() == 2)
                {
                    alphabetTable = new AlphabetTableV2();
                }
                else
                {
                    alphabetTable = new DefaultAlphabetTable();
                }
            }
            else
            {
                alphabetTable = new CustomAlphabetTable(memory, customAlphabetTable);
            }

            IZCharTranslator translator =
              new DefaultZCharTranslator(alphabetTable);

            Abbreviations abbreviations = new Abbreviations(memory,
                memory.readUnsigned16(StoryFileHeaderBase.ABBREVIATIONS));
            decoder = new DefaultZCharDecoder(encoding, translator, abbreviations);
            encoder = new ZCharEncoder(translator, dictionarySizes);
        }

        /// <summary>
        /// Calculates the checksum of the file.
        /// </summary>
        /// <returns>the check sum</returns>
        private int calculateChecksum()
        {
            int filelen = fileheader.getFileLength();
            int sum = 0;
            for (int i = 0x40; i < filelen; i++)
            {
                sum += getMemory().readUnsigned8(i);
            }
            return (sum & 0xffff);
        }

        public int getVersion()
        {
            return getFileHeader().getVersion();
        }

        public int getRelease()
        {
            return getMemory().readUnsigned16(StoryFileHeaderBase.RELEASE);
        }

        public bool hasValidChecksum()
        {
            return this.checksum == getChecksum();
        }

        public IStoryFileHeader getFileHeader() { return fileheader; }

        public IResources getResources() { return resources; }

        #endregion

        #region Memory interface functionality

        /// <summary>
        /// Returns the memory object.
        /// </summary>
        /// <returns>memory object</returns>
        private IMemory getMemory() { return memory; }

        public char readUnsigned16(int address)
        {
            return getMemory().readUnsigned16(address);
        }

        public char readUnsigned8(int address)
        {
            return getMemory().readUnsigned8(address);
        }

        public void writeUnsigned16(int address, char value)
        {
            getMemory().writeUnsigned16(address, value);
        }

        public void writeUnsigned8(int address, char value)
        {
            getMemory().writeUnsigned8(address, value);
        }

        public void copyBytesToArray(byte[] dstData, int dstOffset, int srcOffset, int numBytes)
        {
            getMemory().copyBytesToArray(dstData, dstOffset, srcOffset, numBytes);
        }

        public void copyBytesFromArray(byte[] srcData, int srcOffset, int dstOffset, int numBytes)
        {
            getMemory().copyBytesFromArray(srcData, srcOffset, dstOffset, numBytes);
        }

        public void copyBytesFromMemory(IMemory srcMem, int srcOffset, int dstOffset, int numBytes)
        {
            getMemory().copyBytesFromMemory(srcMem, srcOffset, dstOffset, numBytes);
        }

        public void copyArea(int src, int dst, int numBytes)
        {
            getMemory().copyArea(src, dst, numBytes);
        }

        #endregion

        #region Cpu interface functionality

        /// <summary>
        /// Returns the Cpu object.
        /// </summary>
        /// <returns>cpu object</returns>
        private ICpu getCpu() { return cpu; }

        public char getVariable(char varnum) { return getCpu().getVariable(varnum); }

        public void setVariable(char varnum, char value)
        {
            getCpu().setVariable(varnum, value);
        }

        public char getStackTop() { return getCpu().getStackTop(); }

        public char getStackElement(int index)
        {
            return getCpu().getStackElement(index);
        }

        public void setStackTop(char value) { getCpu().setStackTop(value); }

        public void incrementPC(int length) { getCpu().incrementPC(length); }

        public void setPC(int address) { getCpu().setPC(address); }

        public int getPC() { return getCpu().getPC(); }

        public char getSP() { return getCpu().getSP(); }

        public char popStack(char userstackAddress)
        {
            return getCpu().popStack(userstackAddress);
        }

        public bool pushStack(char stack, char value)
        {
            return getCpu().pushStack(stack, value);
        }

        public List<RoutineContext> getRoutineContexts()
        {
            return getCpu().getRoutineContexts();
        }

        public void setRoutineContexts(List<RoutineContext> routineContexts)
        {
            getCpu().setRoutineContexts(routineContexts);
        }

        public void returnWith(char returnValue)
        {
            getCpu().returnWith(returnValue);
        }

        public RoutineContext getCurrentRoutineContext()
        {
            return getCpu().getCurrentRoutineContext();
        }

        public int unpackStringAddress(char packedAddress)
        {
            return getCpu().unpackStringAddress(packedAddress);
        }

        public RoutineContext call(char packedAddress, int returnAddress, char[] args, char returnVar)
        {
            return getCpu().call(packedAddress, returnAddress, args, returnVar);
        }

        public void doBranch(short branchOffset, int instructionLength)
        {
            getCpu().doBranch(branchOffset, instructionLength);
        }

        #endregion

        #region Dictionary functionality

        private const String WHITESPACE = " \n\t\r";

        /// <summary>
        /// Returns the dictionary object.
        /// </summary>
        /// <returns>dictionary object</returns>
        private IDictionary getDictionary() { return dictionary; }

        public int lookupToken(int dictionaryAddress, String token)
        {
            if (dictionaryAddress == 0)
            {
                return getDictionary().lookup(token);
            }
            return new UserDictionary(getMemory(), dictionaryAddress, getZCharDecoder(), encoder).lookup(token);
        }

        public char[] getDictionaryDelimiters()
        {
            // Retrieve the defined separators
            StringBuilder separators = new StringBuilder();
            separators.Append(WHITESPACE);
            for (int i = 0, n = getDictionary().getNumberOfSeparators(); i < n; i++)
            {
                separators.Append(getZCharDecoder().decodeZChar((char)
                        getDictionary().getSeparator(i)));
            }
            // The tokenizer will also return the delimiters
            return separators.ToString().ToCharArray();
        }

        #endregion

        #region Encoding functionality

        /// <summary>
        /// Returns the decoder object.
        /// </summary>
        /// <returns>decoder object</returns>
        private IZCharDecoder getZCharDecoder() { return decoder; }

        /// <summary>
        /// Returns the encoder object.
        /// </summary>
        /// <returns>encoder object</returns>
        private ZCharEncoder getZCharEncoder() { return encoder; }

        public String convertToZscii(String str)
        {
            return encoding.convertToZscii(str);
        }

        public void encode(int source, int length, int destination)
        {
            getZCharEncoder().encode(getMemory(), source, length, destination);
        }

        public int getNumZEncodedBytes(int address)
        {
            return getZCharDecoder().getNumZEncodedBytes(getMemory(), address);
        }

        public String decode2Zscii(int address, int length)
        {
            return getZCharDecoder().decode2Zscii(getMemory(), address, length);
        }

        public char getUnicodeChar(char zsciiChar)
        {
            return encoding.getUnicodeChar(zsciiChar);
        }

        #endregion

        #region Output stream management

        // **********************************************************************
        // ***** Output stream management, implemented by the OutputImpl object
        // **********************************************************************

        /**
         * Sets the output stream to the specified number.
         * @param streamnumber the stream number
         * @param stream the output stream
         */
        public void setOutputStream(int streamnumber, IOutputStream stream)
        {
            output.setOutputStream(streamnumber, stream);
        }

        public void selectOutputStream(int streamnumber, bool flag)
        {
            output.selectOutputStream(streamnumber, flag);
        }

        public void selectOutputStream3(int tableAddress, int tableWidth)
        {
            output.selectOutputStream3(tableAddress, tableWidth);
        }

        public void printZString(int stringAddress)
        {
            output.printZString(stringAddress);
        }

        public void print(String str)
        {
            output.print(str);
        }

        public void newline()
        {
            output.newline();
        }

        public void printZsciiChar(char zchar)
        {
            output.printZsciiChar(zchar);
        }

        public void printNumber(short num)
        {
            output.printNumber(num);
        }

        public void flushOutput()
        {
            output.flushOutput();
        }

        public void reset()
        {
            output.reset();
        }

        #endregion

        #region Input stream management

        // **********************************************************************
        // ***** Input stream management, implemented by the InputImpl object
        // ********************************************************************

        /**
         * Sets an input stream to the specified number.
         * @param streamNumber the input stream number
         * @param stream the input stream to set
         */
        public void setInputStream(int streamNumber, IInputStream stream)
        {
            input.setInputStream(streamNumber, stream);
        }

        public IInputStream getSelectedInputStream()
        {
            return input.getSelectedInputStream();
        }

        public void selectInputStream(int streamNumber)
        {
            input.selectInputStream(streamNumber);
        }

        public char random(short range)
        {
            if (range < 0)
            {
                _random = new PredictableRandomGenerator(-range);
                return (char)0;
            }
            else if (range == 0)
            {
                _random = new UnpredictableRandomGenerator();
                return (char)0;
            }
            return (char)((_random.next() % range) + 1);
        }

        #endregion

        #region Control functions

        public MachineRunState getRunState() { return runstate; }


        public void setRunState(MachineRunState aRunstate)
        {
            this.runstate = aRunstate;
            if (runstate != null && runstate.isWaitingForInput())
            {
                updateStatusLine();
                flushOutput();
            }
        }

        public void halt(String errormsg)
        {
            print(errormsg);
            runstate = MachineRunState.STOPPED;
        }

        public void warn(String msg)
        {
            LOG.LogWarning("WARNING: " + msg);
        }

        public void restart() { restart(true); }

        public void quit()
        {
            runstate = MachineRunState.STOPPED;
            // On quit, close the streams
            output.print("*Game ended*");
            closeStreams();
        }

        public void start() { runstate = MachineRunState.RUNNING; }

        #endregion

        #region Machine services

        public void tokenize(int textbuffer, int parsebuffer, int dictionaryAddress, bool flag)
        {
            inputFunctions.tokenize(textbuffer, parsebuffer, dictionaryAddress, flag);
        }

        public char readLine(int textbuffer)
        {
            return inputFunctions.readLine(textbuffer);
        }

        public char readChar() { return inputFunctions.readChar(); }

        public ISoundSystem getSoundSystem() { return soundSystem; }

        public IPictureManager getPictureManager() { return pictureManager; }

        public void setSaveGameDataStore(ISaveGameDataStore aDatastore)
        {
            this.datastore = aDatastore;
        }

        public void updateStatusLine()
        {
            if (getFileHeader().getVersion() <= 3 && statusLine != null)
            {
                int objNum = cpu.getVariable((char)0x10);
                String objectName = getZCharDecoder().decode2Zscii(getMemory(),
                  getObjectTree().getPropertiesDescriptionAddress(objNum), 0);
                int global2 = cpu.getVariable((char)0x11);
                int global3 = cpu.getVariable((char)0x12);
                if (getFileHeader().isEnabled(StoryFileHeaderAttribute.SCORE_GAME))
                {
                    statusLine.updateStatusScore(objectName, global2, global3);
                }
                else
                {
                    statusLine.updateStatusTime(objectName, global2, global3);
                }
            }
        }

        public void setStatusLine(IStatusLine statusLine)
        {
            this.statusLine = statusLine;
        }

        public void setScreen(IScreenModel screen)
        {
            this.screenModel = screen;
        }

        public IScreenModel getScreen() { return screenModel; }

        public IScreenModel6 getScreen6() { return (IScreenModel6)screenModel; }

        public bool save(int savepc)
        {
            if (datastore != null)
            {
                PortableGameState gamestate = new PortableGameState();
                gamestate.captureMachineState(this, savepc);
                WritableFormChunk formChunk = gamestate.exportToFormChunk();
                return datastore.saveFormChunk(formChunk);
            }
            return false;
        }

        public bool save_undo(int savepc)
        {
            PortableGameState undoGameState = new PortableGameState();
            undoGameState.captureMachineState(this, savepc);
            undostates.add(undoGameState);
            return true;
        }

        public PortableGameState restore()
        {
            if (datastore != null)
            {
                PortableGameState gamestate = new PortableGameState();
                IFormChunk formchunk = datastore.retrieveFormChunk();
                gamestate.readSaveGame(formchunk);

                // verification has to be here
                if (verifySaveGame(gamestate))
                {
                    // do not reset screen model, since e.g. AMFV simply picks up the
                    // current window state
                    restart(false);
                    gamestate.transferStateToMachine(this);
                    return gamestate;
                }
            }
            return null;
        }

        public PortableGameState restore_undo()
        {
            // do not reset screen model, since e.g. AMFV simply picks up the
            // current window state
            if (undostates.size() > 0)
            {
                PortableGameState undoGameState =
                  undostates.remove(undostates.size() - 1);
                restart(false);
                undoGameState.transferStateToMachine(this);
                LOG.LogInformation(String.Format("restore(), pc is: %4x\n", cpu.getPC()));
                return undoGameState;
            }
            return null;
        }

        #endregion

        #region Private methods

        /**
         * Verifies the integrity of the save game.
         * @param gamestate PortableGameState
         * @return true if valid, false otherwise
         */
        private bool verifySaveGame(PortableGameState gamestate)
        {
            // Verify the game according to the standard
            int saveGameChecksum = getChecksum();
            if (saveGameChecksum == 0)
            {
                saveGameChecksum = this.checksum;
            }
            return gamestate.getRelease() == getRelease()
              && gamestate.getChecksum() == checksum
              && gamestate.getSerialNumber().Equals(getFileHeader().getSerialNumber());
        }

        /**
         * Returns the checksum.
         * @return checksum
         */
        private int getChecksum()
        {
            return memory.readUnsigned16(StoryFileHeaderBase.CHECKSUM);
        }

        /**
         * Close the streams.
         */
        private void closeStreams()
        {
            input.close();
            output.close();
        }

        /**
         * Resets all state to initial values, using the configuration object.
         */
        private void resetState()
        {
            resetGameData();
            output.reset();
            //soundSystem.reset(); // TODO: Implement sound system!
            cpu.reset();
            setStandardRevision(1, 0);
            if (getFileHeader().getVersion() >= 4)
            {
                getFileHeader().setEnabled(StoryFileHeaderAttribute.SUPPORTS_TIMED_INPUT, true);
                // IBM PC
                getMemory().writeUnsigned8(StoryFileHeaderBase.INTERPRETER_NUMBER, (char)6);
                getFileHeader().setInterpreterVersion(1);
            }
        }

        /**
         * Sets standard revision.
         * @param major major revision number
         * @param minor minor revision number
         */
        private void setStandardRevision(int major, int minor)
        {
            memory.writeUnsigned8(StoryFileHeaderBase.STD_REVISION_MAJOR, (char)major);
            memory.writeUnsigned8(StoryFileHeaderBase.STD_REVISION_MINOR, (char)minor);
        }

        /**
         * Restarts the VM.
         * @param resetScreenModel true if screen model should be reset
         */
        private void restart(bool resetScreenModel)
        {
            // Transcripting and fixed font bits survive the restart
            IStoryFileHeader fileHeader = getFileHeader();
            bool fixedFontForced =
              fileHeader.isEnabled(StoryFileHeaderAttribute.FORCE_FIXED_FONT);
            bool transcripting = fileHeader.isEnabled(StoryFileHeaderAttribute.TRANSCRIPTING);

            resetState();

            if (resetScreenModel)
            {
                screenModel.reset();
            }
            fileHeader.setEnabled(StoryFileHeaderAttribute.TRANSCRIPTING, transcripting);
            fileHeader.setEnabled(StoryFileHeaderAttribute.FORCE_FIXED_FONT, fixedFontForced);
        }

        #endregion

        #region Object accesss

        /**
         * Returns the object tree.
         * @return object tree
         */
        private IObjectTree getObjectTree() { return objectTree; }

        public void insertObject(int parentNum, int objectNum)
        {
            getObjectTree().insertObject(parentNum, objectNum);
        }

        public void removeObject(int objectNum)
        {
            getObjectTree().removeObject(objectNum);
        }

        public void clearAttribute(int objectNum, int attributeNum)
        {
            getObjectTree().clearAttribute(objectNum, attributeNum);
        }

        public bool isAttributeSet(int objectNum, int attributeNum)
        {
            return getObjectTree().isAttributeSet(objectNum, attributeNum);
        }

        public void setAttribute(int objectNum, int attributeNum)
        {
            getObjectTree().setAttribute(objectNum, attributeNum);
        }

        public int getParent(int objectNum)
        {
            return getObjectTree().getParent(objectNum);
        }

        public void setParent(int objectNum, int parent)
        {
            getObjectTree().setParent(objectNum, parent);
        }

        public int getChild(int objectNum)
        {
            return getObjectTree().getChild(objectNum);
        }

        public void setChild(int objectNum, int child)
        {
            getObjectTree().setChild(objectNum, child);
        }

        public int getSibling(int objectNum)
        {
            return getObjectTree().getSibling(objectNum);
        }

        public void setSibling(int objectNum, int sibling)
        {
            getObjectTree().setSibling(objectNum, sibling);
        }

        public int getPropertiesDescriptionAddress(int objectNum)
        {
            return getObjectTree().getPropertiesDescriptionAddress(objectNum);
        }

        public int getPropertyAddress(int objectNum, int property)
        {
            return getObjectTree().getPropertyAddress(objectNum, property);
        }

        public int getPropertyLength(int propertyAddress)
        {
            return getObjectTree().getPropertyLength(propertyAddress);
        }

        public char getProperty(int objectNum, int property)
        {
            return getObjectTree().getProperty(objectNum, property);
        }

        public void setProperty(int objectNum, int property, char value)
        {
            getObjectTree().setProperty(objectNum, property, value);
        }

        public int getNextProperty(int objectNum, int property)
        {
            return getObjectTree().getNextProperty(objectNum, property);
        }

        public Resolution getResolution()
        {
            return getScreen6().getResolution();
        }

        #endregion
    }
}
