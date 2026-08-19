
namespace FMBusinessObjects.DataObjects
{
	using Attributes;

	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroupToPointAlarmTestMap : BaseDataObject
	{
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string TagID { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string AlarmID { get; set; }


		[FMPersistedField]
		public Guid PointAccessGroupToPointAlarmTestGuid
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
		public Guid AlarmTestGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid PointGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool View { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Acknowledge { get; set; }

		public static void PurgeByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToPointAlarmTest WHERE PointAccessGroupGuid = @PointAccessGroupGuid";
         cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}


		public static void PurgeBySiteGuidAndPointTemplateGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
			cmd.CommandText = "DELETE pagtpat FROM map.tblPointAccessGroupToPointAlarmTest pagtpat"
									+ " INNER JOIN dbo.tblAlarmTest at ON at.AlarmTestGuid = pagtpat.AlarmTestGuid"
									+ " INNER JOIN dbo.tblAlarm a ON a.AlarmGuid = at.AlarmGuid"
									+ " INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = a.InputTagGuid AND pt.PointTemplateGuid = pointTemplateGuid"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON SiteGuid = @SiteGuid AND pag.PointAccessGroupGuid = pagtat.PointAccessGroupGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = " SET NOCOUNT ON"
									+ " SELECT at.ID, a.ID as AlarmID, at.AlarmTestGuid, pt.ID as TagID, p.PointGuid, p.SiteGuid, pagtpat.PointAccessGroupGuid, pagtpat.PointAccessGroupToPointAlarmTestGuid,"
                                    + " pagtpat.[View],"
                                    + " pagtpat.Acknowledge,"
									+ " pagtpat.CreatedBy, pagtpat.CreatedDate, pagtpat.UpdatedBy, pagtpat.UpdatedDate FROM map.tblPointAccessGroupToPointAlarmTest pagtpat"
                                    + " INNER JOIN dbo.tblAlarmTest at ON at.AlarmTestGuid = pagtpat.AlarmTestGuid"
									+ " INNER JOIN dbo.tblAlarm a ON a.AlarmGuid = at.AlarmGuid"
									+ " INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = a.InputTagGuid"
									+ " INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid"
									+ " WHERE pagtpat.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " ORDER BY at.ID, a.ID, p.ID";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}
