// TODO: Implement TranscriptOutputStream!
///*
// * Created on 11/08/2005
// * Copyright (c) 2005-2010, Wei-ju Wu.
// * All rights reserved.
// *
// * Redistribution and use in source and binary forms, with or without
// * modification, are permitted provided that the following conditions are met:
// *
// * Redistributions of source code must retain the above copyright notice, this
// * list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice,
// * this list of conditions and the following disclaimer in the documentation
// * and/or other materials provided with the distribution.
// * Neither the name of Wei-ju Wu nor the names of its contributors may
// * be used to endorse or promote products derived from this software without
// * specific prior written permission.
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// * POSSIBILITY OF SUCH DAMAGE.
// */

//namespace Zmpp.Core.io
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Text;
//    using Zmpp.Core.encoding;
//    using Microsoft.Extensions.Logging;
//    using System.IO;

//    /// <summary>
//    /// This class defines an output stream for transcript output (Stream 2).
//    /// </summary>
//    public class TranscriptOutputStream : IOutputStream
//    {
//        private readonly ILogger LOG;
//        private IIOSystem iosys;
//        private BufferedWriter output;
//        private Writer transcriptWriter;
//        private bool enabled;
//        private StringBuilder linebuffer;
//        private IZsciiEncoding encoding;
//        private bool initialized;

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="iosys">the I/O system</param>
//        /// <param name="encoding">IZsciiEncoding object</param>
//        public TranscriptOutputStream(ILogger logger, IIOSystem iosys, IZsciiEncoding encoding)
//        {
//            this.LOG = logger;
//            this.iosys = iosys;
//            this.encoding = encoding;
//            linebuffer = new StringBuilder();
//        }

//        /// <summary>
//        /// Initializes the output file.
//        /// </summary>
//        private void initFile()
//        {
//            if (!initialized && transcriptWriter == null)
//            {
//                transcriptWriter = iosys.getTranscriptWriter();
//                if (transcriptWriter != null)
//                {
//                    output = new BufferedWriter(transcriptWriter);
//                }
//                initialized = true;
//            }
//        }

//        public void print(char zsciiChar)
//        {
//            initFile();
//            if (output != null)
//            {
//                if (zsciiChar == ZsciiEncoding.NEWLINE)
//                {
//                    flush();
//                }
//                else if (zsciiChar == ZsciiEncoding.DELETE)
//                {
//                    linebuffer.Remove(linebuffer.Length - 1, 1);
//                }
//                else
//                {
//                    linebuffer.Append(encoding.getUnicodeChar(zsciiChar));
//                }
//                flush();
//            }
//        }

//        public void select(bool flag) { enabled = flag; }

//        public bool isSelected() { return enabled; }

//        public void flush()
//        {
//            try
//            {
//                if (output != null)
//                {
//                    output.write(linebuffer.ToString());
//                    linebuffer = new StringBuilder();
//                }
//            }
//            catch (IOException ex)
//            {
//                LOG.LogError("TranscriptOutputStream", "flush", ex);
//            }
//        }

//        public void close()
//        {
//            if (output != null)
//            {
//                try
//                {
//                    output.close();
//                    output = null;
//                }
//                catch (Exception ex)
//                {
//                    LOG.LogError("TranscriptOutputStream", "close", ex);
//                }
//            }

//            if (transcriptWriter != null)
//            {
//                try
//                {
//                    transcriptWriter.close();
//                    transcriptWriter = null;
//                }
//                catch (Exception ex)
//                {
//                    LOG.LogError("TranscriptOutputStream", "close", ex);
//                }
//            }
//            initialized = false;
//        }
//    }
//}
