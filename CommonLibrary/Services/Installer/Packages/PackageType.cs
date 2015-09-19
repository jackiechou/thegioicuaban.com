using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security;

namespace CommonLibrary.Services.Installer.Packages
{
    public class PackageType
    {
        private string _PackageType = Null.NullString;
        private string _Description;
        private string _EditorControlSrc;
        private SecurityAccessLevel _SecurityAccessLevel;
        public PackageType()
        {
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public string EditorControlSrc
        {
            get { return _EditorControlSrc; }
            set { _EditorControlSrc = value; }
        }
        public string Type
        {
            get { return _PackageType; }
            set { _PackageType = value; }
        }
        public SecurityAccessLevel SecurityAccessLevel
        {
            get { return _SecurityAccessLevel; }
            set { _SecurityAccessLevel = value; }
        }
    }
}
