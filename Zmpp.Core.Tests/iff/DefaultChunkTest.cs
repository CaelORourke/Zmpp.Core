/*
 * Created on 2008/07/20
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

    /// <summary>
    /// Tests for the DefaultChunk class.
    /// </summary>
    [TestClass]
    public class DefaultChunkTest
    {
        public DefaultChunkTest()
        {
        }

        [TestMethod]
        public void testCreateChunkForWriting()
        {
            // arrange
            byte[] id = { (byte)'F', (byte)'O', (byte)'R', (byte)'M' };
            byte[] chunkdata = { (byte)0x01, (byte)0x02, (byte)0x03 };
            IChunk chunk = new Chunk(id, chunkdata);

            // act
            IMemory mem = chunk.Memory;
            var result = MemoryUtil.ReadUnsigned32(mem, 4);

            // assert
            Assert.AreEqual(3, chunk.Size);
            Assert.AreEqual("FORM", chunk.Id);
            Assert.AreEqual(0, chunk.Address);
            Assert.AreEqual('F', mem.ReadUnsigned8(0));
            Assert.AreEqual('O', mem.ReadUnsigned8(1));
            Assert.AreEqual('R', mem.ReadUnsigned8(2));
            Assert.AreEqual('M', mem.ReadUnsigned8(3));
            Assert.AreEqual(3, result);
            Assert.AreEqual(0x01, mem.ReadUnsigned8(8));
            Assert.AreEqual(0x02, mem.ReadUnsigned8(9));
            Assert.AreEqual(0x03, mem.ReadUnsigned8(10));
        }

        [TestMethod]
        public void testCreateChunkForReading()
        {
            // arrange
            byte[] data = {
                (byte) 'F', (byte) 'O', (byte) 'R', (byte) 'M',
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x03,
                (byte) 0x01, (byte) 0x02, (byte) 0x03
            };
            IMemory mem = new Memory(data);
            IChunk chunk = new Chunk(mem, 1234);

            // act

            // assert
            Assert.AreEqual(1234, chunk.Address);
            Assert.AreEqual("FORM", chunk.Id);
            Assert.AreSame(mem, chunk.Memory);
            Assert.AreEqual(3, chunk.Size);
        }
    }
}
