using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonLibrary.Common;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.UI.Skins
{
    public enum SkinDefaultType
    {
        SkinInfo,
        ContainerInfo
    }
    [Serializable()]
    public class SkinDefaults
    {
        private string _adminDefaultName;
        private string _defaultName;
        private string _folder;
        public string AdminDefaultName
        {
            get { return _adminDefaultName; }
            set { _adminDefaultName = value; }
        }
        public string DefaultName
        {
            get { return _defaultName; }
            set { _defaultName = value; }
        }
        public string Folder
        {
            get { return _folder; }
            set { _folder = value; }
        }
        private SkinDefaults(SkinDefaultType DefaultType)
        {
            string nodename = Enum.GetName(DefaultType.GetType(), DefaultType).ToLower();
            string filePath = System.IO.Path.Combine(Globals.ApplicationMapPath, Globals.glbConfig);
            XmlDocument dnndoc = new XmlDocument();
            dnndoc.Load(filePath);
            XmlNode defaultElement = dnndoc.SelectSingleNode("/configuration/skinningdefaults/" + nodename);
            _folder = defaultElement.Attributes["folder"].Value;
            _defaultName = defaultElement.Attributes["default"].Value;
            _adminDefaultName = defaultElement.Attributes["admindefault"].Value;
        }
        private static object GetSkinDefaultsCallback(CacheItemArgs cacheItemArgs)
        {
            SkinDefaultType defaultType = (SkinDefaultType)cacheItemArgs.ParamList[0];
            return new SkinDefaults(defaultType);
        }
        public static SkinDefaults GetSkinDefaults(SkinDefaultType DefaultType)
        {
            return CBO.GetCachedObject<SkinDefaults>(new CacheItemArgs(string.Format(DataCache.SkinDefaultsCacheKey, DefaultType), DataCache.SkinDefaultsCacheTimeOut, DataCache.SkinDefaultsCachePriority, DefaultType), GetSkinDefaultsCallback);
        }
    }
}
