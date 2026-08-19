using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IChangeQueueRecordsClass
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, ChangeQueueRecordClass record);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ChangeQueueRecordClass record);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ProcessChangeQueueRecords(SecurityClass security, ChangeQueueRecordClass record, EntityToSiteMapCollectionClass siteMapCollection);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid changeQueueGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void SetAllIncomplete(SecurityClass security, long startIndex, long stopIndex);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void SetAllCompleted(SecurityClass security, long startIndex, long stopIndex);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void SetAllCompletedByCollection(SecurityClass security, ChangeQueueRecordCollection recordCollection);

		[OperationContract]
		ChangeQueueRecordCollection EnumerateByDate(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate);

		[OperationContract]
		ChangeQueueRecordCollection EnumerateIncompleteRecords(SecurityClass security);
	}
}
