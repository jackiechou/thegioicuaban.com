using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.Modules
{
    public interface ISettingsControl : IModuleControl
    {
        void LoadSettings();
        void UpdateSettings();
    }
}
