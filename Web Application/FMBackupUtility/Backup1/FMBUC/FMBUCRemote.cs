using System;
using System.Collections.Generic;
using System.Text;

namespace FMBUC
{
    public class MessageEventArgs : EventArgs
    {
        public enum MsgType {MSG_STARTED = 1, MSG_COMPLETE, MSG_FAIL, MSG_STATUS, MSG_ERROR};
//        public enum MsgType {MSG_STATUS = 1, MSG_ERROR, MSG_BURunning};

        private MsgType msgType;
        private DateTime dtEvent;
        private string sMessage;
        
        public MsgType MessageType
        {
            get { return msgType; }
        }
        
        public DateTime EventDateTime
        {
            get { return dtEvent; }
        }
        
        public string Message
        {
            get { return sMessage; }
        }

        public MessageEventArgs(MsgType msgType, string sMessage, DateTime dt)
        {
            this.msgType = msgType;
            this.dtEvent = dt;
            this.sMessage = sMessage;
        }
    }

    public class FMBUCRemote : MarshalByRefObject
    {
        // An event used to send messages to BUC application.
        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        public event MessageEventHandler MessageEvent;

        public FMBUCRemote()
        {
        }
        
        public override Object InitializeLifetimeService()
        {
            // Infinite lifetime.
 	        return null;//base.InitializeLifetimeService();
        }
        
        // Remote method to send a message to BUC remote server.
        public void UpdateMessage(MessageEventArgs.MsgType msgType, string sMessage, DateTime dt)
        {
            // Raise an event.
            if (MessageEvent != null) MessageEvent(this, new MessageEventArgs(msgType, sMessage, dt));
        }
    }
}
