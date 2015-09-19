using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Entities.Tabs;
namespace WebApp.portals.news.controls
{
    public partial class uc_header_menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string XSLT_FilePath = "~/portals/news/controls/XsltTransformer.xslt";
                TabMenu admin_menu = new TabMenu();
                int portalid = 0;
                string result = admin_menu.ExecuteXSLTransformation_FrontPage(portalid, XSLT_FilePath, 0);
                if (result != string.Empty)
                {
                    Literal_Menu.Text = result;
                }        

            }
        }
    }
}