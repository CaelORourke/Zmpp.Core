/*
 * Created on 2006/03/05
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
    using System;
    using Zmpp.Core;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This class is the abstract super class of object trees.
    /// </summary>
    public abstract class ObjectTree : IObjectTree
    {
        private readonly IMemory memory;
        private readonly int address;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zmpp.Core.Vm.ObjectTree"/>
        /// class for the specified memory and address.
        /// </summary>
        /// <param name="memory">The Memory object.</param>
        /// <param name="address">The start address of the object tree.</param>
        public ObjectTree(IMemory memory, int address)
        {
            this.memory = memory;
            this.address = address;
        }

        /// <summary>
        /// Gets the memory.
        /// </summary>
        protected IMemory Memory => memory;

        /// <summary>
        /// Gets the start address of this object tree.
        /// </summary>
        protected int Address => address;

        /// <summary>
        /// Gets the address of the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The object address.</returns>
        protected abstract int GetObjectAddress(int objectNum);

        public abstract int GetPropertyLength(int propertyAddress);

        public abstract int GetParent(int objectNum);

        public abstract void SetParent(int objectNum, int parent);

        public abstract int GetSibling(int objectNum);

        public abstract void SetSibling(int objectNum, int sibling);

        public abstract int GetChild(int objectNum);

        public abstract void SetChild(int objectNum, int child);

        public void RemoveObject(int objectNum)
        {
            int oldParent = GetParent(objectNum);
            SetParent(objectNum, 0);

            if (oldParent != 0)
            {
                if (GetChild(oldParent) == objectNum)
                {
                    SetChild(oldParent, GetSibling(objectNum));
                }
                else
                {
                    // Find the child that comes directly before the removed
                    // node and set the direct sibling of the removed node as
                    // its new sibling
                    int currentChild = GetChild(oldParent);
                    int sibling = GetSibling(currentChild);

                    // We have to handle the case that in fact that object is a child
                    // of its parent, but not directly (happens for some reasons).
                    // We stop in this case and simply remove the object from its
                    // parent, probably the object tree modification routines should
                    // be reverified
                    while (sibling != 0 && sibling != objectNum)
                    {
                        currentChild = sibling;
                        sibling = GetSibling(currentChild);
                    }
                    // sibling might be 0, in that case, the object is not
                    // in the hierarchy
                    if (sibling == objectNum)
                    {
                        SetSibling(currentChild, GetSibling(objectNum));
                    }
                }
            }
            SetSibling(objectNum, 0);
        }

        public void InsertObject(int parentNum, int objectNum)
        {
            // we want to ensure, the child has no old parent relationships
            if (GetParent(objectNum) > 0)
            {
                RemoveObject(objectNum);
            }
            int oldChild = GetChild(parentNum);
            SetParent(objectNum, parentNum);
            SetChild(parentNum, objectNum);
            SetSibling(objectNum, oldChild);
        }

        /// <summary>
        /// Gets the size of the property defaults section.
        /// </summary>
        protected abstract int PropertyDefaultsSize { get; }

        /// <summary>
        /// Gets the start address of the object tree section.
        /// </summary>
        protected int ObjectTreeStart => Address + PropertyDefaultsSize;

        /// <summary>
        /// Gets the story file version specific object entry size.
        /// </summary>
        protected abstract int ObjectEntrySize { get; }

        #region Object methods

        public bool IsAttributeSet(int objectNum, int attributeNum)
        {
            char value = memory.ReadUnsigned8(
              GetAttributeByteAddress(objectNum, attributeNum));
            return (value & (0x80 >> (attributeNum & 7))) > 0;
        }

        public void SetAttribute(int objectNum, int attributeNum)
        {
            int attributeByteAddress = GetAttributeByteAddress(objectNum,
              attributeNum);
            char value = memory.ReadUnsigned8(attributeByteAddress);
            value |= (char)(0x80 >> (attributeNum & 7));
            memory.WriteUnsigned8(attributeByteAddress, value);
        }

        public void ClearAttribute(int objectNum, int attributeNum)
        {
            int attributeByteAddress = GetAttributeByteAddress(objectNum,
                attributeNum);
            char value = memory.ReadUnsigned8(attributeByteAddress);
            value &= (char)(~(0x80 >> (attributeNum & 7)));
            memory.WriteUnsigned8(attributeByteAddress, value);
        }

        /// <summary>
        /// Gets the address of the specified attribute in the the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="attributeNum">The attribute number.</param>
        /// <returns>The address of the attribute byte.</returns>
        private int GetAttributeByteAddress(int objectNum, int attributeNum)
        {
            return GetObjectAddress(objectNum) + attributeNum / 8;
        }

        #endregion

        #region Property methods

        public int GetPropertiesDescriptionAddress(int objectNum)
        {
            return GetPropertyTableAddress(objectNum) + 1;
        }

        public int GetPropertyAddress(int objectNum, int property)
        {
            int propAddr = GetPropertyEntriesStart(objectNum);
            while (true)
            {
                int propnum = GetPropertyNum(propAddr);
                if (propnum == 0) return 0; // not found
                if (propnum == property)
                {
                    return propAddr + GetNumPropertySizeBytes(propAddr);
                }
                int numPropBytes = GetNumPropertySizeBytes(propAddr);
                propAddr += numPropBytes + GetPropertyLength(propAddr + numPropBytes);
            }
        }

        public int GetNextProperty(int objectNum, int property)
        {
            if (property == 0)
            {
                int addr = GetPropertyEntriesStart(objectNum);
                return GetPropertyNum(addr);
            }
            int propDataAddr = GetPropertyAddress(objectNum, property);
            if (propDataAddr == 0)
            {
                ReportPropertyNotAvailable(objectNum, property);
                return 0;
            }
            else
            {
                return GetPropertyNum(propDataAddr + GetPropertyLength(propDataAddr));
            }
        }

        /// <summary>
        /// Report that the specified property is not available for the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <param name="property">The property number.</param>
        private void ReportPropertyNotAvailable(int objectNum, int property)
        {
            throw new ArgumentException($"Property {property} of object {objectNum} is not available.");
        }

        public char GetProperty(int objectNum, int property)
        {
            int propertyDataAddress = GetPropertyAddress(objectNum, property);
            if (propertyDataAddress == 0)
            {
                return getPropertyDefault(property);
            }
            int numBytes = GetPropertyLength(propertyDataAddress);
            int value;
            if (numBytes == 1)
            {
                value = memory.ReadUnsigned8(propertyDataAddress) & 0xff;
            }
            else
            {
                int byte1 = memory.ReadUnsigned8(propertyDataAddress);
                int byte2 = memory.ReadUnsigned8(propertyDataAddress + 1);
                value = (byte1 << 8 | (byte2 & 0xff));
            }
            return (char)(value & 0xffff);
        }

        public void SetProperty(int objectNum, int property, char value)
        {

            int propertyDataAddress = GetPropertyAddress(objectNum, property);
            if (propertyDataAddress == 0)
            {
                ReportPropertyNotAvailable(objectNum, property);
            }
            else
            {
                int propsize = GetPropertyLength(propertyDataAddress);
                if (propsize == 1)
                {
                    memory.WriteUnsigned8(propertyDataAddress, (char)(value & 0xff));
                }
                else
                {
                    memory.WriteUnsigned16(propertyDataAddress, ToUnsigned16(value));
                }
            }
        }

        /// <summary>
        /// Gets the property number at the specified table index.
        /// </summary>
        /// <param name="propertyAddress">The property address.</param>
        /// <returns>The property number.</returns>
        protected abstract int GetPropertyNum(int propertyAddress);

        /// <summary>
        /// Gets the address of the property table for the specified object.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The property table address.</returns>
        protected abstract int GetPropertyTableAddress(int objectNum);

        /// <summary>
        /// Gets the number of property size bytes at the specified address.
        /// </summary>
        /// <param name="propertyAddress">The address of the property entry.</param>
        /// <returns>The number of size bytes.</returns>
        protected abstract int GetNumPropertySizeBytes(int propertyAddress);

        /// <summary>
        /// Gets the number of property size bytes at the specified property
        /// data address.
        /// </summary>
        /// <param name="propertyDataAddress">The address of the property entry data.</param>
        /// <returns>The number of size bytes.</returns>
        protected abstract int GetNumPropSizeBytesAtData(int propertyDataAddress);

        /// <summary>
        /// Gets the start address of the actual property entries.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The start address of the property entries.</returns>
        private int GetPropertyEntriesStart(int objectNum)
        {
            return GetPropertyTableAddress(objectNum) +
              GetDescriptionHeaderSize(objectNum);
        }

        /// <summary>
        /// Gets the size of the description header in bytes.
        /// </summary>
        /// <param name="objectNum">The object number.</param>
        /// <returns>The size of the description header.</returns>
        /// <remarks>
        /// Includes the size byte plus the description string size. This stays the same
        /// for all story file versions.
        /// </remarks>
        private int GetDescriptionHeaderSize(int objectNum)
        {
            int startAddr = GetPropertyTableAddress(objectNum);
            return memory.ReadUnsigned8(startAddr) * 2 + 1;
        }

        /// <summary>
        /// Gets the property default value at the specified position in the
        /// property defaults table.
        /// </summary>
        /// <param name="propertyNum">The property number of the default entry.</param>
        /// <returns>The property default value.</returns>
        private char getPropertyDefault(int propertyNum)
        {
            int index = propertyNum - 1;
            return memory.ReadUnsigned16(address + index * 2);
        }

        #endregion
    }
}
