namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Opc.Ua;
	using Attributes;
	using UtilityObjects;
	using System.Xml.Serialization;
	using System.IO;
	using System.Linq;
	using System.Web.Script.Serialization;
	using System.Xml;

	using FMCore;

	[DataContract]
	[Serializable]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(CodedVariables.RoofTypeEnum))]
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
	[KnownType(typeof(CodedVariables.TransferDirection))]
	[KnownType(typeof(CodedVariables.StrapTableSelect))]
	[KnownType(typeof(CodedVariables.Reset))]
	[KnownType(typeof(CodedVariables.NodeTransferMode))]
	[KnownType(typeof(CodedVariables.NodeTransferStatus))]
	[KnownType(typeof(VolumeTransferModuleSettings))]
	[KnownType(typeof(MovementModuleSettings))]
	[KnownType(typeof(MovementData))]
	[KnownType(typeof(MovementNodeData))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(CodedVariables.MovementType))]
	public class AlarmLimitValue
	{
		[DataMember]
		public Guid IdentityGuid { get; set; }

		[DataMember]
		public Guid AlarmPriorityGuid { get; set; }

		[DataMember]
		public object Value { get; set; }
	}

	[DataContract]
	[Serializable]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(CodedVariables.RoofTypeEnum))]
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
	[KnownType(typeof(CodedVariables.TransferDirection))]
	[KnownType(typeof(CodedVariables.StrapTableSelect))]
	[KnownType(typeof(CodedVariables.Reset))]
	[KnownType(typeof(CodedVariables.NodeTransferMode))]
	[KnownType(typeof(CodedVariables.NodeTransferStatus))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(CodedVariables.MovementType))]
	public class PointValue : ICloneable
	{
		private string _valueTypeString;

		[DataMember]
		public PointValueIdentifier PointValueIdentifier { get; set; }

		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public string SiteID { get; set; }

		[DataMember]
		public string SiteNumber { get; set; }



		[DataMember]
		public string PointID { get; set; }

		[DataMember]
		public string PointType { get; set; }

		[DataMember]
		public string PointDescription { get; set; }

		[DataMember]
		public bool Enabled { get; set; }


		[DataMember]
		public Guid PointGuid { get; set; }

		[DataMember]
		public Guid PointTemplateTagGuid { get; set; }

		[DataMember]
		public Guid PointTemplatePropertyGuid { get; set; }


		[DataMember]
		public EngineeringUnitType EngineeringUnitsType { get; set; }

		[DataMember]
		public EngineeringUnit Units { get; set; }

		[DataMember]
		public EngineeringUnit ServerUnits { get; set; }

		[DataMember]
		public object Value { get; set; }

		[DataMember]
		public string ValueTypeString
		{
			get
			{
				return _valueTypeString;
			}
			set
			{
				this._valueTypeString = value;

				if (!string.IsNullOrEmpty(_valueTypeString)
				&& _valueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
				{ 
					if(Value is int)
					{
						Value = Enum.ToObject(Type.GetType(_valueTypeString), Value);
					}
				}
			}
		}

		[DataMember]
		public byte DecimalPlaces { get; set; }

		[DataMember]
		public Int64 Status { get; set; }


		[DataMember]
		public DateTimeOffset ServerTimeStamp { get; set; }

		[DataMember]
		public DateTimeOffset SourceTimeStamp { get; set; }

		[DataMember]
		public double Maximum { get; set; }

		[DataMember]
		public double Minimum { get; set; }

		[DataMember]
		public bool Input { get; set; }

		[DataMember]
		public PointTemplateTag.PointTagInputOutputType InputOutputType { get; set; }

		[DataMember]
		public Guid AlarmPriorityGuid { get; set; }

		[DataMember]
		public bool Acknowledged { get; set; }

		[DataMember]
		public string AlarmState { get; set; }

		[DataMember]
		public bool HasAlarm { get; set; }

		[DataMember]
		public bool IsPointTemplateValue { get; set; }

		[DataMember]
		public string ProductColor { get; set; }

		[DataMember]
		public string PatternColor { get; set; }

		[DataMember]
		public int PatternNumber { get; set; }

		[DataMember]
		public bool HasProductGraphicInfo { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool AlarmsEnabled { get; set; }


		[DataMember]
		[FMPersistedField]
		public bool InhibitOverride { get; set; }

		[DataMember]
		public List<AlarmLimitValue> AlarmLimitList { get; set; }

		[DataMember]
		public PointValueAccess Access { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public Guid WellKnownIdentityGuid { get; set; }



		public PointValue()
		{
			this.Init();
		}

		public PointValue(string id)
		{
			this.Init();
			this.ID = id;
		}

		public PointValue(PointTag pointTag)
		{
			pointTag.ThrowIfNull("pointTag");

			this.Init();
			this.PointGuid = pointTag.PointGuid;
			this.PointTemplateTagGuid = pointTag.PointTemplateTagGuid;
			this.SiteID = pointTag.SiteID;
			this.PointID = pointTag.PointID;
			this.PointType = pointTag.PointType;
			this.PointDescription = pointTag.PointDescription;
			this.Enabled = pointTag.Enabled;
			this.ID = pointTag.ID;

			this.PointValueIdentifier = new PointValueIdentifier()
			{
				IdentityGuid = pointTag.IdentityGuid,
				PointValueType = PointValueType.Tag,
				PropertyID = null,
				SiteGuid = pointTag.SiteGuid,
				UtcTicks = pointTag.ServerTimeStamp.UtcTicks,
				WellKnownIdentityGuid = pointTag.WellKnownIdentityGuid
			};

			this.EngineeringUnitsType = pointTag.EngineeringUnitsType;
			this.Units = pointTag.Units;
			this.ServerUnits = pointTag.ServerUnits;
			this.Value = pointTag.Value;
			this.ValueTypeString = pointTag.ValueTypeString;
			this.DecimalPlaces = pointTag.DecimalPlaces;
			this.ServerTimeStamp = pointTag.ServerTimeStamp;
			this.SourceTimeStamp = pointTag.SourceTimeStamp;
			this.Status = pointTag.Status;
			this.Maximum = pointTag.Maximum;
			this.Minimum = pointTag.Minimum;
			this.Input = pointTag.Input;
			this.InputOutputType = pointTag.InputOutputType;
			this.AlarmPriorityGuid = pointTag.AlarmPriorityGuid;
			this.Acknowledged = pointTag.Acknowledged;
			this.AlarmState = pointTag.AlarmState;
			this.InhibitOverride = pointTag.InhibitOverride;
			this.WellKnownIdentityGuid = pointTag.WellKnownIdentityGuid;
			this.HasAlarm = pointTag.Alarms.Any();
		}

		public PointValue(PointTemplateTag pointTemplateTag, string pointTemplateID)
		{
			pointTemplateTag.ThrowIfNull("pointTemplateTag");

			this.Init();
			this.IsPointTemplateValue = true;
			this.PointGuid = pointTemplateTag.PointTemplateGuid;
			this.PointTemplateTagGuid = pointTemplateTag.PointTemplateTagGuid;
			this.PointID = pointTemplateID;
			this.ID = pointTemplateTag.ID;

			this.PointValueIdentifier = new PointValueIdentifier()
			{
				IdentityGuid = pointTemplateTag.IdentityGuid,
				PointValueType = PointValueType.Tag,
				PropertyID = null,
				SiteGuid = pointTemplateTag.SiteGuid
			};

			this.EngineeringUnitsType = pointTemplateTag.EngineeringUnitsType;
			this.Units = pointTemplateTag.Units;
			this.ServerUnits = pointTemplateTag.ServerUnits;
			this.Value = pointTemplateTag.Value;
			this.ValueTypeString = pointTemplateTag.ValueTypeString;
			this.DecimalPlaces = pointTemplateTag.DecimalPlaces;
			this.Maximum = pointTemplateTag.Maximum;
			this.Minimum = pointTemplateTag.Minimum;
			this.Input = pointTemplateTag.Input;
			this.InputOutputType = pointTemplateTag.InputOutputType;
			this.InhibitOverride = pointTemplateTag.InhibitOverride;
			this.HasAlarm = pointTemplateTag.AlarmTemplates.Any();
		}

		public PointValue(PointValueIdentifier pointValueIdentifier, PointProperty pointProperty, Point point)
		{
			pointValueIdentifier.ThrowIfNull("pointValueIdentifier");
			pointProperty.ThrowIfNull("pointProperty");
			point.ThrowIfNull("point");

			this.Init();
			this.PointGuid = pointProperty.PointGuid;
			this.PointID = pointProperty.PointID;

			this.PointValueIdentifier = new PointValueIdentifier()
			{
				IdentityGuid = pointValueIdentifier.IdentityGuid,
				PointValueType = pointValueIdentifier.PointValueType,
				PropertyID = pointValueIdentifier.PropertyID,
				SiteGuid = point.SiteGuid,
				UtcTicks = pointProperty.UpdatedDate.ToUniversalTime().UtcTicks,
				WellKnownIdentityGuid = pointValueIdentifier.WellKnownIdentityGuid,
				SubIdentifierGuid = pointValueIdentifier.SubIdentifierGuid,
			};

			this.PointTemplatePropertyGuid = pointProperty.PointTemplatePropertyGuid;
			var propertyType = pointProperty.Value.GetType();
			var propertyInfo = propertyType.GetProperty(pointValueIdentifier.PropertyID);
			if (propertyInfo == null)
			{
				throw new Exception("No such property : " + pointValueIdentifier.PropertyID);
			}

			var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
			FMExposedSetting fmExposedSetting = null;
			if (fmExposedSettings.Length > 0)
			{

				fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;
				this.ID = string.Empty;

				if (fmExposedSetting != null)
				{
					this.ID = fmExposedSetting.ID;
					this.Input = fmExposedSetting.Input;
				}
			}
			else
			{
				this.ID = pointValueIdentifier.PropertyID;
				this.Input = true;
			}

			this.Value = propertyInfo.GetValue(pointProperty.Value);
			this.ValueTypeString = propertyInfo.PropertyType.ToString();
			this.ServerTimeStamp = pointProperty.UpdatedDate.ToUniversalTime();
			this.SourceTimeStamp = pointProperty.UpdatedDate.ToUniversalTime();
			this.Status = (this.Value != null) ? StatusCodes.Good : StatusCodes.Bad;

			if (this.ValueTypeString == typeof(PointPropertyUnitTypedDouble).ToString())
			{
				this.EngineeringUnitsType = (this.Value as PointPropertyUnitTypedDouble).EngineeringUnitsType;
				this.Value = (this.Value as PointPropertyUnitTypedDouble).Value;
				this.ValueTypeString = this.Value.GetType().ToString();

				switch (this.EngineeringUnitsType)
				{
					case EngineeringUnitType.FmuLength:
						this.Units = point.LevelUnit;
						this.DecimalPlaces = point.LevelDecimalPlaces;
						this.Maximum = point.LevelMaximum;
						this.Minimum = point.LevelMinimum;
						break;

					case EngineeringUnitType.FmuDensity:
						this.Units = point.DensityUnit;
						this.DecimalPlaces = point.DensityDecimalPlaces;
						this.Maximum = point.DensityMaximum;
						this.Minimum = point.DensityMinimum;
						break;

					case EngineeringUnitType.FmuMass:
						this.Units = point.MassUnit;
						this.DecimalPlaces = point.MassDecimalPlaces;
						this.Maximum = point.MassMaximum;
						this.Minimum = point.MassMinimum;
						break;

					case EngineeringUnitType.FmuMassflow:
						this.Units = point.MassFlowUnit;
						this.DecimalPlaces = point.MassFlowDecimalPlaces;
						this.Maximum = point.MassFlowMaximum;
						this.Minimum = point.MassFlowMinimum;
						break;

					case EngineeringUnitType.FmuPressure:
						this.Units = point.PressureUnit;
						this.DecimalPlaces = point.PressureDecimalPlaces;
						this.Maximum = point.PressureMaximum;
						this.Minimum = point.PressureMinimum;
						break;

					case EngineeringUnitType.FmuTemp:
						this.Units = point.TemperatureUnit;
						this.DecimalPlaces = point.TemperatureDecimalPlaces;
						this.Maximum = point.TemperatureMaximum;
						this.Minimum = point.TemperatureMinimum;
						break;

					case EngineeringUnitType.FmuVolume:
						this.Units = point.VolumeUnit;
						this.DecimalPlaces = point.VolumeDecimalPlaces;
						this.Maximum = point.VolumeMaximum;
						this.Minimum = point.VolumeMinimum;
						break;

					case EngineeringUnitType.FmuVelocity:
						this.Units = point.VelocityUnit;
						this.DecimalPlaces = point.VelocityDecimalPlaces;
						this.Maximum = point.VelocityMaximum;
						this.Minimum = point.VelocityMinimum;
						break;

					case EngineeringUnitType.FmuVolflow:
						this.Units = point.FlowUnit;
						this.DecimalPlaces = point.FlowDecimalPlaces;
						this.Maximum = point.VolumetricFlowMaximum;
						this.Minimum = point.VolumetricFlowMinimum;
						break;
				}
			}

			// Opton to Exposed Settings
			else if(fmExposedSetting != null)
			{
				this.Maximum = fmExposedSetting.Maximum;
				this.Minimum = fmExposedSetting.Minimum;
				this.DecimalPlaces = fmExposedSetting.DecimalPlaces;
				this.EngineeringUnitsType = fmExposedSetting.EngineeringUnitsType;
				this.Units = fmExposedSetting.Units;
			}

			// Set InputOutputType to System for Movement Data, this will prevent persistance in PointServiceManager SetPointValueData
			if (pointProperty.ID == "Movement Data")
			{
				this.InputOutputType = PointTemplateTag.PointTagInputOutputType.System;
			}
			else
			{
				this.InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual;
			}

			this.InhibitOverride = false;
		}

		public PointValue(PointValueIdentifier pointValueIdentifier, PointTemplateProperty pointTemplateProperty, PointTemplate pointTemplate)
		{
			pointValueIdentifier.ThrowIfNull("pointValueIdentifier");
			pointTemplateProperty.ThrowIfNull("pointTemplateProperty");
			pointTemplate.ThrowIfNull("pointTemplate");

			this.Init();
			this.IsPointTemplateValue = true;
			this.PointGuid = pointTemplateProperty.PointTemplateGuid;
			this.PointID = pointTemplate.ID;
			this.PointValueIdentifier = pointValueIdentifier;
			this.PointValueIdentifier.SiteGuid = pointTemplate.SiteGuid;
			this.PointTemplatePropertyGuid = pointTemplateProperty.PointTemplatePropertyGuid;
			var propertyType = pointTemplateProperty.Value.GetType();
			var propertyInfo = propertyType.GetProperty(pointValueIdentifier.PropertyID);
			if (propertyInfo == null)
			{
				throw new Exception("No such property : " + pointValueIdentifier.PropertyID);
			}

			var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
			if (fmExposedSettings.Length > 0)
			{

				var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;
				this.ID = string.Empty;

				if (fmExposedSetting != null)
				{
					this.ID = fmExposedSetting.ID;
				}
			}
			else
			{
				this.ID = pointValueIdentifier.PropertyID;
			}

			this.Value = propertyInfo.GetValue(pointTemplateProperty.Value);
			this.ValueTypeString = propertyInfo.PropertyType.ToString();
			this.ServerTimeStamp = pointTemplateProperty.UpdatedDate;
			this.SourceTimeStamp = pointTemplateProperty.UpdatedDate;
			this.Status = (this.Value != null) ? StatusCodes.Good : StatusCodes.Bad;
			if (this.ValueTypeString == typeof(PointPropertyUnitTypedDouble).ToString())
			{
				this.EngineeringUnitsType = (this.Value as PointPropertyUnitTypedDouble).EngineeringUnitsType;
				this.Value = (this.Value as PointPropertyUnitTypedDouble).Value;
				this.ValueTypeString = this.Value.GetType().ToString();

				switch (this.EngineeringUnitsType)
				{
					case EngineeringUnitType.FmuLength:
						this.Units = pointTemplate.LevelUnit;
						this.DecimalPlaces = pointTemplate.LevelDecimalPlaces;
						this.Maximum = pointTemplate.LevelMaximum;
						this.Minimum = pointTemplate.LevelMinimum;
						break;

					case EngineeringUnitType.FmuDensity:
						this.Units = pointTemplate.DensityUnit;
						this.DecimalPlaces = pointTemplate.DensityDecimalPlaces;
						this.Maximum = pointTemplate.DensityMaximum;
						this.Minimum = pointTemplate.DensityMinimum;
						break;

					case EngineeringUnitType.FmuMass:
						this.Units = pointTemplate.MassUnit;
						this.DecimalPlaces = pointTemplate.MassDecimalPlaces;
						this.Maximum = pointTemplate.MassMaximum;
						this.Minimum = pointTemplate.MassMinimum;
						break;

					case EngineeringUnitType.FmuMassflow:
						this.Units = pointTemplate.MassFlowUnit;
						this.DecimalPlaces = pointTemplate.MassFlowDecimalPlaces;
						this.Maximum = pointTemplate.MassFlowMaximum;
						this.Minimum = pointTemplate.MassFlowMinimum;
						break;

					case EngineeringUnitType.FmuPressure:
						this.Units = pointTemplate.PressureUnit;
						this.DecimalPlaces = pointTemplate.PressureDecimalPlaces;
						this.Maximum = pointTemplate.PressureMaximum;
						this.Minimum = pointTemplate.PressureMinimum;
						break;

					case EngineeringUnitType.FmuTemp:
						this.Units = pointTemplate.TemperatureUnit;
						this.DecimalPlaces = pointTemplate.TemperatureDecimalPlaces;
						this.Maximum = pointTemplate.TemperatureMaximum;
						this.Minimum = pointTemplate.TemperatureMinimum;
						break;

					case EngineeringUnitType.FmuVolume:
						this.Units = pointTemplate.VolumeUnit;
						this.DecimalPlaces = pointTemplate.VolumeDecimalPlaces;
						this.Maximum = pointTemplate.VolumeMaximum;
						this.Minimum = pointTemplate.VolumeMinimum;
						break;

					case EngineeringUnitType.FmuVelocity:
						this.Units = pointTemplate.VelocityUnit;
						this.DecimalPlaces = pointTemplate.VelocityDecimalPlaces;
						this.Maximum = pointTemplate.VelocityMaximum;
						this.Minimum = pointTemplate.VelocityMinimum;
						break;

					case EngineeringUnitType.FmuVolflow:
						this.Units = pointTemplate.FlowUnit;
						this.DecimalPlaces = pointTemplate.FlowDecimalPlaces;
						this.Maximum = pointTemplate.VolumetricFlowMaximum;
						this.Minimum = pointTemplate.VolumetricFlowMinimum;
						break;
				}
			}
			else
			{
				this.DecimalPlaces = 0;
				this.EngineeringUnitsType = EngineeringUnitType.FmuNodim;
				this.Units = EngineeringUnit.FmuNone;
				this.Maximum = 0.0;
				this.Minimum = 0.0;
			}
			this.InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual;
			this.InhibitOverride = false;
		}

		public PointValue(PointValueIdentifier pointValueIdentifier, Point point)
		{
			pointValueIdentifier.ThrowIfNull("pointValueIdentifier");
			point.ThrowIfNull("point");

			this.Init();
			this.PointGuid = point.PointGuid;
			this.SiteID = point.SiteID;
			this.SiteNumber = point.SiteNumber;
			this.PointID = point.ID;
			this.PointType = point.PointType;
			this.PointDescription = point.Description;

			this.PointValueIdentifier = new PointValueIdentifier()
			{
				IdentityGuid = pointValueIdentifier.IdentityGuid,
				PointValueType = pointValueIdentifier.PointValueType,
				PropertyID = pointValueIdentifier.PropertyID,
				SiteGuid = point.SiteGuid,
				UtcTicks = point.UpdatedDate.ToUniversalTime().UtcTicks,
				WellKnownIdentityGuid = pointValueIdentifier.WellKnownIdentityGuid,
				SubIdentifierGuid = pointValueIdentifier.SubIdentifierGuid,
			};

			var propertyType = point.GetType();
			var propertyInfo = propertyType.GetProperty(pointValueIdentifier.PropertyID);

			if (propertyInfo == null)
			{
				throw new Exception("No such property : " + pointValueIdentifier.PropertyID);
			}

			var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);

			if (fmExposedSettings.Length > 0)
			{
				var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;
				this.ID = string.Empty;

				if (fmExposedSetting != null)
				{
					this.ID = fmExposedSetting.ID;
				}
			}
			else
			{
				this.ID = pointValueIdentifier.PropertyID;
			}

			this.Value = propertyInfo.GetValue(point);
			this.ValueTypeString = propertyInfo.PropertyType.ToString();
			this.ServerTimeStamp = point.UpdatedDate.ToUniversalTime();
			this.SourceTimeStamp = point.UpdatedDate.ToUniversalTime();
			var type = Type.GetType(this.ValueTypeString);

			this.Status = (this.Value != null
							|| this.ValueTypeString == "System.String"
							|| (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))) ? StatusCodes.Good : StatusCodes.Bad;

			if (this.ValueTypeString == typeof(PointPropertyUnitTypedDouble).ToString())
			{
				this.EngineeringUnitsType = (this.Value as PointPropertyUnitTypedDouble).EngineeringUnitsType;
				this.Value = (this.Value as PointPropertyUnitTypedDouble).Value;
				this.ValueTypeString = this.Value.GetType().ToString();

				switch (this.EngineeringUnitsType)
				{
					case EngineeringUnitType.FmuLength:
						this.Units = point.LevelUnit;
						this.DecimalPlaces = point.LevelDecimalPlaces;
						this.Maximum = point.LevelMaximum;
						this.Minimum = point.LevelMinimum;
						break;

					case EngineeringUnitType.FmuDensity:
						this.Units = point.DensityUnit;
						this.DecimalPlaces = point.DensityDecimalPlaces;
						this.Maximum = point.DensityMaximum;
						this.Minimum = point.DensityMinimum;
						break;

					case EngineeringUnitType.FmuMass:
						this.Units = point.MassUnit;
						this.DecimalPlaces = point.MassDecimalPlaces;
						this.Maximum = point.MassMaximum;
						this.Minimum = point.MassMinimum;
						break;

					case EngineeringUnitType.FmuMassflow:
						this.Units = point.MassFlowUnit;
						this.DecimalPlaces = point.MassFlowDecimalPlaces;
						this.Maximum = point.MassFlowMaximum;
						this.Minimum = point.MassFlowMinimum;
						break;

					case EngineeringUnitType.FmuPressure:
						this.Units = point.PressureUnit;
						this.DecimalPlaces = point.PressureDecimalPlaces;
						this.Maximum = point.PressureMaximum;
						this.Minimum = point.PressureMinimum;
						break;

					case EngineeringUnitType.FmuTemp:
						this.Units = point.TemperatureUnit;
						this.DecimalPlaces = point.TemperatureDecimalPlaces;
						this.Maximum = point.TemperatureMaximum;
						this.Minimum = point.TemperatureMinimum;
						break;

					case EngineeringUnitType.FmuVolume:
						this.Units = point.VolumeUnit;
						this.DecimalPlaces = point.VolumeDecimalPlaces;
						this.Maximum = point.VolumeMaximum;
						this.Minimum = point.VolumeMinimum;
						break;

					case EngineeringUnitType.FmuVelocity:
						this.Units = point.VelocityUnit;
						this.DecimalPlaces = point.VelocityDecimalPlaces;
						this.Maximum = point.VelocityMaximum;
						this.Minimum = point.VelocityMinimum;
						break;

					case EngineeringUnitType.FmuVolflow:
						this.Units = point.FlowUnit;
						this.DecimalPlaces = point.FlowDecimalPlaces;
						this.Maximum = point.VolumetricFlowMaximum;
						this.Minimum = point.VolumetricFlowMinimum;
						break;
				}
			}
			else
			{
				this.DecimalPlaces = 0;
				this.EngineeringUnitsType = EngineeringUnitType.FmuNodim;
				this.Units = EngineeringUnit.FmuNone;
				this.Maximum = 0.0;
				this.Minimum = 0.0;
			}
			this.InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual;
			this.InhibitOverride = false;
		}

		public PointValue(PointValueIdentifier pointValueIdentifier, PointTemplate pointTemplate)
		{
			pointValueIdentifier.ThrowIfNull("pointValueIdentifier");
			pointTemplate.ThrowIfNull("point");

			this.Init();
			this.IsPointTemplateValue = true;
			this.PointGuid = pointTemplate.PointTemplateGuid;
			this.PointID = pointTemplate.ID;
			this.PointValueIdentifier = pointValueIdentifier;
			this.PointValueIdentifier.SiteGuid = pointTemplate.SiteGuid;
			var propertyType = pointTemplate.GetType();
			var propertyInfo = propertyType.GetProperty(pointValueIdentifier.PropertyID);

			if (propertyInfo == null)
			{
				throw new Exception("No such property : " + pointValueIdentifier.PropertyID);
			}

			var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);

			if (fmExposedSettings.Length > 0)
			{
				var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;
				this.ID = string.Empty;

				if (fmExposedSetting != null)
				{
					this.ID = fmExposedSetting.ID;
				}
			}
			else
			{
				this.ID = pointValueIdentifier.PropertyID;
			}

			this.Value = propertyInfo.GetValue(pointTemplate);
			this.ValueTypeString = propertyInfo.PropertyType.ToString();
			this.ServerTimeStamp = pointTemplate.UpdatedDate;
			this.SourceTimeStamp = pointTemplate.UpdatedDate;
			var type = Type.GetType(this.ValueTypeString);

			this.Status = (this.Value != null
							|| this.ValueTypeString == "System.String"
							|| (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))) ? StatusCodes.Good : StatusCodes.Bad;

			if (this.ValueTypeString == typeof(PointPropertyUnitTypedDouble).ToString())
			{
				this.EngineeringUnitsType = (this.Value as PointPropertyUnitTypedDouble).EngineeringUnitsType;
				this.Value = (this.Value as PointPropertyUnitTypedDouble).Value;
				this.ValueTypeString = this.Value.GetType().ToString();

				switch (this.EngineeringUnitsType)
				{
					case EngineeringUnitType.FmuLength:
						this.Units = pointTemplate.LevelUnit;
						this.DecimalPlaces = pointTemplate.LevelDecimalPlaces;
						this.Maximum = pointTemplate.LevelMaximum;
						this.Minimum = pointTemplate.LevelMinimum;
						break;

					case EngineeringUnitType.FmuDensity:
						this.Units = pointTemplate.DensityUnit;
						this.DecimalPlaces = pointTemplate.DensityDecimalPlaces;
						this.Maximum = pointTemplate.DensityMaximum;
						this.Minimum = pointTemplate.DensityMinimum;
						break;

					case EngineeringUnitType.FmuMass:
						this.Units = pointTemplate.MassUnit;
						this.DecimalPlaces = pointTemplate.MassDecimalPlaces;
						this.Maximum = pointTemplate.MassMaximum;
						this.Minimum = pointTemplate.MassMinimum;
						break;

					case EngineeringUnitType.FmuMassflow:
						this.Units = pointTemplate.MassFlowUnit;
						this.DecimalPlaces = pointTemplate.MassFlowDecimalPlaces;
						this.Maximum = pointTemplate.MassFlowMaximum;
						this.Minimum = pointTemplate.MassFlowMinimum;
						break;

					case EngineeringUnitType.FmuPressure:
						this.Units = pointTemplate.PressureUnit;
						this.DecimalPlaces = pointTemplate.PressureDecimalPlaces;
						this.Maximum = pointTemplate.PressureMaximum;
						this.Minimum = pointTemplate.PressureMinimum;
						break;

					case EngineeringUnitType.FmuTemp:
						this.Units = pointTemplate.TemperatureUnit;
						this.DecimalPlaces = pointTemplate.TemperatureDecimalPlaces;
						this.Maximum = pointTemplate.TemperatureMaximum;
						this.Minimum = pointTemplate.TemperatureMinimum;
						break;

					case EngineeringUnitType.FmuVolume:
						this.Units = pointTemplate.VolumeUnit;
						this.DecimalPlaces = pointTemplate.VolumeDecimalPlaces;
						this.Maximum = pointTemplate.VolumeMaximum;
						this.Minimum = pointTemplate.VolumeMinimum;
						break;

					case EngineeringUnitType.FmuVelocity:
						this.Units = pointTemplate.VelocityUnit;
						this.DecimalPlaces = pointTemplate.VelocityDecimalPlaces;
						this.Maximum = pointTemplate.VelocityMaximum;
						this.Minimum = pointTemplate.VelocityMinimum;
						break;

					case EngineeringUnitType.FmuVolflow:
						this.Units = pointTemplate.FlowUnit;
						this.DecimalPlaces = pointTemplate.FlowDecimalPlaces;
						this.Maximum = pointTemplate.VolumetricFlowMaximum;
						this.Minimum = pointTemplate.VolumetricFlowMinimum;
						break;
				}
			}
			else
			{
				this.DecimalPlaces = 0;
				this.EngineeringUnitsType = EngineeringUnitType.FmuNodim;
				this.Units = EngineeringUnit.FmuNone;
				this.Maximum = 0.0;
				this.Minimum = 0.0;
			}
			this.InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual;
			this.InhibitOverride = false;
		}

		public UInt32 OpcStatusSubCode => new StatusCode((uint)this.Status).SubCode;


		public UInt32 OpcStatusCodeBits => new StatusCode((uint)this.Status).CodeBits;

		public bool IsForced()
		{
			return this.OpcStatusCodeBits == StatusCodes.GoodLocalOverride;
		}

		/// <summary>
		/// Uses XmlSerializer
		/// </summary>
		[XmlIgnore]
		[ScriptIgnore]
		public string ValueXml
		{
			get
			{
				string retValue;
				object value = this.Value;

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
				}

				return retValue;
			}
		}

		public bool IsOverrange()
		{
			if (this.Value is double && (double)this.Value > this.Maximum)
			{
				return true;
			}
			var statusCode = new StatusCode((uint)this.Status);

			return statusCode.LimitBits == LimitBits.High;
		}

		public bool IsUnderrange()
		{
			if (this.Value is double && (double)this.Value < this.Minimum)
			{
				return true;
			}
			var statusCode = new StatusCode((uint)this.Status);

			return statusCode.LimitBits == LimitBits.Low;
		}

		public bool IsUncertain()
		{
			return StatusCode.IsUncertain((uint)this.Status);
		}

		public bool IsGood()
		{
			return StatusCode.IsGood((uint)this.Status);
		}

		public bool IsBad()
		{
			return StatusCode.IsBad((uint)this.Status);
		}

		public bool IsManualTag()
		{
			return this.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual;
		}

		public string QualityAbbreviation
		{
			get
			{

				if (this.IsManualTag())
				{
					return "MAN";
				}

				if (this.IsForced())
				{
					return "FRC";
				}

				if (this.IsOverrange())
				{
					return "OVR";
				}

				if (this.IsUnderrange())
				{
					return "UNR";
				}

				if (this.IsUncertain())
				{
					return "UNC";
				}
				//Needs to return blank string for spacing in Draw and Operate
				return "";
			}
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.HasAlarm = false;
			this.HasProductGraphicInfo = false;
			this.ProductColor = string.Empty;
			this.PatternColor = string.Empty;
			this.PatternNumber = 1;
			this.AlarmLimitList = new List<AlarmLimitValue>();
			this.Access = new PointValueAccess();
		}

		public object Clone()
		{
			var t = (PointValue) this.MemberwiseClone();
			return t;
		}
	}
}
