

namespace FMPointCommon
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using FMBusinessObjects.DataObjects;

	using InProcLogging;

	public class AlarmAndEventArchiveFunctions
	{
		public const string MailboxName = "AlarmAndEventArchiveThread_MailboxName";

		public const int AlarmAndEventArchiveMessage = 0;

		public static void Archive(AandEDataElement ae)
		{
			var msg = new Message(AlarmAndEventArchiveMessage, ae, 1);
			MessageService.instance().sendMessage(msg, MailboxName);
		}
	}
}
