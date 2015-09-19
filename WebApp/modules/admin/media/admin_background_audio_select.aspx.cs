using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using MediaLibrary;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.media
{
    public partial class admin_background_audio_select : System.Web.UI.Page
    {
        

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            Page.Culture = "vi-VN";
            Page.UICulture = "vi";
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (!Page.IsPostBack)
            {
                LoadSelectedBackgroundMusic();
                FillDataInList();
                MultiView1.ActiveViewIndex = 0;
            }    
        }

        protected void LoadSelectedBackgroundMusic()
        {
            MediaFiles media_obj = new MediaFiles();
            DataTable dt = new DataTable();
            dt = media_obj.GetSelectedItem(1);
            string result = string.Empty;
            if (dt.Rows.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<object type=\"application/x-shockwave-flash\" data=\"../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=../../../user_files/multimedia/audio/background_audio/" + dt.Rows[0]["FileName"].ToString() + "\" width=\"240\" height=\"20\" id=\"dewplayer-vol\"><param name=\"wmode\" value=\"transparent\" /><param name=\"movie\" value=\"../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=../../../user_files/multimedia/audio/background_audio/" + dt.Rows[0]["FileName"].ToString() + "\" /></object>");
                result = sb.ToString();
            }
            else
            {
                result = "Không có ";
            }

            Ltr_SeletedItem.Text = result;
        }

        #region Portal List ==================================================
        private void PopulatePortalList2DDL()
        {
            PortalController portal_obj = new PortalController();
            DataTable dtNodes = portal_obj.GetList();

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dtNodes;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedIndex = 0;
            ddlPortalList.AutoPostBack = true;
        }
        protected void ddlPortalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInList();
        }
        #endregion ===============================================================

        #region Media Types ======================================================
        protected void PopulateCulture2DDL()
        {
            //MediaTypes media_type_obj = new MediaTypes();
            //DataTable dt = media_type_obj.GetList();

            //ddlMediaTypeList.Items.Clear();
            //ddlMediaTypeList.DataSource = dt;
            //ddlMediaTypeList.DataTextField = "CultureName";
            //ddlMediaTypeList.DataValueField = "CultureId";
            //ddlMediaTypeList.DataBind();
            //ddlMediaTypeList.SelectedIndex = 0;
            //ddlMediaTypeList.AutoPostBack = true;
        }
        protected void ddlCultureList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInList();
        }
        #endregion ============================================================ 

        private void FillDataInList()
        {
            //int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            //int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            //string Status = "1";
            //MediaClass media_obj = new MediaClass();
            //DataTable dt = new DataTable();
            //dt = media_obj.GetListByPortalIdTypeIdStatus(PortalId, TypeId,Status);
            //ListView1.DataSource = dt;
            //ListView1.DataBind();
        }
      
        protected void ListView1_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (String.Equals(e.CommandName, "UpdateStatus"))
            {
                // Verify that the employee ID is not already in the list. If not, add the
                // employee to the list.
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;
                int id = Convert.ToInt32(ListView1.DataKeys[dataItem.DisplayIndex].Value.ToString());               
                string userid = Session["UserId"].ToString();
                MediaFiles media_obj = new MediaFiles();
                int i = media_obj.SelectItem(userid, id);
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
                    lblErrorMsg.Text = "Dữ liệu đã tồn tại";
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