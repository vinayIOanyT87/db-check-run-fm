namespace FMBusinessObjects.DataObjects
{
	using System.Runtime.Serialization;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class EnterpriseVisibilityData
	{
		[DataMember]
		public EngineeringUnitType EngineeringUnitsType { get; set; }

		[DataMember]
		public EngineeringUnit Units { get; set; }

		[DataMember]
		public object Value { get; set; }

		[DataMember]
		public byte DecimalPlaces { get; set; }

		[DataMember]
		public double Maximum { get; set; }

		[DataMember]
		public double Minimum { get; set; }

		EnterpriseVisibilityData()
		{
			this.EngineeringUnitsType = EngineeringUnitType.FmuNone;
			this.Units = EngineeringUnit.FmuNone;
			this.Value = null;
			this.DecimalPlaces = 0;
			this.Maximum = 0.0;
			this.Minimum = 0.0;
		}

		public EnterpriseVisibilityData(EngineeringUnitType unitType, EngineeringUnit units, object value, byte decimalPlaces, double maximum, double minimum)
		{
			this.EngineeringUnitsType = unitType;
			this.Units = units;
			this.Value = value;
			this.DecimalPlaces = decimalPlaces;
			this.Maximum = maximum;
			this.Minimum = minimum;
		}
	}
}
