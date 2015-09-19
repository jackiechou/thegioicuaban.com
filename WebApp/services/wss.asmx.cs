using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebApp.services
{
    [WebService(Namespace = "http://microsoft.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class wss : System.Web.Services.WebService
    {
        [WebMethod(Description = "HelloWorld")]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string CheckEmail(string Email)
        {
            string result = string.Empty;
            //ShopCartLibrary.Customers.Customers cc_obj = new ShopCartLibrary.Customers.Customers();
            //int i = cc_obj.CheckEmail(Email);
            //if (i == 0) { result = "Email nay được quyền đăng ký"; }
            //if (i == 1) { result = "Email này đã đăng ký"; }
            return result;
        }
   }
}
