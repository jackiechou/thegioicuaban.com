using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Installer.Packages
{
    public class PackageCreatedEventArgs : EventArgs
    {
        private PackageInfo _Package;
        public PackageCreatedEventArgs(PackageInfo package)
        {
            _Package = package;
        }
        public PackageInfo Package
        {
            get { return _Package; }
        }
    }
}
