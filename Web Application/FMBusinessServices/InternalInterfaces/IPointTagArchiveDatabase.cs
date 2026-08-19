namespace FMBusinessServices.InternalInterfaces
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;
	using Cassandra;

	internal interface IPointTagArchiveDatabase
	{
		void Initialize(SecurityClass security);

		void AddArchiveData(SecurityClass security, List<ArchiveDataElement> archiveDataElementList);

		List<List<TrendArchiveDataElement>> GetTrendArchiveData(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end);

		List<List<TrendArchiveDataElement>> GetLeakArchiveData(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end);

        List<List<TrendArchiveDataElement>> GetHistoryArchiveData(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end, int numberOfSamplesPerPen);

        List<ArchiveDataElement> GetArchiveData(SecurityClass security, DateTimeOffset startDateTimeOffset, Guid siteGuid, out bool moreData, out SynchronizationElement synchronizationElement);

		void SynchronizationComplete(SecurityClass security, SynchronizationElement synchronizationElement);
		List<SimpleArchiveDataElement> GetArchiveDataValues(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end);
	}
}
