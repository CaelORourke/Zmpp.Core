/*
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
    /// Represents an alphabet table in a V1 story file.
    /// </summary>
    /// <remarks>
    /// Behaves like an alphabet table in V2, except that
    /// it has a different A2 alphabet and does not support
    /// abbreviations.
    /// 
    /// Furthermore, character 1 returns '\n'. This is a thing that leads
    /// to the extension of the getAnChar() methods, handling index -5.
    /// </remarks>
    public class AlphabetTableV1 : AlphabetTableV2
    {
        private const long serialVersionUID = 1L;

        /// <summary>
        /// Alphabet 2 has a slightly different structure in V1.
        /// </summary>
        private const string A2CHARS = " 0123456789.,!?_#'\"/\\<-:()";

        public override char GetA0Char(byte zchar)
        {
            if (zchar == 1) return '\n';
            return base.GetA0Char(zchar);
        }

        public override char GetA1Char(byte zchar)
        {
            if (zchar == 1) return '\n';
            return base.GetA1Char(zchar);
        }

        public override char GetA2Char(byte zchar)
        {
            if (zchar == 0) return ' ';
            if (zchar == 1) return '\n';
            return A2CHARS[zchar - AlphabetStart];
        }

        public override int GetA2CharCode(char zsciiChar)
        {
            return getCharCodeFor(A2CHARS, zsciiChar);
        }

        public override bool IsAbbreviation(char zchar) { return false; }
    }
}
