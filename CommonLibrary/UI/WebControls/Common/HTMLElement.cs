using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.UI.WebControls.Common
{
    public class HTMLElement
    {

        private string m_strRaw;
        private string m_strText;
        private string m_strTagName;
        private Hashtable m_objAttributes = new Hashtable();

        #region "Constructors"
        public HTMLElement(string TagName)
        {
            m_strTagName = TagName;
        }
        #endregion

        public string Raw
        {
            get { return m_strRaw; }
            set { m_strRaw = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public string TagName
        {
            get { return m_strTagName; }
            set { m_strTagName = value; }
        }

        public Hashtable Attributes
        {
            get { return m_objAttributes; }
        }

        public string ToJSON()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string strAttr in this.Attributes.Keys)
            {
                if (sb.Length == 0)
                {
                    sb.Append("{");
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(string.Format("{0}:{1}", strAttr, SafeJSONString((string)this.Attributes[strAttr])));
            }
            if (!String.IsNullOrEmpty(this.Text))
            {
                if (sb.Length == 0)
                {
                    sb.Append("{");
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(string.Format("{0}:{1}", "__text", SafeJSONString(this.Text)));
            }
            sb.Append("}");
            return sb.ToString();
        }

        private string SafeJSONString(string strString)
        {
            //TODO: Move this to Utility. ClientAPI!
            return "'" + ((strString.Replace(((char)13).ToString(), "\\r")).Replace(((char)10).ToString(), "\\n")).Replace("'", "\\'") + "'";
        }

    }

}
