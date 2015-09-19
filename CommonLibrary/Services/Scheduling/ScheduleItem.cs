using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLibrary.Common.Utilities;
using System.Collections;
using CommonLibrary.Entities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Services.Scheduling
{
    [Serializable()]
    public class ScheduleItem : BaseEntityInfo, IHydratable
    {
        private int _ScheduleID;
        private string _FriendlyName;
        private string _TypeFullName;
        private int _TimeLapse;
        private string _TimeLapseMeasurement;
        private int _RetryTimeLapse;
        private string _RetryTimeLapseMeasurement;
        private string _ObjectDependencies;
        private int _RetainHistoryNum;
        private System.DateTime _NextStart;
        private bool _CatchUpEnabled;
        private bool _Enabled;
        private string _AttachToEvent;
        private int _ThreadID;
        private int _ProcessGroup;
        private ScheduleSource _ScheduleSource;
        private Hashtable _ScheduleItemSettings;
        private string _Servers;
        public ScheduleItem()
        {
            _ScheduleID = Null.NullInteger;
            _TypeFullName = Null.NullString;
            _TimeLapse = Null.NullInteger;
            _TimeLapseMeasurement = Null.NullString;
            _RetryTimeLapse = Null.NullInteger;
            _RetryTimeLapseMeasurement = Null.NullString;
            _ObjectDependencies = Null.NullString;
            _RetainHistoryNum = Null.NullInteger;
            _NextStart = Null.NullDate;
            _CatchUpEnabled = Null.NullBoolean;
            _Enabled = Null.NullBoolean;
            _AttachToEvent = Null.NullString;
            _ThreadID = Null.NullInteger;
            _ProcessGroup = Null.NullInteger;
            _Servers = Null.NullString;
        }
        public string AttachToEvent
        {
            get { return _AttachToEvent; }
            set { _AttachToEvent = value; }
        }
        public bool CatchUpEnabled
        {
            get { return _CatchUpEnabled; }
            set { _CatchUpEnabled = value; }
        }
        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set { _FriendlyName = value; }
        }
        public System.DateTime NextStart
        {
            get
            {
                if (_NextStart == Null.NullDate)
                {
                    _NextStart = DateTime.Now;
                }
                return _NextStart;
            }
            set { _NextStart = value; }
        }
        public string ObjectDependencies
        {
            get { return _ObjectDependencies; }
            set { _ObjectDependencies = value; }
        }
        public int RetainHistoryNum
        {
            get { return _RetainHistoryNum; }
            set { _RetainHistoryNum = value; }
        }
        public int RetryTimeLapse
        {
            get { return _RetryTimeLapse; }
            set { _RetryTimeLapse = value; }
        }
        public string RetryTimeLapseMeasurement
        {
            get { return _RetryTimeLapseMeasurement; }
            set { _RetryTimeLapseMeasurement = value; }
        }
        public int ScheduleID
        {
            get { return _ScheduleID; }
            set { _ScheduleID = value; }
        }
        public string Servers
        {
            get { return _Servers; }
            set { _Servers = value; }
        }
        public int TimeLapse
        {
            get { return _TimeLapse; }
            set { _TimeLapse = value; }
        }
        public string TimeLapseMeasurement
        {
            get { return _TimeLapseMeasurement; }
            set { _TimeLapseMeasurement = value; }
        }
        public string TypeFullName
        {
            get { return _TypeFullName; }
            set { _TypeFullName = value; }
        }
        public bool HasObjectDependencies(string strObjectDependencies)
        {
            if (strObjectDependencies.IndexOf(",") > -1)
            {
                string[] a;
                a = strObjectDependencies.ToLower().Split(',');
                int i;
                for (i = 0; i <= a.Length - 1; i++)
                {
                    if (ObjectDependencies.ToLower().IndexOf(a[i].Trim()) > -1)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (ObjectDependencies.ToLower().IndexOf(strObjectDependencies.ToLower()) > -1)
                {
                    return true;
                }
            }
            return false;
        }
        public int ProcessGroup
        {
            get { return _ProcessGroup; }
            set { _ProcessGroup = value; }
        }
        public ScheduleSource ScheduleSource
        {
            get { return _ScheduleSource; }
            set { _ScheduleSource = value; }
        }
        public int ThreadID
        {
            get { return _ThreadID; }
            set { _ThreadID = value; }
        }
        public void AddSetting(string Key, string Value)
        {
            _ScheduleItemSettings.Add(Key, Value);
        }
        public string GetSetting(string Key)
        {
            if (_ScheduleItemSettings == null)
            {
                GetSettings();
            }
            if (_ScheduleItemSettings.ContainsKey(Key))
            {
                return Convert.ToString(_ScheduleItemSettings[Key]);
            }
            else
            {
                return "";
            }
        }
        public Hashtable GetSettings()
        {
            _ScheduleItemSettings = SchedulingProvider.Instance().GetScheduleItemSettings(this.ScheduleID);
            return _ScheduleItemSettings;
        }
        public int KeyID
        {
            get { return ScheduleID; }
            set { ScheduleID = value; }
        }
        public virtual void Fill(IDataReader dr)
        {
            FillInternal(dr);
        }
        protected override void FillInternal(IDataReader dr)
        {
            ScheduleID = Null.SetNullInteger(dr["ScheduleID"]);
            FriendlyName = Null.SetNullString(dr["FriendlyName"]);
            TypeFullName = Null.SetNullString(dr["TypeFullName"]);
            TimeLapse = Null.SetNullInteger(dr["TimeLapse"]);
            TimeLapseMeasurement = Null.SetNullString(dr["TimeLapseMeasurement"]);
            RetryTimeLapse = Null.SetNullInteger(dr["RetryTimeLapse"]);
            RetryTimeLapseMeasurement = Null.SetNullString(dr["RetryTimeLapseMeasurement"]);
            ObjectDependencies = Null.SetNullString(dr["ObjectDependencies"]);
            AttachToEvent = Null.SetNullString(dr["AttachToEvent"]);
            RetainHistoryNum = Null.SetNullInteger(dr["RetainHistoryNum"]);
            CatchUpEnabled = Null.SetNullBoolean(dr["CatchUpEnabled"]);
            Enabled = Null.SetNullBoolean(dr["Enabled"]);
            Servers = Null.SetNullString(dr["Servers"]);
            try
            {
                NextStart = Null.SetNullDateTime(dr["NextStart"]);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            base.FillInternal(dr);
        }
    }
}
