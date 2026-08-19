namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	#region TransactionAliasDO Class
	public class LRTransactionAliasDO
	{
		#region Attributes
		private Guid transactionAliasGuid;
		private Guid siteGuid;
		private DateTimeOffset createdDateTime;
		private DateTimeOffset updatedDateTime;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction alias data object.
		/// </summary>
		public LRTransactionAliasDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set and return the Custom Function Name to be
		/// use to perform special math. The default is null;
		/// </summary>
		public string CustomFunctionName { get; set; }

		public string AliasName { get; set; }

		public LRTransactionAliases.TransactionTypes TransactionTypeID { get; set; }

		public Guid TransactionAliasGuid
		{
			get { return this.transactionAliasGuid; }
			set { this.transactionAliasGuid = value; }
		}

		//  NOTE:  this item needs removing since the table has changed to be an index...
		public string SiteOwner { get; set; }

		public bool TwentyFourHr { get; set; }
		public bool DistributedImpact { get; set; }

		public bool Bulk { get; set; }
		public ArrayList UserDataList { get; private set; }

		public string CreatedBy { get; set; }

		public string UpdatedBy { get; set; }
		public DateTimeOffset CreatedDateTime
		{
			get { return this.createdDateTime; }
			set { this.createdDateTime = value; }
		}

		public DateTimeOffset UpdatedDateTime
		{
			get { return this.updatedDateTime; }
			set { this.updatedDateTime = value; }
		}

		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		public bool MultipleLineItems { get; set; }

		public bool LineItemEditControl { get; set; }

		public bool MultipleGaugeReadings { get; set; }

		public bool GaugeReadingEditControl { get; set; }

		/// <summary>
		/// This property returns turn if the alias is an aggregate.
		/// </summary>
		public bool IsAggregateAlias { get; set; }

		/// <summary>
		/// This method will return the Aliases used to aggregate collection.
		/// </summary>
		public ArrayList AliasesToAggregate { get; private set; }

		/// <summary>
		/// This method will return the aggregate Alias Symbols collection.
		/// </summary>
		public ArrayList AliasesToAggregateSymbols { get; private set; }
		#endregion

		#region public methods
		/// <summary>
		/// This method will add aggreated aliases and aggregated alias symbols to this
		/// aggregate column.
		/// </summary>
		/// <param name="aggregateAlias"></param>
		/// <param name="aliasSymbol"></param>
		public void AddAggreateAlias(string aggregateAlias, string aliasSymbol)
		{
			if (!string.IsNullOrEmpty(aggregateAlias))
			{
				this.AliasesToAggregate.Add(aggregateAlias);

				if (string.IsNullOrEmpty(aliasSymbol))
				{
					this.AliasesToAggregateSymbols.Add(string.Empty);
				}
				else
				{
					this.AliasesToAggregateSymbols.Add(aliasSymbol);
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.TransactionTypeID				= LRTransactionAliases.TransactionTypes.T_Maximum;
			this.transactionAliasGuid		= Guid.Empty;
			this.AliasName					= string.Empty;
			this.SiteOwner					= string.Empty; //  NOTE:  this item needs removing since the table has changed to be an index...
			this.siteGuid					= LedgerConstants.SiteDefaultGuid;
			this.TwentyFourHr				= false;
			this.Bulk						= false;
			this.DistributedImpact			= false;
			this.MultipleLineItems			= false;
			this.LineItemEditControl		= false;
			this.MultipleGaugeReadings		= false;
			this.GaugeReadingEditControl	= false;
			this.IsAggregateAlias			= false;
			this.CreatedBy					= string.Empty;
			this.UpdatedBy					= string.Empty;
			this.UserDataList				= null;
			this.AliasesToAggregate			= new ArrayList();
			this.AliasesToAggregateSymbols	= new ArrayList();
			this.CustomFunctionName			= string.Empty;
		}
		#endregion
	}
	#endregion

	#region TransactionAliasListDO
	public class LRTransactionAliasListDO
	{
		#region Attributes
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Transaction Alias List data object.
		/// </summary>
		public LRTransactionAliasListDO()
		{
			this.AliasList = new Dictionary<string, LRTransactionAliasDO>(StringComparer.InvariantCultureIgnoreCase);
			this.AliasSortedList = new SortedList();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Transaction Alias DO for a 
		/// given key (alias name).
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The FMBusinessServices.ServiceClasses.LedgerReportClasses.LRTransactionAliasDO.
		/// </returns>
		public LRTransactionAliasDO this[string key]
		{
			get { return this.AliasList[key]; }
		}

		/// <summary>
		/// Gets and sets the alias sorted list.
		/// </summary>
		public SortedList AliasSortedList { get; set; }

		/// <summary>
		/// Gets and Sets the alias list hashtable.
		/// </summary>
		public Dictionary<string, LRTransactionAliasDO> AliasList { get; set; }

		/// <summary>
		/// Gets the values. This property will return an collections of Values in
		/// aliasList.
		/// </summary>
		public ICollection Values
		{
			get { return this.AliasList.Values; }
		}
		#endregion

		#region SQL Methods

		/// <summary>
		/// This method will query the database for the transaction aliases that are associated
		/// to the site Guid or login site Guid. The method will load the aliases into a 
		/// hash table.
		/// </summary>
		/// <param name="siteGuid">
		/// The site guid.
		/// </param>
		/// <param name="ledgerConnection"></param>
		public void PerformQuery(Guid siteGuid, LedgerConnection ledgerConnection)
		{
            DataSet set;

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.usp_GetTransactionAliasesBySite";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
                set = ledgerConnection.GetDataSet(cmd);
            }

            this.LoadAliasAssignments(set);
		}

		/// <summary>
		/// This method will query the database for the aggreate columns and their
		/// associated aggregate aliases. It will load a hash table of objects containing
		/// the data.
		/// </summary>
		/// <param name="siteGuid">
		/// The site guid.
		/// </param>
		/// <param name="ledgerConnection"></param>
		public void PerformAggregateQuery(Guid siteGuid, LedgerConnection ledgerConnection)
		{
			using (var command = new SqlCommand())
			{
				// Retrieve the transaction aliases for the associated sites.
				command.CommandText = this.GetAggregateColumnsSelectSQL();

				command.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				command.Parameters["@SiteGuid"].Value = siteGuid;

				// Load the retrieve data set.
				this.LoadAggregateColumns(ledgerConnection.GetDataSet(command));
			}
		}

		/// <summary>
		/// This method will return an SQL that retrieves all the aggreate columns and their
		/// associated aggregate aliases.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		private string GetAggregateColumnsSelectSQL()
		{
			const string Select = "SELECT lac.LedgerAggregateColumnGuid, " +
			                       "lac.ID AS AggregateColumnID, " +
			                       "lac.CustomFunctionName, " +
			                       "lacm.symbol AS AggregateColumnSymbol, " +
			                       "lac.SiteGuid AS AggregateColumnSiteGuid, " +
			                       "(SELECT ta.AliasName FROM tblTransactionAliases ta WHERE ta.TransactionAliasGuid = lacm.TransactionAliasGuid) AS AggregatedAliasNames ";
			const string From = "FROM map.tblLedgerAggregateColumnToTransactionAlias lacm LEFT OUTER JOIN " +
			                     "tblLedgerAggregateColumns lac ON lacm.LedgerAggregateColumnGuid = lac.LedgerAggregateColumnGuid ";
			const string Where = "WHERE lac.SiteGuid = @SiteGuid " +
			                      "OR lac.LedgerAggregateColumnGuid IN (SELECT LedgerAggregateColumnGuid  " +
			                      "FROM map.tblEntityLedgerAggregateColumnToSite " +
			                      "WHERE SiteGuid = @SiteGuid)";
			const string OrderBy = "ORDER BY lac.ID";

			return Select + From + Where + OrderBy;
		}
		#endregion

		#region Load Methods
		/// <summary>
		/// This method will load the data object with all the aliases for the
		/// selected site.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		private void LoadAliasAssignments(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					foreach (DataRow row in table.Rows)
					{
						var transAlias = new LRTransactionAliasDO
						{
							TransactionAliasGuid = row.IsNull("TransactionAliasGuid") ? Guid.Empty : (Guid)row["TransactionAliasGuid"],
							AliasName = (row.IsNull("AliasName") ? string.Empty : (string)row["AliasName"]).Trim(),
							SiteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"],
							TwentyFourHr = !row.IsNull("MeterCloseout") && (bool)row["MeterCloseout"],
							Bulk = !row.IsNull("BulkShipment") && (bool)row["BulkShipment"],
							DistributedImpact = !row.IsNull("DistributedImpact") && (bool)row["DistributedImpact"],
							MultipleLineItems = !row.IsNull("MultipleLineItems") && (bool)row["MultipleLineItems"],
							LineItemEditControl = !row.IsNull("LineItemEditControl") && (bool)row["LineItemEditControl"],
							MultipleGaugeReadings = !row.IsNull("MultipleWeightReadings") && (bool)row["MultipleWeightReadings"],
							GaugeReadingEditControl = !row.IsNull("WeightReadingEditControl") && (bool)row["WeightReadingEditControl"]
						};

						string transTypeIdStr = row.IsNull("LookupTransTypeIndex") ? "0" : row["LookupTransTypeIndex"].ToString();
						transAlias.TransactionTypeID = (LRTransactionAliases.TransactionTypes)Convert.ToInt32(transTypeIdStr);

						this.AliasList.Add(transAlias.AliasName, transAlias);
						this.AliasSortedList.Add(transAlias.AliasName, transAlias);
					}
				}
			}
		}

		/// <summary>
		/// This method will load the data object with all the aliases for the
		/// selected site.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		private void LoadAggregateColumns(DataSet dataSet)
		{
			string previousAggregateColumn = string.Empty;

			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					foreach (DataRow row in table.Rows)
					{
						string currentAggregateColumn = (row.IsNull("AggregateColumnID") ? string.Empty : (string)row["AggregateColumnID"]).Trim();

						LRTransactionAliasDO aggregateAliasDo;

						if (previousAggregateColumn.Equals(currentAggregateColumn) == false)
						{
							previousAggregateColumn = currentAggregateColumn;
							aggregateAliasDo = new LRTransactionAliasDO
							{
								IsAggregateAlias = true,
								TransactionTypeID = LRTransactionAliases.TransactionTypes.T_Aggregate,

								AliasName = currentAggregateColumn,
								TransactionAliasGuid = row.IsNull("LedgerAggregateColumnGuid") ? Guid.Empty : (Guid)row["LedgerAggregateColumnGuid"],
								SiteGuid = row.IsNull("AggregateColumnSiteGuid") ? Guid.Empty : (Guid)row["AggregateColumnSiteGuid"],
								CustomFunctionName = row.IsNull("CustomFunctionName") ? string.Empty : (string)row["CustomFunctionName"]
							};
							string aggregateAlias					= (row.IsNull("AggregatedAliasNames") ? string.Empty : (string)row["AggregatedAliasNames"]).Trim();
							string aggregateAliasSymbol				= (row.IsNull("AggregateColumnSymbol") ? string.Empty : (string)row["AggregateColumnSymbol"]).Trim();

							aggregateAliasDo.AddAggreateAlias(aggregateAlias, aggregateAliasSymbol);
							this.AliasList.Add(currentAggregateColumn, aggregateAliasDo);
							this.AliasSortedList.Add(currentAggregateColumn, aggregateAliasDo);
						}
						else
						{
							if (this.AliasList.ContainsKey(currentAggregateColumn) == true)
							{
								string aggregateAlias = (row.IsNull("AggregatedAliasNames") ? string.Empty : (string)row["AggregatedAliasNames"]).Trim();
								string aggregateAliasSymbol = (row.IsNull("AggregateColumnSymbol") ? string.Empty : (string)row["AggregateColumnSymbol"]).Trim();

								aggregateAliasDo = (LRTransactionAliasDO) this.AliasList[currentAggregateColumn];
								aggregateAliasDo.AddAggreateAlias(aggregateAlias, aggregateAliasSymbol);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This methos will return true if the alias name affects
		/// the inventory.  Otherwise, it return false.
		/// </summary>
		/// <param name="aliasName">
		/// The alias name.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool AffectsInventory(string aliasName)
		{
			bool doesAffectInventory = true;
			LRTransactionAliases.TransactionTypes transactionType = this.GetTransactionType(aliasName);

			switch (transactionType)
			{
				case LRTransactionAliases.TransactionTypes.T7FillStand:
				case LRTransactionAliases.TransactionTypes.T9Request:
				case LRTransactionAliases.TransactionTypes.T10Unload:
				case LRTransactionAliases.TransactionTypes.T11ConsumerTransfer:
				case LRTransactionAliases.TransactionTypes.T12Type12:
				case LRTransactionAliases.TransactionTypes.T14PhysicalInventory:
				case LRTransactionAliases.TransactionTypes.T17Order:
				case LRTransactionAliases.TransactionTypes.T18SupplyOrder:
				case LRTransactionAliases.TransactionTypes.T19EndOfDay:
				case LRTransactionAliases.TransactionTypes.T20EndOfMonth:
				case LRTransactionAliases.TransactionTypes.T21AccountPayableInvoice:
				case LRTransactionAliases.TransactionTypes.T22AccountReceivableInvoice:
				case LRTransactionAliases.TransactionTypes.T23StorageTransfer:
				case LRTransactionAliases.TransactionTypes.T_Maximum:
					doesAffectInventory = false;
					break;
			}

			return doesAffectInventory;
		}

		/// <summary>
		/// This method will return true if the alias name is a
		/// physical inventory (type 14). Otherwise, it returns false.
		/// </summary>
		/// <param name="aliasName">
		/// The alias name.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool IsPhysicalInventory(string aliasName)
		{
			LRTransactionAliases.TransactionTypes transactionType = this.GetTransactionType(aliasName);

			return transactionType == LRTransactionAliases.TransactionTypes.T14PhysicalInventory;
		}

		/// <summary>
		/// This method will retrieve the transaction type for a given
		/// alias name.
		/// </summary>
		/// <param name="aliasName">
		/// The alias name.
		/// </param>
		/// <returns>
		/// The FMBusinessServices.ServiceClasses.LedgerReportClasses.LRTransactionAliases+TransactionTypes.
		/// </returns>
		/// <exception cref="Exception">Unexpected data object type exception.
		/// </exception>
		public LRTransactionAliases.TransactionTypes GetTransactionType(string aliasName)
		{
			// There are times when the aliasName passed is not a key found in the aliasList
			// hashtable.  This needs to be accounted for.
			if (this.AliasList.ContainsKey(aliasName) && this.AliasList[aliasName] != null)
			{
				Type type = this.AliasList[aliasName].GetType();

				if (type == typeof(LRTransactionAliasDO))
				{
					var ledgerRecTransactionAliasDo = this.AliasList[aliasName] as LRTransactionAliasDO;

					if ( ledgerRecTransactionAliasDo != null )
					{
						return ledgerRecTransactionAliasDo.TransactionTypeID;
					}
				}
			}
			else
			{
				return LRTransactionAliases.TransactionTypes.T_Maximum;
			}

			throw new Exception("TransactionAliasListDO.getTransactionType() - unexpected data object type.");
		}
		#endregion
	}
	#endregion
}