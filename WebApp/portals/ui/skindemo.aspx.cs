using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.UI.Skins;
using System.IO;
using CommonLibrary.Common;
using CommonLibrary.Entities.Portal;

namespace WebApp.portals.ui
{
    public partial class skindemo : System.Web.UI.Page
    {
        private string _Width = "";
        private string _SkinRoot;
        private string _SkinSrc;
        private string _localResourceFile;
        private PortalInfo _objPortal;
        private string _DefaultKey = "System";
        public string DefaultKey
        {
            get { return _DefaultKey; }
            set { _DefaultKey = value; }
        }
        public string Width
        {
            get { return Convert.ToString(ViewState["SkinControlWidth"]); }
            set { _Width = value; }
        }
        public string SkinRoot
        {
            get { return Convert.ToString(ViewState["SkinRoot"]); }
            set { _SkinRoot = value; }
        }
        public string SkinSrc
        {
            get
            {
                if (cboSkin.SelectedItem != null)
                {
                    return cboSkin.SelectedItem.Value;
                }
                else
                {
                    return "";
                }
            }
            set { _SkinSrc = value; }
        }
        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                if (String.IsNullOrEmpty(_localResourceFile))
                {
                    fileRoot = this.TemplateSourceDirectory + "/" + CommonLibrary.Services.Localization.Localization.LocalResourceDirectory + "/SkinControl.ascx";
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set { _localResourceFile = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadTemplate();
            }
        }

        public void LoadTemplate()
        {
            //SkinControl skin_obj= new SkinControl();
            //skin_obj.LoadSkins();
            //Literal_Skin.Text = "test";
            LoadSkins();
        }

        public void LoadSkins()
        {
            string strRoot;
            string[] arrFolders;
            string[] arrFiles;
            string strLastFolder;
            string strSeparator = "----------------------------------------";
            cboSkin.Items.Clear();
            if (optHost.Checked)
            {
                strLastFolder = "";
                strRoot = Globals.HostMapPath + SkinRoot;
                if (Directory.Exists(strRoot))
                {
                    arrFolders = Directory.GetDirectories(strRoot);
                    foreach (string strFolder in arrFolders)
                    {
                        if (!strFolder.EndsWith(Globals.glbHostSkinFolder))
                        {
                            arrFiles = Directory.GetFiles(strFolder, "*.ascx");
                            foreach (string strFile in arrFiles)
                            {
                                string folder = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                                if (strLastFolder != folder)
                                {
                                    if (!String.IsNullOrEmpty(strLastFolder))
                                    {
                                        cboSkin.Items.Add(new ListItem(strSeparator, ""));
                                    }
                                    strLastFolder = folder;
                                }
                                cboSkin.Items.Add(new ListItem(FormatSkinName(folder, Path.GetFileNameWithoutExtension(strFile)), "[G]" + SkinRoot + "/" + folder + "/" + Path.GetFileName(strFile)));
                            }
                        }
                    }
                }
            }
            if (optSite.Checked)
            {
                strLastFolder = "";
                strRoot = _objPortal.HomeDirectoryMapPath + SkinRoot;
                if (Directory.Exists(strRoot))
                {
                    arrFolders = Directory.GetDirectories(strRoot);
                    foreach (string strFolder in arrFolders)
                    {
                        arrFiles = Directory.GetFiles(strFolder, "*.ascx");
                        foreach (string strFile in arrFiles)
                        {
                            string folder = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                            if (strLastFolder != folder)
                            {
                                if (!String.IsNullOrEmpty(strLastFolder))
                                {
                                    cboSkin.Items.Add(new ListItem(strSeparator, ""));
                                }
                                strLastFolder = folder;
                            }
                            cboSkin.Items.Add(new ListItem(FormatSkinName(folder, Path.GetFileNameWithoutExtension(strFile)), "[L]" + SkinRoot + "/" + folder + "/" + Path.GetFileName(strFile)));
                        }
                    }
                }
            }
            if (cboSkin.Items.Count > 0)
            {
                cboSkin.Items.Insert(0, new ListItem(strSeparator, ""));
            }
            cboSkin.Items.Insert(0, new ListItem("<" + CommonLibrary.Services.Localization.Localization.GetString(DefaultKey, LocalResourceFile) + ">", ""));
            if (ViewState["SkinSrc"] != null)
            {
                for (int intIndex = 0; intIndex <= cboSkin.Items.Count - 1; intIndex++)
                {
                    if (cboSkin.Items[intIndex].Value.ToLower() == ViewState["SkinSrc"].ToString().ToLower())
                    {
                        cboSkin.Items[intIndex].Selected = true;
                        break;
                    }
                }
            }
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
    }
}