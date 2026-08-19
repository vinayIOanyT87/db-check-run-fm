using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	public class ShelveDataEntryModel
	{
		public List<Guid> AlarmGuids = new List<Guid>();
			 
		public bool OneShot;

		public int Days;

		public int Hours;

		public int Minutes;

		public bool DoProcessing = true;

		public string AlarmIDs = "";

		public ShelveDataEntryModel()
		{
			
		}

		public ShelveDataEntryModel(Guid alarmGuid, string alarmId)
		{
			this.AlarmGuids.Add(alarmGuid);
			this.AlarmIDs = alarmId;
		}

		public ShelveDataEntryModel(List<Guid> alarmGuidList, string alarmIds)
		{
			this.AlarmGuids = alarmGuidList;
			this.AlarmIDs = alarmIds;
		}
	}
}
