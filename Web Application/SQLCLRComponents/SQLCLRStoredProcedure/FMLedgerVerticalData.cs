/// <summary>
///   File name:	FMLedgerVerticalData.cs
///   Purpose:	   The purpose of this class is to retrieve ledger raw data for a date range in
///               order to sum up daily quantities for each alias using the criterion of manager,
///               owner, and product. It will return the vertical ledger math to the calling client.
///				
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///	2009-06-01		W.Gray		   		Revised to support TrackingProduct (CSI 3911)
///	
///	2009-07-06		W.Gray					Revised CreateSqlStatement and CreateSubLineItemSqlStatement to use NOLOCK (CSI 4581)
///
///   2010-02-15		W.Gray					Revised TransID to TransIndex in ancillary transaction tables (WI 11422)
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data;

public class FMLedgerVerticalData
{
	#region Private data members
	private DateTime beginDate;
	private DateTime endDate;
	private int siteIndex;
	private int productIndex;
	private int managerIndex;
	private int ownerIndex;
	private int loginSiteIndex;
	private int selectedSiteIndex;
	private int userIndex;
	private double volumeConversionFactor;
	private int volumeDecimalPlaces;
	private double massConversionFactor;
	private int massDecimalPlaces;
	private double currencyFactor;
	private int currencyDecimalPlaces;
	private double volumePackageSize;
	private double massPackageSize;
	private bool loadByWeight;
	private int tankIndex;
	private CLRLedgerProcessor.SystemEditions systemEdition;

	private const int TRANSACTION_STATUS_SUSPENCE = 15;

	private SqlCommand ledgerCommand = null;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor for the FuelsManager Ledger Vertical Data class.
	/// </summary>
	/// <param name="BeginDate"></param>
	/// <param name="EndDate"></param>
	/// <param name="SiteIndex"></param>
	/// <param name="ProductIndex"></param>
	/// <param name="Manager"></param>
	/// <param name="Owner"></param>
	/// <param name="LoginSiteIndex"></param>
	/// <param name="SelectedSiteIndex"></param>
	/// <param name="UserIndex"></param>
	/// <param name="VolumeConversionFactor"></param>
	/// <param name="VolumeDecimalPlaces"></param>
	/// <param name="MassConversionFactor"></param>
	/// <param name="MassDecimalPlaces"></param>
	/// <param name="CurrencyFactor"></param>
	/// <param name="CurrencyDecimalPlaces"></param>
	/// <param name="VolumePackageSize"></param>
	/// <param name="MassPackageSize"></param>
	/// <param name="LoadByWeight"></param>
	public FMLedgerVerticalData(	DateTime BeginDate,
											DateTime EndDate,
											string SiteName,
											int ProductIndex,
											int ManagerIndex,
											int OwnerIndex,
											int LoginSiteIndex,
											int SelectedSiteIndex,
											int UserIndex,
											double VolumeConversionFactor,
											int VolumeDecimalPlaces,
											double MassConversionFactor,
											int MassDecimalPlaces,
											double CurrencyFactor,
											int CurrencyDecimalPlaces,
											double VolumePackageSize,
											double MassPackageSize,
											bool LoadByWeight,
											int TankIndex,
											int SystemEdition)
	{
		// Initialize the data members
		this.beginDate                  = BeginDate;
		this.endDate                    = EndDate;
		this.siteIndex                  = SelectedSiteIndex;
		this.productIndex               = ProductIndex;
		this.managerIndex               = ManagerIndex;
		this.ownerIndex                 = OwnerIndex;
		this.loginSiteIndex             = LoginSiteIndex;
		this.selectedSiteIndex          = SelectedSiteIndex;
		this.userIndex                  = UserIndex;
		this.volumeConversionFactor     = VolumeConversionFactor;
		this.volumeDecimalPlaces        = VolumeDecimalPlaces;
		this.massConversionFactor		  = MassConversionFactor;
		this.massDecimalPlaces		     = MassDecimalPlaces;
		this.currencyFactor             = CurrencyFactor;
		this.currencyDecimalPlaces      = CurrencyDecimalPlaces;
		this.volumePackageSize		     = VolumePackageSize;
		this.massPackageSize			     = MassPackageSize;
		this.loadByWeight				     = LoadByWeight;
		this.tankIndex						  = TankIndex;

		this.SetSystemEdition(SystemEdition);
	}
	#endregion

