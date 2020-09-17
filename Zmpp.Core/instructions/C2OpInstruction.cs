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
    using Zmpp.Core.UI;
    using static Zmpp.Core.Vm.Instruction;

    /// <summary>
    /// Implementation for 2OP operand count instructions.
    /// </summary>
    public class C2OpInstruction : AbstractInstruction
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
        public C2OpInstruction(IMachine machine, int opcodeNum, Operand[] operands, char storeVar, BranchInfo branchInfo, int opcodeLength)
            : base(machine, opcodeNum, operands, storeVar, branchInfo, opcodeLength)
        {
        }

        protected override OperandCount getOperandCount() { return OperandCount.C2OP; }

        public override void Execute()
        {
            switch (getOpcodeNum())
            {
                case C2OP_JE:
                    je();
                    break;
                case C2OP_JL:
                    jl();
                    break;
                case C2OP_JG:
                    jg();
                    break;
                case C2OP_JIN:
                    jin();
                    break;
                case C2OP_DEC_CHK:
                    dec_chk();
                    break;
                case C2OP_INC_CHK:
                    inc_chk();
                    break;
                case C2OP_TEST:
                    test();
                    break;
                case C2OP_OR:
                    or();
                    break;
                case C2OP_AND:
                    and();
                    break;
                case C2OP_TEST_ATTR:
                    test_attr();
                    break;
                case C2OP_SET_ATTR:
                    set_attr();
                    break;
                case C2OP_CLEAR_ATTR:
                    clear_attr();
                    break;
                case C2OP_STORE:
                    store();
                    break;
                case C2OP_INSERT_OBJ:
                    insert_obj();
                    break;
                case C2OP_LOADW:
                    loadw();
                    break;
                case C2OP_LOADB:
                    loadb();
                    break;
                case C2OP_GET_PROP:
                    get_prop();
                    break;
                case C2OP_GET_PROP_ADDR:
                    get_prop_addr();
                    break;
                case C2OP_GET_NEXT_PROP:
                    get_next_prop();
                    break;
                case C2OP_ADD:
                    add();
                    break;
                case C2OP_SUB:
                    sub();
                    break;
                case C2OP_MUL:
                    mul();
                    break;
                case C2OP_DIV:
                    div();
                    break;
                case C2OP_MOD:
                    mod();
                    break;
                case C2OP_CALL_2S:
                    call(1);
                    break;
                case C2OP_CALL_2N:
                    call(1);
                    break;
                case C2OP_SET_COLOUR:
                    set_colour();
                    break;
                case C2OP_THROW:
                    z_throw();
                    break;
                default:
                    throwInvalidOpcode();
                    break;
            }
        }

        /// <summary>
        /// JE instruction.
        /// </summary>
        private void je()
        {
            bool equalsFollowing = false;
            char op1 = getUnsignedValue(0);
            if (getNumOperands() <= 1)
            {
                getMachine().Halt("je expects at least two operands, only " + "one provided");
            }
            else
            {
                for (int i = 1; i < getNumOperands(); i++)
                {
                    char value = getUnsignedValue(i);
                    if (op1 == value)
                    {
                        equalsFollowing = true;
                        break;
                    }
                }
                branchOnTest(equalsFollowing);
            }
        }

        /// <summary>
        /// JL instruction.
        /// </summary>
        private void jl()
        {
            short op1 = getSignedValue(0);
            short op2 = getSignedValue(1);
            //System.out.printf("Debugging jl op1: %d op2: %d\n", op1, op2);
            branchOnTest(op1 < op2);
        }

        /// <summary>
        /// JG instruction.
        /// </summary>
        private void jg()
        {
            short op1 = getSignedValue(0);
            short op2 = getSignedValue(1);
            branchOnTest(op1 > op2);
        }

        /// <summary>
        /// JIN instruction.
        /// </summary>
        private void jin()
        {
            int obj1 = getUnsignedValue(0);
            int obj2 = getUnsignedValue(1);
            int parentOfObj1 = 0;

            if (obj1 > 0)
            {
                parentOfObj1 = getMachine().GetParent(obj1);
            }
            else
            {
                getMachine().Warn("@jin illegal access to object " + obj1);
            }
            branchOnTest(parentOfObj1 == obj2);
        }

        /// <summary>
        /// DEC_CHK instruction.
        /// </summary>
        private void dec_chk()
        {
            char varnum = getUnsignedValue(0);
            short value = getSignedValue(1);
            short varValue = (short)(getSignedVarValue(varnum) - 1);
            setSignedVarValue(varnum, varValue);
            branchOnTest(varValue < value);
        }

        /// <summary>
        /// INC_CHK instruction.
        /// </summary>
        private void inc_chk()
        {
            char varnum = getUnsignedValue(0);
            short value = getSignedValue(1);
            short varValue = (short)(getSignedVarValue(varnum) + 1);
            setSignedVarValue(varnum, varValue);
            branchOnTest(varValue > value);
        }

        /// <summary>
        /// TEST instruction.
        /// </summary>
        private void test()
        {
            int op1 = getUnsignedValue(0);
            int op2 = getUnsignedValue(1);
            branchOnTest((op1 & op2) == op2);
        }

        /// <summary>
        /// OR instruction.
        /// </summary>
        private void or()
        {
            int op1 = getUnsignedValue(0);
            int op2 = getUnsignedValue(1);
            storeUnsignedResult((char)((op1 | op2) & 0xffff));
            nextInstruction();
        }

        /// <summary>
        /// AND instruction.
        /// </summary>
        private void and()
        {
            int op1 = getUnsignedValue(0);
            int op2 = getUnsignedValue(1);
            storeUnsignedResult((char)((op1 & op2) & 0xffff));
            nextInstruction();
        }

        /// <summary>
        /// ADD instruction.
        /// </summary>
        private void add()
        {
            short op1 = getSignedValue(0);
            short op2 = getSignedValue(1);
            storeSignedResult((short)(op1 + op2));
            nextInstruction();
        }

        /// <summary>
        /// SUB instruction.
        /// </summary>
        private void sub()
        {
            short op1 = getSignedValue(0);
            short op2 = getSignedValue(1);
            storeSignedResult((short)(op1 - op2));
            nextInstruction();
        }

        /// <summary>
        /// MUL instruction.
        /// </summary>
        private void mul()
        {
            short op1 = getSignedValue(0);
            short op2 = getSignedValue(1);
            storeSignedResult((short)(op1 * op2));
            nextInstruction();
        }

        /// <summary>
        /// DIV instruction.
        /// </summary>
        private void div()
        {
            short op1 = getSignedValue(0);
            short op2 = getSignedValue(1);
            if (op2 == 0)
            {
                getMachine().Halt("@div division by zero");
            }
            else
            {
                storeSignedResult((short)(op1 / op2));
                nextInstruction();
            }
        }

        /// <summary>
        /// MOD instruction.
        /// </summary>
        private void mod()
        {
            short op1 = getSignedValue(0);
            short op2 = getSignedValue(1);
            if (op2 == 0)
            {
                getMachine().Halt("@mod division by zero");
            }
            else
            {
                storeSignedResult((short)(op1 % op2));
                nextInstruction();
            }
        }

        /// <summary>
        /// TEST_ATTR instruction.
        /// </summary>
        private void test_attr()
        {
            int obj = getUnsignedValue(0);
            int attr = getUnsignedValue(1);
            if (obj > 0 && isValidAttribute(attr))
            {
                branchOnTest(getMachine().IsAttributeSet(obj, attr));
            }
            else
            {
                getMachine().Warn("@test_attr illegal access to object " + obj);
                branchOnTest(false);
            }
        }

        /// <summary>
        /// SET_ATTR instruction.
        /// </summary>
        private void set_attr()
        {
            int obj = getUnsignedValue(0);
            int attr = getUnsignedValue(1);
            if (obj > 0 && isValidAttribute(attr))
            {
                getMachine().SetAttribute(obj, attr);
            }
            else
            {
                getMachine().Warn("@set_attr illegal access to object " + obj +
                                  " attr: " + attr);
            }
            nextInstruction();
        }

        /// <summary>
        /// CLEAR_ATTR instruction.
        /// </summary>
        private void clear_attr()
        {
            int obj = getUnsignedValue(0);
            int attr = getUnsignedValue(1);
            if (obj > 0 && isValidAttribute(attr))
            {
                getMachine().ClearAttribute(obj, attr);
            }
            else
            {
                getMachine().Warn("@clear_attr illegal access to object " + obj + " attr: " + attr);
            }
            nextInstruction();
        }

        /// <summary>
        /// STORE instruction.
        /// </summary>
        private void store()
        {
            char varnum = getUnsignedValue(0);
            char value = getUnsignedValue(1);
            // Handle stack variable as a special case (standard 1.1)
            if (varnum == 0)
            {
                getMachine().setStackTop(value);
            }
            else
            {
                getMachine().SetVariable(varnum, value);
            }
            nextInstruction();
        }

        /// <summary>
        /// INSERT_OBJ instruction.
        /// </summary>
        private void insert_obj()
        {
            int obj = getUnsignedValue(0);
            int dest = getUnsignedValue(1);
            if (obj > 0 && dest > 0)
            {
                getMachine().InsertObject(dest, obj);
            }
            else
            {
                getMachine().Warn("@insert_obj with object 0 called, obj: " + obj + ", dest: " + dest);
            }
            nextInstruction();
        }

        /// <summary>
        /// LOADB instruction.
        /// </summary>
        private void loadb()
        {
            int arrayAddress = getUnsignedValue(0);
            int index = getUnsignedValue(1);
            int memAddress = (arrayAddress + index) & 0xffff;
            storeUnsignedResult((char)getMachine().ReadUnsigned8(memAddress));
            nextInstruction();
        }

        /// <summary>
        /// LOADW instruction.
        /// </summary>
        private void loadw()
        {
            int arrayAddress = getUnsignedValue(0);
            int index = getUnsignedValue(1);
            int memAddress = (arrayAddress + 2 * index) & 0xffff;
            storeUnsignedResult(getMachine().ReadUnsigned16(memAddress));
            nextInstruction();
        }

        /// <summary>
        /// GET_PROP instruction.
        /// </summary>
        private void get_prop()
        {
            int obj = getUnsignedValue(0);
            int property = getUnsignedValue(1);

            if (obj > 0)
            {
                char value = (char)getMachine().GetProperty(obj, property);
                storeUnsignedResult(value);
            }
            else
            {
                getMachine().Warn("@get_prop illegal access to object " + obj);
            }
            nextInstruction();
        }

        /// <summary>
        /// GET_PROP_ADDR instruction.
        /// </summary>
        private void get_prop_addr()
        {
            int obj = getUnsignedValue(0);
            int property = getUnsignedValue(1);
            if (obj > 0)
            {
                char value = (char)
                  (getMachine().GetPropertyAddress(obj, property) & 0xffff);
                storeUnsignedResult(value);
            }
            else
            {
                getMachine().Warn("@get_prop_addr illegal access to object " + obj);
            }
            nextInstruction();
        }

        /// <summary>
        /// GET_NEXT_PROP instruction.
        /// </summary>
        private void get_next_prop()
        {
            int obj = getUnsignedValue(0);
            int property = getUnsignedValue(1);
            char value = (char)0;
            if (obj > 0)
            {
                value = (char)(getMachine().GetNextProperty(obj, property) & 0xffff);
                storeUnsignedResult(value);
                nextInstruction();
            }
            else
            {
                // issue warning and continue
                getMachine().Warn("@get_next_prop illegal access to object " + obj);
                nextInstruction();
            }
        }

        /// <summary>
        /// SET_COLOUR instruction.
        /// </summary>
        private void set_colour()
        {
            int window = ScreenModel.CURRENT_WINDOW;
            if (getNumOperands() == 3)
            {
                window = getSignedValue(2);
            }
            getMachine().Screen.SetForeground(getSignedValue(0), window);
            getMachine().Screen.SetBackground(getSignedValue(1), window);
            nextInstruction();
        }

        /// <summary>
        /// THROW instruction.
        /// </summary>
        private void z_throw()
        {
            char returnValue = getUnsignedValue(0);
            int stackFrame = getUnsignedValue(1);

            // Unwind the stack
            int currentStackFrame = getMachine().getRoutineContexts().Count - 1;
            if (currentStackFrame < stackFrame)
            {
                getMachine().Halt("@throw from an invalid stack frame state");
            }
            else
            {
                // Pop off the routine contexts until the specified stack frame is
                // reached
                int diff = currentStackFrame - stackFrame;
                for (int i = 0; i < diff; i++)
                {
                    getMachine().ReturnWith((char)0);
                }

                // and return with the return value
                returnFromRoutine(returnValue);
            }
        }

        /// <summary>
        /// Checks if the specified attribute is valid
        /// </summary>
        /// <param name="attribute">attribute number</param>
        /// <returns>true if valid, false otherwise</returns>
        private bool isValidAttribute(int attribute)
        {
            int numAttr = getStoryVersion() <= 3 ? 32 : 48;
            return attribute >= 0 && attribute < numAttr;
        }
    }
}
