using System.Collections;
using System.Threading;

namespace InProcLogging
{
    public class MessageQueue
    {
        private Queue localQueue = null;
        private int maxQueueSize = 20;
        private int mTimeout = 1000;

        public int Timeout
        {
            get { return mTimeout; }
            set { mTimeout = value; }
        }
        private AutoResetEvent mEvent = new AutoResetEvent(false);

        public MessageQueue(int aMaxQueueSize, int aTimeout)
        {
            maxQueueSize = aMaxQueueSize;
            mTimeout = aTimeout;
            localQueue = new Queue(maxQueueSize, 5);
        }
        public bool put(Message aMsg)
        {
            lock (localQueue)
            {
                if (localQueue.Count >= maxQueueSize)
                {
                    Logger.LogCritical("MessageQueue::put() Queue Full");
                    return false;
                }
                localQueue.Enqueue(aMsg);
            }
            return mEvent.Set();
        }
        public Message peek()
        {
            Message ret = null;
            lock (localQueue)
            {
                if (localQueue.Count > 0)
                {
                    ret = (Message)localQueue.Peek();
                    return ret;
                }
            }
            return null;
        }

        public Message get()
        {
            Message ret = null;
            lock (localQueue)
            {
                if (localQueue.Count > 0)
                {
                    ret = (Message)localQueue.Dequeue();
                    return ret;
                }
            }
            mEvent.WaitOne(mTimeout,false);
            lock (localQueue)
            {
                if (localQueue.Count <= 0)
                {
                    return ret;
                }
                ret = (Message)localQueue.Dequeue();
                return ret;
            }
        }
        public int queueCount()
        {
            lock (localQueue)
            {
                return localQueue.Count;
            }
        }
    }
}
