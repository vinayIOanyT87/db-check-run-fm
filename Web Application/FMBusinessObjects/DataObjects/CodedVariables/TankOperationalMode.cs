namespace FMBusinessObjects.DataObjects.CodedVariables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	[Serializable]
	public enum TankOperationalMode
	{
		Normal = 0,
		Quarantined = 1,
		Market = 2
	}
}
