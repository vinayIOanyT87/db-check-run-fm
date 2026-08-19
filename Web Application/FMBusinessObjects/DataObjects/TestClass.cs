using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
    using FMCore;

   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(TestClass))]
	public class TestCollectionClass : List<TestClass> { }

   [Serializable]
   [DataContract]
	[KnownType(typeof(GregorianCalendar))]
	public class TestClass : BaseDataObject
	{
		#region Public Constants
		public const string ENTITY_TYPE_ID = "Test";
		#endregion

		#region public data members
		public bool AuditLog = false;
		#endregion

		#region constuctors

		public TestClass()
		{
			Reset();
		}

		#endregion

		#region Properties

		[DataMember]
		public string MeasurementUnit { get; set; }
		[DataMember]
		public string ValidationRule { get; set; }
		[DataMember]
		public float SampleSize { get; set; }
		[DataMember]
		public string TestCode { get; set; }
		[DataMember]
		public string TestMethod { get; set; }
		[DataMember]
		public string ProductID { get; set; }

		

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		#endregion

		public override void Reset()
		{
			this.Init();
		}

		private void Init()
		{
			base.Reset();
			MeasurementUnit = string.Empty;
			ValidationRule = string.Empty;
			SampleSize = 0;
			TestCode = "";
			TestMethod = "";
			ProductID = "";
		}

		public void Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			this.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			_IdentityGuid = DataObject.getValue(row["TestDefinitionGuid"], Guid.Empty);
			_SiteGuid = DataObject.getValue(row["OwnerSiteGuid"], Guid.Empty);
			_ID = DataObject.getValue(row["TestName"], "");
			MeasurementUnit = DataObject.getValue(row["MeasurementUnit"], "");
			ValidationRule = DataObject.getValue(row["ValidationRule"], "");
			SampleSize = (float) DataObject.getValue(row["SampleSize"], 0.0);
			TestCode = DataObject.getValue(row["TestCode"], string.Empty);
			TestMethod = DataObject.getValue(row["TestMethod"], string.Empty);
			ProductID = DataObject.getValue(row["ProductID"], string.Empty);
			_CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue(row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTestDefinitions (" +
				"TestName," +
				"OwnerSiteGuid," +
				"MeasurementUnit," +
				"ValidationRule," +
				"SampleSize," +
				"TestCode," +
				"TestMethod," +
				"ProductID," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"TestDefinitionGuid" +
				") VALUES (" +
				"@TestName," +
				"@OwnerSiteGuid," +
				"@MeasurementUnit," +
				"@ValidationRule," +
				"@SampleSize," +
				"@TestCode," +
				"@TestMethod," +
				"@ProductID," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@TestDefinitionGuid)";

			cmd.Parameters.AddWithValue("@TestName", _ID);
			cmd.Parameters.AddWithValue("@OwnerSiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@MeasurementUnit", MeasurementUnit);
			cmd.Parameters.AddWithValue("@ValidationRule", ValidationRule);
			cmd.Parameters.AddWithValue("@SampleSize", SampleSize);
			cmd.Parameters.AddWithValue("@TestCode", TestCode);
			cmd.Parameters.AddWithValue("@TestMethod", TestMethod);
			cmd.Parameters.AddWithValue("@ProductID", ProductID);
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@TestDefinitionGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTestDefinitions SET " +
				"TestName = @TestName, " +
				"OwnerSiteGuid = @OwnerSiteGuid, " +
				"MeasurementUnit = @MeasurementUnit, " +
				"ValidationRule = @ValidationRule, " +
				"SampleSize = @SampleSize, " +
				"TestCode = @TestCode," +
				"TestMethod = @TestMethod," +
				"ProductID = @ProductID," +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy " +
				"WHERE TestDefinitionGuid = @TestDefinitionGuid";

			cmd.Parameters.AddWithValue("@TestName", _ID);
			cmd.Parameters.AddWithValue("@OwnerSiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@MeasurementUnit", MeasurementUnit);
			cmd.Parameters.AddWithValue("@ValidationRule", ValidationRule);
			cmd.Parameters.AddWithValue("@SampleSize", SampleSize);
			cmd.Parameters.AddWithValue("@TestCode", TestCode);
			cmd.Parameters.AddWithValue("@TestMethod", TestMethod);
			cmd.Parameters.AddWithValue("@ProductID", ProductID);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@TestDefinitionGuid", _IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTestDefinitions WHERE TestDefinitionGuid = @TestDefinitionGuid";
			cmd.Parameters.AddWithValue("@TestDefinitionGuid", _IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblTestDefinitions.* FROM tblTestDefinitions " + SQLUpdateLock(bInTransaction) + " WHERE TestDefinitionGuid = @TestDefinitionGuid";
			cmd.Parameters.AddWithValue("@TestDefinitionGuid", _IdentityGuid);
		}

		/// <summary>
		/// Differs from the base implementation:
		/// (1) No "WITH(NOLOCK)"
		/// (2) Uses OwnerSiteGuid instead of SiteGuid in first part of AND clause
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="entityTable"></param>
		/// <param name="entityGuidColumn"></param>
		new virtual public string AppendSiteWhereClause(SqlCommand cmd, SecurityClass security, string entityTable, string entityGuidColumn)
		{
			const string ParamNameSiteguid1 = "@SiteGuid1";
			const SqlDbType ParamTypeSiteguid = SqlDbType.UniqueIdentifier;
			const string ParamNameSiteguid2 = "@SiteGuid2";
			const string ParamNameSiteguid3 = "@SiteGuid3";

			string sql = " (" + entityTable + "." + entityGuidColumn +
						 " IN (SELECT " + entityGuidColumn +
						 " FROM " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) +
						 " "
						 + DataObject.AddParameter(cmd, "WHERE", "SiteGuid", "=", ParamNameSiteguid1, ParamTypeSiteguid, security.SiteGuid) +
						 ")";



			if (security.SiteGuid == security.LoginSiteGuid)
			{
				sql += ")";
			}
			else
			{
				sql += " AND (" +
							DataObject.AddParameter(cmd, false, entityTable + ".OwnerSiteGuid", ParamNameSiteguid2, ParamTypeSiteguid, security.SiteGuid) +
							" OR " + entityTable + "." + entityGuidColumn +
							" IN (SELECT " + entityGuidColumn + " FROM " + EntityToSiteMapClass.GetMappingTableName(EntityType) +
							" " +
							DataObject.AddParameter(cmd, "WHERE", "SiteGuid", "=", ParamNameSiteguid3, ParamTypeSiteguid, security.LoginSiteGuid) +
							")))";
			}

			return sql;
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, string filter, string order)
		{
			string whereFilter = "";

			if (filter != null)
			{
				filter = FuelsManagerExtensions.EscapeLikeClauseCharacters(filter.Trim());
				whereFilter = " AND (TestName LIKE '%" + filter +
					"%' OR MeasurementUnit LIKE '%" + filter +
					"%' OR ValidationRule LIKE '%" + filter +
					"%' ";

				double filterAsDouble;
				if (double.TryParse(filter, out filterAsDouble))
					whereFilter += " OR SampleSize = " + filter;
				whereFilter += ")";
			}

			string orderClause = "TestName ASC";

			if (order != null)
			{
				orderClause = order;
			}

			cmd.CommandText = "SELECT tblTestDefinitions.*  FROM tblTestDefinitions " +
					" WHERE" + AppendSiteWhereClause(cmd, security, "tblTestDefinitions", "TestDefinitionGuid") + whereFilter +
					" ORDER BY " + orderClause;
		}

		public void IsAssociatedWithTestResultSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = @"SELECT COUNT(*) CNT  FROM tblTestDefinitions 
					 WHERE EXISTS(SELECT * FROM tblTestEquipmentResults  WHERE TestName=tblTestDefinitions.TestName
					UNION SELECT * FROM tblTestTankResults  WHERE TestName=tblTestDefinitions.TestName) 
					AND TestDefinitionGuid = @TestDefinitionGuid";

			cmd.Parameters.AddWithValue("@TestDefinitionGuid", _IdentityGuid);
		}

		public void EnumerateByTestSetGuidSQL(SqlCommand cmd, SecurityClass security, Guid testSetDefinitionGuid)
		{
			cmd.CommandText = "SELECT tblTestDefinitions.*  FROM tblTestDefinitions LEFT JOIN  map.tblTestDefinitionToTestSetDefinition" +
					" ON map.tblTestDefinitionToTestSetDefinition.TestDefinitionGuid = tblTestDefinitions.TestDefinitionGuid " +
					" WHERE map.tblTestDefinitionToTestSetDefinition.TestSetDefinitionGuid = @TestSetDefinitionGuid" +
					" ORDER BY tblTestDefinitions.TestName";

			cmd.Parameters.AddWithValue("@TestSetDefinitionGuid", testSetDefinitionGuid);
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = " SELECT tblTestDefinitions.*  FROM tblTestDefinitions " + SQLUpdateLock(bInTransaction) +
					" WHERE" + AppendSiteWhereClause(cmd, security, "tblTestDefinitions", "TestDefinitionGuid") +
					" AND tblTestDefinitions.TestName = @TestName";

			cmd.Parameters.AddWithValue("@TestName", _ID);
		}
	}
}