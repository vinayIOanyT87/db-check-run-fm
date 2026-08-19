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
    [EntityImportExportWorksheet("COMPANYAUTHORIZEDCARRIERMAPS")]
    public class CompanyMapAuthorizedCarrierClass : CompanyMapClass
    {
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyAuthorizedCarrierToCompany";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyAuthorizedCarrierToCompanyGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "AssignedToCompanyGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "CompanyGuid";
        protected override string MappingTableName => ClassMappingTableName;
        protected override string MappingTablePrimaryKeyColumnName => ClassMappingTablePrimaryKeyColumnName;
        protected override string MappingTableAssignedToGuidColumnName => ClassMappingTableAssignedToGuidColumnName;
        protected override string MappingTableAssignedGuidColumnName => ClassMappingTableAssignedGuidColumnName;

        // We need a public constructor for entity import
        // This should not be used in onter istances
        public CompanyMapAuthorizedCarrierClass() {}
        public override COMPANY_MAP_TYPE Type
        {
            get
            {
                return COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        protected override string SelectClause => "SELECT map.tblCompanyAuthorizedCarrierToCompany.*," +
                             "(SELECT tblCompanies.ID FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.AssignedToCompanyGuid ) AS AssignedToID," +
                             "(SELECT ID FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.CompanyGuid) AS AssignedID," +
                             "(SELECT LockedOut FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.CompanyGuid) AS LockedOut," +
                             "(SELECT Name FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.CompanyGuid) AS AssignedName," +
                             "(SELECT Address1 FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.CompanyGuid) AS AssignedAddress," +
                             "(SELECT City FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.CompanyGuid) AS AssignedCity," +
                             "(SELECT State FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.CompanyGuid) AS AssignedState," +
                             "(SELECT Name FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.AssignedToCompanyGuid) AS AssignedToName," +
                             "(SELECT Address1 FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.AssignedToCompanyGuid) AS AssignedToAddress," +
                             "(SELECT City FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.AssignedToCompanyGuid) AS AssignedToCity," +
                             "(SELECT State FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyAuthorizedCarrierToCompany.AssignedToCompanyGuid) AS AssignedToState ";

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

                // Treat Empty Guid as {All} mapping option
                if (this.AssignedGuid == Guid.Empty)
                {
                    this.AssignedID = "{All}";
                }

                DataRow row = table.Rows[0];

                this.AssignedName = DataObject.getValue<string>(row["AssignedName"], "");
                this.AssignedAddress = DataObject.getValue<string>(row["AssignedAddress"], "");
                this.AssignedCity = DataObject.getValue<string>(row["AssignedCity"], "");
                this.AssignedState = DataObject.getValue<string>(row["AssignedState"], "");
                this.AssignedToName = DataObject.getValue<string>(row["AssignedToName"], "");
                this.AssignedToAddress = DataObject.getValue<string>(row["AssignedToAddress"], "");
                this.AssignedToCity = DataObject.getValue<string>(row["AssignedToCity"], "");
                this.AssignedToState = DataObject.getValue<string>(row["AssignedToState"], "");
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
