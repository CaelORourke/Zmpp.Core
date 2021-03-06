/*
 * Created on 2006/01/12
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

namespace Zmpp.Core.Encoding
{
    /// <summary>
    /// Provides a generic alphabet table.
    /// </summary>
    /// <remarks>
    /// The alphabet table is a central part of the Z encoding system. It stores
    /// the characters that are mapped to each alphabet and provides information
    /// about shift and escape situations.
    /// </remarks>
    public abstract class AlphabetTableBase : IAlphabetTable
    {
        public const int AlphabetStart = 6;
        public const int AlphabetEnd = 31;

        public const char Shift2 = (char)0x02; // Shift 1
        public const char Shift3 = (char)0x03; // Shift 2
        public const char Shift4 = (char)0x04; // Shift lock 1
        public const char Shift5 = (char)0x05; // Shift lock 2

        /// <summary>
        /// This character code from A2 denotes that a 10 bit value follows.
        /// </summary>
        public const char A2Escape = (char)0x06; // escape character

        public abstract char GetA0Char(byte zchar);
        public abstract int GetA0CharCode(char zsciiChar);
        public abstract char GetA1Char(byte zchar);
        public abstract int GetA1CharCode(char zsciiChar);
        public abstract char GetA2Char(byte zchar);
        public abstract int GetA2CharCode(char zsciiChar);
        public abstract bool IsAbbreviation(char zchar);
        public abstract bool IsShift(char zchar);
        public abstract bool IsShift1(char zchar);
        public abstract bool IsShift2(char zchar);
        public abstract bool IsShiftLock(char zchar);
    }
}
