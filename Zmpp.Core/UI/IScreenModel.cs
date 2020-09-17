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

namespace Zmpp.Core.UI
{
    using Zmpp.Core.IO;

    /// <summary>
    /// This interface defines the access to the screen model.
    /// </summary>
    public interface IScreenModel
    {
        /// <summary>
        /// Gets the current annotation of the bottom window.
        /// </summary>
        TextAnnotation BottomAnnotation { get; }

        /// <summary>
        /// Gets the current annotation of the top window.
        /// </summary>
        TextAnnotation TopAnnotation { get; }

        /// <summary>
        /// Resets the screen model.
        /// </summary>
        void Reset();

        /// <summary>
        /// Splits the screen into two windows.
        /// </summary>
        /// <param name="linesUpperWindow">The number of lines the upper window will have.</param>
        /// <remarks>
        /// The upper window will contain linesUpperWindow lines. If
        /// linesUpperWindow is 0 the window will be unsplit.
        /// </remarks>
        void SplitWindow(int linesUpperWindow);

        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="window">The active window.</param>
        void SetWindow(int window);

        /// <summary>
        /// Gets the active window.
        /// </summary>
        int ActiveWindow { get; }

        /// <summary>
        /// Sets the text style.
        /// </summary>
        /// <param name="style">The text style.</param>
        void SetTextStyle(int style);

        /// <summary>
        /// Sets the buffer mode.
        /// </summary>
        /// <param name="flag">true to enable buffering; otherwise false.</param>
        void SetBufferMode(bool flag);

        /// <summary>
        /// Version 4/5: If value is 1, erase from current cursor position to the
        /// end of the line.
        /// </summary>
        /// <param name="value">The parameter.</param>
        void EraseLine(int value);

        /// <summary>
        /// Clears the window with the specified number to the background color.
        /// </summary>
        /// <param name="window">The window number.</param>
        /// <remarks>
        /// If window is -1, the screen is unsplit and the area is cleared.
        /// If window is -2, the whole screen is cleared, but the splitting status
        /// is retained.
        /// </remarks>
        void EraseWindow(int window);

        /// <summary>
        /// Moves the cursor in the current window to the specified position.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        /// <param name="window">The window.</param>
        void SetTextCursor(int line, int column, int window);

        /// <summary>
        /// Gets the cursor for the active window.
        /// </summary>
        ITextCursor TextCursor { get; }

        ///// <summary>
        ///// Sets the paging mode. This is useful if the input stream is a file.
        ///// </summary>
        ///// <param name="flag">true to enable paging, false to disable</param>
        //void SetPaging(bool flag);

        /// <summary>
        /// Sets the font in the current window.
        /// </summary>
        /// <param name="fontnumber">The font number.</param>
        /// <returns>The previous font number.</returns>
        char SetFont(char fontnumber);

        /// <summary>
        /// Sets the background color.
        /// </summary>
        /// <param name="colornumber">The color number.</param>
        /// <param name="window">The window.</param>
        void SetBackground(int colornumber, int window);

        /// <summary>
        /// Sets the foreground color.
        /// </summary>
        /// <param name="colornumber">The color number.</param>
        /// <param name="window">The window.</param>
        void SetForeground(int colornumber, int window);

        /// <summary>
        /// Gets the output stream associated with the screen.
        /// </summary>
        IOutputStream OutputStream { get; }
    }
}
