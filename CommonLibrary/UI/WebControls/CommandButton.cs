using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using CommonLibrary.Framework;
using System.Web.UI;

namespace CommonLibrary.UI.WebControls
{
    [ToolboxData("<{0}:CommandButton runat=server></{0}:CommandButton>")]
    public class CommandButton : WebControl, INamingContainer
    {
        private LinkButton link;
        private ImageButton icon;
        private LiteralControl separator;
        private string _onClick;
        private string _resourceKey;
        public event EventHandler Click;
        public event CommandEventHandler Command;
        public string ButtonSeparator
        {
            get
            {
                this.EnsureChildControls();
                return separator.Text;
            }
            set
            {
                this.EnsureChildControls();
                separator.Text = value;
            }
        }
        public bool CausesValidation
        {
            get
            {
                this.EnsureChildControls();
                return link.CausesValidation;
            }
            set
            {
                this.EnsureChildControls();
                icon.CausesValidation = value;
                link.CausesValidation = value;
            }
        }
        public string CommandArgument
        {
            get
            {
                this.EnsureChildControls();
                return link.CommandArgument;
            }
            set
            {
                this.EnsureChildControls();
                icon.CommandArgument = value;
                link.CommandArgument = value;
            }
        }
        public string CommandName
        {
            get
            {
                this.EnsureChildControls();
                return link.CommandName;
            }
            set
            {
                this.EnsureChildControls();
                icon.CommandName = value;
                link.CommandName = value;
            }
        }
        public bool DisplayLink
        {
            get
            {
                this.EnsureChildControls();
                return link.Visible;
            }
            set
            {
                this.EnsureChildControls();
                link.Visible = value;
            }
        }
        public bool DisplayIcon
        {
            get
            {
                this.EnsureChildControls();
                return icon.Visible;
            }
            set
            {
                this.EnsureChildControls();
                icon.Visible = value;
            }
        }
        public string ImageUrl
        {
            get
            {
                this.EnsureChildControls();
                return icon.ImageUrl;
            }
            set
            {
                this.EnsureChildControls();
                icon.ImageUrl = value;
            }
        }
        public string OnClick
        {
            get
            {
                this.EnsureChildControls();
                return link.Attributes["onclick"];
            }
            set
            {
                this.EnsureChildControls();
                if (String.IsNullOrEmpty(value))
                {
                    icon.Attributes.Remove("onclick");
                    link.Attributes.Remove("onclick");
                }
                else
                {
                    icon.Attributes.Add("onclick", value);
                    link.Attributes.Add("onclick", value);
                }
            }
        }
        public string ResourceKey
        {
            get
            {
                this.EnsureChildControls();
                return link.Attributes["resourcekey"];
            }
            set
            {
                this.EnsureChildControls();
                if (String.IsNullOrEmpty(value))
                {
                    icon.Attributes.Remove("resourcekey");
                    link.Attributes.Remove("resourcekey");
                }
                else
                {
                    icon.Attributes.Add("resourcekey", value);
                    link.Attributes.Add("resourcekey", value);
                }
            }
        }
        public string Text
        {
            get
            {
                this.EnsureChildControls();
                return link.Text;
            }
            set
            {
                this.EnsureChildControls();
                icon.AlternateText = value;
                icon.ToolTip = value;
                link.Text = value;
                link.ToolTip = value;
            }
        }
        public string ValidationGroup
        {
            get
            {
                this.EnsureChildControls();
                return link.ValidationGroup;
            }
            set
            {
                this.EnsureChildControls();
                icon.ValidationGroup = value;
                link.ValidationGroup = value;
            }
        }
        protected override void CreateChildControls()
        {
            Controls.Clear();
            if (String.IsNullOrEmpty(CssClass))
                CssClass = "CommandButton";
            icon = new ImageButton();
            icon.Visible = true;
            icon.CausesValidation = true;
            icon.Click += RaiseImageClick;
            icon.Command += RaiseCommand;
            this.Controls.Add(icon);
            separator = new LiteralControl();
            separator.Text = "&nbsp;";
            this.Controls.Add(separator);
            link = new LinkButton();
            link.Visible = true;
            link.CausesValidation = true;
            link.Click += RaiseClick;
            link.Command += RaiseCommand;
            this.Controls.Add(link);
            if (DisplayIcon == true && !String.IsNullOrEmpty(ImageUrl))
            {
                icon.EnableViewState = this.EnableViewState;
            }
            if (DisplayLink)
            {
                link.CssClass = CssClass;
                link.EnableViewState = this.EnableViewState;
            }
        }
        protected virtual void OnButtonClick(EventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }
        protected virtual void OnCommand(CommandEventArgs e)
        {
            if (Command != null)
            {
                Command(this, e);
            }
        }
        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);
            EnsureChildControls();
            separator.Visible = DisplayLink && DisplayIcon;
        }
        public void RegisterForPostback()
        {
            AJAX.RegisterPostBackControl(link);
            AJAX.RegisterPostBackControl(icon);
        }
        private void RaiseClick(object sender, EventArgs e)
        {
            OnButtonClick(e);
        }
        private void RaiseCommand(object sender, CommandEventArgs e)
        {
            OnCommand(e);
        }
        protected void RaiseImageClick(object sender, ImageClickEventArgs e)
        {
            OnButtonClick(new EventArgs());
        }
    }
}
