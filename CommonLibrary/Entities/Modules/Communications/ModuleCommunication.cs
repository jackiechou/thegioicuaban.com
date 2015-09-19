using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Modules.Communications
{
    public interface IModuleCommunicator
    {
        event ModuleCommunicationEventHandler ModuleCommunication;
    }
    public interface IModuleListener
    {
        void OnModuleCommunication(object s, ModuleCommunicationEventArgs e);
    }
    public delegate void ModuleCommunicationEventHandler(object sender, ModuleCommunicationEventArgs e);
    public class RoleChangeEventArgs : ModuleCommunicationEventArgs
    {
        private string _RoleId = null;
        private string _PortalId = null;
        public string PortalId
        {
            get { return _PortalId; }
            set { _PortalId = value; }
        }
        public string RoleId
        {
            get { return _RoleId; }
            set { _RoleId = value; }
        }
    }
    public class ModuleCommunicationEventArgs : System.EventArgs
    {
        private string _Type = null;
        private object _Value = null;
        private string _Sender = null;
        private string _Target = null;
        private string _Text = null;
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public string Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }
        public string Target
        {
            get { return _Target; }
            set { _Target = value; }
        }
        public ModuleCommunicationEventArgs()
        {
        }
        public ModuleCommunicationEventArgs(string Type, object value, string Sender, string Target)
        {
            _Type = Type;
            _Value = value;
            _Sender = Sender;
            _Target = Target;
        }
        public ModuleCommunicationEventArgs(string Text)
        {
            _Text = Text;
        }
    }
    public class ModuleCommunicators : System.Collections.CollectionBase
    {
        public IModuleCommunicator this[int index]
        {
            get { return (IModuleCommunicator)List[index]; }
            set { List[index] = value; }
        }
        public ModuleCommunicators()
        {
        }
        public int Add(IModuleCommunicator item)
        {
            return this.List.Add(item);
        }
    }
    public class ModuleListeners : System.Collections.CollectionBase
    {
        public IModuleListener this[int index]
        {
            get { return (IModuleListener)List[index]; }
            set { List[index] = value; }
        }
        public ModuleListeners()
        {
        }
        public int Add(IModuleListener item)
        {
            return this.List.Add(item);
        }
    }
    public class ModuleCommunicate
    {
        private ModuleCommunicators _ModuleCommunicators = new ModuleCommunicators();
        private ModuleListeners _ModuleListeners = new ModuleListeners();
        public ModuleCommunicators ModuleCommunicators
        {
            get { return _ModuleCommunicators; }
        }
        public ModuleListeners ModuleListeners
        {
            get { return _ModuleListeners; }
        }
        public ModuleCommunicate()
        {
        }
        public void LoadCommunicator(System.Web.UI.Control ctrl)
        {
            if (ctrl is IModuleCommunicator)
            {
                this.Add((IModuleCommunicator)ctrl);
            }
            if (ctrl is IModuleListener)
            {
                this.Add((IModuleListener)ctrl);
            }
        }
        private int Add(IModuleCommunicator item)
        {
            int returnData = _ModuleCommunicators.Add(item);
            int i;
            for (i = 0; i <= _ModuleListeners.Count - 1; i++)
            {
                item.ModuleCommunication += _ModuleListeners[i].OnModuleCommunication;
            }
            return returnData;
        }
        private int Add(IModuleListener item)
        {
            int returnData = _ModuleListeners.Add(item);
            int i;
            for (i = 0; i <= _ModuleCommunicators.Count - 1; i++)
            {
                _ModuleCommunicators[i].ModuleCommunication += item.OnModuleCommunication;
            }
            return returnData;
        }
    }
}
