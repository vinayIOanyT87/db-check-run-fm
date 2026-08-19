using FMCore;

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Runtime.Serialization;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.UtilityObjects;

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(TestEquipmentResultClass))]
	public class TestEquipmentResultCollectionClass : List<TestEquipmentResultClass>
	{
	}

	[Serializable]
	[DataContract]
	[KnownType(typeof(GregorianCalendar))]
	public class TestEquipmentResultClass : BaseDataObject
	{
		#region protected data members
		[DataMember]
		protected Guid _TestSetEquipmentResultGuid;
		[DataMember]
		protected string _TestName;
		[DataMember]
		protected string _Measurement;
		[DataMember]
		protected DateTimeOffset _TestDate;
		[DataMember]
		protected TESTSET_STATUS _Status;
		[DataMember]
		protected string _PerformedBy;
		[DataMember]
		protected string _Supervisor;
		[DataMember]
		protected bool flag01;
        [DataMember]
        public string TestCode { get; set; }
        #endregion protected data members

		#region public properties
		public Guid TestEquipmentResultGuid
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
		public Guid TestSetEquipmentResultGuid
		{
			get
			{
				return _TestSetEquipmentResultGuid;
			}
			set
			{
				_TestSetEquipmentResultGuid = value;
			}
		}
		public string TestName
		{
			get
			{
				return _TestName;
			}
			set
			{
				_TestName = value;
			}
		}
		public string Measurement
		{
			get
			{
				return _Measurement;
			}
			set
			{
				_Measurement = value;
			}
		}
		public DateTimeOffset TestDate
		{
			get
			{
				return _TestDate;
			}
			set
			{
				_TestDate = value;
			}
		}
		public TESTSET_STATUS Status
		{
			get
			{
				return _Status;
			}
			set
			{
				_Status = value;
			}
		}
		public string PerformedBy
		{
			get
			{
				return _PerformedBy;
			}
			set
			{
				_PerformedBy = value;
			}
		}
		public string Supervisor
		{
			get
			{
				return _Supervisor;
			}
			set
			{
				_Supervisor = value;
			}
		}

		public bool Flag01
		{
			get { return this.flag01; }
			set { this.flag01 = value; }
		}
        #endregion public properties

		#region Constructors
		public TestEquipmentResultClass()
		{
			this.Init();
		}
		#endregion Constructors

		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST_EQUIPMENT_RESULT;
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
			this._TestSetEquipmentResultGuid	= Guid.Empty;
			this._TestName						= string.Empty;
			this._PerformedBy					= string.Empty;
			this._Supervisor					= string.Empty;
			this.flag01							= false;
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

			_IdentityGuid				= DataObject.getValue<Guid>(row["TestEquipmentResultGuid"], Guid.Empty);
			_TestSetEquipmentResultGuid = DataObject.getValue<Guid>(row["TestSetEquipmentResultGuid"], Guid.Empty);
			_TestName					= DataObject.getValue<string>(row["TestName"], string.Empty);
			_Measurement				= DataObject.getValue<string>(row["Measurement"], string.Empty);
			_TestDate					= DataObject.getValue<DateTimeOffset>(row["TestDate"], TimeConverter.MinFMDate);
			_Status						= DataObject.getValue<TESTSET_STATUS>(row["LookupTestSetStatusIndex"], TESTSET_STATUS.Pending);

			_Deleted		= DataObject.getValue<bool>(row["DeleteFlag"], false);
			_CreatedDate	= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy		= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			_UpdatedDate	= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
			_UpdatedBy		= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);

			_PerformedBy	= DataObject.getValue<string>(row["PerformedBy"], string.Empty);
			_Supervisor		= DataObject.getValue<string>(row["Supervisor"], string.Empty);
			this.flag01		= DataObject.getValue<bool>(row["Flag01"], false);
            TestCode        = DataObject.getValue<string>(row["TestCode"], string.Empty);
        }

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTestEquipmentResults (" +
				"TestSetEquipmentResultGuid," +
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
				"Flag01," +
				"TestEquipmentResultGuid" +
				") VALUES (" +
				"@TestSetEquipmentResultGuid," +
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
				"@TestEquipmentResultGuid)";

			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", _TestSetEquipmentResultGuid);
			cmd.Parameters.AddWithValue("@TestName", _TestName);
			cmd.Parameters.AddWithValue("@Measurement", _Measurement);
			cmd.Parameters.AddWithValue("@TestDate",  _TestDate);
			cmd.Parameters.AddWithValue("@LookupTestSetStatusIndex", ((int) _Status));
			cmd.Parameters.AddWithValue("@DeleteFlag", (_Deleted ? 1 : 0));
            cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
            cmd.Parameters.AddWithValue("@TestCode", this.TestCode.DefaultIfNull(String.Empty));
            cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@PerformedBy", _PerformedBy);
			cmd.Parameters.AddWithValue("@Supervisor", _Supervisor);
			cmd.Parameters.AddWithValue("@Flag01", this.flag01);
			cmd.Parameters.AddWithValue("@TestEquipmentResultGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTestEquipmentResults SET " +
				"TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid, " +
				"TestName = @TestName, " +
				"Measurement = @Measurement, " +
				"TestDate = @TestDate, " +
				"LookupTestSetStatusIndex = @LookupTestSetStatusIndex, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"PerformedBy = @PerformedBy, " +
				"Supervisor = @Supervisor, " +
                "TestCode = @TestCode, " +
                "Flag01 = @Flag01 " +
				"WHERE TestEquipmentResultGuid = @TestEquipmentResultGuid";

			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", _TestSetEquipmentResultGuid);
			cmd.Parameters.AddWithValue("@TestName", _TestName);
			cmd.Parameters.AddWithValue("@Measurement", _Measurement);
			cmd.Parameters.AddWithValue("@TestDate", _TestDate);
			cmd.Parameters.AddWithValue("@LookupTestSetStatusIndex", ((int) _Status));
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@PerformedBy", _PerformedBy);
			cmd.Parameters.AddWithValue("@Supervisor", _Supervisor);
            cmd.Parameters.AddWithValue("@TestCode", this.TestCode);
            cmd.Parameters.AddWithValue("@Flag01", this.flag01);
            cmd.Parameters.AddWithValue("@TestEquipmentResultGuid", _IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTestEquipmentResults WHERE TestEquipmentResultGuid = @TestEquipmentResultGuid";
			cmd.Parameters.AddWithValue("@TestEquipmentResultGuid", _IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblTestEquipmentResults.* FROM tblTestEquipmentResults " + SQLUpdateLock(bInTransaction) + " WHERE TestEquipmentResultGuid = @TestEquipmentResultGuid";
			cmd.Parameters.AddWithValue("@TestEquipmentResultGuid", _IdentityGuid);
		}

		public void SelectByTestSetEquipmentResultGuidSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblTestEquipmentResults.* FROM tblTestEquipmentResults " + SQLUpdateLock(bInTransaction) +
					" WHERE TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid";
			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", _TestSetEquipmentResultGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblTestEquipmentResults.* FROM tblTestEquipmentResults " +
					"ORDER BY tblTestEquipmentResults.TestEquipmentResultGuid";
		}

		public void EnumerateByTestSetEquipmentResultGuidSQL(SqlCommand cmd, SecurityClass security, Guid testSetEquipmentResultGuid)
		{
			cmd.CommandText = "SELECT tblTestEquipmentResults.* FROM tblTestEquipmentResults " +
					"WHERE tblTestEquipmentResults.TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid " +
					"ORDER BY tblTestEquipmentResults.TestEquipmentResultGuid";
			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", testSetEquipmentResultGuid);
		}
	}
}
