﻿/*
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

namespace Zmpp.Core.Vm.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Zmpp.Core.Vm;
    using System;

    /// <summary>
    /// Test for the RoutineContext class.
    /// </summary>
    [TestClass]
    public class RoutineContextTest
    {
        [TestMethod]
        public void testCreate()
        {
            // arrange
            RoutineContext context = new RoutineContext(2);

            // act
            var result = context.getNumLocalVariables();

            // assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void testSetters()
        {
            // arrange
            RoutineContext context = new RoutineContext(2);

            // act
            context.setLocalVariable((char)0, (char)72);
            var result = context.getLocalVariable((char)0);
            context.setLocalVariable((char)1, (char)76);
            var result2 = context.getLocalVariable((char)1);
            try
            {
                context.setLocalVariable((char)2, (char)815);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException expected)
            {
                // this is good
            }
            context.setReturnAddress((char)0x4711);
            var result3 = context.getReturnAddress();
            context.setReturnVariable((char)0x13);
            var result4 = context.getReturnVariable();
            context.setInvocationStackPointer((char)1234);
            var result5 = context.getInvocationStackPointer();
            context.setNumArguments(3);
            var result6 = context.getNumArguments();

            // assert
            Assert.AreEqual(72, result);
            Assert.AreEqual(76, result2);
            Assert.AreEqual(0x4711, result3);
            Assert.AreEqual(0x13, result4);
            Assert.AreEqual(1234, result5);
            Assert.AreEqual(3, result6);
        }
    }
}
