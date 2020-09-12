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

namespace Zmpp.Core.Encoding.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Zmpp.Core;
    using Zmpp.Core.Encoding;

    /// <summary>
    /// Tests for the CustomAccentTable class.
    /// </summary>
    [TestClass]
    public class CustomAccentTableTest
    {
        private const int ADDRESS = 4711;

        public CustomAccentTableTest()
        {
        }

        [TestMethod]
        public void testGetLengthNoTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            CustomAccentTable noAccentTable = new CustomAccentTable(memory.Object, 0);

            // act
            var result = noAccentTable.getLength();

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void testGetLength()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(ADDRESS)).Returns((char)3);
            CustomAccentTable accentTable = new CustomAccentTable(memory.Object, ADDRESS);

            // act
            var result = accentTable.getLength();

            // assert
            memory.Verify(m => m.ReadUnsigned8(ADDRESS), Times.Once());
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void testGetAccentNoTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            CustomAccentTable noAccentTable = new CustomAccentTable(memory.Object, 0);

            // act
            var result = noAccentTable.getAccent(42);

            // assert
            Assert.AreEqual('?', result);
        }

        [TestMethod]
        public void testGetAccent()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(ADDRESS + 7)).Returns('^');
            CustomAccentTable accentTable = new CustomAccentTable(memory.Object, ADDRESS);

            // act
            var result = accentTable.getAccent(3);

            // assert
            memory.Verify(m => m.ReadUnsigned16(ADDRESS + 7), Times.Once());
            Assert.AreEqual('^', result);
        }

        [TestMethod]
        public void testGetIndexOfLowerCase()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(ADDRESS)).Returns((char)80);
            memory.Setup(m => m.ReadUnsigned16(ADDRESS + 2 * 6 + 1)).Returns('B');
            memory.Setup(m => m.ReadUnsigned16(ADDRESS + 1)).Returns('a');
            memory.Setup(m => m.ReadUnsigned16(ADDRESS + 2 + 1)).Returns('b');
            CustomAccentTable accentTable = new CustomAccentTable(memory.Object, ADDRESS);

            // act
            var result = accentTable.getIndexOfLowerCase(6);

            // assert
            memory.Verify(m => m.ReadUnsigned8(ADDRESS), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(ADDRESS + 2 * 6 + 1), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(ADDRESS + 1), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(ADDRESS + 2 + 1), Times.Once());
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void testGetIndexOfLowerCaseNotFound()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(ADDRESS)).Returns((char)2);
            memory.Setup(m => m.ReadUnsigned16(ADDRESS + 2 * 1 + 1)).Returns('^');
            memory.Setup(m => m.ReadUnsigned16(ADDRESS + 1)).Returns('a');
            CustomAccentTable accentTable = new CustomAccentTable(memory.Object, ADDRESS);

            // act
            var result = accentTable.getIndexOfLowerCase(1);

            // assert
            memory.Verify(m => m.ReadUnsigned8(ADDRESS), Times.AtLeastOnce());
            memory.Verify(m => m.ReadUnsigned16(ADDRESS + 2 * 1 + 1), Times.AtLeastOnce());
            memory.Verify(m => m.ReadUnsigned16(ADDRESS + 1), Times.Once());
            Assert.AreEqual(1, result);
        }
    }
}
