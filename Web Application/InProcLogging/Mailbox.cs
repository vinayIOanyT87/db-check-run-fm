namespace InProcLogging
{
    public class Mailbox : IMailbox
    {
        private MessageQueue mQueue = null;
        private bool mRegistered = false;
        private string mName = "UNKOWN";

        public int Timeout
        {
            get { return mQueue.Timeout; }
            set { mQueue.Timeout = value; }
        }

        public Mailbox(int aMaxMsgCount, int aMsTimeout)
        {
            mQueue = new MessageQueue(aMaxMsgCount, aMsTimeout);
        }
        public Message getNextMessage()
        {
            return mQueue.get();
        }
        public Message peekNextMessage()
        {
            return mQueue.peek();
        }
        public int messageCount()
        {
            return mQueue.queueCount();
        }
        public bool registerMailbox(string aName)
        {
            if (mRegistered)
            {
                return false;
            }
            bool ret = MessageService.instance().registerMailbox(aName,mQueue);
            if (ret)
            {
                mRegistered = true;
                mName = aName;
            }
            return ret;
        }
        public bool deregisterMailbox()
        {
            if (!mRegistered)
            {
                return false;
            }
            bool ret = MessageService.instance().deregisterMailbox(mQueue);
            if (ret)
            {
                mRegistered = false;
                mName = "UNKNOWN";
            }
            return ret;
        }
        public bool sendMessage(Message aMsg, string aTo)
        {
            aMsg.Sender = mName;
            aMsg.SenderHash = mQueue.GetHashCode();
            aMsg.Receiver = aTo;
            return MessageService.instance().sendMessage(aMsg, aTo);
        }
    }
}
