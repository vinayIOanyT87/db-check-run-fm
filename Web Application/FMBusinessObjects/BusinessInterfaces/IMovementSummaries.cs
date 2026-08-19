namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IMovementSummaries
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, MovementSummary movementSummary);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid movementSummaryGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, MovementSummary movementSummary, out byte[] rowVersion);

		[OperationContract]
		MovementSummary Get(SecurityClass security, Guid movementSummaryGuid, Guid userGuid, Guid siteGuid);

		[OperationContract]
		Guid? GetDuplicate(SecurityClass security, string id, int movementSummaryType, Guid ownerUserGuid, Guid siteGuid);

		[OperationContract]
		MovementSummaryCollection EnumerateByUserSite(SecurityClass security, Guid userGuid, Guid siteGuid);

		[OperationContract]
		void GetMovementSummaryIfNewer(SecurityClass security, Guid movementSummaryGuid, byte[] prevRowVersion, out MovementSummary movementSummary);
	}
}
