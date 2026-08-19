using System;
using System.Collections.Generic;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System.Linq;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;

	[Serializable]
	public class SavedViewOwnerOptionModel
	{
		public string Value { get; set; }
		public string Text { get; set; }

		public SavedViewOwnerOptionModel(string value, string text)
		{
			this.Value = value;
			this.Text = text;
		}
	}

	[Serializable]
	public class PointGroupModel : FMBaseModel
	{
		public Guid PointGroupGuid;
		public string Name;
		public string ID;
		public string Description;
		public string Rows;
		public string Columns;
		public int FontSize;
		public PointGroup.PointGroupVisibilityType PointGroupType;
		public Boolean IsEditable;
		public Boolean IsOwnedByMe;
		public Guid OwnerUserGuid;
		public string Owner;
		public bool ViewPointGroupsRight;
      public bool ModifyPointGroupsRight;
      public bool AdministerPointGroupsRight;
      public bool CreatePublicPointGroupsRight;
		public bool ModifyPublicPointGroupsRight;
		public bool CreateSharedPointGroupsRight;
		public bool ModifySharedPointGroupsRight;

		public PointGroupModel()
		{
			this.PointGroupGuid = Guid.Empty;
			this.ID = string.Empty;
			this.Description = string.Empty;
			this.Rows = string.Empty;
			this.Columns = string.Empty;
			this.FontSize = 14;
			this.PointGroupType = PointGroup.PointGroupVisibilityType.Public;
			this.IsEditable = true;
			this.IsOwnedByMe = true;
			this.OwnerUserGuid = Guid.Empty;
			this.Owner = String.Empty;
			this.ViewPointGroupsRight = false;
			this.ModifyPointGroupsRight = false; 
			this.CreatePublicPointGroupsRight = false;
			this.ModifyPublicPointGroupsRight = false;
			this.CreateSharedPointGroupsRight = false;
			this.ModifySharedPointGroupsRight = false;
			this.AdministerPointGroupsRight = false;

      }

	}

	[Serializable]
	public class PointGroupsFilterModel : FMBaseModel
	{
		public List<PointGroupModel> pointGroups;
		public List<SavedViewOwnerOptionModel> OwnerOptions;

		public bool ModifyPointGroupsRight;
      public bool AdministerPointGroupsRight;

      public PointGroupsFilterModel()
		{
			this.pointGroups = new List<PointGroupModel>();
			this.OwnerOptions = new List<SavedViewOwnerOptionModel>();
		}

		public List<PointGroupModel> GetPublicPointGroups()
		{
			return this.pointGroups.Where(x => x.PointGroupType == PointGroup.PointGroupVisibilityType.Public).OrderBy(y => y.ID).ToList();
		}
		public List<PointGroupModel> GetPrivatePointGroups()
		{
			return this.pointGroups.Where(x => x.PointGroupType == PointGroup.PointGroupVisibilityType.Private).OrderBy(y => y.ID).ToList();
		}
		public List<PointGroupModel> GetSharedPointGroups()
		{
			return this.pointGroups.Where(x => x.PointGroupType == PointGroup.PointGroupVisibilityType.Shared).OrderBy(y => y.ID).ToList();
		}

	}

}
