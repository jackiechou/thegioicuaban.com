using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules.Dashboard.Components.Database
{
    public class BackupInfo
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        private DateTime _FinishDate;
        public DateTime FinishDate
        {
            get { return _FinishDate; }
            set { _FinishDate = value; }
        }
        private long _Size;
        public long Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        private string _BackupType;
        public string BackupType
        {
            get { return _BackupType; }
            set { _BackupType = value; }
        }
    }
}
