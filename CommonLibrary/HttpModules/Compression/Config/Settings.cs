using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Cache;
using System.IO;
using System.Xml.XPath;
using CommonLibrary.Common.Utilities;
using System.Text.RegularExpressions;
using CommonLibrary.Common;
using CommonLibrary.Entities.Host;
using System.Collections.Specialized;

namespace CommonLibrary.HttpModules.Compression.Config
{
    [Serializable()]
    public class Settings
    {
        private Algorithms _preferredAlgorithm;
        private StringCollection _excludedPaths;
        private Regex _reg;
        private bool _whitespace;
        private Settings()
        {
            _preferredAlgorithm = Algorithms.None;
            _excludedPaths = new StringCollection();
            _whitespace = false;
        }
        public static Settings Default
        {
            get { return new Settings(); }
        }
        public Algorithms PreferredAlgorithm
        {
            get { return _preferredAlgorithm; }
        }
        public Regex Reg
        {
            get { return _reg; }
        }
        public bool Whitespace
        {
            get { return _whitespace; }
        }
        public static Settings GetSettings()
        {
            Settings _Settings = (Settings)DataCache.GetCache("CompressionConfig");
            if (_Settings == null)
            {
                _Settings = Settings.Default;
                try
                {
                    _Settings._preferredAlgorithm = (Algorithms)Host.HttpCompressionAlgorithm;
                    _Settings._whitespace = Host.WhitespaceFilter;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                string filePath = Globals.ApplicationMapPath + "\\Compression.config";
                if (!File.Exists(filePath))
                {
                    if (File.Exists(Globals.ApplicationMapPath + Globals.glbConfigFolder + "Compression.config"))
                    {
                        File.Copy(Globals.ApplicationMapPath + Globals.glbConfigFolder + "Compression.config", Globals.ApplicationMapPath + "\\Compression.config", true);
                    }
                }
                FileStream fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                XPathDocument doc = new XPathDocument(fileReader);
                _Settings._reg = new Regex(doc.CreateNavigator().SelectSingleNode("compression/whitespace").Value);
                foreach (XPathNavigator nav in doc.CreateNavigator().Select("compression/excludedPaths/path"))
                {
                    _Settings._excludedPaths.Add(nav.Value.ToLower());
                }
                if ((File.Exists(filePath)))
                {
                    DataCache.SetCache("CompressionConfig", _Settings, new CacheDependency(filePath));
                }
            }
            return _Settings;
        }
        public bool IsExcludedPath(string relUrl)
        {
            bool Match = false;
            foreach (string path in _excludedPaths)
            {
                if (relUrl.ToLower().Contains(path))
                {
                    Match = true;
                    break;
                }
            }
            return Match;
        }
    }
}
