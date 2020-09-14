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

    /// <summary>
    /// This class implements the object tree for story file version <= 3.
    /// </summary>
    public class ClassicObjectTree : ObjectTree
    {
        private const int OFFSET_PARENT = 4;
        private const int OFFSET_SIBLING = 5;
        private const int OFFSET_CHILD = 6;
        private const int OFFSET_PROPERTYTABLE = 7;

        /// <summary>
        /// Object entries in version <= 3 have a size of 9 bytes.
        /// </summary>
        private const int OBJECTENTRY_SIZE = 9;

        /// <summary>
        /// Property defaults entries in versions <= 3 have a size of 31 words.
        /// </summary>
        private const int PROPERTYDEFAULTS_SIZE = 31 * 2;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memory">the Memory object</param>
        /// <param name="address">the address</param>
        public ClassicObjectTree(IMemory memory, int address) : base(memory, address)
        {
        }

        protected override int GetObjectAddress(int objectNum)
        {
            return ObjectTreeStart + (objectNum - 1) * ObjectEntrySize;
        }

        protected override int PropertyDefaultsSize => PROPERTYDEFAULTS_SIZE;
        protected override int ObjectEntrySize => OBJECTENTRY_SIZE;
        public override int GetPropertyLength(int propertyAddress)
        {
            return getPropertyLengthAtData(Memory, propertyAddress);
        }

        public override int GetChild(int objectNum)
        {
            return Memory.ReadUnsigned8(GetObjectAddress(objectNum) + OFFSET_CHILD);
        }

        public override void SetChild(int objectNum, int child)
        {
            Memory.WriteUnsigned8(GetObjectAddress(objectNum) + OFFSET_CHILD, (char)(child & 0xff));
        }

        public override int GetParent(int objectNum)
        {
            return Memory.ReadUnsigned8(GetObjectAddress(objectNum) + OFFSET_PARENT);
        }

        public override void SetParent(int objectNum, int parent)
        {
            Memory.WriteUnsigned8(GetObjectAddress(objectNum) + OFFSET_PARENT, (char)(parent & 0xff));
        }

        public override int GetSibling(int objectNum)
        {
            return Memory.ReadUnsigned8(GetObjectAddress(objectNum) + OFFSET_SIBLING);
        }

        public override void SetSibling(int objectNum, int sibling)
        {
            Memory.WriteUnsigned8(GetObjectAddress(objectNum) + OFFSET_SIBLING, (char)(sibling & 0xff));
        }

        protected override int GetPropertyTableAddress(int objectNum)
        {
            return Memory.ReadUnsigned16(GetObjectAddress(objectNum) + OFFSET_PROPERTYTABLE);
        }

        protected override int GetNumPropertySizeBytes(int propertyDataAddress)
        {
            return 1;
        }

        protected override int GetNumPropSizeBytesAtData(int propertyDataAddress)
        {
            return 1;
        }

        protected override int GetPropertyNum(int propertyAddress)
        {
            int sizeByte = Memory.ReadUnsigned8(propertyAddress);
            return sizeByte - 32 * (GetPropertyLength(propertyAddress + 1) - 1);
        }

        /// <summary>
        /// This function represents the universal formula to calculate the length
        /// of a property given the address of its data(as opposed to the address
        /// of the property itself).
        /// </summary>
        /// <param name="memaccess">the memory access object</param>
        /// <param name="addressOfPropertyData">the address of the property data</param>
        /// <returns>the length of the property</returns>
        private static int getPropertyLengthAtData(IMemory memaccess, int addressOfPropertyData)
        {
            if (addressOfPropertyData == 0)
            {
                return 0; // see standard 1.1
            }

            // The size byte is always the byte before the property data in any
            // version, so this is consistent
            char sizebyte =
              memaccess.ReadUnsigned8(addressOfPropertyData - 1);

            return sizebyte / 32 + 1;
        }
    }
}
