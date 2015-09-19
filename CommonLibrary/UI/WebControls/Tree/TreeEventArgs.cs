using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.WebControls
{
    public class TreeEventArgs : EventArgs
    {
        private TreeNode _node;

        public TreeEventArgs(TreeNode node)
        {
            this._node = node;
        }

        public TreeNode Node
        {
            get
            {
                return this._node;
            }
        }
    }

}
