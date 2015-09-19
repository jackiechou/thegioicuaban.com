using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


/// <summary>
/// Summary description for StatusConst
/// </summary>
public class StatusConst
{
    public enum Status
    {
        THANH_TOAN_SAU_PAID = 1,//hình thức thanh toán trả sau và đã thanh toán
        THANH_TOAN_SAU_UNPAID = 2,//hình thức thanh toán trả sau nhưng chưa thanh toán
        THANH_TOAN_TRUOC_PAID = 3,//hình thức thanh toán trả trước và đã thanh toán
        THANH_TOAN_TRUOC_UNPAID = 4,//hình thức thanh toán trả trước nhưng chưa thanh toán
        CANCEL = 5//thanh toán đã bị hủy
    }
    public StatusConst()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}
