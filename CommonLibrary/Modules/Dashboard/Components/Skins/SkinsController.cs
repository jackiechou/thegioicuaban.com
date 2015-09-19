using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Skins;
using CommonLibrary.Common;
using System.IO;

namespace CommonLibrary.Modules.Dashboard.Components.Skins
{
    public class SkinsController : IDashboardData
    {
        private static bool isFallbackSkin(string skinPath)
        {
            SkinDefaults defaultSkin = SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo);
            string defaultSkinPath = (Globals.HostMapPath + SkinController.RootSkin + defaultSkin.Folder).Replace("/", "\\");
            if (defaultSkinPath.EndsWith("\\"))
            {
                defaultSkinPath = defaultSkinPath.Substring(0, defaultSkinPath.Length - 1);
            }
            return skinPath.IndexOf(defaultSkinPath, StringComparison.CurrentCultureIgnoreCase) != -1;
        }
        public static List<SkinInfo> GetInstalledSkins()
        {
            List<SkinInfo> list = new List<SkinInfo>();
            foreach (string folder in Directory.GetDirectories(Path.Combine(Globals.HostMapPath, "Skins")))
            {
                if (!folder.EndsWith(Globals.glbHostSkinFolder))
                {
                    SkinInfo skin = new SkinInfo();
                    skin.SkinName = folder.Substring(folder.LastIndexOf("\\") + 1);
                    skin.InUse = isFallbackSkin(folder) || !SkinController.CanDeleteSkin(folder, "");
                    list.Add(skin);
                }
            }
            return list;
        }
        public void ExportData(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("installedSkins");
            foreach (SkinInfo skin in GetInstalledSkins())
            {
                skin.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
