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

namespace Zmpp.Core.Iff
{
    using Zmpp.Core;
    using System;
    using System.Text;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This is the default implementation of the Chunk interface.
    /// </summary>
    public class DefaultChunk : ChunkBase, IChunk
    {
        /// <summary>
        /// The memory access object.
        /// </summary>
        protected IMemory memory;

        /// <summary>
        /// The chunk id.
        /// </summary>
        private byte[] id;

        /// <summary>
        /// The chunk size.
        /// </summary>
        private int chunkSize;

        /// <summary>
        /// The start address within the form chunk.
        /// </summary>
        private int address;

        /// <summary>
        /// Constructor. Used for reading files.
        /// </summary>
        /// <param name="memory">a Memory object to the chunk data</param>
        /// <param name="address">the address within the form chunk</param>
        public DefaultChunk(IMemory memory, int address)
        {
            this.memory = memory;
            this.address = address;
            id = new byte[CHUNK_ID_LENGTH];
            memory.copyBytesToArray(id, 0, 0, CHUNK_ID_LENGTH);
            chunkSize = (int)readUnsigned32(memory, CHUNK_ID_LENGTH);
        }

        /// <summary>
        /// Constructor. Initialize from byte data. This constructor is used
        /// when writing a file, in that case chunks really are separate
        /// memory areas.
        /// </summary>
        /// <param name="id">the id</param>
        /// <param name="chunkdata">the data without header information, number of bytes
        /// needs to be even</param>
        public DefaultChunk(byte[] id, byte[] chunkdata)
        {
            this.id = id;
            this.chunkSize = chunkdata.Length;
            byte[] chunkDataWithHeader =
              new byte[chunkSize + CHUNK_HEADER_LENGTH];
            this.memory = new DefaultMemory(chunkDataWithHeader);
            memory.copyBytesFromArray(id, 0, 0, id.Length);
            writeUnsigned32(memory, id.Length, chunkSize);
            memory.copyBytesFromArray(chunkdata, 0, id.Length + 4, chunkdata.Length);
        }

        public virtual bool isValid() { return true; }

        public String getId() { return Encoding.UTF8.GetString((byte[])(object)id, 0, id.Length); }

        public int getSize() { return chunkSize; }

        public IMemory getMemory() { return memory; }

        public int getAddress() { return address; }
    }
}
