namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using FMBusinessObjects.DataObjects.CodedVariables;


	[Serializable]
	public class TrendCollection : List<Trend>
	{
	}

	[KnownType(typeof(TrendPen))]
	[DataContract]
	[Serializable]
	public class Trend : BaseDataObject
	{
		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid TrendGuid
		{
			get
			{
				return this.IdentityGuid;
			}
			set
			{
				this.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public Guid PointTemplateGuid { get; set; }

		[DataMember]
		[FMPersistedField(LiteralEnum=true)]
		public TrendModeEnum Mode { get; set; }

		[DataMember]
		[FMPersistedField(LiteralEnum=true)]
		public TrendPeriodType PeriodType { get; set; }

		[DataMember]
		[FMPersistedField]
		public double Period { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset Start { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset End { get; set; }

		[DataMember]
		public List<TrendPen> Pens { get; set; }

		public Trend()
		{
			
			this.Pens = new List<TrendPen>();
			this.Mode = TrendModeEnum.Realtime;
			this.PeriodType = TrendPeriodType.Minutes;
			this.Period = 60;
		this.End = DateTimeOffset.Now.Date;
			this.Start = DateTimeOffset.Now.Date.AddDays(-1);
			
		}


		public static void SelectSQL(SqlCommand cmd, Guid trendGuid)
		{
			cmd.CommandText = "SELECT * FROM dbo.tblTrend WHERE TrendGuid = @TrendGuid"
									+ " IF (SELECT PointTemplateGuid FROM dbo.tblTrend WHERE TrendGuid = @TrendGuid) IS NULL"
									+ " SELECT tptpt.TrendPenToPointTrendGuid AS TrendPenToTrendGuid, tptpt.PointTagGuid, tptpt.TrendGuid, tptpt.PenColor,  tptpt.CreatedDate, tptpt.CreatedBy, tptpt.UpdatedDate, tptpt.UpdatedBy, p.ID AS PointID, p.PointGuid, pt.ID AS TagID, pt.Maximum, pt.Minimum, pt.ValueType, pt.EngineeringUnitsType, pt.EngineeringUnitsIndex AS Units, pt.DecimalPlaces"
									+ " FROM map.tblTrendPenToPointTrend tptpt"
									+ " LEFT JOIN tblPointTag pt ON pt.PointTagGuid = tptpt.PointTagGuid"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = pt.PointGuid"
									+ " WHERE TrendGuid = @TrendGuid"
									+ " ORDER BY PointID, TagID"
									+ " ELSE"
									+ " SELECT tptdt.TrendPenToDetailTrendGuid AS TrendPenToTrendGuid, tptdt.PointTemplateTagGuid, tptdt.TrendGuid, tptdt.PenColor,  tptdt.CreatedDate, tptdt.CreatedBy, tptdt.UpdatedDate, tptdt.UpdatedBy, pt.ID AS PointID, pt.PointTemplateGuid AS PointGuid, ptt.ID AS TagID, ptt.Maximum, ptt.Minimum, ptt.ValueType, ptt.EngineeringUnitsType, ptt.EngineeringUnitsIndex AS Units, ptt.DecimalPlaces"
									+ " FROM map.tblTrendPenToDetailTrend tptdt"
									+ " LEFT JOIN tblPointTemplateTag ptt ON ptt.PointTemplateTagGuid = tptdt.PointTemplateTagGuid"
									+ " LEFT JOIN tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid"
									+ " WHERE TrendGuid = @TrendGuid"
									+ " ORDER BY PointID, TagID";

			cmd.Parameters.AddWithValue("@TrendGuid", trendGuid);
		}

		public static void SelectByPointSQL(SqlCommand cmd, Guid siteGuid, Guid pointGuid)
		{
			cmd.CommandText = "SELECT p.ID, p.Description,t.Mode, t.PeriodType, t.Period, t.Start, t.[End], t.CreatedDate, t.CreatedBy, t.UpdatedDate, t.UpdatedBy, t.TrendGuid, t.SiteGuid, t.PointTemplateGuid FROM dbo.tblTrend t"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = @PointGuid"
									+ " WHERE t.SiteGuid = @SiteGuid AND t.PointTemplateGuid = (SELECT PointTemplateGuid FROM tblPoint WHERE PointGuid = @PointGuid)"
									+ " SELECT tptdt.TrendPenToDetailTrendGuid AS TrendPenToTrendGuid, tptdt.PointTemplateTagGuid, tptdt.TrendGuid, tptdt.PenColor, tptdt.CreatedDate, tptdt.CreatedBy, tptdt.UpdatedDate, tptdt.UpdatedBy, p.ID AS PointID, p.PointGuid, pt.ID AS TagID, pt.PointTagGuid AS PointTagGuid, pt.Maximum, pt.Minimum, pt.ValueType, pt.EngineeringUnitsType, pt.EngineeringUnitsIndex AS Units, pt.DecimalPlaces"
									+ " FROM map.tblTrendPenToDetailTrend tptdt"
									+ " LEFT JOIN tblPointTag pt ON pt.PointTemplateTagGuid = tptdt.PointTemplateTagGuid AND pt.PointGuid = @PointGuid"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = @PointGuid"
									+ " WHERE TrendGuid = (SELECT TrendGuid FROM dbo.tblTrend WHERE SiteGuid = @SiteGuid AND PointTemplateGuid = (SELECT PointTemplateGuid FROM tblPoint WHERE PointGuid = @PointGuid))"
									+ " ORDER BY PointID, TagID";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
		}


		public static void SelectTrendGuidByIdSQL(SqlCommand cmd, Guid siteGuid, string id)
		{
			cmd.CommandText = "SELECT TrendGuid FROM tblTrend WHERE SiteGuid = @SiteGuid AND UPPER(ID) = UPPER(@ID)";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@ID", id);
		}

		public static void SelectTrendGuidByPointTemplateGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SELECT TrendGuid FROM tblTrend WHERE SiteGuid = @SiteGuid AND PointTemplateGuid = @PointTemplateGuid";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		public static void CanCreatePointTrendSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "select CASE WHEN dbo.udf_IsEnterprise() = Enterprise THEN 1 ELSE 0 END as valid FROM tblsites WHERE siteguid = @siteguid";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}
	}
}
