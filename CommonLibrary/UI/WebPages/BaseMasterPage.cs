using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace CommonLibrary.UI.WebPages
{
    public abstract class BaseMasterPage : System.Web.UI.MasterPage
    {
        #region Protected Fields

        protected HtmlControl App_Error;
        protected Label lblErrorMessage;

        protected HtmlControl App_Warning;
        protected Label lblWarningMessage;

        protected HtmlControl App_Information;
        protected Label lblInformationMessage;

        protected Menu mnuMain;
        protected SiteMapPath SiteMapPath1;

        protected Label lblHeader;
        protected Label lblFooter;
        #endregion

        #region Public Properties
        public string HeaderText
        {
            get
            {
                return lblHeader.Text;
            }
            set
            {
                lblHeader.Text = value;
            }
        }

        public string FooterText
        {
            get
            {
                return lblFooter.Text;
            }
            set
            {
                lblFooter.Text = value;
            }
        }


        // Sets the Application Level error message.  
        public string ErrorMessage
        {
            set
            {
                lblErrorMessage.Text = value.Replace("\n", "<br />");
                App_Error.Visible = true;
            }
        }

        public string WarningMessage
        {
            set
            {
                lblWarningMessage.Text = value.Replace("\n", "<br />");
                App_Warning.Visible = true;
            }
        }

        public string InformationMessage
        {
            set
            {
                lblInformationMessage.Text = value.Replace("\n", "<br />");
                App_Information.Visible = true;
            }
        }

        public Menu MainMenu { get { return mnuMain; } }

        public SiteMapPath SiteMapPath { get { return SiteMapPath1; } }

        #endregion
    }
}
