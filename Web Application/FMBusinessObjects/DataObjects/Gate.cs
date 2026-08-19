using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(GateClass))]
	public class GateCollectionClass : List<GateClass> { }


	/// <summary>
	/// Summary description for GateClass.
	/// </summary>
	[DataContract]
   [Serializable]
	public class GateClass : BaseDataObject
	{
		#region Private data members
		[DataMember]
		public string _Description;
		[DataMember]
		public string _ConcourseID;
		#endregion

		#region Constructors
		public GateClass()
		{
			this.Reset();
		}
		#endregion


		#region Properties
		public override string ID
		{
			get { return _ID; }
			set { SetString("ID", 10, value, ref _ID); }
		}

		public string Description
		{
			get { return _Description; }
			set { SetString("Description", 50, value, ref _Description); }
		}

		public string ConcourseID
		{
			get { return _ConcourseID; }
			set { SetString("Concourse ID", 6, value, ref _ConcourseID); }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.GATE; }
			set { ; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO tblGates " +
				"(SiteGuid," +
				"ID," +
				"Description," +
				"ConcourseID," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"GateGuid" +
				") VALUES (" +
				"@SiteGuid," +
				"@ID," +
				"@Description," +
				"@ConcourseID," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@GateGuid" +
				")";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@ConcourseID", SqlDbType.NVarChar, 6);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@GateGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@Description"].Value = Description;
			cmd.Parameters["@ConcourseID"].Value = ConcourseID;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@GateGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblGates " +
				"SET SiteGuid = @SiteGuid," +
				"ID = @ID," +
				"Description = @Description," +
				"ConcourseID = @ConcourseID," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy " +
				"WHERE GateGuid = @GateGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@ConcourseID", SqlDbType.NVarChar, 6);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@GateGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@Description"].Value = Description;
			cmd.Parameters["@ConcourseID"].Value = ConcourseID;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@GateGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblGates WHERE GateGuid = @GateGuid";
			cmd.Parameters.Add("@GateGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@GateGuid"].Value = IdentityGuid;
		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT tblGates.* FROM tblGates" +
				" WHERE SiteGuid = @SiteGuid" +
				" ORDER BY ID";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}

		#endregion

		#region Public and internal methods
		public override void Reset()
		{
			base.Reset();
			this.Description = "";
			this.ConcourseID = "";
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

			_IdentityGuid = DataObject.getValue<Guid>(Row["GateGuid"], Guid.Empty);
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			_ID = DataObject.getValue<string>(Row["ID"], "");
			Description = DataObject.getValue<string>(Row["Description"], "");
			ConcourseID = DataObject.getValue<string>(Row["ConcourseID"], "");
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
		}


		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblGates.* FROM tblGates " + SQLUpdateLock(bInTransaction) + " WHERE GateGuid = @GateGuid";
			cmd.Parameters.Add("@GateGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@GateGuid"].Value = IdentityGuid;
		}

		public void SelectByIDSQL(SqlCommand cmd, bool bInTransaction)
		{

			cmd.CommandText = "SELECT tblGates.* FROM tblGates " + SQLUpdateLock(bInTransaction) +
				" WHERE ID = @ID" +
				" AND SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}

		#endregion
	}
}

