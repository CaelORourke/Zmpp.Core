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

namespace Zmpp.Core.Vm.Utility
{
    using System;
    using System.Text;

    /// <summary>
    /// This ring buffer implementation is an efficient representation for a
    /// dynamic list structure that should have a limited number of entries and
    /// where the oldest n entries can be discarded.
    /// This kind of container is particularly useful for undo and history buffers.
    /// </summary>
    /// <typeparam name="T">type of contained objects</typeparam>
    public class RingBuffer<T>
    {
        private T[] elements;
        private int bufferstart;
        private int bufferend;
        private int _size;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="size">the size of the buffer</param>
        public RingBuffer(int size)
        {
            elements = new T[size];
        }

        /// <summary>
        /// Adds an element to the buffer. If the capacity of the buffer is exceeded,
        /// the oldest element is replaced.
        /// </summary>
        /// <param name="elem">the element</param>
        public void add(T elem)
        {
            if (_size == elements.Length)
            {
                bufferstart = (bufferstart + 1) % elements.Length;
            }
            else
            {
                _size++;
            }
            elements[bufferend++] = elem;
            bufferend = bufferend % elements.Length;
        }

        /// <summary>
        /// Replaces the element at the specified index with the specified element.
        /// </summary>
        /// <param name="index">the replacement index</param>
        /// <param name="elem">the replacement element</param>
        public void set(int index, T elem)
        {
            elements[mapIndex(index)] = elem;
        }

        /// <summary>
        /// Returns the element at the specified index.
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the object</returns>
        public T get(int index) { return elements[mapIndex(index)]; }

        /// <summary>
        /// Returns the size of this ring buffer.
        /// </summary>
        /// <returns>the size</returns>
        public int size() { return _size; }

        /// <summary>
        /// Removes the object at the specified index.
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>the removed object</returns>
        public T remove(int index)
        {
            if (_size > 0)
            {
                // remember the removed element
                T elem = get(index);

                // move the following element by one to the front
                for (int i = index; i < (_size - 1); i++)
                {
                    int idx1 = mapIndex(i);
                    int idx2 = mapIndex(i + 1);
                    elements[idx1] = elements[idx2];
                }
                _size--;
                bufferend = (bufferend - 1) % elements.Length;
                if (bufferend < 0) bufferend = elements.Length + bufferend;
                return elem;
            }
            return default(T);
        }

        /// <summary>
        /// Maps a container index to a ring buffer index.
        /// </summary>
        /// <param name="index">the container index</param>
        /// <returns>the buffer index</returns>
        private int mapIndex(int index)
        {
            return (bufferstart + index) % elements.Length;
        }

        public String toString()
        {
            StringBuilder buffer = new StringBuilder("{ ");
            for (int i = 0; i < size(); i++)
            {
                if (i > 0)
                {
                    buffer.Append(", ");
                }
                buffer.Append(get(i));
            }
            buffer.Append(" }");
            return buffer.ToString();
        }
    }
}
