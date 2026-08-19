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
	using System.Globalization;
	using System.Runtime.Serialization;
	using System.Text;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.ServiceClasses;

	/// <summary>
	/// The ConsolidatedDAClass class is responsible for all database access for FuelsManager.
	/// </summary>
	public partial class ConsolidatedDAClass
	{
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
		/// <param name="conn">
		/// An instance of a <see cref="SqlConnection"/> object that contains an existing connection to use.  This method SHOULD NEVER attempt to close this connection since.
		/// </param>
		/// <returns>Dataset containing the result of the SQL query</returns>
		/// <remarks>
		///   Follows FuelsManager Defense 6.0 SP4 methodology if DESC key is attached.
		///   Database Password is a mangling of the user id and has no relation the 
		///   users application Password.
		/// </remarks>
		public DataSet GetDataSet(SqlCommand command, SecurityClass security, SqlTransaction trans)
		{
			CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			var resultDataSet = new DataSet();

			try
			{
				int nRetryCount = 1;

				do
				{
					try
					{
						command.Connection = trans.Connection;
						command.Transaction = trans;
						var expirationTime = DateTime.Now.AddSeconds(command.CommandTimeout);

						using (var reader = command.ExecuteReader())
						{
							string tableNameSuffix = string.Empty;
							int tableNumber = 0;

							do
							{
								var schema = reader.GetSchemaTable();

								if (schema != null && schema.Rows.Count > 0)
								{
									var table = new DataTable();
									table.TableName = "TableName" + tableNameSuffix;
									tableNumber++;
									tableNameSuffix = tableNumber.ToString(CultureInfo.InvariantCulture);
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
		/// <param name="conn">
		/// An instance of a <see cref="SqlConnection"/> object that contains an existing connection to use.  This method SHOULD NEVER attempt to close this connection since.
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
		public int ExecuteQuery(SecurityClass security, SqlCommand command, SqlTransaction trans)
		{
			this.CheckVersion();

			if (SqlScrubber.IsSqlOk(command) == false)
			{
				WriteEventErrorMessage("Illegal sql string passed in:\n" + command.CommandText);
				throw new ConsolidatedDAException();
			}

			// Always associate the execution of the query with the current session so that audit and change tracking
			// triggers can identify the execution context.

			if (trans != null && trans.Connection.State != ConnectionState.Closed)
			{
				return this.ExecuteQueryWithoutSessionContext(security, command, trans);
			}
			else
			{
				return this.ExecuteQueryWithinSessionContext(security, command);
			}
			
		}

		#region Public ExecuteQuery methods execute the commands within or outside the context of the current Session

		/// <summary>
		/// Executes a query without associating it with the context of a FuelsManager Session.
		/// </summary>
		/// <param name="security">
		/// An instance of the security context that contains a reference to any session token.
		/// </param>
		/// <param name="command">
		/// An instance of a <see cref="SqlCommand"/> object that contains the sql statement to execute.
		/// </param>
		/// <param name="conn">
		/// An instance of a <see cref="SqlConnection"/> object that contains an existing connection to use.  This method SHOULD NEVER attempt to close this connection since.
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
		public int ExecuteQueryWithoutSessionContext(SecurityClass security, SqlCommand command, SqlTransaction trans)
		{
			try
			{
				command.Connection = trans.Connection;
				command.Transaction = trans;

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

				if (se.Number == 547 && se.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") != -1)
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
					throw new ConsolidatedDAException();
				}
			}
			catch (Exception e)
			{
				LogDatabaseError("Exception in ExecuteQuery:", e, command);
				throw new ConsolidatedDAException();
			}
			finally
			{
			}
		}

		#endregion Public ExecuteQuery methods execute the commands within or outside the context of the current Session
	}
}
