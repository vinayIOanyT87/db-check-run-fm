namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface ITestSetEquipmentResults
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, TestSetEquipmentResultClass testSetEquipmentResult);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, TestSetEquipmentResultClass testSetEquipmentResult);

		[OperationContract]
		TestSetEquipmentResultClass Get(SecurityClass security, Guid testSetEquipmentResultGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid testSetEquipmentResultGuid);

		[OperationContract]
		TestSetEquipmentResultCollectionClass Enumerate(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate);

		[OperationContract]
		TestSetEquipmentResultCollectionClass EnumerateByEquipmentGuid(SecurityClass security, Guid equipmentGuid);

		[OperationContract]
		TestSetEquipmentResultClass GetPreviousSampleNumber(SecurityClass security);

		[OperationContract]
		bool FindDuplicateSampleNumber(SecurityClass security, int sampleNumber, Guid testSetEquipmentResultGuid);

		[OperationContract]
		string DetailPageReference();
	}
}
