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
    public static class ScreenModel
    {
        public const int CURRENT_WINDOW = -3;
        public const int WINDOW_BOTTOM = 0;
        public const int WINDOW_TOP = 1;

        /// <summary>
        /// Font number for the standard font.
        /// </summary>
        public const char FONT_NORMAL = (char)1;

        /// <summary>
        /// Font number for the character graphics font.
        /// </summary>
        public const char FONT_CHARACTER_GRAPHICS = (char)3;

        /// <summary>
        /// Font number for the fixed pitch font.
        /// </summary>
        public const char FONT_FIXED = (char)4;

        public const int TEXTSTYLE_ROMAN = 0;
        public const int TEXTSTYLE_REVERSE_VIDEO = 1;
        public const int TEXTSTYLE_BOLD = 2;
        public const int TEXTSTYLE_ITALIC = 4;
        public const int TEXTSTYLE_FIXED = 8;

        // Color definitions.
        public const int UNDEFINED = -1000;
        public const int COLOR_UNDER_CURSOR = -1;
        public const int COLOR_CURRENT = 0;
        public const int COLOR_DEFAULT = 1;
        public const int COLOR_BLACK = 2;
        public const int COLOR_RED = 3;
        public const int COLOR_GREEN = 4;
        public const int COLOR_YELLOW = 5;
        public const int COLOR_BLUE = 6;
        public const int COLOR_MAGENTA = 7;
        public const int COLOR_CYAN = 8;
        public const int COLOR_WHITE = 9;
        public const int COLOR_MS_DOS_DARKISH_GREY = 10;
    }
}
