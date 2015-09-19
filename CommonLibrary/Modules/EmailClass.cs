using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules
{
    public class EmailClass
    {
        public EmailClass()
        {
        }

        public static bool send_mail(string gmail_sender_account, string gmail_sender_pass, string sender_name, string sender_email, string receiver_name, string receiver_email, string subject, string body_content)
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

            smtp.Send(mailMessage);
            flag = true;
            return flag;
        }


        public static bool send_mail_gmail(string gmail_sender_account, string gmail_sender_pass, string sender_name, string sender_email, string receiver_name, string receiver_email, string subject, string body_content)
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
            smtp.Timeout = 100;
            smtp.EnableSsl = true;
            smtp.Credentials = smtp_user_info;

            try
            {
                smtp.Send(mailMessage);               
                flag = true;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return flag;
        }        

        public static bool send_mail_yahoo(string yahoo_sender_account, string yahoo_sender_pass,  string sender_name, string receiver_name, string sender_email, string receiver_email, string subject, string body_content)
        {
            bool flag = false;
            System.Net.NetworkCredential smtp_user_info = new System.Net.NetworkCredential(yahoo_sender_account, yahoo_sender_pass);

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
            //Khai báo Server Port Numbers cho Outgoing mail là 465, cho Incoming mail là 995.
            //Khai báo Incoming mail là pop.mailyahoo.com.vn [cư trú là Việt Nam] và Port là 995.
            //Khai báo Outgoing mail là smtp.yahoo.com.vn và Port là 465.
            //Khai báo Incoming mail là pop.mailyahoo.com và Port là 110.
            //Khai báo Outgoing mail là smtp.mail.yahoo.com và Port là 25.

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.mail.yahoo.com";
            smtp.Port = 465;
            smtp.Timeout = 100;
            smtp.EnableSsl = true;
            smtp.Credentials = smtp_user_info;

            try
            {
                smtp.Send(mailMessage);               
                flag = true;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return flag;
        }

        public static string Send_Email_With_Attachment(string SendTo, string SendFrom, string Subject, string Body, string AttachmentPath)
        {
            try
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

                string from = SendFrom;
                string to = SendTo;
                string subject = Subject;
                string body = Body;

                bool result = regex.IsMatch(to);

                if (result == false)
                {
                    return "Địa chỉ email không hợp lệ.";
                }
                else
                {
                    try
                    {
                        System.Net.Mail.MailMessage em = new System.Net.Mail.MailMessage(from, to, subject, body);
                        System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(AttachmentPath);

                        em.Attachments.Add(attach);
                        em.Bcc.Add(from);
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = "smtp.gmail.com";//Ví dụ xử dụng SMTP của gmail                    
                        smtp.Send(em);
                        return "";
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Send_Email_With_BCC_Attachment(string SendTo, string SendBCC, string SendFrom, string Subject, string Body, string AttachmentPath)
        {
            try
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                string from = SendFrom;
                string to = SendTo; //Danh sách email được ngăn cách nhau bởi dấu ";"
                string subject = Subject;
                string body = Body;
                string bcc = SendBCC;

                bool result = true;
                String[] ALL_EMAILS = to.Split(';');

                foreach (string emailaddress in ALL_EMAILS)
                {
                    result = regex.IsMatch(emailaddress);
                    if (result == false)
                    {
                        return "Địa chỉ email không hợp lệ.";
                    }
                }

                if (result == true)
                {
                    try
                    {
                        System.Net.Mail.MailMessage em = new System.Net.Mail.MailMessage(from, to, subject, body);
                        System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(AttachmentPath);
                        em.Attachments.Add(attach);
                        em.Bcc.Add(bcc);

                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = "smtp.gmail.com";//Ví dụ xử dụng SMTP của gmail
                        smtp.Send(em);

                        return "";
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
