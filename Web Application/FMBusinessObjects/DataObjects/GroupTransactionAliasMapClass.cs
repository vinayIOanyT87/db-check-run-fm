using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class GroupTransactionAliasMapCollectionClass : List<GroupTransactionAliasMapClass> { }

	[DataContract]
   [Serializable]
	public class GroupTransactionAliasMapClass : BaseDataObject
	{
		public enum RIGHT { VIEW = 0, MODIFY };

        [DataMember]
        public override string ID
		{
			get { return _ID; }
			set { _ID = value; }
        }

		[DataMember]
        [XmlIgnore]
		public Guid GroupGuid { get; set; }

		[DataMember]
        [XmlIgnore]
		public Guid TransactionAliasGuid { get; set; }

		[DataMember]
		public RIGHT Right { get; set; }

		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		[XmlIgnore]
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
			TransactionAliasGuid = Guid.Empty;
			Right = RIGHT.VIEW;
		}

		/// <summary>
		/// This method will load the object with the retrieved data.
		/// </summary>
		/// <param name="o"></param>
		public override void Load(Object o)
		{
			base.Load(o);
			this.Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				var set = (DataSet) o;
				DataTable table = set.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				GroupGuid				= DataObject.getValue<Guid>(row["GroupGuid"], Guid.Empty);
				TransactionAliasGuid	= DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
				CreatedDate				= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				CreatedBy				= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
				Right					= (RIGHT)DataObject.getValue<int>(row["LookupRightIndex"], (int)RIGHT.VIEW);
				ID						= DataObject.getValue<string>(row["GroupID"], "");
			}
			else if (typeof(GroupTransactionAliasMapClass).IsInstanceOfType(o))
			{
				var groupTransactionAliasMap	= (GroupTransactionAliasMapClass) o;
				GroupGuid						= groupTransactionAliasMap.GroupGuid;
				TransactionAliasGuid			= groupTransactionAliasMap.TransactionAliasGuid;
				CreatedDate						= groupTransactionAliasMap.CreatedDate;
				CreatedBy						= groupTransactionAliasMap.CreatedBy;
				ID								= groupTransactionAliasMap.ID;
				Right							= RIGHT.VIEW;
			}
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblGroupToTransactionAlias " +
				"(GroupGuid," +
				"TransactionAliasGuid," +
				"LookupRightIndex," +
				"CreatedDate," +
				"CreatedBy" +
				") VALUES (" +
				"@GroupGuid," +
				"@TransactionAliasGuid," +
				"@LookupRightIndex," +
				"@CreatedDate," +
				"@CreatedBy" +
				")";

			cmd.Parameters.AddWithValue("@GroupGuid", GroupGuid);
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
			cmd.Parameters.AddWithValue("@LookupRightIndex", (int)Right);
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE map.tblGroupToTransactionAlias SET " +
				"LookupRightIndex = @LookupRightIndex, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy " +
				"WHERE GroupGuid = @GroupGuid " +
				"AND TransactionAliasGuid = @TransactionAliasGuid";

			cmd.Parameters.AddWithValue("@GroupGuid", GroupGuid);
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
			cmd.Parameters.AddWithValue("@LookupRightIndex", (int)Right);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			string SQL;

			SQL = "DELETE FROM map.tblGroupToTransactionAlias";

			if (GroupGuid != Guid.Empty || TransactionAliasGuid != Guid.Empty)
			{
				SQL += " WHERE";
				if (GroupGuid != Guid.Empty)
				{
					SQL += " GroupGuid = @GroupGuid";
					cmd.Parameters.AddWithValue("@GroupGuid", GroupGuid);

					if (TransactionAliasGuid != Guid.Empty)
					{
						SQL += " AND TransactionAliasGuid = @TransactionAliasGuid";
						cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
					}
				}
				else
				{
					SQL += " TransactionAliasGuid = @TransactionAliasGuid";
					cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
				}
			}

			cmd.CommandText = SQL;
		}

		public void PurgeByGroupAndRightSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblGroupToTransactionAlias WHERE GroupGuid = @GroupGuid AND LookupRightIndex = @LookupRightIndex ";

			cmd.Parameters.AddWithValue("@GroupGuid", this.GroupGuid);
			cmd.Parameters.AddWithValue("@LookupRightIndex", (int) this.Right);
		}


		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT map.tblGroupToTransactionAlias.*, G.GroupID FROM map.tblGroupToTransactionAlias " +
				" LEFT OUTER JOIN tblGroups G on G.GroupGuid = map.tblGroupToTransactionAlias.GroupGuid " +
				SQLUpdateLock(bInTransaction) + " WHERE TransactionAliasGuid = @TransactionAliasGuid";

			cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
		}

		public void EnumerateByTransactionAliasGuidSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT map.tblGroupToTransactionAlias.*, G.GroupID FROM map.tblGroupToTransactionAlias " +
				" LEFT OUTER JOIN tblGroups G on G.GroupGuid = map.tblGroupToTransactionAlias.GroupGuid " +
				" WHERE TransactionAliasGuid = @TransactionAliasGuid";

			cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
		}

		/// <summary>
		/// This method will populate the SQL Command to enumerate Group to Transaction Aliases based on
		/// the transaction alias GUID.
		/// </summary>
		/// <param name="command">SQL command object.</param>
		public void EnumerateByAliasGuidSql(SqlCommand command)
		{
			command.CommandText = "SELECT map.tblGroupToTransactionAlias.*, G.GroupID FROM tblGroupTransactionAliasMap " +
								  " LEFT OUTER JOIN tblGroups G on G.GroupGuid = map.tblGroupToTransactionAlias.GroupGuid " +
								  " WHERE TransactionAliasGuid = @TransactionAliasGuid";

			command.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier).Value = this.TransactionAliasGuid;
		}
	}
}