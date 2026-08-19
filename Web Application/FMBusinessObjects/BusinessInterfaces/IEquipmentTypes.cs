using System;
using System.Data;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Data;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IEquipmentTypes
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, EquipmentTypeClass equipmentType);

		[OperationContract]
		EquipmentTypeCollectionClass Enumerate(SecurityClass security, string filter, string order);

		[OperationContract]
		DataSet EnumerateDataSet(SecurityClass security, string filter, string order);

		[OperationContract]
		EquipmentTypeClass Get(SecurityClass security, Guid equipmentTypeGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Import(SecurityClass security, EquipmentTypeClass EquipmentType);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, EquipmentTypeClass EquipmentType);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid equipmentTypeGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyOnlyQualificationsAndTrainings(SecurityClass security, EquipmentTypeClass equipmentType);
	}
}
