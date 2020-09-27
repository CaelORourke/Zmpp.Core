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

namespace Zmpp.Core.Vm
{
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// Represents the state of the Z machine
    /// so it can be exchanged using the Quetzal IFF format.
    /// </summary>
    public class PortableGameState
    {
        /// <summary>
        /// The return variable value for discard result.
        /// </summary>
        public const char DiscardResult = (char)0xffff;

        /// <summary>
        /// Represents a stack frame.
        /// </summary>
        public class StackFrame
        {
            /// <summary>
            /// The return program counter.
            /// </summary>
            private int pc;

            /// <summary>
            /// The return variable.
            /// </summary>
            private char returnVariable;

            /// <summary>
            /// The local variables.
            /// </summary>
            private char[] locals;

            /// <summary>
            /// The evaluation stack.
            /// </summary>
            private char[] evalStack;

            /// <summary>
            /// The arguments.
            /// </summary>
            private char[] args;

            /// <summary>
            /// Gets or set the program counter.
            /// </summary>
            public int ProgramCounter
            {
                get
                {
                    return pc;
                }

                set
                {
                    pc = value;
                }
            }

            /// <summary>
            /// Gets or sets the return variable.
            /// </summary>
            public char ReturnVariable
            {
                get
                {
                    return returnVariable;
                }

                set
                {
                    returnVariable = value;
                }
            }

            /// <summary>
            /// Gets or sets the eval stack.
            /// </summary>
            public char[] EvalStack
            {
                get
                {
                    return evalStack;
                }

                set
                {
                    evalStack = value;
                }
            }

            /// <summary>
            /// Gets or sets the local variables.
            /// </summary>
            public char[] Locals
            {
                get
                {
                    return locals;
                }

                set
                {
                    locals = value;
                }
            }

            /// <summary>
            /// Gets or sets the routine arguments.
            /// </summary>
            public char[] Args
            {
                get
                {
                    return args;
                }

                set
                {
                    args = value;
                }
            }
        }

        /// <summary>
        /// The release number.
        /// </summary>
        private int release;

        /// <summary>
        /// The story file checksum.
        /// </summary>
        private int checksum;

        /// <summary>
        /// The serial number.
        /// </summary>
        private byte[] serialBytes;

        /// <summary>
        /// The program counter.
        /// </summary>
        private int pc;

        /// <summary>
        /// The uncompressed dynamic memory.
        /// </summary>
        private byte[] dynamicMem;

        /// <summary>
        /// The delta.
        /// </summary>
        private byte[] delta;

        /// <summary>
        /// The list of stack frames in this game state, from oldest to latest.
        /// </summary>
        private readonly List<StackFrame> stackFrames;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Zmpp.Core.Vm.PortableGameState"/> class.
        /// </summary>
        public PortableGameState()
        {
            serialBytes = new byte[6];
            stackFrames = new List<StackFrame>();
        }

        #region Accessing the state

        /// <summary>
        /// Gets or sets the game release number.
        /// </summary>
        public int Release
        {
            get
            {
                return release;
            }

            set
            {
                release = value;
            }
        }

        /// <summary>
        /// Gets or sets the game checksum.
        /// </summary>
        public int Checksum
        {
            get
            {
                return checksum;
            }

            set
            {
                checksum = value;
            }
        }

        /// <summary>
        /// Gets or sets the game serial number.
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return Encoding.UTF8.GetString((byte[])(object) serialBytes, 0, serialBytes.Length);
            }

