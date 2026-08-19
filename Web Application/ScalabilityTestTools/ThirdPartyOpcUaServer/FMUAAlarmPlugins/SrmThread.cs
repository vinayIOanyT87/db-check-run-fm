using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FMUAAlarmPlugins
{
    public abstract class SrmThread
    {
        protected bool mShutdown = false;
        public bool Shutdown
        {
            get { return mShutdown; }
            set { mShutdown = value; }
        }

        public void Start()
        {
            mSrmThread = new Thread(new ThreadStart(ThrStart));
            if (singleApartment)
            {
                mSrmThread.SetApartmentState(ApartmentState.STA);
            }
            // Since there isn't a paramaterized thread start on CE, we need to store the parameters for 
            // a thread in a dictionary by the thread's ID.
            int id = mSrmThread.ManagedThreadId;
            AddThreadInstanceParameter(id, this);
            mSrmThread.Start();
        }

        public void Join()
        {
            mSrmThread.Join();
        }

        public void Terminate()
        {
            mShutdown = true;
            Join();
        }

        private Thread mSrmThread = null;

        private bool singleApartment = false;

        private static Dictionary<int, SrmThread> threadInstanceParameter = new Dictionary<int, SrmThread>();

        public void setToSingleApartment()
        {
            singleApartment = true;
            if (mSrmThread != null)
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
                mShutdown = false;
                Thread local = Thread.CurrentThread;
                local.IsBackground = true;
                while (mShutdown == false)
                {
                    try
                    {
                        Run();
                    }
                    catch (Exception )
                    {
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
