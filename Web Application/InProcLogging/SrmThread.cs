namespace InProcLogging
{
	using System;
	using System.Collections.Generic;
	using System.Threading;


    public abstract class SrmThread
    {
        protected bool mShutdown = false;
        public bool Shutdown
        {
            get { return this.mShutdown; }
            set { this.mShutdown = value; }
        }

        public bool SetThreadPrioirty(ThreadPriority priority)
        {
            if (mSrmThread == null)
            {
                return false;
            }
            mSrmThread.Priority = priority;
            return true;
        }

        public void Start()
        {
            this.mSrmThread = new Thread(new ThreadStart(ThrStart));
            if (this.singleApartment)
            {
                this.mSrmThread.SetApartmentState(ApartmentState.STA);
            }
            // Since there isn't a paramaterized thread start on CE, we need to store the parameters for 
            // a thread in a dictionary by the thread's ID.
            int id = this.mSrmThread.ManagedThreadId;
            AddThreadInstanceParameter(id, this);
            this.mSrmThread.Start();
        }

        public void Join()
        {
            this.mSrmThread.Join();
        }

        public void Terminate()
        {
            this.mShutdown = true;
            this.Join();
            this.mSrmThread = null;
        }

        private Thread mSrmThread = null;

        private bool singleApartment = false;

        private static Dictionary<int, SrmThread> threadInstanceParameter = new Dictionary<int, SrmThread>();

        public void setToSingleApartment()
        {
            this.singleApartment = true;
            if (this.mSrmThread != null)
            {
                this.mSrmThread.SetApartmentState(ApartmentState.STA);
            }
        }

        public static void ThrStart()
        {
            SrmThread instance = PopThreadInstanceParameter(Thread.CurrentThread.ManagedThreadId);
            if (instance != null)
            {
                instance.ThrProc();
            }
        }

        private void ThrProc()
        {
            try
            {
                this.mShutdown = false;
                Thread local = Thread.CurrentThread;
                local.IsBackground = true;
                while (this.mShutdown == false)
                {
                    try
                    {
                        this.Run();
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("SrmThread::ThrProc Exception: {0}", e);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private static void AddThreadInstanceParameter(int id, SrmThread instance)
        {
            lock (threadInstanceParameter)
            {
                threadInstanceParameter[id] = instance;
            }
        }

        private static SrmThread PopThreadInstanceParameter(int id)
        {
            lock (threadInstanceParameter)
            {
                if (threadInstanceParameter.ContainsKey(id))
                {
                    SrmThread instance = threadInstanceParameter[id];
                    threadInstanceParameter.Remove(id);
                    return instance;
                }
            }
            return null;
        }

        public abstract void Run();

    }
}
