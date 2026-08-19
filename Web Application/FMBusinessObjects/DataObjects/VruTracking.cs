
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    // ReSharper disable once InconsistentNaming
    public enum VRU_INTERVAL_TYPE
    {
        // ReSharper disable once InconsistentNaming
        Minute = 0,
        Hour = 1,
        Day = 2,
        Month = 3,
        Year = 4
    }

    /// <summary>
    /// VRU Tracking Collection Class.
    /// </summary>
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(VruTrackingClass))]
    // ReSharper disable once InconsistentNaming
    public class VRUTrackingCollectionClass : List<VruTrackingClass>
    {
    }

    [Serializable]
    [DataContract]
    // ReSharper disable once InconsistentNaming
    public class VruTrackingClass : BaseDataObject, IAlarmAndEventDiscovery
    {
        private const string VruThresholdExceededAlarmKey = "VRU Threshold Exceeded";

        [DataMember]
        private static readonly AlarmAndEventDescriptorClass VcuEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, "VCU Log");

        [DataMember]
        private static readonly AlarmAndEventDescriptorClass VruThresholdExceededAlarmDescriptor = new AlarmAndEventDescriptorClass(
            true,
            LoadRackKey,
            VruThresholdExceededAlarmKey);

        [DataMember]
        public int Interval { get; set; }

        [DataMember]
        public VRU_INTERVAL_TYPE IntervalType { get; set; }

        [DataMember]
        public SIDouble CurrentValue { get; set; }

        [DataMember]
        public SIDouble Limit { get; set; }

        [DataMember]
        public double? Tolerance { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public DateAndTime ResetDate { get; set; }

        [DataMember]
        public DateAndTime LastCalculationDate { get; set; }

        public const string EntityTypeID = "VCU";

        [DataMember]
        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] descriptors =
                    {
                        VcuEventDescriptor,
                        VruThresholdExceededAlarmDescriptor
                    };

                return descriptors;
            }
        }

        public void InsertSql(SqlCommand cmd)
        {
            cmd.CommandText =   "[dbo].[usp_VruThresholdInsert]";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
            cmd.Parameters.AddWithValue("@ID", this.ID);
            cmd.Parameters.AddWithValue("@Interval", this.Interval);
            cmd.Parameters.AddWithValue("@IntervalType", (int)(this.IntervalType));
            cmd.Parameters.AddWithValue("@Limit", this.Limit.SIValue);
            cmd.Parameters.AddWithValue("@Tolerance", this.Tolerance);
            cmd.Parameters.AddWithValue("@Enabled", this.Enabled);

            SqlParameter param = cmd.Parameters.Add("@ResetDate", SqlDbType.DateTime);
            if (this.ResetDate == null)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = this.ResetDate.UTCValue;
            }

            cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
            cmd.Parameters.AddWithValue("@CreatedBy", this.CreatedBy);
            cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
            cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
        }

        public void UpdateSqlCmd(SqlCommand cmd)
        {
            cmd.CommandText =       "UPDATE tblVRUThresholds " +
                                    "SET Interval = @Interval," +
                                    "ID = @ID," +
                                    "IntervalType = @IntervalType," +
                                    "Limit = @Limit," +
                                    "Tolerance = @Tolerance," +
                                    "Enabled = @Enabled," +
                                    "ResetDate = @ResetDate," +
                                    "UpdatedDate = @UpdatedDate," +
                                    "UpdatedBy = @UpdatedBy " +
                                    "WHERE VRUThresholdGuid = @VRUThresholdGuid";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Interval", this.Interval);
            cmd.Parameters.AddWithValue("@ID", this.ID);
            cmd.Parameters.AddWithValue("@IntervalType", (int)this.IntervalType);
            cmd.Parameters.AddWithValue("@Limit", this.Limit.SIValue);
            cmd.Parameters.AddWithValue("@Tolerance", this.Tolerance);
            cmd.Parameters.AddWithValue("@Enabled", this.Enabled);

            SqlParameter param = cmd.Parameters.Add("@ResetDate", SqlDbType.DateTime);
            if (this.ResetDate == null)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = this.ResetDate.UTCValue;
            }

            cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
            cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
            cmd.Parameters.AddWithValue("@VRUThresholdGuid", this.IdentityGuid);
        }

        public void PurgeSql(SqlCommand cmd)
        {
            cmd.CommandText = "DELETE FROM tblVRUThresholds WHERE VRUThresholdGuid = @VRUThresholdGuid";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@VRUThresholdGuid", this.IdentityGuid);
        }

        public void ResetDateSql(SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE tblVRUThresholds "
                            + "SET ResetDate = GETUTCDATE(),"
                            + "UpdatedDate = @UpdatedDate, "
                            + "UpdatedBy = @UpdatedBy "
                            + "WHERE @VRUThresholdGuid = @VRUThresholdGuid";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
            cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
            cmd.Parameters.AddWithValue("@VRUThresholdGuid", this.IdentityGuid);
        }

        public void CalculateCurrentValueSql(SqlCommand cmd)
        {
            cmd.CommandText = "usp_CalculateVRUTotals";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
        }

        public void AggregateDailyBolSql(SqlCommand cmd)
        {
            cmd.CommandText = "usp_AggDailyBOLByProduct";
            cmd.CommandType = CommandType.StoredProcedure;
        }

        public void GetAutoCalculateScheduleSql(SqlCommand cmd)
        {
            cmd.CommandText =   "SELECT freq_subday_interval " + "FROM msdb..sysschedules "
                                + "WHERE name = 'VRU_calculation_schedule'";
            cmd.Parameters.Clear();
        }

        public void IsThresholdExceededSql(SqlCommand cmd)
        {
            cmd.CommandText =   "SELECT 1 as Threshold_Exceeded " + "WHERE EXISTS ( SELECT 1  "
                                + "	FROM tblVRUThresholds "
                                + "	WHERE CurrentValue > Limit - (Limit * (Tolerance / 100.0)) ) ";
            cmd.Parameters.Clear();
        }

        public AlarmAndEventLogClass VruThresholdExceededAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(VruThresholdExceededAlarmDescriptor)
                {
                    AssociatedData
                                                   = this.ID + " Limit " + this.Limit + " in " + this.Interval + " " + this.IntervalType
                };
                return alarmAndEventLog;
            }
            // ReSharper disable once ValueParameterNotUsed
            // Required for data contract
            set
            {
            }
        }

        public static string IntervalTypeID(VRU_INTERVAL_TYPE type)
        {
            switch (type)
            {
                case VRU_INTERVAL_TYPE.Minute:
                    return "minute";
                case VRU_INTERVAL_TYPE.Hour:
                    return "hour";
                case VRU_INTERVAL_TYPE.Day:
                    return "day";
                case VRU_INTERVAL_TYPE.Month:
                    return "month";
                case VRU_INTERVAL_TYPE.Year:
                    return "year";
                default:
                    return "Undefined";
            }
        }

        public void SelectSql(SqlCommand cmd)
        {
            cmd.CommandText =           "SELECT * " + 
                                        "FROM tblVRUThresholds v " + 
                                        " WHERE v.VRUThresholdGuid = @VRUThresholdGuid" + 
                                        " AND v.SiteGuid = @SiteGuid";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@VRUThresholdGuid", this.IdentityGuid);
            cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
        }

        public void Load(DataSet set, SecurityClass security, SiteClass site)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            DataTable table = set.Tables[0];
            if (table.Rows.Count == 0)
            {
                return;
            }

            DataRow row = table.Rows[0];

            //var site = new SiteClass(false) { ID = security.SiteID };
            //site.Load(new ConsolidatedDAClass().GetDataSet(site.SelectByIDSQLParameterized(ContextUtil.IsInTransaction), security));

            this.IdentityGuid = DataObject.getValue(row["VRUThresholdGuid"], Guid.Empty);
            this.ID = DataObject.getValue(row["ID"], string.Empty);
            this.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
            this.Interval = DataObject.getValue(row["Interval"], 0);
            this.IntervalType = (VRU_INTERVAL_TYPE)(DataObject.getValue(row["IntervalType"], 0));

            double limit = row.IsNull("Limit") ? 0 : double.Parse(row["Limit"].ToString());
            this.Limit = new SIDouble(
                                    site.GetSiteUnits(SITE_VARIABLE_TYPE.VOLUME),
                                    site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME),
                                    limit);
            this.Tolerance = row.IsNull("Tolerance") ? (double?)null : double.Parse(row["Tolerance"].ToString());
            this.Enabled = (!row.IsNull("Enabled")) && Convert.ToBoolean(row["Enabled"].ToString());
            var resetDate = new DateAndTime(site);
            if (!row.IsNull("ResetDate"))
            {
                resetDate.UTCValue = DateTime.Parse(row["ResetDate"].ToString());
                this.ResetDate = resetDate;
            }
            else
            {
                this.ResetDate = null;
            }

            double currentValue = row.IsNull("CurrentValue") ? 0 : double.Parse(row["CurrentValue"].ToString());
            this.CurrentValue = new SIDouble(
                                    site.GetSiteUnits(SITE_VARIABLE_TYPE.VOLUME),
                                    site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME),
                                    currentValue);
            this.CreatedDate = (DateTimeOffset)row["CreatedDate"];
            this.CreatedBy = (string)row["CreatedBy"];
            this.UpdatedDate = (DateTimeOffset)row["UpdatedDate"];
            this.UpdatedBy = (string)row["UpdatedBy"];
            var lastCalculationDate = new DateAndTime(site);
            if (!row.IsNull("LastCalculationDate"))
            {
                lastCalculationDate.UTCValue = DateTime.Parse(row["LastCalculationDate"].ToString());
                this.LastCalculationDate = lastCalculationDate;
            }
            else
            {
                this.LastCalculationDate = null;
            }
        }

        public void EnumerateSql(SecurityClass security, SqlCommand cmd)
        {
            cmd.CommandText = "SELECT * " + 
                                    "FROM tblVRUThresholds v " + 
                                    " WHERE v.SiteGuid = @SiteGuid";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
        }

        public void EnumerateThresholdExceededSql(SecurityClass security, SqlCommand cmd)
        {
            cmd.CommandText = "SELECT * " +
                                 "FROM tblVRUThresholds v " +
                                 " WHERE v.SiteGuid = @SiteGuid" +
                                 " AND v.CurrentValue > Limit - (Limit * (Tolerance / 100.0))";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
        }
    }
}
