using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using CommonLibrary.Modules;
using System.Configuration;
using System.Web.Services;

namespace WebApp.portals.news
{
    public partial class contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }
       
        protected void btnSend_Click(object sender, EventArgs e)
        {
            string sender_email = ConfigurationManager.AppSettings["mailaddress"].ToString();
            string sender_account = ConfigurationManager.AppSettings["mailaccount"].ToString();
            string sender_password = ConfigurationManager.AppSettings["mailpassword"].ToString();

            string receiver_name = txtName.Text;
            string receiver_email = txtFrom.Text;

            string subject = txtSubject.Text;
            string body_content = txtBody.Text;

            if (receiver_name == string.Empty || receiver_email == string.Empty || txtSubject.Text == string.Empty || body_content == string.Empty)
            {
                string scriptCode = "<script>alert('Vui lòng điền đầy đủ thông tin.');</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
            }
            else
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                bool result_sender_email = regex.IsMatch(sender_email);
                bool result_receiver_email = regex.IsMatch(receiver_email);
                if (result_sender_email == false)
                {
                    string scriptCode = "<script>alert('Địa chỉ email của người gửi không hợp lệ.');</script>";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                }
                else if (result_receiver_email == false)
                {                    
                    string scriptCode = "<script>alert('Địa chỉ email của người nhận không hợp lệ.');</script>";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                }
                else
                {
                    if (hiddenCaptcha.Value != null)
                    {
                        if (txtCaptcha.Text == hiddenCaptcha.Value)
                        {
                            if (send_mail_gmail_with_attachment(sender_account, sender_password, receiver_name, sender_email, receiver_name, receiver_email, subject, body_content))
                            {                                
                                string scriptCode = "<script>alert('Thông điệp đã được gửi đi.');document.location='/tin-tuc/';</script>";
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                            }
                            else
                            {                     
                                string scriptCode = "<script>alert('Lỗi trong tiến trình gửi.');</script>";
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                            }
                        }
                        else if (txtCaptcha.Text != hiddenCaptcha.Value)
                        {
                            lblResult.Text = "Vui lòng thử lại lần nữa &nbsp;<div style=\"float:left; background:url(images/icons/OK_not_OK_Icons.png);background-position:100%;height:30px; width:30px\"></div>";
                            txtCaptcha.Text = "";
                        }
                        else
                        {
                            lblResult.Text = "";
                            hiddenCaptcha.Value = null;
                        }
                    }
                    else
                    {
                        lblResult.Text = "Session timeout &nbsp;<div style=\"float:left; background:url(images/icons/OK_not_OK_Icons.png);background-position:100%;height:30px; width:30px\"></div>";
                        txtCaptcha.Text = "";
                    }
                }

            }
        }

        public bool send_mail_gmail_with_attachment(string gmail_sender_account, string gmail_sender_pass, string sender_name, string sender_email, string receiver_name, string receiver_email, string subject, string body_content)
        {
            bool flag = false;
            System.Net.NetworkCredential smtp_user_info = new System.Net.NetworkCredential(gmail_sender_account, gmail_sender_pass);

            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(sender_email, sender_name, System.Text.UTF8Encoding.UTF8);
            mailMessage.To.Add(new System.Net.Mail.MailAddress(receiver_email, receiver_name.Trim(), System.Text.UTF8Encoding.UTF8));
            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.Body = body_content;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = System.Text.UnicodeEncoding.UTF8;
            //mailMessage.Priority = MailPriority.High;

            /* Set the SMTP server and send the email - SMTP gmail ="smtp.gmail.com" port=587*/
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587; //port=25           
            smtp.Timeout = 100000;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = smtp_user_info;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            

            /* Beginning of Attachment1 process   & Check the first open file dialog for a attachment */
            string attach1 = null;
            string strFileName = null;
            if (inpAttachment.PostedFile != null)
            {
                /* Get a reference to PostedFile object */
                HttpPostedFile attFile = inpAttachment.PostedFile;
                /* Get size of the file */
                int attachFileLength = attFile.ContentLength;
                /* Make sure the size of the file is > 0  */
                if (attachFileLength > 0)
                {
                    /* Get the file name */
                    strFileName = Path.GetFileName(inpAttachment.PostedFile.FileName);
                    /* Save the file on the server */
                    inpAttachment.PostedFile.SaveAs(Server.MapPath(strFileName));
                    /* Create the email attachment with the uploaded file */
                    Attachment attachment = new Attachment(Server.MapPath(strFileName));
                    /* Attach the newly created email attachment */
                    mailMessage.Attachments.Add(attachment);
                    /* Store the attach filename so we can delete it later */
                    attach1 = strFileName;
                }
            }
            
            smtp.Send(mailMessage);            
            flag = true;

            /* Delete the attachements if any */
            if (attach1 != null)
                File.Delete(Server.MapPath(attach1));
           
            return flag;
        }
    }
}