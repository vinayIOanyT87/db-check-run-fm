using FMBusinessObjects.LogClient;
using FMBusinessObjects.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
    #region Public company roles
    // Modifications on enum COMPANY_ROLE may require update on table lookup.tblCompanyRole
    public enum COMPANY_ROLE
	{
		MANAGER = 0,
		OWNER = 1,
		SHIPPER = 2,
		CUSTOMER_BILLTO = 3,
		CUSTOMER_SHIPTO = 4,
		CARRIER = 5,
		SUPPLIER = 6,
		MAX_COMPANY_ROLE = 7,
		NO_COMPANY_ROLE = 8
	};

	// JS20100820 WI-14934
	public enum COMPANY_SUB_ROLE
	{
		NO_SUBROLE = 0,
		ADF = 1,
		OTHER = 2
	}
	#endregion

	/// <summary>
	/// Summary description for CompanyRoleMapCollectionClass.
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	public class CompanyRoleMapCollectionClass : List<CompanyRoleMapClass> { }

	#region Company Role Map Class
	/// <summary>
	/// Summary description for CompanyRoleMapClass.
	/// </summary>
	[Serializable()]
	[EntityImportExportWorksheetAttribute("COMPANY ROLES")]
	[DataContract]
	public class CompanyRoleMapClass : BaseDataObject
	{
		#region Private data members
		[DataMember]
		private Guid companyGuid;
		[DataMember]
		private COMPANY_ROLE role;
		[DataMember]
		private string companyID;
		[DataMember]
		private string companyName;
		[DataMember]
		private string companyAddress1;
		[DataMember]
		private string companyAddress2;
		[DataMember]
		private bool hasManagerRole;
		[DataMember]
		private bool hasOwnerRole;
		[DataMember]
		private bool hasShipToRole;
		[DataMember]
		private bool hasBillToRole;
		[DataMember]
		private bool hasCarrierRole;
		[DataMember]
		private bool hasSupplierRole;
		[DataMember]
		private bool hasShipperRole;
		[DataMember]
		private bool parentControlled;
		[DataMember]
		private bool recordVersioningOn;


		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Company Role Map Class
		/// </summary>
		public CompanyRoleMapClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets an indicator specifying
		/// if record versioning is turned on for companies.
		/// </summary>
		[XmlIgnore]
		public bool RecordVersioningOn
		{
			get { return recordVersioningOn; }
			set { recordVersioningOn = value; }
		}

		/// <summary>
		/// This property sets and gets an indicator specifying
		/// whether the record is parent controlled or not.  If record
		/// versioning is turned off then this is a meaningless attribute
		/// </summary>
		[XmlIgnore]
		public bool ParentControlled
		{
			get { return parentControlled; }
			set { parentControlled = value; }
		}

		/// <summary>
		/// This property sets and gets the Manager Role flag. It
		/// returns true if the Manager Role is set.
		/// </summary>
		[XmlIgnore]
		public bool HasManagerRole
		{
			get { return this.hasManagerRole; }
			set { this.hasManagerRole = value; }
		}

		/// <summary>
		/// This property sets and gets the Owner Role flag. It
		/// returns true if the Owner Role is set.
		/// </summary>
		[XmlIgnore]
		public bool HasOwnerRole
		{
			get { return this.hasOwnerRole; }
			set { this.hasOwnerRole = value; }
		}

		/// <summary>
		/// This property sets and gets the Ship-To Role flag. It
		/// returns true if the Ship-To Role is set.
		/// </summary>
		[XmlIgnore]
		public bool HasShipToRole
		{
			get { return this.hasShipToRole; }
			set { this.hasShipToRole = value; }
		}

		/// <summary>
		/// This property sets and gets the Bill-To Role flag. It
		/// returns true if the Bill-To Role is set.
		/// </summary>
		[XmlIgnore]
		public bool HasBillToRole
		{
			get { return this.hasBillToRole; }
			set { this.hasBillToRole = value; }
		}

		/// <summary>
		/// This property sets and gets the Carrier Role flag. It
		/// returns true if the Carrier Role is set.
		/// </summary>
		[XmlIgnore]
		public bool HasCarrierRole
		{
			get { return this.hasCarrierRole; }
			set { this.hasCarrierRole = value; }
		}

		/// <summary>
		/// This property sets and gets the Supplier Role flag. It
		/// returns true if the Supplier Role is set.
		/// </summary>
		[XmlIgnore]
		public bool HasSupplierRole
		{
			get { return this.hasSupplierRole; }
			set { this.hasSupplierRole = value; }
		}

		/// <summary>
		/// This property sets and gets the Shipper Role flag. It
		/// returns true if the Shipper Role is set.
		/// </summary>
		[XmlIgnore]
		public bool HasShipperRole
		{
			get { return this.hasShipperRole; }
			set { this.hasShipperRole = value; }
		}

		/// <summary>
		/// This property sets and gets the Company ID data member.
		/// </summary>
		public string CompanyID
		{
			get { return this.companyID; }
			set { this.companyID = value; }
		}

		/// <summary>
		/// This property sets and gets the Company Name data member.
		/// </summary>
		public string CompanyName
		{
			get { return this.companyName; }
			set { this.companyName = value; }
		}

		/// <summary>
		/// This property sets and gets the Company Address 1 data member.
		/// </summary>
		public string CompanyAddress1
		{
			get { return this.companyAddress1; }
			set { this.companyAddress1 = value; }
		}

		/// <summary>
		/// This property sets and gets the Company Address 2 data member.
		/// </summary>    
		public string CompanyAddress2
		{
			get { return this.companyAddress2; }
			set { this.companyAddress2 = value; }
		}

		/// <summary>
		/// This property sets and gets the Company Guid data member.
		/// </summary>
		[XmlIgnore]
		public Guid CompanyGuid
		{
			get { return this.companyGuid; }
			set { this.companyGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the Company Role data member.
		/// </summary>
		[XmlIgnore]
		public COMPANY_ROLE Role
		{
			get { return this.role; }
			set { this.role = value; }
		}

		[EntityImportExportAttribute("ROLEID*", 110, "ID")]
		public override string ID
		{
			get { return RoleID(this.role); }
			set
			{
				var tempValue = value.ToUpper();
				switch (tempValue)
				{
					case "MANAGER":
						this.role = COMPANY_ROLE.MANAGER;
						break;
					case "OWNER":
						this.role = COMPANY_ROLE.OWNER; ;
						break;
					case "SHIPPER":
						this.role = COMPANY_ROLE.SHIPPER; ;
						break;
					case "BILL TO":
						this.role = COMPANY_ROLE.CUSTOMER_BILLTO;
						break;
					case "SHIP TO":
						this.role = COMPANY_ROLE.CUSTOMER_SHIPTO;
						break;
					case "CARRIER":
						this.role = COMPANY_ROLE.CARRIER;
						break;
					case "SUPPLIER":
						this.role = COMPANY_ROLE.SUPPLIER;
						break;
					default:
						{
							this.role = COMPANY_ROLE.NO_COMPANY_ROLE;
							var errormsg = string.IsNullOrWhiteSpace(value) ? "Company role being mapped has no value"
								: "Unknown company role being mapped so no role selected. Invalid Value " + value;
							var logger = new Logger("CompanyRoleMapClass");
							logger.Error(errormsg);
							throw new CompanyRoleMapCollectionException(errormsg);
						}
				}			

					
		
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.COMPANY_ROLE; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.COMPANY; }
		}
		#endregion

		/// <summary>
		/// This method will join all the role into one object for a company role map
		/// that the Company and Site Guids match.  This is for the Company Role
		/// Assignment page.
		/// </summary>
		/// <param name="inRole"></param>
		public void JoinRoles(COMPANY_ROLE inRole)
		{
			switch (inRole)
			{
				case COMPANY_ROLE.MANAGER:
					this.hasManagerRole = true;
					break;
				case COMPANY_ROLE.OWNER:
					this.hasOwnerRole = true;
					break;
				case COMPANY_ROLE.SHIPPER:
					this.hasShipperRole = true;
					break;
				case COMPANY_ROLE.CUSTOMER_BILLTO:
					this.hasBillToRole = true;
					break;
				case COMPANY_ROLE.CUSTOMER_SHIPTO:
					this.hasShipToRole = true;
					break;
				case COMPANY_ROLE.CARRIER:
					this.hasCarrierRole = true;
					break;
				case COMPANY_ROLE.SUPPLIER:
					this.hasSupplierRole = true;
					break;
			}
		}

		static public string RoleID(COMPANY_ROLE inRole)
		{
			switch (inRole)
			{
				case COMPANY_ROLE.MANAGER:
					return "Manager";
				case COMPANY_ROLE.OWNER:
					return "Owner";
				case COMPANY_ROLE.SHIPPER:
					return "Shipper";
				case COMPANY_ROLE.CUSTOMER_BILLTO:
					return "Bill To";
				case COMPANY_ROLE.CUSTOMER_SHIPTO:
					return "Ship To";
				case COMPANY_ROLE.CARRIER:
					return "Carrier";
				case COMPANY_ROLE.SUPPLIER:
					return "Supplier";
				case COMPANY_ROLE.MAX_COMPANY_ROLE:
					return "{All}";
				case COMPANY_ROLE.NO_COMPANY_ROLE:
					return "{None}";
				default:
					return "Undefined";
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.companyGuid = Guid.Empty;
		}

		public override void Load(Object o)
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

				this.IdentityGuid = DataObject.getValue<Guid>(Row["CompanyToRoleGuid"], Guid.Empty);
				this.companyGuid = DataObject.getValue<Guid>(Row["CompanyGuid"], Guid.Empty);
				this.role = DataObject.getValue<COMPANY_ROLE>(Row["LookupCompanyRoleIndex"], COMPANY_ROLE.NO_COMPANY_ROLE);
				base.SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				base.CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				base.CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			}
			else
			{
				base.Load(o);
			}
		}

		/// <summary>
		/// This method will load data base on the EnumerateByCriterionSQL. Each row
		/// represents on Company Role Map object.
		/// </summary>
		/// <param name="row"></param>
		public void LoadByCriterionRow(DataRow row)
		{
			this.Reset();

			if (row != null)
			{
				this.companyGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
				this.companyName = DataObject.getValue<string>(row["CompanyName"], "");
				this.companyID = DataObject.getValue<string>(row["CompanyID"], "");
				this.companyAddress1 = DataObject.getValue<string>(row["CompanyAddress1"], "");
				this.companyAddress2 = DataObject.getValue<string>(row["CompanyAddress2"], "");
				this.role = DataObject.getValue<COMPANY_ROLE>(row["LookupCompanyRoleIndex"], COMPANY_ROLE.NO_COMPANY_ROLE);
				base.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
				base.SiteID = DataObject.getValue<string>(row["SiteID"], "");
				base.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				base.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
				int recVerOnInt = DataObject.getValue<int>(row["IsCompanyRecVerOn"],0);
				this.recordVersioningOn = (recVerOnInt == 0) ? false : true;
				string controlString = DataObject.getValue<string>(row["CompanyRolesFCM"], "ParentSpecific");
				switch (controlString)
				{
					case "ParentSpecific":
						this.parentControlled = true;
						break;
					case "GlobalSpecific":
						this.parentControlled = false;
						break;
                    case "VersionSpecific":
                        this.parentControlled = false;
                        break;
                    default:
						throw new Exception("Unknown CompanyRolesFCM Value " + controlString);
						//this.parentControlled = true;
						//break;
				}

				this.JoinRoles(this.role);
			}
		}

		public override void Store(Object o)
		{
			base.Store(o);
		}

		#region SQL strings

		/// <summary>
		/// This property will return the Insert SQL string.
		/// </summary>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblCompanyToRole " +
				 "(CompanyGuid, " +
				 "LookupCompanyRoleIndex, " +
				 "SiteGuid, " +
				 "CreatedDate, " +
				 "CreatedBy" +
				 ") VALUES (" +
				 "@CompanyGuid," +
				 "@Role," +
				 "@SiteGuid," +
				 "@CreatedDate," +
				 "@CreatedBy" +
				 ")";

			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Role", SqlDbType.Int);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@CompanyGuid"].Value = CompanyGuid;
			cmd.Parameters["@Role"].Value = (int)Role;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
		}

		public void PurgeSQL(SqlCommand cmd)
		{

			cmd.CommandText = "DELETE FROM map.tblCompanyToRole WHERE CompanyGuid = @CompanyGuid" +
				 " AND LookupCompanyRoleIndex = @Role AND SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Role", SqlDbType.Int);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@CompanyGuid"].Value = CompanyGuid;
			cmd.Parameters["@Role"].Value = (int)Role;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}



		public void SelectSQL(SqlCommand cmd)
		{

			cmd.CommandText = "SELECT * FROM map.tblCompanyToRole WHERE CompanyGuid = @CompanyGuid " +
				 " AND LookupCompanyRoleIndex = @Role AND SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Role", SqlDbType.Int);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@CompanyGuid"].Value = CompanyGuid;
			cmd.Parameters["@Role"].Value = (int)Role;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}

		public void EnumerateByCompanySQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM map.tblCompanyToRole WHERE CompanyGuid = @CompanyGuid " +
				" AND SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@CompanyGuid"].Value = CompanyGuid;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}

		/// <summary>
		/// This method return an SQL string that enumerates the company roles along with the company
		/// ID, company name, company addresses, and the site name based on a set of criterions.
		/// </summary>
		/// <param name="inSiteGuid"></param>
		/// <param name="findString"></param>
		/// <param name="inCompanyGuid"></param>
		/// <param name="role"></param>
		/// <param name="memberSites"></param>
		/// <returns></returns>
		public void EnumerateByCriterionSQL(SqlCommand cmd, Guid inSiteGuid,
																string inFindString,
																Guid inCompanyGuid,
																COMPANY_ROLE inRole,
																bool includeMemberSites,
																Guid inLoginSiteGuid)
		{
			if ((inRole == COMPANY_ROLE.NO_COMPANY_ROLE) && inCompanyGuid == Guid.Empty)
			{
				this.EnumerateByCriterionNoRoleSQL(cmd, inSiteGuid, inLoginSiteGuid, inFindString, includeMemberSites);
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			//cmd.CommandText = "Exec map.usp_GetCompanyMapRolesBySite @TargetSiteGuid, @IncludeChildSites, @CompanyMasterRecordGuid, @FindString, @RoleIndex";
			cmd.CommandText = "map.usp_GetCompanyMapRolesBySite";
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = inSiteGuid;
			cmd.Parameters.Add("@IncludeChildSites", SqlDbType.Bit);
			cmd.Parameters["@IncludeChildSites"].Value = (includeMemberSites == true) ? 1 : 0;
			cmd.Parameters.Add("@CompanyMasterRecordGuid", SqlDbType.UniqueIdentifier);
			if (inCompanyGuid != null && inCompanyGuid != Guid.Empty)
			{
				cmd.Parameters["@CompanyMasterRecordGuid"].Value = inCompanyGuid;
			}
			else
			{
				cmd.Parameters["@CompanyMasterRecordGuid"].Value = DBNull.Value;
			}
			cmd.Parameters.Add("@FindString", SqlDbType.NVarChar, 100);
			if (inFindString == null || inFindString.Length <= 0)
			{
				cmd.Parameters["@FindString"].Value = DBNull.Value;
			}
			else
			{
				string findStr = "%" + inFindString + "%";
				cmd.Parameters["@FindString"].Value = findStr;
			}
			cmd.Parameters.Add("@RoleIndex", SqlDbType.Int);
			if (inRole == COMPANY_ROLE.MAX_COMPANY_ROLE)
			{
				cmd.Parameters["@RoleIndex"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@RoleIndex"].Value = Convert.ToInt32(inRole);
			}
		}

		/// <summary>
		/// This method enumerates all the companies that do not have a role and returns
		/// a SQL command.
		/// </summary>
		/// <param name="inSiteGuid"></param>
		/// <param name="inLoginSiteGuid"></param>
		/// <param name="findString"></param>
		/// <returns></returns>
		private void EnumerateByCriterionNoRoleSQL(SqlCommand cmd, Guid inSiteGuid, Guid inLoginSiteGuid, string findString, bool includeMemberSites)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "map.usp_GetCompanyWithNoRolesBySite";
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = inSiteGuid;
			cmd.Parameters.Add("@IncludeChildSites", SqlDbType.Bit);
			cmd.Parameters["@IncludeChildSites"].Value = (includeMemberSites == true) ? 1 : 0;
			cmd.Parameters.Add("@FindString", SqlDbType.NVarChar, 100);
			if (findString == null || findString.Length <= 0)
			{
				cmd.Parameters["@FindString"].Value = DBNull.Value;
			}
			else
			{
				string findStr = "%" + findString + "%";
				cmd.Parameters["@FindString"].Value = findStr;
			}
		}

		#endregion
	}
	#endregion
}
