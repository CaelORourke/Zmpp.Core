/*
 * Created on 10/04/2005
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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.IO;
    using Zmpp.Core;
    using Zmpp.Core.Vm;

    /// <summary>
    /// Testing the tree for Curses.
    /// </summary>
    [TestClass]
	public class CursesObjectTreeTest
    {
		private const int ADDR_7_20 = 7734;
		private const int ADDR_7_1 = 7741;

		[TestMethod]
		public void GetPropertiesDescriptionAddress()
		{
			// arrange
			var logger = new Mock<ILogger>();
			byte[] data = File.ReadAllBytes("testfiles/curses.z5");
			IMemory curses = new Memory(data);
			IMachine machine = new Machine(logger.Object);
			machine.Initialize(data, null);
			IObjectTree objectTree = new ModernObjectTree(curses, machine.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

			// act
			var result = objectTree.GetPropertiesDescriptionAddress(123);

			// assert
			Assert.AreEqual(0x2d40, result);
		}

        [TestMethod]
        public void GetPropertyAddress()
        {
            // arrange
            var logger = new Mock<ILogger>();
            byte[] data = File.ReadAllBytes("testfiles/curses.z5");
            IMemory curses = new Memory(data);
            IMachine machine = new Machine(logger.Object);
            machine.Initialize(data, null);
            IObjectTree objectTree = new ModernObjectTree(curses, machine.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetPropertyAddress(7, 20);
            var result2 = objectTree.GetPropertyAddress(7, 1);

            // assert
            Assert.AreEqual(ADDR_7_20, result);
            Assert.AreEqual(ADDR_7_1, result2);
        }

        [TestMethod]
        public void GetProperty()
        {
            // arrange
            var logger = new Mock<ILogger>();
            byte[] data = File.ReadAllBytes("testfiles/curses.z5");
            IMemory curses = new Memory(data);
            IMachine machine = new Machine(logger.Object);
            machine.Initialize(data, null);
            IObjectTree objectTree = new ModernObjectTree(curses, machine.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetProperty(3, 22);
            var result2 = objectTree.GetProperty(3, 8);
            var result3 = objectTree.GetProperty(2, 20);

            // assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(0x0006, result2);
            Assert.AreEqual(0xb685, result3);
        }

        [TestMethod]
        public void SetGetProperty()
        {
            // arrange
            var logger = new Mock<ILogger>();
            byte[] data = File.ReadAllBytes("testfiles/curses.z5");
            IMemory curses = new Memory(data);
            IMachine machine = new Machine(logger.Object);
            machine.Initialize(data, null);
            IObjectTree objectTree = new ModernObjectTree(curses, machine.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            objectTree.SetProperty(122, 34, (char)0xdefe);
            var result = objectTree.GetProperty(122, 34);

            // assert
            Assert.AreEqual(0xdefe, result);
        }

        [TestMethod]
        public void GetNextProperty()
        {
            // arrange
            var logger = new Mock<ILogger>();
            byte[] data = File.ReadAllBytes("testfiles/curses.z5");
            IMemory curses = new Memory(data);
            IMachine machine = new Machine(logger.Object);
            machine.Initialize(data, null);
            IObjectTree objectTree = new ModernObjectTree(curses, machine.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetNextProperty(7, 0);
            var result2 = objectTree.GetNextProperty(7, 24);
            var result3 = objectTree.GetNextProperty(7, 20);
            var result4 = objectTree.GetNextProperty(7, 8);
            var result5 = objectTree.GetNextProperty(7, 1);

            // assert
            Assert.AreEqual(24, result);
            Assert.AreEqual(20, result2);
            Assert.AreEqual(8, result3);
            Assert.AreEqual(1, result4);
            Assert.AreEqual(0, result5);
        }

        [TestMethod]
        public void GetPropertyLength()
        {
            // arrange
            var logger = new Mock<ILogger>();
            byte[] data = File.ReadAllBytes("testfiles/curses.z5");
            IMemory curses = new Memory(data);
            IMachine machine = new Machine(logger.Object);
            machine.Initialize(data, null);
            IObjectTree objectTree = new ModernObjectTree(curses, machine.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetPropertyLength(ADDR_7_20);
            var result2 = objectTree.GetPropertyLength(ADDR_7_1);

            // assert
            Assert.AreEqual(2, result);
            Assert.AreEqual(6, result2);
        }

        [TestMethod]
        public void GetPropertyLengthAddress0()
        {
            // arrange
            var logger = new Mock<ILogger>();
            byte[] data = File.ReadAllBytes("testfiles/curses.z5");
            IMemory curses = new Memory(data);
            IMachine machine = new Machine(logger.Object);
            machine.Initialize(data, null);
            IObjectTree objectTree = new ModernObjectTree(curses, machine.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetPropertyLength(0);

            // assert
            Assert.AreEqual(0, result);
        }
    }
}
