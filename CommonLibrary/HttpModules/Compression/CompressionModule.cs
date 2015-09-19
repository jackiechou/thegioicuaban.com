using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using CommonLibrary.HttpModules.Compression.Filters;
using CommonLibrary.HttpModules.Compression.Config;

namespace CommonLibrary.HttpModules.Compression
{
    public class CompressionModule : IHttpModule
    {
        private const string INSTALLED_KEY = "httpcompress.attemptedinstall";
        private static readonly object INSTALLED_TAG = new object();
        public void Init(HttpApplication context)
        {
            context.ReleaseRequestState += CompressContent;
            context.PreSendRequestHeaders += CompressContent;
        }
        public void Dispose()
        {
        }
        private void CompressContent(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if ((app == null) || (app.Context == null) || (app.Context.Items == null))
            {
                return;
            }
            else
            {
                CommonLibrary.Framework.CDefault page = app.Context.Handler as CommonLibrary.Framework.CDefault;
                if ((page == null))
                    return;
            }
            if (app.Response == null || app.Response.ContentType == null || app.Response.ContentType.ToLower() != "text/html")
            {
                return;
            }
            if (!app.Context.Items.Contains(INSTALLED_KEY))
            {
                app.Context.Items.Add(INSTALLED_KEY, INSTALLED_TAG);
                string realPath = app.Request.Url.PathAndQuery;
                CommonLibrary.HttpModules.Compression.Config.Settings _Settings = CommonLibrary.HttpModules.Compression.Config.Settings.GetSettings();
                if (_Settings == null)
                {
                    return;
                }
                bool compress = true;
                if (_Settings.PreferredAlgorithm == CommonLibrary.HttpModules.Compression.Config.Algorithms.None)
                {
                    compress = false;
                    if (!_Settings.Whitespace)
                    {
                        return;
                    }
                }
                string acceptedTypes = app.Request.Headers["Accept-Encoding"];
                if (_Settings.IsExcludedPath(realPath) || acceptedTypes == null)
                {
                    return;
                }
                app.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
                CompressingFilter filter = null;
                if (compress)
                {
                    string[] types = acceptedTypes.Split(',');
                    filter = GetFilterForScheme(types, app.Response.Filter, _Settings);
                }
                if (filter == null)
                {
                    if (_Settings.Whitespace)
                    {
                        app.Response.Filter = new WhitespaceFilter(app.Response.Filter, _Settings.Reg);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (_Settings.Whitespace)
                    {
                        app.Response.Filter = new WhitespaceFilter(filter, _Settings.Reg);
                    }
                    else
                    {
                        app.Response.Filter = filter;
                    }
                }
            }
        }
        public static CompressingFilter GetFilterForScheme(string[] schemes, Stream output, CommonLibrary.HttpModules.Compression.Config.Settings prefs)
        {
            bool foundDeflate = false;
            bool foundGZip = false;
            bool foundStar = false;
            float deflateQuality = 0;
            float gZipQuality = 0;
            float starQuality = 0;
            bool isAcceptableDeflate;
            bool isAcceptableGZip;
            bool isAcceptableStar;
            for (int i = 0; i <= schemes.Length - 1; i++)
            {
                string acceptEncodingValue = schemes[i].Trim().ToLower();
                if (acceptEncodingValue.StartsWith("deflate"))
                {
                    foundDeflate = true;
                    float newDeflateQuality = GetQuality(acceptEncodingValue);
                    if (deflateQuality < newDeflateQuality)
                    {
                        deflateQuality = newDeflateQuality;
                    }
                }
                else if ((acceptEncodingValue.StartsWith("gzip") || acceptEncodingValue.StartsWith("x-gzip")))
                {
                    foundGZip = true;
                    float newGZipQuality = GetQuality(acceptEncodingValue);
                    if ((gZipQuality < newGZipQuality))
                    {
                        gZipQuality = newGZipQuality;
                    }
                }
                else if ((acceptEncodingValue.StartsWith("*")))
                {
                    foundStar = true;
                    float newStarQuality = GetQuality(acceptEncodingValue);
                    if ((starQuality < newStarQuality))
                    {
                        starQuality = newStarQuality;
                    }
                }
            }
            isAcceptableStar = foundStar && (starQuality > 0);
            isAcceptableDeflate = (foundDeflate && (deflateQuality > 0)) || (!foundDeflate && isAcceptableStar);
            isAcceptableGZip = (foundGZip && (gZipQuality > 0)) || (!foundGZip && isAcceptableStar);
            if (isAcceptableDeflate && !foundDeflate)
            {
                deflateQuality = starQuality;
            }
            if (isAcceptableGZip && !foundGZip)
            {
                gZipQuality = starQuality;
            }
            if ((!(isAcceptableDeflate || isAcceptableGZip || isAcceptableStar)))
            {
                return null;
            }
            if ((isAcceptableDeflate && (!isAcceptableGZip || (deflateQuality > gZipQuality))))
            {
                return new DeflateFilter(output);
            }
            if ((isAcceptableGZip && (!isAcceptableDeflate || (deflateQuality < gZipQuality))))
            {
                return new GZipFilter(output);
            }
            if ((isAcceptableDeflate && (prefs.PreferredAlgorithm == Algorithms.Deflate || prefs.PreferredAlgorithm == Algorithms.Default)))
            {
                return new DeflateFilter(output);
            }
            if ((isAcceptableGZip && prefs.PreferredAlgorithm == Algorithms.GZip))
            {
                return new GZipFilter(output);
            }
            if ((isAcceptableDeflate || isAcceptableStar))
            {
                return new DeflateFilter(output);
            }
            if ((isAcceptableGZip))
            {
                return new GZipFilter(output);
            }
            return null;
        }
        public static float GetQuality(string acceptEncodingValue)
        {
            int qParam = acceptEncodingValue.IndexOf("q=");
            if ((qParam >= 0))
            {
                float Val = 0;
                try
                {
                    Val = float.Parse(acceptEncodingValue.Substring(qParam + 2, acceptEncodingValue.Length - (qParam + 2)));
                }
                catch (FormatException exc)
                {
                }
                return Val;
            }
            else
            {
                return 1;
            }
        }
    }
}
