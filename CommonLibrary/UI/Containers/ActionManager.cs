using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using System.Web;
using CommonLibrary.UI.Modules;
using CommonLibrary.Entities.Modules.Actions;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Entities.Users;
using CommonLibrary.Common.Utilities;
using System.Web.UI.WebControls;
using CommonLibrary.Security;

namespace CommonLibrary.UI.Containers
{
    public class ActionManager
    {
        private IActionControl _ActionControl;
        private PortalSettings PortalSettings = PortalController.GetCurrentPortalSettings();
        private HttpRequest Request = HttpContext.Current.Request;
        private HttpResponse Response = HttpContext.Current.Response;
        public ActionManager(IActionControl actionControl)
        {
            _ActionControl = actionControl;
        }
        public IActionControl ActionControl
        {
            get { return _ActionControl; }
            set { _ActionControl = value; }
        }
        protected ModuleInstanceContext ModuleContext
        {
            get { return ActionControl.ModuleControl.ModuleContext; }
        }
        private void ClearCache(ModuleAction Command)
        {
            ModuleController.SynchronizeModule(ModuleContext.ModuleId);
            Response.Redirect(Request.RawUrl, true);
        }
        private void Delete(ModuleAction Command)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule(int.Parse(Command.CommandArgument), ModuleContext.TabId, true);
            if (objModule != null)
            {
                objModules.DeleteTabModule(ModuleContext.TabId, int.Parse(Command.CommandArgument), true);
                UserInfo m_UserInfo = UserController.GetCurrentUserInfo();
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(objModule, PortalSettings, m_UserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_SENT_TO_RECYCLE_BIN);
            }
            Response.Redirect(Request.RawUrl, true);
        }
        private void DoAction(ModuleAction Command)
        {
            if (Command.NewWindow)
            {
                UrlUtils.OpenNewWindow(ActionControl.ModuleControl.Control.Page, this.GetType(), Command.Url);
            }
            else
            {
                Response.Redirect(Command.Url, true);
            }
        }
        private void MoveToPane(ModuleAction Command)
        {
            ModuleController objModules = new ModuleController();
            objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, -1, Command.CommandArgument);
            objModules.UpdateTabModuleOrder(ModuleContext.TabId);
            Response.Redirect(Request.RawUrl, true);
        }
        private void MoveUpDown(ModuleAction Command)
        {
            ModuleController objModules = new ModuleController();
            switch (Command.CommandName)
            {
                case ModuleActionType.MoveTop:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, 0, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveUp:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, ModuleContext.Configuration.ModuleOrder - 3, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveDown:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, ModuleContext.Configuration.ModuleOrder + 3, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveBottom:
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, (ModuleContext.Configuration.PaneModuleCount * 2) + 1, Command.CommandArgument);
                    break;
            }
            objModules.UpdateTabModuleOrder(ModuleContext.TabId);
            Response.Redirect(Request.RawUrl, true);
        }
        //public bool DisplayControl(NodeCollection objNodes)
        //{
        //    if (objNodes != null && objNodes.Count > 0 && PortalSettings.UserMode != PortalSettings.Mode.View)
        //    {
        //        Node objRootNode = objNodes[0];
        //        if (objRootNode.HasNodes && objRootNode.DNNNodes.Count == 0)
        //        {
        //            return true;
        //        }
        //        else if (objRootNode.DNNNodes.Count > 0)
        //        {
        //            foreach (DNNNode childNode in objRootNode.DNNNodes)
        //            {
        //                if (!childNode.IsBreak)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}
        public ModuleAction GetAction(string commandName)
        {
            return ActionControl.ModuleControl.ModuleContext.Actions.GetActionByCommandName(commandName);
        }
        public ModuleAction GetAction(int id)
        {
            return ActionControl.ModuleControl.ModuleContext.Actions.GetActionByID(id);
        }
        public void GetClientScriptURL(ModuleAction action, WebControl control)
        {
            if (!String.IsNullOrEmpty(action.ClientScript))
            {
                string Script = action.ClientScript;
                int JSPos = Script.ToLower().IndexOf("javascript:");
                if (JSPos > -1)
                {
                    Script = Script.Substring(JSPos + 11);
                }
                string FormatScript = "javascript: return {0};";
                control.Attributes.Add("onClick", string.Format(FormatScript, Script));
            }
        }
        public bool IsVisible(ModuleAction action)
        {
            bool _IsVisible = false;
            if (action.Visible == true && ModulePermissionController.HasModuleAccess(action.Secure, Null.NullString, ModuleContext.Configuration) == true)
            {
                if ((ModuleContext.PortalSettings.UserMode == PortalSettings.Mode.Edit) || (action.Secure == SecurityAccessLevel.Anonymous || action.Secure == SecurityAccessLevel.View))
                {
                    _IsVisible = true;
                }
                else
                {
                    _IsVisible = false;
                }
            }
            else
            {
                _IsVisible = false;
            }
            return _IsVisible;
        }
        public bool ProcessAction(string id)
        {
            bool bProcessed = true;
            int nid = 0;
            if (Int32.TryParse(id, out nid))
            {
                bProcessed = ProcessAction(ActionControl.ModuleControl.ModuleContext.Actions.GetActionByID(nid));
            }
            return bProcessed;
        }
        public bool ProcessAction(ModuleAction action)
        {
            bool bProcessed = true;
            switch (action.CommandName)
            {
                case ModuleActionType.ModuleHelp:
                    DoAction(action);
                    break;
                case ModuleActionType.OnlineHelp:
                    DoAction(action);
                    break;
                case ModuleActionType.ModuleSettings:
                    DoAction(action);
                    break;
                case ModuleActionType.DeleteModule:
                    Delete(action);
                    break;
                case ModuleActionType.PrintModule:
                case ModuleActionType.SyndicateModule:
                    DoAction(action);
                    break;
                case ModuleActionType.ClearCache:
                    ClearCache(action);
                    break;
                case ModuleActionType.MovePane:
                    MoveToPane(action);
                    break;
                case ModuleActionType.MoveTop:
                case ModuleActionType.MoveUp:
                case ModuleActionType.MoveDown:
                case ModuleActionType.MoveBottom:
                    MoveUpDown(action);
                    break;
                default:
                    if (!String.IsNullOrEmpty(action.Url) && action.UseActionEvent == false)
                    {
                        DoAction(action);
                    }
                    else
                    {
                        bProcessed = false;
                    }
                    break;
            }
            return bProcessed;
        }
    }
}
