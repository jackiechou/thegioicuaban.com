using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace CommonLibrary.Common
{
    public class CountryClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public CountryClass() {}

        public DataTable GetList()
        {
            SqlCommand cmd = new SqlCommand("[aspnet_Countries_GetList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }
    }
}
