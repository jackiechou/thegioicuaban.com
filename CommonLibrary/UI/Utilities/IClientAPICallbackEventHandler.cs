using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.Utilities
{
    public interface IClientAPICallbackEventHandler
    {
        string RaiseClientAPICallbackEvent(string eventArgument);
    }

}
