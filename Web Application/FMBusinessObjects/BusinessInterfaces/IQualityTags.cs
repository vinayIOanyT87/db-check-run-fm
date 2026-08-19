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
	public interface IQualityTags
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, QualityTagClass qualityTag );

		[OperationContract]
		QualityTagClass Get ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, QualityTagClass qualityTag );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid qualityTagGuid );

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string id );

		[OperationContract]
		QualityTagCollectionClass Enumerate ( SecurityClass security, string filter, string order, bool activeTagsOnly );
	}
}
