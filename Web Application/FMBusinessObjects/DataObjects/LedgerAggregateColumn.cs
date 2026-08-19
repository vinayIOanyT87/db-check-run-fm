namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using System.Globalization;
    using System.Diagnostics;
    using System.Data;
    using System.Data.SqlClient;

    using FMCore;

    #region Ledger Aggregate Column Collection Class
    /// <summary>
    /// Summary description for LedgerAggregateColumnCollectionClass.
    /// </summary>
    [Serializable]
   [CollectionDataContract]
	public class LedgerAggregateColumnCollectionClass : List<LedgerAggregateColumnClass>
	{
	}
	#endregion

	#region Ledger Aggregate Column Class
	/// <summary>
	/// Summary description for LedgerAggregateColumnClass.
	/// </summary>
	[Serializable()]
	[XmlInclude(typeof(GregorianCalendar))]
	[DebuggerDisplay("ID={ID},IdentityGuid={IdentityGuid}")]
	[DataContract]
	[KnownType(typeof(LedgerAggregateColumnMapCollectionClass))]
	public class LedgerAggregateColumnClass : BaseDataObject, IComparable
	{
		#region Public data members
		public enum AggregateType
		{
			NetGross,
			Number01,
			Number02,
			Number03,
			Number04,
			Number05,
			Number06,
			CustomFunction
		}
		#endregion

		#region Private data members
		[DataMember]
		private LedgerAggregateColumnMapCollectionClass aliases;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Ledger Aggregate Column class.
		/// </summary>
		public LedgerAggregateColumnClass()
		{
			this.Reset();
		}

		/// <summary>
		/// This constructor initializes the site object.
		/// </summary>
		/// <param name="Site"></param>
		public LedgerAggregateColumnClass(SiteClass Site)
		{
			this.Reset();
		}
		#endregion

		#region Properties
		public override string ID
		{
			get { return _ID; }
			set { SetString("ID", 50, value, ref _ID); }
		}

		[DataMember]
		public string CustomFunctionName
		{
			get;
			set;
		}

		[DataMember]
		public AggregateType AggregateField
		{
			get;
			set;
		}

		public LedgerAggregateColumnMapCollectionClass Aliases
		{
			get { return this.aliases; }
			set { this.aliases = value; }
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

		int IComparable.CompareTo(object obj)
		{
			LedgerAggregateColumnClass LedgerAggregateColumn = obj as LedgerAggregateColumnClass;
			if (LedgerAggregateColumn == null)
			{
				throw new Exception("Invalid LedgerAggregateColumn");
			}

			return ID.CompareTo(LedgerAggregateColumn.ID);
		}

		public override void Reset()
		{
			base.Reset();
			this.aliases = new LedgerAggregateColumnMapCollectionClass();
			CustomFunctionName = string.Empty;
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

				_IdentityGuid = DataObject.getValue<Guid>(Row["LedgerAggregateColumnGuid"], Guid.Empty);
				_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				_ID = DataObject.getValue<string>(Row["ID"], "");
				_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				CustomFunctionName = DataObject.getValue<string>(Row["CustomFunctionName"], "");
				AggregateField = DataObject.getValue<AggregateType>(Row["LookupAggregateFieldIndex"], AggregateType.CustomFunction);

			}
			else
			{
				base.Load(o);
			}
		}

		public override bool Equals(object obj)
		{
			LedgerAggregateColumnClass testObject = (LedgerAggregateColumnClass)obj;
			if (testObject != null)
			{
				return testObject.IdentityGuid == this.IdentityGuid;
			}

			return base.Equals(obj);

		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblLedgerAggregateColumns " +
				"(SiteGuid," +
				"ID," +
				"LookupAggregateFieldIndex," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"CustomFunctionName," +
				"LedgerAggregateColumnGuid " +
				") VALUES (" +
				"@SiteGuid," +
				"@ID," +
				"@LookupAggregateFieldIndex," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@CustomFunctionName," +
				"@LedgerAggregateColumnGuid)";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ID", _ID);
			cmd.Parameters.AddWithValue("@LookupAggregateFieldIndex", ((int)AggregateField));
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@CustomFunctionName", CustomFunctionName);
			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblLedgerAggregateColumns " +
				"SET SiteGuid = @SiteGuid, " +
				"ID = @ID, " +
				"LookupAggregateFieldIndex = @LookupAggregateFieldIndex, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"CustomFunctionName = @CustomFunctionName " +
				"WHERE LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@ID", _ID);
			cmd.Parameters.AddWithValue("@LookupAggregateFieldIndex", ((int)AggregateField));
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@CustomFunctionName", CustomFunctionName);
			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblLedgerAggregateColumns WHERE LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid";
			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblLedgerAggregateColumns " + SQLUpdateLock(bInTransaction) +
				  " WHERE " + AppendSiteWhereClause(cmd, security, "tblLedgerAggregateColumns", "LedgerAggregateColumnGuid") +
				  " AND LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid";

			cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", IdentityGuid);
		}


		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblLedgerAggregateColumns " + SQLUpdateLock(bInTransaction) +
				  " WHERE " + AppendSiteWhereClause(cmd, security, "tblLedgerAggregateColumns", "LedgerAggregateColumnGuid") +
				  " AND [ID] = @ID";

			cmd.Parameters.AddWithValue("@ID", _ID);
		}


		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, string findText)
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblLedgerAggregateColumns " +
				" WHERE" + AppendSiteWhereClause(cmd, security, "tblLedgerAggregateColumns", "LedgerAggregateColumnGuid") +
				" AND ID LIKE N'%" + FuelsManagerExtensions.EscapeLikeClauseCharacters(findText.Trim()) + "%'" +
				" ORDER BY tblLedgerAggregateColumns.ID";
		}

	}
	#endregion
}
