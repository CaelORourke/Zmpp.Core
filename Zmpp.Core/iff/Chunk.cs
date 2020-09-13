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
    using System.Text;
    using Zmpp.Core;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// Represents the basic data structure for an IFF file.
    /// </summary>
    public class Chunk : ChunkBase, IChunk
    {
        /// <summary>
        /// The memory map.
        /// </summary>
        protected IMemory memory;

        /// <summary>
        /// The chunk id.
        /// </summary>
        private readonly byte[] id;

        /// <summary>
        /// The chunk size.
        /// </summary>
        private readonly int chunkSize;

        /// <summary>
        /// The start address within the form chunk.
        /// </summary>
        private readonly int address;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Iff.Chunk"/>
        /// class for the specified memory and address.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The address within the form chunk.</param>
        /// <remarks>This constructor is used when reading files.</remarks>
        public Chunk(IMemory memory, int address)
        {
            this.memory = memory;
            this.address = address;
            id = new byte[ChunkIdLength];
            memory.CopyBytesToArray(id, 0, 0, ChunkIdLength);
            chunkSize = (int)ReadUnsigned32(memory, ChunkIdLength);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Iff.Chunk"/>
        /// class for the specified id and data.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="chunkdata">The data without header information. Number of bytes
        /// needs to be even</param>
        /// <remarks>
        /// This constructor is used when writing to a file. In that case
        /// chunks really are separate memory areas.
        /// </remarks>
        public Chunk(byte[] id, byte[] chunkdata)
        {
            this.id = id;
            this.chunkSize = chunkdata.Length;
            byte[] chunkDataWithHeader = new byte[chunkSize + ChunkHeaderLength];
            this.memory = new Memory(chunkDataWithHeader);
            memory.CopyBytesFromArray(id, 0, 0, id.Length);
            WriteUnsigned32(memory, id.Length, chunkSize);
            memory.CopyBytesFromArray(chunkdata, 0, id.Length + 4, chunkdata.Length);
        }

        public virtual bool IsValid => true;

        public string Id => Encoding.UTF8.GetString((byte[])(object)id, 0, id.Length);

        public int Size => chunkSize;

        public IMemory Memory => memory;

        public int Address => address;
    }
}
