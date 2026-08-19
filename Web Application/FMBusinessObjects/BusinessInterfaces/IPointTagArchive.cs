namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ServiceModel;

	using DataObjects;
	using Cassandra;

	[InheritedExport]
    [ServiceContract]
	public interface IPointTagArchive
	{
		[OperationContract]
		void InitializeArchive(SecurityClass security);

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void AddArchiveData( SecurityClass security, List<ArchiveDataElement> archiveDataElementList );

		[OperationContract]
		List<List<TrendArchiveDataElement>> GetTrendArchiveData(SecurityClass security, List<Guid> tagGuids, DateTimeOffset start, DateTimeOffset end);

        [OperationContract]
        List<List<TrendArchiveDataElement>> GetHistoryArchiveData(SecurityClass security, List<Guid> tagGuids, DateTimeOffset start, DateTimeOffset end, int numberOfSamplesPerPen);

		[OperationContract]
		List<SimpleArchiveDataElement> GetArchiveDataValues(SecurityClass security, List<Guid> tagGuids, DateTimeOffset start, DateTimeOffset end);
	}
}
