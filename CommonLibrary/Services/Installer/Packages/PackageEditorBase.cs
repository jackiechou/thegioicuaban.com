using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Modules;

namespace CommonLibrary.Services.Installer.Packages
{
    public class PackageEditorBase : ModuleUserControlBase, IPackageEditor
    {
        private int _PackageID = Null.NullInteger;
        private PackageInfo _Package = null;
        private bool _IsWizard = Null.NullBoolean;
        public int PackageID
        {
            get { return _PackageID; }
            set { _PackageID = value; }
        }
        public bool IsWizard
        {
            get { return _IsWizard; }
            set { _IsWizard = value; }
        }
        public virtual void Initialize()
        {
        }
        public virtual void UpdatePackage()
        {
        }
        protected virtual string EditorID
        {
            get { return Null.NullString; }
        }
        protected bool IsSuperTab
        {
            get { return ModuleContext.PortalSettings.ActiveTab.IsSuperTab; }
        }
        protected PackageInfo Package
        {
            get
            {
                if (_Package == null)
                {
                    _Package = PackageController.GetPackage(PackageID);
                }
                return _Package;
            }
        }
        protected override void OnInit(System.EventArgs e)
        {
            this.ID = EditorID;
            base.OnInit(e);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}
