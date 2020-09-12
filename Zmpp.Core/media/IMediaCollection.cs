/*
 * Created on 2006/02/12
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

namespace org.zmpp.media
{
    /// <summary>
    /// This interfaces defines the common functions of a media resource
    /// collection.A MediaCollection manages one specific type of media,
    /// e.g.sound effects or pictures.
    /// Resources might be loaded lazily and cached in an internal cache.
    /// </summary>
    public interface IMediaCollection<T>
    {
        /// <summary>
        /// Clears the collection.
        /// </summary>
        void clear();

        /// <summary>
        /// Accesses the resource.
        /// </summary>
        /// <param name="number">the number of the resource</param>
        /// <returns>the resource</returns>
        T getResource(int number);

        /// <summary>
        /// Loads a resource into the internal cache if this collection supports
        /// caching.
        /// </summary>
        /// <param name="number">the number of the resource</param>
        void loadResource(int number);

        /// <summary>
        /// Throws the resource out of the internal cache if this collection
        /// supports caching.
        /// </summary>
        /// <param name="number">the number of the resource</param>
        void unloadResource(int number);

        /// <summary>
        /// Returns the number of resources.
        /// </summary>
        /// <returns>the number of resources</returns>
        int getNumResources();
    }
}
