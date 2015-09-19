using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonLibrary.Services.Installer
{
    [TypeConverter(typeof(EnumConverter))]
    public enum InstallFileType
    {
        AppCode,
        Ascx,
        Assembly,
        CleanUp,
        Language,
        Manifest,
        Other,
        Resources,
        Script
    }
}
