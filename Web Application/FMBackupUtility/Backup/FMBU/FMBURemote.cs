using System;
using System.Collections.Generic;
using System.Text;

namespace FMBU
{
    public class MessageToBUEventArgs : EventArgs
    {
        public enum MsgType {MSG_BACKUPNOW = 1, MSG_UPDATECONFIG};

        private MsgType msgType;
//        private string sMessage;
        
        public MsgType MessageType
        {
            get { return msgType; }
        }
/*
        public string Message
        {
            get { return sMessage; }
        }
*/
//        public MessageToBUEventArgs(MsgType msgType, string sMessage)
        public MessageToBUEventArgs(MsgType msgType)
        {
            this.msgType = msgType;
//            this.sMessage = sMessage;
        }
    }

    public class FMBURemote : MarshalByRefObject
    {
        // An event used to send messages to BU service.
        public delegate void MessageToBUEventHandler(object sender, MessageToBUEventArgs e);
        public event MessageToBUEventHandler MessageToBUEvent;

        public FMBURemote()
        {
        }
        
        public override Object InitializeLifetimeService()
        {
            // Infinite lifetime.
 	        return null;//base.InitializeLifetimeService();
        }
        
        // Remote method to send a message to BU remote server.
//        public void SendMessageToBU(MessageToBUEventArgs.MsgType msgType, string sMessage)
        public void SendMessageToBU(MessageToBUEventArgs.MsgType msgType)
        {
            // Raise an event.
            if (MessageToBUEvent != null) MessageToBUEvent(this, new MessageToBUEventArgs(msgType));
//            if (MessageToBUEvent != null) MessageToBUEvent(this, new MessageToBUEventArgs(msgType, sMessage));
        }

    }
}
