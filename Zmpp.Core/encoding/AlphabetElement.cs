/*
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
    /// Represents an alphabet element which is an alphabet and
    /// an index to that alphabet. 
    /// </summary>
    /// <remarks>
    /// The alphabet is needed to determine the type of encoding.
    /// </remarks>
    public class AlphabetElement
    {
        /// <summary>
        /// The Z character code or the ZSCII code if the alphabet is not set.
        /// </summary>
        private readonly char code;

        /// <summary>
        /// The alphabet.
        /// </summary>
        private readonly Alphabet alphabet;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="code">The Z character code in the alphabet or the ZSCII code if the alphabet is not set.</param>
        public AlphabetElement(Alphabet alphabet, char code)
        {
            this.alphabet = alphabet;
            this.code = code;
        }

        /// <summary>
        /// Gets the alphabet for this alphabet element.
        /// </summary>
        public Alphabet Alphabet => alphabet;

        /// <summary>
        /// Gets the code for this alphabet element.
        /// </summary>
        /// <remarks>
        /// If the alphabet is not set this is the ZSCII code and
        /// should be turned into a 10-bit code by the encoder.
        /// </remarks>
        /// <returns>The Z character code in the specified alphabet or the ZSCII code if the alphabet is not set.</returns>
        public char Code => code;
    }
}
