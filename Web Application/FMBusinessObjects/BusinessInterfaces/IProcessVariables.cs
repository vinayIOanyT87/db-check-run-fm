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
	public interface IProcessVariables
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, ProcessVariableClass ProcessVariable );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, DATA_TYPE Type, ProcessVariableClass ProcessVariable );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection ( SecurityClass security, Guid guid, ProcessVariableCollectionClass NewProcessVariableCollection, ProcessVariableCollectionClass ExistingProcessVariableCollection );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid ProcessVariableGuid, UNIT_TYPE targetUnitType );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		bool ProcessVariableAlreadyUsed(SecurityClass security, ProcessVariableClass pv);
	}
}
