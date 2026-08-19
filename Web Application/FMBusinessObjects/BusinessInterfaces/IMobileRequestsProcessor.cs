using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IMobileRequestsProcessor
	{
		[OperationContract]
		TransactionSelectionCollectionDO GetTransactionSelection(SecurityClass security,
															string operatorID,
															bool filterByOperatorID,
															string vehicleID,
															bool filterByVehicleID,
															string gateID,
															bool filterByGateID,
															int hoursInPast,
															int hoursInFuture);

		[OperationContract]
		TransactionLineItemSelectionCollectionDO GetTransactionLineItemSelection(SecurityClass security,
													string operatorID,
													bool filterByOperatorID,
													string vehicleID,
													bool filterByVehicleID,
													string gateID,
													bool filterByGateID,
													int hoursInPast,
													int hoursInFuture);
	}
}
