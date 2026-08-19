namespace FMBusinessObjects.Attributes
{
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System;
	using System.Runtime.Serialization;

    // When adding FMExposedSetting you need to update [dbo].[usp_EnumerateRestrictedAccessByPointValueIdentifiers]
    // to ensure “Point Access Group” View/Modify permissions are honoured 
    [DataContract]
	[Serializable]
	public class FMExposedSetting : Attribute
	{
		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public double Maximum { get; set; }

		[DataMember]
		public double Minimum { get; set; }

		[DataMember]
		public bool Input { get; set; }

		[DataMember]
		public byte DecimalPlaces { get; set; }

		[DataMember]
		public EngineeringUnitType  EngineeringUnitsType { get; set; }

		[DataMember]
		public EngineeringUnit Units { get; set; }

		[DataMember]
		public bool ModifyDisabled { get; set; }


		public FMExposedSetting()
		{
			this.Init();
		}

		// When adding FMExposedSetting you need to update [dbo].[usp_EnumerateRestrictedAccessByPointValueIdentifiers]
		// and PointAccessGroupToExposedSettingMap.cs to ensure “Point Access Group” View/Modify permissions are honoured 
		public FMExposedSetting(string id)
		{
			this.Init();
			this.ID = id;
		}


		private void Init()
		{
			this.ID = string.Empty;
			this.Maximum = 0.0;
			this.Minimum = 0.0;
			this.Input = true;
			this.DecimalPlaces = 0;
			this.Units = EngineeringUnit.FmuNone;
			this.EngineeringUnitsType = EngineeringUnitType.FmuNodim;
			this.ModifyDisabled = false;
		}
	}
}
