// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportRequests.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportRequests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Definition of the ExportRequests service class.  Provides a database interface for
	/// the ExportRequestClass type and the custom aviation export interfaces.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ExportRequests : IExportRequests
	{
		/// <summary>
		/// The ConsolidatedDAClass object provides database access
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportRequests"/> class. 
		/// </summary>
		public ExportRequests()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		/// <summary>
		/// Gets a list of ExportRequestClass objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of ExportRequestClass objects</returns>
		public List<ExportRequestClass> GetRequests(SecurityClass security)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			using (var cmd = new SqlCommand())
			{
				ExportRequestClass.PrepareSelectAllSqlCommand(cmd);
				DataTable table = this.consolidatedDa.GetDataTable(cmd, security);

				var requests = new List<ExportRequestClass>();
				foreach (DataRow row in table.Rows)
				{
					var newRequest = new ExportRequestClass();
					newRequest.Load(row);
					requests.Add(newRequest);
				}

				return requests;
			}
		}

		/// <summary>
		/// Gets a table of in-memory data from the database.  Executes the
		/// specified SQL command and returns the resultant DataTable.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The DataTable containing the results of the specified SQL command</returns>
		public DataTable GetDataTable(SecurityClass security, SerializableSqlCommand cmd)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand(cmd.CmdText))
			{
				sqlCommand.CommandType = cmd.CmdType;

				foreach (var param in cmd.Parameters)
				{
					var sqlParameter = new SqlParameter(param.Name, param.Value);
					sqlCommand.Parameters.Add(sqlParameter);
				}

				return this.consolidatedDa.GetDataTable(sqlCommand, security);
			}
		}

		/// <summary>
		/// Executes the specified SQL command and returns the number of rows affected.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The number of rows affected</returns>
		public int ExecuteQuery(SecurityClass security, SerializableSqlCommand cmd)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand(cmd.CmdText))
			{
				sqlCommand.CommandType = cmd.CmdType;
				foreach (var param in cmd.Parameters)
				{
					var sqlParameter = new SqlParameter(param.Name, param.Value);
					sqlCommand.Parameters.Add(sqlParameter);
				}

				return this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// Executes the specified SQL command and returns the first column of the first row
		/// in the result set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The first column of the first row of the results of the specified SQL command</returns>
		public object ExecuteScalar(SecurityClass security, SerializableSqlCommand cmd)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand(cmd.CmdText))
			{
				sqlCommand.CommandType = cmd.CmdType;
				foreach (var param in cmd.Parameters)
				{
					var sqlParameter = new SqlParameter(param.Name, param.Value);
					sqlCommand.Parameters.Add(sqlParameter);
				}

				return this.consolidatedDa.ExecuteScalar(sqlCommand, security);
			}
		}

		/// <summary>
		/// Adds an ExportRequestClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to add to the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, ExportRequestClass exportRequest)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			ExportRequestClass existingRequest = this.GetRequestById(security, exportRequest.RequestId);

			if (existingRequest != null && existingRequest.IdentityGuid != Guid.Empty)
			{
				throw new Exception("An export request with the provided request name already exists.");
			}

			exportRequest.CreatedDate = DateTimeOffset.Now;
			exportRequest.CreatedBy = security.UserID;
			exportRequest.UpdatedDate = exportRequest.CreatedDate;
			exportRequest.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				exportRequest.PrepareInsertSqlCommand(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		///  Modifies an existing ExportRequestClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to modify in the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, ExportRequestClass exportRequest)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			ExportRequestClass existingRequest = this.GetRequestById(security, exportRequest.RequestId);

			if (existingRequest != null && existingRequest.IdentityGuid != Guid.Empty && existingRequest.IdentityGuid != exportRequest.IdentityGuid)
			{
				throw new Exception("An export request with the provided request name already exists.");
			}

			if (existingRequest == null || existingRequest.IdentityGuid == Guid.Empty)
			{
				throw new Exception("The export request to modify was not found");
			}

			exportRequest.UpdatedDate = DateTimeOffset.Now;
			exportRequest.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				exportRequest.PrepareUpdateSqlCommand(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Deletes an existing ExportRequestClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">Identifies the object to delete in the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Delete(SecurityClass security, Guid identityGuid)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			ExportRequestClass exportRequest = new ExportRequestClass { IdentityGuid = identityGuid };
			using (SqlCommand cmd = new SqlCommand())
			{
				exportRequest.PreparePurgeSingleSqlCommand(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the ExportRequestClass object</param>
		/// <returns>The specified ExportRequestClass object</returns>
		public ExportRequestClass GetRequestById(SecurityClass security, string id)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				var request = new ExportRequestClass();
				ExportRequestClass.PrepareGetRequestByIDSqlCommand(cmd, id);
				DataTable table = this.consolidatedDa.GetDataTable(cmd, security);
				if (table.Rows.Count == 0)
				{
					return null;
				}

				request.Load(table.Rows[0]);
				return request;
			}
		}

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the Identity Guid (ExportRequestGuid)
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">The identity guid identifying the ExportRequestClass record</param>
		/// <returns>The specified ExportRequestClass record</returns>
		public ExportRequestClass Get(SecurityClass security, Guid identityGuid)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_AVIATION_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				var request = new ExportRequestClass();
				ExportRequestClass.SelectSQL(cmd, identityGuid);
				DataTable table = this.consolidatedDa.GetDataTable(cmd, security);
				if (table.Rows.Count == 0)
				{
					return null;
				}

				request.Load(table.Rows[0]);
				return request;
			}
		}
	}
}
