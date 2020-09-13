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

namespace Zmpp.Core
{
    /// <summary>
    /// Provides read and write access to a memory map.
    /// </summary>
    public interface IMemory
    {
        #region Read access

        /// <summary>
        /// Reads the unsigned 16 bit word at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>The unsigned 16 bit value.</returns>
        char ReadUnsigned16(int address);

        /// <summary>
        /// Reads the unsigned 8 bit value at the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>The unsigned 8 bit value.</returns>
        char ReadUnsigned8(int address);

        #endregion

        #region Write access

        /// <summary>
        /// Writes an unsigned 16 bit value to the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value to write.</param>
        void WriteUnsigned16(int address, char value);

        /// <summary>
        /// Writes an unsigned byte value to the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value to write.</param>
        void WriteUnsigned8(int address, char value);

        #endregion

        /// <summary>
        /// Copies the specified number of bytes from
        /// the offset to a destination array.
        /// </summary>
        /// <param name="dstData">The destination array.</param>
        /// <param name="dstOffset">The offset in the destination array.</param>
        /// <param name="srcOffset">The offset in the source.</param>
        /// <param name="numBytes">The number of bytes to copy.</param>
        void CopyBytesToArray(byte[] dstData, int dstOffset, int srcOffset, int numBytes);

        /// <summary>
        /// Copies the specified number of bytes from the source
        /// array to this <see cref="Zmpp.Core.IMemory"/> object.
        /// </summary>
        /// <param name="srcData">The source array.</param>
        /// <param name="srcOffset">The source offset.</param>
        /// <param name="dstOffset">The destination offset.</param>
        /// <param name="numBytes">The number of bytes to copy.</param>
        void CopyBytesFromArray(byte[] srcData, int srcOffset, int dstOffset, int numBytes);

        /// <summary>
        /// Copies the specified number of bytes from the specified
        /// source <see cref="Zmpp.Core.IMemory"/> object.
        /// </summary>
        /// <param name="srcMem">The source Memory object.</param>
        /// <param name="srcOffset">The source offset.</param>
        /// <param name="dstOffset">The destination offset.</param>
        /// <param name="numBytes">The number of bytes to copy.</param>
        void CopyBytesFromMemory(IMemory srcMem, int srcOffset, int dstOffset, int numBytes);

        /// <summary>
        /// Copies an area of bytes.
        /// </summary>
        /// <param name="src">The source address.</param>
        /// <param name="dst">The destination address.</param>
        /// <param name="numBytes">The number of bytes.</param>
        void CopyArea(int src, int dst, int numBytes);
    }
}
