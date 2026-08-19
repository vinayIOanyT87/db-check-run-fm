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
    [EntityImportExportWorksheet("COMPANYBILLTOSHIPPERMAPS")]
    class CompanyMapBillToShipperClass : CompanyMapClass
    {
        #region Data members
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyBillToToShipper";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyBillToToShipperGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "CompanyShipperToOwnerGuid";
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
            " SELECT cbts.* "
            + ", NULL AS AssignedToID"
            + ", company1.ID AS AssignedID"
            + ", company1.LockedOut AS LockedOut"
            + ", company1.Name AS AssignedName"
            + ", company1.Address1 AS AssignedAddress"
            + ", company1.City AS AssignedCity"
            + ", company1.State AS AssignedState"
            + ", company1.ID AS BillToID"
            + ", (SELECT ID FROM @TempCompany a1 WHERE a1._MasterRecordGuid = (SELECT A.CompanyGuid FROM map.tblCompanyShipperToOwner A"
            + "    WHERE A.CompanyShipperToOwnerGuid = cbts.CompanyShipperToOwnerGuid)) AS ShipperID"
            + ", (SELECT ID FROM @TempCompany a2 WHERE a2._MasterRecordGuid = (SELECT B.CompanyGuid FROM map.tblCompanyLoadOwnerToManager B"
            + "    WHERE B.CompanyLoadOwnerToManagerGuid IN(SELECT A.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner A"
            + "    WHERE A.CompanyShipperToOwnerGuid = cbts.CompanyShipperToOwnerGuid))) AS OwnerID"
            + ", (SELECT ID FROM @TempCompany a3 WHERE a3._MasterRecordGuid = (SELECT B.AssignedToCompanyGuid FROM map.tblCompanyLoadOwnerToManager B"
            + "    WHERE B.CompanyLoadOwnerToManagerGuid IN(SELECT A.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner A"
            + "    WHERE A.CompanyShipperToOwnerGuid = cbts.CompanyShipperToOwnerGuid))) AS ManagerID"
            + Environment.NewLine;
        #endregion

        public override COMPANY_MAP_TYPE Type
        {
            get { return COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP; }

            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

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

//		protected override string SelectClause
//		{
//			get
//			{
//				return "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
//"INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
//"SELECT map.tblCompanyBillToToShipper.*," +
//"NULL AS AssignedToID," + //the Load method sets the AssignedToID based on other values returned by this select clause.
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies14 WHERE tblCompanies14._MasterRecordGuid = map.tblCompanyBillToToShipper.CompanyGuid) AS AssignedID," +
//"(SELECT LockedOut FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies15 WHERE tblCompanies15._MasterRecordGuid = map.tblCompanyBillToToShipper.CompanyGuid) AS LockedOut," +
//"(SELECT Name FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies16 WHERE tblCompanies16._MasterRecordGuid = map.tblCompanyBillToToShipper.CompanyGuid) AS AssignedName," +
//"(SELECT Address1 FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies17 WHERE tblCompanies17._MasterRecordGuid = map.tblCompanyBillToToShipper.CompanyGuid) AS AssignedAddress," +
//"(SELECT City FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies18 WHERE tblCompanies18._MasterRecordGuid = map.tblCompanyBillToToShipper.CompanyGuid) AS AssignedCity," +
//"(SELECT State FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies19 WHERE tblCompanies19._MasterRecordGuid = map.tblCompanyBillToToShipper.CompanyGuid) AS AssignedState, " +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies20 WHERE tblCompanies20._MasterRecordGuid = map.tblCompanyBillToToShipper.CompanyGuid) AS BillToID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies21 WHERE tblCompanies21._MasterRecordGuid IN (SELECT A.CompanyGuid FROM map.tblCompanyShipperToOwner A WHERE A.CompanyShipperToOwnerGuid = map.tblCompanyBillToToShipper.CompanyShipperToOwnerGuid)) AS ShipperID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies22 WHERE tblCompanies22._MasterRecordGuid IN (SELECT B.CompanyGuid FROM map.tblCompanyLoadOwnerToManager B WHERE B.CompanyLoadOwnerToManagerGuid IN (SELECT A.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner A WHERE A.CompanyShipperToOwnerGuid = map.tblCompanyBillToToShipper.CompanyShipperToOwnerGuid))) AS OwnerID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies23 WHERE tblCompanies23._MasterRecordGuid IN (SELECT B.AssignedToCompanyGuid FROM map.tblCompanyLoadOwnerToManager B WHERE B.CompanyLoadOwnerToManagerGuid IN (SELECT A.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner A WHERE A.CompanyShipperToOwnerGuid = map.tblCompanyBillToToShipper.CompanyShipperToOwnerGuid))) AS ManagerID ";
//			}
//		}

		public override void SelectSQLMinimal(SqlCommand cmd)
        {
            cmd.CommandText =
                 @"SELECT CompanyBillToToShipperGuid
                    ,CompanyGuid
                    ,CompanyShipperToOwnerGuid
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
                    ,NULL AS ShipperID
                FROM map.tblCompanyBillToToShipper   
                WHERE CompanyBillToToShipperGuid = @IdentityGuid";

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

                DataRow row = table.Rows[0];
                this.AssignedName = DataObject.getValue<string>(row["AssignedName"], "");
                this.AssignedAddress = DataObject.getValue<string>(row["AssignedAddress"], "");
                this.AssignedCity = DataObject.getValue<string>(row["AssignedCity"], "");
                this.AssignedState = DataObject.getValue<string>(row["AssignedState"], "");

				string managerID = DataObject.getValue<string>(row["ManagerID"], "");
				string ownerID = DataObject.getValue<string>(row["OwnerID"], "");
				string shipperID = DataObject.getValue<string>(row["ShipperID"], "");
                this.AssignedToID = managerID + "->" + ownerID + "->" + shipperID;
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

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void EnumerateByTypeSQL(SqlCommand cmd, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " cbts " 
                     + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = cbts.CompanyGuid"
                     + " LEFT JOIN map.tblEntityCompanyToSite ects ON ects.CompanyGuid = c1.CompanyGuid"
                     + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = cbts.CompanyGuid"
                     + Environment.NewLine;

            string where =
                " WHERE cbts.SiteGuid = @SiteGuid"
                + " AND cbts." + this.MappingTableAssignedGuidColumnName + " = c1.CompanyGuid"
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
                    " FROM " + ClassMappingTableName + " cbts "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = cbts.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = cbts.CompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE cbts.SiteGuid = @SiteGuid"
                + " AND cbts." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid"
                + " AND cbts." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid"
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
                    " FROM " + ClassMappingTableName + " cbts "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = cbts.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = cbts.CompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE cbts.SiteGuid = @SiteGuid"
                + " AND cbts." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid"
                + Environment.NewLine;

            string orderBy = " ORDER BY cbts.ID ASC " + Environment.NewLine;

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
                    " FROM " + ClassMappingTableName + " cbts "
                     + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = cbts.CompanyGuid"
                     + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = cbts.CompanyGuid "
                     + Environment.NewLine;

            string where =
                " WHERE cbts.SiteGuid = @SiteGuid"
                + " AND cbts." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid"
                + Environment.NewLine;

            string orderBy = " ORDER BY cbts.ID ASC " + Environment.NewLine;

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
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void SelectSQL(SqlCommand cmd, bool bInTransaction)
        {
            string from =
                " FROM " + ClassMappingTableName + " cbts "
                    + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = cbts.CompanyGuid"
                    + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = cbts.CompanyGuid " + SQLUpdateLock(bInTransaction)
                    + Environment.NewLine;

            string where =
                " WHERE cbts." + this.MappingTablePrimaryKeyColumnName + " = @IdentityGuid"
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
                " FROM " + ClassMappingTableName + " cbts "
                    + " INNER JOIN tblCompanies c1 ON c1.CompanyGuid = cbts.CompanyGuid"
                    + " INNER JOIN @TempCompany company1 ON company1._MasterRecordGuid = cbts.CompanyGuid " + SQLUpdateLock(bInTransaction)
                    + Environment.NewLine;

            string where =
                " WHERE cbts.SiteGuid = @SiteGuid"
                + " AND cbts.ID = @MapID"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;

            cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@MapID"].Value = this.MapID;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }
    }
}
