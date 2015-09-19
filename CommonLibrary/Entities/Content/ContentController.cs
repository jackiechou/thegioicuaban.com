using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Data;
using CommonLibrary.Entities.Users;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Content.Data;
using CommonLibrary.Entities.Content.Common;
using System.Data.SqlClient;

namespace CommonLibrary.Entities.Content
{
    public class ContentController : IContentController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        #region "Private Members"


        private IDataService _DataService;
        #endregion

        #region "Constructors"

        public ContentController()
            : this(Util.GetDataService())
        {
        }

        public ContentController(IDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion


        #region Methods ==========================================================================================
        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentItems_GetAll", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByContentTypeId(int ContentTypeId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentItems_GetListByContentTypeId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentTypeId", ContentTypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int ContentItemId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentItems_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentItemId", ContentItemId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Add(int ContentTypeId, string Content, string ContentKey, bool Indexed)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentItems_Add", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentTypeId", ContentTypeId);
            cmd.Parameters.AddWithValue("@ContentKey", ContentKey);
            cmd.Parameters.AddWithValue("@Content", Content);
            cmd.Parameters.AddWithValue("@Indexed", Indexed);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Edit(int ContentItemId, int ContentTypeId, string Content, string ContentKey, bool Indexed)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentItems_Edit", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentItemId", ContentItemId);
            cmd.Parameters.AddWithValue("@ContentTypeId", ContentTypeId);
            cmd.Parameters.AddWithValue("@ContentKey", ContentKey);
            cmd.Parameters.AddWithValue("@Content", Content);
            cmd.Parameters.AddWithValue("@Indexed", Indexed);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int ContentItemId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentItems_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentItemId", ContentItemId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        #endregion ===============================================================================================

        #region "Public Methods"

        public int AddContentItem(ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);

            contentItem.ContentItemId = _DataService.AddContentItem(contentItem, UserController.GetCurrentUserInfo().UserID);

            return contentItem.ContentItemId;
        }

        public void DeleteContentItem(ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);

            _DataService.DeleteContentItem(contentItem);
        }

        public ContentItem GetContentItem(int contentItemId)
        {
            //Argument Contract
            Requires.NotNegative("contentItemId", contentItemId);

            return CBO.FillObject<ContentItem>(_DataService.GetContentItem(contentItemId));
        }

        public IQueryable<ContentItem> GetContentItemsByTerm(string term)
        {
            //Argument Contract
            Requires.NotNullOrEmpty("term", term);

            return CBO.FillQueryable<ContentItem>(_DataService.GetContentItemsByTerm(term));
        }

        public IQueryable<ContentItem> GetUnIndexedContentItems()
        {
            return CBO.FillQueryable<ContentItem>(_DataService.GetUnIndexedContentItems());
        }

        public void UpdateContentItem(ContentItem contentItem)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);

            _DataService.UpdateContentItem(contentItem, UserController.GetCurrentUserInfo().UserID);
        }

        public void AddMetaData(ContentItem contentItem, string name, string value)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);
            Requires.NotNullOrEmpty("name", name);

            _DataService.AddMetaData(contentItem, name, value);
        }
        public void DeleteMetaData(ContentItem contentItem, string name, string value)
        {
            //Argument Contract
            Requires.NotNull("contentItem", contentItem);
            Requires.PropertyNotNegative("contentItem", "ContentItemId", contentItem.ContentItemId);
            Requires.NotNullOrEmpty("name", name);

            _DataService.DeleteMetaData(contentItem, name, value);
        }
        public NameValueCollection GetMetaData(int contentItemId)
        {
            //Argument Contract
            Requires.NotNegative("contentItemId", contentItemId);

            NameValueCollection metadata = new NameValueCollection();
            IDataReader dr = _DataService.GetMetaData(contentItemId);
            while (dr.Read())
            {
                metadata.Add(dr.GetString(0), dr.GetString(1));
            }

            return metadata;
        }

        #endregion

    }
}
