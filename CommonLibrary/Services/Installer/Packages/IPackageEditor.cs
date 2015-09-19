using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Installer.Packages
{
    public interface IPackageEditor
    {
        int PackageID { get; set; }
        bool IsWizard { get; set; }
        void Initialize();
        void UpdatePackage();
    }
}
