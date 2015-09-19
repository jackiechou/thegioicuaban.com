using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CommonLibrary.Modules;

namespace WebApp.modules.admin.domain
{
    public partial class checkdomain : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDomainGroup2RadioBtnList();
            LoadDomainExtension2DDL();
        }
        
        public void LoadDomainGroup2RadioBtnList()
        {
            ListItemCollection lst = new ListItemCollection();
            lst.Add(new ListItem("Tên miềm phổ biến", "1"));
            lst.Add(new ListItem("Tên miềm Việt Nam", "2"));
            lst.Add(new ListItem("Tên miềm Việt Nam theo địa giới hành chính", "3"));
            lst.Add(new ListItem("Tên miềm Quốc Tế", "4"));      

            rdlDomainGroup.Items.Clear();            
            rdlDomainGroup.DataSource = lst;
            rdlDomainGroup.DataBind();
            rdlDomainGroup.SelectedIndex = 0;
            rdlDomainGroup.AutoPostBack = true;
        }
        protected void rdlDomainGroup_SelectedIndexChanged(object sender, EventArgs e)
        {            
            LoadDomainExtension2DDL();
        }

        public void LoadDomainExtension2DDL()
        {
            string selected_value = rdlDomainGroup.SelectedValue;            

            string[] myArray;
            string[] popularDomainList = new string[] { ".com", ".net", ".org", ".vn", ".com.vn", ".net.vn" };
            string[] vietnamDomainList = new string[] { "ac.vn", ".biz.vn", ".com.vn", ".edu.vn", ".gov.vn", ".health.vn", ".info.vn", ".name.vn", ".net.vn", ".org.vn", ".pro.vn" };
            string[] vietnamRegionDomainList = new string[] { ".angiang.vn", ".bacgiang.vn", ".backan.vn", ".baclieu.vn", ".bacninh.vn", ".baria-vungtau.vn", ".bentre.vn", ".binhdinh.vn", ".binhduong.vn", ".binhphuoc.vn", ".binhthuan.vn", ".camau.vn", ".cantho.vn", ".caobang.vn", ".daklak.vn", ".daknong.vn", ".danang.vn", ".dienbien.vn", ".dongnai.vn", ".dongthap.vn", ".gialai.vn", ".hagiang.vn", ".haiduong.vn", ".haiphong.vn", ".hanam.vn", ".hanoi.vn", ".hatinh.vn", ".haugiang.vn", ".hoabinh.vn", ".hungyen.vn", ".int.vn", ".khanhhoa.vn", ".kiengiang.vn", ".kontum.vn", ".laichau.vn", ".lamdong.vn", ".langson.vn", ".laocai.vn", ".longan.vn", ".namdinh.vn", ".nghean.vn", ".ninhbinh.vn", ".ninhthuan.vn", ".phutho.vn", ".phuyen.vn", ".quangbinh.vn", ".quangnam.vn", ".quangngai.vn", ".quangninh.vn", ".quangtri.vn", ".soctrang.vn", ".sonla.vn", ".tayninh.vn", ".thaibinh.vn", ".thainguyen.vn", ".thanhhoa.vn", ".thanhphohochiminh.vn", ".thuathienhue.vn", ".tiengiang.vn", ".travinh.vn", ".tuyenquang.vn", ".vinhlong.vn", ".vinhphuc.vn", ".yenbai.vn" };
            string[] internationalDomainList = new string[] { "asia", ".biz", ".bz", ".cc", ".co", ".co.uk", ".com", ".com.co", ".com.tw", ".de", ".eu", ".in", ".info", ".it", ".jp", ".me", ".mobi", ".name", ".net", ".net.co", ".net.tw", ".nom.co", ".org", ".org.tw", ".pro", ".tel", ".tv", ".tw", ".us", ".vn", ".ws", ".xxx" };

            if (selected_value == "1")
            {
                myArray = popularDomainList;
            }
            else if (selected_value == "2")
            {
                myArray = vietnamDomainList;
            }
            else if (selected_value == "3")
            {
                myArray = vietnamRegionDomainList;
            }
            else if (selected_value == "4")
            {
                myArray = internationalDomainList;
            }
            else
            {
                myArray = new string[] { ".com", ".net", ".org", ".vn", ".com.vn", ".net.vn", ".biz.vn", ".edu.vn", ".gov.vn", ".org.vn", ".int.vn", ".ac.vn", ".pro.vn", ".info.vn", ".heath.vn", ".name.vn", ".biz", ".info", ".mobi", ".tel", ".co", ".com.co", ".net.co", ".asia", ".com.tw", ".eu", ".cc", ".net.tw", ".us", ".tw", ".org.tw", ".me", ".tv", ".ws" };
            }            
            //Array.Sort(myArray);

            ddlDomainExtension.Items.Clear();
            ddlDomainExtension.DataSource = myArray;
            ddlDomainExtension.DataBind();
            ddlDomainExtension.SelectedIndex = 0;        
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            string strDomainWithouExtToCheck = txtDomain.Text;
            string strDomainExtToCheck = ddlDomainExtension.SelectedValue;
            
            string strFullDomainToCheck = strDomainWithouExtToCheck + strDomainExtToCheck;
            string strResult = string.Empty;
            // CheckValidDomain(strDomainWithouExtToCheck)== true && 
            if (!string.IsNullOrEmpty(strDomainWithouExtToCheck))
            {
                if (CountOccurencesOfChar(strDomainWithouExtToCheck, '-') < 2)
                {
                    string strCommand = "getwhois";
                    strResult = CheckDomain(strDomainWithouExtToCheck,strDomainExtToCheck, strCommand);
                    //strResult = CheckDomainList(strDomainWithouExtToCheck);
                    //strResult = CheckDomain(strDomainWithouExtToCheck, strDomainExtToCheck);       
                }
                else
                {
                    strResult = "Domain không hợp lệ";
                    //string scriptCode = "<script>alert('Domain không hợp lệ');history.back()</script>";
                    //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                }
            }
            divResult.InnerHtml = strResult;
        }

        public string CheckDomain(string strDomainWithouExtToCheck, string strExt, string strCommand="")
        {              
            StringBuilder strOut = new StringBuilder();
            strOut.Append("<h3>Result</h3>");
            strOut.Append("<table cellpadding=\"4\" cellspacing=\"6\" border=\"0\" width=\"550\">"); 
            strOut.Append("     <tr>");
            strOut.Append("         <td width=\"250\">");
            strOut.Append("             <span id=\"domain_check").Append("\">");
            strOut.Append(strDomainWithouExtToCheck + strExt);
            strOut.Append("             </span>");
            strOut.Append("         </td>");
            strOut.Append("         <td width=\"250\">");
            strOut.Append("             <span id=\"domain_result").Append("\"></span><br />");
            strOut.Append("             <script language=\"javascript\" type=\"text/javascript\">CheckDomain('").Append(strDomainWithouExtToCheck).Append("','").Append(strExt).Append("','").Append(strCommand).Append("','domain_result").Append("');</script>");
            strOut.Append("         </td>");
            strOut.Append("     </tr>");
            strOut.Append("</table>");

            return strOut.ToString();
        }

        public string CheckDomainList(string strDomainWithouExtToCheck, string strCommand = "")
        {
            int iCount = 0;
            string strFullDomainToCheck = "";
            string strDomainExtToCheck = ".com,.biz,.net,.info,.mobi,.org,.tel,.co,.com.co,.net.co,.asia,.com.tw,.eu,.cc,.net.tw,.us,.tw,.org.tw,.me,.tv,.ws,.vn,.com.vn,.biz.vn,.edu.vn,.gov.vn,.net.vn,.org.vn,.int.vn,.ac.vn,.pro.vn,.info.vn,.heath.vn,.name.vn";
            string[] strSplitExt = strDomainExtToCheck.Split(new char[] { ',' });

            StringBuilder strOut = new StringBuilder();
            strOut.Append("<h3>Result</h3>");
            strOut.Append("<table cellpadding=\"4\" cellspacing=\"6\" border=\"0\" width=\"550\">");

            foreach (string strExt in strSplitExt)
            {
                strFullDomainToCheck = strDomainWithouExtToCheck + strExt;

                strOut.Append("     <tr>");
                strOut.Append("         <td width=\"250\">");
                strOut.Append("             <span id=\"domain_").Append(iCount).Append("\">");
                strOut.Append(strFullDomainToCheck);
                strOut.Append("             </span>");
                strOut.Append("         </td>");
                strOut.Append("         <td width=\"250\">");
                strOut.Append("             <span id=\"check_").Append(iCount).Append("\"></span><br />");
                strOut.Append("             <script language=\"javascript\" type=\"text/javascript\">CheckDomain('").Append(strDomainWithouExtToCheck).Append("','").Append(strExt).Append("','").Append(strCommand).Append("','check_").Append(iCount).Append("');</script>");
                strOut.Append("         </td>");
                strOut.Append("     </tr>");

                iCount++;
            }
            strOut.Append("</table>");

            return strOut.ToString();
        }

        

        public static bool CheckValidDomain(string strDomain){
            bool flag = false;  
            if(!string.IsNullOrEmpty(strDomain)){
                string DomainPattern = @"/^([a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?)$/i";  //Domain chỉ bao gồm ký tự,ký số, và dấu - (Không được có 2 dấu - liên tiếp)
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(DomainPattern);
                flag = regex.IsMatch(strDomain);
            }
            return flag;
        }

        public static int CountOccurencesOfChar(string strInput, char c) {
            int result = 0;
            if(!string.IsNullOrEmpty(strInput)){
                foreach (char curChar in strInput) {
                    if (c == curChar) {
                         result++;
                    }
                }
            }
            return result;
        }

        
    }
}