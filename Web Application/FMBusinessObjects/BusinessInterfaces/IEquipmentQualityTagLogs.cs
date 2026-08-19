using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IEquipmentQualityTagLogs
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, EquipmentQualityTagLogClass oEquipmentQualityTagLog);

		[OperationContract]
		EquipmentQualityTagLogCollectionClass Enumerate(SecurityClass security, bool bHistorical);

		[OperationContract]
		DataSet GetDataSet(SecurityClass security,
								bool bHistorical,
								string sDateType,
								DateTimeOffset dateStart,
								DateTimeOffset dateEnd,
								string qualityTag,
								string taggedBy,
								string removedBy,
								string assetID,
								string state);

		[OperationContract]
		EquipmentQualityTagLogClass GetByTagNumber(SecurityClass security, int tagNumber);

		[OperationContract]
		EquipmentQualityTagLogClass Get(SecurityClass security, Guid equipmentQualityTagLogGuid);

		[OperationContract]
		EquipmentQualityTagLogClass GetMostRecentByEquipmentID(SecurityClass security, string equipmentID);

		[OperationContract]
		EquipmentQualityTagLogClass GetPreviousTagNumber(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, EquipmentQualityTagLogClass oEquipmentQualityTagLog);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid equipmentQualityTagLogGuid);

		[OperationContract]
		string QueryWriterSQL(SecurityClass security, string selectClause, string dbName);

		[OperationContract]
		void QueryWriterPostProcess(SecurityClass security, DataSet set);

		[OperationContract]
		string DetailPageReference();
	}
}
