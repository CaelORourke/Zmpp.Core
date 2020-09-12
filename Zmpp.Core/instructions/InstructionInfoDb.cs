/*
 * Created on 2008/07/23
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

namespace org.zmpp.instructions
{
    using org.zmpp.vm;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This is the new representation for the information about instructions in
    /// the Z-machine.As opposed to the old xStaticInfo classes, this is a database
    /// containing all the information. It can be regarded as static configuration
    /// which is compiled into the application.
    /// </summary>
    public class InstructionInfoDb
    {
        // Commonly used version ranges
        private static int[] ALL_VERSIONS = { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static int[] EXCEPT_V6 = { 1, 2, 3, 4, 5, 7, 8 };
        private static int[] V1_TO_V3 = { 1, 2, 3 };
        private static int[] V1_TO_V4 = { 1, 2, 3, 4 };
        private static int[] V5_TO_V8 = { 5, 6, 7, 8 };
        private static int[] V3_TO_V8 = { 3, 4, 5, 6, 7, 8 };
        private static int[] V4_TO_V8 = { 4, 5, 6, 7, 8 };
        private static int[] V4 = { 4 };
        private static int[] V6 = { 6 };

        /// <summary>
        /// Information structure about the instruction.
        /// </summary>
        public class InstructionInfo
        {
            private String name;
            private bool _isStore, _isBranch, _isPrint, _isOutput;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">name</param>
            /// <param name="isBranch">branch flag</param>
            /// <param name="isStore">store flag</param>
            /// <param name="isPrint">print flag</param>
            /// <param name="isOutput">output flag</param>
            public InstructionInfo(String name, bool isBranch, bool isStore, bool isPrint, bool isOutput)
            {
                this.name = name;
                this._isBranch = isBranch;
                this._isStore = isStore;
                this._isPrint = isPrint;
                this._isOutput = isOutput;
            }

            /// <summary>
            /// Determine whether this InstructionInfo represents a store.
            /// </summary>
            /// <returns>true for store, false if not</returns>
            public bool isStore() { return _isStore; }

            /// <summary>
            /// Determine whether this InstructionInfo represents a branch.
            /// </summary>
            /// <returns>true for branch, false if not</returns>
            public bool isBranch() { return _isBranch; }

            /// <summary>
            /// Determine whether this InstructionInfo represents a print instruction.
            /// </summary>
            /// <returns>true for print, false if not</returns>
            public bool isPrint() { return _isPrint; }

            /// <summary>
            /// Determine whether this InstructionInfo represents an output instruction.
            /// </summary>
            /// <returns>true for output, false if not</returns>
            public bool isOutput() { return _isOutput; }

            /// <summary>
            /// Returns the opcode name.
            /// </summary>
            /// <returns>opcode name</returns>
            public String getName() { return name; }
        }

        // Factory methods to create the common InstructionInfo types

        /// <summary>
        /// Creates standard InstructionInfo object.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>InstructionInfo object</returns>
        private InstructionInfo createInfo(String name)
        {
            return new InstructionInfo(name, false, false, false, false);
        }

        /// <summary>
        /// Creates branch-and-store InstructionInfo object.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>InstructionInfo object</returns>
        private InstructionInfo createBranchAndStore(String name)
        {
            return new InstructionInfo(name, true, true, false, false);
        }

        /// <summary>
        /// Creates store InstructionInfo object.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>InstructionInfo object</returns>
        private InstructionInfo createStore(String name)
        {
            return new InstructionInfo(name, false, true, false, false);
        }

        /// <summary>
        /// Creates branch InstructionInfo object.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>InstructionInfo object</returns>
        private InstructionInfo createBranch(String name)
        {
            return new InstructionInfo(name, true, false, false, false);
        }

        /// <summary>
        /// Creates print InstructionInfo object.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>InstructionInfo object</returns>
        private InstructionInfo createPrint(String name)
        {
            return new InstructionInfo(name, false, false, true, true);
        }

        /// <summary>
        /// Creates output InstructionInfo object.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>InstructionInfo object</returns>
        private InstructionInfo createOutput(String name)
        {
            return new InstructionInfo(name, false, false, false, true);
        }

        /// <summary>
        /// The hashmap to represent the database
        /// </summary>
        private Dictionary<String, InstructionInfo> infoMap = new Dictionary<string, InstructionInfo>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private InstructionInfoDb()
        {
            // 0OP
            addInfoForAll(createInfo("RTRUE"), Instruction.OperandCount.C0OP, Instruction.C0OP_RTRUE);
            addInfoForAll(createInfo("RFALSE"), Instruction.OperandCount.C0OP, Instruction.C0OP_RFALSE);
            addInfoForAll(createPrint("PRINT"), Instruction.OperandCount.C0OP, Instruction.C0OP_PRINT);
            addInfoForAll(createPrint("PRINT_RET"), Instruction.OperandCount.C0OP, Instruction.C0OP_PRINT_RET);
            addInfoForAll(createInfo("NOP"), Instruction.OperandCount.C0OP, Instruction.C0OP_NOP);
            addInfoFor(createBranch("SAVE"), Instruction.OperandCount.C0OP, Instruction.C0OP_SAVE, V1_TO_V3);
            addInfoFor(createBranch("RESTORE"), Instruction.OperandCount.C0OP, Instruction.C0OP_RESTORE, V1_TO_V3);
            addInfoFor(createStore("SAVE"), Instruction.OperandCount.C0OP, Instruction.C0OP_SAVE, V4);
            addInfoFor(createStore("RESTORE"), Instruction.OperandCount.C0OP, Instruction.C0OP_RESTORE, V4);
            addInfoForAll(createInfo("RESTART"), Instruction.OperandCount.C0OP, Instruction.C0OP_RESTART);
            addInfoForAll(createInfo("RET_POPPED"), Instruction.OperandCount.C0OP, Instruction.C0OP_RET_POPPED);
            addInfoFor(createInfo("POP"), Instruction.OperandCount.C0OP, Instruction.C0OP_POP, V1_TO_V4);
            addInfoFor(createStore("CATCH"), Instruction.OperandCount.C0OP, Instruction.C0OP_CATCH, V5_TO_V8);
            addInfoForAll(createInfo("QUIT"), Instruction.OperandCount.C0OP, Instruction.C0OP_QUIT);
            addInfoForAll(createOutput("NEW_LINE"), Instruction.OperandCount.C0OP, Instruction.C0OP_NEW_LINE);
            addInfoFor(createInfo("SHOW_STATUS"), Instruction.OperandCount.C0OP, Instruction.C0OP_SHOW_STATUS, new int[] { 3 });
            addInfoFor(createBranch("VERIFY"), Instruction.OperandCount.C0OP, Instruction.C0OP_VERIFY, new int[] { 3, 4, 5, 6, 7, 8 });
            addInfoFor(createInfo("PIRACY"), Instruction.OperandCount.C0OP, Instruction.C0OP_PIRACY, V5_TO_V8);

            // 1OP
            addInfoForAll(createBranch("JZ"), Instruction.OperandCount.C1OP, Instruction.C1OP_JZ);
            addInfoForAll(createBranchAndStore("GET_SIBLING"), Instruction.OperandCount.C1OP, Instruction.C1OP_GET_SIBLING);
            addInfoForAll(createBranchAndStore("GET_CHILD"), Instruction.OperandCount.C1OP, Instruction.C1OP_GET_CHILD);
            addInfoForAll(createStore("GET_PARENT"), Instruction.OperandCount.C1OP, Instruction.C1OP_GET_PARENT);
            addInfoForAll(createStore("GET_PROP_LEN"), Instruction.OperandCount.C1OP, Instruction.C1OP_GET_PROP_LEN);
            addInfoForAll(createInfo("INC"), Instruction.OperandCount.C1OP, Instruction.C1OP_INC);
            addInfoForAll(createInfo("DEC"), Instruction.OperandCount.C1OP, Instruction.C1OP_DEC);
            addInfoForAll(createOutput("PRINT_ADDR"), Instruction.OperandCount.C1OP, Instruction.C1OP_PRINT_ADDR);
            addInfoFor(createStore("CALL_1S"), Instruction.OperandCount.C1OP, Instruction.C1OP_CALL_1S, V4_TO_V8);
            addInfoForAll(createInfo("REMOVE_OBJ"), Instruction.OperandCount.C1OP, Instruction.C1OP_REMOVE_OBJ);
            addInfoForAll(createOutput("PRINT_OBJ"), Instruction.OperandCount.C1OP, Instruction.C1OP_PRINT_OBJ);
            addInfoForAll(createInfo("RET"), Instruction.OperandCount.C1OP, Instruction.C1OP_RET);
            addInfoForAll(createInfo("JUMP"), Instruction.OperandCount.C1OP, Instruction.C1OP_JUMP);
            addInfoForAll(createOutput("PRINT_PADDR"), Instruction.OperandCount.C1OP, Instruction.C1OP_PRINT_PADDR);
            addInfoForAll(createStore("LOAD"), Instruction.OperandCount.C1OP, Instruction.C1OP_LOAD);
            addInfoFor(createStore("NOT"), Instruction.OperandCount.C1OP, Instruction.C1OP_NOT, V1_TO_V4);
            addInfoFor(createInfo("CALL_1N"), Instruction.OperandCount.C1OP, Instruction.C1OP_CALL_1N, V5_TO_V8);

            // 2OP
            addInfoForAll(createBranch("JE"), Instruction.OperandCount.C2OP, Instruction.C2OP_JE);
            addInfoForAll(createBranch("JL"), Instruction.OperandCount.C2OP, Instruction.C2OP_JL);
            addInfoForAll(createBranch("JG"), Instruction.OperandCount.C2OP, Instruction.C2OP_JG);
            addInfoForAll(createBranch("DEC_CHK"), Instruction.OperandCount.C2OP, Instruction.C2OP_DEC_CHK);
            addInfoForAll(createBranch("INC_CHK"), Instruction.OperandCount.C2OP, Instruction.C2OP_INC_CHK);
            addInfoForAll(createBranch("JIN"), Instruction.OperandCount.C2OP, Instruction.C2OP_JIN);
            addInfoForAll(createBranch("TEST"), Instruction.OperandCount.C2OP, Instruction.C2OP_TEST);
            addInfoForAll(createStore("OR"), Instruction.OperandCount.C2OP, Instruction.C2OP_OR);
            addInfoForAll(createStore("AND"), Instruction.OperandCount.C2OP, Instruction.C2OP_AND);
            addInfoForAll(createBranch("TEST_ATTR"), Instruction.OperandCount.C2OP, Instruction.C2OP_TEST_ATTR);
            addInfoForAll(createInfo("SET_ATTR"), Instruction.OperandCount.C2OP, Instruction.C2OP_SET_ATTR);
            addInfoForAll(createInfo("CLEAR_ATTR"), Instruction.OperandCount.C2OP, Instruction.C2OP_CLEAR_ATTR);
            addInfoForAll(createInfo("STORE"), Instruction.OperandCount.C2OP, Instruction.C2OP_STORE);
            addInfoForAll(createInfo("INSERT_OBJ"), Instruction.OperandCount.C2OP, Instruction.C2OP_INSERT_OBJ);
            addInfoForAll(createStore("LOADW"), Instruction.OperandCount.C2OP, Instruction.C2OP_LOADW);
            addInfoForAll(createStore("LOADB"), Instruction.OperandCount.C2OP, Instruction.C2OP_LOADB);
            addInfoForAll(createStore("GET_PROP"), Instruction.OperandCount.C2OP, Instruction.C2OP_GET_PROP);
            addInfoForAll(createStore("GET_PROP_ADDR"), Instruction.OperandCount.C2OP, Instruction.C2OP_GET_PROP_ADDR);
            addInfoForAll(createStore("GET_NEXT_PROP"), Instruction.OperandCount.C2OP, Instruction.C2OP_GET_NEXT_PROP);
            addInfoForAll(createStore("ADD"), Instruction.OperandCount.C2OP, Instruction.C2OP_ADD);
            addInfoForAll(createStore("SUB"), Instruction.OperandCount.C2OP, Instruction.C2OP_SUB);
            addInfoForAll(createStore("MUL"), Instruction.OperandCount.C2OP, Instruction.C2OP_MUL);
            addInfoForAll(createStore("DIV"), Instruction.OperandCount.C2OP, Instruction.C2OP_DIV);
            addInfoForAll(createStore("MOD"), Instruction.OperandCount.C2OP, Instruction.C2OP_MOD);
            addInfoFor(createStore("CALL_2S"), Instruction.OperandCount.C2OP, Instruction.C2OP_CALL_2S, V4_TO_V8);
            addInfoFor(createInfo("CALL_2N"), Instruction.OperandCount.C2OP, Instruction.C2OP_CALL_2N, V5_TO_V8);
            addInfoFor(createInfo("SET_COLOUR"), Instruction.OperandCount.C2OP, Instruction.C2OP_SET_COLOUR, V5_TO_V8);
            addInfoFor(createInfo("THROW"), Instruction.OperandCount.C2OP, Instruction.C2OP_THROW, V5_TO_V8);

            // VAR
            addInfoFor(createStore("CALL"), Instruction.OperandCount.VAR, Instruction.VAR_CALL, V1_TO_V3);
            addInfoFor(createStore("CALL_VS"), Instruction.OperandCount.VAR, Instruction.VAR_CALL_VS, V4_TO_V8);
            addInfoForAll(createInfo("STOREW"), Instruction.OperandCount.VAR, Instruction.VAR_STOREW);
            addInfoForAll(createInfo("STOREB"), Instruction.OperandCount.VAR, Instruction.VAR_STOREB);
            addInfoForAll(createInfo("PUT_PROP"), Instruction.OperandCount.VAR, Instruction.VAR_PUT_PROP);
            addInfoFor(createInfo("SREAD"), Instruction.OperandCount.VAR, Instruction.VAR_SREAD, V1_TO_V4);
            addInfoFor(createStore("AREAD"), Instruction.OperandCount.VAR, Instruction.VAR_AREAD, V5_TO_V8);
            addInfoForAll(createOutput("PRINT_CHAR"), Instruction.OperandCount.VAR, Instruction.VAR_PRINT_CHAR);
            addInfoForAll(createOutput("PRINT_NUM"), Instruction.OperandCount.VAR, Instruction.VAR_PRINT_NUM);
            addInfoForAll(createStore("RANDOM"), Instruction.OperandCount.VAR, Instruction.VAR_RANDOM);
            addInfoForAll(createInfo("PUSH"), Instruction.OperandCount.VAR, Instruction.VAR_PUSH);
            addInfoFor(createInfo("PULL"), Instruction.OperandCount.VAR, Instruction.VAR_PULL, EXCEPT_V6);
            addInfoFor(createStore("PULL"), Instruction.OperandCount.VAR, Instruction.VAR_PULL, V6);
            addInfoFor(createOutput("SPLIT_WINDOW"), Instruction.OperandCount.VAR, Instruction.VAR_SPLIT_WINDOW, V3_TO_V8);
            addInfoFor(createInfo("SET_WINDOW"), Instruction.OperandCount.VAR, Instruction.VAR_SET_WINDOW, V3_TO_V8);
            addInfoFor(createStore("CALL_VS2"), Instruction.OperandCount.VAR, Instruction.VAR_CALL_VS2, V4_TO_V8);
            addInfoFor(createOutput("ERASE_WINDOW"), Instruction.OperandCount.VAR, Instruction.VAR_ERASE_WINDOW, V4_TO_V8);
            addInfoFor(createOutput("ERASE_LINE"), Instruction.OperandCount.VAR, Instruction.VAR_ERASE_LINE, V4_TO_V8);
            addInfoFor(createInfo("SET_CURSOR"), Instruction.OperandCount.VAR, Instruction.VAR_SET_CURSOR, V4_TO_V8);
            addInfoFor(createInfo("GET_CURSOR"), Instruction.OperandCount.VAR, Instruction.VAR_GET_CURSOR, V4_TO_V8);
            addInfoFor(createInfo("SET_TEXT_STYLE"), Instruction.OperandCount.VAR, Instruction.VAR_SET_TEXT_STYLE, V4_TO_V8);
            addInfoFor(createInfo("BUFFER_MODE"), Instruction.OperandCount.VAR, Instruction.VAR_BUFFER_MODE, V4_TO_V8);
            addInfoFor(createInfo("OUTPUT_STREAM"), Instruction.OperandCount.VAR, Instruction.VAR_OUTPUT_STREAM, V3_TO_V8);
            addInfoFor(createInfo("INPUT_STREAM"), Instruction.OperandCount.VAR, Instruction.VAR_INPUT_STREAM, V3_TO_V8);
            addInfoFor(createInfo("SOUND_EFFECT"), Instruction.OperandCount.VAR, Instruction.VAR_SOUND_EFFECT, V3_TO_V8);
            addInfoFor(createStore("READ_CHAR"), Instruction.OperandCount.VAR, Instruction.VAR_READ_CHAR, V4_TO_V8);
            addInfoFor(createBranchAndStore("SCAN_TABLE"), Instruction.OperandCount.VAR, Instruction.VAR_SCAN_TABLE, V4_TO_V8);
            addInfoFor(createStore("NOT"), Instruction.OperandCount.VAR, Instruction.VAR_NOT, V5_TO_V8);
            addInfoFor(createInfo("CALL_VN"), Instruction.OperandCount.VAR, Instruction.VAR_CALL_VN, V5_TO_V8);
            addInfoFor(createInfo("CALL_VN2"), Instruction.OperandCount.VAR, Instruction.VAR_CALL_VN2, V5_TO_V8);
            addInfoFor(createInfo("TOKENISE"), Instruction.OperandCount.VAR, Instruction.VAR_TOKENISE, V5_TO_V8);
            addInfoFor(createInfo("ENCODE_TEXT"), Instruction.OperandCount.VAR, Instruction.VAR_ENCODE_TEXT, V5_TO_V8);
            addInfoFor(createInfo("COPY_TABLE"), Instruction.OperandCount.VAR, Instruction.VAR_COPY_TABLE, V5_TO_V8);
            addInfoFor(createOutput("PRINT_TABLE"), Instruction.OperandCount.VAR, Instruction.VAR_PRINT_TABLE, V5_TO_V8);
            addInfoFor(createBranch("CHECK_ARG_COUNT"), Instruction.OperandCount.VAR, Instruction.VAR_CHECK_ARG_COUNT, V5_TO_V8);

            // EXT
            addInfoFor(createStore("SAVE"), Instruction.OperandCount.EXT, Instruction.EXT_SAVE, V5_TO_V8);
            addInfoFor(createStore("RESTORE"), Instruction.OperandCount.EXT, Instruction.EXT_RESTORE, V5_TO_V8);
            addInfoFor(createStore("LOG_SHIFT"), Instruction.OperandCount.EXT, Instruction.EXT_LOG_SHIFT, V5_TO_V8);
            addInfoFor(createStore("ART_SHIFT"), Instruction.OperandCount.EXT, Instruction.EXT_ART_SHIFT, V5_TO_V8);
            addInfoFor(createStore("SET_FONT"), Instruction.OperandCount.EXT, Instruction.EXT_SET_FONT, V5_TO_V8);
            addInfoFor(createOutput("DRAW_PICTURE"), Instruction.OperandCount.EXT, Instruction.EXT_DRAW_PICTURE, V6);
            addInfoFor(createBranch("PICTURE_DATA"), Instruction.OperandCount.EXT, Instruction.EXT_PICTURE_DATA, V6);
            addInfoFor(createOutput("ERASE_PICTURE"), Instruction.OperandCount.EXT, Instruction.EXT_ERASE_PICTURE, V6);
            addInfoFor(createInfo("SET_MARGINS"), Instruction.OperandCount.EXT, Instruction.EXT_SET_MARGINS, V6);
            addInfoFor(createStore("SAVE_UNDO"), Instruction.OperandCount.EXT, Instruction.EXT_SAVE_UNDO, V5_TO_V8);
            addInfoFor(createStore("RESTORE_UNDO"), Instruction.OperandCount.EXT, Instruction.EXT_RESTORE_UNDO, V5_TO_V8);
            addInfoFor(createOutput("PRINT_UNICODE"), Instruction.OperandCount.EXT, Instruction.EXT_PRINT_UNICODE, V5_TO_V8);
            addInfoFor(createInfo("CHECK_UNICODE"), Instruction.OperandCount.EXT, Instruction.EXT_CHECK_UNICODE, V5_TO_V8);
            addInfoFor(createOutput("MOVE_WINDOW"), Instruction.OperandCount.EXT, Instruction.EXT_MOVE_WINDOW, V6);
            addInfoFor(createInfo("WINDOW_SIZE"), Instruction.OperandCount.EXT, Instruction.EXT_WINDOW_SIZE, V6);
            addInfoFor(createInfo("WINDOW_STYLE"), Instruction.OperandCount.EXT, Instruction.EXT_WINDOW_STYLE, V6);
            addInfoFor(createStore("GET_WIND_PROP"), Instruction.OperandCount.EXT, Instruction.EXT_GET_WIND_PROP, V6);
            addInfoFor(createOutput("SCROLL_WINDOW"), Instruction.OperandCount.EXT, Instruction.EXT_SCROLL_WINDOW, V6);
            addInfoFor(createInfo("POP_STACK"), Instruction.OperandCount.EXT, Instruction.EXT_POP_STACK, V6);
            addInfoFor(createInfo("READ_MOUSE"), Instruction.OperandCount.EXT, Instruction.EXT_READ_MOUSE, V6);
            addInfoFor(createInfo("MOUSE_WINDOW"), Instruction.OperandCount.EXT, Instruction.EXT_MOUSE_WINDOW, V6);
            addInfoFor(createBranch("PUSH_STACK"), Instruction.OperandCount.EXT, Instruction.EXT_PUSH_STACK, V6);
            addInfoFor(createInfo("PUT_WIND_PROP"), Instruction.OperandCount.EXT, Instruction.EXT_PUT_WIND_PROP, V6);
            addInfoFor(createOutput("PRINT_FORM"), Instruction.OperandCount.EXT, Instruction.EXT_PRINT_FORM, V6);
            addInfoFor(createBranch("MAKE_MENU"), Instruction.OperandCount.EXT, Instruction.EXT_MAKE_MENU, V6);
            addInfoFor(createInfo("PICTURE_TABLE"), Instruction.OperandCount.EXT, Instruction.EXT_PICTURE_TABLE, V6);
        }

        /// <summary>
        /// Adds the specified info struct for all Z-machine versions.
        /// </summary>
        /// <param name="info">the InstructionInfo</param>
        /// <param name="opCount">the OperandCount</param>
        /// <param name="opcodeNum">the opcode number</param>
        private void addInfoForAll(InstructionInfo info, Instruction.OperandCount opCount, int opcodeNum)
        {
            addInfoFor(info, opCount, opcodeNum, ALL_VERSIONS);
        }

        /// <summary>
        /// Adds the specified InstructionInfo for the specified Z-machine versions.
        /// </summary>
        /// <param name="info">the InstructionInfo</param>
        /// <param name="opCount">the OperandCount</param>
        /// <param name="opcodeNum">the opcode number</param>
        /// <param name="versions">the valid versions</param>
        private void addInfoFor(InstructionInfo info, Instruction.OperandCount opCount, int opcodeNum, int[] versions)
        {
            foreach (int version in versions)
            {
                infoMap[createKey(opCount, opcodeNum, version)] = info;
            }
        }

        private static InstructionInfoDb instance = new InstructionInfoDb();

        /// <summary>
        /// Returns the Singleton instance of the database.
        /// </summary>
        /// <returns>the database instance</returns>
        public static InstructionInfoDb getInstance() { return instance; }

        /// <summary>
        /// Creates the hash key for the specified instruction information.
        /// </summary>
        /// <param name="opCount">the operand count</param>
        /// <param name="opcodeNum">the opcode number</param>
        /// <param name="version">the story version</param>
        /// <returns>the key</returns>
        private String createKey(Instruction.OperandCount opCount, int opcodeNum, int version)
        {
            return opCount.ToString() + ":" + opcodeNum + ":" + version;
        }

        /// <summary>
        /// Returns the information struct for the specified instruction.
        /// </summary>
        /// <param name="opCount">the operand count</param>
        /// <param name="opcodeNum">the opcode number</param>
        /// <param name="version">the story version</param>
        /// <returns>the instruction info struct</returns>
        public InstructionInfo getInfo(Instruction.OperandCount opCount, int opcodeNum, int version)
        {
            //System.out.println("GENERATING KEY: " + createKey(opCount, opcodeNum, version));
            return infoMap[createKey(opCount, opcodeNum, version)];
        }

        /// <summary>
        /// Determines if the specified operation is valid.
        /// </summary>
        /// <param name="opCount">the operand count</param>
        /// <param name="opcodeNum">the opcode number</param>
        /// <param name="version">the story version</param>
        /// <returns>true if valid, false otherwise</returns>
        public bool isValid(Instruction.OperandCount opCount, int opcodeNum, int version)
        {
            return infoMap.ContainsKey(createKey(opCount, opcodeNum, version));
        }

        /// <summary>
        /// Prints the keys in the info map.
        /// </summary>
        public void printKeys()
        {
            Console.Out.WriteLine("INFO MAP KEYS: ");
            foreach (String key in infoMap.Keys)
            {
                if (key.StartsWith("C1OP:0")) Console.Out.WriteLine(key);
            }
        }
    }
}
