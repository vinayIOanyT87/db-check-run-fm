using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Contains a list of product groups
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	public class ProductGroupCollectionClass : List<ProductGroupClass>
	{
	}

	/// <summary>
	/// Defines a group of products
	/// </summary>
	[DataContract]
	[Serializable()]
	[KnownType(typeof(ProductMapCollectionClass))]
	[KnownType(typeof(ApplicationStringMapCollectionClass))]
	public class ProductGroupClass : BaseDataObject
	{
		#region Public data members
		[DataMember]
		public STRING_TYPE Type;
		[DataMember]
		public ProductMapCollectionClass ProductMapCollection;
		[DataMember]
		public ApplicationStringMapCollectionClass EntryMessageCollection;
		[DataMember]
		public ApplicationStringMapCollectionClass ExitMessageCollection;
		#endregion

		#region Properties
		public override string ID
		{
			get { return _ID; }
			set { SetString("Product Group ID", 30, value, ref _ID); }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.PRODUCT_GROUP;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		#endregion

		public ProductGroupClass()
		{
			this.Reset();
		}

		public override void Reset()
		{
			base.Reset();

			this.Type = STRING_TYPE.PRODUCT_GROUP;
			this.ProductMapCollection = new ProductMapCollectionClass();
			this.EntryMessageCollection = new ApplicationStringMapCollectionClass();
			this.ExitMessageCollection = new ApplicationStringMapCollectionClass();
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("Set");
			}

			this.Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

			_IdentityGuid = DataObject.getValue<Guid>(Row["ApplicationStringGuid"], Guid.Empty);
			Type = DataObject.getValue<STRING_TYPE>(Row["LookupApplicationStringTypeIndex"], STRING_TYPE.PRODUCT_GROUP);
			_ID = DataObject.getValue<string>(Row["ID"], "");
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
		}

		public bool IsProductInGroup(Guid identityGuid)
		{
			foreach (ProductMapClass ProductMap in ProductMapCollection)
			{
				if (ProductMap.AssignedGuid == identityGuid)
				{
					return true;
				}
			}

			return false;
		}

		//public string InsertSQL_
		//{
		//   get
		//   {
		//      string SQL;

		//      SQL = "INSERT INTO tblApplicationString " +
		//        "(LookupApplicationStringTypeIndex," +
		//        "ID," +
		//        "SiteGuid," +
		//        "CreatedDate," +
		//        "CreatedBy," +
		//        "UpdatedDate," +
		//        "UpdatedBy" +
		//        ") VALUES (" +
		//        ((int)Type).ToString() + "," +
		//        "N'" + _ID + "'," +
		//        "'" + _SiteGuid.ToString() + "'," +
		//        _CreatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + "," +
		//        "N'" + _CreatedBy + "'," +
		//        _UpdatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + "," +
		//        "N'" + _UpdatedBy + "'" +
		//        ") SELECT ApplicationStringGuid" +
		//        " FROM tblApplicationString" +
		//        " WHERE ID = N'" + _ID + "'" +
		//        " AND LookupApplicationStringTypeIndex = '" + ((int)Type).ToString() + "'" +
		//        " AND SiteGuid = '" + _SiteGuid.ToString() + "'";

		//      return SQL;
		//   }
		//}

		//public string UpdateSQL
		//{
		//   get
		//   {
		//      string SQL;

		//      SQL = "UPDATE tblApplicationString " +
		//        "SET ID = N'" + _ID + "'," +
		//        "SiteGuid = '" + _SiteGuid.ToString() + "'," +
		//        "UpdatedDate = " + _UpdatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + "," +
		//        "UpdatedBy = N'" + _UpdatedBy + "' " +
		//        "WHERE ApplicationStringGuid = '" + _IdentityGuid.ToString() + "' AND LookupApplicationStringTypeIndex = '" + ((int)Type).ToString() + "'";

		//      return SQL;
		//   }
		//}

		//public string PurgeSql
		//{
		//   get
		//   {
		//      string SQL;

		//      SQL = "DELETE FROM tblApplicationString WHERE ApplicationStringGuid = '" + _IdentityGuid.ToString() + "' AND LookupApplicationStringTypeIndex = '" + ((int)Type).ToString() + "'";

		//      return SQL;
		//   }
		//}

		//public string SelectSQL(bool bInTransaction)
		//{
		//   string SQL;

		//   SQL = "SELECT tblApplicationString.* FROM tblApplicationString " + SQLUpdateLock(bInTransaction) + " WHERE ApplicationStringGuid = '" + _IdentityGuid.ToString() + "'";

		//   return SQL;
		//}

		//public string SelectByIDSQL(SecurityClass security, bool bInTransaction)
		//{
		//   string SQL;

		//   SQL = "SELECT tblApplicationString.*" +
		//      " FROM tblApplicationString " + SQLUpdateLock(bInTransaction) +
		//      " WHERE" + SiteWhereClause(security, "tblApplicationString", "ApplicationStringGuid") +
		//      " AND ID = N'" + _ID + "'" +
		//      " AND LookupApplicationStringTypeIndex = '" + ((int)Type).ToString() + "'";

		//   return SQL;
		//}

		//public string EnumerateSql(SecurityClass security)
		//{
		//   string SQL;

		//   SQL = "SELECT tblApplicationString.*" +
		//      " FROM tblApplicationString" +
		//      " WHERE" + SiteWhereClause(security, "tblApplicationString", "ApplicationStringGuid") +
		//      " AND LookupApplicationStringTypeIndex = '" + ((int)Type).ToString() + "'" +
		//      " ORDER BY ID";

		//   return SQL;
		//}
		
#region Paramaterized SQL Commands

		public void InsertSQL(SqlCommand cmd) 
		{
			cmd.CommandText = "INSERT INTO tblApplicationString " +
				  "(LookupApplicationStringTypeIndex, " +
				  "ID, " +
				  "SiteGuid, " +
				  "CreatedDate, " +
				  "CreatedBy, " +
				  "UpdatedDate, " +
				  "UpdatedBy, " +
				  "ApplicationStringGuid)" +
				  "VALUES (" +
				  "@LookupApplicationStringTypeIndex, " +
				  "@ID, " +
				  "@SiteGuid, " +
				  "@CreatedDate, " +
				  "@CreatedBy, " +
				  "@UpdatedDate, " +
				  "@UpdatedBy," +
				  "@ApplicationStringGuid)";
				  
				  cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
				  cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
				  cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				  cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset); 
				  cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
				  cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
				  cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				  cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
					  
				  cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = Type;
				  cmd.Parameters["@ID"].Value = ID;
				  cmd.Parameters["@SiteGuid"].Value = SiteGuid;  
				  cmd.Parameters["@CreatedDate"].Value = CreatedDate;
				  cmd.Parameters["@CreatedBy"].Value = CreatedBy;
				  cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
				  cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
				  cmd.Parameters["@ApplicationStringGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblApplicationString " +
				"SET ID = @ID, " +
				"SiteGuid = @SiteGuid, " + 
				"UpdatedDate = @UpdatedDate, " + 
				"UpdatedBy = @UpdatedBy " +
				"WHERE ApplicationStringGuid = @ApplicationStringGuid AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier); 
			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);		 
 
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;  
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;	
			cmd.Parameters["@ApplicationStringGuid"].Value = IdentityGuid;
			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = Type;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblApplicationString WHERE ApplicationStringGuid = @ApplicationStringGuid AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";

			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);		 
 
			cmd.Parameters["@ApplicationStringGuid"].Value = IdentityGuid;
			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = Type;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblApplicationString.* FROM tblApplicationString " + SQLUpdateLock(bInTransaction) + 
				" WHERE ApplicationStringGuid = @ApplicationStringGuid";

			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ApplicationStringGuid"].Value = IdentityGuid;
		}

		public void SelectByIDSQL(SqlCommand cmd, bool bInTransaction, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" +
				" FROM tblApplicationString " + SQLUpdateLock(bInTransaction) +
				" WHERE" + AppendSiteWhereClauseParameters(cmd, security, "tblApplicationString", "ApplicationStringGuid") +
				" AND ID = @ID AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";
		
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);		 
 
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = Type;
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" +
					" FROM tblApplicationString" +
					" WHERE" + AppendSiteWhereClauseParameters(cmd, security, "tblApplicationString", "ApplicationStringGuid") +
					" AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex " + 
					" ORDER BY ID";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = Type;
		}

	}
}
#endregion


