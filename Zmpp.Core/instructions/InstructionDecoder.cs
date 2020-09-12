/*
 * Created on 2008/07/23
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
    using Zmpp.Core.Vm;
    using System;
    using static Zmpp.Core.Instructions.AbstractInstruction;
    using static Zmpp.Core.Instructions.InstructionInfoDb;
    using static Zmpp.Core.Vm.Instruction;

    /// <summary>
    /// The revised instruction decoder, a direct port from the Erlang implementation
    /// of ZMPP(Schmalz). This decoding scheme is considerably simpler and stores
    /// more useful information than the previous one.
    /// </summary>
    public class InstructionDecoder
    {
        private const char EXTENDED_MASK = (char)0xbe;
        private const char VAR_MASK = (char)0xc0; // 2#11000000
        private const char SHORT_MASK = (char)0x80; // 2#10000000
        private const char LOWER_4_BITS = (char)0x0f; // 2#00001111
        private const char LOWER_5_BITS = (char)0x1f; // 2#00011111
        private const char LOWER_6_BITS = (char)0x3f; // 2#00111111
        private const char BITS_4_5 = (char)0x30; // 2#00110000
        private const char BIT_7 = (char)0x80; // 2#10000000
        private const char BIT_6 = (char)0x40; // 2#01000000
        private const char BIT_5 = (char)0x20; // 2#00100000
        private const int LEN_OPCODE = 1;
        private const int LEN_LONG_OPERANDS = 2;
        private const int LEN_STORE_VARIABLE = 1;
        private static InstructionInfoDb INFO_DB = InstructionInfoDb.getInstance();
        private static BranchInfo DUMMY_BRANCH_INFO = new BranchInfo(false, 0, 0, (short)0);
        private static int[] NO_OPERAND_TYPES = new int[0];
        private static char[] NO_OPERANDS = new char[0];

        private IMachine machine;

        /// <summary>
        /// Initialize decoder with a valid machine object.
        /// </summary>
        /// <param name="aMachine">a Machine object</param>
        public void initialize(IMachine aMachine)
        {
            this.machine = aMachine;
        }

        /// <summary>
        /// Decode the instruction at the specified address.
        /// </summary>
        /// <param name="instructionAddress">the current instruction's address</param>
        /// <returns>the instruction at the specified address</returns>
        public IInstruction decodeInstruction(int instructionAddress)
        {
            IInstruction instr = null;
            char byte1 = machine.ReadUnsigned8(instructionAddress);
            InstructionForm form = getForm(byte1);
            switch (form)
            {
                case InstructionForm.SHORT:
                    instr = decodeShort(instructionAddress, byte1);
                    break;
                case InstructionForm.LONG:
                    instr = decodeLong(instructionAddress, byte1);
                    break;
                case InstructionForm.VARIABLE:
                    instr = decodeVariable(instructionAddress, byte1);
                    break;
                case InstructionForm.EXTENDED:
                    instr = decodeExtended(instructionAddress);
                    break;
                default:
                    Console.Out.WriteLine("unrecognized form: " + form);
                    break;
            }
            return instr;
        }

        /// <summary>
        /// Decodes an instruction in short form.
        /// </summary>
        /// <param name="instrAddress">the instruction address</param>
        /// <param name="byte1">the first instruction byte</param>
        /// <returns>the decoded instruction</returns>
        private IInstruction decodeShort(int instrAddress, char byte1)
        {
            OperandCount opCount = (byte1 & BITS_4_5) == BITS_4_5 ? OperandCount.C0OP : OperandCount.C1OP;
            char opcodeNum = (char)(byte1 & LOWER_4_BITS);
            InstructionInfo info = INFO_DB.getInfo(opCount, opcodeNum,
                                                  machine.getVersion());
            if (info == null)
            {
                Console.Out.WriteLine("ILLEGAL SHORT operation, instrAddr: $%04x, OC: %s, " +
                                  "opcode: #$%02x, Version: %d\n",
                                  instrAddress, opCount.ToString(), (int)opcodeNum,
                                  machine.getVersion());
                //infoDb.printKeys();
                throw new InvalidOperationException("Exit !!");
            }
            int zsciiLength = 0;

            // extract operand
            String str = null;
            char operand = (char)0;
            int[] operandTypes = NO_OPERAND_TYPES;
            char[] operands = NO_OPERANDS;
            int operandType = getOperandType(byte1, 1);
            if (info.isPrint())
            {
                str = machine.decode2Zscii(instrAddress + 1, 0);
                zsciiLength = machine.getNumZEncodedBytes(instrAddress + 1);
            }
            else
            {
                operand = getOperandAt(instrAddress + 1, operandType);
                operandTypes = new int[] { operandType };
                operands = new char[] { operand };
            }
            int numOperandBytes = getOperandLength(operandType);
            int currentAddr = instrAddress + LEN_OPCODE + numOperandBytes;
            return createInstruction(opCount, instrAddress,
                                     opcodeNum, currentAddr, numOperandBytes,
                                     zsciiLength, operandTypes, operands, str);
        }

        /// <summary>
        /// Decodes a long op count instruction.
        /// </summary>
        /// <param name="instrAddress">instruction address</param>
        /// <param name="byte1">first instruction byte</param>
        /// <returns>instruction object</returns>
        private IInstruction decodeLong(int instrAddress, char byte1)
        {
            char opcodeNum = (char)(byte1 & LOWER_5_BITS);

            // extract long operands
            int operandType1 = (byte1 & BIT_6) != 0 ? Operand.TYPENUM_VARIABLE :
              Operand.TYPENUM_SMALL_CONSTANT;
            int operandType2 = (byte1 & BIT_5) != 0 ? Operand.TYPENUM_VARIABLE :
              Operand.TYPENUM_SMALL_CONSTANT;
            char operand1 = machine.ReadUnsigned8(instrAddress + 1);
            char operand2 = machine.ReadUnsigned8(instrAddress + 2);
            int numOperandBytes = LEN_LONG_OPERANDS;
            int currentAddr = instrAddress + LEN_OPCODE + LEN_LONG_OPERANDS;
            //System.out.printf("LONG 2OP, opnum: %d, byte1: %d, addr: $%04x\n",
            //        (int) opcodeNum, (int) byte1, instrAddress);
            return createInstruction(OperandCount.C2OP, instrAddress, opcodeNum, currentAddr,
                                     numOperandBytes, 0,
              new int[] { operandType1, operandType2 }, new char[] { operand1, operand2 },
              null);
        }

        /// <summary>
        /// Decodes an instruction in variable form.
        /// </summary>
        /// <param name="instrAddress">the instruction address</param>
        /// <param name="byte1">the first opcode byte</param>
        /// <returns>the instruction</returns>
        private IInstruction decodeVariable(int instrAddress, char byte1)
        {
            OperandCount opCount = (byte1 & BIT_5) != 0 ? OperandCount.VAR : OperandCount.C2OP;
            char opcodeNum = (char)(byte1 & LOWER_5_BITS);
            int opTypesOffset;
            int[] operandTypes;
            // The only instruction taking up to 8 parameters is CALL_VS2
            if (isVx2(opCount, opcodeNum))
            {
                operandTypes = joinArrays(
                    extractOperandTypes(machine.ReadUnsigned8(instrAddress + 1)),
                    extractOperandTypes(machine.ReadUnsigned8(instrAddress + 2)));
                opTypesOffset = 3;
            }
            else
            {
                operandTypes =
                    extractOperandTypes(machine.ReadUnsigned8(instrAddress + 1));
                opTypesOffset = 2;
            }
            return decodeVarInstruction(instrAddress, opCount, opcodeNum, operandTypes,
                                        opTypesOffset - 1, opTypesOffset, false);
        }

        /// <summary>
        /// Determines whether the instruction is a CALL_VS2 or CALL_VN2.
        /// </summary>
        /// <param name="opCount">operand count</param>
        /// <param name="opcodeNum">opcode number</param>
        /// <returns>true if it CALL_VS2/CALL_VN2, false otherwise</returns>
        private bool isVx2(OperandCount opCount, char opcodeNum)
        {
            return opCount == OperandCount.VAR &&
                (opcodeNum == VAR_CALL_VN2 || opcodeNum == VAR_CALL_VS2);

        }

        /// <summary>
        /// Join two int arrays which are not null.
        /// </summary>
        /// <param name="arr1">the first int array</param>
        /// <param name="arr2">the second int array</param>
        /// <returns>the concatenation of the two input arrays</returns>
        private int[] joinArrays(int[] arr1, int[] arr2)
        {
            int[] result = new int[arr1.Length + arr2.Length];
            Array.Copy(arr1, 0, result, 0, arr1.Length);
            Array.Copy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        /// <summary>
        /// Decodes an instruction in extended form. Is really just a variation of
        /// variable form and delegates to decodeVarInstruction.
        /// </summary>
        /// <param name="instrAddress">instruction address</param>
        /// <returns>the decoded instruction</returns>
        private IInstruction decodeExtended(int instrAddress)
        {
            return decodeVarInstruction(instrAddress, OperandCount.EXT,
                machine.ReadUnsigned8(instrAddress + 1),
                extractOperandTypes(machine.ReadUnsigned8(instrAddress + 2)), 1, 3, true);
        }

        /// <summary>
        /// Decode VAR form instruction.
        /// </summary>
        /// <param name="instrAddress">instruction address</param>
        /// <param name="opCount">operand count</param>
        /// <param name="opcodeNum">opcode number</param>
        /// <param name="operandTypes">operand types</param>
        /// <param name="numOperandTypeBytes">number of operand type bytes</param>
        /// <param name="opTypesOffset">operand types offset</param>
        /// <param name="isExtended">indicator of extended instruction</param>
        /// <returns>instruction object</returns>
        private IInstruction decodeVarInstruction(int instrAddress,
                                                 OperandCount opCount,
                                                 char opcodeNum,
                                                 int[] operandTypes,
                                                 int numOperandTypeBytes,
                                                 int opTypesOffset,
                                                 bool isExtended)
        {
            char[] operands = extractOperands(instrAddress + opTypesOffset,
                                              operandTypes);
            int numOperandBytes = getNumOperandBytes(operandTypes);
            // it is important to note that extended instructions have an extra byte
            // since the first byte is always $be
            int numExtraOpcodeBytes = isExtended ? 1 : 0;
            int currentAddr = instrAddress + opTypesOffset + numOperandBytes;
            return createInstruction(opCount, instrAddress, opcodeNum, currentAddr,
                                     numExtraOpcodeBytes + numOperandBytes +
                                       numOperandTypeBytes,
                                     0, operandTypes, operands, null);
        }

        /// <summary>
        /// The generic part of instruction decoding, extracting store variable
        /// and branch offset is always the same for all instruction forms.
        /// </summary>
        /// <param name="opCount">the OperandCount</param>
        /// <param name="instrAddress">the instruction address</param>
        /// <param name="opcodeNum">the opcode number</param>
        /// <param name="addrAfterOperands">the address after the operands</param>
        /// <param name="numOperandBytes">the number of operand bytes</param>
        /// <param name="zsciiLength">the length of the ZSCII in bytes if a print instruction</param>
        /// <param name="operandTypes">the operand types</param>
        /// <param name="operands">the operand values</param>
        /// <param name="str">the ZSCII string or null</param>
        /// <returns>the instruction</returns>
        private IInstruction createInstruction(OperandCount opCount,
                                              int instrAddress,
                                              char opcodeNum,
                                              int addrAfterOperands,
                                              int numOperandBytes,
                                              int zsciiLength,
                                              int[] operandTypes, char[] operands,
                                              String str)
        {
            int currentAddr = addrAfterOperands;
            int storeVarLen = 0;
            char storeVar = (char)0;
            Operand[] instrOperands = createOperands(operandTypes, operands);
            InstructionInfo info = INFO_DB.getInfo(opCount, opcodeNum,
                                                  machine.getVersion());
            if (info == null)
            {
                Console.Out.WriteLine("ILLEGAL operation, instrAddr: $%04x OC: %s, " +
                                  "opcode: #$%02x, Version: %d\n",
                                  instrAddress, opCount.ToString(), (int)opcodeNum,
                                  machine.getVersion());
                throw new InvalidOperationException("Exit !!");
            }
            if (info.isStore())
            {
                storeVar = machine.ReadUnsigned8(currentAddr);
                currentAddr++;
                storeVarLen = LEN_STORE_VARIABLE;
            }
            BranchInfo branchInfo = DUMMY_BRANCH_INFO;
            if (info.isBranch())
            {
                branchInfo = getBranchInfo(currentAddr);
            }
            int opcodeLength = LEN_OPCODE + numOperandBytes + storeVarLen +
                    branchInfo.numOffsetBytes + zsciiLength;
            //Console.Out.WriteLine("OPCODELEN: %d, len opcode: %d, # operand bytes: %d, " +
            //                  "len storevar: %d, broffsetbytes: %d, zsciilen: %d\n",
            //                  opcodeLength, LEN_OPCODE, numOperandBytes, storeVarLen,
            //                  branchInfo.numOffsetBytes, zsciiLength);
            switch (opCount)
            {
                case OperandCount.C0OP:
                    return new C0OpInstruction(machine, opcodeNum, instrOperands, str,
                        storeVar, branchInfo, opcodeLength);
                case OperandCount.C1OP:
                    return new C1OpInstruction(machine, opcodeNum, instrOperands,
                        storeVar, branchInfo, opcodeLength);
                case OperandCount.C2OP:
                    return new C2OpInstruction(machine, opcodeNum, instrOperands,
                        storeVar, branchInfo, opcodeLength);
                case OperandCount.VAR:
                    return new VarInstruction(machine, opcodeNum, instrOperands,
                        storeVar, branchInfo, opcodeLength);
                case OperandCount.EXT:
                    return new ExtInstruction(machine, opcodeNum, instrOperands,
                        storeVar, branchInfo, opcodeLength);
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Create operands objects.
        /// </summary>
        /// <param name="operandTypes">operand types.</param>
        /// <param name="operands">operand values</param>
        /// <returns>array of operand objects</returns>
        private Operand[] createOperands(int[] operandTypes, char[] operands)
        {
            Operand[] result = new Operand[operandTypes.Length];
            for (int i = 0; i < operandTypes.Length; i++)
            {
                result[i] = new Operand(operandTypes[i], operands[i]);
            }
            return result;
        }

        #region Helper functions

        private const int NUM_OPERAND_TYPES_PER_BYTE = 4;

        /// <summary>
        /// Extracts operand types.
        /// </summary>
        /// <param name="opTypeByte">operand type byte</param>
        /// <returns>operand types</returns>
        private int[] extractOperandTypes(char opTypeByte)
        {
            int[] opTypes = new int[NUM_OPERAND_TYPES_PER_BYTE];
            int numTypes;
            for (numTypes = 0; numTypes < NUM_OPERAND_TYPES_PER_BYTE; numTypes++)
            {
                int opType = getOperandType(opTypeByte, numTypes);
                if (opType == Operand.TYPENUM_OMITTED) break;
                opTypes[numTypes] = opType;
            }
            int[] result = new int[numTypes];
            for (int i = 0; i < numTypes; i++)
            {
                result[i] = opTypes[i];
            }
            return result;
        }

        /// <summary>
        /// Extract operands.
        /// </summary>
        /// <param name="operandAddr">operand address</param>
        /// <param name="operandTypes">operand types</param>
        /// <returns>operands</returns>
        private char[] extractOperands(int operandAddr, int[] operandTypes)
        {
            char[] result = new char[operandTypes.Length];
            int currentAddr = operandAddr;
            for (int i = 0; i < operandTypes.Length; i++)
            {
                if (operandTypes[i] == Operand.TYPENUM_LARGE_CONSTANT)
                {
                    result[i] = machine.ReadUnsigned16(currentAddr);
                    currentAddr += 2;
                }
                else
                {
                    result[i] = machine.ReadUnsigned8(currentAddr);
                    currentAddr++;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns total number of operand bytes.
        /// </summary>
        /// <param name="operandTypes">operand types</param>
        /// <returns>total operand bytes</returns>
        private int getNumOperandBytes(int[] operandTypes)
        {
            int result = 0;
            for (int i = 0; i < operandTypes.Length; i++)
            {
                result += operandTypes[i] == Operand.TYPENUM_LARGE_CONSTANT ? 2 : 1;
            }
            return result;
        }

        /// <summary>
        /// Extracts the operand type at the specified position of the op type byte.
        /// </summary>
        /// <param name="opTypeByte">the op type byte</param>
        /// <param name="pos">the position</param>
        /// <returns>the operand type</returns>
        private int getOperandType(char opTypeByte, int pos)
        {
            return (((int)((uint)opTypeByte >> (6 - pos * 2))) & 0x03);
        }

        /// <summary>
        /// Extract the branch information at the specified address
        /// </summary>
        /// <param name="branchInfoAddr">the branch info address</param>
        /// <returns>the BranchInfo object</returns>
        private BranchInfo getBranchInfo(int branchInfoAddr)
        {
            char branchByte1 = machine.ReadUnsigned8(branchInfoAddr);
            bool branchOnTrue = (branchByte1 & BIT_7) != 0;
            int numOffsetBytes, branchOffset;
            if (isSimpleOffset(branchByte1))
            {
                numOffsetBytes = 1;
                branchOffset = branchByte1 & LOWER_6_BITS;
            }
            else
            {
                numOffsetBytes = 2;
                char branchByte2 = machine.ReadUnsigned8(branchInfoAddr + 1);
                //Console.Out.WriteLine("14 Bit offset, bracnh byte1: %02x byte2: %02x\n",
                //                  (int) branchByte1, (int) branchByte2);
                branchOffset =
                    toSigned14((char)(((branchByte1 << 8) | branchByte2) & 0x3fff));
            }
            return new BranchInfo(branchOnTrue, numOffsetBytes,
                                  branchInfoAddr + numOffsetBytes,
                                  (short)branchOffset);
        }

        /// <summary>
        /// Determines whether the branch is a simple or compound (2 byte) offset.
        /// </summary>
        /// <param name="branchByte1">the first branch byte</param>
        /// <returns>true if simple offset, false if compound</returns>
        private bool isSimpleOffset(char branchByte1)
        {
            return (branchByte1 & BIT_6) != 0;
        }

        private const short WORD_14_UNSIGNED_MAX = 16383;
        private const short WORD_14_SIGNED_MAX = 8191;

        /// <summary>
        /// Helper function to extract a 14 bit signed branch offset.
        /// </summary>
        /// <param name="value">the value to convert</param>
        /// <returns>the signed offset</returns>
        private short toSigned14(char value)
        {
            return (short)(value > WORD_14_SIGNED_MAX ?
              -(WORD_14_UNSIGNED_MAX - (value - 1)) : value);
        }

        /// <summary>
        /// Returns the operand at the specified address.
        /// </summary>
        /// <param name="operandAddress">operand address</param>
        /// <param name="operandType">operand type</param>
        /// <returns>operand value</returns>
        private char getOperandAt(int operandAddress, int operandType)
        {
            return operandType == Operand.TYPENUM_LARGE_CONSTANT ?
              machine.ReadUnsigned16(operandAddress) :
              machine.ReadUnsigned8(operandAddress);
        }

        /// <summary>
        /// Determines the operand length of a specified type in bytes.
        /// </summary>
        /// <param name="operandType">the operand type</param>
        /// <returns>the number of bytes for the type</returns>
        private int getOperandLength(int operandType)
        {
            switch (operandType)
            {
                case Operand.TYPENUM_SMALL_CONSTANT: return 1;
                case Operand.TYPENUM_LARGE_CONSTANT: return 2;
                case Operand.TYPENUM_VARIABLE: return 1;
                default: return 0;
            }
        }

        /// <summary>
        /// Determine the instruction form from the first instruction byte.
        /// </summary>
        /// <param name="byte1">the first instruction byte</param>
        /// <returns>the InstructionForm</returns>
        private InstructionForm getForm(char byte1)
        {
            if (byte1 == EXTENDED_MASK) return InstructionForm.EXTENDED;
            if ((byte1 & VAR_MASK) == VAR_MASK) return InstructionForm.VARIABLE;
            if ((byte1 & SHORT_MASK) == SHORT_MASK) return InstructionForm.SHORT;
            return InstructionForm.LONG;
        }

        #endregion
    }
}
