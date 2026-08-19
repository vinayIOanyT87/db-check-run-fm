// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataFieldClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the USER_DATA_TYPE type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
using System;
	using System.Collections;
using System.Data;
using System.Data.SqlClient;
	using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

	#region Public enumerations
	/// <summary>
	/// The user data type.
	/// </summary>
	public enum USER_DATA_TYPE : byte
	{
		TEXT = 0,
		LIST = 1
	}
	#endregion

	#region User Data Field Collection Class
	/// <summary>
	/// The user data field collection class.
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(UserDataFieldClass))]
	public class UserDataFieldCollectionClass : FieldCollectionClass
	{
		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="UserDataField">
		/// The user data field.
		/// </param>
		public void Add(UserDataFieldClass UserDataField)
		{
			List.Add(UserDataField);
		}

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="userDataField">
		/// The user data field.
		/// </param>
		public void Remove(UserDataFieldClass userDataField)
		{
			int index = 0;

			foreach ( UserDataFieldClass item in this.List )
			{
				if ( item.IdentityGuid == userDataField.IdentityGuid )
				{
					List.RemoveAt(index);
					return;
				}

				index++;
			}
		}

		/// <summary>
		/// The this.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="UserDataFieldClass"/>.
		/// </returns>
		public UserDataFieldClass this[int index]
		{
			get
			{
				return (UserDataFieldClass) List[index];
			}

			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// The items.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="UserDataFieldClass"/>.
		/// </returns>
		public UserDataFieldClass Items(int index)
		{
			return (UserDataFieldClass) List[index];
		}

		/// <summary>
		/// The find.
		/// </summary>
		/// <param name="dbName">
		/// The DB name.
		/// </param>
		/// <returns>
		/// The <see cref="UserDataFieldClass"/>.
		/// </returns>
		public UserDataFieldClass Find(string dbName)
		{
			foreach ( UserDataFieldClass userDataField in this.List )
			{
				if (dbName == userDataField.DbName)
				{
					return userDataField;
				}
			}

			return null;
		}
	}
	#endregion

	/// <summary>
	/// Summary description for UserDataFieldClass.
	/// </summary>
   [Serializable]
   [DataContract]
	[KnownType(typeof(UserDataListValueCollectionClass))]
	public class UserDataFieldClass : FieldClass
	{
		#region Data Members
		/// <summary>
		/// The user data entity type.
		/// </summary>
		[DataMember]
		protected ENTITY_TYPE userDataEntityType;

		/// <summary>
		/// The number.
		/// </summary>
		[DataMember]
		protected int number;

		/// <summary>
		/// The type.
		/// </summary>
		[DataMember]
		protected USER_DATA_TYPE _Type;

		/// <summary>
		/// The user data list value collection.
		/// </summary>
		[DataMember]
		public UserDataListValueCollectionClass UserDataListValueCollection;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="UserDataFieldClass"/> class.
		/// </summary>
		public UserDataFieldClass( )
		{
			this.Initialize();
		}

		#region Properties
		/// <summary>
		/// Gets or sets the ID.
		/// </summary>
		public override string ID
		{
			get
			{
				string newId = string.Empty;
				string entityTypeId = EntityToSiteMapClass.GetEntityTypeID(this.UserDataEntityType);

				if (entityTypeId.ToUpper().Equals("TRANSACTION ALIASES"))
				{
					newId = "TAUD";
		}

				if ( entityTypeId.ToUpper( ).Equals("TRANSACTION ALIAS LINE ITEM") )
		{
					newId = "TALUD";
		}

				return newId + (this.Number + 1).ToString(CultureInfo.InvariantCulture);
			}

			set { ; }
		}

		/// <summary>
		/// Gets or sets the user data entity type.
		/// </summary>
		public ENTITY_TYPE UserDataEntityType
		{
			get { return this.userDataEntityType; }
			set { this.userDataEntityType = value; }
		}

		/// <summary>
		/// Gets or sets the number.
		/// </summary>
		public int Number 
		{ 
			get { return this.number; }
			set { this.number = value; }
		}

		/// <summary>
		/// Gets or sets the user data type.
		/// </summary>
		public USER_DATA_TYPE UserDataType 
		{ 
			get { return _Type; } 
			set { _Type = value; } 
		}


		[DataMember]
		public new string DefaultValue
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.USER_DATA_FIELD; }
		}

		/// <summary>
		/// Gets the parent entity type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		/// <summary>
		/// Gets the select clause.
		/// </summary>
		private string SelectClause
		{
			get
			{
				return "SELECT " + GetTableName(this.UserDataEntityType) + ".*," +
							"(SELECT GroupID FROM tblGroups WHERE tblGroups.GroupGuid = " + GetTableName(this.UserDataEntityType) + ".UserGroupGuid) AS UserGroupID ";
			}
		}
		#endregion

		/// <summary>
		/// Get a list of the types of entities that user data can be associated with. 
		/// This is used to help functionality that queries all user data in the system, not just
		/// the user data in a particular table
		/// </summary>
		/// <returns>a list of the types of entities that user data can be associated with.</returns>
		public static ArrayList GetUserDataEntityTypes( )
		{
			var types = new ArrayList
				                  {
					                  ENTITY_TYPE.COMPANY,
					                  ENTITY_TYPE.EQUIPMENT,
					                  ENTITY_TYPE.FUEL_CARD,
					                  ENTITY_TYPE.PERSONNEL,
					                  ENTITY_TYPE.PRODUCT,
					                  ENTITY_TYPE.SITE,
					                  ENTITY_TYPE.TRANSACTION_ALIAS,
					                  ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM,
                                      ENTITY_TYPE.IATA_CODE,
									  ENTITY_TYPE.USER
				                  };

			return types;
		}

		/// <summary>
		/// The reset.
		/// </summary>
		public override void Reset( )
		{
			this.Initialize();
		}

		/// <summary>
		/// The type ID.
		/// </summary>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string TypeID(USER_DATA_TYPE type)
		{
			switch ( type )
			{
				case USER_DATA_TYPE.TEXT:
					return "Text";
				case USER_DATA_TYPE.LIST:
					return "List";
				default:
					return "Undefined";
			}
		}

		/// <summary>
		/// The get table name.
		/// </summary>
		/// <param name="userDataEntityType">
		/// The user data entity type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetTableName(ENTITY_TYPE userDataEntityType)
		{
			switch ( userDataEntityType )
			{
				case ENTITY_TYPE.COMPANY:
					return "tblUserDataFieldCompany";
				case ENTITY_TYPE.EQUIPMENT:
					return "tblUserDataFieldEquipment";
				case ENTITY_TYPE.FUEL_CARD:
					return "tblUserDataFieldFuelCard";
				case ENTITY_TYPE.PERSONNEL:
					return "tblUserDataFieldPersonnel";
				case ENTITY_TYPE.PRODUCT:
					return "tblUserDataFieldProduct";
				case ENTITY_TYPE.SITE:
					return "tblUserDataFieldSite";
				case ENTITY_TYPE.TRANSACTION_ALIAS:
					return "tblUserDataFieldTransactionAlias";
				case ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM:
					return "tblUserDataFieldTransactionAliasLineItem";
                case ENTITY_TYPE.IATA_CODE:
                    return "tblUserDataFieldIATA";
				case ENTITY_TYPE.USER:
					return "tblUserDataFieldUser";
				default:
					return "Unknown";
			}
		}

		/// <summary>
		/// The get primary key column name.
		/// </summary>
		/// <param name="userDataEntityType">
		/// The user data entity type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetPrimaryKeyColumnName(ENTITY_TYPE userDataEntityType)
		{
			switch ( userDataEntityType )
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
                case ENTITY_TYPE.IATA_CODE:
                    return "UserDataFieldIATAGuid";
				case ENTITY_TYPE.USER:
					return "UserDataFieldUserGuid";
				default:
					return "Unknown";
			}
		}

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="set">
		/// The set.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		public void Load(DataSet set)
		{
			if (set == null)
		{
				throw new ArgumentNullException("set");
			}

			ENTITY_TYPE tempUserDataEntityType = this.userDataEntityType;

			this.Reset( );

			DataTable table = set.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this.userDataEntityType = tempUserDataEntityType;

			this._IdentityGuid			= DataObject.getValue<Guid>(row[GetPrimaryKeyColumnName(this.UserDataEntityType)], Guid.Empty);
			this._SiteGuid				= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.TransactionAliasGuid	= DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
			this.number					= DataObject.getValue<byte>(row["Number"], 0);
			this._DisplayOrder			= DataObject.getValue<int>(row["DisplayOrder"], 0);
			this._DisplayName			= DataObject.getValue<string>(row["DisplayName"], string.Empty);
			this._Type					= (USER_DATA_TYPE) DataObject.getValue<int>(row["LookupUserDataTypeIndex"], 0);
			this._CreatedDate			= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy				= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this._UpdatedDate			= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy				= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			this.FieldRequired			= DataObject.getValue<bool>(row["Required"], false);
			this.UserGroupGuid			= DataObject.getValue<Guid>(row["UserGroupGuid"], Guid.Empty);
			this.UserGroupID			= DataObject.getValue<string>(row["UserGroupID"], string.Empty);
			this.DispatchField			= DataObject.getValue<bool>(row["DispatchField"], false);
			this.ClearOnNew				= DataObject.getValue<bool>(row["ClearOnNew"], false);
			this.ReadOnly				= DataObject.getValue<bool>(row["ReadOnly"], false);
			this.Visibility				= DataObject.getValue<TransactionFieldVisibility>(row["Visibility"], (int)TransactionFieldVisibility.Always);
			this.DefaultValue			= DataObject.getValue<string>(row["DefaultValue"], string.Empty);
			this._DbName				= string.Format("UserData{0}", this.number + 1);
		}

		/// <summary>
		/// The insert SQL.
		/// </summary>
		/// <param name="cmd">
		/// The Command.
		/// </param>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO " + GetTableName(this.UserDataEntityType) +
				"(SiteGuid," +
				"TransactionAliasGuid," +
				"Number," +
				"DisplayOrder," +
				"DisplayName," +
				"LookupUserDataTypeIndex," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"Required," +
				"UserGroupGuid," +
				"DispatchField," +
				"ClearOnNew," +
				"ReadOnly," +
				"Visibility," +
				"DefaultValue," +
				GetPrimaryKeyColumnName(this.UserDataEntityType) +
				") VALUES (" +
				"@SiteGuid," +
				"@TransactionAliasGuid," +
				"@Number," +
				"@DisplayOrder," +
				"@DisplayName," +
				"@LookupUserDataTypeIndex," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@Required," +
				"@UserGroupGuid," +
				"@DispatchField," +
				"@ClearOnNew," +
				"@ReadOnly," +
				"@Visibility," +
				"@DefaultValue," +
				"@" + GetPrimaryKeyColumnName(this.UserDataEntityType) +
				")";

			
			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier).Value = (this.TransactionAliasGuid == Guid.Empty) ? DBNull.Value : (object) TransactionAliasGuid;
			cmd.Parameters.AddWithValue("@Number", this.number);
			cmd.Parameters.AddWithValue("@DisplayOrder", this._DisplayOrder);
			cmd.Parameters.AddWithValue("@DisplayName", this._DisplayName);
			cmd.Parameters.AddWithValue("@LookupUserDataTypeIndex", (int) this._Type);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.Add("@Required", SqlDbType.Bit).Value = this.FieldRequired ? 1 : 0;
			cmd.Parameters.Add("@UserGroupGuid", SqlDbType.UniqueIdentifier).Value = (this.UserGroupGuid == Guid.Empty) ? DBNull.Value : (object) UserGroupGuid;
			cmd.Parameters.AddWithValue("@DispatchField", this.DispatchField);
			cmd.Parameters.AddWithValue("@ClearOnNew", this.ClearOnNew);
			cmd.Parameters.AddWithValue("@ReadOnly", this.ReadOnly);
			cmd.Parameters.AddWithValue("@Visibility", this.Visibility);
			if (this.DefaultValue == null) {
				cmd.Parameters.AddWithValue("@DefaultValue", DBNull.Value);
			} else {
				cmd.Parameters.AddWithValue("@DefaultValue", this.DefaultValue);
            }
			cmd.Parameters.AddWithValue("@" + GetPrimaryKeyColumnName(this.UserDataEntityType), this._IdentityGuid);
		}

		/// <summary>
		/// The update SQL.
		/// </summary>
		/// <param name="cmd">
		/// The Command.
		/// </param>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE " + GetTableName(this.UserDataEntityType) +
				" SET SiteGuid = @SiteGuid, " +
				"DisplayOrder = @DisplayOrder, " +
				"DisplayName = @DisplayName, " +
				"LookupUserDataTypeIndex = @LookupUserDataTypeIndex, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"Required = @Required, " +
				"UserGroupGuid = @UserGroupGuid, " +
				"DispatchField = @DispatchField, " +
				"ClearOnNew = @ClearOnNew, " +
				"ReadOnly = @ReadOnly," +
				"Visibility = @Visibility," +
				"DefaultValue = @DefaultValue" +
				" WHERE " + GetPrimaryKeyColumnName(this.UserDataEntityType) + " = @UserDataFieldGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@DisplayOrder", this._DisplayOrder);
			cmd.Parameters.AddWithValue("@DisplayName", this._DisplayName);
			cmd.Parameters.AddWithValue("@LookupUserDataTypeIndex", (int) this.UserDataType);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.Add("@Required", SqlDbType.Bit).Value = this.FieldRequired ? 1 : 0;
			cmd.Parameters.Add("@UserGroupGuid", SqlDbType.UniqueIdentifier).Value = (this.UserGroupGuid == Guid.Empty) ? DBNull.Value : (object) UserGroupGuid;
			cmd.Parameters.AddWithValue("@DispatchField", this.DispatchField);
			cmd.Parameters.AddWithValue("@ClearOnNew", this.ClearOnNew);
			cmd.Parameters.AddWithValue("@ReadOnly", this.ReadOnly);
			cmd.Parameters.AddWithValue("@Visibility", this.Visibility);
			if (this.DefaultValue == null)
			{
				cmd.Parameters.AddWithValue("@DefaultValue", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@DefaultValue", this.DefaultValue);
			}
			cmd.Parameters.AddWithValue("@UserDataFieldGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The purge SQL.
		/// </summary>
		/// <param name="cmd">
		/// The command.
		/// </param>
		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + GetTableName(this.UserDataEntityType) +
				" WHERE " + GetPrimaryKeyColumnName(this.UserDataEntityType) + " = @UserDataFieldGuid";

			cmd.Parameters.AddWithValue("@UserDataFieldGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The select SQL.
		/// </summary>
		/// <param name="cmd">
		/// The command.
		/// </param>
		/// <param name="bInTransaction">
		/// The transaction.
		/// </param>
		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
					" FROM " + GetTableName(this.UserDataEntityType) + " " + SQLUpdateLock(bInTransaction) + " WHERE " + GetPrimaryKeyColumnName(this.UserDataEntityType) + " = @UserDataFieldGuid";

			cmd.Parameters.AddWithValue("@UserDataFieldGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The select by idsql.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="bInTransaction">
		/// The b in transaction.
		/// </param>
		public void SelectByIDSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
					" FROM " + GetTableName(this.UserDataEntityType) + " " + SQLUpdateLock(bInTransaction) +
					" WHERE ( SiteGuid = @SiteGuid " +
						" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityUserDataToSite" +
						" WHERE MapToSiteGuid = @SiteGuid )) " +
					" AND Number = @Number" +
					" AND DispatchField = @DispatchField";

			if ( this.TransactionAliasGuid == Guid.Empty )
			{
				cmd.CommandText += " AND TransactionAliasGuid IS NULL";
			}
			else
			{
                cmd.CommandText += " AND TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @TransactionAliasGuid, @SiteGuid) ";
			}

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);

			if ( this.TransactionAliasGuid != Guid.Empty )
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.TransactionAliasGuid);
			}

			cmd.Parameters.AddWithValue("@Number", this.number);
			cmd.Parameters.AddWithValue("@DispatchField", this.DispatchField);
		}

		/// <summary>
		/// The enumerate SQL.
		/// </summary>
		/// <param name="cmd">
		/// The command.
		/// </param>
		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause +
				" FROM " + GetTableName(this.UserDataEntityType) +
				" WHERE SiteGuid = @SiteGuid" +
				" AND TransactionAliasGuid IS NULL";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
		}

		/// <summary>
		/// The enumerate by entity type ID SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="byUser">
		/// The by user.
		/// </param>
		/// <param name="bInTransaction">
		/// The transaction.
		/// </param>
		public void EnumerateByEntityTypeIDSQL(SqlCommand cmd, SecurityClass security, bool byUser, bool bInTransaction)
		{
			string byUserWhereClause = string.Empty;

			if (byUser)
			{
				byUserWhereClause = " AND (UserGroupGuid IS NULL OR UserGroupGuid IN (SELECT GroupGuid FROM map.tblUserToGroup WHERE UserGuid = @UserGuid AND SiteGuid = @SiteGuid))";
			}

			cmd.CommandText = this.SelectClause + " FROM " + GetTableName(this.UserDataEntityType); 

			if (this.TransactionAliasGuid == Guid.Empty)
			{
				cmd.CommandText += " WHERE ( SiteGuid = @SiteGuid " +
					" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityUserDataToSite" +
					" WHERE MapToSiteGuid = @SiteGuid )) " +
				" AND DispatchField = @DispatchField" +
				" AND TransactionAliasGuid IS NULL";
			}
			else
			{
				cmd.CommandText += " WHERE TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @TransactionAliasGuid, @SiteGuid) " +
				" AND DispatchField = @DispatchField";
			}

			cmd.CommandText += byUserWhereClause + " ORDER BY Number";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@DispatchField", this.DispatchField);

			if ( this.TransactionAliasGuid != Guid.Empty )
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.TransactionAliasGuid);
			}

			if (byUser)
			{
				cmd.Parameters.AddWithValue("@UserGuid", security.UserGuid);
			}
		}

		/// <summary>
		/// This method will initialize the object.
		/// </summary>
		private void Initialize( )
		{
			base.Reset();
			this.userDataEntityType = ENTITY_TYPE.NONE;
			this.number = 0;
			this.UserDataListValueCollection = new UserDataListValueCollectionClass( );
		}
	}
}