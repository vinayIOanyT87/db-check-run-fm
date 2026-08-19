namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;

	[Serializable]
	public enum TransferStatuses
	{
		Inactive = 0,
		TransferTarget = 1,
		InProgress = 2,
		Complete = 3
	}
}
