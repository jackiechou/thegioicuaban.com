using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.EventQueue
{
    public abstract class EventMessageProcessorBase
    {
        public abstract bool ProcessMessage(EventMessage message);
    }
}
