using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;

	[Serializable]
	public class PointAccessModel : FMBaseModel
	{
		public enum PointDetailViewMode { PointGroupView, UserGroupView, UserView };
		public List< PointAccessGroup> PointAccessGroupList { get; set; }
		public UserCollectionClass UserList { get; set; }
		public GroupCollectionClass UserGroupList { get; set; }

		public bool HasPointAccessModifyRight;
		public PointDetailViewMode InitialView { get; set; }

		public PointAccessModel()
		{
			this.PointAccessGroupList = new List<PointAccessGroup>();
			this.UserList = new UserCollectionClass();
			this.UserGroupList = new GroupCollectionClass();
			this.InitialView = PointDetailViewMode.PointGroupView;
		}

	}


}