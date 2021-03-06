/*
 * Created on 10/14/2005
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
    using System;

    /// <summary>
    /// This class implements a view on the dictionary within a memory map.
    /// Since it takes the implementations of getN
    /// </summary>
    public class DefaultDictionary : DictionaryBase
    {
        /// <summary>
        /// The maximum entry size.
        /// </summary>
        private int maxEntrySize;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memory">the memory object</param>
        /// <param name="address">the start address of the dictionary</param>
        /// <param name="decoder">ZCharDecoder object</param>
        /// <param name="encoder">ZCharEncoder object</param>
        /// <param name="sizes">a sizes object</param>
        public DefaultDictionary(IMemory memory, int address, IZCharDecoder decoder, ZCharEncoder encoder, IDictionarySizes sizes) : base(memory, address, decoder, encoder, sizes)
        {
        }

        public override int Lookup(String token)
        {
            return lookupBinary(TruncateTokenToBytes(token), 0, NumberOfEntries - 1);
        }

        /// <summary>
        /// Recursive binary search to find an input word in the dictionary.
        /// </summary>
        /// <param name="tokenBytes">the byte array containing the input word</param>
        /// <param name="left">the left index</param>
        /// <param name="right">the right index</param>
        /// <returns>the entry address</returns>
        private int lookupBinary(byte[] tokenBytes, int left, int right)
        {
            if (left > right) return 0;
            int middle = left + (right - left) / 2;
            int entryAddress = GetEntryAddress(middle);
            int res = TokenMatch(tokenBytes, entryAddress);
            if (res < 0)
            {
                return lookupBinary(tokenBytes, left, middle - 1);
            }
            else if (res > 0)
            {
                return lookupBinary(tokenBytes, middle + 1, right);
            }
            else
            {
                return entryAddress;
            }
        }

        protected int getMaxEntrySize() { return maxEntrySize; }
    }
}
