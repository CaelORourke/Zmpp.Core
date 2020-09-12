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
    /// This class represents the state of the Z machine in an external format,
    /// so it can be exchanged using the Quetzal IFF format.
    /// </summary>
    public class PortableGameState
    {
        private const long serialVersionUID = -9131764506887529659L;

        /// <summary>
        /// The return variable value for discard result.
        /// </summary>
        public const char DISCARD_RESULT = (char)0xffff;

        /// <summary>
        /// This class represents a stack frame in the portable game state model.
        /// </summary>
        public class StackFrame///public static class StackFrame
        {
            private const long serialVersionUID = 3880452419775034120L;

            /// <summary>
            /// The return program counter.
            /// </summary>
            public int pc;

            /// <summary>
            /// The return variable.
            /// </summary>
            public char returnVariable;

            /// <summary>
            /// The local variables.
            /// </summary>
            public char[] locals;

            /// <summary>
            /// The evaluation stack.
            /// </summary>
            public char[] evalStack;

            /// <summary>
            /// The arguments.
            /// </summary>
            public char[] args;

            /// <summary>
            /// Returns the program counter.
            /// </summary>
            /// <returns>program counter</returns>
            public int getProgramCounter() { return pc; }

            /// <summary>
            /// Returns the return variable.
            /// </summary>
            /// <returns>return variable</returns>
            public char getReturnVariable() { return returnVariable; }

            /// <summary>
            /// Returns the eval stack.
            /// </summary>
            /// <returns>eval stack</returns>
            public char[] getEvalStack() { return evalStack; }

            /// <summary>
            /// Returns the local variables.
            /// </summary>
            /// <returns>local variables</returns>
            public char[] getLocals() { return locals; }

            /// <summary>
            /// Returns the routine arguments.
            /// </summary>
            /// <returns>routine arguments</returns>
            public char[] getArgs() { return args; }

            /// <summary>
            /// Sets the program counter.
            /// </summary>
            /// <param name="aPc">new program counter</param>
            public void setProgramCounter(int aPc) { this.pc = aPc; }

            /// <summary>
            /// Sets the return variable number.
            /// </summary>
            /// <param name="varnum">variable number</param>
            public void setReturnVariable(char varnum)
            {
                this.returnVariable = varnum;
            }

            /// <summary>
            /// Sets the eval stack.
            /// </summary>
            /// <param name="stack">eval stack</param>
            public void setEvalStack(char[] stack) { this.evalStack = stack; }

            /// <summary>
            /// Sets the local variables.
            /// </summary>
            /// <param name="locals">local variables</param>
            public void setLocals(char[] locals) { this.locals = locals; }

            /// <summary>
            /// Sets the routine arguments.
            /// </summary>
            /// <param name="args">routine arguments</param>
            public void setArgs(char[] args) { this.args = args; }
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
        private List<StackFrame> stackFrames;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PortableGameState()
        {
            serialBytes = new byte[6];
            stackFrames = new List<StackFrame>();
        }

        #region Accessing the state

        /// <summary>
        /// Returns the game release number.
        /// </summary>
        /// <returns>the release number</returns>
        public int getRelease() { return release; }

        /// <summary>
        /// Returns the game checksum.
        /// </summary>
        /// <returns>the checksum</returns>
        public int getChecksum() { return checksum; }

        /// <summary>
        /// Returns the game serial number.
        /// </summary>
        /// <returns>the serial number</returns>
        public String getSerialNumber() { return Encoding.UTF8.GetString((byte[])(object)serialBytes, 0, serialBytes.Length); }

        /// <summary>
        /// Returns the program counter.
        /// </summary>
        /// <returns>the program counter</returns>
        public int getProgramCounter() { return pc; }

        /// <summary>
        /// Returns the list of stack frames.
        /// </summary>
        /// <returns>the stack frames</returns>
        public List<StackFrame> getStackFrames() { return stackFrames; }

        /// <summary>
        /// Returns the delta bytes. This is the changes in dynamic memory, where
        /// 0 represents no change.
        /// </summary>
        /// <returns>the delta bytes</returns>
        public byte[] getDeltaBytes() { return delta; }

        /// <summary>
        /// Returns the current dump of dynamic memory captured from a Machine object.
        /// </summary>
        /// <returns>the dynamic memory dump</returns>
        public byte[] getDynamicMemoryDump() { return dynamicMem; }

        /// <summary>
        /// Sets the release number.
        /// </summary>
        /// <param name="release">release number</param>
        public void setRelease(int release) { this.release = release; }

        /// <summary>
        /// Sets the checksum.
        /// </summary>
        /// <param name="checksum">checksum</param>
        public void setChecksum(int checksum) { this.checksum = checksum; }

        /// <summary>
        /// Sets the serial number.
        /// </summary>
        /// <param name="serial">serial number</param>
        public void setSerialNumber(String serial)
        {
            //this.serialBytes = serial.getBytes();
            this.serialBytes = new byte[Encoding.UTF8.GetByteCount(serial)];
            Encoding.UTF8.GetBytes(serial, 0, serial.Length, (byte[])(object)this.serialBytes, 0);
        }

        /// <summary>
        /// Sets the program counter.
        /// </summary>
        /// <param name="aPc">program counter</param>
        public void setProgramCounter(int aPc) { this.pc = aPc; }

        /// <summary>
        /// Sets the dynamic memory.
        /// </summary>
        /// <param name="memdata">dynamic memory data</param>
        public void setDynamicMem(byte[] memdata) { this.dynamicMem = memdata; }

        #endregion

        #region Reading the state from a file

        /// <summary>
        /// Initialize the state from an IFF form.
        /// </summary>
        /// <param name="formChunk">the IFF form</param>
        /// <returns>false if there was a consistency problem during the read</returns>
        public bool readSaveGame(IFormChunk formChunk)
        {
            stackFrames.Clear();
            if (formChunk != null && "IFZS".Equals(formChunk.getSubId()))
            {
                readIfhdChunk(formChunk);
                readStacksChunk(formChunk);
                readMemoryChunk(formChunk);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluate the contents of the IFhd chunk.
        /// </summary>
        /// <param name="formChunk">the FORM chunk</param>
        private void readIfhdChunk(IFormChunk formChunk)
        {
            IChunk ifhdChunk = formChunk.getSubChunk("IFhd");
            IMemory chunkMem = ifhdChunk.getMemory();
            int offset = ChunkBase.CHUNK_HEADER_LENGTH;

            // read release number
            release = chunkMem.readUnsigned16(offset);
            offset += 2;

            // read serial number
            chunkMem.copyBytesToArray(serialBytes, 0, offset, 6);
            offset += 6;

            // read check sum
            checksum = chunkMem.readUnsigned16(offset);
            offset += 2;

            // read pc
            pc = decodePcBytes(chunkMem.readUnsigned8(offset),
                                chunkMem.readUnsigned8(offset + 1),
                                chunkMem.readUnsigned8(offset + 2));
        }

        /// <summary>
        /// Evaluate the contents of the Stks chunk.
        /// </summary>
        /// <param name="formChunk">the FORM chunk</param>
        private void readStacksChunk(IFormChunk formChunk)
        {
            IChunk stksChunk = formChunk.getSubChunk("Stks");
            IMemory chunkMem = stksChunk.getMemory();
            int offset = ChunkBase.CHUNK_HEADER_LENGTH;
            int chunksize = stksChunk.getSize() + ChunkBase.CHUNK_HEADER_LENGTH;

            while (offset < chunksize)
            {
                StackFrame stackFrame = new StackFrame();
                offset = readStackFrame(stackFrame, chunkMem, offset);
                stackFrames.Add(stackFrame);
            }
        }

        /// <summary>
        /// Reads a stack frame from the specified chunk at the specified
        /// offset.
        /// </summary>
        /// <param name="stackFrame">the stack frame to set the data into</param>
        /// <param name="chunkMem">the Stks chunk to read from</param>
        /// <param name="offset">the offset to read the stack</param>
        /// <returns>the offset after reading the stack frame</returns>
        public int readStackFrame(StackFrame stackFrame, IMemory chunkMem, int offset)
        {
            int tmpoff = offset;
            stackFrame.pc = decodePcBytes(chunkMem.readUnsigned8(tmpoff),
                                            chunkMem.readUnsigned8(tmpoff + 1),
                                            chunkMem.readUnsigned8(tmpoff + 2));
            tmpoff += 3;

            byte pvFlags = (byte)(chunkMem.readUnsigned8(tmpoff++) & 0xff);
            int numLocals = pvFlags & 0x0f;
            bool discardResult = (pvFlags & 0x10) > 0;
            stackFrame.locals = new char[numLocals];

            // Read the return variable, ignore the result if DISCARD_RESULT
            char returnVar = chunkMem.readUnsigned8(tmpoff++);
            stackFrame.returnVariable = discardResult ? DISCARD_RESULT :
                                                        returnVar;
            byte argSpec = (byte)(chunkMem.readUnsigned8(tmpoff++) & 0xff);
            stackFrame.args = getArgs(argSpec);
            int evalStackSize = chunkMem.readUnsigned16(tmpoff);
            stackFrame.evalStack = new char[evalStackSize];
            tmpoff += 2;

            // Read local variables
            for (int i = 0; i < numLocals; i++)
            {
                stackFrame.locals[i] = chunkMem.readUnsigned16(tmpoff);
                tmpoff += 2;
            }

            // Read evaluation stack values
            for (int i = 0; i < evalStackSize; i++)
            {
                stackFrame.evalStack[i] = chunkMem.readUnsigned16(tmpoff);
                tmpoff += 2;
            }
            return tmpoff;
        }

        /// <summary>
        /// Evaluate the contents of the Cmem and the UMem chunks.
        /// </summary>
        /// <param name="formChunk">the FORM chunk</param>
        private void readMemoryChunk(IFormChunk formChunk)
        {
            IChunk cmemChunk = formChunk.getSubChunk("CMem");
            IChunk umemChunk = formChunk.getSubChunk("UMem");
            if (cmemChunk != null) { readCMemChunk(cmemChunk); }
            if (umemChunk != null) { readUMemChunk(umemChunk); }
        }

        /// <summary>
        /// Decompresses and reads the dynamic memory state.
        /// </summary>
        /// <param name="cmemChunk">the CMem chunk</param>
        private void readCMemChunk(IChunk cmemChunk)
        {
            IMemory chunkMem = cmemChunk.getMemory();
            int offset = ChunkBase.CHUNK_HEADER_LENGTH;
            int chunksize = cmemChunk.getSize() + ChunkBase.CHUNK_HEADER_LENGTH;
            List<Byte> byteBuffer = new List<Byte>();
            char b;

            while (offset < chunksize)
            {
                b = chunkMem.readUnsigned8(offset++);
                if (b == 0)
                {
                    char runlength = chunkMem.readUnsigned8(offset++);
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
        /// <param name="umemChunk">the UMem chunk</param>
        private void readUMemChunk(IChunk umemChunk)
        {
            IMemory chunkMem = umemChunk.getMemory();
            int datasize = umemChunk.getSize();
            dynamicMem = new byte[datasize];
            chunkMem.copyBytesToArray(dynamicMem, 0, ChunkBase.CHUNK_HEADER_LENGTH, datasize);
        }

        #endregion

        #region Reading the state from a Machine

        /// <summary>
        /// Makes a snapshot of the current machine state. The savePc argument
        /// is taken as the restore program counter.
        /// </summary>
        /// <param name="machine">a Machine</param>
        /// <param name="savePc">the program counter restore value</param>
        public void captureMachineState(IMachine machine, int savePc)
        {
            IStoryFileHeader fileheader = machine.getFileHeader();
            release = machine.getRelease();
            checksum = machine.readUnsigned16(StoryFileHeaderBase.CHECKSUM);
            //serialBytes = fileheader.getSerialNumber().getBytes();
            serialBytes = new byte[Encoding.UTF8.GetByteCount(fileheader.getSerialNumber())];
            Encoding.UTF8.GetBytes(fileheader.getSerialNumber(), 0, fileheader.getSerialNumber().Length, (byte[])(object)serialBytes, 0);
            pc = savePc;

            // capture dynamic memory which ends at address(staticsMem) - 1
            // uncompressed
            int staticMemStart =
                machine.readUnsigned16(StoryFileHeaderBase.STATIC_MEM);
            dynamicMem = new byte[staticMemStart];
            // Save the state of dynamic memory
            machine.copyBytesToArray(dynamicMem, 0, 0, staticMemStart);
            captureStackFrames(machine);
        }

        /// <summary>
        /// Read the list of RoutineContexts in Machine, convert them to StackFrames,
        /// prepending a dummy stack frame.
        /// </summary>
        /// <param name="machine">the machine object</param>
        private void captureStackFrames(IMachine machine)
        {
            List<RoutineContext> contexts = machine.getRoutineContexts();
            // Put in initial dummy stack frame
            StackFrame dummyFrame = new StackFrame();
            dummyFrame.args = new char[0];
            dummyFrame.locals = new char[0];
            int numElements = calculateNumStackElements(machine, contexts, 0, 0);
            dummyFrame.evalStack = new char[numElements];
            for (int i = 0; i < numElements; i++)
            {
                dummyFrame.evalStack[i] = machine.getStackElement(i);
            }
            stackFrames.Add(dummyFrame);

            // Write out stack frames
            for (int c = 0; c < contexts.Count; c++)
            {
                RoutineContext context = contexts[c];

                StackFrame stackFrame = new StackFrame();
                stackFrame.pc = context.getReturnAddress();
                stackFrame.returnVariable = context.getReturnVariable();

                // Copy local variables
                stackFrame.locals = new char[context.getNumLocalVariables()];
                for (int i = 0; i < stackFrame.locals.Length; i++)
                {
                    stackFrame.locals[i] = context.getLocalVariable((char)i);
                }

                // Create argument array
                stackFrame.args = new char[context.getNumArguments()];
                for (int i = 0; i < stackFrame.args.Length; i++)
                {
                    stackFrame.args[i] = (char)i;
                }

                // Transfer evaluation stack
                int localStackStart = context.getInvocationStackPointer();
                numElements = calculateNumStackElements(machine, contexts, c + 1,
                    localStackStart);
                stackFrame.evalStack = new char[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    stackFrame.evalStack[i] = machine.getStackElement(localStackStart + i);
                }
                stackFrames.Add(stackFrame);
            }
        }

        /// <summary>
        /// Determines the number of stack elements between localStackStart and
        /// the invocation stack pointer of the specified routine context.
        /// If contextIndex is greater than the size of the List contexts, the
        /// functions assumes this is the top routine context and therefore
        /// calculates the difference between the current stack pointer and
        /// localStackStart.
        /// </summary>
        /// <param name="machine">the Machine object</param>
        /// <param name="contexts">a list of RoutineContext</param>
        /// <param name="contextIndex">the index of the context to calculate the difference</param>
        /// <param name="localStackStart">the local stack start pointer</param>
        /// <returns>the number of stack elements in the specified stack frame</returns>
        private int calculateNumStackElements(IMachine machine, List<RoutineContext> contexts, int contextIndex, int localStackStart)
        {

            if (contextIndex < contexts.Count)
            {
                RoutineContext context = contexts[contextIndex];
                return context.getInvocationStackPointer() - localStackStart;
            }
            else
            {
                return machine.getSP() - localStackStart;
            }
        }

        #endregion

        #region Export to an IFF FORM chunk

        /// <summary>
        /// Exports the current object state to a FormChunk.
        /// </summary>
        /// <returns>the state as a FormChunk</returns>
        public WritableFormChunk exportToFormChunk()
        {
            //byte[] id = "IFZS".getBytes();
            byte[] id = new byte[Encoding.UTF8.GetByteCount("IFZS")];
            Encoding.UTF8.GetBytes("IFZS", 0, "IFZS".Length, (byte[])(object)id, 0);
            WritableFormChunk formChunk = new WritableFormChunk(id);
            formChunk.addChunk(createIfhdChunk());
            formChunk.addChunk(createUMemChunk());
            formChunk.addChunk(createStksChunk());

            return formChunk;
        }

        /// <summary>
        /// Creates the IFhd chunk.
        /// </summary>
        /// <returns>IFhd chunk</returns>
        private IChunk createIfhdChunk()
        {
            //byte[] id = "IFhd".getBytes();
            byte[] id = new byte[Encoding.UTF8.GetByteCount("IFhd")];
            Encoding.UTF8.GetBytes("IFhd", 0, "IFhd".Length, (byte[])(object)id, 0);
            byte[] data = new byte[13];
            IChunk chunk = new DefaultChunk(id, data);
            IMemory chunkmem = chunk.getMemory();

            // Write release number
            chunkmem.writeUnsigned16(8, toUnsigned16(release));

            // Copy serial bytes
            chunkmem.copyBytesFromArray(serialBytes, 0, 10, serialBytes.Length);
            chunkmem.writeUnsigned16(16, toUnsigned16(checksum));
            chunkmem.writeUnsigned8(18, (char)(((int)((uint)pc >> 16)) & 0xff));
            chunkmem.writeUnsigned8(19, (char)(((int)((uint)pc >> 8)) & 0xff));
            chunkmem.writeUnsigned8(20, (char)(pc & 0xff));

            return chunk;
        }

        /// <summary>
        /// Creates the UMem chunk.
        /// </summary>
        /// <returns>UMem chunk</returns>
        private IChunk createUMemChunk()
        {
            //byte[] id = "UMem".getBytes();
            byte[] id = new byte[Encoding.UTF8.GetByteCount("UMem")];
            Encoding.UTF8.GetBytes("UMem", 0, "UMem".Length, (byte[])(object)id, 0);

            return new DefaultChunk(id, dynamicMem);
        }

        /// <summary>
        /// Creates the Stks chunk.
        /// </summary>
        /// <returns>Stks chunk</returns>
        private IChunk createStksChunk()
        {
            //byte[] id = "Stks".getBytes();
            byte[] id = new byte[Encoding.UTF8.GetByteCount("Stks")];
            Encoding.UTF8.GetBytes("Stks", 0, "Stks".Length, (byte[])(object)id, 0);
            List<Byte> byteBuffer = new List<Byte>();

            foreach (StackFrame stackFrame in stackFrames)
            {
                writeStackFrameToByteBuffer(byteBuffer, stackFrame);
            }
            byte[] data = new byte[byteBuffer.Count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = byteBuffer[i];
            }
            return new DefaultChunk(id, data);
        }

        /// <summary>
        /// Writes the specified stackframe to the given byte buffer.
        /// </summary>
        /// <param name="byteBuffer">a byte buffer</param>
        /// <param name="stackFrame">the stack frame</param>
        public void writeStackFrameToByteBuffer(List<Byte> byteBuffer, StackFrame stackFrame)
        {
            int returnPC = stackFrame.pc;
            byteBuffer.Add((byte)(((int)((uint)returnPC >> 16)) & 0xff));
            byteBuffer.Add((byte)(((int)((uint)returnPC >> 8)) & 0xff));
            byteBuffer.Add((byte)(returnPC & 0xff));

            // locals flag, is simply the number of local variables
            bool discardResult = stackFrame.returnVariable == DISCARD_RESULT;
            byte pvFlag = (byte)(stackFrame.locals.Length & 0x0f);
            if (discardResult) { pvFlag |= 0x10; }
            byteBuffer.Add(pvFlag);

            // returnvar
            byteBuffer.Add((byte)(discardResult ? 0 : stackFrame.returnVariable));

            // argspec
            byteBuffer.Add(createArgSpecByte(stackFrame.args));

            // eval stack size
            int stacksize = stackFrame.evalStack.Length;
            addUnsigned16ToByteBuffer(byteBuffer, (char)stacksize);

            // local variables
            foreach (char local in stackFrame.locals)
            {
                addUnsigned16ToByteBuffer(byteBuffer, local);
            }

            // stack values
            foreach (char stackValue in stackFrame.evalStack)
            {
                addUnsigned16ToByteBuffer(byteBuffer, stackValue);
            }
        }

        /// <summary>
        /// Appends unsigned 16 bit value to the byte buffer.
        /// </summary>
        /// <param name="buffer">byte buffer</param>
        /// <param name="value">unsigned 16 bit value</param>
        private void addUnsigned16ToByteBuffer(List<Byte> buffer, char value)
        {
            buffer.Add((byte)((int)((uint)(value & 0xff00) >> 8)));
            buffer.Add((byte)(value & 0xff));
        }

        /// <summary>
        /// Makes an arg spec byte from the arguments.
        /// </summary>
        /// <param name="args">arguments</param>
        /// <returns>spec byte</returns>
        private static byte createArgSpecByte(char[] args)
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
        /// <param name="machine">a Machine object</param>
        public void transferStateToMachine(IMachine machine)
        {
            // Copy dynamic memory
            machine.copyBytesFromArray(dynamicMem, 0, 0, dynamicMem.Length);

            // Stack frames
            List<RoutineContext> contexts = new List<RoutineContext>();

            // Dummy frame, only the stack is interesting
            if (stackFrames.Count > 0)
            {
                StackFrame dummyFrame = stackFrames[0];

                // Stack
                for (int s = 0; s < dummyFrame.getEvalStack().Length; s++)
                {
                    machine.setVariable((char)0, dummyFrame.getEvalStack()[s]);
                }
            }

            // Now iterate through all real stack frames
            for (int i = 1; i < stackFrames.Count; i++)
            {

                StackFrame stackFrame = stackFrames[i];
                // ignore the start address
                RoutineContext context =
                    new RoutineContext(stackFrame.locals.Length);

                context.setReturnVariable(stackFrame.returnVariable);
                context.setReturnAddress(stackFrame.pc);
                context.setNumArguments(stackFrame.args.Length);

                // local variables
                for (int l = 0; l < stackFrame.locals.Length; l++)
                {
                    context.setLocalVariable((char)l, stackFrame.locals[l]);
                }

                // Stack
                for (int s = 0; s < stackFrame.evalStack.Length; s++)
                {
                    machine.setVariable((char)0, stackFrame.evalStack[s]);
                }
                contexts.Add(context);
            }
            machine.setRoutineContexts(contexts);

            // Prepare the machine continue
            int resumePc = getProgramCounter();
            if (machine.getVersion() <= 3)
            {
                // In version 3 this is a branch target that needs to be read
                // Execution is continued at the first instruction after the branch offset
                resumePc += getBranchOffsetLength(machine, resumePc);
            }
            else if (machine.getVersion() >= 4)
            {
                // in version 4 and later, this is always 1
                resumePc++;
            }
            machine.setPC(resumePc);
        }

        /// <summary>
        /// For versions >= 4. Returns the store variable
        /// </summary>
        /// <param name="machine">the machine</param>
        /// <returns>the store variable</returns>
        public char getStoreVariable(IMachine machine)
        {
            int storeVarAddress = getProgramCounter();
            return machine.readUnsigned8(storeVarAddress);
        }

        /// <summary>
        /// Determine if the branch offset is one or two bytes long.
        /// </summary>
        /// <param name="memory">the Memory object of the current story</param>
        /// <param name="offsetAddress">the branch offset address</param>
        /// <returns>1 or 2, depending on the value of the branch offset</returns>
        private static int getBranchOffsetLength(IMemory memory, int offsetAddress)
        {
            char offsetByte1 = memory.readUnsigned8(offsetAddress);

            // Bit 6 set -> only one byte needs to be read
            return ((offsetByte1 & 0x40) > 0) ? 1 : 2;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// There is no apparent reason at the moment to implement getArgs().
        /// </summary>
        /// <param name="argspec">the argspec byte</param>
        /// <returns>the specified arguments</returns>
        private char[] getArgs(byte argspec)
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
        private int decodePcBytes(char b0, char b1, char b2)
        {
            return (int)(((b0 & 0xff) << 16) | ((b1 & 0xff) << 8) | (b2 & 0xff));
        }

        #endregion
    }
}
