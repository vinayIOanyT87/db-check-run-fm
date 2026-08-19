using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMPointCommon
{
	using InProcLogging;

	public class SRMTimerFunctions
	{
		public const string MailboxName = "SRMTimerService_MailboxName";

		public const int AddTimerMessage = 0;

		public const int RemoveTimerMessage = 1;

		public static void AddTimer(string timerName, Guid pointGuid, DateTimeOffset utcExpirationTime)
		{
			var timerInfo = new TimerInfo { TimerName = timerName, PointGuid = pointGuid, UtcExpirationTime = utcExpirationTime };
			var msg = new Message(AddTimerMessage,timerInfo,1);
			MessageService.instance().sendMessage(msg, MailboxName);
		}

		public static void AddTimer(string timerName, Guid pointGuid, int delayInSeconds)
		{
			var utcExpirationTime = DateTimeOffset.UtcNow.AddSeconds(delayInSeconds);
			AddTimer(timerName,pointGuid,utcExpirationTime);
		}

		public static void RemoveTimer(string timerName)
		{
			var timerInfo = new TimerInfo { TimerName = timerName };
			var msg = new Message(RemoveTimerMessage, timerInfo, 1);
			MessageService.instance().sendMessage(msg, MailboxName);
		}
	}
}
