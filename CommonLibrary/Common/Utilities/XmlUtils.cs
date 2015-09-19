using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections;
using System.Xml.XPath;

namespace CommonLibrary.Common.Utilities
{
    /// <summary>
    /// String utility class that provides a host of string related operations
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        /// Retrieves a result string from an XPATH query. Null if not found.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="XPath"></param>
        /// <returns></returns>
        public static string GetXmlString(XmlNode node, string XPath, XmlNamespaceManager ns)
        {
            XmlNode selNode = node.SelectSingleNode(XPath, ns);
            if (selNode == null)
                return null;

            return selNode.InnerText;
        }

        /// <summary>
        /// Retrieves a result int value from an XPATH query. 0 if not found.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="XPath"></param>
        /// <returns></returns>
        public static int GetXmlInt(XmlNode node, string XPath, XmlNamespaceManager ns)
        {
            string val = GetXmlString(node, XPath, ns);
            if (val == null)
                return 0;

            int result = 0;
            int.TryParse(val, out result);

            return result;
        }

        /// <summary>
        /// Retrieves a result bool from an XPATH query. false if not found.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="XPath"></param>
        /// <returns></returns>
        public static bool GetXmlBool(XmlNode node, string XPath, XmlNamespaceManager ns)
        {
            string val = GetXmlString(node, XPath, ns);
            if (val == null)
                return false;

            if (val == "1" || val == "true" || val == "True")
                return true;

            return false;
        }

        /// <summary>
        /// Retrieves a result DateTime from an XPATH query. 1/1/1900  if not found.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="XPath"></param>
        /// <returns></returns>
        public static DateTime GetXmlDateTime(XmlNode node, string XPath, XmlNamespaceManager ns)
        {
            DateTime dtVal = new DateTime(1900, 1, 1, 0, 0, 0);

            string val = GetXmlString(node, XPath, ns);
            if (val == null)
                return dtVal;

            try
            {
                dtVal = XmlConvert.ToDateTime(val, XmlDateTimeSerializationMode.Utc);
            }
            catch { }

            return dtVal;
        }

        /// <summary>
        /// Gets an attribute by name
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns>value or null if not available</returns>
        public static string GetXmlAttributeString(XmlNode node, string attributeName)
        {
            XmlAttribute att = node.Attributes[attributeName];
            if (att == null)
                return null;

            return att.InnerText;
        }

        /// <summary>
        /// Returns an integer value from an attribute
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetXmlAttributeInt(XmlNode node, string attributeName, int defaultValue)
        {
            string val = GetXmlAttributeString(node, attributeName);
            if (val == null)
                return defaultValue;

            return XmlConvert.ToInt32(val);
        }



        /// <summary>
        /// Converts a .NET type into an XML compatible type - roughly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MapTypeToXmlType(Type type)
        {
            if (type == typeof(string) || type == typeof(char))
                return "string";
            if (type == typeof(int) || type == typeof(Int32))
                return "integer";
            if (type == typeof(long) || type == typeof(Int64))
                return "long";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(DateTime))
                return "datetime";

