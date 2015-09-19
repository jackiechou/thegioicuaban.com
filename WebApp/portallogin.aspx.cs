using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Entities.Users;

namespace WebApp
{
    public partial class portallogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1000; //Make sure the browser doesn't cache this page
            Response.Buffer = true; //enables our response.redirect to work               
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);

            //Code disables caching by browser =>"Back" browser button
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now); //or a date much earlier than current time       
            Response.AddHeader("Refresh", Convert.ToString((Session.Timeout * 60) - 20)); //Keep session alive forever               

            //check User IP if the user is behind a proxy server.
            //string ips = Request.ServerVariables["REMOTE_ADDR"].ToString();
            //if (ips != "118.69.225.43" && ips != "222.253.144.169" && ips != "222.253.120.148" && ips != "210.245.35.252")
            //{
            //    Response.Write("<script>alert('You have not right to access this page.');window.history.back();</script>");
            //    Response.End();
            //}
            //else
            //{            
            if (!Page.IsPostBack)
            {
                HttpCookie Cookie = Request.Cookies["Cookie_IData"];
                if (Cookie != null)
                {
                    //Server.HtmlEncode(Request.Cookies["Cookie_IData"]["Username"]
                    string user_name = Server.HtmlEncode(Cookie.Values["Username"].ToString());
                    string pass_word = Server.HtmlEncode(Cookie.Values["Password"].ToString());

                    if (user_name != null && user_name != string.Empty)
                    {
                        username.Value = user_name;
                    }

                    if (pass_word != null && pass_word != string.Empty)
                    {
                        password.Value = pass_word;
                    }
                }

                //username.Attributes.Add("onKeyPress", "doClick('" + btnSubmit.ClientID + "',event)");
                //password.Attributes.Add("onKeyPress", "doClick('" + btnSubmit.ClientID + "',event)");

                Page.Form.DefaultFocus = username.ClientID;
                Page.Form.DefaultButton = btnSubmit.UniqueID;
            }           
        }

        protected void check_login(string Username, string Password)
        {
            if (Username == string.Empty)
            {
                string scriptCode = "<script>alert('Username is not empty.');document.location='login.aspx'</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
            }
            else if (Password == string.Empty)
            {
                string scriptCode = "<script>alert('Password is not empty.');document.location='login.aspx'</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
            }
            else if (Username == string.Empty && Password == string.Empty)
            {
                string scriptCode = "<script>alert('Username and Password are not empty.');document.location='login.aspx'</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
            }
            else
            {
                string[] array_list = new string[10];
                UserController users_obj = new UserController();
                array_list = users_obj.CheckLogin(Username, Password);

                string ApplicationId = array_list[0].ToString();
                string UserId = array_list[1].ToString();
                string RoleId = array_list[2].ToString();
                string PortalId = array_list[3].ToString();
                string VendorId = array_list[4].ToString();
                string HomeDirectory = array_list[5].ToString();
                string IsSuperUser = array_list[6].ToString();
                string UpdatePassword = array_list[7].ToString();
                string IsDeleted = array_list[8].ToString();
                string Status = array_list[9].ToString();
                string scriptCode = string.Empty;

                switch (Status)
                {
                    case "-1":
                        scriptCode = "<script>alert('Username or password is empty.');document.location='login.aspx'</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                        break;
                    case "-2":
                        scriptCode = "<script>alert('Username or password is incorrect.');document.location='login.aspx'</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                        break;
                    case "-3":
                        scriptCode = "<script>alert('Error to write log.');document.location='login.aspx'</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                        break;
                    case "1":
                        Session["ApplicationId"] = ApplicationId;
                        Session["UserId"] = UserId;
                        Session["RoleId"] = RoleId;
                        Session["PortalId"] = PortalId;
                        Session["VendorId"] = VendorId;
                        Session["IsSuperUser"] = IsSuperUser;
                        Session["UpdatePassword"] = UpdatePassword;
                        Session["IsDeleted"] = IsDeleted;
                        Session["UserName"] = Username;
                        Session["HomeDirectory"] = HomeDirectory;
                        Session.Timeout = 216000;
                        if (chkRemmberMe.Checked == true)
                        {
                            WriteCookies(UserId, Username, Password);
                        }
                        else
                        {
                            DeleteCookies();
                        }
                        //users_obj.CreateSessionLog(UserID, Username);
                        string portal_url = "~/" + HomeDirectory + "/index.aspx";
                        Response.Redirect(portal_url);
                        break;
                    case "2":
                        scriptCode = "<script>alert('Username or password has not yet activated.');document.location='login.aspx'</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                        break;
                    case "3":
                        scriptCode = "<script>alert('Username or password is blocked.');document.location='login.aspx'</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                        break;
                    default:
                        scriptCode = "<script>alert('System Error.');document.location='login.aspx'</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                        break;
                }
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!IsRefresh)
            {
                string strUsername = username.Value;
                string strPassword = password.Value;
                if (strUsername == string.Empty)
                {
                    Response.Write("<script>alert('Username is not empty');document.location='login.aspx'</script>");
                    Response.End();
                }
                else if (strPassword == string.Empty)
                {
                    Response.Write("<script>alert('Password is not empty');document.location='login.aspx'</script>");
                    Response.End();
                }
                else if (strUsername == string.Empty && strPassword == string.Empty)
                {
                    Response.Write("<script>alert('Username and Password are not empty');document.location='login.aspx'</script>");
                    Response.End();
                }
                else
                {
                    if (Page.IsValid)
                    {
                        check_login(strUsername, strPassword);
                    }
                }
            }
        }


        #region Cookie ==========================================================================
        public string LoadCookies()
        {
            string result = null;
            if (Request.Cookies["Cookie_IData"] != null)
            {
                string UserID = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["UserId"]);
                string Username = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["Username"]);
                string Password = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["Password"]);
                result = Username + "," + Password;
            }
            return result;
        }
        public void WriteCookies(string UserId, string Username, string Password)
        {
            HttpCookie cookie = new HttpCookie("Cookie_IData");
            cookie.Values.Add("UserId", UserId);
            cookie.Values.Add("Username", Username);
            cookie.Values.Add("Password", Password);
            cookie.Values.Add("LastVisit", DateTime.Now.ToString());

            //cookie.Expires = new System.DateTime(2030, 12, 30);
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);
        }
        protected void DeleteCookies()
        {
            if (Request.Cookies["Cookie_IData"] != null)
            {
                HttpCookie aCookie = new HttpCookie("Cookie_IData");
                aCookie.Expires = System.DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }
        }
        #endregion  ==============================================================================

        #region Prevent RERESH PAGE ============================================================================
        private bool _refreshState;
        private bool _isRefresh;

        public bool IsRefresh
        {
            get { return _isRefresh; }
        }
        // Overrides base class' LoadViewState to check and prevent a refresh 
        protected override void LoadViewState(object savedState)
        {
            object[] AllStates = (object[])savedState;
            base.LoadViewState(AllStates[0]); _refreshState = Convert.ToBoolean(AllStates[1]);
            _isRefresh = _refreshState == (Session["__ISREFRESH "] == null ? false : (bool)Session["__ISREFRESH "]);
        }
        // Adds custom data into viewstate by overrinding base class method  
        protected override object SaveViewState()
        {
            Session["__ISREFRESH "] = _refreshState;
            object[] AllStates = new object[2];
            AllStates[0] = base.SaveViewState();
            AllStates[1] = !_refreshState;
            return AllStates;
        }
        #endregion  =========================================================================================== 
    }
}