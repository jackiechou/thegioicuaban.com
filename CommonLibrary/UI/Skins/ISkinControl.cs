using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Modules;

namespace CommonLibrary.UI.Skins
{
    public interface ISkinControl
    {
        IModuleControl ModuleControl { get; set; }
    }
}
