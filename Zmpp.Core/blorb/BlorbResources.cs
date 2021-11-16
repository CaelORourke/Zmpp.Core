/*
 * Created on 2006/02/06
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

namespace Zmpp.Core.Blorb
{
    using System.Collections.Generic;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Media;

    /// <summary>
    /// This class encapsulates a Blorb file and offers access to the sound
    /// and graphics media collections.
    /// </summary>
    public class BlorbResources : IResources
    {
        /// <summary>
        /// The images from the Blorb file.
        /// </summary>
        private readonly IMediaCollection<IZmppImage> images;

        /// <summary>
        /// The sounds from the Blorb file.
        /// </summary>
        private readonly IMediaCollection<ISoundEffect> sounds;

        /// <summary>
        /// The cover art from the Blorb file.
        /// </summary>
        private readonly BlorbCoverArt coverart;

        /// <summary>
        /// The meta data from the Blorb file.
        /// </summary>
        private readonly List<StoryMetadata> metadata;

        /// <summary>
        /// The release number from the Blorb file.
        /// </summary>
        private readonly int release;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageFactory">The INativeImageFactory object.</param>
        /// <param name="soundEffectFactory">The ISoundEffectFactory object.</param>
        /// <param name="formchunk">The form chunk in Blorb format.</param>
        public BlorbResources(INativeImageFactory imageFactory,
            ISoundEffectFactory soundEffectFactory,
            IFormChunk formchunk)
        {
            images = new BlorbImages(imageFactory, formchunk);
            sounds = new BlorbSounds(soundEffectFactory, formchunk);
            coverart = new BlorbCoverArt(formchunk);
            metadata = Blorb.ReadMetadata(formchunk);
        }

        public IMediaCollection<IZmppImage> Images => images;

        public IMediaCollection<ISoundEffect> Sounds => sounds;

        public int CoverArtNum => coverart.CoverArtNum;

        public StoryMetadata Metadata => (metadata?.Count > 0) ? metadata[0] : null;

        public int Release => release;

        public bool HasInfo => (metadata?.Count > 0);
    }
}
