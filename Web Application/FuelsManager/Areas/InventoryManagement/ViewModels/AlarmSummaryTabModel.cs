

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using FMBusinessObjects.DataObjects;


	[Serializable]
	public class AlarmSummaryTabModel
	{
		public List<AlarmStatusClass2> AlarmSummaries;

		public bool HasAcknowledgeAllRight;

		public bool HasAcknowledgeCommentsRight;

		public bool HasSilenceRight;

		public bool HasViewPointDetailRight;

	}
}
