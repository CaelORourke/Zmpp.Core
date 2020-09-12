﻿/*
 * Created on 2006/01/17
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
    /// <summary>
    /// An alphabet table in V1 story files behaves like an alphabet table in
    /// V2, except that it has a different A2 alphabet and does not support
    /// abbreviations.
    /// Furthermore, character 1 returns '\n'. This is a thing that leads
    /// to the extension of the getAnChar() methods, handling index -5.
    /// </summary>
    public class AlphabetTableV1 : AlphabetTableV2
    {
        private const long serialVersionUID = 1L;

        /// <summary>
        /// V1 Alphabet 2 has a slightly different structure.
        /// </summary>
        private const string A2CHARS = " 0123456789.,!?_#'\"/\\<-:()";

        public override char getA0Char(byte zchar)
        {
            if (zchar == 1) return '\n';
            return base.getA0Char(zchar);
        }

        public override char getA1Char(byte zchar)
        {
            if (zchar == 1) return '\n';
            return base.getA1Char(zchar);
        }

        public override char getA2Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            if (zchar == 1) return '\n';
            return A2CHARS[zchar - ALPHABET_START];
        }

        public override int getA2CharCode(char zsciiChar)
        {
            return getCharCodeFor(A2CHARS, zsciiChar);
        }

        public override bool isAbbreviation(char zchar) { return false; }
    }
}
