namespace Zmpp.Core.Blorb
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents metadata for a story file.
    /// </summary>
    public class StoryMetadata
    {
        /// <summary>
        /// The identification section.
        /// </summary>
        /// <remarks>
        /// This section is mandatory.
        /// Created by the design system.
        /// </remarks>
        public IdentificationSection Identification { get; set; }

        /// <summary>
        /// The bibliographic section.
        /// </summary>
        /// <remarks>
        /// This section is mandatory.
        /// Created by the author.
        /// </remarks>
        public BibliographicSection Bibliographic { get; set; }

        /// <summary>
        /// The resources section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the author.
        /// </remarks>
        public ResourcesSection Resources { get; set; }

        /// <summary>
        /// The contacts section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the author.
        /// </remarks>
        public ContactsSection Contacts { get; set; }

        /// <summary>
        /// The cover section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        public CoverSection Cover { get; set; }

        // TODO: Implement the remaining metadata sections!

        /// <summary>
        /// The releases section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        //public ReleasesSection Releases { get; set; }

        /// <summary>
        /// The colophon section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system, or browser tool, etc.
        /// </remarks>
        //public ColophonSection Colophon { get; set; }

        /// <summary>
        /// The annotation section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by third parties, player's tools, etc.
        /// </remarks>
        //public AnnotationSection Annotation { get; set; }

        /// <summary>
        /// The zcode section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        //public ZcodeSection Zcode { get; set; }

        /// <summary>
        /// The tads2 section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        //public Tads2Section Tads2 { get; set; }

        /// <summary>
        /// The tads3 section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        //public Tads3Section Tads3 { get; set; }

        /// <summary>
        /// The glulx section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        //public GlulxSection Glulx { get; set; }

        /// <summary>
        /// The hugo section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        //public HugoSection Hugo { get; set; }

        /// <summary>
        /// The adrift section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        //public AdriftSection Adrift { get; set; }

        /// <summary>
        /// The identification section.
        /// </summary>
        /// <remarks>
        /// This section is mandatory.
        /// Created by the design system.
        /// </remarks>
        /// <example>
        /// <code>
        /// <identification>
        ///     <ifid>731F0480-5CB5-4340-B65B-384FC6B1F5B4</ifid>
        ///     <ifid>ZCODE-8-040205-6630</ifid>
        ///     <format>zcode</format>
        /// </identification>
        /// </code>
        /// </example>
        public class IdentificationSection
        {
            public List<string> Ifids { get; set; }
            public string Format { get; set; }//zcode, glulx, tads2, tads3, hugo, alan, adrift, level9, agt, magscrolls, advsys, html, executable
            public string Bafn { get; set; }
        }

        /// <summary>
        /// The bibliographic section.
        /// </summary>
        /// <remarks>
        /// This section is mandatory.
        /// Created by the author.
        /// </remarks>
        /// <example>
        /// <code>
        /// <bibliographic>
        ///     <title>Lakeside Living</title>
        ///     <author>Emily Short</author>
        ///     <language>en-US</language>
        ///     <headline>An Interactive Example</headline>
        ///     <firstpublished>2006</firstpublished>
        ///     <genre>Fiction</genre>
        ///     <group>Inform</group>
        ///     <forgiveness>Merciful</forgiveness>
        ///     <description>This is example 194, for what it's worth.</description>
        /// </bibliographic>
        /// </code>
        /// </example>
        public class BibliographicSection
        {
            /// <summary>
            /// Gets or sets the story title.
            /// </summary>
            /// <remarks>This property is mandatory.</remarks>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the author.
            /// </summary>
            /// <remarks>This property is mandatory.</remarks>
            public string Author { get; set; }

            public string Language { get; set; }

            /// <summary>
            /// Gets or sets the headline.
            /// </summary>
            public string Headline { get; set; }

            // also called year (in older formats)
            public string FirstPublished { get; set; }

            /// <summary>
            /// Gets or sets the genre.
            /// </summary>
            public string Genre { get; set; }

            /// <summary>
            /// Gets or sets the group.
            /// </summary>
            public string Group { get; set; }

            public string Series { get; set; }

            public string SeriesNumber { get; set; }

            public string Forgiveness { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// The resources section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the author.
        /// </remarks>
        /// <example>
        /// <code>
        /// <resources>
        ///     <auxiliary>
        ///     </auxiliary>
        /// </resources>
        /// </code>
        /// </example>
        public class ResourcesSection
        {
            public List<Auxiliary> Auxiliaries { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        /// <auxiliary>
        ///     <leafname>Bronze Manual.pdf</leafname>
        ///     <description>Manual</description>
        /// </auxiliary>
        /// </code>
        /// </example>
        public class Auxiliary
        {
            public string LeafName { get; set; }
            public string Description { get; set; }
        }

        /// <summary>
        /// The contacts section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the author.
        /// </remarks>
        /// <example>
        /// <code>
        /// <contacts>
        ///     <url>http://www.inform-fiction.org/</url>
        ///     <authoremail>graham @gnelson.demon.co.uk</authoremail>
        /// </contacts>
        /// </code>
        /// </example>
        public class ContactsSection
        {
            public string Url { get; set; }
            public string AuthorEmail { get; set; }
        }

        /// <summary>
        /// The cover section.
        /// </summary>
        /// <remarks>
        /// This section is optional.
        /// Created by the design system.
        /// </remarks>
        /// <example>
        /// <code>
        /// <cover>
        ///     <format>jpg</format>
        ///     <height>960</height>
        ///     <width>960</width>
        ///     <description>A man wearing an unusual hat.</description>
        /// </cover>
        /// </code>
        /// </example>
        public class CoverSection
        {
            public string Format { get; set; }
            public string Height { get; set; }
            public string Width { get; set; }
            public string Description { get; set; }
        }
    }
}
