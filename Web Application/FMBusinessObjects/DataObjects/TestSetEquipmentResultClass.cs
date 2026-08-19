using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	public enum TESTSET_STATUS
	{
		Pending,
		Passed,
		Failed
	};

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(TestSetEquipmentResultClass))]
	public class TestSetEquipmentResultCollectionClass : List<TestSetEquipmentResultClass>
	{
	}

	[Serializable]
	[DataContract]
	[KnownType(typeof(GregorianCalendar))]
	[QueryWriterTopic(typeof(TestSetEquipmentResultClass), "Quality Test Results", SupportsArchiveQuery = true)]
	[QueryWriterTopicSecurity(RIGHT.EXECUTE_QUALITY_TESTS)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_QUALITY_TESTS)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_QUALITY_TESTS)]
	[QueryWriterTopicSecurity(RIGHT.EXECUTE_QUALITY_TESTS)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_QUALITY_TESTS)]
	[QueryWriterTopicSecurity(RIGHT.ADD_QUALITYTAG_RECORD)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_QUALITYTAG_RECORD)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_QUALITYTAG_LOGS)]
	public class TestSetEquipmentResultClass : BaseDataObject
	{
		#region private data members
		[DataMember]
		private DateTimeOffset resultTimeStamp;
		[DataMember]
		private string testSetName;
		[DataMember]
		private string inspector;
		[DataMember]
		private string supervisor;
		[DataMember]
		private Guid equipmentGuid;
		[DataMember]
		private string equipmentID;
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
		#endregion protected data members

		#region protected properties
		[QueryWriterField("Asset Type", "AssetType", GenerateSelect = false)]
		protected string AssetTypeForQuery
		{
			get;
			set;
		}
		#endregion protected properties

		#region public properties
		public Guid TestSetEquipmentResultGuid
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

		[QueryWriterField("Result Time Stamp", false)]
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

		[QueryWriterField("Test Set Name", false)]
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

		[QueryWriterField("Inspector", false)]
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

		[QueryWriterField("Supervisor", false)]
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

		public Guid EquipmentGuid
		{
			get
			{
				return this.equipmentGuid;
			}
			set
			{
				this.equipmentGuid = value;
			}
		}

		[QueryWriterField("Asset ID", false)]
		public string EquipmentID
		{
			get
			{
				return this.equipmentID;
			}
			set
			{
				this.equipmentID = value;
			}
		}

		[QueryWriterField("Sample Number", false)]
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

		[QueryWriterField("Sample Size", false)]
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

		[QueryWriterField("Status", "LookupTestSetStatusIndex", false)]
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

		[QueryWriterField("Retest", false)]
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

		[QueryWriterField("Previous Sample Number", false)]
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

		[QueryWriterField("Document Number", false)]
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

		[QueryWriterField("Memo", false)]
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

		[QueryWriterField("Gallons Represented", false)]
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

		[QueryWriterField("Override", false)]
		public bool Override
		{
			get
			{
				return overrideFlag;
			}
			set
			{
				overrideFlag = value;
			}
		}

		[QueryWriterField("Flag01", false)]
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

		[QueryWriterField("UserData01", false)]
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

		[QueryWriterField("UserData02", false)]
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

		[QueryWriterField("Created By", "CreatedBy", GenerateSelect = false)]
		public string CreatedByQuery
		{
			get
			{
				return this._CreatedBy;
			}
			private set
			{
				;
			}
		}

		[QueryWriterField("Created Date", "CreatedDate", GenerateSelect = false)]
		public DateTimeOffset CreatedDateQuery
		{
			get
			{
				return this._CreatedDate;
			}
			private set
			{
				;
			}
		}

		[QueryWriterField("Updated By", "UpdatedBy", GenerateSelect = false)]
		public string UpdatedByQuery
		{
			get
			{
				return this._UpdatedBy;
			}
			private set
			{
				;
			}
		}

		[QueryWriterField("Updated Date", "UpdatedDate", GenerateSelect = false)]
		public DateTimeOffset UpdatedDateQuery
		{
			get
			{
				return this._UpdatedDate;
			}
			private set
			{
				;
			}
		}

		[DataMember]
		public TestEquipmentResultCollectionClass TestEquipmentResultCollection = new TestEquipmentResultCollectionClass();
		#endregion Public properties

		#region Constructors
		public TestSetEquipmentResultClass()
		{
			this.Init();
		}
		#endregion Constructors

		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST_SET_EQUIPMENT_RESULT;
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
			this.equipmentGuid	= Guid.Empty;
			this.equipmentID	= string.Empty;
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

			this._IdentityGuid			= DataObject.getValue<Guid>(row["TestSetEquipmentResultGuid"], Guid.Empty);
			this._SiteGuid				= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.resultTimeStamp		= DataObject.getValue<DateTimeOffset>(row["ResultTimeStamp"], DateTimeOffset.Now);
			this.testSetName			= DataObject.getValue<string>(row["TestSetName"], string.Empty);
			this.inspector				= DataObject.getValue<string>(row["Inspector"], string.Empty);
			this.supervisor				= DataObject.getValue<string>(row["Supervisor"], string.Empty);
			this.equipmentGuid			= DataObject.getValue<Guid>(row["EquipmentGuid"], Guid.Empty);
			this.equipmentID			= DataObject.getValue<string>(row["EquipmentID"], string.Empty);
			this.sampleNumber			= DataObject.getValue<int>(row["SampleNumber"], 0);
			this.sampleSize				= DataObject.getValue<double>(row["SampleSize"], 0.0);
			this.status					= DataObject.getValue<TESTSET_STATUS>(row["LookupTestSetStatusIndex"], TESTSET_STATUS.Pending);
			this.isRetest				= DataObject.getValue<bool>(row["IsRetest"], false);
			this.previousSampleNumber	= DataObject.getValue<int>(row["PreviousSampleNumber"], 0);
			this.documentNumber			= DataObject.getValue<string>(row["DocumentNumber"], string.Empty);
			this.memo					= DataObject.getValue<string>(row["Memo"], string.Empty);
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
			cmd.CommandText = "INSERT INTO tblTestSetEquipmentResults (" +
				"SiteGuid," +
				"ResultTimeStamp," +
				"TestSetName," +
				"Inspector," +
				"Supervisor," +
				"EquipmentGuid," +
				"EquipmentID," +
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
				"TestSetEquipmentResultGuid" +
				") VALUES (" +
				"@SiteGuid," +
				"@ResultTimeStamp," +
				"@TestSetName," +
				"@Inspector," +
				"@Supervisor," +
				"@EquipmentGuid," +
				"@EquipmentID," +
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
				"@TestSetEquipmentResultGuid)";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ResultTimeStamp", this.resultTimeStamp);
			cmd.Parameters.AddWithValue("@TestSetName", this.testSetName);
			cmd.Parameters.AddWithValue("@Inspector", this.inspector);
			cmd.Parameters.AddWithValue("@Supervisor", this.supervisor);
			cmd.Parameters.AddWithValue("@EquipmentGuid", this.equipmentGuid);
			cmd.Parameters.AddWithValue("@EquipmentID", this.equipmentID);
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
			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", this._IdentityGuid);

		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTestSetEquipmentResults SET " +
				"SiteGuid = @SiteGuid, " +
				"ResultTimeStamp = @ResultTimeStamp, " +
				"TestSetName = @TestSetName, " +
				"Inspector = @Inspector, " +
				"Supervisor = @Supervisor, " +
				"EquipmentGuid = @EquipmentGuid, " +
				"EquipmentID = @EquipmentID, " +
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
				"WHERE TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ResultTimeStamp", this.resultTimeStamp);
			cmd.Parameters.AddWithValue("@TestSetName", this.testSetName);
			cmd.Parameters.AddWithValue("@Inspector", this.inspector);
			cmd.Parameters.AddWithValue("@Supervisor", this.supervisor);
			cmd.Parameters.AddWithValue("@EquipmentGuid", this.equipmentGuid);
			cmd.Parameters.AddWithValue("@EquipmentID", this.equipmentID);
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
			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", this._IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTestSetEquipmentResults WHERE TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid";
			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", this._IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblTestSetEquipmentResults.* FROM tblTestSetEquipmentResults " + SQLUpdateLock(bInTransaction) +
				" WHERE TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid";
			cmd.Parameters.AddWithValue("@TestSetEquipmentResultGuid", this._IdentityGuid);
		}

		public void SelectByEquipmentGuidSQL(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblTestSetEquipmentResults.* FROM tblTestSetEquipmentResults " + SQLUpdateLock(inTransaction) +
					" WHERE EquipmentGuid = @EquipmentGuid";
			cmd.Parameters.AddWithValue("@EquipmentGuid", this.equipmentGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			cmd.CommandText = "SELECT tblTestSetEquipmentResults.*,tblSites.ID AS 'SiteID' FROM tblTestSetEquipmentResults " +
					" 	tblTestSetEquipmentResults INNER JOIN tblSites ON tblSites.SiteGuid = tblTestSetEquipmentResults.SiteGuid " +
					"WHERE ResultTimeStamp BETWEEN @StartDate AND @EndDate AND tblTestSetEquipmentResults.SiteGuid = @SiteGuid " +
					"ORDER BY tblTestSetEquipmentResults.TestSetEquipmentResultGuid";

			// We want only the date parts, not the time parts, and we want to add 1 day to End Date
			cmd.Parameters.AddWithValue("@StartDate", TimeConverter.ToDate(startDate));
			cmd.Parameters.AddWithValue("@EndDate", TimeConverter.ToDate(endDate).AddDays(1));
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblTestSetEquipmentResults.* FROM tblTestSetEquipmentResults " +
					"WHERE tblTestSetEquipmentResults.SiteGuid = @SiteGuid " +
					"ORDER BY tblTestSetEquipmentResults.TestSetEquipmentResultGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}

		public void EnumerateByEquipmentGuidSQL(SqlCommand cmd, SecurityClass security, Guid inEquipmentGuid)
		{
			cmd.CommandText = "SELECT tblTestSetEquipmentResults.* FROM tblTestSetEquipmentResults " +
					"WHERE tblTestSetEquipmentResults.EquipmentGuid = @EquipmentGuid " +
					"ORDER BY tblTestSetEquipmentResults.TestSetEquipmentResultGuid";

			cmd.Parameters.AddWithValue("@EquipmentGuid", inEquipmentGuid);
		}

		public void GetPreviousSampleNumberSQL(SqlCommand cmd, bool bInTransaction, Guid siteGuid)
		{
			cmd.CommandText = "SELECT TOP 1 * FROM (SELECT SiteGuid, ResultTimeStamp, SampleNumber, CreatedDate FROM tblTestSetEquipmentResults UNION SELECT SiteGuid, ResultTimeStamp, SampleNumber, CreatedDate FROM tblTestSetTankResults ) tblTestSetResults" +
					" WHERE SiteGuid = @SiteGuid" +
					" ORDER BY SampleNumber DESC";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public void FindDuplicateSampleNumberSQL(SqlCommand cmd, bool bInTransaction, Guid siteGuid, int inSampleNumber)
		{
			cmd.CommandText = "SELECT * FROM (SELECT SiteGuid, SampleNumber, TestSetEquipmentResultGuid AS [ResultGuid], 'Equip' AS [Asset] FROM tblTestSetEquipmentResults UNION SELECT SiteGuid, SampleNumber, TestSetTankResultGuid AS [ResultGuid], 'Tank' AS [Asset] FROM tblTestSetTankResults) tblTestSetResults" +
					" WHERE SiteGuid = @SiteGuid" +
					" AND SampleNumber = @SampleNumber";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@SampleNumber", inSampleNumber);
		}

		public void QueryWriterSQL(SqlCommand cmd, SecurityClass security, string selectClause, string dbName)
		{
			const string SQL = @"{0},* FROM " +
			                   "( " +
							   "SELECT 'Equipment' AS 'AssetType','E' + CAST(TestSetEquipmentResultGuid AS VARCHAR(40)) AS 'EntityGuid'," +
@"[ResultTimeStamp]
,[TestSetName]
,[Inspector]
,[Supervisor]
,[EquipmentID]
,[SampleNumber]
,[SampleSize]
,[IsRetest]
,[PreviousSampleNumber]
,[DocumentNumber]
,[Memo]
,[GallonsRepresented]
,[Override]
,[DeleteFlag]
,[CreatedDate]
,[CreatedBy]
,[UpdatedDate]
,[UpdatedBy]
,[TestSetEquipmentResultGuid]
,[_RowVersion]
,[SiteGuid]
,[LookupTestSetStatusIndex]
,[EquipmentGuid]
,[Flag01]
,[Flag02]
,[UserData01]
,[UserData02] " + 
							   "FROM [{1}]..tblTestSetEquipmentResults " +
			                   "WHERE SiteGuid = @SiteGuid " +
			                   "UNION  " +
			                   "SELECT 'Tank' AS 'AssetType','T' + CAST(TestSetTankResultGuid AS VARCHAR(40)) AS 'EntityGuid'," +
@"[ResultTimeStamp]
,[TestSetName]
,[Inspector]
,[Supervisor]
,[TankID]
,[SampleNumber]
,[SampleSize]
,[IsRetest]
,[PreviousSampleNumber]
,[DocumentNumber]
,[Memo]
,[GallonsRepresented]
,[Override]
,[DeleteFlag]
,[CreatedDate]
,[CreatedBy]
,[UpdatedDate]
,[UpdatedBy]
,[TestSetTankResultGuid]
,[_RowVersion]
,[SiteGuid]
,[LookupTestSetStatusIndex]
,[TankGuid]
,[Flag01]
,[Flag02]
,[UserData01]
,[UserData02]" +
							   "FROM [{1}]..tblTestSetTankResults " +
			                   "WHERE SiteGuid = @SiteGuid " +
			                   ") tblResult " +
			                   "WHERE 1=1";

			cmd.CommandText = string.Format(SQL, selectClause, dbName);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
		}

		/// <summary>
		/// This method is used when the edit button is clicked on the query writer results form
		/// </summary>
		/// <returns>The page corresponding to this entity</returns>
		public string DetailPageReference()
		{
			return "QualityControlWebApp\\TestSetResultForm.aspx";
		}

		public void QueryWriterPostProcess(SecurityClass security, DataSet set)
		{
			CensorFieldsIfNecessary(security, set);
		}

		private void CensorFieldsIfNecessary(SecurityClass security, DataSet set)
		{
			if (security.HasRight(RIGHT.VIEW_QUALITY_TESTS) == false && security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) == false)
			{
				set.Tables[0].Rows.Clear();
			}
		}
	}
}
