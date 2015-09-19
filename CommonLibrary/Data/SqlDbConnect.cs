using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;

namespace CommonLibrary.DataAccess
{
    public class SqlDbConnect
    {
        private SqlConnection conn;
        public SqlCommand sqlCommand;

        public SqlDbConnect() { }

        public SqlConnection GetConnection
        {
            set
            {
                conn = value;
            }
            get
            {
                return this.conn;
            }
        }

        public void Connect(string strConnect)
        {
            try
            {
                conn = new SqlConnection(strConnect);

                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                sqlCommand = conn.CreateCommand();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        public bool Disconnect()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }

        public DataTable GetDataTable(string cmd)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd, this.GetConnection);
                da.Fill(dt);

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
            return dt;
        }

        public DataSet GetDataSet(string strCommand)
        {
            sqlCommand.CommandText = strCommand;
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(strCommand, this.GetConnection);
                da.Fill(ds);

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
            return ds;
        }

        public int ExecuteCommand(string strCommand)
        {
            int i = -1;
            try
            {
                SqlCommand cmd = new SqlCommand(strCommand, this.conn);
                i = cmd.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return i;
            }
            return i;
        }

        public int ExecuteNonQuery(string queryName, CommandType commandType, string[] parameterName, object[] parameterValue)
        {
            int x = -1;
            try
            {
                SqlCommand sqlCommand = new SqlCommand(queryName, this.GetConnection);
                sqlCommand.CommandType = commandType;
                for (int i = 0; i < parameterName.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue(parameterName[i], parameterValue[i]);
                }
                x = sqlCommand.ExecuteNonQuery();

            }
            catch (Exception exc)
            {
                x = -1;
                Console.WriteLine(exc.Message);
            }
            return x;
        }

        public SqlDataReader ExecuteReader(string strsql)
        {
            sqlCommand.CommandText = strsql;
            return sqlCommand.ExecuteReader();
        }

        public object ExecuteScalar(string strsql)
        {
            sqlCommand.CommandText = strsql;
            return sqlCommand.ExecuteScalar();
        }

        public int ExecuteStoreProcedure(string sp_Name, params object[] pars)
        {
            int x = -1;
            try
            {
                SqlCommand cmd = new SqlCommand(sp_Name, this.conn);
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < pars.Length; i += 2)
                {
                    SqlParameter par = new SqlParameter(pars[i].ToString(), pars[i + 1]);
                    cmd.Parameters.Add(par);
                }
                x = cmd.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return x;
            }
            return x;
        }

        public int CheckPrimaryKey(string queryName, CommandType commandType, string[] parameterName, object[] parameterValue)
        {
            int isDuplicate = -1;
            try
            {
                SqlCommand sqlCommand = new SqlCommand(queryName, this.GetConnection);
                sqlCommand.CommandType = commandType;
                for (int i = 0; i < parameterName.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue(parameterName[i], parameterValue[i]);
                }
                isDuplicate = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                isDuplicate = -1;
                ex.Message.ToString();
            }
            return isDuplicate;
        }

        public int GetMaxRequestLength()
        {
            // presume that the section is not defined in the web.config
            int defaultSize = 4096;

            System.Web.Configuration.HttpRuntimeSection section = System.Configuration.ConfigurationManager.GetSection("system.web/httpRuntime") as System.Web.Configuration.HttpRuntimeSection;
            if (section != null) defaultSize = section.MaxRequestLength;

            return defaultSize;
        }
    }
}
