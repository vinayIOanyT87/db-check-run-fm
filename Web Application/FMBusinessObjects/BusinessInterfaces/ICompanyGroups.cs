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
	public interface ICompanyGroups
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, CompanyGroupClass CompanyGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, CompanyGroupClass CompanyGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid identityGuid);

		[OperationContract]
		CompanyGroupClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		CompanyGroupClass GetByProductIdentityGuid ( SecurityClass security, Guid productGuid );

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string ID );

		[OperationContract]
		CompanyGroupCollectionClass Enumerate ( SecurityClass security );
	}
}
