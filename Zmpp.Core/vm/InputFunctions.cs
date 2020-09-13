/*
 * Created on 12/22/2005
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

namespace Zmpp.Core.Vm
{
    using Zmpp.Core.Encoding;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This class contains functions that deal with user input.
    /// Note: For version 1.5 a number of changes will be performed on this
    /// class. Timed input will be eliminated completely, as well as leftover.
    /// Command history might be left out as well
    /// </summary>
    public class InputFunctions
    {
        private IMachine machine;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">the machine object</param>
        public InputFunctions(IMachine machine)
        {
            this.machine = machine;
        }

        // *********************************************************************
        // ****** SREAD/AREAD - the most complex and flexible function within the
        // ****** Z-machine. This function takes input from the user and
        // ****** calls the tokenizer for lexical analysis. It also recognizes
        // ****** terminator characters and controls the output as well as
        // ****** calling an optional interrupt routine.
        // *********************************************************************

        /// <summary>
        /// By delegating responsibility for timed input to the user interface,
        /// reading input is strongly simplified.
        /// </summary>
        /// <param name="textbuffer">text buffer address</param>
        /// <returns>terminator character</returns>
        public char readLine(int textbuffer)
        {
            String inputLine = machine.getSelectedInputStream().readLine();
            processInput(textbuffer, inputLine);
            return inputLine[inputLine.Length - 1];
        }

        /// <summary>
        /// Depending on the terminating character and the story file version,
        /// either write a 0 to the end of the text buffer or write the length
        /// of to the text buffer's first byte.
        /// </summary>
        /// <param name="terminateChar">the terminating character</param>
        /// <param name="textbuffer">the text buffer</param>
        /// <param name="textpointer">points at the position behind the last input char</param>
        public void checkTermination(char terminateChar, int textbuffer, int textpointer)
        {
            int version = machine.getVersion();
            if (version >= 5)
            {
                // Check if was cancelled
                char numCharsTyped = (terminateChar == ZsciiEncoding.Null) ?
                    (char)0 : (char)(textpointer - 2);

                // Write the number of characters typed in byte 1
                machine.WriteUnsigned8(textbuffer + 1, numCharsTyped);
            }
            else
            {
                // Terminate with 0 byte in versions < 5
                // Check if input was cancelled
                int terminatepos = textpointer; // (textpointer - textbuffer + 2);
                if (terminateChar == ZsciiEncoding.Null)
                {
                    terminatepos = 0;
                }
                machine.WriteUnsigned8(textbuffer + terminatepos, (char)0);
            }
        }

        /// <summary>
        /// Process input.
        /// </summary>
        /// <param name="textbuffer">text buffer address</param>
        /// <param name="inputString">input string</param>
        private void processInput(int textbuffer, String inputString)
        {
            int storeOffset = machine.getVersion() <= 4 ? 1 : 2;
            for (int i = 0; i < inputString.Length; i++)
            {
                machine.WriteUnsigned8(textbuffer + i + storeOffset,
                        (char)(inputString[i] & 0xff));
            }
            char terminateChar = inputString[inputString.Length - 1];
            checkTermination(terminateChar, textbuffer, inputString.Length + 1);
        }

        /*
        private boolean isTerminatingCharacter(final char zsciiChar) {
          return isFileHeaderTerminator(zsciiChar)
                 || zsciiChar == ZsciiEncoding.NEWLINE
                 || zsciiChar == ZsciiEncoding.NULL;
        }

        private boolean isFileHeaderTerminator(final char zsciiChar) {
          if (machine.getVersion() >= 5) {
            final int terminatorTable =
                machine.readUnsigned16(StoryFileHeader.TERMINATORS);
            if (terminatorTable == 0) { return false; }

            // Check the terminator table
            char terminator;

            for (int i = 0; ; i++) {
              terminator = machine.readUnsigned8(terminatorTable + i);
              if (terminator == 0) {
                break;
              }
              if (terminator == 255) {
                return ZsciiEncoding.isFunctionKey(zsciiChar);
              }
              if (terminator == zsciiChar) {
                return true;
              }
            }
          }
          return false;
        }
        */

        /// <summary>
        /// Depending on the terminating character, return the terminator to
        /// the caller.We need this since aread stores the terminating character
        /// as a result. If a newline was typed as the terminator, a newline
        /// will be echoed, in all other cases, the terminator is simply returned.
        /// </summary>
        /// <param name="terminateChar">the terminating character</param>
        /// <returns>a terminating character that can be stored as a result</returns>
        public char handleTerminateChar(char terminateChar)
        {
            if (terminateChar == ZsciiEncoding.Newline)
            {
                // Echo a newline into the streams
                // must be called with isInput == false since we are not
                // in input mode anymore when we receive NEWLINE
                machine.printZsciiChar(ZsciiEncoding.Newline);
            }
            return terminateChar;
        }

        // **********************************************************************
        // ****** READ_CHAR
        // *******************************

        public char readChar()
        {
            String inputLine = machine.getSelectedInputStream().readLine();
            return inputLine[0];
        }

        public void tokenize(int textbuffer, int parsebuffer, int dictionaryAddress, bool flag)
        {
            int version = machine.getVersion();
            int bufferlen = machine.ReadUnsigned8(textbuffer);
            int textbufferstart = determineTextBufferStart(version);
            int charsTyped =
              version >= 5 ? machine.ReadUnsigned8(textbuffer + 1) : 0;

            // from version 5, text starts at position 2
            String input = bufferToZscii(textbuffer + textbufferstart, bufferlen, charsTyped);
            List<String> tokens = tokenize(input);
            Dictionary<String, int> parsedTokens = new Dictionary<String, int>();
            int maxTokens = machine.ReadUnsigned8(parsebuffer);
            int numTokens = Math.Min(maxTokens, tokens.Count);

            // Write the number of parsed tokens into byte 1 of the parse buffer
            machine.WriteUnsigned8(parsebuffer + 1, (char)numTokens);

            int parseaddr = parsebuffer + 2;

            for (int i = 0; i < numTokens; i++)
            {
                String token = tokens[i];
                int entryAddress = machine.lookupToken(dictionaryAddress, token);
                int startIndex = 0;
                if (parsedTokens.ContainsKey(token))
                {
                    int timesContained = parsedTokens[token];
                    parsedTokens[token] = timesContained + 1;
                    for (int j = 0; j < timesContained; j++)
                    {
                        int found = input.IndexOf(token, startIndex);
                        startIndex = found + token.Length;
                    }
                }
                else
                {
                    parsedTokens[token] = 1;
                }
                int tokenIndex = input.IndexOf(token, startIndex);
                tokenIndex++; // adjust by the buffer length byte

                if (version >= 5)
                {
                    // if version >= 5, there is also numbers typed byte
                    tokenIndex++;
                }

                // if the tokenize flag is not set, write out the entry to the
                // parse buffer, if it is set then, only write the token position
                // if the token was recognized
                if (!flag || flag && entryAddress > 0)
                {
                    // This is one slot
                    machine.WriteUnsigned16(parseaddr, ToUnsigned16(entryAddress));
                    machine.WriteUnsigned8(parseaddr + 2, (char)token.Length);
                    machine.WriteUnsigned8(parseaddr + 3, (char)tokenIndex);
                }
                parseaddr += 4;
            }
        }

        /// <summary>
        /// Turns the buffer into a ZSCII string. This function reads at most
        /// |bufferlen| bytes and treats each byte as an ASCII character.
        /// The characters will be concatenated to the result string.
        /// </summary>
        /// <param name="address">the buffer address</param>
        /// <param name="bufferlen">the buffer length</param>
        /// <param name="charsTyped">from version 5, this is the number of characters to include in the input</param>
        /// <returns>the string contained in the buffer</returns>
        private String bufferToZscii(int address, int bufferlen, int charsTyped)
        {
            // If charsTyped is set, use that value as the limit
            int numChars = (charsTyped > 0) ? charsTyped : bufferlen;

            // read input from text buffer
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < numChars; i++)
            {
                char charByte = (char)machine.ReadUnsigned8(address + i);
                if (charByte == 0)
                {
                    break;
                }
                buffer.Append(charByte);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Turns the specified input string into tokens. It will take whitespace
        /// implicitly and dictionary separators explicitly to tokenize the
        /// stream, dictionary specified separators are included in the result list.
        /// </summary>
        /// <param name="input">the input string</param>
        /// <returns>the tokens</returns>
        private List<String> tokenize(String input)
        {
            List<String> result = new List<String>();
            // The tokenizer will also return the delimiters
            char[] delim = machine.getDictionaryDelimiters();
            // include dictionary delimiters as tokens
            //StringTokenizer tok = new StringTokenizer(input, delim, true);
            var tok = input.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            //while (tok.hasMoreTokens())
            for (var i = 0; i < tok.Length; i++)
            {
                //String token = tok.nextToken();
                String token = tok[i];
                if (!char.IsWhiteSpace(token[0]))
                {
                    result.Add(token);
                    //result.Add(delim);
                }
            }
            return result;
        }

        /// <summary>
        /// Depending on the version, this returns the offset where text starts in
        /// the text buffer.In versions up to 4 this is 1, since we have the
        /// buffer size in the first byte, from versions 5, we also have the
        /// number of typed characters in the second byte.
        /// </summary>
        /// <param name="version">the story file version</param>
        /// <returns>1 if version &lt; 4, 2, otherwise</returns>
        private int determineTextBufferStart(int version)
        {
            return (version < 5) ? 1 : 2;
        }
    }
}
