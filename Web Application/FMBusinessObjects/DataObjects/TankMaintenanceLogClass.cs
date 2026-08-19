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
	[KnownType(typeof(TankMaintenanceLogClass))]
	public class TankMaintenanceLogCollectionClass : List<TankMaintenanceLogClass> { }

   [Serializable]
   [DataContract]
	public class TankMaintenanceLogClass :
											BaseDataObject,
											IComparable
	{
		// Defined in abstract base class.
		// protected Guid					_IdentityGuid;
		// protected Guid					_SiteGuid;
		// protected DateTimeOffset	_CreatedDate;
		// protected string				_CreatedBy;
		// protected DateTimeOffset	_UpdatedDate;
		// protected string				_UpdatedBy;
		#region Protected data members
		// Fields.
		[DataMember]
		protected Guid _TankGuid;
		[DataMember]
		protected string _TankID;
		[DataMember]
		protected VESSEL_TYPE _LookupVesselTypeIndex;
		[DataMember]
		protected string _VesselType;
		[DataMember]
		protected Guid _OperatorPersonnelGuid;
		[DataMember]
		protected string _OperatorID;
		[DataMember]
		protected Guid _MaintenanceReasonGuid;
		[DataMember]
		protected string _MaintenanceReason;
		[DataMember]
		protected int _InServiceFlag;
		[DataMember]
		protected DateTimeOffset _ChangeDate;
		[DataMember]
		protected DateTimeOffset _EstReturnToServiceDate;
		[DataMember]
		protected string _WorkOrder;
		[DataMember]
		protected string _Memo;
		#endregion Protected data members

		#region Public properties
		// public override string ID { get { return _ID; }
		//										 set { SetString("ID", 30, value, ref _ID); } }
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

		public VESSEL_TYPE LookupVesselTypeIndex
		{
			get { return _LookupVesselTypeIndex; }
			set { _LookupVesselTypeIndex = value; }
		}

		public string VesselType
		{
			get { return _VesselType; }
			set { _VesselType = value; }
		}

		public Guid OperatorPersonnelGuid
		{
			get { return _OperatorPersonnelGuid; }
			set { _OperatorPersonnelGuid = value; }
		}

		public string OperatorID
		{
			get { return _OperatorID; }
			set { _OperatorID = value; }
		}

		public Guid MaintenanceReasonGuid
		{
			get { return _MaintenanceReasonGuid; }
			set { _MaintenanceReasonGuid = value; }
		}

		public string MaintenanceReason
		{
			get { return _MaintenanceReason; }
			set { _MaintenanceReason = value; }
		}

		public int InServiceFlag
		{
			get { return _InServiceFlag; }
			set { _InServiceFlag = value; }
		}

		public DateTimeOffset ChangeDate
		{
			get { return _ChangeDate; }
			set { _ChangeDate = value; }
		}

		public DateTimeOffset EstReturnToServiceDate
		{
			get { return _EstReturnToServiceDate; }
			set { _EstReturnToServiceDate = value; }
		}

		public string WorkOrder
		{
			get { return _WorkOrder; }
			set { SetString("WorkOrder", 20, value, ref _WorkOrder); }
		}

		public string Memo
		{
			get { return _Memo; }
			set { SetString("Memo", 1000, value, ref _Memo); }
		}
		#endregion  Public properties

		#region Constructors
		public TankMaintenanceLogClass()
		{
			Reset();
		}
		#endregion

		#region IComparable Interface implementation
		int IComparable.CompareTo(object O)
		{
			TankMaintenanceLogClass TankMaintenanceLog = O as TankMaintenanceLogClass;

			if (TankMaintenanceLog == null)
			{
				throw new Exception("Invalid TankMaintenanceLogClass");
			}

			return this._IdentityGuid.CompareTo(TankMaintenanceLog._IdentityGuid);
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
			_TankID = "";
			_OperatorPersonnelGuid = Guid.Empty;
			_MaintenanceReasonGuid = Guid.Empty;
			_InServiceFlag = 1;
			_ChangeDate = DateTimeOffset.Now;
			_EstReturnToServiceDate = DateTimeOffset.Now;
			_WorkOrder = "";
			_Memo = "";
			_LookupVesselTypeIndex = VESSEL_TYPE.UNDEFINED_VESSEL;
			_VesselType = TankClass.VesselTypeID(_LookupVesselTypeIndex);
		}


		public override void Load(Object o)
		{
			Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count > 0)
				{
					DataRow Row = Table.Rows[0];

					// Field-type data members.
					_TankGuid = DataObject.getValue<Guid>(Row["TankGuid"], Guid.Empty);
					_OperatorPersonnelGuid = DataObject.getValue<Guid>(Row["OperatorPersonnelGuid"], Guid.Empty);
					_MaintenanceReasonGuid = DataObject.getValue<Guid>(Row["MaintenanceReasonGuid"], Guid.Empty);
					_InServiceFlag = DataObject.getValue<byte>(Row["InServiceFlag"], 1);

					_LookupVesselTypeIndex = DataObject.getValue<VESSEL_TYPE>(Row["LookupVesselTypeIndex"], VESSEL_TYPE.UNDEFINED_VESSEL);
					_VesselType = TankClass.VesselTypeID(_LookupVesselTypeIndex);

					_TankID = DataObject.getValue<string>(Row["TankID"], "");
					_OperatorID = DataObject.getValue<string>(Row["OperatorID"], "");
					_MaintenanceReason = DataObject.getValue<string>(Row["MaintenanceReason"], "");
					_ChangeDate = DataObject.getValue<DateTimeOffset>(Row["ChangeDate"], DateTimeOffset.Now);
					_EstReturnToServiceDate = DataObject.getValue<DateTimeOffset>(Row["EstReturnToServiceDate"], DateTimeOffset.Now);
					_WorkOrder = DataObject.getValue<string>(Row["WorkOrder"], "");
					_Memo = DataObject.getValue<string>(Row["Memo"], "");

					// Defined in base-class.
					_IdentityGuid = DataObject.getValue<Guid>(Row["TankMaintenanceLogGuid"], Guid.Empty);
					_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
					_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
					_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
					_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
					_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				}
			}
		}


		public void HoursPassed(SqlCommand cmd)
		{
			cmd.CommandText = ""
					+ "SELECT ISNULL(MAX(ChangeDate), SYSDATETIMEOFFSET()) AS ChangeDate"
					+ " FROM dbo.tblTankMaintenanceLog"
					+ " WHERE TankGuid = @TankGuid";

			cmd.Parameters.AddWithValue("@TankGuid", TankGuid);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			string SQL;

			SQL = ""
				+ "INSERT INTO dbo.tblTankMaintenanceLog "
				+ "( "
				+ "TankGuid"
				+ ", TankID"
				+ ", LookupVesselTypeIndex"
				+ ", VesselType"
				+ ", OperatorID"
				+ ", SiteGuid"
				+ ", InServiceFlag"
				+ ", ChangeDate"
				+ ", WorkOrder"
				+ ", Memo"
				+ ", CreatedDate"
				+ ", CreatedBy"
				+ ", UpdatedDate"
				+ ", UpdatedBy"
				+ ", TankMaintenanceLogGuid";

			if (OperatorPersonnelGuid != Guid.Empty)
			{
				SQL += ", OperatorPersonnelGuid";
			}
			if (InServiceFlag == 0)
			{
				SQL += ", MaintenanceReasonGuid"
						+ ", MaintenanceReason"
						+ ", EstReturnToServiceDate";
			}

			SQL +=
				") VALUES ("
				+ "@TankGuid"
				+ ", @TankID"
				+ ", @LookupVesselTypeIndex"
				+ ", @VesselType"
				+ ", @OperatorID"
				+ ", @SiteGuid"
				+ ", @InServiceFlag"
				+ ", @ChangeDate"
				+ ", @WorkOrder"
				+ ", @Memo"
				+ ", @CreatedDate"
				+ ", @CreatedBy"
				+ ", @UpdatedDate"
				+ ", @UpdatedBy"
				+ ", @TankMaintenanceLogGuid";
				
			cmd.Parameters.AddWithValue("@TankGuid", TankGuid);
			cmd.Parameters.AddWithValue("@TankID", TankID);
			cmd.Parameters.AddWithValue("@LookupVesselTypeIndex", LookupVesselTypeIndex);
			cmd.Parameters.AddWithValue("@VesselType", VesselType);
			cmd.Parameters.AddWithValue("@OperatorID", OperatorID);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@InServiceFlag", (byte)InServiceFlag);
			cmd.Parameters.AddWithValue("@ChangeDate", ChangeDate);
			cmd.Parameters.AddWithValue("@WorkOrder", WorkOrder);
			cmd.Parameters.AddWithValue("@Memo", Memo);
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@TankMaintenanceLogGuid", _IdentityGuid);

			if (OperatorPersonnelGuid != Guid.Empty)
			{
				SQL += ", @OperatorPersonnelGuid";
				cmd.Parameters.AddWithValue("@OperatorPersonnelGuid", OperatorPersonnelGuid);
			}
			if (InServiceFlag == 0)
			{
				SQL += ", @MaintenanceReasonGuid"
						+ ", @MaintenanceReason"
						+ ", @EstReturnToServiceDate";

				cmd.Parameters.AddWithValue("@MaintenanceReasonGuid", MaintenanceReasonGuid);
				cmd.Parameters.AddWithValue("@MaintenanceReason", MaintenanceReason);
				cmd.Parameters.AddWithValue("@EstReturnToServiceDate", EstReturnToServiceDate);
			}

			SQL += ")";

			cmd.CommandText = SQL;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			string SQL = ""
				+ "UPDATE dbo.tblTankMaintenanceLog SET "
				+ "   TankGuid = @TankGuid	"
				+ " , TankID = @TankID"
				+ " , TankType = @TankType"
				+ " , OperatorPersonnelGuid = @OperatorPersonnelGuid"
				+ " , OperatorID = @OperatorID"
				+ " , SiteGuid = @SiteGuid"
				+ " , InServiceFlag = @InServiceFlag"
				+ " , ChangeDate = @ChangeDate"
				+ " , WorkOrder = @WorkOrder"
				+ " , Memo = @Memo"
				+ " , MaintenanceReasonGuid = @MaintenanceReasonGuid"
				+ " , MaintenanceReason = @MaintenanceReason"
				+ " , CreatedDate = @CreatedDate"
				+ " , CreatedBy = @CreatedBy"
				+ " , UpdatedDate = @UpdatedDate"
				+ " , UpdatedBy = @UpdatedBy";

			cmd.Parameters.AddWithValue("@TankGuid", TankGuid);
			cmd.Parameters.AddWithValue("@TankID", TankID);
			cmd.Parameters.AddWithValue("@VesselType", VesselType);
			if (OperatorPersonnelGuid != Guid.Empty)
				cmd.Parameters.AddWithValue("@OperatorPersonnelGuid", OperatorPersonnelGuid);
			else
				cmd.Parameters.AddWithValue("@OperatorPersonnelGuid", DBNull.Value);
			cmd.Parameters.AddWithValue("@OperatorID", OperatorID);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@InServiceFlag", InServiceFlag);
			cmd.Parameters.AddWithValue("@ChangeDate", ChangeDate);
			cmd.Parameters.AddWithValue("@WorkOrder", WorkOrder);
			cmd.Parameters.AddWithValue("@Memo", Memo);
			if (MaintenanceReasonGuid != Guid.Empty)
				cmd.Parameters.AddWithValue("@MaintenanceReasonGuid", MaintenanceReasonGuid);
			else
				cmd.Parameters.AddWithValue("@MaintenanceReasonGuid", DBNull.Value);
			cmd.Parameters.AddWithValue("@MaintenanceReason", MaintenanceReason);
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);

			if (InServiceFlag == 0)
			{
				SQL += " , EstReturnToServiceDate = @EstReturnToServiceDate";
				cmd.Parameters.AddWithValue("@EstReturnToServiceDate", EstReturnToServiceDate);
			}

			SQL += ""
				+ " WHERE TankMaintenanceLogGuid = @TankMaintenanceLogGuid";

			cmd.Parameters.AddWithValue("@TankMaintenanceLogGuid", IdentityGuid);

			cmd.CommandText = SQL;
		}


		public void GetSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT * "
						  + "  FROM dbo.tblTankMaintenanceLog "
						  + " WHERE "
						  + "  TankMaintenanceLogGuid = @TankMaintenanceLogGuid";

			cmd.Parameters.AddWithValue("@TankMaintenanceLogGuid", IdentityGuid);
		}


		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblTankMaintenanceLog";
		}


		// The SQL string used by the Maintenance Log form.
		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, bool bHistorical, string sDateType, DateTimeOffset dateStart, DateTimeOffset dateEnd, Guid assetGuid)
		{
			string SQL;
			SQL = ""
				+ "SELECT "
				+ "  tml1.TankMaintenanceLogGuid			AS 'TankMaintenanceLogGuid' "
				+ ", tml1.TankID 									AS 'Tank ID' "
				+ ", CASE tml1.InServiceFlag "
				+ "    WHEN 1 THEN 'Y' "
				+ "    WHEN 0 THEN 'N' "
				+ "    END 											AS 'In Service' "
				+ ", mr.Description 					         AS 'Maintenance Reason' "
				+ ", tml1.EstReturnToServiceDate				AS 'Estimated Return To Service' "
				+ ", tml1.WorkOrder 								AS 'Work Order' "
				+ ", LEFT(tml1.Memo, 50)						AS 'Memo' "
				+ "  FROM dbo.tblTankMaintenanceLog  tml1 "
				+ "   LEFT JOIN dbo.tblMaintenanceReasons mr ON mr.MaintenanceReasonGuid = tml1.MaintenanceReasonGuid "
				+ " WHERE 1 = 1 ";

			if (assetGuid != Guid.Empty)
			{
				SQL += " AND tml1.TankGuid = @AssetGuid ";
				cmd.Parameters.AddWithValue("@AssetGuid", assetGuid);
			}
			if (!bHistorical)
			{
				SQL += ""
					  + " AND ChangeDate = "
					  + "          (SELECT MAX(ChangeDate) "
					  + "             FROM dbo.tblTankMaintenanceLog  tml2 "
					  + "            WHERE tml1.TankGuid = tml2.TankGuid) ";
			}

			if (sDateType != null && 0 < sDateType.Length)
			{
				SQL += (bHistorical) ? " WHERE " : " AND ";

				switch (sDateType)
				{
					case "Est Return To Service": SQL += " [Estimated Return To Service] "; break;
					case "QC Due Date": SQL += " [QC Due Date] "; break;
				}

				SQL += " BETWEEN @StartDate AND @EndDate ";

				// We want only the date parts, not the time parts
				cmd.Parameters.AddWithValue("@StartDate", TimeConverter.ToDate(dateStart));
				cmd.Parameters.AddWithValue("@EndDate", TimeConverter.ToDate(dateEnd));
			}

			cmd.CommandText = SQL;
		}

		public void MaintenanceReasonUsedCount(SqlCommand cmd, Guid maintenanceReasonGuid)
		{
			cmd.CommandText = "SELECT Count(*) FROM tblTankMaintenanceLog WHERE MaintenanceReasonGuid = @MaintenanceReasonGuid";
			cmd.Parameters.AddWithValue("@MaintenanceReasonGuid", maintenanceReasonGuid);
		}

		/// <summary>
		/// Returns SQL for most recent Maintenance Logs of tanks that are not in service. 
		/// </summary>
		/// <param name="maintenanceReasonGuid"></param>
		/// <returns></returns>
		public void EnumerateByMaintenanceReasonSQL(SqlCommand cmd, Guid maintenanceReasonGuid)
		{
			cmd.CommandText =
				"SELECT * FROM tblTankMaintenanceLog tml1 WHERE MaintenanceReasonGuid = @MaintenanceReasonGuid"
				  + " AND InServiceFlag = 0 AND ChangeDate = "
							+ " (SELECT MAX(ChangeDate) "
								+ " FROM dbo.tblTankMaintenanceLog  tml2 "
								+ " WHERE tml1.TankGuid = tml2.TankGuid) ";
			cmd.Parameters.AddWithValue("@MaintenanceReasonGuid", maintenanceReasonGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTankMaintenanceLog WHERE TankMaintenanceLogGuid = @TankMaintenanceLogGuid";
			cmd.Parameters.AddWithValue("@TankMaintenanceLogGuid", _IdentityGuid);
		}
	}
}
