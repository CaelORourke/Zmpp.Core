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
    using Zmpp.Core;
    using System;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This class encodes ZSCII strings into dictionary encoded strings.
    /// Encoding is pretty difficult since there are several variables to
    /// remember during the encoding process.We use the State pattern passing
    /// around the encoding state for a target word until encoding is complete.
    /// </summary>
    public class ZCharEncoder
    {
        private const char PAD_CHAR = (char)5;
        private const int SLOTS_PER_WORD16 = 3;
        private IZCharTranslator translator;
        private IDictionarySizes dictionarySizes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aTranslator">ZCharTranslator object</param>
        /// <param name="dictSizes">DictionarySizes object</param>
        public ZCharEncoder(IZCharTranslator aTranslator, IDictionarySizes dictSizes)
        {
            this.translator = aTranslator;
            this.dictionarySizes = dictSizes;
        }

        /// <summary>
        /// Encodes the Z word at the specified memory address and writes the encoded
        /// for to the target address, using the specified word length.
        /// </summary>
        /// <param name="memory">Memory object</param>
        /// <param name="sourceAddress">source address</param>
        /// <param name="length">Z-word length</param>
        /// <param name="targetAddress">target address</param>
        public void encode(IMemory memory, int sourceAddress, int length, int targetAddress)
        {
            int maxlen = Math.Min(length, dictionarySizes.getMaxEntryChars());
            EncodingState state = new EncodingState();
            state.init(memory, sourceAddress, targetAddress, dictionarySizes.getNumEntryBytes(), maxlen);
            encode(state, translator);
        }

        /// <summary>
        /// Encodes the specified Z-word contained in the String and writes it to the
        /// specified target address.
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="memory">Memory object</param>
        /// <param name="targetAddress">target address</param>
        public void encode(String str, IMemory memory, int targetAddress)
        {
            StringEncodingState state = new StringEncodingState();
            state.init(str, memory, targetAddress, dictionarySizes);
            encode(state, translator);
        }

        /// <summary>
        /// Encodes the string at the specified address and writes it to the target
        /// address.
        /// </summary>
        /// <param name="state">EncodingState</param>
        /// <param name="translator">ZCharTranslator</param>
        private static void encode(EncodingState state, IZCharTranslator translator)
        {
            while (state.hasMoreInput())
            {
                processChar(translator, state);
            }
            // Padding
            // This pads the incomplete currently encoded word
            if (!state.currentWordWasProcessed() && !state.atLastWord16())
            {
                int resultword = state.currentWord;
                for (int i = state.wordPosition; i < SLOTS_PER_WORD16; i++)
                {
                    resultword = writeZcharToWord(resultword, PAD_CHAR, i);
                }
                state.writeUnsigned16(toUnsigned16(resultword));
            }

            // If we did not encode 3 16-bit words, fill the remaining ones with
            // 0x14a5's (= 0-{5,5,5})
            while (state.getTargetOffset() < state.getNumEntryBytes())
            {
                state.writeUnsigned16(toUnsigned16(0x14a5));
            }

            // Always mark the last word as such
            state.markLastWord();
        }

        /// <summary>
        /// Processes the current character.
        /// </summary>
        /// <param name="translator">ZCharTranslator object</param>
        /// <param name="state">the EncodingState</param>
        private static void processChar(IZCharTranslator translator, EncodingState state)
        {
            char zsciiChar = state.nextChar();
            AlphabetElement element = translator.getAlphabetElementFor(zsciiChar);
            if (element.getAlphabet() == Alphabet.Unknown)
            {
                char zcharCode = element.getZCharCode();
                // This is a ZMPP specialty, we do not want to end the string
                // in the middle of encoding, so we only encode if there is
                // enough space in the target (4 5-bit slots are needed to do an
                // A2-escape).
                // We might want to reconsider this, let's see, if there are problems
                // with different dictionaries
                int numRemainingSlots = getNumRemainingSlots(state);
                if (numRemainingSlots >= 4)
                {
                    // Escape A2
                    processWord(state, AlphabetTableBase.SHIFT_5);
                    processWord(state, AlphabetTableBase.A2_ESCAPE);
                    processWord(state, getUpper5Bit(zcharCode));
                    processWord(state, getLower5Bit(zcharCode));
                }
                else
                {
                    // pad remaining slots with SHIFT_5's
                    for (int i = 0; i < numRemainingSlots; i++)
                    {
                        processWord(state, AlphabetTableBase.SHIFT_5);
                    }
                }
            }
            else
            {
                Alphabet alphabet = element.getAlphabet();
                char zcharCode = element.getZCharCode();
                if (alphabet == Alphabet.A1)
                {
                    processWord(state, AlphabetTableBase.SHIFT_4);
                }
                else if (alphabet == Alphabet.A2)
                {
                    processWord(state, AlphabetTableBase.SHIFT_5);
                }
                processWord(state, zcharCode);
            }
        }

        /// <summary>
        /// Returns the number of remaining slots.
        /// </summary>
        /// <param name="state">the EncodingState</param>
        /// <returns>number of remaining slots</returns>
        private static int getNumRemainingSlots(EncodingState state)
        {
            int currentWord = state.getTargetOffset() / 2;
            return ((2 - currentWord) * 3) + (3 - state.wordPosition);
        }

        /// <summary>
        /// Processes the current word.
        /// </summary>
        /// <param name="state">the EncodingState</param>
        /// <param name="value">the char value</param>
        private static void processWord(EncodingState state, char value)
        {
            state.currentWord = writeZcharToWord(state.currentWord, value, state.wordPosition++);
            writeWordIfNeeded(state);
        }

        /// <summary>
        /// Writes the current word if needed.
        /// </summary>
        /// <param name="state">the EncodingState</param>
        private static void writeWordIfNeeded(EncodingState state)
        {
            if (state.currentWordWasProcessed() && !state.atLastWord16())
            {
                // Write the result and increment the target position
                state.writeUnsigned16(toUnsigned16(state.currentWord));
                state.currentWord = 0;
                state.wordPosition = 0;
            }
        }

        /// <summary>
        /// Retrieves the upper 5 bit of the specified ZSCII character.
        /// </summary>
        /// <param name="zsciiChar">the ZSCII character</param>
        /// <returns>the upper 5 bit</returns>
        private static char getUpper5Bit(char zsciiChar)
        {
            return (char)(((int)((uint)zsciiChar >> 5)) & 0x1f);
        }

        /// <summary>
        /// Retrieves the lower 5 bit of the specified ZSCII character.
        /// </summary>
        /// <param name="zsciiChar">the ZSCII character</param>
        /// <returns>the lower 5 bit</returns>
        private static char getLower5Bit(char zsciiChar)
        {
            return (char)(zsciiChar & 0x1f);
        }

        /// <summary>
        /// This function sets a zchar value to the specified position within
        /// a word.There are three positions within a 16 bit word and the bytes
        /// are truncated such that only the lower 5 bit are taken as values.
        /// </summary>
        /// <param name="dataword">the word to set</param>
        /// <param name="zchar">the character to set</param>
        /// <param name="pos">a value between 0 and 2</param>
        /// <returns>the new word with the databyte set in the position</returns>
        private static char writeZcharToWord(int dataword, char zchar, int pos)
        {
            int shiftwidth = (2 - pos) * 5;
            return (char)(dataword | ((zchar & 0x1f) << shiftwidth));
        }
    }

    /// <summary>
    /// EncodingState class.
    /// </summary>
    class EncodingState
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
        public int currentWord;

        /// <summary>
        /// The current slot position within currentWord, can be 0, 1 or 2
        /// </summary>
        public int wordPosition;

        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="mem">memory object</param>
        /// <param name="src">source position</param>
        /// <param name="trgt">target position</param>
        /// <param name="maxEntryBytes">maximum entry bytes</param>
        /// <param name="maxEntryChars">maximum entry characters</param>
        public void init(IMemory mem, int src, int trgt, int maxEntryBytes, int maxEntryChars)
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
        /// Indicates whether the current word was already processed.
        /// </summary>
        /// <returns>true if word was processed</returns>
        public bool currentWordWasProcessed() { return wordPosition > 2; }

        /// <summary>
        /// Returns the target offset.
        /// </summary>
        /// <returns>target offset</returns>
        public int getTargetOffset() { return target - targetStart; }

        /// <summary>
        /// Returns the number of entry bytes.
        /// </summary>
        /// <returns>number of entry bytes</returns>
        public int getNumEntryBytes() { return numEntryBytes; }

        /// <summary>
        /// Determines whether we are already at the last 16-bit word.
        /// </summary>
        /// <returns>true if at the end, false else</returns>
        public bool atLastWord16()
        {
            return target > targetStart + getLastWord16Offset();
        }

        /// <summary>
        /// Returns the offset of the last 16 bit word.
        /// </summary>
        /// <returns>offset of the last 16 bit word</returns>
        private int getLastWord16Offset() { return numEntryBytes - 2; }

        /// <summary>
        /// Returns the next character.
        /// </summary>
        /// <returns>next character</returns>
        public virtual char nextChar() { return memory.readUnsigned8(source++); }

        /// <summary>
        /// Marks the last word.
        /// </summary>
        public void markLastWord()
        {
            int lastword =
                memory.readUnsigned16(targetStart + getLastWord16Offset());
            memory.writeUnsigned16(targetStart + getLastWord16Offset(), toUnsigned16(lastword | 0x8000));
        }

        /// <summary>
        /// Writes the specified 16 bit value to the current memory address.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void writeUnsigned16(char value)
        {
            memory.writeUnsigned16(target, value);
            target += 2;
        }

        /// <summary>
        /// Determines whether there is more input.
        /// </summary>
        /// <returns>true if more input, false otherwise</returns>
        public bool hasMoreInput()
        {
            return source < sourceStart + maxLength;
        }
    }

    /// <summary>
    /// Representation of StringEncodingState.
    /// </summary>
    class StringEncodingState : EncodingState
    {
        private String input;

        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="inputStr">input string</param>
        /// <param name="mem">memory object</param>
        /// <param name="trgt">target position</param>
        /// <param name="dictionarySizes">DictionarySizes object</param>
        public void init(String inputStr, IMemory mem, int trgt, IDictionarySizes dictionarySizes)
        {
            base.init(mem, 0, trgt, dictionarySizes.getNumEntryBytes(),
            Math.Min(inputStr.Length, dictionarySizes.getMaxEntryChars()));
            input = inputStr;
        }

        /// <summary>
        /// Retrieve to next character.
        /// </summary>
        /// <returns>next character</returns>
        public override char nextChar() { return input[source++]; }
    }
}
