using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.UI.CustomControls
{
    public class MyButton : Control, IPostBackEventHandler
    {
        // Defines the Click event.
        public event EventHandler Click;

        // Invokes delegates registered with the Click event.
        protected virtual void OnClick(EventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }

        // Method of IPostBackEventHandler that raises change events.
        public void RaisePostBackEvent(string eventArgument)
        {
            OnClick(EventArgs.Empty);
        }

        protected override void Render(HtmlTextWriter output)
        {
            output.Write("<INPUT TYPE=submit name=" + this.UniqueID +
               " Value='Click Me' />");
        }
    }    
}
