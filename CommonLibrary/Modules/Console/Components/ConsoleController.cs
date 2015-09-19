using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Exceptions;
using System.Collections;
using CommonLibrary.Common;
using CommonLibrary.Entities.Modules;
using System.Xml;

namespace CommonLibrary.Modules.Console.Components
{
    public class ConsoleController : IPortable
    {
        public static IList<string> GetSizeValues()
        {
            IList<string> returnValue = new List<string>();
            returnValue.Add("IconFile");
            returnValue.Add("IconFileLarge");
            return returnValue;
        }
        public static IList<string> GetViewValues()
        {
            IList<string> returnValue = new List<string>();
            returnValue.Add("Hide");
            returnValue.Add("Show");
            return returnValue;
        }
        string[] _SettingKeys = new string[] {
			"ParentTabID",
			"DefaultSize",
			"AllowSizeChange",
			"DefaultView",
			"AllowViewChange",
			"ShowTooltip",
			"ConsoleWidth"
		};
        public string ExportModule(int moduleID)
        {
            ModuleController moduleCtrl = new ModuleController();
            StringBuilder xmlStr = new StringBuilder();
            xmlStr.Append("<ConsoleSettings>");
            Hashtable settings = moduleCtrl.GetModuleSettings(moduleID);
            if ((settings != null))
            {
                foreach (string key in _SettingKeys)
                {
                    AddToXmlStr(xmlStr, settings, key);
                }
            }
            xmlStr.Append("</ConsoleSettings>");
            return xmlStr.ToString();
        }
        public void ImportModule(int ModuleID, string Content, string Version, int UserId)
        {
            XmlNode xmlSettings = Globals.GetContent(Content, "ConsoleSettings");
            ModuleController moduleCtrl = new ModuleController();
            foreach (string key in _SettingKeys)
            {
                XmlNode node = xmlSettings.SelectSingleNode(key);
                bool doUpdate = true;
                string value = string.Empty;
                try
                {
                    if ((node == null))
                    {
                        doUpdate = false;
                    }
                    else
                    {
                        value = node.InnerText;
                        switch (key)
                        {
                            case "ParentTabID":
                                int parentTabID = int.Parse(value);
                                TabInfo tabInfo = new TabController().GetTab(parentTabID, PortalController.GetCurrentPortalSettings().PortalId, false);
                                break;
                            case "DefaultSize":
                                doUpdate = GetSizeValues().Contains(value);
                                break;
                            case "DefaultView":
                                doUpdate = GetViewValues().Contains(value);
                                break;
                            case "AllowSizeChange":
                            case "AllowViewChange":
                            case "ShowTooltip":
                                bool.Parse(value);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    Exceptions.LogException(new Exception("Unable to import value [" + key + "] for Console Module moduleid [" + ModuleID.ToString() + "]"));
                    doUpdate = false;
                }
                if ((doUpdate))
                {
                    moduleCtrl.UpdateModuleSetting(ModuleID, key, value);
                }
            }
        }
        private void AddToXmlStr(StringBuilder xmlStr, Hashtable settings, string key)
        {
            if ((settings.ContainsKey(key)))
            {
                xmlStr.AppendFormat("<{0}>{1}</{0}>", key, settings[key].ToString());
            }
        }
    }
}
