namespace FMBusinessObjects.DataObjects
{
	using Attributes;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading.Tasks;

	[DataContract]
	[Serializable]
	public class BasePoint : BaseDataObject
	{
		#region Properties

		[EntityImportExportAttribute("LEVELUNIT", 100, "LEVELUNIT")]
		[DataMember]
		[FMPersistedField("LevelUnitIndex", DefaultValue = EngineeringUnit.FmlFtIn16Th)]
		public EngineeringUnit LevelUnit { get; set; }

		[EntityImportExportAttribute("TEMPERATUREUNIT", 100, "TEMPERATUREUNIT")]
		[DataMember]
		[FMPersistedField("TemperatureUnitIndex", DefaultValue = EngineeringUnit.FmtDegF)]
		public EngineeringUnit TemperatureUnit { get; set; }

		[EntityImportExportAttribute("DENSITYUNIT", 100, "DENSITYUNIT")]
		[DataMember]
		[FMPersistedField("DensityUnitIndex", DefaultValue = EngineeringUnit.FmdDegApi)]
		public EngineeringUnit DensityUnit { get; set; }

		[EntityImportExportAttribute("PRESSUREUNIT", 100, "PRESSSUREUNIT")]
		[DataMember]
		[FMPersistedField("PressureUnitIndex", DefaultValue = EngineeringUnit.FmpPsi)]
		public EngineeringUnit PressureUnit { get; set; }

		[EntityImportExportAttribute("FLOWUNIT", 100, "FLOWUNIT")]
		[DataMember]
		[FMPersistedField("FlowUnitIndex", DefaultValue = EngineeringUnit.FmvfGpm)]
		public EngineeringUnit FlowUnit { get; set; }

		[EntityImportExportAttribute("VOLUMEUNIT", 100, "VOLUMEUNIT")]
		[DataMember]
		[FMPersistedField("VolumeUnitIndex", DefaultValue = EngineeringUnit.FmvUsGal)]
		public EngineeringUnit VolumeUnit { get; set; }

		[EntityImportExportAttribute("MASSUNIT", 100, "MASSUNIT")]
		[DataMember]
		[FMPersistedField("MassUnitIndex", DefaultValue = EngineeringUnit.FmmLb)]
		public EngineeringUnit MassUnit { get; set; }

		[EntityImportExportAttribute("VELOCITYUNIT", 100, "VELOCITYUNIT")]
		[DataMember]
		[FMPersistedField("VelocityUnitIndex", DefaultValue = EngineeringUnit.FmvrFps)]
		public EngineeringUnit VelocityUnit { get; set; }

		[EntityImportExportAttribute("MASSFLOWUNIT", 100, "MASSFLOWUNIT")]
		[DataMember]
		[FMPersistedField("MassFlowUnitIndex", DefaultValue = EngineeringUnit.FmmfLbMin)]
		public EngineeringUnit MassFlowUnit { get; set; }

