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
    using Moq;
    using Zmpp.Core;

    /// <summary>
    /// Tests for the MemorySection class.
    /// </summary>
    [TestClass]
    public class MemorySectionTest
    {
        private const int OFFSET = 36;
        private const int LENGTH = 6;

        public MemorySectionTest()
        {
        }

        [TestMethod]
        public void testGetLength()
        {
            // arrange
            var memory = new Mock<IMemory>();
            MemorySection section = new MemorySection(memory.Object, OFFSET, LENGTH);

            // act
            var result = section.getLength();

            // assert
            Assert.AreEqual(LENGTH, result);
        }

        [TestMethod]
        public void testWriteUnsignedShort()
        {
            // arrange
            int address = 12;
            var memory = new Mock<IMemory>();
            MemorySection section = new MemorySection(memory.Object, OFFSET, LENGTH);

            // act
            section.writeUnsigned16(address, (char)512);

            // assert
            memory.Verify(m => m.writeUnsigned16(address + OFFSET, (char)512), Times.Once());
        }

        [TestMethod]
        public void testWriteUnsignedByte()
        {
            // arrange
            int address = 12;
            var memory = new Mock<IMemory>();
            MemorySection section = new MemorySection(memory.Object, OFFSET, LENGTH);

            // act
            section.writeUnsigned8(address, (char)120);

            // assert
            memory.Verify(m => m.writeUnsigned8(address + OFFSET, (char)120), Times.Once());
        }

        [TestMethod]
        public void testCopyBytesToArray()
        {
            // arrange
            byte[] dstData = new byte[5];
            int dstOffset = 2;
            int srcOffset = 3;
            int numBytes = 3;
            var memory = new Mock<IMemory>();
            MemorySection section = new MemorySection(memory.Object, OFFSET, LENGTH);

            // act
            section.copyBytesToArray(dstData, dstOffset, srcOffset, numBytes);

            // assert
            memory.Verify(m => m.copyBytesToArray(dstData, dstOffset, OFFSET + srcOffset, numBytes), Times.Once());
        }

        [TestMethod]
        public void testCopyBytesFromArray()
        {
            // arrange
            byte[] srcData = new byte[5];
            int srcOffset = 2;
            int dstOffset = 3;
            int numBytes = 3;
            var memory = new Mock<IMemory>();
            MemorySection section = new MemorySection(memory.Object, OFFSET, LENGTH);

            // act
            section.copyBytesFromArray(srcData, srcOffset, dstOffset, numBytes);

            // assert
            memory.Verify(m => m.copyBytesFromArray(srcData, srcOffset, OFFSET + dstOffset, numBytes), Times.Once());
        }

        [TestMethod]
        public void testCopyBytesFromMemory()
        {
            // arrange
            int srcOffset = 2;
            int dstOffset = 3;
            int numBytes = 3;
            var memory = new Mock<IMemory>();
            MemorySection section = new MemorySection(memory.Object, OFFSET, LENGTH);

            // act
            section.copyBytesFromMemory(memory.Object, srcOffset, dstOffset, numBytes);

            // assert
            memory.Verify(m => m.copyBytesFromMemory(memory.Object, srcOffset, OFFSET + dstOffset, numBytes), Times.Once());
        }

        [TestMethod]
        public void testCopyArea()
        {
            // arrange
            int src = 1;
            int dst = 2;
            int numBytes = 10;
            var memory = new Mock<IMemory>();
            MemorySection section = new MemorySection(memory.Object, OFFSET, LENGTH);

            // act
            section.copyArea(src, dst, numBytes);

            // assert
            memory.Verify(m => m.copyArea(OFFSET + src, OFFSET + dst, numBytes), Times.Once());
        }
    }
}
