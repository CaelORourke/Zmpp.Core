/*
 * Created on 2005/09/23
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
    using Zmpp.Core;

    /// <summary>
    /// This interface provides decoding for the Z character encoding into
    /// the Java character system.It is important to point out that there
    /// is a difference between Z characters and the ZCSII encoding.Where
    /// ZSCII is a character set that is similar to ASCII/iso-8859-1, the
    /// Z characters are a encoded form of characters in memory that provide
    /// some degree of compression and encryption.
    /// 
    /// ZCharConverter uses the alphabet tables specified in the Z machine
    /// standards document 1.0, section 3.
    /// </summary>
    public interface IZCharDecoder
    {
        /// <summary>
        /// Performs a ZSCII decoding at the specified position of
        /// the given memory object.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The string address.</param>
        /// <param name="length">The maximum length in bytes.</param>
        /// <returns>The decoded string.</returns>
        /// <remarks>
        /// This method is exclusively designed to
        /// deal with the problems of dictionary entries.These can be cropped,
        /// leaving the string in a state, that can not be decoded properly
        /// otherwise.If the provided length is 0, the semantics are
        /// equal to the method without the length parameter.
        /// </remarks>
        String Decode2Zscii(IMemory memory, int address, int length);

        /// <summary>
        /// Returns the number of Z encoded bytes at the specified position.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The string address.</param>
        /// <returns>The number of Z encoded bytes.</returns>
        int GetNumZEncodedBytes(IMemory memory, int address);

        /// <summary>
        /// Decodes the given byte value to the specified buffer using the working
        /// alphabet.
        /// </summary>
        /// <param name="zchar">The Z encoded character, needs to be a non-shift character</param>
        /// <returns>The decoded character.</returns>
        char DecodeZChar(char zchar);

        /// <summary>
        /// Gets the translator.
        /// </summary>
        IZCharTranslator Translator { get; }
    }
}
