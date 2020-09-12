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
    /// Manages read and write access to the story file header.
    /// </summary>
    public class StoryFileHeader : IStoryFileHeader
    {
        /// <summary>
        /// The memory map.
        /// </summary>
        private readonly IMemory memory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.StoryFileHeader"/>
        /// class for the specified memory.
        /// </summary>
        /// <param name="Memory">The Memory object.</param>
        public StoryFileHeader(IMemory memory)
        {
            this.memory = memory;
        }

        public int Version => memory.ReadUnsigned8(0x00);

        public String SerialNumber => GetAsciiString(0x12, 6);

        public int FileLength
        {
            get
            {
                // depending on the story file version we have to multiply the
                // file length in the header by a constant
                int fileLength = memory.ReadUnsigned16(0x1a);
                if (Version <= 3)
                {
                    fileLength *= 2;
                }
                else if (Version <= 5)
                {
                    fileLength *= 4;
                }
                else
                {
                    fileLength *= 8;
                }
                return fileLength;
            }
        }

        public void SetInterpreterVersion(int version)
        {
            if (Version == 4 || Version == 5)
            {
                memory.WriteUnsigned8(0x1f, version.ToString()[0]);
            }
            else
            {
                memory.WriteUnsigned8(0x1f, (char)version);
            }
        }

        public void SetFontWidth(int units)
        {
            if (Version == 6)
            {
                memory.WriteUnsigned8(0x27, (char)units);
            }
            else
            {
                memory.WriteUnsigned8(0x26, (char)units);
            }
        }

        public void SetFontHeight(int units)
        {
            if (Version == 6)
            {
                memory.WriteUnsigned8(0x26, (char)units);
            }
            else
            {
                memory.WriteUnsigned8(0x27, (char)units);
            }
        }

        public void SetMouseCoordinates(int x, int y)
        {
            // check the extension table
            int extTable = memory.ReadUnsigned16(0x36);
            if (extTable > 0)
            {
                int numwords = memory.ReadUnsigned16(extTable);
                if (numwords >= 1)
                {
                    memory.WriteUnsigned16(extTable + 2, ToUnsigned16(x));
                }
                if (numwords >= 2)
                {
                    memory.WriteUnsigned16(extTable + 4, ToUnsigned16(y));
                }
            }
        }

        public char CustomAccentTableAddress
        {
            get
            {
                // check the extension table
                char result = (char)0;
                int extTable = memory.ReadUnsigned16(0x36);
                if (extTable > 0)
                {
                    int numwords = memory.ReadUnsigned16(extTable);
                    if (numwords >= 3)
                    {
                        result = memory.ReadUnsigned16(extTable + 6);
                    }
                }
                return result;
            }
        }

        #region Attributes

        public void SetEnabled(StoryFileHeaderAttribute attribute, bool flag)
        {
            switch (attribute)
            {
                case StoryFileHeaderAttribute.DefaultFontIsVariable:
                    DefaultFontIsVariablePitch = flag;
                    break;
                case StoryFileHeaderAttribute.Transcripting:
                    IsTranscriptingOn = flag;
                    break;
                case StoryFileHeaderAttribute.ForceFixedFont:
                    ForceFixedFont = flag;
                    break;
                case StoryFileHeaderAttribute.SupportsTimedInput:
                    SetTimedInputAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SupportsFixedFont:
                    SetFixedFontAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SupportsBold:
                    SetBoldFaceAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SupportsItalic:
                    SetItalicAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SupportsScreenSplitting:
                    SetScreenSplittingAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SupportsStatusLine:
                    SetStatusLineAvailable(flag);
                    break;
                case StoryFileHeaderAttribute.SupportsColours:
                    SetSupportsColours(flag);
                    break;
                default:
                    break;
            }
        }

        public bool IsEnabled(StoryFileHeaderAttribute attribute)
        {
            switch (attribute)
            {
                case StoryFileHeaderAttribute.Transcripting:
                    return IsTranscriptingOn;
                case StoryFileHeaderAttribute.ForceFixedFont:
                    return ForceFixedFont;
                case StoryFileHeaderAttribute.ScoreGame:
                    return IsScoreGame;
                case StoryFileHeaderAttribute.DefaultFontIsVariable:
                    return DefaultFontIsVariablePitch;
                case StoryFileHeaderAttribute.UseMouse:
                    return UseMouse;
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
                builder.Append(string.Format("Addr: {0:x2} Byte: {1:x2}\n", i, (int)memory.ReadUnsigned8(i)));
            }
            return builder.ToString();
        }

        #region Private section

        /// <summary>
        /// Gets an ASCII string of the specified length starting
        /// at the specified address.
        /// </summary>
        /// <param name="address">The start address.</param>
        /// <param name="length">The length of the ASCII string.</param>
        /// <returns>The ASCII string.</returns>
        private String GetAsciiString(int address, int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = address; i < address + length; i++)
            {
                builder.Append((char)memory.ReadUnsigned8(i));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets or sets a value indicating whether transcripting is on.
        /// </summary>
        /// <returns>true if transcripting is on; otherwise false.</returns>
        private bool IsTranscriptingOn
        {
            get
            {
                return (memory.ReadUnsigned16(0x10) & 1) > 0;
            }
            set
            {
                char flags = memory.ReadUnsigned16(0x10);
                flags = (char)(value ? (flags | 1) : (flags & 0xfe));
                memory.WriteUnsigned16(0x10, (char)flags);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether force fixed font is enabled.
        /// </summary>
        /// <returns>true if force fixed font is enabled; otherwise false.</returns>
        private bool ForceFixedFont
        {
            get
            {
                return (memory.ReadUnsigned16(0x10) & 2) > 0;
            }
            set
            {
                char flags = memory.ReadUnsigned16(0x10);
                flags = (char)(value ? (flags | 2) : (flags & 0xfd));
                memory.WriteUnsigned16(0x10, (char)flags);
            }
        }

        /// <summary>
        /// Sets a value indicating whether timed input is supported.
        /// </summary>
        /// <param name="supportsTimedInput">true to support timed input; otherwise false.</param>
        private void SetTimedInputAvailable(bool supportsTimedInput)
        {
            int flags = memory.ReadUnsigned8(0x01);
            flags = supportsTimedInput ? (flags | 128) : (flags & 0x7f);
            memory.WriteUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Gets a value indicating whether this is a "score" game.
        /// </summary>
        /// <returns>true if this is a "score" game; otherwise false.</returns>
        /// <remarks>false indicates this is a "time" game.</remarks>
        private bool IsScoreGame => (memory.ReadUnsigned8(0x01) & 2) == 0;

        /// <summary>
        /// Sets a value indicating whether fixed fonts are supported.
        /// </summary>
        /// <param name="supportsFixedFont">true to support fixed fonts; otherwise false.</param>
        private void SetFixedFontAvailable(bool supportsFixedFont)
        {
            int flags = memory.ReadUnsigned8(0x01);
            flags = supportsFixedFont ? (flags | 16) : (flags & 0xef);
            memory.WriteUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets a value indicating whether bold is supported.
        /// </summary>
        /// <param name="supportsBold">true to support bold; otherwise false.</param>
        private void SetBoldFaceAvailable(bool supportsBold)
        {
            int flags = memory.ReadUnsigned8(0x01);
            flags = supportsBold ? (flags | 4) : (flags & 0xfb);
            memory.WriteUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets a value indicating whether italic is supported.
        /// </summary>
        /// <param name="supportsItalic">true to support italic; otherwise false.</param>
        private void SetItalicAvailable(bool supportsItalic)
        {
            int flags = memory.ReadUnsigned8(0x01);
            flags = supportsItalic ? (flags | 8) : (flags & 0xf7);
            memory.WriteUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets a value indicating whether screen splitting is supported.
        /// </summary>
        /// <param name="supportsScreenSplitting">true to support screen splitting; otherwise false.</param>
        private void SetScreenSplittingAvailable(bool supportsScreenSplitting)
        {
            int flags = memory.ReadUnsigned8(0x01);
            flags = supportsScreenSplitting ? (flags | 32) : (flags & 0xdf);
            memory.WriteUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Sets a value indicating whether a status line is supported.
        /// </summary>
        /// <param name="supportsStatusLine">true to support a status line; otherwise false.</param>
        private void SetStatusLineAvailable(bool supportsStatusLine)
        {
            int flags = memory.ReadUnsigned8(0x01);
            flags = supportsStatusLine ? (flags | 16) : (flags & 0xef);
            memory.WriteUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default font is variable pitch.
        /// </summary>
        /// <returns>true if the default font is variable pitch; otherwise false.</returns>
        private bool DefaultFontIsVariablePitch
        {
            get
            {
                return (memory.ReadUnsigned8(0x01) & 64) > 0;
            }
            set
            {
                int flags = memory.ReadUnsigned8(0x01);
                flags = value ? (flags | 64) : (flags & 0xbf);
                memory.WriteUnsigned8(0x01, (char)flags);
            }
        }

        /// <summary>
        /// Sets a value indicating whether colours are supported.
        /// </summary>
        /// <param name="supportsColours">true to support colours; otherwise false.</param>
        private void SetSupportsColours(bool supportsColours)
        {
            int flags = memory.ReadUnsigned8(0x01);
            flags = supportsColours ? (flags | 1) : (flags & 0xfe);
            memory.WriteUnsigned8(0x01, (char)flags);
        }

        /// <summary>
        /// Gets a value indicating whether mouse input is supported.
        /// </summary>
        /// <returns>true if mouse input is supported; otherwise false.</returns>
        private bool UseMouse => (memory.ReadUnsigned8(0x10) & 32) > 0;

        #endregion
    }
}
