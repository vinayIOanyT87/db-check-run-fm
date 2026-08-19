namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Threading;
	using System.Diagnostics;

	using Cassandra;
	using Cassandra.Data.Linq;
	using Cassandra.Mapping;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using InternalClasses;
	using InternalInterfaces;
	using ServiceClasses;
	using System.Configuration;

	using FMCore;

    internal class CassandraUtilityDAO : ICassandraUtility
	{
		const long TicksPerSecond = 10000000;

		const long maximumAsynchronousQueries = 1000;

		private static readonly int MaxLockDelay = 10000;

		//public static Cassandra.ConsistencyLevel consistencyLevel;

		private static readonly ICassandraKeyspaceCreator CassandraKeyspaceCreator = new CassandraKeyspaceCreator();

		private static readonly ICassandraDataTablesCreator CassandraDataTablesCreator =
				new CassandraDataTablesCreator();

		private static readonly ICassandraConnectionConfig CassandraConnectionConfig = new CassandraConnectionConfig();

		private static Cluster cassandraCluster;

		private static ISession cassandraSession;

		private static ReaderWriterLockSlim sessionLock = new ReaderWriterLockSlim();

		protected EventLogging eventLogging;


		void ICassandraUtility.InitializeWithCredentials(SecurityClass security, string username, string password)
		{
			security.ThrowIfNull("security");

			if (!sessionLock.TryEnterWriteLock(MaxLockDelay))
			{
				throw new Exception("CassandraUtility : Initialize - timeout acquiring write lock");
			}

			try
			{
				this.eventLogging = new EventLogging();

				ShutdownIfRunning();

				var contactPoints = CassandraConnectionConfig.GetContactPoints(security);
				var credentials = new string[] { username, password };

				if (!(String.IsNullOrEmpty(credentials[0]) || String.IsNullOrEmpty(credentials[1])))
				{
					cassandraCluster =
						Cluster.Builder()
							.AddContactPoints(contactPoints)
							.WithReconnectionPolicy(new ExponentialReconnectionPolicy(2 * 1000, 2 * 60 * 1000))
							// this is a minimum retry of 2 seconds for a failed node increasing to a maximum of 2 minutes
							//.WithCompression(CompressionType.Snappy)
							//.WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.Any))
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
						//.WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.Any))
						.Build();
				}

				if (cassandraSession == null)
				{
					cassandraSession = cassandraCluster.Connect();
				}
			}
			catch (Exception ex)
			{
				string errorMessage = string.Format("CassandraUtility.Initialize Error. {0}", ex.Message);
				this.eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
				//throw new Exception(string.Format("PointTagArchiveDatabase.Initialize Error. {0}", ex.Message));
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
					cassandraSession = null;
				}
				catch
				{
				}
			}
		}


		  public bool CreateOrModifyCassandraUser(SecurityClass security, string[] credentials)
		  {
				try
				{
					 int retryCounter = 0;
					 ShutdownIfRunning();
					 ((ICassandraUtility)this).InitializeWithCredentials(security, credentials[0], credentials[1]); //atttempt to connect with old credentials
					 while (cassandraSession == null)
					 {
						  ++retryCounter;

						  if (retryCounter >= 3) // give up and leave the routine
						  {
								break;
						  }

						  Thread.Sleep(500);   // wait 500 msec after an error
						  ((ICassandraUtility)this).InitializeWithCredentials(security, credentials[0], credentials[1]);
					 }
					 if (cassandraSession != null) //success
					 {
						  Cassandra.RowSet roleExists = cassandraSession.Execute("select * FROM system_auth.ROLES WHERE ROLE = '" + credentials[2] + "';");//if user exists, modify the password
						  var rows = roleExists.GetRows();
						  if (rows.Count() == 1) //user exists, update PW
						  {
								Cassandra.RowSet alterResult =
									cassandraSession.Execute("ALTER ROLE '" + credentials[2] + "' WITH PASSWORD = '" + credentials[3] + "';");
								ShutdownIfRunning();
								return true;
						  }
						  else //user does not exist, create it
						  {
								Cassandra.RowSet createResult = cassandraSession.Execute("CREATE ROLE '" + credentials[2] + "' WITH SUPERUSER = true AND LOGIN = true AND PASSWORD = '" + credentials[3] + "';");
								ShutdownIfRunning();
								if (credentials[1] == credentials[3])
								{
									 ((ICassandraUtility)this).InitializeWithCredentials(security, credentials[2], credentials[3]);
									 retryCounter = 0;
									 while (cassandraSession == null)
									 {
										  ++retryCounter;

										  if (retryCounter >= 3) // give up and leave the routine
										  {
												return false;
										  }

										  Thread.Sleep(500);   // wait 500 msec after an error
										  ((ICassandraUtility)this).InitializeWithCredentials(security, credentials[2], credentials[3]);
									 }
									 if (cassandraSession != null)
									 {
										  if (credentials[0] == "cassandra") //first time setup
												cassandraSession.Execute("ALTER ROLE cassandra WITH SUPERUSER = false AND LOGIN = false;");
										  else cassandraSession.Execute("DROP ROLE IF EXISTS '" + credentials[0] + "';");
										  return true;
									 }
									 else
									 {
										  return false;
									 }
								}
								return true;
						  }
					 }
					 else //failure, try the new credentials
					 {
						  ShutdownIfRunning();
						  ((ICassandraUtility)this).InitializeWithCredentials(security, credentials[2], credentials[3]);
						  retryCounter = 0;
						  while (cassandraSession == null)
						  {
								++retryCounter;

								if (retryCounter >= 3) // give up and leave the routine
								{
									 return false;
								}

								Thread.Sleep(500);   // wait 500 msec after an error
								((ICassandraUtility)this).InitializeWithCredentials(security, credentials[2], credentials[3]);
						  }
						  if (cassandraSession != null)
								return true;
						  else
						  {
								return false;
						  }
					 }
				}
				catch (Exception ex)
				{
					 throw new Exception("Authentication may be disabled for this Cassandra instance. Please check your configuration settings to ensure that authentication is enabled if required for your application.", ex);
            }
		}
	}
}