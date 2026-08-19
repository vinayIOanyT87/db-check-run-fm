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

	/// <summary>
	/// Summary description for TankQualityTagLogsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TankQualityTagLogsClass : ITankQualityTagLogs, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TankQualityTagLogsClass()
		{
		}

		public DataSet GetDataSet(SecurityClass security,
									bool bHistorical,
									string sDateType,
									DateTimeOffset dateStart,
									DateTimeOffset dateEnd,
									string state)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS)
			    && !security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
			{
				throw new FMInsufficientRightsException();
			}

			var tankQualityTagLog = new TankQualityTagLogClass();

			using (var cmd = new SqlCommand())
			{
				tankQualityTagLog.EnumerateSQL(cmd, bHistorical, sDateType, dateStart, dateEnd, state);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				return set;
			}
		}

		// Write the sent collection "rowset" to the database.

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TankQualityTagLogClass tankQualityTagLog)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tankQualityTagLog == null)
			{
				throw new ArgumentNullException("tankQualityTagLog");
			}

			if (!security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS) && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			if (tankQualityTagLog.Memo == "")
			{
				throw (new Exception("Memo Required"));
			}

			tankQualityTagLog.SiteGuid = security.SiteGuid;
			tankQualityTagLog.CreatedDate = DateTimeOffset.Now;
			tankQualityTagLog.CreatedBy = security.UserID;
			tankQualityTagLog.UpdatedDate = tankQualityTagLog.CreatedDate;
			tankQualityTagLog.UpdatedBy = security.UserID;
			tankQualityTagLog.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				tankQualityTagLog.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
				return tankQualityTagLog.IdentityGuid;
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TankQualityTagLogClass tankQualityTagLog)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tankQualityTagLog == null)
			{
				throw new ArgumentNullException("tankQualityTagLog");
			}

			if (tankQualityTagLog.Memo == "")
			{
				throw (new Exception("Memo Required"));
			}

			TankQualityTagLogClass tankQualityTagLogOld = Get(security, tankQualityTagLog.IdentityGuid);

			if (tankQualityTagLogOld.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TankQualityTagLog Not Found"));
			}

			tankQualityTagLog.UpdatedDate = DateTimeOffset.Now;
			tankQualityTagLog.UpdatedBy = security.UserID;
			tankQualityTagLog.RemovedDate = DateTimeOffset.Now;
			tankQualityTagLog.RemovedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				tankQualityTagLog.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#region Interface implementations
		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
		}
		#endregion

		public TankQualityTagLogClass GetPreviousTagNumber(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) &&
				!security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
			{
				throw new FMInsufficientRightsException();
			}

			var log = new TankQualityTagLogClass();

			using (var cmd = new SqlCommand())
			{
				log.PreviousTagNumberSQL(cmd, security);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				if (set != null
					&& set.Tables.Count > 0
					&& set.Tables[0].Rows.Count > 0)
				{
					DataRow row = set.Tables[0].Rows[0];

					log.TagNumber = DataObject.getValue<int>(row["TagNumber"], 0);
					log.TaggedDate = DataObject.getValue<DateTimeOffset>(row["TaggedDate"], DateTimeOffset.Now);
				}

				return log;
			}
		}

		public TankQualityTagLogClass GetByTagNumber(SecurityClass security, int tagNumber)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) &&
				!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) &&
				!security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
			{
				throw new FMInsufficientRightsException();
			}

			var tankQualityTagLog = new TankQualityTagLogClass { TagNumber = tagNumber };

			using (var cmd = new SqlCommand())
			{
				tankQualityTagLog.GetByTagNumberSQL(cmd, security);
				tankQualityTagLog.Load(ConsolidatedDA.GetDataSet(cmd, security));

				if (tankQualityTagLog.IdentityGuid == Guid.Empty)
				{
					return null;
				}

				return tankQualityTagLog;
			}
		}

		public TankQualityTagLogClass Get(SecurityClass security, Guid tankQualityTagLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
			{
				throw new FMInsufficientRightsException();
			}

			var tankQualityTagLog = new TankQualityTagLogClass { IdentityGuid = tankQualityTagLogGuid };

			using (var cmd = new SqlCommand())
			{
				tankQualityTagLog.GetSQL(cmd, security);
				tankQualityTagLog.Load(ConsolidatedDA.GetDataSet(cmd, security));

				return tankQualityTagLog;
			}
		}

		public TankQualityTagLogClass GetMostRecentByTankID(SecurityClass security, string equipmentId)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS)
			    && !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) 
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var tankQualityTagLog = new TankQualityTagLogClass { TankID = equipmentId };

			using (var cmd = new SqlCommand())
			{
				tankQualityTagLog.GetMostRecentByTankIDSQL(cmd, security);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				tankQualityTagLog.Load(set);

				return tankQualityTagLog;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid tankQualityTagLogGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			TankQualityTagLogClass tankQualityTagLog = Get(security, tankQualityTagLogGuid);
			
			if (tankQualityTagLog.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Tank Quality Tag Log Not Found"));
			}

			using (var cmd = new SqlCommand())
			{
				tankQualityTagLog.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}

}
