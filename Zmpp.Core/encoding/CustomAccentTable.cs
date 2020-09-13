/*
 * Created on 2005/01/15
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
    /// Represents a custom accent table.
    /// </summary>
    /// <remarks>
    /// If an extension header specifies a custom accent table
    /// instances of this class are used to retrieve the custom accents.
    /// </remarks>
    public class CustomAccentTable : IAccentTable
    {
        /// <summary>
        /// The Memory object.
        /// </summary>
        private readonly IMemory memory;

        /// <summary>
        /// The table adddress.
        /// </summary>
        private readonly int tableAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Encoding.CustomAccentTable"/>
        /// class for the specified memory and address.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The table address.</param>
        public CustomAccentTable(IMemory memory, int address)
        {
            this.memory = memory;
            this.tableAddress = address;
        }

        public int Length
        {
            get
            {
                int result = 0;
                if (tableAddress > 0)
                {
                    result = memory.ReadUnsigned8(tableAddress);
                }
                return result;
            }
        }

        public char GetAccent(int index)
        {
            char result = '?';
            if (tableAddress > 0)
            {
                result = memory.ReadUnsigned16(tableAddress + (index * 2) + 1);
            }
            return result;
        }

        public int GetIndexOfLowerCase(int index)
        {
            char c = (char)GetAccent(index);
            char lower = char.ToLower(c);
            int length = Length;

            for (int i = 0; i < length; i++)
            {
                if (GetAccent(i) == lower) return i;
            }
            return index;
        }
    }
}
