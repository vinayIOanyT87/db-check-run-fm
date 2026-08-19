using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(GroupRightMapClass))]
	public class GroupRightMapCollectionClass : List<GroupRightMapClass> { }

	/// <summary>
	/// Summary description for GroupRightMapClass.
	/// </summary>
	public class GroupRightMapClass : BaseDataObject
	{
		public Guid GroupGuid;
		public RIGHT Right;

		public GroupRightMapClass()
		{
			Reset();
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

			GroupGuid = DataObject.getValue<Guid>(Row["GroupGuid"], Guid.Empty);
			Right = DataObject.getValue<RIGHT>(Row["LookupRightIndex"], RIGHT.VIEW_USERS);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
		}

		public SqlCommand InsertSQLCmd_
		{
			get
			{

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = "INSERT INTO map.tblGroupToRight " +
			  "(GroupGuid," +
			  "LookupRightIndex," +
			  "CreatedDate," +
			  "CreatedBy" +
			  ") VALUES (" +
					DataObject.AddParameter(cmd, string.Empty, "@GroupGuid", GroupGuid) +
					DataObject.AddParameter(cmd, ", ", "@LookupRightIndex", (int)Right) +
					DataObject.AddParameter(cmd, ", ", "@CreatedDate", _CreatedDate) +
					DataObject.AddParameter(cmd, ", ", "@CreatedBy", _CreatedBy)+
			  ")";

				return cmd;
			}
		}

		public SqlCommand PurgeSQLCmd
		{
			get
			{
				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = "DELETE FROM map.tblGroupToRight WHERE " +
					DataObject.AddParameter(cmd, false, "GroupGuid", "@GroupGuid", GroupGuid)+
					DataObject.AddParameter(cmd, true, "LookupRightIndex", "@LookupRightIndex", (int)Right);

				return cmd;
			}
		}

		public SqlCommand SelectSQLCmd(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();
			cmd.CommandText = "SELECT * FROM map.tblGroupToRight " + SQLUpdateLock(bInTransaction) + " WHERE "+
									DataObject.AddParameter(cmd, false, "GroupGuid", "@GroupGuid", GroupGuid) +
									DataObject.AddParameter(cmd, true, "LookupRightIndex", "@LookupRightIndex", (int)Right);

			return cmd;
		}

		/// <summary>
		/// This method queries the database to determine if a group has a specific security right assigned to it.
		/// </summary>
		/// <param name="bInTransaction">A bool indicating if this call is wrapped in a transaction</param>
		/// <param name="groupGuid">A guid representing the unique id of the group</param>
		/// <param name="right">The security right</param>
		/// <returns>A SqlCommand containing a bool that indicates whether or not this group has this right assigned</returns>
		public SqlCommand SelectSQLCmd(bool bInTransaction, Guid groupGuid, RIGHT right)
		{
			SqlCommand cmd = new SqlCommand();
			cmd.CommandText = "SELECT CAST( CASE WHEN EXISTS (SELECT GroupToRightGuid FROM map.tblGroupToRight " +
									SQLUpdateLock(bInTransaction) + " WHERE " +
									DataObject.AddParameter(cmd, false, "GroupGuid", "@GroupGuid", groupGuid) +
									DataObject.AddParameter(cmd, true, "LookupRightIndex", "@LookupRightIndex", (int)right) +
									") THEN 1 ELSE 0 END AS bit) AS HasRight";

			return cmd;
		}
	}
}
