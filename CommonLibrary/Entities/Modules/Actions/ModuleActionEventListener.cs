using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Modules.Actions
{
    public class ModuleActionEventListener
    {
        private int _moduleID;
        private ActionEventHandler _actionEvent;
        public ModuleActionEventListener(int ModID, ActionEventHandler e)
        {
            _moduleID = ModID;
            _actionEvent = e;
        }
        public int ModuleID
        {
            get { return _moduleID; }
        }
        public ActionEventHandler ActionEvent
        {
            get { return _actionEvent; }
        }
    }
}
