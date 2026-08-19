

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	[Serializable]
	public class TagViewerModel
	{
		public NumberFormatInfo Format;

		public string ShortDatePattern;

		public string TimePattern;

		public List<PointValue> Values { get; set; }

		public SiteClass Site { get; set; }

		public Guid? PointTemplateTypeGuid { get; set; }

		[NonSerialized]
		public List<KeyValuePair<string, string>> PointTemplateTypeList;

		public Guid? PointTemplateGuid { get; set; }

		[NonSerialized]
		public List<KeyValuePair<Guid, string>> PointTemplateList;

		public Guid? PointCategoryGuid { get; set; }

		[NonSerialized]
		public List<KeyValuePair<Guid, string>> PointCategoryList;

		public string PointID { get; set; }

		public Guid PointGuid { get; set; }

		[NonSerialized]
		public List<KeyValuePair<Guid, string>> PointList;

		[NonSerialized]
		public List<KeyValuePair<Guid, string>> PointTagList;

		public const string SessionKey = "TagViewerContext";

		public List<List<int>> SortOrder;

		public int lastScrollPosition = 0;

		public int edititemrowposition = 0;

		public TagViewerModel()
		{
				this.Values = new List<PointValue>();
				this.PointGuid = Guid.Empty;
			this.SortOrder = new List<List<int>>();

		}
	}
}
