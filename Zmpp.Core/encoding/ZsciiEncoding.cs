/*
 * Created on 2005/11/10
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

namespace Zmpp.Core.Encoding
{
    using System.Text;

    /// <summary>
    /// Represents a ZSCII encoding.
    /// </summary>
    /// <remarks>
    /// <para>This is for encoding and decoding input and output strings.</para>
    /// <para>For encoding and decoding within a story file alphabet tables are used.</para>
    /// </remarks>
    public class ZsciiEncoding : IZsciiEncoding
    {
        #region Constants

        public const char Null = (char)0;
        public const char Delete = (char)8;
        public const char Newline10 = (char)10;
        public const char Newline = (char)13;
        public const char Escape = (char)27;
        public const char CursorUp = (char)129;
        public const char CursorDown = (char)130;
        public const char CursorLeft = (char)131;
        public const char CursorRight = (char)132;
        public const char AsciiStart = (char)32;
        public const char AsciiEnd = (char)126;

        /// <summary>
        /// The start of the accent range.
        /// </summary>
        public const char AccentStart = (char)155;

        /// <summary>
        /// End of the accent range.
        /// </summary>
        public const char AccentEnd = (char)251;

        public const char MouseDoubleClick = (char)253;
        public const char MouseSingleClick = (char)254;

        #endregion

        private readonly IAccentTable accentTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Encoding.ZsciiEncoding"/>
        /// class for the specified accent table.
        /// </summary>
        /// <param name="accentTable">The accent table.</param>
        public ZsciiEncoding(IAccentTable accentTable)
        {
            this.accentTable = accentTable;
        }

        /// <summary>
        /// Indicates whether the specified character is a
        /// valid ZSCII character.
        /// </summary>
        /// <param name="zchar">The ZSCII character.</param>
        /// <returns>true if the specified character is a
        /// valid ZSCII character; otherwise false.</returns>
        public bool IsZscii(char zchar)
        {
            switch (zchar)
            {
                case Null:
                case Delete:
                case Newline:
                case Escape:
                    return true;
                default:
                    return IsAscii(zchar) || IsAccent(zchar) || IsUnicode(zchar);
            }
        }

        /// <summary>
        /// Indicates whether the specified character can be converted
        /// to a ZSCII character.
        /// </summary>
        /// <param name="c">The Unicode character.</param>
        /// <returns>true if the specified character can be converted; otherwise false.</returns>
        public bool IsConvertibleToZscii(char c)
        {
            return IsAscii(c) || IsInTranslationTable(c) || c == '\n' || c == 0 || IsUnicode(c);
        }

        /// <summary>
        /// Converts the specified ZSCII character to a Unicode character.
        /// </summary>
        /// <param name="zchar">The ZSCII character.</param>
        /// <returns>The Unicode character.</returns>
        public char ToUnicodeChar(char zchar)
        {
            if (IsAscii(zchar)) return zchar;
            if (IsAccent(zchar))
            {
                int index = zchar - AccentStart;
                if (index < accentTable.Length)
                {
                    return (char)accentTable.GetAccent(index);
                }
            }
            if (zchar == Null) return '\0';
            if (zchar == Newline || zchar == Newline10) return '\n';
            if (IsUnicode(zchar)) return zchar;
            return '?';
        }

        /// <summary>
        /// Converts the specified Unicode string to a ZSCII string.
        /// </summary>
        /// <param name="str">The Unicode string.</param>
        /// <returns>The ZSCII string.</returns>
        public string ToZsciiString(string str)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                result.Append(ToZsciiChar(str[i]));
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts the specified Unicode character to a ZSCII character.
        /// </summary>
        /// <param name="c">The Unicode character.</param>
        /// <returns>The ZSCII character or '0' if the character cannot be converted.</returns>
        public char ToZsciiChar(char c)
        {
            if (IsAscii(c))
            {
                return c;
            }
            else if (IsInTranslationTable(c))
            {
                return (char)(GetIndexInTranslationTable(c) + AccentStart);
            }
            else if (c == '\n')
            {
                return Newline;
            }
            return (char)0;
        }

        /// <summary>
        /// Indicates whether the specified character is in the
        /// translation table.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>true if the specified character is in the translation table; otherwise false.</returns>
        private bool IsInTranslationTable(char c)
        {
            return GetIndexInTranslationTable(c) >= 0;
        }

        /// <summary>
        /// Gets the index of the specified character in the
        /// translation table.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The index of the specified character in the translation
        /// table or -1 if the specified character is not found.</returns>
        private int GetIndexInTranslationTable(char c)
        {
            for (int i = 0; i < accentTable.Length; i++)
            {
                if (accentTable.GetAccent(i) == c) return i;
            }
            return -1;
        }

        /// <summary>
        /// Indicates whether the specified ZSCII character is in the ASCII range.
        /// </summary>
        /// <param name="zchar">The character.</param>
        /// <returns>true if the specified character is in the ASCII range; otherwise false.</returns>
        public static bool IsAscii(char zchar)
        {
            return zchar >= AsciiStart && zchar <= AsciiEnd;
        }

        /// <summary>
        /// Indicates whether the specified ZSCII character
        /// is in the special range.
        /// </summary>
        /// <param name="zchar">The character.</param>
        /// <returns>true if the specified character is in the special range; otherwise false.</returns>
        public static bool IsAccent(char zchar)
        {
            return zchar >= AccentStart && zchar <= AccentEnd;
        }

        /// <summary>
        /// Indicates whether the specified ZSCII character is a cursor key.
        /// </summary>
        /// <param name="zsciiChar">The character.</param>
        /// <returns>true if the specified character is a cursor key; otherwise false.</returns>
        public static bool IsCursorKey(char zsciiChar)
        {
            return zsciiChar >= CursorUp && zsciiChar <= CursorRight;
        }

        /// <summary>
        /// Indicates whether the specified ZSCII character is in the Unicode range.
        /// </summary>
        /// <param name="zchar">The ZSCII character.</param>
        /// <returns>true if the specified character is in the Unicode range; otherwise false.</returns>
        private static bool IsUnicode(char zchar)
        {
            return zchar >= 256;
        }

        /// <summary>
        /// Indicates whether the specified ZSCII character is a function key.
        /// </summary>
        /// <param name="zsciiChar">The ZSCII character.</param>
        /// <returns>true if the specified character is a function key; otherwise false.</returns>
        public static bool IsFunctionKey(char zsciiChar)
        {
            return (zsciiChar >= 129 && zsciiChar <= 154) || (zsciiChar >= 252 && zsciiChar <= 254);
        }

        /// <summary>
        /// Converts the specified character to lower case.
        /// </summary>
        /// <param name="zsciiChar">The ZSCII character.</param>
        /// <returns>The lower case of the ZSCII character.</returns>
        public char ToLower(char zsciiChar)
        {
            if (IsAscii(zsciiChar))
            {
                return char.ToLower(zsciiChar);
            }
            if (IsAccent(zsciiChar))
            {
                return (char)(accentTable.GetIndexOfLowerCase(zsciiChar - AccentStart) + AccentStart);
            }
            return zsciiChar;
        }
    }
}
