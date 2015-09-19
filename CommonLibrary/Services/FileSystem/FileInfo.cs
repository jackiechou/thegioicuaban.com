using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Entities.Portal;
using System.Web;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;

namespace CommonLibrary.Services.FileSystem
{
    [XmlRoot("file", IsNullable = false)]
    [Serializable()]
    public class FileInfo
    {
        private int _FileId;
        private int _PortalId;
        private string _FileName;
        private string _Extension;
        private int _Size;
        private int _Width;
        private int _Height;
        private string _ContentType;
        private string _Folder;
        private int _FolderId;
        private int _StorageLocation;
        private bool _IsCached = false;

        public FileInfo()
        {
        }
        [XmlElement("contenttype")]
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        [XmlElement("extension")]
        public string Extension
        {
            get { return _Extension; }
            set { _Extension = value; }
        }
        [XmlIgnore()]
        public int FileId
        {
            get { return _FileId; }
            set { _FileId = value; }
        }
        [XmlElement("filename")]
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        [XmlIgnore()]
        public string Folder
        {
            get { return _Folder; }
            set { _Folder = value; }
        }
        [XmlIgnore()]
        public int FolderId
        {
            get { return _FolderId; }
            set { _FolderId = value; }
        }
        [XmlElement("height")]
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        [XmlIgnore()]
        public bool IsCached
        {
            get { return _IsCached; }
            set { _IsCached = value; }
        }
        [XmlIgnore()]
        public string PhysicalPath
        {
            get
            {
                string _PhysicalPath;
                PortalSettings PortalSettings = null;
                if (HttpContext.Current != null)
                {
                    PortalSettings = PortalController.GetCurrentPortalSettings();
                }
                if (PortalId == Null.NullInteger)
                {
                    _PhysicalPath = Globals.HostMapPath + RelativePath;
                }
                else
                {
                    if (PortalSettings == null || PortalSettings.PortalId != PortalId)
                    {
                        PortalController objPortals = new PortalController();
                        PortalInfo objPortal = objPortals.GetPortal(PortalId);
                        _PhysicalPath = objPortal.HomeDirectoryMapPath + RelativePath;
                    }
                    else
                    {
                        _PhysicalPath = PortalSettings.HomeDirectoryMapPath + RelativePath;
                    }
                }
                return _PhysicalPath.Replace("/", "\\");
            }
        }
        [XmlIgnore()]
        public int PortalId
        {
            get { return _PortalId; }
            set { _PortalId = value; }
        }
        public string RelativePath
        {
            get { return Folder + FileName; }
        }
        [XmlElement("size")]
        public int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        [XmlIgnore()]
        public int StorageLocation
        {
            get { return _StorageLocation; }
            set { _StorageLocation = value; }
        }
        [XmlElement("width")]
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
    }
}
