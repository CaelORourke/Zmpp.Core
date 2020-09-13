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

namespace Zmpp.Core.Encoding
{
    using System;
    using Zmpp.Core;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This class encodes ZSCII strings into dictionary encoded strings.
    /// </summary>
    /// <remarks>
    /// Encoding is pretty difficult since there are several variables to
    /// remember during the encoding process. We use the State pattern passing
    /// around the encoding state for a target word until encoding is complete.
    /// </remarks>
    public class ZCharEncoder
    {
        private const char PadChar = (char)5;
        private const int SlotsPerWord16 = 3;
        private readonly IZCharTranslator translator;
        private readonly IDictionarySizes dictionarySizes;

        /// <summary>
        /// EncodingState class.
        /// </summary>
        private class EncodingState
        {
            private IMemory memory;
            protected int source;
            private int sourceStart;
            private int maxLength;
            private int numEntryBytes;
            private int target;
            private int targetStart;

            /// <summary>
            /// currently public
            /// currentWord represents the state of the current word the encoder is
            /// working on. The encoder attempts to fill the three slots contained in
            /// this word and later writes it to the target memory address
            /// </summary>
            public int CurrentWord;

            /// <summary>
            /// The current slot position within currentWord, can be 0, 1 or 2
            /// </summary>
            public int WordPosition;

            /// <summary>
            /// Initialization.
            /// </summary>
            /// <param name="mem">The Memory object.</param>
            /// <param name="src">The source position.</param>
            /// <param name="trgt">The target position.</param>
            /// <param name="maxEntryBytes">The maximum entry bytes.</param>
            /// <param name="maxEntryChars">The maximum entry characters.</param>
            public void Init(IMemory mem, int src, int trgt, int maxEntryBytes, int maxEntryChars)
            {
                memory = mem;
                source = src;
                sourceStart = src;
                target = trgt;
                targetStart = trgt;
                numEntryBytes = maxEntryBytes;
                maxLength = maxEntryChars;
            }

            /// <summary>
            /// Gets a value indicating whether the current word was already processed.
            /// </summary>
            /// <returns>true if the current word was already processed; otherwise false.</returns>
            public bool CurrentWordWasProcessed => WordPosition > 2;

            /// <summary>
            /// Gets the target offset.
            /// </summary>
            public int TargetOffset => target - targetStart;

            /// <summary>
            /// Gets the number of entry bytes.
            /// </summary>
            public int NumEntryBytes => numEntryBytes;

            /// <summary>
            /// Gets a value indicating whether we are already at the last 16 bit word.
            /// </summary>
            /// <returns>true if we are the last 16 bit word; otherwise false.</returns>
            public bool AtLastWord16 => target > targetStart + LastWord16Offset;

            /// <summary>
            /// Gets the offset of the last 16 bit word.
            /// </summary>
            private int LastWord16Offset => numEntryBytes - 2;

            /// <summary>
            /// Gets the next character.
            /// </summary>
            public virtual char NextChar => memory.ReadUnsigned8(source++);

            /// <summary>
            /// Marks the last word.
            /// </summary>
            public void MarkLastWord()
            {
                int lastword = memory.ReadUnsigned16(targetStart + LastWord16Offset);
                memory.WriteUnsigned16(targetStart + LastWord16Offset, ToUnsigned16(lastword | 0x8000));
            }

            /// <summary>
            /// Writes the specified 16 bit value to the current memory address.
            /// </summary>
            /// <param name="value">The value to write.</param>
            public void WriteUnsigned16(char value)
            {
                memory.WriteUnsigned16(target, value);
                target += 2;
            }

            /// <summary>
            /// Gets a value indicating whether there is more input.
            /// </summary>
            /// <returns>true if there is more input; otherwise false.</returns>
            public bool HasMoreInput => source < sourceStart + maxLength;
        }

        /// <summary>
        /// Representation of StringEncodingState.
        /// </summary>
        private class StringEncodingState : EncodingState
        {
            private string input;

