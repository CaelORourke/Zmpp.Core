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

namespace test.zmpp.encoding
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using org.zmpp.encoding;

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
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);

            // act

            // Unknown
            var result = translator.translate((char)255);

            // alphabet 0
            var result2 = translator.translate((char)6);

            // Alphabet 1
            translator.translate((char)AlphabetTableBase.SHIFT_4);
            var result3 = translator.translate((char)8);

            // Alphabet 2
            translator.translate((char)AlphabetTableBase.SHIFT_5);
            var result4 = translator.translate((char)10);

            // Alphabet 2, NEWLINE
            translator.translate((char)AlphabetTableBase.SHIFT_5);
            var result5 = translator.translate((char)7);

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
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            var result = translator.translate((char)0);
            translator.translate((char)AlphabetTableBase.SHIFT_4);
            var result2 = translator.translate((char)0);
            translator.translate((char)AlphabetTableBase.SHIFT_5);
            var result3 = translator.translate((char)0);

            var result4 = translatorV2.translate((char)0);
            translatorV2.translate((char)AlphabetTableBase.SHIFT_4);
            var result5 = translatorV2.translate((char)0);
            translatorV2.translate((char)AlphabetTableBase.SHIFT_5);
            var result6 = translatorV2.translate((char)0);

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
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);

            // act
            var result = translator.translate((char)AlphabetTableBase.SHIFT_4);
            var result2 = translator.getCurrentAlphabet();

            translator.reset();
            var result3 = translator.getCurrentAlphabet();

            var result4 = translator.translate((char)AlphabetTableBase.SHIFT_5);
            var result5 = translator.getCurrentAlphabet();

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
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);

            // act

            // Switch to A1
            var result = translator.translate((char)AlphabetTableBase.SHIFT_4);

            var result2 = translator.translate((char)AlphabetTableBase.SHIFT_4);
            var result3 = translator.getCurrentAlphabet();

            // Switch to A1 again
            translator.reset();
            var result4 = translator.translate((char)AlphabetTableBase.SHIFT_4);

            var result5 = translator.translate((char)AlphabetTableBase.SHIFT_5);
            var result6 = translator.getCurrentAlphabet();

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
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);

            // act

            // Switch to A2
            var result = translator.translate((char)AlphabetTableBase.SHIFT_5);

            var result2 = translator.translate((char)AlphabetTableBase.SHIFT_4);
            var result3 = translator.getCurrentAlphabet();

            // Switch to A2 again
            translator.reset();
            var result4 = translator.translate((char)AlphabetTableBase.SHIFT_5);

            var result5 = translator.translate((char)AlphabetTableBase.SHIFT_5);
            var result6 = translator.getCurrentAlphabet();

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
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);

            // act

            translator.translate((char)AlphabetTableBase.SHIFT_4);
            translator.translate((char)7);
            var result = translator.getCurrentAlphabet();

            translator.translate((char)AlphabetTableBase.SHIFT_5);
            translator.translate((char)7);
            var result2 = translator.getCurrentAlphabet();

            // assert
            Assert.AreEqual(Alphabet.A0, translator.getCurrentAlphabet());
            Assert.AreEqual(Alphabet.A0, translator.getCurrentAlphabet());
        }

        [TestMethod]
        public void testGetAlphabetElement()
        {
            // arrange
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);

            // act

            // Alphabet A0
            var elem1 = translator.getAlphabetElementFor('c');
            var elem1b = translator.getAlphabetElementFor('a');
            var elem2 = translator.getAlphabetElementFor('d');

            // Alphabet A1
            var elem3 = translator.getAlphabetElementFor('C');

            // Alphabet A2
            var elem4 = translator.getAlphabetElementFor('#');

            // ZSCII code
            var elem5 = translator.getAlphabetElementFor('@');

            // Newline is tricky, this is always A2/7 !!!
            var newline = translator.getAlphabetElementFor('\n');

            // assert

            // Alphabet A0
            Assert.AreEqual(Alphabet.A0, elem1.getAlphabet());
            Assert.AreEqual(8, elem1.getZCharCode());

            Assert.AreEqual(Alphabet.A0, elem1b.getAlphabet());
            Assert.AreEqual(6, elem1b.getZCharCode());

            Assert.AreEqual(Alphabet.A0, elem2.getAlphabet());
            Assert.AreEqual(9, elem2.getZCharCode());

            // Alphabet A1
            Assert.AreEqual(Alphabet.A1, elem3.getAlphabet());
            Assert.AreEqual(8, elem3.getZCharCode());

            // Alphabet A2
            Assert.AreEqual(Alphabet.A2, elem4.getAlphabet());
            Assert.AreEqual(23, elem4.getZCharCode());

            // ZSCII code
            Assert.AreEqual(Alphabet.Unknown, elem5.getAlphabet());
            Assert.AreEqual(64, elem5.getZCharCode());

            // Newline is tricky, this is always A2/7 !!!
            Assert.AreEqual(Alphabet.A2, newline.getAlphabet());
            Assert.AreEqual(7, newline.getZCharCode());
        }

        #region Shifting in V2

        [TestMethod]
        public void testShiftV2FromA0()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            var result = translatorV2.translate((char)AlphabetTableBase.SHIFT_2);
            var result2 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();

            var result3 = translatorV2.translate((char)AlphabetTableBase.SHIFT_4);
            var result4 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();

            var result5 = translatorV2.translate((char)AlphabetTableBase.SHIFT_3);
            var result6 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();

            var result7 = translatorV2.translate((char)AlphabetTableBase.SHIFT_5);
            var result8 = translatorV2.getCurrentAlphabet();

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
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            translatorV2.translate((char)AlphabetTableBase.SHIFT_2);

            var result = translatorV2.translate((char)AlphabetTableBase.SHIFT_2);
            var result2 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();
            translatorV2.translate((char)AlphabetTableBase.SHIFT_2);

            var result3 = translatorV2.translate((char)AlphabetTableBase.SHIFT_4);
            var result4 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();
            translatorV2.translate((char)AlphabetTableBase.SHIFT_2);

            var result5 = translatorV2.translate((char)AlphabetTableBase.SHIFT_3);
            var result6 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();
            translatorV2.translate((char)AlphabetTableBase.SHIFT_2);

            var result7 = translatorV2.translate((char)AlphabetTableBase.SHIFT_5);
            var result8 = translatorV2.getCurrentAlphabet();

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
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            translatorV2.translate((char)AlphabetTableBase.SHIFT_3);

            var result = translatorV2.translate((char)AlphabetTableBase.SHIFT_2);
            var result2 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();
            translatorV2.translate((char)AlphabetTableBase.SHIFT_3);

            var result3 = translatorV2.translate((char)AlphabetTableBase.SHIFT_4);
            var result4 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();
            translatorV2.translate((char)AlphabetTableBase.SHIFT_3);

            var result5 = translatorV2.translate((char)AlphabetTableBase.SHIFT_3);
            var result6 = translatorV2.getCurrentAlphabet();
            translatorV2.reset();
            translatorV2.translate((char)AlphabetTableBase.SHIFT_3);

            var result7 = translatorV2.translate((char)AlphabetTableBase.SHIFT_5);
            var result8 = translatorV2.getCurrentAlphabet();

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
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            translatorV2.translate((char)AlphabetTableBase.SHIFT_2);
            translatorV2.translate((char)10);
            var result = translatorV2.getCurrentAlphabet();

            translatorV2.translate((char)AlphabetTableBase.SHIFT_3);
            translatorV2.translate((char)10);
            var result2 = translatorV2.getCurrentAlphabet();

            // assert
            Assert.AreEqual(Alphabet.A0, result);
            Assert.AreEqual(Alphabet.A0, result2);
        }

        [TestMethod]
        public void testShiftNotLockedChar0()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            translatorV2.translate((char)AlphabetTableBase.SHIFT_2);
            translatorV2.translate((char)0);
            var result = translatorV2.getCurrentAlphabet();

            translatorV2.translate((char)AlphabetTableBase.SHIFT_3);
            translatorV2.translate((char)0);
            var result2 = translatorV2.getCurrentAlphabet();

            // assert
            Assert.AreEqual(Alphabet.A0, result);
            Assert.AreEqual(Alphabet.A0, result2);
        }

        [TestMethod]
        public void testShiftLocked()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act

            translatorV2.translate((char)AlphabetTableBase.SHIFT_4);
            translatorV2.translate((char)10);
            var result = translatorV2.getCurrentAlphabet();
            translatorV2.reset();
            var result2 = translatorV2.getCurrentAlphabet();

            translatorV2.translate((char)AlphabetTableBase.SHIFT_5);
            translatorV2.translate((char)10);
            var result3 = translatorV2.getCurrentAlphabet();

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
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            translatorV2.translate((char)AlphabetTableBase.SHIFT_4);
            translatorV2.translate((char)AlphabetTableBase.SHIFT_2);
            translatorV2.translate((char)10);
            var result = translatorV2.getCurrentAlphabet();

            // assert
            Assert.AreEqual(Alphabet.A1, result);
        }

        [TestMethod]
        public void testShiftLockSequenceLock2()
        {
            // arrange
            IAlphabetTable alphabetTableV2 = new AlphabetTableV2();
            IZCharTranslator translatorV2 = new DefaultZCharTranslator(alphabetTableV2);

            // act
            translatorV2.translate((char)AlphabetTableBase.SHIFT_5);
            translatorV2.translate((char)AlphabetTableBase.SHIFT_3);
            translatorV2.translate((char)10);
            var result = translatorV2.getCurrentAlphabet();

            // assert
            Assert.AreEqual(Alphabet.A2, result);
        }

        #endregion
    }
}
