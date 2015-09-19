using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonLibrary.ComponentModel
{
    public interface IComponentInstaller
    {
        void InstallComponents(IContainer container);
    }
}
