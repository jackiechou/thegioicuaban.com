using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using CommonLibrary.Common.Utilities;
using System.Text.RegularExpressions;

namespace CommonLibrary.Services.Tokens
{
    public abstract class BaseTokenReplace
    {
        private const string ExpressionDefault = "(?:\\[(?:(?<object>[^\\]\\[:]+):(?<property>[^\\]\\[\\|]+))(?:\\|(?:(?<format>[^\\]\\[]+)\\|(?<ifEmpty>[^\\]\\[]+))|\\|(?:(?<format>[^\\|\\]\\[]+)))?\\])|(?<text>\\[[^\\]\\[]+\\])|(?<text>[^\\]\\[]+)";
        private const string ExpressionObjectLess = "(?:\\[(?:(?<object>[^\\]\\[:]+):(?<property>[^\\]\\[\\|]+))(?:\\|(?:(?<format>[^\\]\\[]+)\\|(?<ifEmpty>[^\\]\\[]+))|\\|(?:(?<format>[^\\|\\]\\[]+)))?\\])" + "|(?:(?<object>\\[)(?<property>[A-Z0-9._]+)(?:\\|(?:(?<format>[^\\]\\[]+)\\|(?<ifEmpty>[^\\]\\[]+))|\\|(?:(?<format>[^\\|\\]\\[]+)))?\\])" + "|(?<text>\\[[^\\]\\[]+\\])" + "|(?<text>[^\\]\\[]+)";
        private bool _UseObjectLessExpression = false;
        protected bool UseObjectLessExpression
        {
            get { return _UseObjectLessExpression; }
            set { _UseObjectLessExpression = value; }
        }
        private string TokenReplaceCacheKey
        {
            get
            {
                if (UseObjectLessExpression)
                {
                    return "TokenReplaceRegEx_Objectless";
                }
                else
                {
                    return "TokenReplaceRegEx_Default";
                }
            }
        }
        private string RegExpression
        {
            get
            {
                if (UseObjectLessExpression)
                {
                    return ExpressionObjectLess;
                }
                else
                {
                    return ExpressionDefault;
                }
            }
        }
        protected const string ObjectLessToken = "no_object";
        protected Regex TokenizerRegex
        {
            get
            {
                Regex tokenizer = (Regex)DataCache.GetCache(TokenReplaceCacheKey);
                if (tokenizer == null)
                {
                    tokenizer = new Regex(RegExpression, RegexOptions.Compiled);
                    DataCache.SetCache(TokenReplaceCacheKey, tokenizer);
                }
                return tokenizer;
            }
        }
        private string _Language;
        private CultureInfo _FormatProvider;
        public string Language
        {
            get { return _Language; }
            set
            {
                _Language = value;
                _FormatProvider = new CultureInfo(_Language);
            }
        }
        protected System.Globalization.CultureInfo FormatProvider
        {
            get
            {
                if (_FormatProvider == null)
                {
                    _FormatProvider = System.Threading.Thread.CurrentThread.CurrentCulture;
                }
                return _FormatProvider;
            }
        }
        protected virtual string ReplaceTokens(string strSourceText)
        {
            if (strSourceText == null)
                return string.Empty;
            System.Text.StringBuilder Result = new System.Text.StringBuilder();
            foreach (Match currentMatch in TokenizerRegex.Matches(strSourceText))
            {
                string strObjectName = currentMatch.Result("${object}");
                if (!String.IsNullOrEmpty(strObjectName))
                {
                    if (strObjectName == "[")
                        strObjectName = ObjectLessToken;
                    string strPropertyName = currentMatch.Result("${property}");
                    string strFormat = currentMatch.Result("${format}");
                    string strIfEmptyReplacment = currentMatch.Result("${ifEmpty}");
                    string strConversion = replacedTokenValue(strObjectName, strPropertyName, strFormat);
                    if (!String.IsNullOrEmpty(strIfEmptyReplacment) && String.IsNullOrEmpty(strConversion))
                        strConversion = strIfEmptyReplacment;
                    Result.Append(strConversion);
                }
                else
                {
                    Result.Append(currentMatch.Result("${text}"));
                }
            }
            return Result.ToString();
        }
        protected abstract string replacedTokenValue(string strObjectName, string strPropertyName, string strFormat);
    }
}
