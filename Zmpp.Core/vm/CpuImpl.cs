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

namespace org.zmpp.vm
{
    using Microsoft.Extensions.Logging;
    using org.zmpp.@base;
    using org.zmpp.vmutil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static org.zmpp.@base.MemoryUtil;

    /// <summary>
    /// Cpu interface implementation.
    /// </summary>
    public class CpuImpl : ICpu
    {
        private readonly ILogger LOG;

        /// <summary>
        /// The stack size is now 64 K.
        /// </summary>
        private const char STACKSIZE = (char)32768;

        /// <summary>
        /// The machine object.
        /// </summary>
        private IMachine machine;

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
        public CpuImpl(ILogger logger, IMachine machine)
        {
            this.LOG = logger;
            this.machine = machine;
        }

        public void reset()
        {
            stack = new FastShortStack(STACKSIZE);
            routineContextStack = new List<RoutineContext>();
            globalsAddress = machine.readUnsigned16(StoryFileHeaderBase.GLOBALS);

            if (machine.getVersion() == 6)
            {
                // Call main function in version 6
                call(getProgramStart(), (char)0,
                     new char[0], (char)0);
            }
            else
            {
                programCounter = getProgramStart();
            }
        }

        /// <summary>
        /// Returns the story's start address.
        /// </summary>
        /// <returns>the start address</returns>
        private char getProgramStart()
        {
            return machine.readUnsigned16(StoryFileHeaderBase.PROGRAM_START);
        }

        public int getPC() { return programCounter; }

        public void setPC(int address) { programCounter = address; }

        /// <summary>
        /// Increments the program counter.
        /// </summary>
        /// <param name="offset">the increment value.</param>
        public void incrementPC(int offset) { programCounter += offset; }

        public int unpackStringAddress(char packedAddress)
        {
            int version = machine.getVersion();
            return version == 6 || version == 7 ?
              packedAddress * 4 + 8 * getStaticStringOffset()
              : unpackAddress(packedAddress);
        }

        /// <summary>
        /// Unpacks a routine address, exposed for testing.
        /// </summary>
        /// <param name="packedAddress">the packed address</param>
        /// <returns>the unpacked address</returns>
        public int unpackRoutineAddress(char packedAddress)
        {
            int version = machine.getVersion();
            return version == 6 || version == 7 ?
              packedAddress * 4 + 8 * getRoutineOffset()
              : unpackAddress(packedAddress);
        }

        /// <summary>
        /// Only for V6 and V7 games: the routine offset.
        /// </summary>
        /// <returns>the routine offset</returns>
        private char getRoutineOffset()
        {
            return machine.readUnsigned16(StoryFileHeaderBase.ROUTINE_OFFSET);
        }

        /// <summary>
        /// Only in V6 and V7: the static string offset.
        /// </summary>
        /// <returns>the static string offset</returns>
        private char getStaticStringOffset()
        {
            return machine.readUnsigned16(StoryFileHeaderBase.STATIC_STRING_OFFSET);
        }

        /// <summary>
        /// Version specific unpacking.
        /// </summary>
        /// <param name="packedAddress">the packed address</param>
        /// <returns>the unpacked address</returns>
        private int unpackAddress(char packedAddress)
        {
            switch (machine.getVersion())
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

        public void doBranch(short branchOffset, int instructionLength)
        {
            if (branchOffset >= 2 || branchOffset < 0)
            {
                setPC(computeBranchTarget(branchOffset, instructionLength));
            }
            else
            {
                // FALSE is defined as 0, TRUE as 1, so simply return the offset
                // since we do not have negative offsets
                returnWith((char)branchOffset);
            }
        }

        /// <summary>
        /// Computes the branch target.
        /// </summary>
        /// <param name="offset">offset value</param>
        /// <param name="instructionLength">instruction length</param>
        /// <returns>branch target value</returns>
        private int computeBranchTarget(short offset, int instructionLength)
        {
            return getPC() + instructionLength + offset - 2;
        }

        #region Stack operations

        public char getSP() { return stack.getStackPointer(); }

        /// <summary>
        /// Sets the global stack pointer to the specified value. This might pop off
        /// several values from the stack.
        /// </summary>
        /// <param name="stackpointer">the new stack pointer value</param>
        private void setSP(char stackpointer)
        {
            // remove the last diff elements
            int diff = stack.getStackPointer() - stackpointer;
            for (int i = 0; i < diff; i++) { stack.pop(); }
        }

        public char getStackTop()
        {
            if (stack.size() > 0) { return stack.top(); }
            throw new IndexOutOfRangeException("Stack underflow error");
        }

        public void setStackTop(char value)
        {
            stack.replaceTopElement(value);
        }

        public char getStackElement(int index)
        {
            return stack.getValueAt(index);
        }

        public char popStack(char userstackAddress)
        {
            return userstackAddress == 0 ? getVariable((char)0) :
              popUserStack(userstackAddress);
        }

        /// <summary>
        /// Pops the user stack.
        /// </summary>
        /// <param name="userstackAddress">address of user stack</param>
        /// <returns>popped value</returns>
        private char popUserStack(char userstackAddress)
        {
            int numFreeSlots = machine.readUnsigned16(userstackAddress);
            numFreeSlots++;
            machine.writeUnsigned16(userstackAddress, toUnsigned16(numFreeSlots));
            return machine.readUnsigned16(userstackAddress + (numFreeSlots * 2));
        }

        public bool pushStack(char userstackAddress, char value)
        {
            if (userstackAddress == 0)
            {
                setVariable((char)0, value);
                return true;
            }
            else
            {
                return pushUserStack(userstackAddress, value);
            }
        }

        /// <summary>
        /// Push user stack.
        /// </summary>
        /// <param name="userstackAddress">address of user stack</param>
        /// <param name="value">value to push</param>
        /// <returns>true if successful, false on overflow</returns>
        private bool pushUserStack(char userstackAddress, char value)
        {
            int numFreeSlots = machine.readUnsigned16(userstackAddress);
            if (numFreeSlots > 0)
            {
                machine.writeUnsigned16(userstackAddress + (numFreeSlots * 2), value);
                machine.writeUnsigned16(userstackAddress, toUnsigned16(numFreeSlots - 1));
                return true;
            }
            return false;
        }

        public char getVariable(char variableNumber)
        {
            ICpu.VariableType varType = getVariableType(variableNumber);
            if (varType == ICpu.VariableType.STACK)
            {
                if (stack.size() == getInvocationStackPointer())
                {
                    //throw new IllegalStateException("stack underflow error");
                    LOG.LogCritical("stack underflow error");
                    return (char)0;
                }
                else
                {
                    return stack.pop();
                }
            }
            else if (varType == ICpu.VariableType.LOCAL)
            {
                char localVarNumber = getLocalVariableNumber(variableNumber);
                checkLocalVariableAccess(localVarNumber);
                return getCurrentRoutineContext().getLocalVariable(localVarNumber);
            }
            else
            { // GLOBAL
                return machine.readUnsigned16(globalsAddress +
                    (getGlobalVariableNumber(variableNumber) * 2));
            }
        }

        /// <summary>
        /// Returns the current invocation stack pointer.
        /// </summary>
        /// <returns>the invocation stack pointer</returns>
        private char getInvocationStackPointer()
        {
            return (char)(getCurrentRoutineContext() == null ? 0 :
              getCurrentRoutineContext().getInvocationStackPointer());
        }

        public void setVariable(char variableNumber, char value)
        {
            ICpu.VariableType varType = getVariableType(variableNumber);
            if (varType == ICpu.VariableType.STACK)
            {
                stack.push(value);
            }
            else if (varType == ICpu.VariableType.LOCAL)
            {
                char localVarNumber = getLocalVariableNumber(variableNumber);
                checkLocalVariableAccess(localVarNumber);
                getCurrentRoutineContext().setLocalVariable(localVarNumber, value);
            }
            else
            {
                machine.writeUnsigned16(globalsAddress +
                    (getGlobalVariableNumber(variableNumber) * 2), value);
            }
        }

        /// <summary>
        /// Returns the variable type for the given variable number.
        /// </summary>
        /// <param name="variableNumber">the variable number</param>
        /// <returns>STACK if stack variable, LOCAL if local variable, GLOBAL if global</returns>
        public static ICpu.VariableType getVariableType(int variableNumber)
        {
            if (variableNumber == 0)
            {
                return ICpu.VariableType.STACK;
            }
            else if (variableNumber < 0x10)
            {
                return ICpu.VariableType.LOCAL;
            }
            else
            {
                return ICpu.VariableType.GLOBAL;
            }
        }

        public void pushRoutineContext(RoutineContext routineContext)
        {
            routineContext.setInvocationStackPointer(getSP());
            routineContextStack.Add(routineContext);
        }

        public void returnWith(char returnValue)
        {
            if (routineContextStack.Count > 0)
            {
                //RoutineContext popped = routineContextStack.Remove(routineContextStack.Count - 1);
                RoutineContext popped = routineContextStack[routineContextStack.Count - 1];
                routineContextStack.Remove(popped);
                popped.setReturnValue(returnValue);

                // Restore stack pointer and pc
                setSP(popped.getInvocationStackPointer());
                setPC(popped.getReturnAddress());
                char returnVariable = popped.getReturnVariable();
                if (returnVariable != RoutineContext.DISCARD_RESULT)
                {
                    setVariable(returnVariable, returnValue);
                }
            }
            else
            {
                throw new InvalidOperationException("no routine context active");
            }
        }

        public RoutineContext getCurrentRoutineContext()
        {
            if (routineContextStack.Count == 0)
            {
                return null;
            }
            return routineContextStack[routineContextStack.Count - 1];
        }

        public List<RoutineContext> getRoutineContexts()
        {
            return routineContextStack.AsReadOnly().ToList();
        }

        public void setRoutineContexts(List<RoutineContext> contexts)
        {
            routineContextStack.Clear();
            foreach (RoutineContext context in contexts)
            {
                routineContextStack.Add(context);
            }
        }

        /// <summary>
        /// This function is basically exposed to the debug application.
        /// </summary>
        /// <returns>the current routine stack pointer</returns>
        public char getRoutineStackPointer()
        {
            return (char)routineContextStack.Count;
        }

        public RoutineContext call(char packedRoutineAddress, int returnAddress, char[] args, char returnVariable)
        {
            int routineAddress = unpackRoutineAddress(packedRoutineAddress);
            int numArgs = args == null ? 0 : args.Length;
            RoutineContext routineContext = decodeRoutine(routineAddress);

            // Sets the number of arguments
            routineContext.setNumArguments(numArgs);

            // Save return parameters
            routineContext.setReturnAddress(returnAddress);

            // Only if this instruction stores a result
            if (returnVariable == RoutineContext.DISCARD_RESULT)
            {
                routineContext.setReturnVariable(RoutineContext.DISCARD_RESULT);
            }
            else
            {
                routineContext.setReturnVariable(returnVariable);
            }

            // Set call parameters into the local variables
            // if there are more parameters than local variables,
            // those are thrown away
            int numToCopy = Math.Min(routineContext.getNumLocalVariables(), numArgs);

            for (int i = 0; i < numToCopy; i++)
            {
                routineContext.setLocalVariable((char)i, args[i]);
            }

            // save invocation stack pointer
            routineContext.setInvocationStackPointer(getSP());

            // Pushes the routine context onto the routine stack
            pushRoutineContext(routineContext);

            // Jump to the address
            setPC(machine.getVersion() >= 5 ? routineAddress + 1 :
              routineAddress + 1 + 2 * routineContext.getNumLocalVariables());
            return routineContext;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Decodes the routine at the specified address.
        /// </summary>
        /// <param name="routineAddress">the routine address</param>
        /// <returns>a RoutineContext object</returns>
        private RoutineContext decodeRoutine(int routineAddress)
        {
            int numLocals = machine.readUnsigned8(routineAddress);
            char[] locals = new char[numLocals];

            if (machine.getVersion() <= 4)
            {
                // Only story files <= 4 actually store default values here,
                // after V5 they are assumed as being 0 (standard document 1.0, S.5.2.1)
                for (int i = 0; i < numLocals; i++)
                {
                    locals[i] = machine.readUnsigned16(routineAddress + 1 + 2 * i);
                }
            }
            RoutineContext info = new RoutineContext(numLocals);
            for (int i = 0; i < numLocals; i++)
            {
                info.setLocalVariable((char)i, locals[i]);
            }
            return info;
        }

        /// <summary>
        /// Returns the local variable number for a specified variable number.
        /// </summary>
        /// <param name="variableNumber">the variable number in an operand (0x01-0x0f)</param>
        /// <returns>the local variable number</returns>
        private char getLocalVariableNumber(char variableNumber)
        {
            return (char)(variableNumber - 1);
        }

        /// <summary>
        /// Returns the global variable for the specified variable number.
        /// </summary>
        /// <param name="variableNumber">a variable number (0x10-0xff)</param>
        /// <returns>the global variable number</returns>
        private char getGlobalVariableNumber(char variableNumber)
        {
            return (char)(variableNumber - 0x10);
        }

        /// <summary>
        /// This function throws an exception if a non-existing local variable
        /// is accessed on the current routine context or no current routine context
        /// is set.
        /// </summary>
        /// <param name="localVariableNumber">the local variable number</param>
        private void checkLocalVariableAccess(char localVariableNumber)
        {
            if (routineContextStack.Count == 0)
            {
                throw new InvalidOperationException("no routine context set");
            }

            if (localVariableNumber >= getCurrentRoutineContext()
                .getNumLocalVariables())
            {
                throw new InvalidOperationException("access to non-existent local variable: " + (int)localVariableNumber);
            }
        }

        #endregion
    }
}