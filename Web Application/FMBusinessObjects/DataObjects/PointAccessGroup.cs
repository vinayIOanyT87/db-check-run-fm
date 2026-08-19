
namespace FMBusinessObjects.DataObjects
{
	using Attributes;
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroup : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointAccessGroupGuid
		{
			get
			{
				return base.IdentityGuid;
			}

			set
			{
				base.IdentityGuid = value;
			}
		}

		[DataMember]
		public List<PointAccessGroupToPointTemplateMap> PointAccessGroupToPointTemplateMapList { get; set; }

		[DataMember]
		public List<PointAccessGroupToPointMap> PointAccessGroupToPointMapList { get; set; }

		[DataMember]
		public List<PointAccessGroupToTagMap> PointAccessGroupToTagMapList { get; set; }

		[DataMember]
		public List<PointAccessGroupToAlarmTestMap> PointAccessGroupToAlarmTestMapList { get; set; }

		[DataMember]
		public List<PointAccessGroupToExposedSettingMap> PointAccessGroupToExposedSettingMapList { get; set; }

		[DataMember]
		public List<PointAccessGroupToUserGroupMap> PointAccessGroupToUserGroupMapList { get; set; }

		[DataMember]
		public List<PointAccessGroupToPointAlarmTestMap> PointAccessGroupToPointAlarmTestMapList { get; set; }

		[DataMember]
		public List<PointAccessGroupToPointTagMap> PointAccessGroupToPointTagMapList { get; set; }


		public static void EnumerateBySiteGuidSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "SELECT pag.* FROM dbo.tblPointAccessGroup pag WHERE pag.SiteGuid = @SiteGuid ORDER BY ID";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public static void EnumerateByUserGroupGuidSQL(SqlCommand cmd, Guid siteGuid, Guid userGroupGuid)
		{
			cmd.CommandText = "SELECT pag.* FROM dbo.tblPointAccessGroup pag";
			cmd.CommandText += " JOIN map.tblPointAccessGroupToUserGroup pagug ";
			cmd.CommandText += " ON pag.PointAccessGroupGuid = pagug.PointAccessGroupGuid AND pagug.UserGroupGuid = @UserGroupGuid";
			cmd.CommandText += " WHERE pag.SiteGuid = @SiteGuid ORDER BY ID";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGroupGuid", userGroupGuid);
		}

		public static void GetByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = "SELECT pag.* FROM dbo.tblPointAccessGroup pag WHERE pag.PointAccessGroupGuid = @PointAccessGroupGuid";
			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}

		public static void PurgeBySiteGuidSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToAlarmTest WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToPointTemplate WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToPoint WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToExposedPointSetting WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToExposedPropertySetting WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToTag WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToUserGroup WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM map.tblPointAccessGroupToPointAlarmTest WHERE PointAccessGroupGuid IN (SELECT PointAccessGroupGuid FROM dbo.tblPointAccessGroup WHERE SiteGuid = @SiteGuid)";
			cmd.CommandText += " DELETE FROM dbo.tblPointAccessGroup WHERE siteGuid = @SiteGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}
	}
}
