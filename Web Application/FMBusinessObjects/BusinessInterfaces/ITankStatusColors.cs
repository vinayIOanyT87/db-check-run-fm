using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ServiceModel;

	using DataObjects;

	[ServiceContract]

	public interface ITankStatusColors
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		TankStatusColorsCollectionClass Enumerate(SecurityClass security);
		
	}
}
