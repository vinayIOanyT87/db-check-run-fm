
namespace FMPointCommon
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using InProcLogging;

	public interface ISMRTimerAction
	{
		void PerformAction(Guid pointGuid);
	}

	[Serializable]
	public class TimerInfo : ICloneable
	{
		public string TimerName;

		public Guid PointGuid;

		public DateTimeOffset UtcExpirationTime;

		public object Clone()
		{
			var ret = new TimerInfo
			          {
				          TimerName = this.TimerName,
				          PointGuid = this.PointGuid,
				          UtcExpirationTime = this.UtcExpirationTime
			          };
			return ret;
		}
	}

	public class SRMTimerService : SrmThread
	{
		protected static SRMTimerService Inst = null;

		protected ISMRTimerAction TimerAction = null;

		protected Dictionary<string, TimerInfo> TimerDictionary = new Dictionary<string, TimerInfo>();

		protected Mailbox MailBox = null;

		protected int MaxQueueCount = 1000; 

		protected SRMTimerService(int maxQueueCount)
		{
			this.MaxQueueCount = maxQueueCount;
		}

		protected void SetTimerAction(ISMRTimerAction action)
		{
			this.TimerAction = action;
		}

		public static SRMTimerService Initialize(ISMRTimerAction timerAction, int maxQueueCount)
		{
			var inst = SRMTimerService.Instance(maxQueueCount);
			inst.SetTimerAction(timerAction);
			inst.SetThreadPrioirty(ThreadPriority.Highest);
			inst.Start();
			return inst;
		}

		public static void Term()
		{
			var inst = SRMTimerService.Instance(1000);
			inst.Terminate();
		}

		public static SRMTimerService Instance(int maxQueueCount)
		{
			if (Inst == null)
			{
				Inst = new SRMTimerService(maxQueueCount);
			}
			return Inst;
		}

		protected void HandleAddTimer(TimerInfo tInfo)
		{
			if (this.TimerDictionary.ContainsKey(tInfo.TimerName))
			{
				this.TimerDictionary[tInfo.TimerName] = tInfo;
			}
			else
			{
				this.TimerDictionary.Add(tInfo.TimerName,tInfo);
			}
		}

		protected void HandleRemoveTimer(TimerInfo tInfo)
		{
			if (this.TimerDictionary.ContainsKey(tInfo.TimerName))
			{
				this.TimerDictionary.Remove(tInfo.TimerName);
			}
		}

		protected void ProcessExpiredTimers()
		{
			var now = DateTimeOffset.UtcNow;
			var removalList = new List<TimerInfo>();
			foreach (var tInfo in this.TimerDictionary.Values)
			{
				if (tInfo.UtcExpirationTime < now)
				{
					this.TimerAction.PerformAction(tInfo.PointGuid);
					removalList.Add(tInfo);
				}
			}
			foreach (var tInfo in removalList)
			{
				this.TimerDictionary.Remove(tInfo.TimerName);
			}
		}

		public override void Run()
		{
			this.MailBox = new Mailbox(MaxQueueCount, 1000);
			this.MailBox.registerMailbox(SRMTimerFunctions.MailboxName);

			while (!this.Shutdown)
			{
				try
				{
					Thread.Sleep(100);
					while (this.MailBox.messageCount() > 0)
					{
						var message = this.MailBox.getNextMessage();
						if (message != null)
						{
							var tInfo = (TimerInfo)message.MsgData;
							switch (message.Msg)
							{
								case SRMTimerFunctions.AddTimerMessage:
									this.HandleAddTimer(tInfo);
									break;
								case SRMTimerFunctions.RemoveTimerMessage:
									this.HandleRemoveTimer(tInfo);
									break;
							}
						}
					}
					this.ProcessExpiredTimers();
				}
				catch (Exception e)
				{
					Logger.LogCritical("Exception in SRMTimerService::Run -- " + e.Message);
				}
			}

			this.MailBox.deregisterMailbox();
		}
	}
}
