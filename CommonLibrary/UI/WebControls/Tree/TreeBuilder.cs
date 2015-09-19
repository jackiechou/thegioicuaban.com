using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace CommonLibrary.UI.WebControls
{
    public class TreeBuilder : ControlBuilder
    {
        public override Type GetChildControlType(string tagName, IDictionary attribs)
        {
            if (tagName.ToUpper().EndsWith("TreeNode"))
            {
                return typeof(TreeNode);
            }
            return null;
        }
    }

}
