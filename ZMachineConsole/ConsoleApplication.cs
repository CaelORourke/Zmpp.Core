﻿/*
 * Created on 2008/04/25
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

namespace ZMachineConsole
{
    using Microsoft.Extensions.Logging;
    using System;
    using Zmpp.Core;
    using Zmpp.Core.Instructions;
    using Zmpp.Core.Vm;

    /// <summary>
    /// This class based on the ExecutionControl class.
    /// </summary>
    public class ConsoleApplication
    {
        private int step = 1;
        public const bool DEBUG = false;
        public const bool DEBUG_INTERRUPT = false;

        private readonly ILogger _logger;
        private IMachine machine;
        private InstructionDecoder instructionDecoder;

        public ConsoleApplication(ILogger<ConsoleApplication> logger)
        {
            _logger = logger;

        }

        internal async void Run(string storyFilePath)
        {
            _logger.LogInformation("ZMachine started at {dateTime}", DateTime.UtcNow);

            ConsoleViewModel console = new ConsoleViewModel(storyFilePath);

            instructionDecoder = new InstructionDecoder();

            //Uri uri = new Uri("https://github.com/Adeimantius/Z-Machine/raw/master/Zork%201/DATA/ZORK1.DAT");
            //machine = MachineFactory.Create(_logger, uri, console);
            //storyFilePath = @"C:\shane\ZORK1.DAT";
            machine = MachineFactory.Create(_logger, storyFilePath, console);
            machine.Start();

            instructionDecoder.initialize(machine);

            int version = machine.Version;

            // ZMPP should support everything by default
            if (version <= 3)
            {
                EnableHeaderFlag(StoryFileHeaderAttribute.DefaultFontIsVariable);
                EnableHeaderFlag(StoryFileHeaderAttribute.SupportsStatusLine);
                EnableHeaderFlag(StoryFileHeaderAttribute.SupportsScreenSplitting);
            }
            if (version >= 4)
            {
                EnableHeaderFlag(StoryFileHeaderAttribute.SupportsBold);
                EnableHeaderFlag(StoryFileHeaderAttribute.SupportsFixedFont);
                EnableHeaderFlag(StoryFileHeaderAttribute.SupportsItalic);
                EnableHeaderFlag(StoryFileHeaderAttribute.SupportsTimedInput);
            }
            if (version >= 5)
            {
                EnableHeaderFlag(StoryFileHeaderAttribute.SupportsColours);
            }

            int defaultForeground = DefaultForeground;
            int defaultBackground = DefaultBackground;
            _logger.LogInformation("GAME DEFAULT FOREGROUND: " + defaultForeground);
            _logger.LogInformation("GAME DEFAULT BACKGROUND: " + defaultBackground);
            machine.Screen.setBackground(defaultBackground, -1);
            machine.Screen.setForeground(defaultForeground, -1);

            var runstate = Start();
            while (runstate != null && runstate.IsReadLine)
            {
                runstate = Start();
            }

            _logger.LogInformation("ZMachine stopped at {dateTime}", DateTime.UtcNow);
        }

        /// <summary>
        /// Enables the specified header flag.
        /// </summary>
        /// <param name="attr">The header attribute to enable.</param>
        private void EnableHeaderFlag(StoryFileHeaderAttribute attr)
        {
            FileHeader.SetEnabled(attr, true);
        }

        /// <summary>
        /// Gets the file header.
        /// </summary>
        public IStoryFileHeader FileHeader => machine.FileHeader;

        /// <summary>
        /// Gets the default background color.
        /// </summary>
        public int DefaultBackground => machine.ReadUnsigned8(StoryFileHeaderAddress.DefaultBackground);

        /// <summary>
        /// Gets the default foreground color.
        /// </summary>
        public int DefaultForeground => machine.ReadUnsigned8(StoryFileHeaderAddress.DefaultForeground);

        /// <summary>
        /// Gets the current step number.
        /// </summary>
        public int Step => step;

        /// <summary>
        /// The execution loop. It runs until either an input state is reached
        /// or the machine is set to stop state.
        /// </summary>
        /// <returns>the new MachineRunState</returns>
        internal MachineRunState Start()//public MachineRunState run()
        {
            while (machine.RunState != MachineRunState.STOPPED)
            {
                int pc = machine.PC;
                IInstruction instr = instructionDecoder.decodeInstruction(pc);
                // if the print is executed after execute(), the result is different !!
                if (DEBUG && machine.RunState == MachineRunState.RUNNING)
                {
                    Console.Out.WriteLine(String.Format("{0:D4}: ${1:x5} {2}", step, (int)pc, instr.ToString()));
                }
                instr.Execute();

                // handle input situations here
                if (machine.RunState.IsWaitingForInput)
                {
                    break;
                }
                else
                {
                    step++;
                }
            }
            return machine.RunState;
        }

        ///**
        // * Resumes from an input state to the run state using the specified Unicode
        // * input string.
        // * @param input the Unicode input string
        // * @return the new MachineRunState
        // */
        //public MachineRunState resumeWithInput(String input)
        //{
        //    inputStream.addInputLine(convertToZsciiInputLine(input));
        //    return Start();//return run();
        //}

        ///// <summary>
        ///// This method should be called from a timed input method, to fill
        ///// the text buffer with current input.By using this, it is ensured,
        ///// the game could theoretically process preliminary input.
        ///// </summary>
        ///// <param name="text">the input text as Unicode</param>
        //public void setTextToInputBuffer(String text)
        //{
        //    MachineRunState runstate = machine.getRunState();
        //    if (runstate != null && runstate.isReadLine())
        //    {
        //        inputStream.addInputLine(convertToZsciiInputLine(text));
        //        int textbuffer = machine.getRunState().getTextBuffer();
        //        machine.readLine(textbuffer);
        //    }
        //}
    }
}
