namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;

	[Serializable]
	public enum MovementCommand
	{
		Stop = 0,
		Initiate,
		HoldForHandgaugeData,
		ZeroFlow,
		NonZeroFlow,
		Disable
	}
}