		[EntityImportExportAttribute("LEVELPRECISION", 100, "LEVELDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte LevelDecimalPlaces { get; set; }

		[EntityImportExportAttribute("TEMPERATUREPRECISION", 100, "TEMPERATUREDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte TemperatureDecimalPlaces { get; set; }

		[EntityImportExportAttribute("DENSITYPRECISION", 100, "DENSITYDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte DensityDecimalPlaces { get; set; }

		[EntityImportExportAttribute("PRESSUREPRECISION", 100, "PRESSUREDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte PressureDecimalPlaces { get; set; }

		[EntityImportExportAttribute("FLOWPRECISION", 100, "FLOWDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte FlowDecimalPlaces { get; set; }

		[EntityImportExportAttribute("VOLUMEPRECISION", 100, "VOLUMEDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte VolumeDecimalPlaces { get; set; }

		[EntityImportExportAttribute("MASSPRECISION", 100, "MASSDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte MassDecimalPlaces { get; set; }

		[EntityImportExportAttribute("VELOCITYPRECISION", 100, "VELOCITYDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte VelocityDecimalPlaces { get; set; }

		[EntityImportExportAttribute("MASSFLOWPRECISION", 100, "MASSFLOWDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte MassFlowDecimalPlaces { get; set; }


		[DataMember]
		[FMPersistedField]
		public double LevelMaximum { get; set; }

		[EntityImportExportAttribute("LEVELMAXIMUM", 100, "LEVELMAXIMUM")]
		public string LevelMaximumForImportExport {
			get
			{
				// negative values in FmlFtIn16Th or FmlFtIn8Th are formatted incorrectly ( 0-00-00-00 instead of -00-00-00)
				// we need to manually correct it
				if ((this.LevelUnit == EngineeringUnit.FmlFtIn16Th || this.LevelUnit == EngineeringUnit.FmlFtIn8Th)
							&& this.LevelMaximum < 0)
				{
					this.LevelMaximum *= -1;
					var newNegativeValue = EngineeringUnitsHelperClass.FormatValue(this.LevelMaximum, this.LevelUnit);
					return "-" + newNegativeValue.ToString();
				}
				var newValue= EngineeringUnitsHelperClass.FormatValue(this.LevelMaximum,this.LevelUnit);
				return newValue.ToString();
			}
			set
			{

				if (this.LevelUnit == EngineeringUnit.FmlFtIn16Th || this.LevelUnit == EngineeringUnit.FmlFtIn8Th)
				{
					var isNegative = false;
					if (value.StartsWith("-"))
					{
						value = value.Remove(0, 1);
						isNegative = true;
					}
					this.LevelMaximum =
						(double)EngineeringUnitsHelperClass.ParseValue(typeof(double), value, this.LevelUnit, new NumberFormatInfo());
					if (isNegative)
					{
						this.LevelMaximum *= -1;
					}
				}
				else
				{

					this.LevelMaximum = double.Parse(value);
				}
			}
		}


		[DataMember]
		[FMPersistedField]
		public double LevelMinimum { get; set; }

		[EntityImportExportAttribute("LEVELMINIMUM", 100, "LEVELMINIMUM")]
		public string LevelMinimumForImportExport { 
			get
			{
				// negative values in FmlFtIn16Th or FmlFtIn8Th are formatted incorrectly ( 0-00-00-00 instead of -00-00-00)
				// we need to manually correct it
				if ((this.LevelUnit == EngineeringUnit.FmlFtIn16Th || this.LevelUnit == EngineeringUnit.FmlFtIn8Th) 
							&& this.LevelMinimum < 0 ) {
					this.LevelMinimum *= -1;
					var newNegativeValue = EngineeringUnitsHelperClass.FormatValue(this.LevelMinimum, this.LevelUnit);
					return "-" + newNegativeValue.ToString();
				}
				var newValue = EngineeringUnitsHelperClass.FormatValue(this.LevelMinimum, this.LevelUnit);
				return newValue.ToString();
			}
			set
			{

				if (this.LevelUnit == EngineeringUnit.FmlFtIn16Th || this.LevelUnit == EngineeringUnit.FmlFtIn8Th)
				{
					var isNegative = false;
					if ( value.StartsWith( "-") ) {
						value = value.Remove(0, 1);
						isNegative = true;
					}
					this.LevelMinimum =
						(double)EngineeringUnitsHelperClass.ParseValue(typeof(double), value, this.LevelUnit, new NumberFormatInfo());
					if(isNegative) {
						this.LevelMinimum *= -1;
					}
				}
				else
				{
					this.LevelMinimum = double.Parse(value);
				}
			}
}

[EntityImportExportAttribute("TEMPERATUREMAXIMUM", 100, "TEMPERATUREMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public double TemperatureMaximum { get; set; }

		[EntityImportExportAttribute("TEMPERATUREMINIMUM", 100, "TEMPERATUREMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double TemperatureMinimum { get; set; }

		[EntityImportExportAttribute("DENSITYMAXIMUM", 100, "DENSITYMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public double DensityMaximum { get; set; }

		[EntityImportExportAttribute("DENSITYMINIMUM", 100, "DENSITYMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double DensityMinimum { get; set; }

		[EntityImportExportAttribute("PRESSSUREMAXIMUM", 100, "PRESSSUREMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public double PressureMaximum { get; set; }

		[EntityImportExportAttribute("PRESSSUREMINIMUM", 100, "PRESSSUREMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double PressureMinimum { get; set; }

		[EntityImportExportAttribute("FLOWMAXIMUM", 100, "VOLUMETRICFLOWMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public double VolumetricFlowMaximum { get; set; }

		[EntityImportExportAttribute("FLOWMINIMUM", 100, "VOLUMETRICFLOWMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double VolumetricFlowMinimum { get; set; }

		[EntityImportExportAttribute("VOLUMEMAXIMUM", 100, "VOLUMEMAXIMUM")]

		[DataMember]
		[FMPersistedField]
		public double VolumeMaximum { get; set; }

		[EntityImportExportAttribute("VOLUMEMINIMUM", 100, "VOLUMEMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double VolumeMinimum { get; set; }

		[EntityImportExportAttribute("MASSMAXIMUM", 100, "MASSMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public double MassMaximum { get; set; }

		[EntityImportExportAttribute("MASSMINIMUM", 100, "MASSMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double MassMinimum { get; set; }

		[EntityImportExportAttribute("VELOCITYMAXIMUM", 100, "VELOCITYMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public double VelocityMaximum { get; set; }

		[EntityImportExportAttribute("VELOCITYMINIMUM", 100, "VELOCITYMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double VelocityMinimum { get; set; }

		[EntityImportExportAttribute("MASSFLOWMAXIMUM", 100, "MASSFLOWMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public double MassFlowMaximum { get; set; }

		[EntityImportExportAttribute("MASSFLOWMINIMUM", 100, "MASSFLOWMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public double MassFlowMinimum { get; set; }

		#endregion


		#region Publc methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		public override void Reset()
		{
			this.LevelUnit = EngineeringUnit.FmlFtIn16Th;
			this.TemperatureUnit = EngineeringUnit.FmtDegF;
			this.DensityUnit = EngineeringUnit.FmdDegApi;
			this.PressureUnit = EngineeringUnit.FmpPsi;
			this.FlowUnit = EngineeringUnit.FmvfGpm;
			this.VolumeUnit = EngineeringUnit.FmvUsGal;
			this.MassUnit = EngineeringUnit.FmmLb;
			this.VelocityUnit = EngineeringUnit.FmvrFps;
			this.MassFlowUnit = EngineeringUnit.FmmfLbMin;
			this.LevelDecimalPlaces = 0;
			this.TemperatureDecimalPlaces = 0;
			this.DensityDecimalPlaces = 0;
			this.PressureDecimalPlaces = 0;
			this.FlowDecimalPlaces = 0;
			this.VolumeDecimalPlaces = 0;
			this.MassDecimalPlaces = 0;
			this.VelocityDecimalPlaces = 0;
			this.MassFlowDecimalPlaces = 0;
			this.LevelMaximum = 0;
			this.LevelMinimum = 0;
			this.TemperatureMaximum = 0;
			this.TemperatureMinimum = 0;
			this.DensityMaximum = 0;
			this.DensityMinimum = 0;
			this.PressureMaximum = 0;
			this.PressureMinimum = 0;
			this.VolumetricFlowMaximum = 0;
			this.VolumetricFlowMinimum = 0;
			this.VolumeMaximum = 0;
			this.VolumeMinimum = 0;
			this.MassMaximum = 0;
			this.MassMinimum = 0;
			this.VelocityMaximum = 0;
			this.VelocityMinimum = 0;
			this.MassFlowMaximum = 0;
			this.MassFlowMinimum = 0;

			base.Reset();
		}


		public int GetDecimalPlaces(EngineeringUnitType engUnitType)
		{
			switch (engUnitType)
			{
				case EngineeringUnitType.FmuLength:
					return this.LevelDecimalPlaces;
				case EngineeringUnitType.FmuMassflow:
					return this.MassFlowDecimalPlaces;
				case EngineeringUnitType.FmuVelocity:
					return this.VelocityDecimalPlaces;
				case EngineeringUnitType.FmuMass:
					return this.MassDecimalPlaces;
				case EngineeringUnitType.FmuVolume:
					return this.VolumeDecimalPlaces;
				case EngineeringUnitType.FmuVolflow:
					return this.FlowDecimalPlaces;
				case EngineeringUnitType.FmuPressure:
					return this.PressureDecimalPlaces;
				case EngineeringUnitType.FmuDensity:
					return this.DensityDecimalPlaces;
				case EngineeringUnitType.FmuTemp:
					return this.TemperatureDecimalPlaces;
			}
			return -1;
		}

		public double GetMaximum(EngineeringUnitType engUnitType)
		{
			switch (engUnitType)
			{
				case EngineeringUnitType.FmuLength:
					return this.LevelMaximum;
				case EngineeringUnitType.FmuMassflow:
					return this.MassFlowMaximum;
				case EngineeringUnitType.FmuVelocity:
					return this.VelocityMaximum;
				case EngineeringUnitType.FmuMass:
					return this.MassMaximum;
				case EngineeringUnitType.FmuVolume:
					return this.VolumeMaximum;
				case EngineeringUnitType.FmuVolflow:
					return this.VolumetricFlowMaximum;
				case EngineeringUnitType.FmuPressure:
					return this.PressureMaximum;
				case EngineeringUnitType.FmuDensity:
					return this.DensityMaximum;
				case EngineeringUnitType.FmuTemp:
					return this.TemperatureMaximum;
			}
			return 0;
		}

		public double GetMinimum(EngineeringUnitType engUnitType)
		{
			switch (engUnitType)
			{
				case EngineeringUnitType.FmuLength:
					return this.LevelMinimum;
				case EngineeringUnitType.FmuMassflow:
					return this.MassFlowMinimum;
				case EngineeringUnitType.FmuVelocity:
					return this.VelocityMinimum;
				case EngineeringUnitType.FmuMass:
					return this.MassMinimum;
				case EngineeringUnitType.FmuVolume:
					return this.VolumeMinimum;
				case EngineeringUnitType.FmuVolflow:
					return this.VolumetricFlowMinimum;
				case EngineeringUnitType.FmuPressure:
					return this.PressureMinimum;
				case EngineeringUnitType.FmuDensity:
					return this.DensityMinimum;
				case EngineeringUnitType.FmuTemp:
					return this.TemperatureMinimum;
			}
			return 0;
		}

		public EngineeringUnit GetEngineeringUnits(EngineeringUnitType engUnitType)
		{
			switch (engUnitType)
			{
				case EngineeringUnitType.FmuLength:
					return this.LevelUnit;
				case EngineeringUnitType.FmuMassflow:
					return this.MassUnit;
				case EngineeringUnitType.FmuVelocity:
					return this.VelocityUnit;
				case EngineeringUnitType.FmuMass:
					return this.MassUnit;
				case EngineeringUnitType.FmuVolume:
					return this.VolumeUnit;
				case EngineeringUnitType.FmuVolflow:
					return this.FlowUnit;
				case EngineeringUnitType.FmuPressure:
					return this.PressureUnit;
				case EngineeringUnitType.FmuDensity:
					return this.DensityUnit;
				case EngineeringUnitType.FmuTemp:
					return this.TemperatureUnit;
			}
			return EngineeringUnit.FmuNone;
		}



		#endregion

	}
}
