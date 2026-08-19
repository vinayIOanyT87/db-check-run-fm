namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;

	[Serializable]
	public enum MovementType
	{
		Transfer = 0,
		Shipment,
		Receipt,
		RunDown,
		Charge,
		WaterDrain,
		Blend,
		Circulation,
		DirectLoad
	}
}
