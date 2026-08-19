/// <summary>
///   File name:	CLRLedgerProcessor.cs
///   Purpose:	   The purpose of this class is to orchestrate the process of getting
///               data for the ledger, getting beginning book, and calculating the 
///               ledger.
///				
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///   2010-05-21     W.Gray					Removed Support for Error Indication
///
///	2010-05-28		W.Gray 					Revised to improve performance (WI 14681)
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using Microsoft.SqlServer.Server;

public class CLRLedgerProcessor
{
	#region Public data members
	public enum LedgerRequests { REFRESH, MANAGER_LEDGER };
	public enum SystemEditions { STANDARD, BSME, ADF, MOD };
	#endregion

	#region Private data members
	private TransactionAliasListDO transAliasListDO;
	private Hashtable aliasTypeList;
	private int siteIndex;
	private int loginSiteIndex;
	private int userIndex;
	private int managerIndex;
	private int ownerIndex;
	private int productIndex;
	private DateTime inventoryDate;
	private DateTime beginDate;
	private DateTime endDate;
	private bool reportLedger;
	private bool singleOwnerSystem;
	private double volumeFactor;
	private double volumePrecision;
	private double volumePackageSize;
	private double massFactor;
	private double massPrecision;
	private double massPackageSize;
	private bool loadByWeight;
	private DateConverter dateConverter;
	private LedgerRequests ledgerRequest;
	private int tankIndex;
	private SystemEditions systemEdition;

	private SqlConnection connection = null;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor for the CLR Ledger Processor class.
	/// </summary>
	public CLRLedgerProcessor()
	{
		this.transAliasListDO = null;
		this.aliasTypeList = new Hashtable();
		this.siteIndex = 0;
		this.loginSiteIndex = 0;
		this.managerIndex = 0;
		this.ownerIndex = 0;
		this.productIndex = 0;
		this.inventoryDate = DateTime.UtcNow;
		this.beginDate = DateTime.UtcNow;
		this.endDate = DateTime.UtcNow;
		this.userIndex = 0;
		this.reportLedger = true;
		this.volumeFactor = 1.0;
		this.volumePrecision = 2.0;
		this.volumePackageSize = 0;
		this.massFactor = 1.0;
		this.massPrecision = 2.0;
		this.massPackageSize = 0.0;
		this.loadByWeight = false;
		this.dateConverter = new DateConverter();
		this.ledgerRequest = LedgerRequests.REFRESH;
		this.singleOwnerSystem = false;
		this.tankIndex = 0;
	}
	#endregion

	#region Properties
	/// <summary>
	/// This property gets and sets the report ledger flag. True means
	/// that a report is calling the ledger.
	/// </summary>
	public bool ReportLedger
	{
		get { return this.reportLedger; }
		set { this.reportLedger = value; }
	}

	/// <summary>
	/// This property gets and sets the ledger request to either
	/// a Manager Ledger or Refresh.
	/// </summary>
	public LedgerRequests LedgerRequest
	{
		get { return this.ledgerRequest; }
		set { this.ledgerRequest = value; }
	}

	/// <summary>
	/// This property will set the Ledger request based on an integer value.
	/// The default is REFRESH request.
	/// </summary>
	public int LedgerRequestInt
	{
		get
		{
			switch (this.ledgerRequest)
			{
				case LedgerRequests.MANAGER_LEDGER:
					return 1;
				case LedgerRequests.REFRESH:
					return 0;
				default:
					return 0;
			}
		}
		set
		{
			if (value != 1)
			{
				this.ledgerRequest = LedgerRequests.REFRESH;
			}
			else
			{
				this.ledgerRequest = LedgerRequests.MANAGER_LEDGER;
			}
		}
	}

	/// <summary>
	/// This property gets and sets the ledger begin date data member.
	/// </summary>
	public DateTime BeginDate
	{
		get { return this.beginDate; }
		set { this.beginDate = value; }
	}

	/// <summary>
	/// This property gets and sets the ledger end date data member.
	/// </summary>
	public DateTime EndDate
	{
		get { return this.endDate; }
		set { this.endDate = value; }
	}

	/// <summary>
	/// This property gets and sets the product index data member.
	/// </summary>
	public int ProductIndex
	{
		get { return this.productIndex; }
		set { this.productIndex = value; }
	}

	/// <summary>
	/// This property gets and sets the login site index data member.
	/// </summary>
	public int LoginSiteIndex
	{
		get { return this.loginSiteIndex; }
		set { this.loginSiteIndex = value; }
	}

	/// <summary>
	/// This property gets and sets the site index data member.
	/// </summary>
	public int SiteIndex
	{
		get { return this.siteIndex; }
		set { this.siteIndex = value; }
	}

	/// <summary>
	/// This property gets and sets the user index data member.
	/// </summary>
	public int UserIndex
	{
		get { return this.userIndex; }
		set { this.userIndex = value; }
	}

	/// <summary>
	/// This property gets and sets the manager index data member.
	/// </summary>
	public int ManagerIndex
	{
		get { return this.managerIndex; }
		set { this.managerIndex = value; }
	}

	/// <summary>
	/// This property gets and sets the owner index data member.
	/// </summary>
	public int OwnerIndex
	{
		get { return this.ownerIndex; }
		set { this.ownerIndex = value; }
	}

	/// <summary>
	/// This property gets and sets the tank index data member.
	/// </summary>
	public int TankIndex
	{
		get { return tankIndex; }
		set { tankIndex = value; }
	}

	/// <summary>
	/// This property gets and sets the system edition data member.
	/// </summary>
	public SystemEditions SystemEdition
	{
		get { return this.systemEdition; }
		set
		{
			switch (value)
			{
				case SystemEditions.ADF:
					this.systemEdition = SystemEditions.ADF;
					break;
				case SystemEditions.BSME:
					this.systemEdition = SystemEditions.BSME;
					break;
				case SystemEditions.MOD:
					this.systemEdition = SystemEditions.MOD;
					break;
				default:
					this.systemEdition = SystemEditions.STANDARD;
					break;
			}
		}
	}
	#endregion

