using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for Allocation.
	/// </summary>
	[Serializable()]
	[CollectionDataContract]
	[KnownType(typeof(AllocationClass))]
	public class AllocationCollectionClass : CollectionBase
	{
		public void Add(AllocationClass Allocation)
		{
			List.Add(Allocation);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				List.RemoveAt(index);
			}
		}

		public void Remove(AllocationClass Allocation)
		{
			int index = 0;
			foreach (AllocationClass Item in List)
			{
				if (Item.IdentityGuid == Allocation.IdentityGuid)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public AllocationClass Item(int Index)
		{
			return (AllocationClass)List[Index];
		}
	}

	[Serializable()]
	[DataContract]
	public class AllocationClass : BaseDataObject, IAlarmAndEventDiscovery
	{
		[DataMember]
		public Guid CompanyMapGuid;
		[DataMember]
		public COMPANY_MAP_TYPE CompanyMapType;
		[DataMember]
		public Date _EffectiveDate;
		[DataMember]
		public Date _ExpirationDate;
		[DataMember]
		public double _LoadWarning;
		[DataMember]
		public double _LoadDenial;
		[DataMember]
		public string _ContractNumber;
		[DataMember]
		public Guid AllocationGroupGuid;
		[DataMember]
		public Date LastAllocationResetDate;

		//alarm and event definitions
		public const string AllocationResetFailureKey = "Allocation Reset Failure";
		public static AlarmAndEventDescriptorClass AllocationResetFailureDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, AllocationResetFailureKey);
		public enum UserAllocationStatus { DoesNotHaveAllocationRights, HasGroupCompanyMappingToAll, HasGroupMappingToSome };

		// Linked Items
		[DataMember]
		public string AllocationGroupID;

		public string EffectiveDate { get { return _EffectiveDate.ToString(); } set { SetDate("Effective Date", value, ref _EffectiveDate); } }
		public string ExpirationDate { get { return _ExpirationDate.ToString(); } set { SetDate("Expiration Date", value, ref _ExpirationDate); } }
		public string ContractNumber { get { return _ContractNumber; } set { SetString("Contract Number", 10, value, ref _ContractNumber); } }

		// Ancillary Items
		[DataMember]
		public AllocationLineItemCollectionClass LineItemCollection;

		public double LoadWarning
		{
			get { return _LoadWarning; }
			set
			{
				if (value > 100.0
		  || value < 0.0)
					throw new Exception("Load Warning must be from 0 and 100");
				_LoadWarning = value;
			}
		}

		public double LoadDenial
		{
			get { return _LoadDenial; }
			set
			{
				if (value < 100.0)
					throw new Exception("Load Denial must be greater or equal 100");
				_LoadDenial = value;
			}
		}

		string SelectClause = "SELECT tblAllocations.*," +
								 "(SELECT ID FROM tblApplicationString WHERE AllocationGroupApplicationStringGuid = tblApplicationString.ApplicationStringGuid) AS AllocationGroupID";

		public AllocationClass()
		{
			_EffectiveDate = new Date();
			_ExpirationDate = new Date();
			LastAllocationResetDate = new Date();

			Initialize();
		}

		public AllocationClass(SiteClass Site)
		{
			_EffectiveDate = new Date(Site);
			_ExpirationDate = new Date(Site);
			LastAllocationResetDate = new Date(Site);

			Initialize();
		}


		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.ALLOCATION;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.ALLOCATION_GROUP;
			}
		}

		// alarm and event required members
		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] descriptors = {
																					 AllocationResetFailureDescriptor
																				};
				return descriptors;
			}
		}

		public static AlarmAndEventLogClass AllocationResetFailure
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(AllocationResetFailureDescriptor)
				{
					AssociatedData = "Deferred Allocation Reset Failed"
				};
				return alarmAndEventLog;
			}
		}


		public static string GetCompanyMapForeignKeyColumnName(COMPANY_MAP_TYPE companyMapType)
		{
			switch (companyMapType)
			{
				case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
					return "CompanyLoadOwnerToManagerGuid";
				case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
					return "CompanyShipperToOwnerGuid";
				case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
					return "CompanyBillToToShipperGuid";
				case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
					return "CompanyShipToToBillToGuid";
				case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
					return "CompanySupplierToOwnerGuid";
				case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
					return "CompanyOffLoadOwnerToManagerGuid";
				default:
					return "Unknown";

			}
		}

		private void Initialize()
		{
			CompanyMapGuid = Guid.Empty;
			CompanyMapType = COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE;
			_EffectiveDate.Value = TimeConverter.Today(_EffectiveDate.StandardName);
			_ExpirationDate.Value = TimeConverter.Today(_ExpirationDate.StandardName).AddDays(1);
			LoadWarning = 0.0;
			LoadDenial = 100.0;
			_ContractNumber = "";
			AllocationGroupGuid = Guid.Empty;
			LastAllocationResetDate.Value = TimeConverter.Today(LastAllocationResetDate.StandardName);
			AllocationGroupID = "{None}";
			LineItemCollection = new AllocationLineItemCollectionClass();
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
				throw new ArgumentNullException("Set");

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

			IdentityGuid = DataObject.getValue<Guid>(Row["AllocationGuid"], Guid.Empty);
			CompanyMapType = DataObject.getValue<COMPANY_MAP_TYPE>(Row["LookupCompanyMapTypeIndex"], COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE);
			CompanyMapGuid = DataObject.getValue<Guid>(Row[GetCompanyMapForeignKeyColumnName(CompanyMapType)], Guid.Empty);
			_EffectiveDate.Value = DataObject.getValue<DateTimeOffset>(Row["EffectiveDate"], TimeConverter.Today(_EffectiveDate.StandardName));
			_ExpirationDate.Value = DataObject.getValue<DateTimeOffset>(Row["ExpirationDate"], TimeConverter.Today(_ExpirationDate.StandardName).AddDays(1));
			LoadWarning = DataObject.getValue<double>(Row["LoadWarning"], 0.0);
			LoadDenial = DataObject.getValue<double>(Row["LoadDenial"], 100.0);
			_ContractNumber = DataObject.getValue<string>(Row["ContractNumber"], "");
			AllocationGroupGuid = DataObject.getValue<Guid>(Row["AllocationGroupApplicationStringGuid"], Guid.Empty);
			LastAllocationResetDate.Value = DataObject.getValue<DateTimeOffset>(Row["LastAllocationResetDate"], TimeConverter.Today(LastAllocationResetDate.StandardName));
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			// selected column alias
			AllocationGroupID = DataObject.getValue<string>(Row["AllocationGroupID"], "{None}");
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblAllocations (" +
				GetCompanyMapForeignKeyColumnName(CompanyMapType) + "," +
				"LookupCompanyMapTypeIndex," +
				"EffectiveDate," +
				"ExpirationDate," +
				"LoadWarning," +
				"LoadDenial," +
				"ContractNumber," +
				"AllocationGroupApplicationStringGuid," +
				"LastAllocationResetDate," +
				"SiteGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"AllocationGuid" +
				") VALUES (" +
				"@CompanyMapGuid," +
				"@LookupCompanyMapTypeIndex," +
				"@EffectiveDate," +
				"@ExpirationDate," +
				"@LoadWarning," +
				"@LoadDenial," +
				"@ContractNumber," +
				"@AllocationGroupApplicationStringGuid," +
				"@LastAllocationResetDate," +
				"@SiteGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@AllocationGuid" +
				")";

			cmd.Parameters.Add("@CompanyMapGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupCompanyMapTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@EffectiveDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@LoadWarning", SqlDbType.Float);
			cmd.Parameters.Add("@LoadDenial", SqlDbType.Float);
			cmd.Parameters.Add("@ContractNumber", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@AllocationGroupApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LastAllocationResetDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@CompanyMapGuid"].Value = CompanyMapGuid;
			cmd.Parameters["@LookupCompanyMapTypeIndex"].Value = (int)CompanyMapType;
			cmd.Parameters["@EffectiveDate"].Value = _EffectiveDate.Value;
			cmd.Parameters["@ExpirationDate"].Value = _ExpirationDate.Value;
			cmd.Parameters["@LoadWarning"].Value = LoadWarning;
			cmd.Parameters["@LoadDenial"].Value = LoadDenial;
			cmd.Parameters["@ContractNumber"].Value = ContractNumber;

			if (AllocationGroupGuid != Guid.Empty)
			{
				cmd.Parameters["@AllocationGroupApplicationStringGuid"].Value = AllocationGroupGuid;
			}
			else
			{
				cmd.Parameters["@AllocationGroupApplicationStringGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@LastAllocationResetDate"].Value = LastAllocationResetDate.Value;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@AllocationGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE tblAllocations " +
				"SET " + GetCompanyMapForeignKeyColumnName(CompanyMapType) + " = @CompanyMapGuid," +
				"LookupCompanyMapTypeIndex = @LookupCompanyMapTypeIndex," +
				"EffectiveDate = @EffectiveDate," +
				"ExpirationDate = @ExpirationDate," +
				"LoadWarning = @LoadWarning," +
				"LoadDenial = @LoadDenial," +
				"ContractNumber = @ContractNumber," +
				"AllocationGroupApplicationStringGuid = @AllocationGroupApplicationStringGuid," +
				"LastAllocationResetDate = @LastAllocationResetDate," +
				"SiteGuid = @SiteGuid," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE AllocationGuid = @AllocationGuid";

			cmd.Parameters.Add("@CompanyMapGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupCompanyMapTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@EffectiveDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@LoadWarning", SqlDbType.Float);
			cmd.Parameters.Add("@LoadDenial", SqlDbType.Float);
			cmd.Parameters.Add("@ContractNumber", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@AllocationGroupApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LastAllocationResetDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@CompanyMapGuid"].Value = CompanyMapGuid;
			cmd.Parameters["@LookupCompanyMapTypeIndex"].Value = (int)CompanyMapType;
			cmd.Parameters["@EffectiveDate"].Value = _EffectiveDate.Value;
			cmd.Parameters["@ExpirationDate"].Value = _ExpirationDate.Value;
			cmd.Parameters["@LoadWarning"].Value = LoadWarning;
			cmd.Parameters["@LoadDenial"].Value = LoadDenial;
			cmd.Parameters["@ContractNumber"].Value = ContractNumber;

			if (AllocationGroupGuid != Guid.Empty)
			{
				cmd.Parameters["@AllocationGroupApplicationStringGuid"].Value = AllocationGroupGuid;
			}
			else
			{
				cmd.Parameters["@AllocationGroupApplicationStringGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@LastAllocationResetDate"].Value = LastAllocationResetDate.Value;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@AllocationGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblAllocations " +
				"WHERE AllocationGuid = @AllocationGuid";

			cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AllocationGuid"].Value = IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAllocations" +
				" WHERE AllocationGuid = @AllocationGuid";

			cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AllocationGuid"].Value = IdentityGuid;
		}

		public void SelectByCompanyMapGuidAndDatesSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAllocations " + SQLUpdateLock(bInTransaction) +
				" WHERE " + GetCompanyMapForeignKeyColumnName(CompanyMapType) + " = @CompanyMapGuid" +
				" AND EffectiveDate <= @EffectiveDate" +
				" AND ExpirationDate >= @ExpirationDate";

			cmd.Parameters.Add("@CompanyMapGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EffectiveDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@CompanyMapGuid"].Value = CompanyMapGuid;
			cmd.Parameters["@EffectiveDate"].Value = EffectiveDate;
			cmd.Parameters["@ExpirationDate"].Value = ExpirationDate;
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAllocations" +
				" WHERE tblAllocations.SiteGuid = @SiteGuid" +
				" ORDER BY EffectiveDate";

			//The SQL above used to order by the company map foreign key column as well, but now that there are many company map FK's and we don't know the type of company map when this is executed
			//we are just ordering by effective date.

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
		}

		public void EnumerateByCompanyMapGuidSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAllocations" +
				" WHERE tblAllocations.SiteGuid = @SiteGuid" +
				" AND " + GetCompanyMapForeignKeyColumnName(CompanyMapType) + " = @CompanyMapGuid" +
				" ORDER BY EffectiveDate";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyMapGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@CompanyMapGuid"].Value = CompanyMapGuid;
		}

		public void EnumerateByCompanyMapTypeSQL(SqlCommand cmd, SecurityClass security, COMPANY_MAP_TYPE Type)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAllocations," + CompanyMapClass.GetMappingTableName(Type) +
				" WHERE tblAllocations.SiteGuid = @SiteGuid" +
				" AND " + CompanyMapClass.GetMappingTableName(Type) + "." + CompanyMapClass.GetMappingTablePrimaryKeyColumnName(Type) + "= tblAllocations." + GetCompanyMapForeignKeyColumnName(Type) +
				" ORDER BY " + GetCompanyMapForeignKeyColumnName(Type) + ",EffectiveDate";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
		}

		public void EnumerateByAllocationGroupGuidSQL(SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAllocations" +
				" WHERE AllocationGroupApplicationStringGuid = @AllocationGroupApplicationStringGuid " +
				" ORDER BY EffectiveDate";

			//The SQL above used to order by the company map foreign key column as well, but now that there are many company map FK's and we don't know the type of company map when this is executed
			//we are just ordering by effective date.

			cmd.Parameters.Add("@AllocationGroupApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AllocationGroupApplicationStringGuid"].Value = AllocationGroupGuid;
		}

	}
}
