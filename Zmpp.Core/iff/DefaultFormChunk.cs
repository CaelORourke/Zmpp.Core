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
    using System.IO;
    using System.Text;
    using IMemory = Zmpp.Core.IMemory;
    using MemorySection = Zmpp.Core.MemorySection;

    /// <summary>
    /// This class implements the FormChunk interface.
    /// </summary>
    public class DefaultFormChunk : DefaultChunk, IFormChunk
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
        /// Constructor.
        /// </summary>
        /// <param name="memory">a MemoryAccess object</param>
        public DefaultFormChunk(IMemory memory) : base(memory, 0)
        {
            initBaseInfo();
            readSubChunks();
        }

        /// <summary>
        /// Initialize the id field.
        /// </summary>
        private void initBaseInfo()
        {
            if (!"FORM".Equals(getId())) {
                throw new IOException("not a valid IFF format");
            }

            // Determine the sub id
            subId = new byte[CHUNK_ID_LENGTH];
            memory.copyBytesToArray(subId, 0, CHUNK_HEADER_LENGTH, CHUNK_ID_LENGTH);
        }

        /// <summary>
        /// Read this form chunk's sub chunks.
        /// </summary>
        private void readSubChunks()
        {
            subChunks = new List<IChunk>();

            // skip the identifying information
            int length = getSize();
            int offset = CHUNK_HEADER_LENGTH + CHUNK_ID_LENGTH;
            int chunkTotalSize = 0;

            while (offset < length)
            {
                IMemory memarray = new MemorySection(memory, offset,
                                                                length - offset);
                IChunk subchunk = new DefaultChunk(memarray, offset);
                subChunks.Add(subchunk);
                chunkTotalSize = subchunk.getSize() + CHUNK_HEADER_LENGTH;

                // Determine if padding is necessary
                chunkTotalSize = (chunkTotalSize % 2) == 0 ? chunkTotalSize :
                                                             chunkTotalSize + 1;
                offset += chunkTotalSize;
            }
        }

        public override bool isValid() { return "FORM".Equals(getId()); }

        public String getSubId() { return Encoding.UTF8.GetString((byte[])(object)subId, 0, subId.Length); }

        public IEnumerator<IChunk> getSubChunks()
        {
            return subChunks.GetEnumerator();
        }

        public IChunk getSubChunk(String id)
        {
            foreach (IChunk chunk in subChunks) {
                if (chunk.getId().Equals(id))
                {
                    return chunk;
                }
            }
            return null;
        }

        public IChunk getSubChunk(int address)
        {
            foreach (IChunk chunk in subChunks) {
                if (chunk.getAddress() == address)
                {
                    return chunk;
                }
            }
            return null;
        }
    }
}
