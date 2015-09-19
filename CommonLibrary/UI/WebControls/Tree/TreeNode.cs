using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.WebControls.Common;
using System.ComponentModel;
using CommonLibrary.UI.WebControls;
using System.Web.UI;

namespace CommonLibrary.UI.WebControls
{
    public class TreeNode : Node, IStateManager
    {
        static internal string _separator = ":";
        static internal readonly string _checkboxIDSufix = "checkbox";

        private TreeNodeCollection m_objNodes;
        private Tree m_objTree;

        public TreeNode()
            : base()
        {
        }

        public TreeNode(string strText)
            : base(strText)
        {
        }

        //internal TreeNode(System.Xml.XmlNode objXmlNode, Control ctlOwner)
        //    : base(objXmlNode)
        //{
        //    m_objTree = (Tree)ctlOwner;
        //}

        //internal TreeNode(Control ctlOwner)
        //    : base()
        //{
        //    m_objTree = (Tree)ctlOwner;
        //}

        //This is here for backwards compatibility
        public new string CssClass
        {
            get { return base.CSSClass; }
            set { base.CSSClass = value; }
        }

        //This is here for backwards compatibility
        public new string NavigateUrl
        {
            get { return base.NavigateURL; }
            set { base.NavigateURL = value; }
        }

        [Browsable(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public TreeNodeCollection TreeNodes
        {
            get
            {
                if (m_objNodes == null)
                {
                    m_objNodes = new TreeNodeCollection(this.XmlNode, this.Tree);
                }
                return m_objNodes;
            }
        }

        [Browsable(false)]
        public TreeNode Parent
        {
            get { return (TreeNode)this.ParentNode; }
        }

        [Browsable(false)]
        public Tree Tree
        {
            get { return (Tree)m_objTree; }
        }

        //[DefaultValue(false), Browsable(false)]
        //public bool IsExpanded
        //{
        //    get
        //    {
        //        string _expanded;
        //        if (Tree.IsDownLevel == false)
        //        {
        //            string sExpanded = CommonLibrary.UI.Utilities.ClientAPI.GetClientVariable(m_objTree.Page, m_objTree.ClientID + "_" + this.ClientID + ":expanded");
        //            if (!String.IsNullOrEmpty(sExpanded))
        //            {
        //                _expanded = sExpanded;
        //            }
        //            else
        //            {
        //                _expanded = this.CustomAttribute("expanded", "false");
        //            }
        //        }
        //        else
        //        {
        //            _expanded = this.CustomAttribute("expanded", "false");
        //        }
        //        return Convert.ToBoolean(_expanded);
        //    }
        //}


        //<Bindable(True), DefaultValue(False), PersistenceMode(PersistenceMode.Attribute)> _
        //  Public Property CheckBox() As Boolean
        //      Get
        //              Dim _checkBox As Object = Me.CustomAttribute("checkBox", 0)
        //              Return CType(_checkBox, Boolean)
        //      End Get
        //      Set(ByVal Value As Boolean)
        //              Me.CustomAttribute("checkBox", 0) = Value
        //      End Set
        //End Property

        [Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
        public string CssClassOver
        {
            get { return this.CSSClassHover; }
            set { this.CSSClassHover = value; }
        }

        //[Bindable(true), DefaultValue(-1), PersistenceMode(PersistenceMode.Attribute)]
        //public int ImageIndex
        //{
        //    get
        //    {
        //        if (!String.IsNullOrEmpty(this.CustomAttribute("imgIdx")))
        //        {
        //            return Convert.ToInt32(this.CustomAttribute("imgIdx"));
        //        }
        //        else
        //        {
        //            if (this.Tree.ImageList.Count > 0)
        //            {
        //                return 0;
        //                //BACKWARDS COMPATIBILITY!!!! SHOULD BE -1
        //            }
        //            else
        //            {
        //                return -1;
        //            }
        //        }
        //    }
        //    set { this.CustomAttribute("imgIdx", value.ToString()); }
        //}

        //Public Property PopulateOnDemand() As Boolean
        //      Get
        //              Return CBool(Me.CustomAttribute("pond", False))
        //      End Get
        //      Set(ByVal Value As Boolean)
        //              Me.SetCustomAttribute("pond", Value)
        //      End Set
        //End Property

        public string LeftHTML
        {
            get { return this.CustomAttribute("lhtml", ""); }
            set { this.SetCustomAttribute("lhtml", value); }
        }

        public string RightHTML
        {
            get { return this.CustomAttribute("rhtml", ""); }
            set { this.SetCustomAttribute("rhtml", value); }
        }


        //public new TreeNode ParentNode
        //{
        //    get
        //    {
        //        if ((this.XmlNode.ParentNode != null) && this.XmlNode.ParentNode.NodeType != System.Xml.XmlNodeType.Document) return new TreeNode(this.XmlNode.ParentNode, m_objTree); else return null;
        //    }
        //}

        //public void MakeNodeVisible()
        //{
        //    if ((this.Parent != null))
        //    {
        //        this.Parent.Expand();
        //        this.Parent.MakeNodeVisible();
        //    }
        //}

        //private ITreeNodeWriter NodeWriter
        //{
        //    get
        //    {
        //        if (m_objTree.IsDownLevel)
        //        {
        //            return new TreeNodeWriter();
        //        }
        //        else
        //        {
        //            return null;
        //            // New TreeNodeUpLevelWriter
        //        }
        //    }
        //}

        //public void Expand()
        //{
        //    if (HasNodes)
        //    {
        //        this.SetCustomAttribute("expanded", "true");
        //        Tree.OnExpand(new TreeEventArgs(this));
        //    }
        //}

        //public void Collapse()
        //{
        //    if (HasNodes)
        //    {
        //        this.SetCustomAttribute("expanded", "false");
        //        Tree.OnCollapse(new TreeEventArgs(this));
        //    }
        //}

        //public void Click()
        //{
        //    this.Selected = true;
        //    Tree.OnNodeClick(new TreeNodeClickEventArgs(this));
        //}

        //public virtual void Render(HtmlTextWriter writer)
        //{
        //    NodeWriter.RenderNode(writer, this);
        //}

        internal void SetTree(Tree objTree)
        {
            m_objTree = objTree;
        }

        //BACKWARDS COMPATIBILITY ONLY
        #region "IStateManager Interface"
        public bool IsTrackingViewState
        {
            get { return false; }
        }

 
        public void LoadViewState(object state)
        {
        }

     
        public object SaveViewState()
        {
            return null;
        }

        public void TrackViewState()
        {
        }
        #endregion

    }


}
