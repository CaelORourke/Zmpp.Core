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

namespace Zmpp.Core.Encoding.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Zmpp.Core.Encoding;

    /// <summary>
    /// Test for alphabet table behaviour.
    /// </summary>
    [TestClass]
    public class AlphabetTableTest
    {
        public AlphabetTableTest()
        {
        }

        [TestMethod]
        public void Char0IsSpace()
        {
            // arrange
            IAlphabetTable v1Table = new AlphabetTableV1();
            IAlphabetTable v2Table = new AlphabetTableV2();
            IAlphabetTable defaultTable = new AlphabetTable();

            // act
            var result = v1Table.GetA0Char((byte)0);
            var result2 = v1Table.GetA1Char((byte)0);
            var result3 = v1Table.GetA2Char((byte)0);

            var result4 = v2Table.GetA0Char((byte)0);
            var result5 = v2Table.GetA1Char((byte)0);
            var result6 = v2Table.GetA2Char((byte)0);

            var result7 = defaultTable.GetA0Char((byte)0);
            var result8 = defaultTable.GetA1Char((byte)0);
            var result9 = defaultTable.GetA2Char((byte)0);

            // assert
            Assert.AreEqual(' ', result);
            Assert.AreEqual(' ', result2);
            Assert.AreEqual(' ', result3);

            Assert.AreEqual(' ', result4);
            Assert.AreEqual(' ', result5);
            Assert.AreEqual(' ', result6);

            Assert.AreEqual(' ', result7);
            Assert.AreEqual(' ', result8);
            Assert.AreEqual(' ', result9);
        }

        [TestMethod]
        public void Char1IsNewLineInV1()
        {
            // arrange
            IAlphabetTable v1Table = new AlphabetTableV1();

            // act
            var result = v1Table.GetA0Char((byte)1);
            var result2 = v1Table.GetA1Char((byte)1);
            var result3 = v1Table.GetA2Char((byte)1);

            // assert
            Assert.AreEqual('\n', result);
            Assert.AreEqual('\n', result2);
            Assert.AreEqual('\n', result3);
        }

        [TestMethod]
        public void IsAbbreviation()
        {
            // arrange
            IAlphabetTable v1Table = new AlphabetTableV1();
            IAlphabetTable v2Table = new AlphabetTableV2();

            // act
            var result = v1Table.IsAbbreviation((char)1);
            var result2 = v1Table.IsAbbreviation((char)2);
            var result3 = v1Table.IsAbbreviation((char)3);

            var result4 = v2Table.IsAbbreviation((char)1);
            var result5 = v2Table.IsAbbreviation((char)2);
            var result6 = v2Table.IsAbbreviation((char)3);

            // assert
            Assert.IsFalse(result);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);

            Assert.IsTrue(result4);
            Assert.IsFalse(result5);
            Assert.IsFalse(result6);
        }

        [TestMethod]
        public void ShiftChars()
        {
            // arrange
            IAlphabetTable v1Table = new AlphabetTableV1();
            IAlphabetTable v2Table = new AlphabetTableV2();
            IAlphabetTable defaultTable = new AlphabetTable();

            // act
            var result = v1Table.IsShift((char)AlphabetTableBase.Shift2);
            var result2 = v1Table.IsShift((char)AlphabetTableBase.Shift3);
            var result3 = v2Table.IsShift((char)AlphabetTableBase.Shift2);
            var result4 = v2Table.IsShift((char)AlphabetTableBase.Shift3);
            var result5 = v1Table.IsShiftLock((char)AlphabetTableBase.Shift2);
            var result6 = v1Table.IsShiftLock((char)AlphabetTableBase.Shift3);
            var result7 = v2Table.IsShiftLock((char)AlphabetTableBase.Shift2);
            var result8 = v2Table.IsShiftLock((char)AlphabetTableBase.Shift3);

            var result9 = v1Table.IsShiftLock((char)AlphabetTableBase.Shift2);
            var result10 = v1Table.IsShiftLock((char)AlphabetTableBase.Shift3);
            var result11 = v2Table.IsShiftLock((char)AlphabetTableBase.Shift2);
            var result12 = v2Table.IsShiftLock((char)AlphabetTableBase.Shift3);
            var result13 = v1Table.IsShiftLock((char)AlphabetTableBase.Shift4);
            var result14 = v1Table.IsShiftLock((char)AlphabetTableBase.Shift5);
            var result15 = v2Table.IsShiftLock((char)AlphabetTableBase.Shift4);
            var result16 = v2Table.IsShiftLock((char)AlphabetTableBase.Shift5);

            var result17 = defaultTable.IsShift((char)AlphabetTableBase.Shift2);
            var result18 = defaultTable.IsShift((char)AlphabetTableBase.Shift3);
            var result19 = defaultTable.IsShift((char)AlphabetTableBase.Shift4);
            var result20 = defaultTable.IsShift((char)AlphabetTableBase.Shift5);
            var result21 = defaultTable.IsShiftLock((char)AlphabetTableBase.Shift2);
            var result22 = defaultTable.IsShiftLock((char)AlphabetTableBase.Shift3);
            var result23 = defaultTable.IsShiftLock((char)AlphabetTableBase.Shift4);
            var result24 = defaultTable.IsShiftLock((char)AlphabetTableBase.Shift5);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            Assert.IsFalse(result5);
            Assert.IsFalse(result6);
            Assert.IsFalse(result7);
            Assert.IsFalse(result8);

            Assert.IsFalse(result9);
            Assert.IsFalse(result10);
            Assert.IsFalse(result11);
            Assert.IsFalse(result12);
            Assert.IsTrue(result13);
            Assert.IsTrue(result14);
            Assert.IsTrue(result15);
            Assert.IsTrue(result16);

            Assert.IsFalse(result17);
            Assert.IsFalse(result18);
            Assert.IsTrue(result19);
            Assert.IsTrue(result20);
            Assert.IsFalse(result21);
            Assert.IsFalse(result22);
            Assert.IsFalse(result23);
            Assert.IsFalse(result24);
        }
    }
}
