namespace FMBusinessObjects.DataObjects
{
	using Attributes;

	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class PointAccessGroupToPointMap : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointAccessGroupToPointGuid
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
		[FMPersistedField(ReadOnly = true)]
		public Guid PointTemplateGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid PointGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public bool Assigned { get; set; }


		public static void PurgeByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToPoint WHERE PointAccessGroupGuid = @PointAccessGroupGuid";
			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}

		public static void PurgeByPointGuidSQL(SqlCommand cmd, Guid pointGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToPoint WHERE PointGuid = @PointGuid";
			cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
		}

		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText	+= "SET NOCOUNT ON"
									+ " SELECT p.ID, p.PointGuid, p.PointTemplateGuid, p.SiteGuid, pagtp.PointAccessGroupGuid, pagtp.PointAccessGroupToPointGuid,"
									+ " CAST(1 AS BIT) Assigned,"
									+ " pagtp.CreatedBy, pagtp.CreatedDate, pagtp.UpdatedBy, pagtp.UpdatedDate FROM map.tblPointAccessGroupToPoint pagtp"
									+ " LEFT JOIN dbo.tblPoint p ON  p.PointGuid = pagtp.PointGuid"
									+ " WHERE pagtp.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " ORDER BY p.ID";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}

