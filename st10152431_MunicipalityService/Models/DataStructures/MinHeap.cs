using System;
using System.Collections.Generic;

namespace st10152431_MunicipalityService.Models.DataStructures
{
    /// <summary>
    /// Min-Heap: Priority Queue Implementation
    /// 
    /// Why Min-Heap for Service Requests?
    /// - Efficiently retrieve most urgent request in O(1) time
    /// - Extract urgent requests in O(log n) time
    /// - Better than sorting a List every time (O(n log n))
    /// - Automatically maintains priority order
    /// 
    /// Priority Levels:
    /// 1 = Critical (water main break, electrical hazard)
    /// 2 = High (major pothole, traffic light out)
    /// 3 = Medium (streetlight out)
    /// 4 = Low (minor cosmetic issues)
    /// 5 = Very Low (suggestions, non-urgent)
    /// 
    /// Time Complexity:
    /// - Insert: O(log n) - heapify up
    /// - ExtractMin: O(log n) - heapify down
    /// - Peek: O(1) - just return root
    /// - BuildHeap: O(n) - build from existing list
    /// 
    /// Space Complexity: O(n) where n is number of elements
    /// </summary>
    public class MinHeap<T> where T : IComparable<T>
    {
        private List<T> _heap;

        /// <summary>
        /// Get total number of items in heap
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// Check if heap is empty
        /// </summary>
        public bool IsEmpty => _heap.Count == 0;

        /// <summary>
        /// Constructor: Initialize empty heap
        /// </summary>
        public MinHeap()
        {
            _heap = new List<T>();
        }

        /// <summary>
        /// Constructor: Build heap from existing collection
        /// Time Complexity: O(n) using bottom-up heapification
        /// </summary>
        public MinHeap(IEnumerable<T> items)
        {
            _heap = new List<T>(items);
            BuildHeap();
        }

        /// <summary>
        /// Insert a new item into the heap
        /// Maintains min-heap property by bubbling up
        /// Time Complexity: O(log n)
        /// </summary>
        public void Insert(T item)
        {
            // Add to end of heap (maintains complete binary tree structure)
            _heap.Add(item);

            // Restore heap property by bubbling up
            HeapifyUp(_heap.Count - 1);
        }

        /// <summary>
        /// Extract and return the minimum item (root)
        /// This removes the most urgent service request
        /// Time Complexity: O(log n)
        /// </summary>
        public T ExtractMin()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Heap is empty");

            // Store minimum value (root)
            T min = _heap[0];

            // Move last element to root
            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            // Restore heap property by bubbling down
            if (_heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return min;
        }

        /// <summary>
        /// Peek at minimum item without removing it
        /// Returns most urgent request without changing heap
        /// Time Complexity: O(1)
        /// </summary>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Heap is empty");

            return _heap[0];
        }

        /// <summary>
        /// Get top N most urgent items without removing them
        /// Useful for displaying "Top 5 Urgent Requests"
        /// Time Complexity: O(n log n) due to temporary heap
        /// </summary>
        public List<T> PeekTop(int n)
        {
            if (n <= 0)
                return new List<T>();

            if (n >= _heap.Count)
                return new List<T>(_heap);

            // Create temporary heap to avoid modifying original
            MinHeap<T> tempHeap = new MinHeap<T>(_heap);
            List<T> result = new List<T>();

            for (int i = 0; i < n && !tempHeap.IsEmpty; i++)
            {
                result.Add(tempHeap.ExtractMin());
            }

            return result;
        }

        /// <summary>
        /// Heapify Up: Restore heap property after insertion
        /// Bubble element up until parent is smaller
        /// 
        /// Process:
        /// 1. Compare with parent
        /// 2. If current < parent, swap
        /// 3. Repeat until parent is smaller or reach root
        /// 
        /// Time Complexity: O(log n) - height of tree
        /// </summary>
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;

                // If heap property satisfied, stop
                if (_heap[index].CompareTo(_heap[parentIndex]) >= 0)
                    break;

                // Swap with parent
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        /// <summary>
        /// Heapify Down: Restore heap property after extraction
        /// Bubble element down until children are larger
        /// 
        /// Process:
        /// 1. Find smallest among current, left child, right child
        /// 2. If current is not smallest, swap with smallest child
        /// 3. Repeat until current is smallest or reach leaf
        /// 
        /// Time Complexity: O(log n) - height of tree
        /// </summary>
        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;

            while (index < lastIndex)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                // Check if left child is smaller
                if (leftChild <= lastIndex &&
                    _heap[leftChild].CompareTo(_heap[smallest]) < 0)
                {
                    smallest = leftChild;
                }

                // Check if right child is smaller
                if (rightChild <= lastIndex &&
                    _heap[rightChild].CompareTo(_heap[smallest]) < 0)
                {
                    smallest = rightChild;
                }

                // If current is smallest, heap property satisfied
                if (smallest == index)
                    break;

                // Swap with smallest child and continue
                Swap(index, smallest);
                index = smallest;
            }
        }

        /// <summary>
        /// Build heap from unsorted list
        /// Bottom-up approach starting from last non-leaf node
        /// Time Complexity: O(n) - not O(n log n)!
        /// </summary>
        private void BuildHeap()
        {
            // Start from last non-leaf node and heapify down
            int startIndex = (_heap.Count / 2) - 1;

            for (int i = startIndex; i >= 0; i--)
            {
                HeapifyDown(i);
            }
        }

        /// <summary>
        /// Swap two elements in the heap
        /// Helper method for heapify operations
        /// </summary>
        private void Swap(int index1, int index2)
        {
            T temp = _heap[index1];
            _heap[index1] = _heap[index2];
            _heap[index2] = temp;
        }

        /// <summary>
        /// Clear all items from heap
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        /// <summary>
        /// Get all items as list (unordered)
        /// For debugging or display purposes
        /// </summary>
        public List<T> ToList()
        {
            return new List<T>(_heap);
        }

        /// <summary>
        /// Get all items in sorted order
        /// WARNING: This destroys the heap! Use ToList() for non-destructive access
        /// Time Complexity: O(n log n)
        /// </summary>
        public List<T> GetSorted()
        {
            List<T> sorted = new List<T>();

            while (!IsEmpty)
            {
                sorted.Add(ExtractMin());
            }

            return sorted;
        }

        /// <summary>
        /// Check if heap contains an item
        /// Time Complexity: O(n) - must check all elements
        /// </summary>
        public bool Contains(T item)
        {
            return _heap.Contains(item);
        }

        /// <summary>
        /// Get heap as array representation
        /// Useful for visualizing heap structure
        /// Index 0 = root
        /// Index i children at 2i+1 and 2i+2
        /// </summary>
        public T[] ToArray()
        {
            return _heap.ToArray();
        }
    }
}