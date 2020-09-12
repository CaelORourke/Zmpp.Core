/*
 * Created on 10/03/2005
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
    using Zmpp.Core;
    using Zmpp.Core.Encoding;
    using Zmpp.Core.Media;
    using Zmpp.Core.UI;
    using System;

    /// <summary>
    /// This interface acts as a central access point to the Z-Machine's components.
    /// It is mainly provided as a service point for the instructions to manipulate
    /// and read the VM's internal state.
    /// </summary>
    public interface IMachine : IObjectTree, IInput, IOutput, ICpu, IMemory, IZsciiEncoding
    {
        /// <summary>
        /// Initialization function.
        /// </summary>
        /// <param name="data">the story data</param>
        /// <param name="resources">Blorb resources</param>
        void initialize(byte[] data, IResources resources);

        /// <summary>
        /// Returns the story file version.
        /// </summary>
        /// <returns>the story file version</returns>
        int getVersion();

        /// <summary>
        /// Returns the release.
        /// </summary>
        /// <returns>the release</returns>
        int getRelease();

        /// <summary>
        /// Checks the check sum.
        /// </summary>
        /// <returns>true if valid, false, otherwise</returns>
        bool hasValidChecksum();

        #region Main machine objects

        /// <summary>
        /// Returns the story file header.
        /// </summary>
        /// <returns>file header</returns>
        IStoryFileHeader getFileHeader();

        /// <summary>
        /// Returns story resources.
        /// </summary>
        /// <returns>story resources</returns>
        IResources getResources();

        #endregion

        #region Tokenizing functions

        // **********************************************************************
        // **** We could refine this by exposing the tokenizers
        // **** instead of dictionary functionality
        // **********************************************************

        /// <summary>
        /// Looks up token in dictionary.
        /// </summary>
        /// <param name="dictionaryAddress">address of dictionary</param>
        /// <param name="token">token to look up</param>
        /// <returns>index in dictionary</returns>
        int lookupToken(int dictionaryAddress, String token);

        /// <summary>
        /// Returns the dictionary delimiters.
        /// </summary>
        /// <returns>dictionary delimiters</returns>
        char[] getDictionaryDelimiters();

        #endregion

        #region Encoding functions

        /// <summary>
        /// Encode memory location to ZSCII.
        /// </summary>
        /// <param name="source">source position</param>
        /// <param name="length">memory length in byte</param>
        /// <param name="destination">destination position</param>
        void encode(int source, int length, int destination);

        /// <summary>
        /// Decode memory address to ZSCII.
        /// </summary>
        /// <param name="address">memory address</param>
        /// <param name="length">length in bytes</param>
        /// <returns>ZSCII string</returns>
        String decode2Zscii(int address, int length);

        /// <summary>
        /// Returns the number of Z-encoded bytes at the specified address.
        /// </summary>
        /// <param name="address">the string address</param>
        /// <returns>number of z-encoded bytes</returns>
        int getNumZEncodedBytes(int address);

        #endregion

        #region Control functions

        /// <summary>
        /// Returns the current run state of the machine
        /// </summary>
        /// <returns>the run state</returns>
        MachineRunState getRunState();

        /// <summary>
        /// Sets the current run state of the machine
        /// </summary>
        /// <param name="runstate">the run state</param>
        void setRunState(MachineRunState runstate);

        /// <summary>
        /// Halts the machine with the specified error message.
        /// </summary>
        /// <param name="errormsg">the error message</param>
        void halt(String errormsg);

        /// <summary>
        /// Restarts the virtual machine.
        /// </summary>
        void restart();

        /// <summary>
        /// Starts the virtual machine.
        /// </summary>
        void start();

        /// <summary>
        /// Exits the virtual machine.
        /// </summary>
        void quit();

        /// <summary>
        /// Outputs a warning message.
        /// </summary>
        /// <param name="msg">the message</param>
        void warn(String msg);

        #endregion

        #region Services

        /// <summary>
        /// Tokenizes the text in the text buffer using the specified parse buffer.
        /// </summary>
        /// <param name="textbuffer">the text buffer</param>
        /// <param name="parsebuffer">the parse buffer</param>
        /// <param name="dictionaryAddress">the dictionary address or 0 for the default dictionary</param>
        /// <param name="flag">if set, unrecognized words are not written into the parse buffer and their slots are left unchanged</param>
        void tokenize(int textbuffer, int parsebuffer, int dictionaryAddress, bool flag);

        /// <summary>
        /// Reads a string from the currently selected input stream into the text buffer address.
        /// </summary>
        /// <param name="textbuffer">the text buffer address</param>
        /// <returns>the terminator character</returns>
        char readLine(int textbuffer);

        /// <summary>
        /// Reads a ZSCII char from the selected input stream.
        /// </summary>
        /// <returns>the selected ZSCII char</returns>
        char readChar();

        /// <summary>
        /// Returns the sound system.
        /// </summary>
        /// <returns>the sound system</returns>
        ISoundSystem getSoundSystem();

        /// <summary>
        /// Returns the picture manager.
        /// </summary>
        /// <returns>the picture manager</returns>
        IPictureManager getPictureManager();

        /// <summary>
        /// Generates a number in the range between 1 and <i>range</i>. If range is
        /// negative, the random generator will be seeded to abs(range), if
        /// range is 0, the random generator will be initialized to a new
        /// random seed.In both latter cases, the result will be 0.
        /// </summary>
        /// <param name="range">the range</param>
        /// <returns>a random number</returns>
        char random(short range);

        /// <summary>
        /// Updates the status line.
        /// </summary>
        void updateStatusLine();

        /// <summary>
        /// Sets the Z-machine's status line.
        /// </summary>
        /// <param name="statusline">the status line</param>
        void setStatusLine(IStatusLine statusline);

        /// <summary>
        /// Sets the game screen.
        /// </summary>
        /// <param name="screen">the screen model</param>
        void setScreen(IScreenModel screen);

        /// <summary>
        /// Gets the game screen.
        /// </summary>
        /// <returns>the game screen</returns>
        IScreenModel getScreen();

        /// <summary>
        /// Returns screen model 6.
        /// </summary>
        /// <returns>screen model 6</returns>
        IScreenModel6 getScreen6();

        /// <summary>
        /// Sets the save game data store.
        /// </summary>
        /// <param name="datastore">the data store</param>
        void setSaveGameDataStore(ISaveGameDataStore datastore);

        /// <summary>
        /// Saves the current state.
        /// </summary>
        /// <param name="savepc">the save pc</param>
        /// <returns>true on success, false otherwise</returns>
        bool save(int savepc);

        /// <summary>
        /// Saves the current state in memory.
        /// </summary>
        /// <param name="savepc">the save pc</param>
        /// <returns>true on success, false otherwise</returns>
        bool save_undo(int savepc);

        /// <summary>
        /// Restores a previously saved state.
        /// </summary>
        /// <returns>the portable game state</returns>
        PortableGameState restore();

        /// <summary>
        /// Restores a previously saved state from memory.
        /// </summary>
        /// <returns>the portable game state</returns>
        PortableGameState restore_undo();

        #endregion
    }
}
