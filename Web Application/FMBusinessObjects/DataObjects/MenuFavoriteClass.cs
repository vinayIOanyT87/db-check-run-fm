///***************************************************************************
/// Module Name:	MenuFavoriteClass
/// Author:			Andy Hush
/// Copyright (c) Varec, Inc. All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Collection of MenuFavoriteClass objects
	/// </summary>
   [Serializable]
	[CollectionDataContract]
	public class MenuFavoriteCollectionClass : List<MenuFavoriteClass> { }

	/// <summary>
	/// A class that encapsulates a record from tblMenuFavorites. This
	/// object represents a user's saved menu item that will appear on
	/// the menu either in the Favorites menu or in the Quick Links bar.
	/// </summary>
   [Serializable]
	[DataContract]
	[KnownType(typeof(MenuFavoriteCollectionClass))]
	public class MenuFavoriteClass : BaseDataObject
	{
		#region Public Properties

		/// <summary>
		/// PK of tblUsers
		/// </summary>
		[DataMember]
		public Guid UserGuid { get; set; }

		/// <summary>
		/// Whether this is a Quick Link, or a Favorite
		/// </summary>
		[DataMember]
		public bool IsQuickLink { get; set; }

		/// <summary>
		/// Custom name that the user has configured for the favorite
		/// </summary>
		[DataMember]
		public string CustomName { get; set; }

		/// <summary>
		/// Order to display in the Favorites menu or Quick Links bar
		/// </summary>
		[DataMember]
		public int DisplayOrder { get; set; }

		/// <summary>
		/// The identifier for what menu item it is
		/// </summary>
		[DataMember]
		public FMMenuItemType MenuItemType { get; set; }

		/// <summary>
		/// Guid to uniquely identify the menu item if it is a
		/// dynamically created menu item (e.g., Add Transaction)
		/// </summary>
		[DataMember]
		public Guid DynamicMenuItemGuid { get; set; }

		/// <summary>
		/// Override of BaseDataObject.EntityType
		/// </summary>
		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		/// <summary>
		/// Override of BaseDataObject.ParentEntityType
		/// </summary>
		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Clear out the data object by assigning default values
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			UserGuid = Guid.Empty;
			IsQuickLink = false;
			CustomName = "";
			DisplayOrder = 0;
			MenuItemType = FMMenuItemType.NONE;
			DynamicMenuItemGuid = Guid.Empty;
		}

		/// <summary>
		/// Load the object from a DataRow, DataSet, MenuFavoriteClass,
		/// or XmlNode
		/// </summary>
		/// <param name="o">Object to load from</param>
		public override void Load(object o)
		{
			Reset();

			DataRow Row = null;

			if (typeof(DataRow).IsInstanceOfType(o))
			{
				Row = (DataRow)o;
			}

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;

				DataTable Table = Set.Tables[0];
				if (Table.Rows.Count == 0)
					return;

				Row = Table.Rows[0];
			}

			if (Row != null)
			{
				_IdentityGuid = DataObject.getValue<Guid>(Row["MenuFavoriteGuid"], Guid.Empty);
				UserGuid = DataObject.getValue<Guid>(Row["UserGuid"], Guid.Empty);
				IsQuickLink = DataObject.getValue<bool>(Row["IsQuickLink"], false);
				CustomName = DataObject.getValue<string>(Row["CustomName"], ""); ;
				DisplayOrder = DataObject.getValue<int>(Row["DisplayOrder"], 0);
				MenuItemType = DataObject.getValue<FMMenuItemType>(Row["MenuItemType"], FMMenuItemType.NONE);
				DynamicMenuItemGuid = DataObject.getValue<Guid>(Row["DynamicMenuItemGuid"], Guid.Empty); ;
			}
			else if (typeof(MenuFavoriteClass).IsInstanceOfType(o))
			{
				MenuFavoriteClass menuFav = (MenuFavoriteClass)o;
				this._IdentityGuid = menuFav.IdentityGuid;
				this.UserGuid = menuFav.UserGuid;
				this.IsQuickLink = menuFav.IsQuickLink;
				this.CustomName = menuFav.CustomName;
				this.DisplayOrder = menuFav.DisplayOrder;
				this.MenuItemType = menuFav.MenuItemType;
				this.DynamicMenuItemGuid = menuFav.DynamicMenuItemGuid;
			}
			else
			{
				base.Load(o);
			}
		}

		/// <summary>
		/// Provide SQL to perform a SELECT
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		/// <param name="bInTransaction">Whether to use locking</param>
		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblMenuFavorites " + SQLUpdateLock(bInTransaction) +
				" WHERE MenuFavoriteGuid = @MenuFavoriteGuid";

			cmd.Parameters.AddWithValue("@MenuFavoriteGuid", _IdentityGuid);
		}

		/// <summary>
		/// Provide SQL to enumerate by tblUsers PK
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		/// <param name="bInTransaction">Whether to use locking</param>
		public void EnumerateByUserSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblMenuFavorites " + SQLUpdateLock(bInTransaction) +
				" WHERE UserGuid = @UserGuid " +
				" ORDER BY DisplayOrder";

			cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
		}

		/// <summary>
		/// Provide SQL to enumerate by tblUsers PK and whether Quick Link or not
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		/// <param name="bInTransaction">Whether to use locking</param>
		public void EnumerateByUserAndIsQuickLinkSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblMenuFavorites " + SQLUpdateLock(bInTransaction) +
				" WHERE UserGuid = @UserGuid " +
				" AND IsQuickLink = @IsQuickLink " +
				" ORDER BY DisplayOrder";

			cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
			cmd.Parameters.AddWithValue("@IsQuickLink", IsQuickLink);
		}

		/// <summary>
		/// Provide SQL to insert a new record
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblMenuFavorites " +
				"(UserGuid" +
				",IsQuickLink" +
				",CustomName" +
				",DisplayOrder" +
				",MenuItemType" +
				",DynamicMenuItemGuid" +
				",CreatedDate" +
				",CreatedBy" +
				",UpdatedDate" +
				",UpdatedBy" +
				",MenuFavoriteGuid" +
				") VALUES (" +
				"@UserGuid" +
				",@IsQuickLink" +
				",@CustomName" +
				",@DisplayOrder" +
				",@MenuItemType" +
				",@DynamicMenuItemGuid" +
				",@CreatedDate" +
				",@CreatedBy" +
				",@UpdatedDate" +
				",@UpdatedBy" +
				",@MenuFavoriteGuid)";

			cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
			cmd.Parameters.AddWithValue("@IsQuickLink", IsQuickLink);
			cmd.Parameters.Add("@CustomName", SqlDbType.NVarChar).Value = 
				(CustomName == null || CustomName == "") ? (object)DBNull.Value : (object)CustomName;
			cmd.Parameters.AddWithValue("@DisplayOrder", DisplayOrder);
			cmd.Parameters.AddWithValue("@MenuItemType", MenuItemType);
			cmd.Parameters.Add("@DynamicMenuItemGuid", SqlDbType.UniqueIdentifier).Value = 
				DynamicMenuItemGuid == Guid.Empty ? (object)DBNull.Value : (object)DynamicMenuItemGuid;
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@MenuFavoriteGuid", _IdentityGuid);
		}

		/// <summary>
		/// Provide SQL to update in DB
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblMenuFavorites SET " +
				"UserGuid = @UserGuid, " +
				"IsQuickLink = @IsQuickLink, " +
				"CustomName = @CustomName, " +
				"DisplayOrder = @DisplayOrder, " +
				"MenuItemType = @MenuItemType, " +
				"DynamicMenuItemGuid = @DynamicMenuItemGuid, " +
				"CreatedDate = @CreatedDate, " +
				"CreatedBy = @CreatedBy, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy " +
				"WHERE MenuFavoriteGuid = @MenuFavoriteGuid";

			cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
			cmd.Parameters.AddWithValue("@IsQuickLink", IsQuickLink);
			cmd.Parameters.Add("@CustomName", SqlDbType.NVarChar).Value =
				CustomName == "" ? (object)DBNull.Value : (object)CustomName;
			cmd.Parameters.AddWithValue("@DisplayOrder", DisplayOrder);
			cmd.Parameters.AddWithValue("@MenuItemType", MenuItemType);
			cmd.Parameters.Add("@DynamicMenuItemGuid", SqlDbType.UniqueIdentifier).Value =
				DynamicMenuItemGuid == Guid.Empty ? (object)DBNull.Value : (object)DynamicMenuItemGuid;
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@MenuFavoriteGuid", _IdentityGuid);
		}

		/// <summary>
		/// Provide SQL to delete from DB
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblMenuFavorites WHERE MenuFavoriteGuid = @MenuFavoriteGuid";
			cmd.Parameters.AddWithValue("@MenuFavoriteGuid", _IdentityGuid);
		}

			/// <summary>
		/// Provide SQL to delete from DB
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		public void PurgeByUserSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblMenuFavorites WHERE UserGuid = @UserGuid";
			cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
		}
		#endregion
	}
}
