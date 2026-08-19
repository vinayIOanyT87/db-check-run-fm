

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using Areas.Controllers;
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	[Serializable]
	public class SelectionAssociations : IComparable
	{
		public string Key { get; set; }

		public string Value { get; set; }

		public int CompareTo(object o)
		{
			var selectionAssociation = o as SelectionAssociations;
			if (selectionAssociation == null)
				throw new Exception("Invalid SelectionAssociation");
			return Value.CompareTo(selectionAssociation.Value);
		}

		public SelectionAssociations()
		{
			this.Key = Guid.Empty.ToString();
			this.Value = string.Empty;
		}

		public SelectionAssociations(string key, string value)
		{
			this.Key = key;
			this.Value = value;

		}
	}



	[Serializable]
	public class TagSelectionModel
	{
		public List<PointValue> PointValues { get; set; }

		public List<PointValueFieldType> Fields { get; set; }

		public List<SelectionAssociations> SelectedValues { get; set; }

		public string PointTemplateTypeGuid { get; set; }

		public List<SelectionAssociations> PointTemplateTypeList;

		public string PointTemplateGuid { get; set; }

		public List<SelectionAssociations> PointTemplateList;

		public string PointCategoryGuid { get; set; }

		public List<SelectionAssociations> PointCategoryList;

		public Guid PointGuid { get; set; }

		public PointValueType ValueType { get; set; }

		public List<SelectionAssociations> PointList;

		public List<SelectionAssociations> ValueList;

		public bool AllowMultipleSelect { get; set; }

		public bool AllowPointSelect { get; set; }

		public bool EnableValueTypeSelection { get; set; }

		public bool EnableFieldSelection	{ get; set; }

		public bool EnableTagSelection{ get; set; }

        public PANELTYPE PanelType { get; set; }

	    public bool IsPointDetailDrawing => (this.PanelType.Equals(PANELTYPE.Detail));

	    public bool PointTemplateTagSelectionIndicator { get; set; }

        public bool PointTrendButton { get; set; }

		public bool FilterByDataType { get; set; }

		public string DataTypeFilter { get; set; }

		public PointValueFieldType FieldFilter { get; set; }

		public bool ApplyPointAccess { get; set; }

		public TagSelectionModel()
		{
			this.PointValues = new List<PointValue>();
			this.PointGuid = Guid.Empty;
			this.AllowMultipleSelect = true;
			this.AllowPointSelect = true;
			this.EnableValueTypeSelection = true;
			this.EnableFieldSelection = false;
			this.EnableTagSelection = true;
            this.Fields = new List<PointValueFieldType>();
			this.SelectedValues = new List<SelectionAssociations>();
            this.PanelType = PANELTYPE.Standard;
			this.FilterByDataType = false;
			this.DataTypeFilter = string.Empty;
			this.FieldFilter = PointValueFieldType.VALUE;
			this.ApplyPointAccess = false;
		}



		public List<KeyValuePair<string, string>> GetValueTypeList()
		{
			var valueTypeList = new List<KeyValuePair<string, string>>();

            valueTypeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText(PointValueType.Point.ToString()), PointValueType.Point.ToString()));
            valueTypeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText(PointValueType.Setting.ToString()), PointValueType.Setting.ToString()));
            valueTypeList.Add(new KeyValuePair<string, string>(FMBaseController.TranslateText(PointValueType.Tag.ToString()), PointValueType.Tag.ToString()));

            return valueTypeList;
		}
	}
}
