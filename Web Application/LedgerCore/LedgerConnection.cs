namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Globalization;
	using System.Text;

	public class LedgerConnection
	{
		#region Private data members
		private const string OperationTimedOut = "Operation Timed Out";
		private const int SQLServerCommandTimeoutErrorCode = -2;
		private readonly LRLedgerProcessor.LedgerConnectionTypes connectionType;	
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Ledger Connection class.
		/// </summary>
		public LedgerConnection(LRLedgerProcessor.LedgerConnectionTypes connType)
		{
			this.connectionType = connType;
		}
		#endregion

		#region Properties

		public string ConnectionString
		{
			get
			{
				string connectionString = string.Empty;

				switch (this.connectionType)
				{
					case LRLedgerProcessor.LedgerConnectionTypes.ClrConnection:
						connectionString = "context connection=true";
						break;
					case LRLedgerProcessor.LedgerConnectionTypes.NonClrConnection:
						connectionString = ConfigurationManager.AppSettings["ConnectionString"];
						break;
				}

				if (string.IsNullOrEmpty(connectionString))
				{
					throw new ArgumentNullException("Connection string not configured in configuration file.");
				}

				return connectionString;
			}
		}
		#endregion

		/// <summary>
		/// Updated Get method to accept the user id so that correct
		/// audit log info is captured. 
		/// </summary>
		/// <param name="command">SQL command object containing select query to run</param>
		/// <returns>Dataset containing the result of the SQL query</returns>
		/// <remarks>
		///   Follows FuelsManager Defense 6.0 SP4 methodology if DESC key is attached.
		///   Database Password is a mangling of the user id and has no relation the 
		///   users application Password.
		/// </remarks>
		public DataSet GetDataSet(SqlCommand command)
		{
			if (CommandScrubber.IsSqlOk(command) == false)
			{
				string errMsg = "Illegal sql string passed in:\n" + command.CommandText;
				this.WriteEventErrorMessage(errMsg);
				throw new Exception(errMsg);
			}

			var resultDataSet = new DataSet();
			SqlConnection connection = null;

			try
			{
				int nRetryCount = 1;

				do
				{
					try
					{
						connection = new SqlConnection(this.ConnectionString);
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
									tableNameSuffix = tableNumber.ToString(CultureInfo.InvariantCulture);
									resultDataSet.Tables.Add(table);

									foreach (DataRow schemaRow in schema.Rows)
									{
										var column = new DataColumn
										{
											ColumnName = schemaRow["ColumnName"] as string,
											DataType = schemaRow["DataType"] as Type,
											AllowDBNull = (bool) schemaRow["AllowDBNull"]
										};

										if (column.DataType == typeof(string))
										{
											column.MaxLength = (int) schemaRow["ColumnSize"];
										}

										string columnNameSuffix = string.Empty;
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
										((IDataRecord) reader).GetValues(objects);
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

				this.LogDatabaseError("Exception in GetDataSet:", se, command);

				if (se.Number == SQLServerCommandTimeoutErrorCode)
				{
					throw new Exception(OperationTimedOut);
				}

				throw new Exception();
			}
			catch (Exception e)
			{
				this.LogDatabaseError("Exception in GetDataSet:\n", e, command);

				if (e.Message == OperationTimedOut)
				{
					throw new Exception(e.Message);
				}

			    throw;
			}

			return resultDataSet;
		}

		/// <summary>
		/// This function logs an error message with standard information (including parameters) 
		/// formatted to the event log.
		/// </summary>
		/// <param name="headerText">The header text for the logged error message.</param>
		/// <param name="exception">The exception to be logged.</param>
		/// <param name="command">The SqlCommand object in use that encountered the error.</param>
		private void LogDatabaseError(string headerText, Exception exception, SqlCommand command)
		{
			string parameterMessage;

			if (command.Parameters.Count > 0)
			{
				var sb = new StringBuilder();
				sb.Append("Parameters\n===============\n");
				foreach (SqlParameter parameter in command.Parameters)
				{
					var value = parameter.Value ?? "<value null>";
					sb.Append(string.Format("{0} = \"{1}\"\n", parameter.ParameterName, value));
				}

				parameterMessage = sb.ToString();
			}
			else
			{
				parameterMessage = "No parameters on command object.";
			}

			string messageText = string.Format("{0}\n{1}\n{2}\n\nSQL Command Text:\n{3}\n\n{4}",
												headerText,
												exception.Message,
												exception.StackTrace,
												command.CommandText,
												parameterMessage);

			this.WriteEventErrorMessage(messageText);
			Trace.WriteLine(messageText, headerText);
		}

		/// <summary>
		/// This function logs the messageText to the event log.
		/// </summary>
		/// <param name="messageText"></param>
		private void WriteEventErrorMessage(string messageText)
		{
			const int MaxEventlogMessageLength = 31500;

			try
			{
				using (EventLog eventLog = new EventLog("Application", ".", "FuelsManager"))
				{
					if (messageText.Length <= MaxEventlogMessageLength)
					{
						eventLog.WriteEntry(messageText, EventLogEntryType.Error);
					}
					else
					{
						// If the incoming message exceeds the configured max message size, break the
						// message down into smaller messages that can be written.
						ArrayList messageList = this.SplitLargeMessage(messageText, MaxEventlogMessageLength, true);

						foreach (string currentMessage in messageList)
						{
							eventLog.WriteEntry(currentMessage, EventLogEntryType.Error);
						}
					}
				}
			}
			catch (Exception error)
			{
				//MyTrace(error.Message, FMEventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Splits the passed in <c>message</c> into an array of smaller messages.
		/// <para>If the size of the original message exceeds the specified <c>maximumMessageSize</c>, the method will split the message into smaller messages.  
		/// However if the original message doesn't exceed the <c>maximumMessageSize</c>, the original message will be returned as a single array element.</para>
		/// </summary>
		/// <param name="message">The original message to split.</param>
		/// <param name="maximumMessageSize">The maximum length for a single message.  Ie: The EventLog can only accept 32677 bytes per entry.</param>
		/// <param name="addMessageSeparator">Prefix each smaller message with seperator text.</param>
		/// <returns>Array of smaller messages that can be iterated and processed individually.</returns>
		/// <remarks>The caller can request that a message separator be added to each smaller message.  
		/// A separator such as --- Message 1 of 5 --- is added to the top of each message.
		/// </remarks>
		private ArrayList SplitLargeMessage(string message, int maximumMessageSize, bool addMessageSeparator)
		{
			const string MessageSeparatorFormat = @"--- Message {0} of {1} ---\r\n{2}";

			// If requested, make room to insert a divider string.  Simply taking the length of the format string 
			// gives us room to handle 000 to 999, no need to get fancy.
			int maxAdjustedMessageSize = (addMessageSeparator) ? (maximumMessageSize - MessageSeparatorFormat.Length) : maximumMessageSize;

			int offset = 0;
			int currentMessageCount = 1;
			int totalMessageCount = ((message.Length >= maxAdjustedMessageSize) ? (message.Length / maxAdjustedMessageSize) + 1 : 1);

			ArrayList messageList = new ArrayList();

			// If the message fits within a single line then don't attempt to break the message apart.
			while (currentMessageCount < totalMessageCount)
			{
				messageList.Add(string.Format(
					MessageSeparatorFormat,
					currentMessageCount,
					totalMessageCount,
					message.Substring(offset, maxAdjustedMessageSize)));

				currentMessageCount++;
				offset += maxAdjustedMessageSize;
			}

			// Add the last portion of the message (could be the entire message).
			messageList.Add(string.Format(
					MessageSeparatorFormat,
					currentMessageCount,
					totalMessageCount,
					message.Substring(offset)));

			return (messageList);
		}
	}
}
