using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	public class AlarmAndEventLogCollectionClass : List<AlarmAndEventLogClass> { }

	[DataContract]
	[Serializable]
	[QueryWriterTopic(typeof(AlarmAndEventLogClass), "Alarm & Event Log", SupportsArchiveQuery = true)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_ALARM_EVENT_LOGS)]
	public class AlarmAndEventLogClass : BaseDataObject
	{
		[QueryWriterField("Sequence Number")]
		[DataMember]
		public long SequenceNumber { get; set; }

		[QueryWriterField("Source")]
		[DataMember]
		public string Source { get; set; }

		[QueryWriterField("Alarm")]
		[DataMember]
		public bool Alarm { get; set; }

		[QueryWriterField("Associated Data")]
		[DataMember]
		public string AssociatedData { get; set; }

		[QueryWriterField("Category ID")]
		[DataMember]
		public string CategoryID { get; set; }

		[QueryWriterField("Priority ID")]
		[DataMember]
		public string PriorityID { get; set; }

		[QueryWriterField("Acknowledged")]
		[DataMember]
		public bool Acknowledged { get; set; }

		[QueryWriterField("Created By", "CreatedBy")]
		public string CreatedByQuery => this.CreatedBy;

	    [QueryWriterField("Created Date", "CreatedDate")]
		public DateTimeOffset CreatedDateQuery => this.CreatedDate;

	    [QueryWriterField("Updated By", "UpdatedBy")]
		public string UpdatedByQuery => this.UpdatedBy;

	    [QueryWriterField("Updated Date", "UpdatedDate")]
		public DateTimeOffset UpdatedDateQuery => this.UpdatedDate;

	    public AlarmAndEventLogClass()
		{
	        this.Initialize();
		}

		public AlarmAndEventLogClass(AlarmAndEventDescriptorClass descriptor)
		{
		    this.Initialize();

		    this.Source = descriptor.Source;
		    this.Alarm = descriptor.Alarm;
		    this._ID = descriptor.ID;
		}

		public void Initialize()
		{
		    this.SequenceNumber = 0;
		    this.Source = string.Empty;
		    this.Alarm = false;
		    this.AssociatedData = string.Empty;
		    this.CategoryID = string.Empty;
		    this.PriorityID = string.Empty;
		    this.Acknowledged = false;
		}

		public override void Reset()
		{
			base.Reset();
		    this.Initialize();
		}

		public void InsertSQL(SqlCommand cmd)
		{
		    const string SQL = "INSERT INTO tblAlarmAndEventLog " +
		                       "(Source," +
		                       "Alarm," +
		                       "ID," +
		                       "AssociatedData," +
		                       "CategoryID," +
		                       "PriorityID," +
		                       "Acknowledged," +
		                       "SiteGuid," +
		                       "CreatedDate," +
		                       "CreatedBy," +
		                       "UpdatedDate," +
		                       "UpdatedBy" +
		                       ") VALUES (" +
		                       "@Source," +
		                       "@Alarm," +
		                       "@ID," +
		                       "@AssociatedData," +
		                       "@CategoryID," +
		                       "@PriorityID," +
		                       "@Acknowledged," +
		                       "@SiteGuid," +
		                       "@CreatedDate," +
		                       "@CreatedBy," +
		                       "@UpdatedDate," +
		                       "@UpdatedBy" +
		                       ")";

			cmd.CommandText = SQL;

			cmd.Parameters.Add("@Source", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@Alarm", SqlDbType.Bit);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@AssociatedData", SqlDbType.NVarChar, -1); // -1 = NVARCHAR(MAX)
			cmd.Parameters.Add("@CategoryID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@PriorityID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Acknowledged", SqlDbType.Bit);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@Source"].Value = this.Source;

			if (this.Alarm)
			{
				cmd.Parameters["@Alarm"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Alarm"].Value = 0;
			}

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@AssociatedData"].Value = this.AssociatedData;
			cmd.Parameters["@CategoryID"].Value = this.CategoryID;
			cmd.Parameters["@PriorityID"].Value = this.PriorityID;

			if (this.Acknowledged)
			{
				cmd.Parameters["@Acknowledged"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Acknowledged"].Value = 0;
			}

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE tblAlarmAndEventLog " +
				"SET Acknowledged = @Acknowledged," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE SequenceNumber = @SequenceNumber";

			cmd.Parameters.Add("@Acknowledged", SqlDbType.Bit);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@SequenceNumber", SqlDbType.BigInt);

			if (this.Acknowledged)
			{
				cmd.Parameters["@Acknowledged"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Acknowledged"].Value = 0;
			}

			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@SequenceNumber"].Value = this.SequenceNumber;
		}

		public void PurgeBySiteSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblAlarmAndEventLog WHERE SiteGuid = @SiteGuid";
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		/// <summary>
		/// Get a SQLCommand to delete alarm and event log records older than the corresponding site's maximum days to retain logs
		/// </summary>
		/// <param name="cmd">A SqlCommand to populate</param>
		public static void PurgeOldRecords(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_AlarmAndEventLogDeleteOldRecords";
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="beginning"></param>
        /// <param name="ending"></param>
        /// <param name="type"></param>
        /// <param name="includeMemberSites"></param>
        /// <param name="dbName">This specifies the database name to be used. If querying the archive
        /// database, then this will be the archive database. If standard database, then this will be
        /// the same as the baseDbName</param>
        /// <param name="baseDbName">This specifies the base database name. Regardless of whether this
        /// request is for Achive or Live DB, this is the name of the Live DB. This is because the
        /// reference entity data is not archived; giving the base database name allows a join into
        /// the LiveDB for entity data.</param>
        public void EnumerateSQL(SqlCommand cmd, DateTimeOffset beginning, DateTimeOffset ending, string type, bool includeMemberSites, string dbName, string baseDbName, bool includeGlobalSites)
        {
			string sourceClause = string.Empty;
			string alarmClause = string.Empty;
			string idClause = string.Empty;
			string categoryIDClause = string.Empty;
			string priorityIDClause = string.Empty;

			if (string.IsNullOrWhiteSpace(this.Source) == false)
			{
				sourceClause = " AND Source = @Source";
			}

			if (type == "Alarms" || type == "Events")
			{
				alarmClause = " AND Alarm = @Alarm";
			}

			if (string.IsNullOrWhiteSpace(this.ID) == false)
			{
                idClause = " AND tblAlarmAndEventLog.ID = @ID";
			}

			if (string.IsNullOrWhiteSpace(this.CategoryID) == false)
			{
				categoryIDClause = " AND CategoryID = @CategoryID";
			}

			if (string.IsNullOrWhiteSpace(this.PriorityID) == false)
			{
				priorityIDClause = " AND PriorityID = @PriorityID";
			}

            string sql = "SELECT tblAlarmAndEventLog.*,tblSites.ID AS SiteID" +
                   " FROM {0}..tblAlarmAndEventLog" +
                   " JOIN {1}..tblSites ON tblAlarmAndEventLog.SiteGuid = tblSites.SiteGuid" +
                   " WHERE ({0}..tblAlarmAndEventLog.SiteGuid = @SiteGuid" +
               ((includeMemberSites) ? " OR {0}..tblAlarmAndEventLog.SiteGuid IN (SELECT ChildSiteGuid FROM {1}.map.tblSiteToSite WHERE ParentSiteGuid = @SiteGuid)" : string.Empty) +
                  ((includeGlobalSites) ? " OR {0}..tblAlarmAndEventLog.SiteGuid = '00000000-0000-0000-0000-000000000001'" : string.Empty) +
                  " ) " +
                  " AND ({0}..tblAlarmAndEventLog.CreatedDate >= @Beginning" +
                   " AND {0}..tblAlarmAndEventLog.CreatedDate  <= @Ending)" +
               sourceClause +
               alarmClause +
               idClause +
               categoryIDClause +
               priorityIDClause +
               " ORDER BY SequenceNumber DESC";

            cmd.CommandText = string.Format(sql, dbName, baseDbName);

            if (string.IsNullOrEmpty(this.Source) == false)
			{
				cmd.Parameters.Add("@Source", SqlDbType.NVarChar, 120);
				cmd.Parameters["@Source"].Value = this.Source;
			}

			if (type == "Alarms" || type == "Events")
			{
				cmd.Parameters.Add("@Alarm", SqlDbType.Bit);

				if (type == "Alarms")
				{
					cmd.Parameters["@Alarm"].Value = 1;
				}
				else if (type == "Events")
				{
					cmd.Parameters["@Alarm"].Value = 0;
				}
			}

			if (string.IsNullOrEmpty(this.ID) == false)
			{
				cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 120);
				cmd.Parameters["@ID"].Value = this.ID;
			}

			if (string.IsNullOrEmpty(this.CategoryID) == false)
			{
				cmd.Parameters.Add("@CategoryID", SqlDbType.NVarChar, 50);
				cmd.Parameters["@CategoryID"].Value = this.CategoryID;
			}

			if (string.IsNullOrEmpty(this.PriorityID) == false)
			{
				cmd.Parameters.Add("@PriorityID", SqlDbType.NVarChar, 50);
				cmd.Parameters["@PriorityID"].Value = this.PriorityID;
			}

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Beginning", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Ending", SqlDbType.DateTimeOffset);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@Beginning"].Value = beginning;
			cmd.Parameters["@Ending"].Value = ending;
		}

		public void EnumerateBySequenceNumberSQL(SqlCommand cmd)
		{

			cmd.CommandText =	"SELECT tblAlarmAndEventLog.*,tblSites.ID AS SiteID" +
								" FROM tblAlarmAndEventLog" +
								" JOIN tblSites ON tblAlarmAndEventLog.SiteGuid = tblSites.SiteGuid" +
								" WHERE SequenceNumber > @SequenceNumber " +
								" ORDER BY SequenceNumber";

			cmd.Parameters.Add("@SequenceNumber", SqlDbType.BigInt);
			cmd.Parameters["@SequenceNumber"].Value = this.SequenceNumber;
		}

        public void RowCountSQL(SqlCommand cmd)
        {
            cmd.CommandText = "SELECT COUNT(*) FROM tblAlarmAndEventLog";
        }

        public void EnumerateSourcesSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT DISTINCT Source FROM tblAlarmAndEventLog" +
				" WHERE (tblAlarmAndEventLog.SiteGuid = @SiteGuid OR tblAlarmAndEventLog.SiteGuid IN (SELECT ChildSiteGuid FROM map.tblSiteToSite WHERE ParentSiteGuid = @SiteGuid))" +
				" ORDER BY Source";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public void QueryWriterSQL(SqlCommand cmd, SecurityClass security, string selectClause, string dbName)
		{
			cmd.CommandText = selectClause
				+ " , '" + Guid.Empty.ToString() + "' AS EntityGuid"
				+ " FROM [" + dbName + "]..tblAlarmAndEventLog"
				+ " WHERE SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// This method is used when the edit button is clicked on the query writer results form
		/// </summary>
		/// <returns>The page corresponding to this entity</returns>
		public string DetailPageReference()
		{
			throw new ApplicationException("Entity edit not supported for this query type.");
		}

		public void SequenceNumberSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT TOP 1 SequenceNumber FROM tblAlarmAndEventLog ORDER BY SequenceNumber DESC";
		}

		public void QueryWriterPostProcess(SecurityClass security, DataSet set)
		{
		    this.CensorFieldsIfNecessary(security, set);
		}

		private void CensorFieldsIfNecessary(SecurityClass security, DataSet set)
		{
			if (security.HasRight(RIGHT.VIEW_ALARM_EVENT_LOGS) == false)
			{
				set.Tables[0].Rows.Clear();
			}
		}

		/// <summary>
		/// Override load to read the object correctly from the DB rather than using the Load in the base class
		/// </summary>
		/// <param name="o">an object to load</param>
		public override void Load(Object o)
		{
			DataRow row = null;

		    var dataRow = o as DataRow;
		    if (dataRow != null)
			{
				row = dataRow;
			}

		    var dataSet = o as DataSet;
		    if (dataSet != null)
			{
				var set = dataSet;

				var table = set.Tables[0];
				if (table.Rows.Count == 0)
				{
					return;
				}

				row = table.Rows[0];
			}

			this.SequenceNumber = DataObject.getValue<long>(row?["SequenceNumber"], 0);
			this.Source = DataObject.getValue<string>(row?["Source"], "");
			this.Alarm = DataObject.getValue<bool>(row?["Alarm"], false);
			this._ID = DataObject.getValue<string>(row?["ID"], "");
			this.AssociatedData = DataObject.getValue<string>(row?["AssociatedData"], "");
			this.CategoryID = DataObject.getValue<string>(row?["CategoryID"], "");
			this.PriorityID = DataObject.getValue<string>(row?["PriorityID"], "");
			this.Acknowledged = DataObject.getValue<bool>(row?["Acknowledged"], false);
			this._SiteGuid = DataObject.getValue<Guid>(row?["SiteGuid"], Guid.Empty);
			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row?["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row?["CreatedBy"], ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row?["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row?["UpdatedBy"], ADMIN);
			this.SiteID = DataObject.getValue<string>(row?["SiteID"], "");
		}
	}
}

