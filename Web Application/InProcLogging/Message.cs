using System;

namespace InProcLogging
{
    public class Message : ICloneable
    {
        private int msg = 0;

        public int Msg
        {
            get { return msg; }
            set { msg = value; }
        }

        private string sender = "Anonymous";

        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        private int senderHash = 0;

        public int SenderHash
        {
            get { return senderHash; }
            set { senderHash = value; }
        }

        private string receiver = "Unknown";

        public string Receiver
        {
            get { return receiver; }
            set { receiver = value; }
        }

        private int priority = 0;

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        private ICloneable msgData = null;

        public ICloneable MsgData
        {
            get 
            {
                if (msgData == null)
                {
                    return null;
                }
                return (ICloneable)(msgData.Clone()); 
            }
            set 
            {
                if (value == null)
                {
                    msgData = null;
                }
                else
                {
                    msgData = (ICloneable)(value.Clone());
                }
            }
        }

        public Message()
        {
        }

        public Message(int aMsg, ICloneable aMsgData, int aPriority)
        {
            Msg = aMsg;
            if (aMsgData != null)
            {
                MsgData = (ICloneable)(aMsgData.Clone());
            }
            else
            {
                MsgData = null;
            }
            Priority = aPriority;
        }

        public void CopyFrom(Message aIn)
        {
            Msg = aIn.Msg;
            Sender = aIn.Sender;
            SenderHash = aIn.SenderHash;
            Priority = aIn.Priority;
            if (aIn.MsgData != null)
            {
                MsgData = (ICloneable)(aIn.MsgData.Clone());
            }
            else
            {
                MsgData = null;
            }
        }

        public Object Clone()
        {
            Message ret = new Message();
            ret.CopyFrom(this);
            return ret;
        }

    }
}