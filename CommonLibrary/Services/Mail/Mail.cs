using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;
using System.Collections;
using CommonLibrary.Entities.Portal;

using CommonLibrary.Entities.Host;
using CommonLibrary.Common.Utilities;
using System.Net.Mail;
using CommonLibrary.Common;
using System.Threading;
using System.Web;

namespace CommonLibrary.Services.Mail
{
    public class Mail
    {
        public static string ConvertToText(string sHTML)
        {
            string sContent = sHTML;
            sContent = sContent.Replace("<br />", Environment.NewLine);
            sContent = sContent.Replace("<br>", Environment.NewLine);
            sContent = HtmlUtils.FormatText(sContent, true);
            return HtmlUtils.StripTags(sContent, true);
        }
       
        public static bool IsValidEmailAddress(string Email, int portalid)
        {
            string pattern = Null.NullString;
            if (portalid != Null.NullInteger)
            {
                pattern = Convert.ToString(Entities.Users.UserController.GetUserSettings(portalid)["Security_EmailValidation"]);
            }
            pattern = string.IsNullOrEmpty(pattern) ? Globals.glbEmailRegEx : pattern;
            return System.Text.RegularExpressions.Regex.Match(Email, pattern).Success;
        }
        public static string SendMail(UserInfo user, MessageType msgType, PortalSettings settings)
        {
            int toUser = user.UserID;
            string locale = user.Profile.PreferredLocale;
            string subject = "";
            string body = "";
            ArrayList custom = null;
            switch (msgType)
            {
                case MessageType.UserRegistrationAdmin:
                    subject = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_BODY";
                    toUser = settings.AdministratorId;
                    UserInfo admin = UserController.GetUserById(settings.PortalId, settings.AdministratorId);
                    locale = admin.Profile.PreferredLocale;
                    break;
                case MessageType.UserRegistrationPrivate:
                    subject = "EMAIL_USER_REGISTRATION_PRIVATE_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_PRIVATE_BODY";
                    break;
                case MessageType.UserRegistrationPublic:
                    subject = "EMAIL_USER_REGISTRATION_PUBLIC_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_PUBLIC_BODY";
                    break;
                case MessageType.UserRegistrationVerified:
                    subject = "EMAIL_USER_REGISTRATION_VERIFIED_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_VERIFIED_BODY";
                    if (HttpContext.Current != null)
                    {
                        custom = new ArrayList();
                        custom.Add(HttpContext.Current.Server.UrlEncode(user.Username));
                    }
                    break;
                case MessageType.PasswordReminder:
                    subject = "EMAIL_PASSWORD_REMINDER_SUBJECT";
                    body = "EMAIL_PASSWORD_REMINDER_BODY";
                    break;
                case MessageType.ProfileUpdated:
                    subject = "EMAIL_PROFILE_UPDATED_SUBJECT";
                    body = "EMAIL_PROFILE_UPDATED_BODY";
                    break;
                case MessageType.UserUpdatedOwnPassword:
                    subject = "EMAIL_USER_UPDATED_OWN_PASSWORD_SUBJECT";
                    body = "EMAIL_USER_UPDATED_OWN_PASSWORD_BODY";
                    break;
            }
            subject = Localization.Localization.GetSystemMessage(locale, settings, subject, user, Localization.Localization.GlobalResourceFile, custom, "", settings.AdministratorId);
            body = Localization.Localization.GetSystemMessage(locale, settings, body, user, Localization.Localization.GlobalResourceFile, custom, "", settings.AdministratorId);
            SendEmail(settings.Email, UserController.GetUserById(settings.PortalId, toUser).Email, subject, body);
            return "";
        }

