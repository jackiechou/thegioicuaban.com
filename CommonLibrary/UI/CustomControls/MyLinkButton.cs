using System;
using System.Web.UI;
using System.Collections;

namespace CommonLibrary.UI.CustomControls
{
    public class MyLinkButton : Control, IPostBackEventHandler
    {

        // Defines the Click event.
        //
        public event EventHandler Click;

        // Invokes delegates registered with the Click event.
        //
        protected virtual void OnClick(EventArgs e)
        {

            if (Click != null)
            {
                Click(this, e);
            }
        }


        // Method of IPostBackEventHandler that raises change events.
        //
        public void RaisePostBackEvent(string eventArgument)
        {

            OnClick(new EventArgs());
        }

        protected override void Render(HtmlTextWriter output)
        {

            output.Write("<a  id=\"" + this.UniqueID + "\" href=\"javascript:" + Page.GetPostBackEventReference(this) + "\">");
            output.Write(" " + this.UniqueID + "</a>");
        }
    }    
}
