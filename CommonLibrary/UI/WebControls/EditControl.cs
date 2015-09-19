using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.UI;
using CommonLibrary.Security;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.WebControls
{
    [ValidationPropertyAttribute("Value")]
    public abstract class EditControl : WebControl, IPostBackDataHandler
    {
        private object[] _CustomAttributes;
        private PropertyEditorMode _EditMode;
        private string _LocalResourceFile;
        private string _Name;
        private object _OldValue;
        private bool _Required;
        private string _SystemType;
        private object _Value;
        public event PropertyChangedEventHandler ItemAdded;
        public event PropertyChangedEventHandler ItemDeleted;
        public event PropertyChangedEventHandler ValueChanged;
        public EditControl()
        {
        }
        public object[] CustomAttributes
        {
            get { return _CustomAttributes; }
            set
            {
                _CustomAttributes = value;
                if ((_CustomAttributes != null) && _CustomAttributes.Length > 0)
                {
                    OnAttributesChanged();
                }
            }
        }
        public PropertyEditorMode EditMode
        {
            get { return _EditMode; }
            set { _EditMode = value; }
        }
        public virtual bool IsValid
        {
            get { return true; }
        }
        public string LocalResourceFile
        {
            get { return _LocalResourceFile; }
            set { _LocalResourceFile = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public object OldValue
        {
            get { return _OldValue; }
            set { _OldValue = value; }
        }
        public bool Required
        {
            get { return _Required; }
            set { _Required = value; }
        }
        public string SystemType
        {
            get { return _SystemType; }
            set { _SystemType = value; }
        }
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        protected abstract void OnDataChanged(System.EventArgs e);
        protected abstract string StringValue { get; set; }
        protected virtual void OnAttributesChanged()
        {
        }
        protected virtual void OnItemAdded(PropertyEditorEventArgs e)
        {
            //if (ItemAdded != null)
            //{
            //    ItemAdded(this, e);
            //}
        }
        protected virtual void OnItemDeleted(PropertyEditorEventArgs e)
        {
            //if (ItemDeleted != null)
            //{
            //    ItemDeleted(this, e);
            //}
        }
        protected virtual void OnValueChanged(PropertyEditorEventArgs e)
        {
            //if (ValueChanged != null)
            //{
            //    ValueChanged(this, e);
            //}
        }
        protected virtual void RenderViewMode(System.Web.UI.HtmlTextWriter writer)
        {
            string propValue = this.Page.Server.HtmlDecode(Convert.ToString(this.Value));
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            PortalSecurity security = new PortalSecurity();
            writer.Write(security.InputFilter(propValue, PortalSecurity.FilterFlag.NoScripting));
            writer.RenderEndTag();
        }
        protected virtual void RenderEditMode(System.Web.UI.HtmlTextWriter writer)
        {
            string propValue = Convert.ToString(this.Value);
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, propValue);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            string strOldValue = OldValue as string;
            if (EditMode == PropertyEditorMode.Edit || (Required && string.IsNullOrEmpty(strOldValue)))
            {
                RenderEditMode(writer);
            }
            else
            {
                RenderViewMode(writer);
            }
        }
        public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            bool dataChanged = false;
            string presentValue = StringValue;
            string postedValue = postCollection[postDataKey];
            if (!presentValue.Equals(postedValue))
            {
                Value = postedValue;
                dataChanged = true;
            }
            return dataChanged;
        }
        public void RaisePostDataChangedEvent()
        {
            OnDataChanged(System.EventArgs.Empty);
        }
    }
}
