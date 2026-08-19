namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;

	[Serializable]
	public enum TransferModes
	{
		Inactive = 0,
		Level = 1,
		Batch = 2
	}

	[Serializable]
	public enum TankTransferMode
	{
		Inactive = 0,
		Level = 1,
		Batch = 2
	}

	[Serializable]
	public enum VolumeTransferMode
	{
		Inactive = 0,
		Batch = 1
	}

	[Serializable]
	public enum NodeTransferMode
	{
		Inactive = 0,
		Batch = 1
	}
}

