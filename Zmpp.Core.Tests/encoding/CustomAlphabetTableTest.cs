﻿/*
 * Created on 2006/01/17
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
    /// Tests for the CustomAlphabetTable class.
    /// </summary>
    [TestClass]
    public class CustomAlphabetTableTest
    {
        public CustomAlphabetTableTest()
        {
        }

        [TestMethod]
        public void testGetA0Char()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(1000)).Returns((char)3);
            memory.Setup(m => m.readUnsigned8(1006)).Returns((char)2);
            CustomAlphabetTable alphabetTable = new CustomAlphabetTable(memory.Object, 1000);

            // act
            var result = alphabetTable.getA0Char((byte)6);
            var result2 = alphabetTable.getA0Char((byte)12);
            var result3 = alphabetTable.getA0Char((byte)0);

            // assert
            memory.Verify(m => m.readUnsigned8(1000), Times.Once());
            memory.Verify(m => m.readUnsigned8(1006), Times.Once());
            Assert.AreEqual(3, result);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(' ', result3);
        }

        [TestMethod]
        public void testGetA1Char()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(1026)).Returns((char)3);
            memory.Setup(m => m.readUnsigned8(1032)).Returns((char)2);
            CustomAlphabetTable alphabetTable = new CustomAlphabetTable(memory.Object, 1000);

            // act
            var result = alphabetTable.getA1Char((byte)6);
            var result2 = alphabetTable.getA1Char((byte)12);
            var result3 = alphabetTable.getA1Char((byte)0);

            // assert
            memory.Verify(m => m.readUnsigned8(1026), Times.Once());
            memory.Verify(m => m.readUnsigned8(1032), Times.Once());
            Assert.AreEqual(3, result);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(' ', result3);
        }

        [TestMethod]
        public void testGetA2Char()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(1052)).Returns((char)3);
            memory.Setup(m => m.readUnsigned8(1058)).Returns((char)2);
            CustomAlphabetTable alphabetTable = new CustomAlphabetTable(memory.Object, 1000);

            // act
            var result = alphabetTable.getA2Char((byte)6);
            var result2 = alphabetTable.getA2Char((byte)12);
            var result3 = alphabetTable.getA2Char((byte)0);
            var result4 = alphabetTable.getA2Char((byte)7);

            // assert
            memory.Verify(m => m.readUnsigned8(1052), Times.Once());
            memory.Verify(m => m.readUnsigned8(1058), Times.Once());
            Assert.AreEqual(3, result);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(' ', result3);
            Assert.AreEqual('\n', result4);
        }

        [TestMethod]
        public void testA0IndexOfNotFound()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(It.IsInRange<int>(1000, 1026, Range.Inclusive))).Returns('a');
            CustomAlphabetTable alphabetTable = new CustomAlphabetTable(memory.Object, 1000);

            // act
            var result = alphabetTable.getA0CharCode('@');

            // assert
            memory.Verify(m => m.readUnsigned8(It.IsInRange<int>(1000, 1026, Range.Inclusive)), Times.AtLeastOnce());
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void testA1IndexOfNotFound()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(It.IsInRange<int>(1026, 1052, Range.Inclusive))).Returns('a');
            CustomAlphabetTable alphabetTable = new CustomAlphabetTable(memory.Object, 1000);

            // act
            var result = alphabetTable.getA1CharCode('@');

            // assert
            memory.Verify(m => m.readUnsigned8(It.IsInRange<int>(1026, 1052, Range.Inclusive)), Times.AtLeastOnce());
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void testA2IndexOfNotFound()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(It.Is<int>(i => i >= 1052 && i <= 1078 && i % 2 == 0))).Returns('a');
            CustomAlphabetTable alphabetTable = new CustomAlphabetTable(memory.Object, 1000);

            // act
            var result = alphabetTable.getA2CharCode('@');

            // assert
            memory.Verify(m => m.readUnsigned8(It.Is<int>(i => i >= 1052 && i <= 1078 && i % 2 == 0)), Times.AtLeastOnce());
            Assert.AreEqual(-1, result);
        }
    }
}
