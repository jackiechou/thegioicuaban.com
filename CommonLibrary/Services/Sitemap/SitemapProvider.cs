using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using System.Globalization;

namespace CommonLibrary.Services.Sitemap
{
    public abstract class SitemapProvider
    {
        //        Inherits ProviderBase
        //
        //        Public Overrides Sub Initialize(ByVal name As String, ByVal config As System.Collections.Specialized.NameValueCollection)
        //
        //            ' Verify that config isn't null
        //            If config Is Nothing Then
        //                Throw New ArgumentNullException("config")
        //            End If
        //
        //            ' Assign the provider a default name if it doesn't have one
        //            If String.IsNullOrEmpty(name) Then
        //                name = "SitemapProvider"
        //            End If
        //
        //            ' Add a default "description" attribute to config if the
        //            ' attribute doesn't exist or is empty
        //            If String.IsNullOrEmpty(config("description")) Then
        //                config.Remove("description")
        //                config.Add("description", "DotNetNuke Sitemap provider")
        //            End If
        //
        //            MyBase.Initialize(name, config)
        //
        //        End Sub


        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


        public bool Enabled
        {
            get { return bool.Parse(PortalController.GetPortalSetting(Name + "Enabled", PortalController.GetCurrentPortalSettings().PortalId, "True")); }
            set { PortalController.UpdatePortalSetting(PortalController.GetCurrentPortalSettings().PortalId, Name + "Enabled", value.ToString()); }
        }



        public bool OverridePriority
        {
            get { return bool.Parse(PortalController.GetPortalSetting(Name + "Override", PortalController.GetCurrentPortalSettings().PortalId, "False")); }
            set { PortalController.UpdatePortalSetting(PortalController.GetCurrentPortalSettings().PortalId, Name + "Override", value.ToString()); }
        }

        public float Priority
        {
            get
            {
                float value = 0;
                if ((OverridePriority))
                {
                    // stored as an integer (pr * 100) to prevent from translating errors with the decimal point
                    value = float.Parse(PortalController.GetPortalSetting(Name + "Value", PortalController.GetCurrentPortalSettings().PortalId, "0.5"), NumberFormatInfo.InvariantInfo);
                }
                return value;
            }

            set { PortalController.UpdatePortalSetting(PortalController.GetCurrentPortalSettings().PortalId, Name + "Value", value.ToString(NumberFormatInfo.InvariantInfo)); }
        }




        public abstract List<Sitemap.SitemapUrl> GetUrls(int portalId, PortalSettings ps, string version);
    }
}
