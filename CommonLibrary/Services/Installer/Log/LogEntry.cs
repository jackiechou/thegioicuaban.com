using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Installer.Log
{
    [Serializable()]
    public class LogEntry
    {
        private LogType m_Type;
        private string m_Description;
        public LogEntry(LogType type, string description)
        {
            m_Type = type;
            m_Description = description;
        }
        public LogType Type
        {
            get { return m_Type; }
        }
        public string Description
        {
            get
            {
                if (m_Description == null)
                {
                    return "...";
                }
                else
                {
                    return m_Description;
                }
            }
        }
    }
}
