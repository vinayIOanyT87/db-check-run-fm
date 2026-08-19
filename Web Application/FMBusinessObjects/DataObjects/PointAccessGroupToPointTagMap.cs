namespace FMBusinessObjects.DataObjects
{
	using Attributes;

	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroupToPointTagMap : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointAccessGroupToPointTagGuid
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
		public Guid PointGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid PointTagGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool View { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Modify { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool ExceedRange { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Override { get; set; }


		public static void PurgeByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToPointTag WHERE PointAccessGroupGuid = @PointAccessGroupGuid";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}

		public static void PurgeByPointTagGuidSQL(SqlCommand cmd, Guid pointTagGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToPointTag pagtt"
									+ " WHERE pagtt.TagGuid = @PointTemplateTagGuid";

			cmd.Parameters.AddWithValue("@PointTagGuid", pointTagGuid);
		}

		public static void PurgeBySiteGuidAndPointGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointGuid)
		{
			cmd.CommandText = "DELETE pagtpt FROM map.tblPointAccessGroupToPointTag pagtpt"
									+ " INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = pagtpt.TagGuid AND pt.PointTemplateGuid = @PointGuid"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON pag.SiteGuid = @SiteGuid AND pag.PointAccessGroupGuid = pagtt.PointAccessGroupGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
		}



		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText += "SET NOCOUNT ON"
									+ " SELECT pt.ID, pt.PointTagGuid, pt.PointGuid, p.SiteGuid, pagtpt.PointAccessGroupGuid, pagtpt.PointAccessGroupToPointTagGuid,"
									+ " pagtpt.[View],"
									+ " pagtpt.Modify,"
									+ " pagtpt.ExceedRange,"
									+ " pagtpt.Override,"
									+ " pagtpt.CreatedBy, pagtpt.CreatedDate, pagtpt.UpdatedBy, pagtpt.UpdatedDate FROM map.tblPointAccessGroupToPointTag pagtpt"
									+ " INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = pagtpt.TagGuid "
									+ " INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid"
									+ " WHERE pagtpt.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " ORDER BY pt.ID, p.ID";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}
