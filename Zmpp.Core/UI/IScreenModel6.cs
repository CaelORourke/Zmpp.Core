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

namespace Zmpp.Core.UI
{
    using Zmpp.Core.Media;

    /// <summary>
    /// Screen model 6 interface.
    /// </summary>
    public interface IScreenModel6 : IScreenModel, IDrawingArea
    {
        /// <summary>
        /// Restricts the mouse pointer to the specified window.
        /// </summary>
        /// <param name="window">the window</param>
        void setMouseWindow(int window);

        /// <summary>
        /// Returns the specified window.
        /// </summary>
        /// <param name="window">the window</param>
        /// <returns>the window</returns>
        IWindow6 getWindow(int window);

        /// <summary>
        /// Returns the currently selected window.
        /// </summary>
        /// <returns>the currently selected window</returns>
        IWindow6 getSelectedWindow();

        /// <summary>
        /// Instructs the screen model to set the width of the current string
        /// to the header.
        /// </summary>
        /// <param name="zchars">the z character array</param>
        void setTextWidthInUnits(char[] zchars);

        /**
         * 
         *
         * @param array 
         */

        /// <summary>
        /// Reads the current mouse data into the specified array.
        /// </summary>
        /// <param name="array">the array address</param>
        void readMouse(int array);
    }
}
