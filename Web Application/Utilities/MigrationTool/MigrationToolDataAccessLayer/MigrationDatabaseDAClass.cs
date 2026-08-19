namespace MigrationToolDataAccessLayer
{
    using MigrationToolBusinessObjects.Exceptions;
    using System;
	using System.Collections;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Text;

	public partial class MigrationDatabaseDAClass
	{
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

		public string ConnectionString { get; set; }

		public string DatabaseName
		{
			get
			{
				var builder = new SqlConnectionStringBuilder(ConnectionString);
				return (!builder.InitialCatalog.Contains(" ")) ? builder.InitialCatalog : "[" + builder.InitialCatalog + "]";
			}
		}

		/// <summary>
		/// Opens and closes the connection to the database for test purposes.
		/// </summary>
		public void TestConnection()
		{
			var connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);
			var connection = new SqlConnection(connectionBuilder.ConnectionString);
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
		//public DataSet GetDataSet(SqlCommand command, string userId, string siteId)
		//{
		//	var security = new SecurityClass
		//	{
		//		UserID = userId,
		//		SiteID = siteId
		//	};
		//	return GetDataSet(command, security);
		//}


		/// <summary>
		/// Gets the data table.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		public DataTable GetDataTable(SqlCommand command)
		{
			DataSet resultSet = this.GetDataSet(command);
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
		public DataSet GetDataSet(SqlCommand command)
		{
			DataSet resultDataSet = new DataSet();
			SqlConnection connection = null;

			try
			{
				int nRetryCount = 1;

				do
				{
					try
					{
						var connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

						connection = new SqlConnection(connectionBuilder.ConnectionString);
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
					throw new MigrationDatabaseDAException(OperationTimedOut);
				}
				throw new MigrationDatabaseDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in GetDataSet:\n", e, command);
				if (e.Message == OperationTimedOut)
				{
					throw new MigrationDatabaseDAException(e.Message);
				}
				throw new MigrationDatabaseDAException();
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
		public Object ExecuteScalar(SqlCommand command)
		{
			Object result = null;

			SqlConnection connection = null;

			try
			{
				int nRetryCount = 1;
				do
				{
					try
					{
						SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);
						connection = new SqlConnection(connectionBuilder.ConnectionString);
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
					throw new MigrationDatabaseDAException(OperationTimedOut);
				}

				throw new MigrationDatabaseDAException();
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in ExecuteScalar:\n", e, command);
				throw new MigrationDatabaseDAException();
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
		public void LogDatabaseError(string headerText, Exception exception, SqlCommand command)
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
		public void LogDatabaseError(string headerText, Exception exception, string commandText)
		{
			string messageText = $"{headerText}\n{exception.Message}\n{exception.StackTrace}\n\nSQL Command Text:\n{commandText}";
			WriteEventErrorMessage(messageText);
			Trace.WriteLine(messageText, headerText);
		}

		/// <summary>
		/// This function logs the messageText to the event log.
		/// </summary>
		/// <param name="messageText"></param>
		public void WriteEventErrorMessage(string messageText)
		{
			this.WriteEntry(messageText, EventLogEntryType.Error);
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
		public DataSet ExecuteQuery(SqlCommand command, int uniquifier)
		{

			SqlConnection connection = new SqlConnection();
			DataSet dsIdentity = null;
			SqlDataAdapter adapter = null;

			try
			{
				dsIdentity = new DataSet();
				SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);

				connection.ConnectionString = connectionBuilder.ConnectionString;
				connection.Open();
				command.Connection = connection;

				adapter = new SqlDataAdapter(command);
				adapter.Fill(dsIdentity);
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
					throw new MigrationDatabaseDAException(OperationTimedOut);
				}

				throw new MigrationDatabaseDAException();

			}
			catch (Exception e)
			{
				dsIdentity?.Dispose();

				LogDatabaseError("Exception in ExecuteQuery:", e, command);
				throw new MigrationDatabaseDAException();
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
		/// <exception cref="MigrationDatabaseDAException">
		/// Throws a <see cref="MigrationDatabaseDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// This version of ExecuteQuery was added to deal with BLOBs,
		/// which are not easily built up into a string of T-SQL.
		/// Use of command objects in general is preferred.
		/// More specific <see cref="MigrationDatabaseDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteQuery(SqlCommand command)
		{

			// Always associate the execution of the query with the current session so that audit and change tracking
			// triggers can identify the execution context.
			return this.ExecuteQueryWithinSessionContext(command);
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
		/// <exception cref="MigrationDatabaseDAException">
		/// Throws a <see cref="MigrationDatabaseDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// This version of ExecuteQuery was added to use a service login rather than the
		/// logged in user.  Some actions should be only performed by service accounts to
		/// satisfy audit logging.
		/// More specific <see cref="MigrationDatabaseDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteQuery(SqlCommand command, string databaseServiceLogOn)
		{
			// Always associate the execution of the query with the current session so that audit and change tracking
			// triggers can identify the execution context.
			return this.ExecuteQueryWithinSessionContext(command);
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
		/// <exception cref="MigrationDatabaseDAException">
		/// Throws a <see cref="MigrationDatabaseDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// Use of command objects in general is preferred.
		/// More specific <see cref="MigrationDatabaseDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteSessionCleanupQuery(SqlCommand command)
		{
			// Note: When deleting a session record we're calling a different
			// method which executes without a session context.
			return this.ExecuteQueryWithoutSessionContext(command);
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
		/// <exception cref="MigrationDatabaseDAException">
		/// Throws a <see cref="MigrationDatabaseDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// This version of ExecuteSessionCleanupQuery was added to use a service login rather than the
		/// logged in user.  Some actions should be only performed by service accounts to satisfy audit logging.
		/// More specific <see cref="MigrationDatabaseDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteSessionCleanupQuery(SqlCommand command, string databaseServiceLogOn)
		{
			// Note: When deleting a session record we're calling a different
			// method which executes without a session context.
			return this.ExecuteQueryWithoutSessionContext(command);
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
		/// <exception cref="MigrationDatabaseDAException">
		/// Throws a <see cref="MigrationDatabaseDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// More specific <see cref="MigrationDatabaseDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		private int ExecuteQueryWithinSessionContext(SqlCommand command)
		{
			var connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);
			var connection = new SqlConnection(connectionBuilder.ConnectionString);

			try
			{
				connection.Open();
				command.Connection = connection;

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
					throw new MigrationDatabaseDAException(se.Message.Replace("Uncommittable transaction is detected at the end of the batch. The transaction is rolled back.", ""));
				}

				LogDatabaseError("Exception in ExecuteQuery:", se, command);

				if (se.Number == 547)
				{
					if (se.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint", StringComparison.Ordinal) != -1)
					{
						throw new MigrationDatabaseDAException("Entity is being referenced and cannot be deleted.");
					}
					else if (se.Message.IndexOf("The MERGE statement conflicted with the FOREIGN KEY constraint", StringComparison.Ordinal) != -1)
					{
						throw new MigrationDatabaseDAException("Entity references another that does not exist. Have all referenced entities been assigned to the target Site?");
					}
					else if (se.Message.IndexOf("CK_map_tblUserToUserGroup_ViewOperateOnly") != -1)
					{
						throw new MigrationDatabaseDAException("Group Assignment would add View Operate Only to Administrator");
					}
					else if (se.Message.IndexOf("CK_map_tblGroupToRight_ViewOperateOnly") != -1)
					{
						throw new MigrationDatabaseDAException("Right Assignment would add View Operate Only to Administrator");
					}

					else if (se.Message.IndexOf("Uniqueness", StringComparison.Ordinal) != -1)
					{
						throw new MigrationDatabaseDAException("Operation would result in duplicate identifers.");
					}
				}
				else if (se.Number == SQLServerUniqueConstraintViolationErrorCode || se.Number == SQLServerUniqueIndexViolationErrorCode)
				{
					// If a unique constraint or index is violated, return a user-friendly error message
					throw new MigrationDatabaseDAException(UniqueConstraintViolationErrorMessage);
				}

				if (se.Number == SQLServerStatementTerminatedMaximumRecursion)
				{
					throw new MigrationDatabaseDAException(StatementTerminatedMaximumRecursionErrorMessage);
				}

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new MigrationDatabaseDAException(OperationTimedOut);
				}
				else
				{
					throw new MigrationDatabaseDAException();
				}
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in ExecuteQuery:", e, command);
				throw new MigrationDatabaseDAException();
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
		/// <exception cref="MigrationDatabaseDAException">
		/// Throws a <see cref="MigrationDatabaseDAException"/> exception if a standard Exception or a non-user defined <see cref="SqlException"/> was encountered.
		/// </exception>
		/// <remarks>
		/// More specific <see cref="MigrationDatabaseDAException"/> exceptions are thrown for the following cases: 
		/// A delete query failed due to a foreign key constraint, 
		/// A query exhausts the maximum recursion depth,
		/// A unique constraint violation has taken place
		/// </remarks>
		public int ExecuteQueryWithoutSessionContext(SqlCommand command)
		{
			var connectionBuilder = new SqlConnectionStringBuilder(ConnectionString);
			var connection = new SqlConnection(connectionBuilder.ConnectionString);

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
					throw new MigrationDatabaseDAException("Entity is being referenced and cannot be deleted.");
				}
				else if (se.Number == SQLServerUniqueConstraintViolationErrorCode || se.Number == SQLServerUniqueIndexViolationErrorCode)
				{
					// If a unique constraint or index is violated, return a user-friendly error message
					throw new MigrationDatabaseDAException(UniqueConstraintViolationErrorMessage);
				}

				if (se.Number == SQLServerStatementTerminatedMaximumRecursion)
				{
					throw new MigrationDatabaseDAException(StatementTerminatedMaximumRecursionErrorMessage);
				}
				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new MigrationDatabaseDAException(OperationTimedOut);
				}
				else
				{
					throw new MigrationDatabaseDAException();
				}
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in ExecuteQuery:", e, command);
				throw new MigrationDatabaseDAException();
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

		public void WriteEntry(string message, EventLogEntryType entryType)
		{
			const int MAX_EVENTLOG_MESSAGE_LENGTH = 31500;

			try
			{
				using (EventLog eventLog = new EventLog("Application", ".", "FuelsManager"))
				{
					if (message.Length <= MAX_EVENTLOG_MESSAGE_LENGTH)
					{
						eventLog.WriteEntry(message, entryType);
					}
					else
					{
						// If the incoming message exceeds the configured max message size, break the
						// message down into smaller messages that can be written.
						ArrayList messageList = SplitLargeMessage(message, MAX_EVENTLOG_MESSAGE_LENGTH, true);

						foreach (string currentMessage in messageList)
						{
							eventLog.WriteEntry(currentMessage, entryType);
						}
					}
				}
			}
			catch (Exception error)
			{
				MyTrace(error.Message, EventLogEntryType.Error);
			}
		}

		private void MyTrace(string message, EventLogEntryType entryType)
		{
			switch (entryType)
			{
				case EventLogEntryType.Error:
					Trace.TraceError(message);
					break;
				case EventLogEntryType.Warning:
					Trace.TraceWarning(message);
					break;
				default:
					Trace.TraceInformation(message);
					break;

			}
		}

		public static ArrayList SplitLargeMessage(string message, int maximumMessageSize, bool addMessageSeparator)
		{
			string MESSAGE_SEPARATOR_FORMAT = @"--- Message {0} of {1} ---\r\n{2}";

			// If requested, make room to insert a divider string.  Simply taking the length of the format string 
			// gives us room to handle 000 to 999, no need to get fancy.
			int maxAdjustedMessageSize = (addMessageSeparator) ? (maximumMessageSize - MESSAGE_SEPARATOR_FORMAT.Length) : maximumMessageSize;

			int offset = 0;
			int currentMessageCount = 1;
			int totalMessageCount = ((message.Length >= maxAdjustedMessageSize) ? (message.Length / maxAdjustedMessageSize) + 1 : 1);

			ArrayList messageList = new ArrayList();

			// If the message fits within a single line then don't attempt to break the message apart.
			while (currentMessageCount < totalMessageCount)
			{
				messageList.Add(string.Format(
					MESSAGE_SEPARATOR_FORMAT,
					currentMessageCount,
					totalMessageCount,
					message.Substring(offset, maxAdjustedMessageSize)));

				currentMessageCount++;
				offset += maxAdjustedMessageSize;
			}

			// Add the last portion of the message (could be the entire message).
			messageList.Add(string.Format(
					MESSAGE_SEPARATOR_FORMAT,
					currentMessageCount,
					totalMessageCount,
					message.Substring(offset)));

			return (messageList);
		}
	}
}
