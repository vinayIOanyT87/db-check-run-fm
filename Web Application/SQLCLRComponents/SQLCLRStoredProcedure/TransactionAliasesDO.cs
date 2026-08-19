/// <summary>
///   File name:	TransactionAliasDO.cs
///   Purpose:	
///   
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):  Richard Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///   Date:			   By:						   Reason:
///   ----------		--------------------	   --------------------------------------------
///   yyyy-mm-dd		Coder's name				Reason for the change.
///   
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Data.SqlClient;

#region TransactionAliasDO Class
[System.Serializable]
public class TransactionAliasDO
{
   #region Attributes
   private TransactionAliases.TransactionTypes transTypeID;
   private int    aliasID;
   private string aliasName;
   private string siteOwner; //  NOTE:  this item needs removing since the table has changed to be an index...
   private int    siteIndex;
   private bool   twentyFourHr;
   private bool   bulk;
   private bool   distributedImpact;
   private bool   multipleLineItems;
   private bool   lineItemEditControl;
   private bool   multipleGaugeReadings;
   private bool   gaugeReadingEditControl;
   private bool   isAggregateAlias;
   private string createdBy;
   private string updatedBy;
   private string customFunctionName;

   private DateTime  createdDateTime;
   private DateTime  updatedDateTime;
   private ArrayList userDataList = null;
   private ArrayList aliasesToAggregate;
   private ArrayList aliasesToAggregateSymbols;
   #endregion

   #region Constructors
   /// <summary>
   /// This is the default constructor for the transaction alias data object.
   /// </summary>
   public TransactionAliasDO()
   {
      this.Init();
   }
   #endregion

   #region Properties
   /// <summary>
   /// This property will set and return the Custom Function Name to be
   /// use to perform special math. The default is null;
   /// </summary>
   public string CustomFunctionName
   {
      get { return this.customFunctionName; }
      set { this.customFunctionName = value; }
   }

   public string AliasName
   {
      get { return this.aliasName; }
      set { this.aliasName = value; }
   }
   public TransactionAliases.TransactionTypes TransactionTypeID
   {
      get { return this.transTypeID; }
      set { this.transTypeID = value; }
   }
   public int AliasID
   {
      get { return this.aliasID; }
      set { this.aliasID = value; }
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
   public DateTime CreatedDateTime
   {
      get { return this.createdDateTime; }
      set { this.createdDateTime = value; }
   }
   public DateTime UpdatedDateTime
   {
      get { return this.updatedDateTime; }
      set { this.updatedDateTime = value; }
   }

   public int SiteIndex
   {
      get { return this.siteIndex; }
      set { this.siteIndex = value; }
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
   /// This property returns turn if the alias is an aggregate.
   /// </summary>
   public bool IsAggregateAlias
   {
      get { return this.isAggregateAlias; }
      set { this.isAggregateAlias = value; }
   }

   /// <summary>
   /// This method will return the Aliases used to aggregate collection.
   /// </summary>
   public ArrayList AliasesToAggregate
   {
      get { return this.aliasesToAggregate; }
   }

   /// <summary>
   /// This method will return the aggregate Alias Symbols collection.
   /// </summary>
   public ArrayList AliasesToAggregateSymbols
   {
      get { return this.aliasesToAggregateSymbols; }
   }
   #endregion

   #region public methods
   /// <summary>
   /// This method will add aggreated aliases and aggregated alias symbols to this
   /// aggregate column.
   /// </summary>
   /// <param name="aggregateAlias"></param>
   public void AddAggreateAlias(string aggregateAlias, string aliasSymbol)
   {
      if ((aggregateAlias != null) && (aggregateAlias.Length > 0))
      {
         this.aliasesToAggregate.Add(aggregateAlias);

         if ((aliasSymbol == null) || (aliasSymbol.Length <= 0))
         {
            this.aliasesToAggregateSymbols.Add("");
         }
         else
         {
            this.aliasesToAggregateSymbols.Add(aliasSymbol);
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
      this.transTypeID               = TransactionAliases.TransactionTypes.T_Maximum;
      this.aliasID                   = -1;
      this.aliasName                 = "";
      this.siteOwner                 = ""; //  NOTE:  this item needs removing since the table has changed to be an index...
      this.siteIndex                 = -99;
      this.twentyFourHr              = false;
      this.bulk                      = false;
      this.distributedImpact         = false;
      this.multipleLineItems         = false;
      this.lineItemEditControl       = false;
      this.multipleGaugeReadings     = false;
      this.gaugeReadingEditControl   = false;
      this.isAggregateAlias          = false;
      this.createdBy                 = "";
      this.updatedBy                 = "";
      this.userDataList              = null;
      this.aliasesToAggregate        = new ArrayList();
      this.aliasesToAggregateSymbols = new ArrayList();
      this.customFunctionName        = "";
   }
   #endregion
}
#endregion

#region TransactionAliasListDO
[System.Serializable]
public class TransactionAliasListDO
{
   #region Attributes
   public Hashtable  aliasList;
   public SortedList aliasSortedList;
   #endregion

   #region Constructor
   /// <summary>
   /// This is the default constructor for the Transaction Alias List data object.
   /// </summary>
   public TransactionAliasListDO()
   {
      this.aliasList       = new Hashtable();
      this.aliasSortedList = new SortedList();
   }
   #endregion

   #region Properties
   /// <summary>
   /// This property will return the Transaction Alias DO for a 
   /// given key (alias name).
   /// </summary>
   public TransactionAliasDO this[string key]
   {
      get { return (TransactionAliasDO) aliasList[key]; }
   }

   /// <summary>
   /// This property will return an collections of Values in
   /// aliasList.
   /// </summary>
   public System.Collections.ICollection Values
   {
      get { return this.aliasList.Values; }
   }
   #endregion

   #region SQL Methods
   /// <summary>
   /// This method will query the database for the transaction aliases that are associated
   /// to the site index or login site index. The method will load the aliases into a 
   /// hash table.
   /// </summary>
   /// <param name="siteIndex"></param>
   /// <param name="loginSiteIndex"></param>
   public void PerformQuery(SqlConnection a_connection, int siteIndex, int loginSiteIndex)
   {
      DataSet dataSet = new DataSet();

      // Retrieve the transaction aliases for the associated sites.
      string sql = this.GetAliasAssignmentsSelectSQL(siteIndex, loginSiteIndex);

      //connection.Open();
      SqlCommand command = new SqlCommand(sql, a_connection);

      command.Parameters.Add("@SiteIndex",      System.Data.SqlDbType.Int);
      command.Parameters.Add("@LoginSiteIndex", System.Data.SqlDbType.Int);

      command.Parameters["@SiteIndex"].Value      = siteIndex;
      command.Parameters["@LoginSiteIndex"].Value = loginSiteIndex;

      command.Prepare();

      SqlDataAdapter adapter = new SqlDataAdapter(command);
      adapter.Fill(dataSet);

      // Load the retrieve data set.
      this.LoadAliasAssignments(dataSet);
   }

   /// <summary>
   /// This method will return an SQL that retrieves all the aliases that are assigned
   /// to the given site with the exception of the ones that we have already retreived
   /// from the site alias owner query.
   /// </summary>
   /// <param name="siteIndex"></param>
   /// <param name="loginSiteIndex"></param>
   /// <returns></returns>
   private string GetAliasAssignmentsSelectSQL(int siteIndex, int loginSiteIndex)
   {
      string sql     = "";
		string select = "SELECT tblTransactionAliases.*, (SELECT A.AliasName FROM tblTransactionAliases A WITH(NOLOCK) " +
                      "WHERE A.AliasID = tblTransactionAliases.AssociatedAliasID) AS AssociatedAlias ";
		string from = " FROM tblTransactionAliases WITH(NOLOCK)";
		string where = " WHERE (tblTransactionAliases.AliasID IN (SELECT [Index] FROM tblEntityToSiteMap WITH(NOLOCK) " +
                       "WHERE TypeID = 'Transaction Aliases' AND SiteIndex = @SiteIndex)";
      string orderBy = " ORDER BY tblTransactionAliases.AliasName";

      if (siteIndex == loginSiteIndex)
      {
         sql += select + from + where + ")" + orderBy;
      }
      else
      {
         sql += select + from + where +
                " AND (tblTransactionAliases.SiteIndex = @SiteIndex OR tblTransactionAliases.AliasID IN " +
                "(SELECT [Index] FROM tblEntityToSiteMap WHERE TypeID = 'Transaction Aliases' AND SiteIndex = @LoginSiteIndex)))";
         sql += orderBy;
      }

      return sql;
   }

   /// <summary>
   /// This method will query the database for the aggreate columns and their
   /// associated aggregate aliases. It will load a hash table of objects containing
   /// the data.
   /// </summary>
   /// <param name="siteIndex"></param>
   /// <param name="loginSiteIndex"></param>
   public void PerformAggregateQuery(SqlConnection connection, int siteIndex, int loginSiteIndex)
   {
      DataSet dataSet = new DataSet();

      // Retrieve the transaction aliases for the associated sites.
      string sql = this.GetAggregateColumnsSelectSQL(siteIndex, loginSiteIndex);

      //connection.Open();
      SqlCommand command = new SqlCommand(sql, connection);

      command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
      //command.Parameters.Add("@LoginSiteIndex", System.Data.SqlDbType.Int);

      command.Parameters["@SiteIndex"].Value = siteIndex;
      //command.Parameters["@LoginSiteIndex"].Value = loginSiteIndex;

      command.Prepare();

      SqlDataAdapter adapter = new SqlDataAdapter(command);
      adapter.Fill(dataSet);

      // Load the retrieve data set.
      this.LoadAggregateColumns(dataSet);
   }

   /// <summary>
   /// This method will return an SQL that retrieves all the aggreate columns and their
   /// associated aggregate aliases.
   /// </summary>
   /// <param name="siteIndex"></param>
   /// <param name="loginSiteIndex"></param>
   /// <returns></returns>
   private string GetAggregateColumnsSelectSQL(int siteIndex, int loginSiteIndex)
   {
      string select = "SELECT lac.[Index] AS AggregateColumnIndex, " +
                       "lac.ID AS AggregateColumnID, " +
                       "lac.CustomFunctionName, " +
                       "lacm.symbol AS AggregateColumnSymbol, " +
                       "lac.SiteIndex AS AggregateColumnSiteIndex, " +
                       "(SELECT ta.AliasName FROM tblTransactionAliases ta WHERE ta.AliasID = lacm.AliasIndex) AS AggregatedAliasNames ";
      string from = "FROM tblLedgerAggregateColumnMap lacm LEFT OUTER JOIN " +
                       "tblLedgerAggregateColumns lac ON lacm.ColumnIndex = lac.[Index] ";
      string where = "WHERE lac.SiteIndex = @SiteIndex " +
                           "OR lac.[Index] IN (SELECT [Index] AS AggregateColumnIndex " +
                                               "FROM tblEntityToSiteMap " +
                                               "WHERE TypeID = 'Ledger Aggregate Column' AND SiteIndex = @SiteIndex)";
      string orderBy = "ORDER BY lac.ID";

      string sql = select + from + where + orderBy;

      return sql;
   }
   #endregion

   #region Load Methods
   /// <summary>
   /// This method will load the data object with all the aliases for the
   /// selected site.
   /// </summary>
   /// <param name="dataSet"></param>
   private void LoadAliasAssignments(DataSet dataSet)
   {
      if (dataSet != null)
      {
         System.Data.DataTable table = dataSet.Tables[0];

         if (table.Rows.Count > 0)
         {
            foreach (DataRow row in table.Rows)
            {
               TransactionAliasDO transAlias = new TransactionAliasDO();

               transAlias.AliasID                 = (row.IsNull("AliasID"))                  ? 0     : (int) row["AliasID"];
               transAlias.AliasName               = ((row.IsNull("AliasName"))               ? ""    : (string) row["AliasName"]).Trim();
               transAlias.SiteIndex               = (row.IsNull("SiteIndex"))                ? 0     : (int) row["SiteIndex"];
               transAlias.TwentyFourHr            = (row.IsNull("MeterCloseout"))            ? false : (bool) row["MeterCloseout"];
               transAlias.Bulk                    = (row.IsNull("BulkShipment"))             ? false : (bool) row["BulkShipment"];
               transAlias.DistributedImpact       = (row.IsNull("DistributedImpact"))        ? false : (bool) row["DistributedImpact"];
               transAlias.MultipleLineItems       = (row.IsNull("MultipleLineItems"))        ? false : (bool) row["MultipleLineItems"];
               transAlias.LineItemEditControl     = (row.IsNull("LineItemEditControl"))      ? false : (bool) row["LineItemEditControl"];
               transAlias.MultipleGaugeReadings   = (row.IsNull("MultipleWeightReadings"))   ? false : (bool) row["MultipleWeightReadings"];
               transAlias.GaugeReadingEditControl = (row.IsNull("WeightReadingEditControl")) ? false : (bool) row["WeightReadingEditControl"];

               string transTypeIDStr        = (row.IsNull("TransTypeID")) ? "0" : (row["TransTypeID"]).ToString();
               transAlias.TransactionTypeID = (TransactionAliases.TransactionTypes) Convert.ToInt32(transTypeIDStr);

               this.aliasList.Add(transAlias.AliasName, transAlias);
               this.aliasSortedList.Add(transAlias.AliasName, transAlias);
            }
         }
      }
   }

   /// <summary>
   /// This method will load the data object with all the aliases for the
   /// selected site.
   /// </summary>
   /// <param name="dataSet"></param>
   private void LoadAggregateColumns(DataSet dataSet)
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
               string currentAggregateColumn = ((row.IsNull("AggregateColumnID")) ? "" : (string)row["AggregateColumnID"]).Trim();

               if (previousAggregateColumn.Equals(currentAggregateColumn) == false)
               {
                  previousAggregateColumn = currentAggregateColumn;
                  aggregateAliasDO        = new TransactionAliasDO();

                  aggregateAliasDO.IsAggregateAlias  = true;
                  aggregateAliasDO.TransactionTypeID = TransactionAliases.TransactionTypes.T_Aggregate;

                  aggregateAliasDO.AliasName          = currentAggregateColumn;
                  aggregateAliasDO.AliasID            = (row.IsNull("AggregateColumnIndex"))     ? 0  : (int)    row["AggregateColumnIndex"];
                  aggregateAliasDO.SiteIndex          = (row.IsNull("AggregateColumnSiteIndex")) ? 0  : (int)    row["AggregateColumnSiteIndex"];
                  aggregateAliasDO.CustomFunctionName = (row.IsNull("CustomFunctionName"))       ? "" : (string) row["CustomFunctionName"];
                  string aggregateAlias               = ((row.IsNull("AggregatedAliasNames"))    ? "" : (string) row["AggregatedAliasNames"]).Trim();
                  string aggregateAliasSymbol         = ((row.IsNull("AggregateColumnSymbol"))   ? "" : (string) row["AggregateColumnSymbol"]).Trim();

                  aggregateAliasDO.AddAggreateAlias(aggregateAlias, aggregateAliasSymbol);
                  this.aliasList.Add(currentAggregateColumn, aggregateAliasDO);
                  this.aliasSortedList.Add(currentAggregateColumn, aggregateAliasDO);
               }
               else
               {
                  if (this.aliasList.Contains(currentAggregateColumn) == true)
                  {
                     string aggregateAlias       = ((row.IsNull("AggregatedAliasNames"))  ? "" : (string) row["AggregatedAliasNames"]).Trim();
                     string aggregateAliasSymbol = ((row.IsNull("AggregateColumnSymbol")) ? "" : (string) row["AggregateColumnSymbol"]).Trim();

                     aggregateAliasDO = (TransactionAliasDO) this.aliasList[currentAggregateColumn];
                     aggregateAliasDO.AddAggreateAlias(aggregateAlias, aggregateAliasSymbol);
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
   public bool AffectsInventory(string aliasName)
   {
      bool doesAffectInventory = true;
      TransactionAliases.TransactionTypes transactionType = this.GetTransactionType(aliasName);

      switch (transactionType)
      {
         case TransactionAliases.TransactionTypes.T7_FillStand:
         case TransactionAliases.TransactionTypes.T9_Request:
         case TransactionAliases.TransactionTypes.T10_Unload:
         case TransactionAliases.TransactionTypes.T11_ConsumerTransfer:
         case TransactionAliases.TransactionTypes.T12_Type12:
         case TransactionAliases.TransactionTypes.T14_PhysicalInventory:
         case TransactionAliases.TransactionTypes.T17_Order:
         case TransactionAliases.TransactionTypes.T18_SupplyOrder:
         case TransactionAliases.TransactionTypes.T19_EndOfDay:
         case TransactionAliases.TransactionTypes.T20_EndOfMonth:
         case TransactionAliases.TransactionTypes.T21_AccountPayableInvoice:
         case TransactionAliases.TransactionTypes.T22_AccountReceivableInvoice:
         case TransactionAliases.TransactionTypes.T23_StorageTransfer:
         case TransactionAliases.TransactionTypes.T_Maximum:
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
   public bool IsPhysicalInventory(string aliasName)
   {
      TransactionAliases.TransactionTypes transactionType = this.GetTransactionType(aliasName);

      if (transactionType == TransactionAliases.TransactionTypes.T14_PhysicalInventory)
      {
         return true;
      }

      return false;
   }

   /// <summary>
   /// This method will retrieve the transaction type for a given
   /// alias name.
   /// </summary>
   /// <param name="aliasName"></param>
   /// <returns></returns>
   public TransactionAliases.TransactionTypes GetTransactionType(string aliasName)
   {
      // There are times when the aliasName passed is not a key found in the aliasList
      // hashtable.  This needs to be accounted for.
      if ((this.aliasList.Contains(aliasName) == true) && (this.aliasList[aliasName] != null))
      {
         Type type = this.aliasList[aliasName].GetType();

         if (type == typeof(TransactionAliasDO))
         {
            return (this.aliasList[aliasName] as TransactionAliasDO).TransactionTypeID;
         }
      }
      else
      {
         return TransactionAliases.TransactionTypes.T_Maximum;
      }

      throw new Exception("TransactionAliasListDO.getTransactionType() - unexpected data object type.");
   }
   #endregion
}
#endregion