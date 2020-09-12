/*
 * Created on 2008/07/19
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

namespace org.zmpp.@base
{
    using System;

    /// <summary>
    /// Utility functions for address conversion.
    /// </summary>
    public sealed class MemoryUtil
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private MemoryUtil() { }

        /// <summary>
        /// Convert an integer value to a char, which is an unsigned 16 bit value.
        /// </summary>
        /// <param name="value">the value to convert</param>
        /// <returns>the converted value</returns>
        public static char toUnsigned16(int value)
        {
            return (char)(value & 0xffff);
        }

        /// <summary>
        /// Reads the unsigned 32 bit word at the specified address.
        /// </summary>
        /// <param name="memory">the Memory object</param>
        /// <param name="address">the address</param>
        /// <returns>the 32 bit unsigned value as long</returns>
        public static long readUnsigned32(IMemory memory, int address)
        {
            long a24 = (memory.readUnsigned8(address) & 0xffL) << 24;
            long a16 = (memory.readUnsigned8(address + 1) & 0xffL) << 16;
            long a8 = (memory.readUnsigned8(address + 2) & 0xffL) << 8;
            long a0 = (memory.readUnsigned8(address + 3) & 0xffL);
            return a24 | a16 | a8 | a0;
        }

        /// <summary>
        /// Writes an unsigned 32 bit value to the specified address.
        /// </summary>
        /// <param name="memory">the Memory object</param>
        /// <param name="address">the address to write to</param>
        /// <param name="value">the value to write</param>
        public static void writeUnsigned32(IMemory memory, int address, long value)
        {
            memory.writeUnsigned8(address, (char)((value & 0xff000000) >> 24));
            memory.writeUnsigned8(address + 1, (char)((value & 0x00ff0000) >> 16));
            memory.writeUnsigned8(address + 2, (char)((value & 0x0000ff00) >> 8));
            memory.writeUnsigned8(address + 3, (char)(value & 0x000000ff));
        }

        /// <summary>
        /// Converts the specified signed 16 bit value to an unsigned 16 bit value.
        /// </summary>
        /// <param name="value">the signed value</param>
        /// <returns>the unsigned value</returns>
        public static char signedToUnsigned16(short value)
        {
            return (char)(value >= 0 ? value : char.MaxValue + (value + 1));
        }

        /// <summary>
        /// Converts the specified unsigned 16 bit value to a signed 16 bit value.
        /// </summary>
        /// <param name="value">the unsigned value</param>
        /// <returns>the signed value</returns>
        public static short unsignedToSigned16(char value)
        {
            return (short)(value > short.MaxValue ?
              -(char.MaxValue - (value - 1)) : value);
        }

        /// <summary>
        /// Converts the specified unsigned 8 bit value to a signed 8 bit value.
        /// If the value specified is actually a 16 bit value, only the lower 8 bit
        /// will be used.
        /// </summary>
        /// <param name="value">the unsigned value</param>
        /// <returns>the signed value</returns>
        public static short unsignedToSigned8(char value)
        {
            char workvalue = (char)(value & 0xff);
            return (short)(workvalue > SByte.MaxValue ?
              -(255 - (workvalue - 1)) : workvalue);
        }

    }
}
