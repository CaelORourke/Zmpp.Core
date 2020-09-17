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
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using IMemory = Zmpp.Core.IMemory;
    using MemorySection = Zmpp.Core.MemorySection;

    /// <summary>
    /// Represents a form chunk.
    /// </summary>
    public class FormChunk : Chunk, IFormChunk
    {
        /// <summary>
        /// The sub type id.
        /// </summary>
        private byte[] subId;

        /// <summary>
        /// The list of sub chunks.
        /// </summary>
        private List<IChunk> subChunks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Iff.FormChunk"/>
        /// class for the specified memory.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        public FormChunk(IMemory memory) : base(memory, 0)
        {
            InitBaseInfo();
            ReadSubChunks();
        }

        /// <summary>
        /// Initialize the id field.
        /// </summary>
        private void InitBaseInfo()
        {
            if (!"FORM".Equals(Id)) {
                throw new IOException("This is not a valid IFF format.");
            }

            // Determine the sub id
            subId = new byte[ChunkIdLength];
            memory.CopyBytesToArray(subId, 0, ChunkHeaderLength, ChunkIdLength);
        }

        /// <summary>
        /// Read the sub chunks of this form chunk.
        /// </summary>
        private void ReadSubChunks()
        {
            subChunks = new List<IChunk>();

            // skip the identifying information
            int length = Size;
            int offset = ChunkHeaderLength + ChunkIdLength;
            int chunkTotalSize = 0;

            while (offset < length)
            {
                IMemory memarray = new MemorySection(memory, offset, length - offset);
                IChunk subchunk = new Chunk(memarray, offset);
                subChunks.Add(subchunk);
                chunkTotalSize = subchunk.Size + ChunkHeaderLength;

                // Determine if padding is necessary
                chunkTotalSize = (chunkTotalSize % 2) == 0 ? chunkTotalSize : chunkTotalSize + 1;
                offset += chunkTotalSize;
            }
        }

        public override bool IsValid => "FORM".Equals(Id);

        public string SubId => Encoding.UTF8.GetString((byte[])(object)subId, 0, subId.Length);

        public IEnumerator<IChunk> SubChunks => subChunks.GetEnumerator();

        public IChunk GetSubChunk(string id)
        {
            foreach (IChunk chunk in subChunks) {
                if (chunk.Id.Equals(id))
                {
                    return chunk;
                }
            }
            return null;
        }

        public IChunk GetSubChunk(int address)
        {
            foreach (IChunk chunk in subChunks) {
                if (chunk.Address == address)
                {
                    return chunk;
                }
            }
            return null;
        }
    }
}
