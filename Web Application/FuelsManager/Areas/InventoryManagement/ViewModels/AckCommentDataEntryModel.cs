using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	public class AckCommentDataEntryModel
	{
		public List<Guid> AlarmGuids = new List<Guid>();

		public string Comment;

		public bool DoProcessing = true;

		public AckCommentDataEntryModel()
		{

		}

		public AckCommentDataEntryModel(Guid alarmGuid)
		{
			this.AlarmGuids.Add(alarmGuid);
		}

		public AckCommentDataEntryModel(List<Guid> alarmGuidList)
		{
			this.AlarmGuids = alarmGuidList;
		}
	}
}
