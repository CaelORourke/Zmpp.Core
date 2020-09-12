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
    using Moq;
    using Zmpp.Core;
    using Zmpp.Core.Encoding;
    using Zmpp.Core.Vm;
    using System;

    /// <summary>
    /// Tests for the DefaultZCharDecoder class.
    /// </summary>
    [TestClass]
    public class ZCharDecoderTest
    {
        public ZCharDecoderTest()
        {
        }

        [TestMethod]
        public void testDecodeByte()
        {
            // arrange
            var abbrev = new Mock<IZCharDecoder.AbbreviationsTable>();
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new DefaultZCharDecoder(encoding, translator, abbrev.Object);

            // act
            var result = decoder.decodeZChar((char)6);

            // assert
            Assert.AreEqual('a', result);
        }

        [TestMethod]
        public void testDecode2Unicode2Params()
        {
            // arrange
            byte[] hello = { 0x35, 0x51, (byte)0xc6, (byte)0x85 };
            byte[] Hello = { 0x11, (byte)0xaa, (byte)0xc6, (byte)0x34 };
            IMemory memory1 = new DefaultMemory(hello);
            IMemory memory2 = new DefaultMemory(Hello);

            var abbrev = new Mock<IZCharDecoder.AbbreviationsTable>();
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new DefaultZCharDecoder(encoding, translator, abbrev.Object);

            // act
            var result = decoder.decode2Zscii(memory1, 0, 0).ToString();
            var result2 = decoder.decode2Zscii(memory2, 0, 0).ToString();

            // assert
            Assert.AreEqual("hello", result);
            Assert.AreEqual("Hello", result2);
        }

        #region Real-world tests

        [TestMethod]
        public void testMinizork()
        {
            // arrange
            byte[] zork1data = System.IO.File.ReadAllBytes("testfiles/minizork.z3");
            IMemory mem = new DefaultMemory(zork1data);

            IZCharDecoder.AbbreviationsTable abbr = new Abbreviations(mem, mem.readUnsigned16(StoryFileHeaderBase.ABBREVIATIONS));
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            DefaultAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder dec = new DefaultZCharDecoder(encoding, translator, abbr);

            // act
            var result = dec.decode2Zscii(mem, 0xc120, 0).ToString();
            var result2 = dec.decode2Zscii(mem, 0x3e6d, 0).ToString();

            // assert
            Assert.AreEqual("The Great Underground Empire", result);
            Assert.AreEqual("[I don't understand that sentence.]", result2);
        }

        /// <summary>
        /// A pretty complex example: the Zork I introduction message. This one
        /// clarified that the current shift lock alphabet needs to be restored
        /// after a regular shift occurred.
        /// </summary>
        [TestMethod]
        public void testZork1V1()
        {
            // arrange
            String originalString = "ZORK: The Great Underground Empire - Part I\n"
              + "Copyright (c) 1980 by Infocom, Inc. All rights reserved.\n"
              + "ZORK is a trademark of Infocom, Inc.\n"
              + "Release ";

            // This String was extracted from release 5 of Zork I and contains
            // the same message as in originalString.
            byte[] data = {
                (byte) 0x13, (byte) 0xf4, (byte) 0x5e, (byte) 0x02,
                (byte) 0x74, (byte) 0x19, (byte) 0x15, (byte) 0xaa,
                (byte) 0x00, (byte) 0x4c, (byte) 0x5d, (byte) 0x46,
                (byte) 0x64, (byte) 0x02, (byte) 0x6a, (byte) 0x69,
                (byte) 0x2a, (byte) 0xec, (byte) 0x5e, (byte) 0x9a,
                (byte) 0x4d, (byte) 0x20, (byte) 0x09, (byte) 0x52,
                (byte) 0x55, (byte) 0xd7, (byte) 0x28, (byte) 0x03,
                (byte) 0x70, (byte) 0x02, (byte) 0x54, (byte) 0xd7,
                (byte) 0x64, (byte) 0x02, (byte) 0x38, (byte) 0x22,
                (byte) 0x22, (byte) 0x95, (byte) 0x7a, (byte) 0xee,
                (byte) 0x31, (byte) 0xb9, (byte) 0x00, (byte) 0x7e,
                (byte) 0x20, (byte) 0x7f, (byte) 0x00, (byte) 0xa8,
                (byte) 0x41, (byte) 0xe7, (byte) 0x00, (byte) 0x87,
                (byte) 0x78, (byte) 0x02, (byte) 0x3a, (byte) 0x6b,
                (byte) 0x51, (byte) 0x14, (byte) 0x48, (byte) 0x72,
                (byte) 0x00, (byte) 0x4e, (byte) 0x4d, (byte) 0x03,
                (byte) 0x44, (byte) 0x02, (byte) 0x1a, (byte) 0x31,
                (byte) 0x02, (byte) 0xee, (byte) 0x31, (byte) 0xb9,
                (byte) 0x60, (byte) 0x17, (byte) 0x2b, (byte) 0x0a,
                (byte) 0x5f, (byte) 0x6a, (byte) 0x24, (byte) 0x71,
                (byte) 0x04, (byte) 0x9f, (byte) 0x52, (byte) 0xf0,
                (byte) 0x00, (byte) 0xae, (byte) 0x60, (byte) 0x06,
                (byte) 0x03, (byte) 0x37, (byte) 0x19, (byte) 0x2a,
                (byte) 0x48, (byte) 0xd7, (byte) 0x40, (byte) 0x14,
                (byte) 0x2c, (byte) 0x02, (byte) 0x3a, (byte) 0x6b,
                (byte) 0x51, (byte) 0x14, (byte) 0x48, (byte) 0x72,
                (byte) 0x00, (byte) 0x4e, (byte) 0x4d, (byte) 0x03,
                (byte) 0x44, (byte) 0x22, (byte) 0x5d, (byte) 0x51,
                (byte) 0x28, (byte) 0xd8, (byte) 0xa8, (byte) 0x05,
            };

            IMemory mem = new DefaultMemory(data);

            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            AlphabetTableV1 alphabetTable = new AlphabetTableV1();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder dec = new DefaultZCharDecoder(encoding, translator, null);

            // act
            String decoded = dec.decode2Zscii(mem, 0, 0).ToString();

            // assert
            Assert.AreEqual(originalString, decoded);
        }

        #endregion

        #region Test based on mock objects

        [TestMethod]
        public void testConvertWithAbbreviation()
        {
            // arrange
            byte[] helloAbbrev = {
                0x35, 0x51, (byte) 0x46, (byte) 0x81, (byte) 0x88, (byte) 0xa5, // hello{abbrev_2}
                0x35, 0x51, (byte) 0xc6, (byte) 0x85, // hello
                0x11, (byte) 0xaa, (byte) 0xc6, (byte) 0x34 // Hello
            };
            IMemory mem = new DefaultMemory(helloAbbrev);

            var abbrev = new Mock<IZCharDecoder.AbbreviationsTable>();
            abbrev.Setup(a => a.getWordAddress(2)).Returns(10);
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new DefaultZCharDecoder(encoding, translator, abbrev.Object);

            // act
            var result = decoder.decode2Zscii(mem, 0, 0).ToString();

            // assert
            abbrev.Verify(a => a.getWordAddress(2), Times.Once());
            Assert.AreEqual("helloHello", result);
        }

        [TestMethod]
        public void testEndCharacter()
        {
            // arrange
            char notEndWord = (char)0x7123;
            char endWord = (char)0x8123;

            // act
            var result = DefaultZCharDecoder.isEndWord(notEndWord);
            var result2 = DefaultZCharDecoder.isEndWord(endWord);

            // assert
            Assert.IsFalse(result);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void testExtractZBytesOneWordOnly()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0)).Returns((char)0x9865);
            char[] data = DefaultZCharDecoder.extractZbytes(memory.Object, 0, 0);

            // act
            var result = data.Length;
            var result2 = data[0];
            var result3 = data[1];
            var result4 = data[2];

            // assert
            memory.Verify(m => m.readUnsigned16(0), Times.Once());
            Assert.AreEqual(3, result);
            Assert.AreEqual(6, result2);
            Assert.AreEqual(3, result3);
            Assert.AreEqual(5, result4);
        }

        [TestMethod]
        public void testExtractZBytesThreeWords()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0)).Returns((char)0x5432);
            memory.Setup(m => m.readUnsigned16(2)).Returns((char)0x1234);
            memory.Setup(m => m.readUnsigned16(4)).Returns((char)0x9865);
            char[] data = DefaultZCharDecoder.extractZbytes(memory.Object, 0, 0);

            // act
            var result = data.Length;

            // assert
            memory.Verify(m => m.readUnsigned16(0), Times.Once());
            memory.Verify(m => m.readUnsigned16(2), Times.Once());
            memory.Verify(m => m.readUnsigned16(4), Times.Once());
            Assert.AreEqual(9, result);
        }

        #endregion

        #region Test for string truncation

        //// *********************************************************************
        //// **** We test the truncation algorithm for V3 length only which is
        //// **** 4 bytes, 6 characters. In fact, this should be general enough
        //// **** so we do not need to test 6 bytes, 9 characters as in >= V4
        //// **** files. Since this method is only used within dictionaries, we
        //// **** do not need to test abbreviations
        //// ****************************************

        [TestMethod]
        public void testTruncateAllSmall()
        {
            // arrange
            int length = 4;

            byte[] data = { (byte)0x35, (byte)0x51, (byte)0x46, (byte)0x86, (byte)0xc6, (byte)0x85 };
            IMemory mem = new DefaultMemory(data);

            var abbrev = new Mock<IZCharDecoder.AbbreviationsTable>();
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new DefaultZCharDecoder(encoding, translator, abbrev.Object);

            // act
            var result = decoder.decode2Zscii(mem, 0, 0).ToString(); // With length = 0
            var result2 = decoder.decode2Zscii(mem, 0, length).ToString(); // With length = 4

            // assert
            Assert.AreEqual("helloalo", result); // With length = 0
            Assert.AreEqual("helloa", result2); // With length = 4
        }

        [TestMethod]
        public void testTruncateShiftAtEnd()
        {
            // arrange
            int length = 4;

            byte[] data = { (byte)0x34, (byte)0x8a, (byte)0x45, (byte)0xc4 };
            IMemory mem = new DefaultMemory(data);

            var abbrev = new Mock<IZCharDecoder.AbbreviationsTable>();
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new DefaultZCharDecoder(encoding, translator, abbrev.Object);

            // act
            var result = decoder.decode2Zscii(mem, 0, length).ToString();

            // assert
            Assert.AreEqual("hEli", result);
        }

        /// <summary>
        /// Escape A6 starts at position 0 of the last word.
        /// </summary>
        [TestMethod]
        public void testTruncateEscapeA2AtEndStartsAtWord2_0()
        {
            // arrange
            int length = 4;

            byte[] data = { (byte)0x34, (byte)0xd1, (byte)0x14, (byte)0xc1, (byte)0x80, (byte)0xa5 };
            IMemory mem = new DefaultMemory(data);

            var abbrev = new Mock<IZCharDecoder.AbbreviationsTable>();
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new DefaultZCharDecoder(encoding, translator, abbrev.Object);

            // act
            var result = decoder.decode2Zscii(mem, 0, length).ToString();

            // assert
            Assert.AreEqual("hal", result);
        }

        /// <summary>
        /// Escape A6 starts at position 1 of the last word.
        /// </summary>
        [TestMethod]
        public void testTruncateEscapeA2AtEndStartsAtWord2_1()
        {
            // arrange
            int length = 4;

            byte[] data = { (byte)0x34, (byte)0xd1, (byte)0x44, (byte)0xa6, (byte)0x84, (byte)0x05 };
            IMemory mem = new DefaultMemory(data);

            var abbrev = new Mock<IZCharDecoder.AbbreviationsTable>();
            ZsciiEncoding encoding = new ZsciiEncoding(new DefaultAccentTable());
            IAlphabetTable alphabetTable = new DefaultAlphabetTable();
            IZCharTranslator translator = new DefaultZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new DefaultZCharDecoder(encoding, translator, abbrev.Object);

            // act
            var result = decoder.decode2Zscii(mem, 0, length).ToString();

            // assert
            Assert.AreEqual("hall", result);
        }

        #endregion
    }
}
