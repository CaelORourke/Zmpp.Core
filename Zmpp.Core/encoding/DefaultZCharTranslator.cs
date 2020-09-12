/*
 * Created on 2006/01/15
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

namespace org.zmpp.encoding
{
    using System;
    using static org.zmpp.encoding.AlphabetTableBase;

    /// <summary>
    /// The default implementation of ZCharTranslator
    /// </summary>
    public class DefaultZCharTranslator : IZCharTranslator, ICloneable
    {
        private IAlphabetTable alphabetTable;
        private Alphabet currentAlphabet;
        private Alphabet lockAlphabet;
        private bool shiftLock;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="alphabetTable">the alphabet table</param>
        public DefaultZCharTranslator(IAlphabetTable alphabetTable)
        {
            this.alphabetTable = alphabetTable;
            reset();
        }

        public void reset()
        {
            currentAlphabet = Alphabet.A0;
            lockAlphabet = Alphabet.Unknown;
            shiftLock = false;
        }

        /// <summary>
        /// Reset the translation to use the last alphabet used.
        /// </summary>
        public void resetToLastAlphabet()
        {
            if (lockAlphabet == Alphabet.Unknown)
            {
                currentAlphabet = Alphabet.A0;
            }
            else
            {
                currentAlphabet = lockAlphabet;
                shiftLock = true;
            }
        }

        public Object Clone()
        {
            DefaultZCharTranslator clone = null;
            clone = (DefaultZCharTranslator)this.MemberwiseClone();
            clone.reset();
            return clone;
        }

        public Alphabet getCurrentAlphabet() { return currentAlphabet; }


        public char translate(char zchar)
        {
            if (shift(zchar)) return '\0';

            char result;
            if (isInAlphabetRange(zchar))
            {
                switch (currentAlphabet)
                {
                    case Alphabet.A0:
                        result = (char)alphabetTable.getA0Char((byte)zchar);
                        break;
                    case Alphabet.A1:
                        result = (char)alphabetTable.getA1Char((byte)zchar);
                        break;
                    case Alphabet.A2:
                    default:
                        result = (char)alphabetTable.getA2Char((byte)zchar);
                        break;
                }
            }
            else
            {
                result = '?';
            }
            // Only reset if the shift lock flag is not set
            if (!shiftLock) resetToLastAlphabet();
            return result;
        }

        public bool willEscapeA2(char zchar)
        {
            return currentAlphabet == Alphabet.A2 && zchar == A2_ESCAPE;
        }

        public bool isAbbreviation(char zchar)
        {
            return alphabetTable.isAbbreviation(zchar);
        }

        public AlphabetElement getAlphabetElementFor(char zsciiChar)
        {
            // Special handling for newline !!
            if (zsciiChar == '\n')
            {
                return new AlphabetElement(Alphabet.A2, (char)7);
            }

            Alphabet alphabet = Alphabet.Unknown;
            int zcharCode = alphabetTable.getA0CharCode(zsciiChar);

            if (zcharCode >= 0)
            {
                alphabet = Alphabet.A0;
            }
            else
            {
                zcharCode = alphabetTable.getA1CharCode(zsciiChar);
                if (zcharCode >= 0)
                {
                    alphabet = Alphabet.A1;
                }
                else
                {
                    zcharCode = alphabetTable.getA2CharCode(zsciiChar);
                    if (zcharCode >= 0)
                    {
                        alphabet = Alphabet.A2;
                    }
                }
            }

            if (alphabet == Alphabet.Unknown)
            {
                // It is not in any alphabet table, we are fine with taking the code
                // number for the moment
                zcharCode = zsciiChar;
            }
            return new AlphabetElement(alphabet, (char)zcharCode);
        }

        /// <summary>
        /// Determines if the given byte value falls within the alphabet range.
        /// </summary>
        /// <param name="zchar">the zchar value</param>
        /// <returns>true if the value is in the alphabet range, false, otherwise</returns>
        private static bool isInAlphabetRange(char zchar)
        {
            return 0 <= zchar && zchar <= ALPHABET_END;
        }

        /// <summary>
        /// Performs a shift.
        /// </summary>
        /// <param name="zchar">a z encoded character</param>
        /// <returns>true if a shift was performed, false, otherwise</returns>
        private bool shift(char zchar)
        {
            if (alphabetTable.isShift(zchar))
            {
                currentAlphabet = shiftFrom(currentAlphabet, zchar);

                // Sets the current lock alphabet
                if (alphabetTable.isShiftLock(zchar))
                {
                    lockAlphabet = currentAlphabet;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method contains the rules to shift the alphabets.
        /// </summary>
        /// <param name="alphabet">the source alphabet</param>
        /// <param name="shiftChar">the shift character</param>
        /// <returns>the resulting alphabet</returns>
        private Alphabet shiftFrom(Alphabet alphabet, char shiftChar)
        {
            Alphabet result = Alphabet.Unknown;

            if (alphabetTable.isShift1(shiftChar))
            {
                if (alphabet == Alphabet.A0)
                {
                    result = Alphabet.A1;
                }
                else if (alphabet == Alphabet.A1)
                {
                    result = Alphabet.A2;
                }
                else if (alphabet == Alphabet.A2)
                {
                    result = Alphabet.A0;
                }
            }
            else if (alphabetTable.isShift2(shiftChar))
            {
                if (alphabet == Alphabet.A0)
                {
                    result = Alphabet.A2;
                }
                else if (alphabet == Alphabet.A1)
                {
                    result = Alphabet.A0;
                }
                else if (alphabet == Alphabet.A2)
                {
                    result = Alphabet.A1;
                }
            }
            else
            {
                result = alphabet;
            }
            shiftLock = alphabetTable.isShiftLock(shiftChar);
            return result;
        }
    }
}
