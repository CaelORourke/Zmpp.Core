/*
 * Created on 11/07/2005
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
    using org.zmpp.io;

    /// <summary>
    /// This interface defines the access to the screen model.
    /// </summary>
    public interface IScreenModel
    {
        /// <summary>
        /// Returns the current annotation of the bottom window.
        /// </summary>
        /// <returns>the annotation in the bottom window</returns>
        TextAnnotation getBottomAnnotation();

        /// <summary>
        /// Returns the current annotation of the top window.
        /// </summary>
        /// <returns>the annotation in the top window</returns>
        TextAnnotation getTopAnnotation();

        /// <summary>
        /// Resets the screen model.
        /// </summary>
        void reset();

        /// <summary>
        /// Splits the screen into two windows, the upper window will contain
        /// linesUpperWindow lines.If linesUpperWindow is 0, the window will
        /// be unsplit.
        /// </summary>
        /// <param name="linesUpperWindow">the number of lines the upper window will have</param>
        void splitWindow(int linesUpperWindow);

        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="window">the active window</param>
        void setWindow(int window);

        /// <summary>
        /// Returns the active window.
        /// </summary>
        /// <returns>the active window</returns>
        int getActiveWindow();

        /// <summary>
        /// Sets the text style.
        /// </summary>
        /// <param name="style">the text style</param>
        void setTextStyle(int style);

        /// <summary>
        /// Sets the buffer mode.
        /// </summary>
        /// <param name="flag">true if should be buffered, false otherwise</param>
        void setBufferMode(bool flag);

        /// <summary>
        /// Version 4/5: If value is 1, erase from current cursor position to the
        /// end of the line.
        /// </summary>
        /// <param name="value">the parameter</param>
        void eraseLine(int value);

        /// <summary>
        /// Clears the window with the specified number to the background color.
        /// If window is -1, the screen is unsplit and the area is cleared.
        /// If window is -2, the whole screen is cleared, but the splitting status
        /// is retained.
        /// </summary>
        /// <param name="window">the window number</param>
        void eraseWindow(int window);

        /// <summary>
        /// Moves the cursor in the current window to the specified position.
        /// </summary>
        /// <param name="line">the line</param>
        /// <param name="column">the column</param>
        /// <param name="window">the window</param>
        void setTextCursor(int line, int column, int window);

        /// <summary>
        /// Retrieves the active window's cursor.
        /// </summary>
        /// <returns>the current window's cursor</returns>
        ITextCursor getTextCursor();

        ///// <summary>
        ///// Sets the paging mode. This is useful if the input stream is a file.
        ///// </summary>
        ///// <param name="flag">true to enable paging, false to disable</param>
        //void setPaging(bool flag);

        /// <summary>
        /// Sets the font in the current window.
        /// </summary>
        /// <param name="fontnumber">the font number</param>
        /// <returns>the previous font number</returns>
        char setFont(char fontnumber);

        /// <summary>
        /// Sets the background color.
        /// </summary>
        /// <param name="colornumber">the color number</param>
        /// <param name="window">the window</param>
        void setBackground(int colornumber, int window);

        /// <summary>
        /// Sets the foreground color.
        /// </summary>
        /// <param name="colornumber">a color number</param>
        /// <param name="window">the window</param>
        void setForeground(int colornumber, int window);

        /// <summary>
        /// Returns the output stream associated with the screen.
        /// </summary>
        /// <returns>the output stream</returns>
        IOutputStream getOutputStream();
    }
}
