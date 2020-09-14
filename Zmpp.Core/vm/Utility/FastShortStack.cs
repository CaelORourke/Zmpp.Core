/*
 * Created on 2006/05/10
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

namespace Zmpp.Core.Vm.Utility
{
    /// <summary>
    /// This class implements a faster version of the Z-machin main stack.
    /// </summary>
    /// <remarks>
    /// This combines abstract access with the bypassing of unnecessary
    /// object creation.
    /// </remarks>
    public sealed class FastShortStack
    {
        private readonly char[] values;
        private char stackpointer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Vm.Utility.FastShortStack"/>class.
        /// </summary>
        /// <param name="size">The stack size.</param>
        public FastShortStack(int size)
        {
            values = new char[size];
            stackpointer = (char)0;
        }

        /// <summary>
        /// Gets the current stack pointer.
        /// </summary>
        public char StackPointer => stackpointer;

        /// <summary>
        /// Pushes a value on the stack and increases the stack pointer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Push(char value) { values[stackpointer++] = value; }

        /// <summary>
        /// Gets the top value of the stack without modifying the stack pointer.
        /// </summary>
        public char Top => values[stackpointer - 1];

        /// <summary>
        /// Replaces the top element with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ReplaceTopElement(char value)
        {
            values[stackpointer - 1] = value;
        }

        /// <summary>
        /// Gets the size of the stack.
        /// </summary>
        /// <remarks>
        /// Equal to stack pointer but has a different semantic meaning.
        /// </remarks>
        public int Size => stackpointer;

        /// <summary>
        /// Gets the top value of the stack and decreases the stack pointer.
        /// </summary>
        /// <returns>The top value.</returns>
        public char Pop()
        {
            return values[--stackpointer];
        }

        /// <summary>
        /// Gets the value at the specified index of the stack.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value at the index.</returns>
        /// <remarks>
        /// Here the stack is treated as an array.
        /// </remarks>
        public char GetValueAt(int index) { return values[index]; }
    }
}
