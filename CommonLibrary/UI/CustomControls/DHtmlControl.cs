using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Web;
using System.Web.UI;

namespace CommonLibrary.UI.CustomControls
{
    public class DHtmlControl : Control, IPostBackDataHandler
    {
        public event EventHandler SelectedChanged;

        public string Text
        {
            get
            {
                object obj = ViewState["Text"];
                return (obj == null) ? String.Empty : (string)obj;
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        public bool Selected
        {
            get
            {
                object obj = ViewState["Selected"];
                return (obj == null) ? false : (bool)obj;
            }

            set
            {
                ViewState["Selected"] = value;
            }
        }

        protected string HelperID
        {
            get
            {
                return "__" + ClientID + "_State";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Page != null)
            {
                Page.RegisterRequiresPostBack(this);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Page != null)
            {
                Page.ClientScript.RegisterHiddenField(HelperID, Selected.ToString());
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string postback = "";
            if (Page != null)
            {
                postback = Page.GetPostBackEventReference(this) + ";";
            }

            string click = "onclick=\"var sel=getAttribute('selected'); sel = (sel.toLowerCase() == 'true'); sel=!sel; setAttribute('selected', sel.toString());this.style.backgroundColor=sel?'red':'white';" + HelperID + ".value=sel.toString();" + postback + "\"";
            string style = "style=\"cursor:hand;background-color:" + (Selected ? "red" : "white") + "\"";
            string selected = "selected=\"" + Selected.ToString() + "\"";
            writer.Write("<span " + style + " " + click + " " + selected + ">" + Text + "</span>");
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string value = postCollection[HelperID];

            if (value != null)
            {
                bool newValue = (String.Compare(value, "true", true) == 0);
                bool oldValue = Selected;

                Selected = newValue;

                // If there is a change, raise a change event.
                return (newValue != oldValue);
            }

            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            // There was a change,  so raise any events.
            if (SelectedChanged != null)
            {
                SelectedChanged(this, EventArgs.Empty);
            }
        }
    }
}
