namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IQualificationMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, QualificationMapClass qualificationMap);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, QualificationMapClass qualificationMap);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid targetGuid, Guid assignedGuid, QUALIFICATION_MAP_TYPE type);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPrimaryKey(SecurityClass security, Guid primaryKey, QUALIFICATION_MAP_TYPE qualificationMapType);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeHistoricalRecord(SecurityClass security,
														Guid targetGuid,
														Guid assignedGuid,
														QUALIFICATION_MAP_TYPE type,
														DateTimeOffset updatedDate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyCollection(
			SecurityClass security,
			Guid targetGuid,
			QualificationMapCollectionClass newQualificationMapCollection,
			QualificationMapCollectionClass existingQualificationMapCollection);

		[OperationContract]
		QualificationMapClass Get(SecurityClass security, Guid targetGuid, Guid assignedGuid, QUALIFICATION_MAP_TYPE type);

		[OperationContract]
		QualificationMapCollectionClass EnumerateByGuidAndType(SecurityClass security,
																					Guid targetGuid,
																					QUALIFICATION_MAP_TYPE type,
																					bool getHistoricalData);

		[OperationContract]
		QualificationMapCollectionClass EnumerateWhereQualificationOrTrainingIsUsed(SecurityClass security, Guid targetGuid);

		[OperationContract]
		QualificationMapCollectionClass EnumerateByAssignedGuid(SecurityClass security, Guid assignedGuid);
	}
}
