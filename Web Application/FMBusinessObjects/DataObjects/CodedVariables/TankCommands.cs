namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public enum TankCommands
	{
		Stop = 0,
		Fill,
		Empty,
		Run,
		Test,
		Reset
	}
}
