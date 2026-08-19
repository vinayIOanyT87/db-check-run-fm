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
    [EntityImportExportWorksheet("COMPANYLOADOWNERMANAGERMAPS")]
    class CompanyMapLoadOwnerManagerClass : CompanyMapClass
    {
        #region Data members
        private const string SchemaPrefix = "map.";
        internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyLoadOwnerToManager";
        internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyLoadOwnerToManagerGuid";
        internal const string ClassMappingTableAssignedToGuidColumnName = "AssignedToCompanyGuid";
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
            " SELECT clotm.* "
            + ", assignedToCompany.ID AS AssignedToID"
            + ", assignedCompany.ID AS AssignedID"
            + ", assignedCompany.LockedOut AS LockedOut"
            + ", assignedCompany.Name AS AssignedName"
            + ", assignedCompany.Address1 AS AssignedAddress"
            + ", assignedCompany.City AS AssignedCity"
            + ", assignedCompany.State AS AssignedState"
            + ", assignedCompany.ID AS OwnerID"
            + ", assignedToCompany.ID AS ManagerID "
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
            get { return COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP; }

            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

//		protected override string SelectClause
//		{
//			get
//			{
//				return "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
//"INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
//"SELECT " + this.MappingTableName + ".*," +
//"(SELECT tblCompanies32.ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies32 WHERE tblCompanies32._MasterRecordGuid = " + this.MappingTableName + ".AssignedToCompanyGuid ) AS AssignedToID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies33 WHERE tblCompanies33._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedID," +
//"(SELECT LockedOut FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies34 WHERE tblCompanies34._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS LockedOut," +
//"(SELECT Name FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies35 WHERE tblCompanies35._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedName," +
//"(SELECT Address1 FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies36 WHERE tblCompanies36._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedAddress," +
//"(SELECT City FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies37 WHERE tblCompanies37._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedCity," +
//"(SELECT State FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies38 WHERE tblCompanies38._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS AssignedState, " +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies39 WHERE tblCompanies39._MasterRecordGuid = " + this.MappingTableName + ".CompanyGuid) AS OwnerID," +
//"(SELECT ID FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies40 WHERE tblCompanies40._MasterRecordGuid = " + this.MappingTableName + "." + this.MappingTableAssignedToGuidColumnName + ") AS ManagerID ";
//			}
//		}

        public override void SelectSQLMinimal(SqlCommand cmd)
        {
            cmd.CommandText =
                     @"SELECT CompanyLoadOwnerToManagerGuid
                        ,CompanyGuid
                        ,AssignedToCompanyGuid
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
                    FROM map.tblCompanyLoadOwnerToManager   
                    WHERE CompanyLoadOwnerToManagerGuid = @IdentityGuid";

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

                this.IdentityGuid = DataObject.getValue<Guid>(row[this.MappingTablePrimaryKeyColumnName], Guid.Empty);
                this.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
                this.AssignedToGuid = DataObject.getValue<Guid>(
                    row[this.MappingTableAssignedToGuidColumnName],
                    Guid.Empty);
                this.AssignedGuid = DataObject.getValue<Guid>(row[this.MappingTableAssignedGuidColumnName], Guid.Empty);
                this.MapID = DataObject.getValue<string>(row["ID"], "");
                this.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
                this.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
                this.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
                this.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
                this.AssignedToID = DataObject.getValue<string>(row["AssignedToID"], "");
                this.AssignedID = DataObject.getValue<string>(row["AssignedID"], "");
                this.LockedOut = DataObject.getValue<bool>(row["LockedOut"], false);
                this.AssignedName = DataObject.getValue<string>(row["AssignedName"], "");
                this.AssignedAddress = DataObject.getValue<string>(row["AssignedAddress"], "");
                this.AssignedCity = DataObject.getValue<string>(row["AssignedCity"], "");
                this.AssignedState = DataObject.getValue<string>(row["AssignedState"], "");

				string managerID = DataObject.getValue<string>(row["ManagerID"], "");
                this.AssignedToID = managerID;
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
        /// <param name="bInTransaction">Flag whether to place the the query in a transaction.</param>
        public override void SelectSQL(SqlCommand cmd, bool bInTransaction)
        {
            string from =
                    " FROM " + ClassMappingTableName + " clotm " + SQLUpdateLock(bInTransaction)
                     + " INNER JOIN tblCompanies assignedCompany ON assignedCompany.CompanyGuid = clotm.CompanyGuid"
                     + " INNER JOIN @TempCompany assignedToCompany ON assignedToCompany._MasterRecordGuid = clotm.AssignedToCompanyGuid"
                     + Environment.NewLine;

            string where =
                " WHERE clotm." + this.MappingTablePrimaryKeyColumnName + " = @IdentityGuid"
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
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void EnumerateByTypeSQL(SqlCommand cmd, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " clotm "
                     + " LEFT JOIN tblCompanies assignedCompany ON assignedCompany.CompanyGuid = clotm.CompanyGuid"
                     + " LEFT JOIN map.tblEntityCompanyToSite ects ON ects.CompanyGuid = assignedCompany.CompanyGuid"
                     + " LEFT JOIN @TempCompany assignedToCompany ON assignedToCompany._MasterRecordGuid = clotm.AssignedToCompanyGuid"
                     + Environment.NewLine;

            string where =
                " WHERE clotm.SiteGuid = @SiteGuid"
                + " AND clotm." + this.MappingTableAssignedGuidColumnName + " = assignedCompany.CompanyGuid"
                + " AND ects.SiteGuid = @SiteGuid"
                + " AND ects.CompanyGuid = assignedCompany.CompanyGuid"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
        }

        /// <summary>
        /// This method will override the base class select SQL.
        /// </summary>
        /// <param name="cmd">The SQL command to populate.</param>
        /// <param name="bInTransaction">Indicates whether the query should be in a transaction.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void EnumerateByAssignedToGuidAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " clotm "
                     + " INNER JOIN tblCompanies assignedCompany ON assignedCompany.CompanyGuid = clotm.CompanyGuid"
                     + " INNER JOIN @TempCompany assignedToCompany ON assignedToCompany._MasterRecordGuid = clotm.AssignedToCompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE clotm.SiteGuid = @SiteGuid"
                + " AND clotm." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid"
                + Environment.NewLine;

            string orderBy = " ORDER BY clotm.ID ASC" + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where + orderBy;

            cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
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
                    " FROM " + ClassMappingTableName + " clotm "
                     + " INNER JOIN tblCompanies assignedCompany ON assignedCompany.CompanyGuid = clotm.CompanyGuid"
                     + " INNER JOIN @TempCompany assignedToCompany ON assignedToCompany._MasterRecordGuid = clotm.AssignedToCompanyGuid "
                     + Environment.NewLine;

            string where =
                " WHERE clotm.SiteGuid = @SiteGuid"
                + " AND clotm." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid" 
                + Environment.NewLine;

            string orderBy = " ORDER BY clotm.ID ASC" + Environment.NewLine;

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
        /// <param name="bInTransaction">Indicates whether the query should be in a transaction.</param>
        /// <param name="skipSiteGuid">Skip using site guid flag which is not used.</param>
        public override void SelectByGuidsAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
        {
            string from =
                    " FROM " + ClassMappingTableName + " clotm "
                     + " INNER JOIN tblCompanies assignedCompany ON assignedCompany.CompanyGuid = clotm.CompanyGuid"
                     + " INNER JOIN @TempCompany assignedToCompany ON assignedToCompany._MasterRecordGuid = clotm.AssignedToCompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE clotm.SiteGuid = @SiteGuid"
                + " AND clotm." + this.MappingTableAssignedToGuidColumnName + " = @AssignedToGuid"
                + " AND clotm." + this.MappingTableAssignedGuidColumnName + " = @AssignedGuid"
                + Environment.NewLine;

            string orderBy = " ORDER BY clotm.ID ASC" + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where + orderBy;

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
                    " FROM " + ClassMappingTableName + " clotm "
                     + " INNER JOIN tblCompanies assignedCompany ON assignedCompany.CompanyGuid = clotm.CompanyGuid"
                     + " INNER JOIN @TempCompany assignedToCompany ON assignedToCompany._MasterRecordGuid = clotm.AssignedToCompanyGuid " + SQLUpdateLock(bInTransaction)
                     + Environment.NewLine;

            string where =
                " WHERE clotm.SiteGuid = @SiteGuid"
                + " AND clotm.ID = @MapID"
                + Environment.NewLine;

            cmd.CommandText = this.companyGuidTableSql + this.tempCompanyTableSql + this.select + from + where;

            cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@MapID"].Value = this.MapID;
            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
        }
    }
}
