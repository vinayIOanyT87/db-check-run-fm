namespace FMBusinessObjects.DataObjects
{
	using Attributes;

	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroupToUserGroupMap : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointAccessGroupToUserGroupGuid
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
		[FMPersistedField]
		public Guid PointAccessGroupGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid UserGroupGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public bool Assigned { get; set; }

		public static void PurgeByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToUserGroup WHERE PointAccessGroupGuid = @PointAccessGroupGuid";
			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}

		public static void PurgeBySiteGuidAndUserGroupGuidSQL(SqlCommand cmd, Guid siteGuid, Guid userGroupGuid)
		{
			cmd.CommandText = "DELETE pagtug FROM map.tblPointAccessGroupToUserGroup pagtug"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON pag.PointAccessGroupGuid = pagtug.PointAccessGroupGuid AND pag. SiteGuid = @SiteGuid"
									+ " WHERE UserGroupGuid = @UserGroupGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGroupGuid", userGroupGuid);
		}


		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText	+= "SET NOCOUNT ON"
									+ " SELECT ug.GroupID as ID, ug.GroupGuid as UserGroupGuid, ug.SiteGuid, pagtug.PointAccessGroupGuid, pagtug.PointAccessGroupToUserGroupGuid,"
									+ " CASE WHEN pagtug.PointAccessGroupToUserGroupGuid IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS Assigned,"
									+ " pagtug.CreatedBy, pagtug.CreatedDate, pagtug.UpdatedBy, pagtug.UpdatedDate FROM map.tblPointAccessGroupToUserGroup pagtug"
									+ " INNER JOIN dbo.tblGroups ug ON pagtug.UserGroupGuid = ug.GroupGuid"
									+ " WHERE pagtug.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " ORDER BY ug.GroupID";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}
