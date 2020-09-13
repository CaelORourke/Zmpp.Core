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
    /// <summary>
    /// Defines methods to support getting the mapped character from an alphabet table.
    /// </summary>
    public interface IAlphabetTable
    {
        /// <summary>
        /// Returns the ZSCII character from alphabet 0 at the specified index.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>The mapped character from alphabet 0.</returns>
        char GetA0Char(byte zchar);

        /// <summary>
        /// Returns the ZSCII character from alphabet 1 at the specified index.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>The mapped character from alphabet 1.</returns>
        char GetA1Char(byte zchar);

        /// <summary>
        /// Returns the ZSCII character from alphabet 2 at the specified index.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>The mapped character from alphabet 2.</returns>
        char GetA2Char(byte zchar);

        /// <summary>
        /// Returns the index of the specified ZSCII character in alphabet 0.
        /// </summary>
        /// <param name="zsciiChar">The ZSCII character.</param>
        /// <returns>The index of the character in this alphabet or -1.</returns>
        int GetA0CharCode(char zsciiChar);

        /// <summary>
        /// Returns the index of the specified ZSCII character in alphabet 2.
        /// </summary>
        /// <param name="zsciiChar">The ZSCII character.</param>
        /// <returns>The index of the character in this alphabet or -1.</returns>
        int GetA1CharCode(char zsciiChar);

        /// <summary>
        /// Returns the index of the specified ZSCII character in alphabet 2.
        /// </summary>
        /// <param name="zsciiChar">The ZSCII character.</param>
        /// <returns>The index of the character in this alphabet or -1.</returns>
        int GetA2CharCode(char zsciiChar);

        /// <summary>
        /// Determines if the specified character marks a abbreviation.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>true if the specified character is an abbreviation; otherwise false.</returns>
        bool IsAbbreviation(char zchar);

        /// <summary>
        /// Returns true if the specified character is a shift level 1 character.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>true if the specified character is a shift level 1 character; otherwise false.</returns>
        bool IsShift1(char zchar);

        /// <summary>
        /// Returns true if the specified character is a shift level 2 character.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>true if the specified character is a shift level 2 character; otherwise false.</returns>
        bool IsShift2(char zchar);

        /// <summary>
        /// Returns true if the specified character is a shift lock character.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>true if the specified character is a shift lock character; otherwise false.</returns>
        bool IsShiftLock(char zchar);

        /// <summary>
        /// Returns true if the specified character is a shift character.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>true if the specified character is a shift or shift lock character; otherwise false.</returns>
        bool IsShift(char zchar);
    }
}
