/*
 * Created on 2006/02/13
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

namespace Zmpp.Core.Media
{
    using Zmpp.Core.Blorb;

    /// <summary>
    /// This interface defines access to the Z-machine's media resources.
    /// </summary>
    public interface IResources
    {
        /// <summary>
        /// The release number of the resource file.
        /// </summary>
        /// <returns>the release number</returns>
        int Release { get; }

        /// <summary>
        /// Returns the images of this file.
        /// </summary>
        /// <returns>the images</returns>
        IMediaCollection<IZmppImage> Images { get; }

        /// <summary>
        /// Returns the sounds of this file.
        /// </summary>
        /// <returns>the sounds</returns>
        IMediaCollection<ISoundEffect> Sounds { get; }

        /// <summary>
        /// Returns the number of the cover art picture.
        /// </summary>
        /// <returns>the number of the cover art picture</returns>
        int CoverArtNum { get; }

        /// <summary>
        /// Returns the inform meta data if available.
        /// </summary>
        /// <returns>the meta data</returns>
        StoryMetadata Metadata { get; }

        /// <summary>
        /// Returns true if the resource file has information.
        /// </summary>
        /// <returns>true if information, false, otherwise</returns>
        bool HasInfo { get; }
    }
}
