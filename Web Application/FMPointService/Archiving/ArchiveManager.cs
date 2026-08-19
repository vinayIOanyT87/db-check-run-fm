namespace FMPointService.Archiving
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using Logging;
	using System;

	using FMCore;
    using global::FMPointService.ThreadSupport;

	internal sealed class ArchiveManager
	{
		#region Constants and Fields

		int MaxArchiveRecordsPerCall;

		public ArchiveDataSaver ArchiveDataSaver = new ArchiveDataSaver();

		public StatisticsLogger StatisticsLogger = new StatisticsLogger();

		public ArchiveRecordQueuer ArchiveRecordQueuer = new ArchiveRecordQueuer();

      public ArchiveManager()
      {
			MaxArchiveRecordsPerCall = ThreadSharedData.Instance().MaxArchiveRecordsPerCall;
      }


        #endregion

        #region Methods

        public int Count {
			get
			{
				return ArchiveRecordQueuer.Count;
			}
		}

		public void ProcessArchiveQueue(SecurityClass security)
		{
			security.ThrowIfNull("security");


			var timer = StatisticsLogger.Start("Process Archive Queue");

			while (true)
			{

				List<ArchiveDataElement> archiveList;

				if (ArchiveRecordQueuer.FailedCount > 0)
				{
					archiveList = new List<ArchiveDataElement>(ArchiveRecordQueuer.FailedCount);

					while (ArchiveRecordQueuer.IsFailedEmpty == false)
					{
						if (archiveList.Count < archiveList.Capacity)
						{
							ArchiveDataElement archiveDataElement;
							if (ArchiveRecordQueuer.TryDequeueFailedItem(out archiveDataElement))
							{
								archiveList.Add(archiveDataElement);
							}
						}
						else
						{
							break;
						}
					}


				}
				else
				{
					if (ArchiveRecordQueuer.Count < MaxArchiveRecordsPerCall)
					{
						archiveList = new List<ArchiveDataElement>(ArchiveRecordQueuer.Count);
					}
					else
					{
						archiveList = new List<ArchiveDataElement>(MaxArchiveRecordsPerCall);
					}

					while (ArchiveRecordQueuer.IsEmpty == false)
					{
						if (archiveList.Count < archiveList.Capacity)
						{
							ArchiveDataElement archiveDataElement;
							if (ArchiveRecordQueuer.TryDequeueItem(out archiveDataElement))
							{
								archiveList.Add(archiveDataElement);
							}
						}
						else
						{
							break;
						}
					}
				}


				if (archiveList.Count > 0)
				{
					try
					{
						ArchiveDataSaver.SaveArchiveData(security, archiveList);
					}
					catch (Exception except)
					{
						foreach(var archiveDataElement in archiveList)
						{
							ArchiveRecordQueuer.AddFailedArchiveDataElement(archiveDataElement);
						}

						throw except;
					}
				}
				else
				{
					ArchiveRecordQueuer.QueueOverflowDictionary();
				}

				if (ArchiveRecordQueuer.Count < MaxArchiveRecordsPerCall)
				{
					break;
				}
			}

			StatisticsLogger.Stop(timer);
		}

		#endregion
	}
}