            if (type == typeof(float))
                return "float";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(double))
                return "double";
            if (type == typeof(Single))
                return "single";

            if (type == typeof(byte))
                return "byte";

            if (type == typeof(byte[]))
                return "base64Binary";

            return null;

            // *** hope for the best
            //return type.ToString().ToLower();
        }


        public static Type MapXmlTypeToType(string xmlType)
        {
            xmlType = xmlType.ToLower();

            if (xmlType == "string")
                return typeof(string);
            if (xmlType == "integer")
                return typeof(int);
            if (xmlType == "long")
                return typeof(long);
            if (xmlType == "boolean")
                return typeof(bool);
            if (xmlType == "datetime")
                return typeof(DateTime);
            if (xmlType == "float")
                return typeof(float);
            if (xmlType == "decimal")
                return typeof(decimal);
            if (xmlType == "double")
                return typeof(Double);
            if (xmlType == "single")
                return typeof(Single);

            if (xmlType == "byte")
                return typeof(byte);
            if (xmlType == "base64binary")
                return typeof(byte[]);


            // return null if no match is found
            // don't throw so the caller can decide more efficiently what to do 
            // with this error result
            return null;
        }


        /// <summary>
        /// Creates an Xml NamespaceManager for an XML document by looking
        /// at all of the namespaces defined on the document root element.
        /// </summary>
        /// <param name="doc">The XmlDom instance to attach the namespacemanager to</param>
        /// <param name="defaultNamespace">The prefix to use for prefix-less nodes (which are not supported if any namespaces are used in XmlDoc).</param>
        /// <returns></returns>
        public static XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument doc, string defaultNamespace)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            foreach (XmlAttribute attr in doc.DocumentElement.Attributes)
            {
                if (attr.Prefix == "xmlns")
                    nsmgr.AddNamespace(attr.LocalName, attr.Value);
                if (attr.Name == "xmlns")
                    // default namespace MUST use a prefix
                    nsmgr.AddNamespace(defaultNamespace, attr.Value);
            }

            return nsmgr;
        }

        public static void AppendElement(ref XmlDocument objDoc, XmlNode objNode, string attName, string attValue, bool includeIfEmpty)
        {
            AppendElement(ref objDoc, objNode, attName, attValue, includeIfEmpty, false);
        }
        public static void AppendElement(ref XmlDocument objDoc, XmlNode objNode, string attName, string attValue, bool includeIfEmpty, bool CDATA)
        {
            if (String.IsNullOrEmpty(attValue) && !includeIfEmpty)
            {
                return;
            }
            if (CDATA)
            {
                objNode.AppendChild(CreateCDataElement(objDoc, attName, attValue));
            }
            else
            {
                objNode.AppendChild(CreateElement(objDoc, attName, attValue));
            }
        }
        public static XmlAttribute CreateAttribute(XmlDocument objDoc, string attName, string attValue)
        {
            XmlAttribute attribute = objDoc.CreateAttribute(attName);
            attribute.Value = attValue;
            return attribute;
        }
        public static void CreateAttribute(XmlDocument objDoc, XmlNode objNode, string attName, string attValue)
        {
            XmlAttribute attribute = objDoc.CreateAttribute(attName);
            attribute.Value = attValue;
            objNode.Attributes.Append(attribute);
        }
        public static XmlElement CreateElement(XmlDocument objDoc, string NodeName)
        {
            return objDoc.CreateElement(NodeName);
        }
        public static XmlElement CreateElement(XmlDocument objDoc, string NodeName, string NodeValue)
        {
            XmlElement element = objDoc.CreateElement(NodeName);
            element.InnerText = NodeValue;
            return element;
        }
        public static XmlElement CreateCDataElement(XmlDocument objDoc, string NodeName, string NodeValue)
        {
            XmlElement element = objDoc.CreateElement(NodeName);
            element.AppendChild(objDoc.CreateCDataSection(NodeValue));
            return element;
        }
        public static object Deserialize(string xmlObject, System.Type type)
        {
            XmlSerializer ser = new XmlSerializer(type);
            StringReader sr = new StringReader(xmlObject);
            object obj = ser.Deserialize(sr);
            sr.Close();
            return obj;
        }
        //public static object Deserialize(Stream objStream, System.Type type)
        //{
        //    object obj = Activator.CreateInstance(type);
        //    Dictionary<int, TabInfo> tabDic = obj as Dictionary<int, TabInfo>;
        //    if (tabDic != null)
        //    {
        //        obj = DeSerializeDictionary<TabInfo>(objStream, "dictionary");
        //        return obj;
        //    }
        //    Dictionary<int, ModuleInfo> moduleDic = obj as Dictionary<int, ModuleInfo>;
        //    if (moduleDic != null)
        //    {
        //        obj = DeSerializeDictionary<ModuleInfo>(objStream, "dictionary");
        //        return obj;
        //    }
        //    Dictionary<int, TabPermissionCollection> tabPermDic = obj as Dictionary<int, TabPermissionCollection>;
        //    if (tabPermDic != null)
        //    {
        //        obj = DeSerializeDictionary<TabPermissionCollection>(objStream, "dictionary");
        //        return obj;
        //    }
        //    Dictionary<int, ModulePermissionCollection> modPermDic = obj as Dictionary<int, ModulePermissionCollection>;
        //    if (modPermDic != null)
        //    {
        //        obj = DeSerializeDictionary<ModulePermissionCollection>(objStream, "dictionary");
        //        return obj;
        //    }
        //    XmlSerializer serializer = new XmlSerializer(type);
        //    TextReader tr = new StreamReader(objStream);
        //    obj = serializer.Deserialize(tr);
        //    tr.Close();
        //    return obj;
        //}
        public static Dictionary<int, TValue> DeSerializeDictionary<TValue>(Stream objStream, string rootname)
        {
            Dictionary<int, TValue> objDictionary = null;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(objStream);
            objDictionary = new Dictionary<int, TValue>();
            foreach (XmlElement xmlItem in xmlDoc.SelectNodes(rootname + "/item"))
            {
                int key = Convert.ToInt32(xmlItem.GetAttribute("key"));
                string typeName = xmlItem.GetAttribute("type");
                TValue objValue = Activator.CreateInstance<TValue>();
                XmlSerializer xser = new XmlSerializer(objValue.GetType());
                XmlTextReader reader = new XmlTextReader(new StringReader(xmlItem.InnerXml));
                objDictionary.Add(key, (TValue)xser.Deserialize(reader));
            }
            return objDictionary;
        }
        public static Hashtable DeSerializeHashtable(string xmlSource, string rootname)
        {
            Hashtable objHashTable;
            if (!String.IsNullOrEmpty(xmlSource))
            {
                objHashTable = new Hashtable();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlSource);
                foreach (XmlElement xmlItem in xmlDoc.SelectNodes(rootname + "/item"))
                {
                    string key = xmlItem.GetAttribute("key");
                    string typeName = xmlItem.GetAttribute("type");
                    XmlSerializer xser = new XmlSerializer(Type.GetType(typeName));
                    XmlTextReader reader = new XmlTextReader(new StringReader(xmlItem.InnerXml));
                    objHashTable.Add(key, xser.Deserialize(reader));
                }
            }
            else
            {
                objHashTable = new Hashtable();
            }
            return objHashTable;
        }
        public static string GetAttributeValue(XPathNavigator nav, string AttributeName)
        {
            return nav.GetAttribute(AttributeName, "");
        }
        public static int GetAttributeValueAsInteger(XPathNavigator nav, string AttributeName, int DefaultValue)
        {
            int intValue = DefaultValue;
            string strValue = GetAttributeValue(nav, AttributeName);
            if (!string.IsNullOrEmpty(strValue))
            {
                intValue = Convert.ToInt32(strValue);
            }
            return intValue;
        }
        public static string GetNodeValue(XPathNavigator nav, string path)
        {
            string strValue = Null.NullString;
            XPathNavigator elementNav = nav.SelectSingleNode(path);
            if (elementNav != null)
            {
                strValue = elementNav.Value;
            }
            return strValue;
        }
        public static string GetNodeValue(XmlNode objNode, string NodeName, string DefaultValue)
        {
            string strValue = DefaultValue;
            if ((objNode[NodeName] != null))
            {
                strValue = objNode[NodeName].InnerText;
                if (String.IsNullOrEmpty(strValue) && !String.IsNullOrEmpty(DefaultValue))
                {
                    strValue = DefaultValue;
                }
            }
            return strValue;
        }
        public static bool GetNodeValueBoolean(XmlNode objNode, string NodeName)
        {
            return GetNodeValueBoolean(objNode, NodeName, false);
        }
        public static bool GetNodeValueBoolean(XmlNode objNode, string NodeName, bool DefaultValue)
        {
            bool bValue = DefaultValue;
            if ((objNode[NodeName] != null))
            {
                string strValue = objNode[NodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    bValue = Convert.ToBoolean(strValue);
                }
            }
            return bValue;
        }
        public static DateTime GetNodeValueDate(XmlNode objNode, string NodeName, DateTime DefaultValue)
        {
            DateTime dateValue = DefaultValue;
            if ((objNode[NodeName] != null))
            {
                string strValue = objNode[NodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    dateValue = Convert.ToDateTime(strValue);
                    if (dateValue.Date.Equals(Null.NullDate.Date))
                    {
                        dateValue = Null.NullDate;
                    }
                }
            }
            return dateValue;
        }
        public static int GetNodeValueInt(XmlNode objNode, string NodeName)
        {
            return GetNodeValueInt(objNode, NodeName, 0);
        }
        public static int GetNodeValueInt(XmlNode objNode, string NodeName, int DefaultValue)
        {
            int intValue = DefaultValue;
            if ((objNode[NodeName] != null))
            {
                string strValue = objNode[NodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    intValue = Convert.ToInt32(strValue);
                }
            }
            return intValue;
        }
        public static float GetNodeValueSingle(XmlNode objNode, string NodeName)
        {
            return GetNodeValueSingle(objNode, NodeName, 0);
        }
        public static float GetNodeValueSingle(XmlNode objNode, string NodeName, float DefaultValue)
        {
            float sValue = DefaultValue;
            if ((objNode[NodeName] != null))
            {
                string strValue = objNode[NodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    sValue = Convert.ToSingle(strValue, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return sValue;
        }
        public static XmlWriterSettings GetXmlWriterSettings(ConformanceLevel conformance)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = conformance;
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            return settings;
        }
        public static void UpdateAttribute(XmlNode node, string attName, string attValue)
        {
            if (node != null)
            {
                XmlAttribute attrib = node.Attributes[attName];
                attrib.InnerText = attValue;
            }
        }
        public static string XMLEncode(string HTML)
        {
            return "<![CDATA[" + HTML + "]]>";
        }
        public static void XSLTransform(XmlDocument doc, ref StreamWriter writer, string xsltUrl)
        {
            System.Xml.Xsl.XslCompiledTransform xslt = new System.Xml.Xsl.XslCompiledTransform();
            xslt.Load(xsltUrl);
            xslt.Transform(doc, null, writer);
        }
        public static string Serialize(object obj)
        {
            string xmlObject;
            IDictionary dic = obj as IDictionary;
            if (dic != null)
            {
                xmlObject = SerializeDictionary(dic, "dictionary");
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xser = new XmlSerializer(obj.GetType());
                StringWriter sw = new StringWriter();
                xser.Serialize(sw, obj);
                xmlDoc.LoadXml(sw.GetStringBuilder().ToString());
                XmlNode xmlDocEl = xmlDoc.DocumentElement;
                xmlDocEl.Attributes.Remove(xmlDocEl.Attributes["xmlns:xsd"]);
                xmlDocEl.Attributes.Remove(xmlDocEl.Attributes["xmlns:xsi"]);
                xmlObject = xmlDocEl.OuterXml;
            }
            return xmlObject;
        }
        public static void SerializeHashtable(Hashtable Hashtable, XmlDocument XmlDocument, XmlNode RootNode, string ElementName, string KeyField, string ValueField)
        {
            string sOuterElementName;
            string sInnerElementName;
            XmlNode nodeSetting;
            XmlNode nodeSettings;
            XmlNode nodeSettingName;
            XmlNode nodeSettingValue;
            sOuterElementName = ElementName + "s";
            sInnerElementName = ElementName;
            nodeSettings = RootNode.AppendChild(XmlDocument.CreateElement(sOuterElementName));
            foreach (string sKey in Hashtable.Keys)
            {
                nodeSetting = nodeSettings.AppendChild(XmlDocument.CreateElement(sInnerElementName));
                nodeSettingName = nodeSetting.AppendChild(XmlDocument.CreateElement(KeyField));
                nodeSettingName.InnerText = sKey;
                nodeSettingValue = nodeSetting.AppendChild(XmlDocument.CreateElement(ValueField));
                nodeSettingValue.InnerText = Hashtable[sKey].ToString();
            }
        }
        public static string SerializeDictionary(IDictionary Source, string rootName)
        {
            string strString;
            if (Source.Count != 0)
            {
                XmlSerializer xser;
                StringWriter sw;
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement xmlRoot = xmlDoc.CreateElement(rootName);
                xmlDoc.AppendChild(xmlRoot);
                foreach (object key in Source.Keys)
                {
                    XmlElement xmlItem = xmlDoc.CreateElement("item");
                    xmlItem.SetAttribute("key", Convert.ToString(key));
                    xmlItem.SetAttribute("type", Source[key].GetType().AssemblyQualifiedName.ToString());
                    XmlDocument xmlObject = new XmlDocument();
                    xser = new XmlSerializer(Source[key].GetType());
                    sw = new StringWriter();
                    xser.Serialize(sw, Source[key]);
                    xmlObject.LoadXml(sw.ToString());
                    xmlItem.AppendChild(xmlDoc.ImportNode(xmlObject.DocumentElement, true));
                    xmlRoot.AppendChild(xmlItem);
                }
                strString = xmlDoc.OuterXml;
            }
            else
            {
                strString = "";
            }
            return strString;
        }
    }
}