            /// <summary>
            /// Initialization.
            /// </summary>
            /// <param name="inputStr">The input string.</param>
            /// <param name="mem">The Memory object.</param>
            /// <param name="trgt">The target position.</param>
            /// <param name="dictionarySizes">The IDictionarySizes object.</param>
            public void Init(string inputStr, IMemory mem, int trgt, IDictionarySizes dictionarySizes)
            {
                base.Init(mem, 0, trgt, dictionarySizes.NumEntryBytes,
                Math.Min(inputStr.Length, dictionarySizes.MaxEntryChars));
                input = inputStr;
            }

            /// <summary>
            /// Gets the next character.
            /// </summary>
            public override char NextChar => input[source++];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Encoding.ZCharEncoder"/>
        /// class for the specified translator and dictionary sizes.
        /// </summary>
        /// <param name="aTranslator">The IZCharTranslator object.</param>
        /// <param name="dictSizes">The IDictionarySizes object.</param>
        public ZCharEncoder(IZCharTranslator aTranslator, IDictionarySizes dictSizes)
        {
            this.translator = aTranslator;
            this.dictionarySizes = dictSizes;
        }

        /// <summary>
        /// Encodes the Z-word at the specified memory address and writes the encoded
        /// form to the target address using the specified word length.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="length">The Z-word length.</param>
        /// <param name="targetAddress">The target address.</param>
        public void Encode(IMemory memory, int sourceAddress, int length, int targetAddress)
        {
            int maxlen = Math.Min(length, dictionarySizes.MaxEntryChars);
            EncodingState state = new EncodingState();
            state.Init(memory, sourceAddress, targetAddress, dictionarySizes.NumEntryBytes, maxlen);
            Encode(state, translator);
        }

        /// <summary>
        /// Encodes the Z-word contained in the specified string and writes it to the
        /// specified target address.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="memory">The Memory object.</param>
        /// <param name="targetAddress">The target address.</param>
        public void Encode(string str, IMemory memory, int targetAddress)
        {
            StringEncodingState state = new StringEncodingState();
            state.Init(str, memory, targetAddress, dictionarySizes);
            Encode(state, translator);
        }

        /// <summary>
        /// Encodes the string at the specified address and writes it to the target
        /// address.
        /// </summary>
        /// <param name="state">The EncodingState object.</param>
        /// <param name="translator">The IZCharTranslator object.</param>
        private static void Encode(EncodingState state, IZCharTranslator translator)
        {
            while (state.HasMoreInput)
            {
                ProcessChar(translator, state);
            }
            // Padding
            // This pads the incomplete currently encoded word
            if (!state.CurrentWordWasProcessed&& !state.AtLastWord16)
            {
                int resultword = state.CurrentWord;
                for (int i = state.WordPosition; i < SlotsPerWord16; i++)
                {
                    resultword = WriteZcharToWord(resultword, PadChar, i);
                }
                state.WriteUnsigned16(ToUnsigned16(resultword));
            }

            // If we did not encode 3 16-bit words, fill the remaining ones with
            // 0x14a5's (= 0-{5,5,5})
            while (state.TargetOffset < state.NumEntryBytes)
            {
                state.WriteUnsigned16(ToUnsigned16(0x14a5));
            }

            // Always mark the last word as such
            state.MarkLastWord();
        }