	#region Ledger Processing
	/// <summary>
	/// This method starts the ledger processing for a given manager, owner, product, site, or sites.
	/// </summary>
	public void StartLedgerProcessing()
	{
		connection = new SqlConnection("context connection=true");
		connection.Open();

		try
		{
			// Get the transaction aliase one time for the given site.
			this.GetTransactionAliases();

			// The the product information which includes the conversion factor and
			// precision. If the product is configured with the conversion factor, then
			// class factor and precision members will be set use the product settings.
			// Otherwise the site's settings will be used.
			ProductDO productDO = this.RetrieveProductInfo();

			// Retrieve a site list for the current site and their information.
			SiteListDO siteListDO = this.RetrieveSites(this.siteIndex);
			SiteDO currentSite = this.RetrieveCurrentSite();
			siteListDO.AddSiteToList(currentSite);

			// Loop through the list of sites looking for a site group that is set to 
			// inhibit ledger rollup. If found, then all the children sites under that 
			// site group will have the inhibit ledger rollup flag set to inhibit rollup.
			this.InhibitChildSitesRollupBasedOnParent(siteListDO);

			if (productDO.UseProductVolumeConversionFactor == true)
			{
				this.volumeFactor = productDO.VolumeConversionFactor;
				this.volumePrecision = Convert.ToDouble(productDO.VolumeDecimalPlaces);
			}
			else
			{
				if (productDO.ProductType == ProductDO.PRODUCT_TYPE.ADDITIVE_PRODUCT)
				{
					this.volumeFactor = currentSite.AdditiveVolumeConversionFactor;
					this.volumePrecision = Convert.ToDouble(currentSite.AdditiveVolumeDecimalPlaces);
				}
				else
				{
					this.volumeFactor = currentSite.VolumeConversionFactor;
					this.volumePrecision = Convert.ToDouble(currentSite.VolumeDecimalPlaces);
				}
			}

			this.volumePackageSize = Math.Round(productDO.VolumePackageSize * this.volumeFactor, (int)this.volumePrecision, MidpointRounding.AwayFromZero);

			if (productDO.UseProductMassConversionFactor == true)
			{
				this.massFactor = productDO.MassConversionFactor;
				this.massPrecision = Convert.ToDouble(productDO.MassDecimalPlaces);
			}
			else
			{
				this.massFactor = currentSite.MassConversionFactor;
				this.massPrecision = Convert.ToDouble(currentSite.MassDecimalPlaces);
			}

			this.massPackageSize = Math.Round(productDO.MassPackageSize * this.massFactor, (int)this.massPrecision, MidpointRounding.AwayFromZero);
			this.loadByWeight = productDO.LoadByWeight;

			if (this.ledgerRequest != LedgerRequests.MANAGER_LEDGER)
			{
				this.singleOwnerSystem = currentSite.SingleOwner;
			}

			// Default the currency conversion factor and precision. At some point
			// the GUI needs to have a configuration setting for this.
			double currencyConversion = 1;
			int currencyDecimalPlaces = 2;

			// Set the currency decimal places to -1 which means do not
			// round.
			if (systemEdition == SystemEditions.ADF)
			{
				currencyDecimalPlaces = -1;
			}

			bool usePreviousPhysicalInventory = true;
			bool hasPhysicalInvDate = false;
			QuantityDO initialBookInventory = null;
			LedgerLineItemCollection ledgerLineItemList = null;

			ArrayList arrayOfLedgerLineItems = new ArrayList();
			ArrayList arrayOfInitialBeginInventory = new ArrayList();
			ArrayList arrayOfCloseoutDates = new ArrayList();
			ArrayList arrayOfOwnerCloseoutDates = new ArrayList();
			ArrayList arrayOfBrokenBlendDates = new ArrayList();
			ArrayList arrayOfOwnerCloseoutDO = new ArrayList();
			ArrayList arrayOfCloseoutDO = new ArrayList();

			// Create a ledger for each site that is not a site group.
			IDictionaryEnumerator enumerator = siteListDO.SiteList.GetEnumerator();
			while (enumerator.MoveNext() == true)
			{
				SiteDO siteDO = (SiteDO)enumerator.Value;
				if (siteDO == null)
				{
					continue;
				}
				else if ((siteDO.InhibitSiteLedgerRollup == true) && (siteDO.SiteIndex != this.siteIndex))
				{
					// Do not create a ledger if the site has the inhibit ledger rollup set
					// and the site object is not the current site.
					continue;
				}

				if (productDO.IsProductAssigned(connection, siteDO.SiteIndex) == false)
				{
					continue;
				}

				// This is only used for BSME System.
				bool physicalOnLastDay = false;

				DateTime startDate = new DateTime(1901, 01, 01, 00, 00, 00);

				OwnerCloseoutDO ownerCloseoutDO = new OwnerCloseoutDO();
				arrayOfOwnerCloseoutDO.Add(ownerCloseoutDO);

				CloseoutDO closeoutDO = new CloseoutDO();
				arrayOfCloseoutDO.Add(closeoutDO);

				SortedList inventorySummation = null;
				if (siteDO.SiteGroupFlag == false)
				{
					hasPhysicalInvDate = false;

					// Retrieve closeout record
					this.RetrieveCloseoutRecord(closeoutDO, siteDO.SiteIndex);

					// If not equal to Manager Ledger (meaning owner ledger) get owner closeout records
					if ((this.singleOwnerSystem == false) && (ledgerRequest != LedgerRequests.MANAGER_LEDGER))
					{
						this.RetrieveOwnerCloseoutRecord(ownerCloseoutDO, this.beginDate);

						if (ownerCloseoutDO.FoundOwnerCloseoutRecord == true)
						{
							// Move the start calculating date to the day after the closeout
							startDate = ownerCloseoutDO.CloseoutDate.AddDays(1);
							hasPhysicalInvDate = true;
						}
					}
					else
					{
						// Retrieve the most recent physical inventory date for the given site.
						DateTime physicalInvDate = this.GetMostRecentPhysicalInventoryDate(siteDO.SiteIndex, ref hasPhysicalInvDate);

						if (hasPhysicalInvDate == true)
						{
							startDate = physicalInvDate.AddDays(1);

							// BSME system only
							if (this.SystemEdition == SystemEditions.BSME)
							{
								physicalOnLastDay = this.IsPhysicalOnTheLastDayOfTheMonth(physicalInvDate);

								// Ensure that if there was a physical on the last day of the month, that
								// the physical was on the 1st previous month and not another month.
								if (physicalOnLastDay == true)
								{
									DateTime lastDayOfPreviousMonth = this.beginDate;
									lastDayOfPreviousMonth = lastDayOfPreviousMonth.AddDays(-1);

									if (physicalInvDate.Equals(lastDayOfPreviousMonth) == false)
									{
										physicalOnLastDay = false;
									}
								}
							}
						}
					}

					// Retrieve the beginning book gross and net values. We are assuming a 
					// single owner system, therefore the physical inventory is the beginning
					// book. Since there can be several physicals in one day, we get the sum
					// of those inventories.
					if ((this.singleOwnerSystem == true) || (ledgerRequest == LedgerRequests.MANAGER_LEDGER))
					{
						initialBookInventory = new QuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

						if (this.systemEdition != SystemEditions.BSME)
						{
							this.GetSummedPhysicalInventories(siteDO.SiteIndex, this.inventoryDate, ref initialBookInventory);
						}
						else
						{
							// BSME system
							if (physicalOnLastDay == true)
							{
								this.GetSummedPhysicalInventories(siteDO.SiteIndex, this.inventoryDate, ref initialBookInventory);

								// There is no need to search for inventory data prior to the current month since we already
								// have a initial book inventory.
								startDate = this.beginDate;
							}
							else
							{
								// Since that were no physical inventories on the last day of the month
								// set the start date to be the beginning of the month.
								startDate = this.beginDate;
							}
						}

						arrayOfInitialBeginInventory.Add(initialBookInventory);
					}
					else
					{
						double packageQuantity = 0;

						if (loadByWeight)
						{
							if (massPackageSize != 0)
								packageQuantity = ownerCloseoutDO.BookInventory.MassInventoryChange / massPackageSize;
						}
						else
						{
							if (volumePackageSize != 0)
								packageQuantity = ownerCloseoutDO.BookInventory.NetInventoryChange / volumePackageSize;
						}

						initialBookInventory = new QuantityDO(ownerCloseoutDO.BookInventory.GrossInventoryChange,
																			ownerCloseoutDO.BookInventory.NetInventoryChange,
																			ownerCloseoutDO.BookInventory.MassInventoryChange,
																			packageQuantity,
																			ownerCloseoutDO.BookInventory.GrossPriceInventoryChange,
																			ownerCloseoutDO.BookInventory.NetPriceInventoryChange,
																			ownerCloseoutDO.BookInventory.MassPriceInventoryChange);

						arrayOfInitialBeginInventory.Add(initialBookInventory);
					}

					// Retrieve data for the ledger
					FMLedgerVerticalData ledgerVerticalData = new FMLedgerVerticalData(startDate,
																											 this.endDate,
																											 siteDO.SiteName,
																											 this.productIndex,
																											 this.managerIndex,
																											 this.ownerIndex,
																											 this.loginSiteIndex,
																											 siteDO.SiteIndex,
																											 this.userIndex,
																											 this.volumeFactor,
																											 Convert.ToInt32(this.volumePrecision),
																											 this.massFactor,
																											 Convert.ToInt32(this.massPrecision),
																											 currencyConversion,
																											 currencyDecimalPlaces,
																											 volumePackageSize,
																											 massPackageSize,
																											 loadByWeight,
																											 this.tankIndex,
																											 (int)systemEdition);

					inventorySummation = ledgerVerticalData.RetrieveAndSendData(connection);



				}
				else if ((siteDO != null) && (siteDO.SiteGroupFlag == true))
				{
					hasPhysicalInvDate = false;
					initialBookInventory = new QuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
					arrayOfInitialBeginInventory.Add(initialBookInventory);

					// Retrieve data for the ledger
					startDate = this.beginDate;

					FMLedgerVerticalData ledgerVerticalData = new FMLedgerVerticalData(startDate,
																											 this.endDate,
																											 siteDO.SiteName,
																											 this.productIndex,
																											 this.managerIndex,
																											 this.ownerIndex,
																											 this.loginSiteIndex,
																											 siteDO.SiteIndex,
																											 this.userIndex,
																											 this.volumeFactor,
																											 Convert.ToInt32(this.volumePrecision),
																											 this.massFactor,
																											 Convert.ToInt32(this.massPrecision),
																											 currencyConversion,
																											 currencyDecimalPlaces,
																											 volumePackageSize,
																											 massPackageSize,
																											 loadByWeight,
																											 this.tankIndex,
																											 (int)systemEdition);

					inventorySummation = ledgerVerticalData.RetrieveAndSendData(connection);


				}

				if (inventorySummation != null)
				{
					// Load the inventory data information
					ledgerLineItemList = new LedgerLineItemCollection();
					this.LoadInventoryData(inventorySummation, ref ledgerLineItemList);
					arrayOfLedgerLineItems.Add(ledgerLineItemList);

					// Set the Site and Product indexes to be use for retrieving the WAC
					ledgerLineItemList.SiteIndex = siteDO.SiteIndex;
					ledgerLineItemList.ProductIndex = this.productIndex;

					if (ledgerRequest == LedgerRequests.MANAGER_LEDGER)
					{
						this.RetrieveMostRecentCloseoutDate(siteDO.SiteIndex, closeoutDO, arrayOfCloseoutDates);
						this.RetrieveMostRecentBrokenBlendDate(siteDO.SiteIndex, closeoutDO, arrayOfBrokenBlendDates);
					}
					else
					{
						this.RetrieveMostRecentOwnerCloseoutDate(siteDO.SiteIndex, ownerCloseoutDO, arrayOfCloseoutDates);
					}
				}
			}

			// Calculate the ledger (i.e. beginning and ending books, variances)
			usePreviousPhysicalInventory = this.singleOwnerSystem | (this.ledgerRequest == LedgerRequests.MANAGER_LEDGER);
			LedgerManager ledgerManager = new LedgerManager(this.transAliasListDO, this.aliasTypeList, usePreviousPhysicalInventory, systemEdition);

			LedgerLineItemCollection finalLedgerLineItemCollection = ledgerManager.CreateLedger(connection,
																															arrayOfLedgerLineItems,
																															arrayOfInitialBeginInventory,
																															this.beginDate,
																															this.endDate,
																															arrayOfCloseoutDates,
																															arrayOfBrokenBlendDates);

			// Send the ledger data to the client.
			if (this.reportLedger == true)
			{
				this.SendDataToClient(finalLedgerLineItemCollection);
			}
			else
			{
				this.SendDataToGUIClient(finalLedgerLineItemCollection);
			}
		}
		catch (Exception e)
		{
			EventLog eventLog = new EventLog("Application", ".", "FuelsManager-CLRLedgerProcessor");
			eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
		}
		finally
		{
			connection.Close();
		}
	}
	#endregion

