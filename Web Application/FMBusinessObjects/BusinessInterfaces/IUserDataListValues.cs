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
	public interface IUserDataListValues
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add(SecurityClass security, UserDataListValueClass UserDataListValue, ENTITY_TYPE userDataFieldEntityType);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid UserDataFieldGuid, string Value, ENTITY_TYPE userDataFieldEntityType);

		[OperationContract]
		UserDataListValueCollectionClass Enumerate(SecurityClass security, Guid userDataFieldGuid, ENTITY_TYPE userDataFieldEntityType);
	}
}
