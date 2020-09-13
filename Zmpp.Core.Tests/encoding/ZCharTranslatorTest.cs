/*
 * Copyright 2005-2009 by Wei-ju Wu
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
    /// Tests for the ZCharTranslator class.
    /// </summary>
    [TestClass]
    public class ZCharTranslatorTest
    {
        public ZCharTranslatorTest()
        {
        }

        [TestMethod]
        public void testTranslate()
        {
            // arrange
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);

            // act

            // Unknown
            var result = translator.Translate((char)255);

            // alphabet 0
            var result2 = translator.Translate((char)6);

            // Alphabet 1
            translator.Translate((char)AlphabetTableBase.Shift4);
            var result3 = translator.Translate((char)8);

            // Alphabet 2
            translator.Translate((char)AlphabetTableBase.Shift5);
            var result4 = translator.Translate((char)10);

            // Alphabet 2, NEWLINE
            translator.Translate((char)AlphabetTableBase.Shift5);
            var result5 = translator.Translate((char)7);

            // assert
            Assert.AreEqual('?', result); // Unknown
            Assert.AreEqual('a', result2); // alphabet 0
            Assert.AreEqual('C', result3); // Alphabet 1
            Assert.AreEqual('2', result4); // Alphabet 2
            Assert.AreEqual('\n', result5); // Alphabet 2, NEWLINE
        }

        [TestMethod]
        public void test0IsSpace()
        {
            // arrange
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            var result = translator.Translate((char)0);
            translator.Translate((char)AlphabetTableBase.Shift4);
            var result2 = translator.Translate((char)0);
            translator.Translate((char)AlphabetTableBase.Shift5);
            var result3 = translator.Translate((char)0);

            var result4 = translatorV2.Translate((char)0);
            translatorV2.Translate((char)AlphabetTableBase.Shift4);
            var result5 = translatorV2.Translate((char)0);
            translatorV2.Translate((char)AlphabetTableBase.Shift5);
            var result6 = translatorV2.Translate((char)0);

            // assert
            Assert.AreEqual(' ', result);
            Assert.AreEqual(' ', result2);
            Assert.AreEqual(' ', result3);

            Assert.AreEqual(' ', result4);
            Assert.AreEqual(' ', result5);
            Assert.AreEqual(' ', result6);
        }

        #region Shift

        [TestMethod]
        public void testShiftFromA0()
        {
            // arrange
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);

            // act
            var result = translator.Translate((char)AlphabetTableBase.Shift4);
            var result2 = translator.CurrentAlphabet;

            translator.Reset();
            var result3 = translator.CurrentAlphabet;

            var result4 = translator.Translate((char)AlphabetTableBase.Shift5);
            var result5 = translator.CurrentAlphabet;

            // assert
            Assert.AreEqual('\0', result);
            Assert.AreEqual(Alphabet.A1, result2);

            Assert.AreEqual(Alphabet.A0, result3);

            Assert.AreEqual('\0', result4);
            Assert.AreEqual(Alphabet.A2, result5);
        }

        [TestMethod]
        public void testShiftFromA1()
        {
            // arrange
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);

            // act

            // Switch to A1
            var result = translator.Translate((char)AlphabetTableBase.Shift4);

            var result2 = translator.Translate((char)AlphabetTableBase.Shift4);
            var result3 = translator.CurrentAlphabet;

            // Switch to A1 again
            translator.Reset();
            var result4 = translator.Translate((char)AlphabetTableBase.Shift4);

            var result5 = translator.Translate((char)AlphabetTableBase.Shift5);
            var result6 = translator.CurrentAlphabet;

            // assert

            // Switch to A1
            Assert.AreEqual('\0', result2);
            Assert.AreEqual(Alphabet.A2, result3);

            // Switch to A1 again
            Assert.AreEqual('\0', result5);
            Assert.AreEqual(Alphabet.A0, result6);
        }

        [TestMethod]
        public void testShiftFromA2()
        {
            // arrange
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);

            // act

            // Switch to A2
            var result = translator.Translate((char)AlphabetTableBase.Shift5);

            var result2 = translator.Translate((char)AlphabetTableBase.Shift4);
            var result3 = translator.CurrentAlphabet;

            // Switch to A2 again
            translator.Reset();
            var result4 = translator.Translate((char)AlphabetTableBase.Shift5);

            var result5 = translator.Translate((char)AlphabetTableBase.Shift5);
            var result6 = translator.CurrentAlphabet;

            // assert

            // Switch to A2
            Assert.AreEqual('\0', result2);
            Assert.AreEqual(Alphabet.A0, result3);

            // Switch to A2 again
            Assert.AreEqual('\0', result5);
            Assert.AreEqual(Alphabet.A1, result6);
        }

        #endregion

        /// <summary>
        /// The default alphabet table should reset to A0 after retrieving a
        /// code.
        /// </summary>
        [TestMethod]
        public void testImplicitReset()
        {
            // arrange
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);

            // act

            translator.Translate((char)AlphabetTableBase.Shift4);
            translator.Translate((char)7);
            var result = translator.CurrentAlphabet;

            translator.Translate((char)AlphabetTableBase.Shift5);
            translator.Translate((char)7);
            var result2 = translator.CurrentAlphabet;

            // assert
            Assert.AreEqual(Alphabet.A0, translator.CurrentAlphabet);
            Assert.AreEqual(Alphabet.A0, translator.CurrentAlphabet);
        }

        [TestMethod]
        public void testGetAlphabetElement()
        {
            // arrange
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);

            // act

            // Alphabet A0
            var elem1 = translator.GetAlphabetElementFor('c');
            var elem1b = translator.GetAlphabetElementFor('a');
            var elem2 = translator.GetAlphabetElementFor('d');

            // Alphabet A1
            var elem3 = translator.GetAlphabetElementFor('C');

            // Alphabet A2
            var elem4 = translator.GetAlphabetElementFor('#');

            // ZSCII code
            var elem5 = translator.GetAlphabetElementFor('@');

            // Newline is tricky, this is always A2/7 !!!
            var newline = translator.GetAlphabetElementFor('\n');

            // assert

            // Alphabet A0
            Assert.AreEqual(Alphabet.A0, elem1.Alphabet);
            Assert.AreEqual(8, elem1.Code);

            Assert.AreEqual(Alphabet.A0, elem1b.Alphabet);
            Assert.AreEqual(6, elem1b.Code);

            Assert.AreEqual(Alphabet.A0, elem2.Alphabet);
            Assert.AreEqual(9, elem2.Code);

            // Alphabet A1
            Assert.AreEqual(Alphabet.A1, elem3.Alphabet);
            Assert.AreEqual(8, elem3.Code);

            // Alphabet A2
            Assert.AreEqual(Alphabet.A2, elem4.Alphabet);
            Assert.AreEqual(23, elem4.Code);

            // ZSCII code
            Assert.AreEqual(Alphabet.NotSet, elem5.Alphabet);
            Assert.AreEqual(64, elem5.Code);

            // Newline is tricky, this is always A2/7 !!!
            Assert.AreEqual(Alphabet.A2, newline.Alphabet);
            Assert.AreEqual(7, newline.Code);
        }

        #region Shifting in V2

        [TestMethod]
        public void testShiftV2FromA0()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            var result = translatorV2.Translate((char)AlphabetTableBase.Shift2);
            var result2 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();

            var result3 = translatorV2.Translate((char)AlphabetTableBase.Shift4);
            var result4 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();

            var result5 = translatorV2.Translate((char)AlphabetTableBase.Shift3);
            var result6 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();

            var result7 = translatorV2.Translate((char)AlphabetTableBase.Shift5);
            var result8 = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(Alphabet.A1, result2);

            Assert.AreEqual(0, result3);
            Assert.AreEqual(Alphabet.A1, result4);

            Assert.AreEqual(0, result5);
            Assert.AreEqual(Alphabet.A2, result6);

            Assert.AreEqual(0, result7);
            Assert.AreEqual(Alphabet.A2, result8);
        }

        [TestMethod]
        public void testShiftV2FromA1()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            translatorV2.Translate((char)AlphabetTableBase.Shift2);

            var result = translatorV2.Translate((char)AlphabetTableBase.Shift2);
            var result2 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();
            translatorV2.Translate((char)AlphabetTableBase.Shift2);

            var result3 = translatorV2.Translate((char)AlphabetTableBase.Shift4);
            var result4 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();
            translatorV2.Translate((char)AlphabetTableBase.Shift2);

            var result5 = translatorV2.Translate((char)AlphabetTableBase.Shift3);
            var result6 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();
            translatorV2.Translate((char)AlphabetTableBase.Shift2);

            var result7 = translatorV2.Translate((char)AlphabetTableBase.Shift5);
            var result8 = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(Alphabet.A2, result2);

            Assert.AreEqual(0, result3);
            Assert.AreEqual(Alphabet.A2, result4);

            Assert.AreEqual(0, result5);
            Assert.AreEqual(Alphabet.A0, result6);

            Assert.AreEqual(0, result7);
            Assert.AreEqual(Alphabet.A0, result8);
        }

        [TestMethod]
        public void testShiftV2FromA2()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            translatorV2.Translate((char)AlphabetTableBase.Shift3);

            var result = translatorV2.Translate((char)AlphabetTableBase.Shift2);
            var result2 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();
            translatorV2.Translate((char)AlphabetTableBase.Shift3);

            var result3 = translatorV2.Translate((char)AlphabetTableBase.Shift4);
            var result4 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();
            translatorV2.Translate((char)AlphabetTableBase.Shift3);

            var result5 = translatorV2.Translate((char)AlphabetTableBase.Shift3);
            var result6 = translatorV2.CurrentAlphabet;
            translatorV2.Reset();
            translatorV2.Translate((char)AlphabetTableBase.Shift3);

            var result7 = translatorV2.Translate((char)AlphabetTableBase.Shift5);
            var result8 = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(Alphabet.A0, result2);

            Assert.AreEqual(0, result3);
            Assert.AreEqual(Alphabet.A0, result4);

            Assert.AreEqual(0, result5);
            Assert.AreEqual(Alphabet.A1, result6);

            Assert.AreEqual(0, result7);
            Assert.AreEqual(Alphabet.A1, result8);
        }

        [TestMethod]
        public void testShiftNotLocked()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            translatorV2.Translate((char)AlphabetTableBase.Shift2);
            translatorV2.Translate((char)10);
            var result = translatorV2.CurrentAlphabet;

            translatorV2.Translate((char)AlphabetTableBase.Shift3);
            translatorV2.Translate((char)10);
            var result2 = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(Alphabet.A0, result);
            Assert.AreEqual(Alphabet.A0, result2);
        }

        [TestMethod]
        public void testShiftNotLockedChar0()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            translatorV2.Translate((char)AlphabetTableBase.Shift2);
            translatorV2.Translate((char)0);
            var result = translatorV2.CurrentAlphabet;

            translatorV2.Translate((char)AlphabetTableBase.Shift3);
            translatorV2.Translate((char)0);
            var result2 = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(Alphabet.A0, result);
            Assert.AreEqual(Alphabet.A0, result2);
        }

        [TestMethod]
        public void testShiftLocked()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act

            translatorV2.Translate((char)AlphabetTableBase.Shift4);
            translatorV2.Translate((char)10);
            var result = translatorV2.CurrentAlphabet;
            translatorV2.Reset();
            var result2 = translatorV2.CurrentAlphabet;

            translatorV2.Translate((char)AlphabetTableBase.Shift5);
            translatorV2.Translate((char)10);
            var result3 = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(Alphabet.A1, result);
            Assert.AreEqual(Alphabet.A0, result2);
            Assert.AreEqual(Alphabet.A2, result3);
        }

        /// <summary>
        /// Test if the shift lock is reset after the a non-locking shift was
        /// met.
        /// </summary>
        [TestMethod]
        public void testShiftLockSequenceLock1()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            translatorV2.Translate((char)AlphabetTableBase.Shift4);
            translatorV2.Translate((char)AlphabetTableBase.Shift2);
            translatorV2.Translate((char)10);
            var result = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(Alphabet.A1, result);
        }

        [TestMethod]
        public void testShiftLockSequenceLock2()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new ZCharTranslator(alphabetTableV2);

            // act
            translatorV2.Translate((char)AlphabetTableBase.Shift5);
            translatorV2.Translate((char)AlphabetTableBase.Shift3);
            translatorV2.Translate((char)10);
            var result = translatorV2.CurrentAlphabet;

            // assert
            Assert.AreEqual(Alphabet.A2, result);
        }

        #endregion
    }
}
