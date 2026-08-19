// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LRLedgerVerticalData.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The purpose of this class is to retrieve ledger raw data for a date range in
//   order to sum up daily quantities for each alias using the criterion of manager,
//   owner, and product. It will return the vertical ledger math to the calling client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	using Microsoft.SqlServer.Server;

	/// <summary>
	/// The lr ledger vertical data.
	/// </summary>
	public class LRLedgerVerticalData
	{
		#region Private data members
		private DateTimeOffset beginDate;
		private DateTimeOffset endDate;
		private readonly Guid siteGuid;
		private readonly Guid productGuid;
		private readonly Guid managerGuid;
		private readonly Guid ownerGuid;
		private readonly Guid selectedSiteGuid;
		private readonly Guid userGuid;
		private readonly double volumeConversionFactor;
		private readonly int volumeDecimalPlaces;
		private readonly double massConversionFactor;
		private readonly int massDecimalPlaces;
		private readonly double currencyFactor;
		private readonly int currencyDecimalPlaces;
		private readonly double volumePackageSize;
		private readonly double massPackageSize;
		private readonly bool loadByWeight;
		private readonly Guid tankGuid;
		private readonly List<LRSiteDO> siteList;
		private readonly LRLedgerProcessor.DateProcessTypes dateProcessType;
		private bool isBaseDb;
		private LRLedgerProcessor.SystemEditions systemEdition;
		private LRTransactionAliasListDO transAliasListDo;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="LRLedgerVerticalData"/> class.
		/// This is the default constructor for the FuelsManager Ledger Vertical Data class.
		/// </summary>
		/// <param name="beginDate">The begin date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="siteName">The site name.</param>
		/// <param name="productGuid">The product guid.</param>
		/// <param name="managerGuid">The manager guid.</param>
		/// <param name="ownerGuid">The owner guid.</param>
		/// <param name="selectedSiteGuid">The selected site guid.</param>
		/// <param name="userGuid">The user guid.</param>
		/// <param name="volumeConversionFactor">The volume conversion factor.</param>
		/// <param name="volumeDecimalPlaces">The volume decimal places.</param>
		/// <param name="massConversionFactor">The mass conversion factor.</param>
		/// <param name="massDecimalPlaces">The mass decimal places.</param>
		/// <param name="currencyFactor">The currency factor.</param>
		/// <param name="currencyDecimalPlaces">The currency decimal places.</param>
		/// <param name="volumePackageSize">The volume package size.</param>
		/// <param name="massPackageSize">The mass package size.</param>
		/// <param name="loadByWeight">The load by weight.</param>
		/// <param name="tankGuid">The tank guid.</param>
		/// <param name="systemEdition">The system edition.</param>
		public LRLedgerVerticalData(
									DateTimeOffset beginDate,
									DateTimeOffset endDate,
									string siteName,
									Guid productGuid,
									Guid managerGuid,
									Guid ownerGuid,
									Guid selectedSiteGuid,
									Guid userGuid,
									double volumeConversionFactor,
									int volumeDecimalPlaces,
									double massConversionFactor,
									int massDecimalPlaces,
									double currencyFactor,
									int currencyDecimalPlaces,
									double volumePackageSize,
									double massPackageSize,
									bool loadByWeight,
									Guid tankGuid,
									int systemEdition)
		{
			// Initialize the data members
			this.beginDate				= beginDate;
			this.endDate				= endDate;
			this.siteGuid				= selectedSiteGuid;
			this.productGuid			= productGuid;
			this.managerGuid			= managerGuid;
			this.ownerGuid				= ownerGuid;
			this.selectedSiteGuid		= selectedSiteGuid;
			this.userGuid				= userGuid;
			this.volumeConversionFactor = volumeConversionFactor;
			this.volumeDecimalPlaces	= volumeDecimalPlaces;
			this.massConversionFactor	= massConversionFactor;
			this.massDecimalPlaces		= massDecimalPlaces;
			this.currencyFactor			= currencyFactor;
			this.currencyDecimalPlaces	= currencyDecimalPlaces;
			this.volumePackageSize		= volumePackageSize;
			this.massPackageSize		= massPackageSize;
			this.loadByWeight			= loadByWeight;
			this.tankGuid				= tankGuid;

			this.dateProcessType = LRLedgerProcessor.DateProcessTypes.ByInventoryDate;
			this.isBaseDb = true;
			this.SetSystemEdition(systemEdition);
			this.transAliasListDo = new LRTransactionAliasListDO();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LRLedgerVerticalData"/> class.
		/// </summary>
		/// <param name="beginDate">The begin date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="siteName">The site name.</param>
		/// <param name="productGuid">The product guid.</param>
		/// <param name="managerGuid">The manager guid.</param>
		/// <param name="ownerGuid">The owner guid.</param>
		/// <param name="selectedSiteGuid">The selected site guid.</param>
		/// <param name="userGuid">The user guid.</param>
		/// <param name="volumeConversionFactor">The volume conversion factor.</param>
		/// <param name="volumeDecimalPlaces">The volume decimal places.</param>
		/// <param name="massConversionFactor">The mass conversion factor.</param>
		/// <param name="massDecimalPlaces">The mass decimal places.</param>
		/// <param name="currencyFactor">The currency factor.</param>
		/// <param name="currencyDecimalPlaces">The currency decimal places.</param>
		/// <param name="volumePackageSize">The volume package size.</param>
		/// <param name="massPackageSize">The mass package size.</param>
		/// <param name="loadByWeight">The load by weight.</param>
		/// <param name="tankGuid">The tank guid.</param>
		/// <param name="systemEdition">The system edition.</param>
		/// <param name="siteList">The site list.</param>
		/// <param name="dateProcessType">The date process type.</param>
		/// <param name="isBaseDb">The base/enterprise flag.</param>
		/// <param name="transAliasListDo">List of configured transaction aliases.</param>
		public LRLedgerVerticalData(
									DateTime beginDate,
									DateTime endDate,
									Guid productGuid,
									Guid managerGuid,
									Guid ownerGuid,
									Guid selectedSiteGuid,
									Guid userGuid,
									double volumeConversionFactor,
									int volumeDecimalPlaces,
									double massConversionFactor,
									int massDecimalPlaces,
									double currencyFactor,
									int currencyDecimalPlaces,
									double volumePackageSize,
									double massPackageSize,
									bool loadByWeight,
									Guid tankGuid,
									int systemEdition,
									List<LRSiteDO> siteList,
									LRLedgerProcessor.DateProcessTypes dateProcessType,
									bool isBaseDb,
									LRTransactionAliasListDO transAliasListDo)
		{
			// Initialize the data members
			this.beginDate				= beginDate;
			this.endDate				= endDate;
			this.siteGuid				= selectedSiteGuid;
			this.productGuid			= productGuid;
			this.managerGuid			= managerGuid;
			this.ownerGuid				= ownerGuid;
			this.selectedSiteGuid		= selectedSiteGuid;
			this.userGuid				= userGuid;
			this.volumeConversionFactor = volumeConversionFactor;
			this.volumeDecimalPlaces	= volumeDecimalPlaces;
			this.massConversionFactor	= massConversionFactor;
			this.massDecimalPlaces		= massDecimalPlaces;
			this.currencyFactor			= currencyFactor;
			this.currencyDecimalPlaces	= currencyDecimalPlaces;
			this.volumePackageSize		= volumePackageSize;
			this.massPackageSize		= massPackageSize;
			this.loadByWeight			= loadByWeight;
			this.tankGuid				= tankGuid;
			this.siteList				= siteList;
			this.dateProcessType		= dateProcessType;
			this.isBaseDb				= isBaseDb;
			this.transAliasListDo		= transAliasListDo;

			this.SetSystemEdition(systemEdition);
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will retrieve the Ledger data, sum the daily transactions per
		/// alias and return a SQL Data Record containing the data back to the client.
		/// </summary>
		/// <returns>
		/// The System.Collections.SortedList.
		/// </returns>
		public SortedList RetrieveAndSendData(LedgerConnection ledgerConnection)
		{
			LRLedgerStandardQuery ledgerStandardQuery	= null;
			LRLedgerADFQuery ledgerAdfQuery				= null;
			LRLedgerBSMEQuery ledgerBsmeQuery			= null;
			LRLedgerModQuery ledgerModQuery				= null;

			string sql1;
			string sql2;

			SortedList inventorySummation = null;

			// Create the main sql that will be use to retrieve the transactional
			// data for computing the ledger.
			switch (this.systemEdition)
			{
				case LRLedgerProcessor.SystemEditions.Adf:
					ledgerAdfQuery = new LRLedgerADFQuery(
						this.volumeConversionFactor,
						this.volumeDecimalPlaces,
						this.massConversionFactor,
						this.massDecimalPlaces,
						this.currencyFactor,
						this.currencyDecimalPlaces,
						this.volumePackageSize,
						this.massPackageSize,
						this.loadByWeight,
						this.transAliasListDo);

					sql1 = ledgerAdfQuery.CreateLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					sql2 = ledgerAdfQuery.CreateSubLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					break;
				case LRLedgerProcessor.SystemEditions.Bsme:
					ledgerBsmeQuery = new LRLedgerBSMEQuery(
						this.volumeConversionFactor,
						this.volumeDecimalPlaces,
						this.massConversionFactor,
						this.massDecimalPlaces,
						this.currencyFactor,
						this.currencyDecimalPlaces,
						this.volumePackageSize,
						this.massPackageSize,
						this.loadByWeight,
						this.dateProcessType,
						this.isBaseDb,
						this.transAliasListDo);

					sql1 = ledgerBsmeQuery.CreateLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					sql2 = ledgerBsmeQuery.CreateSubLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					break;
				case LRLedgerProcessor.SystemEditions.Mod:
					ledgerModQuery = new LRLedgerModQuery(
						this.volumeConversionFactor,
						this.volumeDecimalPlaces,
						this.massConversionFactor,
						this.massDecimalPlaces,
						this.currencyFactor,
						this.currencyDecimalPlaces,
						this.volumePackageSize,
						this.massPackageSize,
						this.loadByWeight,
						this.transAliasListDo);

					sql1 = ledgerModQuery.CreateLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					sql2 = ledgerModQuery.CreateSubLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					break;
				default:
					ledgerStandardQuery = new LRLedgerStandardQuery(
						this.volumeConversionFactor,
						this.volumeDecimalPlaces,
						this.massConversionFactor,
						this.massDecimalPlaces,
						this.currencyFactor,
						this.currencyDecimalPlaces,
						this.volumePackageSize,
						this.massPackageSize,
						this.loadByWeight,
						this.transAliasListDo);

					sql1 = ledgerStandardQuery.CreateLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					sql2 = ledgerStandardQuery.CreateSubLineItemSqlStatement(this.managerGuid, this.ownerGuid, this.tankGuid, this.siteList.Count);
					break;
			}

			// Retrieve the transactional data for computing the ledger.
			DataSet dataSet = this.PerformLedgerQuery(ledgerConnection, sql1 + " UNION ALL " + sql2);

			// Sum the transaction quantities and group by inventory date and 
			// alias name.
			switch (this.systemEdition)
			{
				case LRLedgerProcessor.SystemEditions.Adf:
					if (ledgerAdfQuery != null)
					{
						inventorySummation = ledgerAdfQuery.SumAndGroupData(dataSet);
					}
					break;
				case LRLedgerProcessor.SystemEditions.Bsme:
					if (ledgerBsmeQuery != null)
					{
						inventorySummation = ledgerBsmeQuery.SumAndGroupData(dataSet);
					}
					break;
				case LRLedgerProcessor.SystemEditions.Mod:
					if (ledgerModQuery != null)
					{
						inventorySummation = ledgerModQuery.SumAndGroupData(dataSet);
					}
					break;
				default:
					if (ledgerStandardQuery != null)
					{
						inventorySummation = ledgerStandardQuery.SumAndGroupData(dataSet);
					}
					break;
			}

			return inventorySummation;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method returns a data set containing the transactions that will be used
		/// to compute the ledger.
		/// </summary>
		/// <param name="ledgerConnection"></param>
		/// <param name="sql">
		/// The sql.
		/// </param>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		private DataSet PerformLedgerQuery(LedgerConnection ledgerConnection, string sql)
		{
			using (var ledgerCommand = new SqlCommand())
			{
				ledgerCommand.CommandText = sql;

				ledgerCommand.Parameters.Add("@BeginDate", SqlDbType.Date);
				ledgerCommand.Parameters.Add("@EndDate", SqlDbType.Date);
				ledgerCommand.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				ledgerCommand.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
				ledgerCommand.Parameters.Add("@SelectedSiteGuid", SqlDbType.UniqueIdentifier);
				ledgerCommand.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);

				if (this.ownerGuid != Guid.Empty)
				{
					ledgerCommand.Parameters.Add("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier);
				}

				if (this.managerGuid != Guid.Empty)
				{
					ledgerCommand.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
				}

				if (this.tankGuid != Guid.Empty)
				{
					ledgerCommand.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);
				}

				for (int nextSite = 0; nextSite < this.siteList.Count; nextSite++)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					ledgerCommand.Parameters.Add(siteParmName, SqlDbType.UniqueIdentifier);
				}

				ledgerCommand.Parameters["@BeginDate"].Value		= this.beginDate.Date;
				ledgerCommand.Parameters["@EndDate"].Value			= this.endDate.Date;
				ledgerCommand.Parameters["@SiteGuid"].Value			= this.siteGuid;
				ledgerCommand.Parameters["@ProductGuid"].Value		= this.productGuid;
				ledgerCommand.Parameters["@SelectedSiteGuid"].Value	= this.selectedSiteGuid;

				if (this.userGuid != Guid.Empty)
				{
					ledgerCommand.Parameters["@UserGuid"].Value = this.userGuid;
				}
				else
				{
					ledgerCommand.Parameters["@UserGuid"].Value = DBNull.Value;
				}

				if (this.ownerGuid != Guid.Empty)
				{
					ledgerCommand.Parameters["@OwnerCompanyGuid"].Value = this.ownerGuid;
				}

				if (this.managerGuid != Guid.Empty)
				{
					ledgerCommand.Parameters["@ManagerCompanyGuid"].Value = this.managerGuid;
				}

				if (this.tankGuid != Guid.Empty)
				{
					ledgerCommand.Parameters["@TankGuid"].Value = this.tankGuid;
				}

				for (int nextSite = 0; nextSite < this.siteList.Count; nextSite++)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					ledgerCommand.Parameters[siteParmName].Value = this.siteList[nextSite].SiteGuid;
				}

				ledgerCommand.CommandTimeout = 0;

				return ledgerConnection.GetDataSet(ledgerCommand);
			}
		}

		/// <summary>
		/// This method will return the Ledger vertical data via the SQL
		/// Pipe to the client.
		/// </summary>
		/// <param name="inventorySummation">
		/// The inventory summation.
		/// </param>
		private void SendDataTableOverPipe(SortedList inventorySummation)
		{
			// Define the columns for data record rows.
			var columns = new List<SqlMetaData>(8);
			var outputColumn01 = new SqlMetaData("InventoryDate", SqlDbType.NVarChar, 50);
			var outputColumn02 = new SqlMetaData("AliasName", SqlDbType.NVarChar, 50);
			var outputColumn03 = new SqlMetaData("GrossQuantity", SqlDbType.Float);
			var outputColumn04 = new SqlMetaData("GrossPrice", SqlDbType.Float);
			var outputColumn05 = new SqlMetaData("NetQuantity", SqlDbType.Float);
			var outputColumn06 = new SqlMetaData("NetPrice", SqlDbType.Float);
			var outputColumn07 = new SqlMetaData("Site", SqlDbType.NVarChar, 50);
			var outputColumn08 = new SqlMetaData("LookupTransTypeIndex", SqlDbType.Int);
			var outputColumn09 = new SqlMetaData("Number01", SqlDbType.Float);
			var outputColumn10 = new SqlMetaData("Number02", SqlDbType.Float);
			var outputColumn11 = new SqlMetaData("Number03", SqlDbType.Float);
			var outputColumn12 = new SqlMetaData("Number04", SqlDbType.Float);
			var outputColumn13 = new SqlMetaData("Number05", SqlDbType.Float);
			var outputColumn14 = new SqlMetaData("Number06", SqlDbType.Float);
			var outputColumn15 = new SqlMetaData("ErrorFlag", SqlDbType.Bit);

			columns.Add(outputColumn01);
			columns.Add(outputColumn02);
			columns.Add(outputColumn03);
			columns.Add(outputColumn04);
			columns.Add(outputColumn05);
			columns.Add(outputColumn06);
			columns.Add(outputColumn07);
			columns.Add(outputColumn08);
			columns.Add(outputColumn09);
			columns.Add(outputColumn10);
			columns.Add(outputColumn11);
			columns.Add(outputColumn12);
			columns.Add(outputColumn13);
			columns.Add(outputColumn14);
			columns.Add(outputColumn15);

			// Create the columns for the data record.
			var record = new SqlDataRecord(columns.ToArray());

			if ( SqlContext.Pipe == null )
			{
				throw new Exception("Invalid SQL Pipe.");
			}

			SqlContext.Pipe.SendResultsStart(record);

			IDictionaryEnumerator enumerator = inventorySummation.GetEnumerator();

			// Set the data record row values and send it out on the pipe.
			while (enumerator.MoveNext())
			{
				LRInventoryDailyAliasDO inventoryDailyAlias = (LRInventoryDailyAliasDO) enumerator.Value;

				record.SetValue(0, inventoryDailyAlias.InventoryDateStr);
				record.SetValue(1, inventoryDailyAlias.AliasName);
				record.SetValue(2, inventoryDailyAlias.GrossQuantity);
				record.SetValue(3, inventoryDailyAlias.GrossPrice);
				record.SetValue(4, inventoryDailyAlias.NetQuantity);
				record.SetValue(5, inventoryDailyAlias.NetPrice);
				record.SetValue(6, inventoryDailyAlias.Site);
				record.SetValue(7, inventoryDailyAlias.TransTypeID);
				record.SetValue(8, inventoryDailyAlias.Number01);
				record.SetValue(9, inventoryDailyAlias.Number02);
				record.SetValue(10, inventoryDailyAlias.Number03);
				record.SetValue(11, inventoryDailyAlias.Number04);
				record.SetValue(12, inventoryDailyAlias.Number05);
				record.SetValue(13, inventoryDailyAlias.Number06);
				record.SetValue(14, inventoryDailyAlias.ErrorFlag);

				SqlContext.Pipe.SendResultsRow(record);
			}

			// Close the pipe.
			SqlContext.Pipe.SendResultsEnd();
		}

		/// <summary>
		/// This method will set the System Edition to the appropriate system edition
		/// (STANDARD, BSME, or ADF).
		/// </summary>
		/// <param name="inSystemEdition">
		/// The in system edition.
		/// </param>
		private void SetSystemEdition(int inSystemEdition)
		{
			switch (inSystemEdition)
			{
				case 1:
					this.systemEdition = LRLedgerProcessor.SystemEditions.Bsme;
					break;
				case 2:
					this.systemEdition = LRLedgerProcessor.SystemEditions.Adf;
					break;
				case 3:
					this.systemEdition = LRLedgerProcessor.SystemEditions.Mod;
					break;
				default:
					this.systemEdition = LRLedgerProcessor.SystemEditions.Standard;
					break;
			}
		}
		#endregion
	}
}