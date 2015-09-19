using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace CommonLibrary.Services.Installer
{
    [Serializable()]
    public class InstallFile
    {
        private TextEncoding _Encoding;
        private string _Action;
        private InstallerInfo _InstallerInfo;
        private string _Name;
        private string _Path;
        private string _SourceFileName;
        private InstallFileType _Type;
        private System.Version _Version;
        public InstallFile(ZipInputStream zip, ZipEntry entry, InstallerInfo info)
        {
            _InstallerInfo = info;
            ReadZip(zip, entry);
        }
        public InstallFile(string fileName)
        {
            ParseFileName(fileName);
        }
        public InstallFile(string fileName, InstallerInfo info)
        {
            ParseFileName(fileName);
            _InstallerInfo = info;
        }
        public InstallFile(string fileName, string sourceFileName, InstallerInfo info)
        {
            ParseFileName(fileName);
            _SourceFileName = sourceFileName;
            _InstallerInfo = info;
        }
        public InstallFile(string fileName, string filePath)
        {
            _Name = fileName;
            _Path = filePath;
        }
        public string Action
        {
            get { return _Action; }
            set { _Action = value; }
        }
        public string BackupFileName
        {
            get { return System.IO.Path.Combine(BackupPath, Name + ".config"); }
        }
        public virtual string BackupPath
        {
            get { return System.IO.Path.Combine(InstallerInfo.TempInstallFolder, System.IO.Path.Combine("Backup", Path)); }
        }
        public TextEncoding Encoding
        {
            get { return _Encoding; }
        }
        public string Extension
        {
            get
            {
                string ext = System.IO.Path.GetExtension(_Name);
                if (String.IsNullOrEmpty(ext))
                {
                    return "";
                }
                else
                {
                    return ext.Substring(1);
                }
            }
        }
        public string FullName
        {
            get { return System.IO.Path.Combine(_Path, _Name); }
        }
        [Browsable(false)]
        public InstallerInfo InstallerInfo
        {
            get { return _InstallerInfo; }
        }
        public string Name
        {
            get { return _Name; }
        }
        public string Path
        {
            get { return _Path; }
        }
        public string SourceFileName
        {
            get { return _SourceFileName; }
        }
        public string TempFileName
        {
            get
            {
                string fileName = SourceFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = FullName;
                }
                return System.IO.Path.Combine(InstallerInfo.TempInstallFolder, fileName);
            }
        }
        public InstallFileType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        public System.Version Version
        {
            get { return _Version; }
        }
        private TextEncoding GetTextEncodingType(byte[] Buffer)
        {
            if (Buffer[0] == 255 && Buffer[1] == 254)
            {
                return TextEncoding.UTF16LittleEndian;
            }
            if (Buffer[0] == 254 && Buffer[1] == 255)
            {
                return TextEncoding.UTF16BigEndian;
            }
            if (Buffer[0] == 239 && Buffer[1] == 187 && Buffer[2] == 191)
            {
                return TextEncoding.UTF8;
            }
            int i;
            for (i = 0; i <= 100; i++)
            {
                if (Buffer[i] > 127)
                {
                    return TextEncoding.Unknown;
                }
            }
            return TextEncoding.UTF7;
        }
        private void ParseFileName(string fileName)
        {
            int i = fileName.Replace("\\", "/").LastIndexOf("/");
            if (i < 0)
            {
                _Name = fileName.Substring(0, fileName.Length);
                _Path = "";
            }
            else
            {
                _Name = fileName.Substring(i + 1, fileName.Length - (i + 1));
                _Path = fileName.Substring(0, i);
            }
            if (string.IsNullOrEmpty(_Path) && fileName.StartsWith("[app_code]"))
            {
                _Name = fileName.Substring(10, fileName.Length - 10);
                _Path = fileName.Substring(0, 10);
            }
            if (_Name.ToLower() == "manifest.xml")
            {
                _Type = InstallFileType.Manifest;
            }
            else
            {
                switch (Extension.ToLower())
                {
                    case "ascx":
                        _Type = InstallFileType.Ascx;
                        break;
                    case "dll":
                        _Type = InstallFileType.Assembly;
                        break;
                    case "dnn":
                    case "dnn5":
                        _Type = InstallFileType.Manifest;
                        break;
                    case "resx":
                        _Type = InstallFileType.Language;
                        break;
                    case "resources":
                    case "zip":
                        _Type = InstallFileType.Resources;
                        break;
                    default:
                        if (Extension.ToLower().EndsWith("dataprovider"))
                        {
                            _Type = InstallFileType.Script;
                        }
                        else if (_Path.StartsWith("[app_code]"))
                        {
                            _Type = InstallFileType.AppCode;
                        }
                        else
                        {
                            if (Regex.IsMatch(_Name, Util.REGEX_Version + ".txt"))
                            {
                                _Type = InstallFileType.CleanUp;
                            }
                            else
                            {
                                _Type = InstallFileType.Other;
                            }
                        }
                        break;
                }
            }
            _Path = _Path.Replace("[app_code]", "");
            if (_Path.StartsWith("\\"))
            {
                _Path = _Path.Substring(1);
            }
        }
        private void ReadZip(ZipInputStream unzip, ZipEntry entry)
        {
            ParseFileName(entry.Name);
            Util.WriteStream(unzip, TempFileName);
        }
        public void SetVersion(System.Version version)
        {
            _Version = version;
        }
    }
}
