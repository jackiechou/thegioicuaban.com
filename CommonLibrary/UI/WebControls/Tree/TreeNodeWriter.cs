using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using CommonLibrary.UI.WebControls.Common;
using System.Web.UI.WebControls;
using CommonLibrary.UI.Utilities;

namespace CommonLibrary.UI.WebControls
{
    internal class TreeNodeWriter : ITreeNodeWriter
    {
        static readonly string[] _expcol = new string[2] { "+", "-" };
        private TreeNode _Node;

        public void RenderNode(HtmlTextWriter writer, TreeNode Node)
        {
            _Node = Node;
          //  Render(writer);
        }


        //protected void Render(HtmlTextWriter writer)
        //{
        //    RenderContents(writer);
        //    if (_Node.HasNodes && (_Node.IsExpanded || _Node.Tree.IsCrawler))
        //    {
        //        writer.AddAttribute(HtmlTextWriterAttribute.Class, "Child");
        //        writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
        //        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        RenderChildren(writer);
        //        writer.RenderEndTag();
        //    }
        //}

        //protected void RenderContents(HtmlTextWriter writer)
        //{

        //    RenderOpenTag(writer);
        //    if (_Node.Tree.IndentWidth == 0) _Node.Tree.IndentWidth = 9;
        //    //keep old default

        //    if (_Node.Level > 0)
        //    {
        //        RenderSpacer(writer, _Node.Level * _Node.Tree.IndentWidth);
        //    }

        //    RenderExpandNodeIcon(writer);

        //    RenderNodeCheckbox(writer);

        //    RenderNodeIcon(writer);

        //    RenderNodeText(writer);

        //    writer.RenderEndTag();

        //}

        //protected void RenderOpenTag(HtmlTextWriter writer)
        //{
        //    string NodeClass = "Node";

        //    //writer.AddAttribute(HtmlTextWriterAttribute.Class, GetNodeCss(_Node))
        //    writer.AddAttribute(HtmlTextWriterAttribute.Name, _Node.ID);
        //    writer.AddAttribute(HtmlTextWriterAttribute.Id, _Node.ID.Replace(TreeNode._separator, "_"));
        //    writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //}

      
        //protected void RenderExpandNodeIcon(HtmlTextWriter writer)
        //{
        //    if (_Node.HasNodes)
        //    {
        //        HyperLink _link = new HyperLink();
        //        Image _image = new Image();
        //        if (_Node.IsExpanded || _Node.Tree.IsCrawler)
        //        {
        //            _link.Text = _expcol[1];
        //            if (!String.IsNullOrEmpty(_Node.Tree.ExpandedNodeImage))
        //            {
        //                _image.ImageUrl = _Node.Tree.ExpandedNodeImage;
        //            }
        //        }
        //        else
        //        {
        //            _link.Text = _expcol[0];
        //            if (!String.IsNullOrEmpty(_Node.Tree.CollapsedNodeImage))
        //            {
        //                _image.ImageUrl = _Node.Tree.CollapsedNodeImage;
        //            }
        //        }
        //        //If _Node.PopulateOnDemand Then        'handled in postback handler
        //        //      _link.NavigateUrl = ClientAPI.GetPostBackClientHyperlink(_Node.Tree, _Node.ID & ",OnDemand")
        //        //Else
        //        _link.NavigateUrl = ClientAPI.GetPostBackClientHyperlink(_Node.Tree, _Node.ID);
        //        //End If
        //        if (!String.IsNullOrEmpty(_image.ImageUrl))
        //        {
        //            _link.RenderBeginTag(writer);
        //            _image.RenderControl(writer);
        //            _link.RenderEndTag(writer);
        //        }
        //        else
        //        {
        //            _link.RenderControl(writer);
        //        }
        //        _image = null;
        //        _link = null;
        //    }
        //    else
        //    {
        //        RenderSpacer(writer, _Node.Tree.ExpandCollapseImageWidth);
        //    }
        //    writer.Write("&nbsp;");
        //}

      
        //protected void RenderNodeCheckbox(HtmlTextWriter writer)
        //{
        //    if (_Node.Tree.CheckBoxes)
        //    {
        //        CheckBox _checkbox = new CheckBox();
        //        _checkbox.ID = _Node.ID + TreeNode._separator + TreeNode._checkboxIDSufix;
        //        _checkbox.Checked = _Node.Selected;
        //        string strJS = "";
        //        if (!String.IsNullOrEmpty(_Node.JSFunction))
        //        {
        //            if (_Node.JSFunction.EndsWith(";") == false) strJS += _Node.JSFunction + ";";
        //        }
        //        if (!String.IsNullOrEmpty(_Node.Tree.JSFunction))
        //        {
        //            if (_Node.Tree.JSFunction.EndsWith(";") == false) strJS += _Node.Tree.JSFunction + ";";
        //        }

        //        string strClick = ClientAPI.GetPostBackClientHyperlink(_Node.Tree, _Node.ID + ClientAPI.COLUMN_DELIMITER + "Click").Replace("javascript:", "") + ";";
        //        string strCheck = ClientAPI.GetPostBackClientHyperlink(_Node.Tree, _Node.ID + ClientAPI.COLUMN_DELIMITER + "Checked").Replace("javascript:", "") + ";";
        //        if (_Node.Selected == false)
        //        {
        //            if (!String.IsNullOrEmpty(strJS))
        //            {
        //                strJS = "if (eval(\"" + strJS.Replace("\"", "\"\"") + "\") != false) ";
        //                strJS += strClick + " else " + strCheck;
        //            }
        //            else
        //            {
        //                strJS += strClick;
        //            }
        //        }
        //        else
        //        {
        //            strJS = strCheck;
        //        }

