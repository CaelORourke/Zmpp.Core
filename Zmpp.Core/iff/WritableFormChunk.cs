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
    using DefaultMemory = Zmpp.Core.DefaultMemory;
    using IMemory = Zmpp.Core.IMemory;

    /// <summary>
    /// A writable FormChunk class.
    /// </summary>
    public class WritableFormChunk : ChunkBase, IFormChunk
    {
        private byte[] subId;
        private const String FORM_ID = "FORM";
        private List<IChunk> subChunks;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="subId">the sub id</param>
        public WritableFormChunk(byte[] subId)
        {
            this.subId = subId;
            this.subChunks = new List<IChunk>();
        }

        /// <summary>
        /// Adds a sub chunk.
        /// </summary>
        /// <param name="chunk">the sub chunk to add</param>
        public void addChunk(IChunk chunk)
        {
            subChunks.Add(chunk);
        }

        public String getSubId()
        {
            return Encoding.UTF8.GetString((byte[])(object)subId, 0, subId.Length);
        }

        public IEnumerator<IChunk> getSubChunks()
        {
            return subChunks.GetEnumerator();
        }

        public IChunk getSubChunk(String id)
        {
            foreach (IChunk chunk in subChunks)
            {
                if (chunk.getId().Equals(id)) return chunk;
            }
            return null;
        }

        public IChunk getSubChunk(int address)
        {
            // We do not need to implement this
            return null;
        }

        public String getId() { return FORM_ID; }

        public int getSize()
        {
            int size = subId.Length;

            foreach (IChunk chunk in subChunks)
            {
                int chunkSize = chunk.getSize();
                if ((chunkSize % 2) != 0)
                {
                    chunkSize++; // pad if necessary
                }
                size += (CHUNK_HEADER_LENGTH + chunkSize);
            }
            return size;
        }

        public bool isValid() { return true; }

        public IMemory getMemory() { return new DefaultMemory(getBytes()); }

        /// <summary>
        /// Returns the data of this chunk.
        /// </summary>
        /// <returns>the chunk data</returns>
        public byte[] getBytes()
        {
            int datasize = CHUNK_HEADER_LENGTH + getSize();
            byte[] data = new byte[datasize];
            IMemory memory = new DefaultMemory(data);
            memory.writeUnsigned8(0, 'F');
            memory.writeUnsigned8(1, 'O');
            memory.writeUnsigned8(2, 'R');
            memory.writeUnsigned8(3, 'M');
            writeUnsigned32(memory, 4, getSize());

            int offset = CHUNK_HEADER_LENGTH;

            // Write sub id
            memory.copyBytesFromArray(subId, 0, offset, subId.Length);
            offset += subId.Length;

            // Write sub chunk data
            foreach (IChunk chunk in subChunks)
            {
                //byte[] chunkId = chunk.getId().getBytes();
                byte[] chunkId = new byte[Encoding.UTF8.GetByteCount(chunk.getId())];
                Encoding.UTF8.GetBytes(chunk.getId(), 0, chunk.getId().Length, (byte[])(object)chunkId, 0);

                int chunkSize = chunk.getSize();

                // Write id
                memory.copyBytesFromArray(chunkId, 0, offset, chunkId.Length);
                offset += chunkId.Length;

                // Write chunk size
                writeUnsigned32(memory, offset, chunkSize);
                offset += 4; // add the size word length

                // Write chunk data
                IMemory chunkMem = chunk.getMemory();
                memory.copyBytesFromMemory(chunkMem, CHUNK_HEADER_LENGTH, offset, chunkSize);
                offset += chunkSize;
                // Pad if necessary
                if ((chunkSize % 2) != 0)
                {
                    memory.writeUnsigned8(offset++, (char)0);
                }
            }
            return data;
        }

        public int getAddress() { return 0; }
    }
}
