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
    [EntityImportExportWorksheet("COMPANYUSERGROUPCOMPANYMAPS")]
    class CompanyMapUserGroupCompanyClass : CompanyMapClass
    {
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyCompanyToUserGroup";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyCompanyToUserGroupGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "GroupGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "CompanyGuid";
        protected override string MappingTableName => ClassMappingTableName;
        protected override string MappingTablePrimaryKeyColumnName => ClassMappingTablePrimaryKeyColumnName;
        protected override string MappingTableAssignedToGuidColumnName => ClassMappingTableAssignedToGuidColumnName;
        protected override string MappingTableAssignedGuidColumnName => ClassMappingTableAssignedGuidColumnName;

        public override COMPANY_MAP_TYPE Type
        {
            get
            {
                return COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        protected override string SelectClause => "SELECT map.tblCompanyCompanyToUserGroup.*," +
                             "(SELECT GroupID FROM tblGroups WHERE tblGroups.GroupGuid = map.tblCompanyCompanyToUserGroup.GroupGuid) AS AssignedToID," +
                             "(SELECT ID FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyCompanyToUserGroup.CompanyGuid) AS AssignedID," +
                             "(SELECT LockedOut FROM tblCompanies WHERE tblCompanies.CompanyGuid = map.tblCompanyCompanyToUserGroup.CompanyGuid) AS LockedOut ";

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

                // Special Case
                if (this.AssignedGuid == Guid.Empty)
                {
                    this.AssignedID = "{All}";
                }
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
