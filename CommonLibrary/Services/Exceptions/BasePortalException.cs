using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Web;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Application;

namespace CommonLibrary.Services.Exceptions
{
    [Serializable()]
    public class BasePortalException : Exception
    {
        private string m_AssemblyVersion;
        private int m_PortalID;
        private string m_PortalName;
        private int m_UserID;
        private string m_UserName;
        private int m_ActiveTabID;
        private string m_ActiveTabName;
        private string m_RawURL;
        private string m_AbsoluteURL;
        private string m_AbsoluteURLReferrer;
        private string m_UserAgent;
        private string m_DefaultDataProvider;
        private string m_ExceptionGUID;
        private string m_InnerExceptionString;
        private string m_FileName;
        private int m_FileLineNumber;
        private int m_FileColumnNumber;
        private string m_Method;
        private string m_StackTrace;
        private string m_Message;
        private string m_Source;
        public BasePortalException()
            : base()
        {
        }
        public BasePortalException(string message)
            : base(message)
        {
           // InitializePrivateVariables();
        }
        public BasePortalException(string message, Exception inner)
            : base(message, inner)
        {
          //  InitializePrivateVariables();
        }
        protected BasePortalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
          //  InitializePrivateVariables();
            m_AssemblyVersion = info.GetString("m_AssemblyVersion");
            m_PortalID = info.GetInt32("m_PortalID");
            m_PortalName = info.GetString("m_PortalName");
            m_UserID = info.GetInt32("m_UserID");
            m_UserName = info.GetString("m_Username");
            m_ActiveTabID = info.GetInt32("m_ActiveTabID");
            m_ActiveTabName = info.GetString("m_ActiveTabName");
            m_RawURL = info.GetString("m_RawURL");
            m_AbsoluteURL = info.GetString("m_AbsoluteURL");
            m_AbsoluteURLReferrer = info.GetString("m_AbsoluteURLReferrer");
            m_UserAgent = info.GetString("m_UserAgent");
            m_DefaultDataProvider = info.GetString("m_DefaultDataProvider");
            m_ExceptionGUID = info.GetString("m_ExceptionGUID");
            m_InnerExceptionString = info.GetString("m_InnerExceptionString");
            m_FileName = info.GetString("m_FileName");
            m_FileLineNumber = info.GetInt32("m_FileLineNumber");
            m_FileColumnNumber = info.GetInt32("m_FileColumnNumber");
            m_Method = info.GetString("m_Method");
            m_StackTrace = info.GetString("m_StackTrace");
            m_Message = info.GetString("m_Message");
            m_Source = info.GetString("m_Source");
        }
        private void InitializePrivateVariables()
        {
            try
            {
                HttpContext _context = HttpContext.Current;
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                Exception _objInnermostException;
                _objInnermostException = new Exception(this.Message, this);
                while (_objInnermostException.InnerException != null)
                {
                    _objInnermostException = _objInnermostException.InnerException;
                }
                ExceptionInfo _exceptionInfo = Exceptions.GetExceptionInfo(_objInnermostException);
                //try
                //{
                //    m_AssemblyVersion = AppContext.Current.Application.Version.ToString(3);
                //}
                //catch
                //{
                //    m_AssemblyVersion = "-1";
                //}
                try
                {
                    m_PortalID = _portalSettings.PortalId;
                    m_PortalName = _portalSettings.PortalName;
                }
                catch
                {
                    m_PortalID = -1;
                    m_PortalName = "";
                }
                try
                {
                    UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                    m_UserID = objUserInfo.UserID;
                }
                catch
                {
                    m_UserID = -1;
                }
                try
                {
                    if (m_UserID != -1)
                    {
                        UserInfo objUserInfo = UserController.GetUserById(m_PortalID, m_UserID);
                        if (objUserInfo != null)
                        {
                            m_UserName = objUserInfo.Username;
                        }
                        else
                        {
                            m_UserName = "";
                        }
                    }
                    else
                    {
                        m_UserName = "";
                    }
                }
                catch
                {
                    m_UserName = "";
                }
                //try
                //{
                //    m_ActiveTabID = _portalSettings.ActiveTab.TabID;
                //}
                //catch (Exception ex)
                //{
                //    m_ActiveTabID = -1;
                //}
                try
                {
                    m_ActiveTabName = _portalSettings.ActiveTab.TabName;
                }
                catch (Exception ex)
                {
                    m_ActiveTabName = "";
                    ex.ToString();
                }
                try
                {
                    m_RawURL = _context.Request.RawUrl;
                }
                catch
                {
                    m_RawURL = "";
                }
                try
                {
                    m_AbsoluteURL = _context.Request.Url.AbsolutePath;
                }
                catch
                {
                    m_AbsoluteURL = "";
                }
                try
                {
                    m_AbsoluteURLReferrer = _context.Request.UrlReferrer.AbsoluteUri;
                }
                catch
                {
                    m_AbsoluteURLReferrer = "";
                }
                try
                {
                    m_UserAgent = _context.Request.UserAgent;
                }
                catch (Exception ex)
                {
                    m_UserAgent = "";
                    ex.ToString();
                }
                try
                {
                    Framework.Providers.ProviderConfiguration objProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data");
                    string strTypeName = ((Framework.Providers.Provider)objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider]).Type;
                    m_DefaultDataProvider = strTypeName;
                }
                catch (Exception ex)
                {
                    m_DefaultDataProvider = "";
                    ex.ToString();
                }
                try
                {
                    m_ExceptionGUID = Guid.NewGuid().ToString();
                }
                catch (Exception ex)
                {
                    m_ExceptionGUID = "";
                    ex.ToString();
                }
                try
                {
                    m_FileName = _exceptionInfo.FileName;
                }
                catch
                {
                    m_FileName = "";
                }
                try
                {
                    m_FileLineNumber = _exceptionInfo.FileLineNumber;
                }
                catch
                {
                    m_FileLineNumber = -1;
                }
                try
                {
                    m_FileColumnNumber = _exceptionInfo.FileColumnNumber;
                }
                catch
                {
                    m_FileColumnNumber = -1;
                }
                try
                {
                    m_Method = _exceptionInfo.Method;
                }
                catch
                {
                    m_Method = "";
                }
                try
                {
                    m_StackTrace = this.StackTrace;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    m_StackTrace = "";
                }
                try
                {
                    m_Message = this.Message;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    m_Message = "";
                }
                try
                {
                    m_Source = this.Source;
                }
                catch (Exception ex)
                {
                    m_Source = "";
                    ex.ToString();
                }
            }
            catch (Exception exc)
            {
                m_PortalID = -1;
                m_UserID = -1;
                m_AssemblyVersion = "-1";
                m_ActiveTabID = -1;
                m_ActiveTabName = "";
                m_RawURL = "";
                m_AbsoluteURL = "";
                m_AbsoluteURLReferrer = "";
                m_UserAgent = "";
                m_DefaultDataProvider = "";
                m_ExceptionGUID = "";
                m_FileName = "";
                m_FileLineNumber = -1;
                m_FileColumnNumber = -1;
                m_Method = "";
                m_StackTrace = "";
                m_Message = "";
                m_Source = "";
                exc.ToString();
            }
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_AssemblyVersion", m_AssemblyVersion, typeof(string));
            info.AddValue("m_PortalID", m_PortalID, typeof(Int32));
            info.AddValue("m_PortalName", m_PortalName, typeof(string));
            info.AddValue("m_UserID", m_UserID, typeof(Int32));
            info.AddValue("m_UserName", m_UserName, typeof(string));
            info.AddValue("m_ActiveTabID", m_ActiveTabID, typeof(Int32));
            info.AddValue("m_ActiveTabName", m_ActiveTabName, typeof(string));
            info.AddValue("m_RawURL", m_RawURL, typeof(string));
            info.AddValue("m_AbsoluteURL", m_AbsoluteURL, typeof(string));
            info.AddValue("m_AbsoluteURLReferrer", m_AbsoluteURLReferrer, typeof(string));
            info.AddValue("m_UserAgent", m_UserAgent, typeof(string));
            info.AddValue("m_DefaultDataProvider", m_DefaultDataProvider, typeof(string));
            info.AddValue("m_ExceptionGUID", m_ExceptionGUID, typeof(string));
            info.AddValue("m_FileName", m_FileName, typeof(string));
            info.AddValue("m_FileLineNumber", m_FileLineNumber, typeof(Int32));
            info.AddValue("m_FileColumnNumber", m_FileColumnNumber, typeof(Int32));
            info.AddValue("m_Method", m_Method, typeof(string));
            info.AddValue("m_StackTrace", m_StackTrace, typeof(string));
            info.AddValue("m_Message", m_Message, typeof(string));
            info.AddValue("m_Source", m_Source, typeof(string));
            base.GetObjectData(info, context);
        }
        public string AssemblyVersion
        {
            get { return m_AssemblyVersion; }
        }
        public int PortalID
        {
            get { return m_PortalID; }
        }
        public string PortalName
        {
            get { return m_PortalName; }
        }
        public int UserID
        {
            get { return m_UserID; }
        }
        public string UserName
        {
            get { return m_UserName; }
        }
        public int ActiveTabID
        {
            get { return m_ActiveTabID; }
        }
        public string ActiveTabName
        {
            get { return m_ActiveTabName; }
        }
        public string RawURL
        {
            get { return m_RawURL; }
        }
        public string AbsoluteURL
        {
            get { return m_AbsoluteURL; }
        }
        public string AbsoluteURLReferrer
        {
            get { return m_AbsoluteURLReferrer; }
        }
        public string UserAgent
        {
            get { return m_UserAgent; }
        }
        public string DefaultDataProvider
        {
            get { return m_DefaultDataProvider; }
        }
        public string ExceptionGUID
        {
            get { return m_ExceptionGUID; }
        }
        public string FileName
        {
            get { return m_FileName; }
        }
        public int FileLineNumber
        {
            get { return m_FileLineNumber; }
        }
        public int FileColumnNumber
        {
            get { return m_FileColumnNumber; }
        }
        public string Method
        {
            get { return m_Method; }
        }
        [XmlIgnore()]
        public new MethodBase TargetSite
        {
            get { return base.TargetSite; }
        }
    }
}
