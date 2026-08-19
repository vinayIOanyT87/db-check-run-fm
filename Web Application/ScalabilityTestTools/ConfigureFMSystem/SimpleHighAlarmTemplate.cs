

namespace ConfigureFMSystem
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class SimpleHighAlarmTemplate
	{

		public PointTemplate CreateAlarmTestTemplate(string templateID, Guid alarmPriority, Guid alarmCategory)
		{

			var pointTemplate = this.CreatePointTemplate(templateID);
			this.AddAlarmTestTags(pointTemplate, PointTemplateTag.PointTagInputOutputType.Manual); //Only Add Level Product Tag
			this.CreateHighAlarm(
				pointTemplate,
				this.GetTemplateTagByIDFromTemplate(pointTemplate, "Level Product"),
				34.00,alarmPriority,alarmCategory);
			return pointTemplate;

		}

		public PointTemplate CreateAlarmTestTemplateOpc(string templateID, Guid alarmPriority, Guid alarmCategory)
		{

			var pointTemplate = this.CreatePointTemplate(templateID);
			this.AddAlarmTestTags(pointTemplate, PointTemplateTag.PointTagInputOutputType.OpcUa); //Only Add Level Product Tag
			this.CreateHighAlarm(
				pointTemplate,
				this.GetTemplateTagByIDFromTemplate(pointTemplate, "Level Product"),
				34.00, alarmPriority, alarmCategory);
			return pointTemplate;

		}

		private PointTemplate CreatePointTemplate(string templateID)
		{
			var pointTemplate = new PointTemplate
			{
				ID = templateID,
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
			return pointTemplate;
		}

		private void AddAlarmTestTags(PointTemplate template, PointTemplateTag.PointTagInputOutputType type)
		{

			double valLevelProduct = 10.00;
			var tag = new PointTemplateTag
			          {
				          ID = "Level Product",
				          InputOutputType = type,
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
		}

		private string AlarmLimitName(string alarmName, string testName, bool useTestName, PointTemplateTag value)
		{
			string name = alarmName;
			if (!string.IsNullOrEmpty(testName) && useTestName)
			{
				name += " " + testName;
			}
			return value.ID + " " + name + " " + " Limit";
		}

		private string AlarmStateName(string alarmName, PointTemplateTag value)
		{
			return value.ID + " " + alarmName + " Alarm";
		}

		private PointTemplateTag AddAlarmLimitTag(
			PointTemplate template,
			string alarmName,
			string testName,
			bool useTestName,
			PointTemplateTag inputTag,
			double limit)
		{
			var tag = new PointTemplateTag
			{
				ID = this.AlarmLimitName(alarmName, testName, useTestName, inputTag),
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual,
				Input = true,
				ValueType = inputTag.ValueType,
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = inputTag.EngineeringUnitsType,
				Units = inputTag.Units,
				ServerUnits = inputTag.ServerUnits,
				DecimalPlaces = inputTag.DecimalPlaces,
				Maximum = inputTag.Maximum,
				Minimum = inputTag.Minimum,
				AlarmStatus = false,
				ApplyPointTemplateEngineeringUnits = inputTag.ApplyPointTemplateEngineeringUnits,
				ApplyPointTemplateDecimalPlaces = inputTag.ApplyPointTemplateDecimalPlaces,
				ApplyPointTemplateMaximum = inputTag.ApplyPointTemplateMaximum,
				ApplyPointTemplateMinimum = inputTag.ApplyPointTemplateMinimum,
				Value = limit
			};

			template.Tags.Add(tag.IdentityGuid, tag);
			return tag;
		}

		private PointTemplateTag AddAlarmStateTag(
			PointTemplate template,
			string alarmName,
			PointTemplateTag inputTag,
			double limit)
		{
			string highAlarmStateVal = "Normal";

			var tag = new PointTemplateTag
			{
				ID = this.AlarmStateName(alarmName, inputTag),
				InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated,
				Input = false,
				ValueType = typeof(string),
				IdentityGuid = Guid.NewGuid(),
				EngineeringUnitsType = EngineeringUnitType.FmuNodim,
				Units = EngineeringUnit.FmuNone,
				AlarmStatus = true,
				ServerUnits = EngineeringUnit.FmSiteUnits,
				DecimalPlaces = 2,
				Maximum = 1000,
				Minimum = 0,
				ApplyPointTemplateEngineeringUnits = false,
				ApplyPointTemplateDecimalPlaces = false,
				ApplyPointTemplateMaximum = false,
				ApplyPointTemplateMinimum = false,
				Value = highAlarmStateVal
			};

			template.Tags.Add(tag.IdentityGuid, tag);
			return tag;
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

		private void CreateHighAlarm(PointTemplate template, PointTemplateTag value, double limit,Guid alarmPriority, Guid alarmCategory)
		{
			string alarmName = "High";
         var a = new AlarmTemplate
			{
				ID = alarmName,
				InputTemplateTagGuid = value.IdentityGuid,
				AlarmCategoryApplicationStringGuid = alarmCategory,
				Order = 1,
				AlarmStateTemplateTagGuid = this.AddAlarmStateTag(template,alarmName,value,limit).IdentityGuid,
				NotAlarmState = "Normal"
			};
			string testID = "Greater Than";
			var at = new AlarmTestTemplate
			{
				ID = testID,
				AlarmTemplateGuid = a.IdentityGuid,
				LimitTemplateTagGuid = this.AddAlarmLimitTag(template, alarmName, testID, false, value, limit).IdentityGuid,
				AlarmPriorityGuid = alarmPriority,
				TestType = AlarmTestTemplate.TestTypeEnum.GreaterThan,
			};

			var ptas = new PointTemplateTagAlarmStatus
			{
								AlarmTestTemplateGuid = at.IdentityGuid
			};


			a.AlarmTestTemplates.Add(at.IdentityGuid, at);
			a.AlarmStatusTemplates.Add(ptas.IdentityGuid,ptas);
			value.AlarmTemplates.Add(a.IdentityGuid, a);
		}
	}
}
