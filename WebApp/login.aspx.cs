using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules;
using CommonLibrary.Entities.Users;
using System.Web.Routing;

namespace WebApp
{
    public partial class login : System.Web.UI.Page    
    {
        public void Page_PreInit(Object sender, EventArgs e)
        {
            string page_title = "ADMIN";
            Page.Title = page_title.Substring(0, page_title.Length - 5); ;
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            Page.Culture = "vi-VN";
            Page.UICulture = "vi";
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.CacheControl = "no-cache";
            //Response.AddHeader("Pragma", "no-cache");
            //Response.Expires = -1000; //Make sure the browser doesn't cache this page
            //Response.Buffer = true; //enables our response.redirect to work               
            //Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);

            //Code disables caching by browser =>"Back" browser button
           // Response.Cache.SetCacheability(HttpCacheability.NoCache);
          //  Response.Cache.SetExpires(DateTime.Now); //or a date much earlier than current time       
          //  Response.AddHeader("Refresh", Convert.ToString((Session.Timeout * 60) - 20)); //Keep session alive forever               

            //check User IP if the user is behind a proxy server.
            //string ips = Request.ServerVariables["REMOTE_ADDR"].ToString();
            //if (ips != "174.120.212.3")
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
                    string username = Server.HtmlEncode(Cookie.Values["Username"].ToString());
                    string password = Server.HtmlEncode(Cookie.Values["Password"].ToString());
                    if (!String.IsNullOrEmpty(username))
                    {
                        txtUsername.Text = username;
                        txtUsername.Attributes.Add("value", username); 
                    } if (!String.IsNullOrEmpty(password))
                    {
                        txtPassword.Text = password;
                        txtPassword.Attributes.Add("value", password); 
                    }
                }

                txtUsername.Attributes.Add("onKeyPress", "doClick('" + btnSubmit.ClientID + "',event)");
                txtPassword.Attributes.Add("onKeyPress", "doClick('" + btnSubmit.ClientID + "',event)");

                Page.Form.DefaultFocus = txtUsername.ClientID;
                Page.Form.DefaultButton = btnSubmit.UniqueID;
            }

            //}
        }

        protected void check_login(string Username, string Password)
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

            switch (Status)
            {
                case "-1":
                    Response.Write("<script>alert('Username or password is empty');document.location='login'</script>");
                    Response.End();
                    break;
                case "-2":
                    Response.Write("<script>alert('Username or password is incorrect');document.location='login'</script>");
                    Response.End();
                    break;
                case "-3":
                    Response.Write("<script>alert('Error to write log');document.location='login'</script>");
                    Response.End();
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

                    if (chkRemmberMe.Checked)
                        WriteCookies(Username, Password, UserId, RoleId, PortalId, VendorId, IsSuperUser, UpdatePassword, IsDeleted, HomeDirectory);
                    else
                        DeleteCookies();

                    //users_obj.CreateSessionLog(UserId, Username);
                    //GetVirtualPath(RequestContext, RouteValueDictionary)                                              
                    //Response.RedirectToRoutePermanent("admin_index", new { tabid = "13"});
                    Response.RedirectToRoutePermanent("admin_index_tabid", new { tabid = "13" });
                    break;
                case "2":
                    Response.Write("<script>alert('Username or password has not yet activated');document.location='login'</script>");
                    Response.End();
                    break;
                case "3":
                    Response.Write("<script>alert('Username or password is blocked');document.location='login'</script>");
                    Response.End();
                    break;
                default:
                    Response.Write("<script>alert('System Error');document.location='login'</script>");
                    Response.End();
                    break;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!IsRefresh)
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;
                if (username == string.Empty)
                {
                    Response.Write("<script>alert('Username is not empty');document.location='login'</script>");
                    Response.End();
                }
                else if (password == string.Empty)
                {
                    Response.Write("<script>alert('Password is not empty');document.location='login'</script>");
                    Response.End();
                }
                else if (username == string.Empty && password == string.Empty)
                {
                    Response.Write("<script>alert('Username and Password are not empty');document.location='login'</script>");
                    Response.End();
                }
                else
                {
                    if (Page.IsValid)
                    {
                        System.Threading.Thread.Sleep(2000);
                        check_login(username, password);
                    }
                }
            }
        }


        #region Cookie ==========================================================================
        public string[] LoadCookies()
        {            
            List<string> items = new List<string>(8);
            if (Request.Cookies["Cookie_IData"] != null)
            {                
                string Username = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["Username"]);
                string Password = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["Password"]);
                string UserId = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["UserId"]);
                string RoleId = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["RoleId"]);
                string PortalId = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["PortalId"]);
                string VendorId = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["VendorId"]);
                string IsSuperUser = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["IsSuperUser"]);
                string UpdatePassword = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["UpdatePassword"]);
                string IsDeleted = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["IsDeleted"]);
                string HomeDirectory = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["HomeDirectory"]);
                string LastVisit = Server.HtmlEncode(Request.Cookies["Cookie_IData"]["LastVisit"]);   
                items.Add(Username);
                items.Add(Password);
                items.Add(UserId);
                items.Add(RoleId);
                items.Add(PortalId);
                items.Add(VendorId);
                items.Add(IsSuperUser);
                items.Add(UpdatePassword);
                items.Add(IsDeleted);
                items.Add(HomeDirectory);
                items.Add(LastVisit);
            }
            return items.ToArray();
        }
        public void WriteCookies(string Username, string Password, string UserId, string RoleId, string PortalId, 
            string VendorId, string IsSuperUser, string UpdatePassword, string IsDeleted, string HomeDirectory)
        {
            HttpCookie cookie = new HttpCookie("Cookie_IData");            
            cookie.Values.Add("Username", Username);
            cookie.Values.Add("Password", Password);
            cookie.Values.Add("UserId", UserId);
            cookie.Values.Add("RoleId", RoleId);
            cookie.Values.Add("PortalId", PortalId);
            cookie.Values.Add("VendorId", VendorId);
            cookie.Values.Add("IsSuperUser", IsSuperUser);
            cookie.Values.Add("UpdatePassword", UpdatePassword);
            cookie.Values.Add("IsDeleted", IsDeleted);
            cookie.Values.Add("HomeDirectory", HomeDirectory);
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
