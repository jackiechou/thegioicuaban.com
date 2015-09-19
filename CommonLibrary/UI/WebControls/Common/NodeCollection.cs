using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace CommonLibrary.UI.WebControls.Common
{
    public class NodeCollection : XmlCollectionBase
    {

        #region "Constructors"
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructor to call when creating a Root Node
        /// </summary>
        /// <param name="strNamespace">Namespace of node hierarchy</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Jon Henning]   12/22/2004      Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public NodeCollection(string strNamespace)
            : base(strNamespace)
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Loads node collection based off of XML string
        /// </summary>
        /// <param name="strXML">XML string</param>
        /// <param name="strXSLFile">XSL FileName.  Leave empty if no transform needed</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Jon Henning]   12/22/2004      Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public NodeCollection(string strXML, string strXSLFile)
            : base(strXML, strXSLFile)
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructor for all nodes that are not the root.  
        /// </summary>
        /// <param name="objXmlNode">Node whose children will be exposed by this class</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Jon Henning]   12/22/2004      Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public NodeCollection(XmlNode objXmlNode)
            : base(objXmlNode)
        {
        }

        #region "TreeView Backwards Compatibility"
        //In order to maintain backwards compatibility with the tree from versions before 3.2 we need to allow the
        //baseclass to return the treenodeenumerator, thus the need to pass the tree to the new base collection class
        //Yes, this is a hack!
        public NodeCollection(string strNamespace, CommonLibrary.UI.WebControls.Tree objTreeControl)
            : base(strNamespace, objTreeControl)
        {
        }

        public NodeCollection(XmlNode objXmlNode, CommonLibrary.UI.WebControls.Tree objTreeControl)
            : base(objXmlNode, objTreeControl)
        {
        }
        #endregion

        #endregion

        public XmlNode XMLNode
        {
            get { return base.InnerXMLNode; }
        }

        public XmlDocument XMLDoc
        {
            get { return base.InnerXMLDoc; }
        }

        // Node
        public int Add()
        {
            Node objNode = new Node();
            this.XMLNode.AppendChild(this.XMLDoc.ImportNode(objNode.XmlNode, false));
            objNode.ID = objNode.ParentNameSpace + "_" + XMLNode.ChildNodes.Count;
            return this.XMLNode.ChildNodes.Count - 1;
            //Return objNode
        }

        // Node
        public int Add(Node objNode)
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode(objNode.XmlNode, true);
            this.XMLNode.AppendChild(objXmlNode);
            objNode.AssociateXmlNode(objXmlNode);
            return this.XMLNode.ChildNodes.Count - 1;
        }

        // Node
        public int AddBreak()
        {
            int intIndex = this.Add();
            Node objNode = this[intIndex];
            objNode.IsBreak = true;
            return intIndex;
        }

        public int Add(string strID, string strKey, string strText, string strNavigateURL, string strJSFunction, string strTarget, string strToolTip, bool blnEnabled, string strCSSClass, string strCSSClassSelected,
            //Node
        string strCSSClassHover)
        {

            int intIndex = Add();
            Node objNode = this[intIndex];

            if (!!String.IsNullOrEmpty(strID))
            {
                objNode.ID = strID;
            }

            objNode.Key = strKey;
            objNode.Text = strText;
            objNode.NavigateURL = strNavigateURL;
            objNode.JSFunction = strJSFunction;
            objNode.Target = strTarget;
            objNode.ToolTip = strToolTip;
            objNode.Enabled = blnEnabled;
            objNode.CSSClass = strCSSClass;
            objNode.CSSClassSelected = strCSSClassSelected;
            objNode.CSSClassHover = strCSSClassHover;

            return intIndex;
            //objNode

        }

        public int Import(Node objNode)
        {
            return Import(objNode, true);
        }

        // Node
        public int Import(Node objNode, bool blnDeep)
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode(objNode.XmlNode, blnDeep);
            this.XMLNode.AppendChild(objXmlNode);

            return this.XMLNode.ChildNodes.Count - 1;
        }

        public Node this[int index]
        {
            get { return new Node(base.InnerXMLNode.ChildNodes[index]); }
            set
            {
                //MyBase.InnerXMLNode.ChildNodes[index] = Value.XmlNode
                throw new Exception("Cannot Assign Node Directly");
            }
        }

        public int IndexOf(Node value)
        {
            int i;
            for (i = 0; i <= this.XMLNode.ChildNodes.Count - 1; i++)
            {
                if (new Node(this.XMLNode.ChildNodes[i]).ID == value.ID)
                {
                    return i;
                }
            }
            return -1;
        }

        public void InsertAfter(int index, Node value)
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode(value.XmlNode, true);
            this.XMLNode.InsertAfter(objXmlNode, this.XMLNode.ChildNodes[index]);

        }

        public void InsertBefore(int index, Node value)
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode(value.XmlNode, true);
            this.XMLNode.InsertBefore(objXmlNode, this.XMLNode.ChildNodes[index]);
        }

        public void Remove(Node value)
        {
            this.XMLNode.RemoveChild(value.XmlNode);
        }

        public void Remove(int index)
        {
            this.XMLNode.RemoveChild(this.XMLNode.ChildNodes[index]);
        }

        //Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
        //      If m_bNodeCollectionInterfaceCall = False Then Me.InsertAfter(index, CType(value, Node))
        //End Sub

        //Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
        //      If m_bNodeCollectionInterfaceCall = False Then Me.Remove(CType(value, Node))
        //End Sub

        //Protected Overrides Sub OnSet(ByVal index As Integer, ByVal oldValue As Object, ByVal newValue As Object)
        //      Me.Item(index) = CType(newValue, Node)
        //End Sub

        //Protected Overrides Sub OnClear()
        //      Me.Clear()
        //End Sub

        //Public Shadows Sub Clear()
        //      Dim i As Integer
        //      For i = Me.XMLNode.ChildNodes.Count - 1 To 0 Step -1
        //              Me.XMLNode.RemoveChild(Me.XMLNode.ChildNodes[i])
        //      Next
        //End Sub

        public bool Contains(Node value)
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

        internal XmlNode FindFast(string key, string value, XmlNode parent, bool optimizedForSmallData)
        {
            try
            {
                if (optimizedForSmallData)
                {
                    XmlNode objNode = parent.FirstChild;
                    while ((objNode != null))
                    {
                        if ((objNode.Attributes[key] != null) && objNode.Attributes[key].Value == value)
                        {
                            return objNode;
                        }
                        if (parent.HasChildNodes)
                        {
                            XmlNode objFound = FindFast(key, value, objNode, true);
                            if ((objFound != null)) return objFound;
                        }
                        objNode = objNode.NextSibling;
                    }
                }
                else
                {
                    return parent.SelectSingleNode(string.Format(".//n[@{0}='{1}']", key, value));
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Node FindNode(string ID)
        {
            XmlNode objNode = FindFast("id", ID, this.XMLNode, true);
            if ((objNode != null)) return new Node(objNode);
            return null;
        }

        public Node FindNodeByKey(string Key)
        {
            XmlNode objNode = FindFast("key", Key, this.XMLNode, true);
            if ((objNode != null)) return new Node(objNode);
            return null;
        }


        public virtual ArrayList FindSelectedNodes()
        {
            ArrayList colNodes = new ArrayList();
            if ((this.XMLNode != null))
            {
                XmlNodeList objNodeList = this.XMLNode.SelectNodes("//n[@selected='1']");
                foreach (XmlNode objNode in objNodeList)
                {
                    colNodes.Add(new Node(objNode));
                }
            }
            return colNodes;
        }

        public string ToXml()
        {
            return XMLDoc.OuterXml;
        }


        #region "ICollection Implementation"
        public void CopyTo(Array myArr, int index)
        {
            //Implements ICollection.CopyTo
            foreach (XmlNode objNode in base.InnerXMLNode.ChildNodes)
            {
                //myArr(index) = objNode
                myArr.SetValue(objNode, index);
                index = index + 1;
            }
        }
        public new void RemoveAt(int index)
        {
            this.Remove(index);
            NodeCollection o;
        }

        //Implements ICollection.GetEnumerator
        public new IEnumerator GetEnumerator()
        {
            return new NodeEnumerator(base.InnerXMLNode);
        }

        //The IsSynchronized Boolean property returns True if the
        //collection is designed to be thread safe; otherwise, it returns False.
        //ReadOnly Property IsSynchronized() As Boolean           'Implements ICollection.IsSynchronized
        //      Get
        //              Return False
        //      End Get
        //End Property

        //The SyncRoot property returns an object, which is used to synchronize
        //the collection. This should return the instance of the object or return the
        //SyncRoot of another collection if the collection contains other collections.
        //ReadOnly Property SyncRoot() As Object                  'Implements ICollection.SyncRoot
        //      Get
        //              Return Me
        //      End Get
        //End Property

        //The ReadOnly property Count returns the number
        //of items in the custom collection.
        public new int Count
        {
            //Implements ICollection.Count
            get { return base.InnerXMLNode.ChildNodes.Count; }
        }
        #endregion

        #region "Utility Functions"
        private string DoTransform(string XML, string XSL)
        {
            return DoTransform(XML, XSL, null);
        }

        private string DoTransform(string XML, string XSL, System.Xml.Xsl.XsltArgumentList Params)
        {
            try
            {
                System.Xml.Xsl.XslTransform oXsl = new System.Xml.Xsl.XslTransform();
                oXsl.Load(XSL);
                XmlTextReader oXR = new XmlTextReader(XML, XmlNodeType.Document, null);

                System.Xml.XPath.XPathDocument oXml = new System.Xml.XPath.XPathDocument(oXR);
                XmlUrlResolver oResolver = new XmlUrlResolver();

                System.Text.StringBuilder oSB = new System.Text.StringBuilder();
                System.IO.StringWriter oWriter = new System.IO.StringWriter(oSB, null);
                oXsl.Transform(oXml, Params, oWriter, oResolver);
                oWriter.Close();
                return oSB.ToString();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }

    class NodeEnumerator : IEnumerator
    {

        private XmlNode m_objXMLNode;
        private int m_intCursor;

        public NodeEnumerator(XmlNode objRoot)
        {
            m_objXMLNode = objRoot;
            m_intCursor = -1;
        }

        public void Reset()
        {
            m_intCursor = -1;
        }

        public bool MoveNext()
        {
            if (m_intCursor < m_objXMLNode.ChildNodes.Count)
            {
                m_intCursor = m_intCursor + 1;
            }

            if ((m_intCursor == m_objXMLNode.ChildNodes.Count))
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
                if (((m_intCursor < 0) | (m_intCursor == m_objXMLNode.ChildNodes.Count)))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return new Node(m_objXMLNode.ChildNodes[m_intCursor]);
                }
            }
        }

    }

}
