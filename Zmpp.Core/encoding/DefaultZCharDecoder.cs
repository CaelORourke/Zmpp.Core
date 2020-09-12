/*
 * Created on 2006/01/09
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
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This is the default implementation of the ZCharDecoder interface.
    /// The central method is decode2Unicode which handles abbreviations,
    /// 10 Bit escape characters and alphabet table characters.Alphabet
    /// table characters and shift states are handled by the ZCharTranslator
    /// object.
    /// </summary>
    public sealed class DefaultZCharDecoder : IZCharDecoder
    {
        private IZCharTranslator translator;
        private IZsciiEncoding encoding;
        private IZCharDecoder.AbbreviationsTable abbreviations;
        private IZCharDecoder abbreviationDecoder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="encoding">the ZsciiEncoding object</param>
        /// <param name="translator">the ZStringTranslator object</param>
        /// <param name="abbreviations">the abbreviations table used for decoding</param>
        public DefaultZCharDecoder(IZsciiEncoding encoding, IZCharTranslator translator, IZCharDecoder.AbbreviationsTable abbreviations)
        {
            this.abbreviations = abbreviations;
            this.translator = translator;
            this.encoding = encoding;
        }

        /// <summary>
        /// Performs a ZSCII decoding at the specified position of
        /// the given memory object, this method is exclusively designed to
        /// deal with the problems of dictionary entries.These can be cropped,
        /// leaving the string in a state, that can not be decoded properly
        /// otherwise.If the provided length is 0, the semantics are
        /// equal to the method without the length parameter.
        /// </summary>
        /// <param name="memory">a Memory object</param>
        /// <param name="address">the address of the string</param>
        /// <param name="length">the maximum length in bytes</param>
        /// <returns>the decoded string</returns>
        public String decode2Zscii(IMemory memory, int address, int length)
        {
            StringBuilder builder = new StringBuilder();
            translator.reset();
            char[] zbytes = extractZbytes(memory, address, length);
            char zchar;
            int i = 0, newpos;

            while (i < zbytes.Length)
            {
                bool decoded = false;
                zchar = zbytes[i];
                newpos = handleAbbreviation(builder, memory, zbytes, i);
                decoded = (newpos > i);
                i = newpos;

                if (!decoded)
                {
                    newpos = handleEscapeA2(builder, zbytes, i);
                    decoded = newpos > i;
                    i = newpos;
                }
                if (!decoded)
                {
                    decodeZchar(builder, zchar);
                    i++;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Process the abbreviation at the specified memory position.
        /// </summary>
        /// <param name="builder">StringBuilder</param>
        /// <param name="memory">memory object</param>
        /// <param name="data">byte data</param>
        /// <param name="pos">original position</param>
        /// <returns>new position</returns>
        private int handleAbbreviation(StringBuilder builder, IMemory memory, char[] data, int pos)
        {
            int position = pos;
            char zchar = data[position];

            if (translator.isAbbreviation(zchar))
            {

                // we need to check if we are at the end of the buffer, even if an
                // abbreviation is suggested. This happens e.g. in Zork I
                if (position < (data.Length - 1))
                {
                    position++; // retrieve the next byte to determine the abbreviation

                    // the abbreviations table could be null, simply skip that part in this
                    // case
                    if (abbreviations != null)
                    {
                        int x = data[position];
                        int entryNum = 32 * (zchar - 1) + x;
                        int entryAddress = abbreviations.getWordAddress(entryNum);
                        createAbbreviationDecoderIfNotExists();
                        appendAbbreviationAtAddress(memory, entryAddress, builder);
                    }
                }
                position++;
            }
            return position;
        }

        /// <summary>
        /// Creates the abbreviation decoder if it does not exist.
        /// </summary>
        private void createAbbreviationDecoderIfNotExists()
        {
            // TODO: How can we do this in a more elegant way?
            if (abbreviationDecoder == null)
            {

                // We only use one abbreviation decoder instance here, we need
                // to clone the alphabet table, so the abbreviation decoding
                // will not influence the continuation of the decoding process
                try
                {
                    abbreviationDecoder = new DefaultZCharDecoder(encoding,
                            (IZCharTranslator)translator.Clone(), null);
                }
                catch (NotSupportedException ex)
                {
                    // should never happen
                    //ex.printStackTrace();
                }
            }
        }

        /// <summary>
        /// Appends the abbreviation at the specified memory address to the
        /// StringBuilder.
        /// </summary>
        /// <param name="memory">Memory object</param>
        /// <param name="entryAddress">entry address</param>
        /// <param name="builder">StringBuilder to append to</param>
        private void appendAbbreviationAtAddress(IMemory memory, int entryAddress, StringBuilder builder)
        {
            if (abbreviationDecoder != null)
            {
                String abbrev = abbreviationDecoder.decode2Zscii(memory,
                  entryAddress, 0);
                builder.Append(abbrev);
            }
        }

        /// <summary>
        /// Handles the escape character from alphabet 2 and appends the result
        /// to the StringBuidler.
        /// </summary>
        /// <param name="builder">a StringBuilder to append to</param>
        /// <param name="data">byte data</param>
        /// <param name="pos">old position</param>
        /// <returns>new position</returns>
        private int handleEscapeA2(StringBuilder builder, char[] data, int pos)
        {
            int position = pos;
            if (translator.willEscapeA2(data[position]))
            {

                // If the data is truncated, do not continue (check if the
                // constant should be 2 or 3)
                if (position < data.Length - 2)
                {
                    joinToZsciiChar(builder, data[position + 1], data[position + 2]);
                    // skip the three characters read (including the loop increment)
                    position += 2;
                }
                position++;
                translator.resetToLastAlphabet();
            }
            return position;
        }

        public char decodeZChar(char zchar)
        {
            if (ZsciiEncoding.isAscii(zchar) || ZsciiEncoding.isAccent(zchar))
            {
                return zchar;
            }
            else
            {
                return translator.translate(zchar);
            }
        }

        /// <summary>
        /// Decodes an encoded character and adds it to the specified builder object.
        /// </summary>
        /// <param name="builder">a ZsciiStringBuilder object</param>
        /// <param name="zchar">the encoded character to decode and add</param>
        /// <returns>decoded character</returns>
        private char decodeZchar(StringBuilder builder, char zchar)
        {
            char c = decodeZChar(zchar);
            if (c != 0) builder.Append(c);
            return c;
        }

        public IZCharTranslator getTranslator() { return translator; }

        /// <summary>
        /// Determines the last word in a z sequence. The last word has the
        /// MSB set.
        /// </summary>
        /// <param name="zword">the zword</param>
        /// <returns>true if zword is the last word, false, otherwise</returns>
        public static bool isEndWord(char zword)
        {
            return (zword & 0x8000) > 0;
        }

        /// <summary>
        /// This function unfortunately generates a List object on each invocation,
        /// the advantage is that it will return all the characters of the Z string.
        /// </summary>
        /// <param name="memory">the memory access object</param>
        /// <param name="address">the address of the z string</param>
        /// <param name="length">the maximum length that the array should have or 0 for unspecified</param>
        /// <returns>the z characters of the string</returns>
        public static char[] extractZbytes(IMemory memory, int address, int length)
        {
            char zword = (char)0;
            int currentAddr = address;
            IList<char[]> byteList = new List<char[]>();

            do
            {
                zword = memory.ReadUnsigned16(currentAddr);
                byteList.Add(extractZEncodedBytes(zword));
                currentAddr += 2; // increment pointer

                // if this is a dictionary entry, we need to provide the
                // length and cancel the loop earlier
                if (length > 0 && (currentAddr - address) >= length)
                {
                    break;
                }
            } while (!isEndWord(zword));

            char[] result = new char[byteList.Count * 3];
            int i = 0;
            foreach (char[] triplet in byteList)
            {
                foreach (char b in triplet)
                {
                    result[i++] = (char)b;
                }
            }
            return result;
        }

        public int getNumZEncodedBytes(IMemory memory, int address)
        {
            char zword = (char)0;
            int currentAddress = address;
            do
            {
                zword = memory.ReadUnsigned16(currentAddress);
                currentAddress += 2;
            } while (!isEndWord(zword));
            return currentAddress - address;
        }

        #region Private
        /// <summary>
        /// Extracts three 5 bit fields from the given 16 bit word and returns
        /// an array of three bytes containing these characters.
        /// </summary>
        /// <param name="zword">a 16 bit word</param>
        /// <returns>an array of three bytes containing the three 5-bit ZSCII characters encoded in the word</returns>
        private static char[] extractZEncodedBytes(char zword)
        {
            char[] result = new char[3];
            result[2] = (char)(zword & 0x1f);
            result[1] = (char)((zword >> 5) & 0x1f);
            result[0] = (char)((zword >> 10) & 0x1f);
            return result;
        }

        /// <summary>
        /// Joins the specified two bytes into a 10 bit ZSCII character.
        /// </summary>
        /// <param name="builder">the StringBuilder to write to</param>
        /// <param name="top">the byte holding the top 5 bit of the zchar</param>
        /// <param name="bottom">the byte holding the bottom 5 bit of the zchar</param>
        private void joinToZsciiChar(StringBuilder builder, char top, char bottom)
        {
            builder.Append((char)(top << 5 | bottom));
        }
        #endregion
    }
}
