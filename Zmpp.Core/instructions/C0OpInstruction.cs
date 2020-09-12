/*
 * Created on 2008/07/24
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
    using static Zmpp.Core.Vm.Instruction;

    /// <summary>
    /// Instruction of form 0Op.
    /// </summary>
    public class C0OpInstruction : AbstractInstruction
    {
        private String str;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">Machine object</param>
        /// <param name="opcodeNum">opcode number</param>
        /// <param name="operands">operands</param>
        /// <param name="str">string object for print instructions</param>
        /// <param name="storeVar">store variable</param>
        /// <param name="branchInfo">branch information</param>
        /// <param name="opcodeLength">opcode length</param>
        public C0OpInstruction(IMachine machine, int opcodeNum, Operand[] operands, String str, char storeVar, BranchInfo branchInfo, int opcodeLength) 
            : base(machine, opcodeNum, operands, storeVar, branchInfo, opcodeLength)
        {
            this.str = str;
        }

        protected override OperandCount getOperandCount() { return OperandCount.C0OP; }

        public override void execute()
        {
            switch (getOpcodeNum())
            {
                case C0OP_RTRUE:
                    returnFromRoutine(TRUE);
                    break;
                case C0OP_RFALSE:
                    returnFromRoutine(FALSE);
                    break;
                case C0OP_PRINT:
                    getMachine().print(str);
                    nextInstruction();
                    break;
                case C0OP_PRINT_RET:
                    getMachine().print(str);
                    getMachine().newline();
                    returnFromRoutine(TRUE);
                    break;
                case C0OP_NOP:
                    nextInstruction();
                    break;
                case C0OP_SAVE:
                    saveToStorage(getMachine().getPC() + 1);
                    break;
                case C0OP_RESTORE:
                    restoreFromStorage();
                    break;
                case C0OP_RESTART:
                    getMachine().restart();
                    break;
                case C0OP_QUIT:
                    getMachine().quit();
                    break;
                case C0OP_RET_POPPED:
                    returnFromRoutine(getMachine().getVariable((char)0));
                    break;
                case C0OP_POP:
                    if (getMachine().getVersion() < 5)
                    {
                        pop();
                    }
                    else
                    {
                        z_catch();
                    }
                    break;
                case C0OP_NEW_LINE:
                    getMachine().newline();
                    nextInstruction();
                    break;
                case C0OP_SHOW_STATUS:
                    getMachine().updateStatusLine();
                    nextInstruction();
                    break;
                case C0OP_VERIFY:
                    branchOnTest(getMachine().hasValidChecksum());
                    break;
                case C0OP_PIRACY:
                    branchOnTest(true);
                    break;
                default:
                    throwInvalidOpcode();
                    break;
            }
        }

        /// <summary>
        /// Determines whether this instruction is a print instruction.
        /// </summary>
        /// <returns>true if print instruction, false otherwise</returns>
        private bool isPrint()
        {
            return InstructionInfoDb.getInstance().getInfo(getOperandCount(),
                    getOpcodeNum(), getStoryVersion()).isPrint();
        }

        /// <summary>
        /// Returns string representation of operands.
        /// </summary>
        /// <returns>string representation of operands</returns>
        protected override String getOperandString()
        {
            if (isPrint())
            {
                return string.Format("\"{0}\"", str);
            }
            return base.getOperandString();
        }

        /// <summary>
        /// Pop instruction.
        /// </summary>
        private void pop()
        {
            getMachine().getVariable((char)0);
            nextInstruction();
        }

        /// <summary>
        /// Catch instruction.
        /// </summary>
        private void z_catch()
        {
            // Stores the index of the current stack frame
            storeUnsignedResult((char)(getMachine().getRoutineContexts().Count - 1));
            nextInstruction();
        }
    }
}
