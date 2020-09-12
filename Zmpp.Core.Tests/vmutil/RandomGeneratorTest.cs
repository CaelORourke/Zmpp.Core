/*
 * Created on 2005/09/23
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

namespace test.zmpp.vmutil
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using org.zmpp.vmutil;

    /// <summary>
    /// Tests for the RandomGenerator classes.
    /// </summary>
    [TestClass]
    public class RandomGeneratorTest
    {
        [TestMethod]
        public void testUnpredictableRandomSequence()
        {
            // arrange
            IRandomGenerator random1 = new UnpredictableRandomGenerator();

            // act
            int rnd1 = random1.next();
            int rnd2 = random1.next();

            // assert
            Assert.AreNotEqual(rnd1, rnd2);
            Assert.IsTrue(1 <= rnd1 && rnd1 <= RandomGenerator.MAX_VALUE);
        }

        [TestMethod]
        public void testUnpredictableRandomDifferentSequences()
        {
            // arrange
            IRandomGenerator random1 = new UnpredictableRandomGenerator();
            IRandomGenerator random2 = new UnpredictableRandomGenerator();

            // act
            int rnd11 = random1.next();
            int rnd12 = random1.next();
            int rnd21 = random2.next();
            int rnd22 = random2.next();

            // assert
            Assert.AreNotEqual(rnd11, rnd12);
            Assert.AreNotEqual(rnd21, rnd22);
        }

        [TestMethod]
        public void testPredictableRandomSequence()
        {
            // arrange
            int seed = 4711;
            IRandomGenerator predictable1 = new PredictableRandomGenerator(seed);

            // act
            int rnd1 = predictable1.next();
            int rnd2 = predictable1.next();

            // assert
            Assert.AreNotEqual(rnd1, rnd2);
            Assert.IsTrue(1 <= rnd1 && rnd1 <= RandomGenerator.MAX_VALUE);
        }

        [TestMethod]
        public void testPredictableSameSequences()
        {
            // arrange
            int seed = 4711;
            IRandomGenerator predictable1 = new PredictableRandomGenerator(seed);
            IRandomGenerator predictable2 = new PredictableRandomGenerator(seed);

            // act
            int rnd11 = predictable1.next();
            int rnd12 = predictable1.next();
            int rnd21 = predictable2.next();
            int rnd22 = predictable2.next();

            // assert
            Assert.AreEqual(rnd11, rnd21);
            Assert.AreEqual(rnd12, rnd22);
        }
    }
}
