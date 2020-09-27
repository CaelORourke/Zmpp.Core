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

namespace Zmpp.Core.Instructions.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Zmpp.Core;
    using Zmpp.Core.Instructions;
    using Zmpp.Core.IO;
    using Zmpp.Core.Vm;
    using Zmpp.Core.UI;
    using System.IO;

    /// <summary>
    /// Tests for the InstructionDecoder class.
    /// </summary>
    [TestClass]
    public class InstructionDecoderTest
    {
        private Machine machine;

        private Mock<IOutputStream> outputStream1, outputStream2, outputStream3;
        private Mock<IInputStream> inputStream1, inputStream0;
        private Mock<IScreenModel> screen;

        private InstructionDecoder decoder;
        private byte[] call_vs2 = {
            (byte) 0xec, 0x25, (byte) 0xbf, 0x3b, (byte) 0xf7, (byte) 0xa0, 0x10, 0x20, 0x01, 0x00
        };
        private byte[] save_undo = {
            (byte) 0xbe, (byte) 0x09, (byte) 0xff, (byte) 0x00
        };

    [TestInitialize]
    public void TestInitialize()
    {
        var logger = new Mock<ILogger>();

        byte[] data = File.ReadAllBytes("testfiles/minizork.z3");
        machine = new Machine(logger.Object);
        machine.Initialize(data, null);

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

        decoder = new InstructionDecoder();
        decoder.initialize(machine);
        }

        /// <summary>
        /// Tests for minizork's instructions. This is more of an integration test,
        /// leave it here anyways.
        /// </summary>
        [TestMethod]
        public void testMinizorkVarInstruction()
        {
            machine.PushStack((char)0, (char)1);
            // VARIABLE: Instruction at 0x37d9 is call 0x3b36, #3e88, #ffff
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x37d9);
            Assert.AreEqual("CALL $1d9b, $3e88, $ffff -> (SP)", info.toString());
            Assert.AreEqual(9, info.getLength());
            AbstractInstruction info2 = (AbstractInstruction)decoder.decodeInstruction(0x37e2);
            Assert.AreEqual("STOREW (SP)[$0001], $00, $01", info2.toString());
            Assert.AreEqual(5, info2.getLength());
        }

        [TestMethod]
        public void testMinizorkBranch()
        {
            // SHORT 1OP: Instruction at 0x3773 is jz g17 [true] 0x377f
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x3773);
            Assert.AreEqual("JZ G17[$0001]", info.toString());
            Assert.AreEqual(3, info.getLength());
        }

        [TestMethod]
        public void testMinizorkRet()
        {
            // call the method this RET is in
            machine.Call((char)0x1bc5, 0x3e88, new char[] { (char)0xffff }, (char)0);
            machine.SetVariable((char)5, (char)7);
            // SHORT 1OP: Instruction at 0x37d5 is ret L04
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x37d5);
            Assert.AreEqual("RET L04[$0007]", info.toString());
            Assert.AreEqual(2, info.getLength());
        }

        [TestMethod]
        public void testMinizorkC1OP()
        {
            // SHORT 1OP: Instruction at 0x379f is dec L01
            // call the method this RET is in
            machine.Call((char)0x1bc5, 0x3e88, new char[] { (char)0xffff }, (char)0);
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x379f);
            Assert.AreEqual("DEC $02", info.toString());
            Assert.AreEqual(2, info.getLength());
            // SHORT 1OP: Instruction at 0x3816 is jump 0x37d9
            AbstractInstruction info2 = (AbstractInstruction)decoder.decodeInstruction(0x3816);
            Assert.AreEqual("JUMP $ffc2", info2.toString());
            Assert.AreEqual(2, info.getLength());
            // SHORT 1OP: Instruction at 0x37c7 is inc L01
            AbstractInstruction info3 = (AbstractInstruction)decoder.decodeInstruction(0x37c7);
            Assert.AreEqual("INC $03", info3.toString());
            Assert.AreEqual(2, info.getLength());
        }

        [TestMethod]
        public void testMinizorkC0Op()
        {
            // SHORT 0OP: Instruction at 0x3788 is rfalse
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x3788);
            Assert.AreEqual("RFALSE", info.toString().Trim());
            Assert.AreEqual(1, info.getLength());
        }

        [TestMethod]
        public void testMinizorkLong()
        {
            // call the method this instruction is in
            machine.Call((char)0x1bc5, 0x3e88, new char[] { (char)0xffff }, (char)0);
            // LONG: Instruction at 0x37c9 is je L02, L01
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x37c9);
            Assert.AreEqual("JE L02[$0000], L01[$0000]", info.toString());
            Assert.AreEqual(4, info.getLength());
        }

        [TestMethod]
        public void testMinizorkPrint()
        {
            // call the method this instruction is in
            machine.Call((char)0x1c13, 0x3e88, new char[] { (char)0xffff }, (char)0);
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x393f);
            Assert.AreEqual("PRINT \"object\"", info.toString());
            Assert.AreEqual(5, info.getLength());

            // call the method this instruction is in
            machine.Call((char)0x2baf, 0x3e88, new char[] { (char)0xffff }, (char)0);
            AbstractInstruction info2 = (AbstractInstruction)decoder.decodeInstruction(0x5761);
            Assert.AreEqual("PRINT_RET \"Ok.\"", info2.toString());
            Assert.AreEqual(5, info2.getLength());
        }

        [TestMethod]
        public void testMinizorkLongVar()
        {
            machine.SetVariable((char)0, (char)0x4711);
            // AH !!! This is really a long instruction, but encoded as a
            // variable instruction, this is odd !!!!
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x58d4);
            Assert.AreEqual("AND (SP)[$4711], $07ff -> (SP)", info.toString());
            Assert.AreEqual(6, info.getLength());
        }

        [TestMethod]
        public void testMinizorkJump()
        {
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x58f7);
            Assert.AreEqual("JUMP $fff4", info.toString());
            Assert.AreEqual(3, info.getLength());
        }

        [TestMethod]
        public void testMinizorkGetSibling()
        {
            // call the method this instruction is in
            machine.Call((char)0x368b, 0x3e88, new char[] { (char)0xffff }, (char)0);
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x6dbd);
            Assert.AreEqual("GET_SIBLING L03[$0000] -> L03", info.toString());
            Assert.AreEqual(5, info.getLength());
        }

        [TestMethod]
        public void testJe3Operands()
        {
            // call the method this instruction is in
            machine.Call((char)0x368b, 0x3e88, new char[] { (char)0xffff }, (char)0);
            AbstractInstruction info = (AbstractInstruction)decoder.decodeInstruction(0x6dc5);
            Assert.AreEqual("JE L03[$0000], L06[$0000], $1e", info.toString());
            Assert.AreEqual(7, info.getLength());
        }

        [TestMethod]
        public void testDecodeCallVs2()
        {
            // arrange

            // Setup for machine 4
            IMemory call_vs2Mem = new Memory(call_vs2);
            var machine4 = new Mock<IMachine>();
            machine4.Setup(m => m.Version).Returns(4);
            machine4.Setup(m => m.ReadUnsigned8(It.IsInRange(0, 2, Moq.Range.Inclusive))).Returns<int>(address => call_vs2Mem.ReadUnsigned8(address));
            machine4.Setup(m => m.ReadUnsigned16(3)).Returns(call_vs2Mem.ReadUnsigned16(3));
            machine4.Setup(m => m.ReadUnsigned8(It.IsInRange(5, 9, Moq.Range.Inclusive))).Returns<int>(address => call_vs2Mem.ReadUnsigned8(address));
            machine4.Setup(m => m.GetVariable((char)160)).Returns((char)0x4711);
            machine4.Setup(m => m.GetVariable((char)1)).Returns((char)0x4712);
            InstructionDecoder decoder4 = new InstructionDecoder();
            decoder4.initialize(machine4.Object);

            // act
            AbstractInstruction info = (AbstractInstruction)decoder4.decodeInstruction(0);
            var result = info.toString();
            var result2 = info.getLength();

            // assert
            machine4.Verify(m => m.Version, Times.AtLeastOnce());
            machine4.Verify(m => m.ReadUnsigned8(It.IsInRange(0, 2, Moq.Range.Inclusive)), Times.AtLeastOnce());
            machine4.Verify(m => m.ReadUnsigned16(3), Times.AtLeastOnce());
            machine4.Verify(m => m.ReadUnsigned8(It.IsInRange(5, 9, Moq.Range.Inclusive)), Times.AtLeastOnce());
            machine4.Verify(m => m.GetVariable((char)160), Times.Once());
            machine4.Verify(m => m.GetVariable((char)1), Times.Once());

            // Expected:
            Assert.AreEqual("CALL_VS2 $3bf7, G90[$4711], $10, $20, L00[$4712] -> (SP)", result);
            Assert.AreEqual(10, result2);
        }

        [TestMethod]
        public void testDecodeSaveUndo()
        {
            // arrange

            // Setup for machine 5
            IMemory save_undoMem = new Memory(save_undo);
            var machine5 = new Mock<IMachine>();
            machine5.Setup(m => m.Version).Returns(5);
            machine5.Setup(m => m.ReadUnsigned8(It.IsInRange(0, 3, Moq.Range.Inclusive))).Returns<int>(address => save_undoMem.ReadUnsigned8(address));
            InstructionDecoder decoder5 = new InstructionDecoder();
            decoder5.initialize(machine5.Object);

            // act
            AbstractInstruction info = (AbstractInstruction)decoder5.decodeInstruction(0);
            var result = info.toString();
            var result2 = info.getLength();

            // assert
            machine5.Verify(m => m.Version, Times.AtLeastOnce());
            machine5.Verify(m => m.ReadUnsigned8(It.IsInRange(0, 3, Moq.Range.Inclusive)), Times.AtLeastOnce());

            // Expected:
            Assert.AreEqual("SAVE_UNDO  -> (SP)", result);
            Assert.AreEqual(4, result2);
        }
    }
}
