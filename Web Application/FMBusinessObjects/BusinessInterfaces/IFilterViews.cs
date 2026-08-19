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
	public interface IFilterViews
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, FilterViewClass filter );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Update ( SecurityClass security, FilterViewClass filter );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, FilterViewClass filter );

		[OperationContract]
		FilterViewClass GetByIdentityGuid ( SecurityClass security, Guid filterViewGuid );

		[OperationContract]
		FilterViewsCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		FilterViewsCollectionClass EnumerateByTransTypeID ( SecurityClass security, TransactionTypes type );
	}
}
