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
    using Zmpp.Core.Media;
    using Zmpp.Core.Vm;
    using Zmpp.Core.UI;
    using static Zmpp.Core.MemoryUtil;
    using static Zmpp.Core.Vm.Instruction;

    /// <summary>
    /// Implementation of instructions with EXT operand count.
    /// </summary>
    public class ExtInstruction : AbstractInstruction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">Machine object</param>
        /// <param name="opcodeNum">opcode number</param>
        /// <param name="operands">operands</param>
        /// <param name="storeVar">store variable</param>
        /// <param name="branchInfo">branch information</param>
        /// <param name="opcodeLength">opcode length</param>
        public ExtInstruction(IMachine machine, int opcodeNum, Operand[] operands, char storeVar, BranchInfo branchInfo, int opcodeLength) : base(machine, opcodeNum, operands, storeVar, branchInfo, opcodeLength)
        {
        }

        protected override OperandCount getOperandCount() { return OperandCount.EXT; }

        public override void Execute()
        {
            switch (getOpcodeNum())
            {
                case EXT_SAVE:
                    save();
                    break;
                case EXT_RESTORE:
                    restore();
                    break;
                case EXT_LOG_SHIFT:
                    log_shift();
                    break;
                case EXT_ART_SHIFT:
                    art_shift();
                    break;
                case EXT_SET_FONT:
                    set_font();
                    break;
                case EXT_SAVE_UNDO:
                    save_undo();
                    break;
                case EXT_RESTORE_UNDO:
                    restore_undo();
                    break;
                case EXT_PRINT_UNICODE:
                    print_unicode();
                    break;
                case EXT_CHECK_UNICODE:
                    check_unicode();
                    break;
                case EXT_MOUSE_WINDOW:
                    mouse_window();
                    break;
                case EXT_PICTURE_DATA:
                    picture_data();
                    break;
                case EXT_DRAW_PICTURE:
                    draw_picture();
                    break;
                case EXT_ERASE_PICTURE:
                    erase_picture();
                    break;
                case EXT_MOVE_WINDOW:
                    move_window();
                    break;
                case EXT_WINDOW_SIZE:
                    window_size();
                    break;
                case EXT_WINDOW_STYLE:
                    window_style();
                    break;
                case EXT_SET_MARGINS:
                    set_margins();
                    break;
                case EXT_GET_WIND_PROP:
                    get_wind_prop();
                    break;
                case EXT_PICTURE_TABLE:
                    picture_table();
                    break;
                case EXT_PUT_WIND_PROP:
                    put_wind_prop();
                    break;
                case EXT_PUSH_STACK:
                    push_stack();
                    break;
                case EXT_POP_STACK:
                    pop_stack();
                    break;
                case EXT_READ_MOUSE:
                    read_mouse();
                    break;
                case EXT_SCROLL_WINDOW:
                    scroll_window();
                    break;
                default:
                    throwInvalidOpcode();
                    break;
            }
        }

        /// <summary>
        /// SAVE_UNDO instruction.
        /// </summary>
        private void save_undo()
        {
            // Target PC offset is two because of the extra opcode byte and
            // operand type byte compared to the 0OP instruction
            int pc = getMachine().PC + 3;
            bool success = getMachine().SaveUndo(pc);
            storeUnsignedResult(success ? TRUE : FALSE);
            nextInstruction();
        }

        /// <summary>
        /// RESTORE_UNDO instruction.
        /// </summary>
        private void restore_undo()
        {
            PortableGameState gamestate = getMachine().RestoreUndo();
            if (gamestate == null)
            {
                storeUnsignedResult(FALSE);
                nextInstruction();
            }
            else
            {
                char storevar = gamestate.getStoreVariable(getMachine());
                getMachine().SetVariable(storevar, RESTORE_TRUE);
            }
        }

        /// <summary>
        /// ART_SHIFT instruction.
        /// </summary>
        private void art_shift()
        {
            short number = getSignedValue(0);
            short places = getSignedValue(1);
            number = (short)((places >= 0) ? number << places : number >> (-places));
            storeUnsignedResult(SignedToUnsigned16(number));
            nextInstruction();
        }

        /// <summary>
        /// LOG_SHIFT instruction.
        /// </summary>
        private void log_shift()
        {
            char number = getUnsignedValue(0);
            short places = getSignedValue(1);
            number = (char)((places >= 0) ? number << places : (int)((uint)number >> (-places)));
            storeUnsignedResult(number);
            nextInstruction();
        }

        /// <summary>
        /// SET_FONT instruction.
        /// </summary>
        private void set_font()
        {
            char previousFont =
              getMachine().Screen.SetFont(getUnsignedValue(0));
            storeUnsignedResult(previousFont);
            nextInstruction();
        }

        /// <summary>
        /// SAVE instruction.
        /// </summary>
        private void save()
        {
            // Saving to tables is not supported yet, this is the standard save feature
            // Offset is 3 because there are two opcode bytes + 1 optype byte before
            // the actual store var byte
            saveToStorage(getMachine().PC + 3);
        }

        /// <summary>
        /// RESTORE instruction.
        /// </summary>
        private void restore()
        {
            // Reading from tables is not supported yet, this is the standard
            // restore feature
            restoreFromStorage();
        }

        /// <summary>
        /// PRINT_UNICODE instruction.
        /// </summary>
        private void print_unicode()
        {
            char zchar = (char)getUnsignedValue(0);
            getMachine().PrintZsciiChar(zchar);
            nextInstruction();
        }

        /// <summary>
        /// CHECK_UNICODE instruction.
        /// </summary>
        private void check_unicode()
        {
            // always return true, set bit 0 for can print and bit 1 for
            // can read
            storeUnsignedResult((char)3);
            nextInstruction();
        }

        /// <summary>
        /// MOUSE_WINDOW instruction.
        /// </summary>
        private void mouse_window()
        {
            getMachine().Screen6.setMouseWindow(getSignedValue(0));
            nextInstruction();
        }

        /// <summary>
        /// PICTURE_DATA instruction.
        /// </summary>
        private void picture_data()
        {
            int picnum = getUnsignedValue(0);
            int array = getUnsignedValue(1);
            bool result = false;

            if (picnum == 0)
            {
                writePictureFileInfo(array);
                // branch if any pictures are available: this information is only
                // available in the 1.1 spec
                result = getMachine().PictureManager.getNumPictures() > 0;
            }
            else
            {
                Resolution picdim =
                  getMachine().PictureManager.getPictureSize(picnum);
                if (picdim != null)
                {
                    getMachine().WriteUnsigned16(array, ToUnsigned16(picdim.getHeight()));
                    getMachine().WriteUnsigned16(array + 2,
                                                 ToUnsigned16(picdim.getWidth()));
                    result = true;
                }
            }
            branchOnTest(result);
        }

        /// <summary>
        /// Writes the information of the picture file into the specified array.
        /// </summary>
        /// <param name="array">an array address</param>
        private void writePictureFileInfo(int array)
        {
            getMachine().WriteUnsigned16(array,
                ToUnsigned16(getMachine().PictureManager.getNumPictures()));
            getMachine().WriteUnsigned16(array + 2,
                ToUnsigned16(getMachine().PictureManager.getRelease()));
        }

        /// <summary>
        /// DRAW_PICTURE instruction.
        /// </summary>
        private void draw_picture()
        {
            int picnum = getUnsignedValue(0);
            int x = 0, y = 0;

            if (getNumOperands() > 1)
            {
                y = getUnsignedValue(1);
            }

            if (getNumOperands() > 2)
            {
                x = getUnsignedValue(2);
            }
            getMachine().Screen6.getSelectedWindow().drawPicture(
                getMachine().PictureManager.getPicture(picnum), y, x);
            nextInstruction();
        }

        /// <summary>
        /// ERASE_PICTURE instruction.
        /// </summary>
        private void erase_picture()
        {
            int picnum = getUnsignedValue(0);
            int x = 1, y = 1;

            if (getNumOperands() > 1)
            {
                y = getUnsignedValue(1);
            }

            if (getNumOperands() > 2)
            {
                x = getUnsignedValue(2);
            }
            getMachine().Screen6.getSelectedWindow().erasePicture(
                getMachine().PictureManager.getPicture(picnum), y, x);
            nextInstruction();
        }

        /// <summary>
        /// MOVE_WINDOW instruction.
        /// </summary>
        private void move_window()
        {
            getMachine().Screen6.getWindow(getUnsignedValue(0)).move(
                getUnsignedValue(1), getUnsignedValue(2));
            nextInstruction();
        }

        /// <summary>
        /// WINDOW_SIZE instruction.
        /// </summary>
        private void window_size()
        {
            short window = getSignedValue(0);
            char height = getUnsignedValue(1);
            char width = getUnsignedValue(2);
            getMachine().Screen6.getWindow(window).setSize(height, width);
            nextInstruction();
        }

        /// <summary>
        /// WINDOW_STYLE instruction.
        /// </summary>
        private void window_style()
        {
            int operation = 0;
            if (getNumOperands() > 2)
            {
                operation = getUnsignedValue(2);
            }
            getWindow(getSignedValue(0)).setStyle(getUnsignedValue(1), operation);
            nextInstruction();
        }

        /// <summary>
        /// SET_MARGINS instruction.
        /// </summary>
        private void set_margins()
        {
            int window = ScreenModel.CURRENT_WINDOW;
            if (getNumOperands() == 3)
            {
                window = getSignedValue(2);
            }
            getWindow(window).setMargins(getUnsignedValue(0), getUnsignedValue(1));
            nextInstruction();
        }

        /// <summary>
        /// GET_WIND_PROP instruction.
        /// </summary>
        private void get_wind_prop()
        {
            int window = getSignedValue(0);
            int propnum = getUnsignedValue(1);
            char result;
            result = (char)getWindow(window).getProperty(propnum);
            storeUnsignedResult(result);
            nextInstruction();
        }

        /// <summary>
        /// PUT_WIND_PROP instruction.
        /// </summary>
        private void put_wind_prop()
        {
            short window = getSignedValue(0);
            char propnum = getUnsignedValue(1);
            short value = getSignedValue(2);
            getWindow(window).putProperty(propnum, value);
            nextInstruction();
        }

        /// <summary>
        /// PICTURE_TABLE instruction.
        /// </summary>
        private void picture_table()
        {
            // @picture_table is a no-op, because all pictures are held in memory
            // anyways
            nextInstruction();
        }

        /// <summary>
        /// POP_STACK instruction.
        /// </summary>
        private void pop_stack()
        {
            int numItems = getUnsignedValue(0);
            char stack = (char)0;
            if (getNumOperands() == 2)
            {
                stack = getUnsignedValue(1);
            }
            for (int i = 0; i < numItems; i++)
            {
                getMachine().PopStack(stack);
            }
            nextInstruction();
        }

        /// <summary>
        /// PUSH_STACK instruction.
        /// </summary>
        private void push_stack()
        {
            char value = getUnsignedValue(0);
            char stack = (char)0;
            if (getNumOperands() == 2)
            {
                stack = getUnsignedValue(1);
            }
            branchOnTest(getMachine().PushStack(stack, value));
        }

        /// <summary>
        /// SCROLL_WINDOW instruction.
        /// </summary>
        private void scroll_window()
        {
            getWindow(getSignedValue(0)).scroll(getSignedValue(1));
            nextInstruction();
        }

        /// <summary>
        /// READ_MOUSE instruction.
        /// </summary>
        private void read_mouse()
        {
            int array = getUnsignedValue(0);
            getMachine().Screen6.readMouse(array);
            nextInstruction();
        }
    }
}
