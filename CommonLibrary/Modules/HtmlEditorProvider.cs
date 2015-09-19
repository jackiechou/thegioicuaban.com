using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.ComponentModel;
using System.Collections;

namespace CommonLibrary.Modules
{
    public abstract class HtmlEditorProvider : Framework.UserControlBase
    {
        public static HtmlEditorProvider Instance()
        {
            return ComponentFactory.GetComponent<HtmlEditorProvider>();
        }
        public abstract System.Web.UI.Control HtmlEditorControl { get; }
        public abstract ArrayList AdditionalToolbars { get; set; }
        public abstract string ControlID { get; set; }
        public abstract string RootImageDirectory { get; set; }
        public abstract string Text { get; set; }
        public abstract System.Web.UI.WebControls.Unit Width { get; set; }
        public abstract System.Web.UI.WebControls.Unit Height { get; set; }
        public abstract void AddToolbar();
        public abstract void Initialize();
    }
}
