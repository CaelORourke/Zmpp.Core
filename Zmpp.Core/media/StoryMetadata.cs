/*
 * Created on 2006/03/10
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

namespace Zmpp.Core.Media
{
    using System;
    using System.Text;

    /// <summary>
    /// This class holds information about a story.
    /// </summary>
    public class StoryMetadata
    {
        private const char NEWLINE = '\n';

        private String title;
        private String headline;
        private String author;
        private String genre;
        private String description;
        private String year;
        private int coverpicture;
        private String group;

        /// <summary>
        /// Returns the story title.
        /// </summary>
        /// <returns>story title</returns>
        public String getTitle() { return title; }

        /// <summary>
        /// Sets the story title.
        /// </summary>
        /// <param name="title">story title</param>
        public void setTitle(String title) { this.title = title; }

        /// <summary>
        /// Returns the headline.
        /// </summary>
        /// <returns>headline</returns>
        public String getHeadline() { return headline; }

        /// <summary>
        /// Sets the headline.
        /// </summary>
        /// <param name="headline">the headline</param>
        public void setHeadline(String headline) { this.headline = headline; }

        /// <summary>
        /// Returns the author.
        /// </summary>
        /// <returns>author</returns>
        public String getAuthor() { return author; }

        /// <summary>
        /// Sets the author.
        /// </summary>
        /// <param name="author">author</param>
        public void setAuthor(String author) { this.author = author; }

        /// <summary>
        /// Returns the genre.
        /// </summary>
        /// <returns>genre</returns>
        public String getGenre() { return genre; }

        /// <summary>
        /// Sets the genre.
        /// </summary>
        /// <param name="genre">genre</param>
        public void setGenre(String genre) { this.genre = genre; }

        /// <summary>
        /// Returns the description.
        /// </summary>
        /// <returns>description</returns>
        public String getDescription() { return description; }

        /// <summary>
        /// Sets the description.
        /// </summary>
        /// <param name="description">description</param>
        public void setDescription(String description)
        {
            this.description = description;
        }

        /// <summary>
        /// Returns the year.
        /// </summary>
        /// <returns>year</returns>
        public String getYear() { return year; }

        /// <summary>
        /// Sets the year.
        /// </summary>
        /// <param name="year">year</param>
        public void setYear(String year) { this.year = year; }

        /// <summary>
        /// Returns the cover picture number.
        /// </summary>
        /// <returns>cover picture number</returns>
        public int getCoverPicture() { return coverpicture; }

        /// <summary>
        /// Sets the cover picture number.
        /// </summary>
        /// <param name="picnum"></param>
        public void setCoverPicture(int picnum) { this.coverpicture = picnum; }

        /// <summary>
        /// Returns the group.
        /// </summary>
        /// <returns>group</returns>
        public String getGroup() { return group; }

        /// <summary>
        /// Sets the group.
        /// </summary>
        /// <param name="group">group</param>
        public void setGroup(String group) { this.group = group; }

        public String toString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Title: '" + title + NEWLINE);
            builder.Append("Headline: '" + headline + NEWLINE);
            builder.Append("Author: '" + author + NEWLINE);
            builder.Append("Genre: '" + genre + NEWLINE);
            builder.Append("Description: '" + description + NEWLINE);
            builder.Append("Year: '" + year + NEWLINE);
            builder.Append("Cover picture: " + coverpicture + NEWLINE);
            builder.Append("Group: '" + group + NEWLINE);
            return builder.ToString();
        }
    }
}
