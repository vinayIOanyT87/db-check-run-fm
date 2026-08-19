using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(EquipmentMaintenanceLogClass))]
	public class EquipmentMaintenanceLogCollectionClass : List<EquipmentMaintenanceLogClass> { }

	[DataContract]
   [Serializable]
	[QueryWriterTopic(typeof(EquipmentMaintenanceLogClass), "Maintenance Log", SupportsArchiveQuery = true)]
	[QueryWriterTopicSecurity(RIGHT.ADD_MAINTENANCE_RECORD)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_MAINTENANCE_RECORD)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_MAINTENANCE_RECORD)]
	public class EquipmentMaintenanceLogClass : BaseDataObject, IComparable
	{
		#region Data Members.
		// Defined in abstract base class.
		// protected Guid					_IdentityGuid;
		// protected Guid					_SiteGuid;
		// protected DateTimeOffset	_CreatedDate;
		// protected string				_CreatedBy;
		// protected DateTimeOffset	_UpdatedDate;
		// protected string				_UpdatedBy;

		// Fields.
		[DataMember]
		protected Guid _EquipmentGuid;
		[DataMember]
		protected string _EquipmentID;
		[DataMember]
		protected string _EquipmentType;
		[DataMember]
		protected Guid _OperatorPersonnelGuid;
		[DataMember]
		protected string _OperatorID;
		[DataMember]
		protected Guid _MaintenanceReasonGuid;
		[DataMember]
		protected string _MaintenanceReason;
		[DataMember]
		protected byte _InServiceFlag;
		[DataMember]
		protected DateTimeOffset _ChangeDate;
		[DataMember]
		protected DateTimeOffset _EstReturnToServiceDate;
		[DataMember]
		protected string _WorkOrder;
		[DataMember]
		protected string _Memo;
		#endregion

		#region Accessors
		public Guid EquipmentGuid
		{
			get { return _EquipmentGuid; }
			set { _EquipmentGuid = value; }
		}

		[QueryWriterField("Type", "Type", GenerateSelect = false)]
		public string EquipmentType
		{
			get { return _EquipmentType; }
			set { _EquipmentType = value; }
		}

		[QueryWriterField("Asset Type", "AssetType", GenerateSelect = false)]
		protected string AssetTypeForQuery { get; set; }

		[QueryWriterField("ID", "ID", GenerateSelect = false)]
		public string EquipmentID
		{
			get { return _EquipmentID; }
			set { _EquipmentID = value; }
		}

		public Guid OperatorPersonnelGuid
		{
			get { return _OperatorPersonnelGuid; }
			set { _OperatorPersonnelGuid = value; }
		}

		[QueryWriterField("Operator ID", false)]
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

		[QueryWriterField("Reason", false)]
		public string MaintenanceReason
		{
			get { return _MaintenanceReason; }
			set { _MaintenanceReason = value; }
		}

		[QueryWriterField("In Service", false)]
		public byte InServiceFlag
		{
			get { return _InServiceFlag; }
			set { _InServiceFlag = value; }
		}

		[QueryWriterField("Change Date", false)]
		public DateTimeOffset ChangeDate
		{
			get { return _ChangeDate; }
			set { _ChangeDate = value; }
		}

		[QueryWriterField("Est Return To Service", false)]
		public DateTimeOffset EstReturnToServiceDate
		{
			get { return _EstReturnToServiceDate; }
			set { _EstReturnToServiceDate = value; }
		}

		[QueryWriterField("Work Order", false)]
		public string WorkOrder
		{
			get { return _WorkOrder; }
			set { SetString("WorkOrder", 20, value, ref _WorkOrder); }
		}

		[QueryWriterField("Memo", false)]
		public string Memo
		{
			get { return _Memo; }
			set { SetString("Memo", 1000, value, ref _Memo); }
		}

		[QueryWriterField("Created By", "CreatedBy", GenerateSelect = false)]
		public string CreatedByQuery { get { return this._CreatedBy; } }

		[QueryWriterField("Created Date", "CreatedDate", GenerateSelect = false)]
		public DateTimeOffset CreatedDateQuery { get { return this._CreatedDate; } }

		[QueryWriterField("Updated By", "UpdatedBy", GenerateSelect = false)]
		public string UpdatedByQuery { get { return this._UpdatedBy; } }

		[QueryWriterField("Updated Date", "UpdatedDate", GenerateSelect = false)]
		public DateTimeOffset UpdatedDateQuery { get { return this._UpdatedDate; } }

		#endregion

		#region Constructors
		public EquipmentMaintenanceLogClass()
		{
			Reset();
		}
		#endregion

		#region Abstract base class method overrides

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EQUIPMENT_MAINTENANCE_LOG; }
		}

		#endregion

		#region IComparable Interface implementation
		int IComparable.CompareTo(object O)
		{
			EquipmentMaintenanceLogClass EquipmentMaintenanceLog = O as EquipmentMaintenanceLogClass;

			if (EquipmentMaintenanceLog == null)
			{
				throw new Exception("Invalid EquipmentMaintenanceLogClass");
			}

			return this._IdentityGuid.CompareTo(EquipmentMaintenanceLog._IdentityGuid);
		}


		// Common place to initialize all base-class and "field"-type member variables.
		public override void Reset()
		{
			// Handles _IdentityGuid, _ID, _SiteGuid, _CreatedDate, _CreatedBy, _UpdatedDate,
			// _UpdatedBy, and _Deleted.
			base.Reset();

			// Field member variables.
			_EquipmentGuid = Guid.Empty;
			_EquipmentID = "";
			_EquipmentType = "";
			_OperatorPersonnelGuid = Guid.Empty;
			_OperatorID = "";
			_MaintenanceReasonGuid = Guid.Empty;
			_InServiceFlag = 1;

			_ChangeDate = DateTimeOffset.Now;
			_EstReturnToServiceDate = DateTimeOffset.Now;

			_WorkOrder = "";
			_Memo = "";
		}
		#endregion

		public void HoursPassed(SqlCommand cmd)
		{
			cmd.CommandText = ""
				+ "SELECT ISNULL(MAX(ChangeDate), SYSDATETIMEOFFSET()) AS ChangeDate"
				+ "  FROM dbo.tblEquipmentMaintenanceLog "
				+ " WHERE EquipmentGuid = @EquipmentGuid";

			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@EquipmentGuid"].Value = EquipmentGuid;
		}

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = ""
				+ "INSERT INTO dbo.tblEquipmentMaintenanceLog "
				+ "( "
				+ "EquipmentGuid"
				+ ", EquipmentID"
				+ ", EquipmentType"
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
				+ ", EquipmentMaintenanceLogGuid";

			if (this.OperatorPersonnelGuid != Guid.Empty)
			{
				cmd.CommandText += ", OperatorPersonnelGuid";
			}
			if (this.InServiceFlag == 0)
			{
				cmd.CommandText += ", MaintenanceReasonGuid"
										+ ", MaintenanceReason"
										+ ", EstReturnToServiceDate";
			}

			cmd.CommandText +=
				") VALUES ("
				+ "@EquipmentGuid,"
				+ "@EquipmentID,"
				+ "@EquipmentType,"
				+ "@OperatorID,"
				+ "@SiteGuid,"
				+ "@InServiceFlag,"
				+ "@ChangeDate,"
				+ "@WorkOrder,"
				+ "@Memo,"
				+ "@CreatedDate,"
				+ "@CreatedBy,"
				+ "@UpdatedDate,"
				+ "@UpdatedBy,"
				+ "@EquipmentMaintenanceLogGuid";

			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EquipmentID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentType", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@OperatorID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@InServiceFlag", SqlDbType.TinyInt);
			cmd.Parameters.Add("@ChangeDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@WorkOrder", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Memo", SqlDbType.NVarChar, 1000);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@EquipmentMaintenanceLogGuid", SqlDbType.UniqueIdentifier);


			cmd.Parameters["@EquipmentGuid"].Value = EquipmentGuid;
			cmd.Parameters["@EquipmentID"].Value = EquipmentID;
			cmd.Parameters["@EquipmentType"].Value = EquipmentType;
			cmd.Parameters["@OperatorID"].Value = OperatorID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@InServiceFlag"].Value = InServiceFlag;
			cmd.Parameters["@ChangeDate"].Value = ChangeDate;
			cmd.Parameters["@WorkOrder"].Value = WorkOrder;
			cmd.Parameters["@Memo"].Value = Memo;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@EquipmentMaintenanceLogGuid"].Value = _IdentityGuid;

			if (this.OperatorPersonnelGuid != Guid.Empty)
			{
				cmd.CommandText += ", @OperatorPersonnelGuid";
				cmd.Parameters.AddWithValue("@OperatorPersonnelGuid", OperatorPersonnelGuid);
			}
			if (this.InServiceFlag == 0)
			{
				cmd.CommandText += ", @MaintenanceReasonGuid,"
										+ " @MaintenanceReason";

				if (this.EstReturnToServiceDate.Year != 1)
				{
					cmd.CommandText += ", @EstReturnToServiceDate";
					cmd.Parameters.Add("@EstReturnToServiceDate", SqlDbType.DateTimeOffset);

					cmd.Parameters["@EstReturnToServiceDate"].Value = EstReturnToServiceDate;
				}

				cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@MaintenanceReason", SqlDbType.NVarChar, 50);

				cmd.Parameters["@MaintenanceReasonGuid"].Value = MaintenanceReasonGuid;
				cmd.Parameters["@MaintenanceReason"].Value = MaintenanceReason;
			}

			cmd.CommandText += ")";
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			Debug.Assert(this.SiteGuid != Guid.Empty);

			cmd.CommandText = ""
				+ "UPDATE dbo.tblEquipmentMaintenanceLog SET "
				+ "   EquipmentGuid	= @EquipmentGuid "
				+ " , EquipmentID 	= @EquipmentID "
				+ " , EquipmentType 	= @EquipmentType "
				+ " , OperatorPersonnelGuid = @OperatorPersonnelGuid "
				+ " , OperatorID 		= @OperatorID "
				+ " , SiteGuid 		= @SiteGuid "
				+ " , InServiceFlag 	= @InServiceFlag "
				+ " , ChangeDate 		= @ChangeDate "
				+ " , WorkOrder 		= @WorkOrder "
				+ " , Memo 				= @Memo "
				+ " , MaintenanceReasonGuid = @MaintenanceReasonGuid "
				+ " , MaintenanceReason = @MaintenanceReason "
				+ " , CreatedDate 	= @CreatedDate "
				+ " , CreatedBy 		= @CreatedBy "
				+ " , UpdatedDate 	= @UpdatedDate "
				+ " , UpdatedBy 		= @UpdatedBy ";

			if (this.InServiceFlag == 0)
			{
				cmd.CommandText += ""
					+ " , EstReturnToServiceDate = @EstReturnToServiceDate ";

				cmd.Parameters.Add("@EstReturnToServiceDate", SqlDbType.DateTimeOffset);

				cmd.Parameters["@EstReturnToServiceDate"].Value = EstReturnToServiceDate;
			}

			cmd.CommandText += ""
				+ " WHERE EquipmentMaintenanceLogGuid = @IdentityGuid ";

			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EquipmentID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentType", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@OperatorPersonnelGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@OperatorID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@InServiceFlag", SqlDbType.TinyInt);
			cmd.Parameters.Add("@ChangeDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@WorkOrder", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Memo", SqlDbType.NVarChar, 1000);
			cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@MaintenanceReason", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@EquipmentGuid"].Value = EquipmentGuid;
			cmd.Parameters["@EquipmentID"].Value = EquipmentID;
			cmd.Parameters["@EquipmentType"].Value = EquipmentType;
			if (OperatorPersonnelGuid != Guid.Empty)
				cmd.Parameters["@OperatorPersonnelGuid"].Value = OperatorPersonnelGuid;
			else
				cmd.Parameters["@OperatorPersonnelGuid"].Value = DBNull.Value;
			cmd.Parameters["@OperatorID"].Value = OperatorID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@InServiceFlag"].Value = InServiceFlag;
			cmd.Parameters["@ChangeDate"].Value = ChangeDate;
			cmd.Parameters["@WorkOrder"].Value = WorkOrder;
			cmd.Parameters["@Memo"].Value = Memo;
			if (MaintenanceReasonGuid != Guid.Empty)
				cmd.Parameters["@MaintenanceReasonGuid"].Value = MaintenanceReasonGuid;
			else
				cmd.Parameters["@MaintenanceReasonGuid"].Value = DBNull.Value;
			cmd.Parameters["@MaintenanceReason"].Value = MaintenanceReason;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
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
					_EquipmentGuid = DataObject.getValue<Guid>(Row["EquipmentGuid"], Guid.Empty);
					_OperatorPersonnelGuid = DataObject.getValue<Guid>(Row["OperatorPersonnelGuid"], Guid.Empty);
					_MaintenanceReasonGuid = DataObject.getValue<Guid>(Row["MaintenanceReasonGuid"], Guid.Empty);
					_InServiceFlag = DataObject.getValue<byte>(Row["InServiceFlag"], 1);
					_EquipmentType = DataObject.getValue<string>(Row["EquipmentType"], "");
					_EquipmentID = DataObject.getValue<string>(Row["EquipmentID"], "");
					_OperatorID = DataObject.getValue<string>(Row["OperatorID"], "");
					_MaintenanceReason = DataObject.getValue<string>(Row["MaintenanceReason"], "");
					_ChangeDate = DataObject.getValue<DateTimeOffset>(Row["ChangeDate"], DateTimeOffset.Now);
					_EstReturnToServiceDate = DataObject.getValue<DateTimeOffset>(Row["EstReturnToServiceDate"], DateTimeOffset.Now);
					_WorkOrder = DataObject.getValue<string>(Row["WorkOrder"], "");
					_Memo = DataObject.getValue<string>(Row["Memo"], "");

					// Defined in base-class.
					_IdentityGuid = DataObject.getValue<Guid>(Row["EquipmentMaintenanceLogGuid"], Guid.Empty);
					_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
					_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
					_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
					_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
					_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				}
			}
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, bool bHistorical, string sDateType, DateTimeOffset dateStart, DateTimeOffset dateEnd, Guid assetGuid)
		{
			EquipmentClass equipment = new EquipmentClass();

			// First get the Equipments.
			string sql = ""
				+ "SELECT "
				+ "  'EQUIPMENT'									AS 'Asset Type' "
				+ ", log1.EquipmentMaintenanceLogGuid		AS 'MaintenanceLogGuid' "
				+ ", log1.EquipmentID AS 'Asset ID' "
				+ ", CASE InServiceFlag "
				+ "    WHEN 1 THEN 'Y' "
				+ "    WHEN 0 THEN 'N' "
				+ "    END 											AS 'In Service' "
				+ ", mr.Description								AS 'Maintenance Reason' "
				+ ", log1.EstReturnToServiceDate				AS 'Estimated Return To Service' "
				+ ", asset.QCDate									AS 'QC Due Date' "
				+ ", log1.WorkOrder 								AS 'Work Order' "
				+ ", LEFT(log1.Memo, 50)						AS 'Memo'"
				+ ", ChangeDate									AS 'Change Date' "
				+ ", EquipmentType								AS 'Type' "
				+ "  FROM dbo.tblEquipmentMaintenanceLog	log1 "
				+ "  LEFT JOIN dbo.tblEquipment asset "
                + "  ON asset.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', log1.EquipmentGuid, @SiteGuid) "                  					
				+ "   LEFT JOIN dbo.tblMaintenanceReasons mr ON mr.MaintenanceReasonGuid = log1.MaintenanceReasonGuid "
				+ " WHERE 1=1 ";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			if (assetGuid != Guid.Empty)
			{
				sql += " AND log1.EquipmentGuid = @AssetGuid ";
				cmd.Parameters.Add("@AssetGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@AssetGuid"].Value = assetGuid;
			}

			if (!bHistorical)
			{
				sql += " AND ManagedEquipmentFlag = 1 AND " + equipment.AppendSiteWhereClause(cmd, security, "asset", "EquipmentGuid")
					 + "    AND ChangeDate = "
					 + "          (SELECT MAX(ChangeDate) "
					 + "             FROM dbo.tblEquipmentMaintenanceLog  log3 "
					 + "            WHERE log1.EquipmentGuid = log3.EquipmentGuid) ";
			}

			// Then combine the Tanks.
			sql += " UNION ";

			sql += ""
				+ "SELECT "
				+ "  'TANK'											AS 'Asset Type' "
				+ ", log1.TankMaintenanceLogGuid			AS 'MaintenanceLogGuid' "
				+ ", log1.TankID AS 'Asset ID' "
				+ ", CASE  InServiceFlag "
				+ "    WHEN 1 THEN 'Y' "
				+ "    WHEN 0 THEN 'N' "
				+ "    END 											AS 'In Service' "
				+ ", mr.Description 								AS 'Maintenance Reason' "
				+ ", log1.EstReturnToServiceDate				AS 'Estimated Return To Service' "
				+ ", NULL											AS 'QC Due Date' "
				+ ", log1.WorkOrder 								AS 'Work Order' "
				+ ", LEFT(log1.Memo, 50)						AS 'Memo' "
				+ ", ChangeDate									AS 'Change Date' "
				+ ", VesselType									AS 'Type' "
				+ "  FROM dbo.tblTankMaintenanceLog  log1 "
				+ "  LEFT JOIN dbo.tblTanks asset "
				+ "    ON log1.TankGuid = asset.TankGuid "
				+ "   LEFT JOIN dbo.tblMaintenanceReasons mr ON mr.MaintenanceReasonGuid = log1.MaintenanceReasonGuid "
				+ " WHERE 1=1 ";

			if (assetGuid != Guid.Empty)
			{
				sql += " AND log1.TankGuid = @AssetGuid ";
			}

			if (!bHistorical)
			{
				sql += " AND asset.SiteGuid = @SiteGuid "
					 + " AND ChangeDate = "
					 + "          (SELECT MAX(ChangeDate) "
					 + "             FROM dbo.tblTankMaintenanceLog  log3 "
					 + "            WHERE log1.TankGuid = log3.TankGuid) ";
				
			}            

			// Now wrap the two SELECTs into one, so we can use their common 
			// column names to search against.
			cmd.CommandText = "SELECT * FROM ( " + sql + " ) AS tBoth ";

			// Now append the WHERE clause.
			if (sDateType != null && 0 < sDateType.Length)
			{
				if (sDateType == "Estimated Return To Service")
				{
					cmd.CommandText += " WHERE tBoth.[Estimated Return To Service] ";
				}
				else if (sDateType == "QC Due Date")
				{
					cmd.CommandText += " WHERE tBoth.[QC Due Date] ";
				}
				else
				{
					throw new Exception("Unrecognized filter selection");
				}

				cmd.CommandText += " BETWEEN @DateStart AND @DateEnd ";

				cmd.Parameters.Add("@DateStart", SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@DateEnd", SqlDbType.DateTimeOffset);

				cmd.Parameters["@DateStart"].Value = dateStart;
				cmd.Parameters["@DateEnd"].Value = dateEnd;
			}

		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT log1.* FROM tblEquipmentMaintenanceLog log1 "
				+ "  JOIN dbo.tblEquipment asset "
				+ "    ON log1.EquipmentGuid = asset.EquipmentGuid "
				+ " WHERE  ManagedEquipmentFlag = 1";
		}

		/// <summary>
		/// Returns SQL for most recent Maintenance Logs of equipment that are not in service. 
		/// </summary>
		/// <param name="MaintenanceReasonGuid"></param>
		/// <returns></returns>
		public void EnumerateByMaintenanceReasonSQL(SqlCommand cmd, Guid MaintenanceReasonGuid)
		{
			cmd.CommandText = "SELECT * FROM tblEquipmentMaintenanceLog log1 WHERE MaintenanceReasonGuid = @MaintenanceReasonGuid " +
					 " AND InServiceFlag = 0 " +
					 " AND ChangeDate =  " +
									  " (SELECT MAX(ChangeDate) " +
									  " FROM dbo.tblEquipmentMaintenanceLog log3 " +
									  " WHERE log1.EquipmentGuid = log3.EquipmentGuid) ";

			cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MaintenanceReasonGuid"].Value = MaintenanceReasonGuid;

		}

		public void MaintenanceReasonUsedCount(SqlCommand cmd, Guid maintenanceReasonGuid)
		{
			cmd.CommandText =
				"SELECT Count(*) AS 'RecordCount' FROM tblEquipmentMaintenanceLog WHERE MaintenanceReasonGuid = @MaintenanceReasonGuid ";

			cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MaintenanceReasonGuid"].Value = maintenanceReasonGuid;

		}

		public void GetSQL(SqlCommand cmd)
		{
			cmd.CommandText = ""
					  + "SELECT * "
					  + "  FROM dbo.tblEquipmentMaintenanceLog "
					  + " WHERE "
					  + "  EquipmentMaintenanceLogGuid = @EquipmentMaintenanceLogGuid ";

			cmd.Parameters.Add("@EquipmentMaintenanceLogGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@EquipmentMaintenanceLogGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblEquipmentMaintenanceLog WHERE EquipmentMaintenanceLogGuid = @EquipmentMaintenanceLogGuid";

			cmd.Parameters.Add("@EquipmentMaintenanceLogGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@EquipmentMaintenanceLogGuid"].Value = IdentityGuid;
		}

		public void QueryWriterSQL(SqlCommand cmd, SecurityClass Security, string selectClause, string dbName)
		{
			string SQL;

			SQL = "{0},* from " +
				"( " +
				"select  " +
					"'E' + CAST(EquipmentMaintenanceLogGuid AS VARCHAR(40)) as 'EntityGuid', " +
					"'Equipment' as 'AssetType', " +
					"EquipmentID as 'ID', " +
					"EquipmentType as 'Type', " +
					"OperatorID, " +
					"MaintenanceReason, " +
					"InServiceFlag, " +
					"WorkOrder, " +
					"Memo, " +
					"ChangeDate, " +
					"EstReturnToServiceDate, " +
					"CreatedBy, " +
					"CreatedDate, " +
					"UpdatedBy, " +
					"UpdatedDate " +
				"from [{1}]..tblEquipmentMaintenanceLog " +
				"where SiteGuid = @SiteGuid " +
				"union  " +
				"select  " +
					"'T' + CAST(TankMaintenanceLogGuid AS VARCHAR(40)) as 'EntityGuid', " +
					"'Tank' as 'AssetType', " +
					"TankID as 'ID', " +
					"VesselType as 'Type', " +
					"OperatorID, " +
					"MaintenanceReason, " +
					"InServiceFlag, " +
					"WorkOrder, " +
					"Memo, " +
					"ChangeDate, " +
					"EstReturnToServiceDate, " +
					"CreatedBy, " +
					"CreatedDate, " +
					"UpdatedBy, " +
					"UpdatedDate " +
				"from [{1}]..tblTankMaintenanceLog " +
				"where SiteGuid = @SiteGuid " +
				") tblResult " +
				"WHERE 1=1 ";

			cmd.CommandText = string.Format(SQL, selectClause, dbName);
			cmd.Parameters.AddWithValue("@SiteGuid", Security.SiteGuid);
		}

		/// <summary>
		/// This method is used when the edit button is clicked on the query writer results form
		/// </summary>
		/// <returns>The page corresponding to this entity</returns>
		public string DetailPageReference()
		{
			return "MaintenanceWebApp\\MaintenanceAddRecordForm.aspx";
		}

		public void QueryWriterPostProcess(SecurityClass security, DataSet set)
		{
			CensorFieldsIfNecessary(security, set);
		}

		private void CensorFieldsIfNecessary(SecurityClass security, DataSet set)
		{
			if (security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) == false
			&& security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) == false)
			{
				set.Tables[0].Rows.Clear();
			}
		}

	}
}