	#region Retrieve the transaction aliases
	/// <summary>
	/// This method will retrieve the transaction aliases for the current site and
	/// the site that the user logged into.
	/// </summary>
	private void GetTransactionAliases()
	{
		this.transAliasListDO = new TransactionAliasListDO();
		this.transAliasListDO.PerformQuery(connection, this.siteIndex, this.loginSiteIndex);

		ICollection values = transAliasListDO.Values;
		foreach (TransactionAliasDO transAliasDO in values)
		{
			if (this.aliasTypeList.Contains(transAliasDO.AliasName) == false)
			{
				this.aliasTypeList.Add(transAliasDO.AliasName, transAliasDO.TransactionTypeID);
			}
		}

		// Get the aggreated aliases
		this.GetAggregateAliases();
	}

	/// <summary>
	/// This method will retrieve the configured aggregated aliases.
	/// </summary>
	private void GetAggregateAliases()
	{
		this.transAliasListDO.PerformAggregateQuery(connection, this.siteIndex, this.loginSiteIndex);

		ICollection values = transAliasListDO.Values;
		foreach (TransactionAliasDO transAliasDO in values)
		{
			if ((transAliasDO.IsAggregateAlias == true) && (this.aliasTypeList.Contains(transAliasDO.AliasName) == false))
			{
				this.aliasTypeList.Add(transAliasDO.AliasName, transAliasDO.TransactionTypeID);
			}
		}
	}
	#endregion

	#region Get the most recent Physical Inventory date
	/// <summary>
	/// This method will retrieve the most recent physical inventory date that matches the criterion.
	/// If not found then a default date of 1901-01-01 is returned.
	/// </summary>
	/// <param name="siteIndex"></param>
	/// <returns></returns>
	private DateTime GetMostRecentPhysicalInventoryDate(int inSiteIndex, ref bool hasPhysicalInvDate)
	{
		string sql = "usp_GetLatestPhysicalInventoryRecordSelect"; // @SiteIndex, @InventoryDate, @ManagerIndex, @ProductIndex, @TankIndex

		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(sql, connection);
		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@InventoryDate", System.Data.SqlDbType.SmallDateTime);
		command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
		command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
		command.Parameters.Add("@ManagerIndex", System.Data.SqlDbType.Int);
		command.Parameters.Add("@TankIndex", System.Data.SqlDbType.Int);

		command.Parameters["@InventoryDate"].Value = this.beginDate;
		command.Parameters["@ProductIndex"].Value = this.productIndex;
		command.Parameters["@SiteIndex"].Value = inSiteIndex;
		command.Parameters["@ManagerIndex"].Value = this.managerIndex;
		command.Parameters["@TankIndex"].Value = this.tankIndex;

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		return this.LoadMostRecentPhysicalInventoryDate(dataSet, ref hasPhysicalInvDate);
	}

	/// <summary>
	/// This method will return the most recent physical inventory retrieved from
	/// the database. The date will be returned as a string.
	/// </summary>
	/// <param name="dataSet"></param>
	/// <returns></returns>
	private DateTime LoadMostRecentPhysicalInventoryDate(DataSet dataSet, ref bool hasPhysicalInvDate)
	{
		DateTime initDate = new DateTime(1901, 01, 01, 00, 00, 00);

		if ((dataSet != null) && (dataSet.Tables[0] != null))
		{
			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				DataRow row = table.Rows[0];

				if (row.IsNull("InventoryDate") == false)
				{
					this.inventoryDate = (DateTime)row["InventoryDate"];
					initDate = this.inventoryDate;
					hasPhysicalInvDate = true;
				}
			}
		}

