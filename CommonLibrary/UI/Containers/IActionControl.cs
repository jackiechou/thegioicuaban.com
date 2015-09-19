using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Modules.Actions;
using CommonLibrary.UI.Modules;

namespace CommonLibrary.UI.Containers
{
    public interface IActionControl
    {
        event ActionEventHandler Action;
        ActionManager ActionManager { get; }
        IModuleControl ModuleControl { get; set; }
    }
}
