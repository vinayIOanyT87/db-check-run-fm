using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MockEBSService
{

	public abstract class BaseThread
	{
		protected Thread _thread;
		protected int _sleepTime;
		protected ManualResetEvent _stopEvent;
		protected string ThreadName { get; set; }

		protected BaseThread()
		{
			_stopEvent = new ManualResetEvent(false);
			_thread = new Thread(new ThreadStart(RunMethod));
		}

		protected void RunMethod()
		{
			try
			{
				while (!_stopEvent.WaitOne(_sleepTime))
				{
					ThreadHandler();
				}
			}
			catch (Exception e)
			{
			}
		}

		protected abstract void ThreadHandler();

		public void Start()
		{
			_thread.Start();
		}

		public void Stop()
		{
			_stopEvent.Set();
		}
	}
}

