using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace ShopCartLibrary.Orders
{
    public class TransactionMethods
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_GetAll]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetActiveList()
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_GetActiveList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailsByID(int TransactionMethod_ID)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_GetDetailsByID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TransactionMethod_ID", TransactionMethod_ID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByDiscontinued(string Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_GetListByDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string TransactionMethod_Name, double TransactionMethod_Fee, string Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TransactionMethod_Name", TransactionMethod_Name);
            cmd.Parameters.AddWithValue("@TransactionMethod_Fee", TransactionMethod_Fee);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int TransactionMethod_ID, string TransactionMethod_Name, double TransactionMethod_Fee, string Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TransactionMethod_ID", TransactionMethod_ID);
            cmd.Parameters.AddWithValue("@TransactionMethod_Name", TransactionMethod_Name);
            cmd.Parameters.AddWithValue("@TransactionMethod_Fee", TransactionMethod_Fee);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateDiscontinued(int TransactionMethod_ID, string Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_UpdateDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TransactionMethod_ID", TransactionMethod_ID);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int TransactionMethod_ID)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[TransactionMethods_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TransactionMethod_ID", TransactionMethod_ID);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
