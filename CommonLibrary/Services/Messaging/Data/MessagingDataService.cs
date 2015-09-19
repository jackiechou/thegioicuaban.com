using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLibrary.Data;

namespace CommonLibrary.Services.Messaging.Data
{
    public class MessagingDataService : IMessagingDataService
    {

        private DataProvider provider = DataProvider.Instance();

        public System.Data.IDataReader GetMessageByID(int messageId)
        {

            return (IDataReader)provider.ExecuteReader("Messaging_GetMessage", messageId);
        }

        public System.Data.IDataReader GetUserInbox(int PortalID, int UserID, int PageNumber, int PageSize)
        {

            return (IDataReader)provider.ExecuteReader("Messaging_GetInbox", PortalID, UserID, PageNumber, PageSize);
        }

        public int GetInboxCount(int PortalID, int UserID)
        {
            return (int)provider.ExecuteScalar("Messaging_GetInboxCount", PortalID, UserID);
        }

        public long SaveMessage(Message objMessaging)
        {
            object messageId = provider.ExecuteScalar("Messaging_Save_Message", objMessaging.PortalID, objMessaging.FromUserID, objMessaging.ToUserID, objMessaging.ToRoleID, (int)objMessaging.Status, objMessaging.Subject, objMessaging.Body, objMessaging.MessageDate, objMessaging.Conversation,
            objMessaging.ReplyTo, objMessaging.AllowReply, objMessaging.SkipInbox);

            return Convert.ToInt64(messageId);
        }

        public int GetNewMessageCount(int PortalID, int UserID)
        {
            return (int)provider.ExecuteScalar("Messaging_GetNewMessageCount", PortalID, UserID);
        }

        public IDataReader GetNextMessageForDispatch(Guid SchedulerInstance)
        {

            return (IDataReader)provider.ExecuteReader("Messaging_GetNextMessageForDispatch", SchedulerInstance);
        }

        public void MarkMessageAsDispatched(int MessageID)
        {
            provider.ExecuteNonQuery("Messaging_MarkMessageAsDispatched", MessageID);
        }

        public void UpdateMessage(Message message)
        {
            provider.ExecuteNonQuery("Messaging_UpdateMessage", message.MessageID, message.ToUserID, message.ToRoleID, (int)message.Status, message.Subject, message.Body, message.MessageDate, message.ReplyTo, message.AllowReply,
            message.SkipInbox);
        }
    }
}
