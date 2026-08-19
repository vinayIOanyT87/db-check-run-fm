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
    [EntityImportExportWorksheet("COMPANYSHIPPEROWNERMAPS")]
    class CompanyMapShipperOwnerClass : CompanyMapClass
    {
        #region Data members
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyShipperToOwner";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyShipperToOwnerGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "CompanyLoadOwnerToManagerGuid";
        internal const string ClassMappingTableAssignedGuidColumnName = "CompanyGuid";

        private readonly string companyGuidTableSql =
                    " DECLARE @CompanyGuidTable TABLE(CompanyGuid uniqueidentifier NULL)"
                    + " INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM[erv].[udf_GetCompanyRecordVersions](@SiteGuid)"
                    + Environment.NewLine;

        private readonly string tempCompanyTableSql =
            " DECLARE @TempCompany TABLE(CompanyGuid UNIQUEIDENTIFIER, _MasterRecordGuid UNIQUEIDENTIFIER, ID NVARCHAR(100)"
            + ", LockedOut BIT, Name NVARCHAR(100), Address1 NVARCHAR(100), City NVARCHAR(100), State NVARCHAR(100))"
            + " INSERT INTO @TempCompany"
            + " SELECT c.CompanyGuid, c._MasterRecordGuid, c.ID, c.LockedOut, c.[Name], c.Address1, c.City, c.[State]"
            + " FROM tblCompanies c INNER JOIN @CompanyGuidTable cgt on cgt.CompanyGuid = c.CompanyGuid"
            + Environment.NewLine;

        private readonly string select =
            " SELECT csto.* "
            + ", NULL AS AssignedToID"
            + ", company1.ID AS AssignedID"
            + ", company1.LockedOut AS LockedOut"
            + ", company1.Name AS AssignedName"
            + ", company1.Address1 AS AssignedAddress"
            + ", company1.City AS AssignedCity"
            + ", company1.State AS AssignedState"
            + ", company1.ID AS ShipperID"
            + ", (SELECT ID FROM @TempCompany a1 WHERE a1._MasterRecordGuid = (SELECT A.CompanyGuid FROM map.tblCompanyLoadOwnerToManager A"
            + "    WHERE A.CompanyLoadOwnerToManagerGuid = csto.CompanyLoadOwnerToManagerGuid)) AS OwnerID"
            + ", (SELECT ID FROM @TempCompany a2 WHERE a2._MasterRecordGuid = (SELECT A.AssignedToCompanyGuid FROM map.tblCompanyLoadOwnerToManager A"
            + "    WHERE A.CompanyLoadOwnerToManagerGuid = csto.CompanyLoadOwnerToManagerGuid)) AS ManagerID"
            + Environment.NewLine;
        #endregion

        protected override string MappingTableName
		{
			get { return ClassMappingTableName; }
		}

		protected override string MappingTablePrimaryKeyColumnName
		{
			get { return ClassMappingTablePrimaryKeyColumnName; }
		}

		protected override string MappingTableAssignedToGuidColumnName
		{
			get { return ClassMappingTableAssignedToGuidColumnName; }
		}

		protected override string MappingTableAssignedGuidColumnName
		{
			get { return ClassMappingTableAssignedGuidColumnName; }
		}

		public override COMPANY_MAP_TYPE Type
        {
            get { return COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP; }

            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

//		protected override string SelectClause
//		{
//			get
//			{
//				return "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
//"INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
//"SELECT map.tblCompanyShipperToOwner.*," +
//"NULL AS AssignedToID," + //the Load method sets the AssignedToID based on other values returned by this select clause.
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies24 WHERE tblCompanies24._MasterRecordGuid = map.tblCompanyShipperToOwner.CompanyGuid) AS AssignedID," +
//"(SELECT LockedOut FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies25 WHERE tblCompanies25._MasterRecordGuid = map.tblCompanyShipperToOwner.CompanyGuid) AS LockedOut," +
//"(SELECT Name FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies26 WHERE tblCompanies26._MasterRecordGuid = map.tblCompanyShipperToOwner.CompanyGuid) AS AssignedName," +
//"(SELECT Address1 FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies27 WHERE tblCompanies27._MasterRecordGuid = map.tblCompanyShipperToOwner.CompanyGuid) AS AssignedAddress," +
//"(SELECT City FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies28 WHERE tblCompanies28._MasterRecordGuid = map.tblCompanyShipperToOwner.CompanyGuid) AS AssignedCity," +
//"(SELECT State FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies29 WHERE tblCompanies29._MasterRecordGuid = map.tblCompanyShipperToOwner.CompanyGuid) AS AssignedState, " +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies30 WHERE tblCompanies30._MasterRecordGuid = map.tblCompanyShipperToOwner.CompanyGuid) AS ShipperID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies31 WHERE tblCompanies31._MasterRecordGuid IN (SELECT A.CompanyGuid FROM map.tblCompanyLoadOwnerToManager A WHERE A.CompanyLoadOwnerToManagerGuid = map.tblCompanyShipperToOwner.CompanyLoadOwnerToManagerGuid)) AS OwnerID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies32 WHERE tblCompanies32._MasterRecordGuid IN (SELECT A.AssignedToCompanyGuid FROM map.tblCompanyLoadOwnerToManager A WHERE A.CompanyLoadOwnerToManagerGuid = map.tblCompanyShipperToOwner.CompanyLoadOwnerToManagerGuid)) AS ManagerID ";
//			}
//		}

		public override void SelectSQLMinimal(SqlCommand cmd)
        {
            cmd.CommandText =
                 @"SELECT CompanyShipperToOwnerGuid
                    ,CompanyGuid
                    ,CompanyLoadOwnerToManagerGuid
                    ,SiteGuid
                    ,ID
                    ,CreatedDate
                    ,CreatedBy
                    ,UpdatedDate
                    ,UpdatedBy
                    ,ID AS AssignedToID
                    ,NULL AS AssignedID
                    ,NULL AS LockedOut  
                    ,NULL AS AssignedName
                    ,NULL AS AssignedAddress
                    ,NULL AS AssignedCity
                    ,NULL AS AssignedState
                    ,NULL AS ManagerID
                    ,NULL AS OwnerID
                FROM map.tblCompanyShipperToOwner   
                WHERE CompanyShipperToOwnerGuid = @IdentityGuid";

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
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

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void EnumerateByTypeSQL(SqlCommand cmd, bool skipSiteGuid = false) 
        {
            string from =
                    " FROM " + ClassMappingTableName + " csto "
                     + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = csto.CompanyGuid"
                     + " LEFT JOIN map.tblEntityCompanyToSite ects ON ects.CompanyGuid = c1.CompanyGuid"
                     + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = csto.CompanyGuid"
                     + Environment.NewLine;

            string where =
                " WHERE csto.SiteGuid = @SiteGuid"
                + " AND csto." + this.MappingTableAssignedGuidColumnName + " = c1.CompanyGuid"
                + " AND ects.SiteGuid = @SiteGuid"
                + " AND ects.CompanyGuid = c1.CompanyGuid"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
        }

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="bInTransaction">Flag to place the query in a transaction.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void SelectByGuidsAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " csto "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = csto.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = csto.CompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE csto.SiteGuid = @SiteGuid"
                + " AND csto." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid"
                + " AND csto." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;

            cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
            cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="bInTransaction">Flag to place the query in a transaction.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void EnumerateByAssignedToGuidAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " csto "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = csto.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = csto.CompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE csto.SiteGuid = @SiteGuid"
                + " AND csto." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid"
                + Environment.NewLine;

            string orderBy = " ORDER BY csto.ID ASC " + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where + orderBy;

            cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="security">The security object..</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void EnumerateByAssignedGuidAndTypeSQL(SqlCommand cmd, SecurityClass security, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " csto "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = csto.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = csto.CompanyGuid "
                     + Environment.NewLine;

            string where =
                " WHERE csto.SiteGuid = @SiteGuid"
                + " AND csto." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid"
                + Environment.NewLine;

            string orderBy = " ORDER BY csto.ID ASC " + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where + orderBy;

            cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="bInTransaction">Flag to place the query in a transaction.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void SelectSQL(SqlCommand cmd, bool bInTransaction)
        {
            string from =
                    " FROM " + ClassMappingTableName + " csto "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = csto.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = csto.CompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE csto." + this.MappingTablePrimaryKeyColumnName + " = @IdentityGuid"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;


            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
            
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
        }

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="bInTransaction">Flag to place the query in a transaction.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void SelectByTypeAndMapIdsql(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " csto "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = csto.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = csto.CompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE csto.SiteGuid = @SiteGuid"
                + " AND csto.ID = @MapID"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;

            cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@MapID"].Value = this.MapID;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }
    }
}
