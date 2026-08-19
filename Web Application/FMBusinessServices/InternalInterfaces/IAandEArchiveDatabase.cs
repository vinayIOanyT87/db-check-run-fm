namespace FMBusinessServices.InternalInterfaces
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;
	using Cassandra;
	internal interface IAandEArchiveDatabase
	{
		
		void Initialize(SecurityClass security);
		
		void AddArchiveData(SecurityClass security, List<AandEDataElement> AandeDataElementList);

		List<string> GetColumnFilterData(SecurityClass security,int selectedColumn, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList);

		List<AandEDataElement> GetAandEArchiveData(SecurityClass security, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, int recordTypeFilter);

		Tuple<string, DateTimeOffset> UpdateAandEComment(SecurityClass security, DateTimeOffset timeStamp, Guid alarmAndEventRecordGuid, string comment);

		List<AandEDataElement> GetArchiveData(SecurityClass security, DateTimeOffset startDateTimeOffset, Guid siteGuid, out bool moreData, out AlarmAndEventSynchronizationElement synchronizationElement);

		void SynchronizationComplete(SecurityClass security, AlarmAndEventSynchronizationElement synchronizationElement);

	}
}