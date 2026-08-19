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
	[KnownType(typeof(TestSetTankResultClass))]
	public class TestSetTankResultCollectionClass : List<TestSetTankResultClass>
	{
	}

	[Serializable]
	[DataContract]
	[KnownType(typeof(GregorianCalendar))]
	public class TestSetTankResultClass : BaseDataObject
	{
		#region Private data members
		[DataMember]
		private DateTimeOffset resultTimeStamp;
		[DataMember]
		private string testSetName;
		[DataMember]
		private string inspector;
		[DataMember]
		private string supervisor;
		[DataMember]
		private Guid tankGuid;
		[DataMember]
		private string tankID;
		[DataMember]
		private int sampleNumber;
		[DataMember]
		private double sampleSize;
		[DataMember]
		private TESTSET_STATUS status;
		[DataMember]
		private bool isRetest;
		[DataMember]
		private int previousSampleNumber;
		[DataMember]
		private string documentNumber;
		[DataMember]
		private string memo;
		[DataMember]
		private double gallonsRepresented;
		[DataMember]
		private bool overrideFlag;
		[DataMember]
		private bool flag01;
		[DataMember]
		private string userData01;
		[DataMember]
		private string userData02;
		#endregion Protected data members

		#region Public properties
		public Guid TestSetTankResultGuid
		{
			get
			{
				return this._IdentityGuid;
			}
			set
			{
				this._IdentityGuid = value;
			}
		}

		public DateTimeOffset ResultTimeStamp
		{
			get
			{
				return this.resultTimeStamp;
			}
			set
			{
				this.resultTimeStamp = value;
			}
		}

		public string TestSetName
		{
			get
			{
				return this.testSetName;
			}
			set
			{
				this.testSetName = value;
			}
		}

		public string Inspector
		{
			get
			{
				return this.inspector;
			}
			set
			{
				this.inspector = value;
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

		public Guid TankGuid
		{
			get
			{
				return this.tankGuid;
			}
			set
			{
				this.tankGuid = value;
			}
		}

		public string TankID
		{
			get
			{
				return this.tankID;
			}
			set
			{
				this.tankID = value;
			}
		}

		public int SampleNumber
		{
			get
			{
				return this.sampleNumber;
			}
			set
			{
				this.sampleNumber = value;
			}
		}

		public double SampleSize
		{
			get
			{
				return this.sampleSize;
			}
			set
			{
				this.sampleSize = value;
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

		public bool IsRetest
		{
			get
			{
				return this.isRetest;
			}
			set
			{
				this.isRetest = value;
			}
		}

		public int PreviousSampleNumber
		{
			get
			{
				return this.previousSampleNumber;
			}
			set
			{
				this.previousSampleNumber = value;
			}
		}

		public string DocumentNumber
		{
			get
			{
				return this.documentNumber;
			}
			set
			{
				this.documentNumber = value;
			}
		}

		public string Memo
		{
			get
			{
				return this.memo;
			}
			set
			{
				this.memo = value;
			}
		}

		public double GallonsRepresented
		{
			get
			{
				return this.gallonsRepresented;
			}
			set
			{
				this.gallonsRepresented = value;
			}
		}

		public bool Override
		{
			get
			{
				return this.overrideFlag;
			}
			set
			{
				this.overrideFlag = value;
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

		public string UserData01
		{
			get
			{
				return this.userData01;
			}
			set
			{
				this.userData01 = value;
			}
		}

		public string UserData02
		{
			get
			{
				return this.userData02;
			}
			set
			{
				this.userData02 = value;
			}
		}
		#endregion Public properties

		#region Public data member
		[DataMember]
		public TestTankResultCollectionClass TestTankResultCollection = new TestTankResultCollectionClass();
		#endregion Public data memeber

		#region Constructors
		public TestSetTankResultClass()
		{
			this.Init();
		}
		#endregion Constructors

		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST_SET_TANK_RESULT;
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
			this.documentNumber = string.Empty;
			this.tankGuid		= Guid.Empty;
			this.tankID			= string.Empty;
			this.testSetName	= string.Empty;
			this.flag01			= false;
			this.userData01		= string.Empty;
			this.userData02		= string.Empty;
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

			this._IdentityGuid			= DataObject.getValue<Guid>(row["TestSetTankResultGuid"], Guid.Empty);
			this._SiteGuid				= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.resultTimeStamp		= DataObject.getValue<DateTimeOffset>(row["ResultTimeStamp"], DateTimeOffset.Now);
			this.testSetName			= DataObject.getValue<string>(row["TestSetName"], "");
			this.inspector				= DataObject.getValue<string>(row["Inspector"], "");
			this.supervisor				= DataObject.getValue<string>(row["Supervisor"], "");
			this.tankGuid				= DataObject.getValue<Guid>(row["TankGuid"], Guid.Empty);
			this.tankID					= DataObject.getValue<string>(row["TankID"], "");
			this.sampleNumber			= DataObject.getValue<int>(row["SampleNumber"], 0);
			this.sampleSize				= DataObject.getValue<double>(row["SampleSize"], 0.0);
			this.status					= DataObject.getValue<TESTSET_STATUS>(row["LookupTestSetStatusIndex"], TESTSET_STATUS.Pending);
			this.isRetest				= DataObject.getValue<bool>(row["IsRetest"], false);
			this.previousSampleNumber	= DataObject.getValue<int>(row["PreviousSampleNumber"], 0);
			this.documentNumber			= DataObject.getValue<string>(row["DocumentNumber"], "");
			this.memo					= DataObject.getValue<string>(row["Memo"], "");
			this.gallonsRepresented		= DataObject.getValue<double>(row["GallonsRepresented"], 0.0);
			this.overrideFlag			= DataObject.getValue<bool>(row["Override"], false);
			this.flag01					= DataObject.getValue<bool>(row["Flag01"], false);
			this.userData01				= DataObject.getValue<string>(row["UserData01"], string.Empty);
			this.userData02				= DataObject.getValue<string>(row["UserData02"], string.Empty);

			this._Deleted		= DataObject.getValue<bool>(row["DeleteFlag"], false);
			this._CreatedDate	= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy		= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this._UpdatedDate	= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
			this._UpdatedBy		= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);

		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTestSetTankResults (" +
				"SiteGuid," +
				"ResultTimeStamp," +
				"TestSetName," +
				"Inspector," +
				"Supervisor," +
				"TankGuid," +
				"TankID," +
				"SampleNumber," +
				"SampleSize," +
				"LookupTestSetStatusIndex," +
				"IsRetest," +
				"PreviousSampleNumber," +
				"DocumentNumber," +
				"Memo," +
				"GallonsRepresented," +
				"Override," +
				"Flag01, " +
				"UserData01, " +
				"UserData02, " +
				"DeleteFlag," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"TestSetTankResultGuid" +
				") VALUES (" +
				"@SiteGuid," +
				"@ResultTimeStamp," +
				"@TestSetName," +
				"@Inspector," +
				"@Supervisor," +
				"@TankGuid," +
				"@TankID," +
				"@SampleNumber," +
				"@SampleSize," +
				"@LookupTestSetStatusIndex," +
				"@IsRetest," +
				"@PreviousSampleNumber," +
				"@DocumentNumber," +
				"@Memo," +
				"@GallonsRepresented," +
				"@Override," +
				"@Flag01, " +
				"@UserData01, " +
				"@UserData02, " +
				"@DeleteFlag," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@TestSetTankResultGuid)";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ResultTimeStamp", this.resultTimeStamp);
			cmd.Parameters.AddWithValue("@TestSetName", this.testSetName);
			cmd.Parameters.AddWithValue("@Inspector", this.inspector);
			cmd.Parameters.AddWithValue("@Supervisor", this.supervisor);
			cmd.Parameters.AddWithValue("@TankGuid", this.tankGuid);
			cmd.Parameters.AddWithValue("@TankID", this.tankID);
			cmd.Parameters.AddWithValue("@SampleNumber", this.sampleNumber);
			cmd.Parameters.AddWithValue("@SampleSize", this.sampleSize);
			cmd.Parameters.AddWithValue("@LookupTestSetStatusIndex", ((int) this.status));
			cmd.Parameters.AddWithValue("@IsRetest", (this.isRetest ? 1 : 0));
			cmd.Parameters.AddWithValue("@PreviousSampleNumber", this.previousSampleNumber);
			cmd.Parameters.AddWithValue("@DocumentNumber", this.documentNumber);
			cmd.Parameters.AddWithValue("@Memo", this.memo);
			cmd.Parameters.AddWithValue("@GallonsRepresented", this.gallonsRepresented);
			cmd.Parameters.AddWithValue("@Override", (this.overrideFlag ? 1 : 0));
			cmd.Parameters.AddWithValue("@Flag01", (this.flag01 ? 1 : 0));
			cmd.Parameters.AddWithValue("@UserData01", this.userData01);
			cmd.Parameters.AddWithValue("@UserData02", this.userData02);
			cmd.Parameters.AddWithValue("@DeleteFlag", (this._Deleted ? 1 : 0));
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", this._IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTestSetTankResults SET " +
				"SiteGuid = @SiteGuid, " +
				"ResultTimeStamp = @ResultTimeStamp, " +
				"TestSetName = @TestSetName, " +
				"Inspector = @Inspector, " +
				"Supervisor = @Supervisor, " +
				"TankGuid = @TankGuid, " +
				"TankID = @TankID, " +
				"SampleNumber = @SampleNumber, " +
				"SampleSize = @SampleSize, " +
				"LookupTestSetStatusIndex = @LookupTestSetStatusIndex, " +
				"IsRetest = @IsRetest, " +
				"PreviousSampleNumber = @PreviousSampleNumber, " +
				"DocumentNumber = @DocumentNumber, " +
				"Memo = @Memo, " +
				"GallonsRepresented = @GallonsRepresented, " +
				"Override = @Override, " +
				"Flag01 = @Flag01, " +
				"UserData01 = @UserData01, " +
				"UserData02 = @UserData02, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy " +
				"WHERE TestSetTankResultGuid = @TestSetTankResultGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ResultTimeStamp", this.resultTimeStamp);
			cmd.Parameters.AddWithValue("@TestSetName", this.testSetName);
			cmd.Parameters.AddWithValue("@Inspector", this.inspector);
			cmd.Parameters.AddWithValue("@Supervisor", this.supervisor);
			cmd.Parameters.AddWithValue("@TankGuid", this.tankGuid);
			cmd.Parameters.AddWithValue("@TankID", this.tankID);
			cmd.Parameters.AddWithValue("@SampleNumber", this.sampleNumber);
			cmd.Parameters.AddWithValue("@SampleSize", this.sampleSize);
			cmd.Parameters.AddWithValue("@LookupTestSetStatusIndex", ((int) this.status));
			cmd.Parameters.AddWithValue("@IsRetest", (this.isRetest ? 1 : 0));
			cmd.Parameters.AddWithValue("@PreviousSampleNumber", this.previousSampleNumber);
			cmd.Parameters.AddWithValue("@DocumentNumber", this.documentNumber);
			cmd.Parameters.AddWithValue("@Memo", this.memo);
			cmd.Parameters.AddWithValue("@GallonsRepresented", this.gallonsRepresented);
			cmd.Parameters.AddWithValue("@Override", (this.overrideFlag ? 1 : 0));
			cmd.Parameters.AddWithValue("@Flag01", (this.flag01 ? 1 : 0));
			cmd.Parameters.AddWithValue("@UserData01", this.userData01);
			cmd.Parameters.AddWithValue("@UserData02", this.userData02);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", this._IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTestSetTankResults WHERE TestSetTankResultGuid = @TestSetTankResultGuid";
			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", this._IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblTestSetTankResults.* FROM tblTestSetTankResults " + SQLUpdateLock(bInTransaction) + " WHERE TestSetTankResultGuid = @TestSetTankResultGuid";
			cmd.Parameters.AddWithValue("@TestSetTankResultGuid", this._IdentityGuid);
		}

		public void SelectByTankGuidSQL(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblTestSetTankResults.* FROM tblTestSetTankResults " + SQLUpdateLock(inTransaction) +
					" WHERE TankGuid = @TankGuid";
			cmd.Parameters.AddWithValue("@TankGuid", this.tankGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			cmd.CommandText = "SELECT tblTestSetTankResults.*,tblSites.ID AS 'SiteID' FROM tblTestSetTankResults " +
					" 	tblTestSetTankResults INNER JOIN tblSites ON tblSites.SiteGuid = tblTestSetTankResults.SiteGuid " +
					"WHERE ResultTimeStamp BETWEEN @StartDate AND @EndDate AND tblTestSetTankResults.SiteGuid = @SiteGuid " +
					"ORDER BY tblTestSetTankResults.TestSetTankResultGuid";

			// We want only the date parts, not the time parts, and we want to add 1 day to End Date
			cmd.Parameters.AddWithValue("@StartDate", TimeConverter.ToDate(startDate));
			cmd.Parameters.AddWithValue("@EndDate", TimeConverter.ToDate(endDate).AddDays(1));
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblTestSetTankResults.* FROM tblTestSetTankResults " +
					"WHERE tblTestSetTankResults.SiteGuid = @SiteGuid " +
					"ORDER BY tblTestSetTankResults.TestSetTankResultGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}

		public void EnumerateSQL(SqlCommand command, SecurityClass security, DateTime startDate, DateTime endDate)
		{
			command.CommandText =
				"SELECT tblTestSetTankResults.*, tblSites.ID as 'SiteID' " +
				"FROM tblTestSetTankResults INNER JOIN tblSites ON tblSites.SiteGuid = tblTestSetTankResults.SiteGuid " +
				"WHERE ResultTimeStamp BETWEEN @StartDate AND @EndDate AND tblTestSetTankResults.SiteGuid = @SiteGuid " +
				"ORDER BY tblTestSetTankResults.TestSetTankResultGuid ";

			command.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			command.Parameters.AddWithValue("@StartDate", TimeConverter.ToDate(startDate));
			command.Parameters.AddWithValue("@EndDate", TimeConverter.ToDate(endDate).AddDays(1));
		}

		public void EnumerateByTankGuidSQL(SqlCommand cmd, SecurityClass security, Guid inTankGuid)
		{
			cmd.CommandText = "SELECT tblTestSetTankResults.* FROM tblTestSetTankResults " +
					"WHERE tblTestSetTankResults.TankGuid = @TankGuid " +
					"ORDER BY tblTestSetTankResults.TestSetTankResultGuid";

			cmd.Parameters.AddWithValue("@TankGuid", inTankGuid);
		}

		public void GetPreviousSampleNumberSQL(SqlCommand cmd, bool inTransaction, Guid siteGuid)
		{
			cmd.CommandText = "SELECT TOP 1 * FROM (SELECT SiteGuid, ResultTimeStamp, SampleNumber, CreatedDate FROM tblTestSetTankResults UNION SELECT SiteGuid, ResultTimeStamp, SampleNumber, CreatedDate FROM tblTestSetEquipmentResults ) tblTestSetResults" +
					" WHERE SiteGuid = @SiteGuid" +
					" ORDER BY SampleNumber DESC";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public void FindDuplicateSampleNumberSQL(SqlCommand cmd, bool inTransaction, Guid siteGuid, int inSampleNumber)
		{
			cmd.CommandText = "SELECT * FROM (SELECT SiteGuid, SampleNumber, TestSetTankResultGuid AS [ResultGuid], 'Tank' AS [Asset] FROM tblTestSetTankResults UNION SELECT SiteGuid, SampleNumber, TestSetEquipmentResultGuid AS [ResultGuid], 'Equip' AS [Asset] FROM tblTestSetEquipmentResults) tblTestSetResults" +
					" WHERE SiteGuid = @SiteGuid" +
					" AND SampleNumber = @SampleNumber";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@SampleNumber", inSampleNumber);
		}
	}
}
