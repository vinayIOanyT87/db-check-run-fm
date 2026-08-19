/// <summary>
/// File name:	TransactionAliasDO.cs
/// Purpose:	
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			   By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2007-09-18		I.Orndorff				7.3.0.0 - Added new transaction type (T18_SupplyOrder).
///		2007-10-11		Richard Panachida		Due to changes in the transaction alias table this file was not
///												      updated. Corrected the problem in the load method. 
///		2008-12-31     Richard Panachida    Updated the AffectsInventory method with the new transactions that do not
///		                                    affect inventory (defect 613).
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionAliasDO : DataObject
	{
		#region Attributes
		[DataMember] private TransactionTypes transTypeID;
		[DataMember] private Guid transactionAliasGuid;
		[DataMember] private string aliasName;
		[DataMember] private string siteOwner; //  NOTE:  this item needs removing since the table has changed to be an index...
		[DataMember] private Guid siteGuid;
		[DataMember] private bool twentyFourHr;
		[DataMember] private bool bulk;
		[DataMember] private bool distributedImpact;
		[DataMember] private bool includeInDispatch;
		[DataMember] private bool multipleLineItems;
		[DataMember] private bool lineItemEditControl;
		[DataMember] private bool multipleGaugeReadings;
		[DataMember] private bool gaugeReadingEditControl;
		[DataMember] private bool isAggregateAlias;
		[DataMember] private ArrayList userDataList = null;
		[DataMember] private ArrayList aliasesToAggregate;
		[DataMember] private ArrayList aliasesToAggregateSymbols;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		[DataMember] private DateTimeOffset createdDateTime;
		[DataMember] private DateTimeOffset updatedDateTime;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction alias data object class.
		/// </summary>
		public TransactionAliasDO ( )
		{
			this.Init ( );
		}
		#endregion

		#region Properties
		public string AliasName
		{
			get { return this.aliasName; }
			set { this.aliasName = value; }
		}

		public TransactionTypes TransactionTypeID
		{
			get { return this.transTypeID; }
			set { this.transTypeID = value; }
		}
		
		public Guid TransactionAliasGuid
		{
			get { return this.transactionAliasGuid; }
			set { this.transactionAliasGuid = value; }
		}

		//  NOTE:  this item needs removing since the table has changed to be an index...
		public string SiteOwner
		{
			get { return this.siteOwner; }
			set { this.siteOwner = value; }
		}
		
		public bool TwentyFourHr
		{
			get { return this.twentyFourHr; }
			set { this.twentyFourHr = value; }
		}
		
		public bool DistributedImpact
		{
			get { return this.distributedImpact; }
			set { this.distributedImpact = value; }
		}

		/// <summary>
		/// Boolean value which indicates to include this TransactionAlias on the new Dispatch screens.
		/// </summary>
		public bool IncludeInDispatch
		{
			get { return this.includeInDispatch; }
			set { this.includeInDispatch = value; }
		}
		
		public bool Bulk
		{
			get { return this.bulk; }
			set { this.bulk = value; }
		}
		
		public System.Collections.ArrayList UserDataList
		{
			get { return this.userDataList; }
		}
		
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}
		
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}
		
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

		public bool MultipleLineItems
		{
			get { return this.multipleLineItems; }
			set { this.multipleLineItems = value; }
		}

		public bool LineItemEditControl
		{
			get { return this.lineItemEditControl; }
			set { this.lineItemEditControl = value; }
		}

		public bool MultipleGaugeReadings
		{
			get { return this.multipleGaugeReadings; }
			set { this.multipleGaugeReadings = value; }
		}

		public bool GaugeReadingEditControl
		{
			get { return this.gaugeReadingEditControl; }
			set { this.gaugeReadingEditControl = value; }
		}

		/// <summary>
		/// This method will return the Aliases used to aggregate collection.
		/// </summary>
		public ArrayList AliasesToAggregate
		{
			get { return this.aliasesToAggregate; }
			private set { this.aliasesToAggregate = value; }
		}

		/// <summary>
		/// This method will return the aggregate Alias Symbols collection.
		/// </summary>
		public ArrayList AliasesToAggregateSymbols
		{
			get { return this.aliasesToAggregateSymbols; }
			private set { this.aliasesToAggregateSymbols = value; }
		}

		/// <summary>
		/// This property returns turn if the alias is an aggregate.
		/// </summary>
		public bool IsAggregateAlias
		{
			get { return this.isAggregateAlias; }
			set { this.isAggregateAlias = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add aggreated aliases and aggregated alias symbols to this
		/// aggregate column.
		/// </summary>
		/// <param name="aggregateAlias"></param>
		public void AddAggreateAlias ( string aggregateAlias, string aliasSymbol )
		{
			if (( aggregateAlias != null ) && ( aggregateAlias.Length > 0 ))
			{
				this.aliasesToAggregate.Add ( aggregateAlias );

				if (( aliasSymbol == null ) || ( aliasSymbol.Length <= 0 ))
				{
					this.aliasesToAggregateSymbols.Add ( "" );
				}
				else
				{
					this.aliasesToAggregateSymbols.Add ( aliasSymbol );
				}
			}
		}
		#endregion

		#region Override Methods
		override public string getSelectCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getUpdateCommand ( )
		{
			return null;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init ( )
		{
			this.transTypeID				= TransactionTypes.T_Maximum;
			this.transactionAliasGuid	= Guid.Empty;
			this.aliasName					= "";
			this.siteOwner					= ""; //  NOTE:  this item needs removing since the table has changed to be an index...
			this.siteGuid					= Guid.Empty;
			this.twentyFourHr				= false;
			this.bulk						= false;
			this.distributedImpact			= false;
			this.includeInDispatch			= false;
			this.multipleLineItems			= false;
			this.lineItemEditControl		= false;
			this.multipleGaugeReadings		= false;
			this.gaugeReadingEditControl	= false;
			this.isAggregateAlias			= false;
			this.createdBy					= "";
			this.updatedBy					= "";
			this.userDataList				= null;
			this.aliasesToAggregate			= new ArrayList ( );
			this.aliasesToAggregateSymbols	= new ArrayList ( );
		}
		#endregion
	}

   [Serializable]
   [DataContract]
	[KnownType(typeof(TransactionAliasDO))]
	[KnownType(typeof(TransactionAliasClass))]
	public class TransactionAliasListDO : DataObject
	{
		#region Attributes
		[DataMember]
		public Hashtable aliasList;
		#endregion

		#region Constructor
		public TransactionAliasListDO ( )
		{
			aliasList = new Hashtable ( );
		}

		public TransactionAliasListDO ( DataSet ds )
		{
			this.aliasList = new Hashtable ( );
			init ( ds );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Transaction Alias DO for a 
		/// given key (alias name).
		/// </summary>
		public TransactionAliasDO this[string key]
		{
			get { return (TransactionAliasDO) this.aliasList[key]; }
		}

		/// <summary>
		/// This property will return an collections of Values in
		/// aliasList.
		/// </summary>
		public ICollection Values
		{
			get { return this.aliasList.Values; }
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will populate a SqlCommand with SQL that retrieves all the aliases that are assigned
		/// to the given site with the exception of the ones that we have already retreived
		/// from the site alias owner query.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="siteName"></param>
		/// <param name="existingAliases"></param>
		/// <returns></returns>
		public void getAliasAssignmentsSelectSQL(SqlCommand cmd, Guid groupSiteGuid, Guid siteGuid)
		{
			TransactionAliasClass TransactionAlias = new TransactionAliasClass ( );
			SecurityClass Security = new SecurityClass ( );
			Security.LoginSiteGuid = groupSiteGuid;
			Security.SiteGuid = siteGuid;

			TransactionAlias.EnumerateSQL ( cmd, Security );
		}


		/// <summary>
		/// This method will query the database for the aggreate columns and their
		/// associated aggregate aliases. It will load a hash table of objects containing
		/// the data.
		/// </summary>
		/// <param name="siteGuid"></param>
		public void PerformAggregateQuery(SqlCommand cmd, Guid siteGuid)
		{
			// Retrieve the transaction aliases for the associated sites.
			cmd.CommandText = this.GetAggregateColumnsSelectSQL ( siteGuid );

			cmd.Parameters.Add("@SiteGuid", System.Data.SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = siteGuid;
		}

		/// <summary>
		/// This method will return an SQL that retrieves all the aggreate columns and their
		/// associated aggregate aliases.
		/// </summary>
		/// <param name="siteGuid"></param>
		/// <returns></returns>
		private string GetAggregateColumnsSelectSQL ( Guid siteGuid )
		{
			string select = "SELECT lac.LedgerAggregateColumnGuid, " +
							 "lac.ID AS AggregateColumnID, " +
							 "lacm.symbol AS AggregateColumnSymbol, " +
							 "lac.SiteGuid AS AggregateColumnSiteGuid, " +
							 "(SELECT ta.AliasName FROM tblTransactionAliases ta WHERE ta.TransactionAliasGuid = lacm.TransactionAliasGuid) AS AggregatedAliasNames ";
			string from = "FROM map.tblLedgerAggregateColumnToTransactionAlias lacm LEFT OUTER JOIN " +
							 "tblLedgerAggregateColumns lac ON lacm.LedgerAggregateColumnGuid = lac.LedgerAggregateColumnGuid ";
			string where = "WHERE lac.SiteGuid = @SiteGuid " +
									"OR lac.LedgerAggregateColumnGuid IN (SELECT LedgerAggregateColumnGuid " +
														"FROM map.tblEntityLedgerAggregateColumnToSite " +
													   "WHERE SiteGuid = @SiteGuid)";
			string orderBy = "ORDER BY lac.ID";

			string sql = select + from + where + orderBy;

			return sql;
		}
		#endregion

		#region Load Methods
		/// <summary>
		/// This method will load the transaction alias DO with the following info:
		/// alias name, trans type ID, 24 hour, distributed impact, bulk shipments,
		/// and build a collections.
		/// </summary>
		/// <param name="ds"></param>
		private void init ( System.Data.DataSet ds )
		{
			System.Data.DataTable table = ds.Tables[0];
			foreach (System.Data.DataRow row in table.Rows)
			{
				TransactionAliasDO alias = new TransactionAliasDO ( );

				alias.AliasName = DataObject.getValue<string>(row["AliasName"], "").Trim();
				alias.TransactionTypeID = DataObject.getValue<TransactionTypes>(row["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
				alias.TwentyFourHr = DataObject.getValue<bool>(row["MeterCloseout"], false);
				alias.DistributedImpact = DataObject.getValue<bool>(row["DistributedImpact"], false);
				alias.IncludeInDispatch = DataObject.getValue<bool>(row["IncludeInDispatch"], false);
				alias.Bulk = DataObject.getValue<bool>(row["BulkShipment"], false);
				alias.LineItemEditControl = DataObject.getValue<bool>(row["LineItemEditControl"], false);
				alias.MultipleLineItems = DataObject.getValue<bool>(row["MultipleLineItems"], false);
				alias.GaugeReadingEditControl = DataObject.getValue<bool>(row["WeightReadingEditControl"], false);
				alias.MultipleGaugeReadings = DataObject.getValue<bool>(row["MultipleWeightReadings"], false);

				aliasList.Add ( alias.AliasName, alias );
			}
		}


		/// <summary>
		/// This method will load the data object with all the aliases for the
		/// selected site.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadAliasAssignments ( DataSet dataSet )
		{
			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					foreach (DataRow row in table.Rows)
					{
						TransactionAliasDO transAlias = new TransactionAliasDO ( );


						transAlias.TransactionAliasGuid = DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
						transAlias.TransactionTypeID = DataObject.getValue<TransactionTypes>(row["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
						transAlias.AliasName = DataObject.getValue<string>(row["AliasName"], "").Trim();
						transAlias.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
						transAlias.TwentyFourHr = DataObject.getValue<bool>(row["MeterCloseout"], false);
						transAlias.Bulk = DataObject.getValue<bool>(row["BulkShipment"], false);
						transAlias.DistributedImpact = DataObject.getValue<bool>(row["DistributedImpact"], false);
						transAlias.IncludeInDispatch = DataObject.getValue<bool>(row["IncludeInDispatch"], false);
						transAlias.MultipleLineItems = DataObject.getValue<bool>(row["MultipleLineItems"], false);
						transAlias.LineItemEditControl = DataObject.getValue<bool>(row["LineItemEditControl"], false);
						transAlias.MultipleGaugeReadings = DataObject.getValue<bool>(row["MultipleWeightReadings"], false);
						transAlias.GaugeReadingEditControl = DataObject.getValue<bool>(row["WeightReadingEditControl"], false);
						transAlias.CreatedDateTime = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
						transAlias.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
						transAlias.UpdatedDateTime = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], transAlias.CreatedDateTime);
						transAlias.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);

						this.aliasList.Add ( transAlias.AliasName, transAlias );
					}
				}
			}
		}

		/// <summary>
		/// This method will load the data object with all the aliases for the
		/// selected site.
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadAggregateColumns ( DataSet dataSet )
		{
			string previousAggregateColumn = "";
			TransactionAliasDO aggregateAliasDO = null;

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					foreach (DataRow row in table.Rows)
					{
						string currentAggregateColumn = DataObject.getValue<string>(row["AggregateColumnID"], "").Trim();

						if (previousAggregateColumn.Equals ( currentAggregateColumn ) == false)
						{
							previousAggregateColumn = currentAggregateColumn;
							aggregateAliasDO = new TransactionAliasDO ( );

							aggregateAliasDO.IsAggregateAlias = true;
							aggregateAliasDO.TransactionTypeID = TransactionTypes.T24_Aggregate;

							aggregateAliasDO.AliasName	= currentAggregateColumn;
							aggregateAliasDO.TransactionAliasGuid = DataObject.getValue<Guid>(row["LedgerAggregateColumnGuid"], Guid.Empty);
							aggregateAliasDO.SiteGuid	= DataObject.getValue<Guid>(row["AggregateColumnSiteGuid"], Guid.Empty);
							string aggregateAlias		= DataObject.getValue<string>(row["AggregatedAliasNames"], "").Trim();
							string aggregateAliasSymbol = DataObject.getValue<string>(row["AggregateColumnSymbol"], "").Trim();

							aggregateAliasDO.AddAggreateAlias ( aggregateAlias, aggregateAliasSymbol );
							this.aliasList.Add ( currentAggregateColumn, aggregateAliasDO );
						}
						else
						{
							if (this.aliasList.Contains ( currentAggregateColumn ) == true)
							{
								string aggregateAlias = DataObject.getValue<string>(row["AggregatedAliasNames"], "").Trim();
								string aggregateAliasSymbol = DataObject.getValue<string>(row["AggregateColumnSymbol"], "").Trim();

								aggregateAliasDO = (TransactionAliasDO) this.aliasList[currentAggregateColumn];
								aggregateAliasDO.AddAggreateAlias ( aggregateAlias, aggregateAliasSymbol );
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
		/// <param name="aliasName"></param>
		/// <returns></returns>
		public bool AffectsInventory ( string aliasName )
		{
			bool doesAffectInventory = true;
			TransactionTypes transactionType = getTransactionType ( aliasName );

			switch (transactionType)
			{
				case TransactionTypes.T7_FillStand:
				case TransactionTypes.T9_Request:
				case TransactionTypes.T10_Unload:
				case TransactionTypes.T11_ConsumerTransfer:
				case TransactionTypes.T12_InventoryNotAffected:
				case TransactionTypes.T14_PhysicalInventory:
				case TransactionTypes.T17_Order:
				case TransactionTypes.T18_SupplyOrder:
				case TransactionTypes.T19_EndOfDay:
				case TransactionTypes.T20_EndOfMonth:
				case TransactionTypes.T21_AccountPayableInvoice:
				case TransactionTypes.T22_AccountReceivableInvoice:
				case TransactionTypes.T23_StorageTransfer:
					doesAffectInventory = false;
					break;
			}

			return doesAffectInventory;
		}

		/// <summary>
		/// This method will return true if the alias name is a
		/// physical inventory (type 14). Otherwise, it returns false.
		/// </summary>
		/// <param name="aliasName"></param>
		/// <returns></returns>
		public bool IsPhysicalInventory ( string aliasName )
		{
			TransactionTypes transactionType = getTransactionType ( aliasName );
			if (transactionType == TransactionTypes.T14_PhysicalInventory)
				return true;

			return false;
		}

		/// <summary>
		/// This method will retrieve the transaction type for a given
		/// alias name.
		/// </summary>
		/// <param name="aliasName"></param>
		/// <returns></returns>
		public TransactionTypes getTransactionType ( string aliasName )
		{
			// vthompson 11/25/2008
			// There are times when the aliasName passed is not a key found in the aliasList
			// hashtable.  This needs to be accounted for.
			if (aliasList[aliasName] != null)
			{
				Type type = aliasList[aliasName].GetType ( );

				if (type == typeof ( TransactionAliasDO ))
				{
					return ( aliasList[aliasName] as TransactionAliasDO ).TransactionTypeID;
				}

				if (type == typeof ( TransactionAliasClass ))
				{
					return ( aliasList[aliasName] as TransactionAliasClass ).TransTypeID;
				}
			}

			throw new Exception ( "TransactionAliasListDO.getTransactionType() - unexpected data object type." );
		}
		#endregion

		#region Override Methods
		override public string getSelectCommand ( )
		{
            return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getUpdateCommand ( )
		{
			return null;
		}
		#endregion
	}
}
