namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using DataAccessLayer;
	using InternalClasses;

	using IsolationLevel = System.Transactions.IsolationLevel;

 /// <summary>
	/// Summary description for EquipmentQualityTagLogsClass.
	/// </summary>
	[SecuritySafeCritical]
	[QueryWriterTopic(typeof(EquipmentQualityTagLogsClass), "Quality Tag Log", AssociatedTopicType = typeof(EquipmentQualityTagLogClass), SupportsArchiveQuery = true)]
	[QueryWriterTopicSecurity(RIGHT.ADD_QUALITYTAG_RECORD)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_QUALITYTAG_RECORD)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_QUALITYTAG_RECORD)]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class EquipmentQualityTagLogsClass : IDependency, IEquipmentQualityTagLogs
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public DataSet GetDataSet(SecurityClass security,
											bool bHistorical,
											string sDateType,
											DateTimeOffset dateStart,
											DateTimeOffset dateEnd,
											string qualityTag,
											string taggedBy,
											string removedBy,
											string assetID,
											string state)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS)
			&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
				throw new FMInsufficientRightsException();

			EquipmentQualityTagLogClass equipmentQualityTagLog = new EquipmentQualityTagLogClass();

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentQualityTagLog.EnumerateSQL(cmd, 
													security, 
													bHistorical,
													sDateType, 
													dateStart, 
													dateEnd,
													qualityTag,
													taggedBy,
													removedBy,
													assetID,
													state);

				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return set;
		}

		public EquipmentQualityTagLogClass GetMostRecentByEquipmentID(SecurityClass security, string equipmentID)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS)
			&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
				throw new FMInsufficientRightsException();

			EquipmentQualityTagLogClass equipmentQualityTagLog = new EquipmentQualityTagLogClass { EquipmentID = equipmentID };
			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentQualityTagLog.GetMostRecentByEquipmentIDSQL(cmd, security);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			equipmentQualityTagLog.Load(set);
			return equipmentQualityTagLog;
		}

		// Write the sent collection "rowset" to the database.
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, EquipmentQualityTagLogClass oEquipmentQualityTagLog)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (//!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) 
				//&& 
				!security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				//&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
				)
				throw new FMInsufficientRightsException();

			if (oEquipmentQualityTagLog == null)
				throw new ArgumentNullException(nameof(oEquipmentQualityTagLog));

			if (string.IsNullOrEmpty(oEquipmentQualityTagLog.Memo))
				throw (new Exception("Memo Required"));

			if (this.GetByTagNumber(security, oEquipmentQualityTagLog.TagNumber) != null)
			{
				throw new ApplicationException("Tag number must be unique");
			}

			oEquipmentQualityTagLog.SiteGuid = security.SiteGuid;
			oEquipmentQualityTagLog.CreatedDate = DateTimeOffset.Now;
			oEquipmentQualityTagLog.CreatedBy = security.UserID;
			oEquipmentQualityTagLog.UpdatedDate = oEquipmentQualityTagLog.CreatedDate;
			oEquipmentQualityTagLog.UpdatedBy = security.UserID;
			oEquipmentQualityTagLog.TaggedBy = security.UserID;
			oEquipmentQualityTagLog.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				oEquipmentQualityTagLog.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return oEquipmentQualityTagLog.IdentityGuid;
		}

		// The only modification that can be done to a Quality Tag assigned to an 
		// Equipment is to "Remove" it.
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, EquipmentQualityTagLogClass equipmentQualityTagLog)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD))
				throw new FMInsufficientRightsException();

			if (equipmentQualityTagLog == null)
				throw new ArgumentNullException(nameof(equipmentQualityTagLog));

			if (string.IsNullOrEmpty(equipmentQualityTagLog.Memo))
				throw (new Exception("Memo Required"));

			EquipmentQualityTagLogClass equipmentQualityTagLogOld = this.Get(security, equipmentQualityTagLog.IdentityGuid);

			if (equipmentQualityTagLogOld.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("EquipmentQualityTagLog Not Found"));
			}

			EquipmentQualityTagLogClass testTagNumber = this.GetByTagNumber(security, equipmentQualityTagLog.TagNumber);
			if (testTagNumber != null
			&& testTagNumber.IdentityGuid != equipmentQualityTagLog.IdentityGuid)
			{
				throw new ApplicationException("Tag number must be unique");
			}

			equipmentQualityTagLog.UpdatedDate = DateTimeOffset.Now;
			equipmentQualityTagLog.UpdatedBy = security.UserID;
			equipmentQualityTagLog.RemovedDate = DateTimeOffset.Now;
			equipmentQualityTagLog.RemovedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentQualityTagLog.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public EquipmentQualityTagLogClass GetPreviousTagNumber(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
			{
				throw new FMInsufficientRightsException();
			}

			EquipmentQualityTagLogClass log = new EquipmentQualityTagLogClass();

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				log.PreviousTagNumberSQL(cmd, security);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (set != null
			&& set.Tables.Count > 0
			&& set.Tables[0].Rows.Count > 0)
			{
				DataRow row = set.Tables[0].Rows[0];

				log.TagNumber = DataObject.getValue<int>(row["TagNumber"], 0);
				log.TaggedDate = DataObject.getValue(row["TaggedDate"], DateTimeOffset.Now);
			}

			return log;
		}

		public EquipmentQualityTagLogClass GetByTagNumber(SecurityClass security, int tagNumber)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
			{
				throw new FMInsufficientRightsException();
			}

			EquipmentQualityTagLogClass oEquipmentQualityTagLog = new EquipmentQualityTagLogClass { TagNumber = tagNumber };

			using (SqlCommand cmd = new SqlCommand())
			{
				oEquipmentQualityTagLog.GetByTagNumberSQL(cmd, security);
				oEquipmentQualityTagLog.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			if (oEquipmentQualityTagLog.IdentityGuid == Guid.Empty)
			{
				return null;
			}

			return oEquipmentQualityTagLog;
		}

		// Used for coming from Equipment QualityTag Log Form.
		public EquipmentQualityTagLogClass Get(SecurityClass security, Guid equipmentQualityTagLogGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS))
				throw new FMInsufficientRightsException();

			EquipmentQualityTagLogClass oEquipmentQualityTagLog = new EquipmentQualityTagLogClass
																					{
																						IdentityGuid =
																								equipmentQualityTagLogGuid
																					};

			using (SqlCommand cmd = new SqlCommand())
			{
				oEquipmentQualityTagLog.GetSQL(cmd, security);
				oEquipmentQualityTagLog.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return oEquipmentQualityTagLog;
		}

		// Used for coming from Equipment Maintenance Log Form.
		public EquipmentQualityTagLogCollectionClass Enumerate(SecurityClass security, bool bHistorical)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS) &&
			!security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) &&
			!security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS) &&
			!security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD))
				throw new FMInsufficientRightsException();


			EquipmentQualityTagLogClass equipmentQualityTagLog = new EquipmentQualityTagLogClass();
			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentQualityTagLog.EnumerateSQL(cmd, bHistorical, security);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			EquipmentQualityTagLogCollectionClass equipmentQualityTagLogCollection = new EquipmentQualityTagLogCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				equipmentQualityTagLog = new EquipmentQualityTagLogClass();
				equipmentQualityTagLog.Load(set);
				equipmentQualityTagLogCollection.Add(equipmentQualityTagLog);
				table.Rows.RemoveAt(0);
			}

			return equipmentQualityTagLogCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid equipmentQualityTagLogGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
			!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
				throw new FMInsufficientRightsException();

			EquipmentQualityTagLogClass equipmentQualityTagLog = this.Get(security, equipmentQualityTagLogGuid);
			if (equipmentQualityTagLog.IdentityGuid == Guid.Empty)
				throw (new Exception("Equipment Quality Tag Log Not Found"));

			using (SqlCommand cmd = new SqlCommand())
			{
				equipmentQualityTagLog.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public string QueryWriterSQL(SecurityClass security, string selectClause, string dbName)
		{
			var log = new EquipmentQualityTagLogClass();
			return log.QueryWriterSQL(security, selectClause, dbName);
		}

		public void QueryWriterPostProcess(SecurityClass security, DataSet set)
		{
			this.CensorFieldsIfNecessary(security, set);
		}

		public string DetailPageReference()
		{
			return "QualityControlWebApp\\QualityTagAddRecordForm.aspx";
		}

		#region Private methods
		/// <summary>
		/// This method will censor fields if the user does not have the 
		/// correct rights.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="set">The data set to modify.</param>
		private void CensorFieldsIfNecessary(SecurityClass security, DataSet set)
		{
			if (security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) == false
				&& security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) == false)
			{
				set.Tables[0].Rows.Clear();
			}
		}
		#endregion

		#region IDependency implementations
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
	}
}
