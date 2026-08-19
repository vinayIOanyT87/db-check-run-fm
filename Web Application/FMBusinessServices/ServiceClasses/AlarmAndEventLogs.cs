namespace FMBusinessServices.ServiceClasses
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.ServiceModel;

    /// <summary>
	/// Summary description for AlarmAndEventLogsClass.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public sealed class AlarmAndEventLogsClass : IDependency, IAlarmAndEventLogs
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        // Running count of consecutive errors writing to alarm and event log table.  The count is
        // reset to zero when an insert or update to the alarm and event log table is successful.
        private static int errorCount;

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, AlarmAndEventLogClass alarmAndEventLog)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (alarmAndEventLog == null)
			{
				throw new ArgumentNullException(nameof(alarmAndEventLog));
			}

			AlarmAndEventsClass alarmAndEvents = new AlarmAndEventsClass();

			AlarmAndEventClass alarmAndEvent = alarmAndEvents.Get(security, alarmAndEventLog.Source, alarmAndEventLog.ID);

			if (alarmAndEvent.IdentityGuid != Guid.Empty && !alarmAndEvent.Enabled)
			{
				// If the alarm or event source is disabled then do not write a record to the DB
				return;
			}

			alarmAndEventLog.CategoryID = alarmAndEvent.CategoryID;
			alarmAndEventLog.PriorityID = alarmAndEvent.PriorityID;
			alarmAndEventLog.SiteGuid = security.SiteGuid;
			alarmAndEventLog.CreatedDate = DateTimeOffset.Now;
			alarmAndEventLog.CreatedBy = security.UserID;
			alarmAndEventLog.UpdatedDate = alarmAndEventLog.CreatedDate;
			alarmAndEventLog.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
			    try
			    {
			        alarmAndEventLog.InsertSQL(cmd);
			        ConsolidatedDA.ExecuteQuery(security, cmd);

			        // Reset error count on successful enumerate.
			        errorCount = 0;
			    }
			    catch (ConsolidatedDAException)
			    {
			        // Increment error count before throwing FMAlarmAndEventLogException
			        throw new FMAlarmAndEventLogException(++errorCount);
			    }
			}
		}

        /// <summary>
        /// Add multiple alarm and event log records at once
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="alarmAndEventLogs">The alarm and event log records to add</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AddList(SecurityClass security, List<AlarmAndEventLogClass> alarmAndEventLogs)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (alarmAndEventLogs == null)
            {
                throw new ArgumentNullException(nameof(alarmAndEventLogs));
            }

            // If no alarm and event log records need to be saved we have no work to do.
            if (alarmAndEventLogs.Count == 0)
            {
                return;
            }

            foreach (AlarmAndEventLogClass alarmAndEventLog in alarmAndEventLogs)
            {       
                alarmAndEventLog.SiteGuid = security.SiteGuid;
                alarmAndEventLog.CreatedDate = DateTimeOffset.Now;
                alarmAndEventLog.CreatedBy = security.UserID;
                alarmAndEventLog.UpdatedDate = alarmAndEventLog.CreatedDate;
                alarmAndEventLog.UpdatedBy = security.UserID;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                // Execute the stored procedure, passing in the list (table) of alarms and event log records
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_AlarmAndEventLogInsert";

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmAndEventLogs", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecordsForInsert(alarmAndEventLogs);
                tableValuedParameter.TypeName = "dbo.AlarmAndEventLogType";

                try
                {
                    var consolidatedDa = new ConsolidatedDAClass();
                    consolidatedDa.ExecuteQuery(security, cmd);

                    // Reset error count on successful enumerate.
                    errorCount = 0;
                }
                catch (ConsolidatedDAException)
                {
                    // Increment error count before throwing FMAlarmAndEventLogException
                    throw new FMAlarmAndEventLogException(++errorCount);
                }
            }
        }

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AlarmAndEventLogClass alarmAndEventLog)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (alarmAndEventLog == null)
				throw new ArgumentNullException(nameof(alarmAndEventLog));

			alarmAndEventLog.UpdatedDate = DateTimeOffset.Now;
			alarmAndEventLog.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
			    try
			    {
			        alarmAndEventLog.UpdateSQL(cmd);
			        ConsolidatedDA.ExecuteQuery(security, cmd);

			        // Reset error count on successful enumerate.
			        errorCount = 0;
			    }
			    catch (ConsolidatedDAException)
			    {
			        // Increment error count before throwing FMAlarmAndEventLogException
			        throw new FMAlarmAndEventLogException(++errorCount);
			    }
			}
		}

		/// <summary>
		/// Delete alarm and event log records that are older than the maximum number of days to retain logs
		/// specified for the site corresponding to the log
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeOldRecords(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			using (var cmd = new SqlCommand())
			{
			    try
			    {
			        AlarmAndEventLogClass.PurgeOldRecords(cmd);
			        this.ConsolidatedDA.ExecuteQuery(security, cmd);

			        // Reset error count on successful enumerate.
			        errorCount = 0;
			    }
			    catch (ConsolidatedDAException)
			    {
			        // Increment error count before throwing FMAlarmAndEventLogException
			        throw new FMAlarmAndEventLogException(++errorCount);
			    }
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeBySite(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			throw new ArgumentNullException(nameof(security));

			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass { SiteGuid = siteGuid };

			using (SqlCommand cmd = new SqlCommand())
			{
				try
				{
					alarmAndEventLog.PurgeBySiteSQL(cmd);
					ConsolidatedDA.ExecuteQuery(security, cmd);

					// Reset error count on successful enumerate.
					errorCount = 0;
				}
				catch (ConsolidatedDAException)
				{
					// Increment error count before throwing FMAlarmAndEventLogException
					throw new FMAlarmAndEventLogException(++errorCount);
				}
			}
		}

		public AlarmAndEventLogCollectionClass Enumerate(SecurityClass security,
																		DateTimeOffset beginning,
																		DateTimeOffset ending,
																		string source,
																		string type,
																		string id,
																		string categoryID,
																		string priorityID,
																		bool includeMemberSites,
                                                      bool queryArchiveDb,
                                                      bool includeGlobalSites)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			SitesClass sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass
		                                             {
		                                                 Source = source,
		                                                 ID = id,
		                                                 CategoryID = categoryID,
		                                                 PriorityID = priorityID,
		                                                 SiteGuid = security.SiteGuid
		                                             };
		    DataSet set;

            AlarmAndEventLogCollectionClass alarmAndEventLogCollection = new AlarmAndEventLogCollectionClass();
				if (type == "Alarms and Events") // Alarms and Events is an alias for Both
					 type = "Both";

            if (type != "Inventory Management: Alarms")
				{
					 using (SqlCommand cmd = new SqlCommand())
					 {
						  try
						  {
								string baseDbName = ConsolidatedDA.DatabaseName;

								string dbName = ConsolidatedDA.DatabaseName;
								if (queryArchiveDb)
								{
									 dbName = ConsolidatedDA.ArchiveDatabaseName;
								}

								alarmAndEventLog.EnumerateSQL(cmd, beginning, ending, type, includeMemberSites, dbName, baseDbName, includeGlobalSites);
								set = ConsolidatedDA.GetDataSet(cmd, security);

								// Reset error count on successful enumerate.
								errorCount = 0;
						  }
						  catch (ConsolidatedDAException)
						  {
								// Increment error count before throwing FMAlarmAndEventLogException
								throw new FMAlarmAndEventLogException(++errorCount);
						  }
					 }



					 DataTable table = set.Tables[0];
					 while (table.Rows.Count != 0)
					 {
						  alarmAndEventLog = new AlarmAndEventLogClass();
						  alarmAndEventLog.Load(set);
						  alarmAndEventLog.CreatedDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(alarmAndEventLog.CreatedDate, site.TimeZone);
						  alarmAndEventLogCollection.Add(alarmAndEventLog);
						  table.Rows.RemoveAt(0);
					 }
				}
				else // Operate Alarms. Use Cassandra db.
			   {
					 int recordTypeFilter = 1; // 1 = alarms. TODO: make customizeable
					 List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList = new List<AlarmHistoryTabColumnFilterInfo>();
					 columnFilterInfoList.Add(new AlarmHistoryTabColumnFilterInfo());
					 columnFilterInfoList[0].FromDateStr = beginning.ToString("M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                columnFilterInfoList[0].Index = -99;
                columnFilterInfoList[0].Name = "TimeStamp";
                columnFilterInfoList[0].SelectedColumnFilterEnum = 0;
                columnFilterInfoList[0].ToDateStr = ending.ToString("M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                columnFilterInfoList.Add(new AlarmHistoryTabColumnFilterInfo());
                columnFilterInfoList[1].Index = 1;
                columnFilterInfoList[1].Name = "Site";
                columnFilterInfoList[1].SelectedColumnFilterEnum = (AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums)2;


                List<AandEDataElement> archiveDataList = FMChannelHelper.MakeCall<IAandEArchive, List<AandEDataElement>>
                                 (x => x.GetAandEArchiveData(security, columnFilterInfoList, recordTypeFilter));

					 AlarmPriorityClass alarmPriority = new AlarmPriorityClass();

                if (!(priorityID == "{All}" || priorityID == ""))
					 { Guid alarmPriorityGuid = FMChannelHelper.MakeCall<IAlarmPriorities, Guid>(
					 alarmPriorities => alarmPriorities.GetIdentityGuid(security, priorityID));

						  alarmPriority = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityClass>(alarmPriorities => alarmPriorities.Get(security, alarmPriorityGuid));
					 }


                foreach (AandEDataElement element in archiveDataList)
					 {
						  if (element.Priority == alarmPriority.Priority.ToString() || priorityID == "{All}" || priorityID == "")
						  {
								alarmAndEventLog = new AlarmAndEventLogClass();
								alarmAndEventLog.CreatedDate = element.DateAndTime;
								alarmAndEventLog.UpdatedDate = element.DateAndTime;
								alarmAndEventLog.SiteID = element.Site;
								alarmAndEventLog.PriorityID = element.Priority;
								alarmAndEventLog.UpdatedBy = element.User;

								alarmAndEventLog.Source = "Inventory Management";
								alarmAndEventLog.AssociatedData = element.Variable + ": " + element.Value;
								if (element.Action == "Acknowledged")
								{ 
									 alarmAndEventLog.Acknowledged = true;
                            alarmAndEventLog.AssociatedData = element.Variable;
                        }
								else
								{
                            alarmAndEventLog.AssociatedData = element.Variable + ": " + element.Value;
                        }
								alarmAndEventLog.ID = element.Point + "." + element.AlarmState + ": " + element.Action;

								alarmAndEventLogCollection.Add(alarmAndEventLog);
						  }
                }
            }

			return alarmAndEventLogCollection;
		}

		public string[] EnumerateSources(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass { SiteGuid = security.SiteGuid };

		    DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
			    try
			    {
			        alarmAndEventLog.EnumerateSourcesSQL(cmd);
			        set = ConsolidatedDA.GetDataSet(cmd, security);

			        // Reset error count on successful enumerate.
			        errorCount = 0;
			    }
			    catch (ConsolidatedDAException)
			    {
			        // Increment error count before throwing FMAlarmAndEventLogException
			        throw new FMAlarmAndEventLogException(++errorCount);
			    }
			}

			DataTable table = set.Tables[0];
		    var modules = new string[table.Rows.Count];

			for (int item = 0; item < table.Rows.Count; item++)
			{
				DataRow row = table.Rows[item];
				modules[item] = (string)row["Source"];
			}

			return modules;
		}

		public AlarmAndEventLogCollectionClass EnumerateBySequenceNumber(SecurityClass security, long sequenceNumber)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass { SequenceNumber = sequenceNumber };

		    DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
			    try
			    {
			        alarmAndEventLog.EnumerateBySequenceNumberSQL(cmd);
			        set = ConsolidatedDA.GetDataSet(cmd, security);

                    // Reset error count on successful enumerate.
				    errorCount = 0;
			    }
			    catch (ConsolidatedDAException)
			    {
                    // Increment error count before throwing FMAlarmAndEventLogException
                    throw new FMAlarmAndEventLogException(++errorCount);
			    }
				
			}

			AlarmAndEventLogCollectionClass alarmAndEventLogCollection = new AlarmAndEventLogCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				alarmAndEventLog = new AlarmAndEventLogClass();
				alarmAndEventLog.Load(set);
				alarmAndEventLogCollection.Add(alarmAndEventLog);
				table.Rows.RemoveAt(0);
			}

			return alarmAndEventLogCollection;
		}

        /// <summary>
        /// Create SqlDataRecords representing alarm and event log records to insert
        /// </summary>
        /// <param name="alarmAndEventLogs">The alarm and event logs to create SqlDataRecords for</param>
        /// <returns>SqlDataRecords representing alarm and event log records to insert</returns>
	    private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForInsert(IEnumerable<AlarmAndEventLogClass> alarmAndEventLogs)
	    {
            SqlMetaData[] metaData = new SqlMetaData[10];

            int i = 0;
            metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Source", SqlDbType.NVarChar, 120);
            metaData[i++] = new SqlMetaData("Alarm", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("ID", SqlDbType.NVarChar, 120);
            metaData[i++] = new SqlMetaData("AssociatedData", SqlDbType.NVarChar, -1);
            metaData[i++] = new SqlMetaData("Acknowledged", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CreatedDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("CreatedBy", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("UpdatedDate", SqlDbType.DateTimeOffset);
            metaData[i] = new SqlMetaData("UpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (AlarmAndEventLogClass alarmAndEventLog in alarmAndEventLogs)
            {
                int j = 0;

                record.SetGuid(j++, alarmAndEventLog.SiteGuid);
                record.SetString(j++, alarmAndEventLog.Source);
                record.SetBoolean(j++, alarmAndEventLog.Alarm);
                record.SetString(j++, alarmAndEventLog.ID);
                record.SetString(j++, alarmAndEventLog.AssociatedData);
                record.SetBoolean(j++, alarmAndEventLog.Acknowledged);
                record.SetDateTimeOffset(j++, alarmAndEventLog.CreatedDate);
                record.SetString(j++, alarmAndEventLog.CreatedBy);
                record.SetDateTimeOffset(j++, alarmAndEventLog.UpdatedDate);
                record.SetString(j, alarmAndEventLog.UpdatedBy);

                yield return record;
            }
	    }

        /// <summary>
        /// This method checks the number of rows in the table and throws an exception
        /// when the number of rows reaches thresholdPercentage of the CapacityLimitInRows setting.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="capacityLimitInRows"></param>
        /// <param name="thresholdPercentage"></param>
        public void CheckLogSize(SecurityClass security, int capacityLimitInRows, int thresholdPercentage)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            var alarmAndEventLog = new AlarmAndEventLogClass();

            int numberOfRows;
            try
            {
                using (var cmd = new SqlCommand())
                {
                    alarmAndEventLog.RowCountSQL(cmd);
	                numberOfRows = (int)this.ConsolidatedDA.ExecuteScalar(cmd, security);
                }
                
                // Reset error count on successful query.
                errorCount = 0;
            }
            catch (ConsolidatedDAException)
            {
                // Increment error count before throwing FMAlarmAndEventLogException
                throw new FaultException<FMAlarmAndEventLogException>(new  FMAlarmAndEventLogException(++errorCount));
            }

            if (numberOfRows > 0)
            {
                // ReSharper disable RedundantCast
                int currentPercentage = Convert.ToInt32(((double)numberOfRows / (double)capacityLimitInRows) * (double)100.0);
                // ReSharper restore RedundantCast

                if (currentPercentage >= thresholdPercentage)
                {
                    throw new FaultException<FMRowCountThresholdException>( new FMRowCountThresholdException("Alarm and Event Log", currentPercentage.ToString()));
                }
            }
        }

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (Object == null)
				throw new ArgumentNullException(nameof(Object));

		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (Object == null)
				throw new ArgumentNullException(nameof(Object));
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (Object == null)
				throw new ArgumentNullException(nameof(Object));

			// Purge Logs
		    var site = Object as SiteClass;
		    if (site != null)
			{
			    this.PurgeBySite(security, site.SiteGuid);
			}
		}
	}
}
