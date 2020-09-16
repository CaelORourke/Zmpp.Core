/*
 * Created on 2006/03/04
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

namespace Zmpp.Core.Blorb
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Text;
    //import javax.xml.parsers.SAXParser;
    //import javax.xml.parsers.SAXParserFactory;
    //import org.xml.sax.Attributes;
    //import org.xml.sax.helpers.DefaultHandler;
    using Zmpp.Core;
    using Zmpp.Core.Iff;
    using Zmpp.Core.Media;

    /// <summary>
    /// This class parses the metadata chunk in the Blorb file and converts
    /// it into a Treaty of Babel metadata object.
    /// </summary>
    public class BlorbMetadataHandler
    {
        private static readonly ILogger LOG;
        private StoryMetadata story;
        private StringBuilder buffer;
        private bool processAux;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formchunk">The FORM chunk.</param>
        public BlorbMetadataHandler(IFormChunk formchunk)
        {
            ExtractMetadata(formchunk);
        }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        public InformMetadata Metadata => (story == null) ? null : new InformMetadata(story);

        /// <summary>
        /// Extracts inform meta data from the specified FORM chunk.
        /// </summary>
        /// <param name="formchunk">The FORM chunk.</param>
        private void ExtractMetadata(IFormChunk formchunk)
        {
            // TODO: Implement this method!!!
            //IChunk chunk = formchunk.GetSubChunk("IFmd");
            //if (chunk != null)
            //{
            //    IMemory chunkmem = chunk.Memory;
            //    MemoryInputStream meminput =
            //        new MemoryInputStream(chunkmem, Chunk.ChunkHeaderLength, chunk.Size + Chunk.ChunkHeaderLength);

            //    try
            //    {
            //        SAXParser parser = SAXParserFactory.newInstance().newSAXParser();
            //        parser.parse(meminput, this);
            //    }
            //    catch (Exception)
            //    {
            //        //ex.printStackTrace();
            //    }
            //}
        }

        #region Parsing meta data

        // TODO: Implement this method!!!
        //public void startElement(string uri, string localName, string qname, Attributes attributes)
        //{
        //    if ("story".Equals(qname))
        //    {
        //        story = new StoryMetadata();
        //    }
        //    if ("title".Equals(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //    if ("headline".Equals(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //    if ("author".Equals(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //    if ("genre".Equals(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //    if ("description".Equals(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //    if (isPublishYear(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //    if ("auxiliary".Equals(qname))
        //    {
        //        processAux = true;
        //    }
        //    if ("coverpicture".Equals(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //    if ("group".Equals(qname))
        //    {
        //        buffer = new StringBuilder();
        //    }
        //}

        // TODO: Implement this method!!!
        //public void endElement(string uri, string localName, string qname)
        //{
        //    if ("title".Equals(qname))
        //    {
        //        story.setTitle(buffer.ToString());
        //    }
        //    if ("headline".Equals(qname))
        //    {
        //        story.setHeadline(buffer.ToString());
        //    }
        //    if ("author".Equals(qname))
        //    {
        //        story.setAuthor(buffer.ToString());
        //    }
        //    if ("genre".Equals(qname))
        //    {
        //        story.setGenre(buffer.ToString());
        //    }
        //    if ("description".Equals(qname) && !processAux)
        //    {
        //        story.setDescription(buffer.ToString());
        //    }
        //    if (isPublishYear(qname))
        //    {
        //        story.setYear(buffer.ToString());
        //    }
        //    if ("group".Equals(qname))
        //    {
        //        story.setGroup(buffer.ToString());
        //    }
        //    if ("coverpicture".Equals(qname))
        //    {
        //        string val = buffer.ToString().Trim();
        //        try
        //        {
        //            story.setCoverPicture(Integer.parseInt(val));
        //        }
        //        catch (NumberFormatException ex)
        //        {
        //            LOG.throwing("BlorbMetadataHandler", "endElement", ex);
        //        }
        //    }
        //    if ("auxiliary".Equals(qname))
        //    {
        //        processAux = false;
        //    }
        //    if ("br".Equals(qname) && buffer != null)
        //    {
        //        buffer.Append("\n");
        //    }
        //}

        public void characters(char[] ch, int start, int length)
        {
            if (buffer != null)
            {
                StringBuilder partbuilder = new StringBuilder();
                for (int i = start; i < start + length; i++)
                {
                    partbuilder.Append(ch[i]);
                }
                buffer.Append(partbuilder.ToString().Trim());
            }
        }

        /**
         * Unfortunately, year was renamed to firstpublished between the preview
         * metadata version of Inform 7 and the Treaty of Babel version, so
         * we handle both here.
         *
         * @param str the qname
         * @return true if matches, false, otherwise
         */
        private bool isPublishYear(String str)
        {
            return "year".Equals(str) || "firstpublished".Equals(str);
        }

        #endregion
    }
}
