/*
 * Created on 11/23/2005
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
    using Zmpp.Core.IO;
    using System.Collections.Generic;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This class implements output stream 3. This stream writes to dynamic
    /// memory.The stream contains a table address stack in order to
    /// support nested selections.
    /// </summary>
    public class MemoryOutputStream : IOutputStream
    {
        /// <summary>
        /// Maximum nesting depth for this stream.
        /// </summary>
        private const int MAX_NESTING_DEPTH = 16;

        /// <summary>
        /// Table position representation.
        /// </summary>
        public class TablePosition
        {
            public int tableAddress;
            public int bytesWritten;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="tableAddress">address of the table</param>
            public TablePosition(int tableAddress)
            {
                this.tableAddress = tableAddress;
            }
        }

        private IMachine machine;

        /// <summary>
        /// Support nested selections.
        /// </summary>
        private List<TablePosition> tableStack;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">the machine object</param>
        public MemoryOutputStream(IMachine machine)
        {
            tableStack = new List<TablePosition>();
            this.machine = machine;
        }

        public void print(char zsciiChar)
        {
            TablePosition tablePos = tableStack[tableStack.Count - 1];
            int position = tablePos.tableAddress + 2 + tablePos.bytesWritten;
            machine.WriteUnsigned8(position, zsciiChar);
            tablePos.bytesWritten++;
        }

        public void flush()
        {
            // intentionally left empty
        }

        public void close()
        {
            // intentionally left empty
        }

        public void select(bool flag)
        {
            if (!flag && tableStack.Count > 0)
            {
                // Write the total number of written bytes to the first word
                // of the table
                TablePosition tablePos = tableStack[tableStack.Count - 1];
                tableStack.RemoveAt(tableStack.Count - 1);
                machine.WriteUnsigned16(tablePos.tableAddress,
                                        ToUnsigned16(tablePos.bytesWritten));

                if (machine.Version == 6)
                {
                    writeTextWidthInUnits(tablePos);
                }
            }
        }

        /// <summary>
        /// Writes the text width in units.
        /// </summary>
        /// <param name="tablepos">table position</param>
        private void writeTextWidthInUnits(TablePosition tablepos)
        {
            int numwords = tablepos.bytesWritten;
            char[] data = new char[numwords];

            for (int i = 0; i < numwords; i++)
            {
                data[i] = (char)machine.ReadUnsigned8(tablepos.tableAddress + i + 2);
            }
            machine.Screen6.setTextWidthInUnits(data);
        }

        /// <summary>
        /// Selects this memory stream.
        /// </summary>
        /// <param name="tableAddress">the table address</param>
        /// <param name="tableWidth">the table width</param>
        public void select(int tableAddress, int tableWidth)
        {
            //this.tableWidth = tableWidth;
            if (tableStack.Count < MAX_NESTING_DEPTH)
            {
                tableStack.Add(new TablePosition(tableAddress));
            }
            else
            {
                machine.Halt("maximum nesting depth (16) for stream 3 exceeded");
            }
        }

        public bool isSelected()
        {
            return tableStack.Count > 0;
        }
    }
}
