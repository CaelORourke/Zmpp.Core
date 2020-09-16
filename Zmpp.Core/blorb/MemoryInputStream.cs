/*
 * Created on 2006/02/06
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

namespace Zmpp.Core.Blorb
{
    using Zmpp.Core;

    /// <summary>
    /// This class encapsulates a memory object within an input stream.
    /// </summary>
    public class MemoryInputStream// : InputStream
    {
        /// <summary>
        /// The memory object this stream is based on.
        /// </summary>
        private readonly IMemory memory;

        /// <summary>
        /// The position in the stream.
        /// </summary>
        private int position;

        /// <summary>
        /// Supports a mark.
        /// </summary>
        private int mark;

        /// <summary>
        /// The size of the memory.
        /// </summary>
        private readonly int size;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memory">a memory object</param>
        /// <param name="offset">the byte offset</param>
        /// <param name="size">the memory size</param>
        public MemoryInputStream(IMemory memory, int offset, int size)
        {
            this.memory = memory;
            position += offset;
            this.size = size;
        }

        public int Read()
        {
            if (position >= size) return -1;
            return memory.ReadUnsigned8(position++);
        }

        public void Mark(int readLimit) { mark = position; }

        public void Reset() { position = mark; }
    }
}
