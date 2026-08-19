using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	public enum STRING_TYPE
	{
		DOT_HAZARDOUS_MESSAGE = 0,
		PRODUCT_MESSAGE = 1,
		ALLOCATION_GROUP = 2,
		PRODUCT_GROUP = 3,
		COMPANY_TYPE = 4,
		ADDITIVE_PROFILE = 5,
		ALARM_EVENT_CATEGORY = 6,
		EMAIL_ADDRESS = 7,
		COMPANY_GROUP = 8,
		ENTRY_MESSAGE = 9,
		EXIT_MESSAGE = 10,
		PROCESS_VARIABLE_MESSAGE = 11,
		FOOT_NOTE = 12,
		SHIPTO_STATE = 13,
		FUEL_CARD_TYPE = 14,
		POINT_TEMPLATE_TYPE = 15,
		SITE_CERTIFICATE = 16,
		POINT_CATEGORY = 17,
		MAX_STRING_TYPE = 18
	};

	/// <summary>
	/// Summary description for ApplicationStringCollectionClass.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class ApplicationStringCollectionClass : List<ApplicationStringClass>
	{
		public void RemoveByIdentityGuid(ApplicationStringClass applicationString)
		{
			int index = 0;

			foreach (ApplicationStringClass item in this)
			{
				if (item.IdentityGuid == applicationString.IdentityGuid)
				{
					this.RemoveAt(index);
					return;
				}

				index++;
			}
		}
	}


	/// <summary>
	/// Summary description for ApplicationStringClass.
	/// </summary>
	[DataContract]
	[Serializable]
	[KnownType(typeof(STRING_TYPE))]
   [EntityImportExportWorksheetAttribute("APPLICATIONSTRINGS")]
    public class ApplicationStringClass : BaseDataObject
	{

		[EntityImportExportAttribute("APPLICATIONSTRINGGUID", 200, "APPLICATIONSTRINGGUID",2)]
		public Guid ApplicationStringGuid { get { return this.IdentityGuid; } set { base.IdentityGuid = value; } }

		[DataMember]
		public STRING_TYPE Type;

		[EntityImportExportAttribute("ID*", 100, "ID",3)]
		public override string ID
		{
			get { return this._ID; }
			set
			{
				switch (this.Type)
				{
					case STRING_TYPE.DOT_HAZARDOUS_MESSAGE:
						this.SetString("DOT Hazardous Message", 120, value, ref this._ID);
						break;
					case STRING_TYPE.PRODUCT_MESSAGE:
						this.SetString("Product Message", 120, value, ref this._ID);
						break;
					case STRING_TYPE.ALLOCATION_GROUP:
						this.SetString("Allocation Group ID", 30, value, ref this._ID);
						break;
					case STRING_TYPE.PRODUCT_GROUP:
						this.SetString("Product Group ID", 30, value, ref this._ID);
						break;
					case STRING_TYPE.COMPANY_TYPE:
						this.SetString("Company Type ID", 30, value, ref this._ID);
						break;
					case STRING_TYPE.ADDITIVE_PROFILE:
						this.SetString("ID", 30, value, ref this._ID);
						break;
					case STRING_TYPE.ALARM_EVENT_CATEGORY:
						this.SetString("Category Name", 30, value, ref this._ID);
						break;
					case STRING_TYPE.EMAIL_ADDRESS:
						this.SetString("E-mail Address", 60, value, ref this._ID);
						break;
					case STRING_TYPE.COMPANY_GROUP:
						this.SetString("Company Group ID", 30, value, ref this._ID);
						break;
					case STRING_TYPE.ENTRY_MESSAGE:
						this.SetString("Entry Message", 120, value, ref this._ID);
						break;
					case STRING_TYPE.EXIT_MESSAGE:
						this.SetString("Exit Message", 120, value, ref this._ID);
						break;
					case STRING_TYPE.PROCESS_VARIABLE_MESSAGE:
						this.SetString("Process Variable Message", 120, value, ref this._ID);
						break;
					case STRING_TYPE.SHIPTO_STATE:
						this.SetString("State", 30, value, ref this._ID);
						break;
					case STRING_TYPE.FUEL_CARD_TYPE:
						this.SetString("Fuel Card Type ID", 30, value, ref this._ID);
						break;
					case STRING_TYPE.POINT_TEMPLATE_TYPE:
						this.SetString("Point Type", 30, value, ref this._ID);
						break;
					case STRING_TYPE.SITE_CERTIFICATE:
						this.SetString("Site Certificate", 250, value, ref this._ID);
						break;
					case STRING_TYPE.POINT_CATEGORY:
						this.SetString("Point Category", 30, value, ref this._ID);
						break;
					case STRING_TYPE.FOOT_NOTE:
						this.SetString("Footnote", 250, value, ref this._ID);
						break;
					default:
						this.SetString("Application String Import", 250, value, ref this._ID);
						break;
				}
			}
		}


		public ApplicationStringClass()
		{
			this.Initialize();
		}

		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

		[EntityImportExportAttribute("TYPE", 100, "TYPE",1)]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				switch (this.Type)
				{
					case STRING_TYPE.DOT_HAZARDOUS_MESSAGE:
						return ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE;
					case STRING_TYPE.PRODUCT_MESSAGE:
						return ENTITY_TYPE.PRODUCT_MESSAGE;
					case STRING_TYPE.ALLOCATION_GROUP:
						return ENTITY_TYPE.ALLOCATION_GROUP;
					case STRING_TYPE.PRODUCT_GROUP:
						return ENTITY_TYPE.PRODUCT_GROUP;
					case STRING_TYPE.COMPANY_TYPE:
						return ENTITY_TYPE.COMPANY_TYPE;
					case STRING_TYPE.ADDITIVE_PROFILE:
						return ENTITY_TYPE.ADDITIVE_PROFILE;
					case STRING_TYPE.ALARM_EVENT_CATEGORY:
						return ENTITY_TYPE.ALARM_EVENT_CATEGORY;
					case STRING_TYPE.EMAIL_ADDRESS:
						return ENTITY_TYPE.EMAIL_ADDRESS;
					case STRING_TYPE.COMPANY_GROUP:
						return ENTITY_TYPE.COMPANY_GROUP;
					case STRING_TYPE.ENTRY_MESSAGE:
						return ENTITY_TYPE.ENTRY_MESSAGE;
					case STRING_TYPE.EXIT_MESSAGE:
						return ENTITY_TYPE.EXIT_MESSAGE;
					case STRING_TYPE.PROCESS_VARIABLE_MESSAGE:
						return ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE;
					case STRING_TYPE.SHIPTO_STATE:
						return ENTITY_TYPE.SHIPTO_STATE;
					case STRING_TYPE.FUEL_CARD_TYPE:
						return ENTITY_TYPE.FUEL_CARD_TYPE;
					case STRING_TYPE.POINT_TEMPLATE_TYPE:
						return ENTITY_TYPE.POINT_TEMPLATE_TYPE;
					case STRING_TYPE.SITE_CERTIFICATE:
						return ENTITY_TYPE.SITE_CERTIFICATE;
					case STRING_TYPE.POINT_CATEGORY:
						return ENTITY_TYPE.POINT_CATEGORY;
					case STRING_TYPE.FOOT_NOTE:
						return ENTITY_TYPE.FOOTNOTE;
					default:
						return ENTITY_TYPE.UNDEFINED;
				}
			}

			set
			{
				if (value == ENTITY_TYPE.DOT_HAZARDOUS_MESSAGE) this.Type = STRING_TYPE.DOT_HAZARDOUS_MESSAGE;
				else if (value == ENTITY_TYPE.PRODUCT_MESSAGE) this.Type = STRING_TYPE.PRODUCT_MESSAGE;
				else if (value == ENTITY_TYPE.ALLOCATION_GROUP) this.Type = STRING_TYPE.ALLOCATION_GROUP;
				else if (value == ENTITY_TYPE.PRODUCT_GROUP) this.Type = STRING_TYPE.PRODUCT_GROUP;
				else if (value == ENTITY_TYPE.COMPANY_TYPE) this.Type = STRING_TYPE.COMPANY_TYPE;
				else if (value == ENTITY_TYPE.ADDITIVE_PROFILE) this.Type = STRING_TYPE.ADDITIVE_PROFILE;
				else if (value == ENTITY_TYPE.ALARM_EVENT_CATEGORY) this.Type = STRING_TYPE.ALARM_EVENT_CATEGORY;
				else if (value == ENTITY_TYPE.EMAIL_ADDRESS) this.Type = STRING_TYPE.EMAIL_ADDRESS;
				else if (value == ENTITY_TYPE.COMPANY_GROUP) this.Type = STRING_TYPE.COMPANY_GROUP;
				else if (value == ENTITY_TYPE.ENTRY_MESSAGE) this.Type = STRING_TYPE.ENTRY_MESSAGE;
				else if (value == ENTITY_TYPE.EXIT_MESSAGE) this.Type = STRING_TYPE.EXIT_MESSAGE;
				else if (value == ENTITY_TYPE.PROCESS_VARIABLE_MESSAGE) this.Type = STRING_TYPE.PROCESS_VARIABLE_MESSAGE;
				else if (value == ENTITY_TYPE.SHIPTO_STATE) this.Type = STRING_TYPE.SHIPTO_STATE;
				else if (value == ENTITY_TYPE.FUEL_CARD_TYPE) this.Type = STRING_TYPE.FUEL_CARD_TYPE;
				else if (value == ENTITY_TYPE.POINT_TEMPLATE_TYPE) this.Type = STRING_TYPE.POINT_TEMPLATE_TYPE;
				else if (value == ENTITY_TYPE.POINT_CATEGORY) this.Type = STRING_TYPE.POINT_CATEGORY;
				else if (value == ENTITY_TYPE.FOOTNOTE) this.Type = STRING_TYPE.FOOT_NOTE;
				else this.Type = STRING_TYPE.MAX_STRING_TYPE;

			}
		}

		private void Initialize()
		{
			this.Type = STRING_TYPE.MAX_STRING_TYPE;
		}

		public override void Reset()
		{
			base.Reset();
			this.Initialize();
		}

		public void Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			this.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this.IdentityGuid = DataObject.getValue<Guid>(row["ApplicationStringGuid"], Guid.Empty);
			this.Type = DataObject.getValue<STRING_TYPE>(row["LookupApplicationStringTypeIndex"], STRING_TYPE.MAX_STRING_TYPE);
			this.ID = DataObject.getValue<string>(row["ID"], "");
			this.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.CreatedDate);
			this.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);

		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblApplicationString " +
				"(LookupApplicationStringTypeIndex," +
				"ID," +
				"SiteGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"ApplicationStringGuid" +
				") VALUES (" +
				"@LookupApplicationStringTypeIndex," +
				"@ID," +
				"@SiteGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@ApplicationStringGuid)";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)this.Type;
			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@ApplicationStringGuid"].Value = this._IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblApplicationString " +
			  "SET ID = @ID," +
			  "SiteGuid = @SiteGuid," +
			  "UpdatedDate = @UpdatedDate," +
			  "UpdatedBy = @UpdatedBy " +
			  "WHERE ApplicationStringGuid = @ApplicationStringGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@ApplicationStringGuid"].Value = this.IdentityGuid;
		}


		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblApplicationString" +
				" WHERE ApplicationStringGuid = @ApplicationStringGuid";

			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ApplicationStringGuid"].Value = this.IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" +
				" FROM tblApplicationString " + SQLUpdateLock(bInTransaction) +
				" WHERE ApplicationStringGuid = @ApplicationStringGuid";

			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ApplicationStringGuid"].Value = this.IdentityGuid;
		}

		public void EnumerateByApplicationStringGuidListSQL(SqlCommand cmd, bool bInTransaction, List<Guid> applicationStringGuidList)
		{
			cmd.CommandText = "SELECT appStr.*"
					 + " FROM tblApplicationString appStr" + SQLUpdateLock(bInTransaction) 
					 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = appStr.ApplicationStringGuid"
					 + " ORDER BY ApplicationStringGuid";
			GenerateGuidListTable(cmd, applicationStringGuidList);
		}

		/// <summary>
		/// This method will populate the SQL command with the SQL to retrieve all application strings
		/// for company types.
		/// </summary>
		/// <param name="cmd">SQL Command object.</param>
		/// <param name="bInTransaction">Flag to indicate whether or not the query is in a transaction.</param>
		public void EnumerateAllCompanyTypeSql(SqlCommand cmd, bool bInTransaction)
        {
			cmd.CommandText =
				"SELECT ECTTS.SiteGuid AS MappedSiteGuid, A.ID, A.ApplicationStringGuid, A.LookupApplicationStringTypeIndex"
				+ " FROM tblApplicationString A " + SQLUpdateLock(bInTransaction)
				+ " LEFT JOIN map.tblEntityCompanyTypeToSite ECTTS ON A.ApplicationStringGuid = ECTTS.ApplicationStringGuid AND ECTTS.AssignedFromSiteGuid = A.SiteGuid"
				+ " WHERE LookupApplicationStringTypeIndex = " + (int)STRING_TYPE.COMPANY_TYPE + " ";
		}

		public void SelectByIDAndTypeSQL(SecurityClass security, bool bInTransaction, SqlCommand cmd)
		{
			if (this.Type == STRING_TYPE.SHIPTO_STATE)
			{
				cmd.CommandText = "SELECT tblApplicationString.*" + " FROM tblApplicationString " + SQLUpdateLock(bInTransaction)
								  + " WHERE ID = @ID AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";
			}

			else if (this.Type == STRING_TYPE.SITE_CERTIFICATE)
			{
				cmd.CommandText = "SELECT tblApplicationString.*" + " FROM tblApplicationString " + SQLUpdateLock(bInTransaction)
								  + " WHERE ID = @ID "
										+ " AND SiteGuid = '" + security.SiteGuid.ToString() + "'"
										+ " AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";
			}

			else
			{
				cmd.CommandText = "SELECT tblApplicationString.*" +
						" FROM tblApplicationString " + SQLUpdateLock(bInTransaction) +
						" WHERE" + this.SiteWhereClause(security, "tblApplicationString", "ApplicationStringGuid") +
						" AND ID = @ID AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";
			}

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
			cmd.Parameters["@ID"].Value = this.ID;

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)this.Type;
		}

		public void EnumerateByTypeSQL(SqlCommand cmd, SecurityClass security)
		{
			if (this.Type == STRING_TYPE.SHIPTO_STATE)
			{
				cmd.CommandText = "SELECT tblApplicationString.*" +
					" FROM tblApplicationString" +
					" WHERE LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex" +
					" ORDER BY ID";
			}
			else if (this.Type == STRING_TYPE.SITE_CERTIFICATE)
			{
				cmd.CommandText = "SELECT tblApplicationString.*" +
					" FROM tblApplicationString" +
					" WHERE LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex" +
					" AND SiteGuid = '" + security.SiteGuid.ToString() + "'" +
					" ORDER BY ID";
			}
			else
			{
				cmd.CommandText = "SELECT tblApplicationString.*" +
					" FROM tblApplicationString" +
					" WHERE" + this.SiteWhereClause(security, "tblApplicationString", "ApplicationStringGuid") +
					" AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex" +
					" ORDER BY ID";
			}

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)this.Type;
		}

		public void EnumerateByTypeAndSiteSQL(SqlCommand cmd, Guid? siteGuid)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" + " FROM tblApplicationString"
							  + " WHERE LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";

			if (siteGuid.HasValue)
			{
				cmd.CommandText += " AND SiteGuid = '" + siteGuid.ToString() + "'";
			}

			cmd.CommandText += " ORDER BY ID";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)this.Type;
		}
	}
}
