/*
 * Created on 2006/01/09
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
    using org.zmpp.@base;
    using org.zmpp.encoding;
    using System;

    /// <summary>
    /// This class implements a user dictionary. The specification suggests that
    /// lookup is implemented using linear search in case the user dictionary
    /// is specified as unordered(negative number of entries) and in case of
    /// ordered a binary search will be performed.
    /// </summary>
    public class UserDictionary : AbstractDictionary
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memory">the Memory object</param>
        /// <param name="address">the start address of the dictionary</param>
        /// <param name="decoder">a ZCharDecoder object</param>
        /// <param name="encoder">a ZCharEncoder object</param>
        public UserDictionary(IMemory memory, int address, IZCharDecoder decoder, ZCharEncoder encoder) : base(memory, address, decoder, encoder, new DictionarySizesV4ToV8())
        {
        }

        public override int lookup(String token)
        {
            // We only implement linear search for user dictionaries
            int n = Math.Abs(getNumberOfEntries());
            byte[] tokenBytes = truncateTokenToBytes(token);
            for (int i = 0; i < n; i++)
            {
                int entryAddress = getEntryAddress(i);
                if (tokenMatch(tokenBytes, entryAddress) == 0) { return entryAddress; }
            }
            return 0;
        }
    }
}
