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
    using System.Text;
    using Zmpp.Core;
    using Zmpp.Core.Encoding;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// Abstract super class of dictionaries.
    /// </summary>
    public abstract class DictionaryBase : IDictionary
    {
        private readonly IMemory memory;

        /// <summary>
        /// The dictionary start address.
        /// </summary>
        private readonly int address;

        private readonly IZCharDecoder decoder;
        private readonly ZCharEncoder encoder;

        /// <summary>
        /// A sizes object.
        /// </summary>
        private readonly IDictionarySizes sizes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Vm.DictionaryBase"/>
        /// class for the specified memory, address, decoder, encoder, and dictionary sizes.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The start address of the dictionary.</param>
        /// <param name="decoder">The IZCharDecoder object.</param>
        /// <param name="encoder">The IZCharEncoder object.</param>
        /// <param name="sizes">The IDictionarySizes object.</param>
        public DictionaryBase(IMemory memory, int address, IZCharDecoder decoder, ZCharEncoder encoder, IDictionarySizes sizes)
        {
            this.memory = memory;
            this.address = address;
            this.decoder = decoder;
            this.encoder = encoder;
            this.sizes = sizes;
        }

        public int NumberOfSeparators => memory.ReadUnsigned8(address);

        public byte GetSeparator(int i)
        {
            return (byte)memory.ReadUnsigned8(address + i + 1);
        }

        public int EntryLength => memory.ReadUnsigned8(address + NumberOfSeparators + 1);

        public short NumberOfEntries
        {
            get
            {
                // The number of entries is a signed value so that we can recognize a negative number
                return UnsignedToSigned16(memory.ReadUnsigned16(address + NumberOfSeparators + 2));
            }
        }

        public int GetEntryAddress(int entryNum)
        {
            int headerSize = NumberOfSeparators + 4;
            return address + headerSize + entryNum * EntryLength;
        }

        /// <summary>
        /// Gets the decoder.
        /// </summary>
        protected IZCharDecoder Decoder => decoder;

        /// <summary>
        /// Gets the memory map.
        /// </summary>
        protected IMemory Memory => memory;

        /// <summary>
        /// Gets the dictionary sizes for the current story file version.
        /// </summary>
        protected IDictionarySizes Sizes => sizes;

        /// <summary>
        /// Truncate the specified token.
        /// </summary>
        /// <param name="token">The token to truncate.</param>
        /// <returns>The truncated token.</returns>
        /// <remarks>
        /// Unfortunately it seems that the maximum size of an entry is not equal
        /// to the size declared in the dictionary header, therefore we take
        /// the maximum length of a token defined in the Z-machine specification.
        /// The lookup token can only be 6 characters long in version 3
        /// and 9 in versions >= 4.
        /// </remarks>
        protected string TruncateToken(string token)
        {
            return token.Length > sizes.MaxEntryChars ?
                token.Substring(0, sizes.MaxEntryChars) : token;
        }

        /// <summary>
        /// Truncates the specified token and returns a dictionary encoded byte array.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The truncated token as a byte array.</returns>
        protected byte[] TruncateTokenToBytes(string token)
        {
            byte[] result = new byte[sizes.NumEntryBytes];
            IMemory buffer = new Memory(result);
            encoder.Encode(token, buffer, 0);
            return result;
        }

        /// <summary>
        /// Lexicographical comparison of the input word and the dictionary entry
        /// at the specified address.
        /// </summary>
        /// <param name="tokenBytes">The input word bytes.</param>
        /// <param name="entryAddress">The dictionary entry address.</param>
        /// <returns>comparison value, 0 if match, &lt; 0 if lexicographical smaller, &lt; 0 if lexicographical greater</returns>
        protected int TokenMatch(byte[] tokenBytes, int entryAddress)
        {
            for (int i = 0; i < tokenBytes.Length; i++)
            {
                int tokenByte = tokenBytes[i] & 0xff;
                int c = (Memory.ReadUnsigned8(entryAddress + i) & 0xff);
                if (tokenByte != c)
                {
                    return tokenByte - c;
                }
            }
            return 0;
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            int entryAddress;
            int i = 0;
            int n = NumberOfEntries;
            while (true)
            {
                entryAddress = GetEntryAddress(i);
                string str = Decoder.Decode2Zscii(Memory, entryAddress, sizes.NumEntryBytes);
                buffer.Append(string.Format("[{0,4:D}] '{1,-9}' ", (i + 1), str));
                i++;
                if ((i % 4) == 0) { buffer.Append("\n"); }
                if (i == n) { break; }
            }
            return buffer.ToString();
        }

        public abstract int Lookup(string token);
    }
}
