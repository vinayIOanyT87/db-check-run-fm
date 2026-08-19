namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using FMBusinessObjects.DataObjects;
    using Areas.Controllers;

    [Serializable]
	public class PointsFilterModel : FMBaseModel
    {
		public const string SessionKey = "PointsFilterContext";

		public List<Point> Points { get; set; }

		public bool DeleteEnabled = true;

		public bool IsExplorer = false;

		public List<SelectListItem> ActionListCategories { get; set; }
		public List<SelectListItem> ActionListPointTypes { get; set; }

		public string PointTypeId { get; set; }
		public string CategoryId { get; set; }

		public string ParentControl { get; set; }
		public bool PersistChanges { get; set; } 


		public PointsFilterModel()
		{
			this.ActionListCategories = new List<SelectListItem>();
			this.ActionListPointTypes = new List<SelectListItem>();
			this.PointTypeId = string.Empty;
			this.CategoryId = string.Empty;
			this.ParentControl = "";
			this.PersistChanges = false;
		}

	public PointsFilterModel( PointsFilterModel context )
		{
			this.Points = context.Points;
			this.ActionListCategories = new List<SelectListItem>();
			this.ActionListPointTypes = new List<SelectListItem>();
			this.PointTypeId = string.Empty;
			this.CategoryId = string.Empty;
			this.ParentControl = "";
			this.PersistChanges = false;
		}
	}
}