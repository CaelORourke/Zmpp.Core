/*
 * Created on 09/24/2005
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

namespace Zmpp.Core.Instructions
{
    /// <summary>
    /// This is the definition of an instruction's operand. Each operand has
    /// an operand type, and a value which is to be interpreted according to
    /// the type.
    /// </summary>
    public class Operand
    {
        /// <summary>
        /// Type number for a large constant.
        /// </summary>
        public const int TYPENUM_LARGE_CONSTANT = 0x00;

        /// <summary>
        /// Type number for a small constant.
        /// </summary>
        public const int TYPENUM_SMALL_CONSTANT = 0x01;

        /// <summary>
        /// Type number for a variable.
        /// </summary>
        public const int TYPENUM_VARIABLE = 0x02;

        /// <summary>
        /// Type number for omitted.
        /// </summary>
        public const int TYPENUM_OMITTED = 0x03;

        /// <summary>
        /// The available operand types.
        /// </summary>
        public enum OperandType { SMALL_CONSTANT, LARGE_CONSTANT, VARIABLE, OMITTED }

        /// <summary>
        /// This operand's type.
        /// </summary>
        private OperandType type;

        /// <summary>
        /// This operand's value.
        /// </summary>
        private char value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="typenum">the type number, must be < 4</param>
        /// <param name="value">the operand value</param>
        public Operand(int typenum, char value)
        {
            type = getOperandType(typenum);
            this.value = value;
        }

        /// <summary>
        /// Determines the operand type from a two-bit value.
        /// </summary>
        /// <param name="typenum">the type number</param>
        /// <returns>the operand type</returns>
        private static OperandType getOperandType(int typenum)
        {
            switch (typenum)
            {
                case 0x00:
                    return OperandType.LARGE_CONSTANT;
                case 0x01:
                    return OperandType.SMALL_CONSTANT;
                case 0x02:
                    return OperandType.VARIABLE;
                default:
                    return OperandType.OMITTED; // In fact, such a value should never exist..
            }
        }

        /// <summary>
        /// Returns this operand's type.
        /// </summary>
        /// <returns>the operand type</returns>
        public OperandType getType() { return type; }

        /// <summary>
        /// The operand value.
        /// </summary>
        /// <returns>the value</returns>
        public char getValue() { return value; }
    }
}
