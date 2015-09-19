using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Entities.Content
{
    [Serializable()]
    public class ContentType : IHydratable
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();    

        #region "Private Members"

        private int _ContentTypeId;

        private string _ContentType;
        #endregion

        #region "Constructors"

        public ContentType()
            : this(Null.NullString)
        {
        }

        public ContentType(string scopeType)
        {
            _ContentTypeId = Null.NullInteger;
            _ContentType = scopeType;
        }

        #endregion

        #region "Public Properties"

        public int ContentTypeId
        {
            get { return _ContentTypeId; }
            set { _ContentTypeId = value; }
        }

        public string Type
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        #endregion

        #region "IHydratable Implementation"

        public void Fill(System.Data.IDataReader dr)
        {
            ContentTypeId = Null.SetNullInteger(dr["ContentTypeID"]);
            Type = Null.SetNullString(dr["ContentType"]);
        }

        public int KeyID
        {
            get { return ContentTypeId; }
            set { ContentTypeId = value; }
        }

        #endregion

        public override string ToString()
        {
            return Type;
        }

        #region Methods =================================
        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentTypes_GetAll", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int ContentTypeId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentTypes_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentTypeId", ContentTypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string ContentType)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentTypes_Add", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentType", ContentType);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int ContentTypeId, string ContentType)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentTypes_Edit", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentTypeId", ContentTypeId);
            cmd.Parameters.AddWithValue("@ContentType", ContentType);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int ContentTypeId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ContentTypes_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContentTypeId", ContentTypeId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        #endregion ======================================
    }
}
