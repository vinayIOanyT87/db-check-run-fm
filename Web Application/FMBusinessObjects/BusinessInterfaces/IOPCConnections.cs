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
	public interface IOPCConnections
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, OPCConnectionClass OPCConnection );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid opcGuid );
	}
}
