/*
 * Created on 2006/03/10
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
    /// Tests for the RingBuffer class.
    /// </summary>
    [TestClass]
    public class RingBufferTest
    {

        [TestMethod]
        public void testInitial()
        {
            // arrange
            RingBuffer<int> ringbuffer = new RingBuffer<int>(3);

            // act
            var result = ringbuffer.size();

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void testAddElementNormal()
        {
            // arrange
            RingBuffer<int> ringbuffer = new RingBuffer<int>(3);

            // act

            ringbuffer.add(1);
            var result = ringbuffer.size();
            var result2 = ringbuffer.get(0);

            ringbuffer.add(2);
            var result3 = ringbuffer.size();
            var result4 = ringbuffer.get(1);

            ringbuffer.add(3);
            var result5 = ringbuffer.size();
            var result6 = ringbuffer.get(2);

            ringbuffer.set(1, 5);
            var result7 = ringbuffer.size();
            var result8 = ringbuffer.get(1);

            // assert

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, result2);

            Assert.AreEqual(2, result3);
            Assert.AreEqual(2, result4);

            Assert.AreEqual(3, result5);
            Assert.AreEqual(3, result6);

            Assert.AreEqual(3, result7);
            Assert.AreEqual(5, result8);
        }

        [TestMethod]
        public void testAddElementOverflow()
        {
            // arrange
            RingBuffer<int> ringbuffer = new RingBuffer<int>(3);

            // act

            // fill it up to the limit
            ringbuffer.add(1);
            ringbuffer.add(2);
            ringbuffer.add(3);

            // now add one more, the 1 should be gone
            ringbuffer.add(4);
            var result = ringbuffer.size();
            var result2 = ringbuffer.get(2);

            ringbuffer.set(0, 7);
            var result3 = ringbuffer.get(0);

            // assert

            // now add one more, the 1 should be gone
            Assert.AreEqual(3, result);
            Assert.AreEqual(4, result2);

            Assert.AreEqual(7, result3);
        }

        [TestMethod]
        public void testRemoveNormal()
        {
            // arrange
            RingBuffer<int> ringbuffer = new RingBuffer<int>(3);

            // act

            ringbuffer.add(1);
            ringbuffer.add(2);
            int elem = ringbuffer.remove(1);
            var result = ringbuffer.size();
            var result2 = elem;

            ringbuffer.add(3);
            var result3 = ringbuffer.size();
            var result4 = ringbuffer.get(1);

            // assert

            Assert.AreEqual(1, result);
            Assert.AreEqual(2, elem);

            Assert.AreEqual(2, result3);
            Assert.AreEqual(3, result4);
        }

        [TestMethod]
        public void testRemoveOverflow()
        {
            // arrange
            RingBuffer<int> ringbuffer = new RingBuffer<int>(3);

            // act

            // fill it over the limit
            ringbuffer.add(1);
            ringbuffer.add(2);
            ringbuffer.add(3);
            ringbuffer.add(4);

            // contains 2, 3, 4 now
            ringbuffer.remove(1);

            // contains 2, 4 now
            var result = ringbuffer.size();
            var result2 = ringbuffer.get(0);
            var result3 = ringbuffer.get(1);

            // assert

            // contains 2, 4 now
            Assert.AreEqual(2, result);
            Assert.AreEqual(2, result2);
            Assert.AreEqual(4, result3);
        }

        /// <summary>
        /// A more sophisticated test that checks whether internal bounds are
        /// correctly adjusted.
        /// </summary>
        [TestMethod]
        public void testRemoveTooManyAndReadd()
        {
            // arrange
            RingBuffer<int> ringbuffer = new RingBuffer<int>(3);

            // act

            // overflow the ring buffer
            ringbuffer.add(1);
            ringbuffer.add(2);
            ringbuffer.add(3);
            ringbuffer.add(4);

            // underflow the ring buffer
            ringbuffer.remove(0);
            ringbuffer.remove(0);
            ringbuffer.remove(0);
            ringbuffer.remove(0);

            // size should be 0
            var result = ringbuffer.size();

            // adding should work
            ringbuffer.add(5);
            ringbuffer.add(6);
            var result2 = ringbuffer.size();
            var result3 = ringbuffer.get(0);
            var result4 = ringbuffer.get(1);

            // assert

            // size should be 0
            Assert.AreEqual(0, result);

            // adding should work
            Assert.AreEqual(2, result2);
            Assert.AreEqual(5, result3);
            Assert.AreEqual(6, result4);
        }

        [TestMethod]
        public void testToString()
        {
            // arrange
            RingBuffer<int> ringbuffer = new RingBuffer<int>(3);

            // act

            ringbuffer.add(1);
            ringbuffer.add(2);
            var result = ringbuffer.toString();

            // assert
            Assert.AreEqual("{ 1, 2 }", result);
        }
    }
}
