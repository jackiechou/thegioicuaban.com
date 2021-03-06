﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.UI;
using System.Web;
using CommonLibrary.Entities.Portal;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CommonLibrary.Common;
using CommonLibrary.Services.Localization;
using CommonLibrary.Services.Personalization;
using System.Xml;
using System.Configuration;
using CommonLibrary.Common.Utilities;
using System.Text.RegularExpressions;
using System.IO;
using CommonLibrary.Services.Cache;

namespace CommonLibrary.UI.Utilities
{
    public class ClientAPI
    {
        public const string DNNVARIABLE_CONTROLID = "__dnnVariable";
        /// <Summary>
        /// Private variable holding location of client side js files. Shared by entire application.
        /// </Summary>
        private static string m_sScriptPath;
        public const string SCRIPT_CALLBACKID = "__DNNCAPISCI";
        public const string SCRIPT_CALLBACKPAGEID = "__DNNCAPISCPAGEID";
        public const string SCRIPT_CALLBACKPARAM = "__DNNCAPISCP";
        public const string SCRIPT_CALLBACKSTATUSDESCID = "__DNNCAPISCSDI";
        public const string SCRIPT_CALLBACKSTATUSID = "__DNNCAPISCSI";

        public enum MinMaxPersistanceType
        {
            None,
            Page,
            Cookie,
            Personalization
        }
        public enum PageCallBackType
        {
            GetPersonalization = 0,
            SetPersonalization = 1
        }

        public enum ClientFunctionality
        {
            DHTML = 1,
            XML = 2,
            XSLT = 4,
            Positioning = 8,
            XMLJS = 16,
            XMLHTTP = 32,
            XMLHTTPJS = 64,
            SingleCharDelimiters = 128,
        }

        public enum ClientNamespaceReferences
        {
            dnn = 0,
            dnn_dom = 1,
            dnn_dom_positioning = 2,
            dnn_xml = 3,
            dnn_xmlhttp = 4,
        }

