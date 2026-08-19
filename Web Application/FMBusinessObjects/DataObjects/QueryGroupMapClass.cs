using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
	public class QueryGroupMapClass : BaseDataObject
	{
		[DataMember]
		public Guid QueryStorageGuid { get; set; }
		[DataMember]
		public Guid GroupGuid { get; set; }

		public QueryGroupMapClass()
		{
			Reset();
		}

		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.QUERY_GROUP_ASSIGNMENT;
			}
		}

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
			GroupGuid = Guid.Empty;
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
				throw new ArgumentNullException("Set");

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return;

			DataRow Row = Table.Rows[0];

			QueryStorageGuid = DataObject.getValue<Guid>(Row["QueryStorageGuid"], Guid.Empty);
			GroupGuid = DataObject.getValue<Guid>(Row["GroupGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_IdentityGuid = DataObject.getValue<Guid>(Row["QueryStorageToGroupGuid"], Guid.Empty);
		}


		#region SQL Command with Parameters

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblQueryStorageToGroup (" +
				" QueryStorageGuid, " +
				" GroupGuid, " +
				" CreatedDate, " +
				" CreatedBy, " +
				" UpdatedDate, " +
				" UpdatedBy, " +
				" QueryStorageToGroupGuid) " +
				" VALUES (" +
				" @QueryStorageGuid, " +
				" @GroupGuid, " +
				" @CreatedDate, " +
				" @CreatedBy, " +
				" @UpdatedDate, " +
				" @UpdatedBy, " +
				" @QueryStorageToGroupGuid)";

			cmd.Parameters.Add("@QueryStorageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@QueryStorageToGroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@QueryStorageGuid"].Value = QueryStorageGuid;
			cmd.Parameters["@GroupGuid"].Value = GroupGuid;
			cmd.Parameters["@CreatedDate"].Value = _CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = _CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = _UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = _UpdatedBy;
			cmd.Parameters["@QueryStorageToGroupGuid"].Value = _IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblQueryStorageToGroup WHERE QueryStorageGuid = @QueryStorageGuid AND GroupGuid = @GroupGuid";

			cmd.Parameters.Add("@QueryStorageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@QueryStorageGuid"].Value = QueryStorageGuid;
			cmd.Parameters["@GroupGuid"].Value = GroupGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM map.tblQueryStorageToGroup " + SQLUpdateLock(bInTransaction) +
			" WHERE QueryStorageGuid = @QueryStorageGuid AND GroupGuid = @GroupGuid";

			cmd.Parameters.Add("@QueryStorageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@QueryStorageGuid"].Value = QueryStorageGuid;
			cmd.Parameters["@GroupGuid"].Value = GroupGuid;
		}

		public void EnumerateGroupsSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM map.tblQueryStorageToGroup m JOIN dbo.tblGroups g ON m.GroupGuid=g.GroupGuid " + SQLUpdateLock(bInTransaction) +
			" WHERE m.QueryStorageGuid = @QueryGuid";

			cmd.Parameters.Add("@QueryGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@QueryGuid"].Value = this.IdentityGuid;

		}

		#endregion
	}
}
