
namespace FMBusinessObjects.DataObjects
{
	using Attributes;

	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroupToPointTemplateMap : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointAccessGroupToPointTemplateGuid
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
		public Guid PointTemplateGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public bool Assigned { get; set; }



		public static void PurgeByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToPointTemplate WHERE PointAccessGroupGuid = @PointAccessGroupGuid";
			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}

		public static void PurgeBySiteGuidAndPointTemplateGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
			cmd.CommandText = "DELETE pagtpt FROM map.tblPointAccessGroupToPointTemplate pagtpt"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON pag.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid AND pag.SiteGuid = @SiteGuid"
									+ " WHERE pagtpt.PointTemplateGuid = @PointTemplateGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}


		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText	+= "SET NOCOUNT ON"
									+ " SELECT pt.ID, pt.PointTemplateGuid, pt.SiteGuid, pagtpt.PointAccessGroupGuid, pagtpt.PointAccessGroupToPointTemplateGuid,"
									+ " CAST(1 AS BIT) Assigned,"
									+ " pagtpt.CreatedBy, pagtpt.CreatedDate, pagtpt.UpdatedBy, pagtpt.UpdatedDate FROM map.tblPointAccessGroupToPointTemplate pagtpt"
									+ " INNER JOIN dbo.tblPointTemplate pt ON  pt.PointTemplateGuid = pagtpt.PointTemplateGuid"
									+ " WHERE pagtpt.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " ORDER BY pt.ID";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}
