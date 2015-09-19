using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;

namespace CommonLibrary.Services.Tokens
{
    public enum Scope
    {
        NoSettings = 0,
        Configuration = 1,
        DefaultSettings = 2,
        SystemMessages = 3,
        Debug = 4
    }
    public enum CacheLevel : byte
    {
        notCacheable = 0,
        secureforCaching = 5,
        fullyCacheable = 10
    }
    public abstract class BaseCustomTokenReplace : BaseTokenReplace
    {
        private Scope _AccessLevel;
        private Entities.Users.UserInfo _AccessingUser;
        private bool _debugMessages;
        protected Scope CurrentAccessLevel
        {
            get { return _AccessLevel; }
            set { _AccessLevel = value; }
        }
        protected System.Collections.Generic.Dictionary<string, IPropertyAccess> PropertySource = new System.Collections.Generic.Dictionary<string, IPropertyAccess>();
        public Entities.Users.UserInfo AccessingUser
        {
            get { return _AccessingUser; }
            set { _AccessingUser = value; }
        }
        public bool DebugMessages
        {
            get { return _debugMessages; }
            set { _debugMessages = value; }
        }
        protected override string replacedTokenValue(string strObjectName, string strPropertyName, string strFormat)
        {
            bool PropertyNotFound = false;
            string result = string.Empty;
            if (PropertySource.ContainsKey(strObjectName.ToLower()))
            {
                result = PropertySource[strObjectName.ToLower()].GetProperty(strPropertyName, strFormat, FormatProvider, AccessingUser, CurrentAccessLevel, ref PropertyNotFound);
            }
            else
            {
                if (DebugMessages)
                {
                    string message = Localization.Localization.GetString("TokenReplaceUnknownObject", Localization.Localization.SharedResourceFile, FormatProvider.ToString());
                    if (message == string.Empty)
                        message = "Error accessing [{0}:{1}], {0} is an unknown datasource";
                    result = string.Format(message, strObjectName, strPropertyName);
                }
            }
            if (DebugMessages && PropertyNotFound)
            {
                string message;
                if (result == PropertyAccess.PropertyAccess.ContentLocked)
                {
                    message = Localization.Localization.GetString("TokenReplaceRestrictedProperty", Localization.Localization.GlobalResourceFile, FormatProvider.ToString());
                }
                else
                {
                    message = Localization.Localization.GetString("TokenReplaceUnknownProperty", Localization.Localization.GlobalResourceFile, FormatProvider.ToString());
                }
                if (message == string.Empty)
                    message = "Error accessing [{0}:{1}], {1} is unknown for datasource {0}";
                result = string.Format(message, strObjectName, strPropertyName);
            }
            return result;
        }
        public bool ContainsTokens(string strSourceText)
        {
            if (!string.IsNullOrEmpty(strSourceText))
            {
                foreach (System.Text.RegularExpressions.Match currentMatch in TokenizerRegex.Matches(strSourceText))
                {
                    if (currentMatch.Result("${object}").Length > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public CacheLevel Cacheability(string strSourcetext)
        {
            CacheLevel IsSafe = CacheLevel.fullyCacheable;
            if (strSourcetext != null && !string.IsNullOrEmpty(strSourcetext))
            {
                string DummyResult = ReplaceTokens(strSourcetext);
                System.Text.StringBuilder Result = new System.Text.StringBuilder();
                foreach (System.Text.RegularExpressions.Match currentMatch in TokenizerRegex.Matches(strSourcetext))
                {
                    string strObjectName = currentMatch.Result("${object}");
                    if (!String.IsNullOrEmpty(strObjectName))
                    {
                        if (strObjectName == "[")
                        {
                        }
                        else if (!PropertySource.ContainsKey(strObjectName.ToLower()))
                        {
                        }
                        else
                        {
                            CacheLevel c = PropertySource[strObjectName.ToLower()].Cacheability;
                            if (c < IsSafe)
                                IsSafe = c;
                        }
                    }
                }
            }
            return IsSafe;
        }
    }
}
