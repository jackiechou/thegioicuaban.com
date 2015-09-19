using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CommonLibrary.Modules.Dashboard.Components
{
    public interface IDashboardData
    {
        void ExportData(XmlWriter writer);
    }
}