        private static HtmlInputHidden get_ClientVariableControl(Page objPage)
        {
            return RegisterDNNVariableControl(objPage);
        }
        /// <Summary>Character used for delimiting name from value</Summary>
        public static string COLUMN_DELIMITER
        {
            get
            {
                if (!BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return "~|~";
                }
                else
                {
                    return "";
                }
            }
        }
        /// <Summary>Character used for delimiting name from value</Summary>
        public static string CUSTOM_COLUMN_DELIMITER
        {
            get
            {
                if (!BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return "~.~";
                }
                else
                {
                    return "";
                }
            }
        }
        /// <Summary>Character used for delimiting name/value pairs</Summary>
        public static string CUSTOM_ROW_DELIMITER
        {
            get
            {
                if (!BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return "~,~";
                }
                else
                {
                    return "";
                }
            }
        }
        /// <Summary>
        /// In order to reduce payload, substitute out " with different char, since when put in a hidden control it uses "
        /// </Summary>
        public static string QUOTE_REPLACEMENT
        {
            get
            {
                if (!BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return "~!~";
                }
                else
                {
                    return "";
                }
            }
        }
        /// <Summary>Character used for delimiting name/value pairs</Summary>
        public static string ROW_DELIMITER
        {
            get
            {
                if (!BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                {
                    return "~`~";
                }
                else
                {
                    return "";
                }
            }
        }
        /// <Summary>Path where js files are placed</Summary>
        public static string ScriptPath
        {
            get
            {
                if (!String.IsNullOrEmpty(m_sScriptPath))
                {
                    return m_sScriptPath;
                }
                if (HttpContext.Current == null)
                {
                    return String.Empty;
                }
                if (HttpContext.Current.Request.ApplicationPath.EndsWith("/"))
                {
                    return (HttpContext.Current.Request.ApplicationPath + "js/");
                }
                else
                {
                    return (HttpContext.Current.Request.ApplicationPath + "/js/");
                }
            }
            set
            {
                m_sScriptPath = value;
            }
        }
      
        /// <Summary>Common way to handle confirmation prompts on client</Summary>
        /// <Param name="objButton">Button to trap click event</Param>
        /// <Param name="strText">Text to display in confirmation</Param>
        public static void AddButtonConfirm(WebControl objButton, string strText)
        {
            objButton.Attributes.Add("onClick", ("javascript:return confirm('" + GetSafeJSString(strText) + "');"));
        }


        /// <Summary>
        /// A Safe way of accessing XML Attributes - clean way to handle nodes and attributes
        /// that are set to nothing. When not found a "nice" default value is returned.
        /// </Summary>
        private static string GetSafeXMLAttr(XmlNode objNode, string strAttr, string strDef)
        {
            if (objNode == null)
            {
                return strDef;
            }
            XmlNode xmlNode1 = objNode.Attributes.GetNamedItem(strAttr);
            if (xmlNode1 == null)
            {
                return strDef;
            }
            else
            {
                return xmlNode1.Value;
            }
        }

      
     
        public static bool BrowserSupportsFunctionality(ClientFunctionality eFunctionality)
        {
            if (HttpContext.Current == null)
            {
                return true;
            }
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (ClientAPIDisabled())
            {
                return false;
            }
            XmlDocument xmlDocument = GetClientAPICapsDOM();
            string browser = httpRequest.Browser.Browser;
            double dblVersion = (httpRequest.Browser.MajorVersion + httpRequest.Browser.MinorVersion);
            string s = Enum.GetName(eFunctionality.GetType(), eFunctionality);
            bool supportsFunctionality = CapsMatchFound(xmlDocument.SelectSingleNode(("/capabilities/functionality[@nm='" + s + "']/supports")), httpRequest.UserAgent, browser, dblVersion);
            if (!CapsMatchFound(xmlDocument.SelectSingleNode(("/capabilities/functionality[@nm='" + s + "']/excludes")), httpRequest.UserAgent, browser, dblVersion))
            {
                return supportsFunctionality;
            }
            else
            {
                return false;
            }
        }

        /// <Summary>
        /// Looks for a browser match (name supplied by .NET Framework) and processes
        /// matches where UserAgent contains specified text
        /// </Summary>
        private static bool CapsMatchFound(XmlNode objNode, string strAgent, string strBrowser, double dblVersion)
        {
            if (objNode == null)
            {
                return false;
            }
            if (strAgent.Length <= 0)
            {
                return false;
            }
            if (Convert.ToDouble(Convert.ToDecimal(GetSafeXMLAttr(objNode.SelectSingleNode(("browser[@nm='" + strBrowser + "']")), "minversion", "999"))) <= dblVersion)
            {
                return true;
            }
            if (objNode.SelectSingleNode("browser[@nm='*']") != null)
            {
                return true;
            }
            foreach (XmlNode xmlNode in objNode.SelectNodes("browser[@contains]"))
            {
                if (strAgent.ToLower().IndexOf(GetSafeXMLAttr(xmlNode, "contains", "\0").ToLower()) > -1)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ClientAPIDisabled()
        {
            return (String.Compare(ConfigurationManager.AppSettings["ClientAPI"], "0", false) == 0);
        }
        /// <Summary>
        /// Registers a button inside a table for the ability to perform client-side reordering
        /// </Summary>
        /// <Param name="objButton">
        /// Button responsible for moving the row up or down.
        /// </Param>
        /// <Param name="objPage">
        /// Page the table belongs to. Can't just use objButton.Page because inside ItemCreated event of grid the button has no page yet.
        /// </Param>
        /// <Param name="blnUp">
        /// Determines if the button is responsible for moving the row up or down
        /// </Param>
        /// <Param name="strKey">
        /// Unique key for the table/grid to be used to obtain the new order on postback. Needed when calling GetClientSideReOrder
        /// </Param>
        public static void EnableClientSideReorder(Control objButton, Page objPage, bool blnUp, string strKey)
        {
            Control parent;
            if (!BrowserSupportsFunctionality(ClientFunctionality.DHTML))
            {
                return;
            }
            RegisterClientReference(objPage, ClientNamespaceReferences.dnn_dom);
            if (!IsClientScriptBlockRegistered(objPage, "dnn.util.tablereorder.js"))
            {
                RegisterClientScriptBlock(objPage, "dnn.util.tablereorder.js", ("<script src=\"" + ScriptPath + "dnn.util.tablereorder.js\"></script>"));
            }
            string[] args = new string[] { "if (dnn.util.tableReorderMove(this,", Convert.ToString((-(blnUp ? 1 : 0))), ",'", strKey, "')) return false;" };
            AddAttribute(objButton, "onclick", string.Concat(args));
            for (parent = objButton.Parent; (parent != null); parent = parent.Parent)
            {
                if (parent is TableRow)
                {
                    AddAttribute(parent, "origidx", "-1");
                }
            }
        }
        /// <Summary>Loop up parent controls to find form</Summary>
        private static Control FindForm(Control oCtl)
        {
            while (!(oCtl is HtmlForm))
            {
                if (oCtl == null)
                {
                    return null;
                }
                if (oCtl is Page)
                {
                    return null;
                }
                oCtl = oCtl.Parent;
            }
            return oCtl;
        }
        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack, Control objPostChildrenOf)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, objPostChildrenOf.ClientID);
        }
        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack, string strPostChildrenOfId)
        {
            if (strArgument == null)
            {
                strArgument = "null";
            }
            if (strContext == null)
            {
                strContext = "null";
            }
            if (srtClientErrorCallBack == null)
            {
                srtClientErrorCallBack = "null";
            }
            if (strClientStatusCallBack == null)
            {
                strClientStatusCallBack = "null";
            }
            if (strPostChildrenOfId.Length == 0)
            {
                strPostChildrenOfId = "null";
            }
            else if (!strPostChildrenOfId.StartsWith("'"))
            {
                strPostChildrenOfId = ("'" + strPostChildrenOfId + "'");
            }
            string string2 = objControl.ID;
            if (!BrowserSupportsFunctionality(ClientFunctionality.XMLHTTP))
            {
                return "";
            }
            if (!BrowserSupportsFunctionality(ClientFunctionality.XML))
            {
                return "";
            }
            RegisterClientReference(objControl.Page, ClientNamespaceReferences.dnn_xml);
            RegisterClientReference(objControl.Page, ClientNamespaceReferences.dnn_xmlhttp);
            if ((objControl is Page) && (string2.Length == 0))
            {
                string2 = "__DNNCAPISCPAGEID";
            }
            if (!(objControl is Page))
            {
                string2 = (string2 + " " + objControl.ClientID);
            }
            object[] objectArray1 = new object[] { string2, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, "null", strPostChildrenOfId };
            return string.Format("dnn.xmlhttp.doCallBack('{0}',{1},{2},{3},{4},{5},{6},{7});", objectArray1);
        }
        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, Control objPostChildrenOf)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, null, objPostChildrenOf);
        }
        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, null, "");
        }
        public static string GetCallbackEventReference(Control objControl, string strArgument, string strClientCallBack, string strContext, string srtClientErrorCallBack, string strClientStatusCallBack)
        {
            return GetCallbackEventReference(objControl, strArgument, strClientCallBack, strContext, srtClientErrorCallBack, strClientStatusCallBack, "");
        }
        /// <Summary>
        /// Retrieves XML Document representing the featuresets that each browser handles
        /// </Summary>
        private static XmlDocument GetClientAPICapsDOM()
        {
            string mapPath = "";
            XmlDocument clientAPICapsDOM = ((XmlDocument)DataCache.GetCache("ClientAPICaps"));
            if (clientAPICapsDOM != null)
            {
                return clientAPICapsDOM;
            }
            try
            {
                clientAPICapsDOM = new XmlDocument();
                mapPath = HttpContext.Current.Server.MapPath((ScriptPath + "/ClientAPICaps.config"));
            }
            catch (Exception)
            {
            }
            if (mapPath == null)
            {
                return clientAPICapsDOM;
            }
            if (!File.Exists(mapPath))
            {
                return clientAPICapsDOM;
            }
            if (clientAPICapsDOM != null) clientAPICapsDOM.Load(mapPath);
            DataCache.SetCache("ClientAPICaps", clientAPICapsDOM, new CacheDependency(mapPath));
            return clientAPICapsDOM;
        }
        /// <Summary>Retrieves an array of the new order for the rows</Summary>
        /// <Param name="strKey">
        /// Unique key for the table/grid to be used to obtain the new order on postback. Needed when calling GetClientSideReOrder
        /// </Param>
        /// <Param name="objPage">
        /// Page the table belongs to. Can't just use objButton.Page because inside ItemCreated event of grid the button has no page yet.
        /// </Param>
        public static string[] GetClientSideReorder(string strKey, Page objPage)
        {
            if (GetClientVariable(objPage, strKey).Length <= 0)
            {
                return new string[0];
            }
            char[] separator = new char[] { ',' };
            return GetClientVariable(objPage, strKey).Split(separator);
        }
        /// <Summary>Retrieves DNN Client Variable value</Summary>
        /// <Param name="objPage">Current page rendering content</Param>
        /// <Param name="strVar">Variable name to retrieve value for</Param>
        /// <Returns>Value of variable</Returns>
        public static string GetClientVariable(Page objPage, string strVar)
        {
            string s = GetClientVariableNameValuePair(objPage, strVar);
            if (s.IndexOf(COLUMN_DELIMITER) <= -1)
            {
                return "";
            }
            else
            {
                string[] splitter = { COLUMN_DELIMITER };
                return s.Split(splitter, StringSplitOptions.None)[0];
            }
        }
        /// <Summary>Retrieves DNN Client Variable value</Summary>
        /// <Param name="objPage">Current page rendering content</Param>
        /// <Param name="strVar">Variable name to retrieve value for</Param>
        /// <Param name="strDefaultValue">Default value if variable not found</Param>
        /// <Returns>Value of variable</Returns>
        public static string GetClientVariable(Page objPage, string strVar, string strDefaultValue)
        {
            string s = GetClientVariableNameValuePair(objPage, strVar);
            if (s.IndexOf(COLUMN_DELIMITER) <= -1)
            {
                return strDefaultValue;
            }
            else
            {
                string[] splitter = { COLUMN_DELIMITER };
                return s.Split(splitter, StringSplitOptions.None)[0];
            }
        }
        /// <Summary>
        /// Parses DNN Variable control contents and returns out the delimited name/value pair
        /// </Summary>
        /// <Param name="objPage">Current page rendering content</Param>
        /// <Param name="strVar">Name to retrieve</Param>
        /// <Returns>Delimited name/value pair string</Returns>
        private static string GetClientVariableNameValuePair(Page objPage, string strVar)
        {
            HtmlInputHidden htmlInputHidden = get_ClientVariableControl(objPage);
            string replace = "";
            if (htmlInputHidden != null)
            {
                replace = htmlInputHidden.Value;
            }
            if (replace.Length == 0)
            {
                replace = HttpContext.Current.Request["__dnnVariable"];
            }
            if (replace == null || replace.Length <= 0)
            {
                return "";
            }
            replace = replace.Replace(QUOTE_REPLACEMENT, "\"");
            int ii = replace.IndexOf((ROW_DELIMITER + strVar + COLUMN_DELIMITER));
            if (ii <= -1)
            {
                return "";
            }
            ii += COLUMN_DELIMITER.Length;
            int i2 = replace.IndexOf(ROW_DELIMITER, ii);
            if (i2 <= -1)
            {
                return replace.Substring(ii);
            }
            else
            {
                return replace.Substring(ii, i2 - ii);
            }
        }
        /// <Summary>Returns __dnnVariable control if present</Summary>
        private static HtmlInputHidden GetDNNVariableControl(Control objParent)
        {
            return ((HtmlInputHidden)Globals.FindControlRecursive(objParent.Page, "__dnnVariable"));
        }
        /// <Summary>Returns javascript to call dnncore.js key handler logic</Summary>
        /// <Param name="intKeyAscii">ASCII value to trap</Param>
        /// <Param name="strJavascript">Javascript to execute</Param>
        /// <Returns>Javascript to handle key press</Returns>
        private static string GetKeyDownHandler(int intKeyAscii, string strJavascript)
        {
            string[] stringArray1 = new string[] { "return __dnn_KeyDown('", Convert.ToString(intKeyAscii), "', '", strJavascript.Replace("'", "%27"), "', event);" };
            return string.Concat(stringArray1);
        }
        public static string GetPostBackClientEvent(Page objPage, Control objControl, string arg)
        {
            return objPage.ClientScript.GetPostBackEventReference(objControl, arg);
        }
        public static string GetPostBackClientHyperlink(Control objControl, string strArgument)
        {
            return ("javascript:" + GetPostBackEventReference(objControl, strArgument));
        }
        public static string GetPostBackEventReference(Control objControl, string strArgument)
        {
            return objControl.Page.ClientScript.GetPostBackEventReference(objControl, strArgument);
        }
        /// <Summary>
        /// Escapes string to be safely used in client side javascript.
        /// </Summary>
        /// <Param name="strString">String to escape</Param>
        /// <Returns>Escaped string</Returns>
        public static string GetSafeJSString(string strString)
        {
            if (strString != null && strString.Length > 0)
            {
                //Return System.Text.RegularExpressions.Regex.Replace(strString, "(['""\\])", "\$1")
                return System.Text.RegularExpressions.Regex.Replace(strString, "(['\"\\\\])", "\\$1");
            }
            else
            {
                return strString;
            }
        }
     
        //public static void HandleClientAPICallbackEvent(Page objPage)
        //{
        //    if (objPage.Request["__DNNCAPISCI"] == null)
        //    {
        //        return;
        //    }
        //    if (objPage.Request["__DNNCAPISCI"].Length <= 0)
        //    {
        //        return;
        //    }
        //    char[] separator = new char[] { ' ' };
        //    string[] strings = objPage.Request["__DNNCAPISCI"].Split(separator);
        //    string controlName = strings[0];
        //    string clientID = "";
        //    if (strings.Length > 1)
        //    {
        //        clientID = strings[1];
        //    }
        //    string eventArgument = objPage.Server.UrlDecode(objPage.Request["__DNNCAPISCP"]);
        //    ClientAPICallBackResponse clientAPICallBackResponse1 = new ClientAPICallBackResponse(objPage, ClientAPICallBackResponse.CallBackTypeCode.Simple);
        //    try
        //    {
        //        try
        //        {
        //            objPage.Response.Clear();
        //            Control control;
        //            if (String.Compare(controlName, "__DNNCAPISCPAGEID", false) == 0)
        //            {
        //                control = objPage;
        //            }
        //            else
        //            {
        //                control = Globals.FindControlRecursive(objPage, controlName, clientID);
        //            }
        //            if (control == null)
        //            {
        //                clientAPICallBackResponse1.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.ControlNotFound;
        //                clientAPICallBackResponse1.StatusDesc = "Control Not Found";
        //                return;
        //            }
        //            IClientAPICallbackEventHandler callbackEventHandler;
        //            if (control is HtmlForm)
        //            {
        //                callbackEventHandler = ((IClientAPICallbackEventHandler)objPage);
        //            }
        //            else
        //            {
        //                callbackEventHandler = ((IClientAPICallbackEventHandler)control);
        //            }
        //            if (callbackEventHandler != null)
        //            {
        //                try
        //                {
        //                    clientAPICallBackResponse1.Response = callbackEventHandler.RaiseClientAPICallbackEvent(eventArgument);
        //                    clientAPICallBackResponse1.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.OK;
        //                    return;
        //                }
        //                catch (Exception exception)
        //                {
        //                    clientAPICallBackResponse1.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.GenericFailure;
        //                    clientAPICallBackResponse1.StatusDesc = exception.Message;
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                clientAPICallBackResponse1.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.InterfaceNotSupported;
        //                clientAPICallBackResponse1.StatusDesc = "Interface Not Supported";
        //                return;
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            clientAPICallBackResponse1.StatusCode = ClientAPICallBackResponse.CallBackResponseStatusCode.GenericFailure;
        //            clientAPICallBackResponse1.StatusDesc = exception.Message;
        //            return;
        //        }
        //    }
        //    finally
        //    {
        //        clientAPICallBackResponse1.Write();
        //        objPage.Response.End();
        //    }
        //}
        public static bool IsClientScriptBlockRegistered(Page objPage, string key)
        {
            return objPage.ClientScript.IsClientScriptBlockRegistered(key);
        }
        /// <Summary>
        /// Determines if DNNVariable control is present in page's control collection
        /// </Summary>
        public static bool NeedsDNNVariable(Control objParent)
        {
            return (GetDNNVariableControl(objParent) == null);
        }
        public static void RegisterClientReference(Page objPage, ClientNamespaceReferences eRef)
        {
            switch (eRef)
            {
                case ClientNamespaceReferences.dnn:
                    {
                        if (IsClientScriptBlockRegistered(objPage, "dnn.js"))
                        {
                            return;
                        }
                        RegisterClientScriptBlock(objPage, "dnn.js", ("<script type='text/javascript' src='" + ScriptPath + "dnn.js'></script>"));
                        if (BrowserSupportsFunctionality(ClientFunctionality.SingleCharDelimiters))
                        {
                            return;
                        }
                        RegisterClientVariable(objPage, "__scdoff", "1", true);
                        return;
                    }
                case ClientNamespaceReferences.dnn_dom:
                    {
                        RegisterClientReference(objPage, ClientNamespaceReferences.dnn);
                        return;
                    }
                case ClientNamespaceReferences.dnn_dom_positioning:
                    {
                        RegisterClientReference(objPage, ClientNamespaceReferences.dnn);
                        if (IsClientScriptBlockRegistered(objPage, "dnn.positioning.js"))
                        {
                            return;
                        }
                        RegisterClientScriptBlock(objPage, "dnn.positioning.js", ("<script type='text/javascript' src='" + ScriptPath + "dnn.dom.positioning.js'></script>"));
                        return;
                    }
                case ClientNamespaceReferences.dnn_xml:
                    {
                        RegisterClientReference(objPage, ClientNamespaceReferences.dnn);
                        if (IsClientScriptBlockRegistered(objPage, "dnn.xml.js"))
                        {
                            return;
                        }
                        string string1 = ("<script type='text/javascript' src='" + ScriptPath + "dnn.xml.js'></script>");
                        if (BrowserSupportsFunctionality(ClientFunctionality.XMLJS))
                        {
                            string1 = (string1 + "<script type='text/javascript' src='" + ScriptPath + "dnn.xml.jsparser.js'></script>");
                        }
                        RegisterClientScriptBlock(objPage, "dnn.xml.js", string1);
                        return;
                    }
                case ClientNamespaceReferences.dnn_xmlhttp:
                    {
                        RegisterClientReference(objPage, ClientNamespaceReferences.dnn);
                        if (IsClientScriptBlockRegistered(objPage, "dnn.xmlhttp.js"))
                        {
                            return;
                        }
                        string string2 = ("<script type='text/javascript' src='" + ScriptPath + "dnn.xmlhttp.js'></script>");
                        if (BrowserSupportsFunctionality(ClientFunctionality.XMLHTTPJS))
                        {
                            string2 = (string2 + "<script type='text/javascript' src='" + ScriptPath + "dnn.xmlhttp.jsxmlhttprequest.js'></script>");
                        }
                        RegisterClientScriptBlock(objPage, "dnn.xmlhttp.js", string2);
                        return;
                    }
            }
        }
        public static void RegisterClientScriptBlock(Page objPage, string key, string strScript)
        {
            objPage.ClientScript.RegisterClientScriptBlock(objPage.GetType(), key, strScript);
        }
        /// <Summary>Registers a client side variable (name/value) pair</Summary>
        /// <Param name="objPage">Current page rendering content</Param>
        /// <Param name="strVar">Variable name</Param>
        /// <Param name="strValue">Value</Param>
        /// <Param name="blnOverwrite">
        /// Determins if a replace or append is applied when variable already exists
        /// </Param>
        public static void RegisterClientVariable(Page objPage, string strVar, string strValue, bool blnOverwrite)
        {
            string[] stringArray1;
            HtmlInputHidden htmlInputHidden1 = get_ClientVariableControl(objPage);
            string string1 = GetClientVariableNameValuePair(objPage, strVar);
            if (string1.Length > 0)
            {
                string1 = string1.Replace("\"", QUOTE_REPLACEMENT);
                if (blnOverwrite)
                {
                    htmlInputHidden1.Value = htmlInputHidden1.Value.Replace((ROW_DELIMITER + string1), (ROW_DELIMITER + strVar + COLUMN_DELIMITER + strValue));
                }
                else
                {
                    string string2 = GetClientVariable(objPage, strVar);
                    stringArray1 = new string[] { ROW_DELIMITER, strVar, COLUMN_DELIMITER, string2, strValue };
                    htmlInputHidden1.Value = htmlInputHidden1.Value.Replace((ROW_DELIMITER + string1), string.Concat(stringArray1));
                }
            }
            else
            {
                HtmlInputHidden htmlInputHidden2 = htmlInputHidden1;
                stringArray1 = new string[] { htmlInputHidden2.Value, ROW_DELIMITER, strVar, COLUMN_DELIMITER, strValue };
                htmlInputHidden2.Value = string.Concat(stringArray1);
            }
            htmlInputHidden1.Value = htmlInputHidden1.Value.Replace("\"", QUOTE_REPLACEMENT);
        }
        /// <Summary>
        /// Responsible for inputting the hidden field necessary for the ClientAPI to pass variables back in forth
        /// </Summary>
        public static HtmlInputHidden RegisterDNNVariableControl(Control objParent)
        {
            HtmlInputHidden htmlInputHidden1 = GetDNNVariableControl(objParent);
            if (htmlInputHidden1 != null)
            {
                return htmlInputHidden1;
            }
            Control control1 = FindForm(objParent);
            if (control1 == null)
            {
                return htmlInputHidden1;
            }
            htmlInputHidden1 = new HtmlInputHidden();
            htmlInputHidden1.ID = "__dnnVariable";
            control1.Controls.Add(htmlInputHidden1);
            return htmlInputHidden1;
        }
        /// <Summary>
        /// Traps client side keydown event looking for passed in key press (ASCII) and hooks it up with client-side javascript
        /// </Summary>
        /// <Param name="objControl">Control that should trap the keydown</Param>
        /// <Param name="strJavascript">Javascript to execute when event fires</Param>
        /// <Param name="intKeyAscii">ASCII value of key to trap</Param>
        //public static void RegisterKeyCapture(Control objControl, string strJavascript, int intKeyAscii)
        //{
        //    Globals.SetAttribute(objControl, "onkeydown", GetKeyDownHandler(intKeyAscii, strJavascript));
        //}
        /// <Summary>
        /// Traps client side keydown event looking for passed in key press (ASCII) and hooks it up with server side postback handler
        /// </Summary>
        /// <Param name="objControl">Control that should trap the keydown</Param>
        /// <Param name="objPostbackControl">
        /// Server-side control that has its onclick event handled server-side
        /// </Param>
        /// <Param name="intKeyAscii">ASCII value of key to trap</Param>
        //public static void RegisterKeyCapture(Control objControl, Control objPostbackControl, int intKeyAscii)
        //{
        //    Globals.SetAttribute(objControl, "onkeydown", GetKeyDownHandler(intKeyAscii, GetPostBackClientHyperlink(objPostbackControl, "")));
        //}
        public static void RegisterPostBackEventHandler(Control objParent, string strEventName, ClientAPIPostBackControl.PostBackEvent objDelegate, bool blnMultipleHandlers)
        {
            Control control1 = Globals.FindControlRecursive(objParent.Page, "ClientAPIPostBackCtl");
            if (control1 == null)
            {
                control1 = new ClientAPIPostBackControl(objParent.Page, strEventName, objDelegate);
                control1.ID = "ClientAPIPostBackCtl";
                objParent.Controls.Add(control1);
                RegisterClientVariable(objParent.Page, "__dnn_postBack", GetPostBackClientHyperlink(control1, "[DATA]"), true);
                return;
            }
            if (!blnMultipleHandlers)
            {
                return;
            }
            ((ClientAPIPostBackControl)control1).AddEventHandler(strEventName, objDelegate);
        }
        public static void RegisterStartUpScript(Page objPage, string key, string script)
        {
            objPage.ClientScript.RegisterStartupScript(objPage.GetType(), key, script);
        }

        //private static Hashtable m_objEnabledClientPersonalizationKeys = new Hashtable();
        //public static void AddBodyOnloadEventHandler(Page objPage, string strJSFunction)
        //{
        //    if (strJSFunction.EndsWith(";") == false)
        //        strJSFunction += ";";
        //    ClientAPI.RegisterClientReference(objPage, ClientAPI.ClientNamespaceReferences.dnn);
        //    if (ClientAPI.GetClientVariable(objPage, "__dnn_pageload").IndexOf(strJSFunction) == -1)
        //        ClientAPI.RegisterClientVariable(objPage, "__dnn_pageload", strJSFunction, false);
        //}
        //public static void EnableContainerDragAndDrop(Control objTitle, Control objContainer, int ModuleID)
        //{
        //    if (ClientAPI.ClientAPIDisabled() == false && ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.Positioning))
        //    {
        //        ClientAPI.AddBodyOnloadEventHandler(objTitle.Page, "__dnn_enableDragDrop()");
        //        ClientAPI.RegisterClientReference(objTitle.Page, ClientAPI.ClientNamespaceReferences.dnn_dom_positioning);
        //        ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_dragDrop", objContainer.ClientID + " " + objTitle.ClientID + " " + ModuleID.ToString() + ";", false);
        //        string strPanes = "";
        //        string strPaneNames = "";
        //        PortalSettings objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

        //        Control objCtl;
        //        foreach (string strPane in objPortalSettings.ActiveTab.Panes)
        //        {
        //            objCtl = CommonLibrary.Common.Globals.FindControlRecursive(objContainer.Parent, strPane);
        //            if (objCtl != null)
        //                strPanes += objCtl.ClientID + ";";
        //            strPaneNames += strPane + ";";
        //        }
        //        ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_Panes", strPanes, true);
        //        ClientAPI.RegisterClientVariable(objTitle.Page, "__dnn_PaneNames", strPaneNames, true);
        //    }
        //}
        //public static void EnableMinMax(Control objButton, Control objContent, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType)
        //{
        //    EnableMinMax(objButton, objContent, -1, blnDefaultMin, "", "", ePersistanceType);
        //}
        //public static void EnableMinMax(Control objButton, Control objContent, int intModuleId, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType)
        //{
        //    EnableMinMax(objButton, objContent, intModuleId, blnDefaultMin, "", "", ePersistanceType);
        //}
        //public static void EnableMinMax(Control objButton, Control objContent, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType)
        //{
        //    EnableMinMax(objButton, objContent, -1, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType);
        //}
        //public static void EnableMinMax(Control objButton, Control objContent, int intModuleId, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType)
        //{
        //    EnableMinMax(objButton, objContent, intModuleId, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType, 5);
        //}
        //public static void EnableMinMax(Control objButton, Control objContent, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType, string strPersonalizationNamingCtr, string strPersonalizationKey)
        //{
        //    EnableMinMax(objButton, objContent, -1, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType, 5, strPersonalizationNamingCtr, strPersonalizationKey);
        //}
        //public static void EnableMinMax(Control objButton, Control objContent, int intModuleId, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType, int intAnimationFrames)
        //{
        //    EnableMinMax(objButton, objContent, intModuleId, blnDefaultMin, strMinIconLoc, strMaxIconLoc, ePersistanceType, intAnimationFrames, null, null);
        //}
        //public static void EnableMinMax(Control objButton, Control objContent, int intModuleId, bool blnDefaultMin, string strMinIconLoc, string strMaxIconLoc, MinMaxPersistanceType ePersistanceType, int intAnimationFrames, string strPersonalizationNamingCtr, string strPersonalizationKey)
        //{
        //    if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
        //    {
        //        ClientAPI.RegisterClientReference(objButton.Page, ClientAPI.ClientNamespaceReferences.dnn_dom);
        //        switch (ePersistanceType)
        //        {
        //            case MinMaxPersistanceType.None:
        //                AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" + objContent.ClientID + "')) return false;");
        //                if (!String.IsNullOrEmpty(strMinIconLoc))
        //                {
        //                    AddAttribute(objButton, "max_icon", strMaxIconLoc);
        //                    AddAttribute(objButton, "min_icon", strMinIconLoc);
        //                }
        //                break;
        //            case MinMaxPersistanceType.Page:
        //                AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" + objContent.ClientID + "')) return false;");
        //                if (!String.IsNullOrEmpty(strMinIconLoc))
        //                {
        //                    AddAttribute(objButton, "max_icon", strMaxIconLoc);
        //                    AddAttribute(objButton, "min_icon", strMinIconLoc);
        //                }
        //                break;
        //            case MinMaxPersistanceType.Cookie:
        //                if (intModuleId != -1)
        //                {
        //                    AddAttribute(objButton, "onclick", "if (__dnn_ContainerMaxMin_OnClick(this, '" + objContent.ClientID + "')) return false;");
        //                    ClientAPI.RegisterClientVariable(objButton.Page, "containerid_" + objContent.ClientID, intModuleId.ToString(), true);
        //                    ClientAPI.RegisterClientVariable(objButton.Page, "cookieid_" + objContent.ClientID, "_Module" + intModuleId.ToString() + "_Visible", true);
        //                    ClientAPI.RegisterClientVariable(objButton.Page, "min_icon_" + intModuleId.ToString(), strMinIconLoc, true);
        //                    ClientAPI.RegisterClientVariable(objButton.Page, "max_icon_" + intModuleId.ToString(), strMaxIconLoc, true);
        //                    ClientAPI.RegisterClientVariable(objButton.Page, "max_text", CommonLibrary.Services.Localization.Localization.GetString("Maximize"), true);
        //                    ClientAPI.RegisterClientVariable(objButton.Page, "min_text", CommonLibrary.Services.Localization.Localization.GetString("Minimize"), true);
        //                    if (blnDefaultMin)
        //                    {
        //                        ClientAPI.RegisterClientVariable(objButton.Page, "__dnn_" + intModuleId.ToString() + ":defminimized", "true", true);
        //                    }
        //                }
        //                break;
        //            case MinMaxPersistanceType.Personalization:
        //                AddAttribute(objButton, "userctr", strPersonalizationNamingCtr);
        //                AddAttribute(objButton, "userkey", strPersonalizationKey);
        //                if (EnableClientPersonalization(strPersonalizationNamingCtr, strPersonalizationKey, objButton.Page))
        //                {
        //                    AddAttribute(objButton, "onclick", "if (__dnn_SectionMaxMin(this,  '" + objContent.ClientID + "')) return false;");
        //                    if (!String.IsNullOrEmpty(strMinIconLoc))
        //                    {
        //                        AddAttribute(objButton, "max_icon", strMaxIconLoc);
        //                        AddAttribute(objButton, "min_icon", strMinIconLoc);
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    if (GetMinMaxContentVisible(objButton, intModuleId, blnDefaultMin, ePersistanceType))
        //    {
        //        if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
        //        {
        //            AddStyleAttribute(objContent, "display", "");
        //        }
        //        else
        //        {
        //            objContent.Visible = true;
        //        }
        //        if (!String.IsNullOrEmpty(strMinIconLoc))
        //            SetMinMaxProperties(objButton, strMinIconLoc, Localization.GetString("Minimize"), Localization.GetString("Minimize"));
        //    }
        //    else
        //    {
        //        if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
        //        {
        //            AddStyleAttribute(objContent, "display", "none");
        //        }
        //        else
        //        {
        //            objContent.Visible = false;
        //        }
        //        if (!String.IsNullOrEmpty(strMaxIconLoc))
        //            SetMinMaxProperties(objButton, strMaxIconLoc, CommonLibrary.Services.Localization.Localization.GetString("Maximize"), Localization.GetString("Maximize"));
        //    }
        //    if (intAnimationFrames != 5)
        //    {
        //        ClientAPI.RegisterClientVariable(objButton.Page, "animf_" + objContent.ClientID, intAnimationFrames.ToString(), true);
        //    }
        //}
        private static void SetMinMaxProperties(Control objButton, string strImage, string strToolTip, string strAltText)
        {
            if (objButton is LinkButton)
            {
                LinkButton objLB = (LinkButton)objButton;
                objLB.ToolTip = strToolTip;
                if (objLB.Controls.Count > 0)
                {
                    SetImageProperties(objLB.Controls[0], strImage, strToolTip, strAltText);
                }
            }
            else if (objButton is System.Web.UI.WebControls.Image)
            {
                SetImageProperties((System.Web.UI.WebControls.Image)objButton, strImage, strToolTip, strAltText);
            }
            else if (objButton is System.Web.UI.WebControls.ImageButton)
            {
                SetImageProperties((System.Web.UI.WebControls.LinkButton)objButton, strImage, strToolTip, strAltText);
            }
        }
        private static void SetImageProperties(Control objControl, string strImage, string strToolTip, string strAltText)
        {
            if (objControl is System.Web.UI.WebControls.Image)
            {
                System.Web.UI.WebControls.Image objImage = (System.Web.UI.WebControls.Image)objControl;
                objImage.ImageUrl = strImage;
                objImage.AlternateText = strAltText;
                objImage.ToolTip = strToolTip;
            }
            else if (objControl is System.Web.UI.WebControls.ImageButton)
            {
                System.Web.UI.WebControls.ImageButton objImage = (System.Web.UI.WebControls.ImageButton)objControl;
                objImage.ImageUrl = strImage;
                objImage.AlternateText = strAltText;
                objImage.ToolTip = strToolTip;
            }
            else if (objControl is System.Web.UI.HtmlControls.HtmlImage)
            {
                System.Web.UI.HtmlControls.HtmlImage objImage = (System.Web.UI.HtmlControls.HtmlImage)objControl;
                objImage.Src = strImage;
                objImage.Alt = strAltText;
            }
        }
        static bool _MinMaxContentVisible;
        public static bool MinMaxContentVisible
        {
            get { return _MinMaxContentVisible; }
            set { _MinMaxContentVisible = value; }
        }

        //public static bool GetMinMaxContentVisible(
        //    Control objButton, int intModuleId, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType)
        //{
        //    if (System.Web.HttpContext.Current != null)
        //    {
        //        switch (ePersistanceType)
        //        {
        //            case MinMaxPersistanceType.Page:
        //                string sExpanded = ClientAPI.GetClientVariable(objButton.Page, objButton.ClientID + ":exp");
        //                if (!String.IsNullOrEmpty(sExpanded))
        //                {
        //                    _MinMaxContentVisible = sExpanded == "1" ? true : false;
        //                }
        //                else
        //                {
        //                    _MinMaxContentVisible = !blnDefaultMin;
        //                }
        //                break;
        //            case MinMaxPersistanceType.Cookie:
        //                if (intModuleId != -1)
        //                {
        //                    HttpCookie objModuleVisible = System.Web.HttpContext.Current.Request.Cookies["_Module" + intModuleId.ToString() + "_Visible"];
        //                    if (objModuleVisible != null)
        //                    {
        //                        _MinMaxContentVisible = objModuleVisible.Value != "false";
        //                    }
        //                    else
        //                    {
        //                        _MinMaxContentVisible = !blnDefaultMin;
        //                    }
        //                }
        //                else
        //                {
        //                    _MinMaxContentVisible = true;
        //                }
        //                break;
        //            case MinMaxPersistanceType.None:
        //                return !blnDefaultMin;
        //            case MinMaxPersistanceType.Personalization:
        //                string strVisible = Convert.ToString(Personalization.GetProfile(Globals.GetAttribute(objButton, "userctr"), Globals.GetAttribute(objButton, "userkey")));
        //                if (string.IsNullOrEmpty(strVisible))
        //                {
        //                    _MinMaxContentVisible = blnDefaultMin;
        //                }
        //                else
        //                {
        //                    _MinMaxContentVisible = Convert.ToBoolean(strVisible);
        //                }
        //                break;
        //        }
        //    }
        //    return _MinMaxContentVisible;
        //}

        //public static void SetMinMaxContentVisible(
        //    Control objButton, int intModuleId, bool blnDefaultMin, MinMaxPersistanceType ePersistanceType, bool value)
        //{
        //    if (System.Web.HttpContext.Current != null)
        //    {
        //        switch (ePersistanceType)
        //        {
        //            case MinMaxPersistanceType.Page:
        //                ClientAPI.RegisterClientVariable(objButton.Page, objButton.ClientID + ":exp", Convert.ToInt32(value).ToString(), true);
        //                break;
        //            case MinMaxPersistanceType.Cookie:
        //                HttpCookie objModuleVisible =
        //                    new HttpCookie("_Module" + intModuleId + "_Visible");
        //                if (objModuleVisible != null)
        //                {
        //                    objModuleVisible.Value = value.ToString().ToLower();
        //                    objModuleVisible.Expires = DateTime.MaxValue;
        //                    System.Web.HttpContext.Current.Response.AppendCookie(objModuleVisible);
        //                }
        //                break;
        //            case MinMaxPersistanceType.Personalization:
        //                Personalization.SetProfile(
        //                    Globals.GetAttribute(objButton, "userctr"),
        //                    Globals.GetAttribute(objButton, "userkey"),
        //                    value.ToString());
        //                break;
        //        }
        //    }
        //}

        private static void AddAttribute(Control objControl, string strName, string strValue)
        {
            if (objControl is HtmlControl)
            {
                ((HtmlControl)objControl).Attributes.Add(strName, strValue);
            }
            else if (objControl is WebControl)
            {
                ((WebControl)objControl).Attributes.Add(strName, strValue);
            }
        }
        private static void AddStyleAttribute(Control objControl, string strName, string strValue)
        {
            if (objControl is HtmlControl)
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    ((HtmlControl)objControl).Style.Add(strName, strValue);
                }
                else
                {
                    ((HtmlControl)objControl).Style.Remove(strName);
                }
            }
            else if (objControl is WebControl)
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    ((WebControl)objControl).Style.Add(strName, strValue);
                }
                else
                {
                    ((WebControl)objControl).Style.Remove(strName);
                }
            }
        }
        //public static bool EnableClientPersonalization(string strNamingContainer, string strKey, Page objPage)
        //{
        //    if (ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP))
        //    {
        //        ClientAPI.GetCallbackEventReference(objPage, "", "", "", "");
        //        lock (m_objEnabledClientPersonalizationKeys.SyncRoot)
        //        {
        //            if (IsPersonalizationKeyRegistered(strNamingContainer + ClientAPI.CUSTOM_COLUMN_DELIMITER + strKey) == false)
        //            {
        //                m_objEnabledClientPersonalizationKeys.Add(strNamingContainer + ClientAPI.CUSTOM_COLUMN_DELIMITER + strKey, "");
        //            }
        //        }
        //        return true;
        //    }
        //    return false;
        //}
        //public static bool IsPersonalizationKeyRegistered(string strKey)
        //{
        //    return m_objEnabledClientPersonalizationKeys.ContainsKey(strKey);
        //}
    }
}
