/*
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
    using Zmpp.Core;
    using Zmpp.Core.Instructions;
    using Zmpp.Core.Vm;
    using System;
    using System.IO;

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

        internal void Run(string storyFilePath)
        {
            _logger.LogInformation("ZMachine started at {dateTime}", DateTime.UtcNow);

            var saveFileName = Path.Combine(Path.GetDirectoryName(storyFilePath), String.Concat(Path.GetFileNameWithoutExtension(storyFilePath), ".dat"));
            var consoleInput = new ConsoleInput();
            var statusLine = new ConsoleStatusLine();
            var screenModel = new ConsoleScreenModel();
            var saveGameDataStore = new FileSaveGameDataStore(saveFileName);

            MachineFactory.MachineInitStruct initStruct = new MachineFactory.MachineInitStruct()
            {
                storyFile = storyFilePath,
                blorbFile = null,
                storyURL = null,
                blorbURL = null,
                keyboardInputStream = consoleInput,
                statusLine = statusLine,
                screenModel = screenModel,
                ioSystem = null,
                saveGameDataStore = saveGameDataStore,
                nativeImageFactory = null,
                soundEffectFactory = null
            };

            instructionDecoder = new InstructionDecoder();

            MachineFactory factory = new MachineFactory(_logger, initStruct);
            machine = factory.buildMachine();
            machine.start();
            instructionDecoder.initialize(machine);
            int version = machine.getVersion();
            // ZMPP should support everything by default
            if (version <= 3)
            {
                enableHeaderFlag(StoryFileHeaderAttribute.DefaultFontIsVariable);
                enableHeaderFlag(StoryFileHeaderAttribute.SupportsStatusLine);
                enableHeaderFlag(StoryFileHeaderAttribute.SupportsScreenSplitting);
            }
            if (version >= 4)
            {
                enableHeaderFlag(StoryFileHeaderAttribute.SupportsBold);
                enableHeaderFlag(StoryFileHeaderAttribute.SupportsFixedFont);
                enableHeaderFlag(StoryFileHeaderAttribute.SupportsItalic);
                enableHeaderFlag(StoryFileHeaderAttribute.SupportsTimedInput);
            }
            if (version >= 5)
            {
                enableHeaderFlag(StoryFileHeaderAttribute.SupportsColours);
            }
            int defaultForeground = getDefaultForeground();
            int defaultBackground = getDefaultBackground();
            _logger.LogInformation("GAME DEFAULT FOREGROUND: " + defaultForeground);
            _logger.LogInformation("GAME DEFAULT BACKGROUND: " + defaultBackground);
            machine.getScreen().setBackground(defaultBackground, -1);
            machine.getScreen().setForeground(defaultForeground, -1);

            consoleInput.Init(machine);// TODO: consoleInput & IMachine have a weird circular dependency!
            var runstate = Start();
            while (runstate != null && runstate.isReadLine())
            {
                runstate = Start();
            }

            _logger.LogInformation("ZMachine stopped at {dateTime}", DateTime.UtcNow);
        }

        /// <summary>
        /// Enables the specified header flag.
        /// </summary>
        /// <param name="attr">the header attribute to enable</param>
        private void enableHeaderFlag(StoryFileHeaderAttribute attr)
        {
            getFileHeader().SetEnabled(attr, true);
        }

        /// <summary>
        /// Returns the file header.
        /// </summary>
        /// <returns>the file header</returns>
        public IStoryFileHeader getFileHeader() { return machine.getFileHeader(); }

        /// <summary>
        /// Returns the default background color.
        /// </summary>
        /// <returns>default background color</returns>
        public int getDefaultBackground()
        {
            return machine.ReadUnsigned8(StoryFileHeaderAddress.DefaultBackground);
        }

        /// <summary>
        /// Returns the default foreground color.
        /// </summary>
        /// <returns>default foreground color</returns>
        public int getDefaultForeground()
        {
            return machine.ReadUnsigned8(StoryFileHeaderAddress.DefaultForeground);
        }


        /// <summary>
        /// Returns the current step number.
        /// </summary>
        /// <returns>current step number</returns>
        public int getStep() { return step; }

        /// <summary>
        /// The execution loop. It runs until either an input state is reached
        /// or the machine is set to stop state.
        /// </summary>
        /// <returns>the new MachineRunState</returns>
        internal MachineRunState Start()//public MachineRunState run()
        {
            while (machine.getRunState() != MachineRunState.STOPPED)
            {
                int pc = machine.getPC();
                IInstruction instr = instructionDecoder.decodeInstruction(pc);
                // if the print is executed after execute(), the result is different !!
                if (DEBUG && machine.getRunState() == MachineRunState.RUNNING)
                {
                    Console.Out.WriteLine(String.Format("{0:D4}: ${1:x5} {2}", step, (int)pc, instr.ToString()));
                }
                instr.execute();

                // handle input situations here
                if (machine.getRunState().isWaitingForInput())
                {
                    break;
                }
                else
                {
                    step++;
                }
            }
            return machine.getRunState();
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
