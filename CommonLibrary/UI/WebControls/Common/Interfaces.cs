using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.WebControls.Common
{
    /// <summary>
    /// ITreeNodeWriter interface declaration. All the objects which want to implement
    /// a writer class for the TreeNode should inherit from this interface.
    /// </summary>
    internal interface ITreeNodeWriter
    {
        /// <summary>
        /// When implemented renders an Node inside the tree.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="Node"></param>
        void RenderNode(HtmlTextWriter writer, TreeNode Node);
    }
    //ITreeNodeWriter

    /// <summary>
    /// ITreeWriter interface declaration. All the objects which want to implement
    /// a writer class for the Tree should inherit from this interface.
    /// </summary>
    internal interface ITreeWriter
    {
        /// <summary>
        /// When implemented renders the tree.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="tree"></param>
        void RenderTree(HtmlTextWriter writer, Tree tree);
    }

    /// <summary>
    /// IMenuNodeWriter interface declaration. All the objects which want to implement
    /// a writer class for the MenuNode should inherit from this interface.
    /// </summary>
    //internal interface IMenuNodeWriter
    //{
    //    /// <summary>
    //    /// When implemented renders an Node inside the Menu.
    //    /// </summary>
    //    /// <param name="writer"></param>
    //    /// <param name="Node"></param>
    //    void RenderNode(HtmlTextWriter writer, MenuNode Node);
    //}
    //IMenuNodeWriter

    /// <summary>
    /// IMenuWriter interface declaration. All the objects which want to implement
    /// a writer class for the Menu should inherit from this interface.
    /// </summary>
    internal interface IMenuWriter
    {
        /// <summary>
        /// When implemented renders the Menu.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="Menu"></param>
        void RenderMenu(HtmlTextWriter writer, Menu menu);
    }

    public interface IToolBar
    {
        string ToolBarId
        {
            get;
            set;
        }
    }

    public interface IToolBarSupportedActions
    {
        string[] Actions
        {
            get;
        }
    }

}
