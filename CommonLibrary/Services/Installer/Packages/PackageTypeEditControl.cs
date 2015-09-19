using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using CommonLibrary.UI.WebControls;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Installer.Packages
{
    [ToolboxData("<{0}:PackageTypeEditControl runat=server></{0}:PackageTypeEditControl>")]
    public class PackageTypeEditControl : TextEditControl
    {
        protected override void RenderEditMode(System.Web.UI.HtmlTextWriter writer)
        {
            List<PackageType> packageTypes = PackageController.GetPackageTypes();
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);
            writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString);
            if (StringValue == Null.NullString)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Option);
            writer.Write("<" + Localization.Localization.GetString("Not_Specified", Localization.Localization.SharedResourceFile) + ">");
            writer.RenderEndTag();
            foreach (PackageType type in packageTypes)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Value, type.Type);
                if (type.Type == StringValue)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(type.Type);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }
    }
}
