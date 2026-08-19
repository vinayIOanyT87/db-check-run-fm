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
	public class TankMapCollectionClass : List<TankMapClass> { }

	/// <summary>
	/// Summary description for TankMapClass.
	/// </summary>
   [Serializable]
   [DataContract]
	public class TankMapClass : BaseDataObject
	{
		[DataMember]
		protected Guid _TankGuid;

		[DataMember]
		public string AssignedID { get; set; }

		public Guid TankGuid { get { return _TankGuid; } set { _TankGuid = value; } }

		string Select = "SELECT map.tblTankToTankGroup.*," +
		 "(SELECT TankID FROM tblTanks WHERE tblTanks.TankGuid = map.tblTankToTankGroup.TankGuid) AS AssignedID," +
		 "(SELECT ID FROM tblTankGroups WHERE tblTankGroups.TankGroupGuid = map.tblTankToTankGroup.AssignedToTankGroupGuid) AS AssignedToID ";

		public TankMapClass()
		{
			Reset();
		}

		public override string ID
		{
			get
			{
				return _ID + " - " + AssignedID;
			}
			set
			{
				_ID = value;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.TANK_MAP;
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

		public override void Reset()
		{
			base.Reset();
			_TankGuid = Guid.Empty;
			AssignedID = "";
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

				_IdentityGuid = DataObject.getValue<Guid>(Row["AssignedToTankGroupGuid"], Guid.Empty);
				_TankGuid = DataObject.getValue<Guid>(Row["TankGuid"], Guid.Empty);
				_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				AssignedID = DataObject.getValue<string>(Row["AssignedID"], "");
				_ID = DataObject.getValue<string>(Row["AssignedToID"], "");
			}

			else if (typeof(TankMapClass).IsInstanceOfType(o))
			{
				TankMapClass TankMap = (TankMapClass)o;
				_IdentityGuid = TankMap.IdentityGuid;
				_TankGuid = TankMap.TankGuid;
				_CreatedDate = TankMap.CreatedDate;
				_CreatedBy = TankMap.CreatedBy;
				_UpdatedDate = TankMap.UpdatedDate;
				_UpdatedBy = TankMap.UpdatedBy;
				AssignedID = TankMap.AssignedID;
				_ID = TankMap.ID;
			}

		}


		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblTankToTankGroup " +
				"(AssignedToTankGroupGuid," +
				"TankGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy" +
				") VALUES (" +
				"@AssignedToTankGroupGuid," +
				"@TankGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy)";

			cmd.Parameters.AddWithValue("@AssignedToTankGroupGuid", _IdentityGuid);
			cmd.Parameters.AddWithValue("@TankGuid", _TankGuid);
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
		}


		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblTankToTankGroup WHERE AssignedToTankGroupGuid = @AssignedToTankGroupGuid" +
				" AND TankGuid = @TankGuid";

			cmd.Parameters.AddWithValue("@AssignedToTankGroupGuid", _IdentityGuid);
			cmd.Parameters.AddWithValue("@TankGuid", _TankGuid);
		}


		public void EnumerateByAssignedToTankGroupGuidSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = Select +
				" FROM map.tblTankToTankGroup " + SQLUpdateLock(bInTransaction) + " WHERE AssignedToTankGroupGuid = @AssignedToTankGroupGuid";
			cmd.Parameters.AddWithValue("@AssignedToTankGroupGuid", _IdentityGuid);
		}


		public void EnumerateByTankGuidSQL(SqlCommand cmd)
		{
			cmd.CommandText = Select +
				" FROM map.tblTankToTankGroup WHERE TankGuid = @TankGuid";
			cmd.Parameters.AddWithValue("@TankGuid", _TankGuid);
		}
	}
}
