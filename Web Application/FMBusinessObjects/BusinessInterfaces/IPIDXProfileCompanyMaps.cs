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
	public interface IPIDXProfileCompanyMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, PIDXProfileCompanyMapClass PIDXProfileCompanyMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, PIDXProfileCompanyMapClass PIDXProfileCompanyMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid pidxProfileGuid, Guid companyPersonnelToShipToBillToGuid);

		[OperationContract]
		PIDXProfileCompanyMapClass Get(SecurityClass security, Guid pidxProfileGuid, Guid companyPersonnelToShipToBillToGuid);

		[OperationContract]
		PIDXProfileCompanyMapCollectionClass EnumerateByPIDXProfileGuid(SecurityClass security, Guid pidxProfileGuid);

		[OperationContract]
		PIDXProfileCompanyMapCollectionClass EnumerateSiteAndCompanyPersonnelToShipToBillToGuid(SecurityClass security, Guid companyPersonnelToShipToBillToGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection ( SecurityClass security,
															Guid pidxProfileGuid,
															PIDXProfileCompanyMapCollectionClass newPIDXProfileCompanyMapCollection,
															PIDXProfileCompanyMapCollectionClass existingPIDXProfileCompanyMapCollection );
	}
}
