using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AjaxControlToolkit;
using System.Data;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Modules;
using System.Web.Script.Services;
using MediaLibrary;
using CommonLibrary.Application;
using CommonLibrary.Security.Roles;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Users;
using System.Configuration;

namespace WebApp.services
{

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class WebServices : System.Web.Services.WebService
    {
        //User====================================================================
        [WebMethod]
        public string GetUserByEmail(string Email)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            bool Check = regex.IsMatch(Email);
            string o_return = string.Empty;
            if (Check == true)
            {
                string[] result = new string[3];
                string UserName = string.Empty, PassWord = string.Empty;
                UserController user_obj = new UserController();
                result = user_obj.GetUserPassByEmail(Email);
                int i = Convert.ToInt32(result[0].ToString());
                if (i == 1)
                {
                    UserName = result[1];
                    PassWord = result[2];

                    string sender_email = ConfigurationManager.AppSettings["mailaddress"].ToString();
                    string sender_account = ConfigurationManager.AppSettings["mailaccount"].ToString();
                    string sender_password = ConfigurationManager.AppSettings["mailpassword"].ToString();

                    string receiver_name = Email.Substring(0, Email.LastIndexOf("@")); ;
                    string receiver_email = Email;

                    string subject = "Thông tin tài khoản đăng nhập Admin";
                    string body_content = "<div>UserName: " + UserName + "<br/>PassWord: " + PassWord + "</div>";
                    bool send_mail = EmailClass.send_mail(sender_account, sender_password, sender_account, sender_email, receiver_name, receiver_email, subject, body_content);
                    if (send_mail == true)
                        o_return = "Thông tin đã được gửi đến Email của bạn. Vui lòng kiểm tra hộp thư.";
                }
                else if (i == -2)
                    o_return = "Email xác nhận không đúng";
                else
                    o_return = "Vui lòng nhập Email";
            }
            else
                o_return = "Email không hợp lệ.";
            return o_return;
        }
        //==============================================================================
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }


        [WebMethod]
        public CascadingDropDownNameValue[] GetApplicationList(string knownCategoryValues, string category)
        {
            ApplicationController application_obj = new ApplicationController();
            DataTable dt = application_obj.GetApps();

            List<CascadingDropDownNameValue> application_list = new List<CascadingDropDownNameValue>();
            foreach (DataRow dRow in dt.Rows)
            {
                string ApplicationId = dRow["ApplicationId"].ToString();
                string ApplicationName = dRow["ApplicationName"].ToString();
                application_list.Add(new CascadingDropDownNameValue(ApplicationName, ApplicationId));
            }
            return application_list.ToArray();
        }

        [WebMethod]
        public CascadingDropDownNameValue[] GetRoleList(string knownCategoryValues, string category)
        {
            StringDictionary dataValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
            string ApplicationId = dataValues["ApplicationId"];
            RoleController role_obj = new RoleController();
            DataTable dt = role_obj.GetRoleListByApplicationId(ApplicationId);

            List<CascadingDropDownNameValue> role_list = new List<CascadingDropDownNameValue>();
            foreach (DataRow dRow in dt.Rows)
            {
                string RoleId = dRow["RoleId"].ToString();
                string RoleName = dRow["RoleName"].ToString();
                role_list.Add(new CascadingDropDownNameValue(RoleName, RoleId));
            }
            return role_list.ToArray();
        }

        [WebMethod]
        public CascadingDropDownNameValue[] GetPortalList(string knownCategoryValues, string category)
        {
            StringDictionary dataValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
            string ApplicationId = dataValues["ApplicationId"];
            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetListByApplicationId(ApplicationId);

            List<CascadingDropDownNameValue> portal_list = new List<CascadingDropDownNameValue>();
            foreach (DataRow dRow in dt.Rows)
            {
                string PortalId = dRow["PortalId"].ToString();
                string PortalName = dRow["PortalName"].ToString();
                portal_list.Add(new CascadingDropDownNameValue(PortalName, PortalId));
            }
            return portal_list.ToArray();
        }

        //[WebMethod]
        //public CascadingDropDownNameValue[] GetVendorList(string knownCategoryValues, string category)
        //{
        //    StringDictionary dataValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
        //    int PortalId = Convert.ToInt32(dataValues["PortalId"]);
        //    Vendors vendor_obj = new Vendors();
        //    DataTable dt = vendor_obj.GetListByPortalId(PortalId);

        //    List<CascadingDropDownNameValue> vendor_list = new List<CascadingDropDownNameValue>();
        //    foreach (DataRow dRow in dt.Rows)
        //    {
        //        string VendorId = dRow["VendorId"].ToString();
        //        string VendorName = dRow["VendorName"].ToString();
        //        vendor_list.Add(new CascadingDropDownNameValue(VendorName, VendorId));
        //    }
        //    return vendor_list.ToArray();
        //}

        
        [WebMethod]
        public CascadingDropDownNameValue[] GetUserList(string knownCategoryValues, string category)
        {
            StringDictionary dataValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
            string ApplicationId = dataValues["ApplicationId"];
            UserController user_obj = new UserController();
            DataTable dt = user_obj.GetUsers(ApplicationId);

            List<CascadingDropDownNameValue> user_list = new List<CascadingDropDownNameValue>();
            foreach (DataRow dRow in dt.Rows)
            {
                string UserId = dRow["UserId"].ToString();
                string UserName = dRow["UserName"].ToString();
                user_list.Add(new CascadingDropDownNameValue(UserName, UserId));
            }
            return user_list.ToArray();
        }

        [WebMethod]
        public CascadingDropDownNameValue[] GetModuleList(string knownCategoryValues, string category)
        {
            StringDictionary dataValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
            string PortalId = dataValues["PortalId"];
            Modules module_obj = new Modules();
            DataTable dt = module_obj.GetModuleListByPortalId(PortalId);

            List<CascadingDropDownNameValue> module_list = new List<CascadingDropDownNameValue>();
            foreach (DataRow dRow in dt.Rows)
            {
                string ModuleId = dRow["ModuleId"].ToString();
                string ModuleTitle = dRow["ModuleTitle"].ToString();
                module_list.Add(new CascadingDropDownNameValue(ModuleTitle, ModuleId));
            }
            return module_list.ToArray();
        }

        [WebMethod]
        public CascadingDropDownNameValue[] GetPermissionList(string knownCategoryValues, string category)
        {
            StringDictionary dataValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
            int ModuleId =Convert.ToInt32(dataValues["ModuleId"]);
            PermissionController permission_obj = new PermissionController();
            DataTable dt = permission_obj.GetPermissionsByModuleId(ModuleId);

            List<CascadingDropDownNameValue> permission_list = new List<CascadingDropDownNameValue>();
            foreach (DataRow dRow in dt.Rows)
            {
                string PermissionId = dRow["PermissionId"].ToString();
                string PermissionName = dRow["PermissionName"].ToString();
                permission_list.Add(new CascadingDropDownNameValue(PermissionName, PermissionId));
            }
            return permission_list.ToArray();
        }

       

        //[WebMethod]
        //public CascadingDropDownNameValue[] GetBranchList(string knownCategoryValues, string category)
        //{
        //    BranchClass branch_obj = new BranchClass();
        //    DataTable dt = branch_obj.GetActiveList();

        //    List<CascadingDropDownNameValue> user_list = new List<CascadingDropDownNameValue>();
        //    foreach (DataRow dRow in dt.Rows)
        //    {
        //        string Branch_ID = dRow["Branch_ID"].ToString();
        //        string Branch_Name = dRow["Branch_Name"].ToString();
        //        user_list.Add(new CascadingDropDownNameValue(Branch_Name, Branch_ID));
        //    }
        //    return user_list.ToArray();
        //}

        //[WebMethod]
        //public CascadingDropDownNameValue[] GetDepartmentList(string knownCategoryValues, string category)
        //{
        //    StringDictionary dataValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
        //    int Branch_ID = Convert.ToInt32(dataValues["Branch_ID"]);
        //    DepartmentClass department_obj = new DepartmentClass();
        //    DataTable dt = department_obj.GetDepartmentListByBranchID(Branch_ID);

        //    List<CascadingDropDownNameValue> user_list = new List<CascadingDropDownNameValue>();
        //    foreach (DataRow dRow in dt.Rows)
        //    {
        //        string Department_ID = dRow["Department_ID"].ToString();
        //        string Department_Name = dRow["Department_Name"].ToString();
        //        user_list.Add(new CascadingDropDownNameValue(Department_Name, Department_ID));
        //    }
        //    return user_list.ToArray();
        //}

        //=========================================================================================
        [WebMethod]
        public string[] GetInfoTabInsert(int PortalId, int ContentItemId, int ParentId,
            string TabName, string Title, string CssClass, string IconFile, string IconFileLarge, string Description,
            string Keywords, bool DisplayTitle, bool IsDeleted, bool IsVisible, bool DisableLink, bool IsSecure,
            bool PermanentRedirect, decimal SiteMapPriority, string Url, string TabPath, int RouteId, string PageHeadText,
            string Header, string Footer, string StartDate, string EndDate, string CultureCode, string CreatedByUserId)
        {
            string[] array_list = new string[3];
            try
            {
                CommonLibrary.Entities.Tabs.TabController tab_obj = new CommonLibrary.Entities.Tabs.TabController();
                int[] result = tab_obj.Insert(PortalId, ContentItemId, ParentId, TabName, Title, CssClass,
                    IconFile, IconFileLarge, Description, Keywords, DisplayTitle, IsDeleted, IsVisible,
                    DisableLink, IsSecure, PermanentRedirect, SiteMapPriority, Url, TabPath, RouteId, PageHeadText, "", "", StartDate, EndDate, CultureCode, CreatedByUserId);

                int i = result[0];
                int tabid = result[1];
               // result[2] = Keywords;
            }
            catch (IndexOutOfRangeException ex) { ex.ToString(); }

            return array_list;
        }

        //TOUR =========================================================================================
        //[WebMethod]
        //public string[] GetTourProductFee(string Product_No)
        //{
        //    string[] array_list = new string[3];
        //    try
        //    {
        //        ProductInternationalTourInsurance tour_obj = new ProductInternationalTourInsurance();
        //        DataTable dt = tour_obj.GetDetailsByProductNo(Product_No);

        //        array_list[0] = dt.Rows[0]["Fee"].ToString();
        //        array_list[1] = dt.Rows[0]["VAT"].ToString();
        //        array_list[2] = dt.Rows[0]["Promotion_Charge"].ToString();
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }

        //    return array_list;
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public List<ProductInternationalTourInsurance> GetProductListByGeographicalScopeTypeDays(int GeographicalScope_Id, int Type_Id, int TotalSelectedDays)
        //{
        //    TourPeriods tour_period = new TourPeriods();
        //    DataTable dt_Period = tour_period.GetSelectedPeriodByNumOfDays(TotalSelectedDays);
        //    int Period_Id = Convert.ToInt32(dt_Period.Rows[0]["Period_Id"].ToString());

        //    List<ProductInternationalTourInsurance> _list = new List<ProductInternationalTourInsurance>();
        //    ProductInternationalTourInsurance tour_obj = new ProductInternationalTourInsurance();
        //    DataTable dt = tour_obj.GetListByGeographicalScopeTypePeriod(GeographicalScope_Id, Type_Id, Period_Id);
        //    _list = (from DataRow row in dt.Rows
        //             select new ProductInternationalTourInsurance
        //             {
        //                 Product_Name = row["Product_Name"].ToString(),
        //                 Product_No = row["Product_No"].ToString()
        //             }).ToList();
        //    return _list;
        //}   
        //==============================================================================================

       

     
        #region MEDIA ==================================================================================
        //==============================================================================================
        [WebMethod]
        public string[] GetMediaFile(int FileId)
        {
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);

            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetDetailById(FileId);
            string Title = dt.Rows[0]["Title"].ToString();
            string FileName = dt.Rows[0]["FileName"].ToString();
            string FileUrl = dt.Rows[0]["FileUrl"].ToString();
            string Photo = dt.Rows[0]["Photo"].ToString();
            string Thumbnail = dt.Rows[0]["Photo"].ToString();
            int TypeId = Convert.ToInt32(dt.Rows[0]["TypeId"].ToString());

            //====MediaType=================================================================
            MediaTypes media_type_obj = new MediaTypes();            
            string TypePath = media_type_obj.GetTypePathByTypeId(TypeId);
            //==============================================================================

            //string dir_path = Server.MapPath("~/" + TypePath);

            string file_path = baseUrl + "/" + TypePath + "/" + FileName;

            string dir_image_path = System.Configuration.ConfigurationManager.AppSettings["upload_media_image_dir"];
            string image = "";

            if (Photo != string.Empty)
                image = baseUrl + "/" + dir_image_path + "/photo/" + Photo;
            if (Photo == string.Empty)
                image = baseUrl + "/" + dir_image_path + "/thumbnails/" + Thumbnail;
            if (Photo == string.Empty && Thumbnail == string.Empty)
                image = baseUrl + "/images/no_image.jpg";


            string[] array_list = new string[3];
            array_list[0] = Title;
            if (FileName != string.Empty && FileUrl == string.Empty)
                array_list[1] = file_path;
            else
                array_list[1] = FileUrl;
            array_list[2] = image;
            return array_list;
        }
        //==============================================================================================

        //==============================================================================================
        [WebMethod]
        public string[] GetSelectedVideo()
        {
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            int typeid = 2;
            //====MediaType=================================================================
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(typeid);
            string TypePath = type_obj.TypePath;
            //==============================================================================
            int FileId = 0;
            string Title = string.Empty;
            string FileName = string.Empty;
            string FileUrl = string.Empty;
            string Photo = string.Empty;
            string Thumbnail = string.Empty;
            string file_path = string.Empty;

            string _path = baseUrl + "/" + TypePath + "/";
            string dir_image_path = System.Configuration.ConfigurationManager.AppSettings["upload_media_image_dir"];
            string image = "";
            string[] array_list = new string[3];

            MediaFiles media_obj = new MediaFiles();
            DataTable dtMedia = media_obj.GetSelectedItem(typeid);
            for (int i = 0; i < dtMedia.Rows.Count; i++)
            {
                FileId = Convert.ToInt32(dtMedia.Rows[i]["FileId"].ToString());
                Title = dtMedia.Rows[i]["Title"].ToString();
                FileName = dtMedia.Rows[i]["FileName"].ToString();
                FileUrl = dtMedia.Rows[i]["FileUrl"].ToString();
                Photo = dtMedia.Rows[i]["Photo"].ToString();
                Thumbnail = dtMedia.Rows[i]["Photo"].ToString();

                file_path = _path + FileName;

                if (Photo != string.Empty)
                    image = baseUrl + "/" + dir_image_path + "/photo/" + Photo;
                if (Photo == string.Empty)
                    image = baseUrl + "/" + dir_image_path + "/thumbnails/" + Thumbnail;
                if (Photo == string.Empty && Thumbnail == string.Empty)
                    image = baseUrl + "/images/no_image.jpg";

                array_list[0] = array_list[0] + "," + Title;
                if (FileName != string.Empty && FileUrl == string.Empty)
                    array_list[1] = array_list[1] + "," + file_path;
                else
                    array_list[1] = array_list[1] + "," + FileUrl;
                array_list[2] = array_list[2] + "," + image;
            }
            return array_list;
        }
        //==============================================================================================


        //==============================================================================================
        [WebMethod]
        public string GetListMediaFile()
        {
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);

            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetListByTypeId(2);
            JsonMethods json_obj = new JsonMethods();
            string jsonarray = json_obj.makejsonoftable(dt, makejson.e_without_square_brackets);
            return jsonarray;
        }
        //==============================================================================================

        #endregion =====================================================================================
    }
}
