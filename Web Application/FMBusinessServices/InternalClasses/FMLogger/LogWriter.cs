using System;

namespace FMBusinessServices.InternalClasses.FMLogger
{
	/// <summary>
	/// Summary description for LogWriter.
	/// </summary>
	internal class LogWriter
	{
		#region Attributes
		protected LogQueue queue;
		protected System.Collections.Hashtable targetList;
		internal System.Threading.Thread writeThread;
		internal bool bStop;
		internal string lockHandle = "";
		private LoggerImpl loggerImpl;
		protected int timeoutMinutes = 1;

		#endregion Attributes

		public LogWriter(LoggerImpl loggerImpl/*, LogQueue queue*/)
		{
			bStop = false;
			this.loggerImpl = loggerImpl;
			this.queue = LoggerImpl.queue;
			targetList = new System.Collections.Hashtable();
			writeThread = new System.Threading.Thread(new System.Threading.ThreadStart(StartWriteThread));
			writeThread.Name = "LogWriter.WriteThread()";
		}

		~LogWriter()
		{
		}

		public void Close()
		{
			lock (lockHandle)
			{
				foreach (LogFile logFile in targetList)
				{
					logFile.Close();
				}
				targetList.Clear();
				targetList = null;
			}
		}
		internal void CreateLog(string appName)
		{
			if (targetList.ContainsKey(appName) == false)
			{
				targetList.Add(appName, new LogFile(appName));
			}
			lock (lockHandle)
			{
				((BaseTarget)targetList[appName]).AddRef();
			}
		}
		internal void RemoveLog(string appName)
		{
			if (targetList.ContainsKey(appName) == false)
			{
				throw new Exception("Tried to remove non-existent log file [" + appName + "].");
			}
			BaseTarget target = (BaseTarget)targetList[appName];

			lock (lockHandle)
			{
				target.Close();
				targetList.Remove(appName);
			}

		}

		void StartWriteThread()
		{
			//Every 5 minutes we will check to see if we should close some seldom used log files.
			System.Timers.Timer logTimeout = new System.Timers.Timer(1000 /*ms*/ * 60 /*s*/ * timeoutMinutes /*m*/);
			logTimeout.AutoReset = true; ;
			logTimeout.Elapsed += new System.Timers.ElapsedEventHandler(logTimeout_Elapsed);
			logTimeout.Start();

			int dayOfYear = DateTimeOffset.Now.DayOfYear;
			while (bStop == false)
			{
				if (queue.Count == 0)
				{
					System.Threading.Thread.Sleep(50 /*ms*/);
				}
				while (queue.Count > 0)
				{
					if (DateTimeOffset.Now.DayOfYear != dayOfYear)
					{
						RollLogs();
						dayOfYear = DateTimeOffset.Now.DayOfYear;
					}
					LogMessage message;
					lock (lockHandle)
					{
						message = (LogMessage)queue.Dequeue();
						if (targetList.ContainsKey(message.AppName) == false)
						{
							loggerImpl.CreateLog(message.AppName);
						}
					}
					BaseTarget target = (BaseTarget)targetList[message.AppName];
					target.Log(message);
				}
			}
		}

		protected void RollLogs()
		{
			// a hashtable is an enumeration of DictionaryEntry, not an enumeration
			// of the value-type
			foreach (System.Collections.DictionaryEntry targetEntry in targetList)
			{
				BaseTarget target = targetEntry.Value as BaseTarget;
				if (target != null)
				{
					target.RollLog();
				}
			}
		}

		private void logTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			int maxUnusedLogs = 3;
			int maxLogs = 5;

			DateTimeOffset now = DateTimeOffset.Now;
			System.Collections.SortedList removeList = new System.Collections.SortedList();

			lock (lockHandle)
			{
				foreach (BaseTarget target in targetList.Values)
				{
					removeList.Add(target, target);
				}
				//If we are above the max number of logs to stay open, close the logs least recently used 
				//until we are within the limit.
				while (removeList.Count > maxLogs)
				{
					loggerImpl.RemoveLog(((BaseTarget)removeList.GetKey(removeList.Count - 1)).AppName);
					removeList.RemoveAt(removeList.Count - 1);
				}
				//If we still have more seldom-used logs than maxUnusedLogs, then remove the least recently used
				// ones until we are within the limit.
				bool inLimit = false;
				while (inLimit == false)
				{
					if (removeList.Count <= maxUnusedLogs)
					{
						inLimit = true;
						break;
					}
					int testIndex = removeList.Count - maxUnusedLogs;
					BaseTarget target = (BaseTarget)removeList.GetKey(testIndex);
					TimeSpan timeSinceUsed = now - target.LastAccessed;
					if (timeSinceUsed.Minutes > timeoutMinutes)
					{
						loggerImpl.RemoveLog(target.AppName);
						removeList.RemoveAt(testIndex);
					}
					else
					{
						inLimit = true;
					}
				}
			}
		}
	}
}
