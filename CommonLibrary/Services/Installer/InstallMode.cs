using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonLibrary.Services.Installer
{
    [TypeConverter(typeof(EnumConverter))]
    public enum InstallMode
    {
        Install,
        ManifestOnly,
        UnInstall
    }
}
