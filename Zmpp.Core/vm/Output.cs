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
    using Zmpp.Core;
    using Zmpp.Core.Encoding;
    using Zmpp.Core.IO;

    /// <summary>
    /// Output implementation.
    /// </summary>
    public class Output : OutputBase, IOutput
    {
        private readonly IMachine machine;

        /// <summary>
        /// This is the array of output streams.
        /// </summary>
        private readonly IOutputStream[] outputStream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="machine">The Machine object.</param>
        public Output(IMachine machine) : base()
        {
            this.machine = machine;
            outputStream = new IOutputStream[3];
        }

        /// <summary>
        /// Sets the output stream to the specified number.
        /// </summary>
        /// <param name="streamnumber">the stream number</param>
        /// <param name="stream">the output stream</param>
        public void SetOutputStream(int streamnumber, IOutputStream stream)
        {
            outputStream[streamnumber - 1] = stream;
        }

        public void PrintZString(int address)
        {
            Print(machine.Decode2Zscii(address, 0));
        }

        public void Print(string str) { PrintZsciiChars(str); }

        public void NewLine() { PrintZsciiChar(ZsciiEncoding.Newline); }

        public void PrintZsciiChar(char zchar)
        {
            PrintZsciiChars(zchar.ToString());
        }

        /// <summary>
        /// Prints the specified array of ZSCII characters.
        /// </summary>
        /// <param name="zsciiString">The array of ZSCII characters.</param>
        /// <remarks>
        /// This is the only method that communicates with the output streams directly.
        /// </remarks>
        private void PrintZsciiChars(string zsciiString)
        {
            CheckTranscriptFlag();
            if (outputStream[OUTPUTSTREAM_MEMORY - 1].IsSelected())
            {
                for (int i = 0, n = zsciiString.Length; i < n; i++)
                {
                    outputStream[OUTPUTSTREAM_MEMORY - 1].Print(zsciiString[i]);
                }
            }
            else
            {
                for (int i = 0; i < outputStream.Length; i++)
                {
                    if (outputStream[i] != null && outputStream[i].IsSelected())
                    {
                        for (int j = 0, n = zsciiString.Length; j < n; j++)
                        {
                            outputStream[i].Print(zsciiString[j]);
                        }
                    }
                }
            }
        }

        public void PrintNumber(short number)
        {
            Print(number.ToString());
        }

        /// <summary>
        /// Flushes the output.
        /// </summary>
        public void FlushOutput()
        {
            // At the moment flushing only makes sense for screen
            if (!outputStream[OUTPUTSTREAM_MEMORY - 1].IsSelected())
            {
                for (int i = 0; i < outputStream.Length; i++)
                {
                    if (outputStream[i] != null && outputStream[i].IsSelected())
                    {
                        outputStream[i].Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Checks the fileheader if the transcript flag was set by the game
        /// bypassing output_stream, e.g. with a storeb to the fileheader flags
        /// address. Enable the transcript depending on the status of that flag.
        /// </summary>
        private void CheckTranscriptFlag()
        {
            if (outputStream[OUTPUTSTREAM_TRANSCRIPT - 1] != null)
            {
                outputStream[OUTPUTSTREAM_TRANSCRIPT - 1].Select(
                    machine.FileHeader.IsEnabled(StoryFileHeaderAttribute.Transcripting));
            }
        }

        public void SelectOutputStream(int streamnumber, bool flag)
        {
            outputStream[streamnumber - 1].Select(flag);

            // Sets the tranxdQscript flag if the transcipt is specified
            if (streamnumber == OUTPUTSTREAM_TRANSCRIPT)
            {
                machine.FileHeader.SetEnabled(StoryFileHeaderAttribute.Transcripting, flag);
            }
            else if (streamnumber == OUTPUTSTREAM_MEMORY && flag)
            {
                machine.Halt("invalid selection of memory stream");
            }
        }

        public void SelectOutputStream3(int tableAddress, int tableWidth)
        {
            ((MemoryOutputStream)outputStream[OUTPUTSTREAM_MEMORY - 1]).select(tableAddress, tableWidth);
        }

        public void Close()
        {
            if (outputStream != null)
            {
                for (int i = 0; i < outputStream.Length; i++)
                {
                    if (outputStream[i] != null)
                    {
                        outputStream[i].Flush();
                        outputStream[i].Close();
                    }
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < outputStream.Length; i++)
            {
                if (outputStream[i] != null)
                {
                    outputStream[i].Flush();
                }
            }
        }
    }
}
