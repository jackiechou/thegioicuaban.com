using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.UI.Utilities
{
    /// <Summary>
    /// Event arguments passed to a delegate associated to a client postback event
    /// </Summary>
    public class ClientAPIPostBackEventArgs
    {
        public Hashtable EventArguments;
        public string EventName;

        public ClientAPIPostBackEventArgs(string strEventArgument)
        {
            //this.EventArguments = new Hashtable();
            //string[] splitter = { ClientAPI.COLUMN_DELIMITER };
            //string[] strings = strEventArgument.Split(splitter, StringSplitOptions.None);
            //if (strings.Length > 0)
            //{
            //    this.EventName = strings[0];
            //}
            //int len = strings.Length - 1;
            //for (int i = 1; i <= len; i += 2)
            //{
            //    this.EventArguments.Add(strings[i], strings[(i + 1)]);
            //}
        }

        public ClientAPIPostBackEventArgs()
        {
            this.EventArguments = new Hashtable();
        }
    }

}
