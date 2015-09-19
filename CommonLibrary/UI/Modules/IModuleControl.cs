using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.UI.Modules
{
    public interface IModuleControl
    {
        Control Control { get; }
        string ControlPath { get; }
        string ControlName { get; }
        string LocalResourceFile { get; set; }
        ModuleInstanceContext ModuleContext { get; }
    }
}
