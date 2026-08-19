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
	public interface IEmailGroups
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, EmailGroupClass emailGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, EmailGroupClass emailGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid emailGroupGuid);

		[OperationContract]
		EmailGroupClass Get(SecurityClass security, Guid emailGroupGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);

		[OperationContract]
		EmailGroupCollectionClass Enumerate ( SecurityClass security );

        [OperationContract]
        List<EmailGroupClass> EnumerateWithEmailCatAndPriorityInfo(SecurityClass security);

        [OperationContract]
		EmailGroupCollectionClass EnumerateByAlarmPriority ( SecurityClass security, Guid alarmPriorityGuid );
	}
}
