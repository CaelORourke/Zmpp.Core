﻿/*
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

namespace Zmpp.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class is the default implementation for MemoryAccess.
    /// </summary>
    public class DefaultMemory : IMemory
    {
        /// <summary>
        /// The data array containing the story file.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">the story file data</param>
        public DefaultMemory(byte[] data)
        {
            this.data = data;
        }

        public char readUnsigned16(int address)
        {
            return (char)
              (((data[address] & 0xff) << 8 | (data[address + 1] & 0xff)) & 0xffff);
        }

        public char readUnsigned8(int address)
        {
            return (char)(data[address] & 0xff);
        }

        public void writeUnsigned16(int address, char value)
        {
            data[address] = (byte)((value & 0xff00) >> 8);
            data[address + 1] = (byte)(value & 0xff);
        }

        public void writeUnsigned8(int address, char value)
        {
            data[address] = (byte)(value & 0xff);
        }

        public void copyBytesToArray(byte[] dstData, int dstOffset, int srcOffset, int numBytes)
        {
            Array.Copy(data, srcOffset, dstData, dstOffset, numBytes);
        }

        public void copyBytesFromArray(byte[] srcData, int srcOffset, int dstOffset, int numBytes)
        {
            Array.Copy(srcData, srcOffset, data, dstOffset, numBytes);
        }

        public void copyBytesFromMemory(IMemory srcMem, int srcOffset, int dstOffset, int numBytes)
        {
            // This copy method might not be as efficient, because the source
            // memory object could be based on something else than a byte array
            for (int i = 0; i < numBytes; i++)
            {
                data[dstOffset + i] = (byte)(srcMem.readUnsigned8(srcOffset + i) & 0xff);
            }
        }

        public void copyArea(int src, int dst, int numBytes)
        {
            Array.Copy(data, src, data, dst, numBytes);
        }
    }
}