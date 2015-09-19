using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Modules;
using System.Web.UI;
using System.IO;

namespace CommonLibrary.Modules
{
    public class ModuleUserControlBase : UserControl, IModuleControl
    {
        private string _localResourceFile;
        private ModuleInstanceContext _moduleContext;
        public Control Control
        {
            get { return this; }
        }
        public string ControlPath
        {
            get { return this.TemplateSourceDirectory + "/"; }
        }
        public string ControlName
        {
            get { return this.GetType().Name.Replace("_", "."); }
        }
        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    fileRoot = Path.Combine(this.ControlPath, Services.Localization.Localization.LocalResourceDirectory + "/" + this.ID);
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set { _localResourceFile = value; }
        }
        public ModuleInstanceContext ModuleContext
        {
            get
            {
                if (_moduleContext == null)
                {
                    _moduleContext = new ModuleInstanceContext(this);
                }
                return _moduleContext;
            }
        }
    }
}
