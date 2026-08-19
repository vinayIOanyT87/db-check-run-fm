
namespace ConfigureFMSystem
{
	using System;
	using System.Runtime.InteropServices;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	public class CreatePointTemplates
	{
		public SecurityClass Security;

		public string TankPointTemplateID;

		public string Site;

		public string HighPriorityAlarmCategoryString = "High Priority Alarms";

		public string HighPriorityAlarmPriorityString = "Highest";


		public PointTemplate TankPointTemplate
		{
			get
			{
				return GetPointTemplateByID(TankPointTemplateID);
			}
		}

		public Guid HighPriorityAlarmPriority
		{
			get
			{
				var alarmPriorityCollection = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(alarmPriorities => alarmPriorities.Enumerate(Security));
				foreach (var pri in alarmPriorityCollection)
				{
					if (pri.ID == HighPriorityAlarmPriorityString)
					{
						return pri.IdentityGuid;
					}
				}
				return FMChannelHelper.MakeCall<IAlarmPriorities, Guid>(alarmPriorities => alarmPriorities.Add(Security, new AlarmPriorityClass
				{
					ID = this.HighPriorityAlarmPriorityString,
					BackgroundSteady = AlarmPriorityClass.BackgroundSteadyDefaultColor,
					TextSteady = AlarmPriorityClass.TextSteadyDefaultColor,
					BackgroundAlternate = AlarmPriorityClass.BackGroundAlternateDefaultColor,
					TextAlternate = AlarmPriorityClass.TextAlternateDefaultColor
				}));
			}
		}

		public Guid HighPriorityAlarmCategory
		{
			get
			{
				var applicationStringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(appStrings => appStrings.EnumerateByType(this.Security, STRING_TYPE.ALARM_EVENT_CATEGORY));
				foreach (var appStr in applicationStringCollection)
				{
					if (appStr.ID == HighPriorityAlarmCategoryString)
					{
						return appStr.IdentityGuid;
					}
				}
				return FMChannelHelper.MakeCall<IApplicationStrings, Guid>(
				appStrings => appStrings.Add(Security, new ApplicationStringClass
				{
					Type = STRING_TYPE.ALARM_EVENT_CATEGORY,
					ID = HighPriorityAlarmCategoryString
				}
				));
			}
		}

		protected PointTemplate GetHighPriorityAlarmCategory(string ID)
		{

			var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, null));
			foreach (var pt in pointTemplates)
			{
				if (pt.ID == ID)
				{
					return pt;
				}
			}
			return null;
		}

		protected PointTemplate GetPointTemplateByID(string ID)
		{
			var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, null));
			foreach (var pt in pointTemplates)
			{
				if (pt.ID == ID)
				{
					return pt;
				}
			}
			return null;
		}

		public CreatePointTemplates(SecurityClass security, string site, string templateId)
		{
			TankPointTemplateID = templateId;
			Security = security;
			Site = site;
		}

		public void Delete()
		{
			if (TankPointTemplate != null)
			{
				FMChannelHelper.MakeCall<IPointTemplates>(x => x.Purge(this.Security, TankPointTemplate.IdentityGuid));
			}

		}

		#region SimpleHighAlarm

		public void CreateSimpleHighAlarmOpc()
		{
			if (TankPointTemplate == null)
			{
				var simpleHighAlarmTemplate = new SimpleHighAlarmTemplate();
				var tankPointTemplate = simpleHighAlarmTemplate.CreateAlarmTestTemplateOpc(this.TankPointTemplateID, this.HighPriorityAlarmPriority, this.HighPriorityAlarmCategory);

				FMChannelHelper.MakeCall<IPointTemplates>(x => x.Add(this.Security, tankPointTemplate));
			}

		}

		public void CreateSimpleHighAlarm()
		{
			if (TankPointTemplate == null)
			{
				var simpleHighAlarmTemplate = new SimpleHighAlarmTemplate();
				var tankPointTemplate = simpleHighAlarmTemplate.CreateAlarmTestTemplate(this.TankPointTemplateID, this.HighPriorityAlarmPriority, this.HighPriorityAlarmCategory);

				FMChannelHelper.MakeCall<IPointTemplates>(x => x.Add(this.Security, tankPointTemplate));
			}

		}
		#endregion

		#region VerticalTank

		public void CreateVerticalTank()
		{
			if (TankPointTemplate == null)
			{

				var tankPointTemplate = new PointTemplate
				{
					ID = TankPointTemplateID,
					IdentityGuid = Guid.NewGuid(),
					Description = string.Empty,
					Standard = false,
					ExecutionInterval = null,
					LevelUnit = EngineeringUnit.FmlFtIn16Th,
					TemperatureUnit = EngineeringUnit.FmtDegF,
					DensityUnit = EngineeringUnit.FmdDegApi,
					PressureUnit = EngineeringUnit.FmpPsi,
					FlowUnit = EngineeringUnit.FmvfGpm,
					VolumeUnit = EngineeringUnit.FmvUsGal,
					MassUnit = EngineeringUnit.FmmLb,
					VelocityUnit = EngineeringUnit.FmvrFpm,
					MassFlowUnit = EngineeringUnit.FmmfLbHr,
					LevelDecimalPlaces = 0,
					TemperatureDecimalPlaces = 2,
					DensityDecimalPlaces = 2,
					PressureDecimalPlaces = 2,
					FlowDecimalPlaces = 2,
					VolumeDecimalPlaces = 2,
					MassDecimalPlaces = 2,
					VelocityDecimalPlaces = 2,
					MassFlowDecimalPlaces = 2,
					LevelMaximum = 40,
					LevelMinimum = 0,
					TemperatureMaximum = 300,
					TemperatureMinimum = -300,
					DensityMaximum = 100,
					DensityMinimum = 0,
					PressureMaximum = 30,
					PressureMinimum = 0,
					VolumetricFlowMaximum = 1000,
					VolumetricFlowMinimum = -1000,
					VolumeMaximum = 10000,
					VolumeMinimum = 0,
					MassMaximum = 10000000,
					MassMinimum = 0,
					VelocityMaximum = 10,
					VelocityMinimum = -10,
					MassFlowMaximum = 3000,
					MassFlowMinimum = -3000
				};
				// Add a test set of tags
				this.AddVTTags(tankPointTemplate);
				this.AddVTStrapModule(tankPointTemplate);
				this.AddVTVcfModule(tankPointTemplate);
				this.AddVTQuantitiesModule(tankPointTemplate);
				this.AddVTShellCorrectionModule(tankPointTemplate);

				FMChannelHelper.MakeCall<IPointTemplates>(x => x.Add(this.Security, tankPointTemplate));
			}

		}

		private void AddVTTags(PointTemplate template)
		{
			/*
			SELECT 'Level Product' as [ID], 
			3 as [EngineeringUnitsType],
			27 as [EngineeringUnitsIndex],
			0 as [DecimalPlaces],
			0 as [ServerEngineeringUnitsIndex],
			5 as [DataTypeIndex],
			NULL as [Value],
			40 as [Maximum],
			0 as [Minimum],
			1 as [PointTagInputOutputTypeIndex],
			*/
			double valLevelProduct = 10.00;
			var tag = new PointTemplateTag
			{
				ID = "Level Product",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuLength,
				Units = EngineeringUnit.FmlFtIn16Th,
				ServerUnits = EngineeringUnit.FmlFtIn16Th,
				DecimalPlaces = 0,
				Maximum = 40,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true,
				Value = valLevelProduct
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Level Water' ,3,27,0,0,5,NULL,40,0,1
			double valLevelWater = 1.00;
			tag = new PointTemplateTag
			{
				ID = "Level Water",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuLength,
				Units = EngineeringUnit.FmlFtIn16Th,
				ServerUnits = EngineeringUnit.FmlFtIn16Th,
				DecimalPlaces = 0,
				Maximum = 40,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true,
				Value = valLevelWater
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Total Observed' ,5,46,2,0,5,NULL,10000.0,0,2

			//double valVolumeTotalObserved = 50.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Total Observed",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				ServerUnits = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				Maximum = 10000.00,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true//,
														  //Value = valVolumeTotalObserved
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Water' ,5,46,2,0,5,NULL,10000.0,0,2
			//double valVolumeWater = 10.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Water",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				ServerUnits = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				Maximum = 10000.00,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true//,
														  //Value = valVolumeWater
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Temperature Product' ,1,2,1,0,5,NULL,300.0,-300.0,1
			double valTemperatureProduct = 30.00;
			tag = new PointTemplateTag
			{
				ID = "Temperature Product",
				EngineeringUnitsType = EngineeringUnitType.FmuTemp,
				Units = EngineeringUnit.FmtDegF,
				DecimalPlaces = 1,
				ServerUnits = EngineeringUnit.FmtDegF,
				ValueType = typeof(double),
				Value = valTemperatureProduct,
				Maximum = 300.00,
				Minimum = -300.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Temperature Density' ,1,2,1,0,5,NULL,100.0,0.0,1,
			double valTemperatureDensity = 30.00;
			tag = new PointTemplateTag
			{
				ID = "Temperature Density",
				EngineeringUnitsType = EngineeringUnitType.FmuTemp,
				Units = EngineeringUnit.FmtDegF,
				DecimalPlaces = 1,
				ServerUnits = EngineeringUnit.FmtDegF,
				ValueType = typeof(double),
				Value = valTemperatureDensity,
				Maximum = 100.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Density Product Observed' ,11,191,1,0,5,NULL,100.0,0.0,2
			//double valDensityProductObserved = 20.00;
			tag = new PointTemplateTag
			{
				ID = "Density Product Observed",
				EngineeringUnitsType = EngineeringUnitType.FmuDensity,
				Units = EngineeringUnit.FmdDegApi,
				DecimalPlaces = 1,
				ServerUnits = EngineeringUnit.FmdDegApi,
				ValueType = typeof(double),
				//Value = valDensityProductObserved,
				Maximum = 100.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Density Product Standard' ,11,191,1,0,5,NULL,100.0,0.0,1
			double valDensityProductStandard = 20.00;
			tag = new PointTemplateTag
			{
				ID = "Density Product Standard",
				EngineeringUnitsType = EngineeringUnitType.FmuDensity,
				Units = EngineeringUnit.FmdDegApi,
				DecimalPlaces = 1,
				ServerUnits = EngineeringUnit.FmdDegApi,
				ValueType = typeof(double),
				Value = valDensityProductStandard,
				Maximum = 100.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Pressure Vapor' ,7,73,1,0,5,NULL,30.0,0.0,1
			double valPressureVapor = 10.00;
			tag = new PointTemplateTag
			{
				ID = "Pressure Vapor",
				EngineeringUnitsType = EngineeringUnitType.FmuPressure,
				Units = EngineeringUnit.FmpPsi,
				DecimalPlaces = 1,
				ServerUnits = EngineeringUnit.FmpPsi,
				ValueType = typeof(double),
				Value = valPressureVapor,
				Maximum = 30.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Correction for Temperature' ,16,255,5,0,5,NULL,1.0,0.0,2
			//double valVolumeCorrectionforTemperature = 1.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Correction for Temperature",
				EngineeringUnitsType = EngineeringUnitType.FmuNone,
				Units = EngineeringUnit.FmuNone,
				DecimalPlaces = 5,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeCorrectionforTemperature,
				Maximum = 2.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Correction for Pressure' ,16,255,5,0,5,NULL,1.0,0.0,2
			//double valVolumeCorrectionforPressure = 1.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Correction for Pressure",
				EngineeringUnitsType = EngineeringUnitType.FmuNone,
				Units = EngineeringUnit.FmuNone,
				DecimalPlaces = 5,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeCorrectionforPressure,
				Maximum = 2.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Correction for Temp and Press' ,16,255,5,0,5,NULL,1.0,0.0,2
			//double valVolumeCorrectionforTempandPressure = 1.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Correction for Temp and Press",
				EngineeringUnitsType = EngineeringUnitType.FmuNone,
				Units = EngineeringUnit.FmuNone,
				DecimalPlaces = 5,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeCorrectionforTempandPressure,
				Maximum = 2.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Correction Factor' ,16,255,5,0,5,NULL,1.0,0.0,2
			//double valVolumeCorrectionFactor = 1.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Correction Factor",
				EngineeringUnitsType = EngineeringUnitType.FmuNone,
				Units = EngineeringUnit.FmuNone,
				DecimalPlaces = 5,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeCorrectionFactor,
				Maximum = 2.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'API Correction Error' ,16,255,1,0,11,NULL,1.0,0.0,2
			//bool valAPICorrectionError = false;
			tag = new PointTemplateTag
			{
				ID = "API Correction Error",
				EngineeringUnitsType = EngineeringUnitType.FmuNone,
				Units = EngineeringUnit.FmuNone,
				DecimalPlaces = 1,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(bool),
				//Value = valAPICorrectionError,
				Maximum = 1.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Gross Observed' ,5,46,2,0,'System.Double',NULL,10000.0,0,2
			//double valVolumeGrossObserved = 100.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Gross Observed",
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeGrossObserved,
				Maximum = 10000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Roof Correction' ,5,46,2,0,'System.Double',NULL,10000.0,0,1
			double valVolumeRoofCorrection = 100.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Roof Correction",
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				Value = valVolumeRoofCorrection,
				Maximum = 10000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Bottoms' ,5,46,2,0,'System.Double',NULL,10000.0,0,2
			//double valVolumeBottoms = 100.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Bottoms",
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeBottoms,
				Maximum = 10000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Percent BSW' ,15,255,1,0,'System.Double',NULL,100.0,0,1
			double valPercentBSW = 10.00;
			tag = new PointTemplateTag
			{
				ID = "Percent BSW",
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				DecimalPlaces = 1,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				Value = valPercentBSW,
				Maximum = 100.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Tank Shell Correction' ,15,255,2,0,'System.Double',NULL,2.0,0,1
			double valTankShellCorrection = 1.00;
			tag = new PointTemplateTag
			{
				ID = "Tank Shell Correction",
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				Value = valTankShellCorrection,
				Maximum = 2.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Net Standard' ,5,46,2,0,'System.Double',NULL,10000.0,0,2
			//double valVolumeNetStandard = 100.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Net Standard",
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeNetStandard,
				Maximum = 10000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Mass Liquid' ,6,64,2,0,'System.Double',NULL,10000000.0,0,2
			//double valMassLiquid = 1000.00;
			tag = new PointTemplateTag
			{
				ID = "Mass Liquid",
				EngineeringUnitsType = EngineeringUnitType.FmuMass,
				Units = EngineeringUnit.FmmLb,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valMassLiquid,
				Maximum = 10000000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Solids' ,5,46,2,0,'System.Double',NULL,10000.0,0,1
			double valVolumeSolids = 100.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Solids",
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				Value = valVolumeSolids,
				Maximum = 10000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Density Product in Air' ,11,191,2,0,'System.Double',NULL,100.0,0,2
			//double valDensityProductinAir = 10.00;
			tag = new PointTemplateTag
			{
				ID = "Density Product in Air",
				EngineeringUnitsType = EngineeringUnitType.FmuDensity,
				Units = EngineeringUnit.FmdDegApi,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valDensityProductinAir,
				Maximum = 100.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Density Product Standard in Air' ,11,191,2,0,'System.Double',NULL,100.0,0,2
			//double valDensityProductStandardinAir = 10.00;
			tag = new PointTemplateTag
			{
				ID = "Density Product Standard in Air",
				EngineeringUnitsType = EngineeringUnitType.FmuDensity,
				Units = EngineeringUnit.FmdDegApi,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valDensityProductStandardinAir,
				Maximum = 100.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Weight Gross Standard' ,6,64,2,0,'System.Double',NULL,10000000.0,0,2
			//double valWeightGrossStandard = 1000.00;
			tag = new PointTemplateTag
			{
				ID = "Weight Gross Standard",
				EngineeringUnitsType = EngineeringUnitType.FmuMass,
				Units = EngineeringUnit.FmmLb,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valWeightGrossStandard,
				Maximum = 10000000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Weight Net Standard' ,6,64,2,0,'System.Double',NULL,10000000.0,0,2
			//double valWeightNetStandard = 1000.00;
			tag = new PointTemplateTag
			{
				ID = "Weight Net Standard",
				EngineeringUnitsType = EngineeringUnitType.FmuMass,
				Units = EngineeringUnit.FmmLb,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valWeightNetStandard,
				Maximum = 10000000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			//'Volume Gross Standard' ,5,46,2,0,'System.Double',NULL,10000000.0,0,2
			//double valVolumeGrossStandard = 1000.00;
			tag = new PointTemplateTag
			{
				ID = "Volume Gross Standard",
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				//Value = valVolumeGrossStandard,
				Maximum = 10000000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			// Density Vapor
			tag = new PointTemplateTag
			{
				ID = "Density Vapor",
				EngineeringUnitsType = EngineeringUnitType.FmuDensity,
				Units = EngineeringUnit.FmdDegApi,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				Maximum = 100.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);
			// Volume Vapor Net
			tag = new PointTemplateTag
			{
				ID = "Volume Vapor Net",
				EngineeringUnitsType = EngineeringUnitType.FmuVolume,
				Units = EngineeringUnit.FmvUsGal,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				Maximum = 10000000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			// Mass vapor
			tag = new PointTemplateTag
			{
				ID = "Mass Vapor",
				EngineeringUnitsType = EngineeringUnitType.FmuMass,
				Units = EngineeringUnit.FmmLb,
				DecimalPlaces = 2,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				ValueType = typeof(double),
				Maximum = 10000000.00,
				Minimum = 0.00,
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = true,
				IdentityGuid = Guid.NewGuid(),
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = true,
				ApplyPointTemplateDecimalPlaces = true,
				ApplyPointTemplateMaximum = true,
				ApplyPointTemplateMinimum = true
			};

			template.Tags.Add(tag.IdentityGuid, tag);
		}

		private void AddVesselProperty(PointTemplate template, ModuleToPointTemplateMap moduleToPointTemplate)
		{
			Vessel vessel = new Vessel();
			var vesselProp = new PointTemplateProperty()
			{
				ID = "Vessel",
				PointTemplateGuid = template.PointTemplateGuid,
				PointTemplatePropertyGuid = Guid.NewGuid(),
				Value = vessel
			};

			template.Properties.Add(vesselProp.IdentityGuid, vesselProp);

		}

		private void AddStrapProperty(PointTemplate template, ModuleToPointTemplateMap moduleToPointTemplate)
		{
			StrapTable strap = new StrapTable();
			strap.StrapTables = new IndividualStrapTable[6];
			for (int iLoop = 0; iLoop < 6; iLoop++)
			{
				if (strap.StrapTables[iLoop] == null)
					strap.StrapTables[iLoop] = new IndividualStrapTable();
				strap.StrapTables[iLoop].table.Add(new StrapTableEntry(0, 0));
				strap.StrapTables[iLoop].table.Add(new StrapTableEntry(40.00, 10000.00));
				strap.StrapTables[iLoop].RoofLandingHeight.Value = (double)(12);
			}

			var strapProp = new PointTemplateProperty()
			{
				ID = "StrapTable",
				PointTemplateGuid = template.PointTemplateGuid,
				PointTemplatePropertyGuid = Guid.NewGuid(),
				Value = strap
			};
			template.Properties.Add(strapProp.IdentityGuid, strapProp);

		}

		//SELECT 'QuantityCalculation',@QuantityModuleToPointTemplateGuid,@QuantityCalculation,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		/*
	  SELECT @QuantityCalculationGrossObservedVolume,'GrossObsVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Gross Observed',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationRoofVolume,'RoofVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Roof Correction',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationStrapVolume,'StrapVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Total Observed',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationBottomVolume,'BottomVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Bottoms',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationPercentBSW,'PercentBSW', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Percent BSW',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationVCF,'VCF', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Correction for Temp and Press',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationNetStandardVolume,'NetStandardVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Net Standard',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationStdDensity,'StdDensity', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Density Product Standard',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationMass,'Mass', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Mass Liquid',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationWaterVolume,'WaterVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Water',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationSolidsVolume,'SolidsVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Solids',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationTankShellCorrection,'TankShellCorrection', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Tank Shell Correction',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationDensityinAir,'DensityinAir', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Density Product in Air',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationStdDensityinAir,'StdDensityinAir', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Density Product Standard in Air',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationGrossStdWeight,'GrossStdWeight', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Weight Gross Standard',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationNetStdWeight,'NetStdWeight', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Weight Net Standard',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  SELECT @QuantityCalculationProdDensity,'ProductDensity', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Density Product Observed',0,'2015-02-04','Administrator','2015-02-04','Administrator'
		SELECT @QuantityCalculationGrossStdVolume,'GrossStdVolume', @QuantityModuleToPointTemplateGuid, @QuantityCalculation,'Volume Gross Standard',0,'2015-02-04','Administrator','2015-02-04','Administrator'
	  */


		private void AddVTStrapModule(PointTemplate template)
		{
			var mtpt = new ModuleToPointTemplateMap();
			mtpt.IdentityGuid = Guid.NewGuid();
			mtpt.ID = "StrapTable";
//			mtpt.ModuleGuid = module.IdentityGuid;
			mtpt.PointTemplateGuid = template.IdentityGuid;
			template.ModuleInstances.Add(mtpt.IdentityGuid, mtpt);

			AddStrapProperty(template, mtpt);
		}

		private void AddVTShellCorrectionModule(PointTemplate template)
		{
			var mtpt = new ModuleToPointTemplateMap();
			mtpt.IdentityGuid = Guid.NewGuid();
			mtpt.ID = "ShellCorrection";
//			mtpt.ModuleGuid = module.IdentityGuid;
			mtpt.PointTemplateGuid = template.IdentityGuid;
			template.ModuleInstances.Add(mtpt.IdentityGuid, mtpt);
			AddVesselProperty(template, mtpt);
		}

		private void AddVTVcfModule(PointTemplate template)
		{
			var mtpt = new ModuleToPointTemplateMap();
			mtpt.IdentityGuid = Guid.NewGuid();
			mtpt.ID = "VCF";
//			mtpt.ModuleGuid = module.IdentityGuid;
			mtpt.PointTemplateGuid = template.IdentityGuid;
			template.ModuleInstances.Add(mtpt.IdentityGuid, mtpt);

			AddVcfModuleSettingsProperty(template, mtpt);
		}

		private void AddVcfModuleSettingsProperty(PointTemplate template, ModuleToPointTemplateMap moduleToPointTemplate)
		{
			VcfModuleSettings vcfSettings = new VcfModuleSettings();

			var vcfProp = new PointTemplateProperty()
			{
				ID = "VcfSettings",
				PointTemplateGuid = template.PointTemplateGuid,
				PointTemplatePropertyGuid = Guid.NewGuid(),
				Value = vcfSettings
			};
			template.Properties.Add(vcfProp.IdentityGuid, vcfProp);
		}

		//SELECT 'Quantities.dll','Quantities.FMQuantities',@QuantityModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
		//SELECT 'Quantities',@QuantityModuleToPointTemplateGuid,@PointTemplateGuid,@QuantityModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
		private void AddVTQuantitiesModule(PointTemplate template)
		{
			var mtpt = new ModuleToPointTemplateMap();
			mtpt.IdentityGuid = Guid.NewGuid();
			mtpt.ID = "Quantities";
//			mtpt.ModuleGuid = module.IdentityGuid;
			mtpt.PointTemplateGuid = template.IdentityGuid;
			template.ModuleInstances.Add(mtpt.IdentityGuid, mtpt);
		}



		#endregion

		#region SignalSelector

		public void CreateSignalSelector()
		{
			if (TankPointTemplate != null)
			{
				throw new Exception("TankTemplate already created!");
			}

			var tankPointTemplate = new PointTemplate
			{
				ID = TankPointTemplateID,
				IdentityGuid = Guid.NewGuid(),
				Description = string.Empty,
				Standard = false,
				ExecutionInterval = null,
				LevelUnit = EngineeringUnit.FmlFtIn16Th,
				TemperatureUnit = EngineeringUnit.FmtDegF,
				DensityUnit = EngineeringUnit.FmdDegApi,
				PressureUnit = EngineeringUnit.FmpPsi,
				FlowUnit = EngineeringUnit.FmvfGpm,
				VolumeUnit = EngineeringUnit.FmvUsGal,
				MassUnit = EngineeringUnit.FmmLb,
				VelocityUnit = EngineeringUnit.FmvrFps,
				MassFlowUnit = EngineeringUnit.FmmfLbMin,
				LevelDecimalPlaces = 2,
				TemperatureDecimalPlaces = 2,
				DensityDecimalPlaces = 2,
				PressureDecimalPlaces = 2,
				FlowDecimalPlaces = 2,
				VolumeDecimalPlaces = 2,
				MassDecimalPlaces = 2,
				VelocityDecimalPlaces = 2,
				MassFlowDecimalPlaces = 2,
				LevelMaximum = 500,
				LevelMinimum = 0,
				TemperatureMaximum = 200,
				TemperatureMinimum = -60,
				DensityMaximum = 10,
				DensityMinimum = 0,
				PressureMaximum = 1000,
				PressureMinimum = 0,
				VolumetricFlowMaximum = 5000,
				VolumetricFlowMinimum = 0,
				VolumeMaximum = 1000000,
				VolumeMinimum = 0,
				MassMaximum = 1000000,
				MassMinimum = 0,
				VelocityMaximum = 1000,
				VelocityMinimum = 0,
				MassFlowMaximum = 1000000,
				MassFlowMinimum = 0
			};
			// Add a test set of tags
			this.AddTags(tankPointTemplate);
			this.AddModules(tankPointTemplate);

			FMChannelHelper.MakeCall<IPointTemplates>(x => x.Add(this.Security, tankPointTemplate));

		}

		private PointTemplateTag GetTemplateTagByIDFromTemplate(PointTemplate template, string tagId)
		{
			foreach (var tag in template.Tags.Values)
			{
				if (tag.ID == tagId)
				{
					return tag;
				}
			}
			return null;
		}

		private void AddTags(PointTemplate template)
		{
			var val = new Double();
			val = 250.00;
			var tag = new PointTemplateTag
			{
				ID = "Signal 1",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = val
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			tag = new PointTemplateTag
			{
				ID = "Signal 2",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = val
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			tag = new PointTemplateTag
			{
				ID = "Signal 3",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				ValueType = typeof(double),
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = val
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			tag = new PointTemplateTag
			{
				ID = "Signal 4",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = true,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = val
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			tag = new PointTemplateTag
			{
				ID = "High Signal",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = false,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = val
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			tag = new PointTemplateTag
			{
				ID = "Low Signal",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = false,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = val
			};

			template.Tags.Add(tag.IdentityGuid, tag);

			tag = new PointTemplateTag
			{
				ID = "Command Output",
				InputOutputType = PointTemplateTag.PointTagInputOutputType.OpcUa,
				Input = false,
				ValueType = typeof(double),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuNone,
				Units = EngineeringUnit.FmuNone,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = val
			};

			template.Tags.Add(tag.IdentityGuid, tag);
		}

		private void AddModules(PointTemplate template)
		{
			var mtpt = new ModuleToPointTemplateMap();
			mtpt.IdentityGuid = Guid.NewGuid();
			mtpt.ID = "SignalSelector";
//			mtpt.ModuleGuid = module.IdentityGuid;
			mtpt.PointTemplateGuid = template.IdentityGuid;
			template.ModuleInstances.Add(mtpt.IdentityGuid, mtpt);
		}

		#endregion SignalSelector
	}
}
