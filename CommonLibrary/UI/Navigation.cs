using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Portal;
using CommonLibrary.UI.Containers;
using CommonLibrary.Entities.Modules.Actions;
using CommonLibrary.Security;
using CommonLibrary.UI.WebControls;
using CommonLibrary.UI.WebControls.Common;
using CommonLibrary.Entities.Modules;
using System.Web.UI;
using CommonLibrary.Security.Permissions;

namespace CommonLibrary.UI
{
    public class Navigation
    {
        public enum ToolTipSource
        {
            TabName,
            Title,
            Description,
            None
        }
        public enum NavNodeOptions
        {
            IncludeSelf = 1,
            IncludeParent = 2,
            IncludeSiblings = 4,
            MarkPendingNodes = 8
        }
        private static void AddChildActions(ModuleAction objParentAction, Node objParentNode, IActionControl objActionControl)
        {
            AddChildActions(objParentAction, objParentNode, objParentNode, objActionControl, -1);
        }
        private static void AddChildActions(ModuleAction objParentAction, Node objParentNode, Node objRootNode, IActionControl objActionControl, int intDepth)
        {
            bool blnPending;
            foreach (ModuleAction objAction in objParentAction.Actions)
            {
                blnPending = IsActionPending(objParentNode, objRootNode, intDepth);
                if (objAction.Title == "~")
                {
                    if (blnPending == false)
                    {
                        objParentNode.Nodes.AddBreak();
                    }
                }
                else
                {
                    if (objAction.Visible == true && (objAction.Secure != SecurityAccessLevel.Anonymous || ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objActionControl.ModuleControl.ModuleContext.Configuration)) && ModulePermissionController.HasModuleAccess(objAction.Secure, Null.NullString, objActionControl.ModuleControl.ModuleContext.Configuration))
                    {
                        if (blnPending)
                        {
                            objParentNode.HasNodes = true;
                        }
                        else
                        {
                            Node objNode;
                            int i = objParentNode.Nodes.Add();
                            objNode = objParentNode.Nodes[i];
                            objNode.ID = objAction.ID.ToString();
                            objNode.Key = objAction.ID.ToString();
                            objNode.Text = objAction.Title;
                            if (!String.IsNullOrEmpty(objAction.ClientScript))
                            {
                                objNode.JSFunction = objAction.ClientScript;
                                objNode.ClickAction = eClickAction.None;
                            }
                            else
                            {
                                objNode.NavigateURL = objAction.Url;
                                if (objAction.UseActionEvent == false && !String.IsNullOrEmpty(objNode.NavigateURL))
                                {
                                    objNode.ClickAction = eClickAction.Navigate;
                                    if (objAction.NewWindow)
                                    {
                                        objNode.Target = "_blank";
                                    }
                                }
                                else
                                {
                                    objNode.ClickAction = eClickAction.PostBack;
                                }
                            }
                            objNode.Image = objAction.Icon;
                            if (objAction.HasChildren())
                            {
                                AddChildActions(objAction, objNode, objRootNode, objActionControl, intDepth);
                            }
                        }
                    }
                }
            }
        }
        private static void AddNode(TabInfo objTab, NodeCollection objNodes, Hashtable objBreadCrumbs, PortalSettings objPortalSettings, ToolTipSource eToolTips)
        {
            Node objNode = new Node();
            if (objTab.Title == "~")
            {
                objNodes.AddBreak();
            }
            else
            {
                if (objBreadCrumbs.Contains(objTab.TabID))
                {
                    objNode.BreadCrumb = true;
                    if (objTab.TabID == objPortalSettings.ActiveTab.TabID)
                    {
                        objNode.Selected = true;
                    }
                }
                if (objTab.DisableLink)
                    objNode.Enabled = false;
                objNode.ID = objTab.TabID.ToString();
                objNode.Key = objNode.ID;
                objNode.Text = objTab.LocalizedTabName;
                objNode.NavigateURL = objTab.FullUrl;
                objNode.ClickAction = eClickAction.Navigate;
                objNode.Image = objTab.IconFile;
                switch (eToolTips)
                {
                    case ToolTipSource.TabName:
                        objNode.ToolTip = objTab.LocalizedTabName;
                        break;
                    case ToolTipSource.Title:
                        objNode.ToolTip = objTab.Title;
                        break;
                    case ToolTipSource.Description:
                        objNode.ToolTip = objTab.Description;
                        break;
                }
                objNodes.Add(objNode);
            }
        }
        private static bool IsActionPending(Node objParentNode, Node objRootNode, int intDepth)
        {
            if (intDepth == -1)
                return false;
            if (objParentNode.Level + 1 - objRootNode.Level <= intDepth)
                return false;
            return true;
        }
        private static bool IsTabPending(TabInfo objTab, Node objParentNode, Node objRootNode, int intDepth, Hashtable objBreadCrumbs, int intLastBreadCrumbId, bool blnPOD)
        {
            if (intDepth == -1)
                return false;
            if (objParentNode.Level + 1 - objRootNode.Level <= intDepth)
                return false;
            if (blnPOD)
            {
                if (objBreadCrumbs.Contains(objTab.TabID))
                    return false;
                if (objBreadCrumbs.Contains(objTab.ParentId) && intLastBreadCrumbId != objTab.ParentId)
                    return false;
            }
            return true;
        }
        private static bool IsTabSibling(TabInfo objTab, int intStartTabId, Hashtable objTabLookup)
        {
            if (intStartTabId == -1)
            {
                if (objTab.ParentId == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (objTab.ParentId == ((TabInfo)objTabLookup[intStartTabId]).ParentId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void ProcessTab(Node objRootNode, TabInfo objTab, Hashtable objTabLookup, Hashtable objBreadCrumbs, int intLastBreadCrumbId, ToolTipSource eToolTips, int intStartTabId, int intDepth, int intNavNodeOptions)
        {
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            NodeCollection objRootNodes = objRootNode.Nodes;
            if (CanShowTab(objTab, TabPermissionController.CanAdminPage(), true))
            {
                NodeCollection objParentNodes;
                Node objParentNode = objRootNodes.FindNode(objTab.ParentId.ToString());
                bool blnParentFound = objParentNode != null;
                if (objParentNode == null)
                    objParentNode = objRootNode;
                objParentNodes = objParentNode.Nodes;
                if (objTab.TabID == intStartTabId)
                {
                    if ((intNavNodeOptions & (int)NavNodeOptions.IncludeParent) != 0)
                    {
                        if (objTabLookup[objTab.ParentId] != null)
                        {
                            AddNode((TabInfo)objTabLookup[objTab.ParentId], objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                            objParentNode = objRootNodes.FindNode(objTab.ParentId.ToString());
                            objParentNodes = objParentNode.Nodes;
                        }
                    }
                    if ((intNavNodeOptions & (int)NavNodeOptions.IncludeSelf) != 0)
                    {
                        AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                    }
                }
                else if (((intNavNodeOptions & (int)NavNodeOptions.IncludeSiblings) != 0) && IsTabSibling(objTab, intStartTabId, objTabLookup))
                {
                    AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                }
                else
                {
                    if (blnParentFound)
                    {
                        if (((intNavNodeOptions & (int)NavNodeOptions.IncludeSiblings) != 0) || IsTabSibling(objTab, intStartTabId, objTabLookup) == false)
                        {
                            bool blnPOD = (intNavNodeOptions & (int)NavNodeOptions.MarkPendingNodes) != 0;
                            if (IsTabPending(objTab, objParentNode, objRootNode, intDepth, objBreadCrumbs, intLastBreadCrumbId, blnPOD))
                            {
                                if (blnPOD)
                                {
                                    objParentNode.HasNodes = true;
                                }
                            }
                            else
                            {
                                AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                            }
                        }
                    }
                    else if ((intNavNodeOptions & (int)NavNodeOptions.IncludeSelf) == 0 && objTab.ParentId == intStartTabId)
                    {
                        AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips);
                    }
                }
            }
        }
        public static bool CanShowTab(TabInfo objTab, bool isAdminMode, bool showDisabled)
        {
            if (objTab.IsVisible && !objTab.IsDeleted && (!objTab.DisableLink || showDisabled) && (((objTab.StartDate < DateTime.Now || objTab.StartDate == Null.NullDate) && (objTab.EndDate > DateTime.Now || objTab.EndDate == Null.NullDate)) || isAdminMode) && TabPermissionController.CanNavigateToPage(objTab))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Node GetActionNode(string strID, string strNamespace, ModuleAction objActionRoot, Control objControl)
        {
            NodeCollection objNodes = GetActionNodes(objActionRoot, objControl, -1);
            Node objNode = objNodes.FindNode(strID);
            NodeCollection objReturnNodes = new NodeCollection(strNamespace);
            objReturnNodes.Import(objNode);
            objReturnNodes[0].ID = strID;
            return objReturnNodes[0];
        }
        public static NodeCollection GetActionNodes(ModuleAction objActionRoot, Control objControl)
        {
            return GetActionNodes(objActionRoot, objControl, -1);
        }
        public static NodeCollection GetActionNodes(ModuleAction objActionRoot, Control objControl, int intDepth)
        {
            NodeCollection objCol = new NodeCollection(objControl.ClientID);
            IActionControl objActionControl = objControl as IActionControl;
            if (objActionControl != null)
            {
                if (objActionRoot.Visible)
                {
                    objCol.Add();
                    Node objRoot = objCol[0];
                    objRoot.ID = objActionRoot.ID.ToString();
                    objRoot.Key = objActionRoot.ID.ToString();
                    objRoot.Text = objActionRoot.Title;
                    objRoot.NavigateURL = objActionRoot.Url;
                    objRoot.Image = objActionRoot.Icon;
                    objRoot.Enabled = false;
                    AddChildActions(objActionRoot, objRoot, objRoot.ParentNode, objActionControl, intDepth);
                }
            }
            return objCol;
        }
        public static NodeCollection GetActionNodes(ModuleAction objActionRoot, Node objRootNode, Control objControl, int intDepth)
        {
            NodeCollection objCol = objRootNode.ParentNode.Nodes;
            IActionControl objActionControl = objControl as IActionControl;
            if (objActionControl != null)
            {
                AddChildActions(objActionRoot, objRootNode, objRootNode, objActionControl, intDepth);
            }
            return objCol;
        }
        public static Node GetNavigationNode(string strID, string strNamespace)
        {
            NodeCollection objNodes = GetNavigationNodes(strNamespace);
            Node objNode = objNodes.FindNode(strID);
            NodeCollection objReturnNodes = new NodeCollection(strNamespace);
            objReturnNodes.Import(objNode);
            objReturnNodes[0].ID = strID;
            return objReturnNodes[0];
        }
        public static NodeCollection GetNavigationNodes(string strNamespace)
        {
            return GetNavigationNodes(strNamespace, ToolTipSource.None, -1, -1, 0);
        }
        public static NodeCollection GetNavigationNodes(string strNamespace, ToolTipSource eToolTips, int intStartTabId, int intDepth, int intNavNodeOptions)
        {
            NodeCollection objCol = new NodeCollection(strNamespace);
            return GetNavigationNodes(new Node(objCol.XMLNode), eToolTips, intStartTabId, intDepth, intNavNodeOptions);
        }
        public static NodeCollection GetNavigationNodes(Node objRootNode, ToolTipSource eToolTips, int intStartTabId, int intDepth, int intNavNodeOptions)
        {
            int i;
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            bool blnFoundStart = intStartTabId == -1;
            Hashtable objBreadCrumbs = new Hashtable();
            Hashtable objTabLookup = new Hashtable();
            NodeCollection objRootNodes = objRootNode.Nodes;
            int intLastBreadCrumbId = 0;
            for (i = 0; i <= (objPortalSettings.ActiveTab.BreadCrumbs.Count - 1); i++)
            {
                objBreadCrumbs.Add(((TabInfo)objPortalSettings.ActiveTab.BreadCrumbs[i]).TabID, 1);
                intLastBreadCrumbId = ((TabInfo)objPortalSettings.ActiveTab.BreadCrumbs[i]).TabID;
            }
            TabController objTabController = new TabController();
            List<TabInfo> portalTabs = TabController.GetTabsBySortOrder(objPortalSettings.PortalId);
            List<TabInfo> hostTabs = TabController.GetTabsBySortOrder(Null.NullInteger);
            foreach (TabInfo objTab in portalTabs)
            {
                objTabLookup.Add(objTab.TabID, objTab);
            }
            foreach (TabInfo objTab in hostTabs)
            {
                objTabLookup.Add(objTab.TabID, objTab);
            }
            foreach (TabInfo objTab in portalTabs)
            {
                try
                {
                    ProcessTab(objRootNode, objTab, objTabLookup, objBreadCrumbs, intLastBreadCrumbId, eToolTips, intStartTabId, intDepth, intNavNodeOptions);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            foreach (TabInfo objTab in hostTabs)
            {
                try
                {
                    ProcessTab(objRootNode, objTab, objTabLookup, objBreadCrumbs, intLastBreadCrumbId, eToolTips, intStartTabId, intDepth, intNavNodeOptions);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objRootNodes;
        }
    }
}
