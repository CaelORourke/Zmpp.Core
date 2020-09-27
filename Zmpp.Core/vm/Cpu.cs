/*
 * Created on 2006/02/14
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
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Zmpp.Core;
    using Zmpp.Core.Vm.Utility;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// Cpu interface implementation.
    /// </summary>
    public class Cpu : ICpu
    {
        private readonly ILogger LOG;

        /// <summary>
        /// The stack size is now 64 K.
        /// </summary>
        private const char StackSize = (char)32768;

        /// <summary>
        /// The machine object.
        /// </summary>
        private readonly IMachine machine;

        /// <summary>
        /// This machine's current program counter.
        /// </summary>
        private int programCounter;

        /// <summary>
        /// This machine's global stack.
        /// </summary>
        private FastShortStack stack;

        /// <summary>
        /// The routine info.
        /// </summary>
        private List<RoutineContext> routineContextStack;

        /// <summary>
        /// The start of global variables.
        /// </summary>
        private int globalsAddress;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">the logger</param>
        /// <param name="machine">the Machine object</param>
        public Cpu(ILogger logger, IMachine machine)
        {
            this.LOG = logger;
            this.machine = machine;
        }

        public void Reset()
        {
            stack = new FastShortStack(StackSize);
            routineContextStack = new List<RoutineContext>();
            globalsAddress = machine.ReadUnsigned16(StoryFileHeaderAddress.Globals);

            if (machine.Version == 6)
            {
                // Call main function in version 6
                Call(ProgramStart, (char)0,
                     new char[0], (char)0);
            }
            else
            {
                programCounter = ProgramStart;
            }
        }

        /// <summary>
        /// Gets the start address of the story.
        /// </summary>
        private char ProgramStart => machine.ReadUnsigned16(StoryFileHeaderAddress.ProgramStart);

        public int PC
        {
            get
            {
                return programCounter;
            }

            set
            {
                programCounter = value;
            }
        }

        /// <summary>
        /// Increments the program counter.
        /// </summary>
        /// <param name="offset">The increment value.</param>
        public void IncrementPC(int offset) { programCounter += offset; }

        public int UnpackStringAddress(char packedAddress)
        {
            int version = machine.Version;
            return version == 6 || version == 7 ?
              packedAddress * 4 + 8 * StaticStringOffset
              : UnpackAddress(packedAddress);
        }

        /// <summary>
        /// Unpacks a routine address.
        /// </summary>
        /// <param name="packedAddress">The packed address.</param>
        /// <returns>The unpacked address.</returns>
        /// <remarks>Exposed for testing.</remarks>
        public int UnpackRoutineAddress(char packedAddress)
        {
            int version = machine.Version;
            return version == 6 || version == 7 ?
              packedAddress * 4 + 8 * RoutineOffset
              : UnpackAddress(packedAddress);
        }

        /// <summary>
        /// Gets the routine offset.
        /// </summary>
        /// <remarks>Only for V6 and V7 games.</remarks>
        private char RoutineOffset => machine.ReadUnsigned16(StoryFileHeaderAddress.RoutineOffset);

        /// <summary>
        /// Gets the static string offset.
        /// </summary>
        /// <remarks>Only for V6 and V7 games.</remarks>
        private char StaticStringOffset => machine.ReadUnsigned16(StoryFileHeaderAddress.StaticStringOffset);

        /// <summary>
        /// Version specific unpacking.
        /// </summary>
        /// <param name="packedAddress">The packed address.</param>
        /// <returns>The unpacked address.</returns>
        private int UnpackAddress(char packedAddress)
        {
            switch (machine.Version)
            {
                case 1:
                case 2:
                case 3:
                    return packedAddress * 2;
                case 4:
                case 5:
                    return packedAddress * 4;
                case 8:
                default:
                    return packedAddress * 8;
            }
        }

        public void DoBranch(short branchOffset, int instructionLength)
        {
            if (branchOffset >= 2 || branchOffset < 0)
            {
                PC = ComputeBranchTarget(branchOffset, instructionLength);
            }
            else
            {
                // FALSE is defined as 0, TRUE as 1, so simply return the offset
                // since we do not have negative offsets
                ReturnWith((char)branchOffset);
            }
        }

        /// <summary>
        /// Computes the branch target.
        /// </summary>
        /// <param name="offset">The offset value.</param>
        /// <param name="instructionLength">The instruction length.</param>
        /// <returns>The branch target value.</returns>
        private int ComputeBranchTarget(short offset, int instructionLength)
        {
            return PC + instructionLength + offset - 2;
        }

        #region Stack operations

        /// <summary>
        /// Gets the global stack pointer.
        /// </summary>
        public char SP => stack.StackPointer;

        /// <summary>
        /// Sets the global stack pointer to the specified value.
        /// </summary>
        /// <param name="stackpointer">The new stack pointer value.</param>
        /// <remarks>This might pop off several values from the stack.</remarks>
        private void SetSP(char stackpointer)
        {
            // remove the last diff elements
            int diff = stack.StackPointer - stackpointer;
            for (int i = 0; i < diff; i++) { stack.Pop(); }
        }

        public char StackTop
        {
            get
            {
                if (stack.Size> 0) { return stack.Top; }
                throw new IndexOutOfRangeException("Stack underflow error");
            }

            set
            {
                stack.ReplaceTopElement(value);
            }
        }

        public char GetStackElement(int index)
        {
            return stack.GetValueAt(index);
        }

        public char PopStack(char userstackAddress)
        {
            return userstackAddress == 0 ? GetVariable((char)0) :
              PopUserStack(userstackAddress);
        }

        /// <summary>
        /// Pops the user stack.
        /// </summary>
        /// <param name="userstackAddress">The address of the user stack.</param>
        /// <returns>The popped value.</returns>
        private char PopUserStack(char userstackAddress)
        {
            int numFreeSlots = machine.ReadUnsigned16(userstackAddress);
            numFreeSlots++;
            machine.WriteUnsigned16(userstackAddress, ToUnsigned16(numFreeSlots));
            return machine.ReadUnsigned16(userstackAddress + (numFreeSlots * 2));
        }

        public bool PushStack(char userstackAddress, char value)
        {
            if (userstackAddress == 0)
            {
                SetVariable((char)0, value);
                return true;
            }
            else
            {
                return PushUserStack(userstackAddress, value);
            }
        }

        /// <summary>
        /// Push user stack.
        /// </summary>
        /// <param name="userstackAddress">The address of the user stack.</param>
        /// <param name="value">The value to push.</param>
        /// <returns>true if successful; otherwise false on overflow.</returns>
        private bool PushUserStack(char userstackAddress, char value)
        {
            int numFreeSlots = machine.ReadUnsigned16(userstackAddress);
            if (numFreeSlots > 0)
            {
                machine.WriteUnsigned16(userstackAddress + (numFreeSlots * 2), value);
                machine.WriteUnsigned16(userstackAddress, ToUnsigned16(numFreeSlots - 1));
                return true;
            }
            return false;
        }

        public char GetVariable(char variableNumber)
        {
            ICpu.VariableType varType = GetVariableType(variableNumber);
            if (varType == ICpu.VariableType.Stack)
            {
                if (stack.Size== InvocationStackPointer)
                {
                    //throw new IllegalStateException("stack underflow error");
                    LOG.LogCritical("stack underflow error");
                    return (char)0;
                }
                else
                {
                    return stack.Pop();
                }
            }
            else if (varType == ICpu.VariableType.Local)
            {
                char localVarNumber = GetLocalVariableNumber(variableNumber);
                CheckLocalVariableAccess(localVarNumber);
                return CurrentRoutineContext.GetLocalVariable(localVarNumber);
            }
            else
            { // GLOBAL
                return machine.ReadUnsigned16(globalsAddress +
                    (GetGlobalVariableNumber(variableNumber) * 2));
            }
        }

        /// <summary>
        /// Gets the current invocation stack pointer.
        /// </summary>
        private char InvocationStackPointer => (char)(CurrentRoutineContext == null ? 0 : CurrentRoutineContext.InvocationStackPointer);

        public void SetVariable(char variableNumber, char value)
        {
            ICpu.VariableType varType = GetVariableType(variableNumber);
            if (varType == ICpu.VariableType.Stack)
            {
                stack.Push(value);
            }
            else if (varType == ICpu.VariableType.Local)
            {
                char localVarNumber = GetLocalVariableNumber(variableNumber);
                CheckLocalVariableAccess(localVarNumber);
                CurrentRoutineContext.SetLocalVariable(localVarNumber, value);
            }
            else
            {
                machine.WriteUnsigned16(globalsAddress +
                    (GetGlobalVariableNumber(variableNumber) * 2), value);
            }
        }

        /// <summary>
        /// Gets the variable type for the specified variable number.
        /// </summary>
        /// <param name="variableNumber">The variable number.</param>
        /// <returns>The variable type.</returns>
        public static ICpu.VariableType GetVariableType(int variableNumber)
        {
            if (variableNumber == 0)
            {
                return ICpu.VariableType.Stack;
            }
            else if (variableNumber < 0x10)
            {
                return ICpu.VariableType.Local;
            }
            else
            {
                return ICpu.VariableType.Global;
            }
        }

        public void PushRoutineContext(RoutineContext routineContext)
        {
            routineContext.InvocationStackPointer = SP;
            routineContextStack.Add(routineContext);
        }

        public void ReturnWith(char returnValue)
        {
            if (routineContextStack.Count > 0)
            {
                //RoutineContext popped = routineContextStack.Remove(routineContextStack.Count - 1);
                RoutineContext popped = routineContextStack[routineContextStack.Count - 1];
                routineContextStack.Remove(popped);
                popped.ReturnValue = returnValue;

                // Restore stack pointer and pc
                SetSP(popped.InvocationStackPointer);
                PC = popped.ReturnAddress;
                char returnVariable = popped.ReturnVariable;
                if (returnVariable != RoutineContext.DiscardResult)
                {
                    SetVariable(returnVariable, returnValue);
                }
            }
            else
            {
                throw new InvalidOperationException("no routine context active");
            }
        }

        public RoutineContext CurrentRoutineContext
        {
            get
            {
                if (routineContextStack.Count == 0)
                {
                    return null;
                }
                return routineContextStack[routineContextStack.Count - 1];
            }
        }

        public List<RoutineContext> GetRoutineContexts()
        {
            return routineContextStack.AsReadOnly().ToList();
        }

        public void SetRoutineContexts(List<RoutineContext> contexts)
        {
            routineContextStack.Clear();
            foreach (RoutineContext context in contexts)
            {
                routineContextStack.Add(context);
            }
        }

        /// <summary>
        /// Gets the routine stack pointer.
        /// </summary>
        /// <remarks>This property is exposed for debugging.</remarks>
        public char RoutineStackPointer => (char)routineContextStack.Count;

        public RoutineContext Call(char packedRoutineAddress, int returnAddress, char[] args, char returnVariable)
        {
            int routineAddress = UnpackRoutineAddress(packedRoutineAddress);
            int numArgs = args == null ? 0 : args.Length;
            RoutineContext routineContext = DecodeRoutine(routineAddress);

            // Sets the number of arguments
            routineContext.NumArguments = numArgs;

            // Save return parameters
            routineContext.ReturnAddress = returnAddress;

            // Only if this instruction stores a result
            if (returnVariable == RoutineContext.DiscardResult)
            {
                routineContext.ReturnVariable = RoutineContext.DiscardResult;
            }
            else
            {
                routineContext.ReturnVariable = returnVariable;
            }

            // Set call parameters into the local variables
            // if there are more parameters than local variables,
            // those are thrown away
            int numToCopy = Math.Min(routineContext.NumLocalVariables, numArgs);

            for (int i = 0; i < numToCopy; i++)
            {
                routineContext.SetLocalVariable((char)i, args[i]);
            }

            // save invocation stack pointer
            routineContext.InvocationStackPointer = SP;

            // Pushes the routine context onto the routine stack
            PushRoutineContext(routineContext);

            // Jump to the address
            PC = machine.Version >= 5 ? routineAddress + 1 :
              routineAddress + 1 + 2 * routineContext.NumLocalVariables;
            return routineContext;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Decodes the routine at the specified address.
        /// </summary>
        /// <param name="routineAddress">The routine address.</param>
        /// <returns>The RoutineContext object.</returns>
        private RoutineContext DecodeRoutine(int routineAddress)
        {
            int numLocals = machine.ReadUnsigned8(routineAddress);
            char[] locals = new char[numLocals];

            if (machine.Version <= 4)
            {
                // Only story files <= 4 actually store default values here,
                // after V5 they are assumed as being 0 (standard document 1.0, S.5.2.1)
                for (int i = 0; i < numLocals; i++)
                {
                    locals[i] = machine.ReadUnsigned16(routineAddress + 1 + 2 * i);
                }
            }
            RoutineContext info = new RoutineContext(numLocals);
            for (int i = 0; i < numLocals; i++)
            {
                info.SetLocalVariable((char)i, locals[i]);
            }
            return info;
        }

        /// <summary>
        /// Gets the local variable number for the specified variable number.
        /// </summary>
        /// <param name="variableNumber">The variable number in an operand (0x01-0x0f).</param>
        /// <returns>The local variable number.</returns>
        private char GetLocalVariableNumber(char variableNumber)
        {
            return (char)(variableNumber - 1);
        }

        /// <summary>
        /// Gets the global variable number for the specified variable number.
        /// </summary>
        /// <param name="variableNumber">The variable number in an operand (0x10-0xff).</param>
        /// <returns>The global variable number.</returns>
        private char GetGlobalVariableNumber(char variableNumber)
        {
            return (char)(variableNumber - 0x10);
        }

        /// <summary>
        /// This function throws an exception if a non-existing local variable
        /// is accessed on the current routine context or no current routine context
        /// is set.
        /// </summary>
        /// <param name="localVariableNumber">The local variable number.</param>
        private void CheckLocalVariableAccess(char localVariableNumber)
        {
            if (routineContextStack.Count == 0)
            {
                throw new InvalidOperationException("There is no routine context set.");
            }

            if (localVariableNumber >= CurrentRoutineContext
                .NumLocalVariables)
            {
                throw new InvalidOperationException("Access to non-existent local variable: " + (int)localVariableNumber);
            }
        }

        #endregion
    }
}