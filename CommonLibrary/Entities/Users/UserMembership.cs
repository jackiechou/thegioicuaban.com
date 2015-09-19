using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes;

namespace CommonLibrary.Entities.Users
{
    [Serializable()]
    public class UserMembership
    {
        private bool _Approved = true;
        private System.DateTime _CreatedDate;
        private bool _IsOnLine;
        private System.DateTime _LastActivityDate;
        private System.DateTime _LastLockoutDate;
        private System.DateTime _LastLoginDate;
        private System.DateTime _LastPasswordChangeDate;
        private bool _LockedOut = false;
        private bool _ObjectHydrated;
        private string _Password;
        private string _PasswordAnswer;
        private string _PasswordQuestion;
        private bool _UpdatePassword;
        private bool IsSuperUser;
        private UserInfo _User;
        public UserMembership(UserInfo user)
        {
            _User = user;
        }
        public UserMembership()
        {
            _User = new UserInfo();
        }

        [SortOrder(9)]
        public bool Approved
        {
            get { return _Approved; }
            set { _Approved = value; }
        }
        [SortOrder(1), IsReadOnly(true)]
        public System.DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }
        [SortOrder(7)]
        public bool IsOnLine
        {
            get { return _IsOnLine; }
            set { _IsOnLine = value; }
        }
        [SortOrder(3), IsReadOnly(true)]
        public System.DateTime LastActivityDate
        {
            get { return _LastActivityDate; }
            set { _LastActivityDate = value; }
        }
        [SortOrder(5), IsReadOnly(true)]
        public System.DateTime LastLockoutDate
        {
            get { return _LastLockoutDate; }
            set { _LastLockoutDate = value; }
        }
        [SortOrder(2), IsReadOnly(true)]
        public System.DateTime LastLoginDate
        {
            get { return _LastLoginDate; }
            set { _LastLoginDate = value; }
        }
        [SortOrder(4), IsReadOnly(true)]
        public System.DateTime LastPasswordChangeDate
        {
            get { return _LastPasswordChangeDate; }
            set { _LastPasswordChangeDate = value; }
        }
        [SortOrder(8)]
        public bool LockedOut
        {
            get { return _LockedOut; }
            set { _LockedOut = value; }
        }
        [Browsable(false)]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        [Browsable(false)]
        public string PasswordAnswer
        {
            get { return _PasswordAnswer; }
            set { _PasswordAnswer = value; }
        }
        [Browsable(false)]
        public string PasswordQuestion
        {
            get { return _PasswordQuestion; }
            set { _PasswordQuestion = value; }
        }
        [SortOrder(10)]
        public bool UpdatePassword
        {
            get { return _UpdatePassword; }
            set { _UpdatePassword = value; }
        }
    }
}
