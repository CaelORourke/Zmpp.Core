/*
 * Created on 2006/02/15
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
    using Microsoft.Extensions.Logging;
    using Zmpp.Core.UI;

    /// <summary>
    /// Provides static methods for constructing a machine object.
    /// </summary>
    /// <remarks>
    /// Constructing a Machine object is a very complex task, the building process
    /// deals with creating the game objects, the UI and the I/O system.
    /// Initialization was changed so it is not necessary to create a subclass
    /// of MachineFactory. Instead, an init struct and a init callback object
    /// should be provided.
    /// </remarks>
    public static class MachineFactory
    {
        public static IMachine Create(ILogger logger, IViewModel viewModel)
        {
            Machine machine = new Machine(logger);
            InitIOSystem(machine, viewModel);
            return machine;
        }

        #region Private methods

        /// <summary>
        /// Initializes the I/O system.
        /// </summary>
        /// <param name="machine">the machine object</param>
        private static void InitIOSystem(Machine machine, IViewModel viewModel)
        {
            InitInputStreams(machine, viewModel);
            InitOutputStreams(machine, viewModel);
            machine.SetStatusLine(viewModel.StatusLine);
            machine.SetScreen(viewModel.ScreenModel);
            machine.SetSaveGameDataStore(viewModel.SaveGameDataStore);
        }

        /// <summary>
        /// Initializes the input streams.
        /// </summary>
        /// <param name="machine">the machine object</param>
        private static void InitInputStreams(Machine machine, IViewModel viewModel)
        {
            machine.setInputStream(0, viewModel.InputStream);
            // TODO: Implement FileInputStream!
            //machine.setInputStream(1, new FileInputStream(logger, initStruct.ioSystem, machine));
        }

        /// <summary>
        /// Initializes the output streams.
        /// </summary>
        /// <param name="machine">the machine object</param>
        private static void InitOutputStreams(Machine machine, IViewModel viewModel)
        {
            machine.setOutputStream(1, viewModel.ScreenModel.OutputStream);
            machine.SelectOutputStream(1, true);
            // TODO: Implement TranscriptOutputStream!
            //machine.setOutputStream(2, new TranscriptOutputStream(logger, initStruct.ioSystem, machine));
            //machine.selectOutputStream(2, false);
            machine.setOutputStream(3, new MemoryOutputStream(machine));
            machine.SelectOutputStream(3, false);
        }

        #endregion
    }
}
