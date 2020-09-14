/*
 * Created on 10/14/2005
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
    using Zmpp.Core;
    using Zmpp.Core.Encoding;
    using Zmpp.Core.Vm;
    using System.IO;

    /// <summary>
    /// Testing tree access with concrete data of Version 3 - Minizork.
    /// </summary>
    [TestClass]
	public class ClassicObjectTreeTest
    {
		private const int OBJECT1 = 1;
		private const int OBJECT2 = 2;

		[TestMethod]
		public void testObjectSetters()
		{
			// arrange
			byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
			IMemory minizorkmap = new Memory(data);
			IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

			// act
			objectTree.SetParent(OBJECT1, 38);
			var result = objectTree.GetParent(OBJECT1);
			objectTree.SetChild(OBJECT1, 39);
			var result2 = objectTree.GetChild(OBJECT1);
			objectTree.SetSibling(OBJECT1, 42);
			var result3 = objectTree.GetSibling(OBJECT1);

			// assert
			Assert.AreEqual(38, result);
			Assert.AreEqual(39, result2);
			Assert.AreEqual(42, result3);
		}

        [TestMethod]
        public void testMinizorkAttributes()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.IsAttributeSet(OBJECT1, 5);
            var result2 = objectTree.IsAttributeSet(OBJECT1, 6);
            var result3 = objectTree.IsAttributeSet(OBJECT1, 7);

            var result4 = objectTree.IsAttributeSet(OBJECT2, 5);
            var result5 = objectTree.IsAttributeSet(OBJECT2, 7);
            var result6 = objectTree.IsAttributeSet(OBJECT2, 19);
            objectTree.ClearAttribute(OBJECT2, 19);
            var result7 = objectTree.IsAttributeSet(OBJECT2, 19);

            // assert
            Assert.IsFalse(result);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);

            Assert.IsTrue(result4);
            Assert.IsTrue(result5);
            Assert.IsTrue(result6);

            Assert.IsFalse(result7);
        }

        [TestMethod]
        public void testSetAttributes()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.IsAttributeSet(OBJECT1, 5);
            objectTree.SetAttribute(OBJECT1, 5);
            var result2 = objectTree.IsAttributeSet(OBJECT1, 5);

            // act
            Assert.IsFalse(result);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void testClearAttributes()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act

            // Set several bits in a row to make sure there will be no arithmetical shift errors
            objectTree.SetAttribute(OBJECT1, 0);
            objectTree.SetAttribute(OBJECT1, 1);
            objectTree.SetAttribute(OBJECT1, 2);
            objectTree.SetAttribute(OBJECT1, 3);

            var result = objectTree.IsAttributeSet(OBJECT1, 2);
            objectTree.ClearAttribute(OBJECT1, 2);

            var result2 = objectTree.IsAttributeSet(OBJECT1, 0);
            var result3 = objectTree.IsAttributeSet(OBJECT1, 1);
            var result4 = objectTree.IsAttributeSet(OBJECT1, 2);
            var result5 = objectTree.IsAttributeSet(OBJECT1, 3);

            // assert
            Assert.IsTrue(result);

            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsFalse(result4);
            Assert.IsTrue(result5);
        }

        [TestMethod]
        public void testGetPropertiesDescriptionAddress()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            Abbreviations abbreviations = new Abbreviations(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.Abbreviations));
            ZsciiEncoding encoding = new ZsciiEncoding(new AccentTable());
            IAlphabetTable alphabetTable = new AlphabetTable();
            IZCharTranslator translator = new ZCharTranslator(alphabetTable);
            IZCharDecoder decoder = new ZCharDecoder(encoding, translator, abbreviations);

            // act
            int propaddress = objectTree.GetPropertiesDescriptionAddress(OBJECT1);
            var result = decoder.Decode2Zscii(minizorkmap, propaddress, 0);

            // assert
            Assert.AreEqual("forest", result);
        }

        [TestMethod]
        public void testGetPropertyAddress()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetPropertyAddress(OBJECT1, 18);
            var result2 = objectTree.GetPropertyAddress(OBJECT1, 17);
            var result3 = objectTree.GetPropertyAddress(OBJECT1, 15);

            // assert
            Assert.AreEqual(2645, result);
            Assert.AreEqual(2648, result2);
            Assert.AreEqual(0, result3);
        }

        [TestMethod]
        public void testGetProperty()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetProperty(OBJECT2, 22);
            var result2 = objectTree.GetProperty(8, 16);
            var result3 = objectTree.GetProperty(OBJECT1, 20); // not defined, get default

            // assert
            Assert.AreEqual(0x77, result);
            Assert.AreEqual(0xc6c5, result2);
            Assert.AreEqual(0, result3);
        }

        [TestMethod]
        public void testSetGetProperty()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            objectTree.SetProperty(OBJECT2, 22, (char)0xc5);
            objectTree.SetProperty(8, 16, (char)0xcafe);
            var result = objectTree.GetProperty(OBJECT2, 22);
            var result2 = objectTree.GetProperty(8, 16);

            // assert
            Assert.AreEqual(0xc5, result);
            Assert.AreEqual(0xcafe, result2);
        }

        [TestMethod]
        public void testGetNextProperty()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetNextProperty(OBJECT1, 0);
            var result2 = objectTree.GetNextProperty(OBJECT1, 18);
            var result3 = objectTree.GetNextProperty(OBJECT1, 17);

            // assert
            Assert.AreEqual(18, result);
            Assert.AreEqual(17, result2);
            Assert.AreEqual(0, result3);
        }

        [TestMethod]
        public void testGetObject()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetParent(1);
            var result2 = objectTree.GetSibling(1);
            var result3 = objectTree.GetChild(1);

            // assert
            Assert.AreEqual(36, result);
            Assert.AreEqual(147, result2);
            Assert.AreEqual(0, result3);
        }

        [TestMethod]
        public void testRemoveObjectFirstChild()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act

            // remove a thief's lair - object 170
            int thiefslair = 170;
            var result = objectTree.GetParent(thiefslair);
            var result2 = objectTree.GetChild(thiefslair);
            var result3 = objectTree.GetSibling(thiefslair);

            objectTree.RemoveObject(thiefslair);

            // parent needs to be 0
            var result4 = objectTree.GetParent(thiefslair);

            // the old parent needs to point to the next child
            var result5 = objectTree.GetChild(27);

            // assert

            // remove a thief's lair - object 170
            Assert.AreEqual(27, result);
            Assert.AreEqual(175, result2);
            Assert.AreEqual(56, result3);

            // parent needs to be 0
            Assert.AreEqual(0, result4);

            // the old parent needs to point to the next child
            Assert.AreEqual(56, result5);
        }

        [TestMethod]
        public void testRemoveObjectNotFirstChild()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act

            // remove a cyclops room - object 56
            int cyclopsroom = 56;
            var result = objectTree.GetParent(cyclopsroom);
            var result2 = objectTree.GetChild(cyclopsroom);
            var result3 = objectTree.GetSibling(cyclopsroom);

            objectTree.RemoveObject(cyclopsroom);

            // parent needs to be 0
            var result4 = objectTree.GetParent(cyclopsroom);

            // the old parent does not need to change its child, but the
            // sibling chain needs to be corrected, so after 170 there will
            // follow 154 instead of 56
            var result5 = objectTree.GetChild(27);
            var result6 = objectTree.GetSibling(170);

            // assert

            // remove a cyclops room - object 56
            Assert.AreEqual(27, result);
            Assert.AreEqual(137, result2);
            Assert.AreEqual(154, result3);

            // parent needs to be 0
            Assert.AreEqual(0, result4);

            // the old parent does not need to change its child, but the
            // sibling chain needs to be corrected, so after 170 there will
            // follow 154 instead of 56
            Assert.AreEqual(170, result5);
            Assert.AreEqual(154, result6);
        }

        [TestMethod]
        public void testRemoveObjectNotFirstButLastChild()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act

            // remove a burnt out lantern - object 62
            int lantern = 62;
            var result = objectTree.GetParent(lantern);
            var result2 = objectTree.GetChild(lantern);
            var result3 = objectTree.GetSibling(lantern);

            objectTree.RemoveObject(lantern);

            // parent needs to be 0
            var result4 = objectTree.GetParent(lantern);

            // the old parent does not need to change its child, but object 66
            // will have 0 as its sibling
            var result5 = objectTree.GetChild(27);
            var result6 = objectTree.GetSibling(66);

            // assert

            // remove a burnt out lantern - object 62
            Assert.AreEqual(157, result);
            Assert.AreEqual(0, result2);
            Assert.AreEqual(0, result3);

            // parent needs to be 0
            Assert.AreEqual(0, result4);

            // the old parent does not need to change its child, but object 66
            // will have 0 as its sibling
            Assert.AreEqual(170, result5);
            Assert.AreEqual(0, result6);
        }

        [TestMethod]
        public void testRemoveObjectHasNoParent()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            int lantern = 62;
            objectTree.SetParent(lantern, 0);
            objectTree.RemoveObject(lantern);
            var result = objectTree.GetParent(lantern);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void testInsertObjectSimple()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act

            // Simplest and first case: Move a single object without any relationship
            // to a new parent, in this case object 30 ("you") to object 46
            // ("West of house")
            int you = 30;
            int westofhouse = 46;

            objectTree.InsertObject(westofhouse, you);

            // object becomes direct child of the parent
            var result = objectTree.GetParent(you);
            var result2 = objectTree.GetChild(westofhouse);

            // and the former direct child becomes the first sibling
            var result3 = objectTree.GetSibling(you);

            // assert

            // object becomes direct child of the parent
            Assert.AreEqual(westofhouse, result);
            Assert.AreEqual(you, result2);

            // and the former direct child becomes the first sibling
            Assert.AreEqual(82, result3);
        }

        [TestMethod]
        public void testInsertObjectHasSiblingsAndChild()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act

            // In this case, the object to insert has siblings and we do not
            // want to move them with it, furthermore it has a child, and we
            // want to move it
            // move obj 158 ("studio") to obj 46 ("west of house")
            int studio = 158;
            int westofhouse = 46;
            objectTree.InsertObject(westofhouse, studio);
            var result = objectTree.GetParent(studio);
            var result2 = objectTree.GetChild(westofhouse);
            var result3 = objectTree.GetChild(studio);
            var result4 = objectTree.GetSibling(studio);

            // The old siblings line up correctly, i.e. 87 -> 22 instead of 158
            var result5 = objectTree.GetSibling(87);

            // assert

            Assert.AreEqual(westofhouse, result);
            Assert.AreEqual(studio, result2);
            Assert.AreEqual(61, result3);
            Assert.AreEqual(82, result4);

            // The old siblings line up correctly, i.e. 87 -> 22 instead of 158
            Assert.AreEqual(22, result5);
        }

        [TestMethod]
        public void testGetPropertyLength()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetPropertyLength(0x1889);

            // assert
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void testGetPropertyLengthAddress0()
        {
            // arrange
            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));

            // act
            var result = objectTree.GetPropertyLength(0);

            // assert
            Assert.AreEqual(0, result);
        }
    }
}
