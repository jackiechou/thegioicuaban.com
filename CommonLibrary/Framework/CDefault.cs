using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using CommonLibrary.UI.Utilities;

namespace CommonLibrary.Framework
{
    public class CDefault : PageBase
    {
        public string Comment = "";
        public string Description = "";
        public string KeyWords = "";
        public string Copyright = "";
        public string Generator = "";
        public string Author = "";
        public new string Title = "";
        public override void Dispose()
        {
            base.Dispose();
        }
        public void AddStyleSheet(string id, string href, bool isFirst)
        {
            Control objCSS = this.FindControl("CSS");
            if (objCSS != null)
            {
                Control objCtrl = Page.Header.FindControl(id);
                if (objCtrl == null)
                {
                    HtmlLink objLink = new HtmlLink();
                    objLink.ID = id;
                    objLink.Attributes["rel"] = "stylesheet";
                    objLink.Attributes["type"] = "text/css";
                    objLink.Href = href;
                    if (isFirst)
                    {
                        int iLink;
                        for (iLink = 0; iLink <= objCSS.Controls.Count - 1; iLink++)
                        {
                            if (objCSS.Controls[iLink] is HtmlLink)
                            {
                                break;
                            }
                        }
                        objCSS.Controls.AddAt(iLink, objLink);
                    }
                    else
                    {
                        objCSS.Controls.Add(objLink);
                    }
                }
            }
        }
        public void AddStyleSheet(string id, string href)
        {
            AddStyleSheet(id, href, false);
        }
        public void ScrollToControl(Control objControl)
        {
            //if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.Positioning))
            //{
            //    ClientAPI.RegisterClientReference(this, ClientAPI.ClientNamespaceReferences.dnn_dom_positioning);
            //    ClientAPI.RegisterClientVariable(this, "ScrollToControl", objControl.ClientID, true);
            //    ClientAPI.AddBodyOnloadEventHandler(Page, "__dnn_setScrollTop();");
            //}
        }
    }
}
