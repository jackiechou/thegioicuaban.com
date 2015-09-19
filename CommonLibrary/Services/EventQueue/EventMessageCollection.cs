using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Services.EventQueue
{
    public class EventMessageCollection : CollectionBase
    {
        public void Add(EventMessage message)
        {
            this.InnerList.Add(message);
        }
        public virtual EventMessage this[int index]
        {
            get { return (EventMessage)base.List[index]; }
            set { base.List[index] = value; }
        }
    }
}
