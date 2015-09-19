using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.ComponentModel;
using CommonLibrary.Entities.Users;
using System.Xml.Serialization;

namespace CommonLibrary.Entities
{
    [Serializable()]
    public abstract class BaseEntityInfo
    {
        private int _CreatedByUserID = Null.NullInteger;
        private DateTime _CreatedOnDate;
        private int _LastModifiedByUserID = Null.NullInteger;
        private DateTime _LastModifiedOnDate;
        [Browsable(false), XmlIgnore()]
        public int CreatedByUserID
        {
            get { return _CreatedByUserID; }
        }
        [Browsable(false), XmlIgnore()]
        public DateTime CreatedOnDate
        {
            get { return _CreatedOnDate; }
        }
        [Browsable(false), XmlIgnore()]
        public int LastModifiedByUserID
        {
            get { return _LastModifiedByUserID; }
        }
        [Browsable(false), XmlIgnore()]
        public DateTime LastModifiedOnDate
        {
            get { return _LastModifiedOnDate; }
        }
        public UserInfo CreatedByUser(int portalId)
        {
            UserInfo _User = null;
            if (_CreatedByUserID > Null.NullInteger)
            {
                _User = UserController.GetUserById(portalId, _CreatedByUserID);
                return _User;
            }
            else
            {
                return null;
            }
        }

        public UserInfo LastModifiedByUser(int portalId)
        {
            UserInfo _User = null;
            if (_LastModifiedByUserID > Null.NullInteger)
            {
                _User = UserController.GetUserById(portalId, _LastModifiedByUserID);
                return _User;
            }
            else
            {
                return null;
            }
        }

        protected virtual void FillInternal(System.Data.IDataReader dr)
        {
            _CreatedByUserID = Null.SetNullInteger(dr["CreatedByUserID"]);
            _CreatedOnDate = Null.SetNullDateTime(dr["CreatedOnDate"]);
            _LastModifiedByUserID = Null.SetNullInteger(dr["LastModifiedByUserID"]);
            _LastModifiedOnDate = Null.SetNullDateTime(dr["LastModifiedOnDate"]);
        }
        internal void FillBaseProperties(System.Data.IDataReader dr)
        {
            FillInternal(dr);
        }
    }
}
