/*
 * Created on 2006/09/25
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
    using System.Text;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// Abstract super class of dictionaries.
    /// </summary>
    public abstract class AbstractDictionary : IDictionary
    {
        private IMemory memory;

        /// <summary>
        /// The dictionary start address.
        /// </summary>
        private int address;

        private IZCharDecoder decoder;
        private ZCharEncoder encoder;

        /// <summary>
        /// A sizes object.
        /// </summary>
        private IDictionarySizes sizes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memory">the memory object</param>
        /// <param name="address">the start address of the dictionary</param>
        /// <param name="decoder">a ZCharDecoder object</param>
        /// <param name="encoder">a ZCharEncoder object</param>
        /// <param name="sizes">an object specifying the sizes of the dictionary entries</param>
        public AbstractDictionary(IMemory memory, int address, IZCharDecoder decoder, ZCharEncoder encoder, IDictionarySizes sizes)
        {
            this.memory = memory;
            this.address = address;
            this.decoder = decoder;
            this.encoder = encoder;
            this.sizes = sizes;
        }

        public int getNumberOfSeparators()
        {
            return memory.ReadUnsigned8(address);
        }

        public byte getSeparator(int i)
        {
            return (byte)memory.ReadUnsigned8(address + i + 1);
        }

        public int getEntryLength()
        {
            return memory.ReadUnsigned8(address + getNumberOfSeparators() + 1);
        }

        public short getNumberOfEntries()
        {
            // The number of entries is a signed value so that we can recognize
            // a negative number
            return UnsignedToSigned16(memory.ReadUnsigned16(address + getNumberOfSeparators() + 2));
        }

        public int getEntryAddress(int entryNum)
        {
            int headerSize = getNumberOfSeparators() + 4;
            return address + headerSize + entryNum * getEntryLength();
        }

        /// <summary>
        /// Access to the decoder object.
        /// </summary>
        /// <returns>the decoder object</returns>
        protected IZCharDecoder getDecoder() { return decoder; }

        /// <summary>
        /// Access to the Memory object.
        /// </summary>
        /// <returns>the Memory object</returns>
        protected IMemory getMemory() { return memory; }

        /// <summary>
        /// Returns the DictionarySizes object for the current story file version.
        /// </summary>
        /// <returns>the DictionarySizes object</returns>
        protected IDictionarySizes getSizes() { return sizes; }

        /// <summary>
        /// Unfortunately it seems that the maximum size of an entry is not equal
        /// to the size declared in the dictionary header, therefore we take
        /// the maximum length of a token defined in the Z-machine specification.
        /// The lookup token can only be 6 characters long in version 3
        /// and 9 in versions >= 4
        /// </summary>
        /// <param name="token">the token to truncate</param>
        /// <returns>the truncated token</returns>
        protected String truncateToken(String token)
        {
            return token.Length > sizes.getMaxEntryChars() ?
                token.Substring(0, sizes.getMaxEntryChars()) : token;
        }

        /// <summary>
        /// Truncates the specified token and returns a dictionary encoded byte array.
        /// </summary>
        /// <param name="token">the token</param>
        /// <returns>the truncated token as a byte array</returns>
        protected byte[] truncateTokenToBytes(String token)
        {
            byte[] result = new byte[sizes.getNumEntryBytes()];
            IMemory buffer = new Memory(result);
            encoder.encode(token, buffer, 0);
            return result;
        }

        /// <summary>
        /// Lexicographical comparison of the input word and the dictionary entry
        /// at the specified address.
        /// </summary>
        /// <param name="tokenBytes">input word bytes</param>
        /// <param name="entryAddress">dictionary entry address</param>
        /// <returns>comparison value, 0 if match, &lt; 0 if lexicographical smaller, &lt; 0 if lexicographical greater</returns>
        protected int tokenMatch(byte[] tokenBytes, int entryAddress)
        {
            for (int i = 0; i < tokenBytes.Length; i++)
            {
                int tokenByte = tokenBytes[i] & 0xff;
                int c = (getMemory().ReadUnsigned8(entryAddress + i) & 0xff);
                if (tokenByte != c)
                {
                    return tokenByte - c;
                }
            }
            return 0;
        }

        /// <summary>
        /// Creates a string presentation of this dictionary.
        /// </summary>
        /// <returns>the string presentation</returns>
        public String toString()
        {
            StringBuilder buffer = new StringBuilder();
            int entryAddress;
            int i = 0;
            int n = getNumberOfEntries();
            while (true)
            {
                entryAddress = getEntryAddress(i);
                String str = getDecoder().decode2Zscii(getMemory(),
                    entryAddress, sizes.getNumEntryBytes());
                buffer.Append(String.Format("[%4d] '%-9s' ", (i + 1), str));
                i++;
                if ((i % 4) == 0) { buffer.Append("\n"); }
                if (i == n) { break; }
            }
            return buffer.ToString();
        }

        public abstract int lookup(String token);
    }
}
