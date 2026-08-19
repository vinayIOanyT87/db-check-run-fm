
namespace FMPointService.AlarmAndEventArchive
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMPointCommon;

	using InProcLogging;

	public class AlarmAndEventArchiveThread : SrmThread
	{
		protected static AlarmAndEventArchiveThread Inst = null;

		protected Mailbox MailBox = null;

		protected SecurityClass Security;

		protected AlarmAndEventArchiveThread()
		{
		}

		public static AlarmAndEventArchiveThread Initialize(SecurityClass security)
		{
			var inst = AlarmAndEventArchiveThread.Instance();
			inst.Security = security;
			inst.SetThreadPrioirty(ThreadPriority.Highest);
			inst.Start();
			return inst;
		}

		public static void Term()
		{
			var inst = AlarmAndEventArchiveThread.Instance();
			inst.Terminate();
		}

		public static AlarmAndEventArchiveThread Instance()
		{
			if (Inst == null)
			{
				Inst = new AlarmAndEventArchiveThread();
			}
			return Inst;
		}

		public void LogEventsToArchive(PointValue pv)
		{
			try
			{
            // TankStatus in Manual is calculated by TankCommands module based on TankCommand

				if (pv.InputOutputType != PointTemplateTag.PointTagInputOutputType.FCEE &&
				pv.InputOutputType != PointTemplateTag.PointTagInputOutputType.OpcUa &&
				pv.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated)
					return;

				if (pv.OpcStatusCodeBits == Opc.Ua.StatusCodes.GoodLocalOverride)
					return;

				if (pv == null || pv.Value == null)
					return;

				if ((pv.Value.GetType() == typeof(System.Boolean) && pv.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated) ||
					pv.Value.GetType() == typeof(FMBusinessObjects.DataObjects.CodedVariables.TankStatuses) ||
					pv.Value.GetType() == typeof(FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses) ||
					pv.Value.GetType() == typeof(FMBusinessObjects.DataObjects.PointCommandStatusListReference))
				{
					Message msg = new Message();
					AandEDataElement AandEElem = new AandEDataElement(Security, pv);
					msg.MsgData = AandEElem;
					this.MailBox.sendMessage(msg, AlarmAndEventArchiveFunctions.MailboxName);
				}
			}
			catch(Exception e)
			{
				Logger.LogError("Exception in LogEventsToArchive: " + e.Message);
			}
		}


		public override void Run()
		{
			this.MailBox = new Mailbox(1000, 1000);
			this.MailBox.registerMailbox(AlarmAndEventArchiveFunctions.MailboxName);

			while (!this.Shutdown)
			{
				try
				{
					Thread.Sleep(1000);
					var aAndEList = new List<AandEDataElement>();
					while (this.MailBox.messageCount() > 0)
					{
						var message = this.MailBox.getNextMessage();
						if (message != null)
						{
							switch (message.Msg)
							{
								case AlarmAndEventArchiveFunctions.AlarmAndEventArchiveMessage:
									aAndEList.Add((AandEDataElement)message.MsgData);
									break;
							}
						}
					}
					if (aAndEList.Count > 0)
					{
						FMChannelHelper.MakeCall<IAandEArchive>(x => x.AddArchiveData(this.Security, aAndEList));
					}
				}
				catch (Exception e)
				{
					Logger.LogCritical("Exception in AlarmAndEventArchiveThread::Run -- " + e.Message);
				}
			}

			this.MailBox.deregisterMailbox();
		}

	}
}
