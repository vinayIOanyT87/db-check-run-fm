using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class UserDataListValueCollectionClass : List<UserDataListValueClass> { }

   [Serializable]
   [DataContract]
	public class UserDataListValueClass : BaseDataObject
	{
		[DataMember]
		public Guid UserDataFieldGuid { get; set; }

		[DataMember]
		public ENTITY_TYPE UserDataFieldEntityType { get; set; }

		public static string GetTableName(ENTITY_TYPE userDataEntityType)
		{
			switch (userDataEntityType)
			{
				case ENTITY_TYPE.COMPANY:
					return "tblUserDataListValueCompany";
				case ENTITY_TYPE.EQUIPMENT:
					return "tblUserDataListValueEquipment";
				case ENTITY_TYPE.FUEL_CARD:
					return "tblUserDataListValueFuelCard";
				case ENTITY_TYPE.PERSONNEL:
					return "tblUserDataListValuePersonnel";
				case ENTITY_TYPE.PRODUCT:
					return "tblUserDataListValueProduct";
				case ENTITY_TYPE.SITE:
					return "tblUserDataListValueSite";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
					return "tblUserDataListValueTransactionAlias";
				case ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM:
					return "tblUserDataListValueTransactionAliasLineItem";
                case ENTITY_TYPE.USER:
                    return "tblUserDataListValueUser";
                case ENTITY_TYPE.IATA_CODE:
                    return "tblUserDataListValueIATA";
                default:
					return "Unknown";
			}
		}

		public static string GetForeignKeyName(ENTITY_TYPE userDataEntityType)
		{
			switch (userDataEntityType)
			{
				case ENTITY_TYPE.COMPANY:
					return "UserDataFieldCompanyGuid";
				case ENTITY_TYPE.EQUIPMENT:
					return "UserDataFieldEquipmentGuid";
				case ENTITY_TYPE.FUEL_CARD:
					return "UserDataFieldFuelCardGuid";
				case ENTITY_TYPE.PERSONNEL:
					return "UserDataFieldPersonnelGuid";
				case ENTITY_TYPE.PRODUCT:
					return "UserDataFieldProductGuid";
				case ENTITY_TYPE.SITE:
					return "UserDataFieldSiteGuid";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
					return "UserDataFieldTransactionAliasGuid";
				case ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM:
					return "UserDataFieldTransactionAliasLineItemGuid";
                case ENTITY_TYPE.USER:
                    return "UserDataFieldUserGuid";
                case ENTITY_TYPE.IATA_CODE:
                    return "UserDataFieldIATAGuid";
				default:
					return "Unknown";
			}
		}

		public static string GetPrimaryKeyName(ENTITY_TYPE userDataEntityType)
		{
			switch (userDataEntityType)
			{
				case ENTITY_TYPE.COMPANY:
					return "UserDataListValueCompanyGuid";
				case ENTITY_TYPE.EQUIPMENT:
					return "UserDataListValueEquipmentGuid";
				case ENTITY_TYPE.FUEL_CARD:
					return "UserDataListValueFuelCardGuid";
				case ENTITY_TYPE.PERSONNEL:
					return "UserDataListValuePersonnelGuid";
				case ENTITY_TYPE.PRODUCT:
					return "UserDataListValueProductGuid";
				case ENTITY_TYPE.SITE:
					return "UserDataListValueSiteGuid";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
					return "UserDataListValueTransactionAliasGuid";
				case ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM:
					return "UserDataListValueTransactionAliasLineItemGuid";
                case ENTITY_TYPE.USER:
			        return "UserDataListValueUserGuid";
                case ENTITY_TYPE.IATA_CODE:
			        return "UserDataListValueIataGuid";
                default:
					return "Unknown";
			}
		}


		public override void Reset()
		{
			base.Reset();

			UserDataFieldGuid = Guid.Empty;
			UserDataFieldEntityType = ENTITY_TYPE.NONE;
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("Set");
			}

			ENTITY_TYPE userDataFieldEntityType = UserDataFieldEntityType;
 
			Reset();

			UserDataFieldEntityType = userDataFieldEntityType;

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return;

			DataRow Row = Table.Rows[0];

			
			UserDataFieldGuid = DataObject.getValue<Guid>(Row[GetForeignKeyName(UserDataFieldEntityType)], Guid.Empty);
			IdentityGuid = DataObject.getValue<Guid>(Row[GetPrimaryKeyName(UserDataFieldEntityType)], Guid.Empty);
			ID = DataObject.getValue<string>(Row["Value"], "");
			CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO " + GetTableName(UserDataFieldEntityType) + 
				"(" + GetForeignKeyName(UserDataFieldEntityType) + "," +
				"[Value]," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy" +
				") VALUES (" +
				"@UserDataFieldGuid," +
				"@ID," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy" +
				")";

			cmd.Parameters.AddWithValue("UserDataFieldGuid", UserDataFieldGuid);
			cmd.Parameters.AddWithValue("ID", ID);
			cmd.Parameters.AddWithValue("CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("UpdatedBy", UpdatedBy);

		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + GetTableName(UserDataFieldEntityType) + 
				" WHERE " + GetForeignKeyName(UserDataFieldEntityType) + " = @UserDataFieldGuid" +
				" AND [Value] = @ID";

			cmd.Parameters.AddWithValue("UserDataFieldGuid", UserDataFieldGuid);
			cmd.Parameters.AddWithValue("ID", ID);
		}

		public void EnumerateSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM " + GetTableName(UserDataFieldEntityType) + SQLUpdateLock(bInTransaction) +
				" WHERE " + GetForeignKeyName(UserDataFieldEntityType) + " = @UserDataFieldGuid" +
				" ORDER BY [Value]";

			cmd.Parameters.AddWithValue("UserDataFieldGuid", UserDataFieldGuid);
		}
	}
}
