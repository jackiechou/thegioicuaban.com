﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Security.Membership
{
    public enum UserCreateStatus
    {
        AddUser = 0,
        UsernameAlreadyExists = 1,
        UserAlreadyRegistered = 2,
        DuplicateEmail = 3,
        DuplicateProviderUserKey = 4,
        DuplicateUserName = 5,
        InvalidAnswer = 6,
        InvalidEmail = 7,
        InvalidPassword = 8,
        InvalidProviderUserKey = 9,
        InvalidQuestion = 10,
        InvalidUserName = 11,
        ProviderError = 12,
        Success = 13,
        UnexpectedError = 14,
        UserRejected = 15,
        PasswordMismatch = 16,
        AddUserToPortal = 17
    }
}
