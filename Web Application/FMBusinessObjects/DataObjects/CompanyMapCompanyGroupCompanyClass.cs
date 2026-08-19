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
    [EntityImportExportWorksheet("COMPANYCOMPANYGROUPCOMPANYMAPS")]
    class CompanyMapCompanyGroupCompanyClass : CompanyMapClass
    {
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyCompanyToCompanyGroup";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyCompanyToCompanyGroupGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "ApplicationStringGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "CompanyGuid";
        protected override string MappingTableName => ClassMappingTableName;
        protected override string MappingTablePrimaryKeyColumnName => ClassMappingTablePrimaryKeyColumnName;
        protected override string MappingTableAssignedToGuidColumnName => ClassMappingTableAssignedToGuidColumnName;
        protected override string MappingTableAssignedGuidColumnName => ClassMappingTableAssignedGuidColumnName;

        public override COMPANY_MAP_TYPE Type
        {
            get
            {
                return COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        protected override string SelectClause => "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
                            "INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
                            "SELECT map.tblCompanyCompanyToCompanyGroup.*," +
                            "(SELECT ID FROM tblApplicationString WHERE tblApplicationString.ApplicationStringGuid =  map.tblCompanyCompanyToCompanyGroup.ApplicationStringGuid) AS AssignedToID," +
                            "(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies1 WHERE tblCompanies1._MasterRecordGuid =  map.tblCompanyCompanyToCompanyGroup.CompanyGuid) AS AssignedID," +
                            "(SELECT LockedOut FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies2 WHERE tblCompanies2._MasterRecordGuid =  map.tblCompanyCompanyToCompanyGroup.CompanyGuid) AS LockedOut ";

        public override void Load(object o)
        {
            base.Load(o);
            // RRP- I do not think this is needed since the call above to base is doing the same thing.
            //      Because of the code below an error is being thrown since the base load identified the 
            //      object as a dataset and then on the return it tries to execute the code below and the
            //      object is not a node.
            //var node = o as XmlNode;
            //if (node != null)
            //{
            //    if (node.Name == "AuthorizedCarrier")
            //    {
            //        //this.Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP;
            //        this.AssignedID = node.Attributes?["ID"].Value;
            //    }
            //    else if (node.Name == "CompanyGroup")
            //    {
            //        //this.Type = COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP;
            //        this.AssignedID = node.Attributes?["ID"].Value;
            //    }
            //    else if (node.Name == "UserGroup")
            //    {
            //        //this.Type = COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP;
            //        this.AssignedID = node.Attributes?["ID"].Value;
            //    }
            //    else if (node.Name == "AuthorizedCustomer")
            //    {
            //        //this.Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP;
            //        this.AssignedToID = node.Attributes?["ID"].Value;
            //    }
            //    else
            //    {
            //        throw new Exception("Invalid CompanyMap Type");
            //    }

            //    this.AssignedID = node.Attributes?["ID"].Value;
            //}
            //else
            //{
            //    throw new Exception("Load Error - Invalid Object Type : " + o.GetType());
            //}
        }
    }
}
