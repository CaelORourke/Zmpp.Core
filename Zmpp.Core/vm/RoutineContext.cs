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
    /// Represents the context of a routine.
    /// </summary>
    public class RoutineContext
    {
        /// <summary>
        /// Set as return variable value if the call is a call_nx.
        /// </summary>
        public const char DiscardResult = (char)0xffff;

        /// <summary>
        /// The local variables
        /// </summary>
        private readonly char[] locals;

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
        /// Initializes a new instance of the
        /// <see cref="Zmpp.Core.Vm.RoutineContext"/> class
        /// for the specified number of local variables.
        /// </summary>
        /// <param name="numLocalVariables">The number of local variables.</param>
        public RoutineContext(int numLocalVariables)
        {
            locals = new char[numLocalVariables];
        }

        /// <summary>
        /// Gets or sets the number of arguments.
        /// </summary>
        public int NumArguments
        {
            get => numArgs;
            set => this.numArgs = value;
        }

        /// <summary>
        /// Gets the number of local variables.
        /// </summary>
        public int NumLocalVariables => (locals == null) ? 0 : locals.Length;

        /// <summary>
        /// Sets a value for the specified local variable.
        /// </summary>
        /// <param name="localNum">The local variable number starting with 0.</param>
        /// <param name="value">The value.</param>
        public void SetLocalVariable(char localNum, char value)
        {
            locals[localNum] = value;
        }

        /// <summary>
        /// Gets the value of the specified local variable.
        /// </summary>
        /// <param name="localNum">The local variable number starting at 0.</param>
        /// <returns>The value of the specified variable.</returns>
        public char GetLocalVariable(char localNum)
        {
            return locals[localNum];
        }

        /// <summary>
        /// Gets or sets the return address of the routine.
        /// </summary>
        public int ReturnAddress { get => returnAddress; set => this.returnAddress = value; }

        /// <summary>
        /// Gets or sets the return variable number of the routine.
        /// </summary>
        /// <remarks>
        /// Returns the return variable number or DiscardResult.
        /// </remarks>
        public char ReturnVariable { get => returnVarNum; set => returnVarNum = value; }

        /// <summary>
        /// Gets or sets the stack pointer at invocation time.
        /// </summary>
        public char InvocationStackPointer { get => invocationStackPointer; set => invocationStackPointer = value; }

        /// <summary>
        /// Gets or sets the return value.
        /// </summary>
        public char ReturnValue { get => returnValue; set => returnValue = value; }
    }
}
