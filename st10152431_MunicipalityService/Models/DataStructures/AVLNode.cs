using System;

namespace st10152431_MunicipalityService.Models.DataStructures
{
    /// <summary>
    /// Node structure for AVL Tree (self-balancing binary search tree)
    /// Each node contains a ServiceRequest and maintains height for balancing
    /// </summary>
    /// <typeparam name="T">Type must implement IComparable for ordering</typeparam>
    public class AVLNode<T> where T : IComparable<T>
    {
        /// <summary>
        /// The data stored in this node (ServiceRequest)
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Reference to left child (smaller values)
        /// </summary>
        public AVLNode<T> Left { get; set; }

        /// <summary>
        /// Reference to right child (larger values)
        /// </summary>
        public AVLNode<T> Right { get; set; }

        /// <summary>
        /// Height of this node in the tree (used for balancing)
        /// Leaf nodes have height 1, null nodes have height 0
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Constructor: Create a new AVL node with given data
        /// Initial height is 1 (leaf node), no children
        /// </summary>
        public AVLNode(T data)
        {
            Data = data;
            Left = null;
            Right = null;
            Height = 1;  // New node starts at height 1
        }

        /// <summary>
        /// Calculate balance factor for this node
        /// Balance Factor = Height(Left) - Height(Right)
        /// 
        /// Returns:
        ///   > 1  : Left-heavy (needs right rotation)
        ///   < -1 : Right-heavy (needs left rotation)
        ///   -1 to 1 : Balanced (no rotation needed)
        /// </summary>
        public int GetBalanceFactor()
        {
            int leftHeight = Left?.Height ?? 0;
            int rightHeight = Right?.Height ?? 0;
            return leftHeight - rightHeight;
        }

        /// <summary>
        /// Update this node's height based on children's heights
        /// Height = 1 + max(leftHeight, rightHeight)
        /// Called after insertions or rotations
        /// </summary>
        public void UpdateHeight()
        {
            int leftHeight = Left?.Height ?? 0;
            int rightHeight = Right?.Height ?? 0;
            Height = 1 + Math.Max(leftHeight, rightHeight);
        }
    }
}