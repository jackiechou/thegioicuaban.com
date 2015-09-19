using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security.Membership;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Entities.Modules
{
    public class UserUserControlBase : UserModuleBase
    {
        public delegate void UserCreatedEventHandler(object sender, UserCreatedEventArgs e);
        public delegate void UserDeletedEventHandler(object sender, UserDeletedEventArgs e);
        public delegate void UserUpdateErrorEventHandler(object sender, UserUpdateErrorArgs e);
        public event UserCreatedEventHandler UserCreated;
        public event UserCreatedEventHandler UserCreateCompleted;
        public event UserDeletedEventHandler UserDeleted;
        public event UserUpdateErrorEventHandler UserDeleteError;
        public event EventHandler UserUpdated;
        public event EventHandler UserUpdateCompleted;
        public event UserUpdateErrorEventHandler UserUpdateError;
        public void OnUserCreateCompleted(UserCreatedEventArgs e)
        {
            if (UserCreateCompleted != null)
            {
                UserCreateCompleted(this, e);
            }
        }
        public void OnUserCreated(UserCreatedEventArgs e)
        {
            if (UserCreated != null)
            {
                UserCreated(this, e);
            }
        }
        public void OnUserDeleted(UserDeletedEventArgs e)
        {
            if (UserDeleted != null)
            {
                UserDeleted(this, e);
            }
        }
        public void OnUserDeleteError(UserUpdateErrorArgs e)
        {
            if (UserDeleteError != null)
            {
                UserDeleteError(this, e);
            }
        }
        public void OnUserUpdated(EventArgs e)
        {
            if (UserUpdated != null)
            {
                UserUpdated(this, e);
            }
        }
        public void OnUserUpdateCompleted(EventArgs e)
        {
            if (UserUpdateCompleted != null)
            {
                UserUpdateCompleted(this, e);
            }
        }
        public void OnUserUpdateError(UserUpdateErrorArgs e)
        {
            if (UserUpdateError != null)
            {
                UserUpdateError(this, e);
            }
        }
        public class BaseUserEventArgs
        {
            private string _userName;
            private int _userId;
            public BaseUserEventArgs()
            {
            }
            public int UserId
            {
                get { return _userId; }
                set { _userId = value; }
            }
            public string UserName
            {
                get { return _userName; }
                set { _userName = value; }
            }
        }
        public class UserCreatedEventArgs
        {
            private UserInfo _NewUser;
            private UserCreateStatus _CreateStatus = UserCreateStatus.Success;
            private bool _Notify = false;
            public UserCreatedEventArgs(UserInfo newUser)
            {
                _NewUser = newUser;
            }
            public UserCreateStatus CreateStatus
            {
                get { return _CreateStatus; }
                set { _CreateStatus = value; }
            }
            public UserInfo NewUser
            {
                get { return _NewUser; }
                set { _NewUser = value; }
            }
            public bool Notify
            {
                get { return _Notify; }
                set { _Notify = value; }
            }
        }
        public class UserDeletedEventArgs : BaseUserEventArgs
        {
            public UserDeletedEventArgs(int id, string name)
            {
                UserId = id;
                UserName = name;
            }
        }
        public class UserUpdateErrorArgs : BaseUserEventArgs
        {
            private string _message;
            public UserUpdateErrorArgs(int id, string name, string message)
            {
                UserId = id;
                UserName = name;
                _message = message;
            }
            public string Message
            {
                get { return _message; }
                set { _message = value; }
            }
        }
    }
}
