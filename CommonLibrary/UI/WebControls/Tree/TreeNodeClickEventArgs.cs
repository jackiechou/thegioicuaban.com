using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls
{
    public class TreeNodeClickEventArgs: EventArgs
    {
        private TreeNode _node;

        public TreeNodeClickEventArgs(TreeNode Node)
        {
                _node = Node;
        }

        public TreeNode Node {
                get { return _node; }
        }
    }
}
