using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonLibrary.Security.Membership
{
    public class MembershipProviderConfig
    {
        private static MembershipProvider memberProvider = MembershipProvider.Instance();
        [Browsable(false)]
        public static bool CanEditProviderProperties
        {
            get { return memberProvider.CanEditProviderProperties; }
        }
       // [SortOrder(8), Category("Password")]
        public static int MaxInvalidPasswordAttempts
        {
            get { return memberProvider.MaxInvalidPasswordAttempts; }
            set { memberProvider.MaxInvalidPasswordAttempts = value; }
        }
     //   [SortOrder(5), Category("Password")]
        public static int MinNonAlphanumericCharacters
        {
            get { return memberProvider.MinNonAlphanumericCharacters; }
            set { memberProvider.MinNonAlphanumericCharacters = value; }
        }
      //  [SortOrder(4), Category("Password")]
        public static int MinPasswordLength
        {
            get { return memberProvider.MinPasswordLength; }
            set { memberProvider.MinPasswordLength = value; }
        }
      //  [SortOrder(9), Category("Password")]
        public static int PasswordAttemptWindow
        {
            get { return memberProvider.PasswordAttemptWindow; }
            set { memberProvider.PasswordAttemptWindow = value; }
        }
      //  [SortOrder(1), Category("Password")]
        public static PasswordFormat PasswordFormat
        {
            get { return memberProvider.PasswordFormat; }
            set { memberProvider.PasswordFormat = value; }
        }
       // [SortOrder(3), Category("Password")]
        public static bool PasswordResetEnabled
        {
            get { return memberProvider.PasswordResetEnabled; }
            set { memberProvider.PasswordResetEnabled = value; }
        }
       // [SortOrder(2), Category("Password")]
        public static bool PasswordRetrievalEnabled
        {
            get
            {
                bool enabled = memberProvider.PasswordRetrievalEnabled;
                if (memberProvider.PasswordFormat == PasswordFormat.Hashed)
                {
                    enabled = false;
                }
                return enabled;
            }
            set { memberProvider.PasswordRetrievalEnabled = value; }
        }
        //[SortOrder(7), Category("Password")]
        public static string PasswordStrengthRegularExpression
        {
            get { return memberProvider.PasswordStrengthRegularExpression; }
            set { memberProvider.PasswordStrengthRegularExpression = value; }
        }
      //  [SortOrder(6), Category("Password")]
        public static bool RequiresQuestionAndAnswer
        {
            get { return memberProvider.RequiresQuestionAndAnswer; }
            set { memberProvider.RequiresQuestionAndAnswer = value; }
        }
       // [SortOrder(0), Category("User")]
        public static bool RequiresUniqueEmail
        {
            get { return memberProvider.RequiresUniqueEmail; }
            set { memberProvider.RequiresUniqueEmail = value; }
        }
    }
}
