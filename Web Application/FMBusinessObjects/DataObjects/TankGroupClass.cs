using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class TankGroupCollectionClass : List<TankGroupClass> { }

   [Serializable]
   [DataContract]
	public class TankGroupClass : BaseDataObject
	{
		[DataMember] protected Guid _ProductGuid;
		[DataMember] public TankMapCollectionClass TankMapCollection;
		[DataMember] public string ProductID;

		public override string ID { get { return _ID; } set { SetString("Tank Group ID", 30, value, ref _ID); } }

		public Guid ProductGuid { get { return _ProductGuid; } set { _ProductGuid = value; } }

		protected string SelectClause = "SELECT *," +
												 "(SELECT ProductID FROM tblProducts WHERE tblProducts.ProductGuid = tblTankGroups.ProductGuid) AS ProductID ";


		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TANK_GROUP;
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


		public TankGroupClass()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			_ProductGuid = Guid.Empty;
			TankMapCollection = new TankMapCollectionClass();
			ProductID = "{None}";
		}

		public override void Load(Object o)
		{

			Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;

				DataTable Table = Set.Tables[0];
				if (Table.Rows.Count == 0)
					return;

				DataRow Row = Table.Rows[0];

				_IdentityGuid = DataObject.getValue<Guid>(Row["TankGroupGuid"], Guid.Empty);
				_ID = DataObject.getValue<string>(Row["ID"], "");
				_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				_ProductGuid = DataObject.getValue<Guid>(Row["ProductGuid"], Guid.Empty);
				_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				ProductID = DataObject.getValue<string>(Row["ProductID"], "{None}");
			}

			else if (typeof(TankGroupClass).IsInstanceOfType(o))
			{
				TankGroupClass TankGroup = (TankGroupClass)o;
				_IdentityGuid = TankGroup.IdentityGuid;
				_SiteGuid = TankGroup.SiteGuid; 
				_ID = TankGroup.ID;
				_ProductGuid = TankGroup.ProductGuid;
				_CreatedDate = TankGroup.CreatedDate;
				_CreatedBy = TankGroup.CreatedBy;
				_UpdatedDate = TankGroup.UpdatedDate;
				_UpdatedBy = TankGroup.UpdatedBy;
				ProductID = TankGroup.ProductID;
				foreach (TankMapClass ExistingTankMap in TankGroup.TankMapCollection)
				{
					TankMapClass NewTankMap = new TankMapClass();
					NewTankMap.Load(ExistingTankMap);
					TankMapCollection.Add(NewTankMap);
				}
			}
		}

		public bool IsTankInGroup(Guid identityGuid)
		{
			foreach (TankMapClass TankMap in TankMapCollection)
				if (TankMap.TankGuid == identityGuid)
					return true;

			return false;
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTankGroups " +
			  "(ID," +
			  "SiteGuid," +
			  "ProductGuid," +
			  "CreatedDate," +
			  "CreatedBy," +
			  "UpdatedDate," +
			  "UpdatedBy," +
			  "TankGroupGuid" +
			  ") VALUES (" +
			  "@ID," +
			  "@SiteGuid," +
			  "@ProductGuid," +
			  "@CreatedDate," +
			  "@CreatedBy," +
			  "@UpdatedDate," +
			  "@UpdatedBy," +
			  "@TankGroupGuid) ";

			cmd.Parameters.AddWithValue("@ID", _ID);
			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = 
				(_ProductGuid == Guid.Empty) ? (object)DBNull.Value : (object)_ProductGuid;
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@TankGroupGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTankGroups " +
			  "SET ID = @ID, " +
			  "ProductGuid = @ProductGuid, " +
			  "UpdatedDate = @UpdatedDate, " +
			  "UpdatedBy = @UpdatedBy " +
			  "WHERE TankGroupGuid = @TankGroupGuid";

			cmd.Parameters.AddWithValue("@ID", _ID);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = 
				(_ProductGuid == Guid.Empty) ? (object)DBNull.Value : (object)_ProductGuid;
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@TankGroupGuid", _IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTankGroups WHERE TankGroupGuid = @TankGroupGuid";
			cmd.Parameters.AddWithValue("@TankGroupGuid", _IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblTankGroups " + SQLUpdateLock(bInTransaction) +
				" WHERE TankGroupGuid = @TankGroupGuid";

			cmd.Parameters.AddWithValue("@TankGroupGuid", _IdentityGuid);
		}

		public void SelectByIDSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblTankGroups " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid AND " +
				"ID = @ID";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ID", _ID);
		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
			  " FROM tblTankGroups WHERE SiteGuid = @SiteGuid ORDER BY ID";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
		}

		public void EnumerateByProductSQL(SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
			  " FROM tblTankGroups" +
			  " WHERE SiteGuid = @SiteGuid " +
			  " AND ProductGuid = @ProductGuid " +
			  " ORDER BY ID";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ProductGuid", _ProductGuid);
		}
	}
}
