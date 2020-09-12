/*
 * Created on 2005/09/23
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

namespace Zmpp.Core
{
    public abstract class StoryFileHeaderBase
    {
        public static int RELEASE = 0x02;
        public static int PROGRAM_START = 0x06;
        public static int DICTIONARY = 0x08;
        public static int OBJECT_TABLE = 0x0a;
        public static int GLOBALS = 0x0c;
        public static int STATIC_MEM = 0x0e;
        public static int ABBREVIATIONS = 0x18;
        public static int CHECKSUM = 0x1c;
        public static int INTERPRETER_NUMBER = 0x1e;
        public static int SCREEN_HEIGHT = 0x20;
        public static int SCREEN_WIDTH = 0x21;
        public static int SCREEN_WIDTH_UNITS = 0x22;
        public static int SCREEN_HEIGHT_UNITS = 0x24;
        public static int ROUTINE_OFFSET = 0x28;
        public static int STATIC_STRING_OFFSET = 0x2a;
        public static int DEFAULT_BACKGROUND = 0x2c;
        public static int DEFAULT_FOREGROUND = 0x2d;
        public static int TERMINATORS = 0x2e;
        public static int OUTPUT_STREAM3_WIDTH = 0x30; // 16 bit
        public static int STD_REVISION_MAJOR = 0x32;
        public static int STD_REVISION_MINOR = 0x33;
        public static int CUSTOM_ALPHABET = 0x34;
    }
}
