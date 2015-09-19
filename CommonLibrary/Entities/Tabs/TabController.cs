using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using CommonLibrary.Data;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Content;
using CommonLibrary.Entities.Content.Common;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Content.Taxonomy;
using CommonLibrary.Security.Roles;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Services.Localization;
using CommonLibrary.Modules;

namespace CommonLibrary.Entities.Tabs
{
    public class TabController
    {
        ModuleClass module_obj = new ModuleClass();        
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public TabController() { }

        private string ip = IPNetworking.GetIP4Address();
        private static DataProvider provider = DataProvider.Instance();
        public static TabInfo CurrentPage
        {
            get
            {
                TabInfo _tab = null;
                if (PortalController.GetCurrentPortalSettings() != null)
                {
                    _tab = PortalController.GetCurrentPortalSettings().ActiveTab;
                }
                return _tab;
            }
        }

        #region Methods ======================================================================================
        public static List<aspnet_Tab> GetListByPortalId_ContentItemId_IsSecure(int PortalId, int ContentItemId, bool IsSecure)
        {
            using (TabDataContext dbContext = new TabDataContext(Settings.ConnectionString))
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                var query = from x in dbContext.aspnet_Tabs
                            where x.PortalId == PortalId
                            && x.ContentItemId == ContentItemId
                            && x.IsSecure == IsSecure
                            select x;
                return query.ToList();
            }
        }
        public static aspnet_Tab GetTabPathByTabId(int TabId)
        {
            using (TabDataContext tabDC = new TabDataContext(Settings.ConnectionString))
            {
                tabDC.ObjectTrackingEnabled = false;
                aspnet_Tab entity = (from x in tabDC.aspnet_Tabs 
                                where x.TabId == TabId
                            select x).SingleOrDefault();
                return entity;
            }
        }

        public List<aspnet_Tab> GetListByPortalIdIsSecure(int PortalId, bool IsSecure)
        {
            using (TabDataContext tabDC = new TabDataContext(Settings.ConnectionString))
            {
                tabDC.ObjectTrackingEnabled = false;
                var query = from x in tabDC.aspnet_Tabs where x.IsSecure == IsSecure select x;
                if(PortalId >= 0)
                    query = query.Where(x=>x.PortalId == PortalId);
                List<aspnet_Tab> list = query.ToList();
                int count = list.Count;
                return list;
            }
        }

        public DataTable GetActiveListByPortalIdIsSecure(int PortalId, bool IsSecure)
        {
            using (TabDataContext tabDC = new TabDataContext(Settings.ConnectionString))
            {
                tabDC.ObjectTrackingEnabled = false;
                List<aspnet_Tab> list = (from x in tabDC.aspnet_Tabs
                                         where x.PortalId == PortalId && x.IsSecure == IsSecure && x.IsVisible == true
                                         select x).ToList();
                DataTable dt = LinqHelper.ToDataTable(list);
                return dt;
            }
        }

