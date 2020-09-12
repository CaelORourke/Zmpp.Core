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

namespace org.zmpp.vm
{
    using org.zmpp.@base;
    using System;
    using static org.zmpp.@base.MemoryUtil;

    /// <summary>
    /// This class is the abstract super class of object trees.
    /// </summary>
    public abstract class AbstractObjectTree : IObjectTree
    {
        private IMemory memory;
        private int address;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memory">the memory access object</param>
        /// <param name="address">the object table's start address</param>
        public AbstractObjectTree(IMemory memory, int address)
        {
            this.memory = memory;
            this.address = address;
        }

        /// <summary>
        /// Returns the memory object.
        /// </summary>
        /// <returns>the memory object</returns>
        protected IMemory getMemory() { return memory; }

        /// <summary>
        /// Returns this tree's start address.
        /// </summary>
        /// <returns>the address</returns>
        protected int getAddress() { return address; }

        /// <summary>
        /// Returns the address of the specified object.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <returns>the object address</returns>
        protected abstract int getObjectAddress(int objectNum);

        public abstract int getPropertyLength(int propertyAddress);

        public abstract int getParent(int objectNum);

        public abstract void setParent(int objectNum, int parent);

        public abstract int getSibling(int objectNum);

        public abstract void setSibling(int objectNum, int sibling);

        public abstract int getChild(int objectNum);

        public abstract void setChild(int objectNum, int child);

        public void removeObject(int objectNum)
        {
            int oldParent = getParent(objectNum);
            setParent(objectNum, 0);

            if (oldParent != 0)
            {
                if (getChild(oldParent) == objectNum)
                {
                    setChild(oldParent, getSibling(objectNum));
                }
                else
                {
                    // Find the child that comes directly before the removed
                    // node and set the direct sibling of the removed node as
                    // its new sibling
                    int currentChild = getChild(oldParent);
                    int sibling = getSibling(currentChild);

                    // We have to handle the case that in fact that object is a child
                    // of its parent, but not directly (happens for some reasons).
                    // We stop in this case and simply remove the object from its
                    // parent, probably the object tree modification routines should
                    // be reverified
                    while (sibling != 0 && sibling != objectNum)
                    {
                        currentChild = sibling;
                        sibling = getSibling(currentChild);
                    }
                    // sibling might be 0, in that case, the object is not
                    // in the hierarchy
                    if (sibling == objectNum)
                    {
                        setSibling(currentChild, getSibling(objectNum));
                    }
                }
            }
            setSibling(objectNum, 0);
        }

        public void insertObject(int parentNum, int objectNum)
        {
            // we want to ensure, the child has no old parent relationships
            if (getParent(objectNum) > 0)
            {
                removeObject(objectNum);
            }
            int oldChild = getChild(parentNum);
            setParent(objectNum, parentNum);
            setChild(parentNum, objectNum);
            setSibling(objectNum, oldChild);
        }

        /// <summary>
        /// The size of the property defaults section.
        /// </summary>
        /// <returns>the property defaults section</returns>
        protected abstract int getPropertyDefaultsSize();

        /// <summary>
        /// Returns the start address of the object tree section.
        /// </summary>
        /// <returns>the object tree's start address</returns>
        protected int getObjectTreeStart()
        {
            return getAddress() + getPropertyDefaultsSize();
        }

        /// <summary>
        /// Returns the story file version specific object entry size.
        /// </summary>
        /// <returns>the size of an object entry</returns>
        protected abstract int getObjectEntrySize();

        #region Object methods

        public bool isAttributeSet(int objectNum, int attributeNum)
        {
            char value = memory.readUnsigned8(
              getAttributeByteAddress(objectNum, attributeNum));
            return (value & (0x80 >> (attributeNum & 7))) > 0;
        }

        public void setAttribute(int objectNum, int attributeNum)
        {
            int attributeByteAddress = getAttributeByteAddress(objectNum,
              attributeNum);
            char value = memory.readUnsigned8(attributeByteAddress);
            value |= (char)(0x80 >> (attributeNum & 7));
            memory.writeUnsigned8(attributeByteAddress, value);
        }

        public void clearAttribute(int objectNum, int attributeNum)
        {
            int attributeByteAddress = getAttributeByteAddress(objectNum,
                attributeNum);
            char value = memory.readUnsigned8(attributeByteAddress);
            value &= (char)(~(0x80 >> (attributeNum & 7)));
            memory.writeUnsigned8(attributeByteAddress, value);
        }

        /// <summary>
        /// Returns the address of the byte specified object attribute lies in.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <param name="attributeNum">the attribute number</param>
        /// <returns>the address of the attribute byte</returns>
        private int getAttributeByteAddress(int objectNum, int attributeNum)
        {
            return getObjectAddress(objectNum) + attributeNum / 8;
        }

        #endregion

        #region Property methods

