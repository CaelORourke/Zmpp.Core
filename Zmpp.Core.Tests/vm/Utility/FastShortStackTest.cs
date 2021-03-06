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

namespace Zmpp.Core.Vm.Utility.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Zmpp.Core.Vm.Utility;

    /// <summary>
    /// Tests for the FastShortStack class.
    /// </summary>
    [TestClass]
    public class FastShortStackTest
    {
        [TestMethod]
        public void testInitial()
        {
            // arrange
            FastShortStack stack = new FastShortStack(123);

            // act
            var result = stack.StackPointer;
            var result2 = stack.Size;

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(0, result2);
        }

        [TestMethod]
        public void testSize()
        {
            // arrange
            FastShortStack stack = new FastShortStack(123);

            // act
            stack.Push((char)1);
            var result = stack.Size;
            stack.Push((char)3);
            var result2 = stack.Size;
            stack.Pop();
            var result3 = stack.Size;

            // assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(1, result3);
        }

        [TestMethod]
        public void testPushTop()
        {
            // arrange
            FastShortStack stack = new FastShortStack(123);

            // act
            stack.Push((char)3);
            var result = stack.StackPointer;
            var result2 = stack.Top;
            var result3 = stack.StackPointer;

            // assert
            Assert.AreEqual(1, result, "stack pointer should have been increased");
            Assert.AreEqual(3, result2, "the value 3 should be on top of the stack");
            Assert.AreEqual(1, result3, "stack pointer should not have been modified");
        }

        [TestMethod]
        public void testPushPop()
        {
            // arrange
            FastShortStack stack = new FastShortStack(123);

            // act
            stack.Push((char)3);
            var result = stack.Pop();
            var result2 = stack.StackPointer;

            // assert
            Assert.AreEqual(3, result, "the value 3 should be on top of the stack");
            Assert.AreEqual(0, result2, "stack pointer should have been decreased");
        }

        [TestMethod]
        public void testGetValueAt()
        {
            // arrange
            FastShortStack stack = new FastShortStack(123);

            // act
            stack.Push((char)3);
            stack.Push((char)5);
            stack.Push((char)7);
            var result = stack.GetValueAt(0);
            var result2 = stack.GetValueAt(1);
            var result3 = stack.GetValueAt(2);
            var result4 = stack.StackPointer;

            // assert
            Assert.AreEqual(3, result);
            Assert.AreEqual(5, result2);
            Assert.AreEqual(7, result3);
            Assert.AreEqual(3, result4, "stack pointer should not have been modified");
        }

        [TestMethod]
        public void testReplaceTopElement()
        {
            // arrange
            FastShortStack stack = new FastShortStack(123);

            // act
            stack.Push((char)3);
            stack.Push((char)5);
            stack.Push((char)7);
            stack.ReplaceTopElement((char)11);
            var result = stack.Top;
            var result2 = stack.Size;

            // assert
            Assert.AreEqual(11, result, "top element should be 11 now");
            Assert.AreEqual(3, result2, "number of elements should be 3");
        }
    }
}
