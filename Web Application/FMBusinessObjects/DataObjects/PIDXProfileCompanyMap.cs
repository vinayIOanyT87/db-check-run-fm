using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
   #region PIDX Profile Company Map Collection Class
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(PIDXProfileCompanyMapClass))]
	public class PIDXProfileCompanyMapCollectionClass : CollectionBase
	{

		public void Add(PIDXProfileCompanyMapClass PIDXProfileCompanyMap)
		{
			List.Add(PIDXProfileCompanyMap);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw new Exception("Invalid Index");
			}
			else
			{
				List.RemoveAt(index);
			}
		}

		public PIDXProfileCompanyMapClass this[int Index]
		{
			get { return (PIDXProfileCompanyMapClass)List[Index]; }
			set { List[Index] = value; }
		}

		public PIDXProfileCompanyMapClass Find(Guid PIDXProfileGuid)
		{
			foreach (PIDXProfileCompanyMapClass PIDXProfileCompanyMap in List)
				if (PIDXProfileCompanyMap.PIDXProfileGuid == PIDXProfileGuid)
					return PIDXProfileCompanyMap;

			return null;
		}
	}
	#endregion


	#region PIDX Profile Company Map Class
	/// <summary>
	/// Summary description for PIDXProfileCompanyMapClass.
	/// </summary>
   [Serializable]
   [DataContract]
	public class PIDXProfileCompanyMapClass : BaseDataObject
	{
		#region Private Data members
		[DataMember]
		private Guid _PIDXProfileGuid;
		[DataMember]
		private Guid _CompanyPersonnelToShipToBillToGuid;
		[DataMember]
		private string _SellerID;
		[DataMember]
		private string _ShipperID;
		[DataMember]
		private string _ConsigneeNumber;
		[DataMember]
		private bool _DenialOverride;
		[DataMember]
		private bool _UnavailableOverride;
		[DataMember]
		private string _ShipToID;
		[DataMember]
		private string _ShipToName;
		[DataMember]
		private string _ShipToAddress;
		[DataMember]
		private string _ShipToCity;
		[DataMember]
		private string _ShipToState;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the PIDX Profile Company Map Class
		/// </summary>
		public PIDXProfileCompanyMapClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		public override string ID
		{
			get { return _ID; }
			set { SetString("ID", 30, value, ref _ID); }
		}

		public Guid PIDXProfileGuid
		{
			get { return _PIDXProfileGuid; }
			set { _PIDXProfileGuid = value; }
		}

		public Guid CompanyPersonnelToShipToBillToGuid
		{
			get { return _CompanyPersonnelToShipToBillToGuid; }
			set { _CompanyPersonnelToShipToBillToGuid = value; }
		}

		public string SellerID
		{
			get { return _SellerID; }
			set { SetString("Seller ID", 3, value, ref _SellerID); }
		}

		public string ShipperID
		{
			get { return _ShipperID; }
			set { SetString("Shipper ID", 3, value, ref _ShipperID); }
		}

		public string ConsigneeNumber
		{
			get { return _ConsigneeNumber; }
			set { SetString("Consignee Number", 14, value, ref _ConsigneeNumber); }
		}


		public bool DenialOverride
		{
			get { return _DenialOverride; }
			set { _DenialOverride = value; }
		}

		public bool UnavailableOverride
		{
			get { return _UnavailableOverride; }
			set { _UnavailableOverride = value; }
		}

		public string ShipToID
		{
			get { return _ShipToID; }
			set { _ShipToID = value; }
		}

		public string ShipToName
		{
			get { return _ShipToName; }
			set { _ShipToName = value; }
		}

		public string ShipToAddress
		{
			get { return _ShipToAddress; }
			set { _ShipToAddress = value; }
		}

		public string ShipToCity
		{
			get { return _ShipToCity; }
			set { _ShipToCity = value; }
		}

		public string ShipToState
		{
			get { return _ShipToState; }
			set { _ShipToState = value; }
		}

		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.PIDX_PROFILE_COMPANY_MAP; }
			set { }
		}

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public string ShipToToolTip
		{
			get
			{
            string ToolTip = ShipToName != "" ? ShipToName : ShipToID;
            if (ShipToAddress != "")
				{
					ToolTip += ", " + ShipToAddress;
				}

				if (ShipToCity != "")
				{
					ToolTip += ", " + ShipToCity;
				}

				if (ShipToState != "")
				{
					ToolTip += ", " + ShipToState;
				}

				return ToolTip;
			}
		}
		#endregion

		#region SqlCommand with Parameters

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblPIDXProfileToCompany (" +
				"SiteGuid," +
				"PIDXProfileGuid," +
				"CompanyPersonnelToShipToBillToGuid," +
				"SellerID," +
				"ShipperID," +
				"ConsigneeNumber," +
				"DenialOverride," +
				"UnavailableOverride," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"PIDXProfileToCompanyGuid" +
				") VALUES (" +
				"@SiteGuid," +
				"@PIDXProfileGuid," +
				"@CompanyPersonnelToShipToBillToGuid," +
				"@SellerID," +
				"@ShipperID," +
				"@ConsigneeNumber," +
				"@DenialOverride," +
				"@UnavailableOverride," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@PIDXProfileToCompanyGuid)";

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@PIDXProfileGuid" , SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CompanyPersonnelToShipToBillToGuid" , SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@SellerID" , SqlDbType.NVarChar, 3);
				cmd.Parameters.Add("@ShipperID" , SqlDbType.NVarChar, 3);
				cmd.Parameters.Add("@ConsigneeNumber" , SqlDbType.NVarChar, 14);
				cmd.Parameters.Add("@DenialOverride" , SqlDbType.Bit);
				cmd.Parameters.Add("@UnavailableOverride" , SqlDbType.Bit);
				cmd.Parameters.Add("@CreatedDate" , SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@CreatedBy" , SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@UpdatedDate" , SqlDbType.DateTimeOffset);
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@PIDXProfileToCompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@SiteGuid"].Value = _SiteGuid;
				cmd.Parameters["@PIDXProfileGuid" ].Value = _PIDXProfileGuid;
				cmd.Parameters["@CompanyPersonnelToShipToBillToGuid" ].Value = _CompanyPersonnelToShipToBillToGuid;
				cmd.Parameters["@SellerID" ].Value = _SellerID;
				cmd.Parameters["@ShipperID" ].Value = _ShipperID;
				cmd.Parameters["@ConsigneeNumber" ].Value = _ConsigneeNumber;
				cmd.Parameters["@DenialOverride" ].Value = _DenialOverride ? 1 : 0;
				cmd.Parameters["@UnavailableOverride" ].Value = _UnavailableOverride ? 1 : 0;
				cmd.Parameters["@CreatedDate" ].Value = _CreatedDate;
				cmd.Parameters["@CreatedBy" ].Value = _CreatedBy;
				cmd.Parameters["@UpdatedDate" ].Value = _UpdatedDate;
				cmd.Parameters["@UpdatedBy"].Value = _UpdatedBy;
				cmd.Parameters["@PIDXProfileToCompanyGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE map.tblPIDXProfileToCompany SET " +
				" SellerID = @SellerID, " +
				" ShipperID = @ShipperID, " +
				" ConsigneeNumber = @ConsigneeNumber, " +
				" DenialOverride = @DenialOverride, " +
				" UnavailableOverride = @UnavailableOverride, " +
				" UpdatedDate = @UpdatedDate, " +
				" UpdatedBy = @UpdatedBy "  +
				" WHERE SiteGuid = @SiteGuid " +
				" AND PIDXProfileGuid = @PIDXProfileGuid " +
				" AND CompanyPersonnelToShipToBillToGuid = @CompanyPersonnelToShipToBillToGuid";

			cmd.Parameters.Add("@SellerID", SqlDbType.NVarChar, 3);
			cmd.Parameters.Add("@ShipperID", SqlDbType.NVarChar, 3);
			cmd.Parameters.Add("@ConsigneeNumber", SqlDbType.NVarChar, 14);
			cmd.Parameters.Add("@DenialOverride", SqlDbType.Bit);
			cmd.Parameters.Add("@UnavailableOverride", SqlDbType.Bit);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyPersonnelToShipToBillToGuid", SqlDbType.UniqueIdentifier);
		
			cmd.Parameters["@SellerID"].Value = _SellerID;
			cmd.Parameters["@ShipperID"].Value = _ShipperID;
			cmd.Parameters["@ConsigneeNumber"].Value = _ConsigneeNumber;
			cmd.Parameters["@DenialOverride"].Value = _DenialOverride ? 1 : 0;
			cmd.Parameters["@UnavailableOverride"].Value = _UnavailableOverride ? 1 : 0;
			cmd.Parameters["@UpdatedDate"].Value = _UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = _UpdatedBy;
			cmd.Parameters["@SiteGuid"].Value = _SiteGuid;
			cmd.Parameters["@PIDXProfileGuid"].Value = _PIDXProfileGuid;
			cmd.Parameters["@CompanyPersonnelToShipToBillToGuid"].Value = _CompanyPersonnelToShipToBillToGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblPIDXProfileToCompany " +
				" WHERE SiteGuid = @SiteGuid " + 
				" AND PIDXProfileGuid = @PIDXProfileGuid " +
				" AND CompanyPersonnelToShipToBillToGuid = @CompanyPersonnelToShipToBillToGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyPersonnelToShipToBillToGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = _SiteGuid;
			cmd.Parameters["@PIDXProfileGuid"].Value = _PIDXProfileGuid;
			cmd.Parameters["@CompanyPersonnelToShipToBillToGuid"].Value = _CompanyPersonnelToShipToBillToGuid;
		}

		public void EnumerateBySiteAndCompanyPersonnelToShipToBillToGuidSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT PPTC .*, CPTSTBT.ID, C.ID AS ShipToID, C.Name AS ShipToName, C.Address1 AS ShipToAddress, C.City AS ShipToCity, C.State AS ShipToState"
				+ " FROM map.tblPIDXProfileToCompany PPTC"
				+ " INNER JOIN map.tblCompanyPersonnelToShipToBillTo CPTSTBT ON CPTSTBT.CompanyPersonnelToShipToBillToGuid = PPTC.CompanyPersonnelToShipToBillToGuid "
				+ " INNER JOIN map.tblCompanyShipToToBillTo CSTTBT ON CSTTBT.CompanyShipToToBillToGuid = CPTSTBT.CompanyShipToToBillToGuid "
				+ " INNER JOIN [erv].[udf_GetCompanyRecordVersions](@SiteGuid) CRV ON CRV.MasterRecordGuid =  CSTTBT.CompanyGuid"
				+ " INNER JOIN tblCompanies C ON C.CompanyGuid = CRV.CompanyGuid"
				+ " WHERE PPTC.SiteGuid = @SiteGuid"
                + " AND CPTSTBT.CompanyPersonnelToShipToBillToGuid = @CompanyPersonnelToShipToBillToGuid";
			
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyPersonnelToShipToBillToGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = _SiteGuid;
			cmd.Parameters["@CompanyPersonnelToShipToBillToGuid"].Value = _CompanyPersonnelToShipToBillToGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT PPTC.*, CPTSTBT.ID, C.ID AS ShipToID, C.Name AS ShipToName, C.Address1 AS ShipToAddress, C.City AS ShipToCity, C.State AS ShipToState"
				+ " FROM map.tblPIDXProfileToCompany PPTC"
				+ " INNER JOIN map.tblCompanyPersonnelToShipToBillTo CPTSTBT ON CPTSTBT.CompanyPersonnelToShipToBillToGuid = PPTC.CompanyPersonnelToShipToBillToGuid "
				+ " INNER JOIN map.tblCompanyShipToToBillTo CSTTBT ON CSTTBT.CompanyShipToToBillToGuid = CPTSTBT.CompanyShipToToBillToGuid "
				+ " INNER JOIN [erv].[udf_GetCompanyRecordVersions](@SiteGuid) CRV ON CRV.MasterRecordGuid =  CSTTBT.CompanyGuid"
				+ " INNER JOIN tblCompanies C ON C.CompanyGuid = CRV.CompanyGuid"
				+ " WHERE PPTC.SiteGuid = @SiteGuid "
				+ " AND PPTC.PIDXProfileGuid = @PIDXProfileGuid"
				+ " AND CPTSTBT.CompanyPersonnelToShipToBillToGuid = @CompanyPersonnelToShipToBillToGuid";
			
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyPersonnelToShipToBillToGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = _SiteGuid;
			cmd.Parameters["@PIDXProfileGuid"].Value = _PIDXProfileGuid;
			cmd.Parameters["@CompanyPersonnelToShipToBillToGuid"].Value = _CompanyPersonnelToShipToBillToGuid;
		}

		public void EnumerateByPIDXProfileGuidSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT PPTC.*, CPTSTBT.ID, C.ID AS ShipToID, C.Name AS ShipToName, C.Address1 AS ShipToAddress, C.City AS ShipToCity, C.State AS ShipToState"
				+ " FROM map.tblPIDXProfileToCompany PPTC"
				+ " INNER JOIN map.tblCompanyPersonnelToShipToBillTo CPTSTBT ON CPTSTBT.CompanyPersonnelToShipToBillToGuid = PPTC.CompanyPersonnelToShipToBillToGuid "
				+ " INNER JOIN map.tblCompanyShipToToBillTo CSTTBT ON CSTTBT.CompanyShipToToBillToGuid = CPTSTBT.CompanyShipToToBillToGuid "
				+ " INNER JOIN [erv].[udf_GetCompanyRecordVersions](@SiteGuid) CRV ON CRV.MasterRecordGuid =  CSTTBT.CompanyGuid"
				+ " INNER JOIN tblCompanies C ON C.CompanyGuid = CRV.CompanyGuid"
				+ " WHERE PPTC.PIDXProfileGuid = @PIDXProfileGuid "
				+ " ORDER BY ShipToID";
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PIDXProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@PIDXProfileGuid"].Value = _PIDXProfileGuid;
			cmd.Parameters["@SiteGuid"].Value = _SiteGuid;
		}
		#endregion

		#region Public methods
		public override void Reset()
		{
			base.Reset();

			_PIDXProfileGuid = Guid.Empty;
			_CompanyPersonnelToShipToBillToGuid = Guid.Empty;
			_SellerID = "";
			_ShipperID = "";
			_ConsigneeNumber = "";
			_DenialOverride = false;
			_UnavailableOverride = false;
			ShipToID = "";
			ShipToName = "";
			ShipToAddress = "";
			ShipToCity = "";
			ShipToState = "";
		}

		public override void Load(object o)
		{
			this.Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				IdentityGuid = DataObject.getValue(Row["PIDXProfileToCompanyGuid"], Guid.Empty);
				_SiteGuid = DataObject.getValue(Row["SiteGuid"], Guid.Empty);
				_PIDXProfileGuid = DataObject.getValue(Row["PIDXProfileGuid"], Guid.Empty);
				_CompanyPersonnelToShipToBillToGuid = DataObject.getValue(Row["CompanyPersonnelToShipToBillToGuid"], Guid.Empty);
				_SellerID = DataObject.getValue(Row["SellerID"], "");
				_ShipperID = DataObject.getValue(Row["ShipperID"], "");
				_ConsigneeNumber = DataObject.getValue(Row["ConsigneeNumber"], "");
				_DenialOverride = DataObject.getValue(Row["DenialOverride"], false);
				_UnavailableOverride = DataObject.getValue(Row["UnavailableOverride"], false);
				_CreatedDate = DataObject.getValue(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue(Row["UpdatedBy"], ADMIN);
				_ID = DataObject.getValue(Row["ID"], "");
				_ShipToID = DataObject.getValue(Row["ShipToID"], "");
				_ShipToName = DataObject.getValue(Row["ShipToName"], "");
				_ShipToAddress = DataObject.getValue(Row["ShipToAddress"], "");
				_ShipToCity = DataObject.getValue(Row["ShipToCity"], "");
				_ShipToState = DataObject.getValue(Row["ShipToState"], "");
			}
			else
			{
				throw new Exception("Load Error - Invalid Object Type : " + o.GetType().ToString());
			}

		}

		#endregion
	}
	#endregion
}