            set
            {
                this.serialBytes = new byte[Encoding.UTF8.GetByteCount(value)];
                Encoding.UTF8.GetBytes(value, 0, value.Length, (byte[])(object)this.serialBytes, 0);
            }
        }

        /// <summary>
        /// Gets or sets the program counter.
        /// </summary>
        public int ProgramCounter
        {
            get
            {
                return pc;
            }

            set
            {
                pc = value;
            }
        }

        /// <summary>
        /// Gets the stack frames.
        /// </summary>
        public List<StackFrame> StackFrames => stackFrames;

        /// <summary>
        /// Gets the delta bytes.
        /// </summary>
        /// <returns>The delta bytes.</returns>
        /// <remarks>
        /// This is the changes in dynamic memory where
        /// 0 represents no change.
        /// </remarks>
        public byte[] GetDeltaBytes() { return delta; }

        /// <summary>
        /// Gets the current dump of dynamic memory captured from a Machine object.
        /// </summary>
        /// <returns>The dynamic memory dump.</returns>
        public byte[] GetDynamicMemoryDump() { return dynamicMem; }

        /// <summary>
        /// Sets the dynamic memory.
        /// </summary>
        /// <param name="memdata">The dynamic memory data.</param>
        public void SetDynamicMem(byte[] memdata) { this.dynamicMem = memdata; }

        #endregion

        #region Reading the state from a file

        /// <summary>
        /// Initialize the state from an IFF form.
        /// </summary>
        /// <param name="formChunk">The IFF form.</param>
        /// <returns>true if the read is successful; otherwise false.</returns>
        public bool ReadSaveGame(IFormChunk formChunk)
        {
            stackFrames.Clear();
            if (formChunk != null && "IFZS".Equals(formChunk.SubId))
            {
                ReadIfhdChunk(formChunk);
                ReadStacksChunk(formChunk);
                ReadMemoryChunk(formChunk);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Read the contents of the IFhd chunk.
        /// </summary>
        /// <param name="formChunk">The FORM chunk.</param>
        private void ReadIfhdChunk(IFormChunk formChunk)
        {
            IChunk ifhdChunk = formChunk.GetSubChunk("IFhd");
            IMemory chunkMem = ifhdChunk.Memory;
            int offset = ChunkBase.ChunkHeaderLength;

            // read release number
            release = chunkMem.ReadUnsigned16(offset);
            offset += 2;

            // read serial number
            chunkMem.CopyBytesToArray(serialBytes, 0, offset, 6);
            offset += 6;

            // read check sum
            checksum = chunkMem.ReadUnsigned16(offset);
            offset += 2;

            // read pc
            pc = DecodePcBytes(chunkMem.ReadUnsigned8(offset),
                                chunkMem.ReadUnsigned8(offset + 1),
                                chunkMem.ReadUnsigned8(offset + 2));
        }

        /// <summary>
        /// Read the contents of the Stks chunk.
        /// </summary>
        /// <param name="formChunk">the FORM chunk</param>
        private void ReadStacksChunk(IFormChunk formChunk)
        {
            IChunk stksChunk = formChunk.GetSubChunk("Stks");
            IMemory chunkMem = stksChunk.Memory;
            int offset = ChunkBase.ChunkHeaderLength;
            int chunksize = stksChunk.Size + ChunkBase.ChunkHeaderLength;

            while (offset < chunksize)
            {
                StackFrame stackFrame = new StackFrame();
                offset = ReadStackFrame(stackFrame, chunkMem, offset);
                stackFrames.Add(stackFrame);
            }
        }

        /// <summary>
        /// Reads a stack frame from the specified chunk at the specified
        /// offset.
        /// </summary>
        /// <param name="stackFrame">The stack frame to set the data into.</param>
        /// <param name="chunkMem">The Stks chunk to read from.</param>
        /// <param name="offset">The offset to read the stack.</param>
        /// <returns>The offset after reading the stack frame.</returns>
        public int ReadStackFrame(StackFrame stackFrame, IMemory chunkMem, int offset)
        {
            int tmpoff = offset;
            stackFrame.ProgramCounter = DecodePcBytes(chunkMem.ReadUnsigned8(tmpoff),
                                            chunkMem.ReadUnsigned8(tmpoff + 1),
                                            chunkMem.ReadUnsigned8(tmpoff + 2));
            tmpoff += 3;

            byte pvFlags = (byte)(chunkMem.ReadUnsigned8(tmpoff++) & 0xff);
            int numLocals = pvFlags & 0x0f;
            bool discardResult = (pvFlags & 0x10) > 0;
            stackFrame.Locals = new char[numLocals];

            // Read the return variable
            char returnVar = chunkMem.ReadUnsigned8(tmpoff++);
            // ignore the result if DiscardResult is true
            stackFrame.ReturnVariable = discardResult ? DiscardResult : returnVar;

            byte argSpec = (byte)(chunkMem.ReadUnsigned8(tmpoff++) & 0xff);
            stackFrame.Args = GetArgs(argSpec);
            int evalStackSize = chunkMem.ReadUnsigned16(tmpoff);
            stackFrame.EvalStack = new char[evalStackSize];
            tmpoff += 2;

            // Read local variables
            for (int i = 0; i < numLocals; i++)
            {
                stackFrame.Locals[i] = chunkMem.ReadUnsigned16(tmpoff);
                tmpoff += 2;
            }

            // Read evaluation stack values
            for (int i = 0; i < evalStackSize; i++)
            {
                stackFrame.EvalStack[i] = chunkMem.ReadUnsigned16(tmpoff);
                tmpoff += 2;
            }
            return tmpoff;
        }

        /// <summary>
        /// Read the contents of the Cmem and the UMem chunks.
        /// </summary>
        /// <param name="formChunk">The FORM chunk.</param>
        private void ReadMemoryChunk(IFormChunk formChunk)
        {
            IChunk cmemChunk = formChunk.GetSubChunk("CMem");
            IChunk umemChunk = formChunk.GetSubChunk("UMem");
            if (cmemChunk != null) { ReadCMemChunk(cmemChunk); }
            if (umemChunk != null) { ReadUMemChunk(umemChunk); }
        }

        /// <summary>
        /// Decompresses and reads the dynamic memory state.
        /// </summary>
        /// <param name="cmemChunk">The CMem chunk.</param>
        private void ReadCMemChunk(IChunk cmemChunk)
        {
            IMemory chunkMem = cmemChunk.Memory;
            int offset = ChunkBase.ChunkHeaderLength;
            int chunksize = cmemChunk.Size + ChunkBase.ChunkHeaderLength;
            List<Byte> byteBuffer = new List<Byte>();
            char b;

            while (offset < chunksize)
            {
                b = chunkMem.ReadUnsigned8(offset++);
                if (b == 0)
                {
                    char runlength = chunkMem.ReadUnsigned8(offset++);
                    for (int r = 0; r <= runlength; r++)
                    { // (runlength + 1) iterations
                        byteBuffer.Add((byte)0);
                    }
                }
                else
                {
                    byteBuffer.Add((byte)(b & 0xff));
                }
            }

            // Copy the results to the delta array
            delta = new byte[byteBuffer.Count];
            for (int i = 0; i < delta.Length; i++)
            {
                delta[i] = byteBuffer[i];
            }
        }

        /// <summary>
        /// Reads the uncompressed dynamic memory state.
        /// </summary>
        /// <param name="umemChunk">The UMem chunk.</param>
        private void ReadUMemChunk(IChunk umemChunk)
        {
            IMemory chunkMem = umemChunk.Memory;
            int datasize = umemChunk.Size;
            dynamicMem = new byte[datasize];
            chunkMem.CopyBytesToArray(dynamicMem, 0, ChunkBase.ChunkHeaderLength, datasize);
        }

        #endregion

        #region Reading the state from a Machine

        /// <summary>
        /// Captures a snapshot of the current machine state.
        /// </summary>
        /// <param name="machine">The Machine object.</param>
        /// <param name="savePc">The program counter restore value.</param>
        /// <remarks>
        /// The savePc argument is taken as the restore program counter.
        /// </remarks>
        public void CaptureMachineState(IMachine machine, int savePc)
        {
            IStoryFileHeader fileheader = machine.FileHeader;
            release = machine.Release;
            checksum = machine.ReadUnsigned16(StoryFileHeaderAddress.Checksum);
            serialBytes = new byte[Encoding.UTF8.GetByteCount(fileheader.SerialNumber)];
            Encoding.UTF8.GetBytes(fileheader.SerialNumber, 0, fileheader.SerialNumber.Length, (byte[])(object)serialBytes, 0);
            pc = savePc;

            // capture dynamic memory which ends at address(staticsMem) - 1
            // uncompressed
            int staticMemStart =
                machine.ReadUnsigned16(StoryFileHeaderAddress.StaticMem);
            dynamicMem = new byte[staticMemStart];
            // Save the state of dynamic memory
            machine.CopyBytesToArray(dynamicMem, 0, 0, staticMemStart);
            CaptureStackFrames(machine);
        }

        /// <summary>
        /// Captures the routine contexts of the specified machine.
        /// </summary>
        /// <param name="machine">The Machine object.</param>
        /// <remarks>
        /// The routine contexts are captured as a collection of stack frames
        /// prepended with a dummy stack frame.
        /// </remarks>
        private void CaptureStackFrames(IMachine machine)
        {
            List<RoutineContext> contexts = machine.GetRoutineContexts();
            // Put in initial dummy stack frame
            StackFrame dummyFrame = new StackFrame();
            dummyFrame.Args = new char[0];
            dummyFrame.Locals = new char[0];
            int numElements = CalculateNumStackElements(machine, contexts, 0, 0);
            dummyFrame.EvalStack = new char[numElements];
            for (int i = 0; i < numElements; i++)
            {
                dummyFrame.EvalStack[i] = machine.GetStackElement(i);
            }
            stackFrames.Add(dummyFrame);

            // write out the stack frames
            for (int c = 0; c < contexts.Count; c++)
            {
                RoutineContext context = contexts[c];

                StackFrame stackFrame = new StackFrame();
                stackFrame.ProgramCounter = context.ReturnAddress;
                stackFrame.ReturnVariable = context.ReturnVariable;

                // copy the local variables
                stackFrame.Locals = new char[context.NumLocalVariables];
                for (int i = 0; i < stackFrame.Locals.Length; i++)
                {
                    stackFrame.Locals[i] = context.GetLocalVariable((char)i);
                }

                // create an array of arguments
                stackFrame.Args = new char[context.NumArguments];
                for (int i = 0; i < stackFrame.Args.Length; i++)
                {
                    stackFrame.Args[i] = (char)i;
                }

                // transfer evaluation stack
                int localStackStart = context.InvocationStackPointer;
                numElements = CalculateNumStackElements(machine, contexts, c + 1,
                    localStackStart);
                stackFrame.EvalStack = new char[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    stackFrame.EvalStack[i] = machine.GetStackElement(localStackStart + i);
                }
                stackFrames.Add(stackFrame);
            }
        }

        /// <summary>
        /// Gets the number of stack elements between localStackStart and
        /// the invocation stack pointer of the specified routine context.
        /// </summary>
        /// <param name="machine">The Machine object.</param>
        /// <param name="contexts">The list of RoutineContext.</param>
        /// <param name="contextIndex">The index of the context to calculate the difference.</param>
        /// <param name="localStackStart">The local stack start pointer.</param>
        /// <returns>The number of stack elements in the specified stack frame.</returns>
        /// <remarks>
        /// If contextIndex is greater than the size of the List contexts the
        /// method assumes that this is the top routine context and
        /// calculates the difference between the current stack pointer and
        /// localStackStart.
        /// </remarks>
        private int CalculateNumStackElements(IMachine machine, List<RoutineContext> contexts, int contextIndex, int localStackStart)
        {

            if (contextIndex < contexts.Count)
            {
                RoutineContext context = contexts[contextIndex];
                return context.InvocationStackPointer - localStackStart;
            }
            else
            {
                return machine.SP - localStackStart;
            }
        }

        #endregion

        #region Export to an IFF FORM chunk

        /// <summary>
        /// Exports the current object state to a FormChunk.
        /// </summary>
        /// <returns>The FormChunk object.</returns>
        public WritableFormChunk ExportToFormChunk()
        {
            byte[] id = new byte[Encoding.UTF8.GetByteCount("IFZS")];
            Encoding.UTF8.GetBytes("IFZS", 0, "IFZS".Length, (byte[])(object)id, 0);
            WritableFormChunk formChunk = new WritableFormChunk(id);
            formChunk.AddChunk(CreateIfhdChunk());
            formChunk.AddChunk(CreateUMemChunk());
            formChunk.AddChunk(CreateStksChunk());

            return formChunk;
        }

        /// <summary>
        /// Creates the IFhd chunk.
        /// </summary>
        /// <returns>The IFhd chunk.</returns>
        private IChunk CreateIfhdChunk()
        {
            byte[] id = new byte[Encoding.UTF8.GetByteCount("IFhd")];
            Encoding.UTF8.GetBytes("IFhd", 0, "IFhd".Length, (byte[])(object)id, 0);
            byte[] data = new byte[13];
            IChunk chunk = new Chunk(id, data);
            IMemory chunkmem = chunk.Memory;

            // Write release number
            chunkmem.WriteUnsigned16(8, ToUnsigned16(release));

            // Copy serial bytes
            chunkmem.CopyBytesFromArray(serialBytes, 0, 10, serialBytes.Length);
            chunkmem.WriteUnsigned16(16, ToUnsigned16(checksum));
            chunkmem.WriteUnsigned8(18, (char)(((int)((uint)pc >> 16)) & 0xff));
            chunkmem.WriteUnsigned8(19, (char)(((int)((uint)pc >> 8)) & 0xff));
            chunkmem.WriteUnsigned8(20, (char)(pc & 0xff));

            return chunk;
        }

        /// <summary>
        /// Creates the UMem chunk.
        /// </summary>
        /// <returns>The UMem chunk.</returns>
        private IChunk CreateUMemChunk()
        {
            byte[] id = new byte[Encoding.UTF8.GetByteCount("UMem")];
            Encoding.UTF8.GetBytes("UMem", 0, "UMem".Length, (byte[])(object)id, 0);

            return new Chunk(id, dynamicMem);
        }

        /// <summary>
        /// Creates the Stks chunk.
        /// </summary>
        /// <returns>The Stks chunk.</returns>
        private IChunk CreateStksChunk()
        {
            byte[] id = new byte[Encoding.UTF8.GetByteCount("Stks")];
            Encoding.UTF8.GetBytes("Stks", 0, "Stks".Length, (byte[])(object)id, 0);
            List<Byte> byteBuffer = new List<Byte>();

            foreach (StackFrame stackFrame in stackFrames)
            {
                WriteStackFrameToByteBuffer(byteBuffer, stackFrame);
            }
            byte[] data = new byte[byteBuffer.Count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = byteBuffer[i];
            }
            return new Chunk(id, data);
        }

        /// <summary>
        /// Writes the specified stack frame to the specified byte buffer.
        /// </summary>
        /// <param name="byteBuffer">The byte buffer.</param>
        /// <param name="stackFrame">The stack frame.</param>
        public void WriteStackFrameToByteBuffer(List<Byte> byteBuffer, StackFrame stackFrame)
        {
            int returnPC = stackFrame.ProgramCounter;
            byteBuffer.Add((byte)(((int)((uint)returnPC >> 16)) & 0xff));
            byteBuffer.Add((byte)(((int)((uint)returnPC >> 8)) & 0xff));
            byteBuffer.Add((byte)(returnPC & 0xff));

            // locals flag is simply the number of local variables
            bool discardResult = stackFrame.ReturnVariable == DiscardResult;
            byte pvFlag = (byte)(stackFrame.Locals.Length & 0x0f);
            if (discardResult) { pvFlag |= 0x10; }
            byteBuffer.Add(pvFlag);

            // returnvar
            byteBuffer.Add((byte)(discardResult ? 0 : stackFrame.ReturnVariable));

            // argspec
            byteBuffer.Add(CreateArgSpecByte(stackFrame.Args));

            // eval stack size
            int stacksize = stackFrame.EvalStack.Length;
            AddUnsigned16ToByteBuffer(byteBuffer, (char)stacksize);

            // local variables
            foreach (char local in stackFrame.Locals)
            {
                AddUnsigned16ToByteBuffer(byteBuffer, local);
            }

            // stack values
            foreach (char stackValue in stackFrame.EvalStack)
            {
                AddUnsigned16ToByteBuffer(byteBuffer, stackValue);
            }
        }

        /// <summary>
        /// Appends the specified unsigned 16 bit value to the specified byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="value">The unsigned 16 bit value.</param>
        private void AddUnsigned16ToByteBuffer(List<Byte> buffer, char value)
        {
            buffer.Add((byte)((int)((uint)(value & 0xff00) >> 8)));
            buffer.Add((byte)(value & 0xff));
        }

        /// <summary>
        /// Creates an arg spec byte from the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The arg spec byte.</returns>
        private static byte CreateArgSpecByte(char[] args)
        {
            byte result = 0;
            foreach (int arg in args) { result |= (byte)(1 << arg); }
            return result;
        }

        #endregion

        #region Transfer to Machine object

        /// <summary>
        /// Transfers the current object state to the specified Machine object.
        /// The machine needs to be in a reset state in order to function correctly.
        /// </summary>
        /// <param name="machine">The Machine object.</param>
        public void TransferStateToMachine(IMachine machine)
        {
            // Copy dynamic memory
            machine.CopyBytesFromArray(dynamicMem, 0, 0, dynamicMem.Length);

            // Stack frames
            List<RoutineContext> contexts = new List<RoutineContext>();

            // Dummy frame, only the stack is interesting
            if (stackFrames.Count > 0)
            {
                StackFrame dummyFrame = stackFrames[0];

                // Stack
                for (int s = 0; s < dummyFrame.EvalStack.Length; s++)
                {
                    machine.SetVariable((char)0, dummyFrame.EvalStack[s]);
                }
            }

            // Now iterate through all real stack frames
            for (int i = 1; i < stackFrames.Count; i++)
            {

                StackFrame stackFrame = stackFrames[i];
                // ignore the start address
                RoutineContext context =
                    new RoutineContext(stackFrame.Locals.Length);

                context.ReturnVariable = stackFrame.ReturnVariable;
                context.ReturnAddress = stackFrame.ProgramCounter;
                context.NumArguments = stackFrame.Args.Length;

                // local variables
                for (int l = 0; l < stackFrame.Locals.Length; l++)
                {
                    context.SetLocalVariable((char)l, stackFrame.Locals[l]);
                }

                // Stack
                for (int s = 0; s < stackFrame.EvalStack.Length; s++)
                {
                    machine.SetVariable((char)0, stackFrame.EvalStack[s]);
                }
                contexts.Add(context);
            }
            machine.SetRoutineContexts(contexts);

            // Prepare the machine continue
            int resumePc = ProgramCounter;
            if (machine.Version <= 3)
            {
                // In version 3 this is a branch target that needs to be read.
                // Execution is continued at the first instruction after the branch offset.
                resumePc += GetBranchOffsetLength(machine, resumePc);
            }
            else if (machine.Version >= 4)
            {
                // in version 4 and later this is always 1
                resumePc++;
            }
            machine.PC = resumePc;
        }

        /// <summary>
        /// For versions >= 4. Returns the store variable
        /// </summary>
        /// <param name="machine">The Machine object.</param>
        /// <returns>The store variable.</returns>
        public char GetStoreVariable(IMachine machine)
        {
            int storeVarAddress = ProgramCounter;
            return machine.ReadUnsigned8(storeVarAddress);
        }

        /// <summary>
        /// Determine if the branch offset is one or two bytes long.
        /// </summary>
        /// <param name="memory">The Memory object of the current story.</param>
        /// <param name="offsetAddress">The branch offset address.</param>
        /// <returns>1 or 2, depending on the value of the branch offset</returns>
        private static int GetBranchOffsetLength(IMemory memory, int offsetAddress)
        {
            char offsetByte1 = memory.ReadUnsigned8(offsetAddress);

            // Bit 6 set -> only one byte needs to be read
            return ((offsetByte1 & 0x40) > 0) ? 1 : 2;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the arguments for the specified argument spec byte.
        /// </summary>
        /// <param name="argspec">The argspec byte.</param>
        /// <returns>The specified arguments.</returns>
        /// <remarks>
        /// There is no apparent reason at the moment to implement this method.
        /// </remarks>
        private char[] GetArgs(byte argspec)
        {
            int andBit;
            List<char> result = new List<char>();

            for (int i = 0; i < 7; i++)
            {
                andBit = 1 << i;
                if ((andBit & argspec) > 0)
                {
                    result.Add((char)i);
                }
            }
            char[] charArray = new char[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                charArray[i] = result[i];
            }
            return charArray;
        }

        /// <summary>
        /// Joins three bytes to a program counter value.
        /// </summary>
        /// <param name="b0">byte 0</param>
        /// <param name="b1">byte 1</param>
        /// <param name="b2">byte 2</param>
        /// <returns></returns>
        private int DecodePcBytes(char b0, char b1, char b2)
        {
            return (int)(((b0 & 0xff) << 16) | ((b1 & 0xff) << 8) | (b2 & 0xff));
        }

        #endregion
    }
}
