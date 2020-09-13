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

namespace Zmpp.Core.Encoding
{
    using System;
    using static Zmpp.Core.Encoding.AlphabetTableBase;

    /// <summary>
    /// Represents a default implementation of IZCharTranslator.
    /// </summary>
    public class ZCharTranslator : IZCharTranslator, ICloneable
    {
        private readonly IAlphabetTable alphabetTable;
        private Alphabet currentAlphabet;
        private Alphabet lockAlphabet;
        private bool shiftLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Encoding.ZCharTranslator"/>
        /// class for the specified alphabet table.
        /// </summary>
        /// <param name="alphabetTable">The alphabet table.</param>
        public ZCharTranslator(IAlphabetTable alphabetTable)
        {
            this.alphabetTable = alphabetTable;
            Reset();
        }

        public void Reset()
        {
            currentAlphabet = Alphabet.A0;
            lockAlphabet = Alphabet.NotSet;
            shiftLock = false;
        }

        /// <summary>
        /// Reset the translation to use the last alphabet used.
        /// </summary>
        public void ResetToLastAlphabet()
        {
            if (lockAlphabet == Alphabet.NotSet)
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
            ZCharTranslator clone = null;
            clone = (ZCharTranslator)this.MemberwiseClone();
            clone.Reset();
            return clone;
        }

        public Alphabet CurrentAlphabet => currentAlphabet;

        public char Translate(char zchar)
        {
            if (Shift(zchar)) return '\0';

            char result;
            if (IsInAlphabetRange(zchar))
            {
                switch (currentAlphabet)
                {
                    case Alphabet.A0:
                        result = (char)alphabetTable.GetA0Char((byte)zchar);
                        break;
                    case Alphabet.A1:
                        result = (char)alphabetTable.GetA1Char((byte)zchar);
                        break;
                    case Alphabet.A2:
                    default:
                        result = (char)alphabetTable.GetA2Char((byte)zchar);
                        break;
                }
            }
            else
            {
                result = '?';
            }
            // Only reset if the shift lock flag is not set
            if (!shiftLock) ResetToLastAlphabet();
            return result;
        }

        public bool WillEscapeA2(char zchar)
        {
            return currentAlphabet == Alphabet.A2 && zchar == A2Escape;
        }

        public bool IsAbbreviation(char zchar)
        {
            return alphabetTable.IsAbbreviation(zchar);
        }

        public AlphabetElement GetAlphabetElementFor(char zsciiChar)
        {
            // Special handling for newline !!
            if (zsciiChar == '\n')
            {
                return new AlphabetElement(Alphabet.A2, (char)7);
            }

            Alphabet alphabet = Alphabet.NotSet;
            int zcharCode = alphabetTable.GetA0CharCode(zsciiChar);

            if (zcharCode >= 0)
            {
                alphabet = Alphabet.A0;
            }
            else
            {
                zcharCode = alphabetTable.GetA1CharCode(zsciiChar);
                if (zcharCode >= 0)
                {
                    alphabet = Alphabet.A1;
                }
                else
                {
                    zcharCode = alphabetTable.GetA2CharCode(zsciiChar);
                    if (zcharCode >= 0)
                    {
                        alphabet = Alphabet.A2;
                    }
                }
            }

            if (alphabet == Alphabet.NotSet)
            {
                // If it is not found in any alphabet table use the Z-character code.
                zcharCode = zsciiChar;
            }
            return new AlphabetElement(alphabet, (char)zcharCode);
        }

        /// <summary>
        /// Indicates whether the specified Z character falls within the alphabet range.
        /// </summary>
        /// <param name="zchar">The Z character.</param>
        /// <returns>true if the specified Z character is in the alphabet range; otherwise false.</returns>
        private static bool IsInAlphabetRange(char zchar)
        {
            return 0 <= zchar && zchar <= AlphabetEnd;
        }

        /// <summary>
        /// Performs a shift.
        /// </summary>
        /// <param name="zchar">The Z encoded character.</param>
        /// <returns>true if a shift was performed; otherwise false.</returns>
        private bool Shift(char zchar)
        {
            if (alphabetTable.IsShift(zchar))
            {
                currentAlphabet = ShiftFrom(currentAlphabet, zchar);

                // Sets the current lock alphabet
                if (alphabetTable.IsShiftLock(zchar))
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
        /// <param name="alphabet">The source alphabet.</param>
        /// <param name="shiftChar">The shift character.</param>
        /// <returns>The resulting alphabet.</returns>
        private Alphabet ShiftFrom(Alphabet alphabet, char shiftChar)
        {
            Alphabet result = Alphabet.NotSet;

            if (alphabetTable.IsShift1(shiftChar))
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
            else if (alphabetTable.IsShift2(shiftChar))
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
            shiftLock = alphabetTable.IsShiftLock(shiftChar);
            return result;
        }
    }
}
