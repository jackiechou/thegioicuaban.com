using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Messaging.Data;

namespace CommonLibrary.Services.Messaging
{
    public interface IMessagingController
    {
        Message GetMessageByID(int PortalID, int UserID, int MessageID);
        List<Message> GetUserInbox(int PortalID, int UserID, int PageNumber, int PageSize);
        int GetInboxCount(int PortalID, int UserID);
        int GetNewMessageCount(int PortalID, int UserID);
        Message GetNextMessageForDispatch(Guid SchedulerInstance);
        void SaveMessage(Message objMessage);
        void UpdateMessage(Message objMessage);

        void MarkMessageAsDispatched(int MessageID);
    }
}
