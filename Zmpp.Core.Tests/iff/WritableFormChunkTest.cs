/*
 * Created on 2005/12/06
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

namespace test.zmpp.iff
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using org.zmpp.iff;
    using System.Text;

    /// <summary>
    /// Tests for the WritableFormChunk class.
    /// </summary>
    [TestClass]
    public class WritableFormChunkTest
    {
        public WritableFormChunkTest()
        {
        }

        [TestMethod]
        public void testIsValid()
        {
            // arrange
            string id = "IFhd";
            byte[] subId = new byte[Encoding.UTF8.GetByteCount(id)];
            Encoding.UTF8.GetBytes(id, 0, id.Length, (byte[])(object)subId, 0);
            WritableFormChunk formChunk = new WritableFormChunk(subId);

            // act

            // assert
            Assert.IsTrue(formChunk.isValid());
            Assert.IsNotNull(formChunk.getMemory());
            Assert.IsNotNull(formChunk.getSubChunks());
            Assert.IsNull(formChunk.getSubChunk(1234));
            Assert.AreEqual(0, formChunk.getAddress());
        }
    }
}
