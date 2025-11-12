using System;
using System.Collections.Generic;

namespace st10152431_MunicipalityService.Models.DataStructures
{
    /// <summary>
    /// AVL Tree: Self-balancing Binary Search Tree
    /// 
    /// Why AVL Tree for Service Requests?
    /// - Guarantees O(log n) search time for looking up requests by ID
    /// - Better than List which requires O(n) linear search
    /// - Self-balancing prevents worst-case O(n) degradation
    /// 
    /// Time Complexity:
    /// - Insert: O(log n) - includes rebalancing
    /// - Search: O(log n) - guaranteed by balance property
    /// - InOrder Traversal: O(n) - visits all nodes
    /// 
    /// Space Complexity: O(n) where n is number of nodes
    /// </summary>
    public class AVLTree<T> where T : IComparable<T>
    {
        private AVLNode<T> _root;
        private int _count;

        /// <summary>
        /// Get total number of nodes in tree
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Check if tree is empty
        /// </summary>
        public bool IsEmpty => _root == null;

        /// <summary>
        /// Constructor: Initialize empty tree
        /// </summary>
        public AVLTree()
        {
            _root = null;
            _count = 0;
        }

        /// <summary>
        /// Insert a new item into the AVL tree
        /// Automatically rebalances to maintain AVL property
        /// Time Complexity: O(log n)
        /// </summary>
        public void Insert(T data)
        {
            _root = InsertRecursive(_root, data);
            _count++;
        }

