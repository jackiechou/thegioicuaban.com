﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Modules;
using System.Collections;
using CommonLibrary.UI.Skins;
using CommonLibrary.Application;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.ui
{
    public partial class admin_skins_edit : System.Web.UI.Page
    {
        public UIMode.mode _mode
        {
            get
            {
                if (ViewState["mode"] == null)
                    ViewState["mode"] = new UIMode.mode();
                return (UIMode.mode)ViewState["mode"];
            }
            set
            {
                ViewState["mode"] = value;
            }
        }

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
            Page.Title = "VASS";
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
                string qsuimode = Request.QueryString["mode"];

                if (string.IsNullOrEmpty(qsuimode) == false)
                {
                    _mode = (UIMode.mode)Enum.Parse(typeof(UIMode.mode), qsuimode);
                    if (_mode == UIMode.mode.add)
                    {
                        LoadApplicationList2DDL();
                        PopulatePortalList2DDL();
                        LoadSkinTypeList2DDL();
                        LoadPackageList2DDL();
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Convert.ToInt32(Request.QueryString["idx"]);
                        LoadData();
                    }
                    hdnWindowUIMODE.Value = _mode.ToString();
                }

                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Application List =================================================================
        private void LoadApplicationList2DDL()
        {
            ApplicationController obj_data = new ApplicationController();
            DataTable dtNodes = obj_data.GetApps();

            ddlApplicationList.Items.Clear();
            ddlApplicationList.DataSource = dtNodes;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            ddlApplicationList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlApplicationList.SelectedIndex = 0;
        }
        private void LoadApplicationList2DDL(string selected_value)
        {
            ApplicationController obj_data = new ApplicationController();
            DataTable dtNodes = obj_data.GetApps();

            ddlApplicationList.Items.Clear();
            ddlApplicationList.DataSource = dtNodes;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            ddlApplicationList.Items.Insert(0, new ListItem("- Chọn -", ""));
            ddlApplicationList.SelectedValue = selected_value;
        }       
        #endregion =====================================================================

        #region Portal List ==================================================
        private void PopulatePortalList2DDL()
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            PortalController portal_obj = new PortalController();
            DataTable dtNodes = portal_obj.GetListByApplicationId(ApplicationId);

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dtNodes;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedIndex = 0;
            ddlPortalList.AutoPostBack = true;
        }
        private void PopulatePortalList2DDL(string selected_value)
        {
            PortalController portal_obj = new PortalController();
            DataTable dtNodes = portal_obj.GetList();

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dtNodes;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedValue = selected_value;
        }
        #endregion ===============================================================

        #region Skin Type List =================================================================
        //public static void BindToEnum(Type enumType, ListControl lc)
        //{
        //    // get the names from the enumeration
        //    string[] names = Enum.GetNames(enumType);
        //    // get the values from the enumeration
        //    Array values = Enum.GetValues(enumType);
        //    // turn it into a hash table
        //    Hashtable ht = new Hashtable();
        //    for (int i = 0; i < names.Length; i++)
        //        // note the cast to integer here is important
        //        // otherwise we'll just get the enum string back again
        //        ht.Add(names[i], (int)values.GetValue(i));
        //    // return the dictionary to be bound to
        //    lc.DataSource = ht;
        //    lc.DataTextField = "Key";
        //    lc.DataValueField = "Value";
        //    lc.DataBind();
        //}
        //BindToEnum(typeof(NewsType), DropDownList1);
        //BindToEnum(typeof(NewsType), CheckBoxList1);
        //BindToEnum(typeof(NewsType), RadoButtonList1);

        protected void LoadSkinTypeList2DDL()
        {
            Hashtable ht = GetEnumForBind(typeof(SkinType));
            ddlSkinTypeList.DataSource = ht;
            ddlSkinTypeList.DataTextField = "value";
            ddlSkinTypeList.DataValueField = "key";
            ddlSkinTypeList.DataBind();
            ddlSkinTypeList.SelectedIndex = 0;
            ddlSkinTypeList.AutoPostBack = true;
        }

        protected void LoadSkinTypeList2DDL(string selected_value)
        {
            Hashtable ht = GetEnumForBind(typeof(SkinType));
            ddlSkinTypeList.DataSource = ht;
            ddlSkinTypeList.DataTextField = "value";
            ddlSkinTypeList.DataValueField = "key";
            ddlSkinTypeList.DataBind();
            ddlSkinTypeList.SelectedIndex = 0;
            ddlSkinTypeList.AutoPostBack = true;
        }

        public Hashtable GetEnumForBind(Type enumeration)
        {
            string[] names = Enum.GetNames(enumeration);
            Array values = Enum.GetValues(enumeration);
            Hashtable ht = new Hashtable();
            for (int i = 0; i < names.Length; i++)
            {
                ht.Add(Convert.ToInt32(values.GetValue(i)).ToString(), names[i]);
            }
            return ht;
        }

        protected void ddlSkinTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPackageList2DDL();
        }
        #endregion =====================================================================


        #region PackageList =================================================================
        private void LoadPackageList2DDL()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            string SkinType = ddlSkinTypeList.SelectedValue;
            SkinPackages obj_data = new SkinPackages();
            DataTable dtNodes = obj_data.GetListByPortalIdSkinType(PortalId, SkinType);

            ddlPackageList.Items.Clear();
            ddlPackageList.DataSource = dtNodes;
            ddlPackageList.DataTextField = "SkinName";
            ddlPackageList.DataValueField = "SkinPackage_ID";
            ddlPackageList.DataBind();
            ddlPackageList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPackageList.SelectedIndex = 0;
        }

        private void LoadPackageList2DDL(string selected_value)
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            string SkinType = ddlSkinTypeList.SelectedValue;
            SkinPackages obj_data = new SkinPackages();
            DataTable dtNodes = obj_data.GetListByPortalIdSkinType(PortalId, SkinType);

            ddlPackageList.Items.Clear();
            ddlPackageList.DataSource = dtNodes;
            ddlPackageList.DataTextField = "SkinName";
            ddlPackageList.DataValueField = "SkinPackage_ID";
            ddlPackageList.DataBind();
            ddlPackageList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPackageList.SelectedValue = selected_value;
        }
        #endregion =====================================================================

        private void LoadData()
        {
            Skin skin_obj = new Skin();
            DataTable dt = skin_obj.GetDetails(_idx);

            string ControlSrc = dt.Rows[0]["ControlSrc"].ToString();
            txtControlSrc.Text = ControlSrc;
            txtControlSrc.Visible = true;

            string ApplicationId = dt.Rows[0]["ApplicationId"].ToString();
            LoadApplicationList2DDL(ApplicationId);

            string SkinPackage_ID = dt.Rows[0]["SkinPackage_ID"].ToString();
            LoadPackageList2DDL(SkinPackage_ID);

            string SkinType = dt.Rows[0]["SkinType"].ToString();
            LoadSkinTypeList2DDL(SkinType);
        }

        private int AddData()
        {
            int SkinPackage_ID = Convert.ToInt32(ddlPackageList.SelectedValue);
            /*** UPLOAD *************************************************************************************************************/
            HttpPostedFile File_ControlSrc = FileInput.PostedFile;
            string ControlSrc = "";
            if ((File_ControlSrc != null) && (File_ControlSrc.ContentLength > 0))
            {
                ControlSrc = System.IO.Path.GetDirectoryName(File_ControlSrc.FileName);
            }        
            /************************************************************************************************************************/
            Skin skin_obj = new Skin();
            int i = skin_obj.Insert(SkinPackage_ID, ControlSrc);
            return i;
        }

        private int UpdateData()
        {
            int SkinPackage_ID = Convert.ToInt32(ddlPackageList.SelectedValue);

            /*** UPLOAD *************************************************************************************************************/
            HttpPostedFile File_ControlSrc = FileInput.PostedFile;
            string ControlSrc = "";
            if ((File_ControlSrc != null) && (File_ControlSrc.ContentLength > 0))
            {
                ControlSrc = System.IO.Path.GetDirectoryName(File_ControlSrc.FileName);
            }
            else
            {
                ControlSrc = txtControlSrc.Text;
            }
            /************************************************************************************************************************/

            Skin skin_obj = new Skin();
            int i = skin_obj.Update(_idx, SkinPackage_ID, ControlSrc);
            return i;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = 0;
                if (_mode == UIMode.mode.add)
                {
                    i = AddData();
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
                        MultiView1.ActiveViewIndex = 1;
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
                else if (_mode == UIMode.mode.edit)
                {
                    i = UpdateData();
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
}