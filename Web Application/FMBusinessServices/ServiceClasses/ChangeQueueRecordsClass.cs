using System;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;

using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.ChannelFactories;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ChangeQueueRecordsClass : IChangeQueueRecordsClass
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		private ChangeQueueRecordClass Get(SecurityClass security, Guid changeQueueGuid)
		{
			var record = new ChangeQueueRecordClass();

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			record.IdentityGuid = changeQueueGuid;
			record.SiteGuid = security.SiteGuid;

			using (var cmd = new SqlCommand())
			{
				record.SelectSQL(cmd, ContextUtil.IsInTransaction);
				record.Load(security, this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return record;

		}

		private void Validate(ChangeQueueRecordClass record)
		{
			if (record.SiteGuid == Guid.Empty)
			{
				throw new ArgumentOutOfRangeException(nameof(record), "SiteGuid is not set for Change Queue Record");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ChangeQueueRecordClass record)
		{
			if (!security.EnableChangeTracking)
			{
				return Guid.Empty;
			}

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (record == null)
			{
				throw new ArgumentNullException(nameof(record));
			}

		    this.Validate(record);

			record.CreatedDate = DateTimeOffset.Now;
			record.CreatedBy = security.UserID;
			record.UpdatedDate = record.CreatedDate;
			record.UpdatedBy = security.UserID;
			record.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				record.InsertSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return record.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ChangeQueueRecordClass record)
		{
			if (!security.EnableChangeTracking)
			{
				return;
			}

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (record == null)
			{
				throw new ArgumentNullException(nameof(record));
			}

		    this.Validate(record);

			// Verify ID does not exist
			ChangeQueueRecordClass oldRecord = this.Get(security, record.IdentityGuid);
			if (oldRecord.IdentityGuid != Guid.Empty && oldRecord.IdentityGuid != record.IdentityGuid)
			{
				throw (new Exception("Change Queue Record Exists"));
			}

			if (oldRecord.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Change Queue Record Not Found"));
			}

			record.UpdatedDate = DateTimeOffset.Now;
			record.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				record.UpdateSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, ChangeQueueRecordClass record)
		{
			if (record == null)
			{
				throw new ArgumentNullException(nameof(record));
			}

		    this.Purge(security, record.IdentityGuid);
		}

		public void Purge(SecurityClass security, Guid changeQueueGuid)
		{
			if (!security.EnableChangeTracking)
			{
				return;
			}

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			ChangeQueueRecordClass record = this.Get(security, changeQueueGuid);
			if (record.IdentityGuid == Guid.Empty)
			{
				throw new ApplicationException("Change Queue Record Not Found");
			}

			using (var cmd = new SqlCommand())
			{
				record.PurgeSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method marks all ChangeQueueRecords in the [recordCollection] paramater as incomplete
		/// and saves the change in the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="startIndex"></param>
		/// <param name="stopIndex"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void SetAllIncomplete(SecurityClass security, long startIndex, long stopIndex)
		{
		    this.SetAllCompleted(security, startIndex, stopIndex, false);
		}

		/// <summary>
		/// This method marks all ChangeQueueRecords in the [recordCollection] paramater as completed
		/// and saves the change in the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="startIndex"></param>
		/// <param name="stopIndex"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void SetAllCompleted(SecurityClass security, long startIndex, long stopIndex)
		{
		    this.SetAllCompleted(security, startIndex, stopIndex, true);
		}

		protected void SetAllCompleted(SecurityClass security, long startIndex, long stopIndex, bool complete)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			using (var cmd = new SqlCommand())
			{
				ChangeQueueRecordClass.SetAllCompleteFlagSQL(cmd, security, startIndex, stopIndex, complete, DateTimeOffset.Now);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		/// <summary>
		/// This method marks all ChangeQueueRecords in the [recordCollection] paramater as completed
		/// and saves the change in the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="recordCollection"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void SetAllCompletedByCollection(SecurityClass security, ChangeQueueRecordCollection recordCollection)
		{
			this.SetAllCompletedByCollection(security, recordCollection, true);
		}

		protected void SetAllCompletedByCollection(SecurityClass security, ChangeQueueRecordCollection recordCollection, bool complete)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (recordCollection == null)
			{
				throw new ArgumentNullException(nameof(recordCollection));
			}

			if (recordCollection.Count == 0)
			{
				// Nothing to do
				return;
			}

			using (var cmd = new SqlCommand())
			{
				ChangeQueueRecordClass.SetAllCompleteFlagSQL(cmd, security, recordCollection, complete);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public ChangeQueueRecordCollection EnumerateCompleteRecords(SecurityClass security)
		{
			return this.Enumerate(security, false);
		}

		public ChangeQueueRecordCollection EnumerateIncompleteRecords(SecurityClass security)
		{
			return this.Enumerate(security, true);
		}

		protected ChangeQueueRecordCollection Enumerate(SecurityClass security, bool incompleteChanges)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var recordCollection = new ChangeQueueRecordCollection();
			var record = new ChangeQueueRecordClass(security.SiteGuid);

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				record.EnumerateSQL(cmd, security, incompleteChanges);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				record = new ChangeQueueRecordClass();
				record.Load(security, set);
				recordCollection.Add(record);

				table.Rows.RemoveAt(0);
			}

			recordCollection.MarkDuplicates();

			return recordCollection;

		}

		public ChangeQueueRecordCollection EnumerateByDate(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var recordCollection = new ChangeQueueRecordCollection();
			var record = new ChangeQueueRecordClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				record.EnumerateWithStartDateSQL(cmd, security, startDate, endDate);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				record = new ChangeQueueRecordClass();
				record.Load(security, set);
				recordCollection.Add(record);

				table.Rows.RemoveAt(0);
			}

			recordCollection.MarkDuplicates();

			return recordCollection;

		}

		public static Type GetEngineType(ChangeQueueRecordType changeQueueRecordType)
		{
			switch (changeQueueRecordType)
			{
				case ChangeQueueRecordType.Companies:
					return typeof(CompaniesClass);

				case ChangeQueueRecordType.Equipment:
					return typeof(EquipmentsClass);

				case ChangeQueueRecordType.FuelCards:
					return typeof(FuelCardsClass);

				case ChangeQueueRecordType.Groups:
					return typeof(GroupsClass);

				case ChangeQueueRecordType.Personnel:
					return typeof(PersonnelClass);

				case ChangeQueueRecordType.Products:
					return typeof(ProductsClass);

				case ChangeQueueRecordType.TransactionAliases:
					return typeof(TransactionAliasesClass);
			}

			throw new NotImplementedException("Unsupported change record type.");
		}

		public void ProcessChangeQueueRecords(SecurityClass security, ChangeQueueRecordClass record, EntityToSiteMapCollectionClass siteMapCollection)
		{
			if (!security.EnableChangeTracking)
			{
				return;
			}

			if (siteMapCollection == null)
			{
			    this.Add(security, record);
			}
			else
			{
				foreach (EntityToSiteMapClass entityMap in siteMapCollection)
				{
					record.SiteGuid = entityMap.SiteGuid;
				    this.Add(security, record);
				}
			}

		}

		public static void ProcessChangeTxQueueRecords(SecurityClass security, ChangeQueueEventType eventType, Guid transGuid, string transID, Guid targetSiteGuid)
		{

			if (!IsChangeQueueEnabled(security))
			{
				return;
			}


		    ChangeQueueRecordClass record = new ChangeQueueRecordClass
		                                    {
		                                        SiteGuid = targetSiteGuid,
		                                        EventTypeAssignment = eventType,
		                                        RecordType = ChangeQueueRecordType.Transactions,
		                                        RecordGuid = transGuid,
		                                        RecordID = transID,
		                                        CreatedDate = DateTimeOffset.Now,
		                                        CreatedBy = security.UserID
		                                    };

		    ChangeQueueRecordsClass records = new ChangeQueueRecordsClass();
			records.ProcessChangeQueueRecords(security, record, null);
		}

		internal static void ProcessChangeQueueRecords(SecurityClass security, ChangeQueueEventType eventType, BaseDataObject dataObject)
		{
			ProcessChangeQueueRecords(security, eventType, dataObject, true);
		}

		public static void ProcessChangeQueueRecords(SecurityClass security, ChangeQueueEventType eventType, BaseDataObject dataObject, bool processEntitySiteMaps)
		{
			EntityToSiteMapCollectionClass siteMapCollection = null;

			if (processEntitySiteMaps)
			{
				EntityToSiteMaps siteMaps = new EntityToSiteMaps();
				siteMapCollection = siteMaps.EnumerateByTypeIDAndGuid(security, dataObject.EntityType, dataObject.IdentityGuid);
			}

			ChangeQueueRecordClass record = new ChangeQueueRecordClass(security.SiteGuid, eventType, dataObject);
			ChangeQueueRecordsClass records = new ChangeQueueRecordsClass();
			records.ProcessChangeQueueRecords(security, record, siteMapCollection);
		}

		public static void ProcessChangeCloseOutQueueRecords(SecurityClass security, ChangeQueueEventType eventType, Guid targetCloseOutInventoryGuid, string strProductName)
		{
			ProcessChangeQueueRecords(security, eventType, targetCloseOutInventoryGuid, strProductName, ChangeQueueRecordType.CloseoutDO);
		}

		public static void ProcessChangeQueueRecords(
			SecurityClass security,
			ChangeQueueEventType eventType,
			Guid targetGuid,
			String recName,
			ChangeQueueRecordType chngQueRecType)
		{
		    ChangeQueueRecordClass record = new ChangeQueueRecordClass
		                                    {
		                                        SiteGuid = security.SiteGuid,
		                                        EventTypeAssignment = eventType,
		                                        RecordType = chngQueRecType,
		                                        RecordGuid = targetGuid,
		                                        RecordID = recName,
		                                        CreatedDate = DateTimeOffset.Now,
		                                        CreatedBy = security.UserID
		                                    };

		    ChangeQueueRecordsClass records = new ChangeQueueRecordsClass();
			records.ProcessChangeQueueRecords(security, record, null);
		}

		protected static bool IsChangeQueueEnabled(SecurityClass security)
		{
			bool retVal = false;


			string changeQueueEnabled = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ChangeQueueEnabled));

			if (false == string.IsNullOrEmpty(changeQueueEnabled))
			{
				Boolean.TryParse(changeQueueEnabled, out retVal);
			}

			return retVal;
		}

	}
}