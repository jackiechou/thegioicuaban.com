using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Messaging.Data;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.ComponentModel;
using CommonLibrary.Entities.Modules;
using System.Collections;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Messaging
{
    public class MessagingController : IMessagingController
    {

        #region "Private Members"

        private IMessagingDataService _DataService;

        private static TabInfo _MessagingPage = null;
        #endregion

        #region "Constructors"

        public MessagingController()
            : this(GetDataService())
        {
        }

        public MessagingController(IMessagingDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Shared Methods"

        private static IMessagingDataService GetDataService()
        {
            IMessagingDataService ds = ComponentFactory.GetComponent<IMessagingDataService>();

            if (ds == null)
            {
                ds = new MessagingDataService();
                ComponentFactory.RegisterComponentInstance<IMessagingDataService>(ds);
            }
            return ds;
        }

        #endregion

        #region "Public Shared Methods"

        public static string DefaultMessagingURL(string ModuleFriendlyName)
        {
            TabInfo page = MessagingPage(ModuleFriendlyName);
            if (page != null)
            {
                return MessagingPage(ModuleFriendlyName).FullUrl;
            }
            else
            {
                return null;
            }
        }

        public static TabInfo MessagingPage(string ModuleFriendlyName)
        {
            if (((_MessagingPage != null)))
                return _MessagingPage;

            ModuleController mc = new ModuleController();
            ModuleInfo md = mc.GetModuleByDefinition(PortalSettings.Current.PortalId, ModuleFriendlyName);
            if ((md != null))
            {
                ArrayList a = mc.GetModuleTabs(md.ModuleID);
                if ((a != null))
                {
                    ModuleInfo mi = a[0] as ModuleInfo;
                    if ((mi != null))
                    {
                        TabController tc = new TabController();
                        _MessagingPage = tc.GetTab(mi.TabID, PortalSettings.Current.PortalId, false);
                    }
                }
            }

            return _MessagingPage;

        }

        #endregion

        #region "Public Methods"

        public Message GetMessageByID(int PortalID, int UserID, int messageId)
        {
            return (Message)CBO.FillObject(_DataService.GetMessageByID(messageId), typeof(Message));
        }

        public List<Message> GetUserInbox(int PortalID, int UserID, int PageNumber, int PageSize)
        {
            return CBO.FillCollection<Message>(_DataService.GetUserInbox(PortalID, UserID, PageNumber, PageSize));
        }

        public int GetInboxCount(int PortalID, int UserID)
        {
            return _DataService.GetInboxCount(PortalID, UserID);
        }

        public int GetNewMessageCount(int PortalID, int UserID)
        {
            return _DataService.GetNewMessageCount(PortalID, UserID);
        }

        public Message GetNextMessageForDispatch(Guid SchedulerInstance)
        {
            return (Message)CBO.FillObject(_DataService.GetNextMessageForDispatch(SchedulerInstance), typeof(Message));
        }


        public void SaveMessage(Message message)
        {
            if ((PortalSettings.Current != null))
            {
                message.PortalID = PortalSettings.Current.PortalId;
            }

            if ((message.Conversation == null | message.Conversation == Guid.Empty))
            {
                message.Conversation = Guid.NewGuid();
            }

            _DataService.SaveMessage(message);
        }

        public void UpdateMessage(Message message)
        {
            _DataService.UpdateMessage(message);
        }

        public void MarkMessageAsDispatched(int MessageID)
        {
            _DataService.MarkMessageAsDispatched(MessageID);
        }


        #endregion

    }
}