		return initDate;
	}
	#endregion

	#region Sum all the Physical Inventories for a given Day
	/// <summary>
	/// This method will retrieve summed daily physical inventory quantities based on the 
	/// criterion passed into the method. It will return the gross and net quantities.
	/// </summary>
	/// <param name="siteIndex"></param>
	/// <param name="inventoryDate"></param>
	/// <param name="grossQuantity"></param>
	/// <param name="netQuantity"></param>
	private void GetSummedPhysicalInventories(int inSiteIndex,
															DateTime inventoryDate,
															ref QuantityDO quantity)
	{
		string sql = "usb_getOneDaysPhysicalInventorySummationSelect";//@SiteIndex, @InventoryDate, " +
		//"@ManagerIndex, @ProductIndex, @VolumeFactor, @VolumePrecision, @MassFactor, @MassPrecision, @TankIndex";

		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(sql, connection);
		command.CommandType = CommandType.StoredProcedure;

		command.Parameters.Add("@InventoryDate", System.Data.SqlDbType.SmallDateTime);
		command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
		command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
		command.Parameters.Add("@ManagerIndex", System.Data.SqlDbType.Int);
		command.Parameters.Add("@VolumeFactor", System.Data.SqlDbType.Float);
		command.Parameters.Add("@VolumePrecision", System.Data.SqlDbType.Float);
		command.Parameters.Add("@MassFactor", System.Data.SqlDbType.Float);
		command.Parameters.Add("@MassPrecision", System.Data.SqlDbType.Float);
		command.Parameters.Add("@TankIndex", System.Data.SqlDbType.Int);
		command.Parameters["@InventoryDate"].Value = inventoryDate;
		command.Parameters["@ProductIndex"].Value = this.productIndex;
		command.Parameters["@SiteIndex"].Value = inSiteIndex;
		command.Parameters["@ManagerIndex"].Value = this.managerIndex;
		command.Parameters["@VolumeFactor"].Value = this.volumeFactor;
		command.Parameters["@VolumePrecision"].Value = this.volumePrecision;
		command.Parameters["@MassFactor"].Value = this.volumeFactor;
		command.Parameters["@MassPrecision"].Value = this.volumePrecision;
		command.Parameters["@TankIndex"].Value = this.tankIndex;
		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load the quantities
		this.LoadSummedPhysicalInventories(dataSet, ref quantity);
	}

	/// <summary>
	/// This method loads the summed daily physical inventory quantities and returns the
	/// gross and net quantities.
	/// </summary>
	/// <param name="dataSet"></param>
	/// <param name="volume"></param>
	private void LoadSummedPhysicalInventories(DataSet dataSet, ref QuantityDO quantity)
	{
		if ((dataSet != null) && (dataSet.Tables[0] != null))
		{
			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				DataRow row = table.Rows[0];

				quantity.Gross = (row.IsNull("GrossQuantity")) ? 0.0 : (double)row["GrossQuantity"];
				quantity.Net = (row.IsNull("NetQuantity")) ? 0.0 : (double)row["NetQuantity"];
				quantity.Mass = (row.IsNull("MassQuantity")) ? 0.0 : (double)row["MassQuantity"];
			}
		}
	}
	#endregion

	#region Get Owner Closeout records and Closeout records
	/// <summary>
	/// This method will return an Owner Closeout DO based on the manager, owner, product, 
	/// and site.
	/// </summary>
	/// <returns></returns>
	private void RetrieveOwnerCloseoutRecord(OwnerCloseoutDO ownerCloseoutDO, DateTime ledgerStartDate)
	{
		ownerCloseoutDO.ManagerIndex = this.managerIndex;
		ownerCloseoutDO.OwnerIndex = this.ownerIndex;
		ownerCloseoutDO.ProductIndex = this.productIndex;
		ownerCloseoutDO.SiteIndex = this.siteIndex;

		string sql = ownerCloseoutDO.GetCurrentOwnerCloseoutSelectSQL();
		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(sql, connection);

		command.Parameters.Add("@LedgerStartDate", System.Data.SqlDbType.DateTime);
		command.Parameters["@LedgerStartDate"].Value = this.dateConverter.GetDateWithCorrectTimePortion(ledgerStartDate, DateConverter.TimeTypes.START);

		if (this.managerIndex > 0)
		{
			command.Parameters.Add("@ManagerIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ManagerIndex"].Value = this.managerIndex;
		}
		if (this.ownerIndex > 0)
		{
			command.Parameters.Add("@OwnerIndex", System.Data.SqlDbType.Int);
			command.Parameters["@OwnerIndex"].Value = this.ownerIndex;
		}
		if (this.productIndex > 0)
		{
			command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ProductIndex"].Value = this.productIndex;
		}
		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
			command.Parameters["@SiteIndex"].Value = this.siteIndex;
		}

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load current owner closeout record.
		ownerCloseoutDO.LoadCurrentOwnerCloseout(dataSet);

		ownerCloseoutDO.BookInventory.GrossInventoryChange *= this.volumeFactor;
		ownerCloseoutDO.BookInventory.NetInventoryChange *= this.volumeFactor;
		ownerCloseoutDO.BookInventory.MassInventoryChange *= this.massFactor;
		ownerCloseoutDO.BookInventory.GrossPriceInventoryChange *= this.volumeFactor;
		ownerCloseoutDO.BookInventory.NetPriceInventoryChange *= this.volumeFactor;
		ownerCloseoutDO.BookInventory.MassInventoryChange *= this.massFactor;

		Math.Round(ownerCloseoutDO.BookInventory.Gross, (int)this.volumePrecision, MidpointRounding.AwayFromZero);
		Math.Round(ownerCloseoutDO.BookInventory.Net, (int)this.volumePrecision, MidpointRounding.AwayFromZero);
		Math.Round(ownerCloseoutDO.BookInventory.Mass, (int)this.massPrecision, MidpointRounding.AwayFromZero);
	}

	/// <summary>
	/// This method will retrieve the most recent owner closeout date based on the manager, 
	/// owner, product, site, and the closeout that is great than or equal to the ledger
	/// start date.
	/// </summary>
	/// <returns></returns>
	private void RetrieveMostRecentOwnerCloseoutDate(int siteIndex, OwnerCloseoutDO ownerCloseoutDO, ArrayList arrayOfCloseoutDates)
	{
		ownerCloseoutDO.ManagerIndex = this.managerIndex;
		ownerCloseoutDO.OwnerIndex = this.ownerIndex;
		ownerCloseoutDO.ProductIndex = this.productIndex;
		ownerCloseoutDO.SiteIndex = siteIndex;

		string sql = ownerCloseoutDO.GetLatestCloseoutDateSelectSQL();
		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(sql, connection);

		command.Parameters.Add("@LedgerStartDate", System.Data.SqlDbType.DateTime);
		command.Parameters["@LedgerStartDate"].Value = this.dateConverter.GetDateWithCorrectTimePortion(this.beginDate, DateConverter.TimeTypes.START);

		if (this.managerIndex > 0)
		{
			command.Parameters.Add("@ManagerIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ManagerIndex"].Value = this.managerIndex;
		}
		if (this.ownerIndex > 0)
		{
			command.Parameters.Add("@OwnerIndex", System.Data.SqlDbType.Int);
			command.Parameters["@OwnerIndex"].Value = this.ownerIndex;
		}
		if (this.productIndex > 0)
		{
			command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ProductIndex"].Value = this.productIndex;
		}
		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
			command.Parameters["@SiteIndex"].Value = siteIndex;
		}

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load current owner closeout record.
		ownerCloseoutDO.LoadLatestCloseoutDate(dataSet);

		if ((ownerCloseoutDO.CloseoutDateString != null) && (ownerCloseoutDO.CloseoutDateString.Length > 0))
		{
			arrayOfCloseoutDates.Add(ownerCloseoutDO.CloseoutDateString);
		}
		else
		{
			arrayOfCloseoutDates.Add(null);
		}
	}

	/// <summary>
	/// This method will return the Closeout DO with the lastest closeout record based on the
	/// manager, product, and site.
	/// </summary>
	/// <returns></returns>
	private void RetrieveCloseoutRecord(CloseoutDO closeoutDO, int siteIndex)
	{
		closeoutDO.ManagerIndex = this.managerIndex;
		closeoutDO.ProductIndex = this.productIndex;
		closeoutDO.SiteIndex = siteIndex;

		string sql = closeoutDO.GetCurrentCloseoutSelectSQL();
		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(sql, connection);

		command.Parameters.Add("@LedgerStartDate", System.Data.SqlDbType.DateTime);
		command.Parameters["@LedgerStartDate"].Value = this.dateConverter.GetDateWithCorrectTimePortion(this.beginDate, DateConverter.TimeTypes.START);

		if (this.managerIndex > 0)
		{
			command.Parameters.Add("@ManagerIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ManagerIndex"].Value = this.managerIndex;
		}

		if (this.productIndex > 0)
		{
			command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ProductIndex"].Value = this.productIndex;
		}

		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
			command.Parameters["@SiteIndex"].Value = siteIndex;
		}

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load current closeout record.
		closeoutDO.LoadCloseout(dataSet);

		closeoutDO.BookInventory.Gross *= this.volumeFactor;
		closeoutDO.BookInventory.Net *= this.volumeFactor;
		closeoutDO.BookInventory.Mass *= this.massFactor;
		closeoutDO.BookInventory.GrossPrice *= this.volumeFactor;
		closeoutDO.BookInventory.NetPrice *= this.volumeFactor;
		closeoutDO.BookInventory.MassPrice *= this.massFactor;

		Math.Round(closeoutDO.BookInventory.Gross, (int)this.volumePrecision, MidpointRounding.AwayFromZero);
		Math.Round(closeoutDO.BookInventory.Net, (int)this.volumePrecision, MidpointRounding.AwayFromZero);
		Math.Round(closeoutDO.BookInventory.Mass, (int)this.massPrecision, MidpointRounding.AwayFromZero);
	}

	/// <summary>
	/// This method will return the Closeout DO with the most recent closeout date based on the
	/// manager, product, site, and the closeout that is great than or equal to the ledger
	/// start date.
	/// </summary>
	/// <returns></returns>
	private void RetrieveMostRecentCloseoutDate(int siteIndex, CloseoutDO closeoutDO, ArrayList arrayOfCloseoutDates)
	{
		string sql = closeoutDO.GetLatestCloseoutDateSelectSQL();
		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(sql, connection);

		command.Parameters.Add("@LedgerStartDate", System.Data.SqlDbType.DateTime);
		command.Parameters["@LedgerStartDate"].Value = this.dateConverter.GetDateWithCorrectTimePortion(this.beginDate, DateConverter.TimeTypes.START);

		if (this.managerIndex > 0)
		{
			command.Parameters.Add("@ManagerIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ManagerIndex"].Value = this.managerIndex;
		}
		if (this.productIndex > 0)
		{
			command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
			command.Parameters["@ProductIndex"].Value = this.productIndex;
		}
		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
			command.Parameters["@SiteIndex"].Value = siteIndex;
		}

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load current closeout date.
		closeoutDO.LoadLatestCloseoutDate(dataSet);

		if ((closeoutDO.CloseoutDateString != null) && (closeoutDO.CloseoutDateString.Length > 0))
		{
			arrayOfCloseoutDates.Add(closeoutDO.CloseoutDateString);
		}
		else
		{
			arrayOfCloseoutDates.Add(null);
		}
	}


	/// <summary>
	/// This method will return the Closeout DO with the most recent closeout date based on the
	/// manager, product, site, and the closeout that is great than or equal to the ledger
	/// start date.
	/// </summary>
	/// <returns></returns>
	private void RetrieveMostRecentBrokenBlendDate(int siteIndex, CloseoutDO closeoutDO, ArrayList arrayOfBrokenBlendDates)
	{
		string sql = closeoutDO.GetBrokenBlendDateSelectSQL();
		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(sql, connection);

		command.Parameters.Add("@LedgerEndDate", System.Data.SqlDbType.DateTime);
		command.Parameters["@LedgerEndDate"].Value = this.dateConverter.GetDateWithCorrectTimePortion(this.endDate, DateConverter.TimeTypes.END);

		command.Parameters.Add("@LastCloseoutDate", System.Data.SqlDbType.DateTime);
		command.Parameters["@LastCloseoutDate"].Value = this.dateConverter.GetDateWithCorrectTimePortion(closeoutDO.CloseoutDate, DateConverter.TimeTypes.START);

		command.Parameters.Add("@ManagerIndex", System.Data.SqlDbType.Int);
		command.Parameters["@ManagerIndex"].Value = 0;

		command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
		command.Parameters["@ProductIndex"].Value = 0;

		command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
		command.Parameters["@SiteIndex"].Value = 0;

		if (this.managerIndex > 0)
		{
			command.Parameters["@ManagerIndex"].Value = this.managerIndex;
		}

		if (this.productIndex > 0)
		{
			command.Parameters["@ProductIndex"].Value = this.productIndex;
		}

		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			command.Parameters["@SiteIndex"].Value = siteIndex;
		}

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		closeoutDO.LoadBrokenBlendDate(dataSet);


		if ((closeoutDO.BrokenBlendDateString != null) && (closeoutDO.BrokenBlendDateString.Length > 0))
		{
			arrayOfBrokenBlendDates.Add(closeoutDO.BrokenBlendDateString);
		}
		else
		{
			arrayOfBrokenBlendDates.Add(null);
		}
	}
	#endregion

	#region Find if the system is BSME
	/// <summary>
	/// This method will return true if the physical inventory occurred on the
	/// last day of the month. It will return false if it does not.
	/// </summary>
	/// <param name="physicalDate"></param>
	/// <returns></returns>
	private bool IsPhysicalOnTheLastDayOfTheMonth(DateTime physicalDate)
	{
		bool physicalOnLastDay = false;

		switch (physicalDate.Month)
		{
			case 1: // January
				if (physicalDate.Day == 31)
				{
					physicalOnLastDay = true;
				}
				break;
			case 2: // February
				DateTime leap = new DateTime(physicalDate.Year, 3, 1, 0, 0, 0);
				leap = leap.AddDays(-1);
				int lastDay = leap.Day;

				if (physicalDate.Day == lastDay)
				{
					physicalOnLastDay = true;
				}
				break;
			case 3: // March
				if (physicalDate.Day == 31)
				{
					physicalOnLastDay = true;
				}
				break;
			case 4: //April
				if (physicalDate.Day == 30)
				{
					physicalOnLastDay = true;
				}
				break;
			case 5: // May
				if (physicalDate.Day == 31)
				{
					physicalOnLastDay = true;
				}
				break;
			case 6: // June
				if (physicalDate.Day == 30)
				{
					physicalOnLastDay = true;
				}
				break;
			case 7: // July
				if (physicalDate.Day == 31)
				{
					physicalOnLastDay = true;
				}
				break;
			case 8: // August
				if (physicalDate.Day == 31)
				{
					physicalOnLastDay = true;
				}
				break;
			case 9: // September
				if (physicalDate.Day == 30)
				{
					physicalOnLastDay = true;
				}
				break;
			case 10: // October
				if (physicalDate.Day == 31)
				{
					physicalOnLastDay = true;
				}
				break;
			case 11: // November
				if (physicalDate.Day == 30)
				{
					physicalOnLastDay = true;
				}
				break;
			case 12: // December
				if (physicalDate.Day == 31)
				{
					physicalOnLastDay = true;
				}
				break;
		}

		return physicalOnLastDay;
	}
	#endregion

	#region Retrieve Site Info
	/// <summary>
	/// This method will retrieve a site list and site information
	/// for the current and any children sites.
	/// </summary>
	/// <returns></returns>
	private SiteListDO RetrieveSites(int inSiteIndex)
	{
		// Retrieve a list of sites based on the given site index.
		// If the site is not a parent site, then it only returns one
		// in the list.
		SiteListDO siteListDO = new SiteListDO();
		siteListDO.RetrieveSiteList(connection, inSiteIndex);

		// Loop through the site list and get additional site information,
		// such as conversion factor, decimal places, and group flag.
		IDictionaryEnumerator enumerator = siteListDO.SiteList.GetEnumerator();
		while (enumerator.MoveNext() == true)
		{
			SiteDO siteDO = (SiteDO)enumerator.Value;
			siteDO.RetrieveSiteInfo(connection, siteDO.SiteIndex);
		}

		return siteListDO;
	}

	/// <summary>
	/// This method will loop through the list of sites looking for a site group
	/// that is set to inhibit ledger rollup. If found, then all the children
	/// sites under that site group will have the inhibit ledger rollup flag set
	/// to inhibit rollup.
	/// </summary>
	/// <param name="siteListDO"></param>
	private void InhibitChildSitesRollupBasedOnParent(SiteListDO origSiteListDO)
	{
		Hashtable origSiteList = origSiteListDO.SiteList;
		IDictionaryEnumerator origSiteListEnumerator = origSiteListDO.SiteList.GetEnumerator();

		while (origSiteListEnumerator.MoveNext() == true)
		{
			SiteDO siteDO = (SiteDO)origSiteListEnumerator.Value;

			if ((siteDO != null) &&
				 (siteDO.SiteGroupFlag == true) &&
				 (siteDO.InhibitSiteLedgerRollup == true) &&
				 (siteDO.SiteIndex != this.siteIndex))
			{
				SiteListDO newSiteListDO = this.RetrieveSites(siteDO.SiteIndex);
				IDictionaryEnumerator newSiteListEnumerator = newSiteListDO.SiteList.GetEnumerator();

				while (newSiteListEnumerator.MoveNext() == true)
				{
					SiteDO newSiteDO = (SiteDO)newSiteListEnumerator.Value;

					if ((newSiteDO != null) && (origSiteList.Contains(newSiteDO.SiteName) == true))
					{
						SiteDO updateSiteDO = origSiteList[newSiteDO.SiteName] as SiteDO;

						if (updateSiteDO != null)
						{
							updateSiteDO.InhibitSiteLedgerRollup = true;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// This method will return the current site information.
	/// </summary>
	/// <returns></returns>
	private SiteDO RetrieveCurrentSite()
	{
		// Retrieve the current site information
		SiteDO siteDO = new SiteDO();
		siteDO.RetrieveSiteInfo(connection, this.siteIndex);

		return siteDO;
	}
	#endregion

	#region Retrieve Product Info
	/// <summary>
	/// This method will return the current site information.
	/// </summary>
	/// <returns></returns>
	private ProductDO RetrieveProductInfo()
	{
		// Use the product conversion factor and precision if the product is configured to
		// have them.
		ProductDO productDO = new ProductDO();
		productDO.ProductIndex = this.productIndex;
		productDO.RetrieveProductInfo(connection);

		return productDO;
	}
	#endregion


	#region LoadInventoryData
	/// <summary>
	/// This method will load the inventory data in a collection of Inventory Line 
	/// Item Data Objects.
	/// </summary>
	/// <param name="dataSetList"></param>
	/// <param name="lineItemsList"></param>
	public void LoadInventoryData(SortedList inventorySummation, ref LedgerLineItemCollection lineItemsList)
	{
		IDictionaryEnumerator enumerator = inventorySummation.GetEnumerator();

		// Set the data record row values and send it out on the pipe.
		InventoryDailyAliasDO inventoryDailyAlias = null;
		InventoryLineItemDO ledgerLineItem = null;
		string currentDate = "";
		QuantityDO quantity;
		while (enumerator.MoveNext() == true)
		{
			inventoryDailyAlias = (InventoryDailyAliasDO)enumerator.Value;

			string inventoryDate = inventoryDailyAlias.InventoryDateStr;


			// The date returned from the database is formatted as YYYY/MM/DD.  The code below
			// formats the date as MM/DD/YYYY
			char[] separatorList = { '/' };
			string[] stringList = inventoryDate.Split(separatorList);
			inventoryDate = stringList[1] + "/" + stringList[2] + "/" + stringList[0];


			// This is the first row and the inventory date is not empty (it's a string not a datetime) 
			if (inventoryDate != currentDate)
			{
				ledgerLineItem = new InventoryLineItemDO();
				lineItemsList.Add(ledgerLineItem);

				ledgerLineItem.InventoryDate = inventoryDate;
				currentDate = inventoryDate;
			}

			string aliasName = inventoryDailyAlias.AliasName;
			double grossQuantity = inventoryDailyAlias.GrossQuantity;
			double grossPrice = inventoryDailyAlias.GrossPrice;
			double netQuantity = inventoryDailyAlias.NetQuantity;
			double netPrice = inventoryDailyAlias.NetPrice;
			double massQuantity = inventoryDailyAlias.MassQuantity;
			double massPrice = inventoryDailyAlias.MassPrice;
			Int64 maxTransVersion = inventoryDailyAlias.MaxTransVersion;

			double packageQuantity = 0;
			if (loadByWeight)
			{
				if (massPackageSize != 0)
					packageQuantity = massQuantity / massPackageSize;
			}
			else
			{
				if (volumePackageSize != 0)
					packageQuantity = netQuantity / volumePackageSize;
			}

			double number01 = inventoryDailyAlias.Number01;
			double number02 = inventoryDailyAlias.Number02;
			double number03 = inventoryDailyAlias.Number03;
			double number04 = inventoryDailyAlias.Number04;
			double number05 = inventoryDailyAlias.Number05;
			double number06 = inventoryDailyAlias.Number06;
			bool errorFlag = inventoryDailyAlias.ErrorFlag;

			// The following aggregates the volumes (quantities and prices) for each trans alias
			// for a given day.
			if (ledgerLineItem.QuantityList.Contains(aliasName) == false)
			{
				// Create the quantity (quantities and prices) for the alias name returned
				// from the db
				quantity = new QuantityDO(grossQuantity,
													netQuantity,
													massQuantity,
													packageQuantity,
													grossPrice,
													netPrice,
													massPrice,
													number01,
													number02,
													number03,
													number04,
													number05,
													number06,
													errorFlag);
				ledgerLineItem.QuantityList.Add(aliasName, quantity);
			}
			else
			{
				// The line item already has an entry for the volume so add to it
				quantity = ledgerLineItem.QuantityList[aliasName] as QuantityDO;
				quantity.GrossInventoryChange += grossQuantity;
				quantity.NetInventoryChange += netQuantity;
				quantity.MassInventoryChange += massQuantity;
				quantity.GrossPriceInventoryChange += grossPrice;
				quantity.NetPriceInventoryChange += netPrice;
				quantity.MassPriceInventoryChange += massPrice;
				quantity.Number01Change += number01;
				quantity.Number02Change += number02;
				quantity.Number03Change += number03;
				quantity.Number04Change += number04;
				quantity.Number05Change += number05;
				quantity.Number06Change += number06;
				quantity.OrErrorFlag(errorFlag);
			}

			ledgerLineItem.Site = inventoryDailyAlias.Site;

			if (inventoryDailyAlias.MaxTransVersion > ledgerLineItem.MaxTransVersion)
				ledgerLineItem.MaxTransVersion = inventoryDailyAlias.MaxTransVersion;

			string transTypeIDStr = inventoryDailyAlias.TransTypeID.ToString();
			TransactionAliases.TransactionTypes transType = (TransactionAliases.TransactionTypes)Convert.ToInt32(transTypeIDStr);

			//Check Transaction Alias to see if it is a type that affects inventory.
			switch (transType)
			{
				case TransactionAliases.TransactionTypes.T7_FillStand:
				case TransactionAliases.TransactionTypes.T9_Request:
				case TransactionAliases.TransactionTypes.T10_Unload:
				case TransactionAliases.TransactionTypes.T11_ConsumerTransfer:
				case TransactionAliases.TransactionTypes.T12_Type12:
				case TransactionAliases.TransactionTypes.T17_Order:
				case TransactionAliases.TransactionTypes.T18_SupplyOrder:
				case TransactionAliases.TransactionTypes.T19_EndOfDay:
				case TransactionAliases.TransactionTypes.T20_EndOfMonth:
				case TransactionAliases.TransactionTypes.T21_AccountPayableInvoice:
				case TransactionAliases.TransactionTypes.T22_AccountReceivableInvoice:
				case TransactionAliases.TransactionTypes.T23_StorageTransfer:
					{
						quantity.AffectsInventory = false;
						break;
					}
				case TransactionAliases.TransactionTypes.T14_PhysicalInventory:
					{
						quantity.AffectsInventory = false;
						ledgerLineItem.HasPhysicalInventory = true;
						break;
					}
				default:
					{
						quantity.AffectsInventory = true;
						break;
					}
			}
		}
	}

	#endregion

	#region Return data to client
	/// <summary>
	/// This method will send the ledger data to the client via SQL Pipe.
	/// </summary>
	/// <param name="finalLedgerLineItemCollection"></param>
	private void SendDataToClient(LedgerLineItemCollection finalLedgerLineItemCollection)
	{
		string aliasName = "";
		TransactionAliasDO transAliasDO = null;
		SortedList aliasColumnKey = new SortedList();

		// Determine the number of columns by the number of aliases and
		// 11 static columns.
		int columnCount = this.transAliasListDO.aliasList.Count;
		columnCount += 11;

		// Create the columns list based on the column count.
		List<SqlMetaData> columns = new List<SqlMetaData>(columnCount);

		// Create a column for the 11 static columns
		SqlMetaData outputColumn01 = new SqlMetaData("InventoryDate", SqlDbType.NVarChar, 50);
		SqlMetaData outputColumn02 = new SqlMetaData("BeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn03 = new SqlMetaData("BookInventory", SqlDbType.Float);
		SqlMetaData outputColumn04 = new SqlMetaData("Variance", SqlDbType.Float);
		SqlMetaData outputColumn05 = new SqlMetaData("TotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn06 = new SqlMetaData("TotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn07 = new SqlMetaData("Tolerance", SqlDbType.Float);
		SqlMetaData outputColumn08 = new SqlMetaData("AllowableGainLoss", SqlDbType.Float);
		SqlMetaData outputColumn09 = new SqlMetaData("VariancePercentage", SqlDbType.Float);
		SqlMetaData outputColumn10 = new SqlMetaData("ToleranceTestedVolume", SqlDbType.Float);
		SqlMetaData outputColumn11 = new SqlMetaData("MaxTransVersion", SqlDbType.BigInt);

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

		int columnKeyCount = 11;

		// Create a column for each alias
		IDictionaryEnumerator enumerator = this.transAliasListDO.aliasSortedList.GetEnumerator();
		while (enumerator.MoveNext() == true)
		{
			transAliasDO = (TransactionAliasDO)enumerator.Value;

			SqlMetaData outputColumn = new SqlMetaData(transAliasDO.AliasName, SqlDbType.Float);
			columns.Add(outputColumn);

			if (aliasColumnKey.Contains(transAliasDO.AliasName) == false)
			{
				aliasColumnKey.Add(transAliasDO.AliasName, columnKeyCount);
				columnKeyCount++;
			}
		}

		// Create the columns for the data record.
		SqlDataRecord record = new SqlDataRecord(columns.ToArray());
		SqlContext.Pipe.SendResultsStart(record);

		// Create the data for each previously defined column.
		foreach (InventoryLineItemDO lineItem in finalLedgerLineItemCollection)
		{
			record.SetValue(0, lineItem.InventoryDate);
			record.SetValue(1, lineItem.BeginInventory.GrossInventoryChange);
			record.SetValue(2, lineItem.BookInventory.GrossInventoryChange);
			record.SetValue(3, lineItem.Variance.GrossInventoryChange);
			record.SetValue(4, lineItem.TotalVariance.GrossInventoryChange);
			record.SetValue(5, lineItem.TotalPhysicalInventory.GrossInventoryChange);
			record.SetValue(6, lineItem.Tolerance);
			record.SetValue(7, lineItem.AllowableGainLoss.GrossInventoryChange);
			record.SetValue(8, lineItem.VariancePercentage.GrossInventoryChange);
			record.SetValue(9, lineItem.ToleranceTestedQuantity.GrossInventoryChange);
			record.SetValue(10, lineItem.MaxTransVersion);

			IDictionaryEnumerator lineItemEnum = lineItem.QuantityList.GetEnumerator();
			while (lineItemEnum.MoveNext() == true)
			{
				aliasName = (string)lineItemEnum.Key;
				QuantityDO quantity = (QuantityDO)lineItemEnum.Value;
				int columnKey = (int)aliasColumnKey[aliasName];

				record.SetValue(columnKey, quantity.GrossInventoryChange);
			}

			// Send record to client.
			SqlContext.Pipe.SendResultsRow(record);
		}

		// Close the pipe.
		SqlContext.Pipe.SendResultsEnd();
	}

	/// <summary>
	/// This method will send the ledger data to the client via SQL Pipe.
	/// </summary>
	/// <param name="finalLedgerLineItemCollection"></param>
	private void SendDataToGUIClient(LedgerLineItemCollection finalLedgerLineItemCollection)
	{
		TransactionAliasDO transAliasDO = null;
		SortedList aliasColumnKey = new SortedList();

		// Determine the number of columns by the number of aliases and multiply by 15 
		// since there are 15 volumes (Gross, Net, Mass, Package, Gross Price, Net Price, Mass Price, Number01 - Number06), a flag field,
		// and a moniker field being sent back.  In addition there are 51 static columns.
		int columnCount = this.transAliasListDO.aliasList.Count * 15;
		columnCount += 64;

		// Create the columns list based on the column count.
		List<SqlMetaData> columns = new List<SqlMetaData>(columnCount);

		// Create a column for the 64 static columns
		SqlMetaData outputColumn01 = new SqlMetaData("InventoryDate", SqlDbType.NVarChar, 50);
		SqlMetaData outputColumn02 = new SqlMetaData("gvBeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn03 = new SqlMetaData("nvBeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn04 = new SqlMetaData("mBeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn05 = new SqlMetaData("pBeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn06 = new SqlMetaData("gpBeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn07 = new SqlMetaData("npBeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn08 = new SqlMetaData("mpBeginInventory", SqlDbType.Float);
		SqlMetaData outputColumn09 = new SqlMetaData("gvBookInventory", SqlDbType.Float);
		SqlMetaData outputColumn10 = new SqlMetaData("nvBookInventory", SqlDbType.Float);
		SqlMetaData outputColumn11 = new SqlMetaData("mBookInventory", SqlDbType.Float);
		SqlMetaData outputColumn12 = new SqlMetaData("pBookInventory", SqlDbType.Float);
		SqlMetaData outputColumn13 = new SqlMetaData("gpBookInventory", SqlDbType.Float);
		SqlMetaData outputColumn14 = new SqlMetaData("npBookInventory", SqlDbType.Float);
		SqlMetaData outputColumn15 = new SqlMetaData("mpBookInventory", SqlDbType.Float);
		SqlMetaData outputColumn16 = new SqlMetaData("gvVariance", SqlDbType.Float);
		SqlMetaData outputColumn17 = new SqlMetaData("nvVariance", SqlDbType.Float);
		SqlMetaData outputColumn18 = new SqlMetaData("mVariance", SqlDbType.Float);
		SqlMetaData outputColumn19 = new SqlMetaData("pVariance", SqlDbType.Float);
		SqlMetaData outputColumn20 = new SqlMetaData("gpVariance", SqlDbType.Float);
		SqlMetaData outputColumn21 = new SqlMetaData("npVariance", SqlDbType.Float);
		SqlMetaData outputColumn22 = new SqlMetaData("mpVariance", SqlDbType.Float);
		SqlMetaData outputColumn23 = new SqlMetaData("gvTotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn24 = new SqlMetaData("nvTotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn25 = new SqlMetaData("mTotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn26 = new SqlMetaData("pTotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn27 = new SqlMetaData("gpTotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn28 = new SqlMetaData("npTotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn29 = new SqlMetaData("mpTotalVariance", SqlDbType.Float);
		SqlMetaData outputColumn30 = new SqlMetaData("gvTotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn31 = new SqlMetaData("nvTotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn32 = new SqlMetaData("mTotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn33 = new SqlMetaData("pTotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn34 = new SqlMetaData("gpTotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn35 = new SqlMetaData("npTotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn36 = new SqlMetaData("mpTotalPhysical", SqlDbType.Float);
		SqlMetaData outputColumn37 = new SqlMetaData("gvTotalActivity", SqlDbType.Float);
		SqlMetaData outputColumn38 = new SqlMetaData("nvTotalActivity", SqlDbType.Float);
		SqlMetaData outputColumn39 = new SqlMetaData("mTotalActivity", SqlDbType.Float);
		SqlMetaData outputColumn40 = new SqlMetaData("pTotalActivity", SqlDbType.Float);
		SqlMetaData outputColumn41 = new SqlMetaData("gpTotalActivity", SqlDbType.Float);
		SqlMetaData outputColumn42 = new SqlMetaData("npTotalActivity", SqlDbType.Float);
		SqlMetaData outputColumn43 = new SqlMetaData("mpTotalActivity", SqlDbType.Float);
		SqlMetaData outputColumn44 = new SqlMetaData("gvTotalMovement", SqlDbType.Float);
		SqlMetaData outputColumn45 = new SqlMetaData("nvTotalMovement", SqlDbType.Float);
		SqlMetaData outputColumn46 = new SqlMetaData("mTotalMovement", SqlDbType.Float);
		SqlMetaData outputColumn47 = new SqlMetaData("pTotalMovement", SqlDbType.Float);
		SqlMetaData outputColumn48 = new SqlMetaData("gpTotalMovement", SqlDbType.Float);
		SqlMetaData outputColumn49 = new SqlMetaData("npTotalMovement", SqlDbType.Float);
		SqlMetaData outputColumn50 = new SqlMetaData("mpTotalMovement", SqlDbType.Float);
		SqlMetaData outputColumn51 = new SqlMetaData("LineItemStatusFlags", SqlDbType.Int);
		SqlMetaData outputColumn52 = new SqlMetaData("tolerance", SqlDbType.Float);
		SqlMetaData outputColumn53 = new SqlMetaData("gvAllowableGainLoss", SqlDbType.Float);
		SqlMetaData outputColumn54 = new SqlMetaData("nvAllowableGainLoss", SqlDbType.Float);
		SqlMetaData outputColumn55 = new SqlMetaData("mAllowableGainLoss", SqlDbType.Float);
		SqlMetaData outputColumn56 = new SqlMetaData("pAllowableGainLoss", SqlDbType.Float);
		SqlMetaData outputColumn57 = new SqlMetaData("gvVariancePercentage", SqlDbType.Float);
		SqlMetaData outputColumn58 = new SqlMetaData("nvVariancePercentage", SqlDbType.Float);
		SqlMetaData outputColumn59 = new SqlMetaData("mVariancePercentage", SqlDbType.Float);
		SqlMetaData outputColumn60 = new SqlMetaData("pVariancePercentage", SqlDbType.Float);
		SqlMetaData outputColumn61 = new SqlMetaData("gvToleranceTestedVolume", SqlDbType.Float);
		SqlMetaData outputColumn62 = new SqlMetaData("nvToleranceTestedVolume", SqlDbType.Float);
		SqlMetaData outputColumn63 = new SqlMetaData("mToleranceTestedVolume", SqlDbType.Float);
		SqlMetaData outputColumn64 = new SqlMetaData("pToleranceTestedVolume", SqlDbType.Float);

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
		columns.Add(outputColumn16);
		columns.Add(outputColumn17);
		columns.Add(outputColumn18);
		columns.Add(outputColumn19);
		columns.Add(outputColumn20);
		columns.Add(outputColumn21);
		columns.Add(outputColumn22);
		columns.Add(outputColumn23);
		columns.Add(outputColumn24);
		columns.Add(outputColumn25);
		columns.Add(outputColumn26);
		columns.Add(outputColumn27);
		columns.Add(outputColumn28);
		columns.Add(outputColumn29);
		columns.Add(outputColumn30);
		columns.Add(outputColumn31);
		columns.Add(outputColumn32);
		columns.Add(outputColumn33);
		columns.Add(outputColumn34);
		columns.Add(outputColumn35);
		columns.Add(outputColumn36);
		columns.Add(outputColumn37);
		columns.Add(outputColumn38);
		columns.Add(outputColumn39);
		columns.Add(outputColumn40);
		columns.Add(outputColumn41);
		columns.Add(outputColumn42);
		columns.Add(outputColumn43);
		columns.Add(outputColumn44);
		columns.Add(outputColumn45);
		columns.Add(outputColumn46);
		columns.Add(outputColumn47);
		columns.Add(outputColumn48);
		columns.Add(outputColumn49);
		columns.Add(outputColumn50);
		columns.Add(outputColumn51);
		columns.Add(outputColumn52);
		columns.Add(outputColumn53);
		columns.Add(outputColumn54);
		columns.Add(outputColumn55);
		columns.Add(outputColumn56);
		columns.Add(outputColumn57);
		columns.Add(outputColumn58);
		columns.Add(outputColumn59);
		columns.Add(outputColumn60);
		columns.Add(outputColumn61);
		columns.Add(outputColumn62);
		columns.Add(outputColumn63);
		columns.Add(outputColumn64);

		columns.Add(new SqlMetaData("flagsBeginInventory", SqlDbType.Int));
		columns.Add(new SqlMetaData("flagsBookInventory", SqlDbType.Int));
		columns.Add(new SqlMetaData("flagsTotalPhysicalInventory", SqlDbType.Int));
		columns.Add(new SqlMetaData("flagsTotalVariance", SqlDbType.Int));
		columns.Add(new SqlMetaData("flagsVariance", SqlDbType.Int));
		columns.Add(new SqlMetaData("flagsTotalActivity", SqlDbType.Int));

		int columnKeyCount = columns.Count;

		// Create a column for each alias
		IDictionaryEnumerator enumerator = this.transAliasListDO.aliasSortedList.GetEnumerator();
		while (enumerator.MoveNext() == true)
		{
			transAliasDO = (TransactionAliasDO)enumerator.Value;

			string[] aliasNames = {"gv" + transAliasDO.AliasName,  // Gross volume + alias name
										  "nv" + transAliasDO.AliasName,  // Net volume + alias name
										  "m" + transAliasDO.AliasName,   // Mass + alias name
										  "p" + transAliasDO.AliasName,   // Package + alias name
										  "gp" + transAliasDO.AliasName,  // Gross price + alias name
										  "np" + transAliasDO.AliasName,  // Net price + alias name
										  "mp" + transAliasDO.AliasName,  // Mass price + alias name
										  "fl" + transAliasDO.AliasName,  // Cell flag + alias name
										  "n1" + transAliasDO.AliasName,  // Number01 volume + aliasName
										  "n2" + transAliasDO.AliasName,  // Number02 volume + aliasName
										  "n3" + transAliasDO.AliasName,  // Number03 volume + aliasName
										  "n4" + transAliasDO.AliasName,  // Number04 volume + aliasName
										  "n5" + transAliasDO.AliasName,  // Number05 volume + aliasName
										  "n6" + transAliasDO.AliasName,  // Number06 volume + aliasName
										  "mk" + transAliasDO.AliasName   // Moniker for the alias column
										 };

			SqlMetaData outputCol1 = new SqlMetaData(aliasNames[0], SqlDbType.Float);
			SqlMetaData outputCol2 = new SqlMetaData(aliasNames[1], SqlDbType.Float);
			SqlMetaData outputCol3 = new SqlMetaData(aliasNames[2], SqlDbType.Float);
			SqlMetaData outputCol4 = new SqlMetaData(aliasNames[3], SqlDbType.Float);
			SqlMetaData outputCol5 = new SqlMetaData(aliasNames[4], SqlDbType.Float);
			SqlMetaData outputCol6 = new SqlMetaData(aliasNames[5], SqlDbType.Float);
			SqlMetaData outputCol7 = new SqlMetaData(aliasNames[6], SqlDbType.Float);
			SqlMetaData outputCol8 = new SqlMetaData(aliasNames[7], SqlDbType.Int);

			// Number fields
			SqlMetaData outputCol9 = new SqlMetaData(aliasNames[8], SqlDbType.Float);
			SqlMetaData outputCol10 = new SqlMetaData(aliasNames[9], SqlDbType.Float);
			SqlMetaData outputCol11 = new SqlMetaData(aliasNames[10], SqlDbType.Float);
			SqlMetaData outputCol12 = new SqlMetaData(aliasNames[11], SqlDbType.Float);
			SqlMetaData outputCol13 = new SqlMetaData(aliasNames[12], SqlDbType.Float);
			SqlMetaData outputCol14 = new SqlMetaData(aliasNames[13], SqlDbType.Float);

			// Moniker column
			SqlMetaData outputCol15 = new SqlMetaData(aliasNames[14], SqlDbType.NVarChar, 50);

			columns.Add(outputCol1);
			columns.Add(outputCol2);
			columns.Add(outputCol3);
			columns.Add(outputCol4);
			columns.Add(outputCol5);
			columns.Add(outputCol6);
			columns.Add(outputCol7);
			columns.Add(outputCol8);
			columns.Add(outputCol9);
			columns.Add(outputCol10);
			columns.Add(outputCol11);
			columns.Add(outputCol12);
			columns.Add(outputCol13);
			columns.Add(outputCol14);
			columns.Add(outputCol15);

			foreach (string aliasName in aliasNames)
			{
				if (aliasColumnKey.Contains(aliasName) == false)
				{
					aliasColumnKey.Add(aliasName, columnKeyCount);
					columnKeyCount++;
				}
			}
		}

		// Create the columns for the data record.
		SqlDataRecord record = new SqlDataRecord(columns.ToArray());
		SqlContext.Pipe.SendResultsStart(record);

		// Create the data for each previously defined column.
		foreach (InventoryLineItemDO lineItem in finalLedgerLineItemCollection)
		{
			record.SetValue(0, lineItem.InventoryDate);

			record.SetValue(1, lineItem.BeginInventory.GrossInventoryChange);
			record.SetValue(2, lineItem.BeginInventory.NetInventoryChange);
			record.SetValue(3, lineItem.BeginInventory.MassInventoryChange);
			record.SetValue(4, lineItem.BeginInventory.PackageInventoryChange);
			record.SetValue(5, lineItem.BeginInventory.GrossPriceInventoryChange);
			record.SetValue(6, lineItem.BeginInventory.NetPriceInventoryChange);
			record.SetValue(7, lineItem.BeginInventory.MassPriceInventoryChange);

			record.SetValue(8, lineItem.BookInventory.GrossInventoryChange);
			record.SetValue(9, lineItem.BookInventory.NetInventoryChange);
			record.SetValue(10, lineItem.BookInventory.MassInventoryChange);
			record.SetValue(11, lineItem.BookInventory.PackageInventoryChange);
			record.SetValue(12, lineItem.BookInventory.GrossPriceInventoryChange);
			record.SetValue(13, lineItem.BookInventory.NetPriceInventoryChange);
			record.SetValue(14, lineItem.BookInventory.MassPriceInventoryChange);

			record.SetValue(15, lineItem.Variance.GrossInventoryChange);
			record.SetValue(16, lineItem.Variance.NetInventoryChange);
			record.SetValue(17, lineItem.Variance.MassInventoryChange);
			record.SetValue(18, lineItem.Variance.PackageInventoryChange);
			record.SetValue(19, lineItem.Variance.GrossPriceInventoryChange);
			record.SetValue(20, lineItem.Variance.NetPriceInventoryChange);
			record.SetValue(21, lineItem.Variance.MassPriceInventoryChange);

			record.SetValue(22, lineItem.TotalVariance.GrossInventoryChange);
			record.SetValue(23, lineItem.TotalVariance.NetInventoryChange);
			record.SetValue(24, lineItem.TotalVariance.MassInventoryChange);
			record.SetValue(25, lineItem.TotalVariance.PackageInventoryChange);
			record.SetValue(26, lineItem.TotalVariance.GrossPriceInventoryChange);
			record.SetValue(27, lineItem.TotalVariance.NetPriceInventoryChange);
			record.SetValue(28, lineItem.TotalVariance.MassPriceInventoryChange);

			record.SetValue(29, lineItem.TotalPhysicalInventory.GrossInventoryChange);
			record.SetValue(30, lineItem.TotalPhysicalInventory.NetInventoryChange);
			record.SetValue(31, lineItem.TotalPhysicalInventory.MassInventoryChange);
			record.SetValue(32, lineItem.TotalPhysicalInventory.PackageInventoryChange);
			record.SetValue(33, lineItem.TotalPhysicalInventory.GrossPriceInventoryChange);
			record.SetValue(34, lineItem.TotalPhysicalInventory.NetPriceInventoryChange);
			record.SetValue(35, lineItem.TotalPhysicalInventory.MassPriceInventoryChange);

			record.SetValue(36, lineItem.TotalActivity.GrossInventoryChange);
			record.SetValue(37, lineItem.TotalActivity.NetInventoryChange);
			record.SetValue(38, lineItem.TotalActivity.MassInventoryChange);
			record.SetValue(39, lineItem.TotalActivity.PackageInventoryChange);
			record.SetValue(40, lineItem.TotalActivity.GrossPriceInventoryChange);
			record.SetValue(41, lineItem.TotalActivity.NetPriceInventoryChange);
			record.SetValue(42, lineItem.TotalActivity.MassPriceInventoryChange);

			record.SetValue(43, lineItem.TotalMovement.GrossInventoryChange);
			record.SetValue(44, lineItem.TotalMovement.NetInventoryChange);
			record.SetValue(45, lineItem.TotalMovement.MassInventoryChange);
			record.SetValue(46, lineItem.TotalMovement.MassInventoryChange);
			record.SetValue(47, lineItem.TotalMovement.GrossPriceInventoryChange);
			record.SetValue(48, lineItem.TotalMovement.NetPriceInventoryChange);
			record.SetValue(49, lineItem.TotalMovement.MassPriceInventoryChange);

			BaseInventoryLineItemDO.Status flags = lineItem.Flags.Flags;
			int lineItemStatusFlags = Convert.ToInt32(flags);
			record.SetValue(50, lineItemStatusFlags);

			record.SetValue(51, lineItem.Tolerance);
			record.SetValue(52, lineItem.AllowableGainLoss.GrossInventoryChange);
			record.SetValue(53, lineItem.AllowableGainLoss.NetInventoryChange);
			record.SetValue(54, lineItem.AllowableGainLoss.MassInventoryChange);
			record.SetValue(55, lineItem.AllowableGainLoss.PackageInventoryChange);
			record.SetValue(56, lineItem.VariancePercentage.GrossInventoryChange);
			record.SetValue(57, lineItem.VariancePercentage.NetInventoryChange);
			record.SetValue(58, lineItem.VariancePercentage.MassInventoryChange);
			record.SetValue(59, lineItem.VariancePercentage.PackageInventoryChange);
			record.SetValue(60, lineItem.ToleranceTestedQuantity.GrossInventoryChange);
			record.SetValue(61, lineItem.ToleranceTestedQuantity.NetInventoryChange);
			record.SetValue(62, lineItem.ToleranceTestedQuantity.MassInventoryChange);
			record.SetValue(63, lineItem.ToleranceTestedQuantity.PackageInventoryChange);

			record.SetValue(64, Convert.ToInt32(lineItem.GetCellFlags("Begin Inventory").Flags));
			record.SetValue(65, Convert.ToInt32(lineItem.GetCellFlags("Book Inventory").Flags));
			record.SetValue(66, Convert.ToInt32(lineItem.GetCellFlags("Total Physical Inventory").Flags));
			record.SetValue(67, Convert.ToInt32(lineItem.GetCellFlags("Total Variance").Flags));
			record.SetValue(68, Convert.ToInt32(lineItem.GetCellFlags("Variance").Flags));
			record.SetValue(69, Convert.ToInt32(lineItem.GetCellFlags("Total Activity").Flags));

			IDictionaryEnumerator lineItemEnum = lineItem.QuantityList.GetEnumerator();
			while (lineItemEnum.MoveNext() == true)
			{
				string aliasKeyName1 = "gv" + (string)lineItemEnum.Key;
				string aliasKeyName2 = "nv" + (string)lineItemEnum.Key;
				string aliasKeyName3 = "m" + (string)lineItemEnum.Key;
				string aliasKeyName4 = "p" + (string)lineItemEnum.Key;
				string aliasKeyName5 = "gp" + (string)lineItemEnum.Key;
				string aliasKeyName6 = "np" + (string)lineItemEnum.Key;
				string aliasKeyName7 = "mp" + (string)lineItemEnum.Key;
				string aliasKeyName8 = "fl" + (string)lineItemEnum.Key;
				string aliasKeyName9 = "n1" + (string)lineItemEnum.Key;
				string aliasKeyName10 = "n2" + (string)lineItemEnum.Key;
				string aliasKeyName11 = "n3" + (string)lineItemEnum.Key;
				string aliasKeyName12 = "n4" + (string)lineItemEnum.Key;
				string aliasKeyName13 = "n5" + (string)lineItemEnum.Key;
				string aliasKeyName14 = "n6" + (string)lineItemEnum.Key;
				string aliasKeyName15 = "mk" + (string)lineItemEnum.Key;

				int columnKey1 = (int)aliasColumnKey[aliasKeyName1];
				int columnKey2 = (int)aliasColumnKey[aliasKeyName2];
				int columnKey3 = (int)aliasColumnKey[aliasKeyName3];
				int columnKey4 = (int)aliasColumnKey[aliasKeyName4];
				int columnKey5 = (int)aliasColumnKey[aliasKeyName5];
				int columnKey6 = (int)aliasColumnKey[aliasKeyName6];
				int columnKey7 = (int)aliasColumnKey[aliasKeyName7];
				int columnKey8 = (int)aliasColumnKey[aliasKeyName8];
				int columnKey9 = (int)aliasColumnKey[aliasKeyName9];
				int columnKey10 = (int)aliasColumnKey[aliasKeyName10];
				int columnKey11 = (int)aliasColumnKey[aliasKeyName11];
				int columnKey12 = (int)aliasColumnKey[aliasKeyName12];
				int columnKey13 = (int)aliasColumnKey[aliasKeyName13];
				int columnKey14 = (int)aliasColumnKey[aliasKeyName14];
				int columnKey15 = (int)aliasColumnKey[aliasKeyName15];

				QuantityDO quantity = (QuantityDO)lineItemEnum.Value;
				string cellName = (string)lineItemEnum.Key;
				BaseInventoryLineItemDO.StatusFlags statusFlag = lineItem.GetCellFlags(cellName);
				int cellFlag = Convert.ToInt32(statusFlag.Flags);

				record.SetValue(columnKey1, quantity.GrossInventoryChange);
				record.SetValue(columnKey2, quantity.NetInventoryChange);
				record.SetValue(columnKey3, quantity.MassInventoryChange);
				record.SetValue(columnKey4, quantity.PackageInventoryChange);
				record.SetValue(columnKey5, quantity.GrossPriceInventoryChange);
				record.SetValue(columnKey6, quantity.NetPriceInventoryChange);
				record.SetValue(columnKey7, quantity.MassPriceInventoryChange);
				record.SetValue(columnKey8, cellFlag);
				record.SetValue(columnKey9, quantity.Number01Change);
				record.SetValue(columnKey10, quantity.Number02Change);
				record.SetValue(columnKey11, quantity.Number03Change);
				record.SetValue(columnKey12, quantity.Number04Change);
				record.SetValue(columnKey13, quantity.Number05Change);
				record.SetValue(columnKey14, quantity.Number06Change);
				record.SetValue(columnKey15, quantity.Moniker);
			}

			// Send record to client.
			SqlContext.Pipe.SendResultsRow(record);
		}

		// Close the pipe.
		SqlContext.Pipe.SendResultsEnd();
	}
	#endregion
}
