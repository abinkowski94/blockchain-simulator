using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Common.Models
{
    public class TreeItem<T>
    {
        public T Item { get; set; }
        public int Height => (Children != null ? Children.Any() ? Children.Max(c => c.Height) : 0 : 0) + 1;
        public TreeItem<T> Parent { get; set; }
        public List<TreeItem<T>> Children { get; set; }
    }
}