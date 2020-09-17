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

namespace Zmpp.Core.Vm
{
    using Microsoft.Extensions.Logging;
    using Zmpp.Core;
    using Zmpp.Core.Encoding;
    using Zmpp.Core.Iff;
    using Zmpp.Core.IO;
    using Zmpp.Core.Media;
    using Zmpp.Core.Vm.Utility;
    using Zmpp.Core.UI;
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
        private const int NumUndo = 5;

        private MachineRunState runstate;
        private IRandomGenerator _random;
        private IStatusLine statusLine;
        private IScreenModel screenModel;
        private ISaveGameDataStore datastore;
        private RingBuffer<PortableGameState> undostates;
        private readonly InputFunctions inputFunctions;
        private readonly ISoundSystem soundSystem;
        private readonly IPictureManager pictureManager;
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
            this.input = new InputImpl();
            this.output = new OutputImpl(this);
        }

        #region Initialization

        public void Initialize(byte[] data, IResources aResources)
        {
            this.storyfileData = data;
            this.resources = aResources;
            this._random = new UnpredictableRandomGenerator();
            this.undostates = new RingBuffer<PortableGameState>(NumUndo);

            cpu = new CpuImpl(this.LOG, this);
            //output = new OutputImpl(this);
            //input = new InputImpl();

            IMediaCollection<ISoundEffect> sounds = null;
            IMediaCollection <IZmppImage > pictures = null;
            int resourceRelease = 0;

            if (resources != null)
            {
                sounds = resources.Sounds;
                pictures = resources.Images;
                resourceRelease = resources.Release;
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

            memory = new Memory(data);
            fileheader = new StoryFileHeader(memory);
            checksum = calculateChecksum();

            IDictionarySizes dictionarySizes = (fileheader.Version <= 3) ?
                (IDictionarySizes)new DictionarySizesV1ToV3() : (IDictionarySizes)new DictionarySizesV4ToV8();
            // Install the whole character code system here
            initEncodingSystem(dictionarySizes);

            // The object tree and dictionaries depend on the code system
            if (fileheader.Version <= 3)
            {
                objectTree = new ClassicObjectTree(memory,
                    memory.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));
            }
            else
            {
                objectTree = new ModernObjectTree(memory,
                    memory.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));
            }
            // CAUTION: the current implementation of DefaultDictionary reads in all
            // entries into a hash table, so it will break when moving this statement
            // to a different position
            dictionary = (IDictionary)new DefaultDictionary(memory,
                memory.ReadUnsigned16(StoryFileHeaderAddress.Dictionary), decoder, encoder,
                                      dictionarySizes);
        }

        /// <summary>
        /// Initializes the encoding system.
        /// </summary>
        /// <param name="dictionarySizes">the DictionarySizes</param>
        private void initEncodingSystem(IDictionarySizes dictionarySizes)
        {
            IAccentTable accentTable = (fileheader.CustomAccentTableAddress == 0) ?
                (IAccentTable)new AccentTable() :
                (IAccentTable)new CustomAccentTable(memory, fileheader.CustomAccentTableAddress);
            encoding = new ZsciiEncoding(accentTable);

            // Configure the alphabet table
            char customAlphabetTable =
                memory.ReadUnsigned16(StoryFileHeaderAddress.CustomAlphabet);
            if (customAlphabetTable == 0)
            {
                if (fileheader.Version == 1)
                {
                    alphabetTable = new AlphabetTableV1();
                }
                else if (fileheader.Version == 2)
                {
                    alphabetTable = new AlphabetTableV2();
                }
                else
                {
                    alphabetTable = new AlphabetTable();
                }
            }
            else
            {
                alphabetTable = new CustomAlphabetTable(memory, customAlphabetTable);
            }

            IZCharTranslator translator =
              new ZCharTranslator(alphabetTable);

            Abbreviations abbreviations = new Abbreviations(memory,
                memory.ReadUnsigned16(StoryFileHeaderAddress.Abbreviations));
            decoder = new ZCharDecoder(encoding, translator, abbreviations);
            encoder = new ZCharEncoder(translator, dictionarySizes);
        }

        /// <summary>
        /// Calculates the checksum of the file.
        /// </summary>
        /// <returns>the check sum</returns>
        private int calculateChecksum()
        {
            int filelen = fileheader.FileLength;
            int sum = 0;
            for (int i = 0x40; i < filelen; i++)
            {
                sum += getMemory().ReadUnsigned8(i);
            }
            return (sum & 0xffff);
        }

        public int Version => FileHeader.Version;

        public int Release => getMemory().ReadUnsigned16(StoryFileHeaderAddress.Release);

        public bool HasValidChecksum => this.checksum == getChecksum();

        public IStoryFileHeader FileHeader => fileheader;
        public IResources Resources => resources;
        #endregion

        #region Memory interface functionality

        /// <summary>
        /// Returns the memory object.
        /// </summary>
        /// <returns>memory object</returns>
        private IMemory getMemory() { return memory; }

        public char ReadUnsigned16(int address)
        {
            return getMemory().ReadUnsigned16(address);
        }

        public char ReadUnsigned8(int address)
        {
            return getMemory().ReadUnsigned8(address);
        }

        public void WriteUnsigned16(int address, char value)
        {
            getMemory().WriteUnsigned16(address, value);
        }

        public void WriteUnsigned8(int address, char value)
        {
            getMemory().WriteUnsigned8(address, value);
        }

        public void CopyBytesToArray(byte[] dstData, int dstOffset, int srcOffset, int numBytes)
        {
            getMemory().CopyBytesToArray(dstData, dstOffset, srcOffset, numBytes);
        }

        public void CopyBytesFromArray(byte[] srcData, int srcOffset, int dstOffset, int numBytes)
        {
            getMemory().CopyBytesFromArray(srcData, srcOffset, dstOffset, numBytes);
        }

        public void CopyBytesFromMemory(IMemory srcMem, int srcOffset, int dstOffset, int numBytes)
        {
            getMemory().CopyBytesFromMemory(srcMem, srcOffset, dstOffset, numBytes);
        }

        public void CopyArea(int src, int dst, int numBytes)
        {
            getMemory().CopyArea(src, dst, numBytes);
        }

        #endregion

        #region Cpu interface functionality

        /// <summary>
        /// Returns the Cpu object.
        /// </summary>
        /// <returns>cpu object</returns>
        private ICpu getCpu() { return cpu; }

        public char GetVariable(char varnum) { return getCpu().GetVariable(varnum); }

        public void SetVariable(char varnum, char value)
        {
            getCpu().SetVariable(varnum, value);
        }

        public char StackTop => getCpu().StackTop;
        public char GetStackElement(int index)
        {
            return getCpu().GetStackElement(index);
        }

        public void setStackTop(char value) { getCpu().setStackTop(value); }

        public void IncrementPC(int length) { getCpu().IncrementPC(length); }

        public int PC
        {
            get
            {
                return getCpu().PC;
            }

            set
            {
                getCpu().PC = value;
            }
        }

        public char SP => getCpu().SP;
        public char PopStack(char userstackAddress)
        {
            return getCpu().PopStack(userstackAddress);
        }

        public bool PushStack(char stack, char value)
        {
            return getCpu().PushStack(stack, value);
        }

        public List<RoutineContext> getRoutineContexts()
        {
            return getCpu().getRoutineContexts();
        }

        public void setRoutineContexts(List<RoutineContext> routineContexts)
        {
            getCpu().setRoutineContexts(routineContexts);
        }

        public void ReturnWith(char returnValue)
        {
            getCpu().ReturnWith(returnValue);
        }

        public RoutineContext getCurrentRoutineContext()
        {
            return getCpu().getCurrentRoutineContext();
        }

        public int UnpackStringAddress(char packedAddress)
        {
            return getCpu().UnpackStringAddress(packedAddress);
        }

        public RoutineContext Call(char packedAddress, int returnAddress, char[] args, char returnVar)
        {
            return getCpu().Call(packedAddress, returnAddress, args, returnVar);
        }

        public void DoBranch(short branchOffset, int instructionLength)
        {
            getCpu().DoBranch(branchOffset, instructionLength);
        }

        #endregion

        #region Dictionary functionality

        private const String WHITESPACE = " \n\t\r";

        /// <summary>
        /// Returns the dictionary object.
        /// </summary>
        /// <returns>dictionary object</returns>
        private IDictionary getDictionary() { return dictionary; }

        public int LookupToken(int dictionaryAddress, String token)
        {
            if (dictionaryAddress == 0)
            {
                return getDictionary().Lookup(token);
            }
            return new UserDictionary(getMemory(), dictionaryAddress, getZCharDecoder(), encoder).Lookup(token);
        }

        public char[] DictionaryDelimiters
        {
            get
            {
                // Retrieve the defined separators
                StringBuilder separators = new StringBuilder();
                separators.Append(WHITESPACE);
                for (int i = 0, n = getDictionary().NumberOfSeparators; i < n; i++)
                {
                    separators.Append(getZCharDecoder().DecodeZChar((char)
                            getDictionary().GetSeparator(i)));
                }
                // The tokenizer will also return the delimiters
                return separators.ToString().ToCharArray();
            }
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

        public String ToZsciiString(String str)
        {
            return encoding.ToZsciiString(str);
        }

        public void Encode(int source, int length, int destination)
        {
            getZCharEncoder().Encode(getMemory(), source, length, destination);
        }

        public int GetNumZEncodedBytes(int address)
        {
            return getZCharDecoder().GetNumZEncodedBytes(getMemory(), address);
        }

        public String Decode2Zscii(int address, int length)
        {
            return getZCharDecoder().Decode2Zscii(getMemory(), address, length);
        }

        public char ToUnicodeChar(char zsciiChar)
        {
            return encoding.ToUnicodeChar(zsciiChar);
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

        public void SelectOutputStream(int streamnumber, bool flag)
        {
            output.SelectOutputStream(streamnumber, flag);
        }

        public void SelectOutputStream3(int tableAddress, int tableWidth)
        {
            output.SelectOutputStream3(tableAddress, tableWidth);
        }

        public void PrintZString(int stringAddress)
        {
            output.PrintZString(stringAddress);
        }

        public void Print(String str)
        {
            output.Print(str);
        }

        public void NewLine()
        {
            output.NewLine();
        }

        public void PrintZsciiChar(char zchar)
        {
            output.PrintZsciiChar(zchar);
        }

        public void PrintNumber(short num)
        {
            output.PrintNumber(num);
        }

        public void FlushOutput()
        {
            output.FlushOutput();
        }

        public void Reset()
        {
            output.Reset();
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

        public IInputStream SelectedInputStream => input.SelectedInputStream;

        public void SelectInputStream(int streamNumber)
        {
            input.SelectInputStream(streamNumber);
        }

        public char Random(short range)
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
            return (char)((_random.Next() % range) + 1);
        }

        #endregion

        #region Control functions

        public MachineRunState RunState
        {
            get
            {
                return runstate;
            }

            set
            {
                this.runstate = value;
                if (runstate != null && runstate.IsWaitingForInput)
                {
                    UpdateStatusLine();
                    FlushOutput();
                }
            }
        }

        public void Halt(String errormsg)
        {
            Print(errormsg);
            runstate = MachineRunState.STOPPED;
        }

        public void Warn(String msg)
        {
            LOG.LogWarning("WARNING: " + msg);
        }

        public void Restart() { restart(true); }

        public void Quit()
        {
            runstate = MachineRunState.STOPPED;
            // On quit, close the streams
            output.Print("*Game ended*");
            closeStreams();
        }

        public void Start() { runstate = MachineRunState.RUNNING; }

        #endregion

        #region Machine services

        public void Tokenize(int textbuffer, int parsebuffer, int dictionaryAddress, bool flag)
        {
            inputFunctions.tokenize(textbuffer, parsebuffer, dictionaryAddress, flag);
        }

        public char ReadLine(int textbuffer)
        {
            return inputFunctions.ReadLine(textbuffer);
        }

        public char ReadChar() { return inputFunctions.readChar(); }

        public ISoundSystem SoundSystem => soundSystem;
        public IPictureManager PictureManager => pictureManager;
        public void SetSaveGameDataStore(ISaveGameDataStore aDatastore)
        {
            this.datastore = aDatastore;
        }

        public void UpdateStatusLine()
        {
            if (FileHeader.Version <= 3 && statusLine != null)
            {
                int objNum = cpu.GetVariable((char)0x10);
                String objectName = getZCharDecoder().Decode2Zscii(getMemory(),
                  getObjectTree().GetPropertiesDescriptionAddress(objNum), 0);
                int global2 = cpu.GetVariable((char)0x11);
                int global3 = cpu.GetVariable((char)0x12);
                if (FileHeader.IsEnabled(StoryFileHeaderAttribute.ScoreGame))
                {
                    statusLine.updateStatusScore(objectName, global2, global3);
                }
                else
                {
                    statusLine.updateStatusTime(objectName, global2, global3);
                }
            }
        }

        public void SetStatusLine(IStatusLine statusLine)
        {
            this.statusLine = statusLine;
        }

        public void SetScreen(IScreenModel screen)
        {
            this.screenModel = screen;
        }

        public IScreenModel Screen => screenModel;
        public IScreenModel6 Screen6 => (IScreenModel6)screenModel;
        public bool Save(int savepc)
        {
            if (datastore != null)
            {
                PortableGameState gamestate = new PortableGameState();
                gamestate.captureMachineState(this, savepc);
                WritableFormChunk formChunk = gamestate.exportToFormChunk();
                return datastore.WriteFormChunk(formChunk);
            }
            return false;
        }

        public bool SaveUndo(int savepc)
        {
            PortableGameState undoGameState = new PortableGameState();
            undoGameState.captureMachineState(this, savepc);
            undostates.add(undoGameState);
            return true;
        }

        public PortableGameState Restore()
        {
            if (datastore != null)
            {
                PortableGameState gamestate = new PortableGameState();
                IFormChunk formchunk = datastore.ReadFormChunk();
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

        public PortableGameState RestoreUndo()
        {
            // do not reset screen model, since e.g. AMFV simply picks up the
            // current window state
            if (undostates.size() > 0)
            {
                PortableGameState undoGameState =
                  undostates.remove(undostates.size() - 1);
                restart(false);
                undoGameState.transferStateToMachine(this);
                LOG.LogInformation(String.Format("restore(), pc is: %4x\n", cpu.PC));
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
            return gamestate.getRelease() == Release
              && gamestate.getChecksum() == checksum
              && gamestate.getSerialNumber().Equals(FileHeader.SerialNumber);
        }

        /**
         * Returns the checksum.
         * @return checksum
         */
        private int getChecksum()
        {
            return memory.ReadUnsigned16(StoryFileHeaderAddress.Checksum);
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
            output.Reset();
            //soundSystem.reset(); // TODO: Implement sound system!
            cpu.Reset();
            setStandardRevision(1, 0);
            if (FileHeader.Version >= 4)
            {
                FileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsTimedInput, true);
                // IBM PC
                getMemory().WriteUnsigned8(StoryFileHeaderAddress.InterpreterNumber, (char)6);
                FileHeader.SetInterpreterVersion(1);
            }
        }

        /**
         * Sets standard revision.
         * @param major major revision number
         * @param minor minor revision number
         */
        private void setStandardRevision(int major, int minor)
        {
            memory.WriteUnsigned8(StoryFileHeaderAddress.StdRevisionMajor, (char)major);
            memory.WriteUnsigned8(StoryFileHeaderAddress.StdRevisionMinor, (char)minor);
        }

        /**
         * Restarts the VM.
         * @param resetScreenModel true if screen model should be reset
         */
        private void restart(bool resetScreenModel)
        {
            // Transcripting and fixed font bits survive the restart
            IStoryFileHeader fileHeader = FileHeader;
            bool fixedFontForced =
              fileHeader.IsEnabled(StoryFileHeaderAttribute.ForceFixedFont);
            bool transcripting = fileHeader.IsEnabled(StoryFileHeaderAttribute.Transcripting);

            resetState();

            if (resetScreenModel)
            {
                screenModel.Reset();
            }
            fileHeader.SetEnabled(StoryFileHeaderAttribute.Transcripting, transcripting);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.ForceFixedFont, fixedFontForced);
        }

        #endregion

        #region Object accesss

        /**
         * Returns the object tree.
         * @return object tree
         */
        private IObjectTree getObjectTree() { return objectTree; }

        public void InsertObject(int parentNum, int objectNum)
        {
            getObjectTree().InsertObject(parentNum, objectNum);
        }

        public void RemoveObject(int objectNum)
        {
            getObjectTree().RemoveObject(objectNum);
        }

        public void ClearAttribute(int objectNum, int attributeNum)
        {
            getObjectTree().ClearAttribute(objectNum, attributeNum);
        }

        public bool IsAttributeSet(int objectNum, int attributeNum)
        {
            return getObjectTree().IsAttributeSet(objectNum, attributeNum);
        }

        public void SetAttribute(int objectNum, int attributeNum)
        {
            getObjectTree().SetAttribute(objectNum, attributeNum);
        }

        public int GetParent(int objectNum)
        {
            return getObjectTree().GetParent(objectNum);
        }

        public void SetParent(int objectNum, int parent)
        {
            getObjectTree().SetParent(objectNum, parent);
        }

        public int GetChild(int objectNum)
        {
            return getObjectTree().GetChild(objectNum);
        }

        public void SetChild(int objectNum, int child)
        {
            getObjectTree().SetChild(objectNum, child);
        }

        public int GetSibling(int objectNum)
        {
            return getObjectTree().GetSibling(objectNum);
        }

        public void SetSibling(int objectNum, int sibling)
        {
            getObjectTree().SetSibling(objectNum, sibling);
        }

        public int GetPropertiesDescriptionAddress(int objectNum)
        {
            return getObjectTree().GetPropertiesDescriptionAddress(objectNum);
        }

        public int GetPropertyAddress(int objectNum, int property)
        {
            return getObjectTree().GetPropertyAddress(objectNum, property);
        }

        public int GetPropertyLength(int propertyAddress)
        {
            return getObjectTree().GetPropertyLength(propertyAddress);
        }

        public char GetProperty(int objectNum, int property)
        {
            return getObjectTree().GetProperty(objectNum, property);
        }

        public void SetProperty(int objectNum, int property, char value)
        {
            getObjectTree().SetProperty(objectNum, property, value);
        }

        public int GetNextProperty(int objectNum, int property)
        {
            return getObjectTree().GetNextProperty(objectNum, property);
        }

        public Resolution getResolution()
        {
            return Screen6.getResolution();
        }

        #endregion
    }
}
