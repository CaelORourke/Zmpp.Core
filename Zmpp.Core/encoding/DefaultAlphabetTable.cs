/*
 * Created on 2006/01/12
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

    /// <summary>
    /// The default alphabet table implementation.
    /// </summary>
    public class DefaultAlphabetTable : AlphabetTableBase, IAlphabetTable
    {
        private const long serialVersionUID = 1L;
        private const string A0CHARS = "abcdefghijklmnopqrstuvwxyz";
        private const string A1CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string A2CHARS = " \n0123456789.,!?_#'\"/\\-:()";

        public virtual char getA0Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            return A0CHARS[zchar - ALPHABET_START];
        }

        public virtual char getA1Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            return A1CHARS[zchar - ALPHABET_START];
        }

        public virtual char getA2Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            return A2CHARS[zchar - ALPHABET_START];
        }

        public int getA0CharCode(char zsciiChar)
        {
            return getCharCodeFor(A0CHARS, zsciiChar);
        }

        public int getA1CharCode(char zsciiChar)
        {
            return getCharCodeFor(A1CHARS, zsciiChar);
        }

        public virtual int getA2CharCode(char zsciiChar)
        {
            return getCharCodeFor(A2CHARS, zsciiChar);
        }

        /// <summary>
        /// Returns the character code for the specified ZSCII character by searching
        /// the index in the specified chars string.
        /// </summary>
        /// <param name="chars">the search string</param>
        /// <param name="zsciiChar">the ZSCII character</param>
        /// <returns>the character code, which is the index of the character in chars or -1 if not found</returns>
        protected static int getCharCodeFor(String chars, char zsciiChar)
        {
            int index = chars.IndexOf(zsciiChar);
            if (index >= 0) index += ALPHABET_START;
            return index;
        }


        public virtual bool isShift1(char zchar) { return zchar == SHIFT_4; }

        public virtual bool isShift2(char zchar) { return zchar == SHIFT_5; }

        public bool isShift(char zchar)
        {
            return isShift1(zchar) || isShift2(zchar);
        }

        public virtual bool isShiftLock(char zchar) { return false; }

        public virtual bool isAbbreviation(char zchar)
        {
            return 1 <= zchar && zchar <= 3;
        }
    }
}
