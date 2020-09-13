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
            encoding = new ZsciiEncoding(new AccentTable());

            // act
            var result = encoding.IsZscii('A');
            var result2 = encoding.IsZscii('M');
            var result3 = encoding.IsZscii('Z');
            var result4 = encoding.IsZscii('a');
            var result5 = encoding.IsZscii('m');
            var result6 = encoding.IsZscii('z');

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
            encoding = new ZsciiEncoding(new AccentTable());

            // act
            var result = encoding.IsZscii(ZsciiEncoding.Null);
            var result2 = encoding.IsZscii(ZsciiEncoding.Newline);
            var result3 = encoding.IsZscii(ZsciiEncoding.Escape);
            var result4 = encoding.IsZscii(ZsciiEncoding.Delete);

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
            encoding = new ZsciiEncoding(new AccentTable());

            // act
            var result = encoding.IsConvertibleToZscii('A');
            var result2 = encoding.IsConvertibleToZscii('M');
            var result3 = encoding.IsConvertibleToZscii('Z');
            var result4 = encoding.IsConvertibleToZscii('a');
            var result5 = encoding.IsConvertibleToZscii('m');
            var result6 = encoding.IsConvertibleToZscii('z');

            var result7 = encoding.IsConvertibleToZscii('\n');
            var result8 = encoding.IsConvertibleToZscii('\x0007');

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
            encoding = new ZsciiEncoding(new AccentTable());

            // act
            var result = encoding.ToUnicodeChar('A');
            var result2 = encoding.ToUnicodeChar('M');
            var result3 = encoding.ToUnicodeChar('Z');
            var result4 = encoding.ToUnicodeChar('a');
            var result5 = encoding.ToUnicodeChar('m');
            var result6 = encoding.ToUnicodeChar('z');
            var result7 = encoding.ToUnicodeChar(ZsciiEncoding.Newline);
            var result8 = encoding.ToUnicodeChar(ZsciiEncoding.Null);
            var result9 = encoding.ToUnicodeChar(ZsciiEncoding.Delete);

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
            encoding = new ZsciiEncoding(new AccentTable());

            // act
            var result = encoding.ToZsciiChar('A');
            var result2 = encoding.ToZsciiChar('M');
            var result3 = encoding.ToZsciiChar('Z');

            var result4 = encoding.ToZsciiChar('a');
            var result5 = encoding.ToZsciiChar('m');
            var result6 = encoding.ToZsciiChar('z');
            var result7 = encoding.ToZsciiChar('\x0007');

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
            var result = ZsciiEncoding.IsCursorKey(ZsciiEncoding.CursorUp);
            var result2 = ZsciiEncoding.IsCursorKey(ZsciiEncoding.CursorDown);
            var result3 = ZsciiEncoding.IsCursorKey(ZsciiEncoding.CursorLeft);
            var result4 = ZsciiEncoding.IsCursorKey(ZsciiEncoding.CursorRight);
            var result5 = ZsciiEncoding.IsCursorKey(ZsciiEncoding.Newline);

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
            var accentTable = new AccentTable();

            // act
            var result = accentTable.Length;

            // assert
            Assert.AreEqual(69, result);
        }

        [TestMethod]
        public void testToLowerCase()
        {
            // arrange
            encoding = new ZsciiEncoding(new AccentTable());

            // act
            var result = encoding.ToLower('A');
            var result2 = encoding.ToLower((char)158);

            // assert
            Assert.AreEqual('a', result);
            Assert.AreEqual(155, result2);
        }
    }
}
