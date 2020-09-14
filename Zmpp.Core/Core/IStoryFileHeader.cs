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
    /// <summary>
    /// Manages read and write access to the story file header.
    /// </summary>
    public interface IStoryFileHeader
    {
        /// <summary>
        /// Gets the story file version.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets the story file serial number.
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// Gets the story file length.
        /// </summary>
        int FileLength { get; }

        /// <summary>
        /// Gets the address of the custom Unicode translation table.
        /// </summary>
        char CustomAccentTableAddress { get; }

        /// <summary>
        /// Sets the interpreter version.
        /// </summary>
        /// <param name="version">The version.</param>
        void SetInterpreterVersion(int version);

        /// <summary>
        /// Sets the font width.
        /// </summary>
        /// <param name="units">The number of units in width of a '0'.</param>
        void SetFontWidth(int units);

        /// <summary>
        /// Sets the font height.
        /// </summary>
        /// <param name="units">The number of units in height of a '0'.</param>
        void SetFontHeight(int units);

        /// <summary>
        /// Sets the mouse coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        void SetMouseCoordinates(int x, int y);

        #region Attributes

        /// <summary>
        /// Sets the enabled status of the specified attribute.
        /// </summary>
        /// <param name="attribute">The story file header attribute.</param>
        /// <param name="flag">The enabled status.</param>
        void SetEnabled(StoryFileHeaderAttribute attribute, bool flag);

        /// <summary>
        /// Gets the enabled status of the specified attribute.
        /// </summary>
        /// <param name="attribute">The story file header attribute.</param>
        /// <returns>true if the attribute is enabled; otherwise, false.</returns>
        bool IsEnabled(StoryFileHeaderAttribute attribute);

        #endregion
    }
}
