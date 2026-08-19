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
	public interface IGetTransactionProcessor
	{
		[OperationContract]
		GetTransactionDO Process ( GetTransactionSR getTransactionSR );

		[OperationContract]
		bool IsTransactionDuplicate ( Guid transactionGuid, string aliasName, SecurityClass security );
	}
}
