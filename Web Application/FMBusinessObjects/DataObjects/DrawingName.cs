namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;

	[DataContract]
	[Serializable]
	public class DrawingName
	{
		[DataMember]
		[FMPersistedField]
		public Guid DrawingGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public string ID { get; set; }

		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }

		[DataMember]
		[FMPersistedField]
		public PANELTYPE PanelType { get; set; }


		[DataMember]
		[FMPersistedField]
		public string PointTemplateName { get; set; }

		[DataMember]
		[FMPersistedField]
		public string PointTemplateType { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset UpdatedDate { get; set; }

		[DataMember]
		[FMPersistedField]
		public string UpdatedBy { get; set; }

		static public void EnumerateAvailableDrawingNames(SqlCommand cmd, SecurityClass security, Guid siteGuid)
		{
			cmd.CommandText = "SELECT d.DrawingGuid, d.ID, d.[Description], d.PanelType, tpt.ID as PointTemplateName, tas.ID as PointTemplateType,  d.UpdatedDate, d.UpdatedBy "
                            + "FROM tblDrawings d "
                            + "LEFT OUTER JOIN tblPointTemplate tpt ON tpt.PointTemplateGuid = d.PointTemplateGuid "
                            + "LEFT OUTER JOIN tblApplicationString tas on tpt.PointTemplateTypeApplicationStringGuid = tas.ApplicationStringGuid "
                            + "WHERE d.SiteGuid = @SiteGuid ORDER BY[ID]";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		static public void EnumerateByDrawingGuidListSQL(SqlCommand cmd, List<Guid> drawingGuidList)
		{
			cmd.CommandText = "SELECT d.DrawingGuid, d.ID, d.[Description], d.PanelType, tpt.ID as PointTemplateName, tas.ID as PointTemplateType,  d.UpdatedDate, d.UpdatedBy "
                            + "FROM tblDrawings d "
                            + "LEFT OUTER JOIN tblPointTemplate tpt ON tpt.PointTemplateGuid = d.PointTemplateGuid "
                            + "LEFT OUTER JOIN tblApplicationString tas on tpt.PointTemplateTypeApplicationStringGuid = tas.ApplicationStringGuid "
                            + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = d.DrawingGuid"
                            + " ORDER BY DrawingGuid";
			BaseDataObject.GenerateGuidListTable(cmd, drawingGuidList);
		}

		static public void EnumerateAvailableDrawingNamesByPanelType(SqlCommand cmd, SecurityClass security, List<PANELTYPE> paneltypes)
		{
			cmd.CommandText = "SELECT d.DrawingGuid, d.ID, d.[Description], d.PanelType, tpt.ID as PointTemplateName, tas.ID as PointTemplateType,  d.UpdatedDate, d.UpdatedBy "
									 + "FROM tblDrawings d "
									 + "LEFT OUTER JOIN tblPointTemplate tpt ON tpt.PointTemplateGuid = d.PointTemplateGuid "
									 + "LEFT OUTER JOIN tblApplicationString tas on tpt.PointTemplateTypeApplicationStringGuid = tas.ApplicationStringGuid "
									 + "WHERE d.SiteGuid = @SiteGuid AND d.PanelType IN (Select [value] from @PanelTypes) ORDER BY [ID]";
			var panelTypeTable = new DataTable();
			panelTypeTable.Columns.Add(new DataColumn("value", typeof(int)));
			foreach (var panelType in paneltypes)
			{
				var row = panelTypeTable.NewRow();
				row["value"] = (int)panelType;
				panelTypeTable.Rows.Add(row);
			}
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			var param = cmd.Parameters.AddWithValue("@PanelTypes", panelTypeTable);
			param.SqlDbType = SqlDbType.Structured;
			param.TypeName = "dbo.IntegerListType";
		}

		static public void EnumerateAvailableDrawingNamesByPointTemplate(SqlCommand cmd, SecurityClass security, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SELECT DISTINCT d.DrawingGuid, d.ID, d.[Description], d.PanelType, tpt.ID as PointTemplateName, tas.ID as PointTemplateType,  d.UpdatedDate, d.UpdatedBy "
									+ "FROM tblDrawings d "
									+ "INNER JOIN map.tblEntityPointTemplateToSite eptts ON eptts.PointTemplateGuid = d.PointTemplateGuid "
									+ "INNER JOIN tblPointTemplate tpt ON tpt.PointTemplateGuid = d.PointTemplateGuid "
									+ "INNER JOIN tblApplicationString tas on tpt.PointTemplateTypeApplicationStringGuid = tas.ApplicationStringGuid "
									+ "INNER JOIN[dbo].[udf_GetSiteToSiteHierarchyListForSiteGuid](@SiteGuid, 0, 0, 0, 1, 0, 0) h ON h.SiteGuid = d.SiteGuid "
									+ "WHERE d.PointTemplateGuid = @PointTemplateGuid AND eptts.SiteGuid = @SiteGuid ORDER BY [ID]";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		static public void EnumerateAllAvailableDrawingNamesByPointTemplate(SqlCommand cmd, SecurityClass security, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SELECT d.DrawingGuid, d.ID, d.[Description], d.PanelType, tpt.ID as PointTemplateName, tas.ID as PointTemplateType,  d.UpdatedDate, d.UpdatedBy "
								 + "FROM tblDrawings d "
								 + "INNER JOIN tblPointTemplate tpt ON tpt.PointTemplateGuid = d.PointTemplateGuid "
								 + "INNER JOIN tblApplicationString tas on tpt.PointTemplateTypeApplicationStringGuid = tas.ApplicationStringGuid "
								 + "WHERE d.PointTemplateGuid = @PointTemplateGuid ORDER BY [ID]";
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		static public void EnumerateAvailableDrawingNamesByPublished(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT d.DrawingGuid, d.ID, d.[Description], d.PanelType, tpt.ID as PointTemplateName, tas.ID as PointTemplateType,  d.UpdatedDate, d.UpdatedBy "
									 + "FROM tblDrawings d "
									 + "LEFT OUTER JOIN tblPointTemplate tpt ON tpt.PointTemplateGuid = d.PointTemplateGuid "
									 + "LEFT OUTER JOIN tblApplicationString tas on tpt.PointTemplateTypeApplicationStringGuid = tas.ApplicationStringGuid "
									 + "WHERE d.SiteGuid = @SiteGuid AND d.PanelType = 0";

			if(!security.HasRight(RIGHT.OPERATE_VIEW_UNPUBLISHED))
			{
				cmd.CommandText += " AND d.Published = 1 ";
			}

			cmd.CommandText += " ORDER BY [ID]";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}
	}
}
