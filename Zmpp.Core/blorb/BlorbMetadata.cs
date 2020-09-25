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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Zmpp.Core;
    using Zmpp.Core.Iff;

    /// <summary>
    /// This class parses the metadata chunk in the Blorb file and converts
    /// it into a Treaty of Babel metadata object.
    /// </summary>
    public static class BlorbMetadata
    {
        private static readonly ILogger logger;

        /// <summary>
        /// Extracts inform meta data from the specified FORM chunk.
        /// </summary>
        /// <param name="formchunk">The FORM chunk.</param>
        public static List<StoryMetadata> Parse(IFormChunk formchunk)
        {
            // an ifindex can hold multiple stories
            List<StoryMetadata> storyMetadata = new List<StoryMetadata>();

            IChunk chunk = formchunk.GetSubChunk("IFmd");
            if (chunk != null)
            {
                IMemory chunkmem = chunk.Memory;

                byte[] metadata = new byte[chunk.Size + Chunk.ChunkHeaderLength];
                chunkmem.CopyBytesToArray(metadata, 0, Chunk.ChunkHeaderLength, chunk.Size);

                XDocument xDoc = new XDocument();

                using (MemoryStream ms = new MemoryStream(metadata))
                {
                    using (ForgivingXmlStreamReader sr = new ForgivingXmlStreamReader(ms, ' '))
                    {
                        xDoc = XDocument.Load(sr);
                    }
                }

                XNamespace ns = xDoc.Root.GetDefaultNamespace();

                foreach (var storyFileRecords in xDoc.Root.Elements(ns.GetName("story")))
                {
                    var story = new StoryMetadata();
                    storyMetadata.Add(story);

                    var identification = storyFileRecords.Element(ns.GetName("identification"));
                    if (identification != null)
                    {
                        story.Identification = new StoryMetadata.IdentificationSection()
                        {
                            Ifids = new List<string>()
                        };
                        var ifids = identification.Elements(ns.GetName("ifid"));
                        foreach (var ifid in ifids)
                        {
                            story.Identification.Ifids.Add(ifid.Value);
                        }
                        story.Identification.Format = (string)identification.Element(ns.GetName("format"));
                        story.Identification.Bafn = (string)identification.Element(ns.GetName("bafn"));
                    }

                    var bibliographic = storyFileRecords.Element(ns.GetName("bibliographic"));
                    if (bibliographic != null)
                    {
                        story.Bibliographic = new StoryMetadata.BibliographicSection();
                        story.Bibliographic.Title = (string)bibliographic.Element(ns.GetName("title"));
                        story.Bibliographic.Author = (string)bibliographic.Element(ns.GetName("author"));
                        story.Bibliographic.Language = (string)bibliographic.Element(ns.GetName("language"));
                        story.Bibliographic.Headline = (string)bibliographic.Element(ns.GetName("headline"));
                        story.Bibliographic.FirstPublished = (string)bibliographic.Element(ns.GetName("firstpublished"));
                        story.Bibliographic.Genre = (string)bibliographic.Element(ns.GetName("genre"));
                        story.Bibliographic.Group = (string)bibliographic.Element(ns.GetName("group"));
                        story.Bibliographic.Series = (string)bibliographic.Element(ns.GetName("series"));
                        story.Bibliographic.SeriesNumber = (string)bibliographic.Element(ns.GetName("seriesnumber"));
                        story.Bibliographic.Forgiveness = (string)bibliographic.Element(ns.GetName("forgiveness"));
                        story.Bibliographic.Description = (string)bibliographic.Element(ns.GetName("description"));
                    }

                    var resources = storyFileRecords.Element(ns.GetName("resources"));
                    if (resources != null)
                    {
                        story.Resources = new StoryMetadata.ResourcesSection()
                        {
                            Auxiliaries = new List<StoryMetadata.Auxiliary>()
                        };
                        var auxiliaries = resources.Elements(ns.GetName("auxiliary"));
                        foreach (var auxiliary in auxiliaries)
                        {
                            var aux = new StoryMetadata.Auxiliary();
                            aux.LeafName = (string)auxiliary.Element(ns.GetName("leafname"));
                            aux.Description = (string)auxiliary.Element(ns.GetName("description"));
                            story.Resources.Auxiliaries.Add(aux);
                        }
                    }

                    var contacts = storyFileRecords.Element(ns.GetName("contacts"));
                    if (contacts != null)
                    {
                        story.Contacts = new StoryMetadata.ContactsSection();
                        story.Contacts.Url = (string)contacts.Element(ns.GetName("url"));
                        story.Contacts.AuthorEmail = (string)contacts.Element(ns.GetName("authoremail"));
                    }

                    var cover = storyFileRecords.Element(ns.GetName("cover"));
                    if (cover != null)
                    {
                        story.Cover = new StoryMetadata.CoverSection();
                        story.Cover.Format = (string)cover.Element(ns.GetName("format"));
                        story.Cover.Height = (string)cover.Element(ns.GetName("height"));
                        story.Cover.Width = (string)cover.Element(ns.GetName("width"));
                        story.Cover.Description = (string)cover.Element(ns.GetName("description"));
                    }
                }
            }

            return storyMetadata;
        }

        #region Parsing meta data

        /// <summary>
        /// Implements a stream reader that will replace invalid XML characters in the underlying stream.
        /// </summary>
        private class ForgivingXmlStreamReader : StreamReader
        {
            private readonly char replacementCharacter;

            public ForgivingXmlStreamReader(Stream stream, char replacementCharacter) : base(stream)
            {
                this.replacementCharacter = replacementCharacter;
            }

            public override int Peek()
            {
                int character = base.Peek();
                if (character != -1 && IsInvalidXmlCharacter(character))
                {
                    return this.replacementCharacter;
                }
                return character;
            }

            public override int Read()
            {
                int character = base.Read();
                if (character != -1 && IsInvalidXmlCharacter(character))
                {
                    return this.replacementCharacter;
                }
                return character;
            }

            public override int Read(char[] buffer, int index, int count)
            {
                int readCount = base.Read(buffer, index, count);
                for (int i = index; i < readCount + index; i++)
                {
                    char ch = buffer[i];
                    if (IsInvalidXmlCharacter(ch))
                    {
                        buffer[i] = this.replacementCharacter;
                    }
                }
                return readCount;
            }

            private static bool IsInvalidXmlCharacter(int character)
            {
                return (character < 0x0020 || character > 0xD7FF) &&
                       (character < 0xE000 || character > 0xFFFD) &&
                        character != 0x0009 &&
                        character != 0x000A &&
                        character != 0x000D;
            }
        }

        #endregion
    }
}
