﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Modules;
using CommonLibrary.Entities.Users;
using CommonLibrary.Security.Roles;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.security.users
{
    public partial class aspnet_users_edit : System.Web.UI.Page
    {
        UserController user_obj = new UserController();
        DataTable dt = new DataTable();

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

        private string _idx
        {
            get
            {
                if (ViewState["idx"] == null)
                    ViewState["idx"] = "";
                return (string)ViewState["idx"];
            }
            set
            {
                ViewState["idx"] = value;
            }
        }

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               
               _idx = Request.QueryString["idx"];
               LoadData();         
               MultiView1.ActiveViewIndex = 0;
            }
            Page.Header.DataBind();
            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Portal List ======================================================
        protected void PopulatePortal2DDL(string selected_value)
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetListByApplicationId(ApplicationId);

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dt;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            ddlPortalList.SelectedValue = selected_value;
        }
        #endregion ============================================================

        private void LoadData()
        {
            DataTable dt = user_obj.GetDetails(_idx);

            txtUserName.Text = dt.Rows[0]["UserName"].ToString();
            txtUserName.Enabled = false;
            txtDisplayName.Text = dt.Rows[0]["DisplayName"].ToString();
            
            string PasswordSalt = dt.Rows[0]["PasswordSalt"].ToString();            
            txtPassword.Attributes.Add("value", PasswordSalt);
            txtPasswordConfirm.Attributes.Add("value", PasswordSalt);
            txtPasswordConfirm.Attributes.Add("onkeyup", "checkPass(); return false;");
            LoadPassQuestion2DDL(dt.Rows[0]["PasswordQuestion"].ToString());
            txtPassAnswer.Text = dt.Rows[0]["PasswordAnswer"].ToString();

            
            txtFullName.Text = dt.Rows[0]["FullName"].ToString();
            txtAddress.Text = dt.Rows[0]["Address"].ToString();
            txtMobilePIN.Text = dt.Rows[0]["MobilePIN"].ToString();
            txtPhone.Text = dt.Rows[0]["Phone"].ToString();
            txtMobilePIN.Text = dt.Rows[0]["MobilePIN"].ToString();
            txtEmail.Text = dt.Rows[0]["Email"].ToString();

            string ApplicationId = dt.Rows[0]["ApplicationId"].ToString();
            string RoleId = dt.Rows[0]["RoleId"].ToString();
            string PortalId = dt.Rows[0]["PortalId"].ToString();
            string VendorId = dt.Rows[0]["VendorId"].ToString();

            chkIsApproved.Checked = Convert.ToBoolean(dt.Rows[0]["IsApproved"].ToString());
            chkIsLockedOut.Checked = Convert.ToBoolean(dt.Rows[0]["IsLockedOut"].ToString());
            chkIsSuperUser.Checked = Convert.ToBoolean(dt.Rows[0]["IsSuperUser"].ToString());
            chkUpdatePassword.Checked = Convert.ToBoolean(dt.Rows[0]["UpdatePassword"].ToString());
            chkIsDeleted.Checked = Convert.ToBoolean(dt.Rows[0]["IsDeleted"].ToString());

            LoadApplicationList2DDL(ApplicationId);
            LoadRoleList2DDL(RoleId);
            PopulatePortal2DDL(PortalId);
           
        }

        #region Application List =======================================================
        private void LoadApplicationList2DDL(string selected_value)
        {
            //ddlApplicationList.Items.Clear(); //DROPDOWNLIST      
  
            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps(); //select all the nodes from DB            
            ddlApplicationList.DataSource = dt_app;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";            
            ddlApplicationList.Items.Insert(0, new ListItem("- Chọn ứng dụng -", ""));
            ddlApplicationList.SelectedValue = selected_value;
            ddlApplicationList.DataBind();            
            ddlApplicationList.Enabled = false;
        }
        #endregion ====================================================================

        #region Role List =============================================================
        private void LoadRoleList2DDL(string selected_value)
        {
            ddlRoleList.Items.Clear(); //DROPDOWNLIST        
            RoleController role_obj = new RoleController();
            string ApplicationId = ddlApplicationList.SelectedValue;
            DataTable dt = role_obj.GetRoleListByApplicationId(ApplicationId);           
            
            ddlRoleList.DataSource = dt;
            ddlRoleList.DataTextField = "RoleName";
            ddlRoleList.DataValueField = "RoleId";            
            ddlRoleList.Items.Insert(0, new ListItem("- Chọn -", ""));
            ddlRoleList.SelectedValue = selected_value;
            ddlRoleList.DataBind();   
        }
        #endregion =====================================================================
         
     
        

        protected void btnOkay_Click(object sender, EventArgs e)
        {
           Page.Validate("ValidationCheck");
           if (Page.IsValid)
           {
               System.Threading.Thread.Sleep(2000);
               int i = UpdateData();
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

        private int UpdateData()
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            int VendorId = Convert.ToInt32(ddlVendorList.SelectedValue);
            string RoleId = ddlRoleList.SelectedValue;
            string FullName = txtFullName.Text;
            string DisplayName = txtDisplayName.Text;                
            string MobilePIN = txtMobilePIN.Text;
            string Address = txtAddress.Text;
            string Phone = txtPhone.Text;
            string Email = txtEmail.Text;         

            //=================================================
            bool _IsSuperUser = chkIsSuperUser.Checked;                
            int IsSuperUser;
            if (_IsSuperUser == true)
            {
                IsSuperUser = 1;
            }
            else
            {
                IsSuperUser = 0;
            }
            //=================================================
            bool _IsDeleted = chkIsDeleted.Checked;
            int IsDeleted;
            if (_IsDeleted == true)
            {
                IsDeleted = 1;
            }
            else
            {
                IsDeleted = 0;
            }
            //=================================================
            bool _IsApproved = chkIsApproved.Checked;
            int IsApproved;
            if (_IsApproved == true)
            {
                IsApproved = 1;
            }
            else
            {
                IsApproved = 0;
            }
            //=================================================
            bool _UpdatePassword = chkUpdatePassword.Checked;
            int UpdatePassword;
            if (_UpdatePassword == true)
            {
                UpdatePassword = 1;
            }
            else
            {
                UpdatePassword = 0;
            }
            //=================================================
            bool _IsLockedOut = chkIsLockedOut.Checked;
            int IsLockedOut;
            if (_IsLockedOut == true)
            {
                IsLockedOut = 1;
            }
            else
            {
                IsLockedOut = 0;
            }
            //=================================================
            ModuleClass module_obj = new ModuleClass();
            string Username = string.Empty;
            if (module_obj.CheckInput(txtUserName.Text))
            {
                Username = txtUserName.Text;
            }
            else
            {
                Response.Write("<script>alert('Invalid Username');</script>");
                Response.End();
            }

            MD5CryptEncrypt md5_obj = new MD5CryptEncrypt();
            string Password = md5_obj.getMd5Hash(txtPassword.Text);
            string PasswordSalt = txtPassword.Text;
            string passwordConfirm = txtPasswordConfirm.Text;
            string PasswordQuestion = ddlPassQuestion.SelectedValue;
            string PasswordAnswer = txtPassAnswer.Text;

            string Comment = txtComment.Text;

            UserController user_obj = new UserController();
            int result = user_obj.Update(_idx, ApplicationId, PortalId, VendorId, RoleId, Username,
            Password, PasswordSalt, PasswordQuestion, PasswordAnswer,
            FullName, DisplayName, Address, MobilePIN, Phone, Email,
            IsSuperUser, UpdatePassword,IsDeleted, IsApproved, IsLockedOut, Comment);           
            
            return result;

        }

        #region Password Question
        private void LoadPassQuestion2DDL(string selected_value)
        {
            string[] myArray = new string[] { 
                "What was your childhood nickname?", 
                "In what city did you meet your spouse/significant other?", 
                "What is the name of your favorite childhood friend?",
                "What street did you live on in third grade?",
                "What is your oldest sibling’s birthday month and year? (e.g., January 1900)",
                "What is the middle name of your oldest child?",
                "What is your oldest sibling's middle name?",
                "What school did you attend for sixth grade?",
                "What was your childhood phone number including area code? (e.g., 000-000-0000)",
                "What is your oldest cousin's first and last name?",
                "What was the name of your first stuffed animal?",
                "In what city or town did your mother and father meet?",
                "Where were you when you had your first kiss?",
                "What is the first name of the boy or girl that you first kissed?",
                "What was the last name of your third grade teacher?",
                "In what city does your nearest sibling live?",
                "What is your oldest brother’s birthday month and year? (e.g., January 1900)",
                "What is your maternal grandmother's maiden name?",
                "In what city or town was your first job?",
                "What is the name of the place your wedding reception was held?",
                "What is the name of a college you applied to but didn't attend?",
                "Where were you when you first heard about 9/11?",
                "What was the name of your elementary / primary school?",
                "What is the name of the company of your first job?",
                "What was your favorite place to visit as a child?",
                "What is your spouse's mother's maiden name?",
                "What is the country of your ultimate dream vacation?",
                "What is the name of your favorite childhood teacher?",
               "To what city did you go on your honeymoon?",
                "What time of the day were you born?",
                "What was your dream job as a child?",
                "What is the street number of the house you grew up in?",
                "What is the license plate (registration) of your dad's first car?",
                "Who was your childhood hero?",
                "What was the first concert you attended?",
                "What are the last 5 digits of your credit card?",
                "What are the last 5 of your Social Security number?",
                "What is your current car registration number?",
                "What are the last 5 digits of your driver's license number?",
                "What month and day is your anniversary? (e.g., January 2)",
                "What is your grandmother's first name?",
                "What is your mother's middle name?",
                "What is the last name of your favorite high school teacher?",
                "What was the make and model of your first car?",
                "Where did you vacation last year?",
                "What is the name of your grandmother's dog?",
                "What is the name, breed, and color of current pet?",
                "What is your preferred musical genre?",
                "In what city and country do you want to retire?",
                "What is the name of the first undergraduate college you attended?",
                "What was your high school mascot?",
                "What year did you graduate from High School?",
                "What is the name of the first school you attended?" ,     
                "What was your favorite sport in high school?" ,
                "What is the name of the High School you graduated from?" ,
                "What is your pet's name?" ,
                "In what year was your father born?" ,
                "In what year was your mother born?" ,
                "What is your mother’s (father's) first name?" ,
                "What is your mother's maiden name?" ,
                "What was the color of your first car?" ,
                "What is your father's middle name?" ,
                "In what county where you born?" ,
                "How many bones have you broken?" ,
                "What is the first and last name of your favorite college professor?" ,
                "On which wrist do you wear your watch?" ,
                "What is the color of your eyes?" ,
                "What is the title and artist of your favorite song?" ,
                "What is the title and author of your favorite book?" ,
                "What is the name, breed, and color of your favorite pet?" ,
                "What is your favorite animal?" ,
                "What was the last name of your favorite teacher?" ,
                "What is your favorite team?" ,
                "What is your favorite movie?" ,
                "What is your favorite teacher's nickname?" ,
                "What is your favorite TV program?" ,
                "What is your least favorite nickname?" ,
                "What is your favorite sport?" ,
                "What is the name of your hometown?" ,
                "What is the color of your father’s eyes?" ,
                "What is the color of your mother’s eyes?" ,
                "What was the name of your first pet?" ,
                "What sports team do you love to see lose?" ,
                "In what city were you born?" ,
                "What is the city, state/province, and year of your birth?" ,
                "What is the name of your hometown newspaper?" ,
                "What is your favorite color?" ,
                "What was your hair color as a child?" ,
                "What is your work address?" ,
                "What is the street name your work or office is located on?" ,
                "What is your address, phone number?"
            };

            ddlPassQuestion.Items.Clear();
            ddlPassQuestion.DataSource = myArray;            
            ddlPassQuestion.DataBind();
            ddlPassQuestion.Items.Insert(0, new ListItem("- Chọn -", "")); //DROPDOWNLIST
            ddlPassQuestion.SelectedValue = selected_value;
        }
        #endregion          

        #region Branch List =============================================================
        //private void LoadBranchList2DDL(string selected_value)
        //{
        //    ddlBranchList.Items.Clear(); //DROPDOWNLIST        
        //    BranchClass branch_obj = new BranchClass();
        //    DataTable dt = branch_obj.GetActiveList();

        //    ddlBranchList.DataSource = dt;
        //    ddlBranchList.DataTextField = "Branch_Name";
        //    ddlBranchList.DataValueField = "Branch_ID";
        //    ddlBranchList.Items.Insert(0, new ListItem("- Chọn -", ""));
        //    ddlBranchList.SelectedValue = selected_value;
        //    ddlBranchList.DataBind();
        //}
        //protected void ddlBranchList_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LoadDepartmentList2DDL();
        //}
        #endregion =====================================================================

        #region Department List =============================================================
        //private void LoadDepartmentList2DDL()
        //{
        //    ddlDepartmentList.Items.Clear(); //DROPDOWNLIST     
        //    int Branch_ID = Convert.ToInt32(ddlBranchList.SelectedValue);
        //    DepartmentClass department_obj = new DepartmentClass();
        //    DataTable dt = department_obj.GetDepartmentListByBranchID(Branch_ID);

        //    ddlDepartmentList.DataSource = dt;
        //    ddlDepartmentList.DataTextField = "Department_Name";
        //    ddlDepartmentList.DataValueField = "Department_ID";
        //    ddlDepartmentList.Items.Insert(0, new ListItem("- Chọn -", ""));
        //    ddlDepartmentList.SelectedIndex = 0;
        //    ddlDepartmentList.DataBind();

        //}
        //private void LoadDepartmentList2DDL(string selected_value)
        //{
        //    ddlDepartmentList.Items.Clear(); //DROPDOWNLIST     
        //    int Branch_ID = Convert.ToInt32(ddlBranchList.SelectedValue);
        //    DepartmentClass department_obj = new DepartmentClass();
        //    DataTable dt = department_obj.GetDepartmentListByBranchID(Branch_ID);

        //    ddlDepartmentList.DataSource = dt;
        //    ddlDepartmentList.DataTextField = "Department_Name";
        //    ddlDepartmentList.DataValueField = "Department_ID";
        //    ddlDepartmentList.Items.Insert(0, new ListItem("- Chọn -", ""));
        //    ddlDepartmentList.SelectedValue = selected_value;
        //    ddlDepartmentList.DataBind();
        //}
        #endregion =====================================================================

        #region Position List ============================================
        //private void LoadPositionList2DDL()
        //{
        //    string[] myArray = new string[] { 
        //        "Tổng Giám Đốc", 
        //        "Giám Đốc", 
        //        "Phó Giám Đốc",
        //        "Trưởng Phòng",
        //        "Nhân Viên"
        //    };

        //    ddlPostionList.Items.Clear();
        //    ddlPostionList.DataSource = myArray;
        //    ddlPostionList.DataBind();
        //    ddlPostionList.Items.Insert(0, new ListItem("- Chọn -", ""));
        //    ddlPostionList.SelectedIndex = 0;
        //}

        //private void LoadPositionList2DDL(string selected_value)
        //{
        //    string[] myArray = new string[] { 
        //        "Tổng Giám Đốc", 
        //        "Giám Đốc", 
        //        "Phó Giám Đốc",
        //        "Trưởng Phòng",
        //        "Nhân Viên"
        //    };

        //    ddlPostionList.Items.Clear();
        //    ddlPostionList.DataSource = myArray;
        //    ddlPostionList.DataBind();
        //    ddlPostionList.Items.Insert(0, new ListItem("- Chọn -", ""));
        //    ddlPostionList.SelectedValue = selected_value;            
        //}
        #endregion =======================================================
    }
}
