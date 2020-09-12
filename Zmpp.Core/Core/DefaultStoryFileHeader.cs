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

namespace Zmpp.Core
{
    using System;
    using System.Text;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This is the default implementation of the StoryFileHeader interface.
    /// </summary>
    public class DefaultStoryFileHeader : StoryFileHeaderBase, IStoryFileHeader
    {
        /// <summary>
        /// The memory map.
        /// </summary>
        private IMemory memory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Memory">a Memory object</param>
        /// <param name=""></param>
        public DefaultStoryFileHeader(IMemory memory)
        {
            this.memory = memory;
        }

        public int getVersion() { return memory.readUnsigned8(0x00); }

        public String getSerialNumber() { return extractAscii(0x12, 6); }

        public int getFileLength()
        {
            // depending on the story file version we have to multiply the
            // file length in the header by a constant
            int fileLength = memory.readUnsigned16(0x1a);
            if (getVersion() <= 3)
            {
                fileLength *= 2;
            }
            else if (getVersion() <= 5)
            {
                fileLength *= 4;
            }
            else
            {
                fileLength *= 8;
            }
            return fileLength;
        }

        public void setInterpreterVersion(int version)
        {
            if (getVersion() == 4 || getVersion() == 5)
            {
                memory.writeUnsigned8(0x1f, version.ToString()[0]);
            }
            else
            {
                memory.writeUnsigned8(0x1f, (char)version);
            }
        }

        public void setFontWidth(int units)
        {
            if (getVersion() == 6)
            {
                memory.writeUnsigned8(0x27, (char)units);
            }
            else
            {
                memory.writeUnsigned8(0x26, (char)units);
            }
        }

        public void setFontHeight(int units)
        {
            if (getVersion() == 6)
            {
                memory.writeUnsigned8(0x26, (char)units);
            }
            else
            {
                memory.writeUnsigned8(0x27, (char)units);
            }
        }

        public void setMouseCoordinates(int x, int y)
        {
            // check the extension table
            int extTable = memory.readUnsigned16(0x36);
            if (extTable > 0)
            {
                int numwords = memory.readUnsigned16(extTable);
                if (numwords >= 1)
                {
                    memory.writeUnsigned16(extTable + 2, toUnsigned16(x));
                }
                if (numwords >= 2)
                {
                    memory.writeUnsigned16(extTable + 4, toUnsigned16(y));
                }
            }
        }

        public char getCustomAccentTable()
        {
            // check the extension table
            char result = (char)0;
            int extTable = memory.readUnsigned16(0x36);
            if (extTable > 0)
            {
                int numwords = memory.readUnsigned16(extTable);
                if (numwords >= 3)
                {
                    result = memory.readUnsigned16(extTable + 6);
                }
            }
            return result;
        }

        #region Attributes
        public void setEnabled(StoryFileHeaderAttribute attribute, bool flag)
        {
            switch (attribute)
            {
                case StoryFileHeaderAttribute.DEFAULT_FONT_IS_VARIABLE:
                    setDefaultFontIsVariablePitch(flag);
                    break;
                case StoryFileHeaderAttribute.TRANSCRIPTING:
                    setTranscripting(flag);
                    break;
                case StoryFileHeaderAttribute.FORCE_FIXED_FONT:
                    setForceFixedFont(flag);
                    break;
                case StoryFileHeaderAttribute.SUPPORTS_TIMED_INPUT:
                    setTimedInputAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SUPPORTS_FIXED_FONT:
                    setFixedFontAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SUPPORTS_BOLD:
                    setBoldFaceAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SUPPORTS_ITALIC:
                    setItalicAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SUPPORTS_SCREEN_SPLITTING:
                    setScreenSplittingAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SUPPORTS_STATUSLINE:
                    setStatusLineAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SUPPORTS_COLOURS:
                    setSupportsColours(flag);
                    break;
                default:
                    break;
            }
        }

        public bool isEnabled(StoryFileHeaderAttribute attribute)
        {
            switch (attribute)
            {
                case StoryFileHeaderAttribute.TRANSCRIPTING:
                    return isTranscriptingOn();
                case StoryFileHeaderAttribute.FORCE_FIXED_FONT:
                    return forceFixedFont();
                case StoryFileHeaderAttribute.SCORE_GAME:
                    return isScoreGame();
                case StoryFileHeaderAttribute.DEFAULT_FONT_IS_VARIABLE:
                    return defaultFontIsVariablePitch();
                case StoryFileHeaderAttribute.USE_MOUSE:
                    return useMouse();
                default:
                    return false;
            }
        }
        #endregion

        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 55; i++)
            {
                builder.Append(String.Format("Addr: %02x Byte: %02x\n", i, (int)memory.readUnsigned8(i)));
            }
            return builder.ToString();
        }

        #region Private section
        /// <summary>
        /// Extract an ASCII string of the specified length starting at the specified
        /// address.
        /// </summary>
        /// <param name="address">the start address</param>
        /// <param name="length">the length of the ASCII string</param>
        /// <returns>the ASCII string at the specified position</returns>
        private String extractAscii(int address, int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = address; i < address + length; i++)
            {

                builder.Append((char)memory.readUnsigned8(i));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Sets the state of the transcript stream.
        /// </summary>
        /// <param name="flag">new transcript state</param>
        private void setTranscripting(bool flag)
        {
            char flags = memory.readUnsigned16(0x10);
            flags = (char)(flag ? (flags | 1) : (flags & 0xfe));
            memory.writeUnsigned16(0x10, (char)flags);
        }

        /// <summary>
        /// Returns the state of the transcript stream.
        /// </summary>
        /// <returns>transcript state</returns>
        private bool isTranscriptingOn()
        {
            return (memory.readUnsigned16(0x10) & 1) > 0;
        }

        /// <summary>
        /// Returns state of the force fixed font flag.
        /// </summary>
        /// <returns>true if force fixed font, false otherwise</returns>
        private bool forceFixedFont()
        {
            return (memory.readUnsigned16(0x10) & 2) > 0;
        }

        /// <summary>
        /// Sets the force fixed font flag.
        /// </summary>
        /// <param name="flag">true if fixed font forced, false otherwise</param>
        private void setForceFixedFont(bool flag)
        {
            char flags = memory.readUnsigned16(0x10);
            flags = (char)(flag ? (flags | 2) : (flags & 0xfd));
            memory.writeUnsigned16(0x10, (char)flags);
        }

        /// <summary>
        /// Sets the timed input availability flag.
        /// </summary>
        /// <param name="flag">true if timed input available, false otherwise</param>
        private void setTimedInputAvailable(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 128) : (flags & 0x7f);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Determine whether this game is a "score" game or a "time" game.
        /// </summary>
        /// <returns>true if score game, false if time game</returns>
        private bool isScoreGame()
        {
            return (memory.readUnsigned8(0x01) & 2) == 0;
        }

        /// <summary>
        /// Sets the fixed font availability flag.
        /// </summary>
        /// <param name="flag">true if fixed font available, false otherwise</param>
        private void setFixedFontAvailable(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 16) : (flags & 0xef);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets the bold supported flag.
        /// </summary>
        /// <param name="flag">true if bold supported, false otherwise</param>
        private void setBoldFaceAvailable(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 4) : (flags & 0xfb);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets the italic supported flag.
        /// </summary>
        /// <param name="flag">true if italic supported, false otherwise</param>
        private void setItalicAvailable(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 8) : (flags & 0xf7);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets the screen splitting availability flag.
        /// </summary>
        /// <param name="flag">true if splitting supported, false otherwise</param>
        private void setScreenSplittingAvailable(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 32) : (flags & 0xdf);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets the flag whether a status line is available or not.
        /// </summary>
        /// <param name="flag">true if status line available, false otherwise</param>
        private void setStatusLineAvailable(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 16) : (flags & 0xef);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets the state whether the default font is variable or not.
        /// </summary>
        /// <param name="flag">true if default font is variable, false otherwise</param>
        private void setDefaultFontIsVariablePitch(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 64) : (flags & 0xbf);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Returns whether default font is variable pitch.
        /// </summary>
        /// <returns>true if variable pitch, false otherwise</returns>
        private bool defaultFontIsVariablePitch()
        {
            return (memory.readUnsigned8(0x01) & 64) > 0;
        }

        /// <summary>
        /// Returns the status of the supports color flag.
        /// </summary>
        /// <param name="flag">state of supports color flag</param>
        private void setSupportsColours(bool flag)
        {
            int flags = memory.readUnsigned8(0x01);
            flags = flag ? (flags | 1) : (flags & 0xfe);
            memory.writeUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Returns the status of the use mouse flag.
        /// </summary>
        /// <returns>the use mouse flag</returns>
        private bool useMouse()
        {
            return (memory.readUnsigned8(0x10) & 32) > 0;
        }
        #endregion
    }
}
