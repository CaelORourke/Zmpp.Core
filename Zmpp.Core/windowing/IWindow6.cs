/*
 * Created on 2006/02/23
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

namespace org.zmpp.windowing
{
    using org.zmpp.media;

    /// <summary>
    /// Window 6 interface. V6 windows in the Z-machine are probably the hackiest
    /// and trickiest challenge in a Z-machine.
    /// </summary>
    public interface IWindow6
    {
        /// <summary>
        /// Draws the specified picture at the given position.
        /// </summary>
        /// <param name="picture">the picture data</param>
        /// <param name="y">the y coordinate</param>
        /// <param name="x">the x coordinate</param>
        void drawPicture(IZmppImage picture, int y, int x);

        /// <summary>
        /// Clears the area of the specified picture at the given position.
        /// </summary>
        /// <param name="picture">the picture</param>
        /// <param name="y">the y coordinate</param>
        /// <param name="x">the x coordinate</param>
        void erasePicture(IZmppImage picture, int y, int x);

        /// <summary>
        /// Moves the window to the specified coordinates in pixels, (1, 1)
        /// being the top left.
        /// </summary>
        /// <param name="y">the y coordinate</param>
        /// <param name="x">the x coordinate</param>
        void move(int y, int x);

        /// <summary>
        /// Sets window size in pixels.
        /// </summary>
        /// <param name="height">the height in pixels</param>
        /// <param name="width">the width in pixels</param>
        void setSize(int height, int width);

        /// <summary>
        /// Sets the window style.
        /// The<i> styleflags</i> parameter is a bitmask specified as follows:
        /// - Bit 0: keep text within margins
        /// - Bit 1: scroll when at bottom
        /// - Bit 2: copy text to transcript stream (stream 2)
        /// - Bit 3: word wrapping
        /// 
        /// The<i> operation</i> parameter is specified as this:
        /// - 0: set style flags to the specified mask
        /// - 1: set the bits supplied
        /// - 2: clear the bits supplied
        /// - 3: reverse the bits supplied
        /// </summary>
        /// <param name="styleflags">the style flags</param>
        /// <param name="operation">the operation</param>
        void setStyle(int styleflags, int operation);

        /// <summary>
        /// Sets the window margins in pixels. If the cursor is overtaken by the
        /// new margins, set it to the new left margin.
        /// </summary>
        /// <param name="left">the left margin</param>
        /// <param name="right">the right margin</param>
        void setMargins(int left, int right);

        /// <summary>
        /// Returns the specified window property.
        /// 0  y coordinate    6   left margin size            12  font number
        /// 1  x coordinate    7   right margin size           13  font size
        /// 2  y size          8   newline interrupt routine   14  attributes
        /// 3  x size          9   interrupt countdown         15  line count
        /// 4  y cursor        10  text style
        /// 5  x cursor        11  colour data
        /// </summary>
        /// <param name="propertynum">the property number</param>
        /// <returns>the property value</returns>
        int getProperty(int propertynum);

        /// <summary>
        /// Sets the specified window property.
        /// </summary>
        /// <param name="propertynum">the property number</param>
        /// <param name="value">the value</param>
        void putProperty(int propertynum, short value);

        /// <summary>
        /// Scrolls the window by the specified amount of pixels, negative values
        /// scroll down, positive scroll up.
        /// </summary>
        /// <param name="pixels">the number of pixels</param>
        void scroll(int pixels);
    }
}