	#region Public methods
	/// <summary>
	/// This method will retrieve the Ledger data, sum the daily transactions per
	/// alias and return a SQL Data Record containing the data back to the client.
	/// </summary>
	public SortedList RetrieveAndSendData(SqlConnection connection)
	{
		LedgerStandardQuery ledgerStandardQuery = null;
		LedgerADFQuery ledgerAdfQuery      = null;
		LedgerBSMEQuery ledgerBsmeQuery     = null;
	  	LedgerModQuery ledgerModQuery = null;

		string sql1 = null;
		string sql2 = null;

		SortedList inventorySummation = null;

		// Create the main sql that will be use to retrieve the transactional
		// data for computing the ledger.
		switch(systemEdition)
		{
			case CLRLedgerProcessor.SystemEditions.ADF:
				ledgerAdfQuery = new LedgerADFQuery(this.volumeConversionFactor,
																this.volumeDecimalPlaces,
																this.massConversionFactor,
																this.massDecimalPlaces,
																this.currencyFactor,
																this.currencyDecimalPlaces,
																this.volumePackageSize,
																this.massPackageSize,
																this.loadByWeight);

				sql1 = ledgerAdfQuery.CreateLineItemSqlStatement(this.managerIndex,this.ownerIndex,this.tankIndex);
				sql2 = ledgerAdfQuery.CreateSubLineItemSqlStatement(this.managerIndex,this.ownerIndex,this.tankIndex);
				break;
			case CLRLedgerProcessor.SystemEditions.BSME:
				ledgerBsmeQuery = new LedgerBSMEQuery(this.volumeConversionFactor,
														this.volumeDecimalPlaces,
														this.massConversionFactor,
														this.massDecimalPlaces,
														this.currencyFactor,
														this.currencyDecimalPlaces,
														this.volumePackageSize,
														this.massPackageSize,
														this.loadByWeight);

				sql1 = ledgerBsmeQuery.CreateLineItemSqlStatement(this.managerIndex,this.ownerIndex,this.tankIndex);
				sql2 = ledgerBsmeQuery.CreateSubLineItemSqlStatement(this.managerIndex,this.ownerIndex,this.tankIndex);
				break;
		 case CLRLedgerProcessor.SystemEditions.MOD:
				ledgerModQuery = new LedgerModQuery(this.volumeConversionFactor,
														this.volumeDecimalPlaces,
														this.massConversionFactor,
														this.massDecimalPlaces,
														this.currencyFactor,
														this.currencyDecimalPlaces,
														this.volumePackageSize,
														this.massPackageSize,
														this.loadByWeight);

				sql1 = ledgerModQuery.CreateLineItemSqlStatement(this.managerIndex, this.ownerIndex, this.tankIndex);
				sql2 = ledgerModQuery.CreateSubLineItemSqlStatement(this.managerIndex, this.ownerIndex, this.tankIndex);
				break;
			default:
				ledgerStandardQuery = new LedgerStandardQuery(this.volumeConversionFactor,
																				this.volumeDecimalPlaces,
																				this.massConversionFactor,
																				this.massDecimalPlaces,
																				this.currencyFactor,
																				this.currencyDecimalPlaces,
																				this.volumePackageSize,
																				this.massPackageSize,
																				this.loadByWeight);

				sql1 = ledgerStandardQuery.CreateLineItemSqlStatement(this.managerIndex,this.ownerIndex,this.tankIndex);
				sql2 = ledgerStandardQuery.CreateSubLineItemSqlStatement(this.managerIndex,this.ownerIndex,this.tankIndex);
				break;
		}

		// Retrieve the transactional data for computing the ledger.
		DataSet dataSet = PerformLedgerQuery(connection,sql1 + " UNION ALL " + sql2);

		// Sum the transaction quantities and group by inventory date and 
		// alias name.
		switch(systemEdition)
		{
			case CLRLedgerProcessor.SystemEditions.ADF:
				inventorySummation = ledgerAdfQuery.SumAndGroupData(dataSet);
				break;
			case CLRLedgerProcessor.SystemEditions.BSME:
				inventorySummation = ledgerBsmeQuery.SumAndGroupData(dataSet);
				break;
			case CLRLedgerProcessor.SystemEditions.MOD:
				inventorySummation = ledgerModQuery.SumAndGroupData(dataSet);
				break;
			default:
				inventorySummation = ledgerStandardQuery.SumAndGroupData(dataSet);
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
	/// <param name="sql"></param>
	/// <returns></returns>
	private DataSet PerformLedgerQuery(SqlConnection connection,string sql)
	{
		DataSet dataSet = new DataSet();

		ledgerCommand = new SqlCommand(sql,connection);

		ledgerCommand.Parameters.Add("@BeginDate",System.Data.SqlDbType.SmallDateTime);
		ledgerCommand.Parameters.Add("@EndDate",System.Data.SqlDbType.SmallDateTime);
		ledgerCommand.Parameters.Add("@SiteIndex",System.Data.SqlDbType.Int);
		ledgerCommand.Parameters.Add("@ProductIndex",System.Data.SqlDbType.Int);
		ledgerCommand.Parameters.Add("@LoginSiteIndex",System.Data.SqlDbType.Int);
		ledgerCommand.Parameters.Add("@SelectedSiteIndex",System.Data.SqlDbType.Int);
		ledgerCommand.Parameters.Add("@UserIndex",System.Data.SqlDbType.Int);

		if(this.ownerIndex > 0)
			ledgerCommand.Parameters.Add("@OwnerIndex",System.Data.SqlDbType.Int);
		if(this.managerIndex > 0)
			ledgerCommand.Parameters.Add("@ManagerIndex",System.Data.SqlDbType.Int);
		if(this.tankIndex > 0)
			ledgerCommand.Parameters.Add("@TankIndex",System.Data.SqlDbType.Int);

		ledgerCommand.Prepare();

		ledgerCommand.Parameters["@BeginDate"].Value = this.beginDate;
		ledgerCommand.Parameters["@EndDate"].Value = this.endDate;
		ledgerCommand.Parameters["@SiteIndex"].Value = this.siteIndex;
		ledgerCommand.Parameters["@ProductIndex"].Value = this.productIndex;
		ledgerCommand.Parameters["@LoginSiteIndex"].Value = this.loginSiteIndex;
		ledgerCommand.Parameters["@SelectedSiteIndex"].Value = this.selectedSiteIndex;
		ledgerCommand.Parameters["@UserIndex"].Value = this.userIndex;

		if(this.ownerIndex > 0)
			ledgerCommand.Parameters["@OwnerIndex"].Value = this.ownerIndex;
		if(this.managerIndex > 0)
			ledgerCommand.Parameters["@ManagerIndex"].Value = this.managerIndex;
		if(this.tankIndex > 0)
			ledgerCommand.Parameters["@TankIndex"].Value = this.tankIndex;

		ledgerCommand.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(ledgerCommand);
		adapter.Fill(dataSet);

		return dataSet;
	}

	/// <summary>
	/// This method will return the Ledger vertical data via the SQL
	/// Pipe to the client.
	/// </summary>
	/// <param name="inventorySummation"></param>
	private void SendDataTableOverPipe(SortedList inventorySummation)
	{
		// Define the columns for data record rows.
		List<SqlMetaData> columns = new List<SqlMetaData>(8);
		SqlMetaData outputColumn01 = new SqlMetaData("InventoryDate",SqlDbType.NVarChar,50);
		SqlMetaData outputColumn02 = new SqlMetaData("AliasName",SqlDbType.NVarChar,50);
		SqlMetaData outputColumn03 = new SqlMetaData("GrossQuantity",SqlDbType.Float);
		SqlMetaData outputColumn04 = new SqlMetaData("GrossPrice",SqlDbType.Float);
		SqlMetaData outputColumn05 = new SqlMetaData("NetQuantity",SqlDbType.Float);
		SqlMetaData outputColumn06 = new SqlMetaData("NetPrice",SqlDbType.Float);
		SqlMetaData outputColumn07 = new SqlMetaData("Site",SqlDbType.NVarChar,50);
		SqlMetaData outputColumn08 = new SqlMetaData("TransTypeID",SqlDbType.Int);
		SqlMetaData outputColumn09 = new SqlMetaData("Number01",SqlDbType.Float);
		SqlMetaData outputColumn10 = new SqlMetaData("Number02",SqlDbType.Float);
		SqlMetaData outputColumn11 = new SqlMetaData("Number03",SqlDbType.Float);
		SqlMetaData outputColumn12 = new SqlMetaData("Number04",SqlDbType.Float);
		SqlMetaData outputColumn13 = new SqlMetaData("Number05",SqlDbType.Float);
		SqlMetaData outputColumn14 = new SqlMetaData("Number06",SqlDbType.Float);
		SqlMetaData outputColumn15 = new SqlMetaData("ErrorFlag",SqlDbType.Bit);

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
		SqlDataRecord record = new SqlDataRecord(columns.ToArray());
		SqlContext.Pipe.SendResultsStart(record);

		IDictionaryEnumerator enumerator = inventorySummation.GetEnumerator();
		InventoryDailyAliasDO inventoryDailyAlias = null;

		// Set the data record row values and send it out on the pipe.
		while(enumerator.MoveNext() == true)
		{
			inventoryDailyAlias = (InventoryDailyAliasDO)enumerator.Value;

			record.SetValue(0,inventoryDailyAlias.InventoryDateStr);
			record.SetValue(1,inventoryDailyAlias.AliasName);
			record.SetValue(2,inventoryDailyAlias.GrossQuantity);
			record.SetValue(3,inventoryDailyAlias.GrossPrice);
			record.SetValue(4,inventoryDailyAlias.NetQuantity);
			record.SetValue(5,inventoryDailyAlias.NetPrice);
			record.SetValue(6,inventoryDailyAlias.Site);
			record.SetValue(7,inventoryDailyAlias.TransTypeID);
			record.SetValue(8,inventoryDailyAlias.Number01);
			record.SetValue(9,inventoryDailyAlias.Number02);
			record.SetValue(10,inventoryDailyAlias.Number03);
			record.SetValue(11,inventoryDailyAlias.Number04);
			record.SetValue(12,inventoryDailyAlias.Number05);
			record.SetValue(13,inventoryDailyAlias.Number06);
			record.SetValue(14,inventoryDailyAlias.ErrorFlag);

			SqlContext.Pipe.SendResultsRow(record);
		}

		// Close the pipe.
		SqlContext.Pipe.SendResultsEnd();
	}

	/// <summary>
	/// This method will set the System Edition to the appropriate system edition
	/// (STANDARD, BSME, or ADF). 
	/// </summary>
	/// <param name="inSystemEdition"></param>
	private void SetSystemEdition(int inSystemEdition)
	{
		switch(inSystemEdition)
		{
			case 1:
				this.systemEdition = CLRLedgerProcessor.SystemEditions.BSME;
				break;
			case 2:
				this.systemEdition = CLRLedgerProcessor.SystemEditions.ADF;
				break;
			 case 3:
				this.systemEdition = CLRLedgerProcessor.SystemEditions.MOD;
				break;
			default:
				this.systemEdition = CLRLedgerProcessor.SystemEditions.STANDARD;
				break;
		}
	}
	#endregion
}
