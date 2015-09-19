using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CommonLibrary.Services.Messaging.Data
{
    public interface IMessagingDataService
    {
        IDataReader GetMessageByID(int MessageID);
        IDataReader GetUserInbox(int PortalID, int UserID, int PageNumber, int PageSize);
        int GetInboxCount(int PortalID, int UserID);
        long SaveMessage(Message objMessaging);
        int GetNewMessageCount(int PortalID, int UserID);
        IDataReader GetNextMessageForDispatch(Guid SchedulerInstance);
        void MarkMessageAsDispatched(int MessageID);
        void UpdateMessage(Message message);
    }
}
