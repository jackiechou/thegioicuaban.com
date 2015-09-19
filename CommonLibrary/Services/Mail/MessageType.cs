using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Mail
{
    public enum MessageType
    {
        PasswordReminder,
        ProfileUpdated,
        UserRegistrationAdmin,
        UserRegistrationPrivate,
        UserRegistrationPublic,
        UserRegistrationVerified,
        UserUpdatedOwnPassword
    }
}
