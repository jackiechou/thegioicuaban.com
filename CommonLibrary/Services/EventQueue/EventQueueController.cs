using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Data;
using System.Diagnostics.Eventing.Reader;
using System.Data;
using CommonLibrary.Services.EventQueue.Config;

namespace CommonLibrary.Services.EventQueue
{
    public class EventQueueController
    {
        private static EventMessage FillMessage(IDataReader dr, bool CheckForOpenDataReader)
        {
            EventMessage message;
            bool canContinue = true;
            if (CheckForOpenDataReader)
            {
                canContinue = false;
                if (dr.Read())
                {
                    canContinue = true;
                }
            }
            if (canContinue)
            {
                message = new EventMessage();
                message.EventMessageID = Convert.ToInt32(Null.SetNull(dr["EventMessageID"], message.EventMessageID));
                message.Priority = (MessagePriority)Enum.Parse(typeof(MessagePriority), Convert.ToString(Null.SetNull(dr["Priority"], message.Priority)));
                message.ProcessorType = Convert.ToString(Null.SetNull(dr["ProcessorType"], message.ProcessorType));
                message.ProcessorCommand = Convert.ToString(Null.SetNull(dr["ProcessorCommand"], message.ProcessorCommand));
                message.Body = Convert.ToString(Null.SetNull(dr["Body"], message.Body));
                message.Sender = Convert.ToString(Null.SetNull(dr["Sender"], message.Sender));
                message.Subscribers = Convert.ToString(Null.SetNull(dr["Subscriber"], message.Subscribers));
                message.AuthorizedRoles = Convert.ToString(Null.SetNull(dr["AuthorizedRoles"], message.AuthorizedRoles));
                message.ExceptionMessage = Convert.ToString(Null.SetNull(dr["ExceptionMessage"], message.ExceptionMessage));
                message.SentDate = Convert.ToDateTime(Null.SetNull(dr["SentDate"], message.SentDate));
                message.ExpirationDate = Convert.ToDateTime(Null.SetNull(dr["ExpirationDate"], message.ExpirationDate));
                string xmlAttributes = Null.NullString;
                xmlAttributes = Convert.ToString(Null.SetNull(dr["Attributes"], xmlAttributes));
                message.DeserializeAttributes(xmlAttributes);
            }
            else
            {
                message = null;
            }
            return message;
        }
        private static EventMessageCollection FillMessageCollection(IDataReader dr)
        {
            EventMessageCollection arr = new EventMessageCollection();
            try
            {
                EventMessage obj;
                while (dr.Read())
                {
                    obj = FillMessage(dr, false);
                    arr.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return arr;
        }
        private static string[] GetSubscribers(string eventName)
        {
            string[] subscribers = null;
            if (EventQueueConfiguration.GetConfig().PublishedEvents[eventName] != null)
            {
                subscribers = Config.EventQueueConfiguration.GetConfig().PublishedEvents[eventName].Subscribers.Split(';');
            }
            else
            {
                subscribers = new string[] { };
            }
            return subscribers;
        }
        public static EventMessageCollection GetMessages(string eventName)
        {
            return FillMessageCollection(DataProvider.Instance().GetEventMessages(eventName));
        }
        public static EventMessageCollection GetMessages(string eventName, string subscriberId)
        {
            return FillMessageCollection(DataProvider.Instance().GetEventMessagesBySubscriber(eventName, subscriberId));
        }
        public static bool ProcessMessages(string eventName)
        {
            return ProcessMessages(GetMessages(eventName));
        }
        public static bool ProcessMessages(string eventName, string subscriberId)
        {
            return ProcessMessages(GetMessages(eventName, subscriberId));
        }
        public static bool ProcessMessages(EventMessageCollection eventMessages)
        {
            EventMessage message;
            for (int messageNo = 0; messageNo <= eventMessages.Count - 1; messageNo++)
            {
                message = eventMessages[messageNo];
                try
                {
                    object oMessageProcessor = Framework.Reflection.CreateObject(message.ProcessorType, message.ProcessorType);
                    if (!((EventMessageProcessorBase)oMessageProcessor).ProcessMessage(message))
                    {
                        throw new Exception();
                    }
                    DataProvider.Instance().SetEventMessageComplete(message.EventMessageID);
                }
                catch
                {
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                    objEventLogInfo.AddProperty("EventQueue.ProcessMessage", "Message Processing Failed");
                    objEventLogInfo.AddProperty("ProcessorType", message.ProcessorType);
                    objEventLogInfo.AddProperty("Body", message.Body);
                    objEventLogInfo.AddProperty("Sender", message.Sender);
                    foreach (string key in message.Attributes.Keys)
                    {
                        objEventLogInfo.AddProperty(key, message.Attributes[key]);
                    }
                    if (!String.IsNullOrEmpty(message.ExceptionMessage))
                    {
                        objEventLogInfo.AddProperty("ExceptionMessage", message.ExceptionMessage);
                    }
                    objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString();
                    objEventLog.AddLog(objEventLogInfo);
                    if (message.ExpirationDate < DateTime.Now)
                    {
                        DataProvider.Instance().SetEventMessageComplete(message.EventMessageID);
                    }
                }
            }
            return true;
        }
        public static bool SendMessage(EventMessage message, string eventName)
        {
            if (message.SentDate != null)
            {
                message.SentDate = DateTime.Now;
            }
            string[] subscribers = GetSubscribers(eventName);
            int intMessageID = Null.NullInteger;
            bool success = true;
            try
            {
                for (int indx = 0; indx <= subscribers.Length - 1; indx++)
                {
                    intMessageID = DataProvider.Instance().AddEventMessage(eventName, (int)message.Priority, message.ProcessorType, message.ProcessorCommand, message.Body, message.Sender, subscribers[indx], message.AuthorizedRoles, message.ExceptionMessage, message.SentDate,
                    message.ExpirationDate, message.SerializeAttributes());
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                success = Null.NullBoolean;
            }
            return success;
        }
    
    }
}
