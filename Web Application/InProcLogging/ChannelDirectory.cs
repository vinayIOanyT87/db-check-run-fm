using System.Collections;

namespace InProcLogging
{
    public class ChannelDirectory
    {
        #region Classes

        private class Channel
        {
            private string name = "";

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public int Count
            {
                get
                {
                    return queueList.Count;
                }
            }
            private ArrayList queueList = new ArrayList();
            public void removeQueue(MessageQueue aQueue)
            {
                queueList.Remove(aQueue);
            }
            public void addQueue(MessageQueue aQueue)
            {
                if (!doesQueueExist(aQueue))
                {
                    queueList.Add(aQueue);
                }
            }
            public bool doesQueueExist(MessageQueue aQueue)
            {
                foreach (MessageQueue q in queueList)
                {
                    if (q.Equals(aQueue))
                    {
                        return true;
                    }
                }
                return false;
            }
            public bool isChannel(string aName)
            {
                if (aName.CompareTo(Name) == 0)
                {
                    return true;
                }
                return false;
            }
            public bool send(Message aMsg)
            {
                bool ret = true;
                foreach (MessageQueue q in queueList)
                {

                    if (q.put((Message)aMsg.Clone()) == false)
                    {
                        ret = false; ;
                    }
                }
                return ret;
            }
        }

        #endregion

        #region Fields

        private ArrayList mChannelList = new ArrayList();

        #endregion

        #region Methods

        public void add(string aName, MessageQueue aMsgQ)
        {
            foreach (Channel c in mChannelList)
            {
                if (c.isChannel(aName))
                {
                    c.addQueue(aMsgQ);
                    return;
                }
            }
            Channel newChan = new Channel();
            newChan.Name = aName;
            newChan.addQueue(aMsgQ);
            mChannelList.Add(newChan);
        }
        public void remove(MessageQueue aMsgQ)
        {
            ArrayList del = new ArrayList();
            foreach (Channel c in mChannelList)
            {
                c.removeQueue(aMsgQ);
                if (c.Count <= 0)
                {
                    del.Add(c);
                }
            }
            foreach (Channel d in del)
            {
                mChannelList.Remove(d);
            }
        }
        public bool send(string aName, Message aMsg)
        {
            foreach (Channel c in mChannelList)
            {
                if (c.isChannel(aName))
                {
                    return c.send(aMsg);
                }
            }
            return false;
        }

        #endregion
    }
}
