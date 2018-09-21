using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BlockchainSimulator.Hub.BusinessLogic.Helpers.Drawing
{
    public class TreeDrawer
    {
        private const int NodeHeight = 30;
        private const int NodeMarginX = 50;
        private const int NodeMarginY = 40;
        private const int NodeWidth = 30;
        private readonly string _path;
        private readonly Pen _nodePen;

        public TreeDrawer(string path)
        {
            _path = path;
            _nodePen = Pens.Black;
        }

        public void DrawGraph(List<NodeModel> data)
        {
            var tree = GetSampleTree(data);
            TreeHelpers<NodeModel>.CalculateNodePositions(tree);

            var size = GetSize(tree);
            using (var myBitmap = new Bitmap(size.Width, size.Height))
            {
                var graphic = Graphics.FromImage(myBitmap);

                PaintTree(graphic, tree);
                using (var stream = new FileStream(_path, FileMode.Create, FileAccess.ReadWrite))
                {
                    myBitmap.Save(stream, ImageFormat.Bmp);
                }
            }
        }

        private static List<TreeNodeModel<NodeModel>> GetChildNodes(List<NodeModel> data,
            TreeNodeModel<NodeModel> parent)
        {
            var nodes = new List<TreeNodeModel<NodeModel>>();

            foreach (var item in data.Where(p => p.ParentId == parent.Item.Id))
            {
                var treeNode = new TreeNodeModel<NodeModel>(item, parent);
                treeNode.Children = GetChildNodes(data, treeNode);
                nodes.Add(treeNode);
            }

            return nodes;
        }

        // converts list of sample items to hierarchical list of TreeNodeModels
        private static TreeNodeModel<NodeModel> GetSampleTree(List<NodeModel> data)
        {
            var root = data.FirstOrDefault(p => p.ParentId == string.Empty);
            var rootTreeNode = new TreeNodeModel<NodeModel>(root, null);

            // add tree node children recursively
            rootTreeNode.Children = GetChildNodes(data, rootTreeNode);

            return rootTreeNode;
        }

        private static Size GetSize(TreeNodeModel<NodeModel> tree)
        {
            // tree sizes are 0-based, so add 1
            var treeWidth = tree.Width + 1;
            var treeHeight = tree.Height + 1;

            var size = new Size(Convert.ToInt32(treeWidth * NodeWidth + (treeWidth + 1) * NodeMarginX),
                treeHeight * NodeHeight + (treeHeight + 1) * NodeMarginY);

            return size;
        }

        private void DrawNode(TreeNodeModel<NodeModel> node, Graphics graphic)
        {
            // rectangle where node will be positioned
            var nodeRect = new Rectangle(Convert.ToInt32(NodeMarginX + node.X * (NodeWidth + NodeMarginX)),
                NodeMarginY + node.Y * (NodeHeight + NodeMarginY), NodeWidth, NodeHeight);

            // draw box
            graphic.DrawRectangle(_nodePen, nodeRect);

            // draw content
            graphic.DrawString(node.ToString(), new Font(FontFamily.GenericMonospace, 8), Brushes.Black,
                nodeRect.X + 10,
                nodeRect.Y + 10);

            // draw line to parent
            if (node.Parent != null)
            {
                var nodeTopMiddle = new Point(nodeRect.X + nodeRect.Width / 2, nodeRect.Y);
                graphic.DrawLine(_nodePen, nodeTopMiddle,
                    new Point(nodeTopMiddle.X, nodeTopMiddle.Y - NodeMarginY / 2));
            }

            // draw line to children
            if (node.Children.Count > 0)
            {
                var nodeBottomMiddle = new Point(nodeRect.X + (nodeRect.Width / 2), nodeRect.Y + nodeRect.Height);
                graphic.DrawLine(_nodePen, nodeBottomMiddle,
                    new Point(nodeBottomMiddle.X, nodeBottomMiddle.Y + (NodeMarginY / 2)));

                // draw line over children
                if (node.Children.Count > 1)
                {
                    var childrenLineStartX = Convert.ToInt32(NodeMarginX + node.GetRightMostChild().X *
                                                             // ReSharper disable once PossibleLossOfFraction
                                                             (NodeWidth + NodeMarginX) + NodeWidth / 2);
                    var childrenLineStartY = nodeBottomMiddle.Y + NodeMarginY / 2;
                    var childrenLineStart = new Point(childrenLineStartX, childrenLineStartY);

                    var childrenLineEndX = Convert.ToInt32(NodeMarginX +
                                                           node.GetLeftMostChild().X * (NodeWidth + NodeMarginX) +
                                                           // ReSharper disable once PossibleLossOfFraction
                                                           NodeWidth / 2);
                    var childrenLineEndY = nodeBottomMiddle.Y + NodeMarginY / 2;
                    var childrenLineEnd = new Point(childrenLineEndX, childrenLineEndY);

                    graphic.DrawLine(_nodePen, childrenLineStart, childrenLineEnd);
                }
            }

            foreach (var item in node.Children)
            {
                DrawNode(item, graphic);
            }
        }

        private void PaintTree(Graphics graphic, TreeNodeModel<NodeModel> tree)
        {
            graphic.Clear(Color.White);
            DrawNode(tree, graphic);
        }
    }
}