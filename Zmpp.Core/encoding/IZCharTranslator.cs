/*
 * Created on 2006/01/15
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
    /// The Z char translator is central for Z char encoding and decoding.
    /// We provide an abstract interface, so the decoding and encoding algorithms
    /// can be based on this.
    ///
    /// It is basically an alphabet table combined with a current alphabet and
    /// depending on this state, decides, whether to shift or translate.
    /// We want to have alphabet tables as stateless information providers,
    /// so we can keep them fairly simple.
    ///
    /// Shift characters will move the object into another alphabet for the
    /// duration of one character.If the current alphabet is A2, willEscapeA2()
    /// indicates that the given character escapes to 10bit translation, the
    /// client is responsible to join those characters and the translator will
    /// not do anything about it, since it can only handle bytes.
    ///
    /// Shift lock characters are a little special: The object will remember
    /// the shift locked state until a reset() is called, if a regular shift
    /// occurs, the alphabet will be changed for one translation and will
    /// return to the last locked state.Since the translation process employs
    /// abbreviations and ZSCII-Escape-Sequences which are external to this
    /// class, the method resetToLastAlphabet() is provided to reset the state
    /// from the client after an external translation has been performed.
    /// </summary>
    public interface IZCharTranslator
    {
        /// <summary>
        /// Resets the state of the translator.
        /// </summary>
        /// <remarks>
        /// This should be called before a new decoding is started to reset
        /// this object to its initial state.
        /// </remarks>
        void Reset();

        /// <summary>
        /// This method should be invoked within the decoding of one single string.
        /// </summary>
        /// <remarks>
        /// In story file versions >= 3 this is the same as invoking Reset(). In
        /// V1 and V2 the object will reset to the last shift-locked alphabet.
        /// </remarks>
        void ResetToLastAlphabet();

        /// <summary>
        /// Clones this object. Needed, since this object has a modifiable state.
        /// </summary>
        /// <returns>a copy of this object</returns>
        Object Clone();

        /// <summary>
        /// Returns the current alphabet this object works in.
        /// </summary>
        /// <returns>the current alphabet</returns>
        Alphabet CurrentAlphabet { get; }

        /// <summary>
        /// Translates the given zchar to a Unicode character.
        /// </summary>
        /// <param name="zchar">a z encoded character</param>
        /// <returns>a Unicode character</returns>
        char Translate(char zchar);

        /// <summary>
        /// If this object is in alphabet A2 now, this function determines if the
        /// given character is an A2 escape.
        /// </summary>
        /// <param name="zchar">the character</param>
        /// <returns>true if A2 escape, false otherwise</returns>
        bool WillEscapeA2(char zchar);

        /// <summary>
        /// Return true if this the specified character is an abbreviation in the
        /// current alphabet table.
        /// </summary>
        /// <param name="zchar">a Z encoded character</param>
        /// <returns>true if abbreviation, false otherwise</returns>
        bool IsAbbreviation(char zchar);

        /// <summary>
        /// Provides a reverse translation. Given a ZSCII character, determine
        /// the alphabet and the index to this alphabet.If alphabet in the
        /// result is null, this is a plain ZSCII character.
        /// </summary>
        /// <param name="zsciiChar">a ZSCII character</param>
        /// <returns>the reverse translation</returns>
        AlphabetElement GetAlphabetElementFor(char zsciiChar);
    }
}
