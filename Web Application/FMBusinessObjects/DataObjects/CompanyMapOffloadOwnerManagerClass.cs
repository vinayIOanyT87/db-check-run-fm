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
    [EntityImportExportWorksheet("COMPANYOFFLOADOWNERMANAGERMAPS")]
    class CompanyMapOffloadOwnerManagerClass : CompanyMapClass
    {
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyOffLoadOwnerToManager";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyOffLoadOwnerToManagerGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "AssignedToCompanyGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "CompanyGuid";
        protected override string MappingTableName => ClassMappingTableName;
        protected override string MappingTablePrimaryKeyColumnName => ClassMappingTablePrimaryKeyColumnName;
        protected override string MappingTableAssignedToGuidColumnName => ClassMappingTableAssignedToGuidColumnName;
        protected override string MappingTableAssignedGuidColumnName => ClassMappingTableAssignedGuidColumnName;

        public override COMPANY_MAP_TYPE Type
        {
            get
            {
                return COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        protected override string SelectClause => "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
                                  "INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
                                  "SELECT " + this.MappingTableName + ".*," +
                                  "(SELECT tblCompanies32.ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies32 WHERE tblCompanies32._MasterRecordGuid = " + this.MappingTableName + ".AssignedToCompanyGuid ) AS AssignedToID," +
                                  "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies33 WHERE tblCompanies33._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedID," +
                                  "(SELECT LockedOut FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies34 WHERE tblCompanies34._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS LockedOut," +
                                  "(SELECT Name FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies35 WHERE tblCompanies35._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedName," +
                                  "(SELECT Address1 FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies36 WHERE tblCompanies36._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedAddress," +
                                  "(SELECT City FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies37 WHERE tblCompanies37._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedCity," +
                                  "(SELECT State FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies38 WHERE tblCompanies38._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedState, " +
                                  "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies39 WHERE tblCompanies39._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS OwnerID," +
                                  "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies40 WHERE tblCompanies40._MasterRecordGuid = " + this.MappingTableName + "." + this.MappingTableAssignedToGuidColumnName + ") AS ManagerID ";

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
                this.AssignedToID = managerID;
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