        /// <summary>
        /// Processes the current character.
        /// </summary>
        /// <param name="translator">The IZCharTranslator object.</param>
        /// <param name="state">The EncodingState object.</param>
        private static void ProcessChar(IZCharTranslator translator, EncodingState state)
        {
            char zsciiChar = state.NextChar;
            AlphabetElement element = translator.GetAlphabetElementFor(zsciiChar);
            if (element.Alphabet == Alphabet.NotSet)
            {
                char zcharCode = element.Code;
                // This is a ZMPP specialty, we do not want to end the string
                // in the middle of encoding, so we only encode if there is
                // enough space in the target (4 5-bit slots are needed to do an
                // A2-escape).
                // We might want to reconsider this, let's see, if there are problems
                // with different dictionaries
                int numRemainingSlots = GetNumRemainingSlots(state);
                if (numRemainingSlots >= 4)
                {
                    // Escape A2
                    ProcessWord(state, AlphabetTableBase.Shift5);
                    ProcessWord(state, AlphabetTableBase.A2Escape);
                    ProcessWord(state, GetUpper5Bit(zcharCode));
                    ProcessWord(state, GetLower5Bit(zcharCode));
                }
                else
                {
                    // pad remaining slots with SHIFT_5's
                    for (int i = 0; i < numRemainingSlots; i++)
                    {
                        ProcessWord(state, AlphabetTableBase.Shift5);
                    }
                }
            }
            else
            {
                Alphabet alphabet = element.Alphabet;
                char zcharCode = element.Code;
                if (alphabet == Alphabet.A1)
                {
                    ProcessWord(state, AlphabetTableBase.Shift4);
                }
                else if (alphabet == Alphabet.A2)
                {
                    ProcessWord(state, AlphabetTableBase.Shift5);
                }
                ProcessWord(state, zcharCode);
            }
        }

        /// <summary>
        /// Gets the number of remaining slots.
        /// </summary>
        /// <param name="state">The EncodingState object.</param>
        /// <returns>The number of remaining slots.</returns>
        private static int GetNumRemainingSlots(EncodingState state)
        {
            int currentWord = state.TargetOffset / 2;
            return ((2 - currentWord) * 3) + (3 - state.WordPosition);
        }

        /// <summary>
        /// Processes the current word.
        /// </summary>
        /// <param name="state">The EncodingState object.</param>
        /// <param name="value">The char value.</param>
        private static void ProcessWord(EncodingState state, char value)
        {
            state.CurrentWord = WriteZcharToWord(state.CurrentWord, value, state.WordPosition++);
            WriteWordIfNeeded(state);
        }

        /// <summary>
        /// Writes the current word if needed.
        /// </summary>
        /// <param name="state">The EncodingState object.</param>
        private static void WriteWordIfNeeded(EncodingState state)
        {
            if (state.CurrentWordWasProcessed&& !state.AtLastWord16)
            {
                // Write the result and increment the target position
                state.WriteUnsigned16(ToUnsigned16(state.CurrentWord));
                state.CurrentWord = 0;
                state.WordPosition = 0;
            }
        }

        /// <summary>
        /// Gets the upper 5 bits of the specified ZSCII character.
        /// </summary>
        /// <param name="zsciiChar">The ZSCII character.</param>
        /// <returns>The upper 5 bits.</returns>
        private static char GetUpper5Bit(char zsciiChar)
        {
            return (char)(((int)((uint)zsciiChar >> 5)) & 0x1f);
        }

        /// <summary>
        /// Gets the lower 5 bits of the specified ZSCII character.
        /// </summary>
        /// <param name="zsciiChar">The ZSCII character.</param>
        /// <returns>The lower 5 bits.</returns>
        private static char GetLower5Bit(char zsciiChar)
        {
            return (char)(zsciiChar & 0x1f);
        }

        /// <summary>
        /// Writes a zchar value to the specified position within
        /// a word.
        /// </summary>
        /// <param name="dataword">The word.</param>
        /// <param name="zchar">The character to write.</param>
        /// <param name="pos">The position. A value between 0 and 2.</param>
        /// <returns>The new word with the databyte set in the position.</returns>
        /// <remarks>
        /// There are three positions within a 16 bit word and the bytes
        /// are truncated such that only the lower 5 bit are taken as values.
        /// </remarks>
        private static char WriteZcharToWord(int dataword, char zchar, int pos)
        {
            int shiftwidth = (2 - pos) * 5;
            return (char)(dataword | ((zchar & 0x1f) << shiftwidth));
        }
    }
}
