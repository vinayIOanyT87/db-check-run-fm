using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region Group Ledger View Map Collection Class
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(GroupLedgerViewMapClass))]
	public class GroupLedgerViewMapCollectionClass : List<GroupLedgerViewMapClass>
	{
		public GroupLedgerViewMapClass FindByGroupGuid(Guid groupGuid)
		{
			foreach (GroupLedgerViewMapClass item in this)
			{
				if (item.GroupGuid == groupGuid)
				{
					return item;
				}
			}

			return null;
		}
	}
	#endregion

	/// <summary>
	/// Summary description for GroupLedgerViewMapClass.
	/// </summary>
	[DataContract]
   [Serializable]
	public class GroupLedgerViewMapClass : BaseDataObject
	{
		#region Public data members
		[DataMember] public bool AuditLog = false;
		#endregion

		#region Protected data members
		[DataMember] protected Guid _GroupGuid;
		[DataMember] protected Guid _ListViewGuid;
		#endregion


		#region Constructors
		/// <summary>
		/// This is the default constructor for the group ledger view map class.
		/// </summary>
		public GroupLedgerViewMapClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		public Guid GroupGuid
		{
			get { return _GroupGuid; }
			set { _GroupGuid = value; }
		}

		public Guid ListViewGuid
		{
			get { return _ListViewGuid; }
			set { _ListViewGuid = value; }
		}

		//public string InsertSQL_
		//{
		//   get
		//   {
		//      string SQL;

		//      SQL = "INSERT INTO map.tblGroupToLedgerView " +
		//           "(GroupGuid," +
		//           "ListViewGuid," +
		//           "CreatedDate," +
		//           "CreatedBy" +
		//           ") VALUES (" +
		//           "'" + _GroupGuid.ToString() + "'," +
		//           "'" + _ListViewGuid.ToString() + "'," +
		//           _CreatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + "," +
		//           "N'" + _CreatedBy + "'" + ")";

		//      return SQL;
		//   }
		//}

		//public string PurgeSql
		//{
		//   get
		//   {
		//      string SQL;

		//      SQL = "DELETE FROM map.tblGroupToLedgerView WHERE GroupGuid = '" + _GroupGuid.ToString() + "' AND ListViewGuid = '" + _ListViewGuid.ToString() + "'";

		//      return SQL;
		//   }
		//}

		//public string EnumerateByListViewGuid
		//{
		//   get
		//   {
		//      string SQL;

		//      SQL = "SELECT M.*, G.GroupID" +
		//           " FROM map.tblGroupToLedgerView M JOIN tblGroups G on M.GroupGuid = G.GroupGuid" +
		//           " WHERE ListViewGuid = '" + _ListViewGuid.ToString() +"'";

		//      return SQL;
		//   }
		//}
		#endregion


		#region Sql Commands with Parameters

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblGroupToLedgerView ( " +
					  " GroupGuid, " +
					  " ListViewGuid, " +
					  " CreatedDate, " +
					  " CreatedBy, " +
					  " UpdatedDate, " +
					  " UpdatedBy, " +
					  "GroupToLedgerViewGuid" +
					  ") VALUES ( " +
					  " @GroupGuid, " +
					  " @ListViewGuid, " +
					  " @CreatedDate, " +
					  " @CreatedBy, " +
					  " @UpdatedDate, " +
					  " @UpdatedBy, " +
					  " @GroupToLedgerViewGuid)";

			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ListViewGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@GroupToLedgerViewGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@GroupGuid"].Value = _GroupGuid;
			cmd.Parameters["@ListViewGuid"].Value = _ListViewGuid;
			cmd.Parameters["@CreatedDate"].Value = _CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = _CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = _UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = _UpdatedBy;
			cmd.Parameters["@GroupToLedgerViewGuid"].Value = _IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			string SQL;

			SQL = "DELETE FROM map.tblGroupToLedgerView";

			if (this.GroupGuid != Guid.Empty || this.ListViewGuid != Guid.Empty)
			{
				SQL += " WHERE";
				if (this.GroupGuid != Guid.Empty)
				{
					SQL += " GroupGuid = @GroupGuid";
					cmd.Parameters.AddWithValue("@GroupGuid", this.GroupGuid);

					if (this.ListViewGuid != Guid.Empty)
					{
						SQL += " AND ListViewGuid = @ListViewGuid";
						cmd.Parameters.AddWithValue("@ListViewGuid", this.ListViewGuid);
					}
				}
				else
				{
					SQL += " ListViewGuid = @ListViewGuid";
					cmd.Parameters.AddWithValue("@ListViewGuid", this.ListViewGuid);
				}
			}

			cmd.CommandText = SQL;
		}

		public void EnumerateByListViewGuid(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT M.*, G.GroupID" +
				" FROM map.tblGroupToLedgerView M JOIN tblGroups G on M.GroupGuid = G.GroupGuid" +
				 " WHERE ListViewGuid = @ListViewGuid";
			cmd.Parameters.Add("@ListViewGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ListViewGuid"].Value = _ListViewGuid;

		}
		#endregion
		#region Public and internal methods
		public override void Reset()
		{
			base.Reset();
			this._GroupGuid = Guid.Empty;
			this._ListViewGuid = Guid.Empty;
		}

		public override void Load(Object o)
		{
			Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				_GroupGuid = DataObject.getValue<Guid>(Row["GroupGuid"], Guid.Empty);
				_ListViewGuid = DataObject.getValue<Guid>(Row["ListViewGuid"], Guid.Empty);
				_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				_ID = DataObject.getValue<string>(Row["GroupID"], "");
			}
			else if (typeof(GroupLedgerViewMapClass).IsInstanceOfType(o))
			{
				GroupLedgerViewMapClass GroupLedgerViewMap = (GroupLedgerViewMapClass)o;
				_GroupGuid = GroupLedgerViewMap.GroupGuid;
				_ListViewGuid = GroupLedgerViewMap.ListViewGuid;
				_CreatedDate = GroupLedgerViewMap.CreatedDate;
				_CreatedBy = GroupLedgerViewMap.CreatedBy;
			}
		}

		public string SelectSQL(bool bInTransaction)
		{
			string SQL;

			SQL = "SELECT M.*, G.GroupID FROM map.tblGroupToLedgerView M " + SQLUpdateLock(bInTransaction) + " join tblGroups G on M.GroupGuid = G.GroupGuid WHERE ListViewGuid = '" + _ListViewGuid.ToString() + "'";

			return SQL;
		}
		#endregion
	}
}
