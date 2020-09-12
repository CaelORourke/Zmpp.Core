/*
 * Created on 2008/04/26
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
    using System;

    /// <summary>
    /// An annotation to indicate how a sequence of characters should be printed.
    /// </summary>
    public class TextAnnotation
    {
        private const long serialVersionUID = 343600790510649067L;

        // Font flags have the same bit layout as in the ScreenModel interface so
        // so the flags are compatible
        public const char FONT_NORMAL = (char)1;
        public const char FONT_CHARACTER_GRAPHICS = (char)3;
        public const char FONT_FIXED = (char)4;

        // Text styles have the same bit layout as in the ScreenModel interface
        // so the flags are compatible
        public const int TEXTSTYLE_ROMAN = 0;
        public const int TEXTSTYLE_REVERSE_VIDEO = 1;
        public const int TEXTSTYLE_BOLD = 2;
        public const int TEXTSTYLE_ITALIC = 4;
        public const int TEXTSTYLE_FIXED = 8;

        private char font;
        private int style;
        private int background;
        private int foreground;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="font">font number</param>
        /// <param name="style">style number</param>
        /// <param name="background">background color</param>
        /// <param name="foreground">foreground color</param>
        public TextAnnotation(char font, int style, int background, int foreground)
        {
            this.font = font;
            this.style = style;
            this.background = background;
            this.foreground = foreground;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="font">font number</param>
        /// <param name="style">style number</param>
        public TextAnnotation(char font, int style) : this(font, style, ScreenModel.COLOR_DEFAULT, ScreenModel.COLOR_DEFAULT)
        {
        }

        /// <summary>
        /// Derives an annotation with a modified font based on this object.
        /// </summary>
        /// <param name="newFont">new font number</param>
        /// <returns>TextAnnotation with changed font</returns>
        public TextAnnotation deriveFont(char newFont)
        {
            return new TextAnnotation(newFont, this.style, this.background, this.foreground);
        }

        /// <summary>
        /// Derives an annotation with a modified text style based on this object.
        /// </summary>
        /// <param name="newStyle">new style number</param>
        /// <returns>TextAnnotation with changed style</returns>
        public TextAnnotation deriveStyle(int newStyle)
        {
            int finalStyle = style;
            if (newStyle == TextAnnotation.TEXTSTYLE_ROMAN)
            {
                finalStyle = newStyle;
            }
            else
            {
                finalStyle |= newStyle;
            }
            return new TextAnnotation(this.font, finalStyle, this.background, this.foreground);
        }

        /// <summary>
        /// Derives an annotation with a modified background color based
        /// on this object.
        /// </summary>
        /// <param name="newBackground">new background color</param>
        /// <returns>TextAnnotation with changed foreground color</returns>
        public TextAnnotation deriveBackground(int newBackground)
        {
            return new TextAnnotation(this.font, this.style, newBackground, this.foreground);
        }

        /// <summary>
        /// Derives an annotation with a modified foreground color based
        /// on this object.
        /// </summary>
        /// <param name="newForeground">new foreground color</param>
        /// <returns>TextAnnotation with changed foreground color</returns>
        public TextAnnotation deriveForeground(int newForeground)
        {
            return new TextAnnotation(this.font, this.style, this.background, newForeground);
        }

        /// <summary>
        /// Returns the font.
        /// </summary>
        /// <returns>font number</returns>
        public char getFont() { return font; }

        /// <summary>
        /// Determines whether this annotation has a fixed style font.
        /// </summary>
        /// <returns>true if fixed, false if variable font</returns>
        public bool isFixed()
        {
            return font == FONT_FIXED || (style & TEXTSTYLE_FIXED) == TEXTSTYLE_FIXED;
        }

        /// <summary>
        /// Determines whether this annotation has a roman font.
        /// </summary>
        /// <returns>true if roman, false otherwise</returns>
        public bool isRoman() { return style == TEXTSTYLE_ROMAN; }

        /// <summary>
        /// Determines whether this annotation has a bold font.
        /// </summary>
        /// <returns>true if bold, false otherwise</returns>
        public bool isBold()
        {
            return (style & TEXTSTYLE_BOLD) == TEXTSTYLE_BOLD;
        }

        /// <summary>
        /// Determines whether this annotation has an italic font
        /// </summary>
        /// <returns>true if italic, false otherwise</returns>
        public bool isItalic()
        {
            return (style & TEXTSTYLE_ITALIC) == TEXTSTYLE_ITALIC;
        }

        /// <summary>
        /// Determines whether the text is displayed as reverse video.
        /// </summary>
        /// <returns>true if reverse video, false otherwise</returns>
        public bool isReverseVideo()
        {
            return (style & TEXTSTYLE_REVERSE_VIDEO) == TEXTSTYLE_REVERSE_VIDEO;
        }

        /// <summary>
        /// Returns the background color.
        /// </summary>
        /// <returns>background color</returns>
        public int getBackground() { return background; }

        /// <summary>
        /// Returns the foreground color.
        /// </summary>
        /// <returns>foreground color</returns>
        public int getForeground() { return foreground; }

        public String toString()
        {
            return "TextAnnotation, fixed: " + isFixed() + " bold: " + isBold() +
                    " italic: " + isItalic() + " reverse: " + isReverseVideo() +
                    " bg: " + background + " fg: " + foreground;
        }
    }
}
