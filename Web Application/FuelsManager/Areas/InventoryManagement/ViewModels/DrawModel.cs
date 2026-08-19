namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using System.Globalization;

	[Serializable]
	public class TagUnitToUnitType
	{
		public int Unit { get; set; }

		public string UnitStr { get; set; }

		public int UnitType { get; set; }

		public string UnitTypeStr { get; set; }

		public string UnitDescription { get; set; }

		public string UnitAbbreviation { get; set; }
	}

	[Serializable]
	public class FMNumberFormatInfo
	{
		public string NegativeSign { get; set; }
		public int NumberDecimalDigits { get; set; }
		public string NumberDecimalSeparator { get; set; }
		public string NumberGroupSeparator { get; set; }
		public int NumberGroupSizes { get; set; }
		public int NumberNegativePattern { get; set; }
		public string ShortDatePattern { get; set; }
	}

	[Serializable]
	public class DrawModel
	{
		public Drawing Drawing { get; set; }
		public List<DrawPropertyMenuRecord> CommonPropertyList { get; set; }
		public List<DrawPropertyMenuRecord> RectanglePropertyList { get; set; }
		public FMNumberFormatInfo SiteNumFormatInfo { get; set; }

		public List<TagUnitToUnitType> TagUnitToUnitTypeList { get; set; }

		public DateTimeFormatInfo DateTimeFormatInfo { get; set; }

		public DrawModel()
		{
		}

		public DrawModel(DrawContext context)
		{
			if (context != null && context.Model != null)
			{
				this.Drawing = context.Model.Drawing;
			}
		}
	}

	[Serializable]
	public class pointValueIdentifierNamedWitDataType
	{
		public string ID { get; set; }

		public PointValueIdentifier pointValueIdentifier { get; set; }

		public string DataType { get; set; }

		public string UnitType { get; set; }
	}

	[Serializable]
	public class wizardOptions
	{
		public List<pointValueIdentifierNamedWitDataType> pointValueIdentifiers { get; set; }
		public List<KeyValuePair<Guid,string>> pointNameList { get; set; }
		public List<KeyValuePair<Guid, string>> drawingsList { get; set; }
		public List<KeyValuePair<Guid, string>> animationList { get; set; }
		public wizardOptions()
		{
			this.drawingsList = new List<KeyValuePair<Guid, string>>();
			this.pointNameList = new List<KeyValuePair<Guid, string>>();
			this.pointValueIdentifiers = new List<pointValueIdentifierNamedWitDataType>();
			this.animationList = new List<KeyValuePair<Guid, string>>();
		}
	}
}