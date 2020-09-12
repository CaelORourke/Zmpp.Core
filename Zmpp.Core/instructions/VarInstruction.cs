/*
 * Created on 2008/07/24
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

namespace Zmpp.Core.Instructions
{
    using Microsoft.Extensions.Logging;
    using Zmpp.Core.Media;
    using Zmpp.Core.Vm;
    using Zmpp.Core.UI;
    using System;
    using static Zmpp.Core.Vm.Instruction;

    /// <summary>
    /// Implementation of instructions with operand count VAR.
    /// </summary>
    public class VarInstruction : AbstractInstruction
    {
        private readonly ILogger LOG;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">Machine object</param>
        /// <param name="opcodeNum">opcode number</param>
        /// <param name="operands">operands</param>
        /// <param name="storeVar">store variable</param>
        /// <param name="branchInfo">branch information</param>
        /// <param name="opcodeLength">opcode length</param>
        public VarInstruction(IMachine machine, int opcodeNum, Operand[] operands, char storeVar, BranchInfo branchInfo, int opcodeLength) : base(machine, opcodeNum, operands, storeVar, branchInfo, opcodeLength)
        {
        }

        protected override OperandCount getOperandCount() { return OperandCount.VAR; }

        public override void execute()
        {
            switch (getOpcodeNum())
            {
                case VAR_CALL:
                    call();
                    break;
                case VAR_CALL_VS2:
                    call();
                    break;
                case VAR_STOREW:
                    storew();
                    break;
                case VAR_STOREB:
                    storeb();
                    break;
                case VAR_PUT_PROP:
                    put_prop();
                    break;
                case VAR_SREAD:
                    sread();
                    break;
                case VAR_PRINT_CHAR:
                    print_char();
                    break;
                case VAR_PRINT_NUM:
                    print_num();
                    break;
                case VAR_RANDOM:
                    random();
                    break;
                case VAR_PUSH:
                    push();
                    break;
                case VAR_PULL:
                    pull();
                    break;
                case VAR_SPLIT_WINDOW:
                    split_window();
                    break;
                case VAR_SET_TEXT_STYLE:
                    set_text_style();
                    break;
                case VAR_BUFFER_MODE:
                    buffer_mode();
                    break;
                case VAR_SET_WINDOW:
                    set_window();
                    break;
                case VAR_OUTPUT_STREAM:
                    output_stream();
                    break;
                case VAR_INPUT_STREAM:
                    input_stream();
                    break;
                case VAR_SOUND_EFFECT:
                    sound_effect();
                    break;
                case VAR_ERASE_WINDOW:
                    erase_window();
                    break;
                case VAR_ERASE_LINE:
                    erase_line();
                    break;
                case VAR_SET_CURSOR:
                    set_cursor();
                    break;
                case VAR_GET_CURSOR:
                    get_cursor();
                    break;
                case VAR_READ_CHAR:
                    read_char();
                    break;
                case VAR_SCAN_TABLE:
                    scan_table();
                    break;
                case VAR_NOT:
                    not();
                    break;
                case VAR_CALL_VN:
                case VAR_CALL_VN2:
                    call();
                    break;
                case VAR_TOKENISE:
                    tokenise();
                    break;
                case VAR_ENCODE_TEXT:
                    encode_text();
                    break;
                case VAR_COPY_TABLE:
                    copy_table();
                    break;
                case VAR_PRINT_TABLE:
                    print_table();
                    break;
                case VAR_CHECK_ARG_COUNT:
                    check_arg_count();
                    break;
                default:
                    throwInvalidOpcode();
                    break;
            }
        }

        /// <summary>
        /// CALL instruction.
        /// </summary>
        private void call()
        {
            call(getNumOperands() - 1);
        }

        /// <summary>
        /// STOREW instruction.
        /// </summary>
        private void storew()
        {
            int array = getUnsignedValue(0);
            int wordIndex = getSignedValue(1);
            int memAddress = (array + 2 * wordIndex) & 0xffff;
            char value = getUnsignedValue(2);
            getMachine().WriteUnsigned16(memAddress, value);
            nextInstruction();
        }

        /// <summary>
        /// STOREB instruction.
        /// </summary>
        private void storeb()
        {
            int array = getUnsignedValue(0);
            int byteIndex = getSignedValue(1);
            int memAddress = (array + byteIndex) & 0xffff;
            int value = getUnsignedValue(2);
            getMachine().WriteUnsigned8(memAddress, (char)(value & 0xff));
            nextInstruction();
        }

        /// <summary>
        /// PUT_PROP instruction.
        /// </summary>
        private void put_prop()
        {
            int obj = getUnsignedValue(0);
            int property = getUnsignedValue(1);
            char value = getUnsignedValue(2);

            if (obj > 0)
            {
                getMachine().setProperty(obj, property, value);
                nextInstruction();
            }
            else
            {
                // Issue warning for non-existent object
                getMachine().warn("@put_prop illegal access to object " + obj);
                nextInstruction();
            }
        }

        /// <summary>
        /// PRINT_CHAR instruction.
        /// </summary>
        private void print_char()
        {
            char zchar = (char)getUnsignedValue(0);
            getMachine().printZsciiChar(zchar);
            nextInstruction();
        }

        /// <summary>
        /// PRINT_NUM instruction.
        /// </summary>
        private void print_num()
        {
            short number = getSignedValue(0);
            getMachine().printNumber(number);
            nextInstruction();
        }

        /// <summary>
        /// PUSH instruction.
        /// </summary>
        private void push()
        {
            char value = getUnsignedValue(0);
            getMachine().setVariable((char)0, value);
            nextInstruction();
        }

        /// <summary>
        /// PULL instruction.
        /// </summary>
        private void pull()
        {
            if (getStoryVersion() == 6)
            {
                pull_v6();
            }
            else
            {
                pull_std();
            }
            nextInstruction();
        }

        /// <summary>
        /// PULL instruction for version 6 stories.
        /// </summary>
        private void pull_v6()
        {
            char stack = (char)0;
            if (getNumOperands() == 1)
            {
                stack = getUnsignedValue(0);
            }
            storeUnsignedResult(getMachine().popStack(stack));
        }

        /// <summary>
        /// PULL instruction for stories except V6.
        /// </summary>
        private void pull_std()
        {
            char varnum = getUnsignedValue(0);
            char value = getMachine().getVariable((char)0);

            // standard 1.1
            if (varnum == 0)
            {
                getMachine().setStackTop(value);
            }
            else
            {
                getMachine().setVariable(varnum, value);
            }
        }

        /// <summary>
        /// OUTPUT_STREAM instruction.
        /// </summary>
        private void output_stream()
        {
            // Stream number should be a signed byte
            short streamnumber = getSignedValue(0);

            if (streamnumber < 0 && streamnumber >= -3)
            {
                getMachine().selectOutputStream(-streamnumber, false);
            }
            else if (streamnumber > 0 && streamnumber <= 3)
            {
                if (streamnumber == OutputBase.OUTPUTSTREAM_MEMORY)
                {
                    int tableAddress = getUnsignedValue(1);
                    int tablewidth = 0;
                    if (getNumOperands() == 3)
                    {
                        tablewidth = getUnsignedValue(2);
                        LOG.LogInformation(string.Format("@output_stream 3 {0:x} {1:D}\n", tableAddress, tablewidth));
                    }
                    getMachine().selectOutputStream3(tableAddress, tablewidth);
                }
                else
                {
                    getMachine().selectOutputStream(streamnumber, true);
                }
            }
            nextInstruction();
        }

        /// <summary>
        /// INPUT_STREAM instruction.
        /// </summary>
        private void input_stream()
        {
            getMachine().selectInputStream(getUnsignedValue(0));
            nextInstruction();
        }

        /// <summary>
        /// RANDOM instruction.
        /// </summary>
        private void random()
        {
            short range = getSignedValue(0);
            storeUnsignedResult(getMachine().random(range));
            nextInstruction();
        }

        /// <summary>
        /// SREAD instruction.
        /// </summary>
        private void sread()
        {
            if (getMachine().getRunState() == MachineRunState.RUNNING)
            {
                sreadStage1();
            }
            else
            {
                sreadStage2();
            }
        }

        /// <summary>
        /// First stage of SREAD.
        /// </summary>
        private void sreadStage1()
        {
            char textbuffer = getUnsignedValue(0);
            getMachine().setRunState(MachineRunState.createReadLine(
                    getReadInterruptTime(), getReadInterruptRoutine(),
                    getNumLeftOverChars(textbuffer), textbuffer));
        }

        /// <summary>
        /// Returns the read interrupt time.
        /// </summary>
        /// <returns>read interrupt time</returns>
        private int getReadInterruptTime()
        {
            return getNumOperands() >= 3 ? getUnsignedValue(2) : 0;
        }

        /// <summary>
        /// Returns the read interrupt routine address.
        /// </summary>
        /// <returns>interrup routine address</returns>
        private char getReadInterruptRoutine()
        {
            return getNumOperands() >= 4 ? getUnsignedValue(3) : (char)0;
        }

        /// <summary>
        /// Returns the number of characters left in the text buffer when timed
        /// input interrupt occurs.
        /// </summary>
        /// <param name="textbuffer">text buffer address</param>
        /// <returns>number of left over characters</returns>
        private int getNumLeftOverChars(char textbuffer)
        {
            return getStoryVersion() >= 5 ?
              getMachine().ReadUnsigned8(textbuffer + 1) : 0;
        }

        /// <summary>
        /// Second stage of SREAD.
        /// </summary>
        private void sreadStage2()
        {
            getMachine().setRunState(MachineRunState.RUNNING);

            int version = getStoryVersion();
            char textbuffer = getUnsignedValue(0);
            char parsebuffer = (char)0;
            if (getNumOperands() >= 2)
            {
                parsebuffer = getUnsignedValue(1);
            }
            // Here the Z-machine needs to be paused and the user interface
            // handles the whole input
            char terminal = getMachine().readLine(textbuffer);

            if (version < 5 || (version >= 5 && parsebuffer > 0))
            {
                // Do not tokenise if parsebuffer is 0 (See specification of read)
                getMachine().tokenize(textbuffer, parsebuffer, 0, false);
            }

            if (storesResult())
            {
                // The specification suggests that we store the terminating character
                // here, this can be NULL or NEWLINE at the moment
                storeUnsignedResult(terminal);
            }
            nextInstruction();
        }

        /// <summary>
        /// SOUND_EFFECT instruction.
        /// </summary>
        private void sound_effect()
        {
            // Choose some default values
            int soundnum = SoundSystem.BLEEP_HIGH;
            int effect = SoundSystem.EFFECT_START;
            int volume = SoundSystem.VOLUME_DEFAULT;
            int repeats = 0;
            int routine = 0;

            // Truly variable
            // If no operands are set, this function will still try to send something
            if (getNumOperands() >= 1)
            {
                soundnum = getUnsignedValue(0);
            }

            if (getNumOperands() >= 2)
            {
                effect = getUnsignedValue(1);
            }

            if (getNumOperands() >= 3)
            {
                int volumeRepeats = getUnsignedValue(2);
                volume = volumeRepeats & 0xff;
                repeats = ((int)((uint)volumeRepeats >> 8)) & 0xff;
                if (repeats <= 0)
                {
                    repeats = 1;
                }
            }

            if (getNumOperands() == 4)
            {
                routine = getUnsignedValue(3);
            }
            LOG.LogInformation(string.Format("@sound_effect n: {0:D}, fx: {1:D}, vol: {2:D}, rep: {3:D}, " + "routine: ${4:x4}\n", soundnum, effect, volume, repeats, routine));

            // In version 3 repeats is always 1
            if (getStoryVersion() == 3)
            {
                repeats = 1;
            }

            ISoundSystem soundSystem = getMachine().getSoundSystem();
            soundSystem.play(soundnum, effect, volume, repeats, routine);
            nextInstruction();
        }

        /// <summary>
        /// SPLIT_WINDOW instruction.
        /// </summary>
        private void split_window()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                screenModel.splitWindow(getUnsignedValue(0));
            }
            nextInstruction();
        }

        /// <summary>
        /// SET_WINDOW instruction.
        /// </summary>
        private void set_window()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                screenModel.setWindow(getUnsignedValue(0));
            }
            nextInstruction();
        }

        /// <summary>
        /// SET_TEXT_STYLE instruction.
        /// </summary>
        private void set_text_style()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                screenModel.setTextStyle(getUnsignedValue(0));
            }
            nextInstruction();
        }

        /// <summary>
        /// BUFFER_MODE instruction.
        /// </summary>
        private void buffer_mode()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                screenModel.setBufferMode(getUnsignedValue(0) > 0);
            }
            nextInstruction();
        }

        /// <summary>
        /// ERASE_WINDOW instruction.
        /// </summary>
        private void erase_window()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                screenModel.eraseWindow(getSignedValue(0));
            }
            nextInstruction();
        }

        /// <summary>
        /// ERASE_LINE instruction.
        /// </summary>
        private void erase_line()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                screenModel.eraseLine(getUnsignedValue(0));
            }
            nextInstruction();
        }

        /// <summary>
        /// SET_CURSOR instruction.
        /// </summary>
        private void set_cursor()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                short line = getSignedValue(0);
                char column = (char)0;
                short window = ScreenModel.CURRENT_WINDOW;

                if (getNumOperands() >= 2)
                {
                    column = getUnsignedValue(1);
                }
                if (getNumOperands() >= 3)
                {
                    window = getSignedValue(2);
                }
                if (line > 0)
                {
                    screenModel.setTextCursor(line, column, window);
                }
            }
            nextInstruction();
        }

        /// <summary>
        /// GET_CURSOR instruction.
        /// </summary>
        private void get_cursor()
        {
            IScreenModel screenModel = getMachine().getScreen();
            if (screenModel != null)
            {
                ITextCursor cursor = screenModel.getTextCursor();
                int arrayAddr = getUnsignedValue(0);
                getMachine().WriteUnsigned16(arrayAddr, (char)cursor.getLine());
                getMachine().WriteUnsigned16(arrayAddr + 2, (char)cursor.getColumn());
            }
            nextInstruction();
        }

        /// <summary>
        /// SCAN_TABLE instruction.
        /// </summary>
        private void scan_table()
        {
            int x = getUnsignedValue(0);
            char table = getUnsignedValue(1);
            int length = getUnsignedValue(2);
            int form = 0x82; // default value
            if (getNumOperands() == 4)
            {
                form = getUnsignedValue(3);
            }
            int fieldlen = form & 0x7f;
            bool isWordTable = (form & 0x80) > 0;
            char pointer = table;
            bool found = false;

            for (int i = 0; i < length; i++)
            {
                int current;
                if (isWordTable)
                {
                    current = getMachine().ReadUnsigned16(pointer);
                    x &= 0xffff;
                }
                else
                {
                    current = getMachine().ReadUnsigned8(pointer);
                    x &= 0xff;
                }
                if (current == x)
                {
                    storeUnsignedResult(pointer);
                    found = true;
                    break;
                }
                pointer += (char)fieldlen;
            }
            // not found
            if (!found)
            {
                storeUnsignedResult((char)0);
            }
            branchOnTest(found);
        }

        /// <summary>
        /// READ_CHAR instruction.
        /// </summary>
        private void read_char()
        {
            if (getMachine().getRunState() == MachineRunState.RUNNING)
            {
                readCharStage1();
            }
            else
            {
                readCharStage2();
            }
        }

        /// <summary>
        /// First stage of READ_CHAR.
        /// </summary>
        private void readCharStage1()
        {
            getMachine().setRunState(MachineRunState.createReadChar(
              getReadCharInterruptTime(), getReadCharInterruptRoutine()));
        }

        /// <summary>
        /// Returns the interrupt time for READ_CHAR timed input.
        /// </summary>
        /// <returns>interrupt time</returns>
        private int getReadCharInterruptTime()
        {
            return getNumOperands() >= 2 ? getUnsignedValue(1) : 0;
        }

        /// <summary>
        /// Returns the address of the interrupt routine for READ_CHAR timed input.
        /// </summary>
        /// <returns>interrupt routine address</returns>
        private char getReadCharInterruptRoutine()
        {
            return getNumOperands() >= 3 ? getUnsignedValue(2) : (char)0;
        }

        /// <summary>
        /// Second stage of READ_CHAR.
        /// </summary>
        private void readCharStage2()
        {
            getMachine().setRunState(MachineRunState.RUNNING);
            storeUnsignedResult(getMachine().readChar());
            nextInstruction();
        }

        /// <summary>
        /// not instruction. Actually a copy from Short1Instruction, probably we
        /// can remove this duplication.
        /// </summary>
        private void not()
        {
            int notvalue = ~getUnsignedValue(0);
            storeUnsignedResult((char)(notvalue & 0xffff));
            nextInstruction();
        }

        /// <summary>
        /// TOKENISE instruction.
        /// </summary>
        private void tokenise()
        {
            int textbuffer = getUnsignedValue(0);
            int parsebuffer = getUnsignedValue(1);
            int dictionary = 0;
            int flag = 0;
            if (getNumOperands() >= 3)
            {
                dictionary = getUnsignedValue(2);
            }
            if (getNumOperands() >= 4)
            {
                flag = getUnsignedValue(3);
            }
            getMachine().tokenize(textbuffer, parsebuffer, dictionary, (flag != 0));
            nextInstruction();
        }

        /// <summary>
        /// CHECK_ARG_COUNT instruction.
        /// </summary>
        private void check_arg_count()
        {
            int argumentNumber = getUnsignedValue(0);
            int currentNumArgs =
              getMachine().getCurrentRoutineContext().getNumArguments();
            branchOnTest(argumentNumber <= currentNumArgs);
        }

        /// <summary>
        /// COPY_TABLE instruction.
        /// </summary>
        private void copy_table()
        {
            int first = getUnsignedValue(0);
            int second = getUnsignedValue(1);
            int size = Math.Abs(getSignedValue(2));
            if (second == 0)
            {
                // Clear size bytes of first
                for (int i = 0; i < size; i++)
                {
                    getMachine().WriteUnsigned8(first + i, (char)0);
                }
            }
            else
            {
                getMachine().CopyArea(first, second, size);
            }
            nextInstruction();
        }

        /// <summary>
        /// Do the print_table instruction. This method takes a text and formats
        /// it in a specified format.It requires access to the cursor position
        /// in order to be implemented correctly, otherwise horizontal home
        /// position would always be set to the left position of the window.
        /// Interestingly, the text is not encoded, so the characters should be
        /// accessed one by one in ZSCII format.
        /// </summary>
        private void print_table()
        {
            int zsciiText = getUnsignedValue(0);
            int width = getUnsignedValue(1);
            int height = 1;
            int skip = 0;
            if (getNumOperands() >= 3)
            {
                height = getUnsignedValue(2);
            }
            if (getNumOperands() == 4)
            {
                skip = getUnsignedValue(3);
            }

            char zchar = (char)0;
            ITextCursor cursor = getMachine().getScreen().getTextCursor();
            int column = cursor.getColumn();
            int row = cursor.getLine();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int offset = (width * i) + j;
                    zchar = (char)getMachine().ReadUnsigned8(zsciiText + offset);
                    getMachine().printZsciiChar(zchar);
                }
                row += skip + 1;
                getMachine().getScreen().setTextCursor(row, column,
                    ScreenModel.CURRENT_WINDOW);
            }
            nextInstruction();
        }

        /// <summary>
        /// ENCODE_TEXT instruction.
        /// </summary>
        private void encode_text()
        {
            int zsciiText = getUnsignedValue(0);
            int length = getUnsignedValue(1);
            int from = getUnsignedValue(2);
            int codedText = getUnsignedValue(3);
            getMachine().encode(zsciiText + from, length, codedText);
            nextInstruction();
        }
    }
}
