using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public interface IAandEArchive
	{
		[OperationContract]
		void InitializeArchive(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddArchiveData(SecurityClass security, List<AandEDataElement> AandeDataElementList);

		[OperationContract]
		List<string> GetColumnFilterData(SecurityClass security,int selectedColumn, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList);

		[OperationContract]
		Tuple<string, DateTimeOffset> UpdateAandEComment(SecurityClass security, DateTimeOffset timeStamp, Guid alarmAndEventRecordGuid, string comment);

		[OperationContract]
		List<AandEDataElement> GetAandEArchiveData(SecurityClass security, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, int recordTypeFilter);
	}
}
