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
    using System;
    using System.Text;

    /// <summary>
    /// The usage of ZSCII is a little confusing, within a story file it uses
    /// alphabet tables to encode/decode it to an unreadable format, for input
    /// and output it uses a more readable encoding which resembles iso-8859-n.
    /// ZsciiEncoding therefore captures this input/output aspect of ZSCII
    /// whereas ZsciiConverter and ZsciiString handle story file encoded strings.
    /// 
    /// This class has a nonmodifiable state, so it can be shared throughout
    /// the whole application.
    /// </summary>
    public class ZsciiEncoding : ZsciiEncodingBase, IZsciiEncoding
    {
        private IAccentTable accentTable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="accentTable">the accent table.</param>
        public ZsciiEncoding(IAccentTable accentTable)
        {
            this.accentTable = accentTable;
        }

        /// <summary>
        /// Returns true if the input is a valid ZSCII character, false otherwise.
        /// </summary>
        /// <param name="zchar">a ZSCII character</param>
        /// <returns>true if valid, false otherwise</returns>
        public bool isZsciiCharacter(char zchar)
        {
            switch (zchar)
            {
                case NULL:
                case DELETE:
                case NEWLINE:
                case ESCAPE:
                    return true;
                default:
                    return isAscii(zchar) || isAccent(zchar) || isUnicodeCharacter(zchar);
            }
        }

        /// <summary>
        /// Returns true if the specified character can be converted to a ZSCII
        /// character, false otherwise.
        /// </summary>
        /// <param name="c">a unicode character</param>
        /// <returns>true if c can be converted, false, otherwise</returns>
        public bool isConvertableToZscii(char c)
        {
            return isAscii(c) || isInTranslationTable(c) || c == '\n' || c == 0 || isUnicodeCharacter(c);
        }

        /// <summary>
        /// Converts a ZSCII character to a unicode character. Will return
        /// '?' if the given character is not known.
        /// </summary>
        /// <param name="zchar">a ZSCII character.</param>
        /// <returns>the unicode representation</returns>
        public char getUnicodeChar(char zchar)
        {
            if (isAscii(zchar)) return zchar;
            if (isAccent(zchar))
            {
                int index = zchar - ACCENT_START;
                if (index < accentTable.getLength())
                {
                    return (char)accentTable.getAccent(index);
                }
            }
            if (zchar == NULL) return '\0';
            if (zchar == NEWLINE || zchar == NEWLINE_10) return '\n';
            if (isUnicodeCharacter(zchar)) return zchar;
            return '?';
        }

        /// <summary>
        /// Converts the specified string into its ZSCII representation.
        /// </summary>
        /// <param name="str">the input string</param>
        /// <returns>the ZSCII representation</returns>
        public String convertToZscii(String str)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                result.Append(getZsciiChar(str[i]));
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts the specified unicode character to a ZSCII character.
        /// Will return 0 if the character can not be converted.
        /// </summary>
        /// <param name="c">the unicode character to convert</param>
        /// <returns>the ZSCII character</returns>
        public char getZsciiChar(char c)
        {
            if (isAscii(c))
            {
                return c;
            }
            else if (isInTranslationTable(c))
            {
                return (char)(getIndexInTranslationTable(c) + ACCENT_START);
            }
            else if (c == '\n')
            {
                return NEWLINE;
            }
            return (char)0;
        }

        /// <summary>
        /// Determines whether the specified character is in the
        /// translation table.
        /// </summary>
        /// <param name="c">character</param>
        /// <returns>true if in translation table, false otherwise</returns>
        private bool isInTranslationTable(char c)
        {
            return getIndexInTranslationTable(c) >= 0;
        }

        /// <summary>
        /// Determines the index of character c in the translation
        /// table.
        /// </summary>
        /// <param name="c">character</param>
        /// <returns>index in translation table</returns>
        private int getIndexInTranslationTable(char c)
        {
            for (int i = 0; i < accentTable.getLength(); i++)
            {
                if (accentTable.getAccent(i) == c) return i;
            }
            return -1;
        }

        /// <summary>
        /// Tests the given ZSCII character if it falls in the ASCII range.
        /// </summary>
        /// <param name="zchar">the input character</param>
        /// <returns>true if in the ASCII range, false, otherwise</returns>
        public static bool isAscii(char zchar)
        {
            return zchar >= ASCII_START && zchar <= ASCII_END;
        }

        /// <summary>
        /// Tests the given ZSCII character for whether it is in the special range.
        /// </summary>
        /// <param name="zchar">the input character</param>
        /// <returns>true if in special range, false, otherwise</returns>
        public static bool isAccent(char zchar)
        {
            return zchar >= ACCENT_START && zchar <= ACCENT_END;
        }

        /// <summary>
        /// Returns true if zsciiChar is a cursor key.
        /// </summary>
        /// <param name="zsciiChar">a cursor key</param>
        /// <returns>true if cursor key, false, otherwise</returns>
        public static bool isCursorKey(char zsciiChar)
        {
            return zsciiChar >= CURSOR_UP && zsciiChar <= CURSOR_RIGHT;
        }

        /// <summary>
        /// Returns true if zchar is in the unicode range.
        /// </summary>
        /// <param name="zchar">a zscii character</param>
        /// <returns>the unicode character</returns>
        private static bool isUnicodeCharacter(char zchar)
        {
            return zchar >= 256;
        }

        /// <summary>
        /// Returns true if zsciiChar is a function key.
        /// </summary>
        /// <param name="zsciiChar">the zscii char</param>
        /// <returns>true if function key, false, otherwise</returns>
        public static bool isFunctionKey(char zsciiChar)
        {
            return (zsciiChar >= 129 && zsciiChar <= 154) || (zsciiChar >= 252 && zsciiChar <= 254);
        }

        /// <summary>
        /// Converts the character to lower case.
        /// </summary>
        /// <param name="zsciiChar">the ZSCII character to convert</param>
        /// <returns>the lower case character</returns>
        public char toLower(char zsciiChar)
        {
            if (isAscii(zsciiChar))
            {
                return char.ToLower(zsciiChar);
            }
            if (isAccent(zsciiChar))
            {
                return (char)(accentTable.getIndexOfLowerCase(zsciiChar - ACCENT_START) + ACCENT_START);
            }
            return zsciiChar;
        }
    }
}