        public static string SendMail(string MailFrom, string MailTo, string Bcc, string Subject, string Body, string Attachment, string BodyType, string SMTPServer, string SMTPAuthentication, string SMTPUsername,
        string SMTPPassword)
        {
            MailFormat objBodyFormat = MailFormat.Text;
            if (!String.IsNullOrEmpty(BodyType))
            {
                switch (BodyType.ToLower())
                {
                    case "html":
                        objBodyFormat = MailFormat.Html;
                        break;
                    case "text":
                        objBodyFormat = MailFormat.Text;
                        break;
                }
            }
            return SendMail(MailFrom, MailTo, "", Bcc, MailPriority.Normal, Subject, objBodyFormat, System.Text.Encoding.UTF8, Body, Attachment,
            SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword);
        }
        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, System.Text.Encoding BodyEncoding, string Body, string Attachment,
        string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword)
        {
            bool SMTPEnableSSL = Host.EnableSMTPSSL;
            return SendMail(MailFrom, MailTo, Cc, Bcc, Priority, Subject, BodyFormat, BodyEncoding, Body, Attachment,
            SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword, SMTPEnableSSL);
        }
        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, System.Text.Encoding BodyEncoding, string Body, string Attachment,
        string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            return SendMail(MailFrom, MailTo, Cc, Bcc, MailFrom, Priority, Subject, BodyFormat, BodyEncoding, Body,
            Attachment.Split('|'), SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword, SMTPEnableSSL);
        }
        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, System.Text.Encoding BodyEncoding, string Body, string[] Attachment,
        string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            return SendMail(MailFrom, MailTo, Cc, Bcc, MailFrom, Priority, Subject, BodyFormat, BodyEncoding, Body,
            Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword, SMTPEnableSSL);
        }
        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, string ReplyTo, MailPriority Priority, string Subject, MailFormat BodyFormat, System.Text.Encoding BodyEncoding, string Body,
        string[] Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            List<Attachment> attachments = new List<Attachment>();
            foreach (string myAtt in Attachment)
            {
                if (!String.IsNullOrEmpty(myAtt))
                    attachments.Add(new Attachment(myAtt));
            }
            return SendMail(MailFrom, MailTo, Cc, Bcc, ReplyTo, Priority, Subject, BodyFormat, BodyEncoding, Body,
            attachments, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword, SMTPEnableSSL);
        }
        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, string ReplyTo, MailPriority Priority, string Subject, MailFormat BodyFormat, System.Text.Encoding BodyEncoding, string Body,
        List<Attachment> Attachments, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            string retValue = "";
            if (!IsValidEmailAddress(MailFrom, PortalSettings.Current != null ? PortalSettings.Current.PortalId : Null.NullInteger))
            {
                ArgumentException ex = new ArgumentException(string.Format(Localization.Localization.GetString("EXCEPTION_InvalidEmailAddress", PortalSettings.Current), MailFrom));
                Exceptions.Exceptions.LogException(ex);
                return ex.Message;
            }

            if (string.IsNullOrEmpty(SMTPServer) && !string.IsNullOrEmpty(Host.SMTPServer))
            {
                SMTPServer = Host.SMTPServer;
            }
            if (string.IsNullOrEmpty(SMTPAuthentication) && !string.IsNullOrEmpty(Host.SMTPAuthentication))
            {
                SMTPAuthentication = Host.SMTPAuthentication;
            }
            if (string.IsNullOrEmpty(SMTPUsername) && !string.IsNullOrEmpty(Host.SMTPUsername))
            {
                SMTPUsername = Host.SMTPUsername;
            }
            if (string.IsNullOrEmpty(SMTPPassword) && !string.IsNullOrEmpty(Host.SMTPPassword))
            {
                SMTPPassword = Host.SMTPPassword;
            }
            MailTo = MailTo.Replace(";", ",");
            Cc = Cc.Replace(";", ",");
            Bcc = Bcc.Replace(";", ",");
            System.Net.Mail.MailMessage objMail = null;
            try
            {
                objMail = new System.Net.Mail.MailMessage();
                objMail.From = new MailAddress(MailFrom);
                if (!String.IsNullOrEmpty(MailTo))
                {
                    objMail.To.Add(MailTo);
                }
                if (!String.IsNullOrEmpty(Cc))
                {
                    objMail.CC.Add(Cc);
                }
                if (!String.IsNullOrEmpty(Bcc))
                {
                    objMail.Bcc.Add(Bcc);
                }
                if (ReplyTo != string.Empty)
                    objMail.ReplyTo = new System.Net.Mail.MailAddress(ReplyTo);
                objMail.Priority = (System.Net.Mail.MailPriority)Priority;
                objMail.IsBodyHtml = BodyFormat == MailFormat.Html;
                foreach (Attachment myAtt in Attachments)
                {
                    objMail.Attachments.Add(myAtt);
                }
                objMail.SubjectEncoding = BodyEncoding;
                objMail.Subject = HtmlUtils.StripWhiteSpace(Subject, true);
                objMail.BodyEncoding = BodyEncoding;
                System.Net.Mail.AlternateView PlainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(ConvertToText(Body), null, "text/plain");
                objMail.AlternateViews.Add(PlainView);
                if (HtmlUtils.IsHtml(Body))
                {
                    System.Net.Mail.AlternateView HTMLView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                    objMail.AlternateViews.Add(HTMLView);
                }
            }
            catch (Exception objException)
            {
                retValue = MailTo + ": " + objException.Message;
                Exceptions.Exceptions.LogException(objException);
            }
            if (objMail != null)
            {
                int SmtpPort = Null.NullInteger;
                int portPos = SMTPServer.IndexOf(":");
                if (portPos > -1)
                {
                    SmtpPort = Int32.Parse(SMTPServer.Substring(portPos + 1, SMTPServer.Length - portPos - 1));
                    SMTPServer = SMTPServer.Substring(0, portPos);
                }
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                try
                {
                    if (!String.IsNullOrEmpty(SMTPServer))
                    {
                        smtpClient.Host = SMTPServer;
                        if (SmtpPort > Null.NullInteger)
                        {
                            smtpClient.Port = SmtpPort;
                        }
                        switch (SMTPAuthentication)
                        {
                            case "":
                            case "0":
                                break;
                            case "1":
                                if (!String.IsNullOrEmpty(SMTPUsername) && !String.IsNullOrEmpty(SMTPPassword))
                                {
                                    smtpClient.UseDefaultCredentials = false;
                                    smtpClient.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
                                }
                                break;
                            case "2":
                                smtpClient.UseDefaultCredentials = true;
                                break;
                        }
                    }
                    smtpClient.EnableSsl = SMTPEnableSSL;
                    smtpClient.Send(objMail);
                    retValue = "";
                }
                catch (SmtpFailedRecipientException exc)
                {
                    retValue = string.Format(Localization.Localization.GetString("FailedRecipient"), exc.FailedRecipient);
                    Exceptions.Exceptions.LogException(exc);
                }
                catch (SmtpException exc)
                {
                    retValue = Localization.Localization.GetString("SMTPConfigurationProblem");
                    Exceptions.Exceptions.LogException(exc);
                }
                catch (Exception objException)
                {
                    if (objException.InnerException != null)
                    {
                        retValue = string.Concat(objException.Message, Environment.NewLine, objException.InnerException.Message);
                        Exceptions.Exceptions.LogException(objException.InnerException);
                    }
                    else
                    {
                        retValue = objException.Message;
                        Exceptions.Exceptions.LogException(objException);
                    }
                }
                finally
                {
                    objMail.Dispose();
                }
            }
            return retValue;
        }

        public static void SendEmail(string fromAddress, string toAddress, string subject, string body)
        {


            SendEmail(fromAddress, fromAddress, toAddress, subject, body);
        }
        public static void SendEmail(string fromAddress, string senderAddress, string toAddress, string subject, string body)
        {

            if ((string.IsNullOrEmpty(Host.SMTPServer)))
            {
                //throw new InvalidOperationException("SMTP Server not configured");
                return;
            }


            System.Net.Mail.MailMessage emailMessage = new System.Net.Mail.MailMessage(fromAddress, toAddress, subject, body);
            emailMessage.Sender = new MailAddress(senderAddress);

            if (HtmlUtils.IsHtml(body))
            {
                emailMessage.IsBodyHtml = true;
            }

            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(Host.SMTPServer);

            string[] smtpHostParts = Host.SMTPServer.Split(':');
            if (smtpHostParts.Length > 1)
            {
                smtpClient.Host = smtpHostParts[0];
                smtpClient.Port = Convert.ToInt32(smtpHostParts[1]);
            }


            switch (Host.SMTPAuthentication)
            {
                case "":
                case "0":
                    // anonymous
                    break;
                case "1":
                    // basic
                    if (!string.IsNullOrEmpty(Host.SMTPUsername) & !string.IsNullOrEmpty(Host.SMTPPassword))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new System.Net.NetworkCredential(Host.SMTPUsername, Host.SMTPPassword);
                    }
                    break;
                case "2":
                    // NTLM
                    smtpClient.UseDefaultCredentials = true;
                    break;
            }

            smtpClient.EnableSsl = Host.EnableSMTPSSL;

            //Retry up to 5 times to send the message
            for (int index = 0; index < 5; index++)
            {
                try
                {
                    smtpClient.Send(emailMessage);
                    return;
                }
                catch (Exception ex)
                {
                    if (index == 5)
                    {
                        ex.ToString();
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public static string SendEmail(string fromAddress, string senderAddress, string toAddress, string subject, string body, List<Attachment> Attachments)
        {
            if ((string.IsNullOrEmpty(Host.SMTPServer)))
            {
                return "SMTP Server not configured";
            }

            System.Net.Mail.MailMessage emailMessage = new System.Net.Mail.MailMessage(fromAddress, toAddress, subject, body);
            emailMessage.Sender = new MailAddress(senderAddress);

            if ((HtmlUtils.IsHtml(body)))
            {
                emailMessage.IsBodyHtml = true;
            }

            foreach (Attachment myAtt in Attachments)
            {
                emailMessage.Attachments.Add(myAtt);
            }

            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(Host.SMTPServer);

            string[] smtpHostParts = Host.SMTPServer.Split(':');
            if (smtpHostParts.Length > 1)
            {
                smtpClient.Host = smtpHostParts[0];
                smtpClient.Port = Convert.ToInt32(smtpHostParts[1]);
            }


            switch (Host.SMTPAuthentication)
            {
                case "":
                case "0":
                    // anonymous
                    break;
                case "1":
                    // basic
                    if (!string.IsNullOrEmpty(Host.SMTPUsername) & !string.IsNullOrEmpty(Host.SMTPPassword))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new System.Net.NetworkCredential(Host.SMTPUsername, Host.SMTPPassword);
                    }
                    break;
                case "2":
                    // NTLM
                    smtpClient.UseDefaultCredentials = true;
                    break;
            }

            smtpClient.EnableSsl = Host.EnableSMTPSSL;

            //'Retry up to 5 times to send the message
            for (int index = 1; index <= 5; index++)
            {
                try
                {
                    smtpClient.Send(emailMessage);
                    return "";
                }
                catch (Exception ex)
                {
                    if ((index == 5))
                    {
                        ex.ToString();
                    }
                    Thread.Sleep(1000);
                }
            }

            return "";

        }

        static internal bool RouteToUserMessaging(string MailFrom, string MailTo, string Cc, string Bcc, string Subject, string Body, List<Attachment> Attachments)
        {
            int totalRecords = -1;
            ArrayList fromUsersList = UserController.GetUsersByEmail(PortalSettings.Current.PortalId, MailFrom, -1, -1, ref totalRecords);
            UserInfo fromUser = default(UserInfo);
            if ((fromUsersList.Count != 0))
            {
                fromUser = (UserInfo)fromUsersList[0];
            }
            else
            {
                return false;
            }

            List<string> ToEmails = new List<string>();
            List<UserInfo> ToUsers = new List<UserInfo>();

            if ((!string.IsNullOrEmpty(MailTo)))
            {
                ToEmails.AddRange(MailTo.Split(';', ','));
            }

            if ((!string.IsNullOrEmpty(Cc)))
            {
                ToEmails.AddRange(Cc.Split(';', ','));
            }

            if ((!string.IsNullOrEmpty(Bcc)))
            {
                ToEmails.AddRange(Bcc.Split(';', ','));
            }

            foreach (string email in ToEmails)
            {
                if ((!string.IsNullOrEmpty(email)))
                {
                    ArrayList toUsersList = UserController.GetUsersByEmail(PortalSettings.Current.PortalId, email, -1, -1, ref totalRecords);
                    if ((toUsersList.Count != 0))
                    {
                        ToUsers.Add((UserInfo)toUsersList[0]);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            Messaging.MessagingController messageController = new Messaging.MessagingController();

            foreach (UserInfo recepient in ToUsers)
            {
                Messaging.Data.Message message = new Messaging.Data.Message();
                message.FromUserID = fromUser.UserID;
                message.Subject = Subject;
                message.Body = Body;
                message.ToUserID = recepient.UserID;
                message.Status = Messaging.Data.MessageStatusType.Unread;

                messageController.SaveMessage(message);

            }
            return true;
        }
    }
}
