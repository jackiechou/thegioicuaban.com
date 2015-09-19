using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Security;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Messaging.Data
{
    [Serializable()]
    public class Message : IHydratable
    {

        #region "Private Members"

        private string _FromUserName;
        private int _FromUserID;
        private string _Body;
        private DateTime _MessageDate;
        private Guid _Conversation;
        private int _MessageID;
        private int _PortalID;
        private int _ReplyTo;
        private MessageStatusType _Status;
        private string _Subject;
        private int _ToUserID;
        private string _ToUserName;
        private bool _EmailSent;
        private bool _skipInbox;
        private int _ToRoleId;

        private bool _allowReply;
        #endregion

        #region "Constructors"

        public Message()
        {
            Conversation = Guid.Empty;
            this.Status = MessageStatusType.Draft;
            this.MessageDate = DateTime.Now;
        }

        #endregion

        #region "Public Properties"

        public string FromUserName
        {
            get { return _FromUserName; }
            private set { _FromUserName = value; }
        }



        public int FromUserID
        {
            get { return _FromUserID; }
            set { _FromUserID = value; }
        }


        public int ToRoleID
        {
            get { return _ToRoleId; }
            set { _ToRoleId = value; }
        }



        public bool AllowReply
        {
            get { return _allowReply; }
            set { _allowReply = value; }
        }



        public bool SkipInbox
        {
            get { return _skipInbox; }
            set { _skipInbox = value; }
        }

        public bool EmailSent
        {
            get { return _EmailSent; }
            set { _EmailSent = value; }
        }



        public string Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        public DateTime MessageDate
        {
            get { return _MessageDate; }
            set { _MessageDate = value; }
        }

        public Guid Conversation
        {
            get { return _Conversation; }
            set { _Conversation = value; }
        }

        public int MessageID
        {
            get { return _MessageID; }
            private set { _MessageID = value; }
        }



        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }

        public int ReplyTo
        {
            get { return _ReplyTo; }
            private set { _ReplyTo = value; }
        }

        public MessageStatusType Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string Subject
        {
            get
            {
                PortalSecurity ps = new PortalSecurity();
                return ps.InputFilter(_Subject, PortalSecurity.FilterFlag.NoMarkup);
            }
            set
            {
                PortalSecurity ps = new PortalSecurity();
                ps.InputFilter(value, PortalSecurity.FilterFlag.NoMarkup);
                _Subject = ps.InputFilter(value, PortalSecurity.FilterFlag.NoMarkup);
            }
        }


        public int ToUserID
        {
            get { return _ToUserID; }
            set { _ToUserID = value; }
        }

        public string ToUserName
        {
            get { return _ToUserName; }
            private set { _ToUserName = value; }
        }



        private DateTime _EmailSentDate;
        public DateTime EmailSentDate
        {
            get { return _EmailSentDate; }
            private set { _EmailSentDate = value; }
        }


        private Guid _EmailSchedulerInstance;
        public Guid EmailSchedulerInstance
        {
            get { return _EmailSchedulerInstance; }
            private set { _EmailSchedulerInstance = value; }
        }




        #endregion

        #region "Public Methods"



        public Message GetReplyMessage()
        {
            Message message = new Message();
            message.AllowReply = this.AllowReply;
            message.Body = string.Format("<br><br><br>On {0} {1} wrote ", this.MessageDate, this.FromUserName) + this.Body;
            message.Conversation = this.Conversation;
            message.FromUserID = this.ToUserID;
            message.ToUserID = this.FromUserID;
            message.ToUserName = this.FromUserName;
            message.PortalID = this.PortalID;
            message.ReplyTo = this.MessageID;
            message.SkipInbox = this.SkipInbox;
            message.Subject = "RE:" + this.Subject;

            return message;
        }

        #endregion


        #region "IHydratable Implementation"

        public void Fill(System.Data.IDataReader dr)
        {
            MessageID = Null.SetNullInteger(dr["MessageID"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            FromUserID = Null.SetNullInteger(dr["FromUserID"]);
            FromUserName = Null.SetNullString(dr["FromUserName"]);
            ToUserID = Null.SetNullInteger(dr["ToUserID"]);
            //'_ToUserName = Null.SetNullString(dr.Item("ToUserName"))
            ReplyTo = Null.SetNullInteger(dr["ReplyTo"]);
            Status = (MessageStatusType)Enum.Parse(typeof(MessageStatusType), Null.SetNullString(dr["Status"]));
            Body = Null.SetNullString(dr["Body"]);
            Subject = Null.SetNullString(dr["Subject"]);
            MessageDate = Null.SetNullDateTime(dr["Date"]);
            ToRoleID = Null.SetNullInteger(dr["ToRoleID"]);
            AllowReply = Null.SetNullBoolean(dr["AllowReply"]);
            SkipInbox = Null.SetNullBoolean(dr["SkipPortal"]);
            EmailSent = Null.SetNullBoolean(dr["EmailSent"]);
            ToUserName = Null.SetNullString(dr["ToUserName"]);
            string g = Null.SetNullString(dr["Conversation"]);
            EmailSentDate = Null.SetNullDateTime(dr["EmailSentDate"]);
            EmailSchedulerInstance = Null.SetNullGuid(dr["EmailSchedulerInstance"]);
            Conversation = Null.SetNullGuid(dr["Conversation"]);



            //'Conversation = New Guid(g)
        }

        public int KeyID
        {

            get { return MessageID; }
            set { MessageID = value; }
        }

        #endregion

    }
}
