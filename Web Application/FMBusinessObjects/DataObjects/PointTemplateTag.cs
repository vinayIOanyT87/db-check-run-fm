namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.Attributes;
	using FMBusinessObjects.Constants;
	using System.Xml.Serialization;
	using System.IO;
	using System.Linq;
	using System.Xml;

	using FMBusinessObjects.UtilityObjects;

	[KnownType(typeof(string))]
	[KnownType(typeof(double))]
	[KnownType(typeof(bool))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(PointCommandStatusListReference))]
	[KnownType(typeof(DeviceAlarmMapReference))]
	[KnownType(typeof(CodedVariables.TankStatuses))]
	[KnownType(typeof(CodedVariables.TankCommands))]
	[KnownType(typeof(CodedVariables.TransferModes))]
	[KnownType(typeof(CodedVariables.TankTransferMode))]
	[KnownType(typeof(CodedVariables.VolumeTransferMode))]
	[KnownType(typeof(CodedVariables.TransferStatuses))]
	[KnownType(typeof(CodedVariables.TankOperationalMode))]
	[KnownType(typeof(CodedVariables.MovementCommand))]
	[KnownType(typeof(CodedVariables.MovementStatus))]
	[KnownType(typeof(CodedVariables.StrapTableSelect))]
	[KnownType(typeof(CodedVariables.Reset))]
	[KnownType(typeof(CodedVariables.NodeTransferMode))]
	[KnownType(typeof(CodedVariables.NodeTransferStatus))]


	[DataContract]
	[Serializable]
	public class PointTemplateTag : BaseSerializedDataObject, ICloneable
	{
		[EntityImportExportAttribute("TAGID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9 ]*$")) { base.ID = value; } else throw new Exception("Tag ID must be Alphanumeric");}}

		public enum PointTagInputOutputType
		{
			UnAssigned = 0,
			Manual = 1,
			Calculated = 2,
			OpcUa = 3,
			FCEE = 4,
			System = 5
		}

		public enum OutputPointTagChangeAgent
		{
			Manual = 1,
			Calculated = 2
		}

		[EntityImportExportAttribute("POINTTEMPLATEGUID", 200, "PointTemplateGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid PointTemplateGuid { get; set; }

		[DataMember]
		public new Guid SiteGuid
		{
			get
			{
				return base.SiteGuid;
			}
			set
			{
				base.SiteGuid = value;
			}
		}

		[EntityImportExportAttribute("PTTAGGUID", 200, "PointTemplateTagGuid")]
		[FMPersistedField]
		public Guid PointTemplateTagGuid
		{
			get
			{
				return this.IdentityGuid;
			}

			set
			{
				this.IdentityGuid = value;
			}
		}

		[EntityImportExportWorksheet("ALARMTEMPLATES", "ALARMTEMPLATEID*")]
		[DataMember]
		public Dictionary<Guid, AlarmTemplate> AlarmTemplates = new Dictionary<Guid, AlarmTemplate>();

		[EntityImportExport("UNITSTYPE", 110, "ENGINEERINGUNITSTYPE")]
		[DataMember]
		[FMPersistedField]
		public EngineeringUnitType EngineeringUnitsType { get; set; }

		[EntityImportExport("UNITS", 110, "UNITS")]
		[DataMember]
		[FMPersistedField("EngineeringUnitsIndex")]
		public EngineeringUnit Units { get; set; }

		[DataMember]
		[FMPersistedField("ServerEngineeringUnitsIndex")]
		public EngineeringUnit ServerUnits { get; set; }

		[EntityImportExport("PRECISION", 110, "DECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte DecimalPlaces { get; set; }

		[DataMember]
		[FMPersistedField]
		public double Maximum { get; set; }

		[DataMember]
		[FMPersistedField]
		public double Minimum { get; set; }

		[EntityImportExport("MINIMUM", 100, "MINIMUM")]
		public string MinimumForImportExport
		{
			get
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					// negative values in FmlFtIn16Th or FmlFtIn8Th are formatted incorrectly ( 0-00-00-00 instead of -00-00-00)
					// we need to manually correct it
					if (this.Minimum < 0)
					{
						this.Minimum *= -1;
						var newNegativeValue = EngineeringUnitsHelperClass.FormatValue(this.Minimum, this.Units);
						return "-" + newNegativeValue.ToString();
					}
					var newValue = EngineeringUnitsHelperClass.FormatValue(this.Minimum, this.Units);
					return newValue.ToString();
				}
				else return this.Minimum.ToString();
			}
			set
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					var isNegative = false;
					if (value.StartsWith("-"))
					{
						value = value.Remove(0, 1);
						isNegative = true;
					}
					this.Minimum = (double)EngineeringUnitsHelperClass.ParseValue(typeof(double), value, this.Units, new NumberFormatInfo());
					if (isNegative)
					{
						this.Minimum *= -1;
					}
				}
				else
				{
					this.Minimum = double.Parse(value);
				}
			}
		}

		[EntityImportExportAttribute("MAXIMUM", 100, "MAXIMUM")]
		public string MaximumForImportExport
		{
			get
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					// negative values in FmlFtIn16Th or FmlFtIn8Th are formatted incorrectly ( 0-00-00-00 instead of -00-00-00)
					// we need to manually correct it
					if (this.Maximum < 0)
					{
						this.Maximum *= -1;
						var newNegativeValue = EngineeringUnitsHelperClass.FormatValue(this.Maximum, this.Units);
						return "-" + newNegativeValue.ToString();
					}
					var newValue = EngineeringUnitsHelperClass.FormatValue(this.Maximum, this.Units);
					return newValue.ToString();
				}
				else return this.Maximum.ToString();
			}
			set
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					var isNegative = false;
					if (value.StartsWith("-"))
					{
						value = value.Remove(0, 1);
						isNegative = true;
					}
					this.Maximum = (double)EngineeringUnitsHelperClass.ParseValue(typeof(double), value, this.Units, new NumberFormatInfo());
					if (isNegative)
					{
						this.Maximum *= -1;
					}
				}
				else
				{
					this.Maximum = double.Parse(value);
				}
			}
		}


		[EntityImportExport("INPUTOUPUTTYPE", 110, "INPUTOUTPUTTYPE")]
		[DataMember]
		[FMPersistedField("PointTagInputOutputTypeIndex")]
		public PointTagInputOutputType InputOutputType { get; set; }

		[EntityImportExport("INPUT", 110, "INPUT")]
		[DataMember]
		[FMPersistedField]
		public bool Input { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool AlarmStatus { get; set; }

		[EntityImportExport("APPLYUNITS", 110, "APPLYPTENGINEERINGUNITS")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointTemplateEngineeringUnits { get; set; }

		[EntityImportExport("APPLYPRECISION", 110, "APPLYPTDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointTemplateDecimalPlaces { get; set; }

		[EntityImportExport("APPLYMAXIMUM", 110, "APPLYPTMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointTemplateMaximum { get; set; }

		[EntityImportExport("APPLYMINIMUM", 110, "APPLYPTMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointTemplateMinimum { get; set; }

		[EntityImportExport("WELLKNOWNIDENTITYGUID", 100, "WELLKNOWNIDENTITYGUID")]
		[DataMember]
		[FMPersistedField]
		public Guid WellKnownIdentityGuid { get; set; }

		[EntityImportExport("ALARMSENABLED", 100, "ALARMSENABLED")]
		[DataMember]
		[FMPersistedField]
		public bool AlarmsEnabled { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool InhibitInputOutputTypeConfiguration { get; set; }

		// Need to flip excel value to match UI without changing functionality
		[EntityImportExport("CONFIGSOURCE", 100, "InhibitInputOutputTypeConfiguration")]
		public bool ConfigSource
		{          
			get
			{
				return !InhibitInputOutputTypeConfiguration;
			}
			set { InhibitInputOutputTypeConfiguration = !value;  
			} 
		}

		[DataMember]
		[FMPersistedField]
		public bool InhibitOverride { get; set; }

		// Need to flip excel value to match UI without changing functionality
		[EntityImportExport("OVERRIDE", 100, "InhibitOverride")]
		public bool Override {
			get
			{
				return !InhibitOverride;
			}
			set
			{
				InhibitOverride = !value;
			}
		}

		[EntityImportExport("MODULE", 100, "Module")]
		[DataMember]
		[FMPersistedField]
		public bool Module { get; set; }

		[EntityImportExport("ARCHIVED", 100, "Archived")]
		[DataMember]
		[FMPersistedField]
		public bool Archived { get; set; }



		public object Clone()
		{
			var t = (PointTemplateTag)this.MemberwiseClone();
			this.BaseClone(t);
			return t;
		}

		public void EnumerateByPointTemplateSQL(SqlCommand cmd, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SELECT * FROM tblPointTemplateTag WHERE PointTemplateGuid = @PointTemplateGuid ORDER BY ID";
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		public static void SelectSql(SqlCommand cmd, Guid tagGuid)
		{
			cmd.CommandText = "SELECT * FROM tblPointTemplateTag"
									+ " WHERE PointTemplateTagGuid = @PointTemplateTagGuid";

			cmd.Parameters.AddWithValue("@PointTemplateTagGuid", tagGuid);
		}


		static public List<KeyValuePair<string, string>> EnumerateTagDataTypes()
		{
			var dataTypes = new List<KeyValuePair<string, string>>
							{
								new KeyValuePair<string, string>("System.Boolean", "DataType|Boolean"),
								new KeyValuePair<string, string>("System.Double", "DataType|Double64"),
								new KeyValuePair<string, string>("System.Single", "DataType|Float32"),
								new KeyValuePair<string, string>("System.Int16", "DataType|Short16"),
								new KeyValuePair<string, string>("System.UInt16", "DataType|UShort16"),
								new KeyValuePair<string, string>("System.Int32",  "DataType|Integer32"),
								new KeyValuePair<string, string>("System.UInt32", "DataType|UInteger32"),

// Note : Commented out by WCG 2/6/2018 due to problem with Minimum and Maximum being double and not able to represent Int64.MinValue, Int64.MaxValue, UInt64.MinValue and UInt64.MaxValue
//			 Stakeholders decided to not support these types at this time.
//								new KeyValuePair<string, string>("System.Int64",  "DataType|Long64"),
//								new KeyValuePair<string, string>("System.UInt64", "DataType|ULong64"),
								new KeyValuePair<string, string>("System.String", "DataType|String"),
								new KeyValuePair<string, string>("System.DateTimeOffset", "DataType|Date/Time"),
								new KeyValuePair<string, string>("System.DateTime", "DataType|Date"),
								new KeyValuePair<string, string>("System.TimeSpan", "DataType|Duration"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.TankCommands",  "DataType|Tank Command"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode", "DataType|Tank Transfer Mode"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode", "DataType|Volume Transfer Mode"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode", "DataType|Node Transfer Mode"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.TankStatuses",  "DataType|Tank Status"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses", "DataType|Transfer Status"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.NodeTransferStatus", "DataType|Node Transfer Status"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.PointCommandStatusListReference", "DataType|Point Command-Status"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.DeviceAlarmMapReference", "DataType|Device Alarm Map"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode", "DataType|Tank Operational Mode"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.MovementCommand", "DataType|Movement Command"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.MovementStatus", "DataType|Movement Status"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect", "DataType|Strap Table Select"),
								new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.CodedVariables.Reset", "DataType|Reset")
                        };

			return dataTypes;
		}

		static public bool IsNumeric(string dataType)
		{
			return (dataType == "System.Double"
			|| dataType == "System.Single"
			|| dataType == "System.Int16"
			|| dataType == "System.Int32"
			|| dataType == "System.Int64"
			|| dataType == "System.UInt16"
			|| dataType == "System.UInt32"
			|| dataType == "System.UInt64") ? true : false;
		}

		static public List<KeyValuePair<Guid, string>> EnumerateWellKnownTags()
		{
			var wellKnownTags = new List<KeyValuePair<Guid, string>>();

			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guid.Empty, ""));

			// Add to this list any well know Guid used in the standard templates so if standard template is copied they are also copied
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.DensityProductInAirGuid, "WellKnownTag|Density Product in Air"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.DensityProductHighGuid, "WellKnownTag|Density Product Observed High Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.DensityProductLowGuid, "WellKnownTag|Density Product Observed Low Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.DensityProductObservedGuid, "WellKnownTag|Density Product Observed"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.DensityProductStandardGuid, "WellKnownTag|Density Product Standard"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.DensityProductStandardInAirGuid, "WellKnownTag|Density Product Standard in Air"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.InitiateIdentifierGuid, "WellKnownTag|Initiate Identifier"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.InitiationCountGuid, "WellKnownTag|Initiation Count"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.LeakDetectionLastRunGuid, "WellKnownTag|Leak Detection Data Last Run Time"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.LeakRateGuid, "WellKnownTag|Leak Rate"));
         wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.LevelProductGuid, "WellKnownTag|Level Product"));
         wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.LevelProductMaxOpLimitWellKnownGuid, "WellKnownTag|Level Product Max Op Limit"));
         wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.LevelProductMinOpLimitWellKnownGuid, "WellKnownTag|Level Product Min Op Limit"));
         wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.LevelWaterGuid, "WellKnownTag|Level Water"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.MassLiquidGuid, "WellKnownTag|Mass Liquid"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.MovementCommandGuid, "WellKnownTag|Movement Command"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.MovementControlIdentifierWellKnownGuid, "WellKnownTag|Movement Control Identifier"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.MovementDeviationGuid, "WellKnownTag|,Mvement Deviation"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.MovementPercentDeviationGuid, "WellKnownTag|Movement Percent Deviation"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.MovementHistoryWrittenTimeGuid, "WellKnownTag|Movement History Written"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.MovementStatusGuid, "WellKnownTag|Movement Status"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.OperationalModeGuid, "WellKnownTag|Operational Mode"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PercentBSWGuid, "WellKnownTag|Percent BSW"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PressureBottomGuid, "WellKnownTag|Pressure Bottom"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PressureVaporGuid, "WellKnownTag|Pressure Vapor"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PulseMeterNumberOfRollOversWellKnownGuid, "WellKnownTag|PulseMeterNumberOfRollOversWellKnownGuid"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PulseMeterLastValueWellKnownGuid, "WellKnownTag|PulseMeterLastValueWellKnownGuid"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PulseMeterCurrentValueWellKnownGuid, "WellKnownTag|PulseMeterCurrentValueWellKnownGuid"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PulseMeterVolumePerPulseWellKnownGuid, "WellKnownTag|PulseMeterVolumePerPulseWellKnownGuid"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PulseMeterRollOverAmountWellKnownGuid, "WellKnownTag|PulseMeterRollOverAmountWellKnownGuid"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.PulseMeterLastReadWasRollOverWellKnownGuid, "WellKnownTag|PulseMeterLastReadWasRollOverWellKnownGuid"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.StopIdentifierGuid, "WellKnownTag|Stop Identifier"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TankShellCorrectionGuid, "WellKnownTag|Tank Shell Correction"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TankStatusGuid, "WellKnownTag|Tank Status"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TemperatureAmbientGuid, "WellKnownTag|Temperature Ambient"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TemperatureDensityGuid, "WellKnownTag|Temperature Density"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TemperatureProductGuid, "WellKnownTag|Temperature Product"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TemperatureProductHighGuid, "WellKnownTag|Temperature Product High Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TemperatureProductHiHiGuid, "WellKnownTag|Temperature Product HiHi Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TemperatureProductLoLoGuid, "WellKnownTag|Temperature Product LoLo Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TemperatureProductLowGuid, "WellKnownTag|Temperature Product Low Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferLevelTargetGuid, "WellKnownTag|Transfer Level Target"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferModeGuid, "WellKnownTag|Transfer Mode"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferredGOVGuid, "WellKnownTag|Transferred GOV"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferredNSVGuid, "WellKnownTag|Transferred NSV"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferredVolumeGuid, "WellKnownTag|Transferred Volume"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferredWaterVolumeGuid, "WellKnownTag|Transferred Volume Water"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStartGOVGuid, "WellKnownTag|Transfer Start GOV"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStartLevelGuid, "WellKnownTag|Transfer Start Level"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStartNSVGuid, "WellKnownTag|Transfer Start NSV"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStartTimeGuid, "WellKnownTag|Transfer Start Time"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStartVolumeGuid, "WellKnownTag|Transfer Start Volume"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferVolumeTargetGuid, "WellKnownTag|Transfer Volume Target"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStartWaterVolumeGuid, "WellKnownTag|Transfer Start Water Volume"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStatusGuid, "WellKnownTag|Transfer Status"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferStopTimeGuid, "WellKnownTag|Transfer Stop Time"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferTargetGuid, "WellKnownTag|Transfer Target"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferTimeCompletionGuid, "WellKnownTag|Transfer Time Completion"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TransferTimeRemainingGuid, "WellKnownTag|Transfer Time Remaining"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData01WellKnownGuid, "WellKnownTag|User Data 01"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData02WellKnownGuid, "WellKnownTag|User Data 02"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData03WellKnownGuid, "WellKnownTag|User Data 03"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData04WellKnownGuid, "WellKnownTag|User Data 04"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData05WellKnownGuid, "WellKnownTag|User Data 05"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData06WellKnownGuid, "WellKnownTag|User Data 06"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData07WellKnownGuid, "WellKnownTag|User Data 07"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData08WellKnownGuid, "WellKnownTag|User Data 08"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData09WellKnownGuid, "WellKnownTag|User Data 09"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.UserData10WellKnownGuid, "WellKnownTag|User Data 10"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeCorrectionFactorGuid, "WellKnownTag|Volume Correction Factor"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeCorrectionFactorUnroundedGuid, "WellKnownTag|Volume Correction Factor Unrounded"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedAvailableGuid, "WellKnownTag|Volume Gross Observed Available"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedGuid, "WellKnownTag|Volume Gross Observed"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedRateGuid, "WellKnownTag|Volume Gross Observed Rate"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedRemainingGuid, "WellKnownTag|Volume Gross Observed Remaining"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedHiHiWellKnownGuid, "WellKnownTag|Volume Gross Observed HiHi Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedHighWellKnownGuid, "WellKnownTag|Volume Gross Observer High Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedLowWellKnownGuid, "WellKnownTag|Volume Gross Observed Low Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossObservedLoLoWellKnownGuid, "WellKnownTag|Volume Gross Observed LoLo Limit"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeGrossStandardGuid, "WellKnownTag|Volume Gross Standard"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeNetStandardAvailableGuid, "WellKnownTag|Volume Net Standard Available"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeNetStandardGuid, "WellKnownTag|Volume Net Standard"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeNetStandardRateGuid, "WellKnownTag|Volume Net Standard Rate"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeNetStandardRemainingGuid, "WellKnownTag|Volume Net Standard Remaining"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeNetStandardUnroundedGuid, "WellKnownTag|Volume Net Standard Unrounded"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeRoofCorrectionGuid, "WellKnownTag|Volume Roof Correction"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeTotalObservedGuid, "WellKnownTag|Volume Total Observed"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeTotalObservedRateGuid, "WellKnownTag|Volume Total Observed Rate"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeWaterGuid, "WellKnownTag|Volume Water"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeBSWGuid, "WellKnownTag|Volume BSW"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeTotalizerGrossGuid, "WellKnownTag|Totalizer Gross"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.VolumeTotalizerNetGuid, "WellKnownTag|Totalizer Net"));

			// TDU Template Well Known Guids
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduNotepadWellKnownGuid, "WellKnownTag|Notepad"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrainCommWellKnownGuid, "WellKnownTag|TrainComm"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrCurrIndexWellKnownGuid, "WellKnownTag|TrCurrIndex"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrDirectionWellKnownGuid, "WellKnownTag|TrDirection"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrEndVolWellKnownGuid, "WellKnownTag|TrEndVol"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrInitialVolWellKnownGuid, "WellKnownTag|TrInitialVol"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrNextVolWellKnownGuid, "WellKnownTag|TrNextVol"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrRelaxVolWellKnownGuid, "WellKnownTag|TrRelaxVol"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrStatusWellKnownGuid, "WellKnownTag|TrStatus"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrTempWellKnownGuid, "WellKnownTag|TrTemp"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrTkNumberWellKnownGuid, "WellKnownTag|TrTkNumber"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrVolumeWellKnownGuid, "WellKnownTag|TrVolume"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduSearchHartCommandWellKnownGuid, "WellKnownTag|SearchHartCommand"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduSearchHartTkNumWellKnownGuid, "WellKnownTag|SearchHartTkNum"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTduFirmwareWellKnownGuid, "WellKnownTag|TDUFirmware"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTduStatusWellKnownGuid, "WellKnownTag|TDUStatus"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTduTempWellKnownGuid, "WellKnownTag|TDUTemp"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTduVoltageWellKnownGuid, "WellKnownTag|TduVoltage"));
			wellKnownTags.Add(new KeyValuePair<Guid, string>(Guids.TduTrPressureWellKnownGuid, "WellKnownTag|TrPressure"));

			return wellKnownTags;
		}

		public int GetDecimalPlaces(PointTemplate p)
		{
			if (this.ApplyPointTemplateDecimalPlaces)
			{
				int ret = p.GetDecimalPlaces(this.EngineeringUnitsType);
				return (ret >= 0) ? ret : this.DecimalPlaces;
			}
			return this.DecimalPlaces;
		}

		public double GetMaximum(PointTemplate p)
		{
			if (this.ApplyPointTemplateMaximum)
			{
				return p.GetMaximum(this.EngineeringUnitsType);
			}
			return this.Maximum;
		}

		public double GetMinimum(PointTemplate p)
		{
			if (this.ApplyPointTemplateMinimum)
			{
				return p.GetMinimum(this.EngineeringUnitsType);
			}
			return this.Minimum;
		}

		public EngineeringUnit GetEngineeringUnits(PointTemplate p)
		{
			if (this.ApplyPointTemplateEngineeringUnits)
			{
				return p.GetEngineeringUnits(this.EngineeringUnitsType);
			}
			return this.Units;
		}

		public string FormatValue(PointTemplate p, SiteClass s)
		{
			var decimalPlaces = this.GetDecimalPlaces(p);
			var val = EngineeringUnitsHelperClass.FormatValue(this.Value, this.Units);
			string retVal = s.FormatValue(val, decimalPlaces);
			return retVal;
		}

		[EntityImportExportAttribute("VALUETYPE", 100, "ValueTypeString")]
		[DataMember]
		[FMPersistedField("ValueType")]
		new public string ValueTypeString
		{
			get
			{
				return this._valueType.ToString();
			}
			set
			{
				this._valueType = Type.GetType(value);
			}
		}

		[EntityImportExportAttribute("VALUE", 100, "ValueXml")]
		public string ValueXmlForExport
		{
			get
			{
				var retValue = "";
				object value = this._value;
				if (value == null)
				{
					retValue = null;
				}
				else
				{
					XmlSerializer xmlserializer;
					if (value.GetType() == typeof(DateTimeOffset))
					{
						xmlserializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("DateTimeOffset"));
						value = XmlConvert.ToString((DateTimeOffset)value);
					}
					else if (value.GetType() == typeof(TimeSpan))
					{
						xmlserializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("TimeSpan"));
						value = XmlConvert.ToString((TimeSpan)value);
					}

					else
					{
						xmlserializer = CachingXmlSerializerFactory.Create(value.GetType());
					}

					var stringWriter = new StringWriter();
					var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
					// explicitly remove the xml declaration
					var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
					using (var writer = XmlWriter.Create(stringWriter, settings))
					{
						xmlserializer.Serialize(writer, value, emptyNameSpaces);
						retValue = stringWriter.ToString();
					}

					if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
					{
						XmlDocument doc = new XmlDocument();
						doc.LoadXml(retValue);
						XmlNode root = doc.DocumentElement;

						var tempValue = double.Parse(root.InnerText);
						var isNegativeValue = false;
						if (tempValue < 0)
						{
							tempValue *= -1;
							isNegativeValue = true;
						}

						var newValue = EngineeringUnitsHelperClass.FormatValue(
							tempValue,
							this.Units);
						root.InnerText = (isNegativeValue ? "-" :"" ) + newValue.ToString();
						retValue = doc.OuterXml;
					}

				}

				return retValue;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this._value = null;
					return;
				}

				XmlSerializer serializer;

				if (this._valueType == typeof(DateTimeOffset))
				{
					serializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("DateTimeOffset"));
				}
				else if (this._valueType == typeof(TimeSpan))
				{
					serializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("TimeSpan"));
				}
				else
				{
					serializer = CachingXmlSerializerFactory.Create(this._valueType);
				}
				var tempReader = new StringReader(value);
				if (this.Units == EngineeringUnit.FmlFtIn16Th /*|| this.Units == EngineeringUnit.FML_FtIn8th*/)
				{
					string xmlString = tempReader.ReadToEnd();
					XmlDocument doc = new XmlDocument();
					if (xmlString != null)
					{
						doc.LoadXml(xmlString);
						XmlNode root = doc.DocumentElement;
						var xmlValue = root.InnerText;
						var isNegative = false;
						if (xmlValue.StartsWith("-"))
						{
							xmlValue = xmlValue.Remove(0, 1);
							isNegative = true;
						}
						var newValue = EngineeringUnitsHelperClass.ParseValue(
							typeof(double),
							xmlValue,
							this.Units,
							new NumberFormatInfo());

						if (isNegative)
						{
							newValue = (double)newValue * -1;
						}
						root.InnerText = newValue.ToString();
					}
					var tempReader2 = new StringReader(doc.OuterXml);
					this._value = (object)serializer.Deserialize(tempReader2);
				}
				else
				{
					this._value = (object)serializer.Deserialize(tempReader);
				}
				if (this._valueType == typeof(DateTimeOffset))
				{
					this._value = XmlConvert.ToDateTimeOffset(this._value as string);
				}
				else if (this._valueType == typeof(TimeSpan))
				{
					this._value = XmlConvert.ToTimeSpan(this._value as string);
				}
			}
		}

	}
}
