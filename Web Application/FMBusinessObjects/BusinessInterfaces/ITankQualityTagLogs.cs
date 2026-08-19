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
	public interface ITankQualityTagLogs
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, TankQualityTagLogClass oTankQualityTagLog);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, TankQualityTagLogClass oTankQualityTagLog);

		[OperationContract]
		TankQualityTagLogClass GetPreviousTagNumber(SecurityClass security);

		[OperationContract]
		TankQualityTagLogClass GetByTagNumber(SecurityClass security, int tagNumber);

		[OperationContract]
		TankQualityTagLogClass Get(SecurityClass security, Guid tankQualityTagLogGuid);

		[OperationContract]
		TankQualityTagLogClass GetMostRecentByTankID(SecurityClass security, string EquipmentID);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid tankQualityTagLogGuid);
	}
}
