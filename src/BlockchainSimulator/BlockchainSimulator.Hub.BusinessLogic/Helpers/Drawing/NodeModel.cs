﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BlockchainSimulator.Hub.BusinessLogic.Helpers.Drawing
{
    public class NodeModel
    {
        private List<NodeModel> _children = new List<NodeModel>();
        private Pen _colour = Pens.Black;
        private int? _fixedHeight;

        public string Id { get; set; }

        public string ParentId { get; set; }

        public NodeModel Parent { get; set; }

        public string Content { get; set; }

        public bool IsCompressed { get; set; }

        public int Depth => 1 + Parent?.Depth ?? 0;

        public int Height
        {
            get => _fixedHeight ?? 1 + (Children.Any() ? Children.Max(c => c.Height) : 0);
            set => _fixedHeight = value;
        }

        public List<NodeModel> Children
        {
            get => _children;
            set => _children = value ?? new List<NodeModel>();
        }

        public Pen Colour
        {
            get => _colour ?? Pens.Black;
            set => _colour = value ?? Pens.Black;
        }

        public override string ToString()
        {
            return Content;
        }
    }
}