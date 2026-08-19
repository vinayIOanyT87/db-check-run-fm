namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public enum TankStatuses
	{
		Stopped = 0,
		Filling = 1,
		Emptying = 2,
		Running = 3,
		Testing = 4
	}
}
