using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using CommonLibrary.UI.WebControls.Common;

namespace CommonLibrary.UI.WebControls
{
    public class NodeEnumerator : IEnumerator
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
                m_intCursor++;
            }

            if (m_intCursor == m_objXMLNode.ChildNodes.Count)
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
                if ((m_intCursor < 0) || (m_intCursor == m_objXMLNode.ChildNodes.Count))
                {
                    throw (new InvalidOperationException());
                }
                else
                {
                    return new Node(m_objXMLNode.ChildNodes[m_intCursor]);
                }
            }
        }
    }

}
