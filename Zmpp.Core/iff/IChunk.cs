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

    /// <summary>
    /// The basic data structure for an IFF file, a chunk.
    /// </summary>
    public interface IChunk
    {
        /// <summary>
        /// Returns this IFF chunk's id. An id is a 4 byte array.
        /// </summary>
        /// <returns>the id</returns>
        String getId();

        /// <summary>
        /// The chunk data size, excluding id and size word.
        /// </summary>
        /// <returns>the size</returns>
        int getSize();

        /// <summary>
        /// Returns true if this is a valid chunk.
        /// </summary>
        /// <returns>true if valid, false otherwise</returns>
        bool isValid();

        /// <summary>
        /// Returns a memory object to access the chunk.
        /// </summary>
        /// <returns>the Memory object</returns>
        IMemory getMemory();

        /// <summary>
        /// Returns the address of the chunk within the global FORM chunk.
        /// </summary>
        /// <returns>the address within the form chunk</returns>
        int getAddress();
    }
}
