using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(TestTankResultClass))]
	public class TestTankResultCollectionClass : List<TestTankResultClass>
	{
	}

	[Serializable]
	[DataContract]
	[KnownType(typeof(GregorianCalendar))]
	public class TestTankResultClass : BaseDataObject
	{
		#region Private data members
		[DataMember]
		private Guid testSetTankResultGuid;
		[DataMember]
		private string testName;
		[DataMember]
		private string measurement;
		[DataMember]
		private DateTimeOffset testDate;
		[DataMember]
		private TESTSET_STATUS status;
		[DataMember]
		private string performedBy;
		[DataMember]
		private string supervisor;
		[DataMember]
		private bool flag01;
        [DataMember]
        public string TestCode { get; set; }
        #endregion protected data members

		#region Public properties
		public Guid TestTankResultGuid
		{
			get
			{
				return _IdentityGuid;
			}
			set
			{
				_IdentityGuid = value;
			}
		}

		public Guid TestSetTankResultGuid
		{
			get
			{
				return this.testSetTankResultGuid;
			}
			set
			{
				this.testSetTankResultGuid = value;
			}
		}

		public string TestName
		{
			get
			{
				return this.testName;
			}
			set
			{
				this.testName = value;
			}
		}

		public string Measurement
		{
			get
			{
				return this.measurement;
			}
			set
			{
				this.measurement = value;
			}
		}

		public DateTimeOffset TestDate
		{
			get
			{
				return this.testDate;
			}
			set
			{
				this.testDate = value;
			}
		}

		public TESTSET_STATUS Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public string PerformedBy
		{
			get
			{
				return this.performedBy;
			}
			set
			{
				this.performedBy = value;
			}
		}

		public string Supervisor
		{
			get
			{
				return this.supervisor;
			}
			set
			{
				this.supervisor = value;
			}
		}

		public bool Flag01
		{
			get
			{
				return this.flag01;
			}
			set
			{
				this.flag01 = value;
			}
		}
		#endregion Public properties

		#region Constructors
		public TestTankResultClass()
		{
			this.Init();
		}
		#endregion Constructors

		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST_TANK_RESULT;
			}
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		public override void Reset()
		{
			this.Init();
		}

		private void Init()
		{
			base.Reset();
			this.testSetTankResultGuid	= Guid.Empty;
			this.testName				= string.Empty;
			this.performedBy			= string.Empty;
			this.supervisor				= string.Empty;
			this.flag01					= false;
            TestCode = "";
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

			this._IdentityGuid			= DataObject.getValue<Guid>(row["TestTankResultGuid"], Guid.Empty);
			this.testSetTankResultGuid	= DataObject.getValue<Guid>(row["TestSetTankResultGuid"], Guid.Empty);
			this.testName				= DataObject.getValue<string>(row["TestName"], "");
			this.measurement			= DataObject.getValue<string>(row["Measurement"], "");
			this.testDate				= DataObject.getValue<DateTimeOffset>(row["TestDate"], TimeConverter.MinFMDate);
			this.status					= DataObject.getValue<TESTSET_STATUS>(row["LookupTestSetStatusIndex"], TESTSET_STATUS.Pending);
			this.flag01					= DataObject.getValue<bool>(row["Flag01"], false);

			this._Deleted		= DataObject.getValue<bool>(row["DeleteFlag"], false);
			this._CreatedDate	= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy		= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this._UpdatedDate	= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
			this._UpdatedBy		= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			this.performedBy	= DataObject.getValue<string>(row["PerformedBy"], "");
			this.supervisor		= DataObject.getValue<string>(row["Supervisor"], "");
            TestCode            = DataObject.getValue<string>(row["TestCode"], "");
        }

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTestTankResults (" +
				"TestSetTankResultGuid," +
				"TestName," +
				"Measurement," +
				"TestDate," +
				"LookupTestSetStatusIndex," +
				"DeleteFlag," +
                "TestCode," +
                "CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"PerformedBy," +
				"Supervisor," +
				"Flag01, " +
				"TestTankResultGuid" +
				") VALUES (" +
				"@TestSetTankResultGuid," +
				"@TestName," +
				"@Measurement," +
				"@TestDate," +
				"@LookupTestSetStatusIndex," +
				"@DeleteFlag," +
                "@TestCode," +
                "@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@PerformedBy," +
				"@Supervisor," +
				"@Flag01, " +
				"@TestTankResultGuid)";

			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", this.testSetTankResultGuid);
			cmd.Parameters.AddWithValue("@TestName", this.testName);
			cmd.Parameters.AddWithValue("@Measurement", this.measurement);
			cmd.Parameters.AddWithValue("@TestDate", this.testDate);
			cmd.Parameters.AddWithValue("@LookupTestSetStatusIndex", ((int) this.status));
			cmd.Parameters.AddWithValue("@DeleteFlag", (this._Deleted ? 1 : 0));
            cmd.Parameters.AddWithValue("@TestCode", this.TestCode);
            cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@PerformedBy", this.performedBy);
			cmd.Parameters.AddWithValue("@Supervisor", this.supervisor);
			cmd.Parameters.AddWithValue("@Flag01", (this.flag01 ? 1 : 0));
			cmd.Parameters.AddWithValue("@TestTankResultGuid", this._IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTestTankResults SET " +
				"TestSetTankResultGuid = @TestSetTankResultGuid, " +
				"TestName = @TestName, " +
				"Measurement = @Measurement, " +
				"TestDate = @TestDate, " +
				"LookupTestSetStatusIndex = @LookupTestSetStatusIndex, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"PerformedBy = @PerformedBy, " +
				"Supervisor = @Supervisor, " +
                "TestCode = @TestCode, " +
                "Flag01 = @Flag01, " +
				"WHERE TestTankResultGuid = @TestTankResultGuid";

			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", this.testSetTankResultGuid);
			cmd.Parameters.AddWithValue("@TestName", this.testName);
			cmd.Parameters.AddWithValue("@Measurement", this.measurement);
			cmd.Parameters.AddWithValue("@TestDate", this.testDate);
			cmd.Parameters.AddWithValue("@LookupTestSetStatusIndex", ((int) this.status));
			cmd.Parameters.AddWithValue("@DeleteFlag", (this._Deleted ? 1 : 0));
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@PerformedBy", this.performedBy);
            cmd.Parameters.AddWithValue("@Supervisor)", this.supervisor);
            cmd.Parameters.AddWithValue("@TestCode)", this.TestCode);
            cmd.Parameters.AddWithValue("@Flag01", (this.flag01 ? 1 : 0));
			cmd.Parameters.AddWithValue("@TestTankResultGuid", this._IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTestTankResults WHERE TestTankResultGuid = @TestTankResultGuid";
			cmd.Parameters.AddWithValue("@TestTankResultGuid", _IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblTestTankResults.* FROM tblTestTankResults " + SQLUpdateLock(inTransaction) + " WHERE TestTankResultGuid = @TestTankResultGuid";
			cmd.Parameters.AddWithValue("@TestTankResultGuid", this._IdentityGuid);
		}

		public void SelectByTestSetTankResultGuidSQL(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblTestTankResults.* FROM tblTestTankResults " + SQLUpdateLock(inTransaction) +
					" WHERE TestSetTankResultGuid = @TestSetTankResultGuid";
			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", this.testSetTankResultGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblTestTankResults.* FROM tblTestTankResults " +
					"ORDER BY tblTestTankResults.TestTankResultGuid";
		}

		public void EnumerateByTestSetTankResultGuidSQL(SqlCommand cmd, SecurityClass security, Guid inTestSetTankResultGuid)
		{
			cmd.CommandText = "SELECT tblTestTankResults.* FROM tblTestTankResults " +
					"WHERE tblTestTankResults.TestSetTankResultGuid = @TestSetTankResultGuid " +
					"ORDER BY tblTestTankResults.TestTankResultGuid";
			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", inTestSetTankResultGuid);
		}
	}
}