        /// <summary>
        /// Recursive helper method for insertion
        /// Inserts node and rebalances on way back up
        /// </summary>
        private AVLNode<T> InsertRecursive(AVLNode<T> node, T data)
        {
            // Base case: Found insertion position
            if (node == null)
                return new AVLNode<T>(data);

            // Recursive case: Navigate to correct position
            int comparison = data.CompareTo(node.Data);

            if (comparison < 0)
            {
                // Insert in left subtree
                node.Left = InsertRecursive(node.Left, data);
            }
            else if (comparison > 0)
            {
                // Insert in right subtree
                node.Right = InsertRecursive(node.Right, data);
            }
            else
            {
                // Duplicate value - update existing node
                node.Data = data;
                _count--; // Don't increment count for updates
                return node;
            }

            // Update height of current node
            node.UpdateHeight();

            // Get balance factor to check if rebalancing needed
            int balance = node.GetBalanceFactor();

            // REBALANCING CASES:

            // Left-Left Case (LL Rotation)
            // Tree is left-heavy and insertion was in left subtree of left child
            if (balance > 1 && data.CompareTo(node.Left.Data) < 0)
            {
                return RotateRight(node);
            }

            // Right-Right Case (RR Rotation)
            // Tree is right-heavy and insertion was in right subtree of right child
            if (balance < -1 && data.CompareTo(node.Right.Data) > 0)
            {
                return RotateLeft(node);
            }

            // Left-Right Case (LR Rotation)
            // Tree is left-heavy but insertion was in right subtree of left child
            // Solution: Left rotate the left child, then right rotate current node
            if (balance > 1 && data.CompareTo(node.Left.Data) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right-Left Case (RL Rotation)
            // Tree is right-heavy but insertion was in left subtree of right child
            // Solution: Right rotate the right child, then left rotate current node
            if (balance < -1 && data.CompareTo(node.Right.Data) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            // Node is balanced, return unchanged
            return node;
        }

        /// <summary>
        /// Right Rotation (for Left-Left case)
        /// 
        /// Before:        y                After:      x
        ///               / \                          / \
        ///              x   C                        A   y
        ///             / \              -->              / \
        ///            A   B                             B   C
        /// 
        /// Time Complexity: O(1)
        /// </summary>
        private AVLNode<T> RotateRight(AVLNode<T> y)
        {
            AVLNode<T> x = y.Left;
            AVLNode<T> B = x.Right;

            // Perform rotation
            x.Right = y;
            y.Left = B;

            // Update heights (order matters: y first, then x)
            y.UpdateHeight();
            x.UpdateHeight();

            // Return new root of subtree
            return x;
        }

        /// <summary>
        /// Left Rotation (for Right-Right case)
        /// 
        /// Before:    x                    After:        y
        ///           / \                                / \
        ///          A   y                              x   C
        ///             / \              -->           / \
        ///            B   C                          A   B
        /// 
        /// Time Complexity: O(1)
        /// </summary>
        private AVLNode<T> RotateLeft(AVLNode<T> x)
        {
            AVLNode<T> y = x.Right;
            AVLNode<T> B = y.Left;

            // Perform rotation
            y.Left = x;
            x.Right = B;

            // Update heights (order matters: x first, then y)
            x.UpdateHeight();
            y.UpdateHeight();

            // Return new root of subtree
            return y;
        }

        /// <summary>
        /// Search for an item in the tree
        /// Returns the item if found, default(T) if not found
        /// Time Complexity: O(log n) due to balanced tree
        /// </summary>
        public T Search(T data)
        {
            return SearchRecursive(_root, data);
        }

        /// <summary>
        /// Recursive helper for search
        /// </summary>
        private T SearchRecursive(AVLNode<T> node, T data)
        {
            // Base case: Not found
            if (node == null)
                return default(T);

            int comparison = data.CompareTo(node.Data);

            if (comparison == 0)
            {
                // Found!
                return node.Data;
            }
            else if (comparison < 0)
            {
                // Search in left subtree
                return SearchRecursive(node.Left, data);
            }
            else
            {
                // Search in right subtree
                return SearchRecursive(node.Right, data);
            }
        }

        /// <summary>
        /// Check if item exists in tree
        /// Time Complexity: O(log n)
        /// </summary>
        public bool Contains(T data)
        {
            return !EqualityComparer<T>.Default.Equals(Search(data), default(T));
        }

        /// <summary>
        /// Get all items in sorted order (InOrder traversal)
        /// Returns List for easy enumeration and display
        /// Time Complexity: O(n) - must visit all nodes
        /// Space Complexity: O(n) - stores all items
        /// </summary>
        public List<T> InOrderTraversal()
        {
            List<T> result = new List<T>();
            InOrderRecursive(_root, result);
            return result;
        }

        /// <summary>
        /// Recursive helper for InOrder traversal
        /// Visit order: Left subtree -> Current node -> Right subtree
        /// This produces sorted output for BST
        /// </summary>
        private void InOrderRecursive(AVLNode<T> node, List<T> result)
        {
            if (node == null)
                return;

            InOrderRecursive(node.Left, result);   // Visit left
            result.Add(node.Data);                  // Visit current
            InOrderRecursive(node.Right, result);  // Visit right
        }

        /// <summary>
        /// Get all items in level order (breadth-first)
        /// Useful for displaying tree structure by levels
        /// Time Complexity: O(n)
        /// Space Complexity: O(w) where w is maximum width
        /// </summary>
        public List<T> LevelOrderTraversal()
        {
            List<T> result = new List<T>();
            if (_root == null)
                return result;

            Queue<AVLNode<T>> queue = new Queue<AVLNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                AVLNode<T> current = queue.Dequeue();
                result.Add(current.Data);

                if (current.Left != null)
                    queue.Enqueue(current.Left);

                if (current.Right != null)
                    queue.Enqueue(current.Right);
            }

            return result;
        }

        /// <summary>
        /// Remove an item from the AVL tree (by value)
        /// Automatically rebalances to maintain AVL property
        /// Time Complexity: O(log n)
        /// </summary>
        public void Remove(T data)
        {
            _root = RemoveRecursive(_root, data);
        }

        private AVLNode<T> RemoveRecursive(AVLNode<T> node, T data)
        {
            if (node == null)
                return null;

            int comparison = data.CompareTo(node.Data);

            if (comparison < 0)
            {
                node.Left = RemoveRecursive(node.Left, data);
            }
            else if (comparison > 0)
            {
                node.Right = RemoveRecursive(node.Right, data);
            }
            else
            {
                // Node to be deleted found
                if (node.Left == null || node.Right == null)
                {
                    AVLNode<T> temp = node.Left ?? node.Right;
                    if (temp == null)
                    {
                        // No child
                        node = null;
                    }
                    else
                    {
                        // One child
                        node = temp;
                    }
                    _count--;
                }
                else
                {
                    // Node with two children: Get inorder successor (smallest in right subtree)
                    AVLNode<T> temp = GetMinNode(node.Right);
                    node.Data = temp.Data;
                    node.Right = RemoveRecursive(node.Right, temp.Data);
                }
            }

            // If the tree had only one node then return
            if (node == null)
                return null;

            // Update height
            node.UpdateHeight();

            // Get balance factor
            int balance = node.GetBalanceFactor();

            // Left Left Case
            if (balance > 1 && node.Left.GetBalanceFactor() >= 0)
                return RotateRight(node);

            // Left Right Case
            if (balance > 1 && node.Left.GetBalanceFactor() < 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right Right Case
            if (balance < -1 && node.Right.GetBalanceFactor() <= 0)
                return RotateLeft(node);

            // Right Left Case
            if (balance < -1 && node.Right.GetBalanceFactor() > 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        private AVLNode<T> GetMinNode(AVLNode<T> node)
        {
            AVLNode<T> current = node;
            while (current.Left != null)
                current = current.Left;
            return current;
        }

        /// <summary>
        /// Get tree height (maximum depth)
        /// Time Complexity: O(1) - stored in root node
        /// </summary>
        public int GetHeight()
        {
            return _root?.Height ?? 0;
        }

        /// <summary>
        /// Clear all nodes from tree
        /// </summary>
        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        /// <summary>
        /// Get minimum value in tree (leftmost node)
        /// Time Complexity: O(log n)
        /// </summary>
        public T GetMin()
        {
            if (_root == null)
                return default(T);

            AVLNode<T> current = _root;
            while (current.Left != null)
            {
                current = current.Left;
            }
            return current.Data;
        }

        /// <summary>
        /// Get maximum value in tree (rightmost node)
        /// Time Complexity: O(log n)
        /// </summary>
        public T GetMax()
        {
            if (_root == null)
                return default(T);

            AVLNode<T> current = _root;
            while (current.Right != null)
            {
                current = current.Right;
            }
            return current.Data;
        }
    }
}