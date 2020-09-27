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
    /// Tests for the StoryFileHeader class.
    /// </summary>
    [TestClass]
    public class StoryFileHeaderTest
    {
        [TestMethod]
        public void Version()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)3);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.Version;

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.AtLeastOnce());
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void SerialNumber()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x012)).Returns('0');
            memory.Setup(m => m.ReadUnsigned8(0x013)).Returns('5');
            memory.Setup(m => m.ReadUnsigned8(0x014)).Returns('1');
            memory.Setup(m => m.ReadUnsigned8(0x015)).Returns('2');
            memory.Setup(m => m.ReadUnsigned8(0x016)).Returns('0');
            memory.Setup(m => m.ReadUnsigned8(0x017)).Returns('9');
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.SerialNumber;

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x012), Times.Once());
            memory.Verify(m => m.ReadUnsigned8(0x013), Times.Once());
            memory.Verify(m => m.ReadUnsigned8(0x014), Times.Once());
            memory.Verify(m => m.ReadUnsigned8(0x015), Times.Once());
            memory.Verify(m => m.ReadUnsigned8(0x016), Times.Once());
            memory.Verify(m => m.ReadUnsigned8(0x017), Times.Once());
            Assert.AreEqual("051209", result);
        }

        [TestMethod]
        public void FileLengthV3()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)3);
            memory.Setup(m => m.ReadUnsigned16(0x1a)).Returns((char)4718);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.FileLength;

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(0x1a), Times.Once());
            Assert.AreEqual(4718 * 2, result);
        }

        [TestMethod]
        public void FileLengthV4()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)4);
            memory.Setup(m => m.ReadUnsigned16(0x1a)).Returns((char)4718);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.FileLength;

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.AtLeastOnce());
            memory.Verify(m => m.ReadUnsigned16(0x1a), Times.Once());
            Assert.AreEqual(4718 * 4, result);
        }

        [TestMethod]
        public void FileLengthV8()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)8);
            memory.Setup(m => m.ReadUnsigned16(0x1a)).Returns((char)4718);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.FileLength;

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.AtLeastOnce());
            memory.Verify(m => m.ReadUnsigned16(0x1a), Times.Once());
            Assert.AreEqual(4718 * 8, result);
        }

        [TestMethod]
        public void SetInterpreterVersionV5()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)5);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetInterpreterVersion(4);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.AtLeastOnce()); // Story file version 4 or 5: version number as string
            memory.Verify(m => m.WriteUnsigned8(0x1f, '4'), Times.Once());
        }

        [TestMethod]
        public void SetInterpreterVersionV8()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)8);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetInterpreterVersion(4);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.AtLeastOnce()); // Story file version > 5: version number as value
            memory.Verify(m => m.WriteUnsigned8(0x1f, (char)4), Times.Once());
        }

        #region Attributes

        [TestMethod]
        public void IsEnabledNull()
        {
            // arrange
            var memory = new Mock<IMemory>();
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.IsEnabled(StoryFileHeaderAttribute.SupportsStatusLine);

            // assert
            // This is not matched in the code
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SetTranscriptingOn()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(0x00)).Returns((char)0);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.Transcripting, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.Transcripting, false);
            var result = fileHeader.FileLength;

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x10), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned16(0x10, (char)1), Times.Once());
            memory.Verify(m => m.WriteUnsigned16(0x10, (char)0), Times.Once());
        }

        [TestMethod]
        public void IsTranscriptingOn()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned16(0x10))
                .Returns((char)1)
                .Returns((char)0);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.IsEnabled(StoryFileHeaderAttribute.Transcripting);
            var result2 = fileHeader.IsEnabled(StoryFileHeaderAttribute.Transcripting);

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x10), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void SetForceFixedFont()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(0x10)).Returns((char)1);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.ForceFixedFont, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.ForceFixedFont, false);

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x10), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned16(0x10, (char)3), Times.Once());
            memory.Verify(m => m.WriteUnsigned16(0x10, (char)1), Times.Once());
        }

        [TestMethod]
        public void IsForceFixedFont()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned16(0x10))
                .Returns((char)6)
                .Returns((char)5);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.IsEnabled(StoryFileHeaderAttribute.ForceFixedFont);
            var result2 = fileHeader.IsEnabled(StoryFileHeaderAttribute.ForceFixedFont);

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x10), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void SetSupportsTimedInput()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)3)
                .Returns((char)131);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsTimedInput, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsTimedInput, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)131), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)3), Times.Once());
        }

        [TestMethod]
        public void IsScoreGame()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)5)
                .Returns((char)7);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.IsEnabled(StoryFileHeaderAttribute.ScoreGame);
            var result2 = fileHeader.IsEnabled(StoryFileHeaderAttribute.ScoreGame);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void SetSupportsFixed()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)17);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsFixedFont, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsFixedFont, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)17), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void SetSupportsBold()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)5);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsBold, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsBold, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)5), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void SetSupportsItalic()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)9);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsItalic, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsItalic, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)9), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void SetSupportsScreenSplitting()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)33);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsScreenSplitting, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsScreenSplitting, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)33), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void SetSupportsStatusLine()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)17);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsStatusLine, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsStatusLine, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)17), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void SetDefaultFontIsVariable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)1)
                .Returns((char)65);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.DefaultFontIsVariable, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.DefaultFontIsVariable, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)65), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)1), Times.Once());
        }

        [TestMethod]
        public void IsDefaultFontVariable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)69)
                .Returns((char)7);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.IsEnabled(StoryFileHeaderAttribute.DefaultFontIsVariable);
            var result2 = fileHeader.IsEnabled(StoryFileHeaderAttribute.DefaultFontIsVariable);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void SetSupportsColors()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.SetupSequence(m => m.ReadUnsigned8(0x01))
                .Returns((char)4)
                .Returns((char)5);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsColours, true);
            fileHeader.SetEnabled(StoryFileHeaderAttribute.SupportsColours, false);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x01), Times.AtLeastOnce());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)5), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x01, (char)4), Times.Once());
        }

        [TestMethod]
        public void SetFontWidthV5()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)5);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetFontWidth(1);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x26, (char)1), Times.Once());
        }

        [TestMethod]
        public void SetFontWidthV6()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)6);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetFontWidth(1);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x27, (char)1), Times.Once());
        }

        [TestMethod]
        public void SetFontHeightV5()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)5);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetFontHeight(2);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x27, (char)2), Times.Once());
        }

        [TestMethod]
        public void SetFontHeightV6()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x00)).Returns((char)6);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetFontHeight(2);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x00), Times.Once());
            memory.Verify(m => m.WriteUnsigned8(0x26, (char)2), Times.Once());
        }

        [TestMethod]
        public void UseMouseIsFalse()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x10)).Returns((char)2);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.IsEnabled(StoryFileHeaderAttribute.UseMouse);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x10), Times.Once());
        }

        [TestMethod]
        public void UseMouseIsTrue()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned8(0x10)).Returns((char)63);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.IsEnabled(StoryFileHeaderAttribute.UseMouse);

            // assert
            memory.Verify(m => m.ReadUnsigned8(0x10), Times.Once());
        }

        #endregion

        /// <summary>
        /// Simulate a situation to set mouse coordinates.
        /// </summary>
        [TestMethod]
        public void SetMouseCoordinatesNoExtensionTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(0x36)).Returns((char)0);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetMouseCoordinates(1, 2);

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x36), Times.Once());
        }

        [TestMethod]
        public void SetMouseCoordinatesHasExtensionTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(0x36)).Returns((char)100);
            memory.Setup(m => m.ReadUnsigned16(100)).Returns((char)2);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            fileHeader.SetMouseCoordinates(1, 2);

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x36), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(100), Times.Once());
            memory.Verify(m => m.WriteUnsigned16(102, (char)1), Times.Once());
            memory.Verify(m => m.WriteUnsigned16(104, (char)2), Times.Once());
        }

        [TestMethod]
        public void GetUnicodeTranslationTableNoExtensionTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(0x36)).Returns((char)0);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.CustomAccentTableAddress;

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x36), Times.Once());
            Assert.AreEqual((char)0, result);
        }

        [TestMethod]
        public void GetCustomUnicodeTranslationTableNoTableInExtTable()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(0x36)).Returns((char)100);
            memory.Setup(m => m.ReadUnsigned16(100)).Returns((char)2);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.CustomAccentTableAddress;

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x36), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(100), Times.Once());
            Assert.AreEqual((char)0, result);
        }

        [TestMethod]
        public void GetCustomUnicodeTranslationTableHasExtAddress()
        {
            // arrange
            var memory = new Mock<IMemory>();
            memory.Setup(m => m.ReadUnsigned16(0x36)).Returns((char)100);
            memory.Setup(m => m.ReadUnsigned16(100)).Returns((char)3);
            memory.Setup(m => m.ReadUnsigned16(106)).Returns((char)1234);
            IStoryFileHeader fileHeader = new StoryFileHeader(memory.Object);

            // act
            var result = fileHeader.CustomAccentTableAddress;

            // assert
            memory.Verify(m => m.ReadUnsigned16(0x36), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(100), Times.Once());
            memory.Verify(m => m.ReadUnsigned16(106), Times.Once());
            Assert.AreEqual((char)1234, result);
        }
    }
}
