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
    [EntityImportExportWorksheet("COMPANYSHIPTOBILLTOMAPS")]
    class CompanyMapShipToBillToClass : CompanyMapClass
    {
        #region data members
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyShipToToBillTo";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyShipToToBillToGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "CompanyBillToToShipperGuid";
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
            " SELECT cstbt.*"
            + ", NULL AS AssignedToID"
            + ", company1.ID AS AssignedID"
            + ", company1.LockedOut AS LockedOut"
            + ", company1.Name AS AssignedName"
            + ", company1.Address1 AS AssignedAddress"
            + ", company1.City AS AssignedCity"
            + ", company1.State AS AssignedState"
            + ", company1.ID AS ShipToID"
            + ", (SELECT ID FROM @TempCompany a8 WHERE a8._MasterRecordGuid = (SELECT A.CompanyGuid FROM map.tblCompanyBillToToShipper A"
            + " WHERE A.CompanyBillToToShipperGuid = cstbt.CompanyBillToToShipperGuid)) AS BillToID"
            + ", (SELECT ID FROM @TempCompany a9 WHERE a9._MasterRecordGuid = (SELECT B.CompanyGuid FROM map.tblCompanyShipperToOwner B"
            + " WHERE B.CompanyShipperToOwnerGuid IN(SELECT A.CompanyShipperToOwnerGuid FROM map.tblCompanyBillToToShipper A"
            + " WHERE A.CompanyBillToToShipperGuid = cstbt.CompanyBillToToShipperGuid))) AS ShipperID"
            + ", (SELECT ID FROM @TempCompany a10 WHERE a10._MasterRecordGuid = (SELECT C.CompanyGuid FROM map.tblCompanyLoadOwnerToManager C"
            + " WHERE C.CompanyLoadOwnerToManagerGuid IN(SELECT B.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner B"
            + " WHERE B.CompanyShipperToOwnerGuid IN(SELECT A.CompanyShipperToOwnerGuid FROM map.tblCompanyBillToToShipper A"
            + " WHERE A.CompanyBillToToShipperGuid = cstbt.CompanyBillToToShipperGuid)))) AS OwnerID"
            + ", (SELECT ID FROM @TempCompany a11 WHERE a11._MasterRecordGuid = (SELECT C.AssignedToCompanyGuid FROM map.tblCompanyLoadOwnerToManager C"
            + " WHERE C.CompanyLoadOwnerToManagerGuid IN(SELECT B.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner B"
            + " WHERE B.CompanyShipperToOwnerGuid IN(SELECT A.CompanyShipperToOwnerGuid FROM  map.tblCompanyBillToToShipper A"
            + " WHERE A.CompanyBillToToShipperGuid = cstbt.CompanyBillToToShipperGuid)))) AS ManagerID"
            + Environment.NewLine;
        #endregion

        #region Properties
        protected override string MappingTableName
		{
			get { return ClassMappingTableName;}
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
            get { return COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP; }
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

//		protected override string SelectClause
//		{
//			get
//			{
//				return "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
//"INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
//"SELECT map.tblCompanyShipToToBillTo.*," +
//"NULL AS AssignedToID," + //the Load method sets the AssignedToID based on other values returned by this select clause.
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies3 WHERE tblCompanies3._MasterRecordGuid = map.tblCompanyShipToToBillTo.CompanyGuid) AS AssignedID," +
//"(SELECT LockedOut FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies4 WHERE tblCompanies4._MasterRecordGuid = map.tblCompanyShipToToBillTo.CompanyGuid) AS LockedOut," +
//"(SELECT Name FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies5 WHERE tblCompanies5._MasterRecordGuid = map.tblCompanyShipToToBillTo.CompanyGuid) AS AssignedName," +
//"(SELECT Address1 FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies6 WHERE tblCompanies6._MasterRecordGuid = map.tblCompanyShipToToBillTo.CompanyGuid) AS AssignedAddress," +
//"(SELECT City FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies7 WHERE tblCompanies7._MasterRecordGuid = map.tblCompanyShipToToBillTo.CompanyGuid) AS AssignedCity," +
//"(SELECT State FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies8 WHERE tblCompanies8._MasterRecordGuid = map.tblCompanyShipToToBillTo.CompanyGuid) AS AssignedState, " +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies9 WHERE tblCompanies9._MasterRecordGuid = map.tblCompanyShipToToBillTo.CompanyGuid) AS ShipToID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies10 WHERE tblCompanies10._MasterRecordGuid IN (SELECT A.CompanyGuid FROM map.tblCompanyBillToToShipper A WHERE A.CompanyBillToToShipperGuid = map.tblCompanyShipToToBillTo.CompanyBillToToShipperGuid)) AS BillToID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies11 WHERE tblCompanies11._MasterRecordGuid IN (SELECT B.CompanyGuid FROM map.tblCompanyShipperToOwner B WHERE B.CompanyShipperToOwnerGuid IN (SELECT A.CompanyShipperToOwnerGuid FROM map.tblCompanyBillToToShipper A WHERE A.CompanyBillToToShipperGuid = map.tblCompanyShipToToBillTo.CompanyBillToToShipperGuid))) AS ShipperID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies12 WHERE tblCompanies12._MasterRecordGuid IN (SELECT C.CompanyGuid FROM map.tblCompanyLoadOwnerToManager C WHERE C.CompanyLoadOwnerToManagerGuid IN (SELECT B.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner B WHERE B.CompanyShipperToOwnerGuid IN (SELECT A.CompanyShipperToOwnerGuid FROM map.tblCompanyBillToToShipper A WHERE A.CompanyBillToToShipperGuid = map.tblCompanyShipToToBillTo.CompanyBillToToShipperGuid)))) AS OwnerID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies13 WHERE tblCompanies13._MasterRecordGuid IN (SELECT C.AssignedToCompanyGuid FROM map.tblCompanyLoadOwnerToManager C WHERE C.CompanyLoadOwnerToManagerGuid IN (SELECT B.CompanyLoadOwnerToManagerGuid FROM map.tblCompanyShipperToOwner B WHERE B.CompanyShipperToOwnerGuid IN (SELECT A.CompanyShipperToOwnerGuid FROM  map.tblCompanyBillToToShipper A WHERE A.CompanyBillToToShipperGuid = map.tblCompanyShipToToBillTo.CompanyBillToToShipperGuid)))) AS ManagerID ";
//			}
//		}
        #endregion

        public override void SelectSQLMinimal(SqlCommand cmd)
        {
            cmd.CommandText =
                 @"SELECT CompanyShipToToBillToGuid
                    ,CompanyGuid
                    ,CompanyBillToToShipperGuid
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
                    ,NULL AS BillToID
                FROM map.tblCompanyShipToToBillTo   
                WHERE CompanyShipToToBillToGuid = @IdentityGuid";

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

                var managerID = DataObject.getValue<string>(row["ManagerID"], "");
                var ownerID = DataObject.getValue<string>(row["OwnerID"], "");
                var shipperID = DataObject.getValue<string>(row["ShipperID"], "");
                this.AssignedToID = managerID + "->" + ownerID + "->" + shipperID + "->" + DataObject.getValue<string>(row["BillToID"], "");
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
        /// This method overrides the method from the CompanyMapClass.cs file.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="skipSiteGuid">Skip the site guid, but it is not used.</param>
        public override void EnumerateByTypeSQL(SqlCommand cmd, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " cstbt"
                     + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = cstbt.CompanyGuid"
                     + " LEFT JOIN map.tblEntityCompanyToSite ects ON ects.CompanyGuid = c1.CompanyGuid"
                     + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = cstbt.CompanyGuid"
                     + Environment.NewLine;

            string where =
                " WHERE cstbt.SiteGuid = @SiteGuid"
                + " AND cstbt." + this.MappingTableAssignedGuidColumnName + " = c1.CompanyGuid"
                + " AND ects.SiteGuid = @SiteGuid"
                + " AND ects.CompanyGuid = c1.CompanyGuid"
                + Environment.NewLine;

            string orderBy = "ORDER BY AssignedToID, company1.ID";

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where + orderBy;
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }

        /// <summary>
        /// This method overrides the method from the CompanyMapClass.cs file.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="security">The security object.</param>
        /// <param name="skipSiteGuid">Skip the site guid, but it is not used.</param>
        public override void EnumerateByAssignedGuidAndTypeSQL(SqlCommand cmd, SecurityClass security, bool skipSiteGuid = false)
        {
            string from =
                " FROM " + ClassMappingTableName + " cstbt"
                + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = cstbt.CompanyGuid"
                + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = cstbt.CompanyGuid"
                + Environment.NewLine;

            string where =
                " WHERE cstbt.SiteGuid = @SiteGuid"
                + " AND cstbt." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid"
                + Environment.NewLine;

            string orderBy = "ORDER BY cstbt.ID ASC " + Environment.NewLine;

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
        public override void EnumerateByAssignedToGuidAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                " FROM " + ClassMappingTableName + " cstbt"
                + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = cstbt.CompanyGuid"
                + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = cstbt.CompanyGuid " + SQLUpdateLock(bInTransaction)
                + Environment.NewLine;

            string where =
                " WHERE cstbt.SiteGuid = @SiteGuid"
                + " AND cstbt." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid"
                + Environment.NewLine;

            string orderBy = "ORDER BY cstbt.ID ASC" + Environment.NewLine;

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
        /// <param name="bInTransaction">Flag to place the query in a transaction.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void SelectSQL(SqlCommand cmd, bool bInTransaction)
        {
            string from =
                    " FROM " + ClassMappingTableName + " cstbt"
                     + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = cstbt.CompanyGuid"
                     + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = cstbt.CompanyGuid" + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE cstbt." + this.MappingTablePrimaryKeyColumnName + " = @IdentityGuid"
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
        public override void SelectByGuidsAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                " FROM " + ClassMappingTableName + " cstbt"
                + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = cstbt.CompanyGuid"
                + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = cstbt.CompanyGuid " + SQLUpdateLock(bInTransaction)
                + Environment.NewLine;

            string where =
                " WHERE cstbt.SiteGuid = @SiteGuid"
                + " AND cstbt." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid" 
                + " AND cstbt." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid"
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
        public override void SelectByTypeAndMapIdsql(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " cstbt"
                     + " LEFT JOIN tblCompanies c1 ON c1.CompanyGuid = cstbt.CompanyGuid"
                     + " LEFT JOIN @TempCompany company1 ON company1._MasterRecordGuid = cstbt.CompanyGuid" + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE cstbt.SiteGuid = @SiteGuid" 
                + " AND cstbt.ID = @MapID"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;

            cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@MapID"].Value = this.MapID;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }
    }
}
