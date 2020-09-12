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
    /// This interface manages read and write access to the byte array which contains
    /// the story file data. It is the only means to read and manipulate the
    /// memory map.
    /// </summary>
    public interface IMemory
    {
        #region Read access
        /// <summary>
        /// Reads the unsigned 16 bit word at the specified address.
        /// </summary>
        /// <param name="address">the address</param>
        /// <returns>the 16 bit unsigned value as int</returns>
        char readUnsigned16(int address);

        /// <summary>
        /// Reads the unsigned 8 bit value at the specified address.
        /// </summary>
        /// <param name="address">the address</param>
        /// <returns>the 8 bit unsigned value</returns>
        char readUnsigned8(int address);
        #endregion

        #region Write access
        /// <summary>
        /// Writes an unsigned 16 bit value to the specified address.
        /// </summary>
        /// <param name="address">the address to write to</param>
        /// <param name="value">the value to write</param>
        void writeUnsigned16(int address, char value);

        /// <summary>
        /// Writes an unsigned byte value to the specified address.
        /// </summary>
        /// <param name="address">the address to write to</param>
        /// <param name="value">the value to write</param>
        void writeUnsigned8(int address, char value);
        #endregion

        /// <summary>
        /// A rather common operation: copy the specified number of bytes from
        /// the offset to a target array.
        /// </summary>
        /// <param name="dstData">the destination array</param>
        /// <param name="dstOffset">the offset in the destination array</param>
        /// <param name="srcOffset">the offset in the source</param>
        /// <param name="numBytes">the number of bytes to copy</param>
        void copyBytesToArray(byte[] dstData, int dstOffset, int srcOffset, int numBytes);

        /// <summary>
        /// Copy the specified number of bytes from the source array to this
        /// Memory object.
        /// </summary>
        /// <param name="srcData">the source array</param>
        /// <param name="srcOffset">the source offset</param>
        /// <param name="dstOffset">the destination offset</param>
        /// <param name="numBytes">the number of bytes to copy</param>
        void copyBytesFromArray(byte[] srcData, int srcOffset, int dstOffset, int numBytes);

        /// <summary>
        /// Copy the specified number of bytes from the specified source Memory object.
        /// </summary>
        /// <param name="srcMem">the source Memory object</param>
        /// <param name="srcOffset">the source offset</param>
        /// <param name="dstOffset">the destination offset</param>
        /// <param name="numBytes">the number of bytes to copy</param>
        void copyBytesFromMemory(IMemory srcMem, int srcOffset, int dstOffset, int numBytes);

        /// <summary>
        /// Copy an area of bytes efficiently. Since the System.arraycopy() is used,
        /// we do not have to worry about overlapping areas and can take advantage
        /// of the performance gain.
        /// </summary>
        /// <param name="src">the source address</param>
        /// <param name="dst">the destination address</param>
        /// <param name="numBytes">the number of bytes</param>
        void copyArea(int src, int dst, int numBytes);
    }
}
