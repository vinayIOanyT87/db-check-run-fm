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
	public interface IMessageLogs
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, MessageLogClass MessageLog);

		[OperationContract]
		MessageLogClass Get(SecurityClass security, Guid messageGuid, Guid companyGuid, Guid personnelGuid);

		[OperationContract]
		MessageLogClass GetToday(SecurityClass security, Guid messageGuid, Guid companyGuid, Guid personnelGuid);
	}
}
