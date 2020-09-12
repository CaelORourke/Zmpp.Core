// TODO: Implement FileInputStream!
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

//namespace org.zmpp.io
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Text;
//    using org.zmpp.encoding;
//    using Microsoft.Extensions.Logging;
//    using System.IO;

//    /// <summary>
//    /// This class implements a Z-machine input stream that takes its input from
//    /// a file.It queries a screen model to provide the input file.
//    /// </summary>
//    public class FileInputStream : IInputStream
//    {
//        private readonly ILogger LOG;
//        private IIOSystem iosys;
//        private IZsciiEncoding encoding;
//        private Reader filereader;
//        private BufferedReader input;

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="logger"></param>
//        /// <param name="iosys">an IOSystem object</param>
//        /// <param name="encoding">a ZSCII encoding object</param>
//        public FileInputStream(ILogger logger, IIOSystem iosys, IZsciiEncoding encoding)
//        {
//            this.LOG = logger;
//            this.iosys = iosys;
//            this.encoding = encoding;
//        }

//        public String readLine()
//        {
//            checkForReader();
//            if (input != null)
//            {
//                // Read from file
//                try
//                {
//                    if (input.ready())
//                    {
//                        String line = input.readLine();
//                        /*
//                        if (encoding.isConvertableToZscii(c)) {
//                          return encoding.getZsciiChar(c);
//                        }*/
//                        return encoding.convertToZscii(line);
//                    }
//                }
//                catch (IOException ex)
//                {
//                    LOG.LogError("FileInputStream", "readLine", ex);
//                }
//            }
//            return null;
//        }

//        public void close()
//        {
//            if (input != null)
//            {
//                try
//                {
//                    input.close();
//                    input = null;
//                }
//                catch (IOException ex)
//                {
//                    LOG.LogError("FileInputStream", "close", ex);
//                }
//            }

//            if (filereader != null)
//            {
//                try
//                {
//                    filereader.close();
//                    filereader = null;
//                }
//                catch (IOException ex)
//                {
//                    LOG.LogError("FileInputStream", "readLine", ex);
//                }
//            }
//        }

//        /** Creates the reader object if necessary. */
//        private void checkForReader()
//        {
//            if (filereader == null)
//            {
//                filereader = iosys.getInputStreamReader();
//                input = new BufferedReader(filereader);
//            }
//        }
//    }
//}
