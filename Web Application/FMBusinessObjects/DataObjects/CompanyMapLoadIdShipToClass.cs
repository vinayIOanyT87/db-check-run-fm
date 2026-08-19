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
    [EntityImportExportWorksheet("COMPANYLOADIDSHIPTOMAPS")]
    class CompanyMapLoadIdShipToClass : CompanyMapClass
    {
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyPersonnelToShipToBillTo";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyPersonnelToShipToBillToGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "CompanyShipToToBillToGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "PersonnelGuid";
		protected override string MappingTableName
		{
			get
			{
				return ClassMappingTableName;
			}
		}

		protected override string MappingTablePrimaryKeyColumnName
		{
			get
			{
				return ClassMappingTablePrimaryKeyColumnName;
			}
		}

		protected override string MappingTableAssignedToGuidColumnName
		{
			get
			{
				return ClassMappingTableAssignedToGuidColumnName;
			}
		}

		protected override string MappingTableAssignedGuidColumnName
		{
			get
			{
				return ClassMappingTableAssignedGuidColumnName;
			}
		}

		public override COMPANY_MAP_TYPE Type
        {
            get
            {
                return COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

		protected override string SelectClause
		{
			get
			{
				return "declare @PersonnelGuidTable TABLE ( PersonnelGuid uniqueidentifier NULL)" + Environment.NewLine +
"INSERT INTO @PersonnelGuidTable SELECT PersonnelGuid FROM [erv].[udf_GetPersonnelRecordVersions](@SiteGuid)" + Environment.NewLine +
"SELECT map.tblCompanyPersonnelToShipToBillTo.*," +
"ID AS AssignedToID," +
"(SELECT PersonID FROM (select * from tblPersonnel where tblPersonnel.PersonnelGuid IN (SELECT PersonnelGuid FROM @PersonnelGuidTable)) tblPersonnel1 WHERE tblPersonnel1._MasterRecordGuid = map.tblCompanyPersonnelToShipToBillTo.PersonnelGuid) AS AssignedID," +
"(SELECT LockedOut FROM (select * from tblPersonnel where tblPersonnel.PersonnelGuid IN (SELECT PersonnelGuid FROM @PersonnelGuidTable)) tblPersonnel2 WHERE tblPersonnel2._MasterRecordGuid = map.tblCompanyPersonnelToShipToBillTo.PersonnelGuid) AS LockedOut ";
			}
		}

		public override void SelectIdentityGuidByTypeAndMapIdsql(SqlCommand cmd)
        {
            cmd.CommandText =
 @"SELECT msttbt.CompanyPersonnelToShipToBillToGuid
FROM map.tblCompanyPersonnelToShipToBillTo msttbt
WHERE msttbt.SiteGuid = @SiteGuid
    AND msttbt.ID = @MapID";

            cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@MapID"].Value = this.MapID;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }

        public override void SelectSQLMinimal(SqlCommand cmd)
        {
            cmd.CommandText =
 @"SELECT CompanyPersonnelToShipToBillToGuid
    ,PersonnelGuid
    ,CompanyShipToToBillToGuid
    ,SiteGuid
    ,ID
    ,CreatedDate
    ,CreatedBy
    ,UpdatedDate
    ,UpdatedBy
    ,ID AS AssignedToID
    ,NULL AS AssignedID
    ,NULL AS LockedOut  
FROM map.tblCompanyPersonnelToShipToBillTo   
WHERE CompanyPersonnelToShipToBillToGuid = @IdentityGuid";

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
        }

        public override void Load(object o)
        {
            base.Load(o);
			DataSet set = o as DataSet;
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
				XmlNode node = o as XmlNode;
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
