using System;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.ServiceModel;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class EquipmentMaintenanceLogsClass : IDependency, IEquipmentMaintenanceLogs
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

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD) &&
				!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			DataDictionariesClass dataDictionaries = new DataDictionariesClass();
			EquipmentMaintenanceLogClass equipmentMaintenanceLog = new EquipmentMaintenanceLogClass();
			string sDateTypeBeforeDictionaryMap = sDateType;

			if (sDateType == dataDictionaries.Get(security.SiteGuid, "Estimated Return To Service"))
			{
				sDateTypeBeforeDictionaryMap = "Estimated Return To Service";
			}
			else if (sDateType == dataDictionaries.Get(security.SiteGuid, "QC Due Date"))
			{
				sDateTypeBeforeDictionaryMap = "QC Due Date";
			}

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.EnumerateSQL(cmd, security, bHistorical, sDateTypeBeforeDictionaryMap, dateStart, dateEnd, assetGuid);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return set;
		}

		public EquipmentMaintenanceLogClass GetByEquipmentGuid(SecurityClass security, Guid equipmentGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) &&
				!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
					)
			{
				throw new FMInsufficientRightsException();
			}

			DataSet ds = this.GetDataSet(security, false, "", DateTimeOffset.Now, DateTimeOffset.Now, equipmentGuid);
			DataTable table = ds.Tables[0];

			if (table.Rows.Count > 0)
			{
				DataRow row = table.Rows[0];
				Guid equipmentMaintenanceLogGuid = DataObject.getValue(row["MaintenanceLogGuid"], Guid.Empty);

				return this.Get(security, equipmentMaintenanceLogGuid);
			}

			return null;
		}

		// Write the sent collection "rowset" to the database.
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, EquipmentMaintenanceLogClass equipmentMaintenanceLog)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (equipmentMaintenanceLog == null)
				throw new ArgumentNullException(nameof(equipmentMaintenanceLog));

			if (!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD))
				throw new FMInsufficientRightsException();

			this.Validate(equipmentMaintenanceLog);

			equipmentMaintenanceLog.SiteGuid = security.SiteGuid;
			equipmentMaintenanceLog.CreatedDate = DateTimeOffset.Now;
			equipmentMaintenanceLog.CreatedBy = security.UserID;
			equipmentMaintenanceLog.UpdatedDate = equipmentMaintenanceLog.CreatedDate;
			equipmentMaintenanceLog.UpdatedBy = security.UserID;
			equipmentMaintenanceLog.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
			return equipmentMaintenanceLog.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, EquipmentMaintenanceLogClass equipmentMaintenanceLog)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));


			if (!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
				throw new FMInsufficientRightsException();

			if (equipmentMaintenanceLog == null)
				throw new ArgumentNullException(nameof(equipmentMaintenanceLog));

			this.Validate(equipmentMaintenanceLog);

			EquipmentMaintenanceLogClass equipmentMaintenanceLogOld = this.Get(security, equipmentMaintenanceLog.IdentityGuid);

			if (equipmentMaintenanceLogOld.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("EquipmentMaintenanceLog Not Found"));
			}

			equipmentMaintenanceLog.UpdatedDate = DateTimeOffset.Now;
			equipmentMaintenanceLog.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		// ReSharper disable once UnusedParameter.Local
		// The preconditions/tests flagged by ReSharper _are_ the validation
		private void Validate(EquipmentMaintenanceLogClass equipmentMaintenanceLog)
		{
			if (string.IsNullOrEmpty(equipmentMaintenanceLog.EquipmentID?.Trim()))
			{
				throw new Exception("Tank ID required.");
			}

			if (equipmentMaintenanceLog.InServiceFlag == 0 && equipmentMaintenanceLog.MaintenanceReasonGuid == Guid.Empty)
			{
				throw new Exception("Maintenance Reason required.");
			}
		}

		// Returns how many hours it has been since the most recent maintenance log
		// record was added for the sent piece of equipment.
		public int GetHoursPassed(SecurityClass security, EquipmentMaintenanceLogClass equipmentMaintenanceLog)
		{
			if (null == equipmentMaintenanceLog) return 0;

			DataSet ds;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.HoursPassed(cmd);
				ds = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DateTimeOffset dtLatest = DataObject.getValue(ds.Tables[0].Rows[0]["ChangeDate"], DateTimeOffset.Now);
			TimeSpan tsHoursPassed = DateTimeOffset.Now - dtLatest;
			return tsHoursPassed.Hours;
		}

		#region IDependency implementations
		void IDependency.Purge(SecurityClass security, BaseDataObject obj)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			var maintenanceReason = obj as MaintenanceReasonClass;
			if (maintenanceReason != null)
			{
				EquipmentMaintenanceLogCollectionClass enumerateMaintenanceLogCollection = this.Enumerate(security);
				foreach (EquipmentMaintenanceLogClass equipmentMaintenanceLog in enumerateMaintenanceLogCollection)
				{
					if (equipmentMaintenanceLog.MaintenanceReasonGuid == maintenanceReason.IdentityGuid)
					{
						equipmentMaintenanceLog.MaintenanceReasonGuid = Guid.Empty;
					}
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject obj)
		{
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject obj, bool preOperation)
		{
		}
		#endregion

		// Used for coming from Equipment Maintenance Log Form.
		public EquipmentMaintenanceLogClass Get(SecurityClass security, Guid equipmentMaintenanceLogGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) &&
			!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
			!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
				throw new FMInsufficientRightsException();

			EquipmentMaintenanceLogClass equipmentMaintenanceLog = new EquipmentMaintenanceLogClass
																					{
																							IdentityGuid = equipmentMaintenanceLogGuid
																					};

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.GetSQL(cmd);
				equipmentMaintenanceLog.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return equipmentMaintenanceLog;
		}

		// Used for coming from Equipment Maintenance Log Form.
		public EquipmentMaintenanceLogCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) &&
			!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
			!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
				throw new FMInsufficientRightsException();


			EquipmentMaintenanceLogClass equipmentMaintenanceLog = new EquipmentMaintenanceLogClass();
			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.EnumerateSQL(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			EquipmentMaintenanceLogCollectionClass equipmentMaintenanceLogCollection = new EquipmentMaintenanceLogCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				equipmentMaintenanceLog = new EquipmentMaintenanceLogClass();
				equipmentMaintenanceLog.Load(set);
				equipmentMaintenanceLogCollection.Add(equipmentMaintenanceLog);
				table.Rows.RemoveAt(0);
			}

			return equipmentMaintenanceLogCollection;
		}

		/// <summary>
		/// Returns most recent Maintenance Logs of equipments not in service. 
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

			bool toRet = false;

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) 
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			var equipmentMaintenanceLog = new EquipmentMaintenanceLogClass();

			using (var cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.MaintenanceReasonUsedCount(cmd, maintenanceReasonGuid);
				var recordCount = (int)this.ConsolidatedDA.ExecuteScalar(cmd, security);

				if (recordCount > 0)
				{
					toRet = true;
				}
			}


			return toRet;
		}

		/// <summary>
		/// Returns most recent Maintenance Logs of equipments not in service. 
		/// </summary>
		/// <param name="security"></param>
		/// <param name="maintenanceReasonGuid"></param>
		/// <returns></returns>
		public EquipmentMaintenanceLogCollectionClass EnumerateByMaintenanceReason(SecurityClass security, Guid maintenanceReasonGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) &&
			!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
			!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
				throw new FMInsufficientRightsException();

			EquipmentMaintenanceLogCollectionClass equipmentMaintenanceLogCollection = new EquipmentMaintenanceLogCollectionClass();
			EquipmentMaintenanceLogClass equipmentMaintenanceLog = new EquipmentMaintenanceLogClass();

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.EnumerateByMaintenanceReasonSQL(cmd, maintenanceReasonGuid);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				equipmentMaintenanceLog = new EquipmentMaintenanceLogClass();
				equipmentMaintenanceLog.Load(set);
				equipmentMaintenanceLogCollection.Add(equipmentMaintenanceLog);
				table.Rows.RemoveAt(0);
			}

			return equipmentMaintenanceLogCollection;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid equipmentMaintenanceLogGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
				throw new FMInsufficientRightsException();

			EquipmentMaintenanceLogClass equipmentMaintenanceLog = this.Get(security, equipmentMaintenanceLogGuid);
			if (equipmentMaintenanceLog.IdentityGuid == Guid.Empty)
				throw (new Exception("Equipment Maintenance Log Not Found"));

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentMaintenanceLog.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}
