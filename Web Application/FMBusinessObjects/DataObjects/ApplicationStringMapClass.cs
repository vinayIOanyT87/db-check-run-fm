using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class ApplicationStringMapCollectionClass : List<ApplicationStringMapClass> { }

   [DataContract]
   [Serializable]
   public class ApplicationStringMapClass : BaseDataObject
   {
	   [DataMember]
	   public Guid ApplicationStringGuid;
	   [DataMember]
	   public STRING_MAP_TYPE Type;
	   [DataMember]
	   public int Sequence;
	   [EntityImportExportAttribute("ID*", 125, "AssignedToID")]
	   [DataMember]
	   public Guid AssignedToGuid;
	   [DataMember]
	   public string AssignedToID;
	   [DataMember]
	   public string AssignedToName;
	   [DataMember]
	   public string AssignedToAddress;
	   [DataMember]
	   public string AssignedToCity;
	   [DataMember]
	   public string AssignedToState;
	   [DataMember]
	   public string AssignedToCode;
	   [DataMember]
	   public string AssignedToDescription;
	   [DataMember]
	   public ProductType AssignedToProductType;

	   [EntityImportExportAttribute("ID*", 105, "ID")]
	   override public string ID { get { return _ID; } set { _ID = value; } }

	   private string Select
	   {
		   get
		   {
			   string selectSQL = "SELECT " + GetMappingTableName(this.Type) + ".*," +
							   "AssignedApplicationString.ID AS ID,";

			   switch (this.Type)
			   {
				   case STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE:
				   case STRING_MAP_TYPE.PRODUCT_MESSAGE:
				   case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:
					   selectSQL += "tblProducts.ProductID AS AssignedToID," +
							   "tblProducts.ProductCode AS AssignedCode," +
							   "tblProducts.Description AS AssignedDescription," +
							   "tblProducts.LookupProductTypeIndex AS AssignedProductType ";
					   break;

				   case STRING_MAP_TYPE.ALARM_EVENT_CATEGORY:
				   case STRING_MAP_TYPE.EMAIL_ADDRESS:
				   case STRING_MAP_TYPE.ENTRY_MESSAGE:
				   case STRING_MAP_TYPE.EXIT_MESSAGE:
				   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE:
					   selectSQL += "tblApplicationString.ID AS AssignedToID ";
					   break;

				   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO:
				   case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:
					   selectSQL += "tblCompanies.ID AS AssignedToID," +
									   "tblCompanies.Name AS AssignedToName," +
									   "tblCompanies.Address1 AS AssignedToAddress," +
									   "tblCompanies.City AS AssignedToCity," +
									   "tblCompanies.State AS AssignedToState ";
                        break;

                    case STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE:
                        selectSQL += "tblAdditiveProfiles.ID AS AssignedToID ";
					   break;

				   case STRING_MAP_TYPE.POINT_CATEGORY:
					   selectSQL += "tblPoint.ID AS AssignedToID ";
					   break;

				}

				return selectSQL;
		   }
	   }

	   /// <summary>
	   /// Get the SQL to JOIN the application string map table to other tables
	   /// </summary>
	   private string Join
	   {
		   get
		   {
			   string joinSQL = " LEFT JOIN tblApplicationString AssignedApplicationString ON AssignedApplicationString.ApplicationStringGuid = " + GetMappingTableName(this.Type) + ".ApplicationStringGuid ";

			   switch (this.Type)
			   {
				   case STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE:
				   case STRING_MAP_TYPE.PRODUCT_MESSAGE:
				   case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:
					   joinSQL += " LEFT JOIN tblProducts ON tblProducts.ProductGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
					   break;

				   case STRING_MAP_TYPE.ALARM_EVENT_CATEGORY:
				   case STRING_MAP_TYPE.EMAIL_ADDRESS:
				   case STRING_MAP_TYPE.ENTRY_MESSAGE:
				   case STRING_MAP_TYPE.EXIT_MESSAGE:
				   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE:
					   joinSQL += " LEFT JOIN tblApplicationString ON tblApplicationString.ApplicationStringGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
					   break;

				   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO:
				   case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:
					   joinSQL += " LEFT JOIN tblCompanies ON tblCompanies.CompanyGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
					   break;

               case STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE:
                  joinSQL += " LEFT JOIN tblAdditiveProfiles ON tblAdditiveProfiles.AdditiveProfileGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
                  break;

				   case STRING_MAP_TYPE.POINT_CATEGORY:
					   joinSQL += " LEFT JOIN tblPoint ON tblPoint.PointGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
					   break;
				}

				return joinSQL;
		   }
	   }

	   public ApplicationStringMapClass()
	   {
		   Initialize();
	   }

	   public string ToolTip
	   {
		   get
		   {
			   string ToolTip = "";

			   if (Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPPER
				|| Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPTO)
			   {
				   if (string.IsNullOrEmpty(AssignedToName) == false)
					   ToolTip = AssignedToName;
				   else
					   ToolTip = AssignedToID;
				   if (string.IsNullOrEmpty(AssignedToAddress) == false)
					   ToolTip += ", " + AssignedToAddress;
				   if (string.IsNullOrEmpty(AssignedToCity) == false)
					   ToolTip += ", " + AssignedToCity;
				   if (string.IsNullOrEmpty(AssignedToState) == false)
					   ToolTip += ", " + AssignedToState;
			   }

			   else if (Type == STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE
				|| Type == STRING_MAP_TYPE.PRODUCT_MESSAGE
				|| Type == STRING_MAP_TYPE.FOOT_NOTE_PRODUCT)
			   {
				   if (string.IsNullOrEmpty(AssignedToCode) == false)
					   ToolTip = AssignedToCode;
				   if (string.IsNullOrEmpty(AssignedToDescription) == false)
					   ToolTip += ", " + AssignedToDescription;
			   }

			   return ToolTip;
		   }
	   }

	   private void Initialize()
	   {
		   ApplicationStringGuid = Guid.Empty;
		   Type = STRING_MAP_TYPE.MAX_STRING_MAP_TYPE;
		   Sequence = 0;
		   AssignedToID = "";
		   AssignedToName = "";
		   AssignedToAddress = "";
		   AssignedToCity = "";
		   AssignedToState = "";
		   AssignedToCode = "";
		   AssignedToDescription = "";
		   AssignedToProductType = ProductType.MaxProduct;
	   }

	   public override void Reset()
	   {
		   base.Reset();
		   Initialize();
	   }

	   public override void Load(Object o)
	   {
		   STRING_MAP_TYPE stringMapType = this.Type;

		   Reset();

		   if (typeof(DataSet).IsInstanceOfType(o))
		   {
			   DataSet Set = (DataSet)o;

			   DataTable Table = Set.Tables[0];
			   if (Table.Rows.Count == 0)
				   return;

			   DataRow Row = Table.Rows[0];

			   this.Type = stringMapType;
			   IdentityGuid = DataObject.getValue<Guid>(Row[GetIdentityColumnName(this.Type)], Guid.Empty);
			   AssignedToGuid = DataObject.getValue<Guid>(Row[GetAssignedToColumnName(this.Type)], Guid.Empty);
			   ApplicationStringGuid = DataObject.getValue<Guid>(Row["ApplicationStringGuid"], Guid.Empty);
			   Sequence = DataObject.getValue<int>(Row["Sequence"], 0);
			   CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			   CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			   UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], CreatedDate);
			   UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			   ID = DataObject.getValue<string>(Row["ID"], "");
			   AssignedToID = DataObject.getValue<string>(Row["AssignedToID"], "{All}");

			   if (Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPTO
			   || Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPPER)
			   {
				   AssignedToName = DataObject.getValue<string>(Row["AssignedToName"], "");
				   AssignedToAddress = DataObject.getValue<string>(Row["AssignedToAddress"], "");
				   AssignedToCity = DataObject.getValue<string>(Row["AssignedToCity"], "");
				   AssignedToState = DataObject.getValue<string>(Row["AssignedToState"], "");
			   }

			   else if (Type == STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE
				|| Type == STRING_MAP_TYPE.PRODUCT_MESSAGE
				|| Type == STRING_MAP_TYPE.FOOT_NOTE_PRODUCT)
			   {
				   AssignedToCode = DataObject.getValue<string>(Row["AssignedCode"], "");
				   AssignedToDescription = DataObject.getValue<string>(Row["AssignedDescription"], "");
				   AssignedToProductType = DataObject.getValue<ProductType>(Row["AssignedProductType"], ProductType.MaxProduct);
			   }
		   }

		   else if (typeof(ApplicationStringMapClass).IsInstanceOfType(o))
		   {
			   ApplicationStringMapClass ApplicationStringMap = (ApplicationStringMapClass)o;

			   IdentityGuid = ApplicationStringMap.IdentityGuid;
			   AssignedToGuid = ApplicationStringMap.AssignedToGuid;
			   ApplicationStringGuid = ApplicationStringMap.ApplicationStringGuid;
			   Type = ApplicationStringMap.Type;
			   Sequence = ApplicationStringMap.Sequence;
			   CreatedDate = ApplicationStringMap.CreatedDate;
			   CreatedBy = ApplicationStringMap.CreatedBy;
			   UpdatedDate = ApplicationStringMap.UpdatedDate;
			   UpdatedBy = ApplicationStringMap.UpdatedBy;
			   ID = ApplicationStringMap.ID;
			   AssignedToID = ApplicationStringMap.AssignedToID;
			   AssignedToAddress = ApplicationStringMap.AssignedToAddress;
			   AssignedToCity = ApplicationStringMap.AssignedToCity;
			   AssignedToState = ApplicationStringMap.AssignedToState;
			   AssignedToCode = ApplicationStringMap.AssignedToCode;
			   AssignedToDescription = ApplicationStringMap.AssignedToDescription;
			   AssignedToProductType = ApplicationStringMap.AssignedToProductType;
		   }

		   else if (typeof(XmlNode).IsInstanceOfType(o))
		   {
			   XmlNode Node = (XmlNode)o;

			   if (Node.Name == "ProductMessages")
				   Type = STRING_MAP_TYPE.PRODUCT_MESSAGE;
			   else if (Node.Name == "HazardousMaterialMessages")
				   Type = STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE;
			   else
				   throw new Exception("Invalid ApplicationStringMap Type");

			   ID = Node.Attributes["ID"].Value;
		   }

		   else
			   throw new Exception("Load Error - Invalid Object Type : " + o.GetType().ToString());
	   }

	   public override void Store(Object O)
	   {
		   if (O == null)
			   throw new ArgumentNullException("Object");

		   if (typeof(XmlNode).IsInstanceOfType(O))
		   {
			   XmlNode Node = (XmlNode)O;

			   XmlAttribute Attribute = Node.OwnerDocument.CreateAttribute("ID");
			   Attribute.Value = ID;
			   Node.Attributes.Append(Attribute);
		   }
		   else
			   throw new Exception("Store Error - Invalid Object Type : " + O.GetType().ToString());
	   }

	   public void InsertSQL(SqlCommand cmd)
	   {
		   cmd.CommandText = "INSERT INTO " + GetMappingTableName(this.Type) + " " +
			   "(" + GetIdentityColumnName(this.Type) + "," +
			   "ApplicationStringGuid," +
			   GetAssignedToColumnName(this.Type) + "," +
			   "Sequence," +
			   "CreatedDate," +
			   "CreatedBy," +
			   "UpdatedDate," +
			   "UpdatedBy" +
			   ") VALUES (" +
			   "@" + GetIdentityColumnName(this.Type) + "," +
			   "@ApplicationStringGuid," +
			   "@" + GetAssignedToColumnName(this.Type) + "," +
			   "@Sequence," +
			   "@CreatedDate," +
			   "@CreatedBy," +
			   "@UpdatedDate," +
			   "@UpdatedBy" +
			   ")";

		   cmd.Parameters.Add("@" + GetIdentityColumnName(this.Type), SqlDbType.UniqueIdentifier);
		   cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
		   cmd.Parameters.Add("@" + GetAssignedToColumnName(this.Type), SqlDbType.UniqueIdentifier);
		   cmd.Parameters.Add("@Sequence", SqlDbType.Int);
		   cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
		   cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
		   cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
		   cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

		   cmd.Parameters["@" + GetIdentityColumnName(this.Type)].Value = this._IdentityGuid;
		   cmd.Parameters["@ApplicationStringGuid"].Value = this.ApplicationStringGuid;

		   // Guid.Empty indicates that the application string is assigned to All 
		   if (this.AssignedToGuid == Guid.Empty)
		   {
			   cmd.Parameters["@" + GetAssignedToColumnName(this.Type)].Value = DBNull.Value;
		   }
		   else
		   {
			   cmd.Parameters["@" + GetAssignedToColumnName(this.Type)].Value = this.AssignedToGuid;
		   }

		   cmd.Parameters["@Sequence"].Value = this.Sequence;
		   cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
		   cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
		   cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
		   cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
	   }

	   public void UpdateSQL(SqlCommand cmd)
	   {
		   cmd.CommandText = "UPDATE " + GetMappingTableName(this.Type) +
			   " SET Sequence = @Sequence," +
			   "UpdatedDate = @UpdatedDate," +
			   "UpdatedBy = @UpdatedBy " +
			   "WHERE " + GetIdentityColumnName(this.Type) + " = @" + GetIdentityColumnName(this.Type);

		   cmd.Parameters.Add("@Sequence", SqlDbType.Int);
		   cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
		   cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
		   cmd.Parameters.Add("@" + GetIdentityColumnName(this.Type), SqlDbType.UniqueIdentifier);

		   cmd.Parameters["@Sequence"].Value = this.Sequence;
		   cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
		   cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
		   cmd.Parameters["@" + GetIdentityColumnName(this.Type)].Value = this._IdentityGuid;
	   }

	   public void PurgeSQL(SqlCommand cmd)
	   {
		   cmd.CommandText = "DELETE FROM " + GetMappingTableName(this.Type) + " WHERE " + GetIdentityColumnName(this.Type) + " = @" + GetIdentityColumnName(this.Type);

		   cmd.Parameters.Add("@" + GetIdentityColumnName(this.Type), SqlDbType.UniqueIdentifier);
		   cmd.Parameters["@" + GetIdentityColumnName(this.Type)].Value = this._IdentityGuid;
	   }

	   public void SelectSQL(SqlCommand cmd, bool bInTransaction)
	   {
		   cmd.CommandText = this.Select +
			   " FROM " + GetMappingTableName(this.Type) + SQLUpdateLock(bInTransaction) + this.Join +
			   " WHERE " + GetIdentityColumnName(this.Type) + " = @" + GetIdentityColumnName(this.Type);

		   cmd.Parameters.Add("@" + GetIdentityColumnName(this.Type), SqlDbType.UniqueIdentifier);
		   cmd.Parameters["@" + GetIdentityColumnName(this.Type)].Value = this._IdentityGuid;
	   }

	   public void EnumerateByAssignedToGuidAndTypeSQL(SqlCommand cmd)
	   {
		   cmd.CommandText = this.Select +
			   " FROM " + GetMappingTableName(this.Type) + this.Join + 
			   " WHERE ";

		   if (this.AssignedToGuid != Guid.Empty)
		   {
			   cmd.CommandText += GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " = @AssignedToGuid";

			   cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
			   cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
		   }
		   else
		   {
			   cmd.CommandText += GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " IS NULL";
		   }

		   cmd.CommandText += " ORDER BY Sequence";
	   }

	   public void EnumerateByApplicationStringGuidAndTypeSQL(SqlCommand cmd, bool bInTransaction)
	   {
		   cmd.CommandText = this.Select +
			   " FROM " + GetMappingTableName(this.Type) + SQLUpdateLock(bInTransaction) + this.Join +
			   " WHERE " + GetMappingTableName(this.Type) + "." + "ApplicationStringGuid = @ApplicationStringGuid ORDER BY Sequence";

		   cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
		   cmd.Parameters["@ApplicationStringGuid"].Value = this.ApplicationStringGuid;
	   }

	   public static string GetMappingTableName(STRING_MAP_TYPE stringType)
	   {
		   const string SCHEMA_PREFIX = "map.";

		   switch (stringType)
		   {
			   case STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE:
				   return SCHEMA_PREFIX + "tblApplicationStringToDotHazardousMessage";
			   case STRING_MAP_TYPE.PRODUCT_MESSAGE:
				   return SCHEMA_PREFIX + "tblApplicationStringToProductMessage";
			   case STRING_MAP_TYPE.ALARM_EVENT_CATEGORY:
				   return SCHEMA_PREFIX + "tblApplicationStringToAlarmEventCategory";
			   case STRING_MAP_TYPE.EMAIL_ADDRESS:
				   return SCHEMA_PREFIX + "tblApplicationStringToEmailAddress";
			   case STRING_MAP_TYPE.ENTRY_MESSAGE:
				   return SCHEMA_PREFIX + "tblApplicationStringToEntryMessage";
			   case STRING_MAP_TYPE.EXIT_MESSAGE:
				   return SCHEMA_PREFIX + "tblApplicationStringToExitMessage";
			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO:
				   return SCHEMA_PREFIX + "tblApplicationStringToFootNoteShipTo";
			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:
				   return SCHEMA_PREFIX + "tblApplicationStringToFootNoteShipper";
			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE:
				   return SCHEMA_PREFIX + "tblApplicationStringToFootNoteShipToState";
			   case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:
				   return SCHEMA_PREFIX + "tblApplicationStringToFootNoteProduct";
            case STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE:
                return SCHEMA_PREFIX + "tblApplicationStringToFootNoteAdditiveProfile";
			   case STRING_MAP_TYPE.POINT_CATEGORY:
				   return SCHEMA_PREFIX + "tblApplicationStringToPointCategory";

				case STRING_MAP_TYPE.MAX_STRING_MAP_TYPE:
			   default:
				   throw new Exception("Application string mapping table not found.");
		   }
	   }

	   public static string GetAssignedToColumnName(STRING_MAP_TYPE stringType)
	   {
		   switch (stringType)
		   {
			   case STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE:
			   case STRING_MAP_TYPE.PRODUCT_MESSAGE:
			   case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:
				   return "ProductGuid";

			   case STRING_MAP_TYPE.ENTRY_MESSAGE:
			   case STRING_MAP_TYPE.EXIT_MESSAGE:
				   return "ProductGroupApplicationStringGuid";

			   case STRING_MAP_TYPE.ALARM_EVENT_CATEGORY:
			   case STRING_MAP_TYPE.EMAIL_ADDRESS:
				   return "EmailGroupGuid";

			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO:
			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:
				   return "CompanyGuid";

			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE:
				   return "AssignedToApplicationStringGuid";

				case STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE:
					return "AdditiveProfileGuid";

			   case STRING_MAP_TYPE.POINT_CATEGORY:
				   return "PointGuid";


				case STRING_MAP_TYPE.MAX_STRING_MAP_TYPE:
			   default:
				   throw new Exception("Application string assigned to column name not found.");
			   }
	   }

	   public static string GetIdentityColumnName(STRING_MAP_TYPE stringMapType)
	   {
		   switch (stringMapType)
		   {
			   case STRING_MAP_TYPE.ALARM_EVENT_CATEGORY:
				   return "ApplicationStringToAlarmEventCategoryGuid";
			   case STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE:
				   return "ApplicationStringToDotHazardousMessageGuid";
               case STRING_MAP_TYPE.PRODUCT_MESSAGE:
                   return "ApplicationStringToProductMessageGuid";
			   case STRING_MAP_TYPE.EMAIL_ADDRESS:
				   return "ApplicationStringToEmailAddressGuid";
			   case STRING_MAP_TYPE.ENTRY_MESSAGE:
				   return "ApplicationStringToEntryMessageGuid";
			   case STRING_MAP_TYPE.EXIT_MESSAGE:
				   return "ApplicationStringToExitMessageGuid";
			   case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:
				   return "ApplicationStringToFootNoteProductGuid";
			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:
				   return "ApplicationStringToFootNoteShipperGuid";
			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO:
				   return "ApplicationStringToFootNoteShipToGuid";
			   case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE:
				   return "ApplicationStringToFootNoteShipToStateGuid";
            case STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE:
               return "ApplicationStringToFootNoteAdditiveProfileGuid";
			   case STRING_MAP_TYPE.POINT_CATEGORY:
				   return "ApplicationStringToPointCategoryGuid";
			   case STRING_MAP_TYPE.MAX_STRING_MAP_TYPE:
			   default:
				   throw new Exception("Unknown String Map Type");
		   }
	   }
   }

	public enum STRING_MAP_TYPE
	{
		DOT_HAZARDOUS_MESSAGE = 0,
		PRODUCT_MESSAGE = 1,
		ALARM_EVENT_CATEGORY = 6,
		EMAIL_ADDRESS = 7,
		ENTRY_MESSAGE = 9,
		EXIT_MESSAGE = 10,
		FOOT_NOTE_SHIPTO = 11,
		FOOT_NOTE_SHIPPER = 12,
		FOOT_NOTE_SHIPTO_STATE = 13,
		FOOT_NOTE_PRODUCT = 14,
      FOOT_NOTE_ADDITIVE_PROFILE = 15,
		POINT_CATEGORY = 16,
		MAX_STRING_MAP_TYPE = 17
	};
}
