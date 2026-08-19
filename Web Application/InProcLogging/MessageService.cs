namespace InProcLogging
{
    public class MessageService
    {
        private ChannelDirectory mChannels = new ChannelDirectory();
        private static MessageService mInstance = null;
        private MessageService()
        {
        }
        public static MessageService instance()
        {
            if (mInstance == null)
            {
                mInstance = new MessageService();
            }
            return mInstance;
        }
        public bool sendMessage(Message aMsg, string aTo)
        {
            if (aMsg == null || aTo == null || aTo.CompareTo("") == 0)
            {
                return false;
            }
            lock (mChannels)
            {
                return mChannels.send(aTo, aMsg);
            }
        }
        public bool registerMailbox(string aName,MessageQueue aMsgQ)
        {
            if (aName.CompareTo("") == 0 || aMsgQ == null)
            {
                return false;
            }
            lock (mChannels)
            {
                mChannels.add(aName, aMsgQ);
            }
            return true;
        }
        public bool deregisterMailbox(MessageQueue aMsgQ)
        {
            if (aMsgQ == null)
            {
                return false;
            }
            lock (mChannels)
            {
                mChannels.remove(aMsgQ);
            }
            return true;
        }
    }
}
