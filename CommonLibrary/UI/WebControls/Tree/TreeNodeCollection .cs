using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.WebControls.Common;
using System.Web.UI;
using System.Xml;
using System.Collections;

namespace CommonLibrary.UI.WebControls
{
    public class TreeNodeCollection : NodeCollection, IStateManager
    {

        #region "Member Variables"
        private Tree m_objTree;
        #endregion

        #region "Constructors"
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructor to call when creating a Root Node
        /// </summary>
        /// <param name="strNamespace">Namespace of node hierarchy</param>
        /// <param name="objControl">Tree control associated to TreeNodeCollection</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Jon Henning]   12/22/2004      Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public TreeNodeCollection(string strNamespace, Tree objControl)
            : base(strNamespace, objControl)
        {
            m_objTree = objControl;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Loads node collection based off of XML string
        /// </summary>
        /// <param name="strXML">XML String</param>
        /// <param name="strXSLFile">XSL FileName.  Leave empty if no transform needed</param>
        /// <param name="objControl"> Tree control</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Jon Henning]   12/22/2004      Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public TreeNodeCollection(string strXML, string strXSLFile, Tree objControl)
            : base(strXML, strXSLFile)
        {
            m_objTree = objControl;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructor for all nodes that are not the root.  
        /// </summary>
        /// <param name="objXmlNode">Node whose children will be exposed by this class</param>
        /// <param name="objControl"> Tree control</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Jon Henning]   12/22/2004      Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public TreeNodeCollection(XmlNode objXmlNode, Tree objControl)
            : base(objXmlNode, objControl)
        {
            //, ByVal objControl As Control)
            m_objTree = objControl;
        }

        #endregion

        //TreeNode
        //public new int Add()
        //{
        //    return this.Add("");
        //}

        // TreeNode
        //public new int Add(string strText)
        //{
            //TreeNode objNode = new TreeNode(m_objTree);
            //int intIndex = this.Add(objNode);
            //objNode.Text = strText;
            //return intIndex;
        //}

        // TreeNode
        public new int Add(TreeNode objNode)
        {
            int intIndex = base.Add(objNode);
            objNode.SetTree(m_objTree);
            //ChildNodes.count is not guaranteed to be unique... GetHashCode should be a little safer (I assume it is medium trust safe...
            if (String.IsNullOrEmpty(objNode.ID)) objNode.ID = objNode.ParentNameSpace + "_" + objNode.GetHashCode();
            //Me.XMLNode.ChildNodes.Count
            return intIndex;
        }

        public new int Add(string strID, string strKey, string strText, string strNavigateURL)
        {
            return Add(strID, strKey, strText, strNavigateURL, "", "", "", true, "", "",
            "", false, eClickAction.Navigate, "", -1);
        }

        public new int Add(string strID, string strKey, string strText, string strNavigateURL, string strJSFunction, string strTarget, string strToolTip, bool blnEnabled, string strCSSClass, string strCSSClassSelected,
            // TreeNode
        string strCSSClassHover, bool blnSelected, eClickAction enumClickAction, string strCssClassOver, int intImageIndex)
        {

            int intIndex = this.Add();
           // TreeNode objNode = this[intIndex];

           // if (!String.IsNullOrEmpty(strID))
           // {
           //     objNode.ID = strID;
           // }
           // else
           // {
           //     objNode.ID = objNode.ParentNameSpace + "_" + this.XMLNode.ChildNodes.Count;
           // }
           // objNode.Key = strKey;
           // objNode.Text = strText;
           // objNode.NavigateURL = strNavigateURL;
           // objNode.JSFunction = strJSFunction;
           // objNode.Target = strTarget;
           // objNode.ToolTip = strToolTip;
           // objNode.Enabled = blnEnabled;
           // objNode.CSSClass = strCSSClass;
           // objNode.CSSClassSelected = strCSSClassSelected;
           // objNode.CSSClassHover = strCSSClassHover;
           // objNode.Selected = blnSelected;
           // objNode.ClickAction = enumClickAction;
           // //objNode.CheckBox = blnCheckBox
           // objNode.CssClassOver = strCssClassOver;
           //// objNode.ImageIndex = intImageIndex;

            return intIndex;

        }

        //public new TreeNode this[int index]
        //{
            //get { return new TreeNode(this.XMLNode.ChildNodes[index], m_objTree); }
            //set
            //{
            //    throw new Exception("Cannot Assign Node Directly");
            //}
       // }

        public int IndexOf(TreeNode value)
        {
            int i;
            //for (i = 0; i <= this.XMLNode.ChildNodes.Count; i++)
            //{
            //    if (new TreeNode(this.XMLNode.ChildNodes[i], m_objTree).ID == value.ID)
            //    {
            //        return i;
            //    }
            //}
            return -1;
        }

        public void Insert(int index, TreeNode value)
        {
            this.XMLNode.InsertAfter(this.XMLNode.ChildNodes[index], value.XmlNode);
        }

        public void Remove(TreeNode value)
        {
            this.XMLNode.RemoveChild(value.XmlNode);
        }

        public new void Clear()
        {
            int i;
            for (i = this.XMLNode.ChildNodes.Count - 1; i >= 0; i += -1)
            {
                this.XMLNode.RemoveChild(this.XMLNode.ChildNodes[i]);
            }
        }

        public bool Contains(TreeNode value)
        {
            if (this.FindNode(value.ID) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public new TreeNode FindNode(string ID)
        {
            XmlNode objNode = FindFast("id", ID, this.XMLNode, true);
            //if ((objNode != null)) return new TreeNode(objNode, m_objTree); else 
            return null;
        }

        public new TreeNode FindNodeByKey(string Key)
        {
            XmlNode objNode = FindFast("key", Key, this.XMLNode, true);
           // if ((objNode != null)) return new TreeNode(objNode, m_objTree); else 
                return null;
        }

        public override ArrayList FindSelectedNodes()
        {
            ArrayList colTreeNodes = new ArrayList();
            //TreeNodeCollection = New TreeNodeCollection(m_strNamespace, m_objTree)
            //if ((this.XMLNode != null))
            //{
            //    XmlNodeList objNodeList = this.XMLNode.SelectNodes("//n[@selected='1']");
            //    foreach (XmlNode objNode in objNodeList)
            //    {
            //        colTreeNodes.Add(new TreeNode(objNode, m_objTree));
            //    }
            //}
            return colTreeNodes;
        }


        //Public Overloads Function ToXml() As String
        //      Return Me.XMLDoc.OuterXml
        //End Function

        #region "ICollection Implementation"
        public new IEnumerator GetEnumerator()
        {
            return new TreeNodeEnumerator(this.XMLNode, m_objTree);
        }
        #endregion

        //BACKWARDS COMPATIBILITY ONLY
        #region "IStateManager Interface"
        public bool IsTrackingViewState
        {
            get { return false; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
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

    public class TreeNodeEnumerator : IEnumerator
    {

        private XmlNode m_objXmlNode;
        private Tree m_objTree;
        private int m_intCursor;

        public TreeNodeEnumerator(XmlNode objRoot, Tree objControl)
        {
            m_objXmlNode = objRoot;
            m_objTree = objControl;
            m_intCursor = -1;
        }

        public void Reset()
        {
            m_intCursor = -1;
        }

        public bool MoveNext()
        {
            if (m_intCursor < m_objXmlNode.ChildNodes.Count)
            {
                m_intCursor = m_intCursor + 1;
            }

            if ((m_intCursor == m_objXmlNode.ChildNodes.Count))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object Current
        {
            get
            {
                if (((m_intCursor < 0) | (m_intCursor == m_objXmlNode.ChildNodes.Count)))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                   // return new TreeNode(m_objXmlNode.ChildNodes[m_intCursor], m_objTree);
                    return null;
                }
            }
        }
    }

}
