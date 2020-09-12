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
    /// This is the interface definition of the object tree.
    /// </summary>
    public interface IObjectTree
    {
        /// <summary>
        /// Removes an object from its parent.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        void removeObject(int objectNum);

        /// <summary>
        /// Inserts an object to a new parent.
        /// </summary>
        /// <param name="parentNum">the parent number</param>
        /// <param name="objectNum">the object number</param>
        void insertObject(int parentNum, int objectNum);

        /// <summary>
        /// Determines the length of the property at the specified address.
        /// The address is an address returned by ZObject.getPropertyAddress,
        /// i.e.it is starting after the length byte.
        /// </summary>
        /// <param name="propertyAddress">the property address</param>
        /// <returns>the length</returns>
        int getPropertyLength(int propertyAddress);

        #region Methods on objects

        /// <summary>
        /// Tests if the specified attribute is set.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="attributeNum">the attribute number, starting with 0</param>
        /// <returns>true if the attribute is set</returns>
        bool isAttributeSet(int objectNum, int attributeNum);

        /// <summary>
        /// Sets the specified attribute.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="attributeNum">the attribute number, starting with 0</param>
        void setAttribute(int objectNum, int attributeNum);

        /// <summary>
        /// Clears the specified attribute.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="attributeNum">the attribute number, starting with 0</param>
        void clearAttribute(int objectNum, int attributeNum);

        /// <summary>
        /// Returns the number of this object's parent object.
        /// </summary>
        /// <param name="objectNum">object number</param>
        /// <returns>the parent object's number</returns>
        int getParent(int objectNum);

        /// <summary>
        /// Assigns a new parent object.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="parent">the new parent object</param>
        void setParent(int objectNum, int parent);

        /// <summary>
        /// Returns the object number of this object's sibling object.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <returns>the sibling object's object number</returns>
        int getSibling(int objectNum);

        /// <summary>
        /// Assigns a new sibling to this object.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="sibling">the new sibling's object number</param>
        void setSibling(int objectNum, int sibling);

        /// <summary>
        /// Returns the object number of this object's child object.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <returns>the child object's object number</returns>
        int getChild(int objectNum);

        /// <summary>
        /// Assigns a new child to this object.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="child">the new child</param>
        void setChild(int objectNum, int child);

        /// <summary>
        /// Returns the properties description address.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <returns>the description address</returns>
        int getPropertiesDescriptionAddress(int objectNum);

        /// <summary>
        /// Returns the address of the specified property. Note that this will not
        /// include the length byte.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="property">the property</param>
        /// <returns>the specified property's address</returns>
        int getPropertyAddress(int objectNum, int property);

        /// <summary>
        /// Returns the next property in the list. If property is 0, this
        /// will return the first property number, if property is the last
        /// element in the list, it will return 0.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="property">the property number</param>
        /// <returns>the next property in the list or 0</returns>
        int getNextProperty(int objectNum, int property);

        /// <summary>
        /// Returns the the specified property.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="property">the property number</param>
        /// <returns>the value of the specified property</returns>
        char getProperty(int objectNum, int property);

        /// <summary>
        /// Sets the specified property byte to the given value.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="property">the property</param>
        /// <param name="value">the value</param>
        void setProperty(int objectNum, int property, char value);

        #endregion
    }
}
