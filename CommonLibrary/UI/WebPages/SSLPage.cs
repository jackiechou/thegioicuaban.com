using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonLibrary.UI.WebPages
{
    public class SSLPage : System.Web.UI.Page
    {
        [Browsable(true)]
        [Description("Indicates whether or not this page should be forced into or out of SSL")]
        private bool _RequireSSL;
        public virtual bool RequireSSL
        {
            get { return _RequireSSL; }
            set { _RequireSSL = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PushSSL();
        }

        [System.Diagnostics.DebuggerStepThrough()]
        [System.Diagnostics.Conditional("SECURE")]
        private void PushSSL()
        {
            const string SECURE = "https://";
            const string UNSECURE = "http://";

            //Force required into secure channel
            if (RequireSSL && Request.IsSecureConnection == false)
                Response.Redirect(Request.Url.ToString().Replace(UNSECURE, SECURE));

            //Force non-required out of secure channel
            if (!RequireSSL && Request.IsSecureConnection == true)
                Response.Redirect(Request.Url.ToString().Replace(SECURE, UNSECURE));
        }


        private void InitializeComponent()
        {
            this.RequireSSL = true;
            //Other initialization code would be here also
        }
    }
}