        public int getPropertiesDescriptionAddress(int objectNum)
        {
            return getPropertyTableAddress(objectNum) + 1;
        }

        public int getPropertyAddress(int objectNum, int property)
        {
            int propAddr = getPropertyEntriesStart(objectNum);
            while (true)
            {
                int propnum = getPropertyNum(propAddr);
                if (propnum == 0) return 0; // not found
                if (propnum == property)
                {
                    return propAddr + getNumPropertySizeBytes(propAddr);
                }
                int numPropBytes = getNumPropertySizeBytes(propAddr);
                propAddr += numPropBytes + getPropertyLength(propAddr + numPropBytes);
            }
        }

        public int getNextProperty(int objectNum, int property)
        {
            if (property == 0)
            {
                int addr = getPropertyEntriesStart(objectNum);
                return getPropertyNum(addr);
            }
            int propDataAddr = getPropertyAddress(objectNum, property);
            if (propDataAddr == 0)
            {
                reportPropertyNotAvailable(objectNum, property);
                return 0;
            }
            else
            {
                return getPropertyNum(propDataAddr + getPropertyLength(propDataAddr));
            }
        }

        /// <summary>
        /// Reports the non-availability of a property.
        /// </summary>
        /// <param name="objectNum">object number</param>
        /// <param name="property">property number</param>
        private void reportPropertyNotAvailable(int objectNum, int property)
        {
            throw new ArgumentException("Property " + property + " of object " + objectNum + " is not available.");
        }

        public char getProperty(int objectNum, int property)
        {
            int propertyDataAddress = getPropertyAddress(objectNum, property);
            if (propertyDataAddress == 0)
            {
                return getPropertyDefault(property);
            }
            int numBytes = getPropertyLength(propertyDataAddress);
            int value;
            if (numBytes == 1)
            {
                value = memory.readUnsigned8(propertyDataAddress) & 0xff;
            }
            else
            {
                int byte1 = memory.readUnsigned8(propertyDataAddress);
                int byte2 = memory.readUnsigned8(propertyDataAddress + 1);
                value = (byte1 << 8 | (byte2 & 0xff));
            }
            return (char)(value & 0xffff);
        }

        public void setProperty(int objectNum, int property, char value)
        {

            int propertyDataAddress = getPropertyAddress(objectNum, property);
            if (propertyDataAddress == 0)
            {
                reportPropertyNotAvailable(objectNum, property);
            }
            else
            {
                int propsize = getPropertyLength(propertyDataAddress);
                if (propsize == 1)
                {
                    memory.writeUnsigned8(propertyDataAddress, (char)(value & 0xff));
                }
                else
                {
                    memory.writeUnsigned16(propertyDataAddress, toUnsigned16(value));
                }
            }
        }

        /// <summary>
        /// Returns the property number at the specified table index.
        /// </summary>
        /// <param name="propertyAddress">the property address</param>
        /// <returns>the property number</returns>
        protected abstract int getPropertyNum(int propertyAddress);

        /// <summary>
        /// Returns the address of an object's property table.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <returns>the table address</returns>
        protected abstract int getPropertyTableAddress(int objectNum);

        /// <summary>
        /// Returns the number of property size bytes at the specified address.
        /// </summary>
        /// <param name="propertyAddress">the address of the property entry</param>
        /// <returns>the number of size bytes</returns>
        protected abstract int getNumPropertySizeBytes(int propertyAddress);

        /// <summary>
        /// Returns the number of property size bytes at the specified property
        /// data address.
        /// </summary>
        /// <param name="propertyDataAddress">the address of the property entry data</param>
        /// <returns>the number of size bytes</returns>
        protected abstract int getNumPropSizeBytesAtData(int propertyDataAddress);

        /// <summary>
        /// Returns the start address of the actual property entries.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <returns>the property entries' start address</returns>
        private int getPropertyEntriesStart(int objectNum)
        {
            return getPropertyTableAddress(objectNum) +
              getDescriptionHeaderSize(objectNum);
        }

        /// <summary>
        /// Returns the size of the description header in bytes that is,
        /// the size byte plus the description string size.This stays the same
        /// for all story file versions.
        /// </summary>
        /// <param name="objectNum">the object number</param>
        /// <returns>the size of the description header</returns>
        private int getDescriptionHeaderSize(int objectNum)
        {
            int startAddr = getPropertyTableAddress(objectNum);
            return memory.readUnsigned8(startAddr) * 2 + 1;
        }

        /// <summary>
        /// Returns the property default value at the specified position in the
        /// property defaults table.
        /// </summary>
        /// <param name="propertyNum">the default entry's property number</param>
        /// <returns>the property default value</returns>
        private char getPropertyDefault(int propertyNum)
        {
            int index = propertyNum - 1;
            return memory.readUnsigned16(address + index * 2);
        }

        #endregion
    }
}
