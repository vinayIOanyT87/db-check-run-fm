namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    public enum COMPANY_MAP_SORT_CRITERIA
	{
		ASSIGNED = 0,
		ASSIGNEDTO = 1
	}

	// Modifications on enum COMPANY_MAP_TYPE may require update on table lookup.tblCompanyMapType
	public enum COMPANY_MAP_TYPE
	{
		LOAD_OWNER_MANAGER_MAP = 0,
		SHIPPER_OWNER_MAP = 1,
		BILLTO_SHIPPER_MAP = 2,
		SHIPTO_BILLTO_MAP = 3,
		AUTHORIZED_CARRIER_MAP = 4,
		LOADID_SHIPTO_MAP = 5,
		USER_GROUP_COMPANY_MAP = 6,
		COMPANY_GROUP_COMPANY_MAP = 7,
		FOOT_NOTE_SHIPTO_MAP = 8,
		FOOT_NOTE_SHIPPER_MAP = 9,
		LOAD_MAX_COMPANY_MAP_TYPE = 10,
		SUPPLIER_OWNER_MAP = 11,
		OFFLOADID_SUPPLIER_MAP = 12,
		OFFLOAD_MAX_COMPANY_MAP_TYPE = 13,
		OFFLOAD_OWNER_MANAGER_MAP = 14,
        PERSON_ASSIGNED_COMPANY = 15
	};


    public class AssignedComparer : IComparer<CompanyMapClass>
	{

        public int Compare(CompanyMapClass a, CompanyMapClass b)
		{
			if (a == null || b == null)
			{
				throw new Exception("Invalid CompanyMap");
			}

			return string.Compare(a.AssignedID, b.AssignedID, StringComparison.Ordinal);
		}
	}

    public class AssignedToComparer : IComparer<CompanyMapClass>
	{
        public int Compare(CompanyMapClass a, CompanyMapClass b)
		{
			if (a == null || b == null)
			{
				throw new Exception("Invalid CompanyMap");
			}

			return string.Compare(a.AssignedToID, b.AssignedToID, StringComparison.Ordinal);
		}
	}

	/// <summary>
	/// Summary description for CompanyMapCollectionClass.
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(CompanyMapClass))]
    [KnownType(typeof(CompanyMapAuthorizedCarrierClass))]
    [KnownType(typeof(CompanyMapBillToShipperClass))]
    [KnownType(typeof(CompanyMapCompanyGroupCompanyClass))]
    [KnownType(typeof(CompanyMapFootNoteShipperClass))]
    [KnownType(typeof(CompanyMapFootNoteShipToClass))]
    [KnownType(typeof(CompanyMapLoadIdShipToClass))]
    [KnownType(typeof(CompanyMapLoadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapOffloadIdSupplierClass))]
    [KnownType(typeof(CompanyMapOffloadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapPersonAssignedCompanyClass))]
    [KnownType(typeof(CompanyMapShipperOwnerClass))]
    [KnownType(typeof(CompanyMapShipToBillToClass))]
    [KnownType(typeof(CompanyMapSupplierOwnerClass))]
    [KnownType(typeof(CompanyMapUserGroupCompanyClass))]
	public class CompanyMapCollectionClass : List<CompanyMapClass>
	{
       public void Remove(int inx)
       {
           base.RemoveAt(inx);
       }
       public CompanyMapClass Find(Guid guid)
       {
           return this.FindByGuid(guid);
       }

       public void Sort(COMPANY_MAP_SORT_CRITERIA criteria)
       {
           if (criteria == COMPANY_MAP_SORT_CRITERIA.ASSIGNED)
			{
				this.Sort(new AssignedComparer());
			}
			else
			{
				this.Sort(new AssignedToComparer());
			}
		}
	}

	/// <summary>
	/// Summary description for CompanyMapClass.
	/// </summary>
	[Serializable]
	[DataContract]
	[EntityImportExportWorksheet("COMPANYMAPS")]
    [KnownType(typeof(CompanyMapClass))]
    [KnownType(typeof(CompanyMapAuthorizedCarrierClass))]
    [KnownType(typeof(CompanyMapBillToShipperClass))]
    [KnownType(typeof(CompanyMapCompanyGroupCompanyClass))]
    [KnownType(typeof(CompanyMapFootNoteShipperClass))]
    [KnownType(typeof(CompanyMapFootNoteShipToClass))]
    [KnownType(typeof(CompanyMapLoadIdShipToClass))]
    [KnownType(typeof(CompanyMapLoadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapOffloadIdSupplierClass))]
    [KnownType(typeof(CompanyMapOffloadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapPersonAssignedCompanyClass))]
    [KnownType(typeof(CompanyMapShipperOwnerClass))]
    [KnownType(typeof(CompanyMapShipToBillToClass))]
    [KnownType(typeof(CompanyMapSupplierOwnerClass))]
    [KnownType(typeof(CompanyMapUserGroupCompanyClass))]
    [KnownType(typeof(CollectionBase))]
	public abstract class CompanyMapClass : BaseDataObject
	{
	    private const string SchemaPrefix = "map.";

		protected virtual string MappingTableName
		{
			get
			{
				return string.Empty;
			}
		}

		protected virtual string MappingTablePrimaryKeyColumnName
		{
			get
			{
				return string.Empty;
			}
		}

		protected virtual string MappingTableAssignedToGuidColumnName
		{
			get
			{
				return string.Empty;
			}
		}

		protected virtual string MappingTableAssignedGuidColumnName
		{
			get
			{
				return string.Empty;
			}
		}

		public string QualifiedMappingTableName
		{
			get
			{
				return SchemaPrefix + this.MappingTableName;
			}
		}

		public static string GetMappingTableName(COMPANY_MAP_TYPE companyMapType)
        {
            switch (companyMapType)
            {
                case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
                    return CompanyMapLoadOwnerManagerClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                    return CompanyMapShipperOwnerClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                    return CompanyMapBillToShipperClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                    return CompanyMapShipToBillToClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP:
                    return CompanyMapAuthorizedCarrierClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP:
                    return CompanyMapLoadIdShipToClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP:
                    return CompanyMapUserGroupCompanyClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP:
                    return CompanyMapCompanyGroupCompanyClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPTO_MAP:
                    return CompanyMapFootNoteShipToClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPPER_MAP:
                    return CompanyMapFootNoteShipperClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                    return CompanyMapSupplierOwnerClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP:
                    return CompanyMapOffloadIdSupplierClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.OFFLOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
                    return CompanyMapOffloadOwnerManagerClass.ClassMappingTableName;
                case COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY:
                    return CompanyMapPersonAssignedCompanyClass.ClassMappingTableName;
                default:
                    return "Unknown";
            }
        }

        public static string GetMappingTablePrimaryKeyColumnName(COMPANY_MAP_TYPE companyMapType)
		{
			switch (companyMapType)
			{
                case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
                    return CompanyMapLoadOwnerManagerClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                    return CompanyMapShipperOwnerClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                    return CompanyMapBillToShipperClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                    return CompanyMapShipToBillToClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP:
                    return CompanyMapAuthorizedCarrierClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP:
                    return CompanyMapLoadIdShipToClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP:
                    return CompanyMapUserGroupCompanyClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP:
                    return CompanyMapCompanyGroupCompanyClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPTO_MAP:
                    return CompanyMapFootNoteShipToClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPPER_MAP:
                    return CompanyMapFootNoteShipperClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                    return CompanyMapSupplierOwnerClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP:
                    return CompanyMapOffloadIdSupplierClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.OFFLOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
                    return CompanyMapOffloadOwnerManagerClass.ClassMappingTablePrimaryKeyColumnName;
                case COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY:
                    return CompanyMapPersonAssignedCompanyClass.ClassMappingTablePrimaryKeyColumnName;
                default:
                    return "Unknown";
            }
        }

		public static string GetMappingTableAssignedToGuidColumnName(COMPANY_MAP_TYPE companyMapType)
		{
			switch (companyMapType)
			{
                case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
                    return CompanyMapLoadOwnerManagerClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                    return CompanyMapShipperOwnerClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                    return CompanyMapBillToShipperClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                    return CompanyMapShipToBillToClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP:
                    return CompanyMapAuthorizedCarrierClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP:
                    return CompanyMapLoadIdShipToClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP:
                    return CompanyMapUserGroupCompanyClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP:
                    return CompanyMapCompanyGroupCompanyClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPTO_MAP:
                    return CompanyMapFootNoteShipToClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPPER_MAP:
                    return CompanyMapFootNoteShipperClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                    return CompanyMapSupplierOwnerClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP:
                    return CompanyMapOffloadIdSupplierClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.OFFLOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
                    return CompanyMapOffloadOwnerManagerClass.ClassMappingTableAssignedToGuidColumnName;
                case COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY:
                    return CompanyMapPersonAssignedCompanyClass.ClassMappingTableAssignedToGuidColumnName;
                default:
                    return "Unknown";
            }
        }

		public static string GetMappingTableAssignedGuidColumnName(COMPANY_MAP_TYPE companyMapType)
		{
			switch (companyMapType)
			{
                case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
                    return CompanyMapLoadOwnerManagerClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                    return CompanyMapShipperOwnerClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                    return CompanyMapBillToShipperClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                    return CompanyMapShipToBillToClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP:
                    return CompanyMapAuthorizedCarrierClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP:
                    return CompanyMapLoadIdShipToClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP:
                    return CompanyMapUserGroupCompanyClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP:
                    return CompanyMapCompanyGroupCompanyClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPTO_MAP:
                    return CompanyMapFootNoteShipToClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPPER_MAP:
                    return CompanyMapFootNoteShipperClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                    return CompanyMapSupplierOwnerClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP:
                    return CompanyMapOffloadIdSupplierClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.OFFLOAD_MAX_COMPANY_MAP_TYPE:
                    return "Unknown";
                case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
                    return CompanyMapOffloadOwnerManagerClass.ClassMappingTableAssignedGuidColumnName;
                case COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY:
                    return CompanyMapPersonAssignedCompanyClass.ClassMappingTableAssignedGuidColumnName;
                default:
                    return "Unknown";
            }
        }

        public static CompanyMapClass CreateCompanyMap(COMPANY_MAP_TYPE companyMapType)
        {
            switch (companyMapType)
            {
                case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
                    return new CompanyMapLoadOwnerManagerClass();
                case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                    return new CompanyMapShipperOwnerClass();
                case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                    return new CompanyMapBillToShipperClass();
                case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                    return new CompanyMapShipToBillToClass();
                case COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP:
                    return new CompanyMapAuthorizedCarrierClass();
                case COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP:
                    return new CompanyMapLoadIdShipToClass();
                case COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP:
                    return new CompanyMapUserGroupCompanyClass();
                case COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP:
                    return new CompanyMapCompanyGroupCompanyClass();
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPTO_MAP:
                    return new CompanyMapFootNoteShipToClass();
                case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPPER_MAP:
                    return new CompanyMapFootNoteShipperClass();
                case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                    return new CompanyMapSupplierOwnerClass();
                case COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP:
                    return new CompanyMapOffloadIdSupplierClass();
                case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
                    return new CompanyMapOffloadOwnerManagerClass();
                case COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY:
                    return new CompanyMapPersonAssignedCompanyClass();
                default:
                    throw new ArgumentException("Invalid Company Map Type");
            }
        }

        public static CompanyMapClass CreateCompanyMap(COMPANY_MAP_TYPE companyMapType, object o)
        {
			CompanyMapClass companyMap = CreateCompanyMap(companyMapType);

			DataSet set = o as DataSet;
            if (set != null)
            {
                DataTable table = set.Tables[0];
                if (table.Rows.Count == 0)
                {
                    return companyMap;
                }

                DataRow row = table.Rows[0];

                companyMap.IdentityGuid = DataObject.getValue<Guid>(row[GetMappingTablePrimaryKeyColumnName(companyMap.Type)], Guid.Empty);
                companyMap.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
                companyMap.AssignedToGuid = DataObject.getValue<Guid>(row[GetMappingTableAssignedToGuidColumnName(companyMap.Type)], Guid.Empty);
                companyMap.AssignedGuid = DataObject.getValue<Guid>(row[GetMappingTableAssignedGuidColumnName(companyMap.Type)], Guid.Empty);
                companyMap.MapID = DataObject.getValue<string>(row["ID"], "");
                companyMap.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
                companyMap.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
                companyMap.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], companyMap._CreatedDate);
                companyMap.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
                companyMap.AssignedToID = DataObject.getValue<string>(row["AssignedToID"], "");
                companyMap.AssignedID = DataObject.getValue<string>(row["AssignedID"], "");
                companyMap.LockedOut = DataObject.getValue<bool>(row["LockedOut"], false);

                if (companyMap.Type == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
                {
                    companyMap.AssignedName = DataObject.getValue<string>(row["AssignedName"], "");
                    companyMap.AssignedAddress = DataObject.getValue<string>(row["AssignedAddress"], "");
                    companyMap.AssignedCity = DataObject.getValue<string>(row["AssignedCity"], "");
                    companyMap.AssignedState = DataObject.getValue<string>(row["AssignedState"], "");
                    companyMap.AssignedToName = DataObject.getValue<string>(row["AssignedToName"], "");
                    companyMap.AssignedToAddress = DataObject.getValue<string>(row["AssignedToAddress"], "");
                    companyMap.AssignedToCity = DataObject.getValue<string>(row["AssignedToCity"], "");
                    companyMap.AssignedToState = DataObject.getValue<string>(row["AssignedToState"], "");
                }

                if (companyMap.Type == COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP
                  || companyMap.Type == COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP
                  || companyMap.Type == COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP
                  || companyMap.Type == COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP
                  || companyMap.Type == COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP
                  || companyMap.Type == COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
                {
                    companyMap.AssignedName = DataObject.getValue<string>(row["AssignedName"], "");
                    companyMap.AssignedAddress = DataObject.getValue<string>(row["AssignedAddress"], "");
                    companyMap.AssignedCity = DataObject.getValue<string>(row["AssignedCity"], "");
                    companyMap.AssignedState = DataObject.getValue<string>(row["AssignedState"], "");

                    string managerID;
                    string ownerID;
                    string shipperID;

                    switch (companyMap.Type)
                    {
                        case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                            managerID = DataObject.getValue<string>(row["ManagerID"], "");
                            ownerID = DataObject.getValue<string>(row["OwnerID"], "");
                            shipperID = DataObject.getValue<string>(row["ShipperID"], "");
                            companyMap.AssignedToID = managerID + "->" + ownerID + "->" + shipperID + "->" + DataObject.getValue<string>(row["BillToID"], "");
                            break;
                        case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                            managerID = DataObject.getValue<string>(row["ManagerID"], "");
                            ownerID = DataObject.getValue<string>(row["OwnerID"], "");
                            shipperID = DataObject.getValue<string>(row["ShipperID"], "");
                            companyMap.AssignedToID = managerID + "->" + ownerID + "->" + shipperID;
                            break;
                        case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                        case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                            managerID = DataObject.getValue<string>(row["ManagerID"], "");
                            ownerID = DataObject.getValue<string>(row["OwnerID"], "");
                            companyMap.AssignedToID = managerID + "->" + ownerID;
                            break;
                        case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
                        case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
                            managerID = DataObject.getValue<string>(row["ManagerID"], "");
                            companyMap.AssignedToID = managerID;
                            break;
                    }

                }

                // Special Case
                if ((companyMap.Type == COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP || companyMap.Type == COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP || companyMap.Type == COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP)
                  && companyMap.AssignedGuid == Guid.Empty)
				{
					companyMap.AssignedID = "{All}";
				}

				if (companyMap.Type == COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY)
                {
                    companyMap.AssignedName = DataObject.getValue<string>(row["AssignedName"], "");
                    companyMap.AssignedAddress = DataObject.getValue<string>(row["AssignedAddress"], "");
                    companyMap.AssignedCity = DataObject.getValue<string>(row["AssignedCity"], "");
                    companyMap.AssignedState = DataObject.getValue<string>(row["AssignedState"], "");
                    companyMap.AssignedToFirstName = DataObject.getValue<string>(row["AssignedToFirstName"], "");
                    companyMap.AssignedToMiddleName = DataObject.getValue<string>(row["AssignedToMiddleName"], "");
                    companyMap.AssignedToLastName = DataObject.getValue<string>(row["AssignedToLastName"], "");
                }
            }

            return companyMap;
        }

        public static CompanyMapClass CreateCompanyMap(XmlNode node)
        {
            CompanyMapClass companyMap = null;

            if (node != null)
            {
                if (node.Name == "AuthorizedCarrier")
                {
                    companyMap = CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
                    companyMap.AssignedID = node.Attributes?["ID"].Value;
                }
                else if (node.Name == "CompanyGroup")
                {
                    companyMap = CreateCompanyMap(COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP);
                    companyMap.AssignedID = node.Attributes?["ID"].Value;
                }
                else if (node.Name == "UserGroup")
                {
                    companyMap = CreateCompanyMap(COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);
                    companyMap.AssignedID = node.Attributes?["ID"].Value;
                }
                else if (node.Name == "AuthorizedCustomer")
                {
                    companyMap = CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
                    companyMap.AssignedToID = node.Attributes?["ID"].Value;
                }
                else
                {
                    throw new Exception("Invalid CompanyMap Type");
                }

                companyMap.AssignedID = node.Attributes?["ID"].Value;
            }

            return companyMap;
        }

        [DataMember]
		[XmlIgnore]
		public Guid AssignedToGuid;
		[DataMember]
		[XmlIgnore]
		public Guid AssignedGuid;

	    [DataMember]
	    public virtual COMPANY_MAP_TYPE Type
	    {
	        get
	        {
	            return COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE;
	        }
	        // ReSharper disable once ValueParameterNotUsed
	        set
	        {
	        }
	    }

        [DataMember]
		public string MapID;

		// Items resulting from join
		[EntityImportExport("TOID*", 130, "AssignedToID")]
		[DataMember]
		public string AssignedToID;
		[EntityImportExport("ID*", 130, "AssignedID")]
		[DataMember]
		public string AssignedID;
		[DataMember]
		[XmlIgnore]
		public bool LockedOut;
		[DataMember]
		[XmlIgnore]
		public string AssignedToName;
		[DataMember]
		[XmlIgnore]
		public string AssignedToAddress;
		[DataMember]
		[XmlIgnore]
		public string AssignedToCity;
		[DataMember]
		[XmlIgnore]
		public string AssignedToState;
		[DataMember]
		[XmlIgnore]
		public string AssignedName;
		[DataMember]
		[XmlIgnore]
		public string AssignedAddress;
		[DataMember]
		[XmlIgnore]
		public string AssignedCity;
		[DataMember]
		[XmlIgnore]
		public string AssignedState;
        [DataMember]
        [XmlIgnore]
        public string AssignedToFirstName;
        [DataMember]
        [XmlIgnore]
        public string AssignedToMiddleName;
        [DataMember]
        [XmlIgnore]
        public string AssignedToLastName;

		public override string ID
		{
			get
			{
				return this.AssignedToID + " - " + this.AssignedID;
			}
			set
			{
				base.ID = value;
			}
		}

		protected virtual string SelectClause
		{
			get
			{
				return "Unknown";
			}
		}

		protected CompanyMapClass()
		{
            this.AssignedToGuid = Guid.Empty;
            this.AssignedGuid = Guid.Empty;
            this.MapID = "";
            this.AssignedToID = "";
            this.AssignedID = "";
            this.LockedOut = false;
            this.AssignedToName = "";
            this.AssignedToAddress = "";
            this.AssignedToCity = "";
            this.AssignedToState = "";
            this.AssignedName = "";
            this.AssignedAddress = "";
            this.AssignedCity = "";
            this.AssignedState = "";
            this.AssignedToFirstName = "";
            this.AssignedToMiddleName = "";
            this.AssignedToLastName = "";
		}

        public static string TypeID(COMPANY_MAP_TYPE companyType)
		{
			switch (companyType)
			{
				case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
					return "Owner";
				case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
					return "Shipper";
				case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
					return "Bill To";
				case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
					return "Ship To";
				case COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP:
					return "Authorized Carrier";
				case COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP:
					return "User Group Company";
				case COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP:
					return "Company Group Company";
				case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPTO_MAP:
					return "Footnote";
				case COMPANY_MAP_TYPE.FOOT_NOTE_SHIPPER_MAP:
					return "Footnote";
				case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
					return "Supplier";
                case COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY:
                    return "Person Assigned to Company";
				default:
					return "Undefined";
			}
		}

        public override void Load(object O)
        {
			this.Reset();

            if (typeof(DataSet).IsInstanceOfType(O))
            {
                DataSet Set = (DataSet)O;

                DataTable Table = Set.Tables[0];
                if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				this.IdentityGuid = DataObject.getValue<Guid>(Row[GetMappingTablePrimaryKeyColumnName(this.Type)], Guid.Empty);
				this.SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				this.AssignedToGuid = DataObject.getValue<Guid>(Row[GetMappingTableAssignedToGuidColumnName(this.Type)], Guid.Empty);
				this.AssignedGuid = DataObject.getValue<Guid>(Row[GetMappingTableAssignedGuidColumnName(this.Type)], Guid.Empty);
				this.MapID = DataObject.getValue<string>(Row["ID"], "");
				this.CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				this.CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				this.UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
				this.UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				this.AssignedToID = DataObject.getValue<string>(Row["AssignedToID"], "");
				this.AssignedID = DataObject.getValue<string>(Row["AssignedID"], "");
				this.LockedOut = DataObject.getValue<bool>(Row["LockedOut"], false);

                if (this.Type == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
                {
					this.AssignedName = DataObject.getValue<string>(Row["AssignedName"], "");
					this.AssignedAddress = DataObject.getValue<string>(Row["AssignedAddress"], "");
					this.AssignedCity = DataObject.getValue<string>(Row["AssignedCity"], "");
					this.AssignedState = DataObject.getValue<string>(Row["AssignedState"], "");
					this.AssignedToName = DataObject.getValue<string>(Row["AssignedToName"], "");
					this.AssignedToAddress = DataObject.getValue<string>(Row["AssignedToAddress"], "");
					this.AssignedToCity = DataObject.getValue<string>(Row["AssignedToCity"], "");
					this.AssignedToState = DataObject.getValue<string>(Row["AssignedToState"], "");
                }

                if (this.Type == COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP
                  || this.Type == COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP
                  || this.Type == COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP
                  || this.Type == COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP
                  || this.Type == COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP
                  || this.Type == COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
                {
					this.AssignedName = DataObject.getValue<string>(Row["AssignedName"], "");
					this.AssignedAddress = DataObject.getValue<string>(Row["AssignedAddress"], "");
					this.AssignedCity = DataObject.getValue<string>(Row["AssignedCity"], "");
					this.AssignedState = DataObject.getValue<string>(Row["AssignedState"], "");
					string managerID;
					string ownerID;
					string shipperID;
					if (this.Type == COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
					{
						managerID = DataObject.getValue<string>(Row["ManagerID"], "");
						ownerID = DataObject.getValue<string>(Row["OwnerID"], "");
						shipperID = DataObject.getValue<string>(Row["ShipperID"], "");
						this.AssignedToID = managerID + "->" + ownerID + "->" + shipperID + "->" + DataObject.getValue<string>(Row["BillToID"], "");
					}

					else if (this.Type == COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
					{
						managerID = DataObject.getValue<string>(Row["ManagerID"], "");
						ownerID = DataObject.getValue<string>(Row["OwnerID"], "");
						shipperID = DataObject.getValue<string>(Row["ShipperID"], "");
						this.AssignedToID = managerID + "->" + ownerID + "->" + shipperID;
					}

					else if (this.Type == COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP
					   || this.Type == COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP)
					{
						managerID = DataObject.getValue<string>(Row["ManagerID"], "");
						ownerID = DataObject.getValue<string>(Row["OwnerID"], "");
						this.AssignedToID = managerID + "->" + ownerID;
					}


					else if (this.Type == COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP
					   || this.Type == COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
					{
						managerID = DataObject.getValue<string>(Row["ManagerID"], "");
						this.AssignedToID = managerID;
					}

				}

                // Special Case
                if ((this.Type == COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP ||
				  this.Type == COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP ||
				  this.Type == COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP)
                  && this.AssignedGuid == Guid.Empty)
				{
					this.AssignedID = "{All}";
				}
			}

            else if (typeof(XmlNode).IsInstanceOfType(O))
            {
                XmlNode Node = (XmlNode)O;

                if (Node.Name == "AuthorizedCarrier")
                {
					this.Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP;
					this.AssignedID = Node.Attributes["ID"].Value;
                }
                else if (Node.Name == "CompanyGroup")
                {
					this.Type = COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP;
					this.AssignedID = Node.Attributes["ID"].Value;
                }
                else if (Node.Name == "UserGroup")
                {
					this.Type = COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP;
					this.AssignedID = Node.Attributes["ID"].Value;
                }
                else if (Node.Name == "AuthorizedCustomer")
                {
					this.Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP;
					this.AssignedToID = Node.Attributes["ID"].Value;
                }
                else
				{
					throw new Exception("Invalid CompanyMap Type");
				}

				this.AssignedID = Node.Attributes["ID"].Value;
            }

            else
			{
				throw new Exception("Load Error - Invalid Object Type : " + O.GetType().ToString());
			}
		}

		public virtual void SelectSQLMinimal(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public string AssignedCompanyToolTip
		{
			get
			{
				string toolTip;
                if (this.Type == COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY)
                {
                    toolTip = this.AssignedToFirstName + " " + this.AssignedToMiddleName + " " + this.AssignedToLastName;
                }
                else
                {
                    toolTip = string.IsNullOrEmpty(this.AssignedName) == false ? this.AssignedName : this.AssignedID;

					if (string.IsNullOrEmpty(this.AssignedAddress) == false)
                    {
                        toolTip += ", " + this.AssignedAddress;
                    }
                    if (string.IsNullOrEmpty(this.AssignedCity) == false)
                    {
                        toolTip += ", " + this.AssignedCity;
                    }
                    if (string.IsNullOrEmpty(this.AssignedState) == false)
                    {
                        toolTip += ", " + this.AssignedState;
                    }
                }
				return toolTip;
			}
		}

		//public override void Store(object o)
		//{
		//    if (o == null)
		//    {
		//        throw new ArgumentNullException(nameof(o));
		//    }

		//    var node = o as XmlNode;
		//    if (node != null)
		//    {
		//        XmlAttribute attribute = node.OwnerDocument?.CreateAttribute("ID");
		//        if (node.Name == "AuthorizedCustomer")
		//        {
		//            attribute.Value = this.AssignedToID;
		//        }
		//        else
		//        {
		//            attribute.Value = this.AssignedID;
		//        }

		//        node.Attributes?.Append(attribute);
		//    }
		//    else
		//    {
		//        throw new Exception("Store Error - Invalid Object Type : " + o.GetType());
		//    }
		//}

		public string AssignedToolTip
		{
			get
			{
				string toolTip = string.IsNullOrEmpty(this.AssignedName) == false ? this.AssignedName : this.AssignedID;
				if (string.IsNullOrEmpty(this.AssignedAddress) == false)
			    {
			        toolTip += ", " + this.AssignedAddress;
			    }

			    if (string.IsNullOrEmpty(this.AssignedCity) == false)
			    {
			        toolTip += ", " + this.AssignedCity;
			    }

			    if (string.IsNullOrEmpty(this.AssignedState) == false)
			    {
			        toolTip += ", " + this.AssignedState;
			    }

				return toolTip;
			}
		}

		public string AssignedToToolTip
		{
			get
			{
				string toolTip = string.IsNullOrEmpty(this.AssignedToName) == false ? this.AssignedToName : this.AssignedToID;
				if (string.IsNullOrEmpty(this.AssignedToAddress) == false)
			    {
			        toolTip += ", " + this.AssignedToAddress;
			    }

			    if (string.IsNullOrEmpty(this.AssignedToCity) == false)
			    {
			        toolTip += ", " + this.AssignedToCity;
			    }

			    if (string.IsNullOrEmpty(this.AssignedToState) == false)
			    {
			        toolTip += ", " + this.AssignedToState;
			    }

				return toolTip;
			}
		}

		public virtual void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO " + this.MappingTableName +
				"(SiteGuid," + 
                this.MappingTableAssignedToGuidColumnName + "," + 
                this.MappingTableAssignedGuidColumnName + "," +
				"ID," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," + this.MappingTablePrimaryKeyColumnName+
				") VALUES (" +
				"@SiteGuid," +
				"@AssignedToGuid," +
				"@AssignedGuid," +
				"@MapID," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@"+ this.MappingTablePrimaryKeyColumnName+
				")";

			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedToGuid", this.AssignedToGuid, true)); // true means replace Guid.Empty with NULL
			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedGuid", this.AssignedGuid, true));  // true means replace Guid.Empty with NULL
			cmd.Parameters.AddWithValue("@MapID", this.MapID);
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this.CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
			cmd.Parameters.AddWithValue("@"+GetMappingTablePrimaryKeyColumnName(this.Type), this._IdentityGuid);
		}

		public virtual void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE " + this.MappingTableName + " " +
				"SET " + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid," + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid," +
				"ID = @MapID," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy " +
				"WHERE " + this.MappingTablePrimaryKeyColumnName + "= @IdentityGuid";

			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedToGuid", this.AssignedToGuid, true)); // true means replace Guid.Empty with NULL
			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedGuid", this.AssignedGuid, true));  // true means replace Guid.Empty with NULL
			cmd.Parameters.AddWithValue("@MapID", this.MapID);
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
			cmd.Parameters.AddWithValue("@IdentityGuid", this._IdentityGuid);
		}

		public virtual void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + this.MappingTableName + " WHERE " + this.MappingTablePrimaryKeyColumnName + " = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
		}

		public virtual void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
				" FROM " + this.MappingTableName + " " + SQLUpdateLock(bInTransaction) +
				" WHERE " + this.MappingTablePrimaryKeyColumnName + " = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
			if (this.Type == COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP
				|| this.Type == COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP
				|| this.Type == COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP
				|| this.Type == COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP
				|| this.Type == COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP
				|| this.Type == COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP
				|| this.Type == COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP
				|| this.Type == COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP
				|| this.Type == COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP
                || this.Type == COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY)
			{
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			}
		}

		public virtual void SelectByGuidsAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
		{
			cmd.CommandText = this.SelectClause +
				" FROM " + this.MappingTableName + " " + SQLUpdateLock(bInTransaction) +
				" WHERE SiteGuid = @SiteGuid" +
				" AND " + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid" +
				" AND " + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid";

			cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
			cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public virtual void SelectByTypeAndMapIdsql(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
		{
			cmd.CommandText = this.SelectClause +
				" FROM " + this.MappingTableName + " " + SQLUpdateLock(bInTransaction) +
				" WHERE " + this.MappingTableName + ".SiteGuid = @SiteGuid" +
				" AND " + this.MappingTableName + ".ID = @MapID";

			cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MapID"].Value = this.MapID;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

        public virtual void SelectIdentityGuidByTypeAndMapIdsql(SqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public virtual void EnumerateByAssignedToGuidAndTypeSQL(SqlCommand cmd, bool bInTransaction,bool skipSiteGuid = false)
		{
			switch (this.Type)
			{
                // Site Index is not evaluated in USER_GROUP_COMPANY_MAP as the mapping is valid for any
                // site to which the group is assigned.
				case COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP:
					cmd.CommandText = "SELECT ctocg.*,tas.ID AS AssignedToID,c.ID AS AssignedID,c.LockedOut AS LockedOut " +
					"FROM map.tblCompanyCompanyToCompanyGroup ctocg " +
					"INNER JOIN [erv].[udf_GetCompanyRecordVersions](@SiteGuid) rvCompanies ON ctocg.CompanyGuid = rvCompanies.MasterRecordGuid " +
					"LEFT JOIN tblApplicationString tas ON tas.ApplicationStringGuid =  ctocg.ApplicationStringGuid " +
					"LEFT JOIN tblCompanies c ON c.CompanyGuid = rvCompanies.CompanyGuid " +
					"WHERE ctocg.ApplicationStringGuid = @AssignedToGuid ORDER BY ctocg.ID ASC";
					break;
				default:
					cmd.CommandText = this.SelectClause +
						 " FROM " + this.MappingTableName + " " + SQLUpdateLock(bInTransaction) +
						 " WHERE SiteGuid = @SiteGuid" +
						 " AND " + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid" +
						 " ORDER BY " + this.MappingTableName + ".ID ASC";
					break;
			}

			cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public virtual void EnumerateByAssignedGuidAndTypeSQL(SqlCommand cmd, SecurityClass security, bool skipSiteGuid = false)
		{
			if (this.Type == COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP)
			{
				GroupClass group = new GroupClass();
				cmd.CommandText = @"SELECT ctg.*, g.GroupID AS AssignedToID, c.ID AS AssignedID, c.LockedOut
					FROM [map].[tblCompanyCompanyToUserGroup] ctg

					INNER JOIN tblGroups g ON g.GroupGuid = ctg.GroupGuid
					LEFT JOIN map.tblEntityCompanyToSite ects ON ects.CompanyGuid = ctg.CompanyGuid AND ects.SiteGuid = ctg.SiteGuid
					LEFT JOIN tblCompanies c ON c.[_MasterRecordGuid] = ctg.CompanyGuid
					WHERE ctg.SiteGuid = @SiteGuid AND (c.CompanyGuid = @AssignedGuid OR ctg.CompanyGuid IS NULL) ORDER BY AssignedToID ASC";
			}

			else if (this.Type == COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP)
			{
				cmd.CommandText = this.SelectClause +
				  " FROM " + this.MappingTableName +
				  " WHERE @SiteGuid" +
				  " IN (SELECT SiteGuid FROM map.tblEntityCompanyGroupToSite";

				if (this.AssignedToGuid != Guid.Empty)
				{
					cmd.CommandText += " AND " + this.MappingTablePrimaryKeyColumnName + " = @AssignedToGuid";

					cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
				}

				cmd.CommandText += ") AND " + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid " +
					" ORDER BY " + this.MappingTableName + ".ID ASC";
			}

			else
			{
				cmd.CommandText = this.SelectClause +
				  " FROM " + this.MappingTableName +
				  " WHERE SiteGuid = @SiteGuid " +
				  " AND " + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid" +
				  " ORDER BY " + this.MappingTableName + ".ID ASC";
			}

			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public virtual void EnumerateByTypeSQL(SqlCommand cmd, bool skipSiteGuid = false)
		{
			string mappingTableName = this.MappingTableName;

			switch (this.Type)
			{
				case COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP:
					cmd.CommandText = this.SelectClause +
						" FROM " + mappingTableName + "," +
						" (SELECT map.tblCompanyShipToToBillTo.CompanyShipToToBillToGuid FROM map.tblCompanyShipToToBillTo, tblCompanies, map.tblEntityCompanyToSite" +
						" WHERE map.tblCompanyShipToToBillTo.CompanyBillToToShipperGuid = tblCompanies.CompanyGuid" +
						" AND map.tblEntityCompanyToSite.SiteGuid = @SiteGuid" +
						" AND map.tblEntityCompanyToSite.CompanyGuid = tblCompanies.CompanyGuid) AS tblCUSTOMER_SHIPTO_BILLTO_MAP" +
						" WHERE " + mappingTableName + ".SiteGuid = @SiteGuid" +
						" AND " + mappingTableName + "." + this.MappingTableAssignedToGuidColumnName + " = tblCUSTOMER_SHIPTO_BILLTO_MAP.CompanyShipToToBillToGuid" +
						" ORDER BY " + mappingTableName + ".ID ASC";
					break;
				case COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP:
					cmd.CommandText = this.SelectClause +
						" FROM " + mappingTableName + ", tblGroups, map.tblEntityUserGroupToSite" +
						" WHERE " + mappingTableName + "." + this.MappingTableAssignedGuidColumnName + " = tblGroups.GroupGuid" +
						" AND map.tblEntityUserGroupToSite.SiteGuid = @SiteGuid" +
						" AND map.tblEntityUserGroupToSite.GroupGuid = tblGroups.GroupGuid" +
						" ORDER BY " + mappingTableName + ".ID ASC";
					break;
				case COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP:
					cmd.CommandText = this.SelectClause +
						" FROM " + mappingTableName + ", tblCompanies, map.tblEntityCompanyToSite" +
						" WHERE " + mappingTableName + "." + this.MappingTableAssignedGuidColumnName + " = tblCompanies.CompanyGuid" +
						" AND map.tblEntityCompanyToSite.SiteGuid = @SiteGuid" +
						" AND map.tblEntityCompanyToSite.CompanyGuid = tblCompanies.CompanyGuid" +
						" ORDER BY AssignedToID, AssignedID";
					break;
				case COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP:
					cmd.CommandText = this.SelectClause +
						" FROM " + mappingTableName + "," +
						" (SELECT map.tblCompanySupplierToOwner.CompanySupplierToOwnerGuid FROM map.tblCompanySupplierToOwner, tblCompanies, map.tblEntityCompanyToSite" +
						" WHERE map.tblCompanySupplierToOwner.CompanyGuid = tblCompanies.CompanyGuid" +
						" AND map.tblEntityCompanyToSite.SiteGuid = @SiteGuid" +
						" AND map.tblEntityCompanyToSite.CompanyGuid = tblCompanies.CompanyGuid) AS tblCUSTOMER_SUPPLIER_OWNER_MAP" +
						" WHERE " + mappingTableName + ".SiteGuid = @SiteGuid" +
						" AND " + mappingTableName + "." + this.MappingTableAssignedGuidColumnName + " = tblCUSTOMER_SUPPLIER_OWNER_MAP.CompanySupplierToOwnerGuid" +
						" ORDER BY " + mappingTableName + ".ID ASC";
					break;
				default:
					cmd.CommandText = this.SelectClause +
						" FROM " + mappingTableName + ", tblCompanies, map.tblEntityCompanyToSite" +
						" WHERE " + mappingTableName + ".SiteGuid = @SiteGuid" +
						" AND " + mappingTableName + "." + this.MappingTableAssignedGuidColumnName + " = tblCompanies.CompanyGuid" +
						" AND map.tblEntityCompanyToSite.SiteGuid = @SiteGuid" +
						" AND map.tblEntityCompanyToSite.CompanyGuid = tblCompanies.CompanyGuid" +
						" ORDER BY AssignedToID, AssignedID";
					break;
			}

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

        public override void Reset()
        {
            base.Reset();
            this.AssignedToGuid = Guid.Empty;
            this.AssignedGuid = Guid.Empty;
            this.MapID = string.Empty;
            this.AssignedToID = string.Empty;
            this.AssignedID = string.Empty;
            this.LockedOut = false;
            this.AssignedToName = string.Empty;
            this.AssignedToAddress = string.Empty;
            this.AssignedToCity = string.Empty;
            this.AssignedToState = string.Empty;
            this.AssignedName = string.Empty;
            this.AssignedAddress = string.Empty;
            this.AssignedCity = string.Empty;
            this.AssignedState = string.Empty;
            this.AssignedToFirstName = string.Empty;
            this.AssignedToMiddleName = string.Empty;
            this.AssignedToLastName = string.Empty;
        }
    }
}
