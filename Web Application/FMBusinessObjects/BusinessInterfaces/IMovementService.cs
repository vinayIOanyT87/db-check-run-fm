namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IMovementService
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void InitiateMovement(SecurityClass security, Guid movementGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void StopMovement(SecurityClass security, Guid movementGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void InitiateMovementNode(SecurityClass security, Guid movementGuid, Guid nodeGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void StopMovementNode(SecurityClass security, Guid nodeGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<string> CheckForActiveInterlockedMovements(SecurityClass security, Guid movementGuid);
	}
}
