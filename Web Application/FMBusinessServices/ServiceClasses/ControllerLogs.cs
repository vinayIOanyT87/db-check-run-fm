// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerLogsClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Web page resonsible for managing Controller Log entries in the FuelsManager Website.
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
	using System.Transactions;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// This class is responsible for managing Controller Log entries within FuelsManager.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ControllerLogsClass : IControllerLogs
	{
		#region Attributes
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
		#endregion

		#region IControllerLogs implementation
		/// <summary>
		/// This methods Adds a ControllerLog to the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="controllerLog">A ControllerLogClass instance</param>
		/// <returns>A Guid</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ControllerLogClass controllerLog)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (controllerLog == null)
			{
				throw new ArgumentNullException("controllerLog");
			}

			if (!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			controllerLog.UpdatedDate = controllerLog.CreatedDate;
			controllerLog.UpdatedBy = security.UserID;

			controllerLog.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				controllerLog.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return controllerLog.IdentityGuid;
		}

		/// <summary>
		/// This methods adds ControllerLog and ControllersLogToTransaction records to the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="controllerLog">A ControllerLogClass instance</param>
		/// <param name="transactionGuid">A Transaction Id</param>
		/// <returns>The ControllerLog Guid</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public List<Guid> AddControllerLogAndMapRecord(SecurityClass security, ControllerLogClass controllerLog, Guid transactionGuid)
		{
			var retVal = new List<Guid>();

			if (transactionGuid == null || transactionGuid == Guid.Empty)
			{
				throw new ArgumentNullException("transactionGuid");
			}

			var newControllerLogGuid = this.Add(security, controllerLog);
			using (var scope = new TransactionScope())
			{
				if (newControllerLogGuid != Guid.Empty)
				{
					retVal.Add(newControllerLogGuid);

					var newControllerLog = this.Get(security, newControllerLogGuid);
					var controllerLogToTransactionMap = new ControllerLogToTransactionMap();
					var controllersLogToTransactionMapClass = new ControllersLogToTransactionMapClass
					                                          {
						                                          TransactionGuid = transactionGuid,
						                                          ControllersLogGuid = newControllerLog.IdentityGuid,
						                                          UpdatedBy = security.UserID,
						                                          UpdatedDate = DateTimeOffset.Now,
						                                          SiteGuid = security.SiteGuid
					                                          };

					var newControllersLogToTransactionGuid = controllerLogToTransactionMap.Add(security, controllersLogToTransactionMapClass);

					if (newControllersLogToTransactionGuid != Guid.Empty)
					{
						retVal.Add(newControllersLogToTransactionGuid);
						scope.Complete();
					}
				}
			}
			return retVal;
		}

		/// <summary>
		/// This method deletes a ControllerLog from the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteControllerLog(SecurityClass security, Guid controllerLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			ControllerLogClass controllerLog = Get(security, controllerLogGuid);
			if (controllerLog.IdentityGuid == Guid.Empty)
			{
				return;
			}

			controllerLog.Deleted = true;

			using (var cmd = new SqlCommand())
			{
				controllerLog.DeleteSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method enumerates the ControllerLogs by their identity key.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		/// <returns>A ControllerLogClass instance</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public ControllerLogClass EnumerateControllerLogByIdentityGuid(SecurityClass security, Guid controllerLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

			var controllerLog = new ControllerLogClass();

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				controllerLog.EnumerateByIdentityGuid(cmd, security, ContextUtil.IsInTransaction, controllerLogGuid, site);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count == 1)
			{
				controllerLog = new ControllerLogClass();
				controllerLog.Load(set);
				table.Rows.RemoveAt(0);
			}
			else
			{
				throw new ArgumentNullException("Invalid IdentityGuid");
			}

			return controllerLog;
		}

		/// <summary>
		/// This method enumerates the ControllerLogs by a start date and stop date
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="startDate">The start date the controller log was entered</param>
		/// <param name="endDate">The end date the controller log was entered</param>
		/// <param name="deleted">A bool to indicate whether or not to retrieve logically deleted Controller Log records.</param>
		/// <returns>A List of ControllerLogClass instances</returns>
		public List<ControllerLogClass> EnumerateByStartStopDateAndDeleted(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate, bool deleted)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (startDate == DateTimeOffset.MinValue || startDate == DateTimeOffset.MaxValue)
			{
				throw new ArgumentNullException("startDate");
			}

			if (endDate == DateTimeOffset.MinValue || endDate == DateTimeOffset.MaxValue)
			{
				throw new ArgumentNullException("endDate");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

			var controllerLogCollection = new List<ControllerLogClass>();
			var controllerLog = new ControllerLogClass();

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				controllerLog.EnumerateByStartStopDateAndDeletedSQL(cmd, security, startDate, endDate, deleted, site);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				controllerLog = new ControllerLogClass();
				controllerLog.Load(set);
				controllerLogCollection.Add(controllerLog);
				table.Rows.RemoveAt(0);
			}

			return controllerLogCollection;
		}

		/// <summary>
		/// This method enumerates the ControllerLogs by a start date and stop date
		/// </summary>s
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="startDate">The start date the controller log was entered</param>
		/// <param name="endDate">The end date the controller log was entered</param>
		/// <param name="deleted">A bool to indicate whether or not to retrieve logically deleted Controller Log records.</param>
		/// <param name="transactionGuid">A Transaction Guid</param>
		/// <returns>A List of ControllerLogClass instances</returns>
		public List<ControllerLogClass> EnumerateByStartStopTimeAndTransId(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate, bool deleted, Guid transactionGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (startDate == DateTimeOffset.MinValue || startDate == DateTimeOffset.MaxValue)
			{
				throw new ArgumentNullException("startDate");
			}

			if (endDate == DateTimeOffset.MinValue || endDate == DateTimeOffset.MaxValue)
			{
				throw new ArgumentNullException("endDate");
			}

			if (transactionGuid == Guid.Empty)
			{
				throw new ArgumentNullException("transactionGuid");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

			var controllerLogCollection = new List<ControllerLogClass>();
			var controllerLog = new ControllerLogClass();

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				controllerLog.EnumerateByStartStopDates(cmd, security, ContextUtil.IsInTransaction, startDate, endDate, deleted, transactionGuid, site);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				controllerLog = new ControllerLogClass();
				controllerLog.Load(set);
				controllerLogCollection.Add(controllerLog);
				table.Rows.RemoveAt(0);
			}

			return controllerLogCollection;
		}

		/// <summary>
		/// This method enumerates the ControllerLogs by a start date and stop date and is
		/// called by old Dispatch and is here for backward compatiblity.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="startDate">The start date the controller log was entered</param>
		/// <param name="endDate">The end date the controller log was entered</param>
		/// <param name="deleted">A bool to indicate whether or not to retrieve logically deleted Controller Log records.</param>
		/// <returns>A List of ControllerLogClass instances</returns>
		public List<ControllerLogClass> EnumerateByStartStopTime(SecurityClass security, DateTime startDate, DateTime endDate, bool deleted)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (startDate == DateTimeOffset.MinValue || startDate == DateTimeOffset.MaxValue)
			{
				throw new ArgumentNullException("startDate");
			}

			if (endDate == DateTimeOffset.MinValue || endDate == DateTimeOffset.MaxValue)
			{
				throw new ArgumentNullException("endDate");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

			var controllerLogCollection = new List<ControllerLogClass>();
			var controllerLog = new ControllerLogClass();

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				controllerLog.EnumerateByStartStopDateAndDeletedSQL(cmd, security, startDate, endDate, deleted, site);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				controllerLog = new ControllerLogClass();
				controllerLog.Load(set);
				controllerLogCollection.Add(controllerLog);
				table.Rows.RemoveAt(0);
			}

			return controllerLogCollection;
		}

		/// <summary>
		/// This method modifies an existing Controller Log instance in the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLog">A ControllerLogClass instance</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ControllerLogClass controllerLog)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (controllerLog == null)
			{
				throw new ArgumentNullException("controllerLog");
			}

			ControllerLogClass oldControllerLog = Get(security, controllerLog.IdentityGuid);
			if (oldControllerLog.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("ControllerLog Not Found"));
			}

			controllerLog.UpdatedDate = DateTimeOffset.Now;
			controllerLog.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				controllerLog.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This is a special case purge method that instead of deleting the Controller Log instances it flips the logical delete flag inversely.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid controllerLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			ControllerLogClass controllerLog = Get(security, controllerLogGuid);
			if (controllerLog.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				controllerLog.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, controllerLog);

		}

		/// <summary>
		/// This method performs the undelete functionality for a Controller Log instance
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UnDeleteControllerLog(SecurityClass security, Guid controllerLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			ControllerLogClass controllerLog = Get(security, controllerLogGuid);
			if (controllerLog.IdentityGuid == Guid.Empty)
			{
				return;
			}

			controllerLog.Deleted = false;

			using (var cmd = new SqlCommand())
			{
				controllerLog.DeleteSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// This method is used to retrieve a ControllerLogClass record from the database
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="identityGuid">A Guid</param>
		/// <returns>A ControllerLogClass instance</returns>
		public ControllerLogClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

			var controllerLog = new ControllerLogClass();
			controllerLog.IdentityGuid = identityGuid;

			if (identityGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					controllerLog.SelectSQL(cmd, security, site, ContextUtil.IsInTransaction);
					controllerLog.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return controllerLog;
		}

		#endregion

	}
}