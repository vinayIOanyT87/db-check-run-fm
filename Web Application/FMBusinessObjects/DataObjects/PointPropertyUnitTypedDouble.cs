
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.Attributes;
	using System.Xml.Serialization;
	using System.Xml;
	using System.Xml.Schema;

	[KnownType(typeof(double))]
	[DataContract(Namespace = "")]
	[Serializable]
	public class PointPropertyUnitTypedDouble
	{
		protected double value;

		[DataMember]
		[FMPersistedField]
		public EngineeringUnitType EngineeringUnitsType { get; set; }

		[DataMember]
		[FMPersistedField]
		[XmlIgnore]
		public double Value { get; set; }


		[XmlElement("Value")]
		public double Value15Digits
		{
			get
			{
				var digits = 0;
				if (this.Value > 999999999999999 || this.Value < -999999999999999)
					digits = 0;
				else if (this.Value > 99999999999999.9 || this.Value < -99999999999999.9)
					digits = 1;
				else if (this.Value > 9999999999999.99 || this.Value < -9999999999999.99)
					digits = 2;
				else if (this.Value > 999999999999.999 || this.Value < -999999999999.999)
					digits = 3;
				else if (this.Value > 99999999999.9999 || this.Value < -99999999999.9999)
					digits = 4;
				else if (this.Value > 9999999999.99999 || this.Value < -9999999999.99999)
					digits = 5;
				else if (this.Value > 999999999.999999 || this.Value < -999999999.999999)
					digits = 6;
				else if (this.Value > 99999999.9999999 || this.Value < -99999999.9999999)
					digits = 7;
				else if (this.Value > 9999999.99999999 || this.Value < -9999999.99999999)
					digits = 8;
				else if (this.Value > 999999.999999999 || this.Value < -999999.999999999)
					digits = 9;
				else if (this.Value > 99999.9999999999 || this.Value < -99999.9999999999)
					digits = 10;
				else if (this.Value > 9999.99999999999 || this.Value < -9999.99999999999)
					digits = 12;
				else if (this.Value > 999.999999999999 || this.Value < -999.999999999999)
					digits = 13;
				else if (this.Value > 99.9999999999999 || this.Value < -99.9999999999999)
					digits = 14;
				else if (this.Value > 9.99999999999999 || this.Value < -9.99999999999999)
					digits = 15;
				else
					digits = 16;

				string valueString = this.Value.ToString("N" + digits.ToString());

				return double.Parse(valueString);
			}

			set
			{
				this.Value = value;
			}
		}



		public PointPropertyUnitTypedDouble Clone()
		{
			var t = (PointPropertyUnitTypedDouble)this.MemberwiseClone();
			return t;
		}

		public PointPropertyUnitTypedDouble()
		{
		}

		public PointPropertyUnitTypedDouble(double value, EngineeringUnitType engineeringUnitsType)
		{
			EngineeringUnitsType = engineeringUnitsType;
			Value = value;
		}
	}
}
