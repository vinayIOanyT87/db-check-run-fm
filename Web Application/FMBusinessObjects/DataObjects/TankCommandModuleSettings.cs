

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using CodedVariables;
	using System.Collections.Generic;
	using Attributes;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	[DataContract(Namespace = "")]
	[Serializable()]
	public class TankCommandModuleSettings
	{

		public const short TankModeAlarm_MovementAlarm = 1;
		public const short TankModeAlarm_ReverseFlow = 2;
		public const short TankModeAlarm_NoFlow = 4;
		public const short TankModeAlarm_Testing = 8;

		[DataMember(Order = 0)]
		[FMExposedSetting("Movement Alarm Differential")]
		public PointPropertyUnitTypedDouble MovementAlarmDifferential { get; set; }


		public TankCommandModuleSettings()
		{
			MovementAlarmDifferential = new PointPropertyUnitTypedDouble(0.00, EngineeringUnitType.FmuLength);
		}
	}
}
