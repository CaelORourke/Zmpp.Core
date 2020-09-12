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

namespace org.zmpp.iff
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// FormChunk is the wrapper chunk for all other chunks.
    /// </summary>
    public interface IFormChunk : IChunk
    {
        /// <summary>
        /// Returns the sub id.
        /// </summary>
        /// <returns>the sub id</returns>
        String getSubId();

        /// <summary>
        /// Returns an iterator of chunks contained in this form chunk.
        /// </summary>
        /// <returns>the enumeration of sub chunks</returns>
        IEnumerator<IChunk> getSubChunks();

        /// <summary>
        /// Returns the chunk with the specified id.
        /// </summary>
        /// <param name="id">the id</param>
        /// <returns>the chunk with the specified id or null if it does not exist</returns>
        IChunk getSubChunk(String id);

        /// <summary>
        /// Returns the sub chunk at the specified address or null if it does
        /// not exist.
        /// </summary>
        /// <param name="address">the address of the chunk</param>
        /// <returns>the chunk or null if it does not exist</returns>
        IChunk getSubChunk(int address);
    }
}
