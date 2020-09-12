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

namespace test.zmpp.@base
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using org.zmpp.@base;

    /// <summary>
    /// Tests for the DefaultMemory class.
    /// </summary>
    [TestClass]
    public class DefaultMemoryTest
    {
        public DefaultMemoryTest()
        {
        }

        [TestMethod]
        public void testReadUnsignedByte()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            IMemory memory = new DefaultMemory(data);

            // act
            var result = memory.readUnsigned8(0x00);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void testReadUnsignedWord()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            IMemory memory = new DefaultMemory(data);

            // act
            var result = memory.readUnsigned16(0x02);

            // assert
            Assert.AreEqual(0x3709, result);
        }

        [TestMethod]
        public void testReadUnsignedShort()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            IMemory memory = new DefaultMemory(data);

            // act
            var result = memory.readUnsigned16(0x04);

            // assert
            Assert.AreEqual(0xffff, result);
            Assert.AreNotEqual(-1, result);
        }

        [TestMethod]
        public void testWriteUnsignedByte()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            IMemory memory = new DefaultMemory(data);

            // act
            memory.writeUnsigned8(0x02, (char)0xff);
            memory.writeUnsigned8(0x03, (char)0x32);
            var result = memory.readUnsigned8(0x02);
            var result2 = memory.readUnsigned8(0x03);

            // assert
            Assert.AreEqual(0xff, result);
            Assert.AreEqual(0x32, result2);
        }

        [TestMethod]
        public void testWriteUnsignedShort()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            IMemory memory = new DefaultMemory(data);

            // act
            memory.writeUnsigned16(0x02, (char)0xffff);
            memory.writeUnsigned16(0x04, (char)0x00ff);
            var result = memory.readUnsigned16(0x02);
            var result2 = memory.readUnsigned16(0x04);

            // assert
            Assert.AreEqual(0xffff, result);
            Assert.AreEqual(0x00ff, result2);
        }

        [TestMethod]
        public void testCopyBytesToArray()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            byte[] dstData = new byte[4];
            int dstOffset = 1;
            int srcOffset = 2;
            int numBytes = 3;
            IMemory memory = new DefaultMemory(data);

            // act
            memory.copyBytesToArray(dstData, dstOffset, srcOffset, numBytes);

            // assert
            Assert.AreEqual(0x37, dstData[1]);
            Assert.AreEqual(0x09, dstData[2]);
            Assert.AreEqual((byte)0xff, dstData[3]);
        }

        [TestMethod]
        public void testCopyBytesFromArray()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            byte[] srcData = { (byte)0x00, (byte)0xef, (byte)0x10, (byte)0xfe };
            int srcOffset = 1;
            int dstOffset = 0;
            int numBytes = 3;
            IMemory memory = new DefaultMemory(data);

            // act
            memory.copyBytesFromArray(srcData, srcOffset, dstOffset, numBytes);
            var result = memory.readUnsigned8(0);
            var result2 = memory.readUnsigned8(1);
            var result3 = memory.readUnsigned8(2);

            // assert
            Assert.AreEqual(0xef, result);
            Assert.AreEqual(0x10, result2);
            Assert.AreEqual(0xfe, result3);
        }

        [TestMethod]
        public void testCopyBytesFromMemory()
        {
            // arrange
            byte[] srcData = { (byte)0x00, (byte)0xef, (byte)0x10, (byte)0xfe };
            byte[] dstData = { (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00 };
            IMemory srcMem = new DefaultMemory(srcData);
            IMemory dstMem = new DefaultMemory(dstData);
            int srcOffset = 1;
            int dstOffset = 0;
            int numBytes = 3;

            // act
            dstMem.copyBytesFromMemory(srcMem, srcOffset, dstOffset, numBytes);
            var result = dstMem.readUnsigned8(0);
            var result2 = dstMem.readUnsigned8(1);
            var result3 = dstMem.readUnsigned8(2);

            // assert
            Assert.AreEqual(0xef, result);
            Assert.AreEqual(0x10, result2);
            Assert.AreEqual(0xfe, result3);
        }

        [TestMethod]
        public void testCopyArea()
        {
            // arrange
            byte[] data = { 0x03, 0x00, 0x37, 0x09, (byte)0xff, (byte)0xff };
            IMemory memory = new DefaultMemory(data);

            // act
            memory.copyArea(0, 2, 3);
            var result = memory.readUnsigned8(0);
            var result2 = memory.readUnsigned8(1);
            var result3 = memory.readUnsigned8(2);
            var result4 = memory.readUnsigned8(3);
            var result5 = memory.readUnsigned8(4);

            // assert
            Assert.AreEqual(0x03, result);
            Assert.AreEqual(0x00, result2);
            Assert.AreEqual(0x03, result3);
            Assert.AreEqual(0x00, result4);
            Assert.AreEqual(0x37, result5);
        }
    }
}
