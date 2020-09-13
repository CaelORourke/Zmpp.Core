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
    /// <summary>
    /// Provides read access to an accent table.
    /// </summary>
    /// <remarks>
    /// Accent tables are used by <see cref="Zmpp.Core.Encoding.ZsciiEncoding"/>
    /// objects to translate encoded ZSCII characters to Unicode characters.
    /// </remarks>
    public interface IAccentTable
    {
        /// <summary>
        /// Gets the length of the accent table.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the accent character at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The accent character.</returns>
        char GetAccent(int index);

        /// <summary>
        /// Gets the the index of the corresponding lower case
        /// character for the accent character at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The index of the corresponding lower case character.</returns>
        int GetIndexOfLowerCase(int index);
    }
}
