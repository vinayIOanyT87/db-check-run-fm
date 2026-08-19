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
	public interface IGeneralConfigProcessor
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Save ( GeneralConfigSR sr );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(GeneralConfigSR sr);


		[OperationContract]
		GeneralConfigDO Get ( GeneralConfigSR sr );

		[OperationContract]
		string GetAssemblyFileVersion();
	}
}
