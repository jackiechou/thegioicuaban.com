using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;

namespace CommonLibrary.Services.Exceptions
{
    [Serializable()]
    public class SecurityException : BasePortalException
    {
        private string m_IP;
        private string m_Querystring;
        public SecurityException()
            : base()
        {

        }
        public SecurityException(string message)
            : base(message)
        {
            InitilizePrivateVariables();
        }
        public SecurityException(string message, Exception inner)
            : base(message, inner)
        {
            InitilizePrivateVariables();
        }
        protected SecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitilizePrivateVariables();
            m_IP = info.GetString("m_IP");
            m_Querystring = info.GetString("m_Querystring");
        }
        private void InitilizePrivateVariables()
        {
            try
            {
                if (HttpContext.Current.Request.UserHostAddress != null)
                {
                    m_IP = HttpContext.Current.Request.UserHostAddress;
                }
                m_Querystring = HttpContext.Current.Request.MapPath(Querystring, HttpContext.Current.Request.ApplicationPath, false);
            }
            catch (Exception exc)
            {
                m_IP = "";
                m_Querystring = "";
                exc.ToString();
            }
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_IP", m_IP, typeof(string));
            info.AddValue("m_Querystring", m_Querystring, typeof(string));
            base.GetObjectData(info, context);
        }
        [XmlElement("IP")]
        public string IP
        {
            get { return m_IP; }
        }
        [XmlElement("Querystring")]
        public string Querystring
        {
            get { return m_Querystring; }
        }
    }
}
