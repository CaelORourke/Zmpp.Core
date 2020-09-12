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
    using Zmpp.Core;
    using static Zmpp.Core.MemoryUtil;

    /// <summary>
    /// This class implements the object tree for story file version >= 4.
    /// </summary>
    public class ModernObjectTree : AbstractObjectTree
    {
        private const int OFFSET_PARENT = 6;
        private const int OFFSET_SIBLING = 8;
        private const int OFFSET_CHILD = 10;
        private const int OFFSET_PROPERTYTABLE = 12;

        /// <summary>
        /// Object entries in version >= 4 have a size of 14 bytes.
        /// </summary>
        private const int OBJECTENTRY_SIZE = 14;

        /// <summary>
        /// Property defaults entries in versions >= 4 have a size of 63 words.
        /// </summary>
        private const int PROPERTYDEFAULTS_SIZE = 63 * 2;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memory">Memory object</param>
        /// <param name="address">address of tree</param>
        public ModernObjectTree(IMemory memory, int address) : base(memory, address)
        {
        }

        protected override int getObjectAddress(int objectNum)
        {
            return getObjectTreeStart() + (objectNum - 1) * getObjectEntrySize();
        }

        protected override int getPropertyDefaultsSize() { return PROPERTYDEFAULTS_SIZE; }

        protected override int getObjectEntrySize() { return OBJECTENTRY_SIZE; }

        #region Object methods

        public override int getParent(int objectNum)
        {
            return getMemory().ReadUnsigned16(getObjectAddress(objectNum) + OFFSET_PARENT);
        }

        public override void setParent(int objectNum, int parent)
        {
            getMemory().WriteUnsigned16(getObjectAddress(objectNum) + OFFSET_PARENT, ToUnsigned16(parent));
        }

        public override int getSibling(int objectNum)
        {
            return getMemory().ReadUnsigned16(getObjectAddress(objectNum) + OFFSET_SIBLING);
        }

        public override void setSibling(int objectNum, int sibling)
        {
            getMemory().WriteUnsigned16(getObjectAddress(objectNum) + OFFSET_SIBLING, ToUnsigned16(sibling));
        }

        public override int getChild(int objectNum)
        {
            return getMemory().ReadUnsigned16(getObjectAddress(objectNum) + OFFSET_CHILD);
        }

        public override void setChild(int objectNum, int child)
        {
            getMemory().WriteUnsigned16(getObjectAddress(objectNum) + OFFSET_CHILD, ToUnsigned16(child));
        }

        #endregion

        #region Property methods

        public override int getPropertyLength(int propertyAddress)
        {
            return getPropertyLengthAtData(getMemory(), propertyAddress);
        }

        protected override int getPropertyTableAddress(int objectNum)
        {
            return getMemory().ReadUnsigned16(getObjectAddress(objectNum) + OFFSET_PROPERTYTABLE);
        }

        protected override int getNumPropertySizeBytes(int propertyAddress)
        {
            // if bit 7 is set, there are two size bytes, one otherwise
            char first = getMemory().ReadUnsigned8(propertyAddress);
            return ((first & 0x80) > 0) ? 2 : 1;
        }

        protected override int getNumPropSizeBytesAtData(int propertyDataAddress)
        {
            return getNumPropertySizeBytes(propertyDataAddress - 1);
        }

        protected override int getPropertyNum(int propertyAddress)
        {
            // Version >= 4 - take the lower 5 bit of the first size byte
            return getMemory().ReadUnsigned8(propertyAddress) & 0x3f;
        }

        /// <summary>
        /// This function represents the universal formula to calculate the length
        /// of a property given the address of its data(as opposed to the address
        /// of the property itself).
        /// </summary>
        /// <param name="memory">the Memory object</param>
        /// <param name="addressOfPropertyData">the address of the property data</param>
        /// <returns>the length of the property</returns>
        private static int getPropertyLengthAtData(IMemory memory, int addressOfPropertyData)
        {
            if (addressOfPropertyData == 0)
            {
                return 0; // see standard 1.1
            }
            // The size byte is always the byte before the property data in any
            // version, so this is consistent
            char sizebyte =
              memory.ReadUnsigned8(addressOfPropertyData - 1);

            // Bit 7 set => this is the second size byte
            if ((sizebyte & 0x80) > 0)
            {
                int proplen = sizebyte & 0x3f;
                if (proplen == 0)
                {
                    proplen = 64; // Std. doc. 1.0, S 12.4.2.1.1
                }
                return proplen;
            }
            else
            {
                // Bit 7 clear => there is only one size byte, so if bit 6 is set,
                // the size is 2, else it is 1
                return (sizebyte & 0x40) > 0 ? 2 : 1;
            }
        }

        #endregion
    }
}
