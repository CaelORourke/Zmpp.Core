/*
 * Created on 10/03/2005
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
    /// <summary>
    /// This class holds information about a subroutine.
    /// </summary>
    public class RoutineContext
    {
        /// <summary>
        /// Set as return variable value if the call is a call_nx.
        /// </summary>
        public const char DISCARD_RESULT = (char)0xffff;

        /// <summary>
        /// The local variables
        /// </summary>
        private char[] locals;

        /// <summary>
        /// The return address.
        /// </summary>
        private int returnAddress;

        /// <summary>
        /// The return variable number to store the return value to.
        /// </summary>
        private char returnVarNum;

        /// <summary>
        /// The stack pointer at invocation time.
        /// </summary>
        private char invocationStackPointer;

        /// <summary>
        /// The number of arguments.
        /// </summary>
        private int numArgs;

        /// <summary>
        /// The return value.
        /// </summary>
        private char returnValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numLocalVariables">the number of local variables</param>
        public RoutineContext(int numLocalVariables)
        {
            locals = new char[numLocalVariables];
        }

        /// <summary>
        /// Sets the number of arguments.
        /// </summary>
        /// <param name="aNumArgs">the number of arguments</param>
        public void setNumArguments(int aNumArgs)
        {
            this.numArgs = aNumArgs;
        }

        /// <summary>
        /// Returns the number of arguments.
        /// </summary>
        /// <returns>the number of arguments</returns>
        public int getNumArguments() { return numArgs; }

        /// <summary>
        /// Returns the number of local variables.
        /// </summary>
        /// <returns>the number of local variables</returns>
        public int getNumLocalVariables()
        {
            return (locals == null) ? 0 : locals.Length;
        }

        /// <summary>
        /// Sets a value to the specified local variable.
        /// </summary>
        /// <param name="localNum">the local variable number, starting with 0</param>
        /// <param name="value">the value</param>
        public void setLocalVariable(char localNum, char value)
        {
            locals[localNum] = value;
        }

        /// <summary>
        /// Retrieves the value of the specified local variable.
        /// </summary>
        /// <param name="localNum">the local variable number, starting at 0</param>
        /// <returns>the value of the specified variable</returns>
        public char getLocalVariable(char localNum)
        {
            return locals[localNum];
        }

        /// <summary>
        /// Returns the routine's return address.
        /// </summary>
        /// <returns>the routine's return address</returns>
        public int getReturnAddress() { return returnAddress; }

        /// <summary>
        /// Sets the return address.
        /// </summary>
        /// <param name="address">the return address</param>
        public void setReturnAddress(int address)
        {
            this.returnAddress = address;
        }

        /// <summary>
        /// Returns the routine's return variable number.
        /// </summary>
        /// <returns>the return variable number or DISCARD_RESULT</returns>
        public char getReturnVariable() { return returnVarNum; }

        /// <summary>
        /// Sets the routine's return variable number.
        /// </summary>
        /// <param name="varnum">the return variable number or DISCARD_RESULT</param>
        public void setReturnVariable(char varnum) { returnVarNum = varnum; }

        /// <summary>
        /// Returns the stack pointer at invocation time.
        /// </summary>
        /// <returns>the stack pointer at invocation time</returns>
        public char getInvocationStackPointer() { return invocationStackPointer; }

        /// <summary>
        /// Sets the stack pointer at invocation time.
        /// </summary>
        /// <param name="stackpointer">the stack pointer at invocation time.</param>
        public void setInvocationStackPointer(char stackpointer)
        {
            invocationStackPointer = stackpointer;
        }

        /// <summary>
        /// Returns the return value.
        /// </summary>
        /// <returns>the return value</returns>
        public char getReturnValue() { return returnValue; }

        /// <summary>
        /// Sets the return value.
        /// </summary>
        /// <param name="value">the return value</param>
        public void setReturnValue(char value) { returnValue = value; }
    }
}
