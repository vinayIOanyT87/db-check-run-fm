using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
    using FMCore;

   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(TestSetClass))]
	public class TestSetCollectionClass : List<TestSetClass> { }

   [Serializable]
   [DataContract]
	[KnownType(typeof(GregorianCalendar))]
	public class TestSetClass : BaseDataObject
	{
		#region Public Constants
		public const string ENTITY_TYPE_ID = "Test Set";
		#endregion

		#region public data members
		[DataMember]
		public TestCollectionClass testCollection = new TestCollectionClass();
		#endregion public data members

		[DataMember]
		public bool Flag01 { get; set; }

		#region Constructors
		public TestSetClass()
		{
			this.Init();
		}
		#endregion Constructors

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST_SET;
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

		private void Init()
		{
			this.Reset();
			this.Flag01 = false;
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

			_IdentityGuid = DataObject.getValue<Guid>(row["TestSetDefinitionGuid"], Guid.Empty);
			_SiteGuid = DataObject.getValue<Guid>(row["OwnerSiteGuid"], Guid.Empty);
			_ID = DataObject.getValue<string>(row["TestSetName"], "");
			_CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			this.Flag01 = DataObject.getValue<bool>(row["Flag01"], false);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTestSetDefinitions (" +
				"TestSetName," +
				"OwnerSiteGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"Flag01, " +
				"TestSetDefinitionGuid" +
				") VALUES (" +
				"@TestSetName," +
				"@OwnerSiteGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@Flag01, " +
				"@TestSetDefinitionGuid)";

			cmd.Parameters.AddWithValue("@TestSetName", _ID);
			cmd.Parameters.AddWithValue("@OwnerSiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@Flag01", this.Flag01);
			cmd.Parameters.AddWithValue("@TestSetDefinitionGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTestSetDefinitions SET " +
				"TestSetName = @TestSetName, " +
				"OwnerSiteGuid = @OwnerSiteGuid, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"Flag01 = @Flag01 " +
				"WHERE [TestSetDefinitionGuid] = @TestSetDefinitionGuid";

			cmd.Parameters.AddWithValue("@TestSetName", _ID);
			cmd.Parameters.AddWithValue("@OwnerSiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@Flag01", this.Flag01);
			cmd.Parameters.AddWithValue("@TestSetDefinitionGuid", _IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTestSetDefinitions WHERE TestSetDefinitionGuid = @TestSetDefinitionGuid";
			cmd.Parameters.AddWithValue("@TestSetDefinitionGuid", _IdentityGuid);
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
			const string ParamNameSiteguid1		= "@SiteGuid1";
			const SqlDbType ParamTypeSiteguid	= SqlDbType.UniqueIdentifier;
			const string ParamNameSiteguid2		= "@SiteGuid2";
			const string ParamNameSiteguid3		= "@SiteGuid3";

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

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblTestSetDefinitions.* FROM tblTestSetDefinitions " + SQLUpdateLock(bInTransaction) + " WHERE TestSetDefinitionGuid = @TestSetDefinitionGuid";
			cmd.Parameters.AddWithValue("@TestSetDefinitionGuid", _IdentityGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, string filter, string order)
		{
			string orderClause = "TestSetName ASC";
			if (order != null)
			{
				orderClause = order;
			}

			if (filter != null)
				filter = FuelsManagerExtensions.EscapeLikeClauseCharacters(filter.Trim());

			cmd.CommandText = "SELECT tblTestSetDefinitions.*  FROM tblTestSetDefinitions " +
					" WHERE" + AppendSiteWhereClause(cmd, security, "tblTestSetDefinitions", "TestSetDefinitionGuid") +
					(string.IsNullOrEmpty(filter) ? string.Empty : " AND TestSetName LIKE '%" + filter + "%' ") +
					" ORDER BY " + orderClause;
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = " SELECT tblTestSetDefinitions.*  FROM tblTestSetDefinitions " + SQLUpdateLock(bInTransaction) +
					" WHERE" + AppendSiteWhereClause(cmd, security, "tblTestSetDefinitions", "TestSetDefinitionGuid") +
					" AND tblTestSetDefinitions.TestSetName = @TestSetName";

			cmd.Parameters.AddWithValue("@TestSetName", _ID);
		}
	}
}
