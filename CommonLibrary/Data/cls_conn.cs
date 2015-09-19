using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CommonLibrary;

public class cls_conn
{
    private string strConnect = Settings.ConnectionString;
    private SqlConnection objConn;

    public cls_conn()
	{
        objConn = new SqlConnection(strConnect);
        objConn.Open();
	}

    public SqlConnection Connection
    {
        get
        {
            return objConn;
        }
        set { }
    }

    public DataSet GetDataSet(string strSql)
    {
        SqlDataAdapter Data = new SqlDataAdapter(strSql, objConn);
        DataSet DS = new DataSet();
        Data.Fill(DS);
        return DS;
    }

    public void Close()
    {
        objConn.Close();
    }
}
