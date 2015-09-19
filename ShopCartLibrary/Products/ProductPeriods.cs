using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using CommonLibrary;
using System.Data.SqlClient;

namespace ShopCartLibrary.Products
{
    public class ProductPeriods
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductPeriods(){}

        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_GetAll]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetPublishedList()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_GetPublishedList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByDiscontinued(string Period_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_GetListByDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Period_Discontinued", Period_Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }


        public DataTable GetDetails(int Period_ID)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Period_ID", Period_ID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string Period_Name, int Period_Duration, string Period_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Period_Name", Period_Name);
            cmd.Parameters.AddWithValue("@Period_Duration", Period_Duration);
            cmd.Parameters.AddWithValue("@Period_Discontinued", Period_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int Period_ID, string Period_Name, int Period_Duration, string Period_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Period_ID", Period_ID);
            cmd.Parameters.AddWithValue("@Period_Name", Period_Name);
            cmd.Parameters.AddWithValue("@Period_Duration", Period_Duration);
            cmd.Parameters.AddWithValue("@Period_Discontinued", Period_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateDiscontinued(int Period_ID, string Period_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_UpdateDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Period_ID", Period_ID);
            cmd.Parameters.AddWithValue("@Period_Discontinued", Period_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int Period_ID)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Period_ID", Period_ID);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        private string RemoveExtraText(string value)
        {
            //var allowedChars = "01234567890.,";
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }        
}
