using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Data;
using System.Data.SqlClient;
using CommonLibrary.Entities.Tabs;

public partial class user_controls_admin_controls_menu_horizontal_menu_horizontal_menu : System.Web.UI.UserControl
{
    #region SESSION PROPERTIES ===========================
    protected string PortalId
    {
        get
        {
            if (Session["PortalId"] != null)
                return Session["PortalId"].ToString();
            else
                return string.Empty;
        }
    }
    protected string UserId
    {
        get
        {
            if (Session["UserId"] != null)
                return Session["UserId"].ToString();
            else
                return string.Empty;
        }
    }

    protected string RoleId
    {
        get
        {
            if (Session["RoleId"] != null)
                return Session["RoleId"].ToString();
            else
                return string.Empty;
        }
    }
    #endregion ===========================================

    protected void Page_Load(object sender, EventArgs e)
    { 
        if (!Page.IsPostBack)
        {
            if (PortalId != string.Empty && RoleId != string.Empty)
            {
                string XSLT_FilePath = "~/modules/menu/XsltTransformer.xslt";
                TabMenu admin_menu = new TabMenu();
                string result = admin_menu.ExecuteXSLTransformation(Convert.ToInt32(PortalId), RoleId, XSLT_FilePath, 1);
                if (result != string.Empty)
                    Literal_Menu.Text = result;
                else
                    Literal_Menu.Text = "Không tìm thấy dữ liệu";
            }
            else
            {
                string scriptCode = "<script>alert('Session Timeout.');window.location.href='/login/'</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
            }
        }    
    }    
}