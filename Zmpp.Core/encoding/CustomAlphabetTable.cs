/*
 * Created on 2006/01/16
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
    using Zmpp.Core;

    /// <summary>
    /// Represents a custom alphabet table.
    /// </summary>
    /// <remarks>
    /// If the story file header defines a custom alphabet table
    /// instances of this class are used to retrieve the alphabet characters.
    /// </remarks>
    public class CustomAlphabetTable : AlphabetTableBase, IAlphabetTable
    {
        private const long serialVersionUID = 1L;
        private const int AlphabetSize = 26;
        private readonly IMemory memory;
        private readonly int tableAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Encoding.CustomAlphabetTable"/>
        /// class for the specified memory and address.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The table address.</param>
        public CustomAlphabetTable(IMemory memory, int address)
        {
            this.memory = memory;
            tableAddress = address;
        }

        public override char GetA0Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            return (char)memory.ReadUnsigned8(tableAddress + (zchar - AlphabetStart));
        }

        public override char GetA1Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            return (char)memory.ReadUnsigned8(tableAddress + AlphabetSize + (zchar - AlphabetStart));
        }

        public override char GetA2Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            if (zchar == 7) return (char)((short)'\n');
            return (char)memory.ReadUnsigned8(tableAddress + 2 * AlphabetSize + (zchar - AlphabetStart));
        }

        public override int GetA0CharCode(char zsciiChar)
        {
            for (int i = AlphabetStart; i < AlphabetStart + AlphabetSize; i++)
            {
                if (GetA0Char((byte)i) == zsciiChar) return i;
            }
            return -1;
        }

        public override int GetA1CharCode(char zsciiChar)
        {
            for (int i = AlphabetStart; i < AlphabetStart + AlphabetSize; i++)
            {
                if (GetA1Char((byte)i) == zsciiChar) return i;
            }
            return -1;
        }

        public override int GetA2CharCode(char zsciiChar)
        {
            for (int i = AlphabetStart; i < AlphabetStart + AlphabetSize; i++)
            {
                if (GetA2Char((byte)i) == zsciiChar) return i;
            }
            return -1;
        }


        public override bool IsAbbreviation(char zchar)
        {
            return 1 <= zchar && zchar <= 3;
        }

        public override bool IsShift1(char zchar)
        {
            return zchar == AlphabetTableBase.Shift4;
        }

        public override bool IsShift2(char zchar)
        {
            return zchar == AlphabetTableBase.Shift5;
        }

        public override bool IsShiftLock(char zchar) { return false; }

        public override bool IsShift(char zchar)
        {
            return IsShift1(zchar) || IsShift2(zchar);
        }
    }
}
