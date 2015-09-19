using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CommonLibrary.Entities.Modules;
using System.Security.Permissions;
using System.Xml.Serialization;

namespace CommonLibrary.Services.Exceptions
{
    [Serializable()]
    public class ModuleLoadException : BasePortalException
    {
        private int m_ModuleId;
        private int m_ModuleDefId;
        private string m_FriendlyName;
        private ModuleInfo m_ModuleConfiguration;
        private string m_ModuleControlSource;
        public ModuleLoadException()
            : base()
        {
        }
        public ModuleLoadException(string message)
            : base(message)
        {
            InitilizePrivateVariables();
        }
        public ModuleLoadException(string message, Exception inner, ModuleInfo ModuleConfiguration)
            : base(message, inner)
        {
            m_ModuleConfiguration = ModuleConfiguration;
            InitilizePrivateVariables();
        }
        public ModuleLoadException(string message, Exception inner)
            : base(message, inner)
        {
            InitilizePrivateVariables();
        }
        protected ModuleLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitilizePrivateVariables();
            m_ModuleId = info.GetInt32("m_ModuleId");
            m_ModuleDefId = info.GetInt32("m_ModuleDefId");
            m_FriendlyName = info.GetString("m_FriendlyName");
        }
        private void InitilizePrivateVariables()
        {
            if ((m_ModuleConfiguration != null))
            {
                m_ModuleId = m_ModuleConfiguration.ModuleID;
                //m_ModuleDefId = m_ModuleConfiguration.ModuleDefID;
                //m_FriendlyName = m_ModuleConfiguration.ModuleTitle;
                //m_ModuleControlSource = m_ModuleConfiguration.ModuleControl.ControlSrc;
            }
            else
            {
                m_ModuleId = -1;
                m_ModuleDefId = -1;
            }
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_ModuleId", m_ModuleId, typeof(Int32));
            info.AddValue("m_ModuleDefId", m_ModuleDefId, typeof(Int32));
            info.AddValue("m_FriendlyName", m_FriendlyName, typeof(string));
            info.AddValue("m_ModuleControlSource", m_ModuleControlSource, typeof(string));
            base.GetObjectData(info, context);
        }
        [XmlElement("ModuleID")]
        public int ModuleId
        {
            get { return m_ModuleId; }
        }
        [XmlElement("ModuleDefId")]
        public int ModuleDefId
        {
            get { return m_ModuleDefId; }
        }
        [XmlElement("FriendlyName")]
        public string FriendlyName
        {
            get { return m_FriendlyName; }
        }
        [XmlElement("ModuleControlSource")]
        public string ModuleControlSource
        {
            get { return m_ModuleControlSource; }
        }
    }
}
