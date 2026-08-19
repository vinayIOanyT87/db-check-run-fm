using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

namespace DispatchPrototype
{
	/// <summary>
	/// Summary description for TodaysAppointmentsCollectionClass.
	/// </summary>
	[Serializable()]
	public class TodaysAppointmentsCollectionClass : CollectionBase
	{

		public void Add(TodaysAppointmentClass TodaysAppointments)
		{
			List.Add(TodaysAppointments);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw new Exception("Invalid Index");
			}
			else
			{
				List.RemoveAt(index);
			}

		}

		public void Remove(TodaysAppointmentClass TodaysAppointments)
		{
			int index = 0;

			foreach (TodaysAppointmentClass Item in List)
			{
				if (Item.Index == TodaysAppointments.Index)
				{
					List.RemoveAt(index);
					return;
				}

				index++;

			}

		}

		public TodaysAppointmentClass Item(int Index)
		{
			return (TodaysAppointmentClass)List[Index];
		}

	}


	public class TodaysAppointmentClass
	{
		public int Index;
		public string Description;
		public string AppointmentCategory;
		public int Duration;
		public DateTimeOffset DueDate = new DateTime();
		public string AssetText;
		public bool DoNotNotifyAgain;
		public bool InSleepMode;
		public int NumberToSleep;
		public DateTime SleepTimeInterval = new DateTime();
		public bool AppointmentIsDue;

		public TodaysAppointmentClass()
		{
			Description = "";
			AppointmentCategory = "";
			Duration = 0;
			DueDate = DateTime.Now;
			AssetText = "";
			DoNotNotifyAgain = false;
			InSleepMode = false;
			NumberToSleep = 15;
			SleepTimeInterval = DateTime.Now;
			AppointmentIsDue = false;
		}
	}
}
