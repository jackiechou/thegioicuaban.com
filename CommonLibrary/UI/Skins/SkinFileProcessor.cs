using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommonLibrary.Services.Installer;
using System.Xml;
using System.Collections;
using System.Web;
using System.IO;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.UI.Skins
{
    public enum SkinParser
    {
        Localized,
        Portable
    }
    public class SkinFileProcessor
    {
        private readonly string m_SkinRoot;
        private readonly string m_SkinPath;
        private readonly string m_SkinName;
        private readonly XmlDocument m_SkinAttributes = new XmlDocument();
        private readonly PathParser m_PathFactory = new PathParser();
        private readonly ControlParser m_ControlFactory;
        private readonly ObjectParser m_ObjectFactory;
        private readonly Hashtable m_ControlList = new Hashtable();
        private string m_Message = "";
        private string INITIALIZE_PROCESSOR = Util.GetLocalizedString("StartProcessor");
        private string PACKAGE_LOAD = Util.GetLocalizedString("PackageLoad");
        private string PACKAGE_LOAD_ERROR = Util.GetLocalizedString("PackageLoad.Error");
        private string DUPLICATE_ERROR = Util.GetLocalizedString("DuplicateSkinObject.Error");
        private string DUPLICATE_DETAIL = Util.GetLocalizedString("DuplicateSkinObject.Detail");
        private string LOAD_SKIN_TOKEN = Util.GetLocalizedString("LoadingSkinToken");
        private string FILE_BEGIN = Util.GetLocalizedString("BeginSkinFile");
        private string FILE_END = Util.GetLocalizedString("EndSkinFile");
        private string FILES_END = Util.GetLocalizedString("EndSkinFiles");
        private PathParser PathFactory
        {
            get { return m_PathFactory; }
        }
        private ControlParser ControlFactory
        {
            get { return m_ControlFactory; }
        }
        private ObjectParser ObjectFactory
        {
            get { return m_ObjectFactory; }
        }
        private XmlDocument SkinAttributes
        {
            get { return m_SkinAttributes; }
        }
        private string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
        public string SkinRoot
        {
            get { return m_SkinRoot; }
        }
        public string SkinPath
        {
            get { return m_SkinPath; }
        }
        public string SkinName
        {
            get { return m_SkinName; }
        }
        public SkinFileProcessor(string ControlKey, string ControlSrc)
        {
            m_ControlList.Add(ControlKey, ControlSrc);
            m_ControlFactory = new ControlParser(m_ControlList);
            m_ObjectFactory = new ObjectParser(m_ControlList);
        }
        public SkinFileProcessor(string SkinPath, string SkinRoot, string SkinName)
        {
            this.Message += SkinController.FormatMessage(INITIALIZE_PROCESSOR, SkinRoot + " :: " + SkinName, 0, false);
            m_SkinRoot = SkinRoot;
            m_SkinPath = SkinPath;
            m_SkinName = SkinName;
            string FileName = this.SkinPath + this.SkinRoot + "\\" + this.SkinName + "\\" + SkinRoot.Substring(0, SkinRoot.Length - 1) + ".xml";
            if (File.Exists(FileName))
            {
                try
                {
                    this.SkinAttributes.Load(FileName);
                    this.Message += SkinController.FormatMessage(PACKAGE_LOAD, Path.GetFileName(FileName), 2, false);
                }
                catch (Exception ex)
                {
                    this.Message += SkinController.FormatMessage(string.Format(PACKAGE_LOAD_ERROR, ex.Message), Path.GetFileName(FileName), 2, true);
                }
            }
            string Token;
            foreach (SkinControlInfo objSkinControl in SkinControlController.GetSkinControls().Values)
            {
                Token = objSkinControl.ControlKey.ToUpper();
                if (m_ControlList.ContainsKey(Token))
                {
                    this.Message += SkinController.FormatMessage(string.Format(DUPLICATE_ERROR, objSkinControl.ControlKey.ToString().ToUpper()), string.Format(DUPLICATE_DETAIL, (string)m_ControlList[Token], objSkinControl.ControlSrc.ToString()), 2, true);
                }
                else
                {
                    this.Message += SkinController.FormatMessage(string.Format(LOAD_SKIN_TOKEN, objSkinControl.ControlKey.ToString().ToUpper()), objSkinControl.ControlSrc.ToString(), 2, false);
                    m_ControlList.Add(Token, objSkinControl.ControlSrc);
                }
            }
            m_ControlFactory = new ControlParser(m_ControlList);
            m_ObjectFactory = new ObjectParser(m_ControlList);
        }
        public string ProcessFile(string FileName, UI.Skins.SkinParser ParseOption)
        {
            string strMessage = SkinController.FormatMessage(FILE_BEGIN, Path.GetFileName(FileName), 0, false);
            SkinFile objSkinFile = new SkinFile(this.SkinRoot, FileName, this.SkinAttributes);
            switch (objSkinFile.FileExtension)
            {
                case ".htm":
                case ".html":
                    string contents = objSkinFile.Contents;
                    strMessage += this.ObjectFactory.Parse(ref contents);
                    strMessage += this.PathFactory.Parse(ref contents, this.PathFactory.HTMLList, objSkinFile.SkinRootPath, ParseOption);
                    strMessage += this.ControlFactory.Parse(ref contents, objSkinFile.Attributes);
                    objSkinFile.Contents = contents;
                    ArrayList Registrations = new ArrayList();
                    Registrations.AddRange(this.ControlFactory.Registrations);
                    Registrations.AddRange(this.ObjectFactory.Registrations);
                    strMessage += objSkinFile.PrependASCXDirectives(Registrations);
                    break;
            }
            objSkinFile.Write();
            strMessage += objSkinFile.Messages;
            strMessage += SkinController.FormatMessage(FILE_END, Path.GetFileName(FileName), 1, false);
            return strMessage;
        }
        public string ProcessList(ArrayList FileList)
        {
            return ProcessList(FileList, SkinParser.Localized);
        }
        public string ProcessList(ArrayList FileList, UI.Skins.SkinParser ParseOption)
        {
            foreach (string FileName in FileList)
            {
                this.Message += ProcessFile(FileName, ParseOption);
            }
            this.Message += SkinController.FormatMessage(FILES_END, this.SkinRoot + " :: " + this.SkinName, 0, false);
            return this.Message;
        }
        public string ProcessSkin(string SkinSource, XmlDocument SkinAttributes, UI.Skins.SkinParser ParseOption)
        {
            SkinFile objSkinFile = new SkinFile(SkinSource, SkinAttributes);
            string contents = objSkinFile.Contents;
            this.Message += this.ControlFactory.Parse(ref contents, objSkinFile.Attributes);
            this.Message += objSkinFile.PrependASCXDirectives(this.ControlFactory.Registrations);
            return contents;
        }
        private class SkinFile
        {
            private readonly string m_FileName;
            private readonly string m_FileExtension;
            private readonly string m_WriteFileName;
            private readonly string m_SkinRoot;
            private readonly string m_SkinRootPath;
            private readonly XmlDocument m_FileAttributes;
            private string m_FileContents;
            private string m_Messages = "";
            private string FILE_FORMAT_ERROR = Util.GetLocalizedString("FileFormat.Error");
            private string FILE_FORMAT_DETAIL = Util.GetLocalizedString("FileFormat.Detail");
            private string FILE_LOAD = Util.GetLocalizedString("SkinFileLoad");
            private string FILE_LOAD_ERROR = Util.GetLocalizedString("SkinFileLoad.Error");
            private string FILE_WRITE = Util.GetLocalizedString("FileWrite");
            private string CONTROL_DIR = Util.GetLocalizedString("ControlDirective");
            private string CONTROL_REG = Util.GetLocalizedString("ControlRegister");
            public string SkinRoot
            {
                get { return m_SkinRoot; }
            }
            public XmlDocument Attributes
            {
                get { return m_FileAttributes; }
            }
            public string Messages
            {
                get { return m_Messages; }
            }
            public string FileName
            {
                get { return m_FileName; }
            }
            public string WriteFileName
            {
                get { return m_WriteFileName; }
            }
            public string FileExtension
            {
                get { return m_FileExtension; }
            }
            public string SkinRootPath
            {
                get { return m_SkinRootPath; }
            }
            public string Contents
            {
                get { return m_FileContents; }
                set { m_FileContents = value; }
            }
            public SkinFile(string SkinContents, XmlDocument SkinAttributes)
            {
                m_FileAttributes = SkinAttributes;
                this.Contents = SkinContents;
            }
            public SkinFile(string SkinRoot, string FileName, XmlDocument SkinAttributes)
            {
                m_FileName = FileName;
                m_FileExtension = Path.GetExtension(FileName);
                m_SkinRoot = SkinRoot;
                m_FileAttributes = SkinAttributes;
                string strTemp = FileName.Replace(Path.GetFileName(FileName), "");
                strTemp = strTemp.Replace("\\", "/");
                m_SkinRootPath = Common.Globals.ApplicationPath + strTemp.Substring(0, strTemp.ToUpper().IndexOf("/PORTALS"));
                this.Contents = Read(FileName);
                switch (this.FileExtension)
                {
                    case ".htm":
                    case ".html":
                        m_WriteFileName = FileName.Replace(Path.GetExtension(FileName), ".ascx");
                        Regex PaneCheck1 = new Regex("\\s*id\\s*=\\s*\"" + Common.Globals.glbDefaultPane + "\"", RegexOptions.IgnoreCase);
                        Regex PaneCheck2 = new Regex("\\s*[" + Common.Globals.glbDefaultPane + "]", RegexOptions.IgnoreCase);
                        if (PaneCheck1.IsMatch(this.Contents) == false && PaneCheck2.IsMatch(this.Contents) == false)
                        {
                            m_Messages += SkinController.FormatMessage(FILE_FORMAT_ERROR, string.Format(FILE_FORMAT_ERROR, FileName), 2, true);
                        }
                        if (File.Exists(FileName.Replace(this.FileExtension, ".xml")))
                        {
                            try
                            {
                                m_FileAttributes.Load(FileName.Replace(this.FileExtension, ".xml"));
                                m_Messages += SkinController.FormatMessage(FILE_LOAD, FileName, 2, false);
                            }
                            catch (Exception ex)
                            {
                                m_FileAttributes = SkinAttributes;
                                m_Messages += SkinController.FormatMessage(FILE_LOAD_ERROR, FileName, 2, true);
                                ex.ToString();
                            }
                        }
                        break;
                    default:
                        m_WriteFileName = FileName;
                        break;
                }
            }
            private string Read(string FileName)
            {
                StreamReader objStreamReader = new StreamReader(FileName);
                string strFileContents = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                return strFileContents;
            }
            public void Write()
            {
                if (File.Exists(this.WriteFileName))
                {
                    File.Delete(this.WriteFileName);
                }
                m_Messages += SkinController.FormatMessage(FILE_WRITE, Path.GetFileName(this.WriteFileName), 2, false);
                StreamWriter objStreamWriter = new StreamWriter(this.WriteFileName);
                objStreamWriter.WriteLine(this.Contents);
                objStreamWriter.Flush();
                objStreamWriter.Close();
            }
            public string PrependASCXDirectives(ArrayList Registrations)
            {
                string Messages = "";
                string Prefix = "";
                string strPattern = "<\\s*body[^>]*>(?<skin>.*)<\\s*/\\s*body\\s*>";
                Match objMatch;
                objMatch = Regex.Match(this.Contents, strPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (!String.IsNullOrEmpty(objMatch.Groups[1].Value))
                {
                    this.Contents = objMatch.Groups[1].Value;
                }
                if (this.SkinRoot == SkinController.RootSkin)
                {
                    Prefix += "<%@ Control language=\"vb\" AutoEventWireup=\"false\" Explicit=\"True\" Inherits=\"DotNetNuke.UI.Skins.Skin\" %>" + Environment.NewLine;
                }
                else if (this.SkinRoot == SkinController.RootContainer)
                {
                    Prefix += "<%@ Control language=\"vb\" AutoEventWireup=\"false\" Explicit=\"True\" Inherits=\"DotNetNuke.UI.Containers.Container\" %>" + Environment.NewLine;
                }
                Messages += SkinController.FormatMessage(CONTROL_DIR, HttpUtility.HtmlEncode(Prefix), 2, false);
                foreach (string Item in Registrations)
                {
                    Messages += SkinController.FormatMessage(CONTROL_REG, HttpUtility.HtmlEncode(Item), 2, false);
                    Prefix += Item;
                }
                this.Contents = Prefix + this.Contents;
                return Messages;
            }
        }
        private class PathParser
        {
            private ArrayList m_HTMLPatterns = new ArrayList();
            private ArrayList m_CSSPatterns = new ArrayList();
            private string m_SkinPath = "";
            private UI.Skins.SkinParser m_ParseOption;
            private string m_Messages = "";
            private string SUBST = Util.GetLocalizedString("Substituting");
            private string SUBST_DETAIL = Util.GetLocalizedString("Substituting.Detail");
            public ArrayList HTMLList
            {
                get
                {
                    if (m_HTMLPatterns.Count == 0)
                    {
                        string[] arrPattern = {
							"(?<tag><head[^>]*?\\sprofile\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><object[^>]*?\\s(?:codebase|data|usemap)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><img[^>]*?\\s(?:src|longdesc|usemap)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><input[^>]*?\\s(?:src|usemap)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><iframe[^>]*?\\s(?:src|longdesc)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><(?:td|th|table|body)[^>]*?\\sbackground\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><(?:script|bgsound|embed|xml|frame)[^>]*?\\ssrc\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><(?:base|link|a|area)[^>]*?\\shref\\s*=\\s*\")(?!https://|http://|\\\\|[~/]|javascript:|mailto:)(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><(?:blockquote|ins|del|q)[^>]*?\\scite\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><(?:param\\s+name\\s*=\\s*\"(?:movie|src|base)\")[^>]*?\\svalue\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)",
							"(?<tag><embed[^>]*?\\s(?:src)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)"
						};
                        int i;
                        for (i = 0; i <= arrPattern.GetLength(0) - 1; i++)
                        {
                            Regex re = new Regex(arrPattern[i], RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            m_HTMLPatterns.Add(re);
                        }
                        m_HTMLPatterns.TrimToSize();
                    }
                    return m_HTMLPatterns;
                }
            }
            public ArrayList CSSList
            {
                get
                {
                    if (m_CSSPatterns.Count == 0)
                    {
                        string[] arrPattern = { "(?<tag>\\surl\\u0028)(?<content>[^\\u0029]*)(?<endtag>\\u0029.*;)" };
                        int i;
                        for (i = 0; i <= arrPattern.GetLength(0) - 1; i++)
                        {
                            Regex re = new Regex(arrPattern[i], RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            m_CSSPatterns.Add(re);
                        }
                        m_CSSPatterns.TrimToSize();
                    }
                    return m_CSSPatterns;
                }
            }
            private MatchEvaluator Handler
            {
                get { return MatchHandler; }
            }
            private string SkinPath
            {
                get { return m_SkinPath; }
                set { m_SkinPath = value; }
            }
            private UI.Skins.SkinParser ParseOption
            {
                get { return m_ParseOption; }
                set { m_ParseOption = value; }
            }
            public string Parse(ref string Source, ArrayList RegexList, string SkinPath, UI.Skins.SkinParser ParseOption)
            {
                m_Messages = "";
                this.SkinPath = SkinPath;
                this.ParseOption = ParseOption;
                int i;
                for (i = 0; i <= RegexList.Count - 1; i++)
                {
                    Source = ((Regex)RegexList[i]).Replace(Source, this.Handler);
                }
                return m_Messages;
            }
            private string MatchHandler(Match m)
            {
                string strOldTag = m.Groups["tag"].Value + m.Groups["content"].Value + m.Groups["endtag"].Value;
                string strNewTag = strOldTag;
                if (!m.Groups[0].Value.ToLower().Contains("codetype=\"dotnetnuke/client\""))
                {
                    switch (this.ParseOption)
                    {
                        case SkinParser.Localized:
                            if (strNewTag.IndexOf(this.SkinPath) == -1)
                            {
                                strNewTag = m.Groups["tag"].Value + this.SkinPath + m.Groups["content"].Value + m.Groups["endtag"].Value;
                            }
                            break;
                        case SkinParser.Portable:
                            if (strNewTag.ToLower().IndexOf("<%= skinpath %>") == -1)
                            {
                                strNewTag = m.Groups["tag"].Value + "<%= SkinPath %>" + m.Groups["content"].Value + m.Groups["endtag"].Value;
                            }
                            if (strNewTag.IndexOf(this.SkinPath) != -1)
                            {
                                strNewTag = strNewTag.Replace(this.SkinPath, "");
                            }
                            break;
                    }
                }
                m_Messages += SkinController.FormatMessage(SUBST, string.Format(SUBST_DETAIL, System.Web.HttpUtility.HtmlEncode(strOldTag), System.Web.HttpUtility.HtmlEncode(strNewTag)), 2, false);
                return strNewTag;
            }
        }
        private class ControlParser
        {
            private readonly Hashtable m_ControlList = new Hashtable();
            private readonly string m_InitMessages = "";
            private ArrayList m_RegisterList = new ArrayList();
            private XmlDocument m_Attributes = new XmlDocument();
            private string m_ParseMessages = "";
            internal ArrayList Registrations
            {
                get { return m_RegisterList; }
            }
            private MatchEvaluator Handler
            {
                get { return TokenMatchHandler; }
            }
            private ArrayList RegisterList
            {
                get { return m_RegisterList; }
                set { m_RegisterList = value; }
            }
            private Hashtable ControlList
            {
                get { return m_ControlList; }
            }
            private XmlDocument Attributes
            {
                get { return m_Attributes; }
                set { m_Attributes = value; }
            }
            private string Messages
            {
                get { return m_ParseMessages; }
                set { m_ParseMessages = value; }
            }
            public ControlParser(Hashtable ControlList)
            {
                m_ControlList = (Hashtable)ControlList.Clone();
            }
            public string Parse(ref string Source, XmlDocument Attributes)
            {
                this.Messages = m_InitMessages;
                this.Attributes = Attributes;
                this.RegisterList.Clear();
                Regex FindTokenInstance = new Regex("\\[\\s*(?<token>\\w*)\\s*:?\\s*(?<instance>\\w*)\\s*]", RegexOptions.IgnoreCase);
                Source = FindTokenInstance.Replace(Source, this.Handler);
                return Messages;
            }
            private string TokenMatchHandler(Match m)
            {
                string TOKEN_PROC = Util.GetLocalizedString("ProcessToken");
                string TOKEN_SKIN = Util.GetLocalizedString("SkinToken");
                string TOKEN_PANE = Util.GetLocalizedString("PaneToken");
                string TOKEN_FOUND = Util.GetLocalizedString("TokenFound");
                string TOKEN_FORMAT = Util.GetLocalizedString("TokenFormat");
                string TOKEN_NOTFOUND_INFILE = Util.GetLocalizedString("TokenNotFoundInFile");
                string CONTROL_FORMAT = Util.GetLocalizedString("ControlFormat");
                string TOKEN_NOTFOUND = Util.GetLocalizedString("TokenNotFound");
                string Token = m.Groups["token"].Value.ToUpper();
                string ControlName = Token + m.Groups["instance"].Value;
                string AttributeNode = Token + (String.IsNullOrEmpty(m.Groups["instance"].Value) ? "" : ":" + m.Groups["instance"].Value);
                this.Messages += SkinController.FormatMessage(TOKEN_PROC, "[" + AttributeNode + "]", 2, false);
                if (this.ControlList.ContainsKey(Token) == true || Token.IndexOf("CONTENTPANE") != -1)
                {
                    string SkinControl = "";
                    if (this.ControlList.ContainsKey(Token))
                    {
                        this.Messages += SkinController.FormatMessage(TOKEN_SKIN, (string)this.ControlList[Token], 2, false);
                    }
                    else
                    {
                        this.Messages += SkinController.FormatMessage(TOKEN_PANE, Token, 2, false);
                    }
                    if (this.Attributes.DocumentElement != null)
                    {
                        XmlNode xmlSkinAttributeRoot = this.Attributes.DocumentElement.SelectSingleNode("descendant::Object[Token='[" + AttributeNode + "]']");
                        if (xmlSkinAttributeRoot != null)
                        {
                            this.Messages += SkinController.FormatMessage(TOKEN_FOUND, "[" + AttributeNode + "]", 2, false);
                            foreach (XmlNode xmlSkinAttribute in xmlSkinAttributeRoot.SelectNodes(".//Settings/Setting"))
                            {
                                if (!String.IsNullOrEmpty(xmlSkinAttribute.SelectSingleNode("Value").InnerText))
                                {
                                    this.Messages += SkinController.FormatMessage(TOKEN_FORMAT, xmlSkinAttribute.SelectSingleNode("Name").InnerText + "=\"" + xmlSkinAttribute.SelectSingleNode("Value").InnerText + "\"", 2, false);
                                    SkinControl += " " + xmlSkinAttribute.SelectSingleNode("Name").InnerText + "=\"" + xmlSkinAttribute.SelectSingleNode("Value").InnerText.Replace("\"", "&quot;") + "\"";
                                }
                            }
                        }
                        else
                        {
                            this.Messages += SkinController.FormatMessage(TOKEN_NOTFOUND_INFILE, "[" + AttributeNode + "]", 2, false);
                        }
                    }
                    if (this.ControlList.ContainsKey(Token))
                    {
                        SkinControl = "dnn:" + Token + " runat=\"server\" id=\"dnn" + ControlName + "\"" + SkinControl;
                        string ControlRegistration = "<%@ Register TagPrefix=\"dnn\" TagName=\"" + Token + "\" Src=\"~/" + (string)this.ControlList[Token] + "\" %>" + Environment.NewLine;
                        if (RegisterList.Contains(ControlRegistration) == false)
                        {
                            RegisterList.Add(ControlRegistration);
                        }
                        this.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" + SkinControl + " /&gt;", 2, false);
                        SkinControl = "<" + SkinControl + " />";
                    }
                    else
                    {
                        if (SkinControl.ToLower().IndexOf("id=") == -1)
                        {
                            SkinControl = " id=\"ContentPane\"";
                        }
                        SkinControl = "div runat=\"server\"" + SkinControl + "></div";
                        this.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" + SkinControl + "&gt;", 2, false);
                        SkinControl = "<" + SkinControl + ">";
                    }
                    return SkinControl;
                }
                else
                {
                    this.Messages += SkinController.FormatMessage(TOKEN_NOTFOUND, "[" + m.Groups["token"].Value + "]", 2, false);
                    return "[" + m.Groups["token"].Value + "]";
                }
            }
        }
        private class ObjectParser
        {
            private readonly Hashtable m_ControlList = new Hashtable();
            private readonly string m_InitMessages = "";
            private ArrayList m_RegisterList = new ArrayList();
            private string m_ParseMessages = "";
            internal ArrayList Registrations
            {
                get { return m_RegisterList; }
            }
            private MatchEvaluator Handler
            {
                get { return ObjectMatchHandler; }
            }
            private ArrayList RegisterList
            {
                get { return m_RegisterList; }
                set { m_RegisterList = value; }
            }
            private Hashtable ControlList
            {
                get { return m_ControlList; }
            }
            private string Messages
            {
                get { return m_ParseMessages; }
                set { m_ParseMessages = value; }
            }
            public ObjectParser(Hashtable ControlList)
            {
                m_ControlList = (Hashtable)ControlList.Clone();
            }
            public string Parse(ref string Source)
            {
                this.Messages = m_InitMessages;
                this.RegisterList.Clear();
                Regex FindObjectInstance = new Regex("\\<object(?<token>.*?)</object>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                Source = FindObjectInstance.Replace(Source, this.Handler);
                return Messages;
            }
            private string ObjectMatchHandler(Match m)
            {
                string OBJECT_PROC = Util.GetLocalizedString("ProcessObject");
                string OBJECT_SKIN = Util.GetLocalizedString("SkinObject");
                string OBJECT_PANE = Util.GetLocalizedString("PaneObject");
                string OBJECT_FOUND = Util.GetLocalizedString("ObjectFound");
                string CONTROL_FORMAT = Util.GetLocalizedString("ControlFormat");
                string OBJECT_NOTFOUND = Util.GetLocalizedString("ObjectNotFound");
                string EmbeddedObjectAttributes = m.Groups["token"].Value.Substring(0, m.Groups["token"].Value.IndexOf(">"));
                string[] Attributes = EmbeddedObjectAttributes.Split(' ');
                string AttributeNode = "";
                string Token = "";
                string ControlName = "";
                string[] Attribute;
                string AttributeName;
                string AttributeValue;
                foreach (string strAttribute in Attributes)
                {
                    if (strAttribute != string.Empty)
                    {
                        Attribute = strAttribute.Split('=');
                        AttributeName = Attribute[0].Trim();
                        AttributeValue = Attribute[1].Trim().Replace("\"", "");
                        switch (AttributeName.ToLower())
                        {
                            case "id":
                                ControlName = AttributeValue;
                                break;
                            case "codetype":
                                AttributeNode = AttributeValue;
                                break;
                            case "codebase":
                                Token = AttributeValue.ToUpper();
                                break;
                        }
                    }
                }
                if (AttributeNode.ToLower() == "dotnetnuke/server")
                {
                    this.Messages += SkinController.FormatMessage(OBJECT_PROC, Token, 2, false);
                    if (this.ControlList.ContainsKey(Token) == true || Token == "CONTENTPANE")
                    {
                        string SkinControl = "";
                        if (this.ControlList.ContainsKey(Token))
                        {
                            this.Messages += SkinController.FormatMessage(OBJECT_SKIN, (string)this.ControlList[Token], 2, false);
                        }
                        else
                        {
                            this.Messages += SkinController.FormatMessage(OBJECT_PANE, Token, 2, false);
                        }
                        string Parameters = m.Groups["token"].Value.Substring(m.Groups["token"].Value.IndexOf(">") + 1);
                        Parameters = Parameters.Replace("<param name=\"", "");
                        Parameters = Parameters.Replace("\" value", "");
                        Parameters = Parameters.Replace("/>", "");
                        Parameters = Regex.Replace(Parameters, "\\s+", " ");
                        if (this.ControlList.ContainsKey(Token))
                        {
                            SkinControl = "dnn:" + Token + " runat=\"server\" ";
                            if (!String.IsNullOrEmpty(ControlName))
                            {
                                SkinControl += "id=\"" + ControlName + "\" ";
                            }
                            SkinControl += Parameters;
                            string ControlRegistration = "<%@ Register TagPrefix=\"dnn\" TagName=\"" + Token + "\" Src=\"~/" + (string)this.ControlList[Token] + "\" %>" + Environment.NewLine;
                            if (RegisterList.Contains(ControlRegistration) == false)
                            {
                                RegisterList.Add(ControlRegistration);
                            }
                            this.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" + SkinControl + " /&gt;", 2, false);
                            SkinControl = "<" + SkinControl + "/>";
                        }
                        else
                        {
                            SkinControl = "div runat=\"server\" ";
                            if (!String.IsNullOrEmpty(ControlName))
                            {
                                SkinControl += "id=\"" + ControlName + "\" ";
                            }
                            else
                            {
                                SkinControl += "id=\"ContentPane\" ";
                            }
                            SkinControl += Parameters + "></div";
                            this.Messages += SkinController.FormatMessage(CONTROL_FORMAT, "&lt;" + SkinControl + "&gt;", 2, false);
                            SkinControl = "<" + SkinControl + ">";
                        }
                        return SkinControl;
                    }
                    else
                    {
                        this.Messages += SkinController.FormatMessage(OBJECT_NOTFOUND, Token, 2, false);
                        return "<object" + m.Groups["token"].Value + "</object>";
                    }
                }
                else
                {
                    return "<object" + m.Groups["token"].Value + "</object>";
                }
            }
        }
    }
}
