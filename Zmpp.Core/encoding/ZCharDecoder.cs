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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Zmpp.Core;

    /// <summary>
    /// This is the default implementation of the ZCharDecoder interface.
    /// </summary>
    /// <remarks>
    /// The central method is decode2Unicode which handles abbreviations,
    /// 10 Bit escape characters and alphabet table characters.Alphabet
    /// table characters and shift states are handled by the ZCharTranslator
    /// object.
    /// </remarks>
    public sealed class ZCharDecoder : IZCharDecoder
    {
        private readonly IZCharTranslator translator;
        private readonly IZsciiEncoding encoding;
        private readonly IAbbreviationsTable abbreviations;
        private IZCharDecoder abbreviationDecoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Encoding.ZCharDecoder"/>
        /// class for the specified encoding, translator, and abbreviation table.
        /// </summary>
        /// <param name="encoding">The IZsciiEncoding object</param>
        /// <param name="translator">The IZCharTranslator object.</param>
        /// <param name="abbreviations">The abbreviations table.</param>
        public ZCharDecoder(IZsciiEncoding encoding, IZCharTranslator translator, IAbbreviationsTable abbreviations)
        {
            this.abbreviations = abbreviations;
            this.translator = translator;
            this.encoding = encoding;
        }

        /// <summary>
        /// Performs a ZSCII decoding at the specified position of
        /// the given memory object.
        /// </summary>
        /// <param name="memory">The Memory object</param>
        /// <param name="address">The address of the string.</param>
        /// <param name="length">The maximum length in bytes.</param>
        /// <returns>The decoded string.</returns>
        /// <remarks>
        /// This method is exclusively designed to
        /// deal with the problems of dictionary entries.These can be cropped,
        /// leaving the string in a state, that can not be decoded properly
        /// otherwise.If the provided length is 0, the semantics are
        /// equal to the method without the length parameter.
        /// </remarks>
        public string Decode2Zscii(IMemory memory, int address, int length)
        {
            StringBuilder builder = new StringBuilder();
            translator.Reset();
            char[] zbytes = ExtractZbytes(memory, address, length);
            char zchar;
            int i = 0, newpos;

            while (i < zbytes.Length)
            {
                bool decoded = false;
                zchar = zbytes[i];
                newpos = HandleAbbreviation(builder, memory, zbytes, i);
                decoded = (newpos > i);
                i = newpos;

                if (!decoded)
                {
                    newpos = HandleEscapeA2(builder, zbytes, i);
                    decoded = newpos > i;
                    i = newpos;
                }
                if (!decoded)
                {
                    DecodeZchar(builder, zchar);
                    i++;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Process the abbreviation at the specified memory position.
        /// </summary>
        /// <param name="builder">The StringBuilder object.</param>
        /// <param name="memory">The Memory object.</param>
        /// <param name="data">The byte data.</param>
        /// <param name="pos">The original position.</param>
        /// <returns>The new position.</returns>
        private int HandleAbbreviation(StringBuilder builder, IMemory memory, char[] data, int pos)
        {
            int position = pos;
            char zchar = data[position];

            if (translator.IsAbbreviation(zchar))
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
                        int entryAddress = abbreviations.GetWordAddress(entryNum);
                        CreateAbbreviationDecoderIfNotExists();
                        AppendAbbreviationAtAddress(memory, entryAddress, builder);
                    }
                }
                position++;
            }
            return position;
        }

        /// <summary>
        /// Creates the abbreviation decoder if it does not exist.
        /// </summary>
        private void CreateAbbreviationDecoderIfNotExists()
        {
            // TODO: How can we do this in a more elegant way?
            if (abbreviationDecoder == null)
            {

                // We only use one abbreviation decoder instance here, we need
                // to clone the alphabet table, so the abbreviation decoding
                // will not influence the continuation of the decoding process
                try
                {
                    abbreviationDecoder = new ZCharDecoder(encoding, (IZCharTranslator)translator.Clone(), null);
                }
                catch (NotSupportedException)
                {
                    // should never happen
                }
            }
        }

        /// <summary>
        /// Appends the abbreviation at the specified memory address to the
        /// StringBuilder.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="entryAddress">The entry address.</param>
        /// <param name="builder">The StringBuilder object.</param>
        private void AppendAbbreviationAtAddress(IMemory memory, int entryAddress, StringBuilder builder)
        {
            if (abbreviationDecoder != null)
            {
                string abbrev = abbreviationDecoder.Decode2Zscii(memory, entryAddress, 0);
                builder.Append(abbrev);
            }
        }

        /// <summary>
        /// Handles the escape character from alphabet 2 and appends the result
        /// to the StringBuidler.
        /// </summary>
        /// <param name="builder">The StringBuilder object.</param>
        /// <param name="data">The byte data.</param>
        /// <param name="pos">The old position.</param>
        /// <returns>The new position.</returns>
        private int HandleEscapeA2(StringBuilder builder, char[] data, int pos)
        {
            int position = pos;
            if (translator.WillEscapeA2(data[position]))
            {

                // If the data is truncated, do not continue (check if the
                // constant should be 2 or 3)
                if (position < data.Length - 2)
                {
                    JoinToZsciiChar(builder, data[position + 1], data[position + 2]);
                    // skip the three characters read (including the loop increment)
                    position += 2;
                }
                position++;
                translator.ResetToLastAlphabet();
            }
            return position;
        }

        public char DecodeZChar(char zchar)
        {
            if (ZsciiEncoding.IsAscii(zchar) || ZsciiEncoding.IsAccent(zchar))
            {
                return zchar;
            }
            else
            {
                return translator.Translate(zchar);
            }
        }

        /// <summary>
        /// Decodes the specified encoded character and adds it to the specified builder object.
        /// </summary>
        /// <param name="builder">The StringBuilder object.</param>
        /// <param name="zchar">The encoded character.</param>
        /// <returns>The decoded character.</returns>
        private char DecodeZchar(StringBuilder builder, char zchar)
        {
            char c = DecodeZChar(zchar);
            if (c != 0) builder.Append(c);
            return c;
        }

        public IZCharTranslator Translator => translator;

        /// <summary>
        /// Indicates whether the specified word is the last word in a z sequence.
        /// </summary>
        /// <param name="zword">The Z-word.</param>
        /// <returns>true if the specified word is the last word in a z sequence; otherwise false.</returns>
        /// <remarks>The last word in a z sequence has the MSB set.</remarks>
        public static bool IsEndWord(char zword)
        {
            return (zword & 0x8000) > 0;
        }

        /// <summary>
        /// This function unfortunately generates a List object on each invocation,
        /// the advantage is that it will return all the characters of the Z-string.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The address of the Z-string.</param>
        /// <param name="length">The maximum length that the array should have or 0 for unspecified.</param>
        /// <returns>The Z-characters of the string.</returns>
        public static char[] ExtractZbytes(IMemory memory, int address, int length)
        {
            char zword = (char)0;
            int currentAddr = address;
            IList<char[]> byteList = new List<char[]>();

            do
            {
                zword = memory.ReadUnsigned16(currentAddr);
                byteList.Add(ExtractZEncodedBytes(zword));
                currentAddr += 2; // increment pointer

                // if this is a dictionary entry, we need to provide the
                // length and cancel the loop earlier
                if (length > 0 && (currentAddr - address) >= length)
                {
                    break;
                }
            } while (!IsEndWord(zword));

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

        public int GetNumZEncodedBytes(IMemory memory, int address)
        {
            char zword = (char)0;
            int currentAddress = address;
            do
            {
                zword = memory.ReadUnsigned16(currentAddress);
                currentAddress += 2;
            } while (!IsEndWord(zword));
            return currentAddress - address;
        }

        #region Private

        /// <summary>
        /// Extracts three 5 bit fields from the given 16 bit word and returns
        /// an array of three bytes containing these characters.
        /// </summary>
        /// <param name="zword">The 16 bit word.</param>
        /// <returns>The array of three bytes containing the three 5-bit ZSCII characters encoded in the word.</returns>
        private static char[] ExtractZEncodedBytes(char zword)
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
        /// <param name="builder">The StringBuilder object.</param>
        /// <param name="top">The byte holding the top 5 bits of the Z-character.</param>
        /// <param name="bottom">The byte holding the bottom 5 bits of the Z-character.</param>
        private void JoinToZsciiChar(StringBuilder builder, char top, char bottom)
        {
            builder.Append((char)(top << 5 | bottom));
        }

        #endregion
    }
}
