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
	public interface ISequences
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Save( SecurityClass security, SequenceClass Sequence );

		[OperationContract]
		Int64 Get( SecurityClass security, string Key );

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Purge( SecurityClass security, string Key );
	}
}
