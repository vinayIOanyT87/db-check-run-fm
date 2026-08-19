using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IPriceCalculatorInvoker
	{
		[OperationContract]
		TransactionDO CalculateWithLineItems ( SecurityClass security, TransactionDO trans, List<LineItemDO> origLineItems );

		[OperationContract]
		TransactionDO Calculate ( SecurityClass security, TransactionDO trans );
	}
}
