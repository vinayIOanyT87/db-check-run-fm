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
	public interface IProductMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, ProductMapClass productMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, ProductMapClass productMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid identityGuid, PRODUCT_MAP_TYPE type);

		[OperationContract]
		ProductMapClass Get(SecurityClass security, Guid identityGuid, PRODUCT_MAP_TYPE type);

		[OperationContract]
		string GetSpecialInstructions(SecurityClass security, Guid identityGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, Guid assignedToGuid, Guid assignedGuid, PRODUCT_MAP_TYPE type);

		[OperationContract]
        ProductMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass security, Guid assignedToGuid, PRODUCT_MAP_TYPE type, bool hideHiddenProducts = false);

		[OperationContract]
		ProductMapCollectionClass EnumerateByAssignedToGuidAndTypeInstr(SecurityClass security, Guid assignedToGuid, PRODUCT_MAP_TYPE type, bool bGetProcessVariables, bool hideHiddenProducts = false);

		[OperationContract]
		ProductMapCollectionClass EnumerateSpecialInstructionsByAssignedToCompany(SecurityClass security, Guid assignedToGuid);

		[OperationContract]
		ProductMapCollectionClass EnumerateByAssignedGuidAndType(SecurityClass security, Guid assignedGuid, PRODUCT_MAP_TYPE type);

		[OperationContract]
		ProductMapCollectionClass EnumerateByAssignedGuidAndTypeAndInstr(SecurityClass security, Guid assignedGuid, PRODUCT_MAP_TYPE type, bool LoadProcessVariables = true);

		[OperationContract]
		ProductMapCollectionClass EnumerateByType ( SecurityClass security, PRODUCT_MAP_TYPE type );

		[OperationContract]
		ProductMapCollectionClass EnumerateByTypeAndLocalize ( SecurityClass security, PRODUCT_MAP_TYPE type, bool bLocalize );

		[OperationContract]
		ProductMapCollectionClass EnumerateByAdditiveProfileGuid(SecurityClass security, PRODUCT_MAP_TYPE productMapType, Guid additiveProfileIdentityGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection ( SecurityClass security,
											Guid identityGuid,
											string id,
											bool byAssignedGuid,
											ProductMapCollectionClass newProductMapCollection,
											ProductMapCollectionClass existingProductMapCollection );
	}
}
