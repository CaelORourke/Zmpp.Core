/*
 * Created on 2006/01/10
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
    using org.zmpp.@base;
    using org.zmpp.encoding;
    using org.zmpp.vm;

    /// <summary>
    /// This is the test for the encoder class. In general it is not recommended to
    /// rely on existing classes for tests, but mocks are not so practical
    /// in this case either.We will use a decoder and a DefaultMemoryAccess
    /// object to test our implementation instead of mocking these classes.
    /// By this means we can instantly verify our result in an easy way.
    /// </summary>
    [TestClass]
    public class ZCharEncoderTest
    {
        public ZCharEncoderTest()
        {
        }

        /// <summary>
        /// A single character to be encoded. We need to make sure it is in lower
        /// case and the string is padded out with shift characters.
        /// </summary>
        [TestMethod]
        public void testEncodeSingleCharacter()
        {
            // arrange
            int length = 1;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            // we expect to have an end word, padded out with shift 5's
            data[sourceAddress] = (byte)'a'; // Encode an 'a'

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18a5, result); // 'a' + 2 pad

            // Test that the rest is padded and marked with the end bit
            Assert.AreEqual(0x14a5, result2);
            Assert.AreEqual(0x94a5, result3);
        }

        [TestMethod]
        public void testEncodeTwoCharacters()
        {
            // arrange
            int length = 2;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'a';
            data[sourceAddress + 1] = (byte)'b';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e5, result); // 'ab' + pad

            // Test that the rest is padded and marked with the end bit
            Assert.AreEqual(0x14a5, result2);
            Assert.AreEqual(0x94a5, result3);
        }

        [TestMethod]
        public void testEncode4Characters()
        {
            // arrange
            int length = 4;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'a';
            data[sourceAddress + 1] = (byte)'b';
            data[sourceAddress + 2] = (byte)'c';
            data[sourceAddress + 3] = (byte)'d';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e8, result); // 'abc'
            Assert.AreEqual(0x24a5, result2); // 'd' + 2 pads
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// Test with a different alphabet
        /// </summary>
        [TestMethod]
        public void testEncodeAlphabet1()
        {
            // arrange
            int length = 1;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'A';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x10c5, result); // Shift-4 + 'a' + Pad

            // Test that the rest is padded and marked with the end bit
            Assert.AreEqual(0x14a5, result2);
            Assert.AreEqual(0x94a5, result3);
        }

        [TestMethod]
        public void testEncodeAlphabet1SpanWordBound()
        {
            // arrange
            int length = 3;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'a';
            data[sourceAddress + 1] = (byte)'b';
            data[sourceAddress + 2] = (byte)'C';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e4, result); // 'ab' + Shift 4
            Assert.AreEqual(0x20a5, result2); // 'c'
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        [TestMethod]
        public void testEncodeAlphabet2SpanWordBound()
        {
            // arrange
            int length = 3;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'a';
            data[sourceAddress + 1] = (byte)'b';
            data[sourceAddress + 2] = (byte)'3';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e5, result); // 'ab' + Shift 5
            Assert.AreEqual(0x2ca5, result2); // '3'
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// Encoding of special characters in the unicode has to work.
        /// We test this on our favorite character: '@'
        /// Do not forget the testing across word boundaries
        ///
        /// How are characters handled that are larger than a byte ?
        /// See how Frotz handles this
        /// </summary>
        [TestMethod]
        public void testEncodeEscapeA2()
        {
            // arrange
            int length = 1;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'@';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Tricky, tricky (and memory-inefficient)
            // Shift-5 + 6 + '@' (64), encoded in 10 bit, the upper half contains 2
            Assert.AreEqual(0x14c2, result);
            Assert.AreEqual(0x00a5, result2); // the lower half contains 0 + 2 pads
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// For triangulation, we use another character (126)
        /// </summary>
        [TestMethod]
        public void testEncodeEscapeA2Tilde()
        {
            // arrange
            int length = 1;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'~';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Tricky, tricky (and memory-inefficient)
            // Shift-5 + 6 + '~' (126), encoded in 10 bit, the upper half contains 3
            Assert.AreEqual(0x14c3, result);
            Assert.AreEqual(0x78a5, result2); // the lower half contains 30 + 2 pads
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        [TestMethod]
        public void testEncodeEscapeA2TildeSpansWord()
        {
            // arrange
            int length = 2;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            data[sourceAddress] = (byte)'a';
            data[sourceAddress + 1] = (byte)'~';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Tricky, tricky (and memory-inefficient)
            // 'a' + Shift-5 + 6
            Assert.AreEqual(0x18a6, result);
            Assert.AreEqual(0x0fc5, result2); // both halfs of '~' + 1 pad
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// We test a situation where the 6 bytes are exceeded by the 9 source
        /// characters. In practice, this happens, when there are characters
        /// in the source buffer that need to be escaped, since they take the
        /// space of 4 lower case characters, which means that one special character
        /// can be combined with 5 lower case characters
        /// </summary>
        [TestMethod]
        public void testEncodeCharExceedsTargetBuffer()
        {
            // arrange
            int length = 7;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            // Situation 1: there are lower case letters at the end, we need
            // to ensure that the dictionary is cropped and the characters
            // that exceed the buffer are ommitted
            data[sourceAddress] = (byte)'@';
            data[sourceAddress + 1] = (byte)'a';
            data[sourceAddress + 2] = (byte)'b';
            data[sourceAddress + 3] = (byte)'c';
            data[sourceAddress + 4] = (byte)'d';
            data[sourceAddress + 5] = (byte)'e';
            data[sourceAddress + 6] = (byte)'f';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Shift-5 + 6 + '@' (64), encoded in 10 bit, the upper half contains 2
            Assert.AreEqual(0x14c2, result);
            Assert.AreEqual(0x00c7, result2); // the lower half contains 0, 'ab'
            Assert.AreEqual(0xa12a, result3); // 'cde' + end bit
        }

        [TestMethod]
        public void testEncodeCharExceedsTargetBufferEscapeAtEnd()
        {
            // arrange
            int length = 7;
            int sourceAddress = 100;
            int targetAddress = 199;
            byte[] data = new byte[206];

            // Situation 2: in this case the escaped character is at the end,
            // so we need to ommit that escape sequence completely, padding
            // out the rest of the string
            data[sourceAddress] = (byte)'a';
            data[sourceAddress + 1] = (byte)'b';
            data[sourceAddress + 2] = (byte)'c';
            data[sourceAddress + 3] = (byte)'d';
            data[sourceAddress + 4] = (byte)'e';
            data[sourceAddress + 5] = (byte)'f';
            data[sourceAddress + 6] = (byte)'@';

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode(realmem, sourceAddress, length, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e8, result); // 'abc'
            Assert.AreEqual(0x254b, result2); // 'def'
            Assert.AreEqual(0x94a5, result3); // not long enough, pad it out
        }

        #region encode() with source String

        [TestMethod]
        public void testEncodeStringSingleCharacter()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act

            // we expect to have an end word, padded out with shift 5's
            encoderV4.encode("a", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18a5, result); // 'a' + 2 pad

            // Test that the rest is padded and marked with the end bit
            Assert.AreEqual(0x14a5, result2);
            Assert.AreEqual(0x94a5, result3);
        }

        [TestMethod]
        public void testEncodeStringTwoCharacters()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("ab", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e5, result); // 'ab' + pad

            // Test that the rest is padded and marked with the end bit
            Assert.AreEqual(0x14a5, result2);
            Assert.AreEqual(0x94a5, result3);
        }

        [TestMethod]
        public void testEncodeString4Characters()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("abcd", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e8, result); // 'abc'
            Assert.AreEqual(0x24a5, result2); // 'd' + 2 pads
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// Test with a different alphabet
        /// </summary>
        [TestMethod]
        public void testEncodeStringAlphabet1()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("a", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18a5, result); // 'a' + Pad

            // Test that the rest is padded and marked with the end bit
            Assert.AreEqual(0x14a5, result2);
            Assert.AreEqual(0x94a5, result3);
        }

        [TestMethod]
        public void testEncodeStringAlphabet1SpanWordBound()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("abc", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e8, result); // 'abc'
            Assert.AreEqual(0x14a5, result2); // pad
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        [TestMethod]
        public void testEncodeStringAlphabet2SpanWordBound()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("ab3", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e5, result); // 'ab' + Shift 5
            Assert.AreEqual(0x2ca5, result2); // '3'
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// Encoding of special characters in the unicode has to work.
        /// We test this on our favorite character: '@'
        /// Do not forget the testing across word boundaries
        /// </summary>
        [TestMethod]
        public void testEncodeStringEscapeA2()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("@", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Tricky, tricky (and memory-inefficient)
            // Shift-5 + 6 + '@' (64), encoded in 10 bit, the upper half contains 2
            Assert.AreEqual(0x14c2, result);
            Assert.AreEqual(0x00a5, result2); // the lower half contains 0 + 2 pads
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// For triangulation, we use another character (126)
        /// </summary>
        [TestMethod]
        public void testEncodeStringEscapeA2Tilde()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("~", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Tricky, tricky (and memory-inefficient)
            // Shift-5 + 6 + '~' (126), encoded in 10 bit, the upper half contains 3
            Assert.AreEqual(0x14c3, result);
            Assert.AreEqual(0x78a5, result2); // the lower half contains 30 + 2 pads
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        [TestMethod]
        public void testEncodeStringEscapeA2TildeSpansWord()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act
            encoderV4.encode("a~", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Tricky, tricky (and memory-inefficient)
            // 'a' + Shift-5 + 6
            Assert.AreEqual(0x18a6, result);
            Assert.AreEqual(0x0fc5, result2); // both halfs of '~' + 1 pad
            Assert.AreEqual(0x94a5, result3); // Test that the rest is padded and marked with the end bit
        }

        /// <summary>
        /// We test a situation where the 6 bytes are exceeded by the 9 source
        /// characters. In practice, this happens, when there are characters
        /// in the source buffer that need to be escaped, since they take the
        /// space of 4 lower case characters, which means that one special character
        /// can be combined with 5 lower case characters
        /// </summary>
        [TestMethod]
        public void testEncodeStringCharExceedsTargetBuffer()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act

            // Situation 1: there are lower case letters at the end, we need
            // to ensure that the dictionary is cropped and the characters
            // that exceed the buffer are ommitted
            encoderV4.encode("@abcdef", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert

            // Shift-5 + 6 + '@' (64), encoded in 10 bit, the upper half contains 2
            Assert.AreEqual(0x14c2, result);
            Assert.AreEqual(0x00c7, result2); // the lower half contains 0, 'ab'
            Assert.AreEqual(0xa12a, result3); // 'cde' + end bit
        }

        [TestMethod]
        public void testEncodeStringCharExceedsTargetBufferEscapeAtEnd()
        {
            // arrange
            int targetAddress = 199;
            byte[] data = new byte[206];

            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            //ZCharEncoder encoderV1 = new ZCharEncoder(translator, new DictionarySizesV1ToV3());
            ZCharEncoder encoderV4 = new ZCharEncoder(translator, new DictionarySizesV4ToV8());
            IMemory realmem = new DefaultMemory(data);

            // act

            // Situation 2: in this case the escaped character is at the end,
            // so we need to ommit that escape sequence completely, padding
            // out the rest of the string
            encoderV4.encode("abcdef@", realmem, targetAddress);
            var result = realmem.readUnsigned16(targetAddress);
            var result2 = realmem.readUnsigned16(targetAddress + 2);
            var result3 = realmem.readUnsigned16(targetAddress + 4);

            // assert
            Assert.AreEqual(0x18e8, result); // 'abc'
            Assert.AreEqual(0x254b, result2); // 'def'
            Assert.AreEqual(0x94a5, result3); // not long enough, pad it out
        }

        #endregion

        /*
          // Just for debugging purposes
          @Test
          public void testEncodeStringLikeDictionaryV1() {
            encodeAndPrintV1("wooden");
            encodeAndPrintV1("winding");
            encodeAndPrintV1("Y");
          }

          @Test
          public void testEncodeStringLikeDictionaryV4() {
            encodeAndPrintV4("y");
            encodeAndPrintV4("weather");
            encodeAndPrintV4("weedkiller");
          }

          private void encodeAndPrintV1(String str) {
            encoderV1.encode(str, realmem, targetAddress);
            System.out.printf("str = '%s' { %02x, %02x, %02x, %02x }\n", str,
                (int) realmem.readUnsigned8(targetAddress),
                (int) realmem.readUnsigned8(targetAddress + 1),
                (int) realmem.readUnsigned8(targetAddress + 2),
                (int) realmem.readUnsigned8(targetAddress + 3)
                );
          }
          private void encodeAndPrintV4(String str) {
            encoderV4.encode(str, realmem, targetAddress);
            System.out.printf("str = '%s' { %02x, %02x, %02x, %02x, %02x, %02x }\n", str,
                (int) realmem.readUnsigned8(targetAddress),
                (int) realmem.readUnsigned8(targetAddress + 1),
                (int) realmem.readUnsigned8(targetAddress + 2),
                (int) realmem.readUnsigned8(targetAddress + 3),
                (int) realmem.readUnsigned8(targetAddress + 4),
                (int) realmem.readUnsigned8(targetAddress + 5)
                );
          }
          */
    }
}
