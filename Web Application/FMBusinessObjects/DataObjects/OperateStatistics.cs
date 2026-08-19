
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading.Tasks;

	public class OperateStatistics
	{
		public string WindowName { get; set; }
		public DateTimeOffset OperateActiveStartTime { get; set; }
		public DateTimeOffset OperateActiveStopTime { get; set; }

		public int AvgMinuteTimeAlarmNotifications { get; set; }
		public int MaxMinuteTimeAlarmNotifications { get; set; }
		public int AvgSessionTimeAlarmNotifications { get; set; }
		public int MaxSessionTimeAlarmNotifications { get; set; }

		public int AvgMinuteTimeAlarmRefresh { get; set; }
		public int MaxMinuteTimeAlarmRefresh { get; set; }
		public int AvgSessionTimeAlarmRefresh { get; set; }
		public int MaxSessionTimeAlarmRefresh { get; set; }

		public int AvgMinuteTimeUpdateValues { get; set; }
		public int MaxMinuteTimeUpdateValues { get; set; }
		public int AvgSessionTimeUpdateValues { get; set; }
		public int MaxSessionTimeUpdateValues { get; set; }

		public int AvgMinuteTimeDynamicPointGroup { get; set; }
		public int MaxMinuteTimeDynamicPointGroup { get; set; }
		public int AvgSessionTimeDynamicPointGroup { get; set; }
		public int MaxSessionTimeDynamicPointGroup { get; set; }

		public void OperateStatisticsSaveSQL(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_OperateStatisticsSave";
			cmd.Parameters.AddWithValue("@WindowName", this.WindowName);
			cmd.Parameters.AddWithValue("@AvgMinuteTimeAlarmNotifications", this.AvgMinuteTimeAlarmNotifications);
			cmd.Parameters.AddWithValue("@MaxMinuteTimeAlarmNotifications", this.MaxMinuteTimeAlarmNotifications);
			cmd.Parameters.AddWithValue("@AvgSessionTimeAlarmNotifications", this.AvgSessionTimeAlarmNotifications);
			cmd.Parameters.AddWithValue("@MaxSessionTimeAlarmNotifications", this.MaxSessionTimeAlarmNotifications);
			cmd.Parameters.AddWithValue("@AvgMinuteTimeAlarmRefresh", this.AvgMinuteTimeAlarmRefresh);
			cmd.Parameters.AddWithValue("@MaxMinuteTimeAlarmRefresh", this.MaxMinuteTimeAlarmRefresh);
			cmd.Parameters.AddWithValue("@AvgSessionTimeAlarmRefresh", this.AvgSessionTimeAlarmRefresh);
			cmd.Parameters.AddWithValue("@MaxSessionTimeAlarmRefresh", this.MaxSessionTimeAlarmRefresh);
			cmd.Parameters.AddWithValue("@AvgMinuteTimeUpdateValues", this.AvgMinuteTimeUpdateValues);
			cmd.Parameters.AddWithValue("@MaxMinuteTimeUpdateValues", this.MaxMinuteTimeUpdateValues);
			cmd.Parameters.AddWithValue("@AvgSessionTimeUpdateValues", this.AvgSessionTimeUpdateValues);
			cmd.Parameters.AddWithValue("@MaxSessionTimeUpdateValues", this.MaxSessionTimeUpdateValues);
			cmd.Parameters.AddWithValue("@AvgMinuteTimeDynamicPointGroup", this.AvgMinuteTimeDynamicPointGroup);
			cmd.Parameters.AddWithValue("@MaxMinuteTimeDynamicPointGroup", this.MaxMinuteTimeDynamicPointGroup);
			cmd.Parameters.AddWithValue("@AvgSessionTimeDynamicPointGroup", this.AvgSessionTimeDynamicPointGroup);
			cmd.Parameters.AddWithValue("@MaxSessionTimeDynamicPointGroup", this.MaxSessionTimeDynamicPointGroup);
			cmd.Parameters.AddWithValue("@CreatedDate", DateTimeOffset.UtcNow);
			cmd.Parameters.AddWithValue("@CreatedBy", security.UserID);
			cmd.Parameters.AddWithValue("@UpdatedDate", DateTimeOffset.UtcNow);
			cmd.Parameters.AddWithValue("@UpdatedBy", security.UserID);
			cmd.Parameters.AddWithValue("@SessionGuid", security.Token);
		}

		public static void GetActiveOperateScreensListSQL(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = "SELECT os.*, u.UserID, cs.ID,  s.ClientIpAddress, s.WebServerIpAddress "
									+ " FROM dbo.tblOperateStatistics os "
									+ " LEFT JOIN dbo.tblSessions s ON s.SessionGuid = os.SessionGuid"
									+ " LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid"
									+ " LEFT JOIN dbo.tblSites cs ON cs.SiteGuid = s.SiteGuid"
									+ " LEFT JOIN dbo.tblSites ls ON ls.SiteGuid = s.LoginSiteGuid"
									+ " WHERE s.SiteGuid = @SiteGuid AND os.OperateActiveStopTime IS NULL"
									+ " ORDER BY s.ClientIpAddress, os.WindowName";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
		}

		public static void GetActiveOperateScreenCountSQL(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = "select count(1) from dbo.tblOperateStatistics where OperateActiveStopTime IS NULL";
		}

		public static void GetActivateOperateScreenSQL(SecurityClass security, string windowName, bool usingOperate, SqlCommand cmd)
		{
			if (usingOperate == true)
			{
				cmd.CommandText = "UPDATE tblOperateStatistics SET OperateActiveStartTime = SYSDATETIMEOFFSET(), OperateActiveStopTime = NULL WHERE SessionGuid = @SessionGuid AND WindowName = @WindowName";
			}
			else
			{
				cmd.CommandText = "UPDATE tblOperateStatistics SET OperateActiveStopTime = SYSDATETIMEOFFSET() WHERE SessionGuid = @SessionGuid AND WindowName = @WindowName";
			}

			cmd.Parameters.AddWithValue("@SessionGuid", security.Token);
			cmd.Parameters.AddWithValue("@WindowName", windowName);
		}
	}
}