        //        //_checkbox.Attributes.Add("onclick", ClientAPI.GetPostBackClientHyperlink(_Node.Tree, _Node.ID & ",Unchecked"))
        //        //Else
        //        _checkbox.Attributes.Add("onclick", strJS);
        //        //End If
        //        _checkbox.RenderControl(writer);
        //        _checkbox = null;
        //        writer.Write("&nbsp;");
        //    }
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void RenderNodeIcon(HtmlTextWriter writer)
        {
            //Label oSpan = new Label();
            //if (!String.IsNullOrEmpty(_Node.CSSIcon))
            //{
            //    oSpan.CssClass = _Node.CSSIcon;
            //}
            //else if (!String.IsNullOrEmpty(_Node.Tree.DefaultIconCssClass))
            //{
            //    oSpan.CssClass = _Node.Tree.DefaultIconCssClass;
            //}
            //oSpan.RenderBeginTag(writer);

            //if (_Node.ImageIndex > -1)
            //{
            //    NodeImage _NodeImage = _Node.Tree.ImageList[_Node.ImageIndex];
            //    if ((_NodeImage != null))
            //    {
            //        Image _image = new Image();
            //        _image.ImageUrl = _NodeImage.ImageUrl;
            //        _image.RenderControl(writer);
            //        writer.Write("&nbsp;");
            //    }
            //}
            //oSpan.RenderEndTag(writer);
        }

       
        //protected void RenderNodeText(HtmlTextWriter writer)
        //{
        //    //Dim _label As Label = New Label
        //    HyperLink _link = new HyperLink();
        //    string strJS = "";

        //    //_label.Text = _Node.Text
        //    _link.Text = _Node.Text;

        //    if (!String.IsNullOrEmpty(_Node.JSFunction))
        //    {
        //        if (_Node.JSFunction.EndsWith(";") == false) strJS += _Node.JSFunction + ";";
        //    }
        //    else if (!String.IsNullOrEmpty(_Node.Tree.JSFunction))
        //    {
        //        if (_Node.Tree.JSFunction.EndsWith(";") == false) strJS += _Node.Tree.JSFunction + ";";
        //    }

        //    if (_Node.Enabled)
        //    {
        //        switch (_Node.ClickAction)
        //        {
        //            case eClickAction.PostBack:
        //            case eClickAction.Expand:
        //                if (!String.IsNullOrEmpty(strJS)) strJS = "if (eval(\"" + strJS.Replace("\"", "\"\"") + "\") != false) ";
        //                strJS += ClientAPI.GetPostBackClientHyperlink(_Node.Tree, _Node.ID + CommonLibrary.UI.Utilities.ClientAPI.COLUMN_DELIMITER + "Click").Replace("javascript:", "");
        //                ;
        //                break;
        //            case eClickAction.Navigate:
        //                if (!String.IsNullOrEmpty(strJS)) strJS = "if (eval(\"" + strJS.Replace("\"", "\"\"") + "\") != false) ";
        //                if (!String.IsNullOrEmpty(_Node.Tree.Target))
        //                {
        //                    strJS += "window.frames." + _Node.Tree.Target + ".location.href='" + _Node.NavigateURL + "'; void(0);";
        //                    //FOR SOME REASON THIS DOESNT WORK UNLESS WE HAVE CODE AFTER THE SETTING OF THE HREF...
        //                }
        //                else
        //                {
        //                    strJS += "window.location.href='" + _Node.NavigateURL + "';";
        //                }

        //                break;

        //        }

        //        _link.NavigateUrl = "javascript:" + strJS;
        //    }

        //    if (!String.IsNullOrEmpty(_Node.ToolTip))
        //    {
        //        //_label.ToolTip = _Node.ToolTip
        //        _link.ToolTip = _Node.ToolTip;
        //    }
        //    //_label.CssClass = "NodeText"
        //    //_label.RenderControl(writer)

        //    string sCSS = GetNodeCss(_Node);
        //    if (!String.IsNullOrEmpty(sCSS)) _link.CssClass = sCSS;
        //    //If _Node.Selected Then
        //    //      If Len(_Node.Tree.DefaultNodeCssClassSelected) > 0 Then _link.CssClass = _Node.Tree.DefaultNodeCssClassSelected
        //    //Else
        //    //      _link.CssClass = _Node.CSSClass
        //    //End If

        //    _link.RenderControl(writer);
        //}

        //private string GetNodeCss(TreeNode oNode)
        //{
        //    string sNodeCss = oNode.Tree.CssClass;

        //    if (oNode.Level > 0) sNodeCss = oNode.Tree.DefaultChildNodeCssClass;
        //    if (!String.IsNullOrEmpty(oNode.CSSClass)) sNodeCss = oNode.CSSClass;

        //    if (oNode.Selected)
        //    {
        //        if (!String.IsNullOrEmpty(oNode.CSSClassSelected))
        //        {
        //            sNodeCss += " " + oNode.CSSClassSelected;
        //        }
        //        else
        //        {
        //            sNodeCss += " " + oNode.Tree.DefaultNodeCssClassSelected;
        //        }
        //    }

        //    return sNodeCss;
        //}

    
        //protected void RenderSpacer(HtmlTextWriter writer, int Width)
        //{
        //    writer.AddStyleAttribute("width", Width.ToString() + "px");
        //    writer.AddStyleAttribute("height", "1px");
        //    writer.AddAttribute("src", _Node.Tree.SystemImagesPath + "spacer.gif");
        //    writer.RenderBeginTag(HtmlTextWriterTag.Img);
        //    writer.RenderEndTag();
        //}

      
        //protected void RenderChildren(HtmlTextWriter writer)
        //{
        //    foreach (TreeNode _elem in _Node.TreeNodes)
        //    {
        //        _elem.Render(writer);
        //    }
        //}

    }

}
