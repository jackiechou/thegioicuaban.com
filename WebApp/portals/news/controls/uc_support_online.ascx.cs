using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ArticleLibrary;
using System.Data;

namespace WebApp.portals.news.controls
{
    public partial class uc_support_online : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetSupport("SUPPORT");
            }
        }

        private void GetSupport(string code)
        {
            var query = ArticleCategoryController.GetDetailByCode(code);
            divSupport.InnerHtml = query.Description;
        }
    }
}