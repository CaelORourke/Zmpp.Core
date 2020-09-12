/*
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

namespace Zmpp.Core.Iff.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Tests for the DefaultFormChunk class.
    /// </summary>
    [TestClass]
    public class DefaultFormChunkTest
    {
        public DefaultFormChunkTest()
        {
        }

        [TestMethod]
        public void testInvalidIff()
        {
            byte[] illegalData = {
                (byte) 0x01, (byte) 0x02, (byte) 0x03, (byte) 0x04, (byte) 0x05,
                (byte) 0x01, (byte) 0x02, (byte) 0x03, (byte) 0x04, (byte) 0x05,
                (byte) 0x01, (byte) 0x02, (byte) 0x03, (byte) 0x04, (byte) 0x05,
                (byte) 0x01, (byte) 0x02, (byte) 0x03, (byte) 0x04, (byte) 0x05,
            };
            try
            {
                new DefaultFormChunk(new DefaultMemory(illegalData));
                Assert.Fail("IOException should be thrown on an illegal IFF file");
            }
            catch (IOException expected)
            {
                Assert.IsTrue(expected.Message != null);
            }
        }

        [TestMethod]
        public void testCreation()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory mem = new DefaultMemory(data);
            IFormChunk formChunk = new DefaultFormChunk(mem);

            // act

            // assert
            Assert.IsTrue(formChunk.isValid());
            Assert.AreEqual("FORM", formChunk.getId());
            Assert.AreEqual(512, formChunk.getSize());
            Assert.AreEqual("IFZS", formChunk.getSubId());
        }

        [TestMethod]
        public void testSubchunks()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory mem = new DefaultMemory(data);
            IFormChunk formChunk = new DefaultFormChunk(mem);

            // act
            List<IChunk> result = new List<IChunk>();
            using (IEnumerator<IChunk> iter = formChunk.getSubChunks())
            {
                while (iter.MoveNext())
                {
                    IChunk chunk = iter.Current;
                    Assert.IsTrue(chunk.isValid());
                    result.Add(chunk);
                }
            }

            // assert
            Assert.AreEqual("IFhd", result[(0)].getId());
            Assert.AreEqual(13, result[(0)].getSize());
            Assert.AreEqual(0x000c, result[(0)].getAddress());
            Assert.AreEqual("CMem", result[(1)].getId());
            Assert.AreEqual(351, result[(1)].getSize());
            Assert.AreEqual(0x0022, result[(1)].getAddress());
            Assert.AreEqual("Stks", result[(2)].getId());
            Assert.AreEqual(118, result[(2)].getSize());
            Assert.AreEqual(0x018a, result[(2)].getAddress());
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void testGetSubChunk()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory mem = new DefaultMemory(data);
            IFormChunk formChunk = new DefaultFormChunk(mem);

            // act

            // assert
            Assert.IsNotNull(formChunk.getSubChunk("IFhd"));
            Assert.IsNotNull(formChunk.getSubChunk("CMem"));
            Assert.IsNotNull(formChunk.getSubChunk("Stks"));
            Assert.IsNull(formChunk.getSubChunk("Test"));
        }

        [TestMethod]
        public void testGetSubChunkByAddress()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory mem = new DefaultMemory(data);
            IFormChunk formChunk = new DefaultFormChunk(mem);

            // act

            // assert
            Assert.AreEqual("IFhd", formChunk.getSubChunk(0x000c).getId());
            Assert.AreEqual("CMem", formChunk.getSubChunk(0x0022).getId());
            Assert.AreEqual("Stks", formChunk.getSubChunk(0x018a).getId());
            Assert.IsNull(formChunk.getSubChunk(0x1234));
        }
    }
}
