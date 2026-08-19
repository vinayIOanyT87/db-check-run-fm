
namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class AlarmEditorAlarmTestModel
	{
		public Guid AlarmTestGuid;

		public string Id;

		public string AlarmTestEquation;

		public Guid LimitTagGuid;

		public string LimitValue;

		public string LimitTagId;

		public string LimitTagValueType;

		public int LimitTagDecimalPlaces;

		public bool LimitTagEditable;

		public EngineeringUnitType LimitTagUnitsType;

		public EngineeringUnit LimitTagUnits;

		public double LimitTagMax;

		public double LimitTagMin;

		public string AlarmState;

		public string AlarmText;

		public int Order;

		public double HoldOff;

		public string HelpFile;

		public string DrawingId;

		public Guid DrawingGuid;

		public bool Enabled;

		public string HoldOffMinutes;

		public string HoldOffSeconds;

		public Guid AlarmPriorityGuid;

		public Guid NormalUnacknowledgedAlarmPriorityGuid;

		public AlarmTestTemplate.BitwiseOperatorEnum BitwiseOperator;

		public long BitMask;

		public AlarmTestTemplate.TagFieldEnum TagField;

		public AlarmTestTemplate.TestTypeEnum TestType;
	}

	[Serializable]
	public class AlarmEditorAlarmModel
	{
		public Guid AlarmGuid;

		public string Id;

		public string Category;

		public int Order;

		public string SuppressedAndShelvedStatus;

		public Guid AlarmStatusTagGuid;

		public string AlarmStatusTagId;

        public bool Enabled;

        public bool Notify;

        public bool Exclusive;

		public string NotAlarmState;

		public List<AlarmEditorAlarmTestModel> AlarmTests;
	}

	[Serializable]
	public class AlarmEditorTagModel
	{
		public Guid PointTagGuid;

		public string Id;

		public string Hysteresis;

		public int TagDecimalPlaces;

      public EngineeringUnit TagUnits;

		public string DataType;

		public bool AlarmsEnabled;

		public List<AlarmEditorAlarmModel> Alarms;
	}

	[Serializable]
	public class AlarmEditorModel
	{
		public bool HasModifyEnabled;

		public bool HasEnableAlarmOnPointRight;

        public bool HasDisableAlarmOnPointRight;

        public bool HasNotifyAlarmOnPointRight;

        public bool HasEnableAlarmOnPointTemplateRight;

		public bool HasDisableAlarmOnPointTemplateRight;

		public bool HasPointEditRight;

		public bool HasPTEditRight;

		public int[] NumberGroupSizes;

		public string NumberGroupSeparator;

		public string NumberDecimalSeparator;

		public int DecimalPlaces;

		public string ShortDatePattern;

		public Guid PointGuid;

		public Guid PointTemplateGuid;

		public List<AlarmEditorTagModel> Tags;

		public Dictionary<Guid, string> AlarmCategories;

		public AlarmPriorityCollectionClass AlarmPriorities;

		public AlarmPriorityCollectionClass NormalPriorities;

	}
}
