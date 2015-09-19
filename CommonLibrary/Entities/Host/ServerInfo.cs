using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Entities.Host
{
    public class ServerInfo : IHydratable
    {
        private string _IISAppName;
        private int _ServerID;
        private string _ServerName;
        private string _Url;
        private bool _Enabled;
        private DateTime _CreatedDate;
        private DateTime _LastActivityDate;
        public ServerInfo()
            : this(DateTime.Now, DateTime.Now)
        {
        }
        public ServerInfo(DateTime created, DateTime lastactivity)
        {
            _IISAppName = Globals.IISAppName;
            _ServerName = Globals.ServerName;
            _CreatedDate = created;
            _LastActivityDate = lastactivity;
        }
        public int ServerID
        {
            get { return _ServerID; }
            set { _ServerID = value; }
        }
        public string IISAppName
        {
            get { return _IISAppName; }
            set { _IISAppName = value; }
        }
        public string ServerName
        {
            get { return _ServerName; }
            set { _ServerName = value; }
        }
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }
        public System.DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }
        public System.DateTime LastActivityDate
        {
            get { return _LastActivityDate; }
            set { _LastActivityDate = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            ServerID = Null.SetNullInteger(dr["ServerID"]);
            IISAppName = Null.SetNullString(dr["IISAppName"]);
            ServerName = Null.SetNullString(dr["ServerName"]);
            Url = Null.SetNullString(dr["URL"]);
            Enabled = Null.SetNullBoolean(dr["Enabled"]);
            CreatedDate = Null.SetNullDateTime(dr["CreatedDate"]);
            LastActivityDate = Null.SetNullDateTime(dr["LastActivityDate"]);
        }
        public int KeyID
        {
            get { return ServerID; }
            set { ServerID = value; }
        }
    }
}
