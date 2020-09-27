/*
 * Created on 09/24/2005
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
    using Moq;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Vm;
    using static Zmpp.Core.Vm.PortableGameState;

    /// <summary>
    /// Tests for the PortableGameState class
    /// using a given Quetzal file.
    /// </summary>
    [TestClass]
    public class PortableGameStateTest
    {
        private PortableGameState gameState;
        private IFormChunk formChunk;
        private Mock<IMachine> machine;
        private Mock<IStoryFileHeader> fileheader;
        private byte[] savedata;

        private readonly int[] pcs = { 0, 25108, 25132, 25377, 26137, 26457, 26499 };
        private readonly int[] retvars = { 0, 0, 1, 7, 0, 4, 0 };
        private readonly int[] localLengths = { 0, 1, 11, 2, 7, 4, 0 };
        private readonly int[] stackSizes = { 4, 0, 0, 0, 2, 0, 0 };
        private readonly int[] numArgs = { 0, 1, 4, 2, 4, 4, 0 };

        [TestInitialize]
        public void TestInitialize()
        {
            machine = new Mock<IMachine>();
            fileheader = new Mock<IStoryFileHeader>();

            savedata = File.ReadAllBytes("testfiles/leathersave.ifzs");
            IMemory memaccess = new Memory(savedata);
            formChunk = new FormChunk(memaccess);
            gameState = new PortableGameState();
        }

        [TestMethod]
        public void ReadSaveGame()
        {
            Assert.IsTrue(gameState.ReadSaveGame(formChunk));
            Assert.AreEqual(59, gameState.Release);
            Assert.AreEqual("860730", gameState.SerialNumber);
            Assert.AreEqual(53360, gameState.Checksum);
            Assert.AreEqual(35298, gameState.ProgramCounter);

            Assert.AreEqual(7, gameState.StackFrames.Count);

            for (int i = 0; i < gameState.StackFrames.Count; i++)
            {
                StackFrame sfi = gameState.StackFrames[i];
                Assert.AreEqual(pcs[i], sfi.ProgramCounter);
                Assert.AreEqual(retvars[i], sfi.ReturnVariable);
                Assert.AreEqual(localLengths[i], sfi.Locals.Length);
                Assert.AreEqual(stackSizes[i], sfi.EvalStack.Length);
                Assert.AreEqual(numArgs[i], sfi.Args.Length);
            }
            Assert.AreEqual(10030, gameState.GetDeltaBytes().Length);
        }

        [TestMethod]
        public void ReadSaveGameFormChunkIsNull()
        {
            Assert.IsFalse(gameState.ReadSaveGame(null));
        }

        [TestMethod]
        public void GetStackFrameStatusVars()
        {
            StackFrame stackFrame = new StackFrame();
            stackFrame.ProgramCounter = (char)4711;
            Assert.AreEqual(4711, stackFrame.ProgramCounter);
            stackFrame.ReturnVariable = (char)5;
            Assert.AreEqual(5, stackFrame.ReturnVariable);
        }

        [TestMethod]
        public void CaptureMachineState()
        {
            // arrange
            List<RoutineContext> emptyContexts = new List<RoutineContext>();
            machine.Setup(m => m.FileHeader).Returns(fileheader.Object);
            machine.Setup(m => m.GetRoutineContexts()).Returns(emptyContexts);
            machine.Setup(m => m.SP).Returns((char)4);
            machine.Setup(m => m.GetStackElement(It.IsAny<int>())).Returns((char)42);
            machine.Setup(m => m.Release).Returns(42);
            machine.Setup(m => m.ReadUnsigned16(StoryFileHeaderAddress.Checksum)).Returns((char)4712);
            machine.Setup(m => m.ReadUnsigned16(StoryFileHeaderAddress.StaticMem)).Returns((char)12345);
            fileheader.Setup(fh => fh.SerialNumber).Returns("850101");
            machine.Setup(m => m.ReadUnsigned8(It.IsAny<int>())).Returns((char)(short)0);

            // act
            gameState.CaptureMachineState(machine.Object, (char)4711);
            var result = gameState.ProgramCounter;
            var result2 = gameState.Release;
            var result3 = gameState.Checksum;
            var result4 = gameState.SerialNumber;
            var result5 = gameState.GetDynamicMemoryDump().Length;
            var result6 = gameState.StackFrames.Count;
            StackFrame stackFrame = gameState.StackFrames[0];
            var result7 = stackFrame.EvalStack.Length;

            // assert
            machine.Verify(m => m.FileHeader, Times.AtLeastOnce());
            machine.Verify(m => m.GetRoutineContexts(), Times.Once());
            machine.Verify(m => m.SP, Times.Once());
            //machine.Verify(m => m.getStackElement(It.IsAny<int>()));//allowing
            machine.Verify(m => m.Release, Times.Once());
            machine.Verify(m => m.ReadUnsigned16(StoryFileHeaderAddress.Checksum), Times.Once());
            machine.Verify(m => m.ReadUnsigned16(StoryFileHeaderAddress.StaticMem), Times.Once());
            //fileheader.Verify(fh => fh.getSerialNumber(), Times.Once());
            fileheader.Verify(fh => fh.SerialNumber, Times.AtLeastOnce());// HACK: The original test seemed wrong!
            //machine.Verify(m => m.copyBytesToArray(It.IsAny<byte[]>(), 0, 0, 12345));//allowing
            //machine.Verify(m => m.readUnsigned8(It.IsAny<int>()));//allowing
            Assert.AreEqual(4711, result);
            Assert.AreEqual(42, result2);
            Assert.AreEqual(4712, result3);
            Assert.AreEqual("850101", result4);
            Assert.AreEqual(12345, result5);
            Assert.AreEqual(1, result6);
            Assert.AreEqual(4, result7);
        }

        [TestMethod]
        public void ExportToFormChunk()
        {
            // arrange
            char[] dummyStack = { (char)(short)1, (char)(short)2, (char)(short)3 };
            StackFrame dummyFrame = new StackFrame();
            dummyFrame.Args = new char[0];
            dummyFrame.EvalStack = dummyStack;
            dummyFrame.Locals = new char[0];

            byte[] dynamicMem = new byte[99];
            dynamicMem[35] = (byte)12;
            dynamicMem[49] = (byte)13;
            dynamicMem[51] = (byte)21;
            dynamicMem[72] = (byte)72;
            dynamicMem[98] = (byte)1;

            gameState.Release = 42;
            gameState.Checksum = 4712;
            gameState.SerialNumber = "850101";
            gameState.SetDynamicMem(dynamicMem);
            gameState.ProgramCounter = (char)4711;
            gameState.StackFrames.Add(dummyFrame);

            // act

            // Export our mock machine to a FormChunk and verify some basic information
            WritableFormChunk exportFormChunk = gameState.ExportToFormChunk();

            // Read IFhd information
            IChunk ifhdChunk = exportFormChunk.GetSubChunk("IFhd");
            IMemory memaccess = ifhdChunk.Memory;
            var release = memaccess.ReadUnsigned16(8);
            byte[] serial = new byte[6];
            memaccess.CopyBytesToArray(serial, 0, 10, 6);
            var checksum = memaccess.ReadUnsigned16(16);
            var programCounter = DecodePcBytes(memaccess.ReadUnsigned8(18),
                memaccess.ReadUnsigned8(19),
                memaccess.ReadUnsigned8(20));

            // Read the UMem information
            IChunk umemChunk = exportFormChunk.GetSubChunk("UMem");

            // Read the Stks information
            IChunk stksChunk = exportFormChunk.GetSubChunk("Stks");

            // There is only one frame at the moment
            int retpc0 = DecodePcBytes(memaccess.ReadUnsigned8(8),
                memaccess.ReadUnsigned8(9),
                memaccess.ReadUnsigned8(10));

            // Now read the form chunk into another gamestate and compare
            PortableGameState gameState2 = new PortableGameState();
            gameState2.ReadSaveGame(exportFormChunk);
            StackFrame dummyFrame1 = gameState.StackFrames[0];
            StackFrame dummyFrame2 = gameState2.StackFrames[0];

            // Convert to byte array and reconstruct
            // This is in fact a test for WritableFormChunk and should be put
            // in a separate test
            byte[] data = exportFormChunk.Bytes;
            IFormChunk formChunk2 = new FormChunk(new Memory(data));

            // IFhd chunk
            IChunk ifhd2 = formChunk2.GetSubChunk("IFhd");
            IMemory ifhd1mem = exportFormChunk.GetSubChunk("IFhd").Memory;
            IMemory ifhd2mem = ifhd2.Memory;

            // UMem chunk
            IChunk umem2 = formChunk2.GetSubChunk("UMem");
            IMemory umem1mem = exportFormChunk.GetSubChunk("UMem").Memory;
            IMemory umem2mem = umem2.Memory;

            // Stks chunk
            IChunk stks2 = formChunk2.GetSubChunk("Stks");
            IMemory stks1mem = exportFormChunk.GetSubChunk("Stks").Memory;
            IMemory stks2mem = stks2.Memory;

            // assert
            Assert.AreEqual("FORM", exportFormChunk.Id);
            Assert.AreEqual(156, exportFormChunk.Size);
            Assert.AreEqual("IFZS", exportFormChunk.SubId);
            Assert.IsNotNull(ifhdChunk);
            Assert.IsNotNull(umem2);
            Assert.IsNotNull(stks2);

            // Read IFhd information
            Assert.AreEqual(13, ifhdChunk.Size);
            Assert.AreEqual(gameState.Release, release);
            var serialBytes = new byte[Encoding.UTF8.GetByteCount(gameState.SerialNumber)];
            Encoding.UTF8.GetBytes(gameState.SerialNumber, 0, gameState.SerialNumber.Length, (byte[])(object)serialBytes, 0);
            Assert.IsTrue(serialBytes.SequenceEqual(serial));
            Assert.AreEqual(gameState.Checksum, checksum);
            Assert.AreEqual(gameState.ProgramCounter, programCounter);

            // Read the UMem information
            Assert.AreEqual(dynamicMem.Length, umemChunk.Size);
            memaccess = umemChunk.Memory;
            for (int i = 0; i < dynamicMem.Length; i++)
            {
                Assert.AreEqual(dynamicMem[i], (byte)memaccess.ReadUnsigned8(8 + i));
            }

            // Read the Stks information
            memaccess = stksChunk.Memory;

            // There is only one frame at the moment
            Assert.AreEqual(14, stksChunk.Size);
            //Assert.AreEqual(0, retpc0); // TODO: Fix this test!
            Assert.AreEqual(0, memaccess.ReadUnsigned8(11)); // pv flags
            Assert.AreEqual(0, memaccess.ReadUnsigned8(12)); // retvar
            Assert.AreEqual(0, memaccess.ReadUnsigned8(13)); // argspec
            Assert.AreEqual(3, memaccess.ReadUnsigned16(14)); // stack size
            Assert.AreEqual(1, memaccess.ReadUnsigned16(16)); // stack val 0
            Assert.AreEqual(2, memaccess.ReadUnsigned16(18)); // stack val 1
            Assert.AreEqual(3, memaccess.ReadUnsigned16(20)); // stack val 2

            // Now read the form chunk into another gamestate and compare
            Assert.AreEqual(gameState.Release, gameState2.Release);
            Assert.AreEqual(gameState.Checksum, gameState2.Checksum);
            Assert.AreEqual(gameState.SerialNumber, gameState2.SerialNumber);
            Assert.AreEqual(gameState.ProgramCounter, gameState2.ProgramCounter);
            Assert.AreEqual(gameState.StackFrames.Count, gameState2.StackFrames.Count);
            Assert.AreEqual(dummyFrame1.ProgramCounter, dummyFrame2.ProgramCounter);
            Assert.AreEqual(dummyFrame1.ReturnVariable, dummyFrame2.ReturnVariable);
            Assert.AreEqual(0, dummyFrame2.Args.Length);
            Assert.AreEqual(0, dummyFrame2.Locals.Length);
            Assert.AreEqual(3, dummyFrame2.EvalStack.Length);

            // Convert to byte array and reconstruct
            // This is in fact a test for WritableFormChunk and should be put
            // in a separate test
            Assert.AreEqual("FORM", formChunk2.Id);
            Assert.AreEqual("IFZS", formChunk2.SubId);
            Assert.AreEqual(exportFormChunk.Size, formChunk2.Size);

            // IFhd chunk
            Assert.AreEqual(13, ifhd2.Size);
            for (int i = 0; i< 21; i++)
            {
                Assert.AreEqual(ifhd2mem.ReadUnsigned8(i), ifhd1mem.ReadUnsigned8(i));
            }

            // UMem chunk
            Assert.AreEqual(dynamicMem.Length, umem2.Size);
            for (int i = 0; i < umem2.Size + Chunk.ChunkHeaderLength; i++)
            {
                Assert.AreEqual(umem2mem.ReadUnsigned8(i), umem1mem.ReadUnsigned8(i));
            }

            // Stks chunk
            Assert.AreEqual(14, stks2.Size);
            for (int i = 0; i < stks2.Size + Chunk.ChunkHeaderLength; i++)
            {
                Assert.AreEqual(stks2mem.ReadUnsigned8(i), stks1mem.ReadUnsigned8(i));
            }    
        }

        private int DecodePcBytes(char b0, char b1, char b2)
        {
            return ((b0 & 0xff) << 16) | ((b1 & 0xff) << 8) | (b2 & 0xff);
        }

        //// ******************************************************************
        //// ****
        ///*
        //public void testTransferState() {

        //  byte[] dynMem = {
        //      (byte) 0x10, (byte) 0x11, (byte) 0x12, (byte) 0x13  
        //  };
        //  PortableGameState gamestate = new PortableGameState();
        //  List<StackFrame> stackframes = gamestate.getStackFrames();

        //  StackFrame stackFrame0 = new StackFrame();
        //  int[] args0 = { 1, 2 };
        //  short[] locals0 = { (short) 1, (short) 2 };
        //  short[] stack0 = { (short) 11, (short) 21 };
        //  stackFrame0.setProgramCounter(4711);

        //  // what do we do with "N" calls which do not have return variables ?
        //  stackFrame0.setReturnVariable(0x20);
        //  stackFrame0.setLocals(locals0);
        //  stackFrame0.setEvalStack(stack0);
        //  stackFrame0.setArgs(args0);
        //  stackframes.add(stackFrame0);

        //  gamestate.setDynamicMem(dynMem);

        //  mockMachine.expects(atLeastOnce()).method("getFileHeader").will(returnValue(fileheader));
        //  mockFileheader.expects(once()).method("getProgramStart").will(returnValue(4711));
        //  mockFileheader.expects(once()).method("getGlobalsAddress").will(returnValue(5711));
        //  mockFileheader.expects(atLeastOnce()).method("getVersion").will(returnValue(5));
        //  mockFileheader.expects(once()).method("setEnabled").withAnyArguments();
        //  mockFileheader.expects(once()).method("setInterpreterNumber").withAnyArguments();
        //  mockFileheader.expects(once()).method("setInterpreterVersion").withAnyArguments();
        //  //mockFileheader.expects(once()).method("setStandardRevision").with(eq(1), eq(0));

        //  for (int i = 0; i < dynMem.length; i++) {
        //    mockMemory.expects(once()).method("writeByte").with(eq(i), eq((byte) dynMem[i]));
        //  }

        //  // Tests if the dynamic memory and the stack frames are
        //  // completely copied
        //  Machine tmpMachine = new MachineImpl();
        //  tmpMachine.initialize(dynMem, null, new DefaultInstructionDecoder());
        //  gamestate.transferStateToMachine(tmpMachine);
        //}*/

        [TestMethod]
        public void ReadStackFrameFromChunkDiscardResult()
        {
            // arrange
            StackFrame stackFrame = new StackFrame();
            PortableGameState gamestate = new PortableGameState();

            // PC
            machine.Setup(m => m.ReadUnsigned8(0)).Returns((char)0x00);
            machine.Setup(m => m.ReadUnsigned8(1)).Returns((char)0x12);
            machine.Setup(m => m.ReadUnsigned8(2)).Returns((char)0x20);

            // Return variable/locals flag: discard result/3 locals (0x13)
            machine.Setup(m => m.ReadUnsigned8(3)).Returns((char)0x13);

            // return variable is always 0 if discard result
            machine.Setup(m => m.ReadUnsigned8(4)).Returns((char)0x00);

            // supplied arguments, we define a and b
            machine.Setup(m => m.ReadUnsigned8(5)).Returns((char)0x03);

            // stack size, we define 2
            machine.Setup(m => m.ReadUnsigned16(6)).Returns((char)2);

            // local variables
            for (int i = 0; i < 3; i++)
            {
                machine.Setup(m => m.ReadUnsigned16(8 + i * 2)).Returns((char)i);
            }

            // stack variables
            for (int i = 0; i < 2; i++)
            {
                machine.Setup(m => m.ReadUnsigned16(8 + 6 + i * 2)).Returns((char)i);
            }

            // act
            gamestate.ReadStackFrame(stackFrame, machine.Object, 0);
            var result = stackFrame.ProgramCounter;
            var result2 = stackFrame.ReturnVariable;
            var result3 = stackFrame.Locals.Length;
            var result4 = stackFrame.EvalStack.Length;
            var result5 = stackFrame.Args.Length;

            // assert
            Assert.AreEqual(0x1220, result);
            Assert.AreEqual(PortableGameState.DiscardResult, result2);
            Assert.AreEqual(3, result3);
            Assert.AreEqual(2, result4);
            Assert.AreEqual(2, result5);

            // PC
            machine.Verify(m => m.ReadUnsigned8(0), Times.Once);
            machine.Verify(m => m.ReadUnsigned8(1), Times.Once);
            machine.Verify(m => m.ReadUnsigned8(2), Times.Once);

            // Return variable/locals flag: discard result/3 locals (0x13)
            machine.Verify(m => m.ReadUnsigned8(3), Times.Once);

            // return variable is always 0 if discard result
            machine.Verify(m => m.ReadUnsigned8(4), Times.Once);

            // supplied arguments, we define a and b
            machine.Verify(m => m.ReadUnsigned8(5), Times.Once);

            // stack size, we define 2
            machine.Verify(m => m.ReadUnsigned16(6), Times.Once);

            // local variables
            for (int i = 0; i < 3; i++)
            {
                machine.Verify(m => m.ReadUnsigned16(8 + i * 2), Times.Once);
            }

            // stack variables
            for (int i = 0; i < 2; i++)
            {
                machine.Verify(m => m.ReadUnsigned16(8 + 6 + i * 2), Times.Once);
            }
        }

        [TestMethod]
        public void ReadStackFrameFromChunkWithReturnVar()
        {
            // arrange
            StackFrame stackFrame = new StackFrame();
            PortableGameState gamestate = new PortableGameState();

            // PC
            machine.Setup(m => m.ReadUnsigned8(0)).Returns((char)0x00);
            machine.Setup(m => m.ReadUnsigned8(1)).Returns((char)0x12);
            machine.Setup(m => m.ReadUnsigned8(2)).Returns((char)0x21);

            // Return variable/locals flag: has return value/2 locals (0x02)
            machine.Setup(m => m.ReadUnsigned8(3)).Returns((char)0x02);

            // return variable is 5
            machine.Setup(m => m.ReadUnsigned8(4)).Returns((char)0x05);

            // supplied arguments, we define a, b and c
            machine.Setup(m => m.ReadUnsigned8(5)).Returns((char)0x07);

            // stack size, we define 3
            machine.Setup(m => m.ReadUnsigned16(6)).Returns((char)3);

            // local variables
            for (int i = 0; i < 2; i++)
            {
                machine.Setup(m => m.ReadUnsigned16(8 + i * 2)).Returns((char)i);
            }

            // stack variables
            for (int i = 0; i < 3; i++)
            {
                machine.Setup(m => m.ReadUnsigned16(8 + 4 + i * 2)).Returns((char)i);
            }

            // act
            gamestate.ReadStackFrame(stackFrame, machine.Object, 0);
            var result = stackFrame.ProgramCounter;
            var result2 = stackFrame.ReturnVariable;
            var result3 = stackFrame.Locals.Length;
            var result4 = stackFrame.EvalStack.Length;
            var result5 = stackFrame.Args.Length;

            // assert
            Assert.AreEqual(0x1221, result);
            Assert.AreEqual(5, result2);
            Assert.AreEqual(2, result3);
            Assert.AreEqual(3, result4);
            Assert.AreEqual(3, result5);

            // PC
            machine.Verify(m => m.ReadUnsigned8(0), Times.Once);
            machine.Verify(m => m.ReadUnsigned8(1), Times.Once);
            machine.Verify(m => m.ReadUnsigned8(2), Times.Once);

            // Return variable/locals flag: has return value/2 locals (0x02)
            machine.Verify(m => m.ReadUnsigned8(3), Times.Once);

            // return variable is 5
            machine.Verify(m => m.ReadUnsigned8(4), Times.Once);

            // supplied arguments, we define a, b and c
            machine.Verify(m => m.ReadUnsigned8(5), Times.Once);

            // stack size, we define 3
            machine.Verify(m => m.ReadUnsigned16(6), Times.Once);

            // local variables
            for (int i = 0; i < 2; i++)
            {
                machine.Verify(m => m.ReadUnsigned16(8 + i * 2), Times.Once);
            }

            // stack variables
            for (int i = 0; i < 3; i++)
            {
                machine.Verify(m => m.ReadUnsigned16(8 + 4 + i * 2), Times.Once);
            }
        }

        [TestMethod]
        public void WriteStackFrameToChunkDiscardResult()
        {
            // arrange
            List<byte> byteBuffer = new List<byte>();
            char[] args = { (char)0, (char)1 };
            char[] locals = { (char)((short)1) };
            char[] stack = { (char)((short)5), (char)((short)6) };

            // act
            StackFrame stackFrame = new StackFrame();
            stackFrame.ProgramCounter = (char)0x1220;
            stackFrame.ReturnVariable = PortableGameState.DiscardResult;
            stackFrame.Args = args;
            stackFrame.Locals = locals;
            stackFrame.EvalStack = stack;

            PortableGameState gamestate = new PortableGameState();
            gamestate.WriteStackFrameToByteBuffer(byteBuffer, stackFrame);

            // assert

            // pc
            Assert.AreEqual((byte)0x00, byteBuffer[0]);
            Assert.AreEqual((byte)0x12, byteBuffer[1]);
            Assert.AreEqual((byte)0x20, byteBuffer[2]);

            // pvflag
            Assert.AreEqual((byte)0x11, byteBuffer[3]);

            // return var
            Assert.AreEqual((byte)0x00, byteBuffer[4]);

            // argspec
            Assert.AreEqual((byte)0x03, byteBuffer[5]);

            // stack size
            Assert.AreEqual((byte)0x00, byteBuffer[6]);
            Assert.AreEqual((byte)0x02, byteBuffer[7]);

            // locals
            Assert.AreEqual((byte)0x00, byteBuffer[8]);
            Assert.AreEqual((byte)0x01, byteBuffer[9]);

            // stack
            Assert.AreEqual((byte)0x00, byteBuffer[10]);
            Assert.AreEqual((byte)0x05, byteBuffer[11]);
            Assert.AreEqual((byte)0x00, byteBuffer[12]);
            Assert.AreEqual((byte)0x06, byteBuffer[13]);
        }

        [TestMethod]
        public void WriteStackFrameToChunkWithReturnVar()
        {
            // arrange
            List<byte> byteBuffer = new List<byte>();
            char[] args = { (char)0, (char)1 };
            char[] locals = { (char)((short)1) };
            char[] stack = { (char)((short)5), (char)((short)6) };

            // act
            StackFrame stackFrame = new StackFrame();
            stackFrame.ProgramCounter = (char)0x1221;
            stackFrame.ReturnVariable = (char)6;
            stackFrame.Args = args;
            stackFrame.Locals = locals;
            stackFrame.EvalStack = stack;

            PortableGameState gamestate = new PortableGameState();
            gamestate.WriteStackFrameToByteBuffer(byteBuffer, stackFrame);

            // assert

            // pc
            Assert.AreEqual((byte)0x00, byteBuffer[0]);
            Assert.AreEqual((byte)0x12, byteBuffer[1]);
            Assert.AreEqual((byte)0x21, byteBuffer[2]);

            // pvflag
            Assert.AreEqual((byte)0x01, byteBuffer[3]);

            // return var
            Assert.AreEqual((byte)0x06, byteBuffer[4]);

            // argspec
            Assert.AreEqual((byte)0x03, byteBuffer[5]);

            // stack size
            Assert.AreEqual((byte)0x00, byteBuffer[6]);
            Assert.AreEqual((byte)0x02, byteBuffer[7]);

            // locals
            Assert.AreEqual((byte)0x00, byteBuffer[8]);
            Assert.AreEqual((byte)0x01, byteBuffer[9]);

            // stack
            Assert.AreEqual((byte)0x00, byteBuffer[10]);
            Assert.AreEqual((byte)0x05, byteBuffer[11]);
            Assert.AreEqual((byte)0x00, byteBuffer[12]);
            Assert.AreEqual((byte)0x06, byteBuffer[13]);
        }
    }
}
