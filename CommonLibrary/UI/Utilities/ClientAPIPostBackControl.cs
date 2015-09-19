using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.UI;

namespace CommonLibrary.UI.Utilities
{
    /// <Summary>Control used to register post-back events</Summary>
    public class ClientAPIPostBackControl : Control, IPostBackEventHandler
    {
        private Hashtable m_oEventHandlers;

        public delegate void PostBackEvent(ClientAPIPostBackEventArgs Args);

        public ClientAPIPostBackControl(Page objPage, string strEventName, PostBackEvent objDelegate)
        {
            this.m_oEventHandlers = new Hashtable();
            //ClientAPI.GetPostBackClientEvent(objPage, this, "");
            this.AddEventHandler(strEventName, objDelegate);
        }

        public ClientAPIPostBackControl()
        {
            this.m_oEventHandlers = new Hashtable();
        }

        public PostBackEvent get_EventHandlers(string strEventName)
        {
            if (!this.m_oEventHandlers.Contains(strEventName))
            {
                return null;
            }
            else
            {
                return ((PostBackEvent)this.m_oEventHandlers[strEventName]);
            }
        }

        public void AddEventHandler(string strEventName, PostBackEvent objDelegate)
        {
            if (!this.m_oEventHandlers.Contains(strEventName))
            {
                this.m_oEventHandlers.Add(strEventName, objDelegate);
                return;
            }
            this.m_oEventHandlers[strEventName] = Delegate.Combine((PostBackEvent)this.m_oEventHandlers[strEventName], objDelegate);
        }

        /// <Summary>
        /// Function implementing IPostBackEventHandler which allows the ASP.NET page to invoke
        /// the control's events
        /// </Summary>
        public virtual void RaisePostBackEvent(string strEventArgument)
        {
            ClientAPIPostBackEventArgs clientAPIPostBackEventArgs1 = new ClientAPIPostBackEventArgs(strEventArgument);
            if (this.get_EventHandlers(clientAPIPostBackEventArgs1.EventName) == null)
            {
                return;
            }
            this.get_EventHandlers(clientAPIPostBackEventArgs1.EventName)(clientAPIPostBackEventArgs1);
        }
    }

}
