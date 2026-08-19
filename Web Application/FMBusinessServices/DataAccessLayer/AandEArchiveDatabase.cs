
namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Threading;
	using System.Diagnostics;
	using System.Globalization;

	using Cassandra;
	using Cassandra.Data.Linq;
	using Cassandra.Mapping;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using InternalClasses;
	using InternalInterfaces;
	using ServiceClasses;

	using FMCore;

	internal class AandEArchiveDatabase : IAandEArchiveDatabase
	{
		#region data memembers
		const long TicksPerSecond = 10000000;

		const long maximumAsynchronousQueries = 1000;

		private const int MaximumBatchSize = 30;

		private const int MaxLockDelay = 10000;

		public static ConsistencyLevel consistencyLevel;

		private static readonly ICassandraKeyspaceCreator CassandraKeyspaceCreator = new CassandraKeyspaceCreator();

		private static readonly ICassandraDataTablesCreator CassandraDataTablesCreator = new CassandraDataTablesCreator();

		private static readonly ICassandraConnectionConfig CassandraConnectionConfig = new CassandraConnectionConfig();

		private static Cluster cassandraCluster;

		private static ISession aandEcassandraSession;

		private static readonly ReaderWriterLockSlim AandEsessionLock = new ReaderWriterLockSlim();

		private static Table<AandEDataElement> aandEArchiveTable;

		private static Table<AlarmAndEventSynchronizationElement> SynchronizationTable;

		private List<SiteClass> siteHierarchyList; 

		//private static Table<SynchronizationElement> SynchronizationTable;

		protected EventLogging EventLogging;

		public static string KeyspaceName = "FMAandEArchive_Data";

		public enum RecordTypes { None = 0, Alarm, Event, AlarmAndEvent}

		private SiteClass currentSite ;
		#endregion

		void IAandEArchiveDatabase.Initialize(SecurityClass security)
		{
			security.ThrowIfNull("security");

			if (!AandEsessionLock.TryEnterWriteLock(MaxLockDelay))
			{
				throw new Exception("AandEArchiveDatabase : Initialize - timeout acquiring write lock");
			}

			try
			{
				this.EventLogging = new EventLogging();

				ShutdownIfRunning();

				var contactPoints = CassandraConnectionConfig.GetContactPoints(security);
				var credentials = CassandraConnectionConfig.GetCredentials(security);

				try
				{
					consistencyLevel = (ConsistencyLevel)Enum.Parse(typeof(ConsistencyLevel), CassandraConnectionConfig.GetConsistencyLevel(security));
				}
				catch (Exception ex)
				{
					string errorMessage = string.Format("Invalid Consistency Level Configured. Defaulting to a Level of One. {0}", ex.Message);
					this.EventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
					consistencyLevel = ConsistencyLevel.One;
				}


				if (!(String.IsNullOrEmpty(credentials[0]) || String.IsNullOrEmpty(credentials[1])))
				{
					cassandraCluster =
						Cluster.Builder()
							.AddContactPoints(contactPoints)
							.WithReconnectionPolicy(new ExponentialReconnectionPolicy(2 * 1000, 2 * 60 * 1000))
							// this is a minimum retry of 2 seconds for a failed node increasing to a maximum of 2 minutes
							//.WithCompression(CompressionType.Snappy)
							.WithQueryOptions(new QueryOptions().SetConsistencyLevel(consistencyLevel))
							.WithSocketOptions(new SocketOptions().SetReadTimeoutMillis(32000))
							.WithCredentials(credentials[0], credentials[1]).Build();
				}
				else
				{
					cassandraCluster =
						Cluster.Builder()
						.AddContactPoints(contactPoints)
						.WithReconnectionPolicy(new ExponentialReconnectionPolicy(2 * 1000, 2 * 60 * 1000))
						// this is a minimum retry of 2 seconds for a failed node increasing to a maximum of 2 minutes
						//.WithCompression(CompressionType.Snappy)
						.WithQueryOptions(new QueryOptions().SetConsistencyLevel(consistencyLevel))
						.WithSocketOptions(new SocketOptions().SetReadTimeoutMillis(32000))
						.Build();
				}


				try
				{
					// this is global and can be defined only once if we get an exception that equals -2147024809 then it is already defined
					MappingConfiguration.Global.Define<CassandraAandETableMappings>();
				}
				catch (ArgumentException ex)
				{
					if (ex.HResult != -2147024809) // already defined so just ignore and continue
					{
						string errorMessage = string.Format("Invalid Cassandra A and E Table Mappings Encountered. {0}", ex.Message);
						this.EventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
						throw new Exception(string.Format("Invalid Cassandra A and E Table Mappings Encountered. {0}", ex.Message));
					}
				}

				if (aandEcassandraSession == null)
				{
					aandEcassandraSession = cassandraCluster.Connect();
				}

				int replicationFactor = CassandraConnectionConfig.GetReplicationFactor(security);


				CassandraKeyspaceCreator.CreateKeySpaceIfNotExists(aandEcassandraSession, replicationFactor, KeyspaceName);
				CassandraDataTablesCreator.CreateAandETables(aandEcassandraSession, consistencyLevel);

				aandEArchiveTable = new Table<AandEDataElement>(aandEcassandraSession);
				SynchronizationTable = new Table<AlarmAndEventSynchronizationElement>(aandEcassandraSession);

				var query = "select * from \"FMArchive_Data\".synchronizationdata where tablename = 'alarmandeventarchivedata' and siteguid = " + Guid.Empty.ToString();
				var result = aandEcassandraSession.Execute(query) as RowSet;
				if (!result.Any())
				{
					var synchronizationElement = new AlarmAndEventSynchronizationElement()
					{
						TableName = "alarmandeventarchivedata",
						SiteGuid = Guid.Empty,
						LastAlarmAndEventTimeStamp = DateTimeOffset.UtcNow.AddMinutes(-1),
						NumberOfRecordsSynchronized = 0
					};
					aandEcassandraSession.Execute(SynchronizationTable.Insert(synchronizationElement));
				}
			}
			catch (Exception ex)
			{
				ShutdownIfRunning();
				string errorMessage = string.Format("AandEArchiveDatabase.Initialize Error. {0}", ex.Message);
				this.EventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
				throw new Exception(string.Format("AandEArchiveDatabase.Initialize Error. {0}", ex.Message));
			}

			finally
			{
				if (AandEsessionLock.IsWriteLockHeld)
				{
					AandEsessionLock.ExitWriteLock();
				}
			}
		}

		private static void ShutdownIfRunning()
		{
			if (cassandraCluster != null)
			{
				try
				{
					cassandraCluster.Shutdown();
					cassandraCluster = null;
				}
				catch
				{
				}
			}

			if (aandEcassandraSession != null)
			{
				try
				{
					aandEcassandraSession = null;
				}
				catch
				{
					// if we get here then the session is bad so just set at null
					aandEcassandraSession = null;
				}
			}
		}

		void IAandEArchiveDatabase.AddArchiveData(SecurityClass security, List<AandEDataElement> aAndeDataElementList)
		{
			int retryCounter = 0;
			security.ThrowIfNull("security");
			aAndeDataElementList.ThrowIfNull("AandEDataElementList");

			if (aAndeDataElementList.Count == 0)
			{
				return;
			}

			try
			{
				RetryTagDataTransmission:

				AandEsessionLock.EnterReadLock();

				if (aandEcassandraSession == null)
				{

					++retryCounter;

					AandEsessionLock.ExitReadLock();

					if (retryCounter >= 3) // give up and leave the routine
					{
						throw new Exception("Failure Initializing Cassandra Session.");
					}

					Thread.Sleep(500);   // wait 500 msec after an error
					try
					{
						((IAandEArchiveDatabase)this).Initialize(security);
					}
					catch
					{
						// do nothing since we are in a retry
					}
					goto RetryTagDataTransmission;
				}

				AandEsessionLock.ExitReadLock();

				var taskQueue = new ConcurrentQueue<Task>();


				int numberOfBatches = (aAndeDataElementList.Count / MaximumBatchSize) + 1;

				for (var iteration = 0; iteration < numberOfBatches; ++iteration)
				{
					var start = iteration * MaximumBatchSize;
					var batch = new BatchStatement();
					batch.SetBatchType(BatchType.Logged);
					batch.SetConsistencyLevel(consistencyLevel);

					for (var index = start; index < start + MaximumBatchSize; ++index)
					{
						if (index >= aAndeDataElementList.Count)
						{
							break;
						}

						var element = aAndeDataElementList[index];
						batch.Add(aandEArchiveTable.Insert(element));
					}

					if (batch.IsEmpty == false)
					{
						taskQueue.Enqueue(aandEcassandraSession.ExecuteAsync(batch));
					}
				}

				if (!this.WaitOnTasks(taskQueue))
				{
					if (retryCounter >= 3) // give up and leave the routine
					{
						throw new Exception("Failure Writing Data to Cassandra.");
					}

					++retryCounter;

					Thread.Sleep(500);   // wait 500 msec after an error
					((IAandEArchiveDatabase)this).Initialize(security);
					goto RetryTagDataTransmission;
				}
			}
			catch (Exception ex)
			{
				// log the alarm and/or event in the event log so it is not lost
				string errorMessage = string.Format("Error Writing Alarm/Event Data to Cassandra. {0}", ex.Message);
				this.EventLogging.LogEvent(errorMessage, EventLogEntryType.Error);

				// log the alarm/event data to the windows application enevt log so it is not lost
				foreach (var element in aAndeDataElementList)
				{
					string stErrorMessage = this.BuildEventLogMessage(element);
					this.EventLogging.LogEvent(stErrorMessage, EventLogEntryType.Information);
				}
			}
		}

		public string BuildEventLogMessage(AandEDataElement element)
		{
			// build up the event message
			string stReturn1 = "Element.Point = {0}, Element.PointDescription = {1}, Element.PointType = {2}, Element.Action = {3}, ";
			stReturn1 += "Element.AlarmState = {4}, Element.Comments = {5}, Element.DateAndTime = {6}, ";
			stReturn1 += "Element.RecordType = {7}, Element.Site = {8}, Element.Value = {9}, ";
			stReturn1 += "Element.Priority = {10}, Element.User = {11}";

			var stReturn = string.Format(stReturn1, element.Point, element.PointDescription, element.PointType, element.Action,
				element.AlarmState, element.Comments, element.DateAndTime, element.RecordType,
				element.Site, element.Value, element.Priority, element.User);

			return stReturn;

		}

		/// <summary>
		/// This method will retrieve the filter data for a selected column.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="selectedColumn">The column that is requesting the filtering data.</param>
		/// <param name="columnFilterInfoList">Current column filtering data.</param>
		/// <returns></returns>
		List<string> IAandEArchiveDatabase.GetColumnFilterData(SecurityClass security,
																int selectedColumn,
																List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList)
		{
			this.currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));
			List<string> tstring = new List<string>();
			int retryCounter = 0;

			RetryAandEDataRetrieval:

			AandEsessionLock.EnterReadLock();

			if (aandEcassandraSession == null)
			{

				++retryCounter;

				AandEsessionLock.ExitReadLock();

				if (retryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing AandEcassandraSession Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IAandEArchiveDatabase)this).Initialize(security);
				goto RetryAandEDataRetrieval;
			}

			AandEsessionLock.ExitReadLock();

			bool bExitProcess = false;

			// Always get 1 days of data prior to the date passed in.
			DateTimeOffset startTime = DateTimeOffset.Now.AddDays(-1);
			DateTimeOffset endTime = DateTimeOffset.Now;

			var taskList = new List<Task<RowSet>>();
			var tAandeElementList = new List<AandEDataElement>();

			var filterObj = columnFilterInfoList.Find(
								x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.DateAndTime);

			if (filterObj != null)
			{
				DateTime? fromDate = this.ConvertDateTimeStr(filterObj.FromDateStr);
				DateTime? toDate = this.ConvertDateTimeStr(filterObj.ToDateStr);

				if (fromDate != null && toDate != null)
				{
					TimeZoneInfo siteZone = this.currentSite.GetTimeZoneInfo();

					// First convert to the site's time zone.
					var startTimeSiteZone = new DateTimeOffset(fromDate.Value, siteZone.GetUtcOffset(fromDate.Value));
					var endTimeSiteZone = new DateTimeOffset(toDate.Value, siteZone.GetUtcOffset(toDate.Value));

					// Then convert to local time.
					startTime = startTimeSiteZone.ToLocalTime();
					endTime = endTimeSiteZone.ToLocalTime();
				}
			}

			while (bExitProcess == false)
			{
				// Wait on the queries to complete.
				Task.WaitAll(taskList.ToArray());

				var toProcessTaskList = this.QueueQueriesForDataSets(security.SiteGuid, startTime, endTime);

				taskList = toProcessTaskList;

				if (taskList.Count < 1)
				{
					bExitProcess = true;
				}

				foreach (var task in taskList)
				{
					var rowCount = task.Result.GetAvailableWithoutFetching();
					if (rowCount > 0)
					{
						while (!task.Result.IsFullyFetched)
						{
							task.Result.FetchMoreResults();
						}

						foreach (var row in task.Result)
						{
							var archiveDataElement = this.BuildAandEArchiveRecord(row, null, true);
							tAandeElementList.Add(archiveDataElement);
						}
					}

					task.Dispose();
				}

				bExitProcess = true;
			}

			// Get dictionary of unique AlarmTests
			var alarmTestGuidDictionary = new Dictionary<Guid, Guid>();
			foreach(var archiveDataElement in tAandeElementList)
			{
				if(alarmTestGuidDictionary.ContainsKey(archiveDataElement.AlarmTestGuid))
				{
					continue;
				}

				alarmTestGuidDictionary.Add(archiveDataElement.AlarmTestGuid, archiveDataElement.AlarmTestGuid);
			}

			AlarmTests.EnumerateRestrictedAccessByAlarmTestGuidList(security, alarmTestGuidDictionary);

			// produce new list excluding elements the user doesn't have access to.

			var archiveDataListFilteredByAccess = new List<AandEDataElement>();
			foreach (var archiveDataElement in tAandeElementList)
			{
				if (!alarmTestGuidDictionary.ContainsKey(archiveDataElement.AlarmTestGuid))
				{
					continue;
				}

				archiveDataListFilteredByAccess.Add(archiveDataElement);
			}



			// go through the returned list and populate the returned list bds
			tstring.Clear();

			// The site filter will be retrieve from the site hierarchy instead of the data.
			if (selectedColumn == (int) AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Site)
			{
				List<string> siteList = this.GetSiteHierarchy(security);

				if (siteList == null || siteList.Count == 0)
				{
					return tstring;
				}

				foreach (string siteId in siteList)
				{
					tstring.Add(siteId);
				}

				return tstring;
			}

			foreach (var archive in archiveDataListFilteredByAccess)
			{
				bool bAdd = false;
				string stTemp = string.Empty;

				switch (selectedColumn)
				{
					case (int) AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PointType:
						{
							stTemp = archive.PointType;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Point:
						{
							stTemp = archive.Point;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PointDescription:
						{
							stTemp = archive.PointDescription;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Variable:
						{
							stTemp = archive.Variable;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Value:
						{
							stTemp = archive.Value;

							// Need both the converted level and decimal value for the filter dropdown.
							if (string.IsNullOrEmpty(archive.Units) == false && (archive.Units.Equals("FML_FtIn16th") || archive.Units.Equals("FML_FtIn8th")))
							{
								string convertedLevel = this.ConvertLevelValue(archive.Units, archive.Value);
								stTemp = "LV|" + convertedLevel + "|" + archive.Value;
							}

							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Units:
						{
							stTemp = archive.Units;

							// Need both the abbreviation for drowndrop display and the actual units for value.
							if (string.IsNullOrEmpty(archive.Units) == false)
							{
								string abbrev = this.GetEngineeringUnitsAbbreviation(archive.Units);
								stTemp = "UN|" + abbrev + "|" + archive.Units;
							}

							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.AlarmState:
						{
							stTemp = archive.AlarmState;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Priority:
						{
							stTemp = archive.Priority;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Action:
						{
							stTemp = archive.Action;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.User:
						{
							stTemp = archive.User;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Comment:
						{
							stTemp = archive.Comments;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentUserName:
						{
							stTemp = archive.CommentUser;
							bAdd = true;
							break;
						}
					case (int)AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentDateTime:
						{
							stTemp = archive.CommentDateTime.ToString();
							bAdd = true;
							break;
						}
					default:
						{
							break;
						}
				}

				if (bAdd)
				{
					bool bFound = false;

					foreach (var datastring in tstring)
					{
						if (!string.IsNullOrEmpty(stTemp) && stTemp != "null" && stTemp.Length > 0)
						{
							if (string.Compare(datastring, stTemp, StringComparison.CurrentCultureIgnoreCase) == 0)
							{
								bFound = true;
							}
						}
						else
						{
							bFound = true;
						}
					}

					if (bFound == false)
					{
						if (!string.IsNullOrEmpty(stTemp))
						{
							tstring.Add(stTemp);
						}
					}
				}
			}

			return tstring;
		}

		/// <summary>
		/// This method will retrieve the alarm history data based on the column filtering.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="columnFilterInfoList">Current column filtering data.</param>
		/// <param name="recordType">Record type for filtering (alarm, event, alarm & event).</param>
		/// <returns>Returns the alarm history requested data.</returns>
		public List<AandEDataElement> GetAandEArchiveData(SecurityClass security, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, int recordTypeFilter)
		{
			List<AandEDataElement> archiveDataList = new List<AandEDataElement>();

			List<AandEDataElement> archiveDataListFilteredByAccess = new List<AandEDataElement>();
			Dictionary<Guid,Guid > alarmTestGuidDictionary = new Dictionary<Guid,Guid>();

			Dictionary<PointValueIdentifier, PointValueIdentifier> pointValueIdentifierDictionary = new Dictionary<PointValueIdentifier, PointValueIdentifier>();

			var sites = new SitesClass();
			this.currentSite = sites.Get(security, security.SiteGuid, false, false, false);

			int retryCounter = 0;
			RetryAandEDataRetrieval:

			AandEsessionLock.EnterReadLock();

			if (aandEcassandraSession == null)
			{
				++retryCounter;

				AandEsessionLock.ExitReadLock();

				if (retryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing AandEcassandraSession Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IAandEArchiveDatabase)this).Initialize(security);
				goto RetryAandEDataRetrieval;
			}

			AandEsessionLock.ExitReadLock();

			bool bExitProcess = false;

			// always get 30 days of data prior to the date passed in
			DateTimeOffset startTime = DateTimeOffset.Now.AddDays(-30);
			DateTimeOffset endTime = DateTimeOffset.Now;

			var taskList = new List<Task<RowSet>>();

			var filterObj = columnFilterInfoList.Find(
								x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.DateAndTime);

			if (filterObj != null)
			{
				DateTime? fromDate = this.ConvertDateTimeStr(filterObj.FromDateStr);
				DateTime? toDate = this.ConvertDateTimeStr(filterObj.ToDateStr);

				if (fromDate != null && toDate != null)
				{
					TimeZoneInfo siteZone = this.currentSite.GetTimeZoneInfo();

					// First convert to the site's time zone.
					var startTimeSiteZone = new DateTimeOffset(fromDate.Value, siteZone.GetUtcOffset(fromDate.Value));
					var endTimeSiteZone = new DateTimeOffset(toDate.Value, siteZone.GetUtcOffset(toDate.Value));

					// Then convert to local time.
					startTime = startTimeSiteZone.ToLocalTime();
					endTime = endTimeSiteZone.ToLocalTime();
				}
			}

			// Update the column filter with site hierarchy info.
			this.UpdateColumnWithSiteInfo(security, columnFilterInfoList);

			while (bExitProcess == false)
			{
				// Wait on the queries to complete.
				Task.WaitAll(taskList.ToArray());

				var toProcessTaskList = this.QueueQueriesForDataSets(security.SiteGuid, startTime, endTime);

				taskList = toProcessTaskList;

				if (taskList.Count < 1)
				{
					bExitProcess = true;
				}

				foreach (var task in taskList)
				{
					var rowCount = task.Result.GetAvailableWithoutFetching();

					if (rowCount > 0)
					{
						while (!task.Result.IsFullyFetched)
						{
							task.Result.FetchMoreResults();
						}

						foreach (var row in task.Result)
						{
							var archiveDataElement = this.BuildAandEArchiveRecord(row, columnFilterInfoList, false);

							// A null archive data element means it did not match the filter criterion.
							if (archiveDataElement != null)
							{
								// Filter based on Record Type (Alarm, Event, or Alarm & Event)
								bool addRecord = this.FilterOnRecordType(archiveDataElement.RecordType, recordTypeFilter);

								if (addRecord)
								{
									archiveDataList.Add(archiveDataElement);

									if (archiveDataElement.RecordType == (int)RecordTypes.Alarm)
									{
										if (alarmTestGuidDictionary.ContainsKey(archiveDataElement.AlarmTestGuid) == false)
										{
											alarmTestGuidDictionary.Add(archiveDataElement.AlarmTestGuid, archiveDataElement.AlarmTestGuid);
										}
									}
									else
									{
										PointValueIdentifier pointValueIdentifier = new PointValueIdentifier(archiveDataElement.AlarmOrTagGuid, PointValueType.Tag, "");
										if (pointValueIdentifierDictionary.ContainsKey(pointValueIdentifier) == false)
										{
											pointValueIdentifierDictionary.Add(pointValueIdentifier, pointValueIdentifier);
										}
									}
								}
							}
						}
					}

					task.Dispose();
				}

				bExitProcess = true;
			}

			// filter archive list based upon use access
			AlarmTests.EnumerateRestrictedAccessByAlarmTestGuidList(security, alarmTestGuidDictionary);

			Points points = new Points();

			var pointAccessDictionary = points.EnumerateRestrictedAccessByPointValueIdenfierList(security, pointValueIdentifierDictionary.Values.ToList());

			foreach (var archiveDataElement in archiveDataList)
			{
				// Only check the alarm test GUID if the record type is alarm.
				if (archiveDataElement.RecordType == (int)RecordTypes.Alarm)
				{
					if (alarmTestGuidDictionary.ContainsKey(archiveDataElement.AlarmTestGuid) == false)
					{
						continue;
					}
				}
				else
				{
					PointValueIdentifier pointValueIdentifier = new PointValueIdentifier(archiveDataElement.AlarmOrTagGuid, PointValueType.Tag, "");

					if (pointAccessDictionary.ContainsKey(pointValueIdentifier))
					{
						var access = pointAccessDictionary[pointValueIdentifier];

						if (!access.View
						&& !access.Modify)
						{
							continue;
						}
					}
				}

				archiveDataListFilteredByAccess.Add(archiveDataElement);
			}

			return archiveDataListFilteredByAccess;
		}

		/// <summary>
		/// This method filters the archive element record based on the record type filter
		/// (Alarm, Event, or Alarm & Event).
		/// </summary>
		/// <param name="elementRecordType">The archive element record type.</param>
		/// <param name="recordTypeFilter">The filter record type.</param>
		/// <returns>Returns true if the record is to be added. Otherwise, it return false.</returns>
		private bool FilterOnRecordType(int elementRecordType, int recordTypeFilter)
        {
			bool addRecord = false;

			if(recordTypeFilter < (int)RecordTypes.Alarm || recordTypeFilter >= (int)RecordTypes.AlarmAndEvent)
			{
				return true;
            }

			if(elementRecordType < (int)RecordTypes.Alarm || elementRecordType > (int)RecordTypes.Event)
            {
				return true;
            }

			if(elementRecordType == recordTypeFilter)
            {
				addRecord = true;
            }

			return addRecord;
        }

		/// <summary>
		/// This method will update the column filter info list with the site hierarchy if there is no
		/// site filter.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="columnFilterInfoList">The column filter info that contains the filters.</param>
		private void UpdateColumnWithSiteInfo(SecurityClass security, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList)
		{
			foreach (AlarmHistoryTabColumnFilterInfo filterItem in columnFilterInfoList)
			{
				// Add site hierarchy filtering if the user did not select a site filter.
				if (filterItem.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Site && filterItem.FilterCollection.Count == 0)
				{
					List<string> siteList = this.GetSiteHierarchy(security);

					if (siteList == null)
					{
						return;
					}

					foreach (string siteId in siteList)
					{
						filterItem.FilterCollection.Add(siteId);
					}
				}
			}
		}

		/// <summary>
		/// This method will return a list of site under the current.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Return the site and its children.</returns>
		private List<string> GetSiteHierarchy(SecurityClass security)
		{
			if (this.currentSite == null)
			{
				return null;
			}

			var siteList = new List<string>();
			this.siteHierarchyList = new List<SiteClass> { this.currentSite };

			if (this.currentSite.SiteGroup)
			{
				this.HelperSiteHierarchy(this.currentSite, security);
			}

			foreach (SiteClass siteItem in this.siteHierarchyList)
			{
				siteList.Add(siteItem.ID);
			}

			return siteList;
		}

		/// <summary>
		/// This method will traverse the site the sites to get a list of all children sites.
		/// This is a recursive method.
		/// </summary>
		/// <param name="nextSiteGroup">The next site group that may have childern sites.</param>
		/// <param name="security">The security object.</param>
		private void HelperSiteHierarchy(SiteClass nextSiteGroup, SecurityClass security)
		{
			bool moreSiteGroups = false;

			var childSiteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.EnumerateByParentSite(security, nextSiteGroup.IdentityGuid));

			foreach (SiteClass childSite in childSiteCollection)
			{
				var foundSite = this.siteHierarchyList.Find(x => x.ID.ToUpper() == childSite.ID.ToUpper());

				if (foundSite == null)
				{
					this.siteHierarchyList.Add(childSite);
				}

				if (childSite.SiteGroup)
				{
					moreSiteGroups = true;
				}
			}

			if (moreSiteGroups)
			{
				foreach (SiteClass childSite in childSiteCollection)
				{
					if (childSite.SiteGroup)
					{
						this.HelperSiteHierarchy(childSite, security);
					}
				}
			}
		}

		/// <summary>
		/// This method will
		/// </summary>
		/// <param name="columnFilterInfoList">Current column filtering data.</param>
		/// <param name="element">The alarm history record to filter on.</param>
		/// <returns>Returns the alarm history record if it matches the filter, otherwise it returns null.</returns>
		private AandEDataElement FilterData(List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, AandEDataElement element)
		{
			AandEDataElement newElement = null;

			foreach (AlarmHistoryTabColumnFilterInfo filterItem in columnFilterInfoList)
			{
				if (filterItem.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.DateAndTime)
				{
					continue;
				}

				newElement = this.FilterAandEDataSet(filterItem, columnFilterInfoList, element);

				if (newElement == null)
				{
					break;
				}
			}

			return newElement;
		}

		/// <summary>
		/// This method will filter the alarm history record to ensure it is matches the filter
		/// criterion.
		/// </summary>
		/// <param name="filterItem">The filter item that contains the filter information.</param>
		/// <param name="columnFilterInfoList">The filter info list that contains all the filter information.</param>
		/// <param name="element">The alarm history record to filter on.</param>
		/// <returns>Returns null if the alarm history record does not match the criterion, otherwise it returns the record.</returns>
		private AandEDataElement FilterAandEDataSet(AlarmHistoryTabColumnFilterInfo filterItem,
													List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList,
													AandEDataElement element)
		{
			switch (filterItem.SelectedColumnFilterEnum)
			{
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.DateAndTime:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.DateAndTime);

						if (filterObj != null)
						{
							DateTime? fromDate = this.ConvertDateTimeStr(filterObj.FromDateStr);
							DateTime? toDate = this.ConvertDateTimeStr(filterObj.ToDateStr);

							if (fromDate == null || toDate == null)
							{
								return element;
							}

							// Compare the dates. From date is the oldest and To date is the must current.
							DateTime? rowDateTime = element.DateAndTime.LocalDateTime;

							if (toDate >= rowDateTime.Value && rowDateTime.Value >= fromDate)
							{
								return element;
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Site:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Site);

						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Site.ToUpper() == item.ToUpper())
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
				
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PointType:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PointType);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.PointType == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}

						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Point:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Point);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Point == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}

						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PointDescription:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PointDescription);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.PointDescription == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}

						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Variable:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Variable);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Variable == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Value:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Value);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Value == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Units:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Units);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Units == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.AlarmState:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.AlarmState);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.AlarmState == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Priority:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Priority);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Priority == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Action:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Action);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Action == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.User:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.User);
						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.User == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Comment:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Comment);

						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.Comments == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentUserName:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentUserName);

						if (filterObj != null && filterObj.FilterCollection.Count > 0)
						{
							foreach (string item in filterObj.FilterCollection)
							{
								if (element.CommentUser == item)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}
				case AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentDateTime:
					{
						var filterObj = columnFilterInfoList.Find(
											x => x.SelectedColumnFilterEnum == AlarmHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentDateTime);

						if (filterObj != null)
						{
							DateTime? commentFromDate = this.ConvertDateTimeStr(filterObj.CommentFromDateStr);
							DateTime? commentToDate = this.ConvertDateTimeStr(filterObj.CommentToDateStr);

							if (commentFromDate == null || commentToDate == null)
							{
								return element;
							}

							// Compare the dates. From date is the oldest and To date is the must current.
							DateTime? rowDateTime = element.CommentDateTime.LocalDateTime;

							if (rowDateTime != null)
							{
								TimeZoneInfo siteZone = this.currentSite.GetTimeZoneInfo();

								// First convert to the site's time zone.
								var fromTimeSiteZone = new DateTimeOffset(commentFromDate.Value, siteZone.GetUtcOffset(commentFromDate.Value));
								var toTimeSiteZone = new DateTimeOffset(commentToDate.Value, siteZone.GetUtcOffset(commentToDate.Value));

								// Then convert to local time.
								DateTimeOffset commentFromLocalDate = fromTimeSiteZone.ToLocalTime();
								DateTimeOffset commentToLocalDate = toTimeSiteZone.ToLocalTime();

								if (commentToLocalDate >= rowDateTime.Value && rowDateTime.Value >= commentFromLocalDate)
								{
									return element;
								}
							}
						}
						else
						{
							return element;
						}
						break;
					}

				default:
					return element;
			}

			return null;
		}

		/// <summary>
		/// This method will convert the archive unit name which is actually the engineering
		/// unit enum name and return the appropriate unit abbreviation.  If not found it 
		/// will return the input name.
		/// </summary>
		/// <param name="archiveUnitName">This is the enum to string value.</param>
		/// <returns>Returns the real unit name.</returns>
		private string GetEngineeringUnitsAbbreviation(string archiveUnitName)
		{
			if (string.IsNullOrEmpty(archiveUnitName))
			{
				return archiveUnitName;
			}

			EngineeringUnit unitEnumIndex;

			if (Enum.TryParse(archiveUnitName, out unitEnumIndex))
			{
				return EngineeringUnits.GetUnitAbbreviation(unitEnumIndex);
			}

			return archiveUnitName;
		}

		/// <summary>
		/// This method will convert a double to a level in feet/inches/16th or 8th.
		/// </summary>
		/// <param name="archiveUnitName"></param>
		/// <param name="archiveValue"></param>
		/// <returns>Returns a string containing the level.</returns>
		private string ConvertLevelValue(string archiveUnitName, string archiveValue)
		{
			if (string.IsNullOrEmpty(archiveUnitName) || string.IsNullOrEmpty(archiveValue))
			{
				return archiveValue;
			}

			double archiveValueDouble;

			if (double.TryParse(archiveValue, out archiveValueDouble) == false)
			{
				return archiveValue;
			}

			EngineeringUnit unitEnumIndex;

            //If the unitEnumIndex is not ft-in-16th or ft-in-8th then no need to format.
            if (Enum.TryParse(archiveUnitName, out unitEnumIndex) == false ||
                (unitEnumIndex != EngineeringUnit.FmlFtIn8Th &&
                 unitEnumIndex != EngineeringUnit.FmlFtIn16Th))
			{
				return archiveValue;
			}

            return EngineeringUnitsHelperClass.FormatValue(archiveValueDouble, unitEnumIndex).ToString();

		}

		/// <summary>
		/// This method will convert the date string based on the site regional settings.
		/// </summary>
		/// <param name="dateStr">The date string to convert.</param>
		/// <returns></returns>
		private DateTime? ConvertDateTimeStr(string dateStr)
		{
			if (string.IsNullOrEmpty(dateStr) == false && dateStr.Length >= 14)
			{
				string dateTimeFormat = this.currentSite.ShortDatePattern + " " + this.currentSite.TimePattern;
				var mainParts = dateStr.Split(' ');

				if (mainParts.Length == 3)
				{
					if (mainParts[2].Equals(this.currentSite.PMSymbol))
					{
						mainParts[2] = "PM";
					}

					if (mainParts[2].Equals(this.currentSite.AMSymbol))
					{
						mainParts[2] = "AM";
					}
				}

				if (mainParts.Length >= 2)
				{
					mainParts[0]		= mainParts[0].Replace(this.currentSite.DateSeparator, "/");
					mainParts[1]		= mainParts[1].Replace(this.currentSite.TimeSeparator, ":");
					string newDateStr	= mainParts[0] + " " + mainParts[1];

					if (mainParts.Length == 3)
					{
						newDateStr = newDateStr + " " + mainParts[2];
					}

					try
					{
						var newDateTime = DateTime.ParseExact(newDateStr, dateTimeFormat, CultureInfo.InvariantCulture);
						return newDateTime;
					}
					catch (Exception)
					{		
						return null;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// This method will build the archive data element from the alarm history database record.
		/// </summary>
		/// <param name="row">The database record row.</param>
		/// <param name="columnFilterInfoList">The filter information list that contains all the filter info.</param>
		/// <param name="ignoreFiltering">True = ignore filtering; False = apply filtering.</param>
		/// <returns></returns>
		private AandEDataElement BuildAandEArchiveRecord(Row row, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, bool ignoreFiltering)
		{
			// we need to check each string as we do this. it is combursion but necessary
			// to prevent strange screen displays when a null is encountered
			var archiveDataElement = new AandEDataElement();

			string stTemp = row["a"] as string;
			archiveDataElement.PointDescription = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			archiveDataElement.AlarmOrTagGuid = (Guid)row["b"];
			archiveDataElement.AlarmTestGuid = (Guid)row["c"];

			stTemp = row["d"] as string;
			archiveDataElement.Point = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["e"] as string;
			archiveDataElement.Site = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			// Column "F" was removed from the database;

			archiveDataElement.DateAndTime = (DateTimeOffset)row["g"];

			stTemp = row["h"] as string;
			archiveDataElement.AlarmState = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["i"] as string;
			archiveDataElement.PointType = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["j"] as string;
			archiveDataElement.Variable = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["k"] as string;
			archiveDataElement.Value = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["l"] as string;
			archiveDataElement.Units = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["m"] as string;
			archiveDataElement.Priority = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["n"] as string;
			archiveDataElement.Action = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["o"] as string;
			archiveDataElement.User = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			stTemp = row["p"] as string;
			archiveDataElement.Comments = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			archiveDataElement.RecordType = (int)row["q"];
			archiveDataElement.RecordGuid = (Guid)row["r"];

			stTemp = row["s"] as string;
			archiveDataElement.CommentUser = string.IsNullOrEmpty(stTemp) ? string.Empty : stTemp;

			archiveDataElement.CommentDateTime = (DateTimeOffset)row["t"];

			if (ignoreFiltering)
			{
				return archiveDataElement;
			}

			return this.FilterData(columnFilterInfoList, archiveDataElement);
		}

		protected List<Task<RowSet>> QueueQueriesForDataSets(Guid siteGuid, DateTimeOffset intervalStart, DateTimeOffset intervalStop)
		{
			var taskList = new List<Task<RowSet>>();

			var intervalBegin= intervalStart.UtcDateTime;

			var siteGuidString = siteGuid.ToString();

			while (intervalBegin < intervalStop.UtcDateTime)
			{

				var intervalEnd = intervalBegin.Date.AddDays(1.0);
				if(intervalEnd > intervalStop.UtcDateTime)
				{
					intervalEnd = intervalStop.UtcDateTime;
				}

				string partition = (intervalBegin.Year * 10000 + intervalBegin.Month * 100 + intervalBegin.Day).ToString(CultureInfo.InvariantCulture);

				const string Query = "select q, c, r, a, b, d, e, f, g, h, i, j, k, l, m, n, o, p, s, t, u from \"FMAandEArchive_Data\".archivedata where f = {0} and u = {1} and g >= '{2}{3}' and g <= '{4}{5}'";

				var task1 = aandEcassandraSession.ExecuteAsync(new SimpleStatement(string.Format(Query, siteGuidString, partition, intervalBegin.ToString("yyyy-MM-dd HH:mm:ss.fff"), "+0000", intervalEnd.ToString("yyyy-MM-dd HH:mm:ss.fff"), "+0000" )).SetConsistencyLevel(ConsistencyLevel.One));
				taskList.Add(task1);
				intervalBegin = intervalEnd;
			}

			return taskList;
		}

		protected bool WaitOnTasks(ConcurrentQueue<Task> taskQueue)
		{
			if (taskQueue.Count > 0)
			{
				var taskList = new List<Task>(taskQueue.Count);
				Task task;

				while (taskQueue.TryDequeue(out task))
				{
					taskList.Add(task);
				}

				try
				{
					Task.WaitAll(taskList.ToArray());
					foreach (var completedTask in taskList)
					{
						completedTask.Dispose();
					}
				}
				catch (Exception)
				{
					return false;
				}
			}

			return true;
		}

		Tuple<string, DateTimeOffset> IAandEArchiveDatabase.UpdateAandEComment(SecurityClass security,
																DateTimeOffset timeStamp,
																Guid alarmAndEventRecordGuid,
																string comment)
		{
			int retryCounter = 0;

			RetryAandEDataRetrieval:

			AandEsessionLock.EnterReadLock();

			if (aandEcassandraSession == null)
			{
				++retryCounter;

				AandEsessionLock.ExitReadLock();

				if (retryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing AandEcassandraSession Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IAandEArchiveDatabase)this).Initialize(security);
				goto RetryAandEDataRetrieval;
			}

			AandEsessionLock.ExitReadLock();

			DateTimeOffset tNow = DateTimeOffset.UtcNow;
			var timeStampUtc = timeStamp.UtcDateTime;

			string query = "UPDATE \"FMAandEArchive_Data\".archivedata ";
			query += "SET p = '{0}', s = '{1}', t = '{2}' ";
			query += "WHERE f = {3} and u = {4} and g = '{5}{6}' and r = {7} IF EXISTS;";
			aandEcassandraSession.Execute(new SimpleStatement(string.Format(query, 
																				comment, 
																				security.UserID,
																				tNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"),
																				security.SiteGuid.ToString(),
																				(timeStampUtc.Year * 10000 + timeStampUtc.Month * 100 + timeStampUtc.Day).ToString(CultureInfo.InvariantCulture),
																				timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
																				"+0000",
																				alarmAndEventRecordGuid.ToString())));

			return new Tuple<string, DateTimeOffset>(security.UserID, tNow);
		}

		List<AandEDataElement> IAandEArchiveDatabase.GetArchiveData(SecurityClass security, DateTimeOffset startDateTimeOffset, Guid siteGuid, out bool moreData, out AlarmAndEventSynchronizationElement synchronizationElement)
		{
			var maxArchiveRecords = 50000;

			// 1 hour
			var interval = 3600;

			security.ThrowIfNull("security");

			int RetryCounter = 0;

			RetryInitialize:

			AandEsessionLock.EnterReadLock();

			if (aandEcassandraSession == null)
			{

				++RetryCounter;

				AandEsessionLock.ExitReadLock();

				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing Cassandra Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IAandEArchiveDatabase)this).Initialize(security);
				goto RetryInitialize;
			}

			AandEsessionLock.ExitReadLock();

			try
			{

				synchronizationElement = SynchronizationTable.Where(u => u.TableName == "alarmandeventarchivedata" && u.SiteGuid == siteGuid)
						.FirstOrDefault()
						.Execute();

				if (synchronizationElement == null)
				{
					synchronizationElement = SynchronizationTable.Where(u => u.TableName == "alarmandeventarchivedata" && u.SiteGuid == Guid.Empty)
							.FirstOrDefault()
							.Execute();

					if (synchronizationElement == null)
					{
						throw new Exception("Failed to retreive AlarmAndEventArchiveData SynchronizationElement");
					}
				}

				synchronizationElement.NumberOfRecordsSynchronized = 0;

				moreData = false;

				var archiveDataElementList = new List<AandEDataElement>();
				var endTime = synchronizationElement.LastAlarmAndEventTimeStamp;
				endTime = endTime.AddSeconds(interval);

				if (endTime > startDateTimeOffset)
				{
					endTime = startDateTimeOffset;
				}

				var query = "select * from \"FMAandEArchive_Data\".archivedata where f = {0} and u in ({1}) and g > '"
								+ synchronizationElement.LastAlarmAndEventTimeStamp.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")
								+ "' and g <= '"
								+ endTime.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";

				var iterations = 0;

				while (true)
				{
					string datePartitionKeys = "";
					datePartitionKeys = AandEDataElement.GetPartition(synchronizationElement.LastAlarmAndEventTimeStamp).ToString();

					if (synchronizationElement.LastAlarmAndEventTimeStamp.Day != endTime.Day)
					{
						datePartitionKeys += ", " + AandEDataElement.GetPartition(endTime).ToString();
					}

					var result = aandEArchiveTable.GetSession().Execute(string.Format(query, siteGuid, datePartitionKeys)) as RowSet;

					// There is a problem if maxRows encountered because it may not contain a complete set
					var rowCount = result.GetAvailableWithoutFetching();

					if (rowCount > 0)
					{
						while (!result.IsFullyFetched)
						{
							result.FetchMoreResults();
						}

						foreach (var row in result)
						{
							var archiveDataElement = new AandEDataElement()
							{
								PointDescription = row["a"] as string,
								AlarmOrTagGuid = (Guid) row["b"],
								AlarmTestGuid = (Guid) row["c"],
								Point = row["d"] as string,
								Site = row["e"] as string,
								SiteGuid = (Guid) row["f"],
								DateAndTime = (DateTimeOffset) row["g"],
								AlarmState = row["h"] as string,
								PointType = row["i"] as string,
								Variable = row["j"] as string,
								Value = row["k"] as string,
								Units = row["l"] as string,
								Priority = row["m"] as string,
								Action = row["n"] as string,
								User = row["o"] as string,
								Comments = row["p"] as string,
								RecordType = (Int32) row["q"],
								RecordGuid = (Guid) row["r"],
								CommentUser = row["s"] as string,
								CommentDateTime = (DateTimeOffset) row["t"]
							};

							archiveDataElementList.Add(archiveDataElement);
						}
					}

					synchronizationElement.SiteGuid = siteGuid;
					synchronizationElement.LastAlarmAndEventTimeStamp = endTime;

					if (archiveDataElementList.Count > maxArchiveRecords)
					{
						break;
					}

					if (endTime >= startDateTimeOffset)
					{
						break;
					}

					endTime = endTime.AddSeconds(interval);

					if (endTime > startDateTimeOffset)
					{
						endTime = startDateTimeOffset;
					}

					// only advance 6 iterations to allow for stop synchronization and not exceed 30 minute timeout.
					iterations++;
					if (iterations > 6)
					{
						moreData = true;
						break;
					}


					query = "select * from \"FMAandEArchive_Data\".archivedata where f = {0} and u in ({1}) and g > '"
								+ synchronizationElement.LastAlarmAndEventTimeStamp.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")
								+ "' and g <= '"
								+ endTime.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
				}

				if (archiveDataElementList.Count > 0)
				{
					moreData = true;
				}

				synchronizationElement.NumberOfRecordsSynchronized = archiveDataElementList.Count;

				return archiveDataElementList;
			}
			catch (Exception e)
			{

				if (e.Message == "Keyspace 'FMAandEArchive_Data' does not exist"
				|| e.Message.Contains("All hosts tried for query failed "))
				{
					aandEcassandraSession.Dispose();
					aandEcassandraSession = null;
					goto RetryInitialize;
				}

				throw e;
			}
		}

		/// <summary>
		/// Synchronizations the complete.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="synchronizationElement">The synchronization element.</param>
		void IAandEArchiveDatabase.SynchronizationComplete(
			SecurityClass security,
			AlarmAndEventSynchronizationElement synchronizationElement)
		{
			if (aandEcassandraSession != null)
			{
				aandEcassandraSession.Execute(SynchronizationTable.Insert(synchronizationElement));
			}
		}
	}
}