/*
 * Created on 2005/09/23
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

namespace Zmpp.Core
{
    /// <summary>
    /// Represents a subsection within a memory map.
    /// </summary>
    /// <remarks>
    /// All addresses are relative to the start of
    /// this <see cref="Zmpp.Core.MemorySection"/>.
    /// </remarks>
    public class MemorySection : IMemory
    {
        private readonly IMemory memory;
        private readonly int start;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.MemorySection"/>
        /// class for the specified <see cref="Zmpp.Core.Memory"/>, start address,
        /// and length.
        /// </summary>
        /// <param name="memory">The parent <see cref="Zmpp.Core.Memory"/> object.</param>
        /// <param name="start">The start address of this <see cref="Zmpp.Core.MemorySection"/>.</param>
        /// <param name="length">the length in bytes of this <see cref="Zmpp.Core.MemorySection"/>.</param>
        public MemorySection(IMemory memory, int start, int length)
        {
            this.memory = memory;
            this.start = start;
            this.Length = length;
        }

        /// <summary>
        /// Gets the number of bytes in the current
        /// <see cref="Zmpp.Core.MemorySection"/> object.
        /// </summary>
        public int Length { get; }

        public void WriteUnsigned16(int address, char value)
        {
            memory.WriteUnsigned16(address + start, value);
        }

        public void WriteUnsigned8(int address, char value)
        {
            memory.WriteUnsigned8(address + start, value);
        }

        public char ReadUnsigned16(int address)
        {
            return memory.ReadUnsigned16(address + start);
        }

        public char ReadUnsigned8(int address)
        {
            return memory.ReadUnsigned8(address + start);
        }

        public void CopyBytesToArray(byte[] dstData, int dstOffset, int srcOffset, int numBytes)
        {
            memory.CopyBytesToArray(dstData, dstOffset, srcOffset + start, numBytes);
        }

        public void CopyBytesFromArray(byte[] srcData, int srcOffset, int dstOffset, int numBytes)
        {
            memory.CopyBytesFromArray(srcData, srcOffset, dstOffset + start, numBytes);
        }

        public void CopyBytesFromMemory(IMemory srcMem, int srcOffset, int dstOffset, int numBytes)
        {
            memory.CopyBytesFromMemory(srcMem, srcOffset, dstOffset + start, numBytes);
        }

        public void CopyArea(int src, int dst, int numBytes)
        {
            memory.CopyArea(src + start, dst + start, numBytes);
        }
    }
}
