
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Web.Script.Serialization;
	using System.Xml;
	using System.Xml.Serialization;

	[DataContract]
	[Serializable]
	public class AnimationToDrawingMapClass : BaseDataObject
	{
		[DataMember]
		public new Guid SiteGuid
		{
			get
			{
				return base.SiteGuid;
			}
			set
			{
				base.SiteGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public Guid AnimationToDrawingGuid
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
		public Guid AnimationGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid DrawingGuid { get; set; }

		public static void EnumerateByDrawingGuidListSQL(SqlCommand cmd, List<Guid> drawingGuidList)
		{
			cmd.CommandText = "SELECT a.* from map.tblAnimationToDrawing a"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.DrawingGuid"
								 + " ORDER BY AnimationToDrawingGuid";
			GenerateGuidListTable(cmd, drawingGuidList);
		}

		public static void EnumerateByAnimationGuidListSQL(SqlCommand cmd, List<Guid> animationGuidList)
		{
			cmd.CommandText = "SELECT a.* from map.tblAnimationToDrawing a"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AnimationGuid"
								 + " ORDER BY AnimationToDrawingGuid";
			GenerateGuidListTable(cmd, animationGuidList);
		}

		public static void EnumerateByAnimationToDrawingGuidListSQL(SqlCommand cmd, List<Guid> animationToDrawingGuidList)
		{
			cmd.CommandText = "SELECT a.* from map.tblAnimationToDrawing a"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AnimationToDrawingGuid"
								 + " ORDER BY AnimationToDrawingGuid";
			GenerateGuidListTable(cmd, animationToDrawingGuidList);
		}

		public static void DeleteListSQL(SqlCommand cmd, List<Guid> animationToDrawingGuidList)
		{
			cmd.CommandText = "DELETE a FROM map.tblAnimationToDrawing a"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AnimationToDrawingGuid";
			GenerateGuidListTable(cmd, animationToDrawingGuidList);
		}

		public static void DeleteByDrawingListSQL(SqlCommand cmd, List<Guid> drawingGuidList)
		{
			cmd.CommandText = "DELETE a FROM map.tblAnimationToDrawing a"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.DrawingGuid";
			GenerateGuidListTable(cmd, drawingGuidList);
		}

		public static void DeleteByAnimationListSQL(SqlCommand cmd, List<Guid> animationGuidList)
		{
			cmd.CommandText = "DELETE a FROM map.tblAnimationToDrawing a"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AnimationGuid";
			GenerateGuidListTable(cmd, animationGuidList);
		}

		protected static DataTable CreateAnimationToDrawingListDataTable(List<AnimationToDrawingMapClass> animationToDrawingList, SecurityClass security)
		{

			var table = new DataTable();
			table.Columns.Add("AnimationToDrawingGuid", typeof(Guid));
			table.Columns.Add("AnimationGuid", typeof(Guid));
			table.Columns.Add("DrawingGuid", typeof(Guid));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var animationToDrawing in animationToDrawingList)
			{
				var row = table.NewRow();
				row["AnimationToDrawingGuid"] = animationToDrawing.AnimationToDrawingGuid;
				row["AnimationGuid"] = animationToDrawing.AnimationGuid;
				row["DrawingGuid"] = animationToDrawing.DrawingGuid;
				row["UpdatedBy"] = security.UserID;
				table.Rows.Add(row);
			}

			return table;
		}

		public static void AddModifyStoredProcedure(SqlCommand cmd, List<AnimationToDrawingMapClass> animationToDrawingList, SecurityClass security, bool enableAdd, bool enableModify, bool enableDelete)
		{
			if (animationToDrawingList == null || animationToDrawingList.Count < 1)
			{
				return;
			}
			var table = CreateAnimationToDrawingListDataTable(animationToDrawingList, security);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_AnimationToDrawingAddModify";

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@MapAnimationToDrawingTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "map.MapAnimationToDrawingDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
			cmd.Parameters.AddWithValue("@EnableDelete", enableDelete);
		}

	}
}
