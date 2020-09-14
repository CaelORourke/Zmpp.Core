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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Zmpp.Core.UI;
    using Zmpp.Core;
    using Zmpp.Core.Blorb;
    using Zmpp.Core.Iff;
    using Zmpp.Core.IO;
    using Zmpp.Core.Media;
    using Zmpp.Core.Vm.Utility;
    using System.IO;
    using System.Net.Http;

    /// <summary>
    /// Provides methods for constructing a machine object.
    /// </summary>
    /// <remarks>
    /// Constructing a Machine object is a very complex task, the building process
    /// deals with creating the game objects, the UI and the I/O system.
    /// Initialization was changed so it is not necessary to create a subclass
    /// of MachineFactory. Instead, an init struct and a init callback object
    /// should be provided.
    /// </remarks>
    public class MachineFactory
    {
        /// <summary>
        /// Initialization structure.
        /// </summary>
        public sealed class MachineInitStruct
        {
            public string storyFile, blorbFile;
            public Uri storyURL, blorbURL;
            public IInputStream keyboardInputStream;
            public IStatusLine statusLine;
            public IScreenModel screenModel;
            public IIOSystem ioSystem;
            public ISaveGameDataStore saveGameDataStore;
            public INativeImageFactory nativeImageFactory;
            public ISoundEffectFactory soundEffectFactory;
        }

        private readonly ILogger logger;
        private readonly MachineInitStruct initStruct;
        private readonly IFormChunk blorbchunk;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initStruct">The initialization structure.</param>
        public MachineFactory(ILogger logger, MachineInitStruct initStruct)
        {
            this.logger = logger;
            this.initStruct = initStruct;
        }

        /// <summary>
        /// This is the main creation function.
        /// </summary>
        /// <returns>the machine</returns>
        public IMachine BuildMachine()
        {
            MachineImpl machine = new MachineImpl(logger);
            machine.Initialize(ReadStoryData(), ReadResources());
            if (IsInvalidStory(machine.Version)) {
                throw new InvalidStoryFileException();
            }
            InitIOSystem(machine);
            return machine;
        }

        #region Helpers

        /// <summary>
        /// Reads the story data.
        /// </summary>
        /// <returns>the story data</returns>
        private byte[] ReadStoryData()
        {
            if (initStruct.storyFile != null || initStruct.blorbFile != null)
                return ReadStoryDataFromFile();
            //if (initStruct.storyURL != null || initStruct.blorbURL != null)
            //    return readStoryDataFromUrl();
            return null;
        }

        ///// <summary>
        ///// Reads the story file from the specified URL.
        ///// </summary>
        ///// <returns>byte data</returns>
        //private byte[] readStoryDataFromUrl()
        //{
        //    //java.io.InputStream storyis = null, blorbis = null;
        //    Stream storyis = null, blorbis = null;

        //    try
        //    {
        //        if (initStruct.storyURL != null)
        //        {
        //            //storyis = initStruct.storyURL.openStream();
        //            storyis = new HttpClient().GetStreamAsync(initStruct.storyURL).Result;
        //        }
        //        //if (initStruct.blorbURL != null)
        //        //{
        //        //    blorbis = initStruct.blorbURL.openStream();
        //        //}
        //    } catch (Exception ex) {
        //        logger.LogError(ex.StackTrace);
        //    }

        //    if (storyis != null) {
        //        //return FileUtils.readFileBytes(storyis);
        //        MemoryStream ms = storyis as MemoryStream;
        //        if (ms != null)
        //            return ms.ToArray();
        //    }
        //    //else
        //    //{
        //    //    return new BlorbFile(readBlorb(blorbis)).getStoryData();
        //    //}
        //}

        /// <summary>
        /// Reads story data from file.
        /// </summary>
        /// <returns>byte data</returns>
        private byte[] ReadStoryDataFromFile()
        {
            if (initStruct.storyFile != null) {
                return File.ReadAllBytes(initStruct.storyFile);
            }
            //else
            //{
            //    // Read from Z BLORB
            //    FormChunk formchunk = readBlorbFromFile();
            //    return formchunk != null ? new BlorbFile(formchunk).getStoryData() : null;
            return null;
            //}
        }

        /// <summary>
        /// Reads the resource data.
        /// </summary>
        /// <returns>the resource data</returns>
        protected IResources ReadResources()
        {
            // TODO: Implement blorb namespace!
            //if (initStruct.blorbFile != null) return readResourcesFromFile();
            //if (initStruct.blorbURL != null) return readResourcesFromUrl();
            return null;
        }

        ///**
        // * Reads Blorb data from file.
        // * @return the data's form chunk
        // * @throws IOException if i/o error occurred
        // */
        //private FormChunk readBlorbFromFile() throws IOException
        //{
        //    if (blorbchunk == null) {
        //        byte[] data = FileUtils.readFileBytes(initStruct.blorbFile);
        //        if (data != null)
        //        {
        //            blorbchunk = new DefaultFormChunk(new DefaultMemory(data));
        //            if (!"IFRS".equals(blorbchunk.getSubId()))
        //            {
        //                throw new IOException("not a valid Blorb file");
        //            }
        //        }
        //    }
        //    return blorbchunk;
        //}

        ///**
        // * Reads story resources from input blorb file.
        // * @return resources object
        // * @throws IOException if i/o error occurred
        // */
        //private Resources readResourcesFromFile() throws IOException
        //{
        //    FormChunk formchunk = readBlorbFromFile();
        //    return (formchunk != null) ?
        //      new BlorbResources(initStruct.nativeImageFactory,
        //                         initStruct.soundEffectFactory, formchunk) : null;
        //  }

        //  /**
        //   * Reads Blorb's form chunk from the specified input stream object.
        //   * @param blorbis input stream
        //   * @return the form chunk
        //   * @throws IOException i/o error occurred
        //   */
        //  private FormChunk readBlorb(java.io.InputStream blorbis) throws IOException
        //{
        //    if (blorbchunk == null) {
        //        byte[] data = FileUtils.readFileBytes(blorbis);
        //        if (data != null)
        //        {
        //            blorbchunk = new DefaultFormChunk(new DefaultMemory(data));
        //        }
        //    }
        //    return blorbchunk;
        //}

        ///**
        // * Reads story resources from URL.
        // * @return resources object
        // * @throws IOException i/o error occurred
        // */
        //private Resources readResourcesFromUrl() throws IOException
        //{
        //    FormChunk formchunk = readBlorb(initStruct.blorbURL.openStream());
        //    return (formchunk != null) ?
        //      new BlorbResources(initStruct.nativeImageFactory,
        //                         initStruct.soundEffectFactory, formchunk) : null;
        //  }

        #endregion

        #region Private methods

        /// <summary>
        /// Checks the story file version.
        /// </summary>
        /// <param name="version">the story file version</param>
        /// <returns>true if not supported</returns>
        private bool IsInvalidStory(int version)
        {

            return version < 1 || version > 8;
        }

        /// <summary>
        /// Initializes the I/O system.
        /// </summary>
        /// <param name="machine">the machine object</param>
        private void InitIOSystem(MachineImpl machine)
        {
            InitInputStreams(machine);
            InitOutputStreams(machine);
            machine.SetStatusLine(initStruct.statusLine);
            machine.SetScreen(initStruct.screenModel);
            machine.SetSaveGameDataStore(initStruct.saveGameDataStore);
        }

        /// <summary>
        /// Initializes the input streams.
        /// </summary>
        /// <param name="machine">the machine object</param>
        private void InitInputStreams(MachineImpl machine)
        {
            machine.setInputStream(0, initStruct.keyboardInputStream);
            // TODO: Implement FileInputStream!
            //machine.setInputStream(1, new FileInputStream(logger, initStruct.ioSystem, machine));
        }

        /// <summary>
        /// Initializes the output streams.
        /// </summary>
        /// <param name="machine">the machine object</param>
        private void InitOutputStreams(MachineImpl machine)
        {
            machine.setOutputStream(1, initStruct.screenModel.getOutputStream());
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
