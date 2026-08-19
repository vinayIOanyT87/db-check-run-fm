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
	[EntityImportExportWorksheet("COMPANYPERSONASSIGNEDCOMPANYMAPS")]
	class CompanyMapPersonAssignedCompanyClass : CompanyMapClass
	{
		private const string SchemaPrefix = "map.";
		internal const string ClassMappingTableName = SchemaPrefix + "tblCompanyPersonnelAssignedToCompany";
		internal const string ClassMappingTablePrimaryKeyColumnName = "CompanyPersonnelAssignedToCompanyGuid";
		internal const string ClassMappingTableAssignedToGuidColumnName = "PersonnelGuid";
		internal const string ClassMappingTableAssignedGuidColumnName = "CompanyGuid";
		protected override string MappingTableName => ClassMappingTableName;
		protected override string MappingTablePrimaryKeyColumnName => ClassMappingTablePrimaryKeyColumnName;
		protected override string MappingTableAssignedToGuidColumnName => ClassMappingTableAssignedToGuidColumnName;
		protected override string MappingTableAssignedGuidColumnName => ClassMappingTableAssignedGuidColumnName;

		public override COMPANY_MAP_TYPE Type
		{
			get
			{
				return COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY;
			}
			// ReSharper disable once ValueParameterNotUsed
			set
			{
			}
		}

		protected override string SelectClause => "declare @CompanyGuidTable TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
										  "INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)" + Environment.NewLine +
										  "SELECT " + this.MappingTableName + ".*," +
										  "'' AS AssignedToID," +
										  "'' AS AssignedID," +
										  "0 AS LockedOut," +
										  "'' AS AssignedName," +
										  "'' AS AssignedAddress," +
										  "'' AS AssignedCity," +
										  "'' AS AssignedState, " +
										  "'' AS AssignedToFirstName," +
										  "'' AS AssignedToMiddleName," +
										  "'' AS AssignedToLastName ";

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
				this.AssignedToFirstName = DataObject.getValue<string>(row["AssignedToFirstName"], "");
				this.AssignedToMiddleName = DataObject.getValue<string>(row["AssignedToMiddleName"], "");
				this.AssignedToLastName = DataObject.getValue<string>(row["AssignedToLastName"], "");
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

		public override void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany] " +
				 "(SiteGuid," +
				 "PersonnelGuid," +
				 "CompanyGuid," +
				 "ID," +
				 "CreatedDate," +
				 "CreatedBy," +
				 "UpdatedDate," +
				 "UpdatedBy," +
				 "CompanyPersonnelAssignedToCompanyGuid" +
				 ") VALUES (" +
				 "@SiteGuid," +
				 "@AssignedToGuid," +
				 "@AssignedGuid," +
				 "@MapID," +
				 "@CreatedDate," +
				 "@CreatedBy," +
				 "@UpdatedDate," +
				 "@UpdatedBy," +
				 "@CompanyPersonnelAssignedToCompanyGuid" +
				 ")";

			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedToGuid", this.AssignedToGuid, true)); // true means replace Guid.Empty with NULL
			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedGuid", this.AssignedGuid, true));  // true means replace Guid.Empty with NULL
			cmd.Parameters.AddWithValue("@MapID", this.MapID);
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this.CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
			cmd.Parameters.AddWithValue("@CompanyPersonnelAssignedToCompanyGuid", this._IdentityGuid);
		}

		public override void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE [map].[tblCompanyPersonnelAssignedToCompany] " +
				 "SET PersonnelGuid = @AssignedToGuid, CompanyGuid = @AssignedGuid," +
				 "ID = @MapID," +
				 "UpdatedDate = @UpdatedDate," +
				 "UpdatedBy = @UpdatedBy " +
				 "WHERE " + this.MappingTablePrimaryKeyColumnName + "= @IdentityGuid";

			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedToGuid", this.AssignedToGuid, true)); // true means replace Guid.Empty with NULL
			cmd.Parameters.Add(DataObject.NewGuidParameter("@AssignedGuid", this.AssignedGuid, true));  // true means replace Guid.Empty with NULL
			cmd.Parameters.AddWithValue("@MapID", this.MapID);
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
			cmd.Parameters.AddWithValue("@IdentityGuid", this._IdentityGuid);
		}

		public override void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM [map].[tblCompanyPersonnelAssignedToCompany] WHERE CompanyPersonnelAssignedToCompanyGuid = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
		}

		public override void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
									  "AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
									  "prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
									  "AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
									  "SELECT m.*, " + Environment.NewLine +
									  "p.PersonID AS AssignedToID, " + Environment.NewLine +
									  "c.ID AS AssignedID, " + Environment.NewLine +
									  "c.LockedOut AS LockedOut, " + Environment.NewLine +
									  "c.Name AS AssignedName, " + Environment.NewLine +
									  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
									  "c.City AS AssignedCity, " + Environment.NewLine +
									  "c.State AS AssignedState, " + Environment.NewLine +
									  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
									  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
									  "p.LastName AS AssignedToLastName " + Environment.NewLine +
									  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
									  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
									  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
									  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
									  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
									  " WHERE m.CompanyPersonnelAssignedToCompanyGuid = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
		}

		public override void SelectByGuidsAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
		{
			if (!skipSiteGuid)
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
								" WHERE m.PersonnelGuid = @AssignedToGuid" +
										  " AND m.CompanyGuid = @AssignedGuid" +
										  " AND m.SiteGuid = @SiteGuid";
			}
			else
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
								" WHERE m.PersonnelGuid = @AssignedToGuid" +
										  " AND m.CompanyGuid = @AssignedGuid";// +
										  //" AND m.SiteGuid = @SiteGuid";
			}

			cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
			cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public override void SelectByTypeAndMapIdsql(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
		{
			if (!skipSiteGuid)
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
										  " WHERE m.ID = @MapID" +
										  " AND m.SiteGuid = @SiteGuid";
			}
			else
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
										  " WHERE m.ID = @MapID";// +
										  //" AND m.SiteGuid = @SiteGuid";
			}


			cmd.Parameters.Add("@MapID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MapID"].Value = this.MapID;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public override void EnumerateByAssignedToGuidAndTypeSQL(SqlCommand cmd, bool bInTransaction, bool skipSiteGuid = false)
		{
			if (!skipSiteGuid)
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
									"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
									"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
									"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
											  "SELECT m.*, " + Environment.NewLine +
											  "p.PersonID AS AssignedToID, " + Environment.NewLine +
											  "c.ID AS AssignedID, " + Environment.NewLine +
											  "c.LockedOut AS LockedOut, " + Environment.NewLine +
											  "c.Name AS AssignedName, " + Environment.NewLine +
											  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
											  "c.City AS AssignedCity, " + Environment.NewLine +
											  "c.State AS AssignedState, " + Environment.NewLine +
											  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
											  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
											  "p.LastName AS AssignedToLastName " + Environment.NewLine +
											  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
											  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
											  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
											  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
											  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
											  " WHERE m.PersonnelGuid = @AssignedToGuid" +
											  " AND m.SiteGuid = @SiteGuid" +
											  " ORDER BY m.ID ASC";
			}
			else
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
									"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
									"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
									"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
											  "SELECT m.*, " + Environment.NewLine +
											  "p.PersonID AS AssignedToID, " + Environment.NewLine +
											  "c.ID AS AssignedID, " + Environment.NewLine +
											  "c.LockedOut AS LockedOut, " + Environment.NewLine +
											  "c.Name AS AssignedName, " + Environment.NewLine +
											  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
											  "c.City AS AssignedCity, " + Environment.NewLine +
											  "c.State AS AssignedState, " + Environment.NewLine +
											  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
											  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
											  "p.LastName AS AssignedToLastName " + Environment.NewLine +
											  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
											  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
											  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
											  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
											  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
											  " WHERE m.PersonnelGuid = @AssignedToGuid" +
											  //" AND m.SiteGuid = @SiteGuid" +
											  " ORDER BY m.ID ASC";
			}

			cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		public override void EnumerateByAssignedGuidAndTypeSQL(SqlCommand cmd, SecurityClass security, bool skipSiteGuid = false)
		{
			if (!skipSiteGuid)
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
											 " WHERE m.CompanyGuid = @AssignedGuid" +
											 " AND m.SiteGuid = @SiteGuid" +
											 " ORDER BY m.ID ASC";
			}
			else
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
											 " WHERE m.CompanyGuid = @AssignedGuid" +
											 //" AND m.SiteGuid = @SiteGuid" +
											 " ORDER BY m.ID ASC";
			}

			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}


		public override void EnumerateByTypeSQL(SqlCommand cmd, bool skipSiteGuid = false)
		{
			if (!skipSiteGuid)
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
								" WHERE crv.AssignedToSiteGuid = @SiteGuid" +
										  " ORDER BY AssignedToID, AssignedID";
			}
			else
			{
				cmd.CommandText = "WITH crv (CompanyGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT CompanyGuid, MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid)), " + Environment.NewLine +
								"prv (PersonnelGuid, MasterRecordGuid) " + Environment.NewLine +
								"AS (SELECT PersonnelGuid, MasterRecordGuid FROM[erv].[udf_GetPersonnelRecordVersions](@SiteGuid)) " + Environment.NewLine +
										  "SELECT m.*, " + Environment.NewLine +
										  "p.PersonID AS AssignedToID, " + Environment.NewLine +
										  "c.ID AS AssignedID, " + Environment.NewLine +
										  "c.LockedOut AS LockedOut, " + Environment.NewLine +
										  "c.Name AS AssignedName, " + Environment.NewLine +
										  " c.Address1 AS AssignedAddress, " + Environment.NewLine +
										  "c.City AS AssignedCity, " + Environment.NewLine +
										  "c.State AS AssignedState, " + Environment.NewLine +
										  "p.FirstName AS AssignedToFirstName, " + Environment.NewLine +
										  "p.MiddleName AS AssignedToMiddleName, " + Environment.NewLine +
										  "p.LastName AS AssignedToLastName " + Environment.NewLine +
										  "FROM map.tblCompanyPersonnelAssignedToCompany m " + Environment.NewLine +
										  "inner join prv on m.PersonnelGuid = prv.PersonnelGuid " + Environment.NewLine +
										  "inner join dbo.tblPersonnel p on prv.PersonnelGuid = p.PersonnelGuid " + Environment.NewLine +
										  "inner join crv on m.CompanyGuid = crv.CompanyGuid " + Environment.NewLine +
										  "inner join dbo.tblCompanies c on crv.CompanyGuid = c.CompanyGuid " + Environment.NewLine +
								//" WHERE crv.AssignedToSiteGuid = @SiteGuid" +
										  " ORDER BY AssignedToID, AssignedID";
			}

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}
	}
}
