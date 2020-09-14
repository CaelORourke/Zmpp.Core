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
    using System.Collections.Generic;

    /// <summary>
    /// Cpu interface.
    /// </summary>
    public interface ICpu
    {
        /// <summary>
        /// The possible variable types.
        /// </summary>
        enum VariableType { STACK, LOCAL, GLOBAL }

        /// <summary>
        /// Resets this object to initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Translates a packed string address into a byte address.
        /// </summary>
        /// <param name="packedAddress">The packed address.</param>
        /// <returns>The translated byte address.</returns>
        int UnpackStringAddress(char packedAddress);

        /// <summary>
        /// Computes a branch target from an offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="instructionLength">The instruction length.</param>
        void DoBranch(short offset, int instructionLength);

        /// <summary>
        /// Gets or sets the current program counter.
        /// </summary>
        int PC { get; set; }

        /// <summary>
        /// Increments the program counter by the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        void IncrementPC(int offset);

        #region Stack operations

        /// <summary>
        /// Gets the global stack pointer.
        /// </summary>
        /// <remarks>Equals the stack size.</remarks>
        char SP { get; }

        /// <summary>
        /// Gets the value at the top of the stack without removing it.
        /// </summary>
        char StackTop { get; }

        /// <summary>
        /// Sets the value of the element at the top of the stack without
        /// incrementing the stack pointer.
        /// </summary>
        /// <param name="value">the value to set</param>
        void setStackTop(char value);

        /// <summary>
        /// Returns the evaluation stack element at the specified index.
        /// </summary>
        /// <param name="index">an index</param>
        /// <returns>the stack value at the specified index</returns>
        char GetStackElement(int index);

        /// <summary>
        /// Pushes the specified value on the user stack.
        /// </summary>
        /// <param name="userstackAddress">the address of the user stack</param>
        /// <param name="value">the value to push</param>
        /// <returns>true if operation was ok, false if overflow</returns>
        bool PushStack(char userstackAddress, char value);

        /// <summary>
        /// Pops the specified value from the user stack.
        /// </summary>
        /// <param name="userstackAddress">the address of the user stack</param>
        /// <returns>the popped value</returns>
        char PopStack(char userstackAddress);

        #endregion

        #region Variable access

        /// <summary>
        /// Returns the value of the specified variable. 0 is the stack pointer,
        /// 0x01-0x0f are local variables, and 0x10-0xff are global variables.
        /// If the stack pointer is read from, its top value will be popped off.
        /// </summary>
        /// <param name="variableNumber">the variable number</param>
        /// <returns>the value of the variable</returns>
        /// <remarks>
        /// throws IllegalStateException if a local variable is accessed without
        /// a subroutine context or if a non-existent local variable is accessed
        /// </remarks>
        char GetVariable(char variableNumber);

        /// <summary>
        /// Sets the value of the specified variable. If the stack pointer is written
        /// to, the stack will contain one more value.
        /// </summary>
        /// <param name="variableNumber">the variable number</param>
        /// <param name="value">the value to write</param>
        /// <remarks>
        /// throws IllegalStateException if a local variable is accessed without
        /// a subroutine context or if a non-existent local variable is accessed
        /// </remarks>
        void SetVariable(char variableNumber, char value);

        #endregion

        #region Routine stack frames

        /// <summary>
        /// Pops the current routine context from the stack. It will also
        /// restore the state before the invocation of the routine, i.e.it
        /// will restore the program counter and the stack pointers and set
        /// the specfied return value to the return variable.
        /// </summary>
        /// <param name="returnValue">the return value</param>
        /// <remarks>
        /// throws IllegalStateException if no RoutineContext exists
        /// </remarks>
        void ReturnWith(char returnValue);

        /// <summary>
        /// Returns the state of the current routine context stack as a non-
        /// modifiable List.
        /// </summary>
        /// <returns>the list of routine contexts</returns>
        /// <remarks>This is exposed to PortableGameState to take a
        /// machine state snapshot.</remarks>
        List<RoutineContext> getRoutineContexts();

        /// <summary>
        /// Copies the list of routine contexts into this machine's routine context
        /// stack.
        /// </summary>
        /// <param name="contexts">a list of routine contexts</param>
        /// <remarks>This is a consequence of a restore operation.</remarks>
        void setRoutineContexts(List<RoutineContext> contexts);

        /// <summary>
        /// Returns the current routine context without affecting the state
        /// of the machine.
        /// </summary>
        /// <returns>the current routine context</returns>
        RoutineContext getCurrentRoutineContext();

        /// <summary>
        /// Performs a routine call.
        /// </summary>
        /// <param name="routineAddress">the packed routine address</param>
        /// <param name="returnAddress">the return address</param>
        /// <param name="args">the argument list</param>
        /// <param name="returnVariable">the return variable or DISCARD_RESULT</param>
        /// <returns>the routine context created</returns>
        RoutineContext Call(char routineAddress, int returnAddress, char[] args, char returnVariable);

        #endregion
    }
}
