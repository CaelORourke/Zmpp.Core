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

namespace test.zmpp.encoding
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using org.zmpp.encoding;

    /// <summary>
    /// Test class for ZsciiEncoding.
    /// </summary>
    [TestClass]
    public class ZsciiEncodingTest
    {
        private ZsciiEncoding encoding;

        public ZsciiEncodingTest()
        {
        }

        [TestMethod]
        public void testIsZsciiCharacterAscii()
        {
            // arrange
            encoding = new ZsciiEncoding(new DefaultAccentTable());

            // act
            var result = encoding.isZsciiCharacter('A');
            var result2 = encoding.isZsciiCharacter('M');
            var result3 = encoding.isZsciiCharacter('Z');
            var result4 = encoding.isZsciiCharacter('a');
            var result5 = encoding.isZsciiCharacter('m');
            var result6 = encoding.isZsciiCharacter('z');

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            Assert.IsTrue(result5);
            Assert.IsTrue(result6);
        }

        [TestMethod]
        public void testIsZsciiCharacterExtra()
        {
            // arrange
            encoding = new ZsciiEncoding(new DefaultAccentTable());

            // act
            var result = encoding.isZsciiCharacter(ZsciiEncoding.NULL);
            var result2 = encoding.isZsciiCharacter(ZsciiEncoding.NEWLINE);
            var result3 = encoding.isZsciiCharacter(ZsciiEncoding.ESCAPE);
            var result4 = encoding.isZsciiCharacter(ZsciiEncoding.DELETE);

            // assert
            Assert.AreEqual(10, (int)'\n');
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
        }

        [TestMethod]
        public void testIsConvertableToZscii()
        {
            // arrange
            encoding = new ZsciiEncoding(new DefaultAccentTable());

            // act
            var result = encoding.isConvertableToZscii('A');
            var result2 = encoding.isConvertableToZscii('M');
            var result3 = encoding.isConvertableToZscii('Z');
            var result4 = encoding.isConvertableToZscii('a');
            var result5 = encoding.isConvertableToZscii('m');
            var result6 = encoding.isConvertableToZscii('z');

            var result7 = encoding.isConvertableToZscii('\n');
            var result8 = encoding.isConvertableToZscii('\x0007');

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            Assert.IsTrue(result5);
            Assert.IsTrue(result6);

            Assert.IsTrue(result7);
            Assert.IsFalse(result8);
        }

        [TestMethod]
        public void testGetUnicode()
        {
            // arrange
            encoding = new ZsciiEncoding(new DefaultAccentTable());

            // act
            var result = encoding.getUnicodeChar('A');
            var result2 = encoding.getUnicodeChar('M');
            var result3 = encoding.getUnicodeChar('Z');
            var result4 = encoding.getUnicodeChar('a');
            var result5 = encoding.getUnicodeChar('m');
            var result6 = encoding.getUnicodeChar('z');
            var result7 = encoding.getUnicodeChar(ZsciiEncoding.NEWLINE);
            var result8 = encoding.getUnicodeChar(ZsciiEncoding.NULL);
            var result9 = encoding.getUnicodeChar(ZsciiEncoding.DELETE);

            // assert
            Assert.AreEqual('A', result);
            Assert.AreEqual('M', result2);
            Assert.AreEqual('Z', result3);
            Assert.AreEqual('a', result4);
            Assert.AreEqual('m', result5);
            Assert.AreEqual('z', result6);
            Assert.AreEqual('\n', result7);
            Assert.AreEqual('\0', result8);
            Assert.AreEqual('?', result9);
        }

        [TestMethod]
        public void testGetZChar()
        {
            // arrange
            encoding = new ZsciiEncoding(new DefaultAccentTable());

            // act
            var result = encoding.getZsciiChar('A');
            var result2 = encoding.getZsciiChar('M');
            var result3 = encoding.getZsciiChar('Z');

            var result4 = encoding.getZsciiChar('a');
            var result5 = encoding.getZsciiChar('m');
            var result6 = encoding.getZsciiChar('z');
            var result7 = encoding.getZsciiChar('\x0007');

            // assert
            Assert.AreEqual('A', result);
            Assert.AreEqual('M', result2);
            Assert.AreEqual('Z', result3);

            Assert.AreEqual('a', result4);
            Assert.AreEqual('m', result5);
            Assert.AreEqual('z', result6);
            Assert.AreEqual(0, result7);
        }

        [TestMethod]
        public void testIsCursorKey()
        {
            // arrange

            // act
            var result = ZsciiEncoding.isCursorKey(ZsciiEncoding.CURSOR_UP);
            var result2 = ZsciiEncoding.isCursorKey(ZsciiEncoding.CURSOR_DOWN);
            var result3 = ZsciiEncoding.isCursorKey(ZsciiEncoding.CURSOR_LEFT);
            var result4 = ZsciiEncoding.isCursorKey(ZsciiEncoding.CURSOR_RIGHT);
            var result5 = ZsciiEncoding.isCursorKey(ZsciiEncoding.NEWLINE);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
            Assert.IsFalse(result5);
        }

        [TestMethod]
        public void testStandardTable()
        {
            // arrange
            var accentTable = new DefaultAccentTable();

            // act
            var result = accentTable.getLength();

            // assert
            Assert.AreEqual(69, result);
        }

        [TestMethod]
        public void testToLowerCase()
        {
            // arrange
            encoding = new ZsciiEncoding(new DefaultAccentTable());

            // act
            var result = encoding.toLower('A');
            var result2 = encoding.toLower((char)158);

            // assert
            Assert.AreEqual('a', result);
            Assert.AreEqual(155, result2);
        }
    }
}
