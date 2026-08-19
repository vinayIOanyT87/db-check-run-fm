using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Collections;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	public enum ChangeQueueRecordType
	{
		None = 0,
		Companies = 1,
		Equipment,
		FuelCards,
		Personnel,
		Products,
		Transactions,
		Groups,
		TransactionAliases,
		CloseoutDO,
		ApplicationStrings,
		PIDXProfiles,
		PIDXProfileCompanyMaps  // Included in PIDXProfiles

	}

	public enum ChangeQueueEventType
	{
		Add,
		Modify,
		Purge
	}

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(ChangeQueueRecordClass))]
	public class ChangeQueueRecordCollection : List<ChangeQueueRecordClass>
	{
		public void MarkDuplicates()
		{
			// Loop through all items except the first one in the list since 
			// we will be checking prior items for duplicates
			for (int index = 0; index < Count - 1; ++index)
			{
				ChangeQueueRecordClass record = this[index];

				if (record.IsDeletion == false
					&& record.Duplicate == false)
				{
					for (int index2 = index + 1; index2 < Count; ++index2)
					{
						ChangeQueueRecordClass record2 = this[index2];

						if (record2.Duplicate == false
							&& record2.IsDeletion == false
							&& record2.RecordType == record.RecordType
							&& record2.RecordGuid == record.RecordGuid)
						{
							record2.Duplicate = true;
						}
					}
				}
			}
		}
	}

	[DataContract]
   [Serializable]
	public sealed class ChangeQueueRecordClass : BaseDataObject
	{
		[DataMember]
		[XmlIgnore]
		public long EventIndex { get; set; }
		[DataMember]
		public string EventType { get; set; }
		[DataMember]
		public ChangeQueueRecordType RecordType { get; set; }
		[DataMember]
		[XmlIgnore]
		public bool Completed { get; set; }
		[DataMember]
		public string RecordID { get; set; }
		[DataMember]
		[XmlIgnore]
		public bool Duplicate { get; set; }

		/// <summary>
		/// Guid of the recorded item (for example: EquipmentGuid, PersonnelGuid)
		/// </summary>
		[DataMember]
		[XmlIgnore]
		public Guid RecordGuid { get; set; }

		public bool IsDeletion
		{
			get { return EventType.Equals("D"); }
		}

		public bool IsUpdate
		{
			get { return EventType.Equals("U"); }
		}

		public bool IsInsert
		{
			get { return EventType.Equals("I"); }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.CHANGE_QUEUE_RECORD; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public ChangeQueueRecordClass()
		{
			Reset();
		}

		public ChangeQueueRecordClass(Guid newSiteGuid)
		{
			SiteGuid = newSiteGuid;
			Reset();
		}

		public ChangeQueueRecordClass(Guid newSiteGuid, ChangeQueueEventType eventType, BaseDataObject mainObject)
		{
			SiteGuid = newSiteGuid;
			EventType = TranslateEventType(eventType);
			RecordType = DetermineRecordType(mainObject);
			RecordGuid = mainObject.IdentityGuid;
			RecordID = mainObject.ID;
			CreatedDate = mainObject.CreatedDate;
			CreatedBy = mainObject.CreatedBy;
			UpdatedBy = mainObject.UpdatedBy;
			UpdatedDate = mainObject.UpdatedDate;
		}

		private string TranslateEventType(ChangeQueueEventType eventType)
		{
			switch (eventType)
			{
				case ChangeQueueEventType.Add:
					return "I";

				case ChangeQueueEventType.Modify:
					return "U";

				case ChangeQueueEventType.Purge:
					return "D";
			}

			throw new ArgumentOutOfRangeException("Unsupported ChangeQueueEventType type");
		}

		public ChangeQueueEventType EventTypeAssignment
		{
			set { EventType = TranslateEventType(value); }
		}

		private ChangeQueueRecordType DetermineRecordType(object mainObject)
		{
			Type objectType = mainObject.GetType();

			if (objectType.Equals(typeof(CompanyClass)))
			{
				return ChangeQueueRecordType.Companies;
			}
			else if (objectType.Equals(typeof(EquipmentClass)))
			{
				return ChangeQueueRecordType.Equipment;
			}
			else if (objectType.Equals(typeof(FuelCardClass)))
			{
				return ChangeQueueRecordType.FuelCards;
			}
			else if (objectType.Equals(typeof(GroupClass)))
			{
				return ChangeQueueRecordType.Groups;
			}
			else if (objectType.Equals(typeof(PersonClass)))
			{
				return ChangeQueueRecordType.Personnel;
			}
			else if (objectType.Equals(typeof(ProductClass)))
			{
				return ChangeQueueRecordType.Products;
			}
			else if (objectType.Equals(typeof(TransactionAliasClass)))
			{
				return ChangeQueueRecordType.TransactionAliases;
			}
			else if (objectType.Equals(typeof(CloseoutDO)))
			{
				return ChangeQueueRecordType.CloseoutDO;
			}
			else
			{
				return ChangeQueueRecordType.Transactions;
			}

		}

		public override void Reset()
		{
			base.Reset();
			Duplicate = false;
		}

		public void Load(SecurityClass Security, object O)
		{
			if (Security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (O == null)
			{
				throw new ArgumentNullException("O");
			}

			if (typeof(DataSet).IsInstanceOfType(O))
			{
				DataSet Set = (DataSet)O;

				Reset();

				DataTable Table = Set.Tables[0];
				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				_IdentityGuid = DataObject.getValue<Guid>(Row["ChangesQueueGuid"], Guid.Empty);
				EventIndex = DataObject.getValue<long>(Row["EventIndex"], 0);
				EventType = DataObject.getValue<string>(Row["EventType"], "");
				RecordType = DataObject.getValue<ChangeQueueRecordType>(Row["LookupChangeQueueRecordTypeIndex"], ChangeQueueRecordType.None);
				RecordGuid = DataObject.getValue<Guid>(Row["RecordGuid"], Guid.Empty);
				RecordID = DataObject.getValue<string>(Row["RecordId"], "");
				Completed = DataObject.getValue<bool>(Row["Completed"], false);
				_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			}
			else
			{
				base.Load(O);
			}

		}


		/// <summary>
		/// Records of this type should be added by the change tracking triggers in the 
		/// database rather than here in Share Components.
		/// </summary>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText =
			"INSERT INTO tblChangesQueue (" +
				"EventType, " +
				"LookupChangeQueueRecordTypeIndex, " +
				"SiteGuid, " +
				"RecordGuid, " +
				"Completed, " +
				"RecordId, " +
				"CreatedDate, " +
				"CreatedBy, " +
				"UpdatedDate, " +
				"UpdatedBy," +
				"ChangesQueueGuid" +
				") VALUES (" +
				"@EventType, " +
				"@RecordType, " +
				"@SiteGuid, " +
				"@RecordGuid, " +
				"@Completed, " +
				"@RecordId, " +
				"@CreatedDate, " +
				"@CreatedBy, " +
				"@UpdatedDate, " +
				"@UpdatedBy," +
				"@ChangesQueueGuid)";

			cmd.Parameters.AddWithValue("@EventType", EventType);
			cmd.Parameters.AddWithValue("@RecordType", (int)RecordType);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@RecordGuid", RecordGuid);
			cmd.Parameters.AddWithValue("@Completed", Completed);
			cmd.Parameters.AddWithValue("@RecordId", RecordID);
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@ChangesQueueGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText =
				"UPDATE tblChangesQueue " +
				"SET EventType = @EventType," +
					 "LookupChangeQueueRecordTypeIndex = @RecordType," +
					 "SiteGuid = @SiteGuid," +
					 "RecordGuid = @RecordGuid," +
					 "Completed = @Completed," +
					 "RecordId = @RecordId," +
					 "UpdatedDate = @UpdatedDate," +
					 "UpdatedBy = @UpdatedBy " +
				"WHERE ChangesQueueGuid = @ChangesQueueGuid";

			cmd.Parameters.AddWithValue("@EventType", EventType);
			cmd.Parameters.AddWithValue("@RecordType", (int)RecordType);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@RecordGuid", RecordGuid);
			cmd.Parameters.AddWithValue("@Completed", Completed);
			cmd.Parameters.AddWithValue("@RecordId", RecordID);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@ChangesQueueGuid", IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText =
				"DELETE FROM tblChangesQueue " +
				"WHERE ChangesQueueGuid = @ChangesQueueGuid";

			cmd.Parameters.AddWithValue("@ChangesQueueGuid", IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText =
				"SELECT * FROM tblChangesQueue " + SQLUpdateLock(bInTransaction) +
				 " WHERE ChangesQueueGuid = @ChangesQueueGuid";

			cmd.Parameters.AddWithValue("@ChangesQueueGuid", IdentityGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, bool incompleteRecords)
		{
			cmd.CommandText =
				"SELECT tblChangesQueue.* " +
				"FROM tblChangesQueue " +
				"WHERE SiteGuid = @SiteGuid AND Completed = @Completed ORDER BY EventIndex";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@Completed", !incompleteRecords);
		}

		public void EnumerateWithStartDateSQL(SqlCommand cmd, SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			cmd.CommandText =
				"SELECT tblChangesQueue.* " +
				"FROM tblChangesQueue " +
				"WHERE SiteGuid =  @SiteGuid AND UpdatedDate >= @StartDate AND CreatedDate < @EndDate ORDER BY EventIndex";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@StartDate", startDate);
			cmd.Parameters.AddWithValue("@EndDate", endDate);
		}

		public static void SetAllCompleteFlagSQL(SqlCommand cmd, SecurityClass security, long startIndex, long stopIndex, bool complete, DateTimeOffset updatedDate)
		{
			cmd.CommandText =
				"UPDATE tblChangesQueue " +
				"SET UpdatedDate = @UpdatedDate," +
					 "Completed = @Completed " +
				"WHERE SiteGuid =  @SiteGuid AND EventIndex >= @StartIndex AND EventIndex <= @StopIndex";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@Completed", complete);
			cmd.Parameters.AddWithValue("@UpdatedDate", updatedDate);
			cmd.Parameters.AddWithValue("@StartIndex", startIndex);
			cmd.Parameters.AddWithValue("@StopIndex", stopIndex);
		}

		public static void SetAllCompleteFlagSQL(SqlCommand cmd, SecurityClass security, ChangeQueueRecordCollection recordCollection, bool complete)
		{
			DateTimeOffset updatedDate = DateTimeOffset.Now;
			string updatedBy = security.UserID;
			string eventIndexList = "";
			for (int i = 0; i < recordCollection.Count; i++)
			{
				eventIndexList += "," + i.ToString();
			}
			cmd.CommandText =
				"UPDATE tblChangesQueue " +
				"SET Completed = @Completed," +
					 "UpdatedDate = @UpdatedDate," +
					 "UpdatedBy = @UpdatedBy" +
				"WHERE [EventIndex] IN (0" + eventIndexList + ")";

			cmd.Parameters.AddWithValue("@Completed", complete);
			cmd.Parameters.AddWithValue("@UpdatedDate", updatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
		}

		public ENTITY_TYPE GetEntityType()
		{
			switch (RecordType)
			{
				case ChangeQueueRecordType.Companies:
					return ENTITY_TYPE.COMPANY;

				case ChangeQueueRecordType.Equipment:
					return ENTITY_TYPE.EQUIPMENT;

				case ChangeQueueRecordType.FuelCards:
					return ENTITY_TYPE.FUEL_CARD;

				case ChangeQueueRecordType.Groups:
					return ENTITY_TYPE.GROUP;

				case ChangeQueueRecordType.Personnel:
					return ENTITY_TYPE.PERSONNEL;

				case ChangeQueueRecordType.Products:
					return ENTITY_TYPE.PRODUCT;

				case ChangeQueueRecordType.TransactionAliases:
					return ENTITY_TYPE.TRANSACTION_ALIAS;

				case ChangeQueueRecordType.CloseoutDO:
					return ENTITY_TYPE.CLOSEOUT;
			}

			return ENTITY_TYPE.UNKNOWN;
		}
	}
}
