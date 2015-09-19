using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Messaging.Data;
using CommonLibrary.Entities.Users;
using System.Collections;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Scheduling;
using System.ComponentModel;
using System.Threading;
using CommonLibrary.Entities.Host;

namespace CommonLibrary.Services.Messaging.Scheduler
{
    public class MessagingScheduler : SchedulerClient
    {

        AsyncCompletedEventArgs asyncCompletedEventArgs;
        AutoResetEvent waitHandle;
        PortalController _pController = new PortalController();
        MessagingController _mController = new MessagingController();

        UserController _uController = new UserController();

        public MessagingScheduler(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
            waitHandle = new AutoResetEvent(false);
        }

        public override void DoWork()
        {

            try
            {
                Guid _schedulerInstance = Guid.NewGuid();
                this.ScheduleHistoryItem.AddLogNote("MessagingScheduler DoWork Starting " + _schedulerInstance.ToString());

                if ((string.IsNullOrEmpty(Host.SMTPServer)))
                {
                    this.ScheduleHistoryItem.AddLogNote("No SMTP Servers have been configured for this host. Terminating task.");
                    this.ScheduleHistoryItem.Succeeded = true;
                    //'Return
                }
                else
                {
                    Hashtable settings = this.ScheduleHistoryItem.GetSettings();

                    bool _messageLeft = true;
                    int _messagesSent = 0;


                    while (_messageLeft)
                    {
                        Message currentMessage = _mController.GetNextMessageForDispatch(_schedulerInstance);

                        if ((currentMessage != null))
                        {
                            try
                            {
                                SendMessage(currentMessage);
                                _messagesSent = _messagesSent + 1;
                            }
                            catch (Exception e)
                            {
                                this.Errored(ref e);
                            }
                        }
                        else
                        {
                            _messageLeft = false;
                        }

                    }

                    this.ScheduleHistoryItem.AddLogNote(string.Format("Message Scheduler '{0}' sent a total of {1} message(s)", _schedulerInstance, _messagesSent));
                    this.ScheduleHistoryItem.Succeeded = true;

                }

            }
            catch (Exception ex)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("MessagingScheduler Failed: " + ex.ToString());
                this.Errored(ref ex);
            }
        }

        private void SendMessage(Message objMessage)
        {
            string senderAddress = UserController.GetUserById(objMessage.PortalID, objMessage.FromUserID).Email;
            string fromAddress = _pController.GetPortal(objMessage.PortalID).Email;
            string toAddress = _uController.GetUser(objMessage.PortalID, objMessage.ToUserID).Email;


            Mail.Mail.SendEmail(fromAddress, senderAddress, toAddress, objMessage.Subject, objMessage.Body);

            _mController.MarkMessageAsDispatched(objMessage.MessageID);

        }
    }

}
