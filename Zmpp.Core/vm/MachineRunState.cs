/*
 * Created on 05/27/2008
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
    /// <summary>
    /// This class models a machine run state that also stores data for timed
    /// input, so a client application can call an interrupt method on the machine.
    /// </summary>
    public sealed class MachineRunState
    {
        private const long serialVersionUID = 1L;

        /// <summary>
        /// Reading modes.
        /// </summary>
        private enum ReadMode { NONE, READ_CHAR, READ_LINE };

        private readonly int time;
        private readonly int numLeftOverChars;
        private readonly char routine, textbuffer;
        private readonly ReadMode readMode = ReadMode.NONE;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private MachineRunState() { }

        /// <summary>
        /// Constructor for reading modes.
        /// </summary>
        /// <param name="readMode">the read mode</param>
        /// <param name="time">the interrupt routine time interval</param>
        /// <param name="routine">the packed interrupt routine address</param>
        /// <param name="numLeftOverChars">the number of characters indicated as left over</param>
        /// <param name="textbuffer">text buffer address</param>
        private MachineRunState(ReadMode readMode, int time, char routine, int numLeftOverChars, char textbuffer)
        {
            this.readMode = readMode;
            this.time = time;
            this.routine = routine;
            this.numLeftOverChars = numLeftOverChars;
            this.textbuffer = textbuffer;
        }

        /// <summary>
        /// Gets the interrupt interval.
        /// </summary>
        public int Time => time;

        /// <summary>
        /// Gets the packed address of the interrupt address.
        /// </summary>
        public char Routine => routine;

        /// <summary>
        /// Returns true if machine is waiting for input.
        /// </summary>
        /// <returns>true if waiting for input, false otherwise</returns>
        public bool IsWaitingForInput => readMode != ReadMode.NONE;

        /// <summary>
        /// Returns true if machine is in read character mode.
        /// </summary>
        /// <returns>true if read character mode, false otherwise</returns>
        public bool IsReadChar => readMode == ReadMode.READ_CHAR;

        /// <summary>
        /// Returns true if machine is in read line mode.
        /// </summary>
        /// <returns>true if read line mode, false otherwise</returns>
        public bool IsReadLine => readMode == ReadMode.READ_LINE;

        /// <summary>
        /// Returns the number of characters left over from previous input.
        /// </summary>
        /// <returns>the number of left over characters</returns>
        public int NumLeftOverChars => numLeftOverChars;

        /// <summary>
        /// Returns the address of the text buffer.
        /// </summary>
        /// <returns>the text buffer</returns>
        public char TextBuffer => textbuffer;

        /// <summary>
        /// Running state.
        /// </summary>
        public static MachineRunState RUNNING = new MachineRunState();

        /// <summary>
        /// Stopped state.
        /// </summary>
        public static MachineRunState STOPPED = new MachineRunState();

        /// <summary>
        /// Creates a read line mode object with the specified interrupt data.
        /// </summary>
        /// <param name="time">interrupt interval</param>
        /// <param name="routine">interrupt routine</param>
        /// <param name="numLeftOverChars">the number of characters left over</param>
        /// <param name="textbuffer">the address of the text buffer</param>
        /// <returns>machine run state object</returns>
        public static MachineRunState CreateReadLine(int time, char routine, int numLeftOverChars, char textbuffer)
        {
            return new MachineRunState(ReadMode.READ_LINE, time, routine, numLeftOverChars, textbuffer);
        }

        /// <summary>
        /// Creates a read character mode object with the specified interrupt data.
        /// </summary>
        /// <param name="time">interrupt interval</param>
        /// <param name="routine">interrupt routine</param>
        /// <returns>machine state</returns>
        public static MachineRunState CreateReadChar(int time, char routine)
        {
            return new MachineRunState(ReadMode.READ_CHAR, time, routine, 0, (char)0);
        }
    }
}