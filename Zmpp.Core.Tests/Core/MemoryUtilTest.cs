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

namespace Zmpp.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Zmpp.Core;

    /// <summary>
    /// Tests for the MemoryUtil class.
    /// </summary>
    [TestClass]
    public class MemoryUtilTest
    {
        [TestMethod]
        public void ToUnsigned16()
        {
            // arrange

            // act
            var result = MemoryUtil.ToUnsigned16(1234);

            // assert
            Assert.AreEqual((char)1234, result);
        }

        [TestMethod]
        public void ReadUnsigned32()
        {
            // arrange
            byte[] data32 = { (byte)0xd7, (byte)0x4b, (byte)0xd7, (byte)0x53 };
            IMemory memory = new Memory(data32);

            // act
            var result = MemoryUtil.ReadUnsigned32(memory, 0x00);

            // assert
            Assert.AreEqual(0xd74bd753L, result);
        }

        [TestMethod]
        public void WriteUnsigned32()
        {
            // arrange
            byte[] data32 = { (byte)0xd7, (byte)0x4b, (byte)0xd7, (byte)0x53 };
            IMemory memory = new Memory(data32);

            // act
            MemoryUtil.WriteUnsigned32(memory, 0x00, 0xffffffffL);
            var result = MemoryUtil.ReadUnsigned32(memory, 0x00);
            MemoryUtil.WriteUnsigned32(memory, 0x00, 0xf0f00f0fL);
            var result2 = MemoryUtil.ReadUnsigned32(memory, 0x00);

            // assert
            Assert.AreEqual(0x00000000ffffffffL, result);
            Assert.AreEqual(0x00000000f0f00f0fL, result2);
        }

        [TestMethod]
        public void SignedToUnsigned16()
        {
            // arrange

            // act
            var result = MemoryUtil.SignedToUnsigned16((short)0);
            var result2 = MemoryUtil.SignedToUnsigned16((short)-1);
            var result3 = MemoryUtil.SignedToUnsigned16((short)-2);
            var result4 = MemoryUtil.SignedToUnsigned16((short)32767);
            var result5 = MemoryUtil.SignedToUnsigned16((short)-32768);

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual((char)0xffff, result2);
            Assert.AreEqual((char)0xfffe, result3);
            Assert.AreEqual((char)32767, result4);
            Assert.AreEqual((char)32768, result5);
        }

        [TestMethod]
        public void UnsignedToSigned16()
        {
            // arrange

            // act
            var result = MemoryUtil.UnsignedToSigned16((char)0);
            var result2 = MemoryUtil.UnsignedToSigned16((char)1);
            var result3 = MemoryUtil.UnsignedToSigned16((char)32768);
            var result4 = MemoryUtil.UnsignedToSigned16((char)32767);
            var result5 = MemoryUtil.UnsignedToSigned16((char)65535);

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, result2);
            Assert.AreEqual(-32768, result3);
            Assert.AreEqual(32767, result4);
            Assert.AreEqual(-1, result5);
        }

        [TestMethod]
        public void UnsignedToSigned8()
        {
            // arrange

            // act
            var result = MemoryUtil.UnsignedToSigned8((char)0);
            var result2 = MemoryUtil.UnsignedToSigned8((char)1);
            var result3 = MemoryUtil.UnsignedToSigned8((char)128);
            var result4 = MemoryUtil.UnsignedToSigned8((char)127);
            var result5 = MemoryUtil.UnsignedToSigned8((char)0xff);
            var result6 = MemoryUtil.UnsignedToSigned8((char)0x10ff);

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, result2);
            Assert.AreEqual(-128, result3);
            Assert.AreEqual(127, result4);
            Assert.AreEqual(-1, result5);
            Assert.AreEqual(-1, result6);
        }
    }
}
