using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using ArticleLibrary;
using System.Data;

namespace WebApp.modules.admin.articles
{
    public partial class admin_article_comments_edit : System.Web.UI.Page
    {
        private int _idx
        {
            get
            {
                if (ViewState["idx"] == null)
                    ViewState["idx"] = -1;
                return (int)ViewState["idx"];
            }
            set
            {
                ViewState["idx"] = value;
            }
        }

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Theme = "default";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                _idx = Convert.ToInt32(Request.QueryString["idx"]);
                LoadData();
                MultiView1.ActiveViewIndex = 0;
            }
        }

        #region Status ============================================================
        protected void LoadStatus2RadioBtnList(string selected_value)
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Published", "2"));
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlStatus.DataSource = lstColl;
            rdlStatus.DataTextField = "Text";
            rdlStatus.DataValueField = "Value";
            rdlStatus.DataBind();
            rdlStatus.SelectedValue = selected_value;
            rdlStatus.AutoPostBack = false;
        }
        #endregion ================================================================

        protected void LoadData()
        {
            ArticleCommentController comment_obj = new ArticleCommentController();
            DataTable dt = comment_obj.GetDetailById(_idx);
            if (dt.Rows.Count > 0)
            {
                txtArticle.Text = dt.Rows[0]["Headline"].ToString();
                txtName.Text = dt.Rows[0]["CommentName"].ToString();
                txtEmail.Text = dt.Rows[0]["CommentEmail"].ToString();
                txtCommentText.Text = dt.Rows[0]["CommentText"].ToString();
                LoadStatus2RadioBtnList(dt.Rows[0]["Publish"].ToString().Trim());
            }
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            ArticleCommentController comment_obj = new ArticleCommentController();
            string user_id = Session["UserId"].ToString();
            string status = rdlStatus.SelectedValue;
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = comment_obj.UpdateStatus(user_id, _idx, status);
                if (i == -1)
                {
                    lblErrorMsg.Text = "Thông tin không đầy đủ";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 0;
                }
                else if (i == -2)
                {
                    lblErrorMsg.Text = "Tiến trình xử lý bị lỗi";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 2;
                }
                else if (i == -3)
                {
                    lblErrorMsg.Text = "Dữ liệu không tồn tại";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 2;
                }
                else if (i == 1)
                {
                    lblResult.Text = "Cập nhật thành công";
                    MultiView1.ActiveViewIndex = 1;
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 2;
                }
            }
        }
    }
}