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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using static Zmpp.Core.MemoryUtil;
    using Memory = Zmpp.Core.Memory;
    using IMemory = Zmpp.Core.IMemory;

    /// <summary>
    /// Represents a writable FormChunk.
    /// </summary>
    public class WritableFormChunk : ChunkBase, IFormChunk
    {
        private readonly byte[] subId;
        private const string FORM_ID = "FORM";
        private readonly List<IChunk> subChunks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Iff.WritableFormChunk"/>class.
        /// </summary>
        /// <param name="subId">The sub id.</param>
        public WritableFormChunk(byte[] subId)
        {
            this.subId = subId;
            this.subChunks = new List<IChunk>();
        }

        /// <summary>
        /// Adds a sub chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        public void AddChunk(IChunk chunk)
        {
            subChunks.Add(chunk);
        }

        public string SubId => Encoding.UTF8.GetString((byte[])(object)subId, 0, subId.Length);

        public IEnumerator<IChunk> SubChunks => subChunks.GetEnumerator();

        public IChunk GetSubChunk(string id)
        {
            foreach (IChunk chunk in subChunks)
            {
                if (chunk.Id.Equals(id)) return chunk;
            }
            return null;
        }

        public IChunk GetSubChunk(int address)
        {
            // We do not need to implement this
            return null;
        }

        public String Id => FORM_ID;

        public int Size
        {
            get
            {
                int size = subId.Length;

                foreach (IChunk chunk in subChunks)
                {
                    int chunkSize = chunk.Size;
                    if ((chunkSize % 2) != 0)
                    {
                        chunkSize++; // pad if necessary
                    }
                    size += (ChunkHeaderLength + chunkSize);
                }
                return size;
            }
        }

        public bool IsValid => true;

        public IMemory Memory => new Memory(Bytes);

        /// <summary>
        /// Gets the data of this chunk.
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                int datasize = ChunkHeaderLength + Size;
                byte[] data = new byte[datasize];
                IMemory memory = new Memory(data);
                memory.WriteUnsigned8(0, 'F');
                memory.WriteUnsigned8(1, 'O');
                memory.WriteUnsigned8(2, 'R');
                memory.WriteUnsigned8(3, 'M');
                WriteUnsigned32(memory, 4, Size);

                int offset = ChunkHeaderLength;

                // Write sub id
                memory.CopyBytesFromArray(subId, 0, offset, subId.Length);
                offset += subId.Length;

                // Write sub chunk data
                foreach (IChunk chunk in subChunks)
                {
                    //byte[] chunkId = chunk.getId().getBytes();
                    byte[] chunkId = new byte[Encoding.UTF8.GetByteCount(chunk.Id)];
                    Encoding.UTF8.GetBytes(chunk.Id, 0, chunk.Id.Length, (byte[])(object)chunkId, 0);

                    int chunkSize = chunk.Size;

                    // Write id
                    memory.CopyBytesFromArray(chunkId, 0, offset, chunkId.Length);
                    offset += chunkId.Length;

                    // Write chunk size
                    WriteUnsigned32(memory, offset, chunkSize);
                    offset += 4; // add the size word length

                    // Write chunk data
                    IMemory chunkMem = chunk.Memory;
                    memory.CopyBytesFromMemory(chunkMem, ChunkHeaderLength, offset, chunkSize);
                    offset += chunkSize;
                    // Pad if necessary
                    if ((chunkSize % 2) != 0)
                    {
                        memory.WriteUnsigned8(offset++, (char)0);
                    }
                }
                return data;
            }
        }

        public int Address => 0;
    }
}
