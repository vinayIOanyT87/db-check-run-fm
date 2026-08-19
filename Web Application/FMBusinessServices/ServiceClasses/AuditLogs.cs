namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,TransactionTimeout = "00:10:00")]
	public sealed class AuditLogsClass : IAuditLogs
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public DataSet EnumerateIDs(
			 SecurityClass security,
			 Guid siteGuid,
			 DateTimeOffset beginning,
			 DateTimeOffset ending,
			 string actionID,
			 string typeID,
			 bool includeMemberSites)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			string actionIDClause = "";
			string typeIDClause = "";

			if (!string.IsNullOrEmpty(actionID))
			{
				actionIDClause = " AND tblAuditLog.ActionID = @ActionID ";
			}

			if (!string.IsNullOrEmpty(typeID))
			{
				typeIDClause = " AND tblAuditLog.TypeID = @TypeID";
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{


				cmd.CommandText = "SELECT DISTINCT tblAuditLog.ID FROM tblAuditLog JOIN tblSites ON tblAuditLog.SiteGuid = tblSites.SiteGuid" +
									" WHERE (tblAuditLog.SiteGuid = @SiteGuid" +
									((!includeMemberSites) ? ")" : " OR tblAuditLog.SiteGuid IN (SELECT ChildSiteGuid FROM map.tblSiteToSite WHERE ParentSiteGuid = @SiteGuid))") +
									" AND (tblAuditLog.CreatedDate >= @Beginning" +
									" AND tblAuditLog.CreatedDate  <= @Ending)" +
									actionIDClause +
									typeIDClause +
									" ORDER BY tblAuditLog.ID DESC";

				if (!string.IsNullOrEmpty(actionIDClause))
				{
					cmd.Parameters.Add("@ActionID", SqlDbType.NVarChar, 20);
					cmd.Parameters["@ActionID"].Value = actionID;
				}

				if (!string.IsNullOrEmpty(typeIDClause))
				{
					cmd.Parameters.Add("@TypeID", SqlDbType.NVarChar, 50);
					cmd.Parameters["@TypeID"].Value = typeID;
				}


				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@Beginning", SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@Ending", SqlDbType.DateTimeOffset);

				cmd.Parameters["@SiteGuid"].Value = siteGuid;
				cmd.Parameters["@Beginning"].Value = beginning;
				cmd.Parameters["@Ending"].Value = ending;

				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return set;
		}

		public DataSet EnumerateForAuditLogPage(
			  SecurityClass security,
			  DateTimeOffset beginning,
			  DateTimeOffset ending,
			  string actionID,
			  string typeID,
			  string id,
			  string createdBy,
			  string sourceFilter,
			  bool useDataDictionary,
			  bool includeMemberSites, 
           bool useArchiveData,
           bool includeGlobalSites)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

			string actionIDClause = string.Empty;
			string typeIDClause = string.Empty;
			string idClause = string.Empty;
			string createdByClause = string.Empty;
			string sourceFilterClause = string.Empty;

			if (!string.IsNullOrEmpty(actionID))
			{
				actionIDClause = " AND tblAuditLog.ActionID = @ActionID ";
			}

			if (!string.IsNullOrEmpty(typeID))
			{
				typeIDClause = " AND (tblAuditLog.TypeID = @TypeID OR tblAuditLog.ParentTypeID = @TypeID)";
			}

			if (!string.IsNullOrEmpty(id))
			{
				idClause = " AND ((tblAuditLog.ID = @ID AND tblAuditLog.TypeID = @TypeID) OR (tblAuditLog.ID LIKE('%' + @ID + '%') AND tblAuditLog.ParentTypeID = @TypeID))";
			}

			if (!string.IsNullOrEmpty(createdBy))
			{
				createdByClause = " AND tblAuditLog.CreatedBy = @CreatedBy";
			}

			if (!string.IsNullOrEmpty(sourceFilter))
			{
				sourceFilterClause = " AND tblAuditLog.SourceNode LIKE '%' + @SourceNode + '%'";
			}

			DataSet set;
			using (var cmd = new SqlCommand())
			{
                string baseDbName = ConsolidatedDA.DatabaseName;

                string dbName = ConsolidatedDA.DatabaseName;
                if (useArchiveData)
                {
                    dbName = ConsolidatedDA.ArchiveDatabaseName;
                }


                string sql =
                    "SELECT tblAuditLog.*,tblSites.ID AS SiteID FROM {0}..tblAuditLog JOIN {1}..tblSites ON tblAuditLog.SiteGuid = tblSites.SiteGuid"
                    + " WHERE (tblAuditLog.SiteGuid = @SiteGuid"
                    + ((includeMemberSites) ? " OR tblAuditLog.SiteGuid IN (SELECT ChildSiteGuid FROM {1}.map.tblSiteToSite WHERE ParentSiteGuid = @SiteGuid)" : string.Empty)
                    + ((includeGlobalSites) ? " OR {0}..tblAuditLog.SiteGuid = '00000000-0000-0000-0000-000000000001'" : string.Empty)
                       + " ) "
                       + " AND (tblAuditLog.CreatedDate >= @Beginning"
                    + " AND tblAuditLog.CreatedDate  <= @Ending)"
                       + actionIDClause
                       + typeIDClause
                       + idClause
                       + createdByClause
                    + sourceFilterClause
                       + " ORDER BY tblAuditLog._ClusterIdx DESC";

                cmd.CommandText = string.Format(sql, dbName, baseDbName);

                if (!string.IsNullOrEmpty(actionIDClause))
				{
					cmd.Parameters.Add("@ActionID", SqlDbType.NVarChar, 20);
					cmd.Parameters["@ActionID"].Value = actionID;
				}

				if (!string.IsNullOrEmpty(typeIDClause))
				{
					cmd.Parameters.Add("@TypeID", SqlDbType.NVarChar, 50);
					cmd.Parameters["@TypeID"].Value = typeID;
				}

				if (!string.IsNullOrEmpty(idClause))
				{
					cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 256);
					cmd.Parameters["@ID"].Value = id;
				}

				if (!string.IsNullOrEmpty(createdByClause))
				{
					cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
					cmd.Parameters["@CreatedBy"].Value = createdBy;
				}

				if (!string.IsNullOrEmpty(sourceFilterClause))
				{
					cmd.Parameters.Add("@SourceNode", SqlDbType.NVarChar, 256);
					cmd.Parameters["@SourceNode"].Value = sourceFilter;
				}

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@Beginning", SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@Ending", SqlDbType.DateTimeOffset);

				cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@Beginning"].Value = beginning;
				cmd.Parameters["@Ending"].Value = ending;

				set = this.ConsolidatedDA.GetDataSet(cmd, security);

			}

			var dataDictionaries = new DataDictionariesClass();
			var table = set.Tables[0];

			// Loop through all the rows that were retrieved from the database and
			// create a new audit log table for the GUI.
			string translatedAdd = dataDictionaries.Get(security.SiteGuid, "Add");
			string translatedModify = dataDictionaries.Get(security.SiteGuid, "Modify");
			string translatedPurge = dataDictionaries.Get(security.SiteGuid, "Purge");
         string translatedArchive = dataDictionaries.Get(security.SiteGuid, "Archive");


         var typeIDDictionary = new Dictionary<string, string>();
			var propertyIDDictionary = new Dictionary<string, string>();

			foreach(DataRow row in table.Rows)
			{
				row["CreatedDate"] = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTimeOffset) row["CreatedDate"],site.TimeZone);
				if (useDataDictionary)
				{
					if (DataObject.getValue(row["ActionID"], "") == "Add")
					{
						row["ActionID"] = translatedAdd;
					}
					else if (DataObject.getValue(row["ActionID"], "") == "Modify")
					{
						row["ActionID"] = translatedModify;
					}
                    else if (DataObject.getValue(row["ActionID"], "") == "Purge")
                    {
                        row["ActionID"] = translatedPurge;
                    }
                    else
                    {
                        row["ActionID"] = translatedArchive;
                    }

                    string translatedTypeID;
					if (!typeIDDictionary.TryGetValue(DataObject.getValue(row["TypeID"], ""), out translatedTypeID))
					{
						translatedTypeID = dataDictionaries.Get(security.SiteGuid, DataObject.getValue(row["TypeID"], ""));
						typeIDDictionary.Add(DataObject.getValue(row["TypeID"], ""), translatedTypeID);
					}

					row["TypeID"] = translatedTypeID;

					string translatedPropertyID;
					if (!propertyIDDictionary.TryGetValue(DataObject.getValue(row["PropertyID"], ""), out translatedPropertyID))
					{
						translatedPropertyID = dataDictionaries.Get(security.SiteGuid, DataObject.getValue(row["PropertyID"], ""));
						propertyIDDictionary.Add(DataObject.getValue(row["PropertyID"], ""), translatedPropertyID);
					}

					row["PropertyID"] = translatedPropertyID;
				}
				else
				{
					row["ActionID"] = DataObject.getValue(row["ActionID"], "");
					row["TypeID"] = DataObject.getValue(row["TypeID"], "");
					row["PropertyID"] = DataObject.getValue(row["PropertyID"], "");
				}

				var newValue = DataObject.getValue(row["NewValue"], "");

				if (useDataDictionary
				&&	(newValue == "{None}"
				|| newValue == "{Unassigned}"
				|| newValue == "{All}"
				|| DataObject.getValue(row["PropertyID"], "").Contains("Unit Index")
				|| DataObject.getValue(row["PropertyID"], "").Contains("Lookup")))
				{
					row["NewValue"] = dataDictionaries.Get(security.SiteGuid, newValue);
				}
				else
				{
					row["NewValue"] = newValue;
				}

				var oldValue = DataObject.getValue(row["OldValue"], "");

				if (useDataDictionary
				&& (oldValue == "{None}"
				|| oldValue == "{Unassigned}"
				|| oldValue == "{All}"
				|| DataObject.getValue(row["PropertyID"], "").Contains("Unit Index")
				|| DataObject.getValue(row["PropertyID"], "").Contains("Lookup")))
				{
					row["OldValue"] = dataDictionaries.Get(security.SiteGuid, oldValue);
				}
				else
				{
					row["OldValue"] = oldValue;
				}
			}

			return set;
		}

        public AuditLogCollectionClass EnumerateByBatch(
            SecurityClass security,
            DateTimeOffset? auditedDateTimeStart,
            DateTimeOffset? auditedDateTimeEnd,
            string actionID,
            string typeID,
            string id,
            string sourceNode,
            bool useDataDictionary,
            bool includeMemberSites,
            int batchSize,
            int batchNumber)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            var auditLogCollection = new AuditLogCollectionClass();

            var sites = new SitesClass();
            SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

            DataSet set;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetAuditLogRecordsByBatch";

                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@UserGuid"].Value = DBNull.Value;
                if (security.UserGuid != Guid.Empty)
                    cmd.Parameters["@UserGuid"].Value = security.UserGuid;

                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@SiteGuid"].Value = DBNull.Value;
                if (security.SiteGuid != Guid.Empty)
                    cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;

                cmd.Parameters.Add("@AuditedDateTimeStart", SqlDbType.DateTimeOffset);
                cmd.Parameters["@AuditedDateTimeStart"].Value = DBNull.Value;
                if (auditedDateTimeStart != null)
                    cmd.Parameters["@AuditedDateTimeStart"].Value = (DateTimeOffset)auditedDateTimeStart;

                cmd.Parameters.Add("@AuditedDateTimeEnd", SqlDbType.DateTimeOffset);
                cmd.Parameters["@AuditedDateTimeEnd"].Value = DBNull.Value;
                if (auditedDateTimeEnd != null)
                    cmd.Parameters["@AuditedDateTimeEnd"].Value = (DateTimeOffset)auditedDateTimeEnd;

                cmd.Parameters.Add("@ActionId", SqlDbType.NVarChar, 20);
                cmd.Parameters["@ActionId"].Value = DBNull.Value;
                if (string.IsNullOrEmpty(actionID) == false)
                    cmd.Parameters["@ActionId"].Value = actionID;

                cmd.Parameters.Add("@TypeId", SqlDbType.NVarChar, 50);
                cmd.Parameters["@TypeId"].Value = DBNull.Value;
                if (string.IsNullOrEmpty(typeID) == false)
                    cmd.Parameters["@TypeId"].Value = typeID;

                cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 256);
                cmd.Parameters["@Id"].Value = DBNull.Value;
                if (string.IsNullOrEmpty(id) == false)
                    cmd.Parameters["@Id"].Value = id;

                cmd.Parameters.Add("@SourceNode", SqlDbType.NVarChar, 256);
                cmd.Parameters["@SourceNode"].Value = DBNull.Value;
                if (string.IsNullOrEmpty(sourceNode) == false)
                    cmd.Parameters["@SourceNode"].Value = sourceNode;

                cmd.Parameters.Add("@IncludeMemberSites", SqlDbType.Bit);
                cmd.Parameters["@IncludeMemberSites"].Value = Convert.ToInt32(includeMemberSites);

                cmd.Parameters.Add("@BatchSize", SqlDbType.Int);
                cmd.Parameters["@BatchSize"].Value = batchSize;

				cmd.Parameters.Add("@BatchNumber", SqlDbType.Int);
				cmd.Parameters["@BatchNumber"].Value = batchNumber;

				cmd.Parameters.Add("@FullRecordCount", SqlDbType.Int);
                cmd.Parameters["@FullRecordCount"].Value = DBNull.Value;
                cmd.Parameters["@FullRecordCount"].Direction = ParameterDirection.Output;

                set = this.ConsolidatedDA.GetDataSet(cmd, security);

                if ((cmd.Parameters["@FullRecordCount"].Value != null) && (cmd.Parameters["@FullRecordCount"].Value != System.DBNull.Value))
                    auditLogCollection.FullRecordCount = (int)(cmd.Parameters["@FullRecordCount"].Value);
            }

            var dataDictionaries = new DataDictionariesClass();
            var table = set.Tables[0];

            // Loop through all the rows that were retrieved from the database and
            // create a new audit log table for the GUI.
            string translatedAdd    = dataDictionaries.Get(security.SiteGuid, "Add");
            string translatedModify = dataDictionaries.Get(security.SiteGuid, "Modify");
            string translatedPurge  = dataDictionaries.Get(security.SiteGuid, "Purge");

            var typeIDDictionary = new Dictionary<string, string>();
            var propertyIDDictionary = new Dictionary<string, string>();

            foreach (DataRow row in table.Rows)
            {
                AuditLogClass auditLog = new AuditLogClass
                                         {
                                             IdentityGuid   = DataObject.getValue<Guid>(row["AuditLogGuid"], Guid.Empty),
                                             SiteGuid       = DataObject.getValue<Guid>( row["SiteGuid"], Guid.Empty),
                                             SiteID         = DataObject.getValue(row["SiteId"], string.Empty),
                                             SourceNode     = DataObject.getValue(row["SourceNode"], string.Empty),
                                             AuditContext   = DataObject.getValue(row["AuditContext"], string.Empty),
                                             SessionId      = DataObject.getValue(row["SessionId"], string.Empty),
                                             ID             = DataObject.getValue(row["ID"], string.Empty),
                                             CreatedDate    = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTimeOffset)row["CreatedDate"], site.TimeZone),
                                             CreatedBy      = DataObject.getValue(row["CreatedBy"], string.Empty),
                                             AuditedDate    = null
                                         };

                if (!row.IsNull("AuditedDate"))
                {
                    auditLog.AuditedDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTimeOffset)row["AuditedDate"], site.TimeZone);
                }

                if (useDataDictionary)
                {
                    if (DataObject.getValue(row["ActionID"], string.Empty) == "Add")
                    {
                        auditLog.ActionId = translatedAdd;
                    }
                    else if (DataObject.getValue(row["ActionID"], string.Empty) == "Modify")
                    {
                        auditLog.ActionId = translatedModify;
                    }
                    else
                    {
                        auditLog.ActionId = translatedPurge;
                    }

                    string translatedTypeID;
                    if (!typeIDDictionary.TryGetValue(DataObject.getValue(row["TypeID"], string.Empty), out translatedTypeID))
                    {
                        translatedTypeID = dataDictionaries.Get(security.SiteGuid, DataObject.getValue(row["TypeID"], string.Empty));
                        typeIDDictionary.Add(DataObject.getValue(row["TypeID"], string.Empty), translatedTypeID);
                    }
                    auditLog.TypeId = translatedTypeID;

                    string translatedParentTypeID;
                    if (!typeIDDictionary.TryGetValue(DataObject.getValue(row["ParentTypeID"], string.Empty), out translatedParentTypeID))
                    {
                        translatedParentTypeID = dataDictionaries.Get(security.SiteGuid, DataObject.getValue(row["ParentTypeID"], string.Empty));
                        typeIDDictionary.Add(DataObject.getValue(row["ParentTypeID"], string.Empty), translatedParentTypeID);
                    }
                    auditLog.ParentTypeId = translatedParentTypeID;

                    string translatedPropertyID;
                    if (!propertyIDDictionary.TryGetValue(DataObject.getValue(row["PropertyID"], string.Empty), out translatedPropertyID))
                    {
                        translatedPropertyID = dataDictionaries.Get(security.SiteGuid, DataObject.getValue(row["PropertyID"], string.Empty));
                        propertyIDDictionary.Add(DataObject.getValue(row["PropertyID"], string.Empty), translatedPropertyID);
                    }
                    auditLog.PropertyId = translatedPropertyID;
                }
                else
                {
                    auditLog.ActionId       = DataObject.getValue(row["ActionID"], string.Empty);
                    auditLog.TypeId         = DataObject.getValue(row["TypeID"], string.Empty);
                    auditLog.ParentTypeId   = DataObject.getValue(row["ParentTypeID"], string.Empty);
                    auditLog.PropertyId     = DataObject.getValue(row["PropertyID"], string.Empty);
                }

                var newValue = DataObject.getValue(row["NewValue"], string.Empty);

                if (useDataDictionary
                    && (newValue == "{None}"
                    || newValue == "{Unassigned}"
                    || newValue == "{All}"
                    || DataObject.getValue(row["PropertyID"], string.Empty).Contains("Unit Index")
                    || DataObject.getValue(row["PropertyID"], string.Empty).Contains("Lookup")))
                {
                    auditLog.NewValue = dataDictionaries.Get(security.SiteGuid, newValue);
                }
                else
                {
                    auditLog.NewValue = newValue;
                }

                var oldValue = DataObject.getValue(row["OldValue"], string.Empty);

                if (useDataDictionary
                    && (oldValue == "{None}"
                    || oldValue == "{Unassigned}"
                    || oldValue == "{All}"
                    || DataObject.getValue(row["PropertyID"], "").Contains("Unit Index")
                    || DataObject.getValue(row["PropertyID"], "").Contains("Lookup")))
                {
                    auditLog.OldValue = dataDictionaries.Get(security.SiteGuid, oldValue);
                }
                else
                {
                    auditLog.OldValue = oldValue;
                }

                auditLogCollection.AuditLogList.Add(auditLog);
            }

            return auditLogCollection;
        }

        public List<string> EnumerateAuditLogIds(
            SecurityClass security,
            Guid siteGuid,
            DateTimeOffset? auditedDateTimeStart,
            DateTimeOffset? auditedDateTimeEnd,
            string actionID,
            string typeID)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            List<string> auditLogIdCollection = new List<string>();

            DataSet set;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetAuditLogIdByFilter";

                cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
                cmd.Parameters["@UserId"].Value = DBNull.Value;
                if (string.IsNullOrEmpty(security.UserID) == false)
                    cmd.Parameters["@UserId"].Value = security.UserID;

                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@SiteGuid"].Value = DBNull.Value;
                if (security.SiteGuid != Guid.Empty)
                    cmd.Parameters["@SiteGuid"].Value = siteGuid;

                cmd.Parameters.Add("@AuditedDateTimeStart", SqlDbType.DateTimeOffset);
                cmd.Parameters["@AuditedDateTimeStart"].Value = DBNull.Value;
                if (auditedDateTimeStart != null)
                    cmd.Parameters["@AuditedDateTimeStart"].Value = (DateTimeOffset)auditedDateTimeStart;

                cmd.Parameters.Add("@AuditedDateTimeEnd", SqlDbType.DateTimeOffset);
                cmd.Parameters["@AuditedDateTimeEnd"].Value = DBNull.Value;
                if (auditedDateTimeEnd != null)
                    cmd.Parameters["@AuditedDateTimeEnd"].Value = (DateTimeOffset)auditedDateTimeEnd;

                cmd.Parameters.Add("@ActionId", SqlDbType.NVarChar, 20);
                cmd.Parameters["@ActionId"].Value = DBNull.Value;
                if (string.IsNullOrEmpty(actionID) == false)
                    cmd.Parameters["@ActionId"].Value = actionID;

                cmd.Parameters.Add("@TypeId", SqlDbType.NVarChar, 50);
                cmd.Parameters["@TypeId"].Value = DBNull.Value;
                if (string.IsNullOrEmpty(typeID) == false)
                    cmd.Parameters["@TypeId"].Value = typeID;

                set = this.ConsolidatedDA.GetDataSet(cmd, security);
            }

            if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
            {
                return auditLogIdCollection;
            }

            var table = set.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                auditLogIdCollection.Add(DataObject.getValue(row["Id"], string .Empty));
            }

            return auditLogIdCollection;
        }

		public void ProcessAuditPurgeOld(SecurityClass security, Guid siteGuid, int maxDaysToRetain)
		{

			using (var cmd = new SqlCommand())
			{
				cmd.CommandTimeout = 600;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "DELETE al FROM  tblAuditLog al WHERE al.Auditeddate < DateAdd(Day, -1 * @MaximumDaysToRetainLogs , sysdatetimeoffset()) and al.siteGuid = @siteGuid";

				cmd.Parameters.Add("@siteguid", SqlDbType.UniqueIdentifier).Value = siteGuid;
				cmd.Parameters.Add("@MaximumDaysToRetainLogs", SqlDbType.Int).Value = maxDaysToRetain;

				ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
			}
		}

		public void PurgeShadowSiteTable(SecurityClass security, Guid siteGuid)
		{

			string strSql = "DECLARE @t TABLE(tablename VARCHAR(100)) " +
						 "DECLARE @sqlClean VARCHAR(MAX) " +
						 " SET @sqlClean = '' " +
						 " INSERT INTO @t " +
						 "SELECT  t.name AS TableName " +
						 "FROM    sys.tables t " +
						 "INNER JOIN sys.schemas s ON s.schema_id = t.schema_id " +
						 "INNER JOIN INFORMATION_SCHEMA.COLUMNS IC ON IC.TABLE_NAME = t.name and ic.Table_schema = s.name " +
						 "WHERE s.name = 'fmaudit' and IC.COLUMN_NAME = 'SiteGuid' " +
						 " SELECT @sqlClean = @sqlClean + 'Select TOP 1 SiteGuid From fmaudit.' + tablename + ' WHERE SiteGuid = @siteGuid union ' FROM @t " +
						 " SELECT @sqlClean = @sqlClean + 'Select TOP 1 SiteGuid From dbo.tblAuditLog WHERE SiteGuid = @siteGuid' " +
						 " SELECT @sqlClean = 'DELETE FROM dbo.tblSitesShadow WHERE SiteGuid = @siteguid AND NOT EXISTS(' + @sqlClean + ') AND DeletedDate IS NOT NULL'";

			using (var cmd = new SqlCommand())
			{
				cmd.CommandTimeout = 600;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = strSql;

				cmd.Parameters.Add("@siteguid", SqlDbType.UniqueIdentifier).Value = siteGuid;

				ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
			}
		}

		public Dictionary<Guid, int?> GetAllSiteRetentionForShadowTable(SecurityClass security)
		{
			DataSet siteGuids;
			var retVal = new Dictionary<Guid, int?>();

			using (var cmd = new SqlCommand())
			{
				cmd.CommandTimeout = 600;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT SiteGuid, MaximumDaysToRetainLogs FROM tblsitesshadow";

				siteGuids = ConsolidatedDA.GetDataSet(cmd, security);
			}

			foreach (DataRow dr in siteGuids.Tables[0].Rows)
			{
				retVal.Add((Guid)dr["SiteGuid"], DataObject.getValue<int?>(dr["MaximumDaysToRetainLogs"], null));
			}

			return retVal;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ProcessPendingAudits(SecurityClass security)
		{
			using (var cmd = new SqlCommand())
			{
				cmd.CommandTimeout = 600;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "[dbo].[usp_AuditProcessor]";

				ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
			}
		}
	}
}