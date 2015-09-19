using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Framework.Providers;
using CommonLibrary.Framework;
using CommonLibrary.Application;

namespace CommonLibrary.Modules.Dashboard.Components.Host
{
    [XmlRoot("hostInfo")]
    public class HostInfo : IXmlSerializable
    {
        public string CachingProvider
        {
            get { return ProviderConfiguration.GetProviderConfiguration("caching").DefaultProvider; }
        }
        public string DataProvider
        {
            get { return ProviderConfiguration.GetProviderConfiguration("data").DefaultProvider; }
        }
        public string FriendlyUrlProvider
        {
            get { return ProviderConfiguration.GetProviderConfiguration("friendlyUrl").DefaultProvider; }
        }
        public string FriendlyUrlEnabled
        {
            get { return CommonLibrary.Entities.Host.Host.UseFriendlyUrls.ToString(); }
        }
        public string FriendlyUrlType
        {
            get
            {
                Provider urlprovider = (Provider)ProviderConfiguration.GetProviderConfiguration("friendlyUrl").Providers[FriendlyUrlProvider];
                return (urlprovider.Attributes["urlformat"] == "humanfriendly") ? "humanfriendly" : "searchfriendly";
            }
        }
        public string HostGUID
        {
            get { return CommonLibrary.Entities.Host.Host.GUID; }
        }
        public string HtmlEditorProvider
        {
            get { return ProviderConfiguration.GetProviderConfiguration("htmlEditor").DefaultProvider; }
        }
        public string LoggingProvider
        {
            get { return ProviderConfiguration.GetProviderConfiguration("logging").DefaultProvider; }
        }
        public string Permissions
        {
            get { return SecurityPolicy.Permissions; }
        }
        public string Product
        {
            get { return AppContext.Current.Application.Description; }
        }
        public string SchedulerMode
        {
            get { return CommonLibrary.Entities.Host.Host.SchedulerMode.ToString(); }
        }
        public string Version
        {
            get { return AppContext.Current.Application.Version.ToString(3); }
        }
        public string WebFarmEnabled
        {
            get { return CommonLibrary.Services.Cache.CachingProvider.Instance().IsWebFarm().ToString(); }
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("hostGUID", HostGUID);
            writer.WriteElementString("version", Version);
            writer.WriteElementString("permissions", Permissions);
            writer.WriteElementString("dataProvider", DataProvider);
            writer.WriteElementString("cachingProvider", CachingProvider);
            writer.WriteElementString("friendlyUrlProvider", FriendlyUrlProvider);
            writer.WriteElementString("friendlyUrlEnabled", FriendlyUrlEnabled);
            writer.WriteElementString("friendlyUrlType", FriendlyUrlType);
            writer.WriteElementString("htmlEditorProvider", HtmlEditorProvider);
            writer.WriteElementString("loggingProvider", LoggingProvider);
            writer.WriteElementString("schedulerMode", SchedulerMode);
            writer.WriteElementString("webFarmEnabled", WebFarmEnabled);
        }
    }
}
