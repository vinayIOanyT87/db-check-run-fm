// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LRLedgerQueryBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The purpose of this class is to return ledger vertical data queries and results.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;

	/// <summary>
	/// The lr ledger query base.
	/// </summary>
	public class LRLedgerQueryBase
	{
		#region Protected data members
		protected const int TransactionStatusSuspence = 15;
		protected double volumeConversionFactor;
		protected int volumeDecimalPlaces;
		protected double massConversionFactor;
		protected int massDecimalPlaces;
		protected double currencyFactor;
		protected int currencyDecimalPlaces;
		protected double volumePackageSize;
		protected double massPackageSize;
		protected bool loadByWeight;
		protected LRTransactionAliasListDO transAliasListDo;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="LRLedgerQueryBase"/> class.
		/// This is the default for the Ledger Query Base class.
		/// </summary>
		/// <param name="volumeConversionFactor">
		/// The volume conversion factor.
		/// </param>
		/// <param name="volumeDecimalPlaces">
		/// The volume decimal places.
		/// </param>
		/// <param name="massConversionFactor">
		/// The mass conversion factor.
		/// </param>
		/// <param name="massDecimalPlaces">
		/// The mass decimal places.
		/// </param>
		/// <param name="currencyFactor">
		/// The currency factor.
		/// </param>
		/// <param name="currencyDecimalPlaces">
		/// The currency decimal places.
		/// </param>
		/// <param name="volumePackageSize">
		/// The volume package size.
		/// </param>
		/// <param name="massPackageSize">
		/// The mass package size.
		/// </param>
		/// <param name="loadByWeight">
		/// The load by weight.
		/// </param>
		public LRLedgerQueryBase(
								double volumeConversionFactor,
								int volumeDecimalPlaces,
								double massConversionFactor,
								int massDecimalPlaces,
								double currencyFactor, 
								int currencyDecimalPlaces,
								double volumePackageSize, 
								double massPackageSize, 
								bool loadByWeight,
								LRTransactionAliasListDO transAliasListDo)
		{
			this.volumeConversionFactor = volumeConversionFactor;
			this.volumeDecimalPlaces	= volumeDecimalPlaces;
			this.massConversionFactor	= massConversionFactor;
			this.massDecimalPlaces		= massDecimalPlaces;
			this.currencyFactor			= currencyFactor;
			this.currencyDecimalPlaces	= currencyDecimalPlaces;
			this.volumePackageSize		= volumePackageSize;
			this.massPackageSize		= massPackageSize;
			this.loadByWeight			= loadByWeight;
			this.transAliasListDo		= transAliasListDo;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method returns an SQL string containing the SQL used 
		/// to retrieve the transactional data for computing the ledger.
		/// </summary>
		/// <param name="managerGuid">
		/// The manager guid.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner guid.
		/// </param>
		/// <param name="tankGuid">
		/// The tank guid.
		/// </param>
		/// <param name="siteCount">
		/// The number of sites to process.
		/// </param>
		/// <returns>
		/// The System.String.
		/// </returns>
		public virtual string CreateLineItemSqlStatement(Guid managerGuid, Guid ownerGuid, Guid tankGuid, int siteCount)
		{
			const string Select = "SELECT CONVERT(CHAR(10), t.InventoryDate, 111) as InventoryDate, " +
			                       "t.AliasName, " +
								   "t.TransactionAliasGuid, " +
			                       "l.GrossQuantity AS GrossQuantity, " +
			                       "l.ProductPrice, " +
			                       "l.NetQuantity AS NetQuantity, " +
			                       "l.MassQuantity AS MassQuantity, " +
			                       "t.Site, " +
			                       "t.LookupTransTypeIndex, " +
			                       "l.Number01, " +
			                       "l.Number02, " +
			                       "l.Number03, " +
			                       "l.Number04, " +
			                       "l.Number05, " +
			                       "l.Number06, " +
			                       "t.ErrorFlag, " +
			                       "t.ReversalType, " +
			                       "t.TransVersion ";
			const string From = "FROM tblTransactionLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransactionGuid = t.TransactionGuid ";
			string where	  = "WHERE (t.InventoryDate BETWEEN @BeginDate AND @EndDate) " +
								"AND t.DeleteFlag = CAST(0 AS BIT) " +
                                "AND l.DeleteFlag = CAST(0 AS BIT)" +
								"AND t.SubmittedToAccounting = 1 " +
								"AND (l.ProductGuid = @ProductGuid " +
								"OR l.ProductGuid IN (SELECT ProductGuid FROM tblProducts WHERE TrackingProductGuid = @ProductGuid AND ProductGuid IN (SELECT ProductGuid FROM map.tblEntityProductToSite WHERE SiteGuid = @SelectedSiteGuid))) " +
								"AND ISNULL(l.LookupQualityIndex, 1) = 1 " +
								"AND t.LookupTransactionStatusIndex <> " + TransactionStatusSuspence.ToString(CultureInfo.InvariantCulture) + " " +
								"AND t.TransactionAliasGuid IN (SELECT B.TransactionAliasGuid FROM (SELECT TransactionAliasGuid FROM [dbo].[udf_AliasList](@SelectedSiteGuid)) B) " +
								"AND EXISTS (SELECT A.CompanyGuid FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SelectedSiteGuid, @UserGuid)) A  " +
								"WHERE A.CompanyGuid IS NULL OR A.CompanyGuid IN ( t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, " +
								"t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) ";

			if (ownerGuid != Guid.Empty)
			{
				// If an owner guid is passed in, we need to have that guid on the transaction.  NULL guids should not
				// be included.
				where = where + "AND (t.OwnerCompanyGuid = @OwnerCompanyGuid) ";
			}

			if (managerGuid != Guid.Empty)
			{
				where = where + "AND t.ManagerCompanyGuid = @ManagerCompanyGuid ";
			}

			if (tankGuid != Guid.Empty)
			{
				where = where + "AND l.StorageLocationTankGuid = @TankGuid ";
			}

			if (siteCount > 0)
			{
				string siteWhere = "AND t.SiteGuid IN ( ";

				for (int nextSite = 0; nextSite < siteCount; nextSite++)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					siteWhere = siteWhere + siteParmName + ", ";
				}

				int lastComma = siteWhere.LastIndexOf(',');
				siteWhere = siteWhere.Remove(lastComma);
				siteWhere = siteWhere + " ) ";

				where = where + siteWhere;
			}

			return Select + From + where;
		}

		/// <summary>
		/// This method returns an SQL string containing the SQL used 
		/// to retrieve the transactional sub-line item data for computing the ledger.
		/// </summary>
		/// <param name="managerGuid">
		/// The manager guid.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner guid.
		/// </param>
		/// <param name="tankGuid">
		/// The tank guid.
		/// </param>
		/// <param name="siteCount">
		/// The number of sites to process.
		/// </param>
		/// <returns>
		/// The System.String.
		/// </returns>
		public virtual string CreateSubLineItemSqlStatement(Guid managerGuid, Guid ownerGuid, Guid tankGuid, int siteCount)
		{
			const string Select = "SELECT CONVERT(CHAR(10), t.InventoryDate, 111) AS InventoryDate, " +
			                       "t.AliasName, " +
								   "t.TransactionAliasGuid, " +
								   "l.GrossQuantity AS GrossQuantity, " +
			                       "CAST(0.0 AS FLOAT) AS ProductPrice, " +
			                       "l.NetQuantity AS NetQuantity, " +
			                       "l.MassQuantity AS MassQuantity, " +
			                       "t.Site, " +
			                       "t.LookupTransTypeIndex, " +
			                       "CAST(0.0 AS FLOAT) AS Number01, " +
			                       "CAST(0.0 AS FLOAT) AS Number02, " +
			                       "CAST(0.0 AS FLOAT) AS Number03, " +
			                       "CAST(0.0 AS FLOAT) AS Number04, " +
			                       "CAST(0.0 AS FLOAT) AS Number05, " +
			                       "CAST(0.0 AS FLOAT) AS Number06, " +
			                       "t.ErrorFlag, " +
			                       "t.ReversalType," +
			                       "t.TransVersion ";
			const string From = "FROM tblTransactionSubLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransactionGuid = t.TransactionGuid ";
			string where = "WHERE (t.InventoryDate BETWEEN @BeginDate AND @EndDate) " +
								"AND t.DeleteFlag = CAST(0 AS BIT) " +
                                "AND l.DeleteFlag = CAST(0 AS BIT)" +
								"AND t.SubmittedToAccounting = 1 " +
								"AND (l.ProductGuid = @ProductGuid " +
								"OR l.ProductGuid IN (SELECT ProductGuid FROM tblProducts	WHERE TrackingProductGuid = @ProductGuid AND ProductGuid IN (SELECT ProductGuid FROM map.tblEntityProductToSite WHERE SiteGuid = @SelectedSiteGuid))) " +
								"AND ISNULL(l.LookupQualityIndex, 1) = 1 " +
								"AND t.LookupTransactionStatusIndex <> " + TransactionStatusSuspence.ToString(CultureInfo.InvariantCulture) + " " +
								"AND t.TransactionAliasGuid IN (SELECT B.TransactionAliasGuid FROM (SELECT TransactionAliasGuid FROM [dbo].[udf_AliasList](@SelectedSiteGuid)) B) " +
								"AND EXISTS (SELECT A.CompanyGuid FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SelectedSiteGuid, @UserGuid)) A  " +
								"WHERE A.CompanyGuid IS NULL OR A.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, " +
								"t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) ";

			if (ownerGuid != Guid.Empty)
			{
				// If an owner guid is passed in, we need to have that guid on the transaction.  NULL guids should not
				// be included.
				where = where + "AND (t.OwnerCompanyGuid = @OwnerCompanyGuid) ";
			}

			if (managerGuid != Guid.Empty)
			{
				where = where + "AND t.ManagerCompanyGuid = @ManagerCompanyGuid ";
			}

			if (tankGuid != Guid.Empty)
			{
				where = where + "AND l.StorageLocationTankGuid = @TankGuid ";
			}

			if (siteCount > 0)
			{
				string siteWhere = "AND t.SiteGuid IN ( ";

				for (int nextSite = 0; nextSite < siteCount; nextSite++)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					siteWhere = siteWhere + siteParmName + ", ";
				}

				int lastComma = siteWhere.LastIndexOf(',');
				siteWhere = siteWhere.Remove(lastComma);
				siteWhere = siteWhere + " ) ";

				where = where + siteWhere;
			}

			return Select + From + where;
		}

		/// <summary>
		/// This method will return a sorted list of the transaction information
		/// to compute a ledger. This method will sum up all the transactions for
		/// a given day and alias combination.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <returns>
		/// The System.Collections.SortedList.
		/// </returns>
		public virtual SortedList SumAndGroupData(DataSet dataSet)
		{
			var inventorySummation = new SortedList();

			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
					{
						DataRow row = table.Rows[rowIndex];

						string inventoryDate				= row.IsNull("InventoryDate") ? string.Empty : (string)row["InventoryDate"];
						string aliasName					= row.IsNull("AliasName") ? string.Empty : (string)row["AliasName"];
						Guid transAliasGuid					= row.IsNull("TransactionAliasGuid") ? Guid.Empty : (Guid)row["TransactionAliasGuid"];
						string site							= row.IsNull("Site") ? string.Empty : (string)row["Site"];
						string reversalType					= row.IsNull("ReversalType") ? string.Empty : (string)row["ReversalType"];
						long transVersion					= row.IsNull("TransVersion") ? 0 : (long)row["TransVersion"];
						string lookupTransTypeIndexString	= row.IsNull("LookupTransTypeIndex") ? "0" : row["LookupTransTypeIndex"].ToString();
						int lookupTransTypeIndex			= Convert.ToInt32(lookupTransTypeIndexString);

						double gross	= row.IsNull("GrossQuantity") ? 0.0 : (double)row["GrossQuantity"];
						double net		= row.IsNull("NetQuantity") ? 0.0 : (double)row["NetQuantity"];
						double mass		= row.IsNull("MassQuantity") ? 0.0 : (double)row["MassQuantity"];
						double price	= row.IsNull("ProductPrice") ? 0.0 : (double)row["ProductPrice"];
						double number01 = row.IsNull("Number01") ? 0.0 : (double)row["Number01"];
						double number02 = row.IsNull("Number02") ? 0.0 : (double)row["Number02"];
						double number03 = row.IsNull("Number03") ? 0.0 : (double)row["Number03"];
						double number04 = row.IsNull("Number04") ? 0.0 : (double)row["Number04"];
						double number05 = row.IsNull("Number05") ? 0.0 : (double)row["Number05"];
						double number06 = row.IsNull("Number06") ? 0.0 : (double)row["Number06"];
						bool errorFlag	= row.IsNull("ErrorFlag") ? false : (bool)row["ErrorFlag"];
						bool isReversal = row.IsNull("ReversalType") ? false : ((string)row["ReversalType"]).ToUpper().Contains("R");

						// Must have an inventory date and alias name.
						if (string.IsNullOrEmpty(inventoryDate) || 
							string.IsNullOrEmpty(aliasName) ||
							string.IsNullOrEmpty(site))
						{
							continue;
						}

						// Find the configured transaction alias name if it differs from the
						// transaction record alias name.
						aliasName = this.FindConfiguredAliasName(aliasName, transAliasGuid);

						// This key will be sorted by the SortedList on site, inventory date, and alias name.
						string key = site + "|" + inventoryDate + "|" + aliasName;

						LRInventoryDailyAliasDO invDailyAlias;
						if (inventorySummation.Contains(key))
						{
							invDailyAlias = (LRInventoryDailyAliasDO)inventorySummation[key];

							invDailyAlias.InventoryDateStr	= inventoryDate;
							invDailyAlias.AliasName			= aliasName;
							invDailyAlias.Site				= site;
							invDailyAlias.TransTypeID		= lookupTransTypeIndex;

							// Only set the reversal type if not empty. The reason is that
							// there may be many transactions that make up the inventory/Alias combination
							// and if the last one may clear the reversal type.
							if (string.IsNullOrEmpty(reversalType) == false && reversalType.ToUpper().Contains("R"))
							{
								invDailyAlias.ReversalType = reversalType;
							}

							if (transVersion > invDailyAlias.MaxTransVersion)
							{
								invDailyAlias.MaxTransVersion = transVersion;
							}

							invDailyAlias.SumGross(gross);
							invDailyAlias.SumNet(net);
							invDailyAlias.SumMass(mass);
							invDailyAlias.SumGrossPrice(price, gross);
							invDailyAlias.SumNetPrice(price, net);
							invDailyAlias.SumMassPrice(price, mass);
							invDailyAlias.SumNumberField(number01, 1);
							invDailyAlias.SumNumberField(number02, 2);
							invDailyAlias.SumNumberField(number03, 3);
							invDailyAlias.SumNumberField(number04, 4);
							invDailyAlias.SumNumberField(number05, 5);
							invDailyAlias.SumNumberField(number06, 6);
							invDailyAlias.OrErrorFlag(errorFlag);
							invDailyAlias.OrReversalFlag(isReversal);
						}
						else
						{
							invDailyAlias = new LRInventoryDailyAliasDO(
																		this.volumeConversionFactor,
																		this.massConversionFactor,
																		this.currencyFactor,
																		this.volumeDecimalPlaces,
																		this.massDecimalPlaces,
																		this.currencyDecimalPlaces,
																		this.volumePackageSize,
																		this.massPackageSize,
																		this.loadByWeight)
								                {
									                InventoryDateStr = inventoryDate,
									                AliasName		= aliasName,
									                Site			= site,
									                TransTypeID		= lookupTransTypeIndex
								                };

							if (string.IsNullOrEmpty(reversalType) == false && reversalType.ToUpper().Contains("R"))
							{
								invDailyAlias.ReversalType = reversalType;
							}

							if (transVersion > invDailyAlias.MaxTransVersion)
							{
								invDailyAlias.MaxTransVersion = transVersion;
							}

							invDailyAlias.SumGross(gross);
							invDailyAlias.SumNet(net);
							invDailyAlias.SumMass(mass);
							invDailyAlias.SumGrossPrice(price, gross);
							invDailyAlias.SumNetPrice(price, net);
							invDailyAlias.SumMassPrice(price, mass);
							invDailyAlias.SumNumberField(number01, 1);
							invDailyAlias.SumNumberField(number02, 2);
							invDailyAlias.SumNumberField(number03, 3);
							invDailyAlias.SumNumberField(number04, 4);
							invDailyAlias.SumNumberField(number05, 5);
							invDailyAlias.SumNumberField(number06, 6);
							invDailyAlias.OrErrorFlag(errorFlag);
							invDailyAlias.OrReversalFlag(isReversal);

							inventorySummation.Add(key, invDailyAlias);
						}
					}
				}
			}

			return inventorySummation;
		}

		/// <summary>
		/// This method will compare the configured transaction alias name with the 
		/// alias name on the transaction. If they match, then the transaction alias
		/// name is returned. If they do not match, then a search based on the alias
		/// GUID is performed. If found, then the configured transaction alias name is
		/// returned.
		/// </summary>
		/// <param name="transAliasName">Transaction record alias name.</param>
		/// <param name="transAliasGuid">Transaction record alias GUID.</param>
		/// <returns>Returns the transaction record alias name or configured transaction alias name.</returns>
		protected string FindConfiguredAliasName(string transAliasName, Guid transAliasGuid)
        {
			// Ensure the transaction alias DO and alias lists have data and
			// if the configured alias name matches the transaction record name,
			// then return the transaction record alias name.
			if (this.transAliasListDo == null 
				|| this.transAliasListDo.AliasList == null 
				|| this.transAliasListDo.AliasList.Count == 0
				|| this.transAliasListDo.AliasSortedList == null
				|| this.transAliasListDo.AliasSortedList.Count == 0
				|| this.transAliasListDo.AliasList.ContainsKey(transAliasName))
            {
				return transAliasName;
            }

			// Since the configured alias name does not match the transaction alias name,
			// Search by transaction alias GUID and if found, returned the configured transaction
			// alias name.
			foreach (LRTransactionAliasDO transAliasDo in this.transAliasListDo.AliasSortedList.Values)
			{
				if (transAliasDo.TransactionAliasGuid.Equals(transAliasGuid))
				{
					return transAliasDo.AliasName;
				}
			}

			// Return input as the default.
			return transAliasName;
		}
		#endregion
	}
}