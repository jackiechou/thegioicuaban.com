using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Modules.Actions
{
    public class ActionEventArgs : EventArgs
    {
        private ModuleAction _action;
        private ModuleInfo _moduleConfiguration;
        public ActionEventArgs(ModuleAction Action, ModuleInfo ModuleConfiguration)
        {
            _action = Action;
            _moduleConfiguration = ModuleConfiguration;
        }
        public ModuleAction Action
        {
            get { return _action; }
        }
        public ModuleInfo ModuleConfiguration
        {
            get { return _moduleConfiguration; }
        }
    }
}
