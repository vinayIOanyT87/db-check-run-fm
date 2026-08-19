// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsolidatedDA.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ConsolidatedDAClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Text;
	using System.ServiceModel;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using ServiceClasses;

	/// <summary>
	/// The ConsolidatedDAClass class is responsible for all database access for FuelsManager.
	/// </summary>
	public partial class ConsolidatedDAClass
	{
		#region ------------------------- CHANGE THIS WHENEVER THE FIRST FOUR SEGMENTS OF THE DATABASE VERSION NUMBER CHANGE!
		// ------------------------------------------------------------------------------------------------
		// The Database Version is not the same as the application version.  Multiple application
		// versions can be implemented on a given Database Version.
		// 
		// The Database Version number consists of five parts.  The first three have the same meaning as
		// the first three in the application version number:
		//
		//                                    8   .   0   .    0     .   2    .   39 
		//                                  major . minor . svc-pack . schema . build 
		// 
		// The "schema" and "build number" values in the Database Version have special purposes:
		// 
		//		o	The "schema" number is incremented by the Database Owner only when a “breaking change” 
		//			(one which will “break” existing Applications and Reports, such as adding, modifying,
		//			or removing columns or tables) is made to the database. 
		// 
		//		o	The "build" number is incremented whenever any change is made to the database,
		//			non-breaking or breaking.  Generally it is only used to distinguish one database
		//			from another.
		// ------------------------------------------------------------------------------------------------

		private string sExpectedByThisApp = "12.0.9.0";    // You usually want to change only the fourth segment.

		#endregion ---------------------- CHANGE THIS WHENEVER THE FIRST FOUR SEGMENTS OF THE DATABASE VERSION NUMBER CHANGE!

		private const string HardwareKeyExpirationName = "FMHardwareKeyExpiration";

		/// <summary>
		/// Represents the error code returned by sql server when a maximum recursion is exhausted. The error message is:
		/// </summary>
		private const int SQLServerStatementTerminatedMaximumRecursion = 530;

		/// <summary>
		/// Represents the error code returned by sql server when a unique constraint is violated. The error message is:
		/// Violation of %ls constraint '%.*ls'. Cannot insert duplicate key in object '%.*ls'. The duplicate key value is %ls.
		/// </summary>
		private const int SQLServerUniqueConstraintViolationErrorCode = 2627;

		/// <summary>
		/// Represents the error code returned by sql server when a unique index is violated. The error message is:
		/// Cannot insert duplicate key row in object '%.*ls' with unique index '%.*ls'. The duplicate key value is %ls.
		/// </summary>
		private const int SQLServerUniqueIndexViolationErrorCode = 2601;

		/// <summary>
		/// Represents the error code returned by sql server when a command timeout occurs.
		/// </summary>
		private const int SQLServerCommandTimeoutErrorCode = -2;

		/// <summary>
		/// The error message returned to the user when a unique constraint or index is violated.
		/// This is used to return something more user friendly than "Database error".
		/// </summary>
		private const string UniqueConstraintViolationErrorMessage = "The action requested would result in a duplicate record";

		public const string StatementTerminatedMaximumRecursionErrorMessage = "Statement Terminated Maximum Recursion Exhausted";

		public const string OperationTimedOut = "Operation Timed Out";

		public const string SpecialKeyCodesName = "FMSpecialKeyCodes";
		public const string OptionsCellName = "FMOptionsCell";
		public const string OpcAllowedFunctionsName = "FMOPCAllowedFunctions";
		public const string ProgramVersionName = "FMProgramVersion";

		public const string UseNewLicenseFile = "FMUseNewLicenseFile";
		public const string ProgramVersionNameLIN = "FMProgramVersionLIN";
		public const string word1LIN = "word1LIN";
		public const string word2LIN = "word2LIN";

		public const int Uniquifier = 42;

		static public string ConnectionString
		{
			get
			{
				var connectionString = ConfigurationManager.AppSettings["ConnectionString"];

				if (string.IsNullOrEmpty(connectionString))
				{
					throw new ArgumentException("Connection string not configured in configuration file.");
				}

				return connectionString;
			}
		}

		public string DatabaseName
		{
			get
			{
				var builder = new SqlConnectionStringBuilder(ConnectionString);
				return (!builder.InitialCatalog.Contains(" ")) ? builder.InitialCatalog : "[" + builder.InitialCatalog + "]";
			}
		}

		public string ArchiveDatabaseName
		{
			get
			{
				var builder = new SqlConnectionStringBuilder(ConnectionString);
				return (!builder.InitialCatalog.Contains(" ")) ? builder.InitialCatalog + "Archive" : "[" + builder.InitialCatalog + "Archive]";
			}
		}

		/// <summary>
		/// The get version.
		/// </summary>
		/// <returns>
		/// The <see cref="VersionInfo"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// Throws an Exception if the version information is missing from the database or if we were
		/// unable to parse the version string located in the database.
		/// </exception>
		public static VersionInfo GetVersion()
		{
			// This keeps CheckVersion() from being called every time.
			AppDomain domain = AppDomain.CurrentDomain;

			if (domain.GetData("ConsolidatedDBVersion") == null)
			{
				VersionDO versionDo = GetVersionDetails();

				if (null != versionDo)
				{
					domain.SetData("ConsolidatedDBVersion", versionDo.ToVersionInfo());
				}
			}

			return domain.GetData("ConsolidatedDBVersion") as VersionInfo;
		}

		/// <summary>
		/// Gets the latest <see cref="VersionDO"/> data object from tblVersions.
		/// </summary>
		/// <returns>
		/// An instance of a <see cref="VersionDO"/> data object.
		/// </returns>
		/// <exception cref="Exception">
		/// Throws an Exception if the version information is missing from the database or if we were
		/// unable to parse the version string located in the database.
		/// </exception>
		public static VersionDO GetVersionDetails()
		{
			// This keeps CheckVersion() from being called every time.
			AppDomain domain = AppDomain.CurrentDomain;

			if (domain.GetData("ConsolidatedDBVersionDO") == null)
			{
				try
				{
					// Get version from database tblVersion table.  There may be many rows in the tblVersion table:
					// we're only interested in the most recent (the highest 'VersionIndex').  This allows us to
					// maintain a record of the versions this database has been.  Also, several different flavors
					// of scripts can affect the schema (upgrade scripts, Deployment scripts, hotfix scripts,
					// "Edition"-specific scripts), so we have a "PackageName" identifier to make it easier what
					// was done to the database in what order.
					using (DataSet resultDataSet = new DataSet())
					{
						SqlConnection connection = new SqlConnection(ConnectionString);

						var cmd = new SqlCommand
						{
							CommandText = "dbo.usp_VersionSelectCurrent",
							CommandType = CommandType.StoredProcedure
						};

						cmd.Parameters.Clear();
						cmd.Connection = connection;

						try
						{
							using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
							{
								adapter.Fill(resultDataSet);
							}
						}
						finally
						{
							connection.Close();
						}

						// Was data in the tblVersion table?  There must be at least one row.
						if (resultDataSet.Tables.Count != 1 || resultDataSet.Tables[0].Rows.Count != 1
							|| resultDataSet.Tables[0].Rows[0].IsNull(0))
						{
							throw new Exception("Database version information not found.");
						}

						DataRow row = resultDataSet.Tables[0].Rows[0];

						if (null != row)
						{
							VersionDO versionDo = new VersionDO
							{
								IdentityGuid = DataObject.getValue(row["VersionGuid"], Guid.Empty),
								VersionIndex = DataObject.getOptionalInt(row["VersionIndex"]),
								Version = DataObject.getString(row["Version"]),
								PackageName = DataObject.getString(row["PackageName"]),
								DateApplied = DataObject.getOptionalDateTimeOffset(row["DateApplied"]),
								Comments = DataObject.getString(row["Comments"]),
								Check1 = DataObject.getLong(row["Check1"]),
								Check2 = DataObject.getLong(row["Check2"]),
								SyncCompletedFlag = DataObject.getValue(row["SyncCompletedFlag"], false),
								RowVersionSnapshot = DataObject.getOptionalVarBinary(row["RowVersionSnapshot"]),
								RowVersion = DataObject.getOptionalVarBinary(row["_RowVersion"])
							};

							var createdDate = DataObject.getValue<DateTime>(row["CreatedDate"], DateTime.Now);
							var updatedDate = DataObject.getValue<DateTime>(row["UpdatedDate"], DateTime.Now);

							versionDo.CreatedDate = new DateTimeOffset(createdDate);
							versionDo.CreatedBy = DataObject.getString(row["CreatedBy"]);
							versionDo.UpdatedDate = new DateTimeOffset(updatedDate);
							versionDo.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

							domain.SetData("ConsolidatedDBVersionDO", versionDo);
						}
					}
				}
				catch (ArgumentOutOfRangeException e)
				{
					throw new Exception("Database error: " + e);
				}
				catch (Exception e)
				{
					// This error message can also occur if the database is not in
					// MULTI_USER mode.   Try:  ALTER DATABASE {dbname} SET MULTI_USER
					throw new Exception("Database error: " + e);
				}
			}

			return domain.GetData("ConsolidatedDBVersionDO") as VersionDO;
		}

		/// <summary>
		/// Function to verify that the code version is in sync with the database version.
		/// Also performs the hardware key check
		/// </summary>
		/// <remarks>
		/// When testing this, remember that if you change the values in tblVersion,
		/// you have to restart IIS *and* kill DllHost.exe.
		/// </remarks>
		/// <exception cref="Exception">
		/// Thrown if the database version does not match the version the code expects.
		/// Also thrown on failure to access the database.
		/// </exception>
		public void CheckVersion()
		{
			// This keeps CheckVersion() from being called every time.
			AppDomain domain = AppDomain.CurrentDomain;

			if (domain.GetData("ConsolidatedDBVersionCheck") == null)
			{
				try
				{
					// Get version from database tblVersion table.  There may be many rows in the tblVersion table:
					// we're only interested in the most recent (the highest 'VersionIndex').  This allows us to
					// maintain a record of the versions this database has been.  Also, several different flavors
					// of scripts can affect the schema (upgrade scripts, Deployment scripts, hotfix scripts,
					// "Edition"-specific scripts), so we have a "PackageName" identifier to make it easier what
					// was done to the database in what order.
					const string SQL = "SELECT TOP 1 Version "
						+ "  FROM tblVersion WITH (NOLOCK) "
						+ " WHERE PackageName = 'StandardDatabase' "
						+ " ORDER BY VersionIndex DESC ";

					string inDatabase;

					using (var resultDataSet = new DataSet())
					{
						var connection = new SqlConnection(ConnectionString);

						try
						{
							using (var adapter = new SqlDataAdapter(SQL, connection))
							{
								adapter.Fill(resultDataSet);
							}
						}
						finally
						{
							connection.Close();
						}

						// Was data in the tblVersion table?  There must be at least one row.
						if (resultDataSet.Tables.Count != 1
							|| resultDataSet.Tables[0].Rows.Count != 1
							|| resultDataSet.Tables[0].Rows[0].IsNull(0))
						{
							throw new Exception("No row, or NULL value, returned from database.");
						}

						// Set up for version string comparison.
						inDatabase = (string)resultDataSet.Tables[0].Rows[0][0];
					}

					string sMsg;
					var eventLog = new FMEventLog();

					if (null != (sMsg = this.CompareVersions(inDatabase)))
					{
						eventLog.WriteEntry(sMsg, FMEventLogEntryType.Error);
						throw new Exception(sMsg);
					}
				}
				catch (Exception e)
				{
					// This error message can also occur if the database is not in
					// MULTI_USER mode.   Try:  ALTER DATABASE {dbname} SET MULTI_USER
					throw new Exception("Database error: " + e.ToString());
				}

				domain.SetData("ConsolidatedDBVersionCheck", "Just has to exist");
			}

			// Check the hardware key next
			object hardwareExpirationDateTime = domain.GetData(HardwareKeyExpirationName);
			bool hardwareKeyRead = false;

			if (hardwareExpirationDateTime == null)
			{
				ReadHardwareKey();
				hardwareKeyRead = true;
				hardwareExpirationDateTime = DateTimeOffset.Now.AddHours(-1);
			}

			if ((DateTimeOffset)hardwareExpirationDateTime < DateTimeOffset.Now)
			{
				if (hardwareKeyRead == false)
				{
					ReadHardwareKey();
				}

				hardwareExpirationDateTime = DateTimeOffset.Now.AddMinutes(15);
				domain.SetData(HardwareKeyExpirationName, hardwareExpirationDateTime);
			}
		}

		public static void ReadHardwareKey()
		{
			HardwareKeyClass hardwareKey = new HardwareKeyClass();
			hardwareKey.ReadHardwareKey();
		}

		// Helper to allow for unit testing.  Returns an error message, or null on success.
		// Looks awkward because I'm doing calling this for unit testing.
		private string CompareVersions(string sInDatabase)
		{
			const string StringFormat = "Application expected '{0}', found '{1}' in database.  ";
			char[] chSeparators = { '.' };
			int maxSegmentsToCompare = 3;

			// Specify our expected version.
			string[] saSegmentsExpectedByThisApp = this.sExpectedByThisApp.Split(chSeparators);

			if (saSegmentsExpectedByThisApp.Length < maxSegmentsToCompare)
			{
				return string.Format(StringFormat, this.sExpectedByThisApp, sInDatabase) + "'sExpectedByThisApp' failed to split correctly.";
			}

			// Get the version that's in the database.
			string[] saSegmentsInDatabase = sInDatabase.Split(chSeparators);

			if (saSegmentsInDatabase.Length < maxSegmentsToCompare)
			{
				return string.Format(StringFormat, this.sExpectedByThisApp, sInDatabase) + "'ResultDataSet' failed to parse correctly.";
			}

			// Do the comparison, converting each segment to an integer, in case someone
			// enters a version like "7.5.0000.01".
			while (0 < maxSegmentsToCompare--)
			{
				Int32 nInDatabase;
				Int32 nExpectedByThisApp;

				if (!Int32.TryParse(saSegmentsInDatabase[maxSegmentsToCompare], out nInDatabase))
				{
					return string.Format(StringFormat, this.sExpectedByThisApp, sInDatabase) + "The version number found in the database did not parse to an integer.";
				}

				if (!Int32.TryParse(saSegmentsExpectedByThisApp[maxSegmentsToCompare], out nExpectedByThisApp))
				{
					return string.Format(StringFormat, this.sExpectedByThisApp, sInDatabase) + "The version number expected by this application did not parse to an integer.";
				}

				if (nInDatabase != nExpectedByThisApp)
				{
					return string.Format(StringFormat, this.sExpectedByThisApp, sInDatabase) + "The version numbers do not match.";
				}
			}

			// Success!
			return null;
		}

		/// <summary>
		/// Opens and closes the connection to the database for test purposes.
		/// </summary>
		public void TestConnection()
		{
			var connection = new SqlConnection(ConnectionString);
			connection.Open();
			connection.Close();
		}

		/// <summary>
		/// Updated Get method to accept the user id and site id so that correct
		/// audit log info is captured.  Note that if <paramref name="userId"/>
		/// is <see cref="DBAccess.ServiceLoginAccess">DBAccess.ServiceLogin</see> then
		/// access will by the NETWORK_SERVICE account
		/// </summary>
		/// <param name="command">SQL command object containing select query to run</param>
		/// <param name="userId">
		/// User id of user making the request resulting
		/// in this query being run
		/// </param>
		/// <param name="siteId">
		/// Site id of user making the request resulting
		/// in this query being run - needed for user uniquification
		/// As this function will only work correctly currently for service logins,
		/// this can be passed in as an empty string
		/// </param>
		/// <returns>Dataset containing the result of the SQL query</returns>
		/// <remarks>
		///   Follows FuelsManager Defense 6.0 SP4 methodology if DESC key is attached.
		///   Database Password is a mangling of the user password; so this method can only
		///   work for the database service login (which has special handling internal anyways).
		/// </remarks>
		public DataSet GetDataSet(SqlCommand command, string userId, string siteId)
		{
			var security = new SecurityClass
			{
				UserID = userId,
				SiteID = siteId
			};
			return GetDataSet(command, security);
		}


		/// <summary>
		/// Gets the data table.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		public DataTable GetDataTable(SqlCommand command, SecurityClass security)
		{
			DataSet resultSet = this.GetDataSet(command, security);
			DataTable resultTable = null;
			if (resultSet.Tables.Count > 0)
			{
				resultTable = resultSet.Tables[0];
			}

			return resultTable;
		}

		/// <summary>
		/// Updated Get method to accept the user id so that correct
		/// audit log info is captured.  Note that if <paramref name="security"/>.UserID
		/// is <see cref="DBAccess.ServiceLoginAccess">DBAccess.ServiceLogin</see> then
		/// access will by the NETWORK_SERVICE account
		/// </summary>
		/// <param name="command">SQL command object containing select query to run</param>
		/// <param name="security">
		/// Security object of user making the request resulting
		/// in this query being run
		/// </param>
		/// <returns>Dataset containing the result of the SQL query</returns>
		/// <remarks>
		///   Follows FuelsManager Defense 6.0 SP4 methodology if DESC key is attached.
		///   Database Password is a mangling of the user id and has no relation the 
		///   users application Password.
		/// </remarks>
		public DataSet GetDataSet(SqlCommand command, SecurityClass security)
		{
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			DataSet resultDataSet = new DataSet();
			SqlConnection connection = null;

			try
			{
				int nRetryCount = 1;

				do
				{
					try
					{
						connection = new SqlConnection(ConnectionString);
						command.Connection = connection;
						var expirationTime = DateTime.Now.AddSeconds(command.CommandTimeout);
						connection.Open();

						using (var reader = command.ExecuteReader())
						{
							string tableNameSuffix = string.Empty;
							int tableNumber = 0;

							do
							{
								var schema = reader.GetSchemaTable();


								if (schema != null && schema.Rows.Count > 0)
								{
									var table = new DataTable { TableName = "TableName" + tableNameSuffix };
									tableNumber++;
									tableNameSuffix = tableNumber.ToString();
									resultDataSet.Tables.Add(table);

									foreach (DataRow schemaRow in schema.Rows)
									{
										var column = new DataColumn
										{
											ColumnName = schemaRow["ColumnName"] as string,
											DataType = schemaRow["DataType"] as Type,
											AllowDBNull = (bool)schemaRow["AllowDBNull"]
										};

										if (column.DataType == typeof(string))
										{
											column.MaxLength = (int)schemaRow["ColumnSize"];
										}

										string columnNameSuffix = "";
										int columnNameSuffixInt = 1;

										while (table.Columns.Contains(column.ColumnName + columnNameSuffix))
										{
											columnNameSuffix = columnNameSuffixInt.ToString();
											columnNameSuffixInt++;
										}

										column.ColumnName += columnNameSuffix;
										table.Columns.Add(column);
									}

									if (command.CommandTimeout != 0 && DateTime.Now > expirationTime)
									{
										throw new Exception(OperationTimedOut);
									}

									var objects = new Object[table.Columns.Count];

									while (reader.Read())
									{
										var row = table.NewRow();
										((IDataRecord)reader).GetValues(objects);
										row.ItemArray = objects;
										table.Rows.Add(row);

										if (command.CommandTimeout != 0 && DateTime.Now > expirationTime)
										{
											throw new Exception(OperationTimedOut);
										}
									}
								}
							} while (reader.NextResult());
						}

						return resultDataSet;
					}
					catch (SqlException exception)
					{
						// Transport error most likely due to SQL Server failover
						// Clear out pools if the following error occurs: "New request is not allowed to 
						// start because it should come with valid transaction descriptor" (3989).
						if ((exception.Number == 10054) || (exception.Number == 3989))
						{
							SqlConnection.ClearAllPools();
						}
						else
						{
							throw;
						}
					}
					finally
					{
						if (connection != null && connection.State != ConnectionState.Closed)
						{
							connection.Close();
						}
					}
				}
				while (--nRetryCount > 0);
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in GetDataSet:", se, command);

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}
				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in GetDataSet:\n", e, command);
				if (e.Message == OperationTimedOut)
				{
					throw new ConsolidatedDAException(e.Message);
				}
				throw new ConsolidatedDAException();
			}

			return resultDataSet;
		}


		/// <summary>
		/// Executes the specified Sql command and returns the first column of the first row 
		/// in the result set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="command">The Sql command</param>
		/// <param name="security">The security object</param>
		/// <returns>Returns the value of the @Result parameter.</returns>
		public Object ExecuteScalar(SqlCommand command, SecurityClass security)
		{
			Object result = null;
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			SqlConnection connection = null;

			try
			{
				int nRetryCount = 1;
				do
				{
					try
					{
						connection = new SqlConnection(ConnectionString);
						connection.Open();
						command.Connection = connection;
						command.CommandTimeout = 180;
						SqlDataAdapter adapter = new SqlDataAdapter(command);
						result = adapter.SelectCommand.ExecuteScalar();
					}
					catch (SqlException exception)
					{
						// Transport error most likely due to SQL Server failover
						// Clear out pools if the following error occurs: "New request is not allowed to 
						// start because it should come with valid transaction descriptor" (3989).
						if ((exception.Number == 10054) || (exception.Number == 3989))
						{
							SqlConnection.ClearAllPools();
						}
						else
						{
							throw;
						}
					}
					finally
					{
						if (connection != null && connection.State != ConnectionState.Closed)
						{
							connection.Close();
						}
					}
				}
				while (--nRetryCount > 0);
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteScalar:", se, command);

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}

				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in ExecuteScalar:\n", e, command);
				throw new ConsolidatedDAException();
			}

			return result;
		}

		/// <summary>
		/// This function logs an error message with standard information (including parameters) 
		/// formatted to the event log.
		/// </summary>
		/// <param name="headerText">The header text for the logged error message.</param>
		/// <param name="exception">The exception to be logged.</param>
		/// <param name="command">The SqlCommand object in use that encountered the error.</param>
		public static void LogDatabaseError(string headerText, Exception exception, SqlCommand command)
		{
			string parameterMessage;

			if (command.Parameters.Count > 0)
			{
				var sb = new StringBuilder();
				sb.Append("Parameters\n===============\n");
				foreach (SqlParameter parameter in command.Parameters)
				{
					var value = parameter.Value ?? "<value null>";
					sb.Append($"{parameter.ParameterName} = \"{value}\"\n");
				}

				parameterMessage = sb.ToString();
			}
			else
			{
				parameterMessage = "No parameters on command object.";
			}

			string messageText =
				 $"{headerText}\n{exception.Message}\n{exception.StackTrace}\n\nSQL Command Text:\n{command.CommandText}\n\n{parameterMessage}";

         WriteEventErrorMessage(messageText);
			Trace.WriteLine(messageText, headerText);
		}

		/// <summary>
		/// This function logs an error message with standard information formatted to the event log.
		/// </summary>
		/// <param name="headerText"></param>
		/// <param name="exception"></param>
		/// <param name="commandText"></param>
		/// <returns></returns>
		public static void LogDatabaseError(string headerText, Exception exception, string commandText)
		{
			string messageText = $"{headerText}\n{exception.Message}\n{exception.StackTrace}\n\nSQL Command Text:\n{commandText}";
			WriteEventErrorMessage(messageText);
			Trace.WriteLine(messageText, headerText);
		}

		/// <summary>
		/// This function logs the messageText to the event log.
		/// </summary>
		/// <param name="messageText"></param>
		public static void WriteEventErrorMessage(string messageText)
		{
			var eventLog = new FMEventLog();
			eventLog.WriteEntry(messageText, FMEventLogEntryType.Error);
		}


		/// <summary>
		/// Executes SQL Insert commands.  This version is specifically intended
		/// for insert SQL followed by select SQL selecting the index of the newly
		/// inserted record.  Note that if <paramref name="security"/>.UserID
		/// is <see cref="DBAccess.ServiceLoginAccess">DBAccess.ServiceLogin</see> then
		/// access will by the NETWORK_SERVICE account
		/// </summary>
		/// <param name="security">
		/// SecurityClass object of the
		/// user making the call.  Contains
		/// info used for logging end user's connection details.
		/// </param>
		/// <param name="command">
		/// Insert command to be run.  Should also contain a select
		/// statement selecting the index of the newly inserted record
		/// </param>
		/// <param name="uniquifier">See remarks</param>
		/// <returns>
		/// Dataset containing one table with one row containing the
		/// index of the newly inserted record.  This depends on the insert being formed
		/// appropriately.  The Dataset actually conforms to any selection query passed in.
		/// </returns>
		/// <remarks>
		/// This handles only INSERT statements.  I named it "ExecuteQuery()"
		/// instead of "GetDataSet()" to make it easier to, one day, fix all
		/// by using SCOPE_IDENTITY() in a stored procedure (somehow).
		/// Doing an INSERT, and then turning around and doing a SELECT
		/// to get the IDENTITY just created, is very inefficient.
		////
		/// I added "Uniquifier" because you can't overload based only on return type
		/// (in any CLS language - see Rule 38), and the only alternative, an OUT parameter,
		/// would have required more work.  Remember this is just stopgap code, until
		/// we can find time to make all our INSERTs twice as fast by using SCOPE_IDENTITY().
		/// 
		/// CHK - new version to accept SqlCommand objects
		/// </remarks>
		public DataSet ExecuteQuery(SecurityClass security, SqlCommand command, int uniquifier)
		{
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			SqlConnection connection = null;
			DataSet dsIdentity = null;
			SqlDataAdapter adapter = null;

			try
			{
				dsIdentity = new DataSet();
		
				//HardwareKeyClass hardwareKey = new HardwareKeyClass();
				//if (hardwareKey.IsDescKey() && (security.UserID != DBAccess.ServiceLoginAccess))
				//{
				//   connectionBuilder.IntegratedSecurity = false;
				//   connectionBuilder.UserID = security.UserID;
				//   connectionBuilder.Password = DBAccess.GetDBPasswordAccess( security.Password );
				//}

				connection = new SqlConnection(ConnectionString);
				connection.Open();
				command.Connection = connection;

				long? sessionToSqlIndex = MapSqlConnectionToSession(security, connection);

				adapter = new SqlDataAdapter(command);
				adapter.Fill(dsIdentity);

				if (sessionToSqlIndex.HasValue)
				{
					UnMapSqlConnectionFromSession(connection, sessionToSqlIndex.Value);
				}
			}
			catch (SqlException se)
			{
				if (se.Number == 50000)
				{
					// user defined error message from RAISERROR
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);

				dsIdentity?.Dispose();

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}

				throw new ConsolidatedDAException();

			}
			catch (Exception e)
			{
				dsIdentity?.Dispose();

				LogDatabaseError("Exception in ExecuteQuery:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (null != command.Connection)
				{
					if (connection.State == ConnectionState.Open)
					{
						connection.Close();
					}
				}

				adapter?.Dispose();
			}

			return dsIdentity;
		}

		/// <summary>
		/// This is used to handle UPDATE and DELETE commands.  Note that if <paramref name="security"/>.UserID
		/// is <see cref="DBAccess.ServiceLoginAccess">DBAccess.ServiceLogin</see> then
		/// access will by the NETWORK_SERVICE account
		/// </summary>
		/// <param name="security">
		/// SecurityClass object of the user making the call.  Contains
		/// info used for logging end user's connection details.
		/// </param>
		/// <param name="command">
		/// <see cref="SqlCommand"/> with the command text, command type, and all parameters set
		/// </param>
		/// <returns>
		/// The number of rows affected by the main query, i.e. not the session preamble and post-amble updates
		/// </returns>
		/// <exception cref="SqlException">
		/// Immediately re-throws a <see cref="SqlException"/> if a the query executed a RAISERROR with a user defined error message (50000)
		/// </exception>
		/// <exception cref="ConsolidatedDAException">
		/// Throws a <see cref="ConsolidatedDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// This version of ExecuteQuery was added to deal with BLOBs,
		/// which are not easily built up into a string of T-SQL.
		/// Use of command objects in general is preferred.
		/// More specific <see cref="ConsolidatedDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteQuery(SecurityClass security, SqlCommand command)
		{
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			// Always associate the execution of the query with the current session so that audit and change tracking
			// triggers can identify the execution context.
			return this.ExecuteQueryWithinSessionContext(security, command);
		}

		/// <summary>
		/// This is used to handle UPDATE and DELETE commands.  This is to 
		/// be used for those calls which must be handled by a special service login.
		/// </summary>
		/// <param name="security">
		/// SecurityClass object of the user making the call.  Contains
		/// info used for logging end user's connection details.
		/// </param>
		/// <param name="command">
		/// <see cref="SqlCommand"/> with the command text, command type, and all parameters set
		/// </param>
		/// <param name="databaseServiceLogOn">
		/// The database service login Id
		/// </param>
		/// <returns>
		/// The number of rows affected by the main query, i.e. not the session preamble and post-amble updates
		/// </returns>
		/// <exception cref="SqlException">
		/// Immediately re-throws a <see cref="SqlException"/> if a the query executed a RAISERROR with a user defined error message (50000)
		/// </exception>
		/// <exception cref="ConsolidatedDAException">
		/// Throws a <see cref="ConsolidatedDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// This version of ExecuteQuery was added to use a service login rather than the
		/// logged in user.  Some actions should be only performed by service accounts to
		/// satisfy audit logging.
		/// More specific <see cref="ConsolidatedDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteQuery(SecurityClass security, SqlCommand command, string databaseServiceLogOn)
		{
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			if (!DBAccess.IsValidServiceLogin(databaseServiceLogOn))
			{
				throw new ArgumentException(@"This function may only be executed by a service login.", nameof(databaseServiceLogOn));
			}

			// Always associate the execution of the query with the current session so that audit and change tracking
			// triggers can identify the execution context.
			return this.ExecuteQueryWithinSessionContext(security, command);
		}

		/// <summary>
		/// Used specifically to delete session records.  This implementation bypasses the association between the 
		/// session record and the SQL process ID before and after the execution of the passed in <see cref="SqlCommand"/>.
		/// Note that if <paramref name="security"/>.UserID is 
		/// <see cref="DBAccess.ServiceLoginAccess">DBAccess.ServiceLogin</see> then
		/// access will by the NETWORK_SERVICE account
		/// </summary>
		/// <param name="security">
		/// SecurityClass object of the user making the call.  Contains
		/// info used for logging end user's connection details.
		/// </param>
		/// <param name="command">
		/// <see cref="SqlCommand"/> with the command text, command type, and all parameters set
		/// </param>
		/// <returns>
		/// The number of rows affected by the main query.
		/// </returns>
		/// <exception cref="SqlException">
		/// Immediately re-throws a <see cref="SqlException"/> if a the query executed a RAISERROR with a user defined error message (50000)
		/// </exception>
		/// <exception cref="ConsolidatedDAException">
		/// Throws a <see cref="ConsolidatedDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// Use of command objects in general is preferred.
		/// More specific <see cref="ConsolidatedDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteSessionCleanupQuery(SecurityClass security, SqlCommand command)
		{
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			// Note: When deleting a session record we're calling a different
			// method which executes without a session context.
			return this.ExecuteQueryWithoutSessionContext(security, command);
		}

		/// <summary>
		/// Used specifically to delete session records.  This implementation bypasses the association between the 
		/// session record and the SQL process ID before and after the execution of the passed in <see cref="SqlCommand"/>.
		/// This is to be used for those calls which must be handled by a special service login.
		/// </summary>
		/// <param name="security">
		/// SecurityClass object of the user making the call.  Contains
		/// info used for logging end user's connection details.
		/// </param>
		/// <param name="command">
		/// <see cref="SqlCommand"/> with the command text, command type, and all parameters set
		/// </param>
		/// <param name="databaseServiceLogOn">
		/// The database service login Id
		/// </param>
		/// <returns>
		/// The number of rows affected by the main query.
		/// </returns>
		/// <exception cref="SqlException">
		/// Immediately re-throws a <see cref="SqlException"/> if a the query executed a RAISERROR with a user defined error message (50000)
		/// </exception>
		/// <exception cref="ConsolidatedDAException">
		/// Throws a <see cref="ConsolidatedDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// This version of ExecuteSessionCleanupQuery was added to use a service login rather than the
		/// logged in user.  Some actions should be only performed by service accounts to satisfy audit logging.
		/// More specific <see cref="ConsolidatedDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteSessionCleanupQuery(SecurityClass security, SqlCommand command, string databaseServiceLogOn)
		{
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			if (!DBAccess.IsValidServiceLogin(databaseServiceLogOn))
			{
				throw new ArgumentException(@"This function may only be executed by a service login.", nameof(databaseServiceLogOn));
			}

			// Note: When deleting a session record we're calling a different
			// method which executes without a session context.
			return this.ExecuteQueryWithoutSessionContext(security, command);
		}

		/// <summary>
		/// Special purpose function to cause the appearance of a failed login
		/// attempt.  Needed for DESC security requirements.
		/// </summary>
		/// <param name="security">Security object of user to show failed login attempt for</param>
		public void SplashLogin(SecurityClass security)
		{
			HardwareKeyClass hardwareKey = new HardwareKeyClass();
			if (!hardwareKey.IsDescKey())
			{
				return;
			}

			this.CheckVersion();

			// Run the SQL.
			using (var connection = new SqlConnection(ConnectionString))
			{
				try
				{
					connection.Open();
				}
				catch (SqlException)
				{
					// Do nothing.  This exception is expected
				}
			}
		}

		/// <summary>
		/// Special purpose function to create the database user associated with the application user.
		/// </summary>
		/// <param name="security">Security context of the administrator creating the new user in the system</param>
		/// <param name="userId">User ID of the new user to be created</param>
		/// <param name="userSite">
		/// Site to whom the new user belongs
		/// 2009-09-25 - no longer used.
		/// </param>
		/// <param name="userApplicationPassword">Application password of the new database user</param>
		/// <remarks>
		/// This function only applies for DESC systems.  For non-DESC systems, this function
		/// returns immediately after doing nothing.
		/// 
		/// This function not only creates the database user, it also maps the user into both the ConsolidatedDB
		/// and master databases and adds the user to the FMDUserRole database role in both databases.
		/// </remarks>
		// ReSharper disable once InconsistentNaming
		public void CreateDBUser(SecurityClass security, string userId, string userSite, string userApplicationPassword)
		{
			this.CheckVersion();

			var hardwareKey = new HardwareKeyClass();

			if (!hardwareKey.IsDescKey())
			{
				// This only applies to US DoD security requirements.
				return;
			}
			if (SqlScrubber.IsSqlOk(userId) == false)
			{
				WriteEventErrorMessage("Illegal SQL string passed in:\n" + userId);
				throw new ConsolidatedDAException();
			}

			// Run the SQL.
			var connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);
			var connection = new SqlConnection(connectionBuilder.ConnectionString);

			using (var command = new SqlCommand())
			{
				try
				{
					connection.Open();

					command.Connection = connection;

					command.CommandText = "CREATE LOGIN [" + userId + "] WITH PASSWORD = '" + DBAccess.GetDBPasswordAccess(userApplicationPassword) + "', ";
					command.CommandText += "DEFAULT_DATABASE = " + this.DatabaseName + ", CHECK_POLICY = OFF";
					command.CommandType = CommandType.Text;

					command.ExecuteNonQuery();

					SetCreateUserCommandText(userId, command, isSQLDependency: true);
					command.ExecuteNonQuery();

					SetGrantImpresonateCommandText(userId, command);
					command.ExecuteNonQuery();

					command.CommandText = "sp_addrolemember";
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@rolename", "FMDUserRole");
					command.Parameters.AddWithValue("@membername", userId);

					command.ExecuteNonQuery();
				}
				catch (SqlException se)
				{
					if (se.Number == 50000) // user defined error message from RAISERROR
					{
						// Allow exceptions from our own RAISERROR calls to pass through
						throw;
					}

					LogDatabaseError("Exception in CreateDBUser:", se, command);

					if (se.Number == SQLServerCommandTimeoutErrorCode)
					{
						throw new ConsolidatedDAException(OperationTimedOut);
					}

					throw new ConsolidatedDAException();
				}
				catch (Exception e)
				{
					LogDatabaseError("Exception in CreateDBUser:", e, command);
					throw new ConsolidatedDAException();
				}
				finally
				{
					if (command.Connection?.State == ConnectionState.Open)
					{
						command.Connection.Close();
					}
				}

				// Now create user in archive database.
				connectionBuilder.InitialCatalog = this.ArchiveDatabaseName;
				command.Connection = new SqlConnection(connectionBuilder.ConnectionString);
				try
				{
					command.Connection.Open();

					command.Parameters.Clear();

					SetCreateUserCommandText(userId, command, isSQLDependency: false);
					command.ExecuteNonQuery();

					command.Parameters.Clear();

					command.CommandText = "sp_addrolemember";
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@rolename", "FMDUserRole");
					command.Parameters.AddWithValue("@membername", userId);

					command.ExecuteNonQuery();
				}
				catch (SqlException se)
				{
					// Ignore it if the database does not exist
					if (se.Number != 4060)
					{
						if (se.Number == 50000) // user defined error message from RAISERROR
						{
							// Allow exceptions from our own RAISERROR calls to pass through
							throw;
						}

						LogDatabaseError("Exception in CreateDBUser:", se, command);

						if (se.Number == SQLServerCommandTimeoutErrorCode)
						{
							throw new ConsolidatedDAException(OperationTimedOut);
						}


						throw new ConsolidatedDAException();
					}
				}
				catch (Exception e)
				{
					LogDatabaseError("Exception in CreatedDBUser:", e, command);
					throw new ConsolidatedDAException();
				}
				finally
				{
					if (command.Connection?.State == ConnectionState.Open)
					{
						command.Connection.Close();
					}
				}

				// Now create user in master.  Lockdown boxes require this
				connectionBuilder.InitialCatalog = "master";
				command.Connection = new SqlConnection(connectionBuilder.ConnectionString);
				try
				{
					command.Connection.Open();

					command.Parameters.Clear();

					command.CommandText = "CREATE USER [" + userId + "]";
					command.CommandType = CommandType.Text;

					command.ExecuteNonQuery();

					command.Parameters.Clear();

					command.CommandText = "sp_addrolemember";
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@rolename", "FMDUserRole");
					command.Parameters.AddWithValue("@membername", userId);

					command.ExecuteNonQuery();
				}
				catch (SqlException se)
				{
					if (se.Number == 50000) // user defined error message from RAISERROR
					{
						// Allow exceptions from our own RAISERROR calls to pass through
						throw;
					}

					LogDatabaseError("Exception in CreateDBUser:", se, command);

					if (se.Number == SQLServerCommandTimeoutErrorCode)
					{
						throw new ConsolidatedDAException(OperationTimedOut);
					}

					throw new ConsolidatedDAException();
				}
				catch (Exception e)
				{
					LogDatabaseError("Exception in CreatedDBUser:", e, command);
					throw new ConsolidatedDAException();
				}
				finally
				{
					if (command.Connection?.State == ConnectionState.Open)
					{
						command.Connection.Close();
					}
				}
			}
		}

		[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		private static void SetGrantImpresonateCommandText(string userId, SqlCommand command)
		{
			command.CommandText = "grant impersonate on USER::[NT AUTHORITY\\NETWORK SERVICE] to [" + userId + "]";
			command.CommandType = CommandType.Text;
		}

		/// <summary>
		/// The purpose of this method is to set the CREATE USER command text and provide a small code nugget
		/// we can decorate with the SQL Review suppression attribute.  This is because we cannot do parameterized
		/// calls to CREATE USER.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="command"></param>
		/// <param name="isSQLDependency"></param>
		[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		private static void SetCreateUserCommandText(string userId, SqlCommand command, bool isSQLDependency)
		{
			command.CommandText = "CREATE USER [" + userId + "]";
			if (isSQLDependency)
			{
				command.CommandText += " WITH DEFAULT_SCHEMA = [SQLDependency]";
			}
			command.CommandType = CommandType.Text;
		}

		/// <summary>
		/// Special purpose function to delete the database user associated with the application user.
		/// </summary>
		/// <param name="security">Security context of the administrator deleting the user in the system</param>
		/// <param name="userId">User ID of the user to be deleted</param>
		/// <param name="userSite">
		/// Site to whom the user to delete belongs
		/// 2009-09-25 - no longer used.
		/// </param>
		/// <remarks>This function only applies for DESC systems.  For non-DESC systems, this function
		/// returns immediately after doing nothing</remarks>
		// ReSharper disable once InconsistentNaming
		public void DeleteDBUser(SecurityClass security, string userId, string userSite)
		{
			this.CheckVersion();

			HardwareKeyClass hardwareKey = new HardwareKeyClass();

			if (!hardwareKey.IsDescKey())
			{
				// This only applies to US DoD security requirements.
				return;
			}

			if (SqlScrubber.IsSqlOk(userId) == false)
			{
				WriteEventErrorMessage("Illegal SQL string passed in:\n" + userId);
				throw new ConsolidatedDAException();
			}

			// Run the SQL.
			SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

			string ifUserExistsSQL = "IF EXISTS(SELECT * FROM sys.database_principals WHERE type='S' AND name=N'" + userId + "') ";
			SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString);

			SqlCommand command = new SqlCommand();
			try
			{
				connection.Open();

				command.Connection = connection;

				command.CommandText = ifUserExistsSQL + "DROP USER [" + userId + "]";
				command.CommandType = CommandType.Text;

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in DeleteDBUser:", se, command);

				if (se.Number == -2)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}

				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in DeleteDBUser:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}

			// Now remove user from Archive.  Lockdown boxes require this
			connectionBuilder.InitialCatalog = this.ArchiveDatabaseName;
			command.Connection = new SqlConnection(connectionBuilder.ConnectionString);
			try
			{
				command.Connection.Open();

				command.CommandText = ifUserExistsSQL + "DROP USER [" + userId + "]";
				command.CommandType = CommandType.Text;

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				// Ignore and go on if the database does not exist
				if (se.Number != 4060)
				{
					if (se.Number == 50000) // user defined error message from RAISERROR
					{
						// Allow exceptions from our own RAISERROR calls to pass through
						throw;
					}

					LogDatabaseError("Exception in DeleteDBUser:", se, command);

					if (se.Number == SQLServerCommandTimeoutErrorCode)
					{
						throw new ConsolidatedDAException(OperationTimedOut);
					}

					throw new ConsolidatedDAException();
				}
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in DeleteDBUser:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}

			ifUserExistsSQL = "IF EXISTS(SELECT * FROM master.sys.database_principals WHERE type='S' AND name=N'" + userId + "') ";
			string ifLoginExistsSQL = "IF EXISTS(SELECT * FROM master.sys.server_principals WHERE type='S' AND name=N'" + userId + "') ";
			// Now remove user from master.  Lockdown boxen require this
			connectionBuilder.InitialCatalog = "master";
			command.Connection = new SqlConnection(connectionBuilder.ConnectionString);
			try
			{
				command.Connection.Open();

				command.CommandText = ifUserExistsSQL + "DROP USER [" + userId + "]";
				command.CommandType = CommandType.Text;

				command.ExecuteNonQuery();

				command.CommandText = ifLoginExistsSQL + "DROP LOGIN [" + userId + "]";
				command.CommandType = CommandType.Text;

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);
				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in DeleteDBUser:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}
		}


		/// <summary>
		/// Adds the role backup operator.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="userId">The user id.</param>
		/// <exception cref="FMBusinessObjects.Exceptions.ConsolidatedDAException">
		/// </exception>
		public void AddRoleBackupOperator(SecurityClass security, string userId)
		{
			var hardwareKey = new HardwareKeyClass();

			if (!hardwareKey.IsDescKey())
			{
				// This only applies to US DoD security requirements.
				return;
			}

			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(userId) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + userId);
				throw new ConsolidatedDAException();
			}

			// Run the SQL.
			var connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

			var connection = new SqlConnection(connectionBuilder.ConnectionString);
			var command = new SqlCommand();

			try
			{
				connection.Open();

				command.Connection = connection;

				command.CommandText = "sp_addrolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "db_backupoperator");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);
				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in PromoteToAdmin:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}
		}

		public void DropRoleBackupOperator(SecurityClass security, string userId)
		{
			var hardwareKey = new HardwareKeyClass();

			if (!hardwareKey.IsDescKey())
			{
				// This only applies to US DoD security requirements.
				return;
			}

			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(userId) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + userId);
				throw new ConsolidatedDAException();
			}

			// Run the SQL.
			SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

			var connection = new SqlConnection(connectionBuilder.ConnectionString);
			var command = new SqlCommand();

			try
			{
				connection.Open();

				command.Connection = connection;

				command.CommandText = "sp_droprolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "db_backupoperator");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);
				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in PromoteToAdmin:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}
		}

		/// <summary>
		/// Used to grant application administrators sufficient rights in the database to
		/// perform application administrative operations.
		/// </summary>
		/// <param name="security">Security context of the logged in administrator</param>
		/// <param name="userId">id of the user to promote to administrator</param>
		/// <remarks>This function only applies for DESC systems.  For non-DESC systems, this function
		/// returns immediately after doing nothing</remarks>
		public void PromoteToAdmin(SecurityClass security, string userId)
		{
			var hardwareKey = new HardwareKeyClass();

			if (!hardwareKey.IsDescKey())
			{
				// This only applies to US DoD security requirements.
				return;
			}

			if (security.UserID.Equals(userId, StringComparison.CurrentCultureIgnoreCase))
			{
				throw new ConsolidatedDAException("Users may not promote themselves to admin");
			}

			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(userId) == false)
			{
				WriteEventErrorMessage("Illegal SQL string passed in:\n" + userId);
				throw new ConsolidatedDAException();
			}

			// Run the SQL.
			SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

			var connection = new SqlConnection(connectionBuilder.ConnectionString);
			var command = new SqlCommand();

			try
			{
				connection.Open();

				command.Connection = connection;

				command.CommandText = "sp_addrolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "FMDAdminRole");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();

				command.Parameters.Clear();

				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "db_owner");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();

				command.Parameters.Clear();

				command.CommandText = "sp_addsrvrolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@loginame", userId);
				command.Parameters.AddWithValue("@rolename", "securityadmin");

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);
				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in PromoteToAdmin:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}

			connectionBuilder.InitialCatalog = this.ArchiveDatabaseName;
			command.Connection = new SqlConnection(connectionBuilder.ConnectionString);
			try
			{
				command.Connection.Open();

				command.Parameters.Clear();

				command.CommandText = "sp_addrolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "FMDAdminRole");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();

				command.Parameters.Clear();

				command.CommandText = "sp_addsrvrolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@loginame", userId);
				command.Parameters.AddWithValue("@rolename", "securityadmin");

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number != 4060)
				{
					if (se.Number == 50000) // user defined error message from RAISERROR
					{
						// Allow exceptions from our own RAISERROR calls to pass through
						throw;
					}

					LogDatabaseError("Exception in ExecuteQuery:", se, command);
					throw new ConsolidatedDAException();
				}
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in PromoteToAdmin:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}

			// Now create user in master.  Lockdown boxen require this
			connectionBuilder.InitialCatalog = "master";
			command.Connection = new SqlConnection(connectionBuilder.ConnectionString);
			try
			{
				command.Connection.Open();

				command.Parameters.Clear();

				command.CommandText = "sp_addrolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "FMDAdminRole");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();

				command.Parameters.Clear();

				command.CommandText = "GRANT ALTER TRACE TO [" + userId + "]";
				command.CommandType = CommandType.Text;

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);
				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in PromoteToAdmin:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}
		}

		/// <summary>
		/// Used to revoke database rights from users who are 
		/// no longer application administrators.
		/// </summary>
		/// <param name="security">Security context of the logged in administrator</param>
		/// <param name="userId">id of the user to demote to regular user</param>
		/// <remarks>This function only applies for DESC systems.  For non-DESC systems, this function
		/// returns immediately after doing nothing</remarks>
		public void DemoteFromAdmin(SecurityClass security, string userId)
		{
			var hardwareKey = new HardwareKeyClass();

			if (!hardwareKey.IsDescKey())
			{
				// This only applies to US DoD security requirements.
				return;
			}

			if (security.UserID.Equals(userId, StringComparison.CurrentCultureIgnoreCase))
			{
				throw new ConsolidatedDAException("Users may not demote themselves from admin");
			}

			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(userId) == false)
			{
				WriteEventErrorMessage("Illegal SQL string passed in:\n" + userId);
				throw new ConsolidatedDAException();
			}

			// Run the SQL.
			SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

			var connection = new SqlConnection(connectionBuilder.ConnectionString);
			var command = new SqlCommand();

			try
			{
				connection.Open();

				command.Connection = connection;

				command.CommandText = "sp_dropsrvrolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@loginame", userId);
				command.Parameters.AddWithValue("@rolename", "securityadmin");

				command.ExecuteNonQuery();

				command.Parameters.Clear();
				command.CommandText = "sp_droprolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "FMDAdminRole");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();

				command.Parameters.Clear();
				command.Parameters.AddWithValue("@rolename", "db_owner");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);
				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in DemoteFromAdmin:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				command.Connection.Close();
			}

			// Now remove user from master.  Lockdown boxen require this
			connectionBuilder.InitialCatalog = "master";
			command.Connection = new SqlConnection(connectionBuilder.ConnectionString);
			try
			{
				command.Connection.Open();

				command.Parameters.Clear();

				command.CommandText = "sp_droprolemember";
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@rolename", "FMDAdminRole");
				command.Parameters.AddWithValue("@membername", userId);

				command.ExecuteNonQuery();

				command.Parameters.Clear();

				command.CommandText = "REVOKE ALTER TRACE FROM [" + userId + "]";
				command.CommandType = CommandType.Text;

				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in DemoteFromAdmin:", se, command);

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}

				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in DemoteFromAdmin:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				command.Connection?.Close();
			}
		}

		/// <summary>
		/// Special purpose function to update the database users password associated with the application user.
		/// </summary>
		/// <param name="security">Security context of the administrator creating the new user in the system</param>
		/// <param name="userId">User ID of the new user to be created</param>
		/// <param name="userSite">
		/// Site to whom the new user belongs
		/// 2009-09-25 - no longer used.
		/// </param>
		/// <param name="oldApplicationPassword">Old application password of the database user</param>
		/// <param name="newApplicationPassword">New application password of the database user</param>
		/// <remarks>This function only applies for DESC systems.  For non-DESC systems, this function
		/// returns immediately after doing nothing</remarks>
		// ReSharper disable once InconsistentNaming
		public void UpdateDBUserPassword(SecurityClass security, string userId, string userSite, string oldApplicationPassword, string newApplicationPassword)
		{
			HardwareKeyClass hardwareKey = new HardwareKeyClass();

			if (!hardwareKey.IsDescKey())
			{
				// This only applies to US DoD security requirements.
				return;
			}

			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(userId) == false)
			{
				WriteEventErrorMessage("Illegal SQL string passed in:\n" + userId);
				throw new ConsolidatedDAException();
			}

			// Run the SQL.
			SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

			using (var connection = new SqlConnection(connectionBuilder.ConnectionString))
			{
				using (SqlCommand command = new SqlCommand())
				{
					AlterPassword(security, userId, oldApplicationPassword, newApplicationPassword, connection, command);
				}
			}
		}

		private static void AlterPassword(SecurityClass security, string userId, string oldApplicationPassword, string newApplicationPassword, SqlConnection connection, SqlCommand command)
		{
			try
			{
				command.Connection = connection;

				connection.Open();

				command.CommandText = "ALTER LOGIN [" + userId + "] WITH PASSWORD = '" + DBAccess.GetDBPasswordAccess(newApplicationPassword) + "' ";
				if (security.UserID.ToUpper() == userId.ToUpper())
				{
					// supply OLD_PASSWORD ONLY when a user is changing own password
					command.CommandText += "OLD_PASSWORD = '" + DBAccess.GetDBPasswordAccess(oldApplicationPassword) + "'";
				}
				else
				{
					command.CommandText += ", CHECK_POLICY = OFF";
				}
				command.CommandType = CommandType.Text;
				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000) // user defined error message from RAISERROR
				{
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in UpdateDBUserPassword:", se, command);

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}

				throw new ConsolidatedDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in UpdateDBUserPassword:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				command.Connection?.Close();
			}
		}

		/// <summary>
		/// Takes a collection of values and creates SqlParameters and a comma-delimited
		/// list for use in a SQL IN clause.
		/// </summary>
		/// <param name="paramCollection">Collection to which the new parameters will be
		/// appended</param>
		/// <param name="paramValues">The values for the parameters</param>
		/// <param name="baseParamName">The base name to use, e.g., "@Prm" for this
		/// would cause parameters to be created with names "@Prm1", "@Prm2", etc.</param>
		/// <param name="dbType">Type of the parameters</param>
		/// <returns>Comma-delimited list of the parameters, e.g., "@Prm1, @Prm2, @Prm3"</returns>
		public static string ConstructSqlParametersFromCollection(SqlParameterCollection paramCollection, IEnumerable paramValues, string baseParamName, SqlDbType dbType)
		{
			return ConstructSqlParametersFromCollection(paramCollection, paramValues, baseParamName, dbType, -1);
		}

		/// <summary>
		/// Takes a collection of values and creates SqlParameters and a comma-delimited
		/// list for use in a SQL IN clause.
		/// </summary>
		/// <param name="paramCollection">Collection to which the new parameters will be
		/// appended</param>
		/// <param name="paramValues">The values for the parameters</param>
		/// <param name="baseParamName">The base name to use, e.g., "@Prm" for this
		/// would cause parameters to be created with names "@Prm1", "@Prm2", etc.</param>
		/// <param name="dbType">Type of the parameters</param>
		/// <param name="size">Length of the parameters</param>
		/// <returns>Comma-delimited list of the parameters, e.g., "@Prm1, @Prm2, @Prm3"</returns>
		public static string ConstructSqlParametersFromCollection(SqlParameterCollection paramCollection, IEnumerable paramValues, string baseParamName, SqlDbType dbType, int size)
		{
			StringBuilder paramList = new StringBuilder();
			int paramNumber = 0;

			if (!baseParamName.StartsWith("@"))
			{
				baseParamName = "@" + baseParamName;
			}

			foreach (object value in paramValues)
			{
				paramNumber++;
				string paramName = baseParamName + paramNumber;

				paramList.Append(paramName + ",");

				SqlParameter param;
				if (size > 0)
				{
					param = paramCollection.Add(paramName, dbType, size);
				}
				else
				{
					param = paramCollection.Add(paramName, dbType);
				}

				param.Value = value ?? DBNull.Value;
			}

			if (paramList.Length > 0)
			{
				return paramList.ToString().TrimEnd(',');
			}

			return string.Empty;
		}

		/// <summary>
		/// The map SQL connection to session.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="connection">
		/// The connection.
		/// </param>
		/// <returns>
		/// Returns the <see cref="long"/> index of the newly created map record that was just created.
		/// </returns>
		public static long? MapSqlConnectionToSession(SecurityClass security, SqlConnection connection)
		{
			long? index;

			using (SqlCommand insertCommand = SecurityClass.CreateInsertSqlSessionCommand(security))
			{
				try
				{
					insertCommand.Connection = connection;
					object result = insertCommand.ExecuteScalar();
					index = ((result == null) || DataObject.isNull(result)) ? (long?)null : Convert.ToInt64(result);
				}
				catch (Exception insertException)
				{

					if (insertException.Message.IndexOf("Violation of UNIQUE KEY constraint", 0, StringComparison.CurrentCultureIgnoreCase) != -1)
					{
						// if insertion causes unique constraint violation, delete the previous mapping
						// delete the record with the existing sql id
						using (var deleteCommand = new SqlCommand(SecurityClass.CreateDeleteCurrentSqlSessionCommandString(), connection))
						{
							try
							{
								// delete the existing mapping
								deleteCommand.Connection = connection;
								deleteCommand.ExecuteNonQuery();
							}

							catch (Exception deleteException)
							{
								// pass the exception up
								LogDatabaseError("Exception in MapSqlConnectionToSession:", deleteException, deleteCommand);
								throw new Exception("Exception Mapping SqlConnection To Session", deleteException);
							}

							try
							{
								// retry the insert of the new mapping, capturing the index
								object result = insertCommand.ExecuteScalar();
								index = ((result == null) || DataObject.isNull(result)) ? (long?)null : Convert.ToInt64(result);
							}
							catch (Exception insertException2)
							{
								// pass the exception up
								LogDatabaseError("Exception in MapSqlConnectionToSession:", insertException2, insertCommand);
								throw new Exception("Exception Mapping SqlConnection To Session", insertException2);
							}
						}
					}
					else
					{
						throw new Exception("Exception Mapping SqlConnection To Session", insertException);
					}
				}
			}

			return index;
		}

		/// <summary>
		/// Removes a previously mapped SQL connection from the specified session.
		/// </summary>
		/// <param name="connection">
		/// The connection.
		/// </param>
		/// <param name="index">
		/// The index of the mapping record that should be removed.
		/// </param>
		public static void UnMapSqlConnectionFromSession(SqlConnection connection, long index)
		{
			using (var deleteCommand = SecurityClass.CreateDeleteSqlSessionCommand(index))
			{
				try
				{
					deleteCommand.Connection = connection;
					deleteCommand.ExecuteNonQuery();
				}
				catch (Exception deleteException)
				{
					LogDatabaseError("Exception in UnMapSqlConnectionFromSession:", deleteException, deleteCommand);
					throw new Exception("Exception Un-Mapping SqlConnection From Session", deleteException);
				}
			}
		}

		#region Private ExecuteQuery methods execute the commands within or outside the context of the current Session

		/// <summary>
		/// Executes a query within context of a FuelsManager Session by associating the sql server process id with the passed in session.
		/// </summary>
		/// <param name="security">
		/// An instance of the security context that contains a reference to any session token.
		/// </param>
		/// <param name="command">
		/// An instance of a <see cref="SqlCommand"/> object that contains the sql statement to execute.
		/// </param>
		/// <returns>
		/// Returns an <see cref="int"/> output of the executed <see cref="SqlCommand"/>.
		/// </returns>
		/// <exception cref="SqlException">
		/// Immediately re-throws a <see cref="SqlException"/> if a the query executed a RAISERROR with a user defined error message (50000)
		/// </exception>
		/// <exception cref="ConsolidatedDAException">
		/// Throws a <see cref="ConsolidatedDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// More specific <see cref="ConsolidatedDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		private int ExecuteQueryWithinSessionContext(SecurityClass security, SqlCommand command)
		{
			long? sessionToSqlIndex = null;

			var connection = new SqlConnection(ConnectionString);

			try
			{
				connection.Open();
				command.Connection = connection;

				sessionToSqlIndex = MapSqlConnectionToSession(security, connection);

				int rowsAffected = command.ExecuteNonQuery();

				return rowsAffected;
			}
			catch (SqlException se)
			{
				if (se.Number == 50000
				&& se.Class != 18)
				{
					// 50000 = User defined error message from RAISERROR
					// Allow exceptions from our own RAISERROR calls to pass through
					// Strip the Uncommittable.. error as it is expected from a try catch within transaction scope.
					throw new ConsolidatedDAException(se.Message.Replace("Uncommittable transaction is detected at the end of the batch. The transaction is rolled back.", ""));
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);

            if (se.Number == 547 || se.Number == 50000)
				{
					if (se.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint", StringComparison.Ordinal) != -1)
					{
						throw new ConsolidatedDAException("Entity is being referenced and cannot be deleted.");
					}
					else if (se.Message.IndexOf("The MERGE statement conflicted with the FOREIGN KEY constraint", StringComparison.Ordinal) != -1)
					{
						throw new ConsolidatedDAException("Entity references another that does not exist. Have all referenced entities been assigned to the target Site?");
					}
					else if (se.Message.IndexOf("CK_map_tblUserToUserGroup_ViewOperateOnly") != -1)
					{
						throw new ConsolidatedDAException("Group Assignment would add View Operate Only to Administrator");
					}
					else if (se.Message.IndexOf("CK_map_tblGroupToRight_ViewOperateOnly") != -1)
					{
						throw new ConsolidatedDAException("Right Assignment would add View Operate Only to Administrator");
					}

					else if (se.Message.IndexOf("Uniqueness", StringComparison.Ordinal) != -1)
					{
						throw new ConsolidatedDAException("Operation would result in duplicate identifiers.");
					}
				}
				else if (se.Number == SQLServerUniqueConstraintViolationErrorCode || se.Number == SQLServerUniqueIndexViolationErrorCode)
				{
					// If a unique constraint or index is violated, return a user-friendly error message
					throw new ConsolidatedDAException(UniqueConstraintViolationErrorMessage);
				}

				if (se.Number == SQLServerStatementTerminatedMaximumRecursion)
				{
					throw new ConsolidatedDAException(StatementTerminatedMaximumRecursionErrorMessage);
				}

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}
				else
				{
					string[] result = se.Message.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
					throw new ConsolidatedDAException(result[0].Replace("Error: ", ""));
				}
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in ExecuteQuery:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				try
				{
					if (sessionToSqlIndex.HasValue)
					{
						UnMapSqlConnectionFromSession(connection, sessionToSqlIndex.Value);
					}
				}
				catch (Exception e)
				{
					LogDatabaseError("Exception in ExecuteQuery:", e, command);
				}

				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}
		}

		/// <summary>
		/// Executes a query without associating it with the context of a FuelsManager Session.
		/// </summary>
		/// <param name="security">
		/// An instance of the security context that contains a reference to any session token.
		/// </param>
		/// <param name="command">
		/// An instance of a <see cref="SqlCommand"/> object that contains the sql statement to execute.
		/// </param>
		/// <returns>
		/// Returns an <see cref="int"/> output of the executed <see cref="SqlCommand"/>.
		/// </returns>
		/// <exception cref="SqlException">
		/// Immediately re-throws a <see cref="SqlException"/> if a the query executed a RAISERROR with a user defined error message (50000)
		/// </exception>
		/// <exception cref="ConsolidatedDAException">
		/// Throws a <see cref="ConsolidatedDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// More specific <see cref="ConsolidatedDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteQueryWithoutSessionContext(SecurityClass security, SqlCommand command)
		{
			var connection = new SqlConnection(ConnectionString);

			try
			{
				connection.Open();
				command.Connection = connection;

				return command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				if (se.Number == 50000)
				{
					// 50000 = User defined error message from RAISERROR
					// Allow exceptions from our own RAISERROR calls to pass through
					throw;
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);

				if (se.Number == 547 && se.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint", StringComparison.Ordinal) != -1)
				{
					throw new ConsolidatedDAException("Entity is being referenced and cannot be deleted.");
				}
				else if (se.Number == SQLServerUniqueConstraintViolationErrorCode || se.Number == SQLServerUniqueIndexViolationErrorCode)
				{
					// If a unique constraint or index is violated, return a user-friendly error message
					throw new ConsolidatedDAException(UniqueConstraintViolationErrorMessage);
				}

				if (se.Number == SQLServerStatementTerminatedMaximumRecursion)
				{
					throw new ConsolidatedDAException(StatementTerminatedMaximumRecursionErrorMessage);
				}
				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new ConsolidatedDAException(OperationTimedOut);
				}
				else
				{
					string[] result = se.Message.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
					throw new ConsolidatedDAException(result[0].Replace("Error: ", ""));
				}
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in ExecuteQuery:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
				if (command.Connection?.State == ConnectionState.Open)
				{
					command.Connection.Close();
				}
			}
		}

		#endregion Private ExecuteQuery methods execute the commands within or outside the context of the current Session
	}
}
