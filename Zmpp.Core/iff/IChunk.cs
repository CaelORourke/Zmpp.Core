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
    using Zmpp.Core;

    /// <summary>
    /// Represents the basic data structure for an IFF file.
    /// </summary>
    public interface IChunk
    {
        /// <summary>
        /// Gets the id of this chunk.
        /// </summary>
        /// <remarks>An id is a 4 byte array.</remarks>
        String Id { get; }

        /// <summary>
        /// Gets the size of the data for this chunk.
        /// </summary>
        /// <remarks>Size does not include the id and the size word.</remarks>
        int Size { get; }

        /// <summary>
        /// Gets a value indicating whether this chunk is valid.
        /// </summary>
        /// <returns>true if the chunk is valid; otherwise false.</returns>
        bool IsValid { get; }

        /// <summary>
        /// Gets the memory object for this chunk.
        /// </summary>
        IMemory Memory { get; }

        /// <summary>
        /// Gets the address of this chunk within the global FORM chunk.
        /// </summary>
        int Address { get; }
    }
}
