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
    using System.IO;
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Media;
    using static Zmpp.Core.MemoryUtil;
    using static Zmpp.Core.Blorb.BlorbImage;

    /// <summary>
    /// Represents the collection of images in a Blorb file.
    /// </summary>
    public class BlorbImages : BlorbMediaCollection<IZmppImage>
    {
        /// <summary>
        /// This map implements the image database.
        /// </summary>
        private Dictionary<int, BlorbImage> images;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageFactory">the NativeImageFactory object.</param>
        /// <param name="formchunk">the form chunk.</param>
        public BlorbImages(INativeImageFactory imageFactory, IFormChunk formchunk) : base(imageFactory, null, formchunk)
        {
            HandleResoChunk();
        }

        public override void Clear()
        {
            base.Clear();
            images.Clear();
        }

        protected override void InitDatabase()
        {
            images = new Dictionary<int, BlorbImage>();
        }

        protected override bool IsHandledResource(byte[] usageId)
        {
            return usageId[0] == 'P' && usageId[1] == 'i' && usageId[2] == 'c' && usageId[3] == 't';
        }

        public override IZmppImage GetResource(int resourcenumber)
        {
            return images[resourcenumber];
        }

        protected override bool PutToDatabase(IChunk chunk, int resnum)
        {
            if (!HandlePlaceholder(chunk, resnum))
            {
                return HandlePicture(chunk, resnum);
            }
            return true;
        }

        /// <summary>
        /// Handles a placeholder image.
        /// </summary>
        /// <param name="chunk">the Chunk object.</param>
        /// <param name="resnum">the resource number.</param>
        /// <returns>true if successful, false otherwise.</returns>
        private bool HandlePlaceholder(IChunk chunk, int resnum)
        {
            if ("Rect".Equals(chunk.Id))
            {
                // Place holder
                IMemory memory = chunk.Memory;
                int width = (int)ReadUnsigned32(memory, Chunk.ChunkHeaderLength);
                int height = (int)ReadUnsigned32(memory, Chunk.ChunkHeaderLength + 4);
                images[resnum] = new BlorbImage(width, height);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Processes the picture contained in the specified chunk.
        /// </summary>
        /// <param name="chunk">the Chunk.</param>
        /// <param name="resnum">the resource number.</param>
        /// <returns>true if successful, false otherwise.</returns>
        private bool HandlePicture(IChunk chunk, int resnum)
        {
            // TODO: Implement this method!!!
            //Stream inputStream
            //    = new MemoryInputStream(chunk.Memory, Chunk.ChunkHeaderLength, chunk.Size + Chunk.ChunkHeaderLength);
            //try
            //{
            //    images[resnum] = new BlorbImage(imageFactory.createImage(inputStream));
            //    return true;
            //}
            //catch (IOException)
            //{
            //    //ex.printStackTrace();
            //}
            return false;
        }

        /// <summary>
        /// Process the Reso chunk.
        /// </summary>
        private void HandleResoChunk()
        {
            IChunk resochunk = FormChunk.GetSubChunk("Reso");
            if (resochunk != null)
            {
                AdjustResolution(resochunk);
            }
        }

        /// <summary>
        /// Adjusts the resolution of the image.
        /// </summary>
        /// <param name="resochunk">the Reso chunk.</param>
        private void AdjustResolution(IChunk resochunk)
        {
            IMemory memory = resochunk.Memory;
            int offset = Chunk.ChunkIdLength;
            int size = (int)ReadUnsigned32(memory, offset);
            offset += Chunk.ChunkSizewordLength;
            int px = (int)ReadUnsigned32(memory, offset);
            offset += 4;
            int py = (int)ReadUnsigned32(memory, offset);
            offset += 4;
            int minx = (int)ReadUnsigned32(memory, offset);
            offset += 4;
            int miny = (int)ReadUnsigned32(memory, offset);
            offset += 4;
            int maxx = (int)ReadUnsigned32(memory, offset);
            offset += 4;
            int maxy = (int)ReadUnsigned32(memory, offset);
            offset += 4;

            ResolutionInfo resinfo = new ResolutionInfo(new Resolution(px, py),
                new Resolution(minx, miny), new Resolution(maxx, maxy));

            for (int i = 0; i < NumResources; i++)
            {
                if (offset >= size) break;
                int imgnum = (int)ReadUnsigned32(memory, offset);
                offset += 4;
                int ratnum = (int)ReadUnsigned32(memory, offset);
                offset += 4;
                int ratden = (int)ReadUnsigned32(memory, offset);
                offset += 4;
                int minnum = (int)ReadUnsigned32(memory, offset);
                offset += 4;
                int minden = (int)ReadUnsigned32(memory, offset);
                offset += 4;
                int maxnum = (int)ReadUnsigned32(memory, offset);
                offset += 4;
                int maxden = (int)ReadUnsigned32(memory, offset);
                offset += 4;
                ScaleInfo scaleinfo = new ScaleInfo(resinfo, new Ratio(ratnum, ratden), new Ratio(minnum, minden), new Ratio(maxnum, maxden));
                BlorbImage img = images[imgnum];

                if (img != null)
                {
                    img.SetScaleInfo(scaleinfo);
                }
            }
        }
    }
}