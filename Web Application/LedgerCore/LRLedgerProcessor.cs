// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LRLedgerProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The Ledger Record ledger processor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LedgerCore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Microsoft.SqlServer.Server;
    using System.Linq;

    /// <summary>
    /// The Ledger Record ledger processor.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class LRLedgerProcessor
	{
		#region Public data members
		/// <summary>
		/// The ledger requests.
		/// </summary>
		public enum LedgerRequests
		{
			/// <summary>
			/// This means that it is a owner type ledger.
			/// </summary>
			Refresh,

			/// <summary>
			/// This means that the ledger is produced for a manager and not owners.
			/// </summary>
			ManagerLedger
		}

		public enum SystemEditions
		{
			/// <summary>
			/// The standard system.
			/// </summary>
			Standard,

			/// <summary>
			/// A BSME system.
			/// </summary>
			Bsme,

			/// <summary>
			/// An Australian Defence Force system.
			/// </summary>
			Adf,

			/// <summary>
			/// A MOD system.
			/// </summary>
			Mod
		}

		/// <summary>
		/// Processing date select. By Inventory date, EBS post date , create date, EBS sent to date, or EBS acknowledged date
		/// </summary>
		public enum DateProcessTypes
		{
			/// <summary>
			/// Standard is to process by Inventory Date.
			/// </summary>
			ByInventoryDate = 0,

			/// <summary>
			/// For BSME, process the ledger on the EBS Post Date.
			/// </summary>
			ByEbsPostDate = 1,

			/// <summary>
			/// For BSME, process the ledger on the Create Date.
			/// </summary>
			ByCreateDate = 2,

			/// <summary>
			/// For BSME, process the ledger on the EBS Sent to Date.
			/// </summary>
			ByEbsSentToDate = 3,

			/// <summary>
			/// For BSME, process the ledger on the EBS Acknowledged Date.
			/// </summary>
			ByEbsAcknowledgedDate = 4
		}

		/// <summary>
		/// Determines whether the ledger connection should be a CLR connection or
		/// a connection like FMBusinessServices.
		/// </summary>
		public enum LedgerConnectionTypes
		{
			/// <summary>
			/// CLR Connection type.
			/// </summary>
			ClrConnection = 1, 

			/// <summary>
			/// Non-CLR connection type.
			/// </summary>
			NonClrConnection = 2
		}
		#endregion

		#region Private data members
		private LRTransactionAliasListDO transAliasListDO;
		private Hashtable aliasTypeList;
		private Guid siteGuid;
		private Guid userGuid;
		private Guid managerGuid;
		private Guid ownerGuid;
		private Guid productGuid;
/*
		private DateTime inventoryDate;
*/
		private DateTime beginDate;
		private DateTime endDate;
		private bool singleOwnerSystem;
		private double volumeFactor;
		private double volumePrecision;
		private double volumePackageSize;
		private double massFactor;
		private double massPrecision;
		private double massPackageSize;
		private bool loadByWeight;
		private Guid tankGuid;
		private SystemEditions systemEdition;
		private int nonSiteGroupCount;
		private string siteId;
		private LedgerConnection ledgerConnection;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LRLedgerProcessor"/> class. 
        /// This is the default constructor for the CLR Ledger Processor class.
        /// </summary>
        public LRLedgerProcessor(LedgerConnectionTypes ledgerConnectionType)
        {
            this.InitialSetup(ledgerConnectionType);
		}
        #endregion

        public LRLedgerProcessor()
        {
            this.InitialSetup();
        }
        private void InitialSetup(LedgerConnectionTypes connectionType = LedgerConnectionTypes.ClrConnection)
        {
            this.transAliasListDO = null;
            this.aliasTypeList = new Hashtable();
            this.siteGuid = Guid.Empty;
            this.managerGuid = Guid.Empty;
            this.ownerGuid = Guid.Empty;
            this.productGuid = Guid.Empty;
            this.beginDate = LedgerTime.Today().Date;
            this.endDate = LedgerTime.Today().Date;
            this.userGuid = Guid.Empty;
            this.ReportLedger = true;
            this.volumeFactor = 1.0;
            this.volumePrecision = 2.0;
            this.volumePackageSize = 0;
            this.massFactor = 1.0;
            this.massPrecision = 2.0;
            this.massPackageSize = 0.0;
            this.loadByWeight = false;
            this.LedgerRequest = LedgerRequests.Refresh;
            this.singleOwnerSystem = false;
            this.tankGuid = Guid.Empty;
            this.DateProcessType = DateProcessTypes.ByInventoryDate;
            this.IsBaseDb = true;
            this.LedgerConnectionType = connectionType;
            this.siteId = string.Empty;
            this.ledgerConnection       = new LedgerConnection(this.LedgerConnectionType);
        }
		#region Properties
		/// <summary>
		/// This property gets and sets the report ledger flag. True means
		/// that a report is calling the ledger.
		/// </summary>
		public bool ReportLedger { get; set; }

		/// <summary>
		/// This property gets and sets the ledger request to either
		/// a Manager Ledger or Refresh.
		/// </summary>
		public LedgerRequests LedgerRequest { get; set; }

		/// <summary>
		/// This property will set the Ledger request based on an integer value.
		/// The default is REFRESH request.
		/// </summary>
		public int LedgerRequestInt
		{
			get
			{
				switch (this.LedgerRequest)
				{
					case LedgerRequests.ManagerLedger:
						return 1;
					case LedgerRequests.Refresh:
						return 0;
					default:
						return 0;
				}
			}
			set
			{
				this.LedgerRequest = value != 1 ? LedgerRequests.Refresh : LedgerRequests.ManagerLedger;
			}
		}

		/// <summary>
		/// This property gets and sets the ledger begin date data member.
		/// </summary>
		public DateTime BeginDate
		{
			get { return this.beginDate; }
			set { this.beginDate = LedgerTime.ToDate(value).Date; }
		}

		/// <summary>
		/// This property gets and sets the ledger end date data member.
		/// </summary>
		public DateTime EndDate
		{
			get { return this.endDate; }
			set { this.endDate = LedgerTime.ToDate(value).Date; }
		}

		/// <summary>
		/// This property gets and sets the product Guid data member.
		/// </summary>
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the site Guid data member.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the user Guid data member.
		/// </summary>
		public Guid UserGuid
		{
			get { return this.userGuid; }
			set { this.userGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the manager Guid data member.
		/// </summary>
		public Guid ManagerGuid
		{
			get { return this.managerGuid; }
			set { this.managerGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the owner Guid data member.
		/// </summary>
		public Guid OwnerGuid
		{
			get { return this.ownerGuid; }
			set { this.ownerGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the tank Guid data member.
		/// </summary>
		public Guid TankGuid
		{
			get { return this.tankGuid; }
			set { this.tankGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the system edition data member.
		/// </summary>
		public SystemEditions SystemEdition
		{
			get
			{
				return this.systemEdition;
			}

			set
			{
				switch (value)
				{
					case SystemEditions.Adf:
						this.systemEdition = SystemEditions.Adf;
						break;
					case SystemEditions.Bsme:
						this.systemEdition = SystemEditions.Bsme;
						break;
					case SystemEditions.Mod:
						this.systemEdition = SystemEditions.Mod;
						break;
					default:
						this.systemEdition = SystemEditions.Standard;
						break;
				}
			}
		}

		/// <summary>
		/// Gets or sets the date process type.
		/// </summary>
		public DateProcessTypes DateProcessType { get; set; }

		/// <summary>
		/// Gets or sets the base/enterprise flag.
		/// </summary>
		public bool IsBaseDb { get; set; }

		/// <summary>
		/// Gets or sets the Ledger Connection type (CLR or Non-CLR)
		/// </summary>
		public LedgerConnectionTypes LedgerConnectionType { get; set; }
		#endregion

		#region Ledger Processing
		/// <summary>
		/// The get ledger processing result data set.
		/// </summary>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		/// <exception cref="Exception">Throw processing exception.
		/// </exception>
		public DataSet GetLedgerProcessingResultDataSet()
		{
			this.ledgerConnection = new LedgerConnection(this.LedgerConnectionType);

            LRLedgerLineItemCollection finalLedgerLineItemCollection = this.GetLedgerProcessingLineItems();

			// Send the ledger data to the client.
			if (this.ReportLedger)
			{
				if (this.LedgerConnectionType == LedgerConnectionTypes.ClrConnection)
				{
					this.SendDataToClient(finalLedgerLineItemCollection);
					return null;
				}
					
				return this.GetDataSetForClient(finalLedgerLineItemCollection);
			}

			if (this.LedgerConnectionType == LedgerConnectionTypes.ClrConnection)
			{
				this.SendDataToGuiClient(finalLedgerLineItemCollection);
				return null;
			}

			return this.GetDataSetForGuiClient(finalLedgerLineItemCollection);
		}

		/// <summary>
        /// This method determines if a product is associated with a site
        /// </summary>
        /// <returns>
        /// The System.bool
        /// </returns>
        private bool IsProductAssociatedWithSite(Guid inProductGuid, Guid inSiteGuid)
        {
			bool result = false;

			using (var cmd = new SqlCommand())
			{
				//entityToSiteMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.usp_GetProductToSiteByRecordGuid";

				cmd.Parameters.Add("@EntityRecordGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AssignedToSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@IncludeChildrenSites", SqlDbType.Bit);

				cmd.Parameters["@EntityRecordGuid"].Value = inProductGuid;
				cmd.Parameters["@AssignedToSiteGuid"].Value = inSiteGuid;
				cmd.Parameters["@IncludeChildrenSites"].Value = 0;

				DataSet dataSet = this.ledgerConnection.GetDataSet(cmd);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];
					Guid identityGuid = row.IsNull("EntityRecordGuid") ? Guid.Empty : (Guid)row["EntityRecordGuid"];

					if (identityGuid != Guid.Empty)
					{
						result = true;
					}
				}
			}

			return result;
        }

		/// <summary>
		/// This method starts the ledger processing for a given manager, owner, product, site, or sites.
		/// </summary>
		/// <returns>
		/// The FMBusinessServices.ServiceClasses.LedgerReportClasses.LRLedgerLineItemCollection.
		/// </returns>
		private LRLedgerLineItemCollection GetLedgerProcessingLineItems()
		{
			// Get the transaction aliase one time for the given site.
			this.GetTransactionAliases();

			// The the product information which includes the conversion factor and
			// precision. If the product is configured with the conversion factor, then
			// class factor and precision members will be set use the product settings.
			// Otherwise the site's settings will be used.
			LRProductDO productDo = this.RetrieveProductInfo();

			// Retrieve a site list for the current site and their information.
			LRSiteDO currentSite = this.RetrieveCurrentSite();
            this.siteId = currentSite.SiteName;
			LRSiteListDO siteListDo = this.RetrieveSites();

			// Get the list of sites that are non-inhibited sites and
			// has a product assigned.
			List<LRSiteDO> siteListToProcess = this.ExcludeInhibitedSites(siteListDo, productDo);

			// Set the volume/mass/package factors and precisions for this ledger.
			this.SetProductConvertionInfo(productDo, currentSite);

			// If Owner Ledger and not Reconciliation check to see if single owner.
			if (this.LedgerRequest != LedgerRequests.ManagerLedger)
			{
				this.singleOwnerSystem = currentSite.SingleOwner;
			}

			// Default the currency conversion factor and precision. At some point
			// the GUI needs to have a configuration setting for this.
			const double CurrencyConversion = 1;
			int currencyDecimalPlaces = 2;

			// Set the currency decimal places to -1 which means do not
			// round.
			if (this.systemEdition == SystemEditions.Adf)
			{
				currencyDecimalPlaces = -1;
			}

			// Make the worst case starting date only 6 months back of the
			// ledger begin date.
			DateTime worstCaseStartDate;

			// For non-single owner system or owner ledger retrieve the owner closeout records
			// for each site. This is to get the initial book inventory and the start date to
			// retrieve the data for calculation.
			if ((this.singleOwnerSystem == false) && (this.LedgerRequest != LedgerRequests.ManagerLedger))
			{
				worstCaseStartDate = this.RetrieveOwnerCloseoutRecord(ref siteListToProcess, this.beginDate);
			}
			else
			{
				// Retrieve the most recent physical inventory date for the given site.
				worstCaseStartDate = this.GetMostRecentPhsyicalInventoryDateAllSites(ref siteListToProcess);
			}

			// Retrieve the beginning book gross and net values. We are assuming a 
			// single owner system, therefore the physical inventory is the beginning
			// book. Since there can be several physicals in one day, we get the sum
			// of those inventories.
			if (this.singleOwnerSystem || this.LedgerRequest == LedgerRequests.ManagerLedger)
			{
				this.GetSummedPhysicalInventories(ref siteListToProcess);

				if (this.systemEdition == SystemEditions.Bsme)
				{
					foreach (LRSiteDO siteDo in siteListToProcess)
					{
						siteDo.StartDate = this.beginDate;
					}
				}
			}

			// We are retrieve either the closeout dates (manager only ledger) or the owner
			// closeout dates (owner ledger).  These dates will be used to set the ledger
			// status for closing out the rows in the ledger.
			if (this.LedgerRequest == LedgerRequests.ManagerLedger)
			{
				this.RetrieveMostRecentCloseoutDate(ref siteListToProcess);
				this.RetrieveMostRecentBrokenBlendDate(ref siteListToProcess);
			}
			else
			{
				this.RetrieveMostRecentOwnerCloseoutDate(ref siteListToProcess);
			}

			// Retrieve data for the ledger
			var ledgerVerticalData = new LRLedgerVerticalData(	worstCaseStartDate,
																this.endDate,
																this.productGuid,
																this.managerGuid,
																this.ownerGuid,
																currentSite.SiteGuid,
																this.userGuid,
																this.volumeFactor,
																Convert.ToInt32(this.volumePrecision),
																this.massFactor,
																Convert.ToInt32(this.massPrecision),
																CurrencyConversion,
																currencyDecimalPlaces,
																this.volumePackageSize,
																this.massPackageSize,
																this.loadByWeight,
																this.tankGuid,
																(int) this.systemEdition,
																siteListToProcess,
																this.DateProcessType,
																this.IsBaseDb,
																this.transAliasListDO);

			SortedList inventorySummation = ledgerVerticalData.RetrieveAndSendData(this.ledgerConnection);
			var ledgers = new List<LRLedgerLineItemCollection>();

			// Load the inventory data if present. If not, then we must create an empty
			// ledger for each site in order for the ledger to be filled out with
			// default values.
			if ((inventorySummation != null) && (inventorySummation.Count > 0))
			{
				// Load the inventory data information
				ledgers = this.LoadInventoryData(inventorySummation, siteListToProcess);
			}
			else
			{
				foreach (LRSiteDO siteDo in siteListToProcess)
				{
					var ledgerLineItemCollection = new LRLedgerLineItemCollection
					{
						ProductGuid = this.productGuid,
						SiteGuid = siteDo.SiteGuid
					};

					ledgers.Add(ledgerLineItemCollection);
				}
			}

			// Calculate the ledger (i.e. beginning and ending books, variances)
			bool usePreviousPhysicalInventory = this.singleOwnerSystem | (this.LedgerRequest == LedgerRequests.ManagerLedger);
			var ledgerManager = new LRLedgerManager(this.transAliasListDO, 
													this.aliasTypeList, 
													usePreviousPhysicalInventory, 
													this.systemEdition, 
													this.ledgerConnection);

			LRLedgerLineItemCollection finalLedgerLineItemCollection = ledgerManager.CreateLedger(	this.siteGuid,
																									ledgers,
																									this.beginDate,
																									this.endDate,
																									siteListToProcess);

			return finalLedgerLineItemCollection;
		}
		#endregion

		#region Retrieve the transaction aliases
		/// <summary>
		/// This method will retrieve the transaction aliases for the current site and
		/// the site that the user logged into.
		/// </summary>
		private void GetTransactionAliases()
		{
			this.transAliasListDO = new LRTransactionAliasListDO();
			this.transAliasListDO.PerformQuery(this.siteGuid, this.ledgerConnection);

			ICollection values = this.transAliasListDO.Values;

			foreach (LRTransactionAliasDO transAliasDo in values)
			{
				if (this.aliasTypeList.Contains(transAliasDo.AliasName) == false)
				{
					this.aliasTypeList.Add(transAliasDo.AliasName, transAliasDo.TransactionTypeID);
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
			this.transAliasListDO.PerformAggregateQuery(this.siteGuid, this.ledgerConnection);

			ICollection values = this.transAliasListDO.Values;

			foreach (LRTransactionAliasDO transAliasDo in values)
			{
				if ( transAliasDo.IsAggregateAlias && (this.aliasTypeList.Contains(transAliasDo.AliasName) == false) )
				{
					this.aliasTypeList.Add(transAliasDo.AliasName, transAliasDo.TransactionTypeID);
				}
			}
		}
		#endregion

		#region Get the most recent Physical Inventory date
		/// <summary>
		/// This method will return the starting date for the worse case amoung all the 
		/// sites.  This date will be used as a starting date to retrieve all the site
		/// data.
		/// </summary>
		/// <param name="siteList">
		/// The site List.
		/// </param>
		/// <returns>
		/// The <see cref="DateTime"/>.
		/// </returns>
		private DateTime GetMostRecentPhsyicalInventoryDateAllSites(ref List<LRSiteDO> siteList)
		{
			List<LRSiteDO> physicalInvSiteList;
			DateTime worstCaseSitePhysicalInvDate;
			var physicalInventory = new LRPhysicalInventoryProcessor(this.ledgerConnection);

			if (siteList.Count == 1)
			{
				// Get the physical inventory starting date to retrieve data for the one site.
				worstCaseSitePhysicalInvDate = physicalInventory.GetMostRecentPhysicalInventoryDateOneSite(
																									this.beginDate,
																									this.productGuid,
																									this.managerGuid,
																									this.tankGuid,
																									siteList,
																									out physicalInvSiteList);
			}
			else
			{
				// Get the physical inventory starting date to retrieve data for the worst case site.
				// the starting point list is ordered by the site name.
				worstCaseSitePhysicalInvDate = physicalInventory.GetMostRecentPhysicalInventoryDateAllSites(
																									this.beginDate,
																									this.productGuid,
																									this.managerGuid,
																									this.tankGuid,
																									siteList,
																									out physicalInvSiteList);
			}

			// Set the start date for the site if there is a physical inventory and
			// determine if at the end of the month.
			foreach (LRSiteDO physicalSiteDo in physicalInvSiteList)
			{
				LRSiteDO siteDo = siteList.Find(x => x.SiteGuid == physicalSiteDo.SiteGuid);

				if (siteDo == null)
				{
					continue;
				}

				siteDo.HasPhysicalInventory = physicalSiteDo.HasPhysicalInventory;
				siteDo.PhysicalInvDateForLedgerStart = physicalSiteDo.PhysicalInvDateForLedgerStart;
				siteDo.PhysicalOnLastDay = false;

				if (siteDo.HasPhysicalInventory)
				{
					siteDo.StartDate = physicalSiteDo.PhysicalInvDateForLedgerStart.AddDays(1);

					if (this.SystemEdition == SystemEditions.Bsme)
					{
						siteDo.PhysicalOnLastDay = this.IsPhysicalOnTheLastDayOfTheMonth(siteDo.PhysicalInvDateForLedgerStart);

						// Ensure that if there was a physical on the last day of the month, that
						// the physical was on the 1st previous month and not another month.
						if (siteDo.PhysicalOnLastDay)
						{
							DateTime lastDayOfPreviousMonth = this.beginDate;
							lastDayOfPreviousMonth = lastDayOfPreviousMonth.AddDays(-1);

							if (siteDo.PhysicalInvDateForLedgerStart.Equals(lastDayOfPreviousMonth) == false)
							{
								siteDo.PhysicalOnLastDay = false;
							}
						}
					}
				}
			}

			return worstCaseSitePhysicalInvDate;
		}
		#endregion

		#region Sum all the Physical Inventories for a given Day
		/// <summary>
		/// This method will retrieve summed daily physical inventory quantities based on the 
		/// criterion passed into the method. It will return the gross and net quantities.
		/// </summary>
		/// <param name="siteList"></param>
		private void GetSummedPhysicalInventories(ref List<LRSiteDO> siteList)
		{
			var physicalInventory = new LRPhysicalInventoryProcessor(this.ledgerConnection);
			physicalInventory.GetSummedPhysicalInventories(	ref siteList,
															this.productGuid,
															this.managerGuid,
															this.tankGuid,
															this.volumeFactor,
															this.volumePrecision,
															this.massFactor,
															this.massPrecision,
															this.systemEdition);
		}
		#endregion

		#region Get Owner Closeout records and Closeout records
		/// <summary>
		/// This method will populate the site list with the owner closeout date 
		/// based on the manager, owner, product, and site.
		/// </summary>
		/// <param name="siteList">
		/// The site List.
		/// </param>
		/// <param name="ledgerStartDate">
		/// The ledger Start Date.
		/// </param>
		/// <returns>
		/// The <see cref="DateTime"/>.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		private DateTime RetrieveOwnerCloseoutRecord(ref List<LRSiteDO> siteList, DateTime ledgerStartDate)
		{
			// Return the ledger start date if the site list is empty.
			if (siteList == null || siteList.Count == 0)
			{
				return ledgerStartDate;
			}

			var ownerCloseoutList = new List<LROwnerCloseoutDO>();
			
			if (siteList.Count == 1)
			{
				var ownerCloseoutDo = new LROwnerCloseoutDO
				                      {
					                      ManagerGuid = this.managerGuid,
					                      OwnerGuid = this.ownerGuid,
					                      ProductGuid = this.productGuid,
					                      SiteGuid = siteList[0].SiteGuid,
                                          VolumeFactor = this.volumeFactor
                                      };

				using (var command = new SqlCommand())
				{
					ownerCloseoutDo.GetCurrentOwnerCloseoutSingleSiteSelectSQL(command, ledgerStartDate);
					DataSet dataSet = this.ledgerConnection.GetDataSet(command);

					if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
					{
						DataRow row = dataSet.Tables[0].Rows[0];

						// Load current owner closeout record.
						ownerCloseoutDo.LoadCurrentOwnerCloseout(row);
						ownerCloseoutList.Add(ownerCloseoutDo);
					}
				}
			}
			else
			{
				var ownerCloseoutDo = new LROwnerCloseoutDO
				                      {
					                      ManagerGuid = this.managerGuid,
					                      OwnerGuid = this.ownerGuid,
					                      ProductGuid = this.productGuid,
                                          VolumeFactor = this.volumeFactor
				                      };

				using (var command = new SqlCommand())
				{
					ownerCloseoutDo.GetCurrentOwnerCloseoutSelectSQL(command, siteList, ledgerStartDate);
					DataSet dataSet = this.ledgerConnection.GetDataSet(command);

					if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
					{
						foreach (DataRow row in dataSet.Tables[0].Rows)
						{
							// Load current owner closeout record.
							ownerCloseoutDo = new LROwnerCloseoutDO()
							                  {
							                      VolumeFactor = this.volumeFactor
							                  };
							ownerCloseoutDo.LoadCurrentOwnerCloseout(row);
							ownerCloseoutList.Add(ownerCloseoutDo);
						}
					}
				}
			}

			foreach (LROwnerCloseoutDO ownerCloseoutDo in ownerCloseoutList)
			{
				LRSiteDO siteDo = siteList.Find(x => x.SiteGuid == ownerCloseoutDo.SiteGuid);

				siteDo.OwnerCloseoutDateForLedgerStart = ownerCloseoutDo.CloseoutDate;
				siteDo.FoundOnwerCloseoutRecord = ownerCloseoutDo.FoundOwnerCloseoutRecord;

				if (siteDo.FoundOnwerCloseoutRecord)
				{
					double packageQuantity = 0;

					// Move the start calculating date to the day after the closeout
					siteDo.StartDate = siteDo.OwnerCloseoutDateForLedgerStart.AddDays(1);
					siteDo.HasPhysicalInventory = true;
					siteDo.InitialBookInventory = ownerCloseoutDo.BookInventory;

					if (this.loadByWeight)
					{
						if (this.massPackageSize != 0)
						{
							packageQuantity = siteDo.InitialBookInventory.MassInventoryChange / this.massPackageSize;
						}
					}
					else
					{
						if (this.volumePackageSize != 0)
						{
							packageQuantity = siteDo.InitialBookInventory.NetInventoryChange / this.volumePackageSize;
						}
					}

					siteDo.InitialBookInventory.Package = packageQuantity;
				}
			}


			List<LROwnerCloseoutDO> orderedList = ownerCloseoutList.OrderByDescending(x => x.CloseoutDate).ToList();

			// Return the worst case ledger starting point based on the
			// closeout records. If no closeout, return the ledger start date.
			return orderedList.Count > 0 ? orderedList[0].CloseoutDate : ledgerStartDate;
		}

		/// <summary>
		/// This method will retrieve the most recent owner closeout date based on the manager, 
		/// owner, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <param name="siteList"></param>
		private void RetrieveMostRecentOwnerCloseoutDate(ref List<LRSiteDO> siteList)
		{
			var closeoutProcessor = new LRCloseoutProcessor(this.ledgerConnection);
			List<LROwnerCloseoutDO> ownerCloseoutList = siteList.Count == 1
				? closeoutProcessor.RetrieveMostRecentOwnerCloseoutDateSingleSite(
                                                                                            siteList,
                                                                                            this.managerGuid,
                                                                                            this.ownerGuid,
                                                                                            this.productGuid,
                                                                                            this.beginDate)
				: closeoutProcessor.RetrieveMostRecentOwnerCloseoutDate(
                                                                                    siteList,
                                                                                    this.nonSiteGroupCount,
                                                                                    this.managerGuid,
                                                                                    this.ownerGuid,
                                                                                    this.productGuid,
                                                                                    this.beginDate);
			foreach (LROwnerCloseoutDO ownerCloseoutDo in ownerCloseoutList)
			{
				LRSiteDO siteDo = siteList.Find(x => x.SiteGuid == ownerCloseoutDo.SiteGuid);
				siteDo.LedgerCloseoutStatusDate = ownerCloseoutDo.CloseoutDate;
				siteDo.LedgerCloseoutStatusDateStr = ownerCloseoutDo.CloseoutDateString;
			}
		}

		/// <summary>
		/// This method will return the Closeout DO with the most recent closeout date based on the
		/// manager, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <param name="siteList"></param>
		private void RetrieveMostRecentCloseoutDate(ref List<LRSiteDO> siteList)
		{
			List<LRCloseoutDO> closeoutList;
			var closeoutProcessor = new LRCloseoutProcessor(this.ledgerConnection);

			closeoutList = siteList.Count == 1
				? closeoutProcessor.RetrieveMostRecentCloseoutDateSingleSite(   siteList,
                                                                                            this.managerGuid,
                                                                                            this.productGuid,
                                                                                            this.beginDate)
				: closeoutProcessor.RetrieveMostRecentCloseoutDate(siteList,
                                                                                this.nonSiteGroupCount,
                                                                                this.managerGuid,
                                                                                this.productGuid,
                                                                                this.beginDate);

			foreach (LRCloseoutDO closeoutDo in closeoutList)
			{
				LRSiteDO siteDo = siteList.Find(x => x.SiteGuid == closeoutDo.SiteGuid);
				siteDo.LedgerCloseoutStatusDate = closeoutDo.CloseoutDate;
				siteDo.LedgerCloseoutStatusDateStr = closeoutDo.CloseoutDateString;
			}
		}

		/// <summary>
		/// This method will return the Closeout DO with the most recent closeout date based on the
		/// manager, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <param name="siteList"></param>
		private void RetrieveMostRecentBrokenBlendDate(ref List<LRSiteDO> siteList)
		{
			var closeoutProcessor = new LRCloseoutProcessor(this.ledgerConnection);
			List<LRCloseoutDO> brokenBlendList = siteList.Count == 1
				? closeoutProcessor.RetrieveMostRecentBrokenBlendDateSingleSite(siteList,
                                                                                                this.managerGuid,
                                                                                                this.productGuid,
                                                                                                this.endDate)
				: closeoutProcessor.RetrieveMostRecentBrokenBlendDate(    siteList,
                                                                                        this.nonSiteGroupCount,
                                                                                        this.managerGuid,
                                                                                        this.productGuid,
                                                                                        this.endDate);
			foreach (LRCloseoutDO closeoutDo in brokenBlendList)
			{
				LRSiteDO siteDo = siteList.Find(x => x.SiteGuid == closeoutDo.SiteGuid);
				siteDo.LedgerBrokenBlendStatusDate = closeoutDo.BrokenBlendDate;
				siteDo.LedgerBrokenBlendStatusDateStr = closeoutDo.BrokenBlendDateString;
			}
		}
		#endregion

		#region Find if the system is BSME
		/// <summary>
		/// This method will return true if the physical inventory occurred on the
		/// last day of the month. It will return false if it does not.
		/// </summary>
		/// <param name="physicalDate">
		/// The physical date.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		private bool IsPhysicalOnTheLastDayOfTheMonth(DateTimeOffset physicalDate)
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
					var leap = new DateTimeOffset(physicalDate.Year, 3, 1, 0, 0, 0, TimeSpan.Zero);
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
				case 4: // April
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
		/// <returns>
		/// The FMBusinessServices.ServiceClasses.LedgerReportClasses.LRSiteListDO.
		/// </returns>
		private LRSiteListDO RetrieveSites()
		{
			// Retrieve a list of sites based on the given site Guid.
			// If the site is not a parent site, then it only returns one
			// in the list. In addition, it will exclude sites that have
			// the "InhibitSiteLedgerRollup" flag set.
			var siteListDo = new LRSiteListDO(this.beginDate, this.ledgerConnection);
			siteListDo.RetrieveSiteList(this.siteId);

			return siteListDo;
		}


		/// <summary>
		/// This method will return the current site information.
		/// </summary>
		/// <returns>
		/// The FMBusinessServices.ServiceClasses.LedgerReportClasses.LRSiteDO.
		/// </returns>
		private LRSiteDO RetrieveCurrentSite()
		{
			// Retrieve the current site information
			var siteDo = new LRSiteDO(this.beginDate);
			siteDo.RetrieveSiteInfo(this.siteGuid, this.ledgerConnection);
			return siteDo;
		}

		/// <summary>
		/// The purpose of this method is to return a list of Site Data Objects that
		/// only contains non-inhibited sites and the product is assigned.
		/// </summary>
		/// <param name="origSiteListDo">
		/// The orig Site List data object.
		/// </param>
		/// <param name="productDo">
		/// The product data object.
		/// </param>
		/// <returns>
		/// Returns a collection of LRSiteDO.
		/// </returns>
		private List<LRSiteDO> ExcludeInhibitedSites(LRSiteListDO origSiteListDo, LRProductDO productDo)
		{
			this.nonSiteGroupCount = 0;

			var siteListToProcess = new List<LRSiteDO>();
			IDictionaryEnumerator origSiteListEnumerator = origSiteListDo.SiteList.GetEnumerator();

			while (origSiteListEnumerator.MoveNext())
			{
				var siteDo = (LRSiteDO) origSiteListEnumerator.Value;
				bool isProductAssigned = this.IsProductAssociatedWithSite(productDo.ProductGuid, siteDo.SiteGuid);

				if (siteDo.InhibitSiteLedgerRollup == false && isProductAssigned)
				{
					if (siteDo.SiteGroupFlag)
					{
						siteDo.InitialBookInventory = new LRQuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
						siteDo.HasPhysicalInventory = false;
						siteDo.StartDate = this.beginDate;
					}
					else
					{
						this.nonSiteGroupCount++;
					}

					siteListToProcess.Add(siteDo);
				}
			}

			return siteListToProcess;
		}
		#endregion

		#region Retrieve Product Info
		/// <summary>
		/// This method will return the current site information.
		/// </summary>
		/// <returns>
		/// The FMBusinessServices.ServiceClasses.LedgerReportClasses.LRProductDO.
		/// </returns>
		private LRProductDO RetrieveProductInfo()
		{
			// Use the product conversion factor and precision if the product is configured to
			// have them.
			var productDo = new LRProductDO { ProductGuid = this.productGuid };
			productDo.RetrieveProductInfo(this.ledgerConnection, this.siteGuid);

			return productDo;
		}

		/// <summary>
		/// This method sets the volume/mass/package factors and precisions values for
		/// this ledger.
		/// </summary>
		/// <param name="productDo">
		/// The product Data Object.
		/// </param>
		/// <param name="currentSite">
		/// The current Site.
		/// </param>
		private void SetProductConvertionInfo(LRProductDO productDo, LRSiteDO currentSite)
		{
			if (productDo.UseProductVolumeConversionFactor)
			{
				this.volumeFactor = productDo.VolumeConversionFactor;
				this.volumePrecision = Convert.ToDouble(productDo.VolumeDecimalPlaces);
			}
			else
			{
				if (productDo.ProductType == LRProductDO.PRODUCT_TYPE.AdditiveProduct)
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

			this.volumePackageSize = Math.Round(productDo.VolumePackageSize * this.volumeFactor, (int) this.volumePrecision, MidpointRounding.AwayFromZero);

			if (productDo.UseProductMassConversionFactor)
			{
				this.massFactor = productDo.MassConversionFactor;
				this.massPrecision = Convert.ToDouble(productDo.MassDecimalPlaces);
			}
			else
			{
				this.massFactor = currentSite.MassConversionFactor;
				this.massPrecision = Convert.ToDouble(currentSite.MassDecimalPlaces);
			}

			this.massPackageSize = Math.Round(productDo.MassPackageSize * this.massFactor, (int) this.massPrecision, MidpointRounding.AwayFromZero);
			this.loadByWeight = productDo.LoadByWeight;
		}
		#endregion

		#region LoadInventoryData
		/// <summary>
		/// This method will load the inventory data in a collection of Inventory Line 
		/// Item Data Objects.
		/// </summary>
		/// <param name="inventorySummation">
		/// The inventory Summation.
		/// </param>
		/// <param name="siteListToProcess">
		/// The site List To Process.
		/// </param>
		/// <returns>
		/// The LRLedgerLineItemCollection.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		private List<LRLedgerLineItemCollection> LoadInventoryData(SortedList inventorySummation, List<LRSiteDO> siteListToProcess)
		{
			IDictionaryEnumerator inventorySummationEnumerator = inventorySummation.GetEnumerator();

			// Set the data record row values and send it out on the pipe.
			LRInventoryLineItemDO ledgerLineItem = null;

			string currentDate = string.Empty;
			string currentSite = string.Empty;

			var ledgers = new List<LRLedgerLineItemCollection>();
			LRLedgerLineItemCollection ledgerLineItemCollection = null;
			LRSiteDO siteDo = null;

			// The inventory summation is sorted by site/inventory date/alias name. This loop is going
			// to create a ledger for each site.
			while (inventorySummationEnumerator.MoveNext())
			{
				var inventoryDailyAlias = (LRInventoryDailyAliasDO) inventorySummationEnumerator.Value;

				// For each new site create a new ledger line item collection.  This collection
				// contains a line item for each inventory date.
				if (inventoryDailyAlias.Site.Equals(currentSite) == false)
				{
					siteDo = siteListToProcess.Find(x => string.Compare(x.SiteName, inventoryDailyAlias.Site, StringComparison.OrdinalIgnoreCase) == 0);

					ledgerLineItemCollection = new LRLedgerLineItemCollection
					{
						ProductGuid = this.productGuid,
						SiteGuid = siteDo.SiteGuid
					};

					ledgers.Add(ledgerLineItemCollection);

					currentDate = string.Empty;
					currentSite = inventoryDailyAlias.Site;
				}

				// Since inventory summary contains the worst case starting date for the 
				// all sites, we need to ignore any data items that the inventory date
				// is less than the corresponding site's start date.
				if (siteDo != null && inventoryDailyAlias.InventoryDate < siteDo.StartDate)
				{
					continue;
				}

				// The date returned from the database is formatted as YYYY/MM/DD.  The code below
				// formats the date as MM/DD/YYYY
				string inventoryDateStr = inventoryDailyAlias.InventoryDateStr;
				char[] separatorList = { '/' };
				string[] stringList = inventoryDateStr.Split(separatorList);
				inventoryDateStr = stringList[1] + "/" + stringList[2] + "/" + stringList[0];

				// This is the first row and the inventory date is not empty (it's a string not a datetime) 
				if (inventoryDateStr.Equals(currentDate) == false)
				{
					ledgerLineItem = new LRInventoryLineItemDO();

				    ledgerLineItemCollection?.Add(ledgerLineItem);

				    ledgerLineItem.InventoryDate = inventoryDateStr;
					currentDate = inventoryDateStr;
				}

				string aliasName		= inventoryDailyAlias.AliasName;
				double grossQuantity	= inventoryDailyAlias.GrossQuantity;
				double grossPrice		= inventoryDailyAlias.GrossPrice;
				double netQuantity		= inventoryDailyAlias.NetQuantity;
				double netPrice			= inventoryDailyAlias.NetPrice;
				double massQuantity		= inventoryDailyAlias.MassQuantity;
				double massPrice		= inventoryDailyAlias.MassPrice;

				double packageQuantity = 0;

				if (this.loadByWeight)
				{
					if (this.massPackageSize != 0)
					{
						packageQuantity = massQuantity / this.massPackageSize;
					}
				}
				else
				{
					if (this.volumePackageSize != 0)
					{
						packageQuantity = netQuantity / this.volumePackageSize;
					}
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
				LRQuantityDO quantity = null;

				if (ledgerLineItem != null && ledgerLineItem.QuantityList.ContainsKey(aliasName) == false)
				{
					// Create the quantity (quantities and prices) for the alias name returned
					// from the db
					quantity = new LRQuantityDO(grossQuantity,
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
					if (ledgerLineItem != null)
					{
						quantity = ledgerLineItem.QuantityList[aliasName] as LRQuantityDO;
					}

					if (quantity != null)
					{
						quantity.GrossInventoryChange		+= grossQuantity;
						quantity.NetInventoryChange			+= netQuantity;
						quantity.MassInventoryChange		+= massQuantity;
						quantity.GrossPriceInventoryChange	+= grossPrice;
						quantity.NetPriceInventoryChange	+= netPrice;
						quantity.MassPriceInventoryChange	+= massPrice;

						quantity.Number01Change += number01;
						quantity.Number02Change += number02;
						quantity.Number03Change += number03;
						quantity.Number04Change += number04;
						quantity.Number05Change += number05;
						quantity.Number06Change += number06;

						quantity.OrErrorFlag(errorFlag);
					}
				}

				if (inventoryDailyAlias.ReversalFlag)
				{
				    ledgerLineItem?.SetCellFlag(aliasName, LRBaseInventoryLineItemDO.Status.TransWithReversals);
				}

			    if (ledgerLineItem != null)
				{
					ledgerLineItem.Site = inventoryDailyAlias.Site;

					if (inventoryDailyAlias.MaxTransVersion > ledgerLineItem.MaxTransVersion)
					{
						ledgerLineItem.MaxTransVersion = inventoryDailyAlias.MaxTransVersion;
					}
				}

				string transTypeIdStr = inventoryDailyAlias.TransTypeID.ToString(CultureInfo.InvariantCulture);
				var transType = (LRTransactionAliases.TransactionTypes) Convert.ToInt32(transTypeIdStr);

				// Check Transaction Alias to see if it is a type that affects inventory.
				switch (transType)
				{
					case LRTransactionAliases.TransactionTypes.T7FillStand:
					case LRTransactionAliases.TransactionTypes.T9Request:
					case LRTransactionAliases.TransactionTypes.T10Unload:
					case LRTransactionAliases.TransactionTypes.T11ConsumerTransfer:
					case LRTransactionAliases.TransactionTypes.T12Type12:
					case LRTransactionAliases.TransactionTypes.T17Order:
					case LRTransactionAliases.TransactionTypes.T18SupplyOrder:
					case LRTransactionAliases.TransactionTypes.T19EndOfDay:
					case LRTransactionAliases.TransactionTypes.T20EndOfMonth:
					case LRTransactionAliases.TransactionTypes.T21AccountPayableInvoice:
					case LRTransactionAliases.TransactionTypes.T22AccountReceivableInvoice:
					case LRTransactionAliases.TransactionTypes.T23StorageTransfer:
						{
							if (quantity != null)
							{
								quantity.AffectsInventory = false;
							}

							break;
						}

					case LRTransactionAliases.TransactionTypes.T14PhysicalInventory:
						{
							if (quantity != null)
							{
								quantity.AffectsInventory = false;
							}

							if (ledgerLineItem != null)
							{
								ledgerLineItem.HasPhysicalInventory = true;
							}

							break;
						}

					default:
						{
							if (quantity != null)
							{
								quantity.AffectsInventory = true;
							}

							break;
						}
				}
			}

			return ledgers;
		}
		#endregion

		#region Return data to report client
		/// <summary>
		/// This method will send the ledger data to the client via SQL Pipe.
		/// </summary>
		/// <param name="finalLedgerLineItemCollection"></param>
		private void SendDataToClient(LRLedgerLineItemCollection finalLedgerLineItemCollection)
		{
			var aliasColumnKey = new SortedList();

			// Determine the number of columns by the number of aliases and
			// 11 static columns.
			int columnCount = this.transAliasListDO.AliasList.Count;
			columnCount += 11;

			// Create the columns list based on the column count.
			var columns = new List<SqlMetaData>(columnCount);

			// Create a column for the 11 static columns
			var outputColumn01 = new SqlMetaData("InventoryDate", SqlDbType.NVarChar, 50);
			var outputColumn02 = new SqlMetaData("BeginInventory", SqlDbType.Float);
			var outputColumn03 = new SqlMetaData("BookInventory", SqlDbType.Float);
			var outputColumn04 = new SqlMetaData("Variance", SqlDbType.Float);
			var outputColumn05 = new SqlMetaData("TotalVariance", SqlDbType.Float);
			var outputColumn06 = new SqlMetaData("TotalPhysical", SqlDbType.Float);
			var outputColumn07 = new SqlMetaData("Tolerance", SqlDbType.Float);
			var outputColumn08 = new SqlMetaData("AllowableGainLoss", SqlDbType.Float);
			var outputColumn09 = new SqlMetaData("VariancePercentage", SqlDbType.Float);
			var outputColumn10 = new SqlMetaData("ToleranceTestedVolume", SqlDbType.Float);
			var outputColumn11 = new SqlMetaData("MaxTransVersion", SqlDbType.BigInt);

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
			IDictionaryEnumerator enumerator = this.transAliasListDO.AliasSortedList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var transAliasDo = (LRTransactionAliasDO) enumerator.Value;

				var outputColumn = new SqlMetaData(transAliasDo.AliasName, SqlDbType.Float);
				columns.Add(outputColumn);

				if (aliasColumnKey.Contains(transAliasDo.AliasName) == false)
				{
					aliasColumnKey.Add(transAliasDo.AliasName, columnKeyCount);
					columnKeyCount++;
				}
			}

			// Create the columns for the data record.
			var record = new SqlDataRecord(columns.ToArray());

			if (SqlContext.Pipe != null)
			{
				SqlContext.Pipe.SendResultsStart(record);

				// Create the data for each previously defined column.
				foreach (LRInventoryLineItemDO lineItem in finalLedgerLineItemCollection)
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

					while (lineItemEnum.MoveNext())
					{
						var aliasName = (string) lineItemEnum.Key;
						var quantity = (LRQuantityDO) lineItemEnum.Value;
					    // ReSharper disable once AssignNullToNotNullAttribute
						var columnKey = (int) aliasColumnKey[aliasName];

						record.SetValue(columnKey, quantity.GrossInventoryChange);
					}

					// Send record to client.
					SqlContext.Pipe.SendResultsRow(record);
				}

				// Close the pipe.
				SqlContext.Pipe.SendResultsEnd();
			}
		}

		/// <summary>
		/// The get data set for client.
		/// </summary>
		/// <param name="finalLedgerLineItemCollection">
		/// The final ledger line item collection.
		/// </param>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		private DataSet GetDataSetForClient(LRLedgerLineItemCollection finalLedgerLineItemCollection)
		{
			var aliasColumnKey = new SortedList();

			// Determine the number of columns by the number of aliases and
			// 11 static columns.
			int columnCount = this.transAliasListDO.AliasList.Count;
			columnCount += 11;

			// Create the columns list based on the column count.
			var columns = new List<DataColumn>(columnCount);

			// Create a column for the 11 static columns
			var outputColumn01 = new DataColumn("InventoryDate", typeof(string));
			var outputColumn02 = new DataColumn("BeginInventory", typeof(double));
			var outputColumn03 = new DataColumn("BookInventory", typeof(double));
			var outputColumn04 = new DataColumn("Variance", typeof(double));
			var outputColumn05 = new DataColumn("TotalVariance", typeof(double));
			var outputColumn06 = new DataColumn("TotalPhysical", typeof(double));
			var outputColumn07 = new DataColumn("Tolerance", typeof(double));
			var outputColumn08 = new DataColumn("AllowableGainLoss", typeof(double));
			var outputColumn09 = new DataColumn("VariancePercentage", typeof(double));
			var outputColumn10 = new DataColumn("ToleranceTestedVolume", typeof(double));
			var outputColumn11 = new DataColumn("MaxTransVersion", typeof(long));

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
			IDictionaryEnumerator enumerator = this.transAliasListDO.AliasSortedList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var transAliasDo = (LRTransactionAliasDO)enumerator.Value;

				var outputColumn = new DataColumn(transAliasDo.AliasName, typeof(double));
				columns.Add(outputColumn);

				if ( aliasColumnKey.Contains(transAliasDo.AliasName) == false )
				{
					aliasColumnKey.Add(transAliasDo.AliasName, columnKeyCount);
					columnKeyCount++;
				}
			}

			// Create the columns for the data record.
			var table = new DataTable("LedgerProcessorResultsTable");
			table.Columns.AddRange(columns.ToArray());

			// Create the data for each previously defined column.
			foreach (LRInventoryLineItemDO lineItem in finalLedgerLineItemCollection)
			{
				DataRow record = table.NewRow();
				record[0] = lineItem.InventoryDate;
				record[1] = lineItem.BeginInventory.GrossInventoryChange;
				record[2] = lineItem.BookInventory.GrossInventoryChange;
				record[3] = lineItem.Variance.GrossInventoryChange;
				record[4] = lineItem.TotalVariance.GrossInventoryChange;
				record[5] = lineItem.TotalPhysicalInventory.GrossInventoryChange;
				record[6] = lineItem.Tolerance;
				record[7] = lineItem.AllowableGainLoss.GrossInventoryChange;
				record[8] = lineItem.VariancePercentage.GrossInventoryChange;
				record[9] = lineItem.ToleranceTestedQuantity.GrossInventoryChange;
				record[10] = lineItem.MaxTransVersion;

				IDictionaryEnumerator lineItemEnum = lineItem.QuantityList.GetEnumerator();
				while (lineItemEnum.MoveNext())
				{
					var aliasName = (string)lineItemEnum.Key;
					var quantity  = (LRQuantityDO)lineItemEnum.Value;
				    // ReSharper disable once AssignNullToNotNullAttribute
					var columnKey = (int)aliasColumnKey[aliasName];

					record[columnKey] = quantity.GrossInventoryChange;
				}

				table.Rows.Add(record);
			}

			var ds = new DataSet("LedgerProcessorResultsDataSet");
			ds.Tables.Add(table);
			return ds;
		}
		#endregion

		#region Return data to GUI client
		/// <summary>
		/// This method will send the ledger data to the client via SQL Pipe.
		/// </summary>
		/// <param name="finalLedgerLineItemCollection">
		/// The final ledger line item collection.
		/// </param>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		private DataSet GetDataSetForGuiClient(LRLedgerLineItemCollection finalLedgerLineItemCollection)
		{
			var aliasColumnKey = new SortedList();

			// Determine the number of columns by the number of aliases and multiply by 15 
			// since there are 15 volumes (Gross, Net, Mass, Package, Gross Price, Net Price, Mass Price, Number01 - Number06), a flag field,
			// and a moniker field being sent back.  In addition there are 51 static columns.
			int columnCount = this.transAliasListDO.AliasList.Count * 15;
			columnCount += 64;

			// Create the columns list based on the column count.
			var columns = new List<DataColumn>(columnCount);

			// Create a column for the 64 static columns
			var outputColumn01 = new DataColumn("InventoryDate", typeof(string));
			var outputColumn02 = new DataColumn("gvBeginInventory", typeof(double));
			var outputColumn03 = new DataColumn("nvBeginInventory", typeof(double));
			var outputColumn04 = new DataColumn("mBeginInventory", typeof(double));
			var outputColumn05 = new DataColumn("pBeginInventory", typeof(double));
			var outputColumn06 = new DataColumn("gpBeginInventory", typeof(double));
			var outputColumn07 = new DataColumn("npBeginInventory", typeof(double));
			var outputColumn08 = new DataColumn("mpBeginInventory", typeof(double));
			var outputColumn09 = new DataColumn("gvBookInventory", typeof(double));
			var outputColumn10 = new DataColumn("nvBookInventory", typeof(double));
			var outputColumn11 = new DataColumn("mBookInventory", typeof(double));
			var outputColumn12 = new DataColumn("pBookInventory", typeof(double));
			var outputColumn13 = new DataColumn("gpBookInventory", typeof(double));
			var outputColumn14 = new DataColumn("npBookInventory", typeof(double));
			var outputColumn15 = new DataColumn("mpBookInventory", typeof(double));
			var outputColumn16 = new DataColumn("gvVariance", typeof(double));
			var outputColumn17 = new DataColumn("nvVariance", typeof(double));
			var outputColumn18 = new DataColumn("mVariance", typeof(double));
			var outputColumn19 = new DataColumn("pVariance", typeof(double));
			var outputColumn20 = new DataColumn("gpVariance", typeof(double));
			var outputColumn21 = new DataColumn("npVariance", typeof(double));
			var outputColumn22 = new DataColumn("mpVariance", typeof(double));
			var outputColumn23 = new DataColumn("gvTotalVariance", typeof(double));
			var outputColumn24 = new DataColumn("nvTotalVariance", typeof(double));
			var outputColumn25 = new DataColumn("mTotalVariance", typeof(double));
			var outputColumn26 = new DataColumn("pTotalVariance", typeof(double));
			var outputColumn27 = new DataColumn("gpTotalVariance", typeof(double));
			var outputColumn28 = new DataColumn("npTotalVariance", typeof(double));
			var outputColumn29 = new DataColumn("mpTotalVariance", typeof(double));
			var outputColumn30 = new DataColumn("gvTotalPhysical", typeof(double));
			var outputColumn31 = new DataColumn("nvTotalPhysical", typeof(double));
			var outputColumn32 = new DataColumn("mTotalPhysical", typeof(double));
			var outputColumn33 = new DataColumn("pTotalPhysical", typeof(double));
			var outputColumn34 = new DataColumn("gpTotalPhysical", typeof(double));
			var outputColumn35 = new DataColumn("npTotalPhysical", typeof(double));
			var outputColumn36 = new DataColumn("mpTotalPhysical", typeof(double));
			var outputColumn37 = new DataColumn("gvTotalActivity", typeof(double));
			var outputColumn38 = new DataColumn("nvTotalActivity", typeof(double));
			var outputColumn39 = new DataColumn("mTotalActivity", typeof(double));
			var outputColumn40 = new DataColumn("pTotalActivity", typeof(double));
			var outputColumn41 = new DataColumn("gpTotalActivity", typeof(double));
			var outputColumn42 = new DataColumn("npTotalActivity", typeof(double));
			var outputColumn43 = new DataColumn("mpTotalActivity", typeof(double));
			var outputColumn44 = new DataColumn("gvTotalMovement", typeof(double));
			var outputColumn45 = new DataColumn("nvTotalMovement", typeof(double));
			var outputColumn46 = new DataColumn("mTotalMovement", typeof(double));
			var outputColumn47 = new DataColumn("pTotalMovement", typeof(double));
			var outputColumn48 = new DataColumn("gpTotalMovement", typeof(double));
			var outputColumn49 = new DataColumn("npTotalMovement", typeof(double));
			var outputColumn50 = new DataColumn("mpTotalMovement", typeof(double));
			var outputColumn51 = new DataColumn("LineItemStatusFlags", typeof(int));
			var outputColumn52 = new DataColumn("tolerance", typeof(double));
			var outputColumn53 = new DataColumn("gvAllowableGainLoss", typeof(double));
			var outputColumn54 = new DataColumn("nvAllowableGainLoss", typeof(double));
			var outputColumn55 = new DataColumn("mAllowableGainLoss", typeof(double));
			var outputColumn56 = new DataColumn("pAllowableGainLoss", typeof(double));
			var outputColumn57 = new DataColumn("gvVariancePercentage", typeof(double));
			var outputColumn58 = new DataColumn("nvVariancePercentage", typeof(double));
			var outputColumn59 = new DataColumn("mVariancePercentage", typeof(double));
			var outputColumn60 = new DataColumn("pVariancePercentage", typeof(double));
			var outputColumn61 = new DataColumn("gvToleranceTestedVolume", typeof(double));
			var outputColumn62 = new DataColumn("nvToleranceTestedVolume", typeof(double));
			var outputColumn63 = new DataColumn("mToleranceTestedVolume", typeof(double));
			var outputColumn64 = new DataColumn("pToleranceTestedVolume", typeof(double));

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

			columns.Add(new DataColumn("flagsBeginInventory", typeof(int)));
			columns.Add(new DataColumn("flagsBookInventory", typeof(int)));
			columns.Add(new DataColumn("flagsTotalPhysicalInventory", typeof(int)));
			columns.Add(new DataColumn("flagsTotalVariance", typeof(int)));
			columns.Add(new DataColumn("flagsVariance", typeof(int)));
			columns.Add(new DataColumn("flagsTotalActivity", typeof(int)));

			int columnKeyCount = columns.Count;

			// Create a column for each alias
			IDictionaryEnumerator enumerator = this.transAliasListDO.AliasSortedList.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var transAliasDO = (LRTransactionAliasDO)enumerator.Value;

				string[] aliasNames = 
									 { 
										"gv" + transAliasDO.AliasName, // Gross volume + alias name
				                        "nv" + transAliasDO.AliasName, // Net volume + alias name
				                        "m" + transAliasDO.AliasName, // Mass + alias name
				                        "p" + transAliasDO.AliasName, // Package + alias name
				                        "gp" + transAliasDO.AliasName, // Gross price + alias name
				                        "np" + transAliasDO.AliasName, // Net price + alias name
				                        "mp" + transAliasDO.AliasName, // Mass price + alias name
				                        "fl" + transAliasDO.AliasName, // Cell flag + alias name
				                        "n1" + transAliasDO.AliasName, // Number01 volume + aliasName
				                        "n2" + transAliasDO.AliasName, // Number02 volume + aliasName
				                        "n3" + transAliasDO.AliasName, // Number03 volume + aliasName
				                        "n4" + transAliasDO.AliasName, // Number04 volume + aliasName
				                        "n5" + transAliasDO.AliasName, // Number05 volume + aliasName
				                        "n6" + transAliasDO.AliasName, // Number06 volume + aliasName
				                        "mk" + transAliasDO.AliasName // Moniker for the alias column
				                      };

				var outputCol1 = new DataColumn(aliasNames[0], typeof(double));
				var outputCol2 = new DataColumn(aliasNames[1], typeof(double));
				var outputCol3 = new DataColumn(aliasNames[2], typeof(double));
				var outputCol4 = new DataColumn(aliasNames[3], typeof(double));
				var outputCol5 = new DataColumn(aliasNames[4], typeof(double));
				var outputCol6 = new DataColumn(aliasNames[5], typeof(double));
				var outputCol7 = new DataColumn(aliasNames[6], typeof(double));
				var outputCol8 = new DataColumn(aliasNames[7], typeof(int));

				// Number fields
				var outputCol9 = new DataColumn(aliasNames[8], typeof(double));
				var outputCol10 = new DataColumn(aliasNames[9], typeof(double));
				var outputCol11 = new DataColumn(aliasNames[10], typeof(double));
				var outputCol12 = new DataColumn(aliasNames[11], typeof(double));
				var outputCol13 = new DataColumn(aliasNames[12], typeof(double));
				var outputCol14 = new DataColumn(aliasNames[13], typeof(double));

				// Moniker column
				var outputCol15 = new DataColumn(aliasNames[14], typeof(string));

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
			var table = new DataTable("LedgerProcessorResultsTable");
			table.Columns.AddRange(columns.ToArray());

			// Create the data for each previously defined column.
			foreach (LRInventoryLineItemDO lineItem in finalLedgerLineItemCollection)
			{
				DataRow record = table.NewRow();
				record[0] = lineItem.InventoryDate;

				record[1] = lineItem.BeginInventory.GrossInventoryChange;
				record[2] = lineItem.BeginInventory.NetInventoryChange;
				record[3] = lineItem.BeginInventory.MassInventoryChange;
				record[4] = lineItem.BeginInventory.PackageInventoryChange;
				record[5] = lineItem.BeginInventory.GrossPriceInventoryChange;
				record[6] = lineItem.BeginInventory.NetPriceInventoryChange;
				record[7] = lineItem.BeginInventory.MassPriceInventoryChange;

				record[8] = lineItem.BookInventory.GrossInventoryChange;
				record[9] = lineItem.BookInventory.NetInventoryChange;
				record[10] = lineItem.BookInventory.MassInventoryChange;
				record[11] = lineItem.BookInventory.PackageInventoryChange;
				record[12] = lineItem.BookInventory.GrossPriceInventoryChange;
				record[13] = lineItem.BookInventory.NetPriceInventoryChange;
				record[14] = lineItem.BookInventory.MassPriceInventoryChange;

				record[15] = lineItem.Variance.GrossInventoryChange;
				record[16] = lineItem.Variance.NetInventoryChange;
				record[17] = lineItem.Variance.MassInventoryChange;
				record[18] = lineItem.Variance.PackageInventoryChange;
				record[19] = lineItem.Variance.GrossPriceInventoryChange;
				record[20] = lineItem.Variance.NetPriceInventoryChange;
				record[21] = lineItem.Variance.MassPriceInventoryChange;

				record[22] = lineItem.TotalVariance.GrossInventoryChange;
				record[23] = lineItem.TotalVariance.NetInventoryChange;
				record[24] = lineItem.TotalVariance.MassInventoryChange;
				record[25] = lineItem.TotalVariance.PackageInventoryChange;
				record[26] = lineItem.TotalVariance.GrossPriceInventoryChange;
				record[27] = lineItem.TotalVariance.NetPriceInventoryChange;
				record[28] = lineItem.TotalVariance.MassPriceInventoryChange;

				record[29] = lineItem.TotalPhysicalInventory.GrossInventoryChange;
				record[30] = lineItem.TotalPhysicalInventory.NetInventoryChange;
				record[31] = lineItem.TotalPhysicalInventory.MassInventoryChange;
				record[32] = lineItem.TotalPhysicalInventory.PackageInventoryChange;
				record[33] = lineItem.TotalPhysicalInventory.GrossPriceInventoryChange;
				record[34] = lineItem.TotalPhysicalInventory.NetPriceInventoryChange;
				record[35] = lineItem.TotalPhysicalInventory.MassPriceInventoryChange;

				record[36] = lineItem.TotalActivity.GrossInventoryChange;
				record[37] = lineItem.TotalActivity.NetInventoryChange;
				record[38] = lineItem.TotalActivity.MassInventoryChange;
				record[39] = lineItem.TotalActivity.PackageInventoryChange;
				record[40] = lineItem.TotalActivity.GrossPriceInventoryChange;
				record[41] = lineItem.TotalActivity.NetPriceInventoryChange;
				record[42] = lineItem.TotalActivity.MassPriceInventoryChange;

				record[43] = lineItem.TotalMovement.GrossInventoryChange;
				record[44] = lineItem.TotalMovement.NetInventoryChange;
				record[45] = lineItem.TotalMovement.MassInventoryChange;
				record[46] = lineItem.TotalMovement.MassInventoryChange;
				record[47] = lineItem.TotalMovement.GrossPriceInventoryChange;
				record[48] = lineItem.TotalMovement.NetPriceInventoryChange;
				record[49] = lineItem.TotalMovement.MassPriceInventoryChange;

				LRBaseInventoryLineItemDO.Status flags = lineItem.Flags.Flags;
				int lineItemStatusFlags = Convert.ToInt32(flags);
				record[50] = lineItemStatusFlags;

				record[51] = lineItem.Tolerance;
				record[52] = lineItem.AllowableGainLoss.GrossInventoryChange;
				record[53] = lineItem.AllowableGainLoss.NetInventoryChange;
				record[54] = lineItem.AllowableGainLoss.MassInventoryChange;
				record[55] = lineItem.AllowableGainLoss.PackageInventoryChange;
				record[56] = lineItem.VariancePercentage.GrossInventoryChange;
				record[57] = lineItem.VariancePercentage.NetInventoryChange;
				record[58] = lineItem.VariancePercentage.MassInventoryChange;
				record[59] = lineItem.VariancePercentage.PackageInventoryChange;
				record[60] = lineItem.ToleranceTestedQuantity.GrossInventoryChange;
				record[61] = lineItem.ToleranceTestedQuantity.NetInventoryChange;
				record[62] = lineItem.ToleranceTestedQuantity.MassInventoryChange;
				record[63] = lineItem.ToleranceTestedQuantity.PackageInventoryChange;

				record[64] = Convert.ToInt32(lineItem.GetCellFlags("Begin Inventory").Flags);
				record[65] = Convert.ToInt32(lineItem.GetCellFlags("Book Inventory").Flags);
				record[66] = Convert.ToInt32(lineItem.GetCellFlags("Total Physical Inventory").Flags);
				record[67] = Convert.ToInt32(lineItem.GetCellFlags("Total Variance").Flags);
				record[68] = Convert.ToInt32(lineItem.GetCellFlags("Variance").Flags);
				record[69] = Convert.ToInt32(lineItem.GetCellFlags("Total Activity").Flags);

				IDictionaryEnumerator lineItemEnum = lineItem.QuantityList.GetEnumerator();
				while (lineItemEnum.MoveNext())
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

					var quantity = (LRQuantityDO)lineItemEnum.Value;
					var cellName = (string)lineItemEnum.Key;
					LRBaseInventoryLineItemDO.StatusFlags statusFlag = lineItem.GetCellFlags(cellName);
					int cellFlag = Convert.ToInt32(statusFlag.Flags);

					record[columnKey1] = quantity.GrossInventoryChange;
					record[columnKey2] = quantity.NetInventoryChange;
					record[columnKey3] = quantity.MassInventoryChange;
					record[columnKey4] = quantity.PackageInventoryChange;
					record[columnKey5] = quantity.GrossPriceInventoryChange;
					record[columnKey6] = quantity.NetPriceInventoryChange;
					record[columnKey7] = quantity.MassPriceInventoryChange;
					record[columnKey8] = cellFlag;
					record[columnKey9] = quantity.Number01Change;
					record[columnKey10] = quantity.Number02Change;
					record[columnKey11] = quantity.Number03Change;
					record[columnKey12] = quantity.Number04Change;
					record[columnKey13] = quantity.Number05Change;
					record[columnKey14] = quantity.Number06Change;
					record[columnKey15] = quantity.Moniker;
				}

				table.Rows.Add(record);
			}

			var ds = new DataSet("LedgerProcessorResultsDataSet");
			ds.Tables.Add(table);
			return ds;
		}

		private void SendDataToGuiClient(LRLedgerLineItemCollection finalLedgerLineItemCollection)
		{
			var aliasColumnKey = new SortedList();

			// Determine the number of columns by the number of aliases and multiply by 15 
			// since there are 15 volumes (Gross, Net, Mass, Package, Gross Price, Net Price, Mass Price, Number01 - Number06), a flag field,
			// and a moniker field being sent back.  In addition there are 64 static columns.
			int columnCount = this.transAliasListDO.AliasList.Count * 15;
			columnCount += 64;

			// Create the columns list based on the column count.
			var columns = new List<SqlMetaData>(columnCount);

			// Create a column for the 64 static columns
			var outputColumn01 = new SqlMetaData("InventoryDate", SqlDbType.NVarChar, 50);
			var outputColumn02 = new SqlMetaData("gvBeginInventory", SqlDbType.Float);
			var outputColumn03 = new SqlMetaData("nvBeginInventory", SqlDbType.Float);
			var outputColumn04 = new SqlMetaData("mBeginInventory", SqlDbType.Float);
			var outputColumn05 = new SqlMetaData("pBeginInventory", SqlDbType.Float);
			var outputColumn06 = new SqlMetaData("gpBeginInventory", SqlDbType.Float);
			var outputColumn07 = new SqlMetaData("npBeginInventory", SqlDbType.Float);
			var outputColumn08 = new SqlMetaData("mpBeginInventory", SqlDbType.Float);
			var outputColumn09 = new SqlMetaData("gvBookInventory", SqlDbType.Float);
			var outputColumn10 = new SqlMetaData("nvBookInventory", SqlDbType.Float);
			var outputColumn11 = new SqlMetaData("mBookInventory", SqlDbType.Float);
			var outputColumn12 = new SqlMetaData("pBookInventory", SqlDbType.Float);
			var outputColumn13 = new SqlMetaData("gpBookInventory", SqlDbType.Float);
			var outputColumn14 = new SqlMetaData("npBookInventory", SqlDbType.Float);
			var outputColumn15 = new SqlMetaData("mpBookInventory", SqlDbType.Float);
			var outputColumn16 = new SqlMetaData("gvVariance", SqlDbType.Float);
			var outputColumn17 = new SqlMetaData("nvVariance", SqlDbType.Float);
			var outputColumn18 = new SqlMetaData("mVariance", SqlDbType.Float);
			var outputColumn19 = new SqlMetaData("pVariance", SqlDbType.Float);
			var outputColumn20 = new SqlMetaData("gpVariance", SqlDbType.Float);
			var outputColumn21 = new SqlMetaData("npVariance", SqlDbType.Float);
			var outputColumn22 = new SqlMetaData("mpVariance", SqlDbType.Float);
			var outputColumn23 = new SqlMetaData("gvTotalVariance", SqlDbType.Float);
			var outputColumn24 = new SqlMetaData("nvTotalVariance", SqlDbType.Float);
			var outputColumn25 = new SqlMetaData("mTotalVariance", SqlDbType.Float);
			var outputColumn26 = new SqlMetaData("pTotalVariance", SqlDbType.Float);
			var outputColumn27 = new SqlMetaData("gpTotalVariance", SqlDbType.Float);
			var outputColumn28 = new SqlMetaData("npTotalVariance", SqlDbType.Float);
			var outputColumn29 = new SqlMetaData("mpTotalVariance", SqlDbType.Float);
			var outputColumn30 = new SqlMetaData("gvTotalPhysical", SqlDbType.Float);
			var outputColumn31 = new SqlMetaData("nvTotalPhysical", SqlDbType.Float);
			var outputColumn32 = new SqlMetaData("mTotalPhysical", SqlDbType.Float);
			var outputColumn33 = new SqlMetaData("pTotalPhysical", SqlDbType.Float);
			var outputColumn34 = new SqlMetaData("gpTotalPhysical", SqlDbType.Float);
			var outputColumn35 = new SqlMetaData("npTotalPhysical", SqlDbType.Float);
			var outputColumn36 = new SqlMetaData("mpTotalPhysical", SqlDbType.Float);
			var outputColumn37 = new SqlMetaData("gvTotalActivity", SqlDbType.Float);
			var outputColumn38 = new SqlMetaData("nvTotalActivity", SqlDbType.Float);
			var outputColumn39 = new SqlMetaData("mTotalActivity", SqlDbType.Float);
			var outputColumn40 = new SqlMetaData("pTotalActivity", SqlDbType.Float);
			var outputColumn41 = new SqlMetaData("gpTotalActivity", SqlDbType.Float);
			var outputColumn42 = new SqlMetaData("npTotalActivity", SqlDbType.Float);
			var outputColumn43 = new SqlMetaData("mpTotalActivity", SqlDbType.Float);
			var outputColumn44 = new SqlMetaData("gvTotalMovement", SqlDbType.Float);
			var outputColumn45 = new SqlMetaData("nvTotalMovement", SqlDbType.Float);
			var outputColumn46 = new SqlMetaData("mTotalMovement", SqlDbType.Float);
			var outputColumn47 = new SqlMetaData("pTotalMovement", SqlDbType.Float);
			var outputColumn48 = new SqlMetaData("gpTotalMovement", SqlDbType.Float);
			var outputColumn49 = new SqlMetaData("npTotalMovement", SqlDbType.Float);
			var outputColumn50 = new SqlMetaData("mpTotalMovement", SqlDbType.Float);
			var outputColumn51 = new SqlMetaData("LineItemStatusFlags", SqlDbType.Int);
			var outputColumn52 = new SqlMetaData("tolerance", SqlDbType.Float);
			var outputColumn53 = new SqlMetaData("gvAllowableGainLoss", SqlDbType.Float);
			var outputColumn54 = new SqlMetaData("nvAllowableGainLoss", SqlDbType.Float);
			var outputColumn55 = new SqlMetaData("mAllowableGainLoss", SqlDbType.Float);
			var outputColumn56 = new SqlMetaData("pAllowableGainLoss", SqlDbType.Float);
			var outputColumn57 = new SqlMetaData("gvVariancePercentage", SqlDbType.Float);
			var outputColumn58 = new SqlMetaData("nvVariancePercentage", SqlDbType.Float);
			var outputColumn59 = new SqlMetaData("mVariancePercentage", SqlDbType.Float);
			var outputColumn60 = new SqlMetaData("pVariancePercentage", SqlDbType.Float);
			var outputColumn61 = new SqlMetaData("gvToleranceTestedVolume", SqlDbType.Float);
			var outputColumn62 = new SqlMetaData("nvToleranceTestedVolume", SqlDbType.Float);
			var outputColumn63 = new SqlMetaData("mToleranceTestedVolume", SqlDbType.Float);
			var outputColumn64 = new SqlMetaData("pToleranceTestedVolume", SqlDbType.Float);

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
			IDictionaryEnumerator enumerator = this.transAliasListDO.AliasSortedList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var transAliasDo = (LRTransactionAliasDO) enumerator.Value;

				string[] aliasNames = 
								{
									"gv" + transAliasDo.AliasName,  // Gross volume + alias name
									"nv" + transAliasDo.AliasName,  // Net volume + alias name
									"m" + transAliasDo.AliasName,   // Mass + alias name
									"p" + transAliasDo.AliasName,   // Package + alias name
									"gp" + transAliasDo.AliasName,  // Gross price + alias name
									"np" + transAliasDo.AliasName,  // Net price + alias name
									"mp" + transAliasDo.AliasName,  // Mass price + alias name
									"fl" + transAliasDo.AliasName,  // Cell flag + alias name
									"n1" + transAliasDo.AliasName,  // Number01 volume + aliasName
									"n2" + transAliasDo.AliasName,  // Number02 volume + aliasName
									"n3" + transAliasDo.AliasName,  // Number03 volume + aliasName
									"n4" + transAliasDo.AliasName,  // Number04 volume + aliasName
									"n5" + transAliasDo.AliasName,  // Number05 volume + aliasName
									"n6" + transAliasDo.AliasName,  // Number06 volume + aliasName
									"mk" + transAliasDo.AliasName   // Moniker for the alias column
                               };

				var outputCol1 = new SqlMetaData(aliasNames[0], SqlDbType.Float);
				var outputCol2 = new SqlMetaData(aliasNames[1], SqlDbType.Float);
				var outputCol3 = new SqlMetaData(aliasNames[2], SqlDbType.Float);
				var outputCol4 = new SqlMetaData(aliasNames[3], SqlDbType.Float);
				var outputCol5 = new SqlMetaData(aliasNames[4], SqlDbType.Float);
				var outputCol6 = new SqlMetaData(aliasNames[5], SqlDbType.Float);
				var outputCol7 = new SqlMetaData(aliasNames[6], SqlDbType.Float);
				var outputCol8 = new SqlMetaData(aliasNames[7], SqlDbType.Int);

				// Number fields
				var outputCol9 = new SqlMetaData(aliasNames[8], SqlDbType.Float);
				var outputCol10 = new SqlMetaData(aliasNames[9], SqlDbType.Float);
				var outputCol11 = new SqlMetaData(aliasNames[10], SqlDbType.Float);
				var outputCol12 = new SqlMetaData(aliasNames[11], SqlDbType.Float);
				var outputCol13 = new SqlMetaData(aliasNames[12], SqlDbType.Float);
				var outputCol14 = new SqlMetaData(aliasNames[13], SqlDbType.Float);

				// Moniker column
				var outputCol15 = new SqlMetaData(aliasNames[14], SqlDbType.NVarChar, 50);

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
			var record = new SqlDataRecord(columns.ToArray());

			if (SqlContext.Pipe != null)
			{
			    // ReSharper disable once PossibleNullReferenceException
				SqlContext.Pipe.SendResultsStart(record);

				// Create the data for each previously defined column.
				foreach (LRInventoryLineItemDO lineItem in finalLedgerLineItemCollection)
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

					LRBaseInventoryLineItemDO.Status flags = lineItem.Flags.Flags;
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
					while (lineItemEnum.MoveNext())
					{
						string aliasKeyName1 = "gv" + (string) lineItemEnum.Key;
						string aliasKeyName2 = "nv" + (string) lineItemEnum.Key;
						string aliasKeyName3 = "m" + (string) lineItemEnum.Key;
						string aliasKeyName4 = "p" + (string) lineItemEnum.Key;
						string aliasKeyName5 = "gp" + (string) lineItemEnum.Key;
						string aliasKeyName6 = "np" + (string) lineItemEnum.Key;
						string aliasKeyName7 = "mp" + (string) lineItemEnum.Key;
						string aliasKeyName8 = "fl" + (string) lineItemEnum.Key;
						string aliasKeyName9 = "n1" + (string) lineItemEnum.Key;
						string aliasKeyName10 = "n2" + (string) lineItemEnum.Key;
						string aliasKeyName11 = "n3" + (string) lineItemEnum.Key;
						string aliasKeyName12 = "n4" + (string) lineItemEnum.Key;
						string aliasKeyName13 = "n5" + (string) lineItemEnum.Key;
						string aliasKeyName14 = "n6" + (string) lineItemEnum.Key;
						string aliasKeyName15 = "mk" + (string) lineItemEnum.Key;

						int columnKey1 = (int) aliasColumnKey[aliasKeyName1];
						int columnKey2 = (int) aliasColumnKey[aliasKeyName2];
						int columnKey3 = (int) aliasColumnKey[aliasKeyName3];
						int columnKey4 = (int) aliasColumnKey[aliasKeyName4];
						int columnKey5 = (int) aliasColumnKey[aliasKeyName5];
						int columnKey6 = (int) aliasColumnKey[aliasKeyName6];
						int columnKey7 = (int) aliasColumnKey[aliasKeyName7];
						int columnKey8 = (int) aliasColumnKey[aliasKeyName8];
						int columnKey9 = (int) aliasColumnKey[aliasKeyName9];
						int columnKey10 = (int) aliasColumnKey[aliasKeyName10];
						int columnKey11 = (int) aliasColumnKey[aliasKeyName11];
						int columnKey12 = (int) aliasColumnKey[aliasKeyName12];
						int columnKey13 = (int) aliasColumnKey[aliasKeyName13];
						int columnKey14 = (int) aliasColumnKey[aliasKeyName14];
						int columnKey15 = (int) aliasColumnKey[aliasKeyName15];

						var quantity = (LRQuantityDO) lineItemEnum.Value;
						var cellName = (string) lineItemEnum.Key;
						LRBaseInventoryLineItemDO.StatusFlags statusFlag = lineItem.GetCellFlags(cellName);
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
				    // ReSharper disable once PossibleNullReferenceException
					SqlContext.Pipe.SendResultsRow(record);
				}

				// Close the pipe.
			    // ReSharper disable once PossibleNullReferenceException
				SqlContext.Pipe.SendResultsEnd();
			}
		}
		#endregion
	}
}