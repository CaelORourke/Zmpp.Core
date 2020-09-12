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

namespace Zmpp.Core.Vm
{
    /// <summary>
    /// This interface defines an instruction's public methods.
    /// </summary>
    public static class Instruction
    {
        /// <summary>
        /// The available operand count types.
        /// </summary>
        public enum OperandCount { C0OP, C1OP, C2OP, VAR, EXT }

        /// <summary>
        /// The available instruction forms.
        /// </summary>
        public enum InstructionForm { LONG, SHORT, VARIABLE, EXTENDED }

        // Opcode numbers for 0OP
        public const int C0OP_RTRUE = 0x00;
        public const int C0OP_RFALSE = 0x01;
        public const int C0OP_PRINT = 0x02;
        public const int C0OP_PRINT_RET = 0x03;
        public const int C0OP_NOP = 0x04;
        public const int C0OP_SAVE = 0x05;
        public const int C0OP_RESTORE = 0x06;
        public const int C0OP_RESTART = 0x07;
        public const int C0OP_RET_POPPED = 0x08;
        public const int C0OP_POP = 0x09; // Versions 1-4
        public const int C0OP_CATCH = 0x09; // Versions 5-8
        public const int C0OP_QUIT = 0x0a;
        public const int C0OP_NEW_LINE = 0x0b;
        public const int C0OP_SHOW_STATUS = 0x0c;
        public const int C0OP_VERIFY = 0x0d;
        public const int C0OP_PIRACY = 0x0f;

        // Opcode numbers for 1OP
        public const int C1OP_JZ = 0x00;
        public const int C1OP_GET_SIBLING = 0x01;
        public const int C1OP_GET_CHILD = 0x02;
        public const int C1OP_GET_PARENT = 0x03;
        public const int C1OP_GET_PROP_LEN = 0x04;
        public const int C1OP_INC = 0x05;
        public const int C1OP_DEC = 0x06;
        public const int C1OP_PRINT_ADDR = 0x07;
        public const int C1OP_CALL_1S = 0x08;
        public const int C1OP_REMOVE_OBJ = 0x09;
        public const int C1OP_PRINT_OBJ = 0x0a;
        public const int C1OP_RET = 0x0b;
        public const int C1OP_JUMP = 0x0c;
        public const int C1OP_PRINT_PADDR = 0x0d;
        public const int C1OP_LOAD = 0x0e;
        public const int C1OP_NOT = 0x0f; // Versions 1-4
        public const int C1OP_CALL_1N = 0x0f; // Versions >= 5

        // Opcode numbers for 2OP
        public const int C2OP_JE = 0x01;
        public const int C2OP_JL = 0x02;
        public const int C2OP_JG = 0x03;
        public const int C2OP_DEC_CHK = 0x04;
        public const int C2OP_INC_CHK = 0x05;
        public const int C2OP_JIN = 0x06;
        public const int C2OP_TEST = 0x07;
        public const int C2OP_OR = 0x08;
        public const int C2OP_AND = 0x09;
        public const int C2OP_TEST_ATTR = 0x0a;
        public const int C2OP_SET_ATTR = 0x0b;
        public const int C2OP_CLEAR_ATTR = 0x0c;
        public const int C2OP_STORE = 0x0d;
        public const int C2OP_INSERT_OBJ = 0x0e;
        public const int C2OP_LOADW = 0x0f;
        public const int C2OP_LOADB = 0x10;
        public const int C2OP_GET_PROP = 0x11;
        public const int C2OP_GET_PROP_ADDR = 0x12;
        public const int C2OP_GET_NEXT_PROP = 0x13;
        public const int C2OP_ADD = 0x14;
        public const int C2OP_SUB = 0x15;
        public const int C2OP_MUL = 0x16;
        public const int C2OP_DIV = 0x17;
        public const int C2OP_MOD = 0x18;
        public const int C2OP_CALL_2S = 0x19;
        public const int C2OP_CALL_2N = 0x1a;
        public const int C2OP_SET_COLOUR = 0x1b;
        public const int C2OP_THROW = 0x1c;

        // Opcode numbers for VAR
        public const int VAR_CALL = 0x00; // Versions 1-3
        public const int VAR_CALL_VS = 0x00; // Versions 4-8
        public const int VAR_STOREW = 0x01;
        public const int VAR_STOREB = 0x02;
        public const int VAR_PUT_PROP = 0x03;
        public const int VAR_SREAD = 0x04; // Versions 1-4
        public const int VAR_AREAD = 0x04; // Versions >= 5
        public const int VAR_PRINT_CHAR = 0x05;
        public const int VAR_PRINT_NUM = 0x06;
        public const int VAR_RANDOM = 0x07;
        public const int VAR_PUSH = 0x08;
        public const int VAR_PULL = 0x09;
        public const int VAR_SPLIT_WINDOW = 0x0a;
        public const int VAR_SET_WINDOW = 0x0b;
        public const int VAR_CALL_VS2 = 0x0c;
        public const int VAR_ERASE_WINDOW = 0x0d;
        public const int VAR_ERASE_LINE = 0x0e;
        public const int VAR_SET_CURSOR = 0x0f;
        public const int VAR_GET_CURSOR = 0x10;
        public const int VAR_SET_TEXT_STYLE = 0x11;
        public const int VAR_BUFFER_MODE = 0x12;
        public const int VAR_OUTPUT_STREAM = 0x13;
        public const int VAR_INPUT_STREAM = 0x14;
        public const int VAR_SOUND_EFFECT = 0x15;
        public const int VAR_READ_CHAR = 0x16;
        public const int VAR_SCAN_TABLE = 0x17;
        public const int VAR_NOT = 0x18; // Versions >= 5
        public const int VAR_CALL_VN = 0x19; // Versions >= 5
        public const int VAR_CALL_VN2 = 0x1a; // Versions >= 5
        public const int VAR_TOKENISE = 0x1b; // Versions >= 5
        public const int VAR_ENCODE_TEXT = 0x1c; // Versions >= 5
        public const int VAR_COPY_TABLE = 0x1d;
        public const int VAR_PRINT_TABLE = 0x1e;
        public const int VAR_CHECK_ARG_COUNT = 0x1f; // Versions >= 5

        // Opcode numbers for EXT
        public const int EXT_SAVE = 0x00;
        public const int EXT_RESTORE = 0x01;
        public const int EXT_LOG_SHIFT = 0x02;
        public const int EXT_ART_SHIFT = 0x03;
        public const int EXT_SET_FONT = 0x04;
        public const int EXT_DRAW_PICTURE = 0x05;
        public const int EXT_PICTURE_DATA = 0x06;
        public const int EXT_ERASE_PICTURE = 0x07;
        public const int EXT_SET_MARGINS = 0x08;
        public const int EXT_SAVE_UNDO = 0x09;
        public const int EXT_RESTORE_UNDO = 0x0a;
        public const int EXT_PRINT_UNICODE = 0x0b;
        public const int EXT_CHECK_UNICODE = 0x0c;
        public const int EXT_MOVE_WINDOW = 0x10;
        public const int EXT_WINDOW_SIZE = 0x11;
        public const int EXT_WINDOW_STYLE = 0x12;
        public const int EXT_GET_WIND_PROP = 0x13;
        public const int EXT_SCROLL_WINDOW = 0x14;
        public const int EXT_POP_STACK = 0x15;
        public const int EXT_READ_MOUSE = 0x16;
        public const int EXT_MOUSE_WINDOW = 0x17;
        public const int EXT_PUSH_STACK = 0x18;
        public const int EXT_PUT_WIND_PROP = 0x19;
        public const int EXT_PRINT_FORM = 0x1a;
        public const int EXT_MAKE_MENU = 0x1b;
        public const int EXT_PICTURE_TABLE = 0x1c;

        /// <summary>
        /// The constant for false.
        /// </summary>
        public const char FALSE = (char)0;

        /// <summary>
        /// The constant for true.
        /// </summary>
        public const char TRUE = (char)1;

        /// <summary>
        /// The constant for true from restore.
        /// </summary>
        public const char RESTORE_TRUE = (char)2;
    }
}
