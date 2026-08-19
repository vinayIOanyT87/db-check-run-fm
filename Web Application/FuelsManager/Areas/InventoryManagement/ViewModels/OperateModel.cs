namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using System.Configuration;
	using System.Runtime.InteropServices;
	using static FMBusinessObjects.DataObjects.PointGroupSchedule;
	using System.Linq;

	[Serializable]
	public class OperateModel
	{

		public NumberFormatInfo Format;

		public DateTimeFormatInfo DateTimeFormatInfo { get; set; }

		public FMNumberFormatInfo SiteNumFormatInfo { get; set; }

		public string ShortDatePattern;

		public string TimePattern;

		public string TimeZone;

		public double TimeZoneOffset;

		public string DatepickerTimezoneString { get; set; }

		public Guid SiteGuid;

		public Guid UserGuid;

		public bool IsTabGroupEnabled;

		public bool ViewPointsRight;

		public bool ViewGraphicsRight;

		public bool ViewPointHistoryRight;

		public bool ViewPointGroupsRight;

		public bool ModifyPointGroupsRight;

		public bool CreatePublicPointGroupsRight;

		public bool ModifyPublicPointGroupsRight;

		public bool CreateSharedPointGroupsRight;

		public bool ModifySharedPointGroupsRight;

		public bool ViewMovementSummaryRight;

		public bool ModifyMovementSummaryRight;

		public bool CreatePublicMovementSummaryRight;

		public bool ModifyPublicMovementSummaryRight;

		public bool CreateSharedMovementSummaryRight;

		public bool ModifySharedMovementSummaryRight;

		public bool PointCalculatorRight;

		public bool LeakAnalysisRight;

		public bool ViewTrendsRight;

		public bool ModifyTrendsRight;

		public bool ViewIMReportsRight;

		public bool ModifyPointsRight;

		public bool ViewAlarmSummaryRight;

		public bool ViewAlarmHistoryRight;

		public bool ModifyMovementHistoryRight;
		public bool ViewMovementHistoryRight;

		public bool IsOperateViewOnlyMode { get; set; }

		public bool OpenAlarmSummary { get; set; }
		public string pointgroupreportgeneration { get; set; }

		public int MaxOperateTabsAllowed;

		public bool DisplayCUIDataMark { get; set; }

		public int OperateTagRefreshInterval { get; set; }
		public int OperateAlarmRefreshInterval { get; set; }

		public OperateModel()
		{
			this.OpenAlarmSummary = false;
			this.pointgroupreportgeneration = "";

			SetRefreshIntervals();
		}

		public OperateModel(OperateContext context)
		{
			_ = context;
			this.OpenAlarmSummary = false;

			SetRefreshIntervals();
		}

		private void SetRefreshIntervals()
		{
			this.OperateTagRefreshInterval = 1;
			this.OperateAlarmRefreshInterval = 1;
		}
	}

	public class OperateGraphic : FMBaseModel
	{
		[Required]
		public string Drawing { get; set; }
		public OperatePoint PointInformation { get; set; }

		public List<AnimationClass> Animations { get; set; }

	}

	public class OperatePointValue
	{
		public Guid SiteGuid { get; set; }
		public string SiteID { get; set; }
		public Guid PointValueIdentifier_IdentityGuid { get; set; }

		public PointValueType PointValueIdentifier_PointValueType { get; set; }

		public string PointValueIdentifier_PropertyID { get; set; }

		public string PointValueIdentifier_UtcTicks { get; set; }

		public Guid IdentityGuid { get; set; }

		public PointValueType PointValueType { get; set; }

		public string PropertyID { get; set; }

		public Guid PointGuid { get; set; }

		public string PointID { get; set; }

		public object Value { get; set; }

		public string ValueTypeString { get; set; }

		public long Status { get; set; }

		public string ID { get; set; }

		public DateTimeOffset ServerTimeStamp { get; set; }

		public EngineeringUnit Units { get; set; }

		public int DecimalPlaces { get; set; }

		public double Maximum { get; set; }

		public double Minimum { get; set; }

		public string QualityAbbreviation { get; set; }

		public EngineeringUnitType EngineeringUnitsType { get; set; }

		public bool Acknowledged { get; set; }

		public Guid AlarmPriorityGuid { get; set; }

		public string AlarmState { get; set; }

		public string ProductColor { get; set; }

		public string PatternColor { get; set; }

		public int PatternNumber { get; set; }

		public bool HasProductGraphicInfo { get; set; }

		public List<AlarmLimitValue> AlarmLimits { get; set; }

		public PointValueAccess Access { get; set; }

		public PointTemplateTag.PointTagInputOutputType InputOutputType { get; set; }

		public bool InhibitOverride { get; set; }

		public bool CommunicationsFailure { get; set; }

		public OperatePointValue()
		{
			this.AlarmLimits = null;
		}

		public OperatePointValue(PointValue p)
		{
			this.SiteGuid = p.PointValueIdentifier.SiteGuid;
			this.SiteID = p.SiteID;
			this.PointValueIdentifier_IdentityGuid = p.PointValueIdentifier.IdentityGuid;
			this.PointValueIdentifier_PointValueType = p.PointValueIdentifier.PointValueType;
			this.PointValueIdentifier_PropertyID = p.PointValueIdentifier.PropertyID;
			this.PointValueIdentifier_UtcTicks = p.ServerTimeStamp.UtcTicks.ToString();
			this.IdentityGuid = p.PointValueIdentifier.IdentityGuid;
			this.PointValueType = p.PointValueIdentifier.PointValueType;
			this.PropertyID = p.PointValueIdentifier.PropertyID;
			this.PointGuid = p.PointGuid;
			this.PointID = p.PointID;
			this.Value = p.Value;
			this.ValueTypeString = p.ValueTypeString;
			this.Status = p.Status;
			this.ID = (p.PointValueIdentifier.PointValueType == PointValueType.Point ? p.PointValueIdentifier.PropertyID : p.ID);
			this.ServerTimeStamp = p.ServerTimeStamp;
			this.Units = p.Units;
			this.DecimalPlaces = p.DecimalPlaces;
			this.Maximum = p.Maximum;
			this.Minimum = p.Minimum;
			this.QualityAbbreviation = p.QualityAbbreviation;
			this.EngineeringUnitsType = p.EngineeringUnitsType;
			this.Acknowledged = p.Acknowledged;
			this.AlarmPriorityGuid = p.AlarmPriorityGuid;
			this.AlarmState = p.AlarmState;
			this.ProductColor = p.ProductColor;
			this.PatternColor = p.PatternColor;
			this.PatternNumber = p.PatternNumber;
			this.HasProductGraphicInfo = p.HasProductGraphicInfo;
			this.Access = p.Access;
			this.InputOutputType = p.InputOutputType;
			this.InhibitOverride = p.InhibitOverride;
			this.AlarmLimits = null;
			this.CommunicationsFailure = false;
			if (p.AlarmLimitList != null && p.AlarmLimitList.Count > 0)
			{
				this.AlarmLimits = p.AlarmLimitList;
			}
		}
	}

	public class OperatePointGroupScheduleModel : FMBaseModel
	{
		[Required]
		public PointGroupSchedule PointGroupSchedule { get; set; }

		public List<string> Printers { get; set; }
		public List<string> ExportFileFormat { get; set; }

		public OperatePointGroupScheduleModel()
		{
			this.PointGroupSchedule = new PointGroupSchedule();
			this.Printers = new List<string>();
			this.ExportFileFormat = new List<string>();
			ExportFileFormat.AddRange(Enum.GetNames(typeof(ExportFileType)).ToList());
		}
	}

}