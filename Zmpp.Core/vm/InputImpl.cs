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
    using Zmpp.Core.IO;

    /// <summary>
    /// Input interface implementation.
    /// </summary>
    public class InputImpl : IInput
    {
        /// <summary>
        /// This is the array of input streams.
        /// </summary>
        private IInputStream[] inputStream = new IInputStream[2];

        /// <summary>
        /// The selected input stream.
        /// </summary>
        private int selectedInputStreamIndex = 0;

        public void close()
        {
            if (inputStream != null)
            {
                for (int i = 0; i < inputStream.Length; i++)
                {
                    if (inputStream[i] != null)
                    {
                        inputStream[i].Close();
                    }
                }
            }
        }

        /// <summary>
        /// Sets an input stream to the specified number.
        /// </summary>
        /// <param name="streamnumber">the input stream number</param>
        /// <param name="stream">the input stream to set</param>
        public void setInputStream(int streamnumber, IInputStream stream)
        {
            inputStream[streamnumber] = stream;
        }

        public void SelectInputStream(int streamnumber)
        {
            selectedInputStreamIndex = streamnumber;
        }

        public IInputStream SelectedInputStream => inputStream[selectedInputStreamIndex];
    }
}
