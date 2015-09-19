using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;

namespace CommonLibrary.Services.Installer.Installers
{
    public class AssemblyInstaller : FileInstaller
    {
        protected override string CollectionNodeName
        {
            get { return "assemblies"; }
        }
        protected override string DefaultPath
        {
            get { return "bin\\"; }
        }
        protected override string ItemNodeName
        {
            get { return "assembly"; }
        }
        protected override string PhysicalBasePath
        {
            get { return PhysicalSitePath + "\\"; }
        }
        public override string AllowableFiles
        {
            get { return "dll"; }
        }
        protected override void DeleteFile(InstallFile file)
        {
            if (DataProvider.Instance().UnRegisterAssembly(this.Package.PackageID, file.Name))
            {
                Log.AddInfo(Util.ASSEMBLY_UnRegistered + " - " + file.FullName);
                base.DeleteFile(file);
            }
            else
            {
                Log.AddInfo(Util.ASSEMBLY_InUse + " - " + file.FullName);
            }
        }
        protected override bool IsCorrectType(InstallFileType type)
        {
            return (type == InstallFileType.Assembly);
        }
        protected override bool InstallFile(InstallFile file)
        {
            bool bSuccess = true;
            if (file.Action == "UnRegister")
            {
                DeleteFile(file);
            }
            else
            {
                int returnCode = DataProvider.Instance().RegisterAssembly(this.Package.PackageID, file.Name, file.Version.ToString(3));
                switch (returnCode)
                {
                    case 0:
                        Log.AddInfo(Util.ASSEMBLY_Added + " - " + file.FullName);
                        break;
                    case 1:
                        Log.AddInfo(Util.ASSEMBLY_Updated + " - " + file.FullName);
                        break;
                    case 2:
                    case 3:
                        Log.AddInfo(Util.ASSEMBLY_Registered + " - " + file.FullName);
                        break;
                }
                if (returnCode < 2 || (returnCode == 2 && file.InstallerInfo.RepairInstall))
                {
                    bSuccess = base.InstallFile(file);
                }
            }
            return bSuccess;
        }
    }
}
