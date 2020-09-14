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
    using static Zmpp.Core.Vm.Instruction;

    /// <summary>
    /// Implementation of 1OP instructions.
    /// </summary>
    public class C1OpInstruction : AbstractInstruction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">Machine object</param>
        /// <param name="opcodeNum">opcode number</param>
        /// <param name="operands">operands</param>
        /// <param name="storeVar">store variable number</param>
        /// <param name="branchInfo">branch information</param>
        /// <param name="opcodeLength">opcode length</param>
        public C1OpInstruction(IMachine machine, int opcodeNum, Operand[] operands, char storeVar, BranchInfo branchInfo, int opcodeLength) : base(machine, opcodeNum, operands, storeVar, branchInfo, opcodeLength)
        {
        }

        protected override OperandCount getOperandCount() { return OperandCount.C1OP; }

        public override void Execute()
        {
            switch (getOpcodeNum())
            {
                case C1OP_JZ:
                    Jz();
                    break;
                case C1OP_GET_SIBLING:
                    get_sibling();
                    break;
                case C1OP_GET_CHILD:
                    get_child();
                    break;
                case C1OP_GET_PARENT:
                    get_parent();
                    break;
                case C1OP_GET_PROP_LEN:
                    get_prop_len();
                    break;
                case C1OP_INC:
                    Inc();
                    break;
                case C1OP_DEC:
                    Dec();
                    break;
                case C1OP_PRINT_ADDR:
                    print_addr();
                    break;
                case C1OP_REMOVE_OBJ:
                    remove_obj();
                    break;
                case C1OP_PRINT_OBJ:
                    print_obj();
                    break;
                case C1OP_JUMP:
                    Jump();
                    break;
                case C1OP_RET:
                    ret();
                    break;
                case C1OP_PRINT_PADDR:
                    print_paddr();
                    break;
                case C1OP_LOAD:
                    Load();
                    break;
                case C1OP_NOT:
                    if (getStoryVersion() <= 4)
                    {
                        Not();
                    }
                    else
                    {
                        call_1n();
                    }
                    break;
                case C1OP_CALL_1S:
                    call_1s();
                    break;
                default:
                    throwInvalidOpcode();
                    break;
            }
        }

        /// <summary>
        /// INC instruction.
        /// </summary>
        private void Inc()
        {
            char varNum = getUnsignedValue(0);
            short value = getSignedVarValue(varNum);
            setSignedVarValue(varNum, (short)(value + 1));
            nextInstruction();
        }

        /// <summary>
        /// DEC instruction.
        /// </summary>
        private void Dec()
        {
            char varNum = getUnsignedValue(0);
            short value = getSignedVarValue(varNum);
            setSignedVarValue(varNum, (short)(value - 1));
            nextInstruction();
        }

        /// <summary>
        /// NOT instruction.
        /// </summary>
        private void Not()
        {
            int notvalue = ~getUnsignedValue(0);
            storeUnsignedResult((char)(notvalue & 0xffff));
            nextInstruction();
        }

        /// <summary>
        /// JUMP instruction.
        /// </summary>
        private void Jump()
        {
            getMachine().IncrementPC(getSignedValue(0) + 1);
        }

        /// <summary>
        /// LOAD instruction.
        /// </summary>
        private void Load()
        {
            char varnum = getUnsignedValue(0);
            char value = varnum == 0 ? getMachine().StackTop :
              getMachine().GetVariable(varnum);
            storeUnsignedResult(value);
            nextInstruction();
        }

        /// <summary>
        /// JZ instruction.
        /// </summary>
        private void Jz()
        {
            branchOnTest(getUnsignedValue(0) == 0);
        }

        /// <summary>
        /// GET_PARENT instruction.
        /// </summary>
        private void get_parent()
        {
            int obj = getUnsignedValue(0);
            int parent = 0;
            if (obj > 0)
            {
                parent = getMachine().GetParent(obj);
            }
            else
            {
                getMachine().Warn("@get_parent illegal access to object " + obj);
            }
            storeUnsignedResult((char)(parent & 0xffff));
            nextInstruction();
        }

        /// <summary>
        /// GET_SIBLING instruction.
        /// </summary>
        private void get_sibling()
        {
            int obj = getUnsignedValue(0);
            int sibling = 0;
            if (obj > 0)
            {
                sibling = getMachine().GetSibling(obj);
            }
            else
            {
                getMachine().Warn("@get_sibling illegal access to object " + obj);
            }
            storeUnsignedResult((char)(sibling & 0xffff));
            branchOnTest(sibling > 0);
        }

        /// <summary>
        /// GET_CHILD instruction.
        /// </summary>
        private void get_child()
        {
            int obj = getUnsignedValue(0);
            int child = 0;
            if (obj > 0)
            {
                child = getMachine().GetChild(obj);
            }
            else
            {
                getMachine().Warn("@get_child illegal access to object " + obj);
            }
            storeUnsignedResult((char)(child & 0xffff));
            branchOnTest(child > 0);
        }

        /// <summary>
        /// PRINT_ADDR instruction.
        /// </summary>
        private void print_addr()
        {
            getMachine().PrintZString(getUnsignedValue(0));
            nextInstruction();
        }

        /// <summary>
        /// PRINT_PADDR instruction.
        /// </summary>
        private void print_paddr()
        {
            getMachine().PrintZString(
                getMachine().UnpackStringAddress(getUnsignedValue(0)));
            nextInstruction();
        }

        /// <summary>
        /// RET instruction.
        /// </summary>
        private void ret()
        {
            returnFromRoutine(getUnsignedValue(0));
        }

        /// <summary>
        /// PRINT_OBJ instruction.
        /// </summary>
        private void print_obj()
        {
            int obj = getUnsignedValue(0);
            if (obj > 0)
            {
                getMachine().PrintZString(
                  getMachine().GetPropertiesDescriptionAddress(obj));
            }
            else
            {
                getMachine().Warn("@print_obj illegal access to object " + obj);
            }
            nextInstruction();
        }

        /// <summary>
        /// REMOVE_OBJ instruction.
        /// </summary>
        private void remove_obj()
        {
            int obj = getUnsignedValue(0);
            if (obj > 0)
            {
                getMachine().RemoveObject(obj);
            }
            nextInstruction();
        }

        /// <summary>
        /// GET_PROP_LEN instruction.
        /// </summary>
        private void get_prop_len()
        {
            int propertyAddress = getUnsignedValue(0);
            char proplen = (char)
              getMachine().GetPropertyLength(propertyAddress);
            storeUnsignedResult(proplen);
            nextInstruction();
        }

        /// <summary>
        /// CALL_1S instruction.
        /// </summary>
        private void call_1s() { call(0); }

        /// <summary>
        /// CALL_1N instruction.
        /// </summary>
        private void call_1n() { call(0); }
    }
}
