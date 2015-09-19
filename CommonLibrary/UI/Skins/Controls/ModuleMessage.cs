using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;

namespace CommonLibrary.UI.Skins.Controls
{
    public class ModuleMessage : UI.Skins.SkinObjectBase
    {
        public enum ModuleMessageType
        {
            GreenSuccess,
            YellowWarning,
            RedError,
            BlueInfo
        }
        private string _text;
        private string _heading;
        private ModuleMessageType _iconType;
        private string _iconImage;
        protected System.Web.UI.WebControls.Label lblMessage;
        protected System.Web.UI.WebControls.Label lblHeading;
        protected System.Web.UI.WebControls.Image imgIcon;
        protected System.Web.UI.WebControls.Image imgLogo;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public string Heading
        {
            get { return _heading; }
            set { _heading = value; }
        }
        public ModuleMessageType IconType
        {
            get { return _iconType; }
            set { _iconType = value; }
        }
        public string IconImage
        {
            get { return _iconImage; }
            set { _iconImage = value; }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                string strMessage = "";
                if (!String.IsNullOrEmpty(IconImage))
                {
                    strMessage += this.Text;
                    lblHeading.CssClass = "SubHead";
                    lblMessage.CssClass = "Normal";
                    imgIcon.ImageUrl = IconImage;
                    imgIcon.Visible = true;
                }
                else
                {
                    switch (IconType)
                    {
                        case UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess:
                            strMessage += this.Text;
                            lblHeading.CssClass = "SubHead";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/green-ok.gif";
                            imgIcon.Visible = true;
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess.ToString());
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess.ToString());
                            break;
                        case UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning:
                            strMessage += this.Text;
                            lblHeading.CssClass = "Normal";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/yellow-warning.gif";
                            imgIcon.Visible = true;
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning.ToString());
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning.ToString());
                            break;
                        case UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo:
                            strMessage += this.Text;
                            lblHeading.CssClass = "Normal";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/blue-info.gif";
                            imgIcon.Visible = true;
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo.ToString());
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo.ToString());
                            break;
                        case UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError:
                            strMessage += this.Text;
                            lblHeading.CssClass = "NormalRed";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/red-error.gif";
                            imgIcon.Visible = true;
                            imgIcon.AlternateText = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError.ToString());
                            imgIcon.ToolTip = Localization.GetString(UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError.ToString());
                            break;
                    }
                }
                lblMessage.Text = strMessage;
                if (!String.IsNullOrEmpty(Heading))
                {
                    lblHeading.Visible = true;
                    lblHeading.Text = Heading + "<br/>";
                }
            }
            catch (Exception exc)
            {
                Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc, false);
            }
        }
    }
}
