using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(TestToTestSetMapClass))]
	public class TestToTestSetMapCollectionClass : List<TestToTestSetMapClass> { }

   [Serializable]
   [DataContract]
	[KnownType(typeof(GregorianCalendar))]
	public class TestToTestSetMapClass : BaseDataObject
	{
		#region private data members

		[DataMember]
		private Guid _TestSetDefinitionGuid = Guid.Empty;
		[DataMember]
		private Guid _TestDefinitionGuid = Guid.Empty;

		#endregion private data members

		#region Constructors
		public TestToTestSetMapClass()
		{
			Reset();
		}
		#endregion Constructors

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#region public properties
		public Guid TestSetDefinitionGuid
		{
			get { return _TestSetDefinitionGuid; }
			set { _TestSetDefinitionGuid = value; }
		}

		public Guid TestDefinitionGuid
		{
			get { return _TestDefinitionGuid; }
			set { _TestDefinitionGuid = value; }
		}

		public Guid TestDefinitionToTestSetDefinitionGuid
		{
			get { return _IdentityGuid; }
			set { _IdentityGuid = value; }
		}

		#endregion public properties

		public override void Reset()
		{
			base.Reset();
			_TestDefinitionGuid = Guid.Empty;
			_TestSetDefinitionGuid = Guid.Empty;
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("Set");
			}

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

			_IdentityGuid = DataObject.getValue<Guid>(Row["TestDefinitionToTestSetDefinitionGuid"], Guid.Empty);
			_TestSetDefinitionGuid = DataObject.getValue<Guid>(Row["TestSetDefinitionGuid"], Guid.Empty);
			_TestDefinitionGuid = DataObject.getValue<Guid>(Row["TestDefinitionGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblTestDefinitionToTestSetDefinition (" +
				"TestDefinitionGuid," +
				"TestSetDefinitionGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"TestDefinitionToTestSetDefinitionGuid" +
				") VALUES (" +
				"@TestDefinitionGuid," +
				"@TestSetDefinitionGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@TestDefinitionToTestSetDefinitionGuid)";

			cmd.Parameters.AddWithValue("@TestDefinitionGuid", _TestDefinitionGuid);
			cmd.Parameters.AddWithValue("@TestSetDefinitionGuid", _TestSetDefinitionGuid);
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@TestDefinitionToTestSetDefinitionGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE map.tblTestDefinitionToTestSetDefinition SET " +
				"TestDefinitionGuid = @TestDefinitionGuid, " +
				"TestSetDefinitionGuid = @TestSetDefinitionGuid, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"WHERE TestDefinitionToTestSetDefinitionGuid = @IdentityGuid";

			cmd.Parameters.AddWithValue("TestDefinitionGuid", _TestDefinitionGuid);
			cmd.Parameters.AddWithValue("TestSetDefinitionGuid", _TestSetDefinitionGuid);
			cmd.Parameters.AddWithValue("UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("IdentityGuid", _IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblTestDefinitionToTestSetDefinition WHERE TestDefinitionToTestSetDefinitionGuid = @IdentityGuid";

			cmd.Parameters.AddWithValue("IdentityGuid", _IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT map.tblTestDefinitionToTestSetDefinition.* FROM map.tblTestDefinitionToTestSetDefinition " + 
				SQLUpdateLock(bInTransaction) + " WHERE TestDefinitionToTestSetDefinitionGuid = @IdentityGuid";

			cmd.Parameters.AddWithValue("IdentityGuid", _IdentityGuid);
		}

		public void SelectByTestAndTestSetGuidSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT map.tblTestDefinitionToTestSetDefinition.* FROM map.tblTestDefinitionToTestSetDefinition " + 
				SQLUpdateLock(bInTransaction) +
				" WHERE TestDefinitionGuid = @TestDefinitionGuid " +
				" AND TestSetDefinitionGuid = @TestSetDefinitionGuid";

			cmd.Parameters.AddWithValue("TestDefinitionGuid", _TestDefinitionGuid);
			cmd.Parameters.AddWithValue("TestSetDefinitionGuid", _TestSetDefinitionGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT map.tblTestDefinitionToTestSetDefinition.*  FROM map.tblTestDefinitionToTestSetDefinition " +
					" ORDER BY map.tblTestDefinitionToTestSetDefinition.TestDefinitionToTestSetDefinitionGuid";
		}

		public void EnumerateByTestSetGuidSQL(SqlCommand cmd, SecurityClass security, Guid testSetDefinitionGuid)
		{
			cmd.CommandText = "SELECT map.tblTestDefinitionToTestSetDefinition.*  FROM map.tblTestDefinitionToTestSetDefinition " +
					" WHERE map.tblTestDefinitionToTestSetDefinition.TestSetDefinitionGuid = @TestSetDefinitionGuid" +
					" ORDER BY map.tblTestDefinitionToTestSetDefinition.TestDefinitionToTestSetDefinitionGuid";

			cmd.Parameters.AddWithValue("TestSetDefinitionGuid", testSetDefinitionGuid);
		}

		public void EnumerateByTestGuidSQL(SqlCommand cmd, SecurityClass security, Guid testDefinitionGuid)
		{
			cmd.CommandText = "SELECT map.tblTestDefinitionToTestSetDefinition.*  FROM map.tblTestDefinitionToTestSetDefinition " +
					" WHERE map.tblTestDefinitionToTestSetDefinition.TestDefinitionGuid = @TestDefinitionGuid " +
					" ORDER BY map.tblTestDefinitionToTestSetDefinition.TestDefinitionToTestSetDefinitionGuid";

			cmd.Parameters.AddWithValue("TestDefinitionGuid", testDefinitionGuid);
		}

	}
}
