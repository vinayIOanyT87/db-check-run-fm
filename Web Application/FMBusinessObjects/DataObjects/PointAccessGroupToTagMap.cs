using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
   using Attributes;

	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroupToTagMap : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointAccessGroupToTagGuid
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
		public Guid PointTemplateTagGuid { get; set; }

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
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToTag WHERE PointAccessGroupGuid = @PointAccessGroupGuid";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}

		public static void PurgeByPointTemplateTagGuidSQL(SqlCommand cmd, Guid pointTemplateTagGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblPointAccessGroupToTag pagtt"
									+ " WHERE pagtt.TagGuid = @PointTemplateTagGuid";

			cmd.Parameters.AddWithValue("@PointTemplateTagGuid", pointTemplateTagGuid);
		}

		public static void PurgeBySiteGuidAndPointTemplateGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
			cmd.CommandText = "DELETE pagtt FROM map.tblPointAccessGroupToTag pagtt"
									+ " INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateTagGuid = pagtt.TagGuid AND ptt.PointTemplateGuid = @PointTemplateGuid"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON pag.SiteGuid = @SiteGuid AND pag.PointAccessGroupGuid = pagtt.PointAccessGroupGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}



		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText	+= "SET NOCOUNT ON"
									+ " SELECT ptt.ID, ptt.PointTemplateTagGuid, pt.PointTemplateGuid, pt.SiteGuid, pagtt.PointAccessGroupGuid, pagtt.PointAccessGroupToTagGuid,"
									+ " pagtt.[View],"
									+ " pagtt.Modify,"
									+ " pagtt.ExceedRange,"
									+ " pagtt.Override,"
									+ " pagtt.CreatedBy, pagtt.CreatedDate, pagtt.UpdatedBy, pagtt.UpdatedDate FROM map.tblPointAccessGroupToTag pagtt"
									+ " INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateTagGuid = pagtt.TagGuid "
									+ " INNER JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid"
									+ " WHERE pagtt.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " ORDER BY ptt.ID, pt.ID";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}
