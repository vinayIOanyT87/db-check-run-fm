
namespace FMBusinessObjects.DataObjects
{
	using Attributes;

	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroupToAlarmTestMap : BaseDataObject
	{
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string TagID { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string AlarmID { get; set; }


		[FMPersistedField]
		public Guid PointAccessGroupToAlarmTestGuid
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
		public Guid AlarmTestTemplateGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool View { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Acknowledge { get; set; }

		public static void PurgeByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToAlarmTest WHERE PointAccessGroupGuid = @PointAccessGroupGuid";
         cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}


		public static void PurgeBySiteGuidAndPointTemplateGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
			cmd.CommandText = "DELETE pagtat FROM map.tblPointAccessGroupToAlarmTest pagtat"
									+ " INNER JOIN dbo.tblAlarmTestTemplate att ON att.AlarmTestTemplateGuid = pagtat.AlarmTestGuid"
									+ " INNER JOIN dbo.tblAlarmTemplate at ON at.AlarmTemplateGuid = att.AlarmTemplateGuid"
									+ " INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateGuid = at.InputTemplateTagGuid AND ptt.PointTemplateGuid = @PointTemplateGuid"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON SiteGuid = @SiteGuid AND pag.PointAccessGroupGuid = pagtat.PointAccessGroupGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText = " SET NOCOUNT ON"
									+ " SELECT att.ID, at.ID as AlarmID, att.AlarmTestTemplateGuid, ptt.ID as TagID, pt.PointTemplateGuid, pt.SiteGuid, pagtat.PointAccessGroupGuid, pagtat.PointAccessGroupToAlarmTestGuid,"
									+ " pagtat.[View],"
									+ " pagtat.Acknowledge,"
									+ " pagtat.CreatedBy, pagtat.CreatedDate, pagtat.UpdatedBy, pagtat.UpdatedDate FROM map.tblPointAccessGroupToAlarmTest pagtat"
									+ " INNER JOIN dbo.tblAlarmTestTemplate att ON att.AlarmTestTemplateGuid = pagtat.AlarmTestGuid"
									+ " INNER JOIN dbo.tblAlarmTemplate at ON at.AlarmTemplateGuid = att.AlarmTemplateGuid"
									+ " INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateTagGuid = at.InputTemplateTagGuid"
									+ " INNER JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid"
									+ " WHERE pagtat.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " ORDER BY att.ID, at.ID, pt.ID";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}
