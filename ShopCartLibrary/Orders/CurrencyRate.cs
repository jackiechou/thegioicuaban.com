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
    public class CurrencyRate
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public CurrencyRate()
        {
        }

        public DataTable GetLists()
        {
            SqlCommand cmd = new SqlCommand("[Sales].[CurrencyRate_GetList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailsByCurrencyCode(string FromCurrencyCode, string ToCurrencyCode)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[CurrencyRate_GetDetailsByCurrencyCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@FromCurrencyCode", FromCurrencyCode);
            cmd.Parameters.AddWithValue("@ToCurrencyCode", ToCurrencyCode);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int CurrencyRateID)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[CurrencyRate_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CurrencyRateID", CurrencyRateID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string FromCurrencyCode, string ToCurrencyCode, string AverageRate, string EndOfDayRate)
        {
            AverageRate = RemoveExtraText(AverageRate);
            SqlCommand cmd = new SqlCommand("[Sales].[CurrencyRate_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@FromCurrencyCode", FromCurrencyCode);
            cmd.Parameters.AddWithValue("@ToCurrencyCode", ToCurrencyCode);
            cmd.Parameters.AddWithValue("@AverageRate", AverageRate);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int CurrencyRateID, string FromCurrencyCode, string ToCurrencyCode, string AverageRate, string EndOfDayRate)
        {
            AverageRate = RemoveExtraText(AverageRate);
            EndOfDayRate = RemoveExtraText(EndOfDayRate);

            SqlCommand cmd = new SqlCommand("[Sales].[CurrencyRate_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CurrencyRateID", CurrencyRateID);
            cmd.Parameters.AddWithValue("@FromCurrencyCode", FromCurrencyCode);
            cmd.Parameters.AddWithValue("@ToCurrencyCode", ToCurrencyCode);
            cmd.Parameters.AddWithValue("@AverageRate", AverageRate);
            cmd.Parameters.AddWithValue("@EndOfDayRate", EndOfDayRate);
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
