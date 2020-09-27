/*
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
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using Zmpp.Core.IO;
    using Zmpp.Core.Vm;
    using Zmpp.Core.UI;
    using System;
    using System.IO;

    /// <summary>
    /// Tests the external i/o of the machine.
    /// </summary>
    [TestClass]
    public class MachineTest
    {
        private IMemory minizorkmap;
        private IStoryFileHeader fileheader;
        private Abbreviations abbreviations;
        private Machine machine;

        private Mock<IOutputStream> outputStream1, outputStream2, outputStream3;
        private Mock<IInputStream> inputStream1, inputStream0;
        private Mock<IStatusLine> statusLine;
        private Mock<IScreenModel> screen;
        private Mock<ISaveGameDataStore> datastore;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = new Mock<ILogger>();

            byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
            IMemory minizorkmap = new Memory(data);
            IObjectTree objectTree = new ClassicObjectTree(minizorkmap, minizorkmap.ReadUnsigned16(StoryFileHeaderAddress.ObjectTable));
            machine = new Machine(logger.Object);
            machine.Initialize(data, null);
            fileheader = machine.FileHeader;

            statusLine = new Mock<IStatusLine>();
            screen = new Mock<IScreenModel>();
            outputStream1 = new Mock<IOutputStream>();
            outputStream2 = new Mock<IOutputStream>();
            outputStream3 = new Mock<IOutputStream>();

            inputStream0 = new Mock<IInputStream>();
            inputStream1 = new Mock<IInputStream>();

            machine.SetScreen(screen.Object);

            machine.setOutputStream(OutputBase.OUTPUTSTREAM_SCREEN, outputStream1.Object);
            machine.setOutputStream(OutputBase.OUTPUTSTREAM_TRANSCRIPT, outputStream2.Object);
            machine.setOutputStream(OutputBase.OUTPUTSTREAM_MEMORY, outputStream3.Object);

            machine.setInputStream(InputStreamType.Keyboard, inputStream0.Object);
            machine.setInputStream(InputStreamType.File, inputStream1.Object);

            datastore = new Mock<ISaveGameDataStore>();
        }

        [TestMethod]
        public void testInitialState()
        {
            Assert.AreEqual(fileheader, machine.FileHeader);
            Assert.IsTrue(machine.HasValidChecksum);
        }

        [TestMethod]
        public void testSetOutputStream()
        {
            // arrange
            outputStream1.Setup(os => os.isSelected()).Returns(true);
            outputStream2.Setup(os => os.isSelected()).Returns(false);
            outputStream3.Setup(os => os.isSelected()).Returns(false);

            // act
            machine.SelectOutputStream(1, true);
            machine.Print("test");

            // assert
            outputStream1.Verify(os => os.select(true), Times.Once());
            outputStream2.Verify(os => os.select(false), Times.Once());
            outputStream1.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream2.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream3.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream1.Verify(os => os.print('t'), Times.Exactly(2));
            outputStream1.Verify(os => os.print('e'), Times.Once());
            outputStream1.Verify(os => os.print('s'), Times.Once());
        }

        [TestMethod]
        public void testSelectOutputStream()
        {
            // arrange

            // act
            machine.SelectOutputStream(1, true);

            // assert
            outputStream1.Verify(os => os.select(true), Times.Once());
        }

        [TestMethod]
        public void testInputStream1()
        {
            // arrange

            // act
            machine.setInputStream(InputStreamType.Keyboard, inputStream0.Object);
            machine.setInputStream(InputStreamType.File, inputStream1.Object);
            machine.SelectInputStream(InputStreamType.File);
            var result = machine.SelectedInputStream;

            // assert
            Assert.AreEqual(inputStream1.Object, result);
        }

        [TestMethod]
        public void testInputStream0()
        {
            // arrange

            // act
            machine.setInputStream(InputStreamType.Keyboard, inputStream0.Object);
            machine.setInputStream(InputStreamType.File, inputStream1.Object);
            machine.SelectInputStream(InputStreamType.Keyboard);
            var result = machine.SelectedInputStream;

            // assert
            Assert.AreEqual(inputStream0.Object, result);
        }

        [TestMethod]
        public void testRandom()
        {
            // arrange

            // act

            char random1 = machine.Random((short)23);
            var result = machine.Random((short)0);

            char random2 = machine.Random((short)23);
            var result2 = machine.Random((short)-23);

            char random3 = machine.Random((short)23);

            // assert

            Assert.IsTrue(0 < random1 && random1 <= 23);
            Assert.AreEqual(0, result);

            Assert.IsTrue(0 < random2 && random2 <= 23);
            Assert.AreEqual(0, result2);

            Assert.IsTrue(0 < random3 && random3 <= 23);
        }

        [TestMethod]
        public void testRandom1()
        {
            char value;
            for (int i = 0; i < 10; i++)
            {
                value = machine.Random((short)1);
                Assert.AreEqual(value, 1);
            }
        }

        [TestMethod]
        public void testRandom2()
        {
            char value;
            bool contains1 = false;
            bool contains2 = false;
            for (int i = 0; i < 10; i++)
            {

                value = machine.Random((short)2);
                Assert.IsTrue(0 < value && value <= 2);
                if (value == 1) contains1 = true;
                if (value == 2) contains2 = true;
            }
            Assert.IsTrue(contains1);
            Assert.IsTrue(contains2);
        }

        [TestMethod]
        public void testStartQuit()
        {
            // arrange
            outputStream1.Setup(os => os.isSelected()).Returns(true);
            outputStream2.Setup(os => os.isSelected()).Returns(false);
            outputStream3.Setup(os => os.isSelected()).Returns(false);

            // act
            machine.Start();
            var result = machine.RunState;
            machine.Quit();
            var result2 = machine.RunState;

            // assert
            outputStream2.Verify(os => os.select(false), Times.Once());
            outputStream1.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream2.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream3.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream1.Verify(os => os.print(It.IsAny<char>()), Times.AtLeastOnce());
            outputStream1.Verify(os => os.flush(), Times.AtLeastOnce());
            outputStream1.Verify(os => os.close(), Times.Once());
            outputStream2.Verify(os => os.flush(), Times.AtLeastOnce());
            outputStream2.Verify(os => os.close(), Times.Once());
            outputStream3.Verify(os => os.flush(), Times.AtLeastOnce());
            outputStream3.Verify(os => os.close(), Times.Once());
            inputStream0.Verify(os => os.Close(), Times.Once());
            inputStream1.Verify(os => os.Close(), Times.Once());

            Assert.AreEqual(MachineRunState.RUNNING, result);
            Assert.AreEqual(MachineRunState.STOPPED, result2);
        }

        [TestMethod]
        public void testStatusLineScore()
        {
            // arrange

            // act
            machine.SetVariable((char)0x10, (char)2);
            machine.SetStatusLine(statusLine.Object);
            machine.UpdateStatusLine();

            // assert
            statusLine.Verify(sl => sl.updateStatusScore(It.IsAny<String>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void testStatusLineTime()
        {
            // arrange

            // act
            machine.SetVariable((char)0x10, (char)2);
            machine.SetStatusLine(statusLine.Object); // set the "time" flag
            machine.WriteUnsigned8(1, (char)2);
            machine.UpdateStatusLine();

            // assert
            statusLine.Verify(sl => sl.updateStatusTime(It.IsAny<String>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void testGetSetScreen()
        {
            // arrange

            // act
            machine.SetScreen(screen.Object);
            var result = machine.Screen;

            // assert
            Assert.IsTrue(screen.Object == result);
        }

        [TestMethod]
        public void testHalt()
        {
            // arrange
            outputStream1.Setup(os => os.isSelected()).Returns(true);
            outputStream2.Setup(os => os.isSelected()).Returns(false);
            outputStream3.Setup(os => os.isSelected()).Returns(false);

            // act
            machine.Start();
            var result = machine.RunState;
            machine.Halt("error");
            var result2 = machine.RunState;

            // assert
            outputStream2.Verify(os => os.select(false), Times.Once());
            outputStream1.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream2.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream3.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream1.Verify(os => os.print(It.IsAny<char>()));

            Assert.AreEqual(MachineRunState.RUNNING, result);
            Assert.AreEqual(MachineRunState.STOPPED, result2);
        }

        [TestMethod]
        public void testRestart()
        {
            // arrange

            // act
            machine.Restart();

            // assert
            outputStream1.Verify(os => os.flush(), Times.Once());
            outputStream2.Verify(os => os.flush(), Times.Once());
            outputStream3.Verify(os => os.flush(), Times.Once());
            screen.Verify(s => s.Reset(), Times.Once());
        }

        [TestMethod]
        public void testSave()
        {
            // arrange
            datastore.Setup(ds => ds.WriteFormChunk(It.IsAny<WritableFormChunk>())).Returns(true);

            // act
            machine.SetSaveGameDataStore(datastore.Object);
            var result = machine.Save((char)4711);

            // assert
            datastore.Verify(ds => ds.WriteFormChunk(It.IsAny<WritableFormChunk>()), Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void testSelectTranscriptOutputStream()
        {
            // arrange

            // act
            machine.SelectOutputStream(OutputBase.OUTPUTSTREAM_TRANSCRIPT, true);
            var result = machine.FileHeader.IsEnabled(StoryFileHeaderAttribute.Transcripting);

            // assert
            outputStream2.Verify(os => os.select(true), Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void testSelectMemoryOutputStreamWithoutTable()
        {
            // arrange
            outputStream1.Setup(os => os.isSelected()).Returns(true);
            outputStream2.Setup(os => os.isSelected()).Returns(false);
            outputStream3.Setup(os => os.isSelected()).Returns(false);

            // act
            machine.SelectOutputStream(OutputBase.OUTPUTSTREAM_MEMORY, true);

            // assert
            outputStream2.Verify(os => os.select(false), Times.AtLeastOnce());
            outputStream3.Verify(os => os.select(true), Times.Once());
            outputStream1.Verify(os => os.isSelected(), Times.Once());
            outputStream2.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream3.Verify(os => os.isSelected(), Times.AtLeastOnce());
            outputStream1.Verify(os => os.print(It.IsAny<char>()));
        }

        //[TestMethod]
        //public void testSelectMemoryOutputStreamWithTable()
        //{
        //    int tableAddress;
        //    MemoryOutputStream memstream = new MemoryOutputStream(machine) {
        //    //    //  @Override
        //    //    //  public void select(int table, int tableWidth)
        //    //    //{
        //    //    //    tableAddress = table;
        //    //    //}
        //    };
        //    machine.setOutputStream(OutputBase.OUTPUTSTREAM_MEMORY, memstream);
        //    machine.selectOutputStream3(4711, 0);

        //    Assert.AreEqual(4711, tableAddress);
        //}
    }
}
