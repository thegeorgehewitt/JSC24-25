using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.Collections.Generic
{
    public class SparseSet<T> : IEnumerable<T>
    {
        private int[] sparse;
        private T[] dense;
        private int[] denseIndices;
        private int capacity;
        private int count;

        public int Count => count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



        public SparseSet(int initialCapacity = 16)
        {
            capacity = initialCapacity;

            sparse = new int[capacity];
            Array.Fill(sparse, -1);

            dense = new T[capacity];
            denseIndices = new int[capacity];

            count = 0;
        }



        public T this[int _key] => Get(_key);



        private void EnsureCapacity(int key)
        {
            if (key < capacity) return;

            int newCapacity = Math.Max(capacity * 2, key + 1);
            capacity = newCapacity;

            Array.Resize(ref sparse, newCapacity);
            Array.Fill(sparse, -1, capacity, newCapacity - capacity);

            Array.Resize(ref dense, newCapacity);
            Array.Resize(ref denseIndices, newCapacity);
        }

        public bool Contains(int key)
        {
            return key < capacity && sparse[key] != -1 && sparse[key] < count;
        }

        public void Add(int key, T value)
        {
            EnsureCapacity(key);
            if (Contains(key)) return;

            sparse[key] = count;
            dense[count] = value;
            denseIndices[count] = key;
            count++;
        }

        public void Remove(int key)
        {
            if (!Contains(key)) return;

            int index = sparse[key];
            count--;

            if (index != count)
            {
                dense[index] = dense[count];
                denseIndices[index] = denseIndices[count];
                sparse[denseIndices[index]] = index;
            }

            sparse[key] = -1;
        }

        public T Get(int key)
        {
            if (!Contains(key)) throw new KeyNotFoundException($"Key {key} not found in SparseSet.");
            return dense[sparse[key]];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return dense[i];
            }
        }
    }
}
