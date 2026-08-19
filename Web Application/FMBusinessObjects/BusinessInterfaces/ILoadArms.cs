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
	public interface ILoadArms
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, LoadArmClass loadArm );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, LoadArmClass loadArm );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid loadArmGuid);
	}
}
