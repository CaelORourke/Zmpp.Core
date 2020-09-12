/*
 * Created on 11/22/2005
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

namespace org.zmpp.windowing
{
    /// <summary>
    /// This defines the operations that can be performed on a text cursor.
    /// </summary>
    public interface ITextCursor
    {
        /// <summary>
        /// Returns the current line.
        /// </summary>
        /// <returns>the current line</returns>
        int getLine();

        /// <summary>
        /// Returns the current column.
        /// </summary>
        /// <returns>the current column</returns>
        int getColumn();

        /// <summary>
        /// Sets the current line. A value <= 0 will set the line to 1.
        /// </summary>
        /// <param name="line">the new current line</param>
        void setLine(int line);

        /// <summary>
        /// Sets the current column. A value <= 0 will set the column to 1.
        /// </summary>
        /// <param name="column">the new current column</param>
        void setColumn(int column);

        /// <summary>
        /// Sets the new position. Values <= 0 will set the corresponding values
        /// to 1.
        /// </summary>
        /// <param name="line">the new line</param>
        /// <param name="column">the new column</param>
        void setPosition(int line, int column);
    }
}
