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
	public interface ISaveWeightedAverageCostsProcessor
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		CustomResultDO Process ( SaveWeightedAverageCostsSR sr );

		[OperationContract]
		bool ShouldWacUpdate ( TransactionDO trans, LineItemDO lineItem, TransactionDO origTrans );

		[OperationContract]
		bool ValidateWAC ( ref CustomResultDO results, WeightedAverageCostDO wac );

		[OperationContract]
		bool QualityWasNotUsable ( TransactionDO trans, LineItemDO lineItem );

		[OperationContract]
		double QuantityChangedSinceLastSave ( TransactionDO trans, LineItemDO lineItem );

		[OperationContract]
		double QuantityChangedSinceLastSaveWithOrigTrans ( TransactionDO trans, LineItemDO lineItem, TransactionDO origTrans );
	}
}
