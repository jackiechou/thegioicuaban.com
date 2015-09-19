using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Web.UI;
using CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes;

namespace CommonLibrary.UI.WebControls
{
    [ToolboxData("<{0}:TextEditControl runat=server></{0}:TextEditControl>")]
    public class TextEditControl : EditControl
    {
        public TextEditControl()
        {
        }
        public TextEditControl(string type)
        {
            this.SystemType = type;
        }
        protected string OldStringValue
        {
            get { return Convert.ToString(OldValue); }
        }
        protected override string StringValue
        {
            get
            {
                string strValue = Null.NullString;
                if (Value != null)
                {
                    strValue = Convert.ToString(Value);
                }
                return strValue;
            }
            set { this.Value = value; }
        }
        protected override void OnDataChanged(EventArgs e)
        {
            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            args.Value = StringValue;
            args.OldValue = OldStringValue;
            args.StringValue = StringValue;
            base.OnValueChanged(args);
        }
        protected override void RenderEditMode(System.Web.UI.HtmlTextWriter writer)
        {
            int length = Null.NullInteger;
            if ((CustomAttributes != null))
            {
                foreach (System.Attribute attribute in CustomAttributes)
                {
                    if (attribute is MaxLengthAttribute)
                    {
                        MaxLengthAttribute lengthAtt = (MaxLengthAttribute)attribute;
                        length = lengthAtt.Length;
                        break;
                    }
                }
            }
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, StringValue);
            if (length > Null.NullInteger)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, length.ToString());
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }
    }
}
