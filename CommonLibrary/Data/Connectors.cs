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
    public class Connectors
    {
        protected string strConnect = Settings.ConnectionString;
        private SqlConnection conn;
        public SqlCommand sqlCommand;

        public Connectors() { }

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

        public void Connect()
        {
            try
            {
                conn = new SqlConnection(strConnect);

                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandTimeout = this.conn.ConnectionTimeout;
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
            sqlCommand.CommandTimeout = this.conn.ConnectionTimeout;
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
                sqlCommand.CommandTimeout = this.conn.ConnectionTimeout;
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
                sqlCommand.CommandTimeout = this.conn.ConnectionTimeout;
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

        public SqlCommand ExecNoneQueryWithOutput(string spName, params SqlParameter[] param)
        {
            Connect();
            SqlCommand objComm = new SqlCommand();
            objComm.Connection = this.conn;
            objComm.CommandType = CommandType.StoredProcedure;
            objComm.CommandText = spName;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] != null)
                    objComm.Parameters.Add(param[i]);
            }
            try
            {
                objComm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.ToString());
            }
            Disconnect();
            return objComm;
        }

        public SqlDataReader ExecuteReader(string strsql)
        {
            sqlCommand.CommandText = strsql;
            return sqlCommand.ExecuteReader();
        }

        public object ExecuteScalar(string strsql)
        {
            Connect();
            SqlCommand sqlCommand = new SqlCommand(strsql, this.GetConnection);
            sqlCommand.CommandType = CommandType.Text;
            return sqlCommand.ExecuteScalar();
        }

        public string ExecScalar(string spName, params SqlParameter[] param)
        {
            Connect();
            SqlCommand objComm = new SqlCommand();
            objComm.Connection = this.conn;
            objComm.CommandType = CommandType.StoredProcedure;
            objComm.CommandText = spName;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] != null)
                    objComm.Parameters.Add(param[i]);
            }
            string strRe = "";
            try
            {
                strRe = objComm.ExecuteScalar().ToString();
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.ToString());
            }
            Disconnect();
            return strRe;
        }

        public int ExecuteStoredProcedure(string spName)
        {
            Connect();
            SqlCommand command = new SqlCommand();
            command.Connection = this.conn;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            // Return value as parameter
            SqlParameter returnValue = new SqlParameter("returnVal", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(returnValue);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.ToString());
            }
            Disconnect();

            return Convert.ToInt32(returnValue.Value);
        }

        public void ExecuteStoreProcedure(string spName, params SqlParameter[] param)
        {
            Connect();
            SqlCommand objComm = new SqlCommand();
            objComm.Connection = this.conn;
            objComm.CommandType = CommandType.StoredProcedure;
            objComm.CommandText = spName;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] != null)
                    objComm.Parameters.Add(param[i]);
            }
            try
            {
                objComm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.ToString());
            }
            Disconnect();
        }

        public int ExecuteStoreProcedure(string sp_Name, params object[] pars)
        {
            int x = -1;
            try
            {
                SqlCommand cmd = new SqlCommand(sp_Name, this.conn);
                sqlCommand.CommandTimeout = this.conn.ConnectionTimeout;
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
                sqlCommand.CommandTimeout = this.conn.ConnectionTimeout;
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
