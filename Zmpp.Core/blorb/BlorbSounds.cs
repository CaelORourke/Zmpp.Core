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
    using System;
    using System.Collections.Generic;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Media;

    /// <summary>
    /// Represents the Blorb sound collection.
    /// </summary>
    public class BlorbSounds : BlorbMediaCollection<ISoundEffect>
    {
        /// <summary>
        /// This map implements the database.
        /// </summary>
        private Dictionary<int, ISoundEffect> sounds;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory">The ISoundEffectFactory object.</param>
        /// <param name="formchunk">The form chunk.</param>
        public BlorbSounds(ISoundEffectFactory factory, IFormChunk formchunk) : base(null, factory, formchunk)
        {
        }

        public override void Clear()
        {
            base.Clear();
            sounds.Clear();
        }

        protected override void InitDatabase()
        {
            sounds = new Dictionary<int, ISoundEffect>();
        }

        protected override bool IsHandledResource(byte[] usageId)
        {
            return usageId[0] == 'S' && usageId[1] == 'n' && usageId[2] == 'd' && usageId[3] == ' ';
        }

        public override ISoundEffect GetResource(int resourcenumber)
        {
            return sounds[resourcenumber];
        }

        protected override bool PutToDatabase(IChunk aiffChunk, int resnum)
        {
            try
            {
                sounds[resnum] = soundEffectFactory.createSoundEffect(aiffChunk);
                return true;
            }
            catch (Exception)
            {
                //ex.printStackTrace();
            }
            return false;
        }
    }
}