        public DataTable GetAllParentNodesOfSelectedNode(int TabId, int IsSecure)
        {
           SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetAllParentNodesOfSelectedNode", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@IsSecure", IsSecure);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetAll", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByPortalId(string PortalId, int isadmin)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetListByPortalId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@isadmin", isadmin);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataSet GetActiveList()
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetVisibleList", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            con.Close();
            return ds;
        }

        public DataTable GetVisibleList()
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetVisibleList", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;

        }

        public DataTable GetDetailById(int TabId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetDetailById", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };

            cmd.Parameters.AddWithValue("@TabId", TabId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByStatus(string status)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetListByStatus", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };

            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByPositionStatus(string position, string status)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetListByPositionStatus", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@position", status);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByNumStatus(int num_rows, string status)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetListByNumStatus", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@num_rows", num_rows);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetIconFileById(int TabId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetIconFileById", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            object retunvalue = cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue.ToString();
        }

        public string GetIconFileLargeById(int TabId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetIconFileLargeById", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            object retunvalue = cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue.ToString();
        }

        public int[] Insert(int PortalId, int ContentItemId, int ParentId,
            string TabName, string Title, string CssClass, string IconFile, string IconFileLarge, string Description,
            string Keywords, bool DisplayTitle, bool IsDeleted, bool IsVisible, bool DisableLink, bool IsSecure,
            bool PermanentRedirect, decimal SiteMapPriority, string Url, string TabPath, int RouterId, string PageHeadText,
            string PageFooterText, string PageControlBar, string StartDate, string EndDate, string CultureCode, string CreatedByUserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_Insert", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@ContentItemId", ContentItemId);
            cmd.Parameters.AddWithValue("@ParentId", ParentId);
            cmd.Parameters.AddWithValue("@TabName", TabName);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@CssClass", CssClass);
            cmd.Parameters.AddWithValue("@IconFile", IconFile);
            cmd.Parameters.AddWithValue("@IconFileLarge", IconFileLarge);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@Keywords", Keywords);
            cmd.Parameters.AddWithValue("@DisplayTitle", DisplayTitle);
            cmd.Parameters.AddWithValue("@IsDeleted", IsDeleted);
            cmd.Parameters.AddWithValue("@IsVisible", IsVisible);
            cmd.Parameters.AddWithValue("@DisableLink", DisableLink);
            cmd.Parameters.AddWithValue("@IsSecure", IsSecure);
            cmd.Parameters.AddWithValue("@PermanentRedirect", PermanentRedirect);
            cmd.Parameters.AddWithValue("@SiteMapPriority", SiteMapPriority);
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@TabPath", TabPath);
            cmd.Parameters.AddWithValue("@RouterId", RouterId);
            cmd.Parameters.AddWithValue("@PageHeadText", PageHeadText);
            cmd.Parameters.AddWithValue("@PageFooterText", PageFooterText);
            cmd.Parameters.AddWithValue("@PageControlBar", PageControlBar);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@CreatedByUserId", CreatedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@o_TabId", SqlDbType.Int) { Direction = ParameterDirection.Output });

            con.Open();
            int i = cmd.ExecuteNonQuery();
            int[] retunvalue = new int[2];
            retunvalue[0] = int.Parse(cmd.Parameters["@o_return"].Value.ToString());
            retunvalue[1] = int.Parse(cmd.Parameters["@o_TabId"].Value.ToString());
            con.Close();
            return retunvalue;

        }

        public int Update(int TabId, int PortalId, int ContentItemId, int ParentId,
            string TabName, string Title, string CssClass, string IconFile, string IconFileLarge, string Description,
            string Keywords, bool DisplayTitle, bool IsDeleted, bool IsVisible, bool DisableLink, bool IsSecure,
            bool PermanentRedirect, decimal SiteMapPriority, string Url, string TabPath, int RouterId, string PageHeadText,
            string PageFooterText, string PageControlBar, string StartDate, string EndDate, string CultureCode, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_Update", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@ContentItemId", ContentItemId);
            cmd.Parameters.AddWithValue("@ParentId", ParentId);
            cmd.Parameters.AddWithValue("@TabName", TabName);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@CssClass", CssClass);
            cmd.Parameters.AddWithValue("@IconFile", IconFile);
            cmd.Parameters.AddWithValue("@IconFileLarge", IconFileLarge);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@Keywords", Keywords);
            cmd.Parameters.AddWithValue("@DisplayTitle", DisplayTitle);
            cmd.Parameters.AddWithValue("@IsDeleted", IsDeleted);
            cmd.Parameters.AddWithValue("@IsVisible", IsVisible);
            cmd.Parameters.AddWithValue("@DisableLink", DisableLink);
            cmd.Parameters.AddWithValue("@IsSecure", IsSecure);
            cmd.Parameters.AddWithValue("@PermanentRedirect", PermanentRedirect);
            cmd.Parameters.AddWithValue("@SiteMapPriority", SiteMapPriority);
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@TabPath", TabPath);
            cmd.Parameters.AddWithValue("@RouterId", RouterId);
            cmd.Parameters.AddWithValue("@PageHeadText", PageHeadText);
            cmd.Parameters.AddWithValue("@PageFooterText", PageFooterText);
            cmd.Parameters.AddWithValue("@PageControlBar", PageControlBar);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateLinkFrontPage(string TabId, string TabPath, int RouterId, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_UpdateLinkFrontPage", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@TabPath", TabPath);
            cmd.Parameters.AddWithValue("@RouterId", RouterId);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateVisiblity(int TabId, bool IsVisible, int LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_UpdateVisiblity", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@IsVisible", IsVisible);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        
        public int Delete(int TabId)
        {
            string small_dir_path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]), "tab_images\\large_icons");
            string large_dir_path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]), "tab_images\\small_icons");

            string small_icon_file = GetIconFileById(TabId);
            string large_icon_file = GetIconFileLargeById(TabId);

            if (small_icon_file != string.Empty)
            {
                module_obj.deleteFile(small_icon_file, small_dir_path);
            }

            if (large_icon_file != string.Empty)
            {
                module_obj.deleteFile(large_icon_file, large_dir_path);
            }

            SqlCommand cmd = new SqlCommand("aspnet_Tabs_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public string GenerateXmlFormat(int userId)
        {
            TabController menu_obj = new TabController();
            DataSet ds = new DataSet();
            //ds = menu_obj.GetHierarchicalTreeByUserId(userId);
            ds.DataSetName = "Menus";
            ds.Tables[0].TableName = "aspnet_Menu";
            //create Relation Parent and Child
            DataRelation relation = new DataRelation("ParentChild", ds.Tables[0].Columns["Idx"], ds.Tables[0].Columns["ParentId"], true);
            relation.Nested = true;
            ds.Relations.Add(relation);

            return ds.GetXml();
        }

        public string ExecuteXSLTransformation(int userId, string xslt_filepath)
        {
            string ErrorMsg, HtmlTags, XsltPath;
            MemoryStream DataStream = default(MemoryStream);
            StreamReader streamReader = default(StreamReader);

            try
            {
                //Path of XSLT file
                XsltPath = HttpContext.Current.Server.MapPath(xslt_filepath);

                //Encode all Xml format string to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(GenerateXmlFormat(userId));

                DataStream = new MemoryStream(bytes);

                //Create Xmlreader from memory stream

                XmlReader reader = XmlReader.Create(DataStream);

                // Load the XML 
                XPathDocument document = new XPathDocument(reader);


                XslCompiledTransform XsltFormat = new XslCompiledTransform();

                // Load the style sheet.
                XsltFormat.Load(XsltPath);

                DataStream = new MemoryStream();

                XmlTextWriter writer = new XmlTextWriter(DataStream, Encoding.UTF8);


                //Apply transformation from xml format to html format and save it in xmltextwriter
                XsltFormat.Transform(document, writer);

                streamReader = new StreamReader(DataStream);

                DataStream.Position = 0;

                HtmlTags = streamReader.ReadToEnd();

                return HtmlTags;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return ErrorMsg;
            }
            finally
            {
                //Release the resources 
                streamReader.Close();
                DataStream.Close();
            }
        }
        #endregion ===========================================================================================

        private void AddAllTabsModules(TabInfo objTab)
        {
            ModuleController objmodules = new ModuleController();
            foreach (ModuleInfo allTabsModule in objmodules.GetAllTabsModules(objTab.PortalID, true))
            {
                bool canAdd = false;
                foreach (ModuleInfo allTabsInstance in objmodules.GetModuleTabs(allTabsModule.ModuleID))
                {
                    TabInfo tab = new TabController().GetTab(allTabsInstance.TabID, objTab.PortalID, false);
                    if (!tab.IsDeleted)
                    {
                        canAdd = true;
                        break;
                    }
                }
                if (canAdd)
                {
                    objmodules.CopyModule(allTabsModule.ModuleID, allTabsModule.TabID, objTab.TabID, "", true);
                }
            }
        }
        private int AddTabInternal(TabInfo objTab, bool includeAllTabsModules)
        {
            bool newTab = true;
            objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
            int iTabID = GetTabByTabPath(objTab.PortalID, objTab.TabPath);
            if (iTabID > Null.NullInteger)
            {
                throw new TabExistsException(iTabID, "Tab Exists");
            }
            else
            {
                //First create ContentItem as we need the ContentItemID
                IContentTypeController typeController = new ContentTypeController();
                ContentType contentType = (from t in typeController.GetContentTypes()
                                           where t.Type == "Tab"
                                           select t)
                                                 .SingleOrDefault();

                IContentController contentController = Util.GetContentController();
                if (String.IsNullOrEmpty(objTab.Title))
                {
                    objTab.Content = objTab.TabName;
                }
                else
                {
                    objTab.Content = objTab.Title;
                }
                objTab.ContentTypeId = contentType.ContentTypeId;
                objTab.Indexed = false;
                int contentItemID = contentController.AddContentItem(objTab);

                //Add Module
                iTabID = provider.AddTab(contentItemID, objTab.PortalID, objTab.TabName, objTab.IsVisible, objTab.DisableLink, objTab.ParentId, objTab.IconFile, objTab.IconFileLarge, objTab.Title, objTab.Description, objTab.KeyWords,
                objTab.Url, objTab.SkinSrc, objTab.ContainerSrc, objTab.TabPath, objTab.StartDate, objTab.EndDate, objTab.RefreshInterval, objTab.PageHeadText, objTab.IsSecure, objTab.PermanentRedirect,
                objTab.SiteMapPriority, UserController.GetCurrentUserInfo().UserID, PortalController.GetActivePortalLanguage(objTab.PortalID));
                objTab.TabID = iTabID;
                //Now we have the TabID - update the contentItem
                contentController.UpdateContentItem(objTab);

                ITermController termController = Util.GetTermController();
                termController.RemoveTermsFromContent(objTab);
                foreach (Term _Term in objTab.Terms)
                {
                    termController.AddTermToContent(_Term, objTab);
                }
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.TAB_CREATED);
                TabPermissionController.SaveTabPermissions(objTab);
                try
                {
                    UpdateTabSettings(ref objTab);
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                }
                if (includeAllTabsModules)
                {
                    AddAllTabsModules(objTab);
                }
            }
            return objTab.TabID;
        }
        private void AddTabToEndOfList(TabInfo objTab, bool updateTabPath)
        {
            List<TabInfo> siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            TabInfo parentTab = GetTab(objTab.ParentId, objTab.PortalID, false);
            if (parentTab == null)
            {
                objTab.Level = 0;
            }
            else
            {
                objTab.Level = parentTab.Level + 1;
            }
            UpdateTabOrder(siblingTabs, 2);
            objTab.TabOrder = 2 * siblingTabs.Count + 1;
            UpdateTabOrder(objTab, updateTabPath);
        }
        private void DeleteTabInternal(int tabId, int portalId)
        {
            //Get the tab from the Cache
            TabInfo objTab = GetTab(tabId, portalId, false);

            //Delete Tab
            provider.DeleteTab(tabId);

            //Remove the Content Item
            if (objTab.ContentItemId > Null.NullInteger)
            {
                IContentController ctl = Util.GetContentController();
                ctl.DeleteContentItem(objTab);
            }

            //Log deletion
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("TabID", tabId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, ((int)Services.Log.EventLog.EventLogController.EventLogType.TAB_DELETED).ToString());
        }
        private void DemoteTab(TabInfo objTab, List<TabInfo> siblingTabs)
        {
            int siblingCount = siblingTabs.Count;
            int tabIndex = GetIndexOfTab(objTab, siblingTabs);
            if (tabIndex > 0)
            {
                UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);
                TabInfo parentTab = siblingTabs[tabIndex - 1];
                List<TabInfo> descendantTabs = GetTabsByPortal(objTab.PortalID).DescendentsOf(objTab.TabID);
                objTab.ParentId = parentTab.TabID;
                AddTabToEndOfList(objTab, true);
                UpdateDescendantLevel(descendantTabs, 1);
            }
        }
        private TabInfo GetTabByNameAndParent(string TabName, int PortalId, int ParentId)
        {
            ArrayList arrTabs = GetTabsByNameAndPortal(TabName, PortalId);
            int intTab = -1;
            if (arrTabs != null)
            {
                switch (arrTabs.Count)
                {
                    case 0:
                        break;
                    case 1:
                        intTab = 0;
                        break;
                    default:
                        int intIndex;
                        TabInfo objTab;
                        for (intIndex = 0; intIndex <= arrTabs.Count - 1; intIndex++)
                        {
                            objTab = (TabInfo)arrTabs[intIndex];
                            if (objTab.ParentId == ParentId)
                            {
                                intTab = intIndex;
                            }
                        }

                        if (intTab == -1)
                        {
                            intTab = 0;
                        }
                        break;
                }
            }
            if (intTab != -1)
            {
                return (TabInfo)arrTabs[intTab];
            }
            else
            {
                return null;
            }
        }
        private ArrayList GetTabsByNameAndPortal(string TabName, int PortalId)
        {
            ArrayList returnTabs = new ArrayList();
            foreach (KeyValuePair<int, TabInfo> kvp in GetTabsByPortal(PortalId))
            {
                TabInfo objTab = kvp.Value;
                if (String.Compare(objTab.TabName, TabName, true) == 0)
                {
                    returnTabs.Add(objTab);
                }
            }
            return returnTabs;
        }
        private int GetIndexOfTab(TabInfo objTab, List<TabInfo> tabs)
        {
            int tabIndex = Null.NullInteger;
            for (int index = 0; index <= tabs.Count - 1; index++)
            {
                if (tabs[index].TabID == objTab.TabID)
                {
                    tabIndex = index;
                    break;
                }
            }
            return tabIndex;
        }
        private void PromoteTab(TabInfo objTab, List<TabInfo> siblingTabs)
        {
            int siblingCount = siblingTabs.Count;
            TabInfo parentTab = GetTab(objTab.ParentId, objTab.PortalID, false);
            if (parentTab != null)
            {
                int tabIndex = GetIndexOfTab(objTab, siblingTabs);
                UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);
                siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(parentTab.ParentId);
                siblingCount = siblingTabs.Count;
                UpdateTabOrder(siblingTabs, 2);
                int parentIndex = GetIndexOfTab(parentTab, siblingTabs);
                UpdateTabOrder(siblingTabs, parentIndex + 1, siblingCount - 1, 2);
                List<TabInfo> descendantTabs = GetTabsByPortal(objTab.PortalID).DescendentsOf(objTab.TabID);
                objTab.ParentId = parentTab.ParentId;
                objTab.TabOrder = parentTab.TabOrder + 2;
                UpdateTab(objTab);
                //objTab.Level = objTab.Level - 1;
                //UpdateTabOrder(objTab, true);
                UpdateDescendantLevel(descendantTabs, -1);
            }
        }
        private void RemoveTab(TabInfo objTab)
        {
            List<TabInfo> siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            int siblingCount = siblingTabs.Count;
            for (int index = 0; index <= siblingCount - 1; index++)
            {
                TabInfo sibling = siblingTabs[index];
                if (sibling.TabID == objTab.TabID)
                {
                    UpdateTabOrder(siblingTabs, index + 1, siblingCount - 1, -2);
                    break;
                }
            }
        }
        private void SwapAdjacentTabs(TabInfo firstTab, TabInfo secondTab)
        {
            firstTab.TabOrder -= 2;
            UpdateTabOrder(firstTab, false);
            secondTab.TabOrder += 2;
            UpdateTabOrder(secondTab, false);
        }
        private void UpdateChildTabPath(int intTabid, int portalId)
        {
            foreach (TabInfo objtab in GetTabsByPortal(portalId).DescendentsOf(intTabid))
            {
                string oldTabPath = objtab.TabPath;
                objtab.TabPath = Globals.GenerateTabPath(objtab.ParentId, objtab.TabName);
                if (oldTabPath != objtab.TabPath)
                {
                    provider.UpdateTab(objtab.TabID, objtab.ContentItemId, objtab.PortalID, objtab.TabName, objtab.IsVisible, objtab.DisableLink, objtab.ParentId, objtab.IconFile, objtab.IconFileLarge, objtab.Title, objtab.Description,
                    objtab.KeyWords, objtab.IsDeleted, objtab.Url, objtab.SkinSrc, objtab.ContainerSrc, objtab.TabPath, objtab.StartDate, objtab.EndDate, objtab.RefreshInterval, objtab.PageHeadText,
                    objtab.IsSecure, objtab.PermanentRedirect, objtab.SiteMapPriority, UserController.GetCurrentUserInfo().UserID, PortalController.GetActivePortalLanguage(objtab.PortalID));
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog("TabID", intTabid.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Services.Log.EventLog.EventLogController.EventLogType.TAB_UPDATED);
                }
            }
        }
        private void UpdateDescendantLevel(List<TabInfo> descendantTabs, int levelDelta)
        {
            foreach (TabInfo descendent in descendantTabs)
            {
                descendent.Level = descendent.Level + levelDelta;
                UpdateTabOrder(descendent, true);
            }
        }
        private void UpdateTabOrder(TabInfo objTab, bool updateTabPath)
        {
            if (updateTabPath)
            {
                objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
            }
            provider.UpdateTabOrder(objTab.TabID, objTab.TabOrder, objTab.Level, objTab.ParentId, objTab.TabPath, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.TAB_ORDER_UPDATED);
        }
        private void UpdateTabOrder(List<TabInfo> tabs, int startIndex, int endIndex, int increment)
        {
            for (int index = startIndex; index <= endIndex; index++)
            {
                TabInfo objTab = tabs[index];
                objTab.TabOrder += increment;
                UpdateTabOrder(objTab, false);
            }
        }
        private void UpdateTabOrder(List<TabInfo> tabs, int increment)
        {
            int tabOrder = 1;
            for (int index = 0; index <= tabs.Count - 1; index++)
            {
                TabInfo objTab = tabs[index];
                objTab.TabOrder = tabOrder;
                UpdateTabOrder(objTab, false);
                tabOrder += increment;
            }
        }
        public int AddTab(TabInfo objTab)
        {
            return AddTab(objTab, true);
        }
        public int AddTab(TabInfo objTab, bool includeAllTabsModules)
        {
            int tabID = AddTabInternal(objTab, includeAllTabsModules);
            AddTabToEndOfList(objTab, false);
            ClearCache(objTab.PortalID);
            DataCache.RemoveCache(DataCache.PortalDictionaryCacheKey);
            return tabID;
        }
        public int AddTabAfter(TabInfo objTab, int afterTabId)
        {
            List<TabInfo> siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            int siblingCount = siblingTabs.Count;
            UpdateTabOrder(siblingTabs, 2);
            int tabID = AddTabInternal(objTab, true);
            for (int index = 0; index <= siblingCount - 1; index++)
            {
                TabInfo sibling = siblingTabs[index];
                if (sibling.TabID == afterTabId)
                {
                    objTab.Level = sibling.Level;
                    objTab.TabOrder = sibling.TabOrder + 2;
                    UpdateTabOrder(objTab, false);
                    UpdateTabOrder(siblingTabs, index + 1, siblingCount - 1, 2);
                    break;
                }
            }
            ClearCache(objTab.PortalID);
            return tabID;
        }

        public void MoveTabAfter(TabInfo objTab, int afterTabId)
        {
            if ((objTab.TabID < 0))
            {
                return;
            }

            //Get the List of tabs with the same parent
            List<TabInfo> siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            int siblingCount = siblingTabs.Count;

            //First make sure that the siblings are sorted correctly
            UpdateTabOrder(siblingTabs, 2);

            //New tab is to be inserted into the siblings List after TabId=afterTabId
            for (int index = 0; index <= siblingCount - 1; index++)
            {
                TabInfo sibling = siblingTabs[index];
                if (sibling.TabID == afterTabId)
                {
                    //we need to insert the tab here
                    objTab.Level = sibling.Level;
                    objTab.TabOrder = sibling.TabOrder + 2;

                    //UpdateOrder - Parent hasn't changed so we don't need to regenerate TabPath
                    UpdateTabOrder(objTab, false);

                    //We need to update the taborder for the remaining items, excluding the current tab
                    //UpdateTabOrder(siblingTabs, index + 1, siblingCount - 1, 2)
                    int remainingTabOrder = objTab.TabOrder;
                    for (int remainingIndex = index + 1; remainingIndex <= siblingCount - 1; remainingIndex++)
                    {
                        TabInfo remainingTab = siblingTabs[remainingIndex];

                        if ((remainingTab.TabID == objTab.TabID))
                        {
                            continue;
                        }
                        remainingTabOrder = remainingTabOrder + 2;
                        remainingTab.TabOrder = remainingTabOrder;

                        //UpdateOrder - Parent hasn't changed so we don't need to regenerate TabPath
                        UpdateTabOrder(remainingTab, false);
                    }
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            //Clear the Cache
            ClearCache(objTab.PortalID);
        }

        public void MoveTabBefore(TabInfo objTab, int beforeTabId)
        {
            if ((objTab.TabID < 0))
            {
                return;
            }

            //Get the List of tabs with the same parent
            List<TabInfo> siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            int siblingCount = siblingTabs.Count;

            //First make sure that the siblings are sorted correctly
            UpdateTabOrder(siblingTabs, 2);

            //New tab is to be inserted into the siblings List before TabId=beforeTabId
            for (int index = 0; index <= siblingCount - 1; index++)
            {
                TabInfo sibling = siblingTabs[index];
                if (sibling.TabID == beforeTabId)
                {
                    //we need to insert the tab here
                    objTab.Level = sibling.Level;
                    objTab.TabOrder = sibling.TabOrder;

                    //UpdateOrder - Parent hasn't changed so we don't need to regenerate TabPath
                    UpdateTabOrder(objTab, false);

                    //We need to update the taborder for the remaining items, including the current one
                    int remainingTabOrder = objTab.TabOrder;
                    for (int remainingIndex = index; remainingIndex <= siblingCount - 1; remainingIndex++)
                    {
                        TabInfo remainingTab = siblingTabs[remainingIndex];

                        if ((remainingTab.TabID == objTab.TabID))
                        {
                            continue;
                        }

                        remainingTabOrder = remainingTabOrder + 2;
                        remainingTab.TabOrder = remainingTabOrder;

                        //UpdateOrder - Parent hasn't changed so we don't need to regenerate TabPath
                        UpdateTabOrder(remainingTab, false);
                    }
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            //Clear the Cache
            ClearCache(objTab.PortalID);
        }

        public int AddTabBefore(TabInfo objTab, int beforeTabId)
        {
            List<TabInfo> siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            int siblingCount = siblingTabs.Count;
            UpdateTabOrder(siblingTabs, 2);
            int tabID = AddTabInternal(objTab, true);
            for (int index = 0; index <= siblingCount - 1; index++)
            {
                TabInfo sibling = siblingTabs[index];
                if (sibling.TabID == beforeTabId)
                {
                    objTab.Level = sibling.Level;
                    objTab.TabOrder = sibling.TabOrder;
                    UpdateTabOrder(objTab, false);
                    UpdateTabOrder(siblingTabs, index, siblingCount - 1, 2);
                    break;
                }
            }
            ClearCache(objTab.PortalID);
            return tabID;
        }
        public void ClearCache(int portalId)
        {
            DataCache.ClearTabsCache(portalId);
            DataCache.ClearPortalCache(portalId, false);
        }
        public void CopyTab(int PortalId, int FromTabId, int ToTabId, bool asReference)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;
            foreach (KeyValuePair<int, ModuleInfo> kvp in objModules.GetTabModules(FromTabId))
            {
                objModule = kvp.Value;
                if (!objModule.AllTabs)
                {
                    if (asReference == false)
                    {
                        objModule.ModuleID = Null.NullInteger;
                    }
                    objModule.TabID = ToTabId;
                    objModules.AddModule(objModule);
                }
            }
        }
        public void CreateContentItem(TabInfo updatedTab)
        {
            IContentTypeController typeController = new ContentTypeController();
            ContentType contentType = (from t in typeController.GetContentTypes()
                                       where t.Type == "Tab"
                                       select t).SingleOrDefault();

            //This tab does not have a valid ContentItem
            //create ContentItem
            IContentController contentController = Util.GetContentController();
            if (string.IsNullOrEmpty(updatedTab.Title))
            {
                updatedTab.Content = updatedTab.TabName;
            }
            else
            {
                updatedTab.Content = updatedTab.Title;
            }
            updatedTab.ContentTypeId = contentType.ContentTypeId;
            updatedTab.Indexed = false;
            updatedTab.ContentItemId = contentController.AddContentItem(updatedTab);
        }
        public void DeleteTab(int TabId, int PortalId)
        {
            if (GetTabsByPortal(PortalId).WithParentId(TabId).Count == 0)
            {
                TabInfo objTab = GetTab(TabId, PortalId, false);
                List<TabInfo> siblingTabs = GetTabsByPortal(PortalId).WithParentId(objTab.ParentId);
                int siblingCount = siblingTabs.Count;
                for (int index = 0; index <= siblingCount - 1; index++)
                {
                    TabInfo sibling = siblingTabs[index];
                    if (sibling.TabID == TabId)
                    {
                        DeleteTabInternal(TabId, PortalId);
                        UpdateTabOrder(siblingTabs, index + 1, siblingCount - 1, -2);
                        break;
                    }
                }
                ClearCache(PortalId);
                DataCache.RemoveCache(DataCache.PortalDictionaryCacheKey);
            }
        }
        public void DeleteTab(int TabId, int PortalId, bool deleteDescendants)
        {
            List<TabInfo> descendantList = GetTabsByPortal(PortalId).DescendentsOf(TabId);
            if (deleteDescendants && descendantList.Count > 0)
            {
                for (int i = descendantList.Count - 1; i >= 0; i += -1)
                {
                    DeleteTabInternal(descendantList[i].TabID, PortalId);
                }
                ClearCache(PortalId);
            }
            DeleteTab(TabId, PortalId);
        }
        public ArrayList GetAllTabs(bool CheckLegacyFields)
        {
            return CBO.FillCollection(provider.GetAllTabs(), typeof(TabInfo));
        }
        public ArrayList GetAllTabs()
        {
            return GetAllTabs(true);
        }
        public TabInfo GetTab(int TabId, int PortalId, bool ignoreCache)
        {
            TabInfo tab = null;
            bool bFound = false;
            if (!ignoreCache)
            {
                if (Null.IsNull(PortalId))
                {
                    Dictionary<int, int> portalDic = PortalController.GetPortalDictionary();
                    if (portalDic != null && portalDic.ContainsKey(TabId))
                    {
                        PortalId = portalDic[TabId];
                    }
                }
                if (!Null.IsNull(PortalId))
                {
                    Dictionary<int, TabInfo> dicTabs;
                    dicTabs = GetTabsByPortal(PortalId);
                    bFound = dicTabs.TryGetValue(TabId, out tab);
                }
            }
            if (ignoreCache || !bFound)
            {
                tab = CBO.FillObject<TabInfo>(provider.GetTab(TabId));
            }
            return tab;
        }
        public TabInfo GetTabByName(string TabName, int PortalId)
        {
            return GetTabByNameAndParent(TabName, PortalId, int.MinValue);
        }
        public TabInfo GetTabByName(string TabName, int PortalId, int ParentId)
        {
            return GetTabByNameAndParent(TabName, PortalId, ParentId);
        }
        public int GetTabCount(int portalId)
        {
            return GetTabsByPortal(portalId).Count;
        }
        public TabCollection GetTabsByPortal(int portalId)
        {
            string cacheKey = string.Format(DataCache.TabCacheKey, portalId.ToString());
            return CBO.GetCachedObject<TabCollection>(new CacheItemArgs(cacheKey, DataCache.TabCacheTimeOut, DataCache.TabCachePriority, portalId), GetTabsByPortalCallBack);
        }
        public IDictionary<int, TabInfo> GetTabsByModuleID(int moduleID)
        {
            return CBO.FillDictionary<int, TabInfo>("TabID", provider.GetTabsByModuleID(moduleID));
        }
        public IDictionary<int, TabInfo> GetTabsByPackageID(int portalID, int packageID, bool forHost)
        {
            return CBO.FillDictionary<int, TabInfo>("TabID", provider.GetTabsByPackageID(portalID, packageID, forHost));
        }
        public void MoveTab(TabInfo objTab, TabMoveType type)
        {
            List<TabInfo> siblingTabs = GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            int siblingCount = siblingTabs.Count;
            int tabIndex = 0;
            UpdateTabOrder(siblingTabs, 2);
            switch (type)
            {
                case TabMoveType.Top:
                    objTab.TabOrder = 1;
                    UpdateTabOrder(objTab, false);
                    tabIndex = GetIndexOfTab(objTab, siblingTabs);
                    UpdateTabOrder(siblingTabs, 0, tabIndex - 1, 2);
                    break;
                case TabMoveType.Bottom:
                    objTab.TabOrder = siblingCount * 2 - 1;
                    UpdateTabOrder(objTab, false);
                    tabIndex = GetIndexOfTab(objTab, siblingTabs);
                    UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);
                    break;
                case TabMoveType.Up:
                    tabIndex = GetIndexOfTab(objTab, siblingTabs);
                    if (tabIndex > 0)
                    {
                        TabInfo swapTab = siblingTabs[tabIndex - 1];
                        SwapAdjacentTabs(objTab, swapTab);
                    }
                    break;
                case TabMoveType.Down:
                    tabIndex = GetIndexOfTab(objTab, siblingTabs);
                    if (tabIndex < siblingCount - 1)
                    {
                        TabInfo swapTab = siblingTabs[tabIndex + 1];
                        SwapAdjacentTabs(swapTab, objTab);
                    }
                    break;
                case TabMoveType.Promote:
                    PromoteTab(objTab, siblingTabs);
                    break;
                case TabMoveType.Demote:
                    DemoteTab(objTab, siblingTabs);
                    break;
            }
            ClearCache(objTab.PortalID);
        }
        public void UpdateTab(TabInfo updatedTab)
        {
            UpdateTab(updatedTab, PortalController.GetActivePortalLanguage(updatedTab.PortalID));
        }
        public void UpdateTab(TabInfo updatedTab, string CultureCode)
        {
            TabInfo originalTab = GetTab(updatedTab.TabID, updatedTab.PortalID, true);
            bool updateOrder = (originalTab.ParentId != updatedTab.ParentId);
            int levelDelta = (updatedTab.Level - originalTab.Level);
            bool updateChildren = (originalTab.TabName != updatedTab.TabName || updateOrder);
            //Update ContentItem If neccessary
            if (updatedTab.ContentItemId == Null.NullInteger && updatedTab.TabID != Null.NullInteger)
            {
                CreateContentItem(updatedTab);
            }
            provider.UpdateTab(updatedTab.TabID, updatedTab.ContentItemId, updatedTab.PortalID, updatedTab.TabName, updatedTab.IsVisible, updatedTab.DisableLink, updatedTab.ParentId, updatedTab.IconFile, updatedTab.IconFileLarge, updatedTab.Title, updatedTab.Description,
            updatedTab.KeyWords, updatedTab.IsDeleted, updatedTab.Url, updatedTab.SkinSrc, updatedTab.ContainerSrc, updatedTab.TabPath, updatedTab.StartDate, updatedTab.EndDate, updatedTab.RefreshInterval, updatedTab.PageHeadText,
            updatedTab.IsSecure, updatedTab.PermanentRedirect, updatedTab.SiteMapPriority, UserController.GetCurrentUserInfo().UserID, CultureCode);
            //Update Tags
            ITermController termController = Util.GetTermController();
            termController.RemoveTermsFromContent(updatedTab);
            foreach (Term _Term in updatedTab.Terms)
            {
                termController.AddTermToContent(_Term, updatedTab);
            }
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(updatedTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.TAB_UPDATED);
            TabPermissionController.SaveTabPermissions(updatedTab);
            try
            {
                UpdateTabSettings(ref updatedTab);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            if (levelDelta != 0)
            {
                List<TabInfo> descendantTabs = GetTabsByPortal(updatedTab.PortalID).DescendentsOf(updatedTab.TabID);
                UpdateDescendantLevel(descendantTabs, levelDelta);
            }
            if (updateOrder)
            {
                RemoveTab(updatedTab);
                AddTabToEndOfList(updatedTab, true);
            }
            if (updateChildren)
            {
                ClearCache(updatedTab.PortalID);
                UpdateChildTabPath(updatedTab.TabID, updatedTab.PortalID);
            }
            ClearCache(updatedTab.PortalID);
            if (updatedTab.PortalID != originalTab.PortalID)
            {
                ClearCache(originalTab.PortalID);
            }
        }
        private void UpdateTabSettings(ref TabInfo updatedTab)
        {
            foreach (string sKey in updatedTab.TabSettings.Keys)
            {
                UpdateTabSetting(updatedTab.TabID, sKey, Convert.ToString(updatedTab.TabSettings[sKey]));
            }
        }
        public void UpdateTabOrder(TabInfo objTab)
        {
            UpdateTabOrder(objTab, true);
        }
        public void PopulateBreadCrumbs(ref TabInfo tab)
        {
            if ((tab.BreadCrumbs == null))
            {
                ArrayList crumbs = new ArrayList();
                PopulateBreadCrumbs(tab.PortalID, ref crumbs, tab.TabID);
                tab.BreadCrumbs = crumbs;
            }
        }
        public void PopulateBreadCrumbs(int portalID, ref ArrayList breadCrumbs, int tabID)
        {
            TabInfo objTab = null;
            TabController objTabController = new TabController();
            TabCollection portalTabs = objTabController.GetTabsByPortal(portalID);
            TabCollection hostTabs = objTabController.GetTabsByPortal(Null.NullInteger);
            bool blnFound = portalTabs.TryGetValue(tabID, out objTab);
            if (!blnFound)
            {
                blnFound = hostTabs.TryGetValue(tabID, out objTab);
            }
            if (blnFound)
            {
                breadCrumbs.Insert(0, objTab.Clone());
                if (!Null.IsNull(objTab.ParentId))
                {
                    PopulateBreadCrumbs(portalID, ref breadCrumbs, objTab.ParentId);
                }
            }
        }
        public Hashtable GetTabSettings(int TabId)
        {
            Hashtable objSettings;
            string strCacheKey = "GetTabSettings" + TabId.ToString();
            objSettings = (Hashtable)DataCache.GetCache(strCacheKey);
            if (objSettings == null)
            {
                objSettings = new Hashtable();
                IDataReader dr = provider.GetTabSettings(TabId);
                while (dr.Read())
                {
                    if (!dr.IsDBNull(1))
                    {
                        objSettings[dr.GetString(0)] = dr.GetString(1);
                    }
                    else
                    {
                        objSettings[dr.GetString(0)] = "";
                    }
                }
                dr.Close();
                int intCacheTimeout = 20 * Convert.ToInt32(Host.Host.PerformanceSetting);
                DataCache.SetCache(strCacheKey, objSettings, TimeSpan.FromMinutes(intCacheTimeout));
            }
            return objSettings;
        }
        public void UpdateTabSetting(int TabId, string SettingName, string SettingValue)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TabId", TabId.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingValue", SettingValue.ToString()));
            IDataReader dr = provider.GetTabSetting(TabId, SettingName);
            if (dr.Read())
            {
                provider.UpdateTabSetting(TabId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TAB_SETTING_UPDATED.ToString();
                objEventLog.AddLog(objEventLogInfo);
            }
            else
            {
                provider.AddTabSetting(TabId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TAB_SETTING_CREATED.ToString();
                objEventLog.AddLog(objEventLogInfo);
            }
            dr.Close();
            DataCache.RemoveCache("GetTabSettings" + TabId.ToString());
        }
        public void DeleteTabSetting(int TabId, string SettingName)
        {
            provider.DeleteTabSetting(TabId, SettingName);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TabID", TabId.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString()));
            objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TAB_SETTING_DELETED.ToString();
            objEventLog.AddLog(objEventLogInfo);
            DataCache.RemoveCache("GetTabSettings" + TabId.ToString());
        }
        public void DeleteTabSettings(int TabId)
        {
            provider.DeleteTabSettings(TabId);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TabId", TabId.ToString()));
            objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TAB_SETTING_DELETED.ToString();
            objEventLog.AddLog(objEventLogInfo);
            DataCache.RemoveCache("GetTabSettings" + TabId.ToString());
        }
        private static bool DeleteChildTabs(int intTabid, PortalSettings PortalSettings, int UserId)
        {
            TabController objtabs = new TabController();
            bool bDeleted = true;
            foreach (TabInfo objtab in GetTabsByParent(intTabid, PortalSettings.PortalId))
            {
                bDeleted = DeleteTab(objtab, PortalSettings, UserId);
                if (!bDeleted)
                {
                    break;
                }
            }
            return bDeleted;
        }
        private static bool DeleteTab(TabInfo objtab, PortalSettings PortalSettings, int UserId)
        {
            TabController objtabs = new TabController();
            bool bDeleted = true;
            if (!IsSpecialTab(objtab.TabID, PortalSettings))
            {
                if (DeleteChildTabs(objtab.TabID, PortalSettings, UserId))
                {
                    objtab.IsDeleted = true;
                    objtabs.UpdateTab(objtab);
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog(objtab, PortalSettings, UserId, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TAB_SENT_TO_RECYCLE_BIN);
                }
                else
                {
                    bDeleted = false;
                }
            }
            else
            {
                bDeleted = false;
            }
            return bDeleted;
        }
        private static void DeserializeTabSettings(XmlNodeList nodeTabSettings, TabInfo objTab)
        {
            string sKey;
            string sValue;
            foreach (XmlNode oTabSettingNode in nodeTabSettings)
            {
                sKey = XmlUtils.GetNodeValue(oTabSettingNode.CreateNavigator(), "settingname");
                sValue = XmlUtils.GetNodeValue(oTabSettingNode.CreateNavigator(), "settingvalue");
                objTab.TabSettings[sKey] = sValue;
            }
        }
        private static void DeserializeTabPermissions(XmlNodeList nodeTabPermissions, TabInfo objTab, bool IsAdminTemplate)
        {
            Security.Permissions.PermissionController objPermissionController = new Security.Permissions.PermissionController();
            Security.Permissions.PermissionInfo objPermission;
            Security.Permissions.TabPermissionInfo objTabPermission;
            RoleController objRoleController = new RoleController();
            RoleInfo objRole;
            int RoleID;
            int PermissionID = 0;
            string PermissionKey;
            string PermissionCode;
            string RoleName;
            bool AllowAccess;
            ArrayList arrPermissions;
            int i;
            foreach (XmlNode xmlTabPermission in nodeTabPermissions)
            {
                PermissionKey = XmlUtils.GetNodeValue(xmlTabPermission.CreateNavigator(), "permissionkey");
                PermissionCode = XmlUtils.GetNodeValue(xmlTabPermission.CreateNavigator(), "permissioncode");
                RoleName = XmlUtils.GetNodeValue(xmlTabPermission.CreateNavigator(), "rolename");
                AllowAccess = XmlUtils.GetNodeValueBoolean(xmlTabPermission, "allowaccess");
                arrPermissions = objPermissionController.GetPermissionByCodeAndKey(PermissionCode, PermissionKey);
                for (i = 0; i <= arrPermissions.Count - 1; i++)
                {
                    objPermission = (Security.Permissions.PermissionInfo)arrPermissions[i];
                    PermissionID = objPermission.PermissionID;
                }
                RoleID = int.MinValue;
                switch (RoleName)
                {
                    case Globals.glbRoleAllUsersName:
                        RoleID = Convert.ToInt32(Globals.glbRoleAllUsers);
                        break;
                    case Common.Globals.glbRoleUnauthUserName:
                        RoleID = Convert.ToInt32(Globals.glbRoleUnauthUser);
                        break;
                    default:
                        PortalController objPortals = new PortalController();
                        PortalInfo objPortal = objPortals.GetPortal(objTab.PortalID);
                        objRole = objRoleController.GetRoleByName(objPortal.PortalID, RoleName);
                        if (objRole != null)
                        {
                            RoleID = objRole.RoleID;
                        }
                        else
                        {
                            if (IsAdminTemplate && RoleName.ToLower() == "administrators")
                            {
                                RoleID = objPortal.AdministratorRoleId;
                            }
                        }
                        break;
                }
                if (RoleID != int.MinValue)
                {
                    objTabPermission = new Security.Permissions.TabPermissionInfo();
                    objTabPermission.TabID = objTab.TabID;
                    objTabPermission.PermissionID = PermissionID;
                    objTabPermission.RoleID = RoleID;
                    objTabPermission.AllowAccess = AllowAccess;
                    objTab.TabPermissions.Add(objTabPermission);
                }
            }
        }
        private static object GetTabsByPortalCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            List<TabInfo> tabs = CBO.FillCollection<TabInfo>(provider.GetTabs(portalID));
            return new TabCollection(tabs);
        }
        private static object GetTabPathDictionaryCallback(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            Dictionary<string, int> tabpathDic = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            IDataReader dr = DataProvider.Instance().GetTabPaths(portalID);
            try
            {
                while (dr.Read())
                {
                    tabpathDic[Null.SetNullString(dr["TabPath"])] = Null.SetNullInteger(dr["TabID"]);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return tabpathDic;
        }
        public static void CopyDesignToChildren(TabInfo parentTab, string skinSrc, string containerSrc)
        {
            CopyDesignToChildren(parentTab, skinSrc, containerSrc, PortalController.GetActivePortalLanguage(parentTab.PortalID));
        }
        public static void CopyDesignToChildren(TabInfo parentTab, string skinSrc, string containerSrc, string CultureCode)
        {
            bool clearCache = Null.NullBoolean;
            List<TabInfo> childTabs = new TabController().GetTabsByPortal(parentTab.PortalID).DescendentsOf(parentTab.TabID);
            foreach (TabInfo objTab in childTabs)
            {
                if (TabPermissionController.CanAdminPage(objTab))
                {
                    provider.UpdateTab(objTab.TabID, objTab.ContentItemId, objTab.PortalID, objTab.TabName, objTab.IsVisible, objTab.DisableLink, objTab.ParentId, objTab.IconFile, objTab.IconFileLarge, objTab.Title, objTab.Description,
                    objTab.KeyWords, objTab.IsDeleted, objTab.Url, skinSrc, containerSrc, objTab.TabPath, objTab.StartDate, objTab.EndDate, objTab.RefreshInterval, objTab.PageHeadText,
                    objTab.IsSecure, objTab.PermanentRedirect, objTab.SiteMapPriority, UserController.GetCurrentUserInfo().UserID, CultureCode);
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.TAB_UPDATED);
                    clearCache = true;
                }
            }
            if (clearCache)
            {
                DataCache.ClearTabsCache(childTabs[0].PortalID);
            }
        }
        public static void CopyPermissionsToChildren(TabInfo parentTab, TabPermissionCollection newPermissions)
        {
            Security.Permissions.TabPermissionController objTabPermissionController = new Security.Permissions.TabPermissionController();
            bool clearCache = Null.NullBoolean;
            List<TabInfo> childTabs = new TabController().GetTabsByPortal(parentTab.PortalID).DescendentsOf(parentTab.TabID);
            foreach (TabInfo objTab in childTabs)
            {
                if (TabPermissionController.CanAdminPage(objTab))
                {
                    objTab.TabPermissions.Clear();
                    objTab.TabPermissions.AddRange(newPermissions);
                    TabPermissionController.SaveTabPermissions(objTab);
                    clearCache = true;
                }
            }
            if (clearCache)
            {
                DataCache.ClearTabsCache(childTabs[0].PortalID);
            }
        }
        public static bool DeleteTab(int tabId, PortalSettings PortalSettings, int UserId)
        {
            bool bDeleted = true;
            TabController objController = new TabController();
            TabInfo objTab = objController.GetTab(tabId, PortalSettings.PortalId, false);
            if (objTab != null)
            {
                List<TabInfo> siblingTabs = objController.GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
                int siblingCount = siblingTabs.Count;
                objController.UpdateTabOrder(siblingTabs, 2);
                int tabIndex = objController.GetIndexOfTab(objTab, siblingTabs);
                bDeleted = DeleteTab(objTab, PortalSettings, UserId);
                objTab.TabOrder = 0;
                objController.UpdateTabOrder(objTab, false);
                objController.UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);
            }
            else
            {
                bDeleted = false;
            }
            return bDeleted;
        }
        public static void DeserializePanes(XmlNode nodePanes, int PortalId, int TabId, PortalTemplateModuleAction mergeTabs, Hashtable hModules)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;
            Dictionary<int, ModuleInfo> dicModules = objModules.GetTabModules(TabId);
            if (mergeTabs == PortalTemplateModuleAction.Replace)
            {
                foreach (KeyValuePair<int, ModuleInfo> kvp in dicModules)
                {
                    objModule = kvp.Value;
                    objModules.DeleteTabModule(TabId, objModule.ModuleID, false);
                }
            }
            foreach (XmlNode nodePane in nodePanes.ChildNodes)
            {
                if (nodePane.SelectSingleNode("modules") != null)
                {
                    foreach (XmlNode nodeModule in nodePane.SelectSingleNode("modules"))
                    {
                        ModuleController.DeserializeModule(nodeModule, nodePane, PortalId, TabId, mergeTabs, hModules);
                    }
                }
            }
        }
        public static TabInfo DeserializeTab(XmlNode nodeTab, TabInfo objTab, int PortalId, PortalTemplateModuleAction mergeTabs)
        {
            return TabController.DeserializeTab(nodeTab, objTab, new Hashtable(), PortalId, false, mergeTabs, new Hashtable());
        }
        public static TabInfo DeserializeTab(XmlNode nodeTab, TabInfo objTab, Hashtable hTabs, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, Hashtable hModules)
        {
            TabController objTabs = new TabController();
            string tabName = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "name");
            if (!String.IsNullOrEmpty(tabName))
            {
                if (objTab == null)
                {
                    objTab = new TabInfo();
                    objTab.TabID = Null.NullInteger;
                    objTab.ParentId = Null.NullInteger;
                    objTab.TabName = tabName;
                }
                objTab.PortalID = PortalId;
                objTab.Title = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "title");
                objTab.Description = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "description");
                objTab.KeyWords = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "keywords");
                objTab.IsVisible = XmlUtils.GetNodeValueBoolean(nodeTab, "visible", true);
                objTab.DisableLink = XmlUtils.GetNodeValueBoolean(nodeTab, "disabled");
                objTab.IconFile = Globals.ImportFile(PortalId, XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "iconfile"));
                objTab.IconFileLarge = Globals.ImportFile(PortalId, XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "iconfilelarge"));
                objTab.Url = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "url");
                objTab.StartDate = XmlUtils.GetNodeValueDate(nodeTab, "startdate", Null.NullDate);
                objTab.EndDate = XmlUtils.GetNodeValueDate(nodeTab, "enddate", Null.NullDate);
                objTab.RefreshInterval = XmlUtils.GetNodeValueInt(nodeTab, "refreshinterval", Null.NullInteger);
                objTab.PageHeadText = XmlUtils.GetNodeValue(nodeTab, "pageheadtext", Null.NullString);
                objTab.IsSecure = XmlUtils.GetNodeValueBoolean(nodeTab, "issecure", false);
                objTab.SiteMapPriority = XmlUtils.GetNodeValueSingle(nodeTab, "sitemappriority", (float)0.5);
                objTab.TabPermissions.Clear();
                DeserializeTabPermissions(nodeTab.SelectNodes("tabpermissions/permission"), objTab, IsAdminTemplate);
                DeserializeTabSettings(nodeTab.SelectNodes("tabsettings/tabsetting"), objTab);
                if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab, "skinsrc", "")))
                {
                    objTab.SkinSrc = XmlUtils.GetNodeValue(nodeTab, "skinsrc", "");
                }
                if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab, "containersrc", "")))
                {
                    objTab.ContainerSrc = XmlUtils.GetNodeValue(nodeTab, "containersrc", "");
                }
                tabName = objTab.TabName;
                if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")))
                {
                    if (hTabs[XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")] != null)
                    {
                        objTab.ParentId = Convert.ToInt32(hTabs[XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")]);
                        tabName = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent") + "/" + objTab.TabName;
                    }
                    else
                    {
                        TabInfo objParent = objTabs.GetTabByName(XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent"), PortalId);
                        if (objParent != null)
                        {
                            objTab.ParentId = objParent.TabID;
                            tabName = objParent.TabName + "/" + objTab.TabName;
                        }
                        else
                        {
                            objTab.ParentId = Null.NullInteger;
                            tabName = objTab.TabName;
                        }
                    }
                }
                if (objTab.TabID == Null.NullInteger)
                {
                    objTab.TabID = objTabs.AddTab(objTab);
                }
                else
                {
                    objTabs.UpdateTab(objTab);
                }
                if (hTabs[tabName] == null)
                {
                    hTabs.Add(tabName, objTab.TabID);
                }
            }
            if (nodeTab.SelectSingleNode("panes") != null)
            {
                DeserializePanes(nodeTab.SelectSingleNode("panes"), PortalId, objTab.TabID, mergeTabs, hModules);
            }
            nodeTab.AppendChild(XmlUtils.CreateElement(nodeTab.OwnerDocument, "tabid", objTab.TabID.ToString()));
            return objTab;
        }
        public static List<TabInfo> GetPortalTabs(int portalId, int excludeTabId, bool includeNoneSpecified, bool includeHidden)
        {
            return GetPortalTabs(GetTabsBySortOrder(portalId), excludeTabId, includeNoneSpecified, "<" + Localization.GetString("None_Specified") + ">", includeHidden, false, false, false, false);
        }
        public static List<TabInfo> GetPortalTabs(int portalId, int excludeTabId, bool includeNoneSpecified, bool includeHidden, bool includeDeleted, bool includeURL)
        {
            return GetPortalTabs(GetTabsBySortOrder(portalId), excludeTabId, includeNoneSpecified, "<" + Localization.GetString("None_Specified") + ">", includeHidden, includeDeleted, includeURL, false, false);
        }
        public static List<TabInfo> GetPortalTabs(int portalId, int excludeTabId, bool includeNoneSpecified, string NoneSpecifiedText, bool includeHidden, bool includeDeleted, bool includeURL, bool checkViewPermisison, bool checkEditPermission)
        {
            return GetPortalTabs(GetTabsBySortOrder(portalId), excludeTabId, includeNoneSpecified, NoneSpecifiedText, includeHidden, includeDeleted, includeURL, checkViewPermisison, checkEditPermission);
        }
        public static List<TabInfo> GetPortalTabs(List<TabInfo> tabs, int excludeTabId, bool includeNoneSpecified, string NoneSpecifiedText, bool includeHidden, bool includeDeleted, bool includeURL, bool checkViewPermisison, bool checkEditPermission)
        {
            List<TabInfo> listTabs = new List<TabInfo>();
            if (includeNoneSpecified)
            {
                TabInfo objTab = new TabInfo();
                objTab.TabID = -1;
                objTab.TabName = NoneSpecifiedText;
                objTab.TabOrder = 0;
                objTab.ParentId = -2;
                listTabs.Add(objTab);
            }
            foreach (TabInfo objTab in tabs)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                if (((excludeTabId < 0) || (objTab.TabID != excludeTabId)) && (!objTab.IsSuperTab || objUserInfo.IsSuperUser))
                {
                    if ((objTab.IsVisible == true || includeHidden == true) && (objTab.IsDeleted == false || includeDeleted == true) && (objTab.TabType == TabType.Normal || includeURL == true))
                    {
                        if (checkEditPermission || checkViewPermisison)
                        {
                            string permissionList = "ADD,COPY,EDIT,MANAGE";
                            if (checkEditPermission && TabPermissionController.HasTabPermission(objTab.TabPermissions, permissionList))
                            {
                                listTabs.Add(objTab);
                            }
                            else if (checkViewPermisison && TabPermissionController.CanViewPage(objTab))
                            {
                                listTabs.Add(objTab);
                            }
                        }
                        else
                        {
                            listTabs.Add(objTab);
                        }
                    }
                }
            }
            return listTabs;
        }
        public static int GetTabByTabPath(int portalId, string tabPath)
        {
            Dictionary<string, int> tabpathDic = GetTabPathDictionary(portalId);
            if (tabpathDic.ContainsKey(tabPath))
            {
                return tabpathDic[tabPath];
            }
            else
            {
                return -1;
            }
        }
        public static Dictionary<string, int> GetTabPathDictionary(int portalId)
        {
            string cacheKey = string.Format(DataCache.TabPathCacheKey, portalId.ToString());
            return CBO.GetCachedObject<Dictionary<string, int>>(new CacheItemArgs(cacheKey, DataCache.TabPathCacheTimeOut, DataCache.TabPathCachePriority, portalId), GetTabPathDictionaryCallback);
        }
        public static List<TabInfo> GetTabsByParent(int parentId, int portalId)
        {
            return new TabController().GetTabsByPortal(portalId).WithParentId(parentId);
        }
        public static List<TabInfo> GetTabsBySortOrder(int portalId)
        {
            return new TabController().GetTabsByPortal(portalId).AsList();
        }
        public static bool IsSpecialTab(int tabId, PortalSettings PortalSettings)
        {
            return tabId == PortalSettings.SplashTabId || tabId == PortalSettings.HomeTabId || tabId == PortalSettings.LoginTabId || tabId == PortalSettings.UserTabId || tabId == PortalSettings.AdminTabId || tabId == PortalSettings.SuperTabId;
        }
        public static void RestoreTab(TabInfo objTab, PortalSettings PortalSettings, int UserId)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            TabController objController = new TabController();
            objTab.IsDeleted = false;
            objController.UpdateTab(objTab);
            List<TabInfo> siblingTabs = objController.GetTabsByPortal(objTab.PortalID).WithParentId(objTab.ParentId);
            int siblingCount = siblingTabs.Count;
            objTab.TabOrder = 2 * siblingTabs.Count + 1;
            objController.UpdateTabOrder(objTab, false);
            objEventLog.AddLog(objTab, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.TAB_RESTORED);
            ModuleController objmodules = new ModuleController();
            ArrayList arrMods = objmodules.GetAllTabsModules(objTab.PortalID, true);
            foreach (ModuleInfo objModule in arrMods)
            {
                objmodules.CopyModule(objModule.ModuleID, objModule.TabID, objTab.TabID, "", true);
            }
            objController.ClearCache(objTab.PortalID);
        }
        public static XmlNode SerializeTab(XmlDocument xmlTab, TabInfo objTab, bool includeContent)
        {
            return SerializeTab(xmlTab, null, objTab, null, includeContent);
        }
        public static XmlNode SerializeTab(XmlDocument xmlTab, Hashtable hTabs, TabInfo objTab, PortalInfo objPortal, bool includeContent)
        {
            XmlNode nodeTab;
            XmlNode urlNode;
            XmlNode newnode;
            CBO.SerializeObject(objTab, xmlTab);
            nodeTab = xmlTab.SelectSingleNode("tab");
            nodeTab.Attributes.Remove(nodeTab.Attributes["xmlns:xsd"]);
            nodeTab.Attributes.Remove(nodeTab.Attributes["xmlns:xsi"]);
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("tabid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("moduleID"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("taborder"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("portalid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("parentid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("isdeleted"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("tabpath"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("haschildren"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("skindoctype"));
            foreach (XmlNode nodePermission in nodeTab.SelectNodes("tabpermissions/permission"))
            {
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("tabpermissionid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("permissionid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("tabid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("roleid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("userid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("username"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("displayname"));
            }
            urlNode = xmlTab.SelectSingleNode("tab/url");
            switch (objTab.TabType)
            {
                case TabType.Normal:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "Normal"));
                    break;
                case TabType.Tab:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "Tab"));
                    TabInfo tab = new TabController().GetTab(Int32.Parse(objTab.Url), objTab.PortalID, false);
                    urlNode.InnerXml = tab.TabPath;
                    break;
                case TabType.File:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "File"));
                    Services.FileSystem.FileInfo file = new Services.FileSystem.FileController().GetFileById(Int32.Parse(objTab.Url.Substring(7)), objTab.PortalID);
                    urlNode.InnerXml = file.RelativePath;
                    break;
                case TabType.Url:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "Url"));
                    break;
            }
            XmlUtils.SerializeHashtable(objTab.TabSettings, xmlTab, nodeTab, "tabsetting", "settingname", "settingvalue");
            if (objPortal != null)
            {
                if (objTab.TabID == objPortal.SplashTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "splashtab";
                    nodeTab.AppendChild(newnode);
                }
                else if (objTab.TabID == objPortal.HomeTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "hometab";
                    nodeTab.AppendChild(newnode);
                }
                else if (objTab.TabID == objPortal.UserTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "usertab";
                    nodeTab.AppendChild(newnode);
                }
                else if (objTab.TabID == objPortal.LoginTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "logintab";
                    nodeTab.AppendChild(newnode);
                }
            }
            if (hTabs != null)
            {
                if (!Null.IsNull(objTab.ParentId))
                {
                    newnode = xmlTab.CreateElement("parent");
                    newnode.InnerXml = HttpContext.Current.Server.HtmlEncode(hTabs[objTab.ParentId].ToString());
                    nodeTab.AppendChild(newnode);
                    hTabs.Add(objTab.TabID, hTabs[objTab.ParentId].ToString() + "/" + objTab.TabName);
                }
                else
                {
                    hTabs.Add(objTab.TabID, objTab.TabName);
                }
            }
            XmlNode nodePanes;
            XmlNode nodePane;
            XmlNode nodeName;
            XmlNode nodeModules;
            XmlNode nodeModule;
            XmlDocument xmlModule;
            ModuleInfo objmodule;
            ModuleController objmodules = new ModuleController();
            nodePanes = nodeTab.AppendChild(xmlTab.CreateElement("panes"));
            foreach (KeyValuePair<int, ModuleInfo> kvp in objmodules.GetTabModules(objTab.TabID))
            {
                objmodule = kvp.Value;
                if (!objmodule.IsDeleted)
                {
                    xmlModule = new XmlDocument();
                    nodeModule = ModuleController.SerializeModule(xmlModule, objmodule, includeContent);
                    if (nodePanes.SelectSingleNode("descendant::pane[name='" + objmodule.PaneName + "']") == null)
                    {
                        nodePane = xmlModule.CreateElement("pane");
                        nodeName = nodePane.AppendChild(xmlModule.CreateElement("name"));
                        nodeName.InnerText = objmodule.PaneName;
                        nodePane.AppendChild(xmlModule.CreateElement("modules"));
                        nodePanes.AppendChild(xmlTab.ImportNode(nodePane, true));
                    }
                    nodeModules = nodePanes.SelectSingleNode("descendant::pane[name='" + objmodule.PaneName + "']/modules");
                    nodeModules.AppendChild(xmlTab.ImportNode(nodeModule, true));
                }
            }
            return nodeTab;
        }
    }
}
