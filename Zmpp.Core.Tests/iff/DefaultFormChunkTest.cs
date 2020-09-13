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
                new FormChunk(new Memory(illegalData));
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
            IMemory mem = new Memory(data);
            IFormChunk formChunk = new FormChunk(mem);

            // act

            // assert
            Assert.IsTrue(formChunk.IsValid);
            Assert.AreEqual("FORM", formChunk.Id);
            Assert.AreEqual(512, formChunk.Size);
            Assert.AreEqual("IFZS", formChunk.SubId);
        }

        [TestMethod]
        public void testSubchunks()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory mem = new Memory(data);
            IFormChunk formChunk = new FormChunk(mem);

            // act
            List<IChunk> result = new List<IChunk>();
            using (IEnumerator<IChunk> iter = formChunk.SubChunks)
            {
                while (iter.MoveNext())
                {
                    IChunk chunk = iter.Current;
                    Assert.IsTrue(chunk.IsValid);
                    result.Add(chunk);
                }
            }

            // assert
            Assert.AreEqual("IFhd", result[(0)].Id);
            Assert.AreEqual(13, result[(0)].Size);
            Assert.AreEqual(0x000c, result[(0)].Address);
            Assert.AreEqual("CMem", result[(1)].Id);
            Assert.AreEqual(351, result[(1)].Size);
            Assert.AreEqual(0x0022, result[(1)].Address);
            Assert.AreEqual("Stks", result[(2)].Id);
            Assert.AreEqual(118, result[(2)].Size);
            Assert.AreEqual(0x018a, result[(2)].Address);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void testGetSubChunk()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory mem = new Memory(data);
            IFormChunk formChunk = new FormChunk(mem);

            // act

            // assert
            Assert.IsNotNull(formChunk.GetSubChunk("IFhd"));
            Assert.IsNotNull(formChunk.GetSubChunk("CMem"));
            Assert.IsNotNull(formChunk.GetSubChunk("Stks"));
            Assert.IsNull(formChunk.GetSubChunk("Test"));
        }

        [TestMethod]
        public void testGetSubChunkByAddress()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory mem = new Memory(data);
            IFormChunk formChunk = new FormChunk(mem);

            // act

            // assert
            Assert.AreEqual("IFhd", formChunk.GetSubChunk(0x000c).Id);
            Assert.AreEqual("CMem", formChunk.GetSubChunk(0x0022).Id);
            Assert.AreEqual("Stks", formChunk.GetSubChunk(0x018a).Id);
            Assert.IsNull(formChunk.GetSubChunk(0x1234));
        }
    }
}
