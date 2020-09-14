/*
 * Created on 10/14/2005
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
    /// <summary>
    /// Represents an object tree.
    /// </summary>
    public interface IObjectTree
    {
        /// <summary>
        /// Removes an object from its parent.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        void RemoveObject(int objectNum);

        /// <summary>
        /// Inserts an object to a new parent.
        /// </summary>
        /// <param name="parentNum">The object number of the parent.</param>
        /// <param name="objectNum">The object number.</param>
        void InsertObject(int parentNum, int objectNum);

        /// <summary>
        /// Gets the length of the property at the specified address.
        /// </summary>
        /// <param name="propertyAddress">The property address.</param>
        /// <returns>The length.</returns>
        /// <remarks>
        /// The address is an address returned by GetPropertyAddress
        /// so it is starting after the length byte.
        /// </remarks>
        int GetPropertyLength(int propertyAddress);

        #region Methods on objects

        /// <summary>
        /// Indicates whether the specified attribute is set
        /// on the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="attributeNum">The zero-based attribute number.</param>
        /// <returns>true if the attribute is set; otherwise false.</returns>
        bool IsAttributeSet(int objectNum, int attributeNum);

        /// <summary>
        /// Sets the specified attribute on the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="attributeNum">The zero-based attribute number.</param>
        void SetAttribute(int objectNum, int attributeNum);

        /// <summary>
        /// Clears the specified attribute on the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="attributeNum">The zero-based attribute number.</param>
        void ClearAttribute(int objectNum, int attributeNum);

        /// <summary>
        /// Gets the object number for the parent of the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The object number of the parent object.</returns>
        int GetParent(int objectNum);

        /// <summary>
        /// Assigns a new parent object for the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="parent">The new parent object.</param>
        void SetParent(int objectNum, int parent);

        /// <summary>
        /// Gets the object number for the sibling of the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The object number of the sibling object.</returns>
        int GetSibling(int objectNum);

        /// <summary>
        /// Assigns a new sibling to the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="sibling">The object number of the new sibling.</param>
        void SetSibling(int objectNum, int sibling);

        /// <summary>
        /// Gets the object number for the child of the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The object number of the child object.</returns>
        int GetChild(int objectNum);

        /// <summary>
        /// Assigns a new child to the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="child">The object number of the child object.</param>
        void SetChild(int objectNum, int child);

        /// <summary>
        /// Gets the properties description address for the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The properties description address.</returns>
        int GetPropertiesDescriptionAddress(int objectNum);

        /// <summary>
        /// Gets the the address of the specified property for the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="property">The property.</param>
        /// <returns>The address of specified property.</returns>
        /// <remarks>This will not include the length byte.</remarks>
        int GetPropertyAddress(int objectNum, int property);

        /// <summary>
        /// Gets the next property in the list.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="property">The property number.</param>
        /// <returns>The next property in the list or 0.</returns>
        /// <remarks>
        /// If property is 0 this will return the first property number.
        /// If property is the last element in the list this will return 0.
        /// </remarks>
        int GetNextProperty(int objectNum, int property);

        /// <summary>
        /// Gets the value of the specified property for the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="property">The property number.</param>
        /// <returns>The value of the specified property.</returns>
        char GetProperty(int objectNum, int property);

        /// <summary>
        /// Sets the value of the specified property for the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        void SetProperty(int objectNum, int property, char value);

        #endregion
    }
}
