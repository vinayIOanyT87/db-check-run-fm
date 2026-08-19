// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerLogToTransactionMap.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Business service class responsible for saving instances of ControllerLogToTransactionMap.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.ServiceModel;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// This class provides access to web pages for manipulating instances of ControllerLogToTransactionMap.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ControllerLogToTransactionMap : IControllerLogToTransactionMap
	{
		#region Protected Data members
		internal ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
		#endregion

		#region Constructors

		public ControllerLogToTransactionMap()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		#endregion

		///<summary>
		/// Returns a collection of ControllersLogToTransactionMapClass instances with the matching transactionGuid.
		///</summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="transactionGuid">A Transaction Guid</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public ControllersLogToTransactionCollectionClass EnumerateByTransactionGuid(SecurityClass security, Guid transactionGuid)
		{
			ControllersLogToTransactionCollectionClass retVal = null;

			if (security == null)
				throw new ArgumentNullException("Security");

			if (transactionGuid == null)
				throw new ArgumentNullException("TransactionGuid");

			ControllersLogToTransactionMapClass controllersLogToTransactionMap =
				new ControllersLogToTransactionMapClass();
			controllersLogToTransactionMap.TransactionGuid = transactionGuid;
			DataSet set = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				controllersLogToTransactionMap.SelectByTransactionGuidSQL(cmd);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}
			retVal = new ControllersLogToTransactionCollectionClass();
			DataTable Table = set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				controllersLogToTransactionMap = new ControllersLogToTransactionMapClass();
				controllersLogToTransactionMap.Load(set);
				retVal.Add(controllersLogToTransactionMap);
				Table.Rows.RemoveAt(0);
			}

			return retVal;
		}

		///<summary>
		/// Returns a matching ControllersLogToTransactionMapClass instance.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="transactionGuid">A Transaction Guid</param>
		/// <returns>A ControllersLogToTransactionMapClass instance.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public ControllersLogToTransactionMapClass GetByTransactionGuid(SecurityClass security, Guid transactionGuid)
		{
			ControllersLogToTransactionMapClass retVal = null;

			if (security == null)
				throw new ArgumentNullException("Security");

			if (transactionGuid == null)
				throw new ArgumentNullException("TransactionGuid");

			ControllersLogToTransactionMapClass controllersLogToTransactionMap =
				new ControllersLogToTransactionMapClass();
			controllersLogToTransactionMap.TransactionGuid = transactionGuid;
			DataSet set = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				controllersLogToTransactionMap.SelectByTransactionGuidSQL(cmd);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}
			if (set != null && set.Tables != null)
			{
				DataTable Table = set.Tables[0];
				if (Table.Rows.Count != 0)
				{
					controllersLogToTransactionMap = new ControllersLogToTransactionMapClass();
					controllersLogToTransactionMap.Load(set);
					retVal = controllersLogToTransactionMap;
				}
			}

			return retVal;
		}

		///<summary>
		/// Returns a matching ControllersLogToTransactionMapClass instance.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param param name="controllersLogToTransactionGuid">The key field for this object.</param>
		/// <returns>A ControllersLogToTransactionMapClass instance.</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		public ControllersLogToTransactionMapClass GetById(SecurityClass security, Guid controllersLogToTransactionGuid)
		{
			ControllersLogToTransactionMapClass retVal = null;

			if (security == null)
				throw new ArgumentNullException("Security");

			if (controllersLogToTransactionGuid == null)
				throw new ArgumentNullException("ControllersLogToTransactionGuid");

			ControllersLogToTransactionMapClass controllersLogToTransactionMap =
				new ControllersLogToTransactionMapClass();
			controllersLogToTransactionMap.ControllersLogToTransactionGuid = controllersLogToTransactionGuid;
			DataSet set = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				controllersLogToTransactionMap.SelectById(cmd);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}
			if (set != null && set.Tables != null)
			{
				DataTable Table = set.Tables[0];
				if (Table.Rows.Count != 0)
				{
					controllersLogToTransactionMap = new ControllersLogToTransactionMapClass();
					controllersLogToTransactionMap.Load(set);
					retVal = controllersLogToTransactionMap;
				}
			}

			return retVal;
		}

		/// <summary>
		/// This method will add a ControllersLogToTransactionMapClass instance to map.tblControllersLogToTransaction
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="ControllersLogToTransactionMap">A ControllersLogToTransactionMapClass instance.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ControllersLogToTransactionMapClass controllersLogToTransactionMap)
		{
			Guid retVal = Guid.Empty;

			if (security == null)
				throw new ArgumentNullException("Security");

			if (controllersLogToTransactionMap == null)
				throw new ArgumentNullException("ControllersLogToTransactionMap");
			
			if (security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
			{
				throw new ApplicationException("Access denied.");
			}
			controllersLogToTransactionMap.SiteGuid = security.SiteGuid;
		

			using (var cmd = new SqlCommand())
			{
				controllersLogToTransactionMap.IdentityGuid = Guid.NewGuid();
				controllersLogToTransactionMap.InsertSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
			controllersLogToTransactionMap = this.GetByTransactionGuid(security, controllersLogToTransactionMap.TransactionGuid);

			retVal = controllersLogToTransactionMap.ControllersLogToTransactionGuid;
			return retVal;
		}

		/// <summary>
		/// This method will modify a ControllersLogToTransactionMapClass instance in table map.tblControllersLogToTransaction
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="ControllersLogToTransactionMap">A ControllersLogToTransactionMapClass instance.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ControllersLogToTransactionMapClass controllersLogToTransactionMap)
		{
			Guid retVal = Guid.Empty;

			if (security == null)
				throw new ArgumentNullException("Security");

			if (controllersLogToTransactionMap == null)
				throw new ArgumentNullException("ControllersLogToTransactionMap");

			if (security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
			{
				throw new ApplicationException("Access denied.");
			}
			var oldControllerLogToTransactionClass = this.GetByTransactionGuid(security, controllersLogToTransactionMap.TransactionGuid);

			oldControllerLogToTransactionClass.UpdatedDate = DateTimeOffset.Now;
			oldControllerLogToTransactionClass.UpdatedBy = security.UserID;
			using (var cmd = new SqlCommand())
			{
				oldControllerLogToTransactionClass.UpdateSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method will purge a row in the map.tblControllersLogToTransaction
		/// </summary>
		/// <param name="security">A SecurityClass instance. </param>
		/// <param name="ControllersLogToTransactionMap">A ControllersLogToTransactionMapClass instance.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, ControllersLogToTransactionMapClass controllersLogToTransactionMapClass)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (controllersLogToTransactionMapClass == null)
				throw new ArgumentNullException("ControllersLogToTransactionMap");

			
			ControllerLogToTransactionMap ControllerLogToTransactionMap = new ServiceClasses.ControllerLogToTransactionMap();
			var result = ControllerLogToTransactionMap.GetById(security,
				controllersLogToTransactionMapClass.ControllersLogToTransactionGuid);
			if (result == null || result.ControllersLogToTransactionGuid == Guid.Empty)
			{
				throw new Exception("Controller Log Entry Not Found");
			}

			using (var cmd = new SqlCommand())
			{
				controllersLogToTransactionMapClass.Purge(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Returns a collection of  the ControllersLogToTransactionMapClass instances that meet 
		/// the criteria.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="bInTransaction">A bool representing if this method is wrapped in a transaction</param>
		/// <param name="StartDate">A Start Date</param>
		/// <param name="EndDate">The end date the ControllerLogClass record was stored in the database</param>
		/// <param name="Deleted">A bool indicating whether or not to retrieve logically deleted records</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public ControllersLogToTransactionCollectionClass EnumerateByStartStopDates(SecurityClass security, 
			DateTimeOffset startDate, DateTimeOffset endDate, bool deleted, Guid transactionGuid)
		{
			ControllersLogToTransactionCollectionClass retVal = null;

			if (security == null)
				throw new ArgumentNullException("Security");

			if (transactionGuid == null)
				throw new ArgumentNullException("TransactionGuid");

			ControllersLogToTransactionMapClass controllerLogToTransactionMap = new ControllersLogToTransactionMapClass();
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				controllerLogToTransactionMap.EnumerateByStartStopDatesSQL(cmd, security, startDate, endDate, deleted, transactionGuid);
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}
			retVal = new ControllersLogToTransactionCollectionClass();
			DataTable Table = set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				controllerLogToTransactionMap = new ControllersLogToTransactionMapClass();
				controllerLogToTransactionMap.Load(set);
				retVal.Add(controllerLogToTransactionMap);
				Table.Rows.RemoveAt(0);
			}
			return retVal;
		}



		
	}
}
