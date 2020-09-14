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
    using System;
    using Zmpp.Core;
    using Zmpp.Core.Encoding;
    using Zmpp.Core.Media;
    using Zmpp.Core.UI;

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
        void Initialize(byte[] data, IResources resources);

        /// <summary>
        /// Gets the story file version.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets the release.
        /// </summary>
        int Release { get; }

        /// <summary>
        /// Gets a value indicating whether the checksum is valid.
        /// </summary>
        /// <returns>true if the checksum is valid; otherwise false.</returns>
        bool HasValidChecksum { get; }

        #region Main machine objects

        /// <summary>
        /// Gets the story file header.
        /// </summary>
        IStoryFileHeader FileHeader { get; }

        /// <summary>
        /// Gets the story resources.
        /// </summary>
        IResources Resources { get; }

        #endregion

        #region Tokenizing functions

        // **********************************************************************
        // **** We could refine this by exposing the tokenizers
        // **** instead of dictionary functionality
        // **********************************************************

        /// <summary>
        /// Gets the index for the specified token in the specified dictionary.
        /// </summary>
        /// <param name="dictionaryAddress">The address of the dictionary.</param>
        /// <param name="token">The token to look up.</param>
        /// <returns>The index in the dictionary.</returns>
        int LookupToken(int dictionaryAddress, String token);

        /// <summary>
        /// Gets the dictionary delimiters.
        /// </summary>
        char[] DictionaryDelimiters { get; }

        #endregion

        #region Encoding functions

        /// <summary>
        /// Encode memory location to ZSCII.
        /// </summary>
        /// <param name="source">The source position.</param>
        /// <param name="length">The memory length in bytes.</param>
        /// <param name="destination">The destination position.</param>
        void Encode(int source, int length, int destination);

        /// <summary>
        /// Decode memory address to ZSCII.
        /// </summary>
        /// <param name="address">The memory address.</param>
        /// <param name="length">The length in bytes.</param>
        /// <returns>ZSCII string</returns>
        string Decode2Zscii(int address, int length);

        /// <summary>
        /// Gets the number of Z-encoded bytes at the specified address.
        /// </summary>
        /// <param name="address">The string address.</param>
        /// <returns>The number of Z-encoded bytes.</returns>
        int GetNumZEncodedBytes(int address);

        #endregion

        #region Control functions

        /// <summary>
        /// Gets or sets the current run state of the machine.
        /// </summary>
        MachineRunState RunState { get; set;  }

        /// <summary>
        /// Halts the machine with the specified error message.
        /// </summary>
        /// <param name="errormsg">The error message.</param>
        void Halt(string errormsg);

        /// <summary>
        /// Restarts the virtual machine.
        /// </summary>
        void Restart();

        /// <summary>
        /// Starts the virtual machine.
        /// </summary>
        void Start();

        /// <summary>
        /// Exits the virtual machine.
        /// </summary>
        void Quit();

        /// <summary>
        /// Outputs a warning message.
        /// </summary>
        /// <param name="msg">The message.</param>
        void Warn(string msg);

        #endregion

        #region Services

        /// <summary>
        /// Tokenizes the text in the text buffer using the specified parse buffer.
        /// </summary>
        /// <param name="textbuffer">The text buffer.</param>
        /// <param name="parsebuffer">The parse buffer.</param>
        /// <param name="dictionaryAddress">The dictionary address or 0 for the default dictionary.</param>
        /// <param name="flag">If set, unrecognized words are not written into the parse buffer and their slots are left unchanged.</param>
        void Tokenize(int textbuffer, int parsebuffer, int dictionaryAddress, bool flag);

        /// <summary>
        /// Reads a string from the currently selected input stream into the text buffer address.
        /// </summary>
        /// <param name="textbuffer">The text buffer address.</param>
        /// <returns>The terminator character.</returns>
        char ReadLine(int textbuffer);

        /// <summary>
        /// Reads a ZSCII char from the selected input stream.
        /// </summary>
        /// <returns>The selected ZSCII char.</returns>
        char ReadChar();

        /// <summary>
        /// Gets the sound system.
        /// </summary>
        ISoundSystem SoundSystem { get; }

        /// <summary>
        /// Gets the picture manager.
        /// </summary>
        IPictureManager PictureManager { get; }

        /// <summary>
        /// Generates a number in the range between 1 and <i>range</i>.
        /// </summary>
        /// <param name="range">the range</param>
        /// <returns>a random number</returns>
        /// <remarks>
        /// If range is negative, the random generator will be seeded
        /// to Abs(range), if range is 0, the random generator will
        /// be initialized to a new random seed. In both latter cases,
        /// the result will be 0.
        /// </remarks>
        char Random(short range);

        /// <summary>
        /// Updates the status line.
        /// </summary>
        void UpdateStatusLine();

        /// <summary>
        /// Sets the status line.
        /// </summary>
        /// <param name="statusline">The status line.</param>
        void SetStatusLine(IStatusLine statusline);

        /// <summary>
        /// Sets the screen model.
        /// </summary>
        /// <param name="screen">The screen model.</param>
        void SetScreen(IScreenModel screen);

        /// <summary>
        /// Gets the screen model.
        /// </summary>
        IScreenModel Screen { get; }

        /// <summary>
        /// Gets the screen model 6.
        /// </summary>
        IScreenModel6 Screen6 { get; }

        /// <summary>
        /// Sets the save game data store.
        /// </summary>
        /// <param name="datastore">The save game data store.</param>
        void SetSaveGameDataStore(ISaveGameDataStore datastore);

        /// <summary>
        /// Saves the current state.
        /// </summary>
        /// <param name="savepc">The save pc.</param>
        /// <returns>true if the save is successful; otherwise false.</returns>
        bool Save(int savepc);

        /// <summary>
        /// Saves the current state in memory.
        /// </summary>
        /// <param name="savepc">The save pc.</param>
        /// <returns>true if the save is successful; otherwise false.</returns>
        bool SaveUndo(int savepc);

        /// <summary>
        /// Restores a previously saved state.
        /// </summary>
        /// <returns>The portable game state.</returns>
        PortableGameState Restore();

        /// <summary>
        /// Restores a previously saved state from memory.
        /// </summary>
        /// <returns>The portable game state.</returns>
        PortableGameState RestoreUndo();

        #endregion
    }
}
