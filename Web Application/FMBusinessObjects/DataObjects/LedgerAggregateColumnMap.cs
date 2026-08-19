using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region Ledger Aggregate Column Map Collection Class
   [Serializable]
   [CollectionDataContract]
	public class LedgerAggregateColumnMapCollectionClass : List<LedgerAggregateColumnMapClass>
	{
	}
	#endregion

	#region Ledger Aggregate Column Map Class
	/// <summary>
	/// Summary description for LedgerAggregateColumnMapClass.
	/// </summary>
	[Serializable()]
	[DebuggerDisplay("LedgerAggregateColumnGuid={LedgerAggregateColumnGuid},TransactionAliasGuid={TransactionAliasGuid},Symbol={Symbol}")]
	[DataContract]
	public class LedgerAggregateColumnMapClass : BaseDataObject
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the ledger aggreate column map class.
		/// </summary>
		public LedgerAggregateColumnMapClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		[DataMember]
		public Guid LedgerAggregateColumnGuid
		{
			get;
			set;
		}

		[DataMember]
		public Guid TransactionAliasGuid
		{
			get;
			set;
		}

		[DataMember]
		public string AliasName
		{
			get;
			set;
		}

		[DataMember]
		public string Symbol
		{
			get;
			set;
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.LEDGER_AGGREGATE_COLUMN; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
		#endregion

		public override void Reset()
		{
			base.Reset();
			LedgerAggregateColumnGuid = Guid.Empty;
			TransactionAliasGuid = Guid.Empty;
			Symbol = string.Empty;
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

			this.TransactionAliasGuid = DataObject.getValue<Guid>(Row["TransactionAliasGuid"], Guid.Empty);
			this.LedgerAggregateColumnGuid = DataObject.getValue<Guid>(Row["LedgerAggregateColumnGuid"], Guid.Empty);
			base._IdentityGuid = DataObject.getValue<Guid>(Row["LedgerAggregateColumnToTransactionAliasGuid"], Guid.Empty);
			this.Symbol = DataObject.getValue<string>(Row["Symbol"], "");
			base._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			base._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			this.AliasName = DataObject.getValue<string>(Row["AliasName"], "");

		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblLedgerAggregateColumnToTransactionAlias " +
				"(LedgerAggregateColumnGuid," +
				"TransactionAliasGuid," +
				"Symbol," +
				"CreatedDate," +
				"CreatedBy" +
				") VALUES (" +
				"@LedgerAggregateColumnGuid," +
				"@TransactionAliasGuid," +
				"@Symbol," +
				"@CreatedDate," +
				"@CreatedBy)";

			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", LedgerAggregateColumnGuid);
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
			cmd.Parameters.AddWithValue("@Symbol", Symbol);
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
		}


		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblLedgerAggregateColumnToTransactionAlias" +
					" WHERE LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid" +
					" AND TransactionAliasGuid = @TransactionAliasGuid";

			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", LedgerAggregateColumnGuid);
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT map.tblLedgerAggregateColumnToTransactionAlias.*, tblTransactionAliases.AliasName as AliasName " +
					" FROM map.tblLedgerAggregateColumnToTransactionAlias " + SQLUpdateLock(bInTransaction) +
					" LEFT OUTER JOIN tblTransactionAliases ON map.tblLedgerAggregateColumnToTransactionAlias.TransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid" +
					" WHERE LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid" +
					" AND map.tblLedgerAggregateColumnToTransactionAlias.TransactionAliasGuid = @TransactionAliasGuid";

			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", LedgerAggregateColumnGuid);
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", TransactionAliasGuid);
		}


		public void Enumerate(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT map.tblLedgerAggregateColumnToTransactionAlias.*, tblTransactionAliases.AliasName as AliasName FROM map.tblLedgerAggregateColumnToTransactionAlias" +
					" LEFT OUTER JOIN tblTransactionAliases ON map.tblLedgerAggregateColumnToTransactionAlias.TransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid" +
					" WHERE LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid" +
					" ORDER BY AliasName";

			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", LedgerAggregateColumnGuid);
		}

	}
	#endregion
}
