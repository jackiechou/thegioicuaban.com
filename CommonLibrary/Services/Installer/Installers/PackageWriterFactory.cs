using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Lists;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.Framework;

namespace CommonLibrary.Services.Installer.Installers
{
    public class PackageWriterFactory
    {
        public static PackageWriterBase GetWriter(PackageInfo package)
        {
            PackageWriterBase writer = null;
            switch (package.PackageType)
            {
                case "Auth_System":
                    writer = new AuthenticationPackageWriter(package);
                    break;
                case "Module":
                    writer = new ModulePackageWriter(package);
                    break;
                case "Container":
                    writer = new ContainerPackageWriter(package);
                    break;
                case "Skin":
                    writer = new SkinPackageWriter(package);
                    break;
                case "CoreLanguagePack":
                case "ExtensionLanguagePack":
                    writer = new LanguagePackWriter(package);
                    break;
                case "SkinObject":
                    writer = new SkinControlPackageWriter(package);
                    break;
                case "Provider":
                    writer = new ProviderPackageWriter(package);
                    break;
                case "Library":
                    writer = new LibraryPackageWriter(package);
                    break;
                case "Widget":
                    writer = new WidgetPackageWriter(package);
                    break;
                default:
                    CommonLibrary.Common.Lists.ListController listController = new CommonLibrary.Common.Lists.ListController();
                    ListEntryInfo entry = listController.GetListEntryInfo("PackageWriter", package.PackageType);
                    if (entry != null && !string.IsNullOrEmpty(entry.Text))
                    {
                        writer = (PackageWriterBase)Reflection.CreateObject(entry.Text, "PackageWriter_" + entry.Value);
                    }
                    break;
            }
            return writer;
        }
    }
}
