namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using FMBusinessObjects.DataObjects;
    using FuelsManager.Areas.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class MovementSummaryMenuModel : FMBaseModel
	{
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementSummaryMenuModel()
        {
			this.Init();
        }
        #endregion

        #region Properties
        public Guid MovementSummaryGuid { get; set; }
		public string Name { get; set; }
		public string ID { get; set; }
		public string Description { get; set; }
		public string Rows { get; set; }
		public string Columns { get; set; }
		public int FontSize { get; set; }
		public MovementSummary.MovementSummaryVisibilityType MovementSummaryType { get; set; }
		public bool IsEditable { get; set; }
		public bool IsOwnedByMe { get; set; }
		public Guid OwnerUserGuid { get; set; }
		public string Owner { get; set; }
		public bool ViewMovementSummaryRight { get; set; }
		public bool ModifyMovementSummaryRight { get; set; }
		public bool CreatePublicMovementSummaryRight { get; set; }
		public bool ModifyPublicMovementSummaryRight { get; set; }
		public bool CreateSharedMovementSummaryRight { get; set; }
      public bool ModifySharedMovementSummaryRight { get; set; }
      public bool AdministerMovementSummaryRight { get; set; }
      public string RowVersion { get; set; }
        #endregion

        #region Private methods
		private void Init()
        {
			this.MovementSummaryGuid				= Guid.Empty;
			this.Name								= string.Empty;
			this.ID									= string.Empty;
			this.Description						= string.Empty;
			this.Rows								= string.Empty;
			this.Columns							= string.Empty;
			this.FontSize							= 14;
			this.IsEditable							= true;
			this.IsOwnedByMe						= true;
			this.OwnerUserGuid						= Guid.Empty;
			this.Owner								= string.Empty;
			this.ViewMovementSummaryRight			= false;
			this.ModifyMovementSummaryRight			= false;
			this.CreatePublicMovementSummaryRight	= false;
			this.ModifyPublicMovementSummaryRight	= false;
			this.CreateSharedMovementSummaryRight	= false;
			this.ModifySharedMovementSummaryRight	= false;
			this.MovementSummaryType				= MovementSummary.MovementSummaryVisibilityType.Public;
			this.RowVersion = "00000000";
		}
		#endregion
	}

    [Serializable]
	public class MovementSummaryFilterModel : FMBaseModel
	{
		public const string SessionKey = "MovementSummariesFilterContext";

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementSummaryFilterModel()
		{
			this.MovementSummaries = new List<MovementSummaryMenuModel>();
			this.OwnerOptions = new List<SavedViewOwnerOptionModel>();
			this.ActionListCategories = new List<SelectListItem>();
			this.ActionListPointTypes = new List<SelectListItem>();
			this.PointTypeId = string.Empty;
			this.CategoryId = string.Empty;
			this.ParentControl = "";
			this.PersistChanges = false;
		}
		public MovementSummaryFilterModel(MovementSummaryFilterModel context)
		{
			this.MovementPoints = context.MovementPoints;
			this.MovementSummaries = new List<MovementSummaryMenuModel>();
			this.OwnerOptions = new List<SavedViewOwnerOptionModel>();
			this.ActionListCategories = new List<SelectListItem>();
			this.ActionListPointTypes = new List<SelectListItem>();
			this.PointTypeId = string.Empty;
			this.CategoryId = string.Empty;
			this.ParentControl = "";
			this.PersistChanges = false;
		}
		#endregion

		#region Properties
		public List<MovementSummaryMenuModel> MovementSummaries { get; set; }
		public List<SavedViewOwnerOptionModel> OwnerOptions { get; set; }
      public bool ModifyMovementSummaryRight { get; set; }
      public bool AdministerMovementSummaryRight { get; set; }
      public List<Point> MovementPoints { get; set; }

		public bool DeleteEnabled = true;

		public bool IsExplorer = false;

		public List<SelectListItem> ActionListCategories { get; set; }
		public List<SelectListItem> ActionListPointTypes { get; set; }

		public string PointTypeId { get; set; }
		public string CategoryId { get; set; }

		public string ParentControl { get; set; }
		public bool PersistChanges { get; set; }
		#endregion

		#region Public methods
		public List<MovementSummaryMenuModel> GetPublicMovementSummary()
		{
			return this.MovementSummaries.Where(x => x.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public).OrderBy(y => y.ID).ToList();
		}
		public List<MovementSummaryMenuModel> GetPrivateMovementSummary()
		{
			return this.MovementSummaries.Where(x => x.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Private).OrderBy(y => y.ID).ToList();
		}
		public List<MovementSummaryMenuModel> GetSharedMovementSummary()
		{
			return this.MovementSummaries.Where(x => x.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Shared).OrderBy(y => y.ID).ToList();
		}
        #endregion
    }
}
