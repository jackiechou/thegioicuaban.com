using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Modules;
using CommonLibrary.Entities.Modules.Actions;
using System.Web.UI;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Exceptions;

namespace CommonLibrary.UI.Containers
{
    public abstract class ActionBase : UserControl, IActionControl
    {
        private ActionManager _ActionManager;
        private ModuleAction _ActionRoot;
        private IModuleControl _ModuleControl;
        protected ModuleActionCollection Actions
        {
            get { return ModuleContext.Actions; }
        }
        protected ModuleAction ActionRoot
        {
            get
            {
                if (_ActionRoot == null)
                {
                    _ActionRoot = new ModuleAction(ModuleContext.GetNextActionID(), " ", "", "", "action.gif");
                }
                return _ActionRoot;
            }
        }
        protected ModuleInstanceContext ModuleContext
        {
            get { return ModuleControl.ModuleContext; }
        }
        //protected PortalSettings PortalSettings
        //{
            //get
            //{
            //    PortalSettings _settings = ModuleControl.ModuleContext.PortalSettings;
            //    if (!_settings.ActiveTab.IsSuperTab)
            //    {
            //        m_tabPreview = (_settings.UserMode == PortalSettings.Mode.View);
            //    }
            //    return _settings;
            //}
        //}
        //protected bool DisplayControl(NodeCollection objNodes)
        //{
        //    return ActionManager.DisplayControl(objNodes);
        //}
        public bool EditMode
        {
            get { return ModuleContext.PortalSettings.UserMode != PortalSettings.Mode.View; }
        }
        protected virtual void OnAction(ActionEventArgs e)
        {
            if (Action != null)
            {
                Action(this, e);
            }
        }
        protected void ProcessAction(string ActionID)
        {
            int output;
            if (Int32.TryParse(ActionID, out output))
            {
                ModuleAction action = Actions.GetActionByID(output);
                if (action != null)
                {
                    if (!ActionManager.ProcessAction(action))
                    {
                        OnAction(new ActionEventArgs(action, ModuleContext.Configuration));
                    }
                }
            }
        }
        protected bool m_supportsIcons = true;
        public bool SupportsIcons
        {
            get { return m_supportsIcons; }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                ActionRoot.Actions.AddRange(Actions);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        public event ActionEventHandler Action;
        public ActionManager ActionManager
        {
            get
            {
                if (_ActionManager == null)
                {
                    _ActionManager = new ActionManager(this);
                }
                return _ActionManager;
            }
        }
        public IModuleControl ModuleControl
        {
            get { return _ModuleControl; }
            set { _ModuleControl = value; }
        }
      
    }
}
