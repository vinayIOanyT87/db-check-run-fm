using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;

	[Serializable]
	public class PointAccessDetailModel : FMBaseModel
	{
		public enum PointDetailViewMode { PointGroupView, UserGroupView, UserView };
		public string Name;
		public Guid IdentityGuid;
		public bool HasPointAccessModifyRight;
		public List<PointAccessDetailCategory> CategoryList { get; set; }
		public List<PointAccessDetailPointTemplate> PointTemplateList { get; set; }
		public List<PointAccessDetailPoint> PointList { get; set; }
		public List<PointAccessDetailTag> TagList { get; set; }
		public List<PointAccessDetailAlarmTest> AlarmTestList { get; set; }
		public List<PointAccessDetailSetting> SettingList { get; set; }
		public PointDetailViewMode ScreenMode { get; set; }
		public List<PointAccessGroupAssignment> PointAccessGroupAssignmentList { get; set; }
		public GroupCollectionClass UserGroupList { get; set; }
		public UserCollectionClass Users { get; set; }
		public UserGroupMapCollectionClass UsertoGroupMap { get; set; }
		public Dictionary< string, List<Guid>> UserGroupToPointAccessGroupMap { get; set; }

		public PointAccessDetailModel()
		{
			this.PointTemplateList = new List<PointAccessDetailPointTemplate>();
			this.PointList = new List<PointAccessDetailPoint>();
			this.TagList = new List<PointAccessDetailTag>();
			this.AlarmTestList = new List<PointAccessDetailAlarmTest>();
			this.SettingList = new List<PointAccessDetailSetting>();
			this.ScreenMode = PointDetailViewMode.PointGroupView;
			this.PointAccessGroupAssignmentList = new List<PointAccessGroupAssignment>();
			this.UserGroupList = new GroupCollectionClass();
			this.Users = new UserCollectionClass();
			this.UsertoGroupMap = new UserGroupMapCollectionClass();
			this.UserGroupToPointAccessGroupMap = new Dictionary<string, List<Guid>>();
		}

	}

	[Serializable]
	public class PointAccessDetailCategory : FMBaseModel { 
		public Guid IdentityGuid { get; set; }
		public string Id {  get; set; }
	}

	[Serializable]
	public class PointAccessDetailPointTemplate : FMBaseModel
	{
		public string PointTemplateId { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public Guid? ProfileImageGuid { get; set; }
	}

	[Serializable]
	public class PointAccessDetailPoint : FMBaseModel
	{
		public string PointId { get; set; }
		public Guid PointGuid { get; set; }
		public string PointTemplateId { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public Guid? ProfileImageGuid { get; set; }
		public string Categories { get; set; }
		public bool HasDeviceAccessMapTags { get; set; }

    }

	[Serializable]
	public class PointAccessDetailSetting : FMBaseModel
	{
		public string SettingName { get; set; }
		public string PropertyID { get; set; }
		public Guid ExposedSettingGuid { get; set; }
		public string ModuleId { get; set; }
		public string PointTemplateId { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public bool ModifyDisabled { get; set; }
	}

	[Serializable]
	public class PointAccessDetailTag : FMBaseModel
	{
		public string TagId { get; set; }
		public Guid TagGuid { get; set; }
		public string PointTemplateId { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public string PointId { get; set; }
		public Guid PointGuid { get; set; }
		public bool IsDeviceAlarmMapTag { get;set; }
	}


	[Serializable]
	public class PointAccessDetailAlarmTest : FMBaseModel
	{
		public string PointAlarmTestId { get; set; }
		public Guid PointAlarmTestGuid { get; set; }
		public string TagId { get; set; }
		public Guid TagGuid { get; set; }
		public string PointTemplateId { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public bool IsDeviceAlarmMapAlarmTest { get; set; }
		public string PointId { get; set; }
		public Guid PointGuid { get; set; }
	}



	[Serializable]
	public class PointAccessGroupAssignment 
	{
		public string Name { get; set; }
		public Guid PointAccessGroupGuid { get; set; }
		public List<PointAccessGroupToPointTemplateAssignment> PointAccessGroupToPointTemplateAssignmentList { get; set; }
		public List<PointAccessGroupToPointAssignment> PointAccessGroupToPointAssignmentList { get; set; }
		public List<PointAccessGroupToSettingAssignment> PointAccessGroupToSettingAssignmentList { get; set; }
		public List<PointAccessGroupToTagAssignment> PointAccessGroupToTagAssignmentList { get; set; }
		public List<PointAccessGroupToAlarmTestAssignment> PointAccessGroupToAlarmTestAssignmentList { get; set; }
		public List<PointAccessGroupToUserGroupAssignment> PointAccessGroupToUserGroupAssignmentList { get; set; }
		public List<PointAccessGroupToPointAlarmTestAssignment> PointAccessGroupToPointAlarmTestAssignmentList { get; set; }
		public List<PointAccessGroupToPointTagAssignment> PointAccessGroupToPointTagAssignmentList { get; set; }

		public PointAccessGroupAssignment()
		{
			this.PointAccessGroupToPointTemplateAssignmentList = new List<PointAccessGroupToPointTemplateAssignment>();
			this.PointAccessGroupToPointAssignmentList = new List<PointAccessGroupToPointAssignment>();
			this.PointAccessGroupToSettingAssignmentList = new List<PointAccessGroupToSettingAssignment>();
			this.PointAccessGroupToTagAssignmentList = new List<PointAccessGroupToTagAssignment>();
			this.PointAccessGroupToAlarmTestAssignmentList = new List<PointAccessGroupToAlarmTestAssignment>();
			this.PointAccessGroupToUserGroupAssignmentList = new List<PointAccessGroupToUserGroupAssignment>();
			this.PointAccessGroupToPointAlarmTestAssignmentList = new List<PointAccessGroupToPointAlarmTestAssignment>();
			this.PointAccessGroupToPointTagAssignmentList = new List<PointAccessGroupToPointTagAssignment>();
		}
	}

	[Serializable]
	public class PointAccessGroupToPointTemplateAssignment
	{
		public Guid PointTemplateGuid { get; set; }
		public Guid PointAccessGroupToPointTemplateGuid { get; set; }
		public bool Assigned { get; set; }
	}

	[Serializable]
	public class PointAccessGroupToPointAssignment
	{
		public Guid PointGuid { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public Guid PointAccessGroupToPointGuid { get; set; }
		public bool Assigned { get; set; }
	}

	[Serializable]
	public class PointAccessGroupToSettingAssignment
	{
		public Guid PointAccessGroupToExposedSettingGuid { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public Guid ExposedSettingGuid { get; set; }
		public string PropertyID { get; set; }
		public bool View { get; set; }
		public bool Modify { get; set; }
	}

	[Serializable]
	public class PointAccessGroupToTagAssignment
	{
		public Guid PointAccessGroupToTagGuid { get; set; }
		public Guid PointTagGuid { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public bool IsDeviceAlarmMapTag { get; set; }
		public bool View { get; set; }
		public bool Modify { get; set; }
		public bool ExceedRange { get; set; }
		public bool Override { get; set; }
	}

	[Serializable]
	public class PointAccessGroupToPointTagAssignment
	{
		public Guid PointAccessGroupToPointTagGuid { get; set; }
		public Guid PointTagGuid { get; set; }
		public Guid PointGuid { get; set; }
		public bool IsDeviceAlarmMapTag { get; set; }
		public bool View { get; set; }
		public bool Modify { get; set; }
		public bool ExceedRange { get; set; }
		public bool Override { get; set; }
	}


	[Serializable]
	public class PointAccessGroupToAlarmTestAssignment
	{
		public Guid PointAccessGroupToAlarmTestGuid { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public Guid AlarmTestTemplateGuid { get; set; }
		public bool View { get; set; }
		public bool Acknowledge { get; set; }
	}

	[Serializable]
	public class PointAccessGroupToUserGroupAssignment
	{
		public Guid UserGroupGuid { get; set; }
		public Guid PointAccessGroupToUserGroupGuid { get; set; }
		public bool Assigned { get; set; }
	}

	[Serializable]
	public class PointAccessGroupToPointAlarmTestAssignment {
		public Guid PointAccessGroupToPointAlarmTestGuid { get; set; }
		public Guid PointGuid { get; set; }
		public Guid AlarmTestGuid { get; set; }
		public bool View { get; set; }
		public bool Acknowledge { get; set; }
    }
}
