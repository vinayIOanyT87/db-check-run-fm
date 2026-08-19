namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Represents a rowset of tblEquipmentQualityTagLog rows.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(EquipmentQualityTagLogClass))]
	public class EquipmentQualityTagLogCollectionClass : List<EquipmentQualityTagLogClass>
	{
	}

	/// <summary>
	/// Represents a row in tblEquipmentQualityTagLog.
	/// </summary>
	[DataContract]
	[Serializable]
	public class EquipmentQualityTagLogClass : BaseDataObject, IComparable
	{
		#region Data Members.
		// Fields.
		[DataMember] private Guid qualityTagGuid;
		[DataMember] private string qualityTagName;
		[DataMember] private Guid equipmentGuid;
		[DataMember] private string equipmentID;
		[DataMember] private string equipmentType;
		[DataMember] private DateTimeOffset? taggedDate;
		[DataMember] private string taggedBy;
		[DataMember] private string memo;
		[DataMember] private DateTimeOffset? removedDate;
		[DataMember] private string removedBy;
		[DataMember] private string assetTypeForQuery;
		[DataMember] private int tagNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public EquipmentQualityTagLogClass()
		{
			this.Init();
		}
		#endregion

		#region Accessors
		[QueryWriterField("Tag Number", false)]
		public int TagNumber
		{
			get { return this.tagNumber; }
			set { this.tagNumber = value; }
		}

		[QueryWriterField("Asset Type", "AssetType", GenerateSelect = false)]
		protected string AssetTypeForQuery
		{
			get { return this.assetTypeForQuery; }
			set { this.assetTypeForQuery = value; }
		}

		public Guid EquipmentGuid
		{
			get { return this.equipmentGuid; }
			set { this.equipmentGuid = value; }
		}

		[QueryWriterField("Type", "Type", GenerateSelect = false)]
		public string EquipmentType
		{
			get { return this.equipmentType; }
			set { this.equipmentType = value; }
		}

		[QueryWriterField("ID", "ID", GenerateSelect = false)]
		public string EquipmentID
		{
			get { return this.equipmentID; }
			set { this.equipmentID = value; }
		}

		public Guid QualityTagGuid
		{
			get { return this.qualityTagGuid; }
			set { this.qualityTagGuid = value; }
		}

		[QueryWriterField("Quality Tag Name", false)]
		public string QualityTagName
		{
			get { return this.qualityTagName; }
			set { this.qualityTagName = value; }
		}

		[QueryWriterField("Tagged Date", false)]
		public DateTimeOffset? TaggedDate
		{
			get { return this.taggedDate; }
			set { this.taggedDate = value; }
		}

		[QueryWriterField("Tagged By", false)]
		public string TaggedBy
		{
			get { return this.taggedBy; }
			set { this.taggedBy = value; }
		}

		[QueryWriterField("Memo", false)]
		public string Memo
		{
			get { return this.memo; }
			set { this.memo = value; }
		}

		[QueryWriterField("Removed Date", false)]
		public DateTimeOffset? RemovedDate
		{
			get { return this.removedDate; }
			set { this.removedDate = value; }
		}

		[QueryWriterField("Removed By", false)]
		public string RemovedBy
		{
			get { return this.removedBy; }
			set { this.removedBy = value; }
		}

		[QueryWriterField("Created By", "CreatedBy", GenerateSelect = false)]
		public string CreatedByQuery
		{
			get { return this._CreatedBy; }
		}

		[QueryWriterField("Created Date", "CreatedDate", GenerateSelect = false)]
		public DateTimeOffset CreatedDateQuery
		{
			get { return this._CreatedDate; }
		}

		[QueryWriterField("Updated By", "UpdatedBy", GenerateSelect = false)]
		public string UpdatedByQuery
		{
			get { return this._UpdatedBy; }
		}

		[QueryWriterField("Updated Date", "UpdatedDate", GenerateSelect = false)]
		public DateTimeOffset UpdatedDateQuery
		{
			get { return this._UpdatedDate; }
		}
		#endregion

		#region Abstract base class method overrides
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EQUIPMENT_QUALITY_TAG_LOG; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
		#endregion

		#region IComparable Interface implementation
		int IComparable.CompareTo(object compareObject)
		{
			var equipmentQualityTagLog = compareObject as EquipmentQualityTagLogClass;

			if (equipmentQualityTagLog == null)
			{
				throw new Exception("Invalid EquipmentQualityTagLogClass");
			}

			return this._IdentityGuid.CompareTo(equipmentQualityTagLog._IdentityGuid);
		}


		// Common place to initialize all base-class and "field"-type member variables.
		public override void Reset()
		{
			this.Init();
		}
		#endregion

		#region Public methods
		public override void Load(Object o)
		{
			this.Reset();

			// Load from DataSet or XML.
			var dataSetObject = o as DataSet;

			if (dataSetObject != null)
			{
				DataSet set = dataSetObject;
				DataTable table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					// Field-type data members.
					this.equipmentGuid	= DataObject.getValue<Guid>(row["EquipmentGuid"], Guid.Empty);
					this.qualityTagGuid = DataObject.getValue<Guid>(row["QualityTagGuid"], Guid.Empty);
					this.equipmentID	= DataObject.getValue<string>(row["EquipmentID"], "");
					this.qualityTagName = DataObject.getValue<string>(row["QualityTagName"], "");
					this.equipmentType	= DataObject.getValue<string>(row["EquipmentType"], "");
					this.taggedDate		= DataObject.getValue<DateTimeOffset>(row["TaggedDate"], DateTimeOffset.Now);
					this.taggedBy		= DataObject.getValue<string>(row["TaggedBy"], "");
					this.memo			= DataObject.getValue<string>(row["Memo"], "");
					this.tagNumber		= DataObject.getValue<int>(row["TagNumber"], 0);
					this.removedBy		= DataObject.getValue<string>(row["RemovedBy"], "");

					if (!row.IsNull("RemovedDate"))
					{
						this.removedDate = DataObject.getValue<DateTimeOffset>(row["RemovedDate"], DateTimeOffset.Now);
					}

					// Defined in base-class.
					this._IdentityGuid	= DataObject.getValue<Guid>(row["EquipmentQualityTagLogGuid"], Guid.Empty);
					this._SiteGuid		= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
					this._CreatedDate	= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
					this._CreatedBy		= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
					this._UpdatedDate	= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
					this._UpdatedBy		= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
				}
			}
		}

		public void GetMostRecentByEquipmentIDSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText =	"SELECT log1.* FROM dbo.tblEquipmentQualityTagLog log1 "
								+ " JOIN dbo.tblEquipment asset ON log1.EquipmentGuid = asset.EquipmentGuid "
								+ " WHERE  ManagedEquipmentFlag = 1 "
								+ " AND asset.ID = @EquipmentID"
								+ " AND log1.UpdatedDate = "
								+ "          (SELECT MAX(UpdatedDate) "
								+ "           FROM dbo.tblEquipmentQualityTagLog  log2 "
								+ "           WHERE log1.EquipmentGuid = log2.EquipmentGuid) ";

			cmd.Parameters.Add("@EquipmentID", SqlDbType.NVarChar, 30);
			cmd.Parameters["@EquipmentID"].Value = EquipmentID;
		}

		public void GetByTagNumberSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT * "
							  + " FROM dbo.tblEquipmentQualityTagLog "
							  + " WHERE TagNumber = @TagNumber AND SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@TagNumber", SqlDbType.Int);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@TagNumber"].Value = this.tagNumber;
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
		}

		public void GetSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT * "
							  + " FROM dbo.tblEquipmentQualityTagLog "
							  + " WHERE EquipmentQualityTagLogGuid = @IdentityGuid ";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
		}

		public void EnumerateSQL(SqlCommand cmd, bool historical, SecurityClass security)
		{
			cmd.CommandText =	"SELECT * "
								+ "FROM tblEquipmentQualityTagLog log1 JOIN dbo.tblEquipment asset "
								+ "  ON asset.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', log1.EquipmentGuid, @SiteGuid) "
								+ " WHERE  ManagedEquipmentFlag = 1 ";

			if (!historical)
			{
				cmd.CommandText += " AND log1.UpdatedDate = (SELECT MAX(UpdatedDate) "
									 + "            FROM dbo.tblEquipmentQualityTagLog  log3 "
									 + "            WHERE log1.EquipmentGuid= log3.EquipmentGuid) ";
			}

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
		}

		// The SQL string used by the QualityTag Log form.  
		public void EnumerateSQL(	SqlCommand cmd, 
									SecurityClass security, 
									bool historical,
									string dateType, 
									DateTimeOffset dateStart, 
									DateTimeOffset dateEnd,
									string qualityTag,
									string inTaggedBy,
									string inRemovedBy,
									string assetID,
									string state)
		{
			var equipment = new EquipmentClass();

			// First get the Equipments.
			string sql = "SELECT 'EQUIPMENT' AS 'Asset Type' "
						+ ", log1.EquipmentQualityTagLogGuid		AS 'QualityTagLogGuid' "
						+ ", log1.EquipmentID	AS 'Asset ID' "
						+ ", log1.QualityTagName 					AS 'Quality Tag Reason' "
						+ ", log1.TaggedDate								AS 'Tagged Date' "
						+ ", log1.TaggedBy							AS 'Tagged By' "
						+ ", log1.TagNumber                         AS 'Tag Number' "
						+ ", log1.RemovedDate						AS 'Removed Date' "
						+ ", log1.RemovedBy							AS 'Removed By' "
						+ ", LEFT(log1.Memo, 15)					AS 'Memo' "
						+ "  FROM dbo.tblEquipmentQualityTagLog  log1 "
						+ "  LEFT JOIN dbo.tblEquipment asset "
						+ "  ON asset.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', log1.EquipmentGuid, @SiteGuid) "
						+ " WHERE 1=1 ";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;

			if (!historical)
			{
				sql = sql + " AND ManagedEquipmentFlag = 1 AND " + equipment.AppendSiteWhereClause(cmd, security, "asset", "EquipmentGuid")
						  + " AND log1.UpdatedDate = "
						  + "          (SELECT MAX(UpdatedDate) "
						  + "             FROM dbo.tblEquipmentQualityTagLog  log3 "
						  + "            WHERE log1.EquipmentGuid = log3.EquipmentGuid) ";
			}

			switch (state)
			{
				case "Active Tags Only":
					sql = sql + " AND RemovedDate IS NULL ";
					break;
				case "Removed Tags Only":
					sql = sql + " AND RemovedDate IS NOT NULL ";
					break;
			}

			if (!string.IsNullOrEmpty(qualityTag))
			{
				sql = sql + "  AND log1.QualityTagName = @QualityTag ";
				cmd.Parameters.Add("@QualityTag", SqlDbType.NVarChar, 50);
				cmd.Parameters["@QualityTag"].Value = qualityTag;
			}

			if (!string.IsNullOrEmpty(inTaggedBy))
			{
				sql = sql + "  AND log1.TaggedBy = @TaggedBy ";
				cmd.Parameters.Add("@TaggedBy", SqlDbType.NVarChar, 50);
				cmd.Parameters["@TaggedBy"].Value = inTaggedBy;
			}

			if (!string.IsNullOrEmpty(inRemovedBy))
			{
				sql = sql + "  AND log1.RemovedBy = @RemovedBy ";
				cmd.Parameters.Add("@RemovedBy", SqlDbType.NVarChar, 225);
				cmd.Parameters["@RemovedBy"].Value = inRemovedBy;
			}

			if (!string.IsNullOrEmpty(assetID))
			{
				sql = sql + "  AND log1.EquipmentID = @AssetID ";
				cmd.Parameters.Add("@AssetID", SqlDbType.NVarChar, 50);
				cmd.Parameters["@AssetID"].Value = assetID;
			}

			// Then combine the Tanks.
			sql = sql	+ " UNION "
						+ "SELECT "
						+ "  'TANK'									AS 'Asset Type' "
						+ ", log1.TankQualityTagLogGuid				AS 'QualityTagLogGuid' "
						+ ", log1.TankID                            AS 'Asset ID' "
						+ ", log1.QualityTagName 					AS 'Quality Tag Reason' "
						+ ", log1.TaggedDate						AS 'Tagged Date' "
						+ ", log1.TaggedBy							AS 'Tagged By' "
						+ ", log1.TagNumber                         AS 'Tag Number' "
						+ ", log1.RemovedDate						AS 'Removed Date' "
						+ ", log1.RemovedBy							AS 'Removed By' "
						+ ", LEFT(log1.Memo, 15)					AS 'Memo' "
						+ "  FROM dbo.tblTankQualityTagLog  log1 "
						+ "  LEFT JOIN dbo.tblTanks asset ON log1.TankGuid = asset.TankGuid "
						+ " WHERE 1 = 1 ";

			if (!historical)
			{
				sql = sql + " AND asset.SiteGuid = @SiteGuid "
						  + " AND log1.UpdatedDate = "
						  + "          (SELECT MAX(UpdatedDate) "
						  + "             FROM dbo.tblTankQualityTagLog  log3 "
						  + "            WHERE log1.TankGuid = log3.TankGuid) ";
			}

			switch (state)
			{
				case "Active Tags Only":
					sql = sql + " AND RemovedDate IS NULL ";
					break;
				case "Removed Tags Only":
					sql = sql + " AND RemovedDate IS NOT NULL ";
					break;
			}

			if (!string.IsNullOrEmpty(qualityTag))
			{
				sql = sql + "  AND log1.QualityTagName = @QualityTag ";
			}

			if (!string.IsNullOrEmpty(inTaggedBy))
			{
				sql = sql + "  AND log1.TaggedBy = @TaggedBy ";
			}

			if (!string.IsNullOrEmpty(inRemovedBy))
			{
				sql = sql + "  AND log1.RemovedBy = @RemovedBy ";
			}

			if (!string.IsNullOrEmpty(assetID))
			{
				sql = sql + "  AND log1.TankID = @AssetID ";
			}

			// Now wrap the two SELECTs into one, so we can use their common 
			// column names to search against.
			cmd.CommandText = "SELECT * FROM ( " + sql + " ) AS tBoth ";

			// Now append the WHERE clause.
			if (!string.IsNullOrEmpty(dateType))
			{
				switch (dateType)
				{
					case "Tagged Date": cmd.CommandText += " WHERE tBoth.[Tagged Date] "; break;
					case "Removed Date": cmd.CommandText += " WHERE tBoth.[Removed Date] "; break;
					default: Debug.Assert(false, "Impossible - we checked the length already"); break;
				}

				cmd.CommandText += " BETWEEN @DateStart AND @DateEnd ";

				cmd.Parameters.Add("@DateStart", SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@DateEnd", SqlDbType.DateTimeOffset);

				DateTimeOffset dateStartValue = TimeConverter.ToStartOfDay(dateStart);
				DateTimeOffset dateEndValue = TimeConverter.ToEndOfDay(dateEnd);

				cmd.Parameters["@DateStart"].Value = dateStartValue;
				cmd.Parameters["@DateEnd"].Value = dateEndValue;
			}
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblEquipmentQualityTagLog WHERE EquipmentQualityTagLogGuid = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
		}

		public void PreviousTagNumberSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT TOP 1 TagNumber, TaggedDate FROM "
							+ "(SELECT TagNumber, TaggedDate FROM tblEquipmentQualityTagLog WHERE SiteGuid = @SiteGuid "
							+ "UNION SELECT TagNumber, TaggedDate FROM tblTankQualityTagLog WHERE SiteGuid = @SiteGuid ) tblResults "
							+ "ORDER BY TagNumber DESC";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText =	"INSERT INTO dbo.tblEquipmentQualityTagLog "
								+ "( "
								+ "QualityTagGuid"
								+ ", QualityTagName"
								+ ", EquipmentGuid"
								+ ", EquipmentID"
								+ ", EquipmentType"
								+ ", TaggedDate"
								+ ", TaggedBy"
								+ ", Memo"
								+ ", RemovedDate"
								+ ", RemovedBy"
								+ ", SiteGuid"
								+ ", DeleteFlag"
								+ ", CreatedDate"
								+ ", CreatedBy"
								+ ", UpdatedDate"
								+ ", UpdatedBy"
								+ ", TagNumber"
								+ ", EquipmentQualityTagLogGuid"
								+ ") VALUES ( "
								+ "@QualityTagGuid,"
								+ "@QualityTagName,"
								+ "@EquipmentGuid,"
								+ "@EquipmentID,"
								+ "@EquipmentType,"
								+ "@TaggedDate,"
								+ "@TaggedBy,"
								+ "@Memo,"
								+ " NULL,"
								+ " NULL,"
								+ "@SiteGuid,"
								+ "@Deleted,"
								+ "@CreatedDate,"
								+ "@CreatedBy,"
								+ "@UpdatedDate,"
								+ "@UpdatedBy,"
								+ "@TagNumber, "
								+ "@EquipmentQualityTagLogGuid"
								+ ") ";

			cmd.Parameters.Add("@QualityTagGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@QualityTagName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EquipmentID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentType", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@TaggedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@TaggedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Memo", SqlDbType.NVarChar, 1000);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Deleted", SqlDbType.Bit);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@TagNumber", SqlDbType.Int);
			cmd.Parameters.Add("@EquipmentQualityTagLogGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@QualityTagGuid"].Value = QualityTagGuid;
			cmd.Parameters["@QualityTagName"].Value = QualityTagName;
			cmd.Parameters["@EquipmentGuid"].Value = EquipmentGuid;
			cmd.Parameters["@EquipmentID"].Value = EquipmentID;
			cmd.Parameters["@EquipmentType"].Value = EquipmentType;
			cmd.Parameters["@TaggedDate"].Value = TaggedDate;
			cmd.Parameters["@TaggedBy"].Value = TaggedBy;
			cmd.Parameters["@Memo"].Value = Memo;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;

			if (Deleted)
			{
				cmd.Parameters["@Deleted"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Deleted"].Value = 0;
			}

			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@TagNumber"].Value = tagNumber;
			cmd.Parameters["@EquipmentQualityTagLogGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			Debug.Assert(Guid.Empty != this.SiteGuid);

			cmd.CommandText =	"UPDATE dbo.tblEquipmentQualityTagLog SET "
								+ "   QualityTagGuid = @QualityTagGuid "
								+ " , QualityTagName = @QualityTagName "
								+ " , EquipmentGuid  = @EquipmentGuid "
								+ " , EquipmentID 	 = @EquipmentID "
								+ " , EquipmentType  = @EquipmentType "
								+ " , TaggedDate 	 = @TaggedDate "
								+ " , TaggedBy 		 = @TaggedBy "
								+ " , Memo 			 = @Memo "
								+ " , RemovedDate 	 = @RemovedDate "
								+ " , RemovedBy 	 = @RemovedBy "
								+ " , SiteGuid 		 = @SiteGuid "
								+ " , DeleteFlag 	 = @Deleted "
								+ " , CreatedDate 	 = @CreatedDate "
								+ " , CreatedBy 	 = @CreatedBy "
								+ " , UpdatedDate 	 = @UpdatedDate "
								+ " , UpdatedBy 	 = @UpdatedBy "
								+ " , TagNumber      = @TagNumber "
								+ " WHERE EquipmentQualityTagLogGuid = @IdentityGuid";

			cmd.Parameters.Add("@QualityTagGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@QualityTagName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EquipmentID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentType", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@TaggedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@TaggedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Memo", SqlDbType.NVarChar, 1000);
			cmd.Parameters.Add("@RemovedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@RemovedBy", SqlDbType.NVarChar, 255);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Deleted", SqlDbType.Bit);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@TagNumber", SqlDbType.Int);
			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@QualityTagGuid"].Value = QualityTagGuid;
			cmd.Parameters["@QualityTagName"].Value = QualityTagName;
			cmd.Parameters["@EquipmentGuid"].Value = EquipmentGuid;
			cmd.Parameters["@EquipmentID"].Value = EquipmentID;
			cmd.Parameters["@EquipmentType"].Value = EquipmentType;

			if (TaggedDate == null || TaggedDate.Value.Year == 1)
			{
				cmd.Parameters["@TaggedDate"].Value = DateTimeOffset.Now;
			}
			else
			{
				cmd.Parameters["@TaggedDate"].Value = TaggedDate;
			}

			cmd.Parameters["@TaggedBy"].Value = TaggedBy;
			cmd.Parameters["@Memo"].Value = Memo;
			cmd.Parameters["@RemovedDate"].Value = RemovedDate;
			cmd.Parameters["@RemovedBy"].Value = RemovedBy;
			cmd.Parameters["@Memo"].Value = Memo;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;

			if (Deleted)
			{
				cmd.Parameters["@Deleted"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Deleted"].Value = 0;
			}

			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@TagNumber"].Value = TagNumber;
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
		}

		/// <summary>
		/// This method will return a query write SQL statement
		/// </summary>
		/// <param name="security"></param>
		/// <param name="selectClause"></param>
		/// <param name="dbName"></param>
		/// <returns></returns>
		public string QueryWriterSQL(SecurityClass security, string selectClause, string dbName)
		{
			const string SQL = @"{1},* FROM
						(
						SELECT 
							'E' + CAST(EquipmentQualityTagLogGuid AS VARCHAR(64)) as 'EntityIndex',
							'Equipment' as 'AssetType',
							EquipmentID as 'ID',
							EquipmentType as 'Type',
							TagNumber,
							QualityTagName,
							TaggedDate,
							TaggedBy,
							Memo,
							RemovedDate,
							RemovedBy,
							CreatedBy,
							CreatedDate,
							UpdatedBy,
							UpdatedDate
						FROM {2}..tblEquipmentQualityTagLog
						WHERE SiteGuid = '{0}'
						UNION 
						SELECT 
							'T' + CAST(TankQualityTagLogGuid AS VARCHAR(64)) as 'EntityIndex',
							'Tank' as 'AssetType',
							TankID as 'ID',
							VesselType as 'Type',
							TagNumber,
							QualityTagName,
							TaggedDate,
							TaggedBy,
							Memo,
							RemovedDate,
							RemovedBy,
							CreatedBy,
							CreatedDate,
							UpdatedBy,
							UpdatedDate
						FROM {2}..tblTankQualityTagLog
						WHERE SiteGuid = '{0}'
						) tblResult
						WHERE 1=1
						";

			return string.Format(SQL, security.SiteGuid, selectClause, dbName);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the object to its original state.
		/// </summary>
		private void Init()
		{
			// Handles _IdentityGuid, _ID, _SiteGuid, _CreatedDate, _CreatedBy, _UpdatedDate,
			// _UpdatedBy, and _Deleted.
			base.Reset();

			// Field member variables.
			this.equipmentGuid		= Guid.Empty;
			this.equipmentID		= string.Empty;
			this.equipmentType		= string.Empty;
			this.qualityTagGuid		= Guid.Empty;
			this.memo				= string.Empty;
			this.TagNumber			= 0;
			this.qualityTagName		= string.Empty;
			this.taggedDate			= null;
			this.taggedBy			= string.Empty;
			this.removedDate		= null;
			this.removedBy			= string.Empty;
			this.assetTypeForQuery	= string.Empty;
		}
		#endregion
	}
}
