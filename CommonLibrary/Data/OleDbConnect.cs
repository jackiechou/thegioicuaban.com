using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;

namespace CommonLibrary.DataAccess
{
    public class OleDbConnect
    {
        OleDbConnection objConn;
        DataSet DS = new DataSet();
        DataTable DT = new DataTable();
        public OleDbConnect() { }

        public void Connect(string strConnect)
        {
            try
            {
                objConn = new OleDbConnection(strConnect);
                objConn.Open();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        public void Disconnect()
        {
            try
            {
                objConn.Close();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        public OleDbConnection Connection
        {
            set
            {
                objConn = value;
            }
            get
            {
                return objConn;
            }
        }

        public DataSet GetDataSet(string strSQL)
        {
            OleDbDataAdapter objData = new OleDbDataAdapter(strSQL, objConn);
            DataSet DS = new DataSet();
            try
            {
                objData.Fill(DS);
                Disconnect();
            }
            catch (Exception ex)
            {
                Disconnect();
                HttpContext.Current.Response.Write(ex.ToString());
            }
            return DS;
        }

        public OleDbDataReader ExecReader(string query)
        {

            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
            objComm.CommandType = CommandType.Text;
            objComm.CommandText = query;
            OleDbDataReader reader;
            reader = objComm.ExecuteReader();
            return reader;

        }

        public void ExecNoneQuery(string spName)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
            objComm.CommandType = CommandType.StoredProcedure;
            objComm.CommandText = spName;
            try
            {
                objComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        public void ExecNoneQuery(string spName, params OleDbParameter[] param)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
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
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        public OleDbCommand ExecNoneQueryWithOutput(string spName)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
            objComm.CommandType = CommandType.StoredProcedure;
            objComm.CommandText = spName;
            try
            {
                objComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
            return objComm;
        }

        public OleDbCommand ExecNoneQueryWithOutput(string spName, params OleDbParameter[] param)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
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
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
            return objComm;
        }

        public DataTable ExecQuery(string strQuery)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
            objComm.CommandType = CommandType.Text;
            objComm.CommandText = strQuery;
            OleDbDataAdapter objData = new OleDbDataAdapter(objComm);
            DataTable DT = new DataTable();
            try
            {
                objData.Fill(DT);
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.ToString());
            }
            Disconnect();
            return DT;
        }

        public DataTable ExecQuery(string spName, params OleDbParameter[] param)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
            objComm.CommandType = CommandType.StoredProcedure;
            objComm.CommandText = spName;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] != null)
                    objComm.Parameters.Add(param[i]);
            }
            OleDbDataAdapter objData = new OleDbDataAdapter(objComm);
            DataTable DT = new DataTable();
            try
            {
                objData.Fill(DT);
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.ToString());
            }
            Disconnect();
            return DT;
        }

        public string ExeScalar(string spName)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
            objComm.CommandType = CommandType.StoredProcedure;
            objComm.CommandText = spName;
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

        public string ExeScalar(string spName, params OleDbParameter[] param)
        {
            OleDbCommand objComm = new OleDbCommand();
            objComm.Connection = objConn;
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

    }

}
