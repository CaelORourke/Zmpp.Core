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
    using System;
    using System.IO;
    using System.Net.Http;
    using Zmpp.Core;
    using Zmpp.Core.Blorb;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Instructions;
    using Zmpp.Core.Media;
    using Zmpp.Core.UI;
    using Zmpp.Core.Vm;

    /// <summary>
    /// Represents a z-machine.
    /// </summary>
    /// <remarks>
    /// Based on the ExecutionControl class from the original source.
    /// </remarks>
    public class ZMachine
    {
        public const bool DEBUG = false;
        public const bool DEBUG_INTERRUPT = false;

        private readonly ILogger logger;
        private readonly IMachine machine;
        private readonly InstructionDecoder instructionDecoder;
        private readonly IViewModel viewModel;

        private int step = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZMachineConsole.ZMachine"/>
        /// class for the specified view model.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="viewModel">The view model.</param>
        public ZMachine(ILogger logger, IViewModel viewModel)
        {
            this.logger = logger;
            this.instructionDecoder = new InstructionDecoder();
            this.machine = MachineFactory.Create(logger, viewModel);
            this.viewModel = viewModel;
        }

        /// <summary>
        /// Opens the story file from the specified file path.
        /// </summary>
        /// <param name="storyFilePath">The file path.</param>
        public void OpenStoryFile(string storyFilePath)
        {
            IResources resources = null;
            byte[] storyData = ReadStoryData(storyFilePath);

            if (IsBlorb(storyData))
            {
                IFormChunk formChunk = ReadBlorb(storyData);
                storyData = (formChunk != null) ?
                    new BlorbFile(formChunk).StoryData : null;
                resources = (formChunk != null) ?
                    new BlorbResources(viewModel.NativeImageFactory, viewModel.SoundEffectFactory, formChunk) : null;
            }

            Initialize(storyData, resources);
        }

        /// <summary>
        /// Opens the story file from the specified URL.
        /// </summary>
        /// <param name="storyFileUrl">The URL.</param>
        public void OpenStoryFile(Uri storyFileUrl)
        {
            IResources resources = null;
            byte[] storyData = ReadStoryData(storyFileUrl);

            if (IsBlorb(storyData))
            {
                IFormChunk formChunk = ReadBlorb(storyData);
                storyData = (formChunk != null) ?
                    new BlorbFile(formChunk).StoryData : null;
                resources = (formChunk != null) ?
                    new BlorbResources(viewModel.NativeImageFactory, viewModel.SoundEffectFactory, formChunk) : null;
            }

            Initialize(storyData, resources);
        }

        /// <summary>
        /// Opens the specified story file.
        /// </summary>
        /// <param name="storyFile">The file path or the URL of the story file.</param>
        public void Open(string storyFile)
        {
            // if the story file is a valid path
            if (File.Exists(storyFile))
            {
                OpenStoryFile(storyFile);
            }
            else
            {
                // try the story file as a URL
                if (Uri.TryCreate(storyFile, UriKind.RelativeOrAbsolute, out Uri uri))
                {
                    OpenStoryFile(uri);
                }
                throw new StoryFileNotFoundException("Could not open the requested story file.");
            }
        }

        private bool IsBlorb(byte[] data)
        {
            IMemory memory = new Memory(data);
            byte[] id = new byte[ChunkBase.ChunkIdLength];
            memory.CopyBytesToArray(id, 0, 0, ChunkBase.ChunkIdLength);
            // Array.Copy(data, 0, id, 0, ChunkBase.ChunkIdLength);
            string Id = System.Text.Encoding.UTF8.GetString((byte[])(object)id, 0, id.Length);
            if (!"FORM".Equals(Id))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initializes the z-machine using the specified story data and resources.
        /// </summary>
        /// <param name="data">The story data.</param>
        /// <param name="resources">The story resources.</param>
        private void Initialize(byte[] data, IResources resources)
        {
            machine.Initialize(data, resources);
            if (IsInvalidStory(machine.Version))
            {
                throw new InvalidStoryFileException($"I don't understand version {machine.Version} story files.");
            }

            instructionDecoder.initialize(machine);
        }

        /// <summary>
        /// Indicates whether the story file version is valid.
        /// </summary>
        /// <param name="version">The story file version.</param>
        /// <returns>true if the story file version is supported; otherwise false.</returns>
        private bool IsInvalidStory(int version)
        {

            return version < 1 || version > 8;
        }

        #region Story & Resource Data

        /// <summary>
        /// Reads the story file data from the specified file path.
        /// </summary>
        /// <returns>The story file data.</returns>
        private byte[] ReadStoryData(string path)
        {
            return File.ReadAllBytes(path);
        }

        /// <summary>
        /// Reads the story file data from the specified URL.
        /// </summary>
        /// <returns>The story file data.</returns>
        private byte[] ReadStoryData(Uri uri)
        {
            using (var client = new HttpClient())
            {
                byte[] data = client.GetByteArrayAsync(uri.AbsoluteUri).Result;
                return data;
            }
        }

        /// <summary>
        /// Reads the Blorb data from the specified file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The form chunk.</returns>
        private IFormChunk ReadBlorb(byte[] data)
        {
            IFormChunk blorbchunk = null;
            if (data != null)
            {
                blorbchunk = new FormChunk(new Memory(data));
                if (!"IFRS".Equals(blorbchunk.SubId))
                {
                    throw new IOException($"Is not a valid Blorb file.");
                }
            }
            return blorbchunk;
        }

        #endregion

        /// <summary>
        /// Starts the z-machine.
        /// </summary>
        public void Start()
        {
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
            logger.LogDebug($"Game Default Foreground: {defaultForeground}.");
            logger.LogDebug($"Game Default Background: {defaultBackground}.");
            machine.Screen.SetBackground(defaultBackground, -1);
            machine.Screen.SetForeground(defaultForeground, -1);

            logger.LogInformation("Z-machine started at {dateTime}.", DateTime.UtcNow);

            machine.Start();

            var runstate = Run();
            while (runstate != null && runstate.IsReadLine)
            {
                runstate = Run();
            }

            logger.LogInformation("Z-machine stopped at {dateTime}.", DateTime.UtcNow);
        }

        /// <summary>
        /// Stops the z-machine.
        /// </summary>
        public void Stop()
        {
            // TODO: Implement this method!!!
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
        /// The execution loop.
        /// </summary>
        /// <returns>the new MachineRunState</returns>
        /// <remarks>
        /// It runs until either an input state is reached
        /// or the machine is set to stop state.
        /// </remarks>
        private MachineRunState Run()
        {
            while (machine.RunState != MachineRunState.STOPPED)
            {
                int pc = machine.PC;
                IInstruction instr = instructionDecoder.decodeInstruction(pc);
                // if the print is executed after Execute() the result is different !!
                if (DEBUG && machine.RunState == MachineRunState.RUNNING)
                {
                    Console.Out.WriteLine(string.Format("{0:D4}: ${1:x5} {2}", step, (int)pc, instr.ToString()));
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
