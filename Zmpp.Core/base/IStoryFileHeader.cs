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

namespace org.zmpp.@base
{
    using System;

    /// <summary>
    /// This interface defines the structure of a story file header in the Z-machine.
    /// It is designed as a read only view to the byte array containing the
    /// story file data.
    /// By this means, changes in the memory map will be implicitly change
    /// the header structure.
    /// </summary>
    public interface IStoryFileHeader
    {
        /// <summary>
        /// Returns the story file version.
        /// </summary>
        /// <returns>the story file version</returns>
        int getVersion();

        /// <summary>
        /// Returns this game's serial number.
        /// </summary>
        /// <returns>the serial number</returns>
        String getSerialNumber();

        /// <summary>
        /// Returns this story file's length.
        /// </summary>
        /// <returns>the file length</returns>
        int getFileLength();

        /// <summary>
        /// Sets the interpreter version.
        /// </summary>
        /// <param name="version">the version</param>
        void setInterpreterVersion(int version);

        /// <summary>
        /// Sets the font width in width of a '0'.
        /// </summary>
        /// <param name="units">the number of units in widths of a '0'</param>
        void setFontWidth(int units);

        /// <summary>
        /// Sets the font height in width of a '0'.
        /// </summary>
        /// <param name="units">the number of units in heights of a '0'</param>
        void setFontHeight(int units);

        /// <summary>
        /// Sets the mouse coordinates.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        void setMouseCoordinates(int x, int y);

        /// <summary>
        /// Returns the address of the cutom unicode translation table.
        /// </summary>
        /// <returns>the address of the custom unicode translation table</returns>
        char getCustomAccentTable();

        #region Attributes
        /// <summary>
        /// Enables the specified attribute.
        /// </summary>
        /// <param name="attribute">the attribute to set</param>
        /// <param name="flag">the value</param>
        void setEnabled(StoryFileHeaderAttribute attribute, bool flag);

        /// <summary>
        /// Checks the enabled status of the specified attribute
        /// </summary>
        /// <param name="attribute">the attribute name</param>
        /// <returns>true if enabled, false otherwise</returns>
        bool isEnabled(StoryFileHeaderAttribute attribute);
        #endregion
    }
}
