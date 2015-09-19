using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Modules
{
    public interface IPortable
    {
        string ExportModule(int ModuleID);
        void ImportModule(int ModuleID, string Content, string Version, int UserID);
    }
}
