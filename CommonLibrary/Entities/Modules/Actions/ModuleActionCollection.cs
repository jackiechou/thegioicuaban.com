using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CommonLibrary.Security;

namespace CommonLibrary.Entities.Modules.Actions
{
    public class ModuleActionCollection : CollectionBase
    {
        public ModuleActionCollection()
        {
        }
        public ModuleActionCollection(ModuleActionCollection value)
        {
            AddRange(value);
        }
        public ModuleActionCollection(ModuleAction[] value)
        {
            AddRange(value);
        }
        public ModuleAction this[int index]
        {
            get { return (ModuleAction)List[index]; }
            set { List[index] = value; }
        }
        public int Add(ModuleAction value)
        {
            return List.Add(value);
        }
        public ModuleAction Add(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, bool UseActionEvent, SecurityAccessLevel Secure, bool Visible, bool NewWindow)
        {
            return this.Add(ID, Title, CmdName, CmdArg, Icon, Url, "", UseActionEvent, Secure, Visible,
            NewWindow);
        }
        public ModuleAction Add(int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent, SecurityAccessLevel Secure, bool Visible,
        bool NewWindow)
        {
            ModuleAction ModAction = new ModuleAction(ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, Visible,
            NewWindow);
            this.Add(ModAction);
            return ModAction;
        }
        public void AddRange(ModuleAction[] value)
        {
            int i;
            for (i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }
        public void AddRange(ModuleActionCollection value)
        {
            foreach (ModuleAction mA in value)
            {
                Add(mA);
            }
        }
        public bool Contains(ModuleAction value)
        {
            return this.Contains(value);
        }
        public ModuleAction GetActionByCommandName(string name)
        {
            ModuleAction retAction = null;
            foreach (ModuleAction modAction in List)
            {
                if (modAction.CommandName == name)
                {
                    retAction = modAction;
                    break;
                }
                if (modAction.HasChildren())
                {
                    ModuleAction childAction = modAction.Actions.GetActionByCommandName(name);
                    if (childAction != null)
                    {
                        retAction = childAction;
                        break;
                    }
                }
            }
            return retAction;
        }
        public ModuleActionCollection GetActionsByCommandName(string name)
        {
            ModuleActionCollection retActions = new ModuleActionCollection();
            foreach (ModuleAction modAction in List)
            {
                if (modAction.CommandName == name)
                {
                    retActions.Add(modAction);
                }
                if (modAction.HasChildren())
                {
                    retActions.AddRange(modAction.Actions.GetActionsByCommandName(name));
                }
            }
            return retActions;
        }
        public ModuleAction GetActionByID(int id)
        {
            ModuleAction retAction = null;
            foreach (ModuleAction modAction in List)
            {
                if (modAction.ID == id)
                {
                    retAction = modAction;
                    break;
                }
                if (modAction.HasChildren())
                {
                    ModuleAction childAction = modAction.Actions.GetActionByID(id);
                    if (childAction != null)
                    {
                        retAction = childAction;
                        break;
                    }
                }
            }
            return retAction;
        }
        public int IndexOf(ModuleAction value)
        {
            return List.IndexOf(value);
        }
        public void Insert(int index, ModuleAction value)
        {
            List.Insert(index, value);
        }
        public void Remove(ModuleAction value)
        {
            List.Remove(value);
        }
    }
}
