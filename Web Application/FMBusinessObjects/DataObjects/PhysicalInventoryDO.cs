/// <summary>
/// File name:	PhysicalInventoryDO.cs
/// Purpose:	The purpose of the physical inventory data object is load all the data
///				from the database that includes the most recent physical inventory
///				or closeout prior to the month that is displayed on the ledger.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			   By:						Reason:
///		----------		--------------------	----------------------------------
///		2007-03-30		Richard Panachida		Added code to the line item to indicated if the 
///												      line item has a physical inventory (CSI 4077).
///												      Fixed a defect in the query where clause that 
///												      caused an error if manager, owner, or product was null.
///
///		2007-09-18		I.Orndorff				7.3.0.0 - Added new transaction type (T18_SupplyOrder). 
///
///		2007-10-03		Richard Panachida		Modified code for ADF pricing on the ledger.
///
///		2008-12-31     Richard Panachida    Updated the loadInventoryData method with the new transactions that do not
///		                                    affect inventory (defect 613).
///
///		2009-03-11		W.Gray					Modified loadInventoryData to eliminate CloseoutDate
///		
///      2009-03-19     G.Kendall            WI# 1416 - Change physical inventory date queries to improve performance.
///
///		2009-07-06		W.Gray					7.4.6.0 - Revised getStartingSearchPointSelectSQL,
///														to use NOLOCK (CSI 4581)													
///		
///														
///		17-Feb-2010		B. Schaal				Implemented use of getStartingSearchPointSelectSQL and constrained
///														database request for closeout to improve performance and stop timeout error.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class PhysicalInventoryDO : DataObject
	{
		#region Private Attributes
		[DataMember]
		private LedgerLineItemCollection lineItemsList;
		[DataMember]
		private string startSeachingDate;
		[DataMember]
		private string firstLedgerDate;
		[DataMember]
		private bool noRecordsPrior;
		[DataMember]
		private bool noPhysicalInventory;

		private const int EMPTY_STRING = 0;

		[DataMember]
		private string firstDateCalculation;
		[DataMember]
		private double startingGrossQuantity;
		[DataMember]
		private double startingNetQuantity;
		[DataMember]
		private double startingGrossPrice;
		[DataMember]
		private double startingNetPrice;
		#endregion

		#region Contructor
		/// <summary>
		/// This is the default constructor for the physical inventory
		/// data object.
		/// </summary>
		public PhysicalInventoryDO()
		{
			this.init();
		}
		#endregion

		#region SQL Public methods
		/// <summary>
		/// This method will return an SQL statement that retrieves the summation of the gross
		/// and net volumes for a given inventory date, manager, product and site.
		/// </summary>
		/// <param name="ledgerSR"></param>
		/// <param name="siteName"></param>
		/// <returns></returns>
		public string getOneDaysPhysicalInventorySummationSelectSQL(LedgerSR ledgerSR, string siteName, string manager,
			string product, Guid loginSiteGuid, Guid siteGuid, Guid userGuid, string factor, string precision, Guid productGuid, Guid managerGuid)
		{

			string sql = "EXEC [usp_getOneDaysPhysicalInventorySummationSelect] " +
				  siteGuid + ",'" + firstDateCalculation + "'," + managerGuid + "," + productGuid + "," + factor + "," + precision;

			return sql;
		}

		/// <summary>
		/// This method will return a SQL string that searches the database for the first
		/// record prior to the first day of the requested Ledger month.  For example:
		/// Ledger month is set to February, then the first record prior to that would be
		/// the last record with a date of January 31st (2003-01-31 23:23:00).
		/// </summary>
		/// <param name="ledgerSR"></param>
		/// <returns></returns>
		public string getStartingSearchPointSelectSQL(LedgerSR ledgerSR)
		{
			int deleted = 0;
			this.firstLedgerDate = ledgerSR.GetLedgerStartDate();

			string sql = "SELECT MAX(InventoryDate) " +
						 "FROM tblTransactions WITH(NOLOCK)" +
						 "WHERE InventoryDate < '" + this.firstLedgerDate + "' AND DeleteFlag = " + deleted;

			return sql;
		}

		/// <summary>
		/// This method will return a SQL string that searches the database for the latest
		/// physical inventory record prior to the first day of the requested 
		/// ledger month. 
		/// </summary>
		/// <returns></returns>
		public string getLatestPhysicalInventoryRecordSelectSQL(LedgerSR ledgerSR, string siteName,
			Guid loginSiteGuid, Guid siteGuid, Guid userGuid, Guid productGuid, Guid managerGuid)
		{
			string sql = "EXEC usp_GetLatestPhysicalInventoryRecordSelectTimeSpan '" + siteGuid.ToString() + "','" + firstLedgerDate + "','" + startSeachingDate + "'," + managerGuid + "," + productGuid;
			return sql;
		}

		/// <summary>
		/// This method is used to retreive a SQL string that will get any records
		/// that has an error associated with the inventory. It contains a sub-query
		/// to only look for records for a given inventory date range, manager, product,
		/// and owner.
		/// </summary>
		/// <returns></returns>
		public string getTransactionErrorSelectSQL(LedgerSR ledgerSR, string siteName, Guid loginSiteGuid, Guid siteGuid, Guid userGuid)
		{
			ArrayList siteList = ledgerSR.SiteList;
			int deleted = 0;

			// This is the main select to find any errors associated with a inventory
			// record.
			string select = "SELECT DISTINCT t.AliasName, CONVERT(char(10), t.InventoryDate, 111) AS Inv, r.ErrorStatus ";
			string from = "FROM tblTransactions t, tblB2BResults r, tblTransactionLineItems l ";
			string where = "WHERE t.TransID = r.TransID AND r.ErrorStatus > 0 AND t.DeleteFlag = " + deleted +
				" AND t.InventoryDate <= '" + ledgerSR.GetLedgerEndDate() + "'" +
				" AND l.TransactionInventoryDate <= '" + ledgerSR.GetLedgerEndDate() +
								"' AND t.LookupTransTypeIndex NOT IN (" +
								(int)TransactionTypes.T10_Unload + ", " + (int)TransactionTypes.T11_ConsumerTransfer +
								", " + (int)TransactionTypes.T14_PhysicalInventory + ") AND t.Site = '" +
				siteName + "' ";
			string orderBy = "ORDER BY 2 ";

			// If no physical inventory or closeout date present, then the query will
			// have to start at the beginning of the data.
			if (this.noPhysicalInventory == false)
				where = where + " AND t.InventoryDate >= '" + this.firstDateCalculation + "' ";

			// Get the "where" clause that filters on the user's permission to view
			// transaction data. Pass in the table name. It may return an empty string.
			where = where + PhysicalInventoryDO.GetUserToCompanyWhereClauseOld("t", loginSiteGuid, siteGuid, userGuid);

			// Add the Manager, Owner, and Product to the where clause if they
			// contain data.
			if ((ledgerSR.Manager != null) && (ledgerSR.Manager.Length > EMPTY_STRING))
				where = where + " AND t.ManagerID = '" + ledgerSR.Manager + "' ";

			if ((ledgerSR.Owner != null) && (ledgerSR.Owner.Length > EMPTY_STRING))
				where = where + " AND t.OwnerID = '" + ledgerSR.Owner + "' ";

			if ((ledgerSR.Product != null) && (ledgerSR.Product.Length > EMPTY_STRING))
				where = where + " AND l.Product = '" + ledgerSR.Product + "' ";

			// Build the main select and sub-select query.
			string sql = select + from + where + orderBy;
			return sql;
		}
		#endregion

		#region Load Public Methods
		/// <summary>
		/// This method will set the summation of the gross
		/// and net volumes for a given inventory date, manager, product and site.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadOneDaysPhysicalInventorySummation(System.Data.DataSet dataSet)
		{
			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					this.startingGrossQuantity = DataObject.getDouble(row[0]);
					this.startingNetQuantity = DataObject.getDouble(row[1]);
					this.startingGrossPrice = DataObject.getDouble(row[2]);
					this.startingNetPrice = DataObject.getDouble(row[3]);
				}
			}
		}

		/// <summary>
		/// This method loads the object with the first date prior to the the 
		/// first date of the month requested by the Ledger. Example: Ledger month 
		/// is set to February, then the first record prior to that would be
		/// the last record with a date of January 31st (2003-01-31 23:23:00).
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadStartingSearchPoint(System.Data.DataSet dataSet)
		{
			// Default to the starting search date to the first day of the
			// ledger month. Default the there is no records prior to the
			// requested month to true.
			this.startSeachingDate = this.firstLedgerDate;
			this.noRecordsPrior = true;

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row;
					row = table.Rows[0];
					DateTimeOffset startDate = DataObject.getValue<DateTimeOffset>(row[0], TimeConverter.Today());
					this.startSeachingDate = DateEfficacy.convertToYearMonthDayTime(startDate);

					// Since there was a record found, then set no records found
					// to false.
					this.noRecordsPrior = false;
				}
			}
		}

		/// <summary>
		/// This method load the object with the latest physical inventory 
		/// record prior to the first day of the requested ledger month. 
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadLatestPhysicalInventoryRecord(System.Data.DataSet dataSet)
		{
			// Default to the first physical inventory date to the first day of the
			// ledger month. Default the there is no records prior to the
			// requested month to true.
			this.firstDateCalculation = this.firstLedgerDate;
			this.noPhysicalInventory = true;

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					DateTime inventoryDate = new DateTime();

					object oInventoryDate = row[0];
					bool bInventoryExists = !isNull(oInventoryDate);

					if (bInventoryExists)
					{
						inventoryDate = DataObject.getValue<DateTime>(oInventoryDate, DateTime.Today);
						this.firstDateCalculation = DateEfficacy.convertToYearMonthDayTimeNoneFill(inventoryDate);
						this.noPhysicalInventory = false;
					}
					else
					{
						this.noPhysicalInventory = true;
					}
				}
			}
		}

		/// <summary>
		/// This method will load the transaction volume line items retrieved from the
		/// database. The data contains pricing and quantity information.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadInventoryData(System.Data.DataSet dataSet)
		{
			if (dataSet != null)
			{
				string INVENTORY_DATE = "InventoryDate";
				string ALIAS_NAME = "AliasName";
				string GROSS_QTY = "GrossQuantity";
				string GROSS_PRICE = "GrossPrice";
				string NET_QTY = "NetQuantity";
				string NET_PRICE = "NetPrice";
				string MASS_QTY = "MassQuantity";
				string MASS_PRICE = "MassPrice";
				string SITE_NAME = "Site";
				string TRANSTYPE_ID = "LookupTransTypeIndex";

				// Empties the list of all objects.
				this.lineItemsList = new LedgerLineItemCollection();
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					LedgerLineItemDO ledgerLineItem = null;
					System.Data.DataRow row;
					string currentDate = "";

					for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
					{
						row = table.Rows[rowIndex];
						string inventoryDate = getString(row[INVENTORY_DATE]);

						// The date returned from the database is formatted as YYYY/MM/DD.  The code below
						// formats the date as MM/DD/YYYY
						char[] separatorList = { '/' };
						string[] stringList = inventoryDate.Split(separatorList);
						inventoryDate = stringList[1] + "/" + stringList[2] + "/" + stringList[0];

						// This is the first row and the inventory date is not empty (it's a string not a datetime) 
						if (rowIndex == 0 || (inventoryDate != currentDate))
						{
							ledgerLineItem = new LedgerLineItemDO();
							this.lineItemsList.Add(ledgerLineItem);

							ledgerLineItem.InventoryDate = inventoryDate;
							currentDate = inventoryDate;
						}

						string aliasName = DataObject.getString(row[ALIAS_NAME]);
						double grossQuantity = DataObject.getDouble(row[GROSS_QTY]);
						double grossPrice = DataObject.getDouble(row[GROSS_PRICE]);
						double netQuantity = DataObject.getDouble(row[NET_QTY]);
						double netPrice = DataObject.getDouble(row[NET_PRICE]);
						double massQuantity = DataObject.getDouble(row[MASS_QTY]);
						double massPrice = DataObject.getDouble(row[MASS_PRICE]);

						// The following aggregates the volumes (quantities and prices) for each trans alias
						// for a given day.
						QuantityDO quantity;
						if (ledgerLineItem.QuantityList.ContainsKey(aliasName) == false)
						{
							// Create the volume (quantities and prices) for the alias name returned
							// from the db
							quantity = new QuantityDO(grossQuantity, netQuantity, massQuantity, 0, grossPrice, netPrice, massPrice);
							ledgerLineItem.QuantityList.Add(aliasName, quantity);
						}
						else
						{
							// The line item already has an entry for the volume so add to it
							quantity = ledgerLineItem.QuantityList[aliasName] as QuantityDO;
							quantity.GrossInventoryChange += grossQuantity;
							quantity.NetInventoryChange += netQuantity;
							quantity.GrossPriceInventoryChange += grossPrice;
							quantity.NetPriceInventoryChange += netPrice;
						}

						ledgerLineItem.Site = DataObject.getString(row[SITE_NAME]);

						string transTypeIDStr = (row.IsNull(TRANSTYPE_ID)) ? "0" : row[TRANSTYPE_ID].ToString();
						TransactionTypes transType = (TransactionTypes)Convert.ToInt32(transTypeIDStr); ;

						//Check Transaction Alias to see if it is a type that affects inventory.
						switch (transType)
						{
							case TransactionTypes.T7_FillStand:
							case TransactionTypes.T9_Request:
							case TransactionTypes.T10_Unload:
							case TransactionTypes.T11_ConsumerTransfer:
							case TransactionTypes.T12_InventoryNotAffected:
							case TransactionTypes.T17_Order:
							case TransactionTypes.T18_SupplyOrder:
							case TransactionTypes.T19_EndOfDay:
							case TransactionTypes.T20_EndOfMonth:
							case TransactionTypes.T21_AccountPayableInvoice:
							case TransactionTypes.T22_AccountReceivableInvoice:
							case TransactionTypes.T23_StorageTransfer:
								quantity.AffectsInventory = false;
								break;
							case TransactionTypes.T14_PhysicalInventory:
								quantity.AffectsInventory = false;
								ledgerLineItem.HasPhysicalInventory = true;
								break;
							default:
								quantity.AffectsInventory = true;
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// This method is used to load all the transaction error records
		/// that are associated with the list inventories.
		/// </summary>
		/// <param name="dataSet"></param>
		public ArrayList loadTransactionError(System.Data.DataSet dataSet)
		{
			ArrayList errorList = new ArrayList();

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row;

					for (int nextRow = 0; nextRow < table.Rows.Count; nextRow++)
					{
						row = table.Rows[nextRow];
						TransactionErrorDO transErrorDO = new TransactionErrorDO();
						transErrorDO.AliasName = DataObject.getString(row[0]);
						transErrorDO.InventoryDate = DataObject.getString(row[1]);
						char[] separatorList = { '/' };
						string[] stringList = transErrorDO.InventoryDate.Split(separatorList);
						transErrorDO.InventoryDate = stringList[1] + "/" + stringList[2] + "/" + stringList[0];
						transErrorDO.ErrorStatus = DataObject.getInt(row[2]);

						errorList.Add(transErrorDO);
					}
				}
			}

			return errorList;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the collection of ledger line item
		/// data objects for consumption.
		/// </summary>
		public LedgerLineItemCollection LedgerLineItemList
		{
			get { return this.lineItemsList; }
			private set { this.lineItemsList = value; }
		}

		/// <summary>
		/// This property indicates that true if there were no records found
		/// prior the the requested ledger month.  Returns false otherwise.
		/// </summary>
		public bool NoRecordsPrior
		{
			get { return this.noRecordsPrior; }
			private set { this.noRecordsPrior = value; }
		}

		/// <summary>
		/// This property indicates that true if there were no physical inventory records found
		/// prior the the requested ledger month.  Returns false otherwise.
		/// </summary>
		public bool NoPhysicalInventory
		{
			get { return this.noPhysicalInventory; }
			set { this.noPhysicalInventory = value; }
		}

		/// <summary>
		/// This property sets and gets the 1st date that is used to start
		/// the ledger calculation.
		/// </summary>
		public string FirstDateCalculation
		{
			get { return this.firstDateCalculation; }
			set { this.firstDateCalculation = value; }
		}

		/// <summary>
		/// This property sets and gets the starting gross volume for
		/// a physical inventory.
		/// </summary>
		public double StartingGrossVolume
		{
			get { return this.startingGrossQuantity; }
			set { this.startingGrossQuantity = value; }
		}

		/// <summary>
		/// This property sets and gets the starting net volume for
		/// a physical inventory.
		/// </summary>
		public double StartingNetVolume
		{
			get { return this.startingNetQuantity; }
			set { this.startingNetQuantity = value; }
		}

		/// <summary>
		/// This property sets and gets the starting gross volume for
		/// a physical inventory.
		/// </summary>
		public double StartingGrossPrice
		{
			get { return this.startingGrossPrice; }
			set { this.startingGrossPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the starting net volume for
		/// a physical inventory.
		/// </summary>
		public double StartingNetPrice
		{
			get { return this.startingNetPrice; }
			set { this.startingNetPrice = value; }
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// This methods initializes the Ledger DO object.
		/// </summary>
		private void init()
		{
			this.lineItemsList = new LedgerLineItemCollection();
			this.noRecordsPrior = true;
			this.noPhysicalInventory = true;
		}

		/// <summary>
		/// This method will return back a "where" clause that filters the
		/// query on transactions that are associated with a user's permission
		/// to only view data that they are a party to. If the user has
		/// permissions to see all data, then the "where" clause will be empty.
		/// An example format looks like this:
		///    AND (ShipToID IN ('Epic Aviation', ...) OR SupplierID IN ('Epic Aviation', ...) OR ...)
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		static internal string GetUserToCompanyWhereClauseOld(string tableAlias, Guid loginSiteGuid, Guid siteGuid, Guid userGuid)
		{
			string userGuidString = "'" + userGuid.ToString() + "'";

			//pass in NULL if the Guid is empty
			if (userGuid == Guid.Empty)
			{
				userGuidString = "NULL";
			}

			string sql =
			" AND EXISTS(SELECT ID FROM [dbo].[udf_AuthorizedCompanies]('" + loginSiteGuid.ToString() + "', '" + siteGuid.ToString() + "', " + userGuidString + ")" +
			"       WHERE ID IN (" + tableAlias + ".OwnerID, " + tableAlias + ".ManagerID, " + tableAlias + ".BillToID,"
				+ tableAlias + ".ShipToID, " + tableAlias + ".ShipperID, " + tableAlias + ".SupplierID, " + tableAlias + ".CarrierID)) ";
			return sql;
		}

		/// <summary>
		/// This method will return back a "where" clause that filters the
		/// query on transactions that are associated with a user's permission
		/// to only view data that they are a party to. If the user has
		/// permissions to see all data, then the "where" clause will be empty.
		/// An example format looks like this:
		///    AND (ShipToID IN ('Epic Aviation', ...) OR SupplierID IN ('Epic Aviation', ...) OR ...)
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		static internal string GetUserToCompanyWhereClause(string tableAlias, Guid loginSiteGuid, Guid siteGuid, Guid userGuid, SqlCommand cmd)
		{
			string sql =
				" AND EXISTS(SELECT ID FROM [dbo].[udf_AuthorizedCompanies]( @LoginSiteGuid, @SiteGuid, @UserGuid )" +
				"  WHERE ID IN (" + tableAlias + ".OwnerID, " + tableAlias + ".ManagerID, " + tableAlias + ".BillToID,"
				+ tableAlias + ".ShipToID, " + tableAlias + ".ShipperID, " + tableAlias + ".SupplierID, " + tableAlias + ".CarrierID)) ";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", loginSiteGuid);

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);

			if (userGuid == Guid.Empty)
			{
				cmd.Parameters["@UserGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@UserGuid"].Value = userGuid;
			}

			return sql;
		}
		#endregion

		#region Override Public Methods
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return "SELECT * from tblTransactions";
		}
		#endregion
	}
}
