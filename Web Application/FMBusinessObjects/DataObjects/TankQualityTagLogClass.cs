using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(TankQualityTagLogClass))]
	public class TankQualityTagLogCollectionClass : List<TankQualityTagLogClass> { }
   [Serializable]
   [DataContract]
	public class TankQualityTagLogClass :
											BaseDataObject,
											IComparable
	{
		#region Data Members.

		// Fields.
		[DataMember]
		protected Guid _QualityTagGuid;
		[DataMember]
		protected string _QualityTagName;
		[DataMember]
		protected Guid _TankGuid;
		[DataMember]
		protected string _TankID;
		[DataMember]
		protected VESSEL_TYPE _VesselTypeIndex;
		[DataMember]
		protected string _VesselType;
		[DataMember]
		protected DateTimeOffset _TaggedDate;
		[DataMember]
		protected string _TaggedBy;
		[DataMember]
		protected string _Memo;
		[DataMember]
		protected DateTimeOffset _RemovedDate;
		[DataMember]
		protected string _RemovedBy;
		#endregion

		#region public data members
		[DataMember]
		public int TagNumber { get; set; }

		public string RemovedBy
		{
			get { return _RemovedBy; }
			set { _RemovedBy = value; }
		}

		public DateTimeOffset RemovedDate
		{
			get { return _RemovedDate; }
			set { _RemovedDate = value; }
		}

		public string Memo
		{
			get { return _Memo; }
			set { _Memo = value; }
		}

		public Guid TankGuid
		{
			get { return _TankGuid; }
			set { _TankGuid = value; }
		}

		public string TankID
		{
			get { return _TankID; }
			set { _TankID = value; }
		}

		public VESSEL_TYPE VesselTypeIndex
		{
			get { return _VesselTypeIndex; }
			set { _VesselTypeIndex = value; }
		}

		public string VesselType
		{
			get { return _VesselType; }
			set { _VesselType = value; }
		}

		public string TaggedBy
		{
			get { return _TaggedBy; }
			set { _TaggedBy = value; }
		}

		public Guid QualityTagGuid
		{
			get { return _QualityTagGuid; }
			set { _QualityTagGuid = value; }
		}

		public string QualityTagName
		{
			get { return _QualityTagName; }
			set { _QualityTagName = value; }
		}

		public DateTimeOffset TaggedDate
		{
			get { return _TaggedDate; }
			set { _TaggedDate = value; }
		}
		#endregion public data members

		#region Constructors
		public TankQualityTagLogClass()
		{
			Reset();
		}
		#endregion

		#region Abstract base class method overrides - business logic layer uses these.

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.TANK_QUALITY_TAG_LOG; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#endregion Abstract base class method overrides - business logic layer uses these.

		#region IComparable Interface implementation
		int IComparable.CompareTo(object O)
		{
			TankQualityTagLogClass TankQualityTagLog = O as TankQualityTagLogClass;

			if (TankQualityTagLog == null)
			{
				throw new Exception("Invalid TankQualityTagLogClass");
			}

			return this._IdentityGuid.CompareTo(TankQualityTagLog._IdentityGuid);
		}
		#endregion IComparable Interface implementation

		// Common place to initialize all base-class and "field"-type member variables.
		public override void Reset()
		{
			// Handles _IdentityGuid, _ID, _SiteGuid, _CreatedDate, _CreatedBy, _UpdatedDate,
			// _UpdatedBy, and _Deleted.
			base.Reset();

			// Field member variables.
			_TankGuid = Guid.Empty;
			_QualityTagGuid = Guid.Empty;
		}


		public override void Load(Object o)
		{
			Reset();

			// Load from DataSet or XML.
			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count > 0)
				{
					DataRow Row = Table.Rows[0];

					// Field-type data members.
					_TankGuid = DataObject.getValue<Guid>(Row["TankGuid"], Guid.Empty);
					_QualityTagGuid = DataObject.getValue<Guid>(Row["QualityTagGuid"], Guid.Empty);
					_TankID = DataObject.getValue<string>(Row["TankID"], "");
					_QualityTagName = DataObject.getValue<string>(Row["QualityTagName"], "");
					_VesselType = DataObject.getValue<string>(Row["VesselType"], "");
					_VesselTypeIndex = (VESSEL_TYPE)DataObject.getValue<int>(Row["LookupVesselTypeIndex"], (int)VESSEL_TYPE.UNDEFINED_VESSEL);
					_TaggedDate = DataObject.getValue<DateTimeOffset>(Row["TaggedDate"], DateTimeOffset.Now);
					_TaggedBy = DataObject.getValue<string>(Row["TaggedBy"], "");
					_Memo = DataObject.getValue<string>(Row["Memo"], "");
					_RemovedDate = DataObject.getValue<DateTimeOffset>(Row["RemovedDate"], DateTimeOffset.MinValue);
					_RemovedBy = DataObject.getValue<string>(Row["RemovedBy"], "");
					TagNumber = DataObject.getValue<int>(Row["TagNumber"], 0);

					// Defined in base-class.
					IdentityGuid = DataObject.getValue<Guid>(Row["TankQualityTagLogGuid"], Guid.Empty);
					SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				}
			}
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO dbo.tblTankQualityTagLog "
				+ "( "
				+ "TankGuid"
				+ ", TankID"
				+ ", LookupVesselTypeIndex"
				+ ", VesselType"
				+ ", QualityTagGuid"
				+ ", QualityTagName"
				+ ", TaggedDate"
				+ ", TaggedBy"
				+ ", Memo"
				+ ", SiteGuid"
				+ ", DeleteFlag"
				+ ", CreatedDate"
				+ ", CreatedBy"
				+ ", UpdatedDate"
				+ ", UpdatedBy"
				+ ", TagNumber"
				+ ", TankQualityTagLogGuid"
				+ ") VALUES "
				+ "( "
				+ "@TankGuid"
				+ ", @TankID"
				+ ", @LookupVesselTypeIndex"
				+ ", @VesselType"
				+ ", @QualityTagGuid"
				+ ", @QualityTagName"
				+ ", @TaggedDate"
				+ ", @TaggedBy"
				+ ", @Memo"
				+ ", @SiteGuid"
				+ ", @DeleteFlag"
				+ ", @CreatedDate"
				+ ", @CreatedBy"
				+ ", @UpdatedDate"
				+ ", @UpdatedBy"
				+ ", @TagNumber"
				+ ", @TankQualityTagLogGuid)";

			cmd.Parameters.AddWithValue("@TankGuid", TankGuid);
			cmd.Parameters.AddWithValue("@TankID", TankID);
			cmd.Parameters.AddWithValue("@LookupVesselTypeIndex", (int)VesselTypeIndex);
			cmd.Parameters.AddWithValue("@VesselType", VesselType);
			cmd.Parameters.AddWithValue("@QualityTagGuid", QualityTagGuid);
			cmd.Parameters.AddWithValue("@QualityTagName", QualityTagName);
			cmd.Parameters.AddWithValue("@TaggedDate", TaggedDate);
			cmd.Parameters.AddWithValue("@TaggedBy", TaggedBy);
			cmd.Parameters.AddWithValue("@Memo", Memo);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@DeleteFlag", Deleted ? 1 : 0);
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@TagNumber", TagNumber);
			cmd.Parameters.AddWithValue("@TankQualityTagLogGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = ""
				+ "UPDATE dbo.tblTankQualityTagLog SET "
				+ "   QualityTagGuid = @QualityTagGuid"
				+ " , QualityTagName  = @QualityTagName"
				+ " , TankGuid = @TankGuid"
				+ " , TankID = @TankID"
				+ " , VesselType = @VesselType"
				+ " , TaggedDate = @TaggedDate"
				+ " , TaggedBy = @TaggedBy"
				+ " , Memo = @Memo"
				+ " , RemovedDate = @RemovedDate"
				+ " , RemovedBy = @RemovedBy"
				+ " , SiteGuid = @SiteGuid"
				+ " , DeleteFlag = @DeleteFlag"
				+ " , CreatedDate = @CreatedDate"
				+ " , CreatedBy = @CreatedBy"
				+ " , UpdatedDate = @UpdatedDate"
				+ " , UpdatedBy = @UpdatedBy"
				+ " , TagNumber = @TagNumber"
				+ " WHERE TankQualityTagLogGuid = @TankQualityTagLogGuid";

			cmd.Parameters.AddWithValue("@QualityTagGuid", QualityTagGuid);
			cmd.Parameters.AddWithValue("@QualityTagName", QualityTagName);
			cmd.Parameters.AddWithValue("@TankGuid", TankGuid);
			cmd.Parameters.AddWithValue("@TankID", TankID);
			cmd.Parameters.AddWithValue("@VesselType", VesselType);
			cmd.Parameters.AddWithValue("@TaggedDate", TaggedDate);
			cmd.Parameters.AddWithValue("@TaggedBy", TaggedBy);
			cmd.Parameters.AddWithValue("@Memo", Memo);
			cmd.Parameters.AddWithValue("@RemovedDate", RemovedDate);
			cmd.Parameters.AddWithValue("@RemovedBy", RemovedBy);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@DeleteFlag", _Deleted ? 1 : 0);
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@TagNumber", TagNumber);
			cmd.Parameters.AddWithValue("@TankQualityTagLogGuid", IdentityGuid);
		}

		public void GetMostRecentByTankIDSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = ""
				+ "SELECT log1.* FROM dbo.tblTankQualityTagLog log1  "
				+ "  JOIN dbo.tblTanks               asset "
				+ "    ON log1.TankGuid = asset.[TankGuid] "
				+ " WHERE  "
				+ " asset.TankID = @TankID"
				+ " AND log1.UpdatedDate = "
				+ "          (SELECT MAX(UpdatedDate) "
				+ "             FROM dbo.tblTankQualityTagLog  log2 "
				+ "            WHERE log1.TankGuid = log2.TankGuid) ";

			cmd.Parameters.AddWithValue("@TankID", TankID);
		}

		public void GetByTagNumberSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = ""
						  + "SELECT * "
						  + "  FROM dbo.tblTankQualityTagLog "
						  + " WHERE "
						  + "   TagNumber = @TagNumber"
						  + "   AND SiteGuid = @SiteGuid";

			cmd.Parameters.AddWithValue("@TagNumber", TagNumber);
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}

		public void GetSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = ""
						  + "SELECT * "
						  + "  FROM dbo.tblTankQualityTagLog "
						  + " WHERE "
						  + "   TankQualityTagLogGuid = @TankQualityTagLogGuid ";

			cmd.Parameters.AddWithValue("@TankQualityTagLogGuid", IdentityGuid);
		}


		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblTankQualityTagLog";
		}

		// The SQL string used by the QualityTag Log form.
		public void EnumerateSQL(SqlCommand cmd, bool bHistorical, string sDateType, DateTimeOffset dateStart, DateTimeOffset dateEnd, string state)
		{
			string SQL;

			SQL = ""
				+ "SELECT "
				+ "  tml1.TankQualityTagLogGuid				AS 'TankQualityTagLogGuid' "
				+ ", tml1.TankID 									AS 'Tank ID' "
				+ ", tml1.QualityTagName	 					AS 'Quality Tag Name' "
				+ "  FROM dbo.tblTankQualityTagLog  tml1 WHERE 1=1 ";

			if (!bHistorical)
			{
				SQL += ""
					  + " AND UpdatedDate = "
					  + "          (SELECT MAX(UpdatedDate) "
					  + "             FROM dbo.tblTankQualityTagLog  tml2 "
					  + "            WHERE tml1.TankGuid = tml2.TankGuid) ";
			}

			switch (state)
			{
				case "Active Tags Only":
					SQL += " AND RemovedDate IS NOT NULL ";
					break;
				case "Removed Tags Only":
					SQL += " AND RemovedDate IS NULL ";
					break;
			}

			if (sDateType != null && 0 < sDateType.Length)
			{
				SQL += " AND ";

				switch (sDateType)
				{
					case "Est Return To Service": SQL += " tml1.EstReturnToServiceDate "; break;
				}

				SQL += " BETWEEN @StartDate AND @EndDate ";

				// We want only the date parts, not the time parts
				cmd.Parameters.AddWithValue("@StartDate", TimeConverter.ToDate(dateStart));
				cmd.Parameters.AddWithValue("@EndDate", TimeConverter.ToDate(dateEnd));
			}

			cmd.CommandText = SQL;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTankQualityTagLog WHERE TankQualityTagLogGuid = @TankQualityTagLogGuid";
			cmd.Parameters.AddWithValue("@TankQualityTagLogGuid", _IdentityGuid);
		}

		public void PreviousTagNumberSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT TOP 1 TagNumber, TaggedDate FROM "
								+ "(SELECT TagNumber, TaggedDate FROM tblEquipmentQualityTagLog WHERE SiteGuid = @SiteGuid "
								+ "UNION SELECT TagNumber, TaggedDate FROM tblTankQualityTagLog WHERE SiteGuid = @SiteGuid) tblResults "
								+ "ORDER BY TagNumber DESC";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}
	}
}
