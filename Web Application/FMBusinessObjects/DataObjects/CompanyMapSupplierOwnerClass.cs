namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// Summary description for CompanyMapClass.
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(COMPANY_MAP_TYPE))]
    [EntityImportExportWorksheet("COMPANYSUPPLIEROWNERMAPS")]
    class CompanyMapSupplierOwnerClass : CompanyMapClass
    {
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanySupplierToOwner";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanySupplierToOwnerGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "CompanyOffLoadOwnerToManagerGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "CompanyGuid";
        protected override string MappingTableName => ClassMappingTableName;
        protected override string MappingTablePrimaryKeyColumnName => ClassMappingTablePrimaryKeyColumnName;
        protected override string MappingTableAssignedToGuidColumnName => ClassMappingTableAssignedToGuidColumnName;
        protected override string MappingTableAssignedGuidColumnName => ClassMappingTableAssignedGuidColumnName;

        public override COMPANY_MAP_TYPE Type
        {
            get
            {
                return COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        protected override string SelectClause => "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
                                "INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
                                "SELECT map.tblCompanySupplierToOwner.*," +
                                "NULL AS AssignedToID," + //the Load method sets the AssignedToID based on other values returned by this select clause.
                                "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies41 WHERE tblCompanies41._MasterRecordGuid = map.tblCompanySupplierToOwner.CompanyGuid) AS AssignedID," +
                                "(SELECT LockedOut FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies42 WHERE tblCompanies42._MasterRecordGuid = map.tblCompanySupplierToOwner.CompanyGuid) AS LockedOut," +
                                "(SELECT Name FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies43 WHERE tblCompanies43._MasterRecordGuid = map.tblCompanySupplierToOwner.CompanyGuid) AS AssignedName," +
                                "(SELECT Address1 FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies44 WHERE tblCompanies44._MasterRecordGuid = map.tblCompanySupplierToOwner.CompanyGuid) AS AssignedAddress," +
                                "(SELECT City FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies45 WHERE tblCompanies45._MasterRecordGuid = map.tblCompanySupplierToOwner.CompanyGuid) AS AssignedCity," +
                                "(SELECT State FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies46 WHERE tblCompanies46._MasterRecordGuid = map.tblCompanySupplierToOwner.CompanyGuid) AS AssignedState," +
                                "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies47 WHERE tblCompanies47._MasterRecordGuid = map.tblCompanySupplierToOwner.CompanyGuid) AS SupplierID," +
                                "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies48 WHERE tblCompanies48._MasterRecordGuid IN (SELECT A.CompanyGuid FROM map.tblCompanyOffLoadOwnerToManager A WHERE A.CompanyOffLoadOwnerToManagerGuid = map.tblCompanySupplierToOwner.CompanyOffLoadOwnerToManagerGuid)) AS OwnerID," +
                                "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies49 WHERE tblCompanies49._MasterRecordGuid IN (SELECT A.AssignedToCompanyGuid FROM map.tblCompanyOffLoadOwnerToManager A WHERE A.CompanyOffLoadOwnerToManagerGuid = map.tblCompanySupplierToOwner.CompanyOffLoadOwnerToManagerGuid)) AS ManagerID ";

        public override void Load(object o)
        {
            base.Load(o);
            var set = o as DataSet;
            if (set != null)
            {
                DataTable table = set.Tables[0];
                if (table.Rows.Count == 0)
                {
                    return;
                }

                DataRow row = table.Rows[0];
                this.AssignedName = DataObject.getValue<string>(row["AssignedName"], "");
                this.AssignedAddress = DataObject.getValue<string>(row["AssignedAddress"], "");
                this.AssignedCity = DataObject.getValue<string>(row["AssignedCity"], "");
                this.AssignedState = DataObject.getValue<string>(row["AssignedState"], "");

                var managerID = DataObject.getValue<string>(row["ManagerID"], "");
                var ownerID = DataObject.getValue<string>(row["OwnerID"], "");
                this.AssignedToID = managerID + "->" + ownerID;
            }
            else
            {
                var node = o as XmlNode;
                if (node != null)
                {
                    if (node.Name == "AuthorizedCarrier")
                    {
                        //this.Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP;
                        this.AssignedID = node.Attributes?["ID"].Value;
                    }
                    else if (node.Name == "CompanyGroup")
                    {
                        //this.Type = COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP;
                        this.AssignedID = node.Attributes?["ID"].Value;
                    }
                    else if (node.Name == "UserGroup")
                    {
                        //this.Type = COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP;
                        this.AssignedID = node.Attributes?["ID"].Value;
                    }
                    else if (node.Name == "AuthorizedCustomer")
                    {
                        //this.Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP;
                        this.AssignedToID = node.Attributes?["ID"].Value;
                    }
                    else
                    {
                        throw new Exception("Invalid CompanyMap Type");
                    }

                    this.AssignedID = node.Attributes?["ID"].Value;
                }
                else
                {
                    throw new Exception("Load Error - Invalid Object Type : " + o.GetType());
                }
            }
        }
    }
}
