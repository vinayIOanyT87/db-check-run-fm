namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;


	using FMBusinessObjects.Exceptions;


	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TankMaintenanceLogsClass : ITankMaintenanceLogs, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public DataSet GetDataSet(SecurityClass security,
									bool bHistorical,
									string sDateType,
									DateTimeOffset dateStart,
									DateTimeOffset dateEnd,
									Guid assetGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) 
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
			    && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			var tankMaintenanceLog = new TankMaintenanceLogClass();

			using (var cmd = new SqlCommand())
			{
				tankMaintenanceLog.EnumerateSQL(cmd, security, bHistorical, sDateType, dateStart, dateEnd, assetGuid);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				return set;
			}
		}

		public TankMaintenanceLogClass GetByTankGuid(SecurityClass security, Guid tankGuid)
		{
			DataSet ds = this.GetDataSet(security, false, string.Empty, DateTimeOffset.Now, DateTimeOffset.Now, tankGuid);
			DataTable table = ds.Tables[0];

			if (table.Rows.Count > 0)
			{
				var tankMaintenanceLog = new TankMaintenanceLogClass();
				DataRow row = table.Rows[0];
				tankMaintenanceLog.IdentityGuid = DataObject.getValue<Guid>(row["TankMaintenanceLogGuid"], Guid.Empty);

				return this.Get(security, tankMaintenanceLog.IdentityGuid);
			}

			return null;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TankMaintenanceLogClass tankMaintenanceLog)
		{

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (tankMaintenanceLog == null)
			{
				throw new ArgumentNullException(nameof(tankMaintenanceLog));
			}

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(tankMaintenanceLog);

			tankMaintenanceLog.SiteGuid = security.SiteGuid;
			tankMaintenanceLog.CreatedDate = DateTimeOffset.Now;
			tankMaintenanceLog.CreatedBy = security.UserID;
			tankMaintenanceLog.UpdatedDate = tankMaintenanceLog.CreatedDate;
			tankMaintenanceLog.UpdatedBy = security.UserID;
			tankMaintenanceLog.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				tankMaintenanceLog.InsertSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);

				return tankMaintenanceLog.IdentityGuid;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TankMaintenanceLogClass tankMaintenanceLog)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (tankMaintenanceLog == null)
			{
				throw new ArgumentNullException(nameof(tankMaintenanceLog));
			}

			TankMaintenanceLogClass tankMaintenanceLogOld = this.Get(security, tankMaintenanceLog.IdentityGuid);

			if (tankMaintenanceLogOld.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TankMaintenanceLog Not Found"));
			}

			this.Validate(tankMaintenanceLog);

			tankMaintenanceLog.UpdatedDate = DateTimeOffset.Now;
			tankMaintenanceLog.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				tankMaintenanceLog.UpdateSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}


		// Returns how many hours it has been since the most recent maintenance log
		// record was added for the sent piece of Tank.
		public int GetHoursPassed(SecurityClass security, TankMaintenanceLogClass tankMaintenanceLog)
		{
			if (null == tankMaintenanceLog) return 0;

			using (var cmd = new SqlCommand())
			{
				tankMaintenanceLog.HoursPassed(cmd);
				DataSet ds = this.ConsolidatedDA.GetDataSet(cmd, security);

				var dtLatest = (DateTimeOffset)ds.Tables[0].Rows[0]["ChangeDate"];
				TimeSpan tsHoursPassed = DateTimeOffset.Now - dtLatest;

				if (tsHoursPassed.Hours < 0)
				{
					return 0;
				}

				return tsHoursPassed.Hours;
			}
		}

	    // ReSharper disable once UnusedParameter.Local
        // The checks are the validation
		private void Validate(TankMaintenanceLogClass tankMaintenanceLog)
		{
			if (string.IsNullOrEmpty(tankMaintenanceLog.TankID))
			{
				throw new Exception("Tank ID required.");
			}

			if (tankMaintenanceLog.InServiceFlag == 0 && tankMaintenanceLog.MaintenanceReasonGuid == Guid.Empty)
			{
				throw new Exception("Maintenance Reason required.");
			}
		}

		#region Interface implementations

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			//Debug.Assert(false, "Not implemented for this table.");
		}


		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			//Debug.Assert(false, "Not implemented for this table.");
		}


		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			//Debug.Assert(false, "Not implemented for this table.");
		}
		#endregion

		public TankMaintenanceLogClass Get(SecurityClass security, Guid tankMaintenanceLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
			    && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			var tankMaintenanceLog = new TankMaintenanceLogClass { IdentityGuid = tankMaintenanceLogGuid };

			using (var cmd = new SqlCommand())
			{
				tankMaintenanceLog.GetSQL(cmd, security);
				tankMaintenanceLog.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return tankMaintenanceLog;
		}

		/// <summary>
		/// Returns most recent Maintenance Logs of tanks not in service. 
		/// </summary>
		/// <param name="security"></param>
		/// <param name="maintenanceReasonGuid"></param>
		/// <returns></returns>

		public bool IsMaintenanceReasonUsed(SecurityClass security, Guid maintenanceReasonGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
			    && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			bool toRet = false;

			var tankMaintenanceLog = new TankMaintenanceLogClass();

			using (var cmd = new SqlCommand())
			{
				tankMaintenanceLog.MaintenanceReasonUsedCount(cmd, maintenanceReasonGuid);
				var recordCount = (int)this.ConsolidatedDA.ExecuteScalar(cmd, security);

				if (recordCount > 0)
				{
					toRet = true;
				}
			}

			return toRet;
		}

		/// <summary>
		/// Returns most recent Maintenance Logs of tanks not in service. 
		/// </summary>
		/// <param name="security"></param>
		/// <param name="maintenanceReasonGuid"></param>
		/// <returns></returns>

		public TankMaintenanceLogCollectionClass EnumerateByMaintenanceReason(SecurityClass security, Guid maintenanceReasonGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}
			TankMaintenanceLogCollectionClass TankMaintenanceLogCollection = new TankMaintenanceLogCollectionClass();
			TankMaintenanceLogClass TankMaintenanceLog = new TankMaintenanceLogClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				TankMaintenanceLog.EnumerateByMaintenanceReasonSQL(cmd, maintenanceReasonGuid);
				DataSet Set = ConsolidatedDA.GetDataSet(cmd, security);

				DataTable Table = Set.Tables[0];

				while (Table.Rows.Count != 0)
				{
					TankMaintenanceLog = new TankMaintenanceLogClass();
					TankMaintenanceLog.Load(Set);
					TankMaintenanceLogCollection.Add(TankMaintenanceLog);
					Table.Rows.RemoveAt(0);
				}

				return TankMaintenanceLogCollection;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid tankMaintenanceLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			TankMaintenanceLogClass tankMaintenanceLog = this.Get(security, tankMaintenanceLogGuid);

			if (tankMaintenanceLog.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Tank Maintenance Log Not Found"));
			}

			using (var cmd = new SqlCommand())
			{
				tankMaintenanceLog.PurgeSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}
