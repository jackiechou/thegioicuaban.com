using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Skins;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ContainerInstaller : SkinInstaller
    {
        protected override string CollectionNodeName
        {
            get { return "containerFiles"; }
        }
        protected override string ItemNodeName
        {
            get { return "containerFile"; }
        }
        protected override string SkinNameNodeName
        {
            get { return "containerName"; }
        }
        protected override string SkinRoot
        {
            get { return SkinController.RootContainer; }
        }
        protected override string SkinType
        {
            get { return "Container"; }
        }
    }
}
