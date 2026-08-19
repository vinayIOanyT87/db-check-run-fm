// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueries.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IQueries type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IQueries
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		[ReferencePreservingDataContractFormat]
		Guid Add( SecurityClass security, QueryClass query );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		[ReferencePreservingDataContractFormat]
		void Modify( SecurityClass security, QueryClass query );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void PurgeByIdentityGuid( SecurityClass security, Guid identityGuid );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		[ReferencePreservingDataContractFormat]
		void Purge( SecurityClass fuelsManagerSecurityObject, QueryClass query );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		[ReferencePreservingDataContractFormat]
		void PurgeByUser( SecurityClass fuelsManagerSecurityObject, Guid userGuid );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		QueryClass Get(SecurityClass security, Guid queryGuid);

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		QueryClass GetByQuickLoad(SecurityClass security, Guid identityGuid, bool isQuickLoad);

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		QueryClass GetByQueryName( SecurityClass security, string queryName );
				
		[OperationContract]
		[ReferencePreservingDataContractFormat]
		QueryClass GetByNodePath(SecurityClass security, string queryNodePath);
		
		[OperationContract]
		[ReferencePreservingDataContractFormat]
		QueryCollectionClass EnumerateQueryNodes(SecurityClass security);

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		QueryCollectionClass Enumerate( SecurityClass security, bool isQuickLoad );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		QueryClass NewQuery ( SecurityClass security, QueryWriterTopic topic );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		DataSet GetQueryResults( SecurityClass security, QueryClass query, QueryCriteriaPhraseCollection pageFilters );
	}
}
