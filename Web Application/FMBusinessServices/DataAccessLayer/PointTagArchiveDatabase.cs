namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Threading;
	using System.Diagnostics;

	using Cassandra;
	using Cassandra.Data.Linq;
	using Cassandra.Mapping;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using InternalInterfaces;
	using ServiceClasses;
	using System.Configuration;

	using FMBusinessServices.InternalClasses;

	using FMCore;

	internal class ArchiveElementComparer : IComparer<ArchiveDataElement>
	{
		public int Compare(ArchiveDataElement a, ArchiveDataElement b)
		{
			if (a.ValueTimeStamp > b.ValueTimeStamp)
			{
				return 1;
			}
			else if (a.ValueTimeStamp < b.ValueTimeStamp)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
	}

	internal class PointTagArchiveDatabase : IPointTagArchiveDatabase
	{
		const int SecondsPerDay = 86400;

		const long TicksPerSecond = 10000000;

		const long maximumAsynchronousQueries = 1000;

		private static readonly int MaximumBatchSize = 20;

		private static readonly int MaxLockDelay = 10000;

		public static Cassandra.ConsistencyLevel consistencyLevel;

		private static readonly ICassandraKeyspaceCreator CassandraKeyspaceCreator = new CassandraKeyspaceCreator();

		private static readonly ICassandraDataTablesCreator CassandraDataTablesCreator =
				new CassandraDataTablesCreator();

		private static readonly ICassandraConnectionConfig CassandraConnectionConfig = new CassandraConnectionConfig();

		private static Cluster cassandraCluster;

		private static ISession cassandraSession;

		private static ReaderWriterLockSlim sessionLock = new ReaderWriterLockSlim();

		private static Table<ArchiveDataElement> ValueArchiveTable;

		private static Table<AlarmDataElement> AlarmArchiveTable;

		private static Table<SynchronizationElement> SynchronizationTable; 

		protected EventLogging eventLogging;

		public static string KeyspaceName = "FMArchive_Data";

		void IPointTagArchiveDatabase.Initialize(SecurityClass security)
		{
			security.ThrowIfNull("security");

			if(!sessionLock.TryEnterWriteLock(MaxLockDelay))
			{
				throw new Exception("PointTagArchiveDatabase : Initialize - timeout acquiring write lock");
			}

			try
			{
				this.eventLogging = new EventLogging();

				ShutdownIfRunning();

				var contactPoints = CassandraConnectionConfig.GetContactPoints(security);
				var credentials = CassandraConnectionConfig.GetCredentials(security);

				try
				{
					consistencyLevel = (Cassandra.ConsistencyLevel)Enum.Parse(typeof(Cassandra.ConsistencyLevel), CassandraConnectionConfig.GetConsistencyLevel(security));
				}
				catch (Exception ex)
				{
					string errorMessage = string.Format("Invalid Consistency Level Configured. Defaulting to a Level of One. {0}", ex.Message);
					this.eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
					consistencyLevel = Cassandra.ConsistencyLevel.One;
				}

				if (!(String.IsNullOrEmpty(credentials[0])  || String.IsNullOrEmpty(credentials[1])))
				{
					cassandraCluster =
						Cluster.Builder()
							.AddContactPoints(contactPoints)
							.WithReconnectionPolicy(new ExponentialReconnectionPolicy(2 * 1000, 2 * 60 * 1000))
							// this is a minimum retry of 2 seconds for a failed node increasing to a maximum of 2 minutes
							//.WithCompression(CompressionType.Snappy)
							.WithQueryOptions(new QueryOptions().SetConsistencyLevel(consistencyLevel))
							.WithSocketOptions(new SocketOptions().SetReadTimeoutMillis(0))
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
						.WithSocketOptions(new SocketOptions().SetReadTimeoutMillis(0))
						.Build();
				}


				try
				{
					// this is global and can be defined only once if we get an exception that equals -2147024809 then it is already defined
					MappingConfiguration.Global.Define<CassandraArchiveTableMappings>();
				}
				catch (ArgumentException ex)
				{
					if (ex.HResult != -2147024809) // already defined so just ignore and continue
					{
						string errorMessage = string.Format("Invalid Cassandra Archive Table Mappings Encountered. {0}", ex.Message);
						this.eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
						throw new Exception(string.Format("Invalid Cassandra Archive Table Mappings Encountered. {0}", ex.Message));
					}
				}

				if (cassandraSession == null)
				{
					cassandraSession = cassandraCluster.Connect();
				}

				int ReplicationFactor = CassandraConnectionConfig.GetReplicationFactor(security);


				CassandraKeyspaceCreator.CreateKeySpaceIfNotExists(cassandraSession, ReplicationFactor, KeyspaceName);
				CassandraDataTablesCreator.CreateArchiveTables(cassandraSession, consistencyLevel);

				ValueArchiveTable = new Table<ArchiveDataElement>(cassandraSession);
				AlarmArchiveTable = new Table<AlarmDataElement>(cassandraSession);
				SynchronizationTable = new Table<SynchronizationElement>(cassandraSession);

				var query = "select * from \"FMArchive_Data\".synchronizationdata where tablename = 'valuearchivedata' and siteguid = " + Guid.Empty.ToString();
				var result = cassandraSession.Execute(query) as RowSet;
				if (!result.Any())
				{
					var synchronizationElement = new SynchronizationElement()
					{
						TableName = "valuearchivedata",
						SiteGuid = Guid.Empty,
						LastValueTimeStamp = DateTimeOffset.UtcNow.AddMinutes(-1),
						LastPointValueGuid = Guid.Empty,
						LastPointValuePropertyID = null,
						NumberOfRecordsSynchronized = 0
					};
					cassandraSession.Execute(SynchronizationTable.Insert(synchronizationElement));
				}
			}
			catch (Exception ex)
			{
				ShutdownIfRunning();
				string errorMessage = string.Format("PointTagArchiveDatabase.Initialize Error. {0}", ex.Message);
				this.eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
				throw new Exception(string.Format("PointTagArchiveDatabase.Initialize Error. {0}", ex.Message));
			}

			finally
			{
				if (sessionLock.IsWriteLockHeld)
				{
					sessionLock.ExitWriteLock();
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

			if (cassandraSession != null)
			{
				try
				{
					cassandraSession = null;
				}
				catch
				{
					// if we get here then the session is bad so just set at null
					cassandraSession = null;
				}
			}
		}


		void IPointTagArchiveDatabase.AddArchiveData(
				SecurityClass security,
				List<ArchiveDataElement> archiveDataElementList)
		{

			int RetryCounter = 0;
			security.ThrowIfNull("security");
			archiveDataElementList.ThrowIfNull("archiveDataElementList");

			if (archiveDataElementList.Count == 0)
			{
				return;
			}

			var sites = new SitesClass();

			var maxArchiveDictionary = new Dictionary<Guid, int>();

			RetryTagDataTransmission:

			sessionLock.EnterReadLock();

			if (cassandraSession == null)
			{

				++RetryCounter;

				sessionLock.ExitReadLock();

				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing Cassandra Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTagDataTransmission;
			}

			sessionLock.ExitReadLock();

			var taskQueue = new ConcurrentQueue<Task>();

			var archiveBatch = new BatchStatement();
			archiveBatch.SetBatchType(BatchType.Logged);
			archiveBatch.SetConsistencyLevel(consistencyLevel);

			var batchCount = 0;

			for (var index = 0; index < archiveDataElementList.Count; index++)
			{
				// Add ValueDataElement
				var element = archiveDataElementList[index];

				int maximumDaysToArchive = 365;

				if (!maxArchiveDictionary.TryGetValue(element.SiteGuid, out maximumDaysToArchive))
				{
					var site = sites.GetBasic(security, element.SiteGuid);

					if (site != null)
					{
						maximumDaysToArchive = site.maximumDaysToRetainArchive;
						maxArchiveDictionary.Add(element.SiteGuid, maximumDaysToArchive);
					}
				}


				archiveBatch.Add(ValueArchiveTable.Insert(element).SetTTL(maximumDaysToArchive*SecondsPerDay));
				batchCount++;

				if (batchCount == MaximumBatchSize)
				{
					taskQueue.Enqueue(cassandraSession.ExecuteAsync(archiveBatch));
					batchCount = 0;

					archiveBatch = new BatchStatement();
					archiveBatch.SetBatchType(BatchType.Logged);
					archiveBatch.SetConsistencyLevel(consistencyLevel);
				}

				// Add AlarmDataElemnent
				if (element.AlarmOrStatusChanged)
				{
					archiveBatch.Add(AlarmArchiveTable.Insert(new AlarmDataElement(element)).SetTTL(maximumDaysToArchive * SecondsPerDay));
					batchCount++;

					if (batchCount == MaximumBatchSize)
					{
						taskQueue.Enqueue(cassandraSession.ExecuteAsync(archiveBatch));
						batchCount = 0;

						archiveBatch = new BatchStatement();
						archiveBatch.SetBatchType(BatchType.Logged);
						archiveBatch.SetConsistencyLevel(consistencyLevel);
					}
				}
			}

			if (archiveBatch.IsEmpty == false)
			{
				taskQueue.Enqueue(cassandraSession.ExecuteAsync(archiveBatch));
			}

			if (!this.WaitOnTasks(taskQueue))
			{
				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Writing Data to Cassandra.");
				}

				++RetryCounter;

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTagDataTransmission;
			}
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
					foreach(var completedTask in taskList)
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


		protected List<Task<RowSet>> QueueQueriesForPen(Guid tagGuid, ref DateTime intervalStart, ref DateTimeOffset intervalEnd, DateTimeOffset end, double intervalInSeconds)
		{
			var taskList = new List<Task<RowSet>>();

			var valuequery = "select d, e, f, g, h, i, j, k, l, m from valuearchivedata where a = {0} and b = '{1}' and c = {2} and f > '{3}' and f <= '{4}'"
								+ " ORDER BY f DESC LIMIT 1";


			while(taskList.Count < maximumAsynchronousQueries && intervalEnd <= end)
			{
				var datePartitionStart = ArchiveDataElement.GetPartition(intervalStart.ToUniversalTime());
				var datePartitionEnd = ArchiveDataElement.GetPartition(intervalEnd.ToUniversalTime());
				var task1 = cassandraSession.ExecuteAsync(new SimpleStatement(string.Format(valuequery, tagGuid, "", datePartitionStart.ToString(), intervalStart.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"), intervalEnd.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"))).SetConsistencyLevel(ConsistencyLevel.One));
				taskList.Add(task1);

				// when start and end span two partitions, must run two queries
				if (datePartitionStart != datePartitionEnd)
				{
					var task2 = cassandraSession.ExecuteAsync(new SimpleStatement(string.Format(valuequery, tagGuid, "", datePartitionEnd.ToString(), intervalStart.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"), intervalEnd.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"))).SetConsistencyLevel(ConsistencyLevel.One));
					taskList.Add(task2);
				}

				intervalStart = intervalEnd.DateTime;
				intervalEnd = intervalEnd.ToUniversalTime().AddSeconds(intervalInSeconds).ToLocalTime();
			}


			return taskList;
		}

		List<List<TrendArchiveDataElement>> IPointTagArchiveDatabase.GetTrendArchiveData(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end)
		{
			var numberOfSamplesPerPen = int.Parse(ConfigurationManager.AppSettings["NumberOfSamplesPerPen"]);

			var restricted = "Restricted";
			if (security.UseDataDictionary)
			{
				var dataDictionaries = new DataDictionariesClass();
				restricted = dataDictionaries.Get(security.SiteGuid, restricted);
			}



			int RetryCounter = 0;
			tagList.ThrowIfNull("tagList");

			var trendList = new List<List<TrendArchiveDataElement>>();

			if (tagList.Count == 0)
			{
				return trendList;
			}

			RetryTrendTagDataRetrieval:

			sessionLock.EnterReadLock();

			if (cassandraSession == null)
			{

				++RetryCounter;

				sessionLock.ExitReadLock();

				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing Cassandra Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTrendTagDataRetrieval;
			}

			sessionLock.ExitReadLock();

			// get the interval between samples
			var intervalInSeconds = ((double) (end.Ticks - start.Ticks)) / ((double) (TicksPerSecond * numberOfSamplesPerPen));
			if(intervalInSeconds == 0)
			{
				intervalInSeconds = 1;
			}

			try
			{



				// Build all the queries
				foreach (Guid tagGuid in tagList)
				{
					// First Query Begins 1 millisecond prior to midnight and ends at start time plus the intervalInSeconds
					var intervalStart = start.ToUniversalTime().Date.AddMilliseconds(-1).ToLocalTime();
					var intervalEnd = start;

					var taskList = new List<Task<RowSet>>();
					var trendElementList = new List<TrendArchiveDataElement>();
					var alarmQueryQueued = false;


					while (intervalEnd < end || taskList.Count != 0)
					{
						// Wait on the queries to complete.
						Task.WaitAll(taskList.ToArray());

						var toProcessTaskList = QueueQueriesForPen(tagGuid, ref intervalStart, ref intervalEnd, end, intervalInSeconds);

						// at the end add a query for the alarm information
						if (!alarmQueryQueued && intervalEnd > end)
						{
							var alarmquery = "select d, e, f, g, h, i, j, k, l, m from alarmarchivedata where a = {0} and b = '{1}' and f > '{2}' and f <= '{3}'"
							+ " ORDER BY f DESC";
							var task = cassandraSession.ExecuteAsync(new SimpleStatement(string.Format(alarmquery, tagGuid, "", start.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"), end.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"))).SetConsistencyLevel(ConsistencyLevel.One));
							toProcessTaskList.Add(task);
							alarmQueryQueued = true;
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
									var trendArchiveDataElement = new TrendArchiveDataElement()
									{
										ArchiveRecordType = (Int32)row["g"],
										EngineeringUnitsIndex = (Int32)row["i"],
										Value = row["d"] as string,
										ValueOpcStatus = (Int64)row["e"],
										ValueTimeStamp = (DateTimeOffset)row["f"],
										AlarmPriorityGuid = (Guid)row["j"],
										Acknowledged = (bool)row["k"],
										AlarmState = row["l"] as string,
										AlarmOrStatusChanged = (bool)row["m"]
									};


									trendElementList.Add(trendArchiveDataElement);

								}
							}

							task.Dispose();
						}

						taskList = toProcessTaskList;
					}


					trendElementList.Sort();

					var preenedElementList = new List<TrendArchiveDataElement>();
					intervalStart = start.ToUniversalTime().Date.AddMilliseconds(-1).ToLocalTime();
					intervalEnd = intervalStart.ToUniversalTime().AddSeconds(intervalInSeconds).ToLocalTime();
					foreach (var trendElement in trendElementList)
					{
						while(intervalEnd < trendElement.ValueTimeStamp)
						{
							intervalStart = intervalEnd.DateTime;
							intervalEnd = intervalEnd.ToUniversalTime().AddSeconds(intervalInSeconds).ToLocalTime();
						}

						if (preenedElementList.Count > 0)
						{
							// do not remove any AlarmOrStatusChange but return only one for a given ValueTimeStamp
							// there may be two, one from AlarmArchiveData and one from ValueArchiveData
							if(preenedElementList[preenedElementList.Count - 1].AlarmOrStatusChanged
							&& preenedElementList[preenedElementList.Count - 1].ValueTimeStamp == trendElement.ValueTimeStamp)
							{
								preenedElementList[preenedElementList.Count - 1] = trendElement;
							}

							// due to the need to run multiple queries when an interval spans two partitions there may be duplicates for one or more invtervals.
							// remove any non AlarmOrStatusChange in the same interval
							else if (!preenedElementList[preenedElementList.Count - 1].AlarmOrStatusChanged
							&& preenedElementList[preenedElementList.Count - 1].ValueTimeStamp > intervalStart
							&& trendElement.ValueTimeStamp <= intervalEnd)
							{
								preenedElementList[preenedElementList.Count - 1] = trendElement;
							}

							else
							{
								preenedElementList.Add(trendElement);
							}
						}
						else
						{
							preenedElementList.Add(trendElement);
						}
					}

					trendElementList = preenedElementList;

					// When the first record is prior to start, force the time stamp to start
					if (trendElementList.Count > 0
					&& trendElementList[0].ValueTimeStamp < start)
					{
						trendElementList[0].ValueTimeStamp = start.ToUniversalTime();
					}

					// When there is no record at the start or the first record is after the start, insert a null record
					else if (trendElementList.Count == 0 || (trendElementList.Count > 0 && trendElementList[0].ValueTimeStamp > start))
					{
						TrendArchiveDataElement trendArchiveDataElement;
						if (tagGuid == Guid.Empty)
						{
							 trendArchiveDataElement = new TrendArchiveDataElement()
							                              {
								                              ArchiveRecordType = 3,
								                              EngineeringUnitsIndex = 0,
								                              Value = restricted,
								                              ValueOpcStatus = 0,
								                              ValueTimeStamp = start,
								                              AlarmPriorityGuid = Guid.Empty,
								                              Acknowledged = true,
								                              AlarmState = restricted
							 };
						}
						else
						{ 
							 trendArchiveDataElement = new TrendArchiveDataElement()
							                              {
								                              ArchiveRecordType = 3,
								                              EngineeringUnitsIndex = 0,
								                              Value = null,
								                              ValueOpcStatus = 0,
								                              ValueTimeStamp = start,
								                              AlarmPriorityGuid = Guid.Empty,
								                              Acknowledged = true,
								                              AlarmState = string.Empty
							                              };
						}

					trendElementList.Insert(0, trendArchiveDataElement);
					}

					// When there is no value for the end, add an end value identical to the prior
					if (trendElementList.Count > 0
					&& trendElementList[trendElementList.Count-1].ValueTimeStamp < end)
					{
						var trendArchiveDataElement = new TrendArchiveDataElement()
						{
							ArchiveRecordType = trendElementList[trendElementList.Count-1].ArchiveRecordType,
							EngineeringUnitsIndex = trendElementList[trendElementList.Count-1].EngineeringUnitsIndex,
							Value = trendElementList[trendElementList.Count-1].Value,
							ValueOpcStatus = trendElementList[trendElementList.Count-1].ValueOpcStatus,
							AlarmPriorityGuid = trendElementList[trendElementList.Count - 1].AlarmPriorityGuid,
							Acknowledged = trendElementList[trendElementList.Count - 1].Acknowledged,
							AlarmState = trendElementList[trendElementList.Count - 1].AlarmState,
							ValueTimeStamp = end.ToUniversalTime()
						};

						trendElementList.Add(trendArchiveDataElement);
					}

					trendList.Add(trendElementList);
				}
			}
			catch
			{
				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Reading Data from Cassandra.");
				}

				++RetryCounter;

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTrendTagDataRetrieval;
			}

			return trendList;
		}

		List<List<TrendArchiveDataElement>> IPointTagArchiveDatabase.GetLeakArchiveData(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end)
		{
			int RetryCounter = 0;
			tagList.ThrowIfNull(nameof(tagList));

			var trendList = new List<List<TrendArchiveDataElement>>();

			if (tagList.Count == 0)
			{
				return trendList;
			}

		RetryTrendTagDataRetrieval:

			sessionLock.EnterReadLock();

			if (cassandraSession == null)
			{

				++RetryCounter;

				sessionLock.ExitReadLock();

				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing Cassandra Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTrendTagDataRetrieval;
			}

			sessionLock.ExitReadLock();

			// get the interval between samples
			double intervalInSeconds = 300; // Leak detection expects samples every 5 minutes; this was from DoD certifications

			try
			{
				// Build all the queries
				foreach (Guid tagGuid in tagList)
				{
					// First Query Begins 1 millisecond prior to midnight and ends at start time plus the intervalInSeconds
					var intervalStart = start.ToUniversalTime().Date.AddMilliseconds(-1).ToLocalTime();
					var intervalEnd = start;

					var taskList = new List<Task<RowSet>>();
					var trendElementList = new List<TrendArchiveDataElement>();
					var alarmQueryQueued = false;


					while (intervalEnd < end || taskList.Count != 0)
					{
						// Wait on the queries to complete.
						Task.WaitAll(taskList.ToArray());

						var toProcessTaskList = QueueQueriesForPen(tagGuid, ref intervalStart, ref intervalEnd, end, intervalInSeconds);

						// at the end add a query for the alarm information
						if (!alarmQueryQueued && intervalEnd > end)
						{
							var alarmquery = "select d, e, f, g, h, i, j, k, l, m from alarmarchivedata where a = {0} and b = '{1}' and f > '{2}' and f <= '{3}'"
							+ " ORDER BY f DESC";
							var task = cassandraSession.ExecuteAsync(new SimpleStatement(string.Format(alarmquery, tagGuid, "", start.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"), end.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"))).SetConsistencyLevel(ConsistencyLevel.One));
							toProcessTaskList.Add(task);
							alarmQueryQueued = true;
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
									var trendArchiveDataElement = new TrendArchiveDataElement()
									{
										ArchiveRecordType = (int)row["g"],
										EngineeringUnitsIndex = (int)row["i"],
										Value = row["d"] as string,
										ValueOpcStatus = (long)row["e"],
										ValueTimeStamp = (DateTimeOffset)row["f"],
										AlarmPriorityGuid = (Guid)row["j"],
										Acknowledged = (bool)row["k"],
										AlarmState = row["l"] as string,
										AlarmOrStatusChanged = (bool)row["m"]
									};


									trendElementList.Add(trendArchiveDataElement);

								}
							}

							task.Dispose();
						}

						taskList = toProcessTaskList;
					}


					trendElementList.Sort();

					var preenedElementList = new List<TrendArchiveDataElement>();
					intervalStart = start.ToUniversalTime().Date.AddMilliseconds(-1).ToLocalTime();
					intervalEnd = intervalStart.ToUniversalTime().AddSeconds(intervalInSeconds).ToLocalTime();
					foreach (var trendElement in trendElementList)
					{
						while (intervalEnd < trendElement.ValueTimeStamp)
						{
							intervalStart = intervalEnd.DateTime;
							intervalEnd = intervalEnd.ToUniversalTime().AddSeconds(intervalInSeconds).ToLocalTime();
						}

						if (preenedElementList.Count > 0)
						{
							// do not remove any AlarmOrStatusChange but return only one for a given ValueTimeStamp
							// there may be two, one from AlarmArchiveData and one from ValueArchiveData
							if (preenedElementList[preenedElementList.Count - 1].AlarmOrStatusChanged
							&& preenedElementList[preenedElementList.Count - 1].ValueTimeStamp == trendElement.ValueTimeStamp)
							{
								preenedElementList[preenedElementList.Count - 1] = trendElement;
							}

							// due to the need to run multiple queries when an interval spans two partitions there may be duplicates for one or more invtervals.
							// remove any non AlarmOrStatusChange in the same interval
							else if (!preenedElementList[preenedElementList.Count - 1].AlarmOrStatusChanged
							&& preenedElementList[preenedElementList.Count - 1].ValueTimeStamp > intervalStart
							&& trendElement.ValueTimeStamp <= intervalEnd)
							{
								preenedElementList[preenedElementList.Count - 1] = trendElement;
							}

							else
							{
								preenedElementList.Add(trendElement);
							}
						}
						else
						{
							preenedElementList.Add(trendElement);
						}
					}

					trendElementList = preenedElementList;

					// When the first record is prior to start, force the time stamp to start
					if (trendElementList.Count > 0
					&& trendElementList[0].ValueTimeStamp < start)
					{
						trendElementList[0].ValueTimeStamp = start.ToUniversalTime();
					}

					// When there is no record at the start or the first record is after the start, insert a null record
					else if (trendElementList.Count == 0 || (trendElementList.Count > 0 && trendElementList[0].ValueTimeStamp > start))
					{
						TrendArchiveDataElement trendArchiveDataElement;
						trendArchiveDataElement = new TrendArchiveDataElement()
						{
							ArchiveRecordType = 3,
							EngineeringUnitsIndex = 0,
							Value = null,
							ValueOpcStatus = 0,
							ValueTimeStamp = start.ToUniversalTime(),
							AlarmPriorityGuid = Guid.Empty,
							Acknowledged = true,
							AlarmState = string.Empty
						};

						trendElementList.Insert(0, trendArchiveDataElement);
					}

					// When there is no value for the end, add an end value identical to the prior
					if (trendElementList.Count > 0
					&& trendElementList[trendElementList.Count - 1].ValueTimeStamp < end)
					{
						var trendArchiveDataElement = new TrendArchiveDataElement()
						{
							ArchiveRecordType = trendElementList[trendElementList.Count - 1].ArchiveRecordType,
							EngineeringUnitsIndex = trendElementList[trendElementList.Count - 1].EngineeringUnitsIndex,
							Value = trendElementList[trendElementList.Count - 1].Value,
							ValueOpcStatus = trendElementList[trendElementList.Count - 1].ValueOpcStatus,
							AlarmPriorityGuid = trendElementList[trendElementList.Count - 1].AlarmPriorityGuid,
							Acknowledged = trendElementList[trendElementList.Count - 1].Acknowledged,
							AlarmState = trendElementList[trendElementList.Count - 1].AlarmState,
							ValueTimeStamp = end.ToUniversalTime()
						};

						trendElementList.Add(trendArchiveDataElement);
					}

					trendList.Add(trendElementList);
				}
			}
			catch
			{
				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Reading Data from Cassandra.");
				}

				++RetryCounter;

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTrendTagDataRetrieval;
			}

			return trendList;
		}

        List<List<TrendArchiveDataElement>> IPointTagArchiveDatabase.GetHistoryArchiveData(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end, int numberOfSamplesPerPen)
        {
            var restricted = "Restricted";
            if (security.UseDataDictionary)
            {
                var dataDictionaries = new DataDictionariesClass();
                restricted = dataDictionaries.Get(security.SiteGuid, restricted);
            }



            int RetryCounter = 0;
            tagList.ThrowIfNull("tagList");

            var trendList = new List<List<TrendArchiveDataElement>>();

            if (tagList.Count == 0)
            {
                return trendList;
            }

            RetryTrendTagDataRetrieval:

            sessionLock.EnterReadLock();

            if (cassandraSession == null)
            {

                ++RetryCounter;

                sessionLock.ExitReadLock();

                if (RetryCounter >= 3) // give up and leave the routine
                {
                    throw new Exception("Failure Initializing Cassandra Session.");
                }

                Thread.Sleep(500);   // wait 500 msec after an error
                ((IPointTagArchiveDatabase)this).Initialize(security);
                goto RetryTrendTagDataRetrieval;
            }

            sessionLock.ExitReadLock();

            // get the interval between samples
            var intervalInSeconds = ((double)(end.Ticks - start.Ticks)) / ((double)(TicksPerSecond * numberOfSamplesPerPen));
            if (intervalInSeconds == 0)
            {
                intervalInSeconds = 1;
            }

            try
            {



                // Build all the queries
                foreach (Guid tagGuid in tagList)
                {
                    // First Query Begins 1 millisecond prior to midnight and ends at start time plus the intervalInSeconds
                    var intervalStart = start.ToUniversalTime().Date.AddMilliseconds(-1).ToLocalTime();
                    var intervalEnd = start;

                    var taskList = new List<Task<RowSet>>();
                    var trendElementList = new List<TrendArchiveDataElement>();

                    while (intervalEnd < end || taskList.Count != 0)
                    {
                        // Wait on the queries to complete.
                        Task.WaitAll(taskList.ToArray());

                        var toProcessTaskList = QueueQueriesForPen(tagGuid, ref intervalStart, ref intervalEnd, end, intervalInSeconds);

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
                                    var trendArchiveDataElement = new TrendArchiveDataElement()
                                    {
                                        ArchiveRecordType = (Int32)row["g"],
                                        EngineeringUnitsIndex = (Int32)row["i"],
                                        Value = row["d"] as string,
                                        ValueOpcStatus = (Int64)row["e"],
                                        ValueTimeStamp = (DateTimeOffset)row["f"],
                                        AlarmPriorityGuid = (Guid)row["j"],
                                        Acknowledged = (bool)row["k"],
                                        AlarmState = row["l"] as string,
                                        AlarmOrStatusChanged = (bool)row["m"]
                                    };


                                    trendElementList.Add(trendArchiveDataElement);

                                }
                            }

                            task.Dispose();
                        }

                        taskList = toProcessTaskList;
                    }


                    trendElementList.Sort();

                    // When the first record is prior to start, force the time stamp to start
                    if (trendElementList.Count > 0
                    && trendElementList[0].ValueTimeStamp < start)
                    {
                        trendElementList[0].ValueTimeStamp = start.ToUniversalTime();
                    }

                    // When there is no record at the start or the first record is after the start, insert a null record
                    else if (trendElementList.Count == 0 || (trendElementList.Count > 0 && trendElementList[0].ValueTimeStamp > start))
                    {
                        TrendArchiveDataElement trendArchiveDataElement;
                        if (tagGuid == Guid.Empty)
                        {
                            trendArchiveDataElement = new TrendArchiveDataElement()
                            {
                                ArchiveRecordType = 3,
                                EngineeringUnitsIndex = 0,
                                Value = restricted,
                                ValueOpcStatus = 0,
                                ValueTimeStamp = start,
                                AlarmPriorityGuid = Guid.Empty,
                                Acknowledged = true,
                                AlarmState = restricted
                            };
                        }
                        else
                        {
                            trendArchiveDataElement = new TrendArchiveDataElement()
                            {
                                ArchiveRecordType = 3,
                                EngineeringUnitsIndex = 0,
                                Value = null,
                                ValueOpcStatus = 0,
                                ValueTimeStamp = start,
                                AlarmPriorityGuid = Guid.Empty,
                                Acknowledged = true,
                                AlarmState = string.Empty
                            };
                        }

                        trendElementList.Insert(0, trendArchiveDataElement);
                    }

                    // When there is no value for the end, add an end value identical to the prior
                    if (trendElementList.Count > 0
                    && trendElementList[trendElementList.Count - 1].ValueTimeStamp < end)
                    {
                        var trendArchiveDataElement = new TrendArchiveDataElement()
                        {
                            ArchiveRecordType = trendElementList[trendElementList.Count - 1].ArchiveRecordType,
                            EngineeringUnitsIndex = trendElementList[trendElementList.Count - 1].EngineeringUnitsIndex,
                            Value = trendElementList[trendElementList.Count - 1].Value,
                            ValueOpcStatus = trendElementList[trendElementList.Count - 1].ValueOpcStatus,
                            AlarmPriorityGuid = trendElementList[trendElementList.Count - 1].AlarmPriorityGuid,
                            Acknowledged = trendElementList[trendElementList.Count - 1].Acknowledged,
                            AlarmState = trendElementList[trendElementList.Count - 1].AlarmState,
                            ValueTimeStamp = end.ToUniversalTime()
                        };

                        trendElementList.Add(trendArchiveDataElement);
                    }

                    trendList.Add(trendElementList);
                }
            }
            catch
            {
                if (RetryCounter >= 3) // give up and leave the routine
                {
                    throw new Exception("Failure Reading Data from Cassandra.");
                }

                ++RetryCounter;

                Thread.Sleep(500);   // wait 500 msec after an error
                ((IPointTagArchiveDatabase)this).Initialize(security);
                goto RetryTrendTagDataRetrieval;
            }

            return trendList;
        }
        
		List<ArchiveDataElement> IPointTagArchiveDatabase.GetArchiveData(SecurityClass security, DateTimeOffset startDateTimeOffset, Guid siteGuid, out bool moreData, out SynchronizationElement synchronizationElement)
		{
			var points = new Points();
			var pointValueIdentifierList = points.EnumerateArchivedPointValueIdentifiersBySite(security, siteGuid);

			var maxArchiveRecords = 10000;

			// 10 Minutes
			var interval = 600;

			security.ThrowIfNull("security");

			int RetryCounter = 0;

			RetryInitialize:

			sessionLock.EnterReadLock();

			if (cassandraSession == null)
			{

				++RetryCounter;

				sessionLock.ExitReadLock();

				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing Cassandra Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryInitialize;
			}

			sessionLock.ExitReadLock();

			try
			{

				synchronizationElement = SynchronizationTable.Where(u => u.TableName == "valuearchivedata" && u.SiteGuid == siteGuid)
						.FirstOrDefault()
						.Execute();

				if (synchronizationElement == null)
				{
					synchronizationElement = SynchronizationTable.Where(u => u.TableName == "valuearchivedata" && u.SiteGuid == Guid.Empty)
							.FirstOrDefault()
							.Execute();

					if (synchronizationElement == null)
					{
						throw new Exception("Failed to retreive ArchiveData SynchronizationElement");
					}
				}

				synchronizationElement.NumberOfRecordsSynchronized = 0;

				moreData = false;

				var archiveDataElementList = new List<ArchiveDataElement>();
				var endTime = synchronizationElement.LastValueTimeStamp;
				endTime = endTime.AddSeconds(interval);

				if (endTime > startDateTimeOffset)
				{
					endTime = startDateTimeOffset;
				}

				var query = "select * from \"FMArchive_Data\".valuearchivedata where a = {0} and b = '{1}' and c in ({2}) and f > '"
								+ synchronizationElement.LastValueTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff+0000")
								+ "' and f <= '"
								+ endTime.ToString("yyyy-MM-dd HH:mm:ss.fff+0000") + "'";

				var iterations = 0;

				while (true)
				{
					string datePartitionKeys = "";
					datePartitionKeys = ArchiveDataElement.GetPartition(synchronizationElement.LastValueTimeStamp).ToString();

					if (synchronizationElement.LastValueTimeStamp.Month != endTime.Month)
					{
						datePartitionKeys += ", " + ArchiveDataElement.GetPartition(endTime).ToString();
					}

					foreach (var pointValueIdentifier in pointValueIdentifierList)
					{

						// Advance to LastPointValueGuid and PropertyID
						if (synchronizationElement.LastPointValueGuid != Guid.Empty
						&& (synchronizationElement.LastPointValueGuid != pointValueIdentifier.IdentityGuid
						|| synchronizationElement.LastPointValuePropertyID != pointValueIdentifier.PropertyID))
						{
							continue;
						}

						// LastPointValueGuid proceed with the next
						if (synchronizationElement.LastPointValueGuid != Guid.Empty)
						{
							synchronizationElement.LastPointValueGuid = Guid.Empty;
							continue;
						}


						var identityGuid = pointValueIdentifier.IdentityGuid;
						var propertyID = (pointValueIdentifier.PropertyID == null) ? "" : pointValueIdentifier.PropertyID;
						var result = ValueArchiveTable.GetSession().Execute(new SimpleStatement(string.Format(query, identityGuid, propertyID, datePartitionKeys)).SetConsistencyLevel(ConsistencyLevel.One)) as RowSet;

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
								var archiveDataElement = new ArchiveDataElement()
								{
									PointValueGuid = (Guid)row["a"],
									PropertyID = row["b"] as string,
									Value = row["d"] as string,
									ValueOpcStatus = (Int64)row["e"],
									ValueTimeStamp = (DateTimeOffset)row["f"],
									ArchiveRecordType = (Int32)row["g"],
									DataType = (string)row["h"],
									EngineeringUnitsIndex = (Int32)row["i"],
									AlarmPriorityGuid = (Guid)row["j"],
									Acknowledged = (bool)row["k"],
									AlarmState = (string)row["l"],
									AlarmOrStatusChanged = (bool)row["m"],
									RecordTimeStamp = (DateTimeOffset)row["n"],
									QualityString = (string)row["o"],
									SiteGuid = siteGuid
								};

								archiveDataElementList.Add(archiveDataElement);
							}
						}

						// at maxArchiveRecords record LastPointValueGuid and PropertyID for resumption
						if (archiveDataElementList.Count > maxArchiveRecords)
						{
							synchronizationElement.LastPointValueGuid = pointValueIdentifier.IdentityGuid;
							synchronizationElement.LastPointValuePropertyID = pointValueIdentifier.PropertyID;
							break;
						}
					}

					synchronizationElement.SiteGuid = siteGuid;

					// look for records 1 interval at a time.
					if (archiveDataElementList.Count > 0)
					{
						break;
					}

					synchronizationElement.LastValueTimeStamp = endTime;

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


					query = "select * from \"FMArchive_Data\".valuearchivedata where a = {0} and b = '{1}' and c in ({2}) and f > '"
								+ synchronizationElement.LastValueTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff+0000")
								+ "' and f <= '"
								+ endTime.ToString("yyyy-MM-dd HH:mm:ss.fff+0000") + "'";
				}

				if (archiveDataElementList.Count > 0)
				{
					if (synchronizationElement.LastPointValueGuid == Guid.Empty)
					{
						synchronizationElement.LastValueTimeStamp = endTime;
					}
					else
					{
						moreData = true;
					}

					if (synchronizationElement.LastValueTimeStamp < startDateTimeOffset)
					{
						moreData = true;
					}
				}

				synchronizationElement.NumberOfRecordsSynchronized = archiveDataElementList.Count;

				return archiveDataElementList;
			}
			catch (Exception e)
			{
				if (e.Message == "Keyspace 'FMArchive_Data' does not exist"
				|| e.Message.Contains("All hosts tried for query failed "))
				{
					cassandraSession.Dispose();
					cassandraSession = null;
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
		void IPointTagArchiveDatabase.SynchronizationComplete(
			SecurityClass security,
			SynchronizationElement synchronizationElement)
		{
			if (cassandraSession != null)
			{
				cassandraSession.Execute(SynchronizationTable.Insert(synchronizationElement));
			}
		}

		protected List<Task<RowSet>> QueueQueriesForTag(Guid tagGuid, DateTimeOffset start, DateTimeOffset end)
		{
			var taskList = new List<Task<RowSet>>();

			var valuequery = "select d, e, f, g, h, i, o from valuearchivedata where a = {0} and b = '{1}' and c = {2} and f <= '{3}'"
								+ " ORDER BY f DESC LIMIT 1";

			var datePartitionStart = ArchiveDataElement.GetPartition(start.ToUniversalTime());
			var datePartitionEnd = ArchiveDataElement.GetPartition(end.ToUniversalTime());
			var task1 = cassandraSession.ExecuteAsync(new SimpleStatement(string.Format(valuequery, tagGuid, "", datePartitionEnd.ToString(), end.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"))).SetConsistencyLevel(ConsistencyLevel.One));
			taskList.Add(task1);

			// when start and end span two partitions, must run two queries
			if (datePartitionStart != datePartitionEnd)
			{
				var task2 = cassandraSession.ExecuteAsync(new SimpleStatement(string.Format(valuequery, tagGuid, "", datePartitionStart.ToString(), end.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff+0000"))).SetConsistencyLevel(ConsistencyLevel.One));
				taskList.Add(task2);
			}

			return taskList;
		}

		List<SimpleArchiveDataElement> IPointTagArchiveDatabase.GetArchiveDataValues(SecurityClass security, List<Guid> tagList, DateTimeOffset start, DateTimeOffset end)
		{
			var restricted = "Restricted";
			if (security.UseDataDictionary)
			{
				var dataDictionaries = new DataDictionariesClass();
				restricted = dataDictionaries.Get(security.SiteGuid, restricted);
			}

			int RetryCounter = 0;
			tagList.ThrowIfNull("tagList");

			var trendList = new List<SimpleArchiveDataElement>();

			if (tagList.Count == 0)
			{
				return trendList;
			}

		RetryTrendTagDataRetrieval:

			sessionLock.EnterReadLock();

			if (cassandraSession == null)
			{

				++RetryCounter;

				sessionLock.ExitReadLock();

				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Initializing Cassandra Session.");
				}

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTrendTagDataRetrieval;
			}

			sessionLock.ExitReadLock();

			List<SimpleArchiveDataElement> tagValues = new List<SimpleArchiveDataElement>();

			try
			{
				// Build all the queries
				foreach (Guid tagGuid in tagList)
				{
					var tagValueList = new List<SimpleArchiveDataElement>();

					var taskList = QueueQueriesForTag(tagGuid, start, end);

					// Wait on the queries to complete.
					Task.WaitAll(taskList.ToArray());

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
								var archiveDataElement = new SimpleArchiveDataElement()
								{
									PointValueGuid = tagGuid,
									Value = row["d"] as string,
									ValueOpcStatus = (Int64)row["e"],
									ValueTimeStamp = (DateTimeOffset)row["f"],
									DataType = (string)row["h"],
									EngineeringUnitsIndex = (Int32)row["i"],
									QualityString = (string)row["o"],
								};
								tagValueList.Add(archiveDataElement);
							}
						}

						task.Dispose();
					}

					// if no records exist
					if(tagValueList.Count == 0)
					{
						var archiveDataElement = new SimpleArchiveDataElement()
						{
								PointValueGuid = tagGuid,
								Value = null,
								ValueOpcStatus = 0,
								ValueTimeStamp = start,
								DataType = null,
								EngineeringUnitsIndex = 0,
								QualityString = "BAD",
						};

						tagValues.Add(archiveDataElement);
					}
					else
					{
						var sortedTagValueList = tagValueList.OrderByDescending(obj => obj.ValueTimeStamp).ToList();
						// Get the latest record for each tag
						tagValues.Add(sortedTagValueList[0]);
					}
				}
			}
			catch
			{
				if (RetryCounter >= 3) // give up and leave the routine
				{
					throw new Exception("Failure Reading Data from Cassandra.");
				}

				++RetryCounter;

				Thread.Sleep(500);   // wait 500 msec after an error
				((IPointTagArchiveDatabase)this).Initialize(security);
				goto RetryTrendTagDataRetrieval;
			}

			return tagValues;
		}
	}
}
