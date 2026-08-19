using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
    using System.Data;

    [ServiceContract]
	public interface IIATACodes
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, IATACodeClass IATACode);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, IATACodeClass IATACode);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid identityGuid);

		[OperationContract]
		IATACodeClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string IATACodeID);

		[OperationContract]
		IATACodeCollectionClass Enumerate(SecurityClass security);

        [OperationContract]
        DataSet EnumerateWithFilter(SecurityClass security, string filterString);

		[OperationContract]
		IATACodeCollectionClass EnumerateWhereCoordinatesExist(SecurityClass security);

		[OperationContract]
		IATACodeCollectionClass EnumerateByPrefix(SecurityClass security, string Prefix);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Import(SecurityClass security, IATACodeClass IATACode);
	}
}
