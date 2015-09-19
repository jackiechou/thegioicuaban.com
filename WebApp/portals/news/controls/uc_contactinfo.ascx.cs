using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace WebApp.portals.news.controls
{
    public partial class uc_contactinfo : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ContactInfoData();
            }
        }

        protected void ContactInfoData()
        {
            string strHTML = string.Empty, address = string.Empty,phone= string.Empty,fax = string.Empty, email=string.Empty,website=string.Empty, description=string.Empty  ;
            //Vendors Vendors_obj = new Vendors();
            //DataTable dt = Vendors_obj.GetListByPortalId(0);
            //if (dt.Rows.Count > 0)
            //{
            //    address = dt.Rows[0]["address"].ToString();
            //    phone = dt.Rows[0]["Telephone"].ToString();
            //    fax = dt.Rows[0]["Fax"].ToString();
            //    email = dt.Rows[0]["Email"].ToString();
            //    website= dt.Rows[0]["Website"].ToString();
            //    description = dt.Rows[0]["Description"].ToString();

            //     strHTML = "<table>"                                       
            //                    + "<tr>"
            //                + "<td valign=\"top\" width=\"70\" >Địa chỉ: &nbsp;</td>"
            //                + "<td valign=\"top\"><span id=\"address\">"+address+"</span></td>"
            //            + "</tr>"
            //            + "<tr>"
            //                + "<td valign=\"top\">Điện thoại:&nbsp;</td>"
            //                + "<td valign=\"top\"><span id=\"phone\">" + phone + "</span><br>"
            //                + "</td>"
            //            + "</tr>"
            //            + "<tr>"
            //                + "<td valign=\"top\">Fax: &nbsp;</td>"
            //                + "<td valign=\"top\"><span id=\"fax\">" + fax + "</span></td>"
            //            + "</tr>"
            //            + "<tr>"
            //                + "<td valign=\"top\">Website: </td>"
            //                + "<td valign=\"top\" id=\"website\">" + website + "</td>"
            //            + "</tr>"
            //            + "<tr>"
            //                + "<td valign=\"top\">Email:&nbsp; </td>"
            //                + "<td><p valign=\"top\" id=\"email\">" + email + "</p></td>"
            //            + "</tr>"
            //           + " <tr>"
            //                + "<td valign=\"top\"> </td>"
            //                + "<td valign=\"top\" id=\"des\">"+description+"</td>"
            //           + " </tr>"                   
            //      + "</table>";
            //}
            divContactItems.InnerHtml = strHTML;
        }
    }
}