using System.Collections.Generic;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The tree item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeItem<T>
    {
        /// <summary>
        /// Inner item
        /// </summary>
        public T Item { get; set; }
        
        /// <summary>
        /// Height of the tree
        /// </summary>
        public int Height { get; set; }
        
        /// <summary>
        /// The parent of the tree
        /// </summary>
        public TreeItem<T> Parent { get; set; }
        
        /// <summary>
        /// The children of te tree
        /// </summary>
        public List<TreeItem<T>> Children { get; set; }
    }
}