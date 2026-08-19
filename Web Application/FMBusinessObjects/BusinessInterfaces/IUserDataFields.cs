using System;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IUserDataFields
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, UserDataFieldClass userDataField);

		[OperationContract]
		UserDataFieldClass Get(SecurityClass security, Guid identityGuid, ENTITY_TYPE entityType);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, ENTITY_TYPE entityType, Guid transactionAliasGuid, int Number, bool dispatchField);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, UserDataFieldClass userDataField);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid identityGuid, ENTITY_TYPE userDataFieldEntityType);

		[OperationContract]
		UserDataFieldCollectionClass Enumerate(SecurityClass security, ENTITY_TYPE userDataFieldEntityType);

		[OperationContract]
		UserDataFieldCollectionClass EnumerateByEntityType(SecurityClass security, ENTITY_TYPE entityType, Guid transactionAliasGuid, bool byUser, bool dispatchField);
	}
}
