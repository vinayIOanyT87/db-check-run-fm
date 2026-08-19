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
	public interface IProductGroups
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, ProductGroupClass ProductGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, ProductGroupClass ProductGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		ProductGroupClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string ID );

		[OperationContract]
		ProductGroupCollectionClass Enumerate ( SecurityClass security );
	}
}
