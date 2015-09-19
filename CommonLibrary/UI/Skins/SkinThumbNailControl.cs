using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.Skins
{
    public abstract class SkinThumbNailControl : Framework.UserControlBase
    {
        protected System.Web.UI.WebControls.RadioButtonList optSkin;
        protected System.Web.UI.HtmlControls.HtmlGenericControl ControlContainer;
        private int _Columns = -1;
        public string Border
        {
            get { return Convert.ToString(ViewState["SkinControlBorder"]); }
            set
            {
                ViewState["SkinControlBorder"] = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ControlContainer.Style.Add("border-top", value);
                    ControlContainer.Style.Add("border-bottom", value);
                    ControlContainer.Style.Add("border-left", value);
                    ControlContainer.Style.Add("border-right", value);
                }
            }
        }
        public int Columns
        {
            get { return Convert.ToInt32(ViewState["SkinControlColumns"]); }
            set
            {
                ViewState["SkinControlColumns"] = value;
                if (value > 0)
                {
                    optSkin.RepeatColumns = value;
                }
            }
        }
        public string Height
        {
            get { return Convert.ToString(ViewState["SkinControlHeight"]); }
            set
            {
                ViewState["SkinControlHeight"] = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ControlContainer.Style.Add("height", value);
                }
            }
        }
        public string SkinRoot
        {
            get { return Convert.ToString(ViewState["SkinRoot"]); }
            set { ViewState["SkinRoot"] = value; }
        }
        public string SkinSrc
        {
            get
            {
                if (optSkin.SelectedItem != null)
                {
                    return optSkin.SelectedItem.Value;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                int intIndex;
                for (intIndex = 0; intIndex <= optSkin.Items.Count - 1; intIndex++)
                {
                    if (optSkin.Items[intIndex].Value == value)
                    {
                        optSkin.Items[intIndex].Selected = true;
                        break;
                    }
                }
            }
        }
        public string Width
        {
            get { return Convert.ToString(ViewState["SkinControlWidth"]); }
            set
            {
                ViewState["SkinControlWidth"] = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ControlContainer.Style.Add("width", value);
                }
            }
        }
        private void AddDefaultSkin()
        {
            string strDefault = Services.Localization.Localization.GetString("Not_Specified") + "<br>";
            strDefault += "<img src=\"" + Common.Globals.ApplicationPath + "/images/spacer.gif\" width=\"140\" height=\"135\" border=\"0\">";
            optSkin.Items.Insert(0, new ListItem(strDefault, ""));
        }
        private void AddSkin(string root, string strFolder, string strFile)
        {
            string strImage = "";
            if (File.Exists(strFile.Replace(".ascx", ".jpg")))
            {
                strImage += "<a href=\"" + CreateThumbnail(strFile.Replace(".ascx", ".jpg")).Replace("thumbnail_", "") + "\" target=\"_new\"><img src=\"" + CreateThumbnail(strFile.Replace(".ascx", ".jpg")) + "\" border=\"1\"></a>";
            }
            else
            {
                strImage += "<img src=\"" + Common.Globals.ApplicationPath + "/images/thumbnail.jpg\" border=\"1\">";
            }
            optSkin.Items.Add(new ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)) + "<br>" + strImage, root + "/" + strFolder + "/" + Path.GetFileName(strFile)));
        }
        private string FormatSkinName(string strSkinFolder, string strSkinFile)
        {
            if (strSkinFolder.ToLower() == "_default")
            {
                return strSkinFile;
            }
            else
            {
                switch (strSkinFile.ToLower())
                {
                    case "skin":
                    case "container":
                    case "default":
                        return strSkinFolder;
                    default:
                        return strSkinFolder + " - " + strSkinFile;
                }
            }
        }
        private string CreateThumbnail(string strImage)
        {
            bool blnCreate = true;
            string strThumbnail = strImage.Replace(Path.GetFileName(strImage), "thumbnail_" + Path.GetFileName(strImage));
            if (File.Exists(strThumbnail))
            {
                System.DateTime d1 = File.GetLastWriteTime(strThumbnail);
                System.DateTime d2 = File.GetLastWriteTime(strImage);
                if (File.GetLastWriteTime(strThumbnail) == File.GetLastWriteTime(strImage))
                {
                    blnCreate = false;
                }
            }
            if (blnCreate)
            {
                double dblScale;
                int intHeight;
                int intWidth;
                int intSize = 140;
                System.Drawing.Image objImage;
                try
                {
                    objImage = System.Drawing.Image.FromFile(strImage);
                    if (objImage.Height > objImage.Width)
                    {
                        dblScale = intSize / objImage.Height;
                        intHeight = intSize;
                        intWidth = Convert.ToInt32(objImage.Width * dblScale);
                    }
                    else
                    {
                        dblScale = intSize / objImage.Width;
                        intWidth = intSize;
                        intHeight = Convert.ToInt32(objImage.Height * dblScale);
                    }
                    System.Drawing.Image objThumbnail;
                    objThumbnail = objImage.GetThumbnailImage(intWidth, intHeight, null, IntPtr.Zero);
                    if (File.Exists(strThumbnail))
                    {
                        File.Delete(strThumbnail);
                    }
                    objThumbnail.Save(strThumbnail, objImage.RawFormat);
                    File.SetAttributes(strThumbnail, FileAttributes.Normal);
                    File.SetLastWriteTime(strThumbnail, File.GetLastWriteTime(strImage));
                    objImage.Dispose();
                    objThumbnail.Dispose();
                }
                catch
                {
                }
            }
            strThumbnail = Common.Globals.ApplicationPath + "\\" + strThumbnail.Substring(strThumbnail.ToLower().IndexOf("portals\\"));
            return strThumbnail;
        }
        public void Clear()
        {
            optSkin.Items.Clear();
        }
        public void LoadAllSkins(bool includeNotSpecified)
        {
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            LoadHostSkins(false);
            LoadPortalSkins(false);
        }
        public void LoadHostSkins(bool includeNotSpecified)
        {
            string strRoot;
            string[] arrFolders;
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            strRoot = Common.Globals.HostMapPath + SkinRoot;
            if (Directory.Exists(strRoot))
            {
                arrFolders = Directory.GetDirectories(strRoot);
                foreach (string strFolder in arrFolders)
                {
                    if (!strFolder.EndsWith(Common.Globals.glbHostSkinFolder))
                    {
                        LoadSkins(strFolder, "[G]", false);
                    }
                }
            }
        }
        public void LoadPortalSkins(bool includeNotSpecified)
        {
            string strRoot;
            string[] arrFolders;
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            strRoot = PortalSettings.HomeDirectoryMapPath + SkinRoot;
            if (Directory.Exists(strRoot))
            {
                arrFolders = Directory.GetDirectories(strRoot);
                foreach (string strFolder in arrFolders)
                {
                    LoadSkins(strFolder, "[L]", false);
                }
            }
        }
        public void LoadSkins(string strFolder, string skinType, bool includeNotSpecified)
        {
            string[] arrFiles;
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            if (Directory.Exists(strFolder))
            {
                arrFiles = Directory.GetFiles(strFolder, "*.ascx");
                strFolder = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                foreach (string strFile in arrFiles)
                {
                    AddSkin(skinType + SkinRoot, strFolder, strFile);
                }
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }
    }
}
