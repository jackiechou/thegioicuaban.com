using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security;

namespace CommonLibrary.Entities.Modules.Actions
{
    public class ModuleAction
    {
        private ModuleActionCollection _actions;
        private int _id;
        private string _commandName;
        private string _commandArgument;
        private string _title;
        private string _icon;
        private string _url;
        private string _clientScript;
        private bool _useActionEvent;
        private SecurityAccessLevel _secure;
        private bool _visible;
        private bool _newwindow;
        public ModuleAction(int ID) :
            this(ID, "", "", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {
        }
        public ModuleAction(int ID, string Title, string CmdName) :
            this(ID, Title, CmdName, "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {
        }
        public ModuleAction(int ID, string Title, string CmdName, string CmdArg) :
            this(ID, Title, CmdName, CmdArg, "", "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {

        }
        public ModuleAction(int ID, string Title, string CmdName, string CmdArg, string Icon) :
            this(ID, Title, CmdName, CmdArg, Icon, "", "", false, SecurityAccessLevel.Anonymous, true, false)
        {

        }
        public ModuleAction(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url) :
            this(ID, Title, CmdName, CmdArg, Icon, Url, "", false, SecurityAccessLevel.Anonymous, true, false)
        {

        }
        public ModuleAction(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript) :
            this(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, false, SecurityAccessLevel.Anonymous, true, false)
        {

        }
        public ModuleAction(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent) :
            this(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, SecurityAccessLevel.Anonymous, true, false)
        {

        }
        public ModuleAction(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent, SecurityAccessLevel Secure) :
            this(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, true, false)
        {

        }
        public ModuleAction(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent, SecurityAccessLevel Secure, bool Visible) :
            this(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, Visible, false)
        {

        }
        public ModuleAction(
            int ID,
            string Title,
            string CmdName,
            string CmdArg,
            string Icon,
            string Url,
            string ClientScript,
            bool UseActionEvent,
            SecurityAccessLevel Secure,
            bool Visible,
            bool NewWindow)
        {
            _id = ID;
            _title = Title;
            _commandName = CmdName;
            _commandArgument = CmdArg;
            _icon = Icon;
            _url = Url;
            _clientScript = ClientScript;
            _useActionEvent = UseActionEvent;
            _secure = Secure;
            _visible = Visible;
            _newwindow = NewWindow;
        }
        public bool HasChildren()
        {
            return (Actions.Count > 0);
        }
        public ModuleActionCollection Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new ModuleActionCollection();
                }
                return _actions;
            }
            set { _actions = value; }
        }
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public SecurityAccessLevel Secure
        {
            get { return _secure; }
            set { _secure = value; }
        }
        public string CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }
        public string CommandArgument
        {
            get { return _commandArgument; }
            set { _commandArgument = value; }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        public string ClientScript
        {
            get { return _clientScript; }
            set { _clientScript = value; }
        }
        public bool UseActionEvent
        {
            get { return _useActionEvent; }
            set { _useActionEvent = value; }
        }
        public bool NewWindow
        {
            get { return _newwindow; }
            set { _newwindow = value; }
        }
    }
}
