namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;

	[Serializable]
	public enum MovementStatus
	{
		Inactive = 0,
		Active,
		Halted,
		Disabled,
		Stopping,
		Starting,
		Complete
	}
}
