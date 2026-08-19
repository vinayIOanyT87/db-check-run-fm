// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The ledger processor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.ServiceClasses.LedgerReportClasses;

	/// <summary>
	/// The ledger processor class.
	/// </summary>
	public class LedgerProcessorClass : ILedgerProcessor
	{
		#region Public Attributes
		public static int timeout = 600;
		public enum SystemEditions { STANDARD, BSME, ADF, MOD };
		#endregion

		#region  Private Attributes
		/// <summary>
		/// The ledger service request.
		/// </summary>
		private LedgerSR ledgerSr;

		/// <summary>
		/// The ledger data object.
		/// </summary>
		private LedgerDO ledgerDO;

		/// <summary>
		/// The transaction alias list data object.
		/// </summary>
		private TransactionAliasListDO transAliasListDO;

		/// <summary>
		/// The alias type list.
		/// </summary>
		private Hashtable aliasTypeList;

		/// <summary>
		/// The timer.
		/// </summary>
		private StopWatch timer;

		/// <summary>
		/// The consolidated data layer.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDa;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="LedgerProcessorClass"/> class.
		/// This is the default constructor for the ledger processor class.
		/// </summary>
		public LedgerProcessorClass()
		{
			this.ConsolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// This method starts the processing of gathering all the data for the Ledger
		/// page.
		/// </summary>
		/// <param name="inLedgerSr">
		/// The ledger service request.
		/// </param>
		/// <param name="accountingSite">Optional populated accounting site object.</param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.LedgerDO.
		/// </returns>
		/// <exception cref="AccountingServicesException">Owner is required.
		/// </exception>
		public LedgerDO Process(LedgerSR inLedgerSr, AccountingSite accountingSite = null)
		{
			this.timer = new StopWatch(StopWatch.Appnames.AccountingBLL, string.Empty);
			this.timer.Info("##### Ledger Processor Start ######");

			this.ledgerSr = inLedgerSr;
			this.ledgerDO = null;

			if (this.ledgerSr.GetRequestType() != LedgerSR.LedgerRequests.ManagerLedger && string.IsNullOrEmpty(this.ledgerSr.Owner))
			{
				throw new AccountingServicesException("Owner is required for calculating the ledger.");
			}

			this.ledgerDO = new LedgerDO();

			// Need to retrieve the current site information to determine whether or
			// not the current site is a site group.  If it is, then we need to retrieve
			// all the related sites and store in in the ledger service request.
			if (accountingSite == null)
			{
				var accountingSites = new AccountingSites();
				accountingSite = accountingSites.LoadSiteInfo(this.ledgerSr.Security, this.ledgerSr.CurrentSiteGuid);
			}

			this.ledgerSr.SiteList = accountingSite.SiteList;

			// Rerieve all the aliases for either the site or site group.  This must be
			// performed prior to actually retrieving the ledger tranactions because the
			// aliases are used in the query to only retrieve the necessary data.
			this.GetTransactionAliases(accountingSite);

			// Retrieve the entire ledger from the SQL CLR.
			this.RetrieveLedgerData(accountingSite);

			return this.ledgerDO;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will retrieve the entire ledger from the SQL CLR.
		/// </summary>
		/// <param name="accountingSite">
		/// The accounting site.
		/// </param>
		private void RetrieveLedgerData(AccountingSite accountingSite)
		{
			var products = new ProductsClass();
			var companies = new CompaniesClass();
			var tanks = new TanksClass();

			var managerGuid = Guid.Empty;
			var ownerGuid = Guid.Empty;
			var productGuid = Guid.Empty;
			var tankGuid = Guid.Empty;

			ProductClass product = null;

			if (string.IsNullOrEmpty(this.ledgerSr.Manager) == false)
			{
				if (this.ledgerSr.ManagerMasterGuid == Guid.Empty)
				{
					// Companies are captured in transactions using their MasterRecordGuids, not their CompanyGuids.
					managerGuid = companies.GetMasterRecordGuid(this.ledgerSr.Security, this.ledgerSr.Manager);
				}
				else
				{
					managerGuid = this.ledgerSr.ManagerMasterGuid;
				}
			}

			if (string.IsNullOrEmpty(this.ledgerSr.Owner) == false)
			{
				if (this.ledgerSr.OwnerMasterGuid == Guid.Empty)
				{
					// Companies are captured in transactions using their MasterRecordGuids, not their CompanyGuids.
					ownerGuid = companies.GetMasterRecordGuid(this.ledgerSr.Security, this.ledgerSr.Owner);
				}
				else
				{
					ownerGuid = this.ledgerSr.OwnerMasterGuid;
				}
			}

			if (string.IsNullOrEmpty(this.ledgerSr.Product) == false)
			{
				// Products are captured in transactions using their MasterRecordGuids, not their ProductGuids.
				product = products.GetByID(this.ledgerSr.Security, this.ledgerSr.Product);
				productGuid = product.MasterRecordGuid;
			}

			if (string.IsNullOrEmpty(this.ledgerSr.TankId) == false)
			{
				tankGuid = tanks.GetIdentityGuid(this.ledgerSr.Security, this.ledgerSr.TankId);
			}

			var systemEdition = (int)SystemEditions.STANDARD;
			var hardwareKey = new HardwareKeyClass();
			bool isBaseDb = true;

			try
			{
				if (hardwareKey.IsADFKey())
				{
					systemEdition = (int)SystemEditions.ADF;
				}
				else if (hardwareKey.IsMODKey())
				{
					systemEdition = (int)SystemEditions.MOD;
				}
				else if (hardwareKey.IsDescEnterpriseKey() ||
						 hardwareKey.IsDescKey() ||
						 hardwareKey.IsDescProfessionalKey())
				{
					systemEdition = (int)SystemEditions.BSME;
				}

				isBaseDb = !hardwareKey.IsDescEnterpriseKey();
			}
			catch (NullReferenceException)
			{
				systemEdition = (int)SystemEditions.STANDARD;
			}

			int ledgerRequest = Convert.ToInt32(this.ledgerSr.GetRequestType());
			const int ReportLedger = 0;

			var ledgerCalc = new LedgerReportCalculator();

			this.timer.ActionName = "LedgerProcessor.RetrieveLedgerData()";

			DateTime startTime = DateTimeOffset.Parse(this.ledgerSr.GetLedgerStartDate(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).Date;
			DateTime endTime = DateTimeOffset.Parse(this.ledgerSr.GetLedgerEndDate(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).Date;
			DataSet dataSet = ledgerCalc.Calculate(
													this.ledgerSr.Security.Token.ToString(),
													startTime,
													endTime,
													productGuid,
													managerGuid,
													ownerGuid,
													accountingSite.CurrentSiteGuid,
													this.ledgerSr.Security.UserGuid,
													ledgerRequest,
													ReportLedger,
													tankGuid,
													systemEdition,
													this.ledgerSr.DateType,
													isBaseDb);


			LedgerLineItemCollection ledger = this.LoadLedgerData(dataSet, ownerGuid);
			this.ledgerDO.LedgerLineItems = ledger;

			// Set any warning flags for out of tolerance
			if (!hardwareKey.IsTFMDKey())
			{
				this.SetOutOfToleranceFlag(accountingSite);
			}

			// Set any warning flags for out of tolerance
			this.SetFlags(product, accountingSite);
		}

		/// <summary>
		/// This method will load a completed Ledger that was retrieve from the 
		/// SQL CLR Ledger SP.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner GUID.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.LedgerLineItemCollection.
		/// </returns>
		private LedgerLineItemCollection LoadLedgerData(DataSet dataSet, Guid ownerGuid)
		{
			// User must belong to a user group that contains the owner (company) in the
			// being searched.
			bool isUserAuthorized = this.IsUserAuthorizatedToSeeBookColumns(this.ledgerSr.Security, ownerGuid);

			var ledger = new LedgerLineItemCollection();

			this.timer.ActionName = "LedgerProcessor.LoadLedgerData()";
			this.timer.Start();

			if (dataSet != null)
			{
				if (dataSet.Tables.Count > 0)
				{
					DataTable table = dataSet.Tables[0];

					foreach (DataRow row in table.Rows)
					{
						var ledgerLineItemDo = new LedgerLineItemDO();
						IDictionaryEnumerator aliasListEnum = this.transAliasListDO.aliasList.GetEnumerator();

						string inventoryDateStr = row.IsNull("InventoryDate") ? string.Empty : (string)row["InventoryDate"];
						ledgerLineItemDo.InventoryDate = inventoryDateStr;

						double gvBeginInventory = row.IsNull("gvBeginInventory") ? 0.0 : (double)row["gvBeginInventory"];
						double nvBeginInventory = row.IsNull("nvBeginInventory") ? 0.0 : (double)row["nvBeginInventory"];
						double mBeginInventory = row.IsNull("mBeginInventory") ? 0.0 : (double)row["mBeginInventory"];
						double pBeginInventory = row.IsNull("pBeginInventory") ? 0.0 : (double)row["pBeginInventory"];
						double gpBeginInventory = row.IsNull("gpBeginInventory") ? 0.0 : (double)row["gpBeginInventory"];
						double npBeginInventory = row.IsNull("npBeginInventory") ? 0.0 : (double)row["npBeginInventory"];
						double mpBeginInventory = row.IsNull("mpBeginInventory") ? 0.0 : (double)row["mpBeginInventory"];
						var quantityDO = new QuantityDO(gvBeginInventory, nvBeginInventory, mBeginInventory, pBeginInventory, gpBeginInventory, npBeginInventory, mpBeginInventory);
						ledgerLineItemDo.BeginInventory = quantityDO;
						int cellFlags = row.IsNull("flagsBeginInventory") ? 0 : (int)row["flagsBeginInventory"];

						// Set beginning book to N/A if user's user group does not an authorized owner
						// company associated to it.
						if (isUserAuthorized)
						{
							ledgerLineItemDo.SetCellFlag("BeginInventory", (BaseLineItemDO.Status)cellFlags);
							ledgerLineItemDo.BeginInventory = quantityDO;
						}
						else
						{
							ledgerLineItemDo.SetCellFlag("BeginInventory", BaseLineItemDO.Status.NA);
							quantityDO = new QuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
							ledgerLineItemDo.BeginInventory = quantityDO;
						}

						double gvBookInventory = row.IsNull("gvBookInventory") ? 0.0 : (double)row["gvBookInventory"];
						double nvBookInventory = row.IsNull("nvBookInventory") ? 0.0 : (double)row["nvBookInventory"];
						double mBookInventory = row.IsNull("mBookInventory") ? 0.0 : (double)row["mBookInventory"];
						double pBookInventory = row.IsNull("pBookInventory") ? 0.0 : (double)row["pBookInventory"];
						double gpBookInventory = row.IsNull("gpBookInventory") ? 0.0 : (double)row["gpBookInventory"];
						double npBookInventory = row.IsNull("npBookInventory") ? 0.0 : (double)row["npBookInventory"];
						double mpBookInventory = row.IsNull("mpBookInventory") ? 0.0 : (double)row["mpBookInventory"];
						quantityDO = new QuantityDO(gvBookInventory, nvBookInventory, mBookInventory, pBookInventory, gpBookInventory, npBookInventory, mpBookInventory);
						ledgerLineItemDo.BookInventory = quantityDO;
						cellFlags = row.IsNull("flagsBookInventory") ? 0 : (int)row["flagsBookInventory"];

						// Set book to N/A if user's user group does not an authorized owner
						// company associated to it.
						if (isUserAuthorized)
						{
							ledgerLineItemDo.SetCellFlag("BookInventory", (BaseLineItemDO.Status)cellFlags);
							ledgerLineItemDo.BookInventory = quantityDO;
						}
						else
						{
							ledgerLineItemDo.SetCellFlag("BookInventory", BaseLineItemDO.Status.NA);
							quantityDO = new QuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
							ledgerLineItemDo.BookInventory = quantityDO;
						}

						double gvVariance = row.IsNull("gvVariance") ? 0.0 : (double)row["gvVariance"];
						double nvVariance = row.IsNull("nvVariance") ? 0.0 : (double)row["nvVariance"];
						double mVariance = row.IsNull("mVariance") ? 0.0 : (double)row["mVariance"];
						double pVariance = row.IsNull("pVariance") ? 0.0 : (double)row["pVariance"];
						double gpVariance = row.IsNull("gpVariance") ? 0.0 : (double)row["gpVariance"];
						double npVariance = row.IsNull("npVariance") ? 0.0 : (double)row["npVariance"];
						double mpVariance = row.IsNull("mpVariance") ? 0.0 : (double)row["mpVariance"];
						quantityDO = new QuantityDO(gvVariance, nvVariance, mVariance, pVariance, gpVariance, npVariance, mpVariance);
						ledgerLineItemDo.Variance = quantityDO;
						cellFlags = row.IsNull("flagsVariance") ? 0 : (int)row["flagsVariance"];
						ledgerLineItemDo.SetCellFlag("Variance", (BaseLineItemDO.Status)cellFlags);

						double gvTotalVariance = row.IsNull("gvTotalVariance") ? 0.0 : (double)row["gvTotalVariance"];
						double nvTotalVariance = row.IsNull("nvTotalVariance") ? 0.0 : (double)row["nvTotalVariance"];
						double mTotalVariance = row.IsNull("mTotalVariance") ? 0.0 : (double)row["mTotalVariance"];
						double pTotalVariance = row.IsNull("pTotalVariance") ? 0.0 : (double)row["pTotalVariance"];
						double gpTotalVariance = row.IsNull("gpTotalVariance") ? 0.0 : (double)row["gpTotalVariance"];
						double npTotalVariance = row.IsNull("npTotalVariance") ? 0.0 : (double)row["npTotalVariance"];
						double mpTotalVariance = row.IsNull("mpTotalVariance") ? 0.0 : (double)row["mpTotalVariance"];
						quantityDO = new QuantityDO(gvTotalVariance, nvTotalVariance, mTotalVariance, pTotalVariance, gpTotalVariance, npTotalVariance, mpTotalVariance);
						ledgerLineItemDo.TotalVariance = quantityDO;
						cellFlags = row.IsNull("flagsTotalVariance") ? 0 : (int)row["flagsTotalVariance"];
						ledgerLineItemDo.SetCellFlag("Total Variance", (BaseLineItemDO.Status)cellFlags);

						double gvTotalPhysical = row.IsNull("gvTotalPhysical") ? 0.0 : (double)row["gvTotalPhysical"];
						double nvTotalPhysical = row.IsNull("nvTotalPhysical") ? 0.0 : (double)row["nvTotalPhysical"];
						double mTotalPhysical = row.IsNull("mTotalPhysical") ? 0.0 : (double)row["mTotalPhysical"];
						double pTotalPhysical = row.IsNull("pTotalPhysical") ? 0.0 : (double)row["pTotalPhysical"];
						double gpTotalPhysical = row.IsNull("gpTotalPhysical") ? 0.0 : (double)row["gpTotalPhysical"];
						double npTotalPhysical = row.IsNull("npTotalPhysical") ? 0.0 : (double)row["npTotalPhysical"];
						double mpTotalPhysical = row.IsNull("mpTotalPhysical") ? 0.0 : (double)row["mpTotalPhysical"];
						quantityDO = new QuantityDO(gvTotalPhysical, nvTotalPhysical, mTotalPhysical, pTotalPhysical, gpTotalPhysical, npTotalPhysical, mpTotalPhysical);
						ledgerLineItemDo.TotalPhysicalInventory = quantityDO;
						cellFlags = row.IsNull("flagsTotalPhysicalInventory") ? 0 : (int)row["flagsTotalPhysicalInventory"];
						ledgerLineItemDo.SetCellFlag("Total Physical Inventory", (BaseLineItemDO.Status)cellFlags);

						double gvTotalActivity = row.IsNull("gvTotalActivity") ? 0.0 : (double)row["gvTotalActivity"];
						double nvTotalActivity = row.IsNull("nvTotalActivity") ? 0.0 : (double)row["nvTotalActivity"];
						double mTotalActivity = row.IsNull("mTotalActivity") ? 0.0 : (double)row["mTotalActivity"];
						double pTotalActivity = row.IsNull("pTotalActivity") ? 0.0 : (double)row["pTotalActivity"];
						double gpTotalActivity = row.IsNull("gpTotalActivity") ? 0.0 : (double)row["gpTotalActivity"];
						double npTotalActivity = row.IsNull("npTotalActivity") ? 0.0 : (double)row["npTotalActivity"];
						double mpTotalActivity = row.IsNull("mpTotalActivity") ? 0.0 : (double)row["mpTotalActivity"];
						quantityDO = new QuantityDO(gvTotalActivity, nvTotalActivity, mTotalActivity, pTotalActivity, gpTotalActivity, npTotalActivity, mpTotalActivity);
						ledgerLineItemDo.TotalActivity = quantityDO;
						cellFlags = row.IsNull("flagsTotalActivity") ? 0 : (int)row["flagsTotalActivity"];
						ledgerLineItemDo.SetCellFlag("Total Activity", (BaseLineItemDO.Status)cellFlags);

						double gvTotalMovement = row.IsNull("gvTotalMovement") ? 0.0 : (double)row["gvTotalMovement"];
						double nvTotalMovement = row.IsNull("nvTotalMovement") ? 0.0 : (double)row["nvTotalMovement"];
						double mTotalMovement = row.IsNull("mTotalMovement") ? 0.0 : (double)row["mTotalMovement"];
						double pTotalMovement = row.IsNull("pTotalMovement") ? 0.0 : (double)row["pTotalMovement"];
						double gpTotalMovement = row.IsNull("gpTotalMovement") ? 0.0 : (double)row["gpTotalMovement"];
						double npTotalMovement = row.IsNull("npTotalMovement") ? 0.0 : (double)row["npTotalMovement"];
						double mpTotalMovement = row.IsNull("mpTotalMovement") ? 0.0 : (double)row["mpTotalMovement"];
						quantityDO = new QuantityDO(gvTotalMovement, nvTotalMovement, mTotalMovement, pTotalMovement, gpTotalMovement, npTotalMovement, mpTotalMovement);
						ledgerLineItemDo.TotalMovement = quantityDO;

						double tolerance = row.IsNull("tolerance") ? 0.0 : (double)row["tolerance"];
						ledgerLineItemDo.Tolerance = tolerance;

						double gvAllowableGainLoss = row.IsNull("gvAllowableGainLoss") ? 0.0 : (double)row["gvAllowableGainLoss"];
						double nvAllowableGainLoss = row.IsNull("nvAllowableGainLoss") ? 0.0 : (double)row["nvAllowableGainLoss"];
						double mAllowableGainLoss = row.IsNull("mAllowableGainLoss") ? 0.0 : (double)row["mAllowableGainLoss"];
						double pAllowableGainLoss = row.IsNull("pAllowableGainLoss") ? 0.0 : (double)row["pAllowableGainLoss"];
						var allowableGainLoss = new QuantityDO(gvAllowableGainLoss, nvAllowableGainLoss, mAllowableGainLoss, pAllowableGainLoss);
						ledgerLineItemDo.AllowableGainLoss = allowableGainLoss;

						double gvVariancePercentage = row.IsNull("gvVariancePercentage") ? 0.0 : (double)row["gvVariancePercentage"];
						double nvVariancePercentage = row.IsNull("nvVariancePercentage") ? 0.0 : (double)row["nvVariancePercentage"];
						double mVariancePercentage = row.IsNull("mVariancePercentage") ? 0.0 : (double)row["mVariancePercentage"];
						double pVariancePercentage = row.IsNull("pVariancePercentage") ? 0.0 : (double)row["pVariancePercentage"];
						var variancePercentage = new QuantityDO(gvVariancePercentage, nvVariancePercentage, mVariancePercentage, pVariancePercentage);
						ledgerLineItemDo.VariancePercentage = variancePercentage;

						while (aliasListEnum.MoveNext())
						{
							string aliasName = aliasListEnum.Key as string;
							string gvColumnName = "gv" + aliasName;
							string nvColumnName = "nv" + aliasName;
							string mColumnName = "m" + aliasName;
							string pColumnName = "p" + aliasName;
							string gpColumnName = "gp" + aliasName;
							string npColumnName = "np" + aliasName;
							string mpColumnName = "mp" + aliasName;
							string flColumnName = "fl" + aliasName;
							string n1ColumnName = "n1" + aliasName;
							string n2ColumnName = "n2" + aliasName;
							string n3ColumnName = "n3" + aliasName;
							string n4ColumnName = "n4" + aliasName;
							string n5ColumnName = "n5" + aliasName;
							string n6ColumnName = "n6" + aliasName;
							string mkColumnName = "mk" + aliasName;

							double grossQuantity = row.IsNull(gvColumnName) ? 0.0 : (double)row[gvColumnName];
							double netQuantity = row.IsNull(nvColumnName) ? 0.0 : (double)row[nvColumnName];
							double massQuantity = row.IsNull(mColumnName) ? 0.0 : (double)row[mColumnName];
							double packageQuantity = row.IsNull(pColumnName) ? 0.0 : (double)row[pColumnName];
							double grossPrice = row.IsNull(gpColumnName) ? 0.0 : (double)row[gpColumnName];
							double netPrice = row.IsNull(npColumnName) ? 0.0 : (double)row[npColumnName];
							double massPrice = row.IsNull(mpColumnName) ? 0.0 : (double)row[mpColumnName];
							int cellFlag = row.IsNull(flColumnName) ? 0 : (int)row[flColumnName];
							double number01 = row.IsNull(n1ColumnName) ? 0.0 : (double)row[n1ColumnName];
							double number02 = row.IsNull(n2ColumnName) ? 0.0 : (double)row[n2ColumnName];
							double number03 = row.IsNull(n3ColumnName) ? 0.0 : (double)row[n3ColumnName];
							double number04 = row.IsNull(n4ColumnName) ? 0.0 : (double)row[n4ColumnName];
							double number05 = row.IsNull(n5ColumnName) ? 0.0 : (double)row[n5ColumnName];
							double number06 = row.IsNull(n6ColumnName) ? 0.0 : (double)row[n6ColumnName];
							string moniker = row.IsNull(mkColumnName) ? string.Empty : (string)row[mkColumnName];

							quantityDO = new QuantityDO(
								grossQuantity,
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
								number06)
							{ Moniker = moniker };

							if (ledgerLineItemDo.QuantityList.ContainsKey(aliasName) == false)
							{
								ledgerLineItemDo.QuantityList.Add(aliasName, quantityDO);
							}

							var flag = (BaseLineItemDO.Status)cellFlag;
							ledgerLineItemDo.SetCellFlag(aliasName, flag);
						}

						int lineItemFlags = row.IsNull("LineItemStatusFlags") ? 0 : (int)row["LineItemStatusFlags"];
						ledgerLineItemDo.Flags = new BaseLineItemDO.StatusFlags((BaseLineItemDO.Status)lineItemFlags);

						// Build ledger
						ledger.Add(ledgerLineItemDo);
					}
				}
			}

			this.timer.Stop();
			return ledger;
		}

		/// <summary>
		/// This method will retrieve all the transaction aliases that are owned by the site or
		/// assigned to the site.  In the case of a site group, we only want the aliases that
		/// are owned by the site group.
		/// </summary>
		/// <param name="accountingSite">
		/// The accounting Site.
		/// </param>
		private void GetTransactionAliases(AccountingSite accountingSite)
		{
			this.transAliasListDO = new TransactionAliasListDO();

			using (var cmd = new SqlCommand())
			{
				// Always use the current site guid.  It will be either the group site
				// or a single site. If it is the site group, then we are doing a rollup and we
				// only want the aliases that the site group knows about.
				this.transAliasListDO.getAliasAssignmentsSelectSQL(cmd, accountingSite.CurrentSiteGuid, accountingSite.CurrentSiteGuid);

				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.ledgerSr.Security);

				// Build a list of alias names associated to the site or site group.
				if (dataSet != null)
				{
					this.aliasTypeList = new Hashtable();
					var aliasList = new ArrayList();
					this.transAliasListDO.loadAliasAssignments(dataSet);
					ICollection values = this.transAliasListDO.Values;

					foreach (TransactionAliasDO transAliasDO in values)
					{
						aliasList.Add(transAliasDO.AliasName);

						if (this.aliasTypeList.Contains(transAliasDO.AliasName) == false)
						{
							this.aliasTypeList.Add(transAliasDO.AliasName, transAliasDO.TransactionTypeID);
						}
					}

					// Get the aggreated aliases
					this.GetAggregateAliases(accountingSite);

					this.ledgerSr.AliasList = aliasList;
				}
			}
		}

		/// <summary>
		/// This method will retrieve the configured aggregated aliases.
		/// </summary>
		/// <param name="accountingSite">
		/// The accounting Site.
		/// </param>
		private void GetAggregateAliases(AccountingSite accountingSite)
		{
			using (var cmd = new SqlCommand())
			{
				this.transAliasListDO.PerformAggregateQuery(cmd, accountingSite.CurrentSiteGuid);

				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.ledgerSr.Security);
				this.transAliasListDO.LoadAggregateColumns(dataSet);
			}

			foreach (TransactionAliasDO transAliasDO in this.transAliasListDO.Values)
			{
				if (transAliasDO.IsAggregateAlias && (this.aliasTypeList.Contains(transAliasDO.AliasName) == false))
				{
					this.aliasTypeList.Add(transAliasDO.AliasName, transAliasDO.TransactionTypeID);
				}
			}
		}

		/// <summary>
		/// Sets warning flags such as violation of variance tolerance
		/// </summary>
		/// <remarks>The intent is to set warning flags on the final ledger.
		/// Originally this will only set flags for variance tolerance.</remarks>
		private void SetOutOfToleranceFlag(AccountingSite accountingSite = null)
		{
			if (accountingSite == null)
			{
				var accountingSites = new AccountingSites();
				accountingSite = accountingSites.LoadSiteInfo(this.ledgerSr.Security, this.ledgerSr.CurrentSiteGuid);
			}
			if ((this.ledgerSr.GetRequestType() == LedgerSR.LedgerRequests.ManagerLedger) ||
			  (accountingSite.CurrentSite.EnforceSingleOwner == true))
			{
				foreach (LedgerLineItemDO ledgerItem in this.ledgerDO.LedgerLineItems)
				{
					// Must have both total movement and total variance in order to calculate
					// out of tolerance condition.
					if ((ledgerItem.TotalMovement == null) || (ledgerItem.TotalVariance == null))
					{
						continue;
					}

					if (ledgerItem.VariancePercentage.Gross > ledgerItem.Tolerance)
					{
						ledgerItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_GROSS;
					}

					if (ledgerItem.VariancePercentage.Net > ledgerItem.Tolerance)
					{
						ledgerItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_NET;
					}
				}
			}
		}

		/// <summary>
		/// This method determines if the User is part of the User Group that has a
		/// company that is an owner.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner GUID.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool IsUserAuthorizatedToSeeBookColumns(SecurityClass security, Guid ownerGuid)
		{
			// A manager ledger does not have an owner (Reconciliation).  Therefore,
			// there is no owner.  Return authorized.
			if (this.ledgerSr.GetRequestType() == LedgerSR.LedgerRequests.ManagerLedger)
			{
				return true;
			}

			bool isUserAuthorized = false;

			const string Select = "SELECT DISTINCT cctug.CompanyCompanyToUserGroupGuid AS CompanyToUserGroupGuid ";

			const string From = "FROM map.tblCompanyCompanyToUserGroup cctug "
								+ "LEFT OUTER JOIN map.tblUserToGroup utg ON utg.GroupGuid = cctug.GroupGuid ";

			// The Company Guid check for null is there for the "All" companies assignment.
			const string Where = "WHERE utg.UserGuid = @UserGuid "
								 + "AND utg.SiteGuid = @CurrentSite "
								 + "AND (cctug.CompanyGuid IS NULL OR cctug.CompanyGuid = @OwnerCompanyGuid) ";

			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandText = Select + From + Where;
				var parm = new SqlParameter("@UserGuid", SqlDbType.UniqueIdentifier) { Value = security.UserGuid };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@CurrentSite", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = ownerGuid };
				sqlCommand.Parameters.Add(parm);

				DataSet dataSet = this.ConsolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];
					Guid companyToUserGroupGuid = row.IsNull("CompanyToUserGroupGuid") ? Guid.Empty : (Guid)row["CompanyToUserGroupGuid"];

					if (companyToUserGroupGuid != Guid.Empty)
					{
						isUserAuthorized = true;
					}
				}
			}

			return isUserAuthorized;
		}

		/// <summary>
		/// Sets warning flags such as violation of variance tolerance
		/// </summary>
		/// <param name="product">
		/// The product.
		/// </param>
		/// <remarks>
		/// The intent is to set warning flags on the final ledger.
		/// Originally this will only set flags for variance tolerance.
		/// </remarks>
		private void SetFlags(ProductClass product, AccountingSite accountingSite = null)
		{
			if (accountingSite == null)
			{
				var accountingSites = new AccountingSites();
				accountingSite = accountingSites.LoadSiteInfo(this.ledgerSr.Security, this.ledgerSr.CurrentSiteGuid);
			}

			if ((this.ledgerSr.GetRequestType() == LedgerSR.LedgerRequests.ManagerLedger) ||
			  (accountingSite.CurrentSite.EnforceSingleOwner == true))
			{
				// Get the variance percentage and convert it to a float represenation to match our 
				// comparison calculation in the loop below
				double tolerancePct = product.VarianceTolerance / 100.0;

				foreach (LedgerLineItemDO ledgerItem in this.ledgerDO.LedgerLineItems)
				{
					// Must have both total movement and total variance in order to calculate
					// out of tolerance condition.
					if ((ledgerItem.TotalMovement == null) || (ledgerItem.TotalVariance == null))
					{
						continue;
					}

					// Check gross tolerance.
					if (ledgerItem.TotalMovement.GrossInventoryChange != 0)
					{
						// Calculate the Gross variance percentage by the following formula:
						// Total Variance / Total Movement. Total movement is equal to the sum of
						// type 5 and 6 transactions.
						double variancePct = Math.Abs(ledgerItem.TotalVariance.GrossInventoryChange /
																			 ledgerItem.TotalMovement.GrossInventoryChange);

						if ((variancePct > tolerancePct) || (tolerancePct != 0 && variancePct == tolerancePct))
						{
							ledgerItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_GROSS;
						}
					}
               else
               {
						// If total movement is zero, then we define ANY variance as an out-of-tolerance
						// situation - TFS-143998
						if (ledgerItem.TotalVariance.GrossInventoryChange != 0)
                  {
							ledgerItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_GROSS;
						}
					}

					// Check net tolerance.
 					if (ledgerItem.TotalMovement.NetInventoryChange != 0)
					{
						// Calculate the Net variance percentage by the following formula:
						// Total Variance / Total Movement. Total movement is equal to the sum of
						// type 5 and 6 transactions.
						double variancePct = Math.Abs(ledgerItem.TotalVariance.NetInventoryChange /
																			 ledgerItem.TotalMovement.NetInventoryChange);

						if ((variancePct > tolerancePct) || (tolerancePct != 0 && variancePct == tolerancePct))
						{
							ledgerItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_NET;
						}
					}
					else
					{
						// If total movement is zero, then we define ANY variance as an out-of-tolerance
						// situation - TFS-143998
						if (ledgerItem.TotalVariance.NetInventoryChange != 0)
						{
							ledgerItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_NET;
						}
					}
				}
			}
		}
		#endregion
	}
}
