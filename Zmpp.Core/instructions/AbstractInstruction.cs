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
    using Zmpp.Core;
    using Zmpp.Core.Vm;
    using Zmpp.Core.UI;
    using System;
    using System.Text;

    /// <summary>
    /// An abstract instruction to replace the old instruction scheme.
    /// Goes with the NewInstructionDecoder.
    /// </summary>
    public abstract class AbstractInstruction : IInstruction
    {
        /// <summary>
        /// Branch information.
        /// </summary>
        public class BranchInfo
        {
            public bool branchOnTrue;
            public int numOffsetBytes;
            public int addressAfterBranchData;
            public short branchOffset;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="branchOnTrue">branch on true flag</param>
            /// <param name="numOffsetBytes">number of offset bytes</param>
            /// <param name="addressAfterBranchData">address after branch data</param>
            /// <param name="branchOffset">branch offset</param>
            public BranchInfo(bool branchOnTrue, int numOffsetBytes, int addressAfterBranchData, short branchOffset)
            {
                this.branchOnTrue = branchOnTrue;
                this.numOffsetBytes = numOffsetBytes;
                this.addressAfterBranchData = addressAfterBranchData;
                this.branchOffset = branchOffset;
            }
        }

        private IMachine machine;
        private int opcodeNum;
        private Operand[] operands;
        private char storeVariable;
        private BranchInfo branchInfo;
        private int opcodeLength;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">Machine object</param>
        /// <param name="opcodeNum">opcode number</param>
        /// <param name="operands">operands</param>
        /// <param name="storeVar">store variable</param>
        /// <param name="branchInfo">branch information</param>
        /// <param name="opcodeLength">opcode length</param>
        public AbstractInstruction(IMachine machine, int opcodeNum, Operand[] operands, char storeVar, BranchInfo branchInfo, int opcodeLength)
        {
            this.machine = machine;
            this.opcodeNum = opcodeNum;
            this.operands = operands;
            this.storeVariable = storeVar;
            this.branchInfo = branchInfo;
            this.opcodeLength = opcodeLength;
        }

        /// <summary>
        /// Returns the machine object.
        /// </summary>
        /// <returns>the Machine object</returns>
        protected IMachine getMachine() { return machine; }

        /// <summary>
        /// Returns the story version.
        /// </summary>
        /// <returns>story version</returns>
        protected int getStoryVersion() { return machine.getVersion(); }

        /// <summary>
        /// Returns the operand count.
        /// </summary>
        /// <returns>operand count</returns>
        protected abstract Instruction.OperandCount getOperandCount();

        /// <summary>
        /// The opcode length is a crucial attribute for program control, expose it
        /// for testing.
        /// </summary>
        /// <returns>the length of the instruction in memory</returns>
        public int getLength() { return opcodeLength; }

        /// <summary>
        /// Returns the instruction's opcode.
        /// </summary>
        /// <returns>the opcode</returns>
        protected int getOpcodeNum() { return opcodeNum; }

        /// <summary>
        /// Determines whether this instruction stores a result.
        /// </summary>
        /// <returns>true if stores result, false otherwise</returns>
        protected bool storesResult()
        {
            return InstructionInfoDb.getInstance().getInfo(getOperandCount(), opcodeNum, machine.getVersion()).isStore();
        }

        #region Variable access

        /// <summary>
        /// Returns the number of operands.
        /// </summary>
        /// <returns>the number of operands</returns>
        protected int getNumOperands() { return operands.Length; }

        /// <summary>
        /// Converts the specified value into a signed value, depending on the
        /// type of the operand.If the operand is LARGE_CONSTANT or VARIABLE,
        /// the value is treated as a 16 bit signed integer, if it is SMALL_CONSTANT,
        /// it is treated as an 8 bit signed integer.
        /// </summary>
        /// <param name="operandNum">the operand number</param>
        /// <returns>a signed value</returns>
        protected short getSignedValue(int operandNum)
        {
            /*
            // I am not sure if this is ever applicable....
            if (operands[operandNum].getType() == OperandType.SMALL_CONSTANT) {
              return MemoryUtil.unsignedToSigned8(getUnsignedValue(operandNum));
            }*/
            return MemoryUtil.UnsignedToSigned16(getUnsignedValue(operandNum));
        }

        /// <summary>
        /// A method to return the signed representation of the contents of a variable
        /// </summary>
        /// <param name="varnum">the variable number</param>
        /// <returns>the signed value</returns>
        protected short getSignedVarValue(char varnum)
        {
            return MemoryUtil.UnsignedToSigned16(getMachine().getVariable(varnum));
        }

        /// <summary>
        /// A method to set a signed 16 Bit integer to the specified variable.
        /// </summary>
        /// <param name="varnum">the variable number</param>
        /// <param name="value">the signed value</param>
        protected void setSignedVarValue(char varnum, short value)
        {
            getMachine().setVariable(varnum, MemoryUtil.SignedToUnsigned16(value));
        }

        /// <summary>
        /// Retrieves the value of the specified operand as an unsigned 16 bit
        /// integer.
        /// </summary>
        /// <param name="operandNum">the operand number</param>
        /// <returns>the value</returns>
        protected char getUnsignedValue(int operandNum)
        {
            Operand operand = operands[operandNum];
            switch (operand.getType())
            {
                case Operand.OperandType.VARIABLE:
                    return getMachine().getVariable(operand.getValue());
                case Operand.OperandType.SMALL_CONSTANT:
                case Operand.OperandType.LARGE_CONSTANT:
                default:
                    return operand.getValue();
            }
        }

        /// <summary>
        /// Stores the specified value in the result variable.
        /// </summary>
        /// <param name="value">the value to store</param>
        protected void storeUnsignedResult(char value)
        {
            getMachine().setVariable(storeVariable, value);
        }

        /// <summary>
        /// Stores a signed value in the result variable.
        /// </summary>
        /// <param name="value">the value to store</param>
        protected void storeSignedResult(short value)
        {
            storeUnsignedResult(MemoryUtil.SignedToUnsigned16(value));
        }

        #endregion

        #region Program flow control

        /// <summary>
        /// Advances the program counter to the next instruction.
        /// </summary>
        protected void nextInstruction() { machine.incrementPC(opcodeLength); }

        /// <summary>
        /// Performs a branch, depending on the state of the condition flag.
        /// If branchIfConditionTrue is true, the branch will be performed if
        /// condition is true, if branchIfCondition is false, the branch will
        /// be performed if condition is false.
        /// </summary>
        /// <param name="condition">the test condition</param>
        protected void branchOnTest(bool condition)
        {
            bool test = branchInfo.branchOnTrue ? condition : !condition;
            //Console.Out.WriteLine("ApplyBranch, offset: %d, opcodeLength: %d,
            //                  branchIfTrue: %b, test: %b\n",
            //                  branchInfo.branchOffset, opcodeLength,
            //                  branchInfo.branchOnTrue, test);
            if (test)
            {
                applyBranch();
            }
            else
            {
                nextInstruction();
            }
        }

        /// <summary>
        /// Applies a jump by applying the branch formula on the pc given the specified
        /// offset.
        /// </summary>
        private void applyBranch()
        {
            machine.doBranch(branchInfo.branchOffset, opcodeLength);
        }

        /// <summary>
        /// This function returns from the current routine, setting the return value
        /// into the specified return variable.
        /// </summary>
        /// <param name="returnValue">the return value</param>
        protected void returnFromRoutine(char returnValue)
        {
            machine.returnWith(returnValue);
        }

        /// <summary>
        /// Calls in the Z-machine are all very similar and only differ in the
        /// number of arguments.
        /// </summary>
        /// <param name="numArgs">the number of arguments</param>
        protected void call(int numArgs)
        {
            char packedAddress = getUnsignedValue(0);
            char[] args = new char[numArgs];
            for (int i = 0; i < numArgs; i++)
            {
                args[i] = getUnsignedValue(i + 1);
            }
            call(packedAddress, args);
        }

        /// <summary>
        /// Perform a call to a packed address.
        /// </summary>
        /// <param name="packedRoutineAddress">routine address</param>
        /// <param name="args">arguments</param>
        protected void call(char packedRoutineAddress, char[] args)
        {
            if (packedRoutineAddress == 0)
            {
                if (storesResult())
                {
                    // only if this instruction stores a result
                    storeUnsignedResult(Instruction.FALSE);
                }
                nextInstruction();
            }
            else
            {
                int returnAddress = getMachine().getPC() + opcodeLength;
                char returnVariable = storesResult() ? storeVariable : RoutineContext.DISCARD_RESULT;
                machine.call(packedRoutineAddress, returnAddress, args, returnVariable);
            }
        }

        /// <summary>
        /// Halt the virtual machine with an error message about this instruction.
        /// </summary>
        protected void throwInvalidOpcode()
        {
            machine.halt("illegal instruction, operand count: " + getOperandCount() + " opcode: " + opcodeNum);
        }

        /// <summary>
        /// Save game state to persistent storage.
        /// </summary>
        /// <param name="pc">current pc</param>
        protected void saveToStorage(int pc)
        {
            // This is a little tricky: In version 3, the program counter needs to
            // point to the branch offset, and not to an instruction position
            // In version 4, this points to the store variable. In both cases this
            // address is the instruction address + 1
            bool success = getMachine().save(pc);

            if (machine.getVersion() <= 3)
            {
                //int target = getMachine().getProgramCounter() + getLength();
                //target--; // point to the previous branch offset
                //boolean success = getMachine().save(target);
                branchOnTest(success);
            }
            else
            {
                // changed behaviour in version >= 4
                storeUnsignedResult(success ? Instruction.TRUE : Instruction.FALSE);
                nextInstruction();
            }
        }

        /// <summary>
        /// Restore game from persistent storage.
        /// </summary>
        protected void restoreFromStorage()
        {
            PortableGameState gamestate = getMachine().restore();
            if (machine.getVersion() <= 3)
            {
                if (gamestate == null)
                {
                    // If failure on restore, just continue
                    nextInstruction();
                }
            }
            else
            {
                // changed behaviour in version >= 4
                if (gamestate == null)
                {
                    storeUnsignedResult(Instruction.FALSE);
                    // If failure on restore, just continue
                    nextInstruction();
                }
                else
                {
                    char storevar = gamestate.getStoreVariable(getMachine());
                    getMachine().setVariable(storevar, Instruction.RESTORE_TRUE);
                }
            }
        }

        /// <summary>
        /// Returns the window for a given window number.
        /// </summary>
        /// <param name="windownum">the window number</param>
        /// <returns>the window</returns>
        protected IWindow6 getWindow(int windownum)
        {
            return (windownum == ScreenModel.CURRENT_WINDOW) ?
                    getMachine().getScreen6().getSelectedWindow() :
                    getMachine().getScreen6().getWindow(windownum);
        }

        /// <summary>
        /// Helper function
        /// </summary>
        /// <returns>true if output, false otherwise</returns>
        public bool isOutput()
        {
            return InstructionInfoDb.getInstance().getInfo(getOperandCount(), opcodeNum, getStoryVersion()).isOutput();
        }

        public String toString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(InstructionInfoDb.getInstance().getInfo(getOperandCount(),
                          opcodeNum, getStoryVersion()).getName());
            buffer.Append(" ");
            buffer.Append(getOperandString());
            if (storesResult())
            {
                buffer.Append(" -> ");
                buffer.Append(getVarName(storeVariable));
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Returns the string representation of the specified variable.
        /// </summary>
        /// <param name="varnum">variable number</param>
        /// <returns>variable name</returns>
        private String getVarName(int varnum)
        {
            if (varnum == 0)
            {
                return "(SP)";
            }
            else if (varnum <= 15)
            {
                return string.Format("L{0:x2}", (varnum - 1));
            }
            else
            {
                return string.Format("G{0:x2}", (varnum - 16));
            }
        }

        /// <summary>
        /// Returns the value of the specified variable.
        /// </summary>
        /// <param name="varnum">the variable number</param>
        /// <returns>value of the specified variable</returns>
        private String getVarValue(char varnum)
        {
            char value = (char)0;
            if (varnum == 0)
            {
                value = machine.getStackTop();
            }
            else
            {
                value = machine.getVariable(varnum);
            }
            return string.Format("${0:x4}", (int)value);
        }

        /// <summary>
        /// Returns the string representation of the operands.
        /// </summary>
        /// <returns>string representation of operands</returns>
        protected virtual String getOperandString()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < getNumOperands(); i++)
            {
                if (i > 0)
                {
                    buffer.Append(", ");
                }
                Operand operand = operands[i];
                switch (operand.getType())
                {
                    case Operand.OperandType.SMALL_CONSTANT:
                        buffer.Append(string.Format("${0:x2}", (int)operand.getValue()));
                        break;
                    case Operand.OperandType.LARGE_CONSTANT:
                        buffer.Append(string.Format("${0:x4}", (int)operand.getValue()));
                        break;
                    case Operand.OperandType.VARIABLE:
                        buffer.Append(getVarName(operand.getValue()));
                        buffer.Append("[");
                        buffer.Append(getVarValue(operand.getValue()));
                        buffer.Append("]");
                        goto default;
                    default:
                        break;
                }
            }
            return buffer.ToString();
        }

        #endregion

        public abstract void execute();
    }
}
