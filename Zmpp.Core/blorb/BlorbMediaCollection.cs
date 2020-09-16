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
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Media;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This class defines an abstract media collection based on the Blorb format.
    /// </summary>
    /// <remarks>
    /// It also defines the common read process for resources.The collection
    /// is represented by a database and an index to the database, which maps
    /// index numbers to resource numbers.The implementation of the database
    /// is left to the sub classes.
    /// </remarks>
    public abstract class BlorbMediaCollection<T> : IMediaCollection<T>
    {
        /// <summary>
        /// The list of resource numbers in the file.
        /// </summary>
        private readonly List<int> resourceNumbers;

        /// <summary>
        /// Access to the form chunk.
        /// </summary>
        private readonly IFormChunk formchunk;

        protected INativeImageFactory imageFactory;
        protected ISoundEffectFactory soundEffectFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageFactory">The INativeImageFactory object.</param>
        /// <param name="soundEffectFactory">The ISoundEffectFactory object.</param>
        /// <param name="formchunk">The Blorb file form chunk.</param>
        public BlorbMediaCollection(INativeImageFactory imageFactory, ISoundEffectFactory soundEffectFactory, IFormChunk formchunk)
        {
            resourceNumbers = new List<int>();
            this.formchunk = formchunk;
            this.imageFactory = imageFactory;
            this.soundEffectFactory = soundEffectFactory;
            InitDatabase();

            // Ridx chunk
            IChunk ridxChunk = formchunk.GetSubChunk("RIdx");
            IMemory chunkmem = ridxChunk.Memory;
            int numresources = (int)ReadUnsigned32(chunkmem, 8);
            int offset = 12;
            byte[] usage = new byte[4];

            for (int i = 0; i < numresources; i++)
            {
                chunkmem.CopyBytesToArray(usage, 0, offset, 4);
                if (IsHandledResource(usage))
                {
                    int resnum = (int)ReadUnsigned32(chunkmem, offset + 4);
                    int address = (int)ReadUnsigned32(chunkmem, offset + 8);
                    IChunk chunk = formchunk.GetSubChunk(address);

                    if (PutToDatabase(chunk, resnum))
                    {
                        resourceNumbers.Add(resnum);
                    }
                }
                offset += 12;
            }
        }

        public virtual void Clear() { resourceNumbers.Clear(); }

        public int NumResources => resourceNumbers.Count;

        /// <summary>
        /// Gets the resource number for the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The resource number.</returns>
        public int GetResourceNumber(int index)
        {
            return resourceNumbers[index];
        }

        public void LoadResource(int resourcenumber)
        {
            // This space left intentionally blank.
        }

        public void UnloadResource(int resourcenumber)
        {
            // This space left intentionally blank.
        }

        /// <summary>
        /// Gets the form chunk.
        /// </summary>
        protected IFormChunk FormChunk => formchunk;

        /// <summary>
        /// Initialize the database.
        /// </summary>
        protected abstract void InitDatabase();

        /// <summary>
        /// Indicates whether the specified resource is handled by this media collection.
        /// </summary>
        /// <param name="usageId">The usage id.</param>
        /// <returns>true if the media collection handles the specfied resource; otherwise false.</returns>
        protected abstract bool IsHandledResource(byte[] usageId);

        /// <summary>
        /// Puts the media object based on this sub chunk into the database.
        /// </summary>
        /// <param name="chunk">The blorb sub chunk.</param>
        /// <param name="resnum">The resource number.</param>
        /// <returns>true if successful; otherwise false.</returns>
        protected abstract bool PutToDatabase(IChunk chunk, int resnum);

        public abstract T GetResource(int number);
    }
}
