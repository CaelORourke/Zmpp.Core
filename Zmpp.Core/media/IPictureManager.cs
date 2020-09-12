/*
 * Created on 2006/02/22
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
    /// Interface for managing pictures.
    /// </summary>
    public interface IPictureManager
    {
        /// <summary>
        /// Returns the size of the specified picture or null if the picture does not
        /// exist.
        /// </summary>
        /// <param name="picturenum">the number of the picture</param>
        /// <returns>the size</returns>
        Resolution getPictureSize(int picturenum);

        /// <summary>
        /// Returns the data of the specified picture. If it is not available, this
        /// method returns null.
        /// </summary>
        /// <param name="picturenum">the picture number</param>
        /// <returns>the data</returns>
        IZmppImage getPicture(int picturenum);

        /// <summary>
        /// Returns the number of pictures.
        /// </summary>
        /// <returns>the number of pictures</returns>
        int getNumPictures();

        /// <summary>
        /// Preloads the specified picture numbers.
        /// </summary>
        /// <param name="picnumbers">the picture numbers to preload</param>
        void preload(int[] picnumbers);

        /// <summary>
        /// Returns the release number of the picture file.
        /// </summary>
        /// <returns>the release number</returns>
        int getRelease();

        /// <summary>
        /// Resets the picture manager.
        /// </summary>
        void reset();
    }
}
