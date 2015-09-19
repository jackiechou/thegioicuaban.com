using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Localization;
using System.Globalization;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.WebControls
{
    public class LanguageSelector : Control, INamingContainer
    {
        private Panel pnlControl;
        public enum LanguageSelectionMode
        {
            Multiple = 1,
            Single = 2
        }
        public enum LanguageSelectionObject
        {
            NeutralCulture = 1,
            SpecificCulture = 2
        }
        public enum LanguageListDirection
        {
            Horizontal = 1,
            Vertical = 2
        }
        public enum LanguageItemStyle
        {
            FlagOnly = 1,
            FlagAndCaption = 2,
            CaptionOnly = 3
        }
        public LanguageSelectionMode SelectionMode
        {
            get
            {
                if (ViewState["SelectionMode"] == null)
                {
                    return LanguageSelectionMode.Single;
                }
                else
                {
                    return (LanguageSelectionMode)ViewState["SelectionMode"];
                }
            }
            set
            {
                if (SelectionMode != value)
                {
                    ViewState["SelectionMode"] = value;
                    if (this.Controls.Count > 0)
                        CreateChildControls();
                }
            }
        }
        public LanguageSelectionObject SelectionObject
        {
            get
            {
                if (ViewState["SelectionObject"] == null)
                {
                    return LanguageSelectionObject.SpecificCulture;
                }
                else
                {
                    return (LanguageSelectionObject)ViewState["SelectionObject"];
                }
            }
            set
            {
                if ((int)SelectionMode != (int)value)
                {
                    ViewState["SelectionObject"] = value;
                    if (this.Controls.Count > 0)
                        CreateChildControls();
                }
            }
        }
        public LanguageItemStyle ItemStyle
        {
            get
            {
                if (ViewState["ItemStyle"] == null)
                {
                    return LanguageItemStyle.FlagAndCaption;
                }
                else
                {
                    return (LanguageItemStyle)ViewState["ItemStyle"];
                }
            }
            set
            {
                if (ItemStyle != value)
                {
                    ViewState["ItemStyle"] = value;
                    if (this.Controls.Count > 0)
                        CreateChildControls();
                }
            }
        }
        public LanguageListDirection ListDirection
        {
            get
            {
                if (ViewState["ListDirection"] == null)
                {
                    return LanguageListDirection.Vertical;
                }
                else
                {
                    return (LanguageListDirection)ViewState["ListDirection"];
                }
            }
            set
            {
                if (ListDirection != value)
                {
                    ViewState["ListDirection"] = value;
                    if (this.Controls.Count > 0)
                        CreateChildControls();
                }
            }
        }
        //public string[] SelectedLanguages
        //{
        //    get
        //    {
        //        EnsureChildControls();
        //        ArrayList a = new ArrayList();
        //        if (GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture).Length < 2)
        //        {
        //            PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
        //            foreach (string strLocale in Localization.GetLocales(_Settings.PortalId).Keys)
        //            {
        //                a.Add(strLocale);
        //            }
        //        }
        //        else
        //        {
        //            foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
        //            {
        //                if (SelectionMode == LanguageSelectionMode.Single)
        //                {
        //                    if (((RadioButton)pnlControl.FindControl("opt" + c.Name)).Checked)
        //                        a.Add(c.Name);
        //                }
        //                else
        //                {
        //                    if (((CheckBox)pnlControl.FindControl("chk" + c.Name)).Checked)
        //                        a.Add(c.Name);
        //                }
        //            }
        //        }
        //        return a.ToArray(typeof(string)) as string[];
        //    }
        //    set
        //    {
        //        EnsureChildControls();
        //        if (SelectionMode == LanguageSelectionMode.Single && value.Length > 1)
        //            throw new ArgumentException("Selection mode 'single' cannot have more than one selected item.");
        //        foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
        //        {
        //            if (SelectionMode == LanguageSelectionMode.Single)
        //            {
        //                ((RadioButton)pnlControl.FindControl("opt" + c.Name)).Checked = false;
        //            }
        //            else
        //            {
        //                ((CheckBox)pnlControl.FindControl("chk" + c.Name)).Checked = false;
        //            }
        //        }
        //        foreach (string strLocale in value)
        //        {
        //            if (SelectionMode == LanguageSelectionMode.Single)
        //            {
        //                Control ctl = pnlControl.FindControl("opt" + strLocale);
        //                if (ctl != null)
        //                    ((RadioButton)ctl).Checked = true;
        //            }
        //            else
        //            {
        //                Control ctl = pnlControl.FindControl("chk" + strLocale);
        //                if (ctl != null)
        //                    ((CheckBox)ctl).Checked = true;
        //            }
        //        }
        //    }
        //}
        //protected override void CreateChildControls()
        //{
        //    this.Controls.Clear();
        //    pnlControl = new Panel();
        //    this.Controls.Add(pnlControl);
        //    foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
        //    {
        //        System.Web.UI.HtmlControls.HtmlGenericControl lblLocale = new System.Web.UI.HtmlControls.HtmlGenericControl("label");
        //        if (SelectionMode == LanguageSelectionMode.Single)
        //        {
        //            RadioButton optLocale = new RadioButton();
        //            optLocale.ID = "opt" + c.Name;
        //            optLocale.GroupName = pnlControl.ID + "_Locale";
        //            if (c.Name == Localization.SystemLocale)
        //                optLocale.Checked = true;
        //            pnlControl.Controls.Add(optLocale);
        //            lblLocale.Attributes["for"] = optLocale.ClientID;
        //        }
        //        else
        //        {
        //            CheckBox chkLocale = new CheckBox();
        //            chkLocale.ID = "chk" + c.Name;
        //            pnlControl.Controls.Add(chkLocale);
        //            lblLocale.Attributes["for"] = chkLocale.ClientID;
        //        }
        //        pnlControl.Controls.Add(lblLocale);
        //        if (ItemStyle != LanguageItemStyle.CaptionOnly)
        //        {
        //            System.Web.UI.WebControls.Image imgLocale = new System.Web.UI.WebControls.Image();
        //            imgLocale.ImageUrl = ResolveUrl("~/images/Flags/" + c.Name + ".gif");
        //            imgLocale.AlternateText = c.DisplayName;
        //            imgLocale.Style["vertical-align"] = "middle";
        //            lblLocale.Controls.Add(imgLocale);
        //        }
        //        if (ItemStyle != LanguageItemStyle.FlagOnly)
        //        {
        //            lblLocale.Attributes["class"] = "Normal";
        //            lblLocale.Controls.Add(new LiteralControl("&nbsp;" + c.DisplayName));
        //        }
        //        if (ListDirection == LanguageListDirection.Vertical)
        //        {
        //            pnlControl.Controls.Add(new LiteralControl("<br />"));
        //        }
        //        else
        //        {
        //            pnlControl.Controls.Add(new LiteralControl(" "));
        //        }
        //    }
        //    if (GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture).Length < 2)
        //    {
        //        this.Visible = false;
        //    }
        //}
        //private System.Globalization.CultureInfo[] GetCultures(bool specific)
        //{
        //    ArrayList a = new ArrayList();
        //    PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
        //    foreach (string strLocale in Localization.GetLocales(_Settings.PortalId).Keys)
        //    {
        //        System.Globalization.CultureInfo c = new System.Globalization.CultureInfo(strLocale);
        //        if (specific)
        //        {
        //            a.Add(c);
        //        }
        //        else
        //        {
        //            System.Globalization.CultureInfo p = c.Parent;
        //            if (!a.Contains(p))
        //                a.Add(p);
        //        }
        //    }
        //    return (System.Globalization.CultureInfo[])a.ToArray(typeof(CultureInfo));
        //}
    }
}
