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
    /// Tests for the DefaultStoryFileHeader class.
    /// </summary>
    [TestClass]
    public class DefaultStoryFileHeaderTest
    {
        public DefaultStoryFileHeaderTest()
        {
        }

        [TestMethod]
        public void testGetVersion()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)3);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getVersion();

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.AtLeastOnce());
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void testGetSerialNumber()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x012)).Returns('0');
            memory.Setup(m => m.readUnsigned8(0x013)).Returns('5');
            memory.Setup(m => m.readUnsigned8(0x014)).Returns('1');
            memory.Setup(m => m.readUnsigned8(0x015)).Returns('2');
            memory.Setup(m => m.readUnsigned8(0x016)).Returns('0');
            memory.Setup(m => m.readUnsigned8(0x017)).Returns('9');
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getSerialNumber();

            // assert
            memory.Verify(m => m.readUnsigned8(0x012), Times.Once());
            memory.Verify(m => m.readUnsigned8(0x013), Times.Once());
            memory.Verify(m => m.readUnsigned8(0x014), Times.Once());
            memory.Verify(m => m.readUnsigned8(0x015), Times.Once());
            memory.Verify(m => m.readUnsigned8(0x016), Times.Once());
            memory.Verify(m => m.readUnsigned8(0x017), Times.Once());
            Assert.AreEqual("051209", result);
        }

        [TestMethod]
        public void testGetFileLengthV3()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)3);
            memory.Setup(m => m.readUnsigned16(0x1a)).Returns((char)4718);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getFileLength();

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.readUnsigned16(0x1a), Times.Once());
            Assert.AreEqual(4718 * 2, result);
        }

        [TestMethod]
        public void testGetFileLengthV4()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)4);
            memory.Setup(m => m.readUnsigned16(0x1a)).Returns((char)4718);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getFileLength();

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.AtLeastOnce());
            memory.Verify(m => m.readUnsigned16(0x1a), Times.Once());
            Assert.AreEqual(4718 * 4, result);
        }

        [TestMethod]
        public void testGetFileLengthV8()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)8);
            memory.Setup(m => m.readUnsigned16(0x1a)).Returns((char)4718);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getFileLength();

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.AtLeastOnce());
            memory.Verify(m => m.readUnsigned16(0x1a), Times.Once());
            Assert.AreEqual(4718 * 8, result);
        }

        [TestMethod]
        public void testSetInterpreterVersionV5()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)5);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setInterpreterVersion(4);

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.AtLeastOnce()); // Story file version 4 or 5: version number as string
            memory.Verify(m => m.writeUnsigned8(0x1f, '4'), Times.Once());
        }

        [TestMethod]
        public void testSetInterpreterVersionV8()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)8);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setInterpreterVersion(4);

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.AtLeastOnce()); // Story file version > 5: version number as value
            memory.Verify(m => m.writeUnsigned8(0x1f, (char)4), Times.Once());
        }

        #region Attributes

        [TestMethod]
        public void testIsEnabledNull()
        {
            // arrange
            var memory = new Mock<IMemory>();
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.isEnabled(StoryFileHeaderAttribute.SUPPORTS_STATUSLINE);

            // assert
            // This is not matched in the code
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void testSetTranscripting()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0x00)).Returns((char)0);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.TRANSCRIPTING, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.TRANSCRIPTING, false);
            var result = fileHeader.getFileLength();

            // assert
            memory.Verify(m => m.readUnsigned16(0x10), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned16(0x10, (char)1), Times.Once());
            memory.Verify(m => m.writeUnsigned16(0x10, (char)0), Times.Once());
        }

        [TestMethod]
        public void testIsTranscriptingEnabled()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned16(0x10))
                .Returns((char)1)
                .Returns((char)0);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.isEnabled(StoryFileHeaderAttribute.TRANSCRIPTING);
            var result2 = fileHeader.isEnabled(StoryFileHeaderAttribute.TRANSCRIPTING);

            // assert
            memory.Verify(m => m.readUnsigned16(0x10), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void testSetForceFixedFont()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0x10)).Returns((char)1);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.FORCE_FIXED_FONT, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.FORCE_FIXED_FONT, false);

            // assert
            memory.Verify(m => m.readUnsigned16(0x10), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned16(0x10, (char)3), Times.Once());
            memory.Verify(m => m.writeUnsigned16(0x10, (char)1), Times.Once());
        }

        [TestMethod]
        public void testIsForceFixedFont()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned16(0x10))
                .Returns((char)6)
                .Returns((char)5);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.isEnabled(StoryFileHeaderAttribute.FORCE_FIXED_FONT);
            var result2 = fileHeader.isEnabled(StoryFileHeaderAttribute.FORCE_FIXED_FONT);

            // assert
            memory.Verify(m => m.readUnsigned16(0x10), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void testSetSupportsTimedInput()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)3)
                .Returns((char)131);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_TIMED_INPUT, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_TIMED_INPUT, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)131), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)3), Times.Once());
        }

        [TestMethod]
        public void testIsScoreGame()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)5)
                .Returns((char)7);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.isEnabled(StoryFileHeaderAttribute.SCORE_GAME);
            var result2 = fileHeader.isEnabled(StoryFileHeaderAttribute.SCORE_GAME);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void testSetSupportsFixed()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)17);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_FIXED_FONT, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_FIXED_FONT, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)17), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void testSetSupportsBold()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)5);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_BOLD, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_BOLD, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)5), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void testSetSupportsItalic()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)9);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_ITALIC, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_ITALIC, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)9), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void testSetSupportsScreenSplitting()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)33);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_SCREEN_SPLITTING, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_SCREEN_SPLITTING, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)33), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void testSetSupportsStatusLine()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)17);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_STATUSLINE, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_STATUSLINE, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)17), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void testSetDefaultFontIsVariable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)65);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.DEFAULT_FONT_IS_VARIABLE, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.DEFAULT_FONT_IS_VARIABLE, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)65), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void testIsDefaultFontVariable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)69)
                .Returns((char)7);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.isEnabled(StoryFileHeaderAttribute.DEFAULT_FONT_IS_VARIABLE);
            var result2 = fileHeader.isEnabled(StoryFileHeaderAttribute.DEFAULT_FONT_IS_VARIABLE);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void testSetSupportsColors()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.readUnsigned8(0x01))
                .Returns((char)4)
                .Returns((char)5);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_COLOURS, true);
            fileHeader.setEnabled(StoryFileHeaderAttribute.SUPPORTS_COLOURS, false);

            // assert
            memory.Verify(m => m.readUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)5), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x01, (char)4), Times.Once());
        }

        [TestMethod]
        public void testSetFontWidthV5()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)5);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setFontWidth(1);

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x26, (char)1), Times.Once());
        }

        [TestMethod]
        public void testSetFontWidthV6()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)6);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setFontWidth(1);

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x27, (char)1), Times.Once());
        }

        [TestMethod]
        public void testSetFontHeightV5()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)5);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setFontHeight(2);

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x27, (char)2), Times.Once());
        }

        [TestMethod]
        public void testSetFontHeightV6()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x00)).Returns((char)6);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setFontHeight(2);

            // assert
            memory.Verify(m => m.readUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.writeUnsigned8(0x26, (char)2), Times.Once());
        }

        [TestMethod]
        public void testUseMouseFalse()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x10)).Returns((char)2);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.isEnabled(StoryFileHeaderAttribute.USE_MOUSE);

            // assert
            memory.Verify(m => m.readUnsigned8(0x10), Times.Once());
        }

        [TestMethod]
        public void testUseMouseTrue()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned8(0x10)).Returns((char)63);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.isEnabled(StoryFileHeaderAttribute.USE_MOUSE);

            // assert
            memory.Verify(m => m.readUnsigned8(0x10), Times.Once());
        }

        #endregion

        /// <summary>
        /// Simulate a situation to set mouse coordinates
        /// </summary>
        [TestMethod]
        public void testSetMouseCoordinatesNoExtensionTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0x36)).Returns((char)0);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setMouseCoordinates(1, 2);

            // assert
            memory.Verify(m => m.readUnsigned16(0x36), Times.Once());
        }

        [TestMethod]
        public void testSetMouseCoordinatesHasExtensionTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0x36)).Returns((char)100);
            memory.Setup(m => m.readUnsigned16(100)).Returns((char)2);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            fileHeader.setMouseCoordinates(1, 2);

            // assert
            memory.Verify(m => m.readUnsigned16(0x36), Times.Once());
            memory.Verify(m => m.readUnsigned16(100), Times.Once());
            memory.Verify(m => m.writeUnsigned16(102, (char)1), Times.Once());
            memory.Verify(m => m.writeUnsigned16(104, (char)2), Times.Once());
        }

        [TestMethod]
        public void testGetUnicodeTranslationTableNoExtensionTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0x36)).Returns((char)0);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getCustomAccentTable();

            // assert
            memory.Verify(m => m.readUnsigned16(0x36), Times.Once());
            Assert.AreEqual((char)0, result);
        }

        [TestMethod]
        public void testGetCustomUnicodeTranslationTableNoTableInExtTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0x36)).Returns((char)100);
            memory.Setup(m => m.readUnsigned16(100)).Returns((char)2);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getCustomAccentTable();

            // assert
            memory.Verify(m => m.readUnsigned16(0x36), Times.Once());
            memory.Verify(m => m.readUnsigned16(100), Times.Once());
            Assert.AreEqual((char)0, result);
        }

        [TestMethod]
        public void testGetCustomUnicodeTranslationTableHasExtAddress()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.readUnsigned16(0x36)).Returns((char)100);
            memory.Setup(m => m.readUnsigned16(100)).Returns((char)3);
            memory.Setup(m => m.readUnsigned16(106)).Returns((char)1234);
            IStoryFileHeader fileHeader = new DefaultStoryFileHeader(memory.Object);

            // act
            var result = fileHeader.getCustomAccentTable();

            // assert
            memory.Verify(m => m.readUnsigned16(0x36), Times.Once());
            memory.Verify(m => m.readUnsigned16(100), Times.Once());
            memory.Verify(m => m.readUnsigned16(106), Times.Once());
            Assert.AreEqual((char)1234, result);
        }
    }
}
