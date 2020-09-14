/*
 * Created on 2006/02/14
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
    /// Provides methods for managing and writing to output streams.
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Selects or deselects the specified output stream.
        /// </summary>
        /// <param name="streamnumber">The output stream number.</param>
        /// <param name="flag">The flag. true to enable the output stream; otherwise false to disable the output stream.</param>
        /// <remarks>
        /// If the streamnumber is negative |streamnumber| is deselected.
        /// if the stream number is positive it is selected. Stream 3 (the memory stream) can
        /// not be selected by this function but can be deselected here.
        /// </remarks>
        void SelectOutputStream(int streamnumber, bool flag);

        /// <summary>
        /// Selects the output stream 3 which writes to memory.
        /// </summary>
        /// <param name="tableAddress">The table address to write to.</param>
        /// <param name="tableWidth">The table width.</param>
        void SelectOutputStream3(int tableAddress, int tableWidth);

        /// <summary>
        /// Prints the ZSCII string at the specified address to the active
        /// output streams.
        /// </summary>
        /// <param name="stringAddress">The address of the ZSCII string.</param>
        void PrintZString(int stringAddress);

        /// <summary>
        /// Prints the specified string to the active output streams.
        /// </summary>
        /// <param name="str">The string to print. Encoding is ZSCII.</param>
        void Print(string str);

        /// <summary>
        /// Prints a newline to the active output streams.
        /// </summary>
        void NewLine();

        /// <summary>
        /// Prints the specified ZSCII character to the active output streams.
        /// </summary>
        /// <param name="zchar">The ZSCII character to print.</param>
        void PrintZsciiChar(char zchar);

        /// <summary>
        /// Prints the specified signed number to the active output streams.
        /// </summary>
        /// <param name="num">The number to print.</param>
        void PrintNumber(short num);

        /// <summary>
        /// Flushes the active output streams.
        /// </summary>
        void FlushOutput();

        /// <summary>
        /// Resets the output streams.
        /// </summary>
        void Reset();
    }
}
