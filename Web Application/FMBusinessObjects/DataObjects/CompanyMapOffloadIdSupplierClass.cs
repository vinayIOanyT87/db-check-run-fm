namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
   using System.Data.SqlClient;
   using System.Runtime.Serialization;
   using System.Xml;

   /// <summary>
   /// Summary description for CompanyMapClass.
   /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(COMPANY_MAP_TYPE))]
    [EntityImportExportWorksheet("COMPANYOFFLOADIDSUPPLIERMAPS")]
    class CompanyMapOffloadIdSupplierClass : CompanyMapClass
    {
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyPersonnelToSupplierOwner";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyPersonnelToSupplierOwnerGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "CompanySupplierToOwnerGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "PersonnelGuid";
        protected override string MappingTableName => ClassMappingTableName;
        protected override string MappingTablePrimaryKeyColumnName => ClassMappingTablePrimaryKeyColumnName;
        protected override string MappingTableAssignedToGuidColumnName => ClassMappingTableAssignedToGuidColumnName;
        protected override string MappingTableAssignedGuidColumnName => ClassMappingTableAssignedGuidColumnName;

        public override COMPANY_MAP_TYPE Type
        {
            get
            {
                return COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        protected override string SelectClause => "declare @PersonnelGuidTable TABLE ( PersonnelGuid uniqueidentifier NULL)" + Environment.NewLine +
                                "INSERT INTO @PersonnelGuidTable SELECT PersonnelGuid FROM [erv].[udf_GetPersonnelRecordVersions](@SiteGuid)" + Environment.NewLine +
                                "SELECT map.tblCompanyPersonnelToSupplierOwner.*," +
                                "ID AS AssignedToID," +
                                "(SELECT PersonID FROM (select * from tblPersonnel where tblPersonnel.PersonnelGuid IN (SELECT PersonnelGuid FROM @PersonnelGuidTable)) tblPersonnel1 WHERE tblPersonnel1._MasterRecordGuid = map.tblCompanyPersonnelToSupplierOwner.PersonnelGuid) AS AssignedID," +
                                "(SELECT LockedOut FROM (select * from tblPersonnel where tblPersonnel.PersonnelGuid IN (SELECT PersonnelGuid FROM @PersonnelGuidTable)) tblPersonnel2 WHERE tblPersonnel2._MasterRecordGuid = map.tblCompanyPersonnelToSupplierOwner.PersonnelGuid) AS LockedOut ";

      public override void SelectIdentityGuidByTypeAndMapIdsql(SqlCommand cmd)
      {
         cmd.CommandText =
@"SELECT mptso.CompanyPersonnelToSupplierOwnerGuid
FROM map.tblCompanyPersonnelToSupplierOwner mptso
WHERE mptso.SiteGuid = @SiteGuid
    AND mptso.ID = @MapID";

         cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
         cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

         cmd.Parameters["@MapID"].Value = this.MapID;
         cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
      }

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
