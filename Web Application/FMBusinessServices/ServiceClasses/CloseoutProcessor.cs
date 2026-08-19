#pragma warning disable 1587
/// <summary>
/// File name:	CloseoutProcessor.cs
/// Purpose:	To decipher the request to retrieve the Closeout
///				data object.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000. This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0 Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		--------------------	----------------------------------
///		2006/06/19		Richard Panachida		There were several problems in calculating the
///														begin, book, and physical inventories. The FROM
///														and TO dates where not being set correctly and
///														the conversion was going to SI instead of the 
///														reverse.
///														There was a problem with the conversion when creating
///														a closeout record. The conversion was going the wrong 
///														way.
///		2006/06/20		Richard Panachida		The TO/FROM dates were getting set correctly prior to 
///														the previous change. Therefore, they were set back to
///														reflect the inventory date.
///		2006/06/21		Richard Panachida		The calculation of book inventory was not working due
///														to the difference in how the ledger processor works. Therefore,
///														removed the call to the ledger and added the correct process
///														to this processor.
///		2006/07/14		Richard Panachida		Modified the code to encompass database transactions for updating
///														all the different tables (CSI 3036).
///		2006-08-08		Richard Panachida		Changed the LedgerSP stored procedure to fm_Ledger.
///		2006-09-15		Richard Panachida		Corrected the problem with the misnamed store procedure. 
///														Added debug logging (CSI 3417).
///		2007-04-03		Richard Panachida		Changed the way the book inventory was being calculated. The code
///														is now using the ledger processor (CSI 4077).
///		2007-04-11		Richard Panachida		The calculation of the new beginning book inventory was off because the previous
///														closeout record was not added to the current book inventory (CSI 4077). 
///		2007-04-13		Richard Panachida		Added a check to the calculation of the new book inventory to see if a previous
///														closeout record existing. If one existed, then calculation is a little different
///														than if one did not exist. (CSI 4077). 
///		2007-04-25		A.Sang					Change the currentSiteIndex from loginSiteIndex to SiteIndex in RetrieveTransactionsForSelectedPeriod CSI4606
///		2007-04-27		G.Kendall				Fixed calculation of beginning book value for multi-owner closeout calculations.
///		2007-11-28		Richard Panachida		7.3.0.4	Removed code for enforcing single owner, not needed (CSI 5246).
///		2008-08-21		W.Gray					7.4.6.1 - Change to not GetAuthorizedCompanies with product
///		2008-08-22		W.Gray					7.4.6.2 - Change to closout blends
///		2008-10-03		W.Gray					7.4.6.3 - Change StampTransactionsForDay to use CmdTimeout = 0 (CSI 6196)
///		2008-10-06		W.Gray					7.4.6.4 - Added CheckBrokenBlend (CSI 6040)
///		2008-12-15		W.Gray					7.4.6.5 - Correction to CheckBrokenBlend to exclude transactions
///														that have been deleted (CSI 6040)
///		2009-01-22		W.Gray					7.4.6.6 - Correction to determine the LastCloseoutDate
///		2009-02-18		W.Gray					7.4.6.7 - Revised to process CLOSEOUT_ALL_COMPLETE (CSI 1543)
///		2009-03-03		W.Gray					7.4.6.8 - Eliminated call to fm_CloseoutTransactions (CSI 992)
///		2009-07-06		W.Gray					7.4.6.9 - Changed CheckBrokenBlend to use NOLOCK (CSI 4581)
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Constants;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using System.Globalization;
	using System.Threading;

	using FMBusinessObjects.Interfaces;

	using Microsoft.SqlServer.Server;
	using FMBusinessObjects.ChannelFactories;

	// [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CloseoutProcessorClass : ICloseoutProcessor
	{
		#region Attributes
		private bool noPhysicalInventories;
		private SecurityClass security;
		private ConsolidatedDAClass consolidatedDA;
		public static int timeout = 600;
		#endregion

		/// <summary>
		/// Get the indicator of whether there were no Physical Inventory transactions
		/// for the closeout date
		/// </summary>
		public bool NoPhysicalInventories => this.noPhysicalInventories;

		#region Constructor
		/// <summary>
		/// This is the default constructor for the closeout processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public CloseoutProcessorClass()
		{
			//this.logger = new LogClient.Logger ( "CloseoutProcessor" );
			this.noPhysicalInventories = false;
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method implements the base class get command method. It will
		/// return the closeout service request command (class name string).
		/// This is used during the registrations of the processors in the accounting
		/// service object.
		/// </summary>
		/// <returns></returns>
		protected void SendEnterpriseCloseout(SecurityClass securityParam, CloseoutSR sr)
		{
			try
			{
				ConfigurationSettingsClass configSettings = new ConfigurationSettingsClass();
				string strEnterpriseIf = configSettings.GetKeyValueByKey(securityParam, ConfigurationSettingDOClass.Key_AccountingEnterpriseInterface);

				if (string.IsNullOrEmpty(strEnterpriseIf) == false)
				{
					char[] separator = { ';' };
					string[] enterpriseIfList = strEnterpriseIf.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					foreach (string assemblyName in enterpriseIfList)
					{
						try
						{
							Assembly dll = null;
							if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
							{ 
								try
								{
									dll = Assembly.LoadFrom(assemblyName.ToString());
								}
								catch
								{
									try
									{
										dll = Assembly.Load(assemblyName);
									}
									catch (Exception ex)
									{
										string message = "Assembly Load Error in Send Enterprise Closeout. " + ex.Message;
										FMEventLog eventLog = new FMEventLog();
										eventLog.WriteEntry(message, FMEventLogEntryType.Warning);
									}
								}

								if (dll != null)
									AssemblyDictionary.Add(assemblyName.ToLower(), dll);
							}
							else
							{
								dll = AssemblyDictionary.Get(assemblyName.ToLower());
							}

							if (dll == null)
								continue;

							try
							{
								Type[] types = dll.GetTypes();

								foreach (Type module in types)
								{
									Type enterprise = module.GetInterface("IEnterprise");

									if (enterprise != null)
									{
										object engine = Activator.CreateInstance(module);
										IEnterprise enterpriseEngine = (IEnterprise)engine;

										enterpriseEngine.Send(securityParam, sr);
									}
								}
							}
							catch { }
						}
						catch (Exception e)
						{
							FMEventLog eventLog = new FMEventLog();
							eventLog.WriteEntry(e.ToString(), FMEventLogEntryType.Error);
						}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry(e.ToString(), FMEventLogEntryType.Error);
			}
		}

		[OperationBehavior(TransactionScopeRequired = false)]
		public CloseoutDO Process(CloseoutSR inCloseoutSR)
		{
			CloseoutSR sr = inCloseoutSR;
			this.security = sr.Security;

			switch (sr.CloseoutCommand)
			{
				case CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP1:
					{
						return this.DoCloseoutsForAllProductsStep1(sr);
					}
				case CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP2:
					{
						return this.DoCloseoutsForAllProductsStep2(sr);
					}
				case CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP3:
					{
						return this.DoCloseoutsForAllProductsStep3(sr);
					}
				case CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS:
					{
						return this.CloseoutAllProducts(sr);
					}
				case CloseoutSR.CloseoutType.CALCULATE:
					{
						return this.Calculate(sr);
					}
				case CloseoutSR.CloseoutType.CALCULATE_FOR_IMPORT:
					{
						return this.ImportCalculateToCompare(sr);
					}
				case CloseoutSR.CloseoutType.CREATE:
					{
						return this.Create(sr);
					}
				case CloseoutSR.CloseoutType.CLOSEOUT_ALL_COMPLETE:
					{
						this.SendEnterpriseCloseout(sr.Security, sr);
						break;
					}

				case CloseoutSR.CloseoutType.GET_TO_EXPORT:
					{
						return this.ExportCloseoutDO(sr.Security, sr);
					}

				case CloseoutSR.CloseoutType.SAVE_TO_IMPORT:
					{
						this.ImportCloseoutDO(sr.Security, sr);
						break;
					}
			}

			return null;
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method will calculate the begin, book, and physical inventories along with the variances.
		/// It uses the ledger to get the begin and book values. It uses the get physical inventory to find
		/// the current physical inventory.
		/// </summary>
		/// <param name="sr"></param>
		/// <returns></returns>
		protected CloseoutDO Calculate(CloseoutSR sr)
		{
			DateTimeOffset txStartDate = sr.InventoryDate;
			DateTimeOffset txEndDate = sr.InventoryDate;

			CloseoutListSR listSR = new CloseoutListSR
			{
				Security = sr.Security,
				Site = sr.Site,
				CurrentSiteGuid = sr.CurrentSiteGuid,
				StartDate = sr.InventoryDate,
				EndDate = sr.InventoryDate,
				ManagerGuid = sr.ManagerCompanyGuid,
				ProductGuid = sr.ProductGuid,
				ConvertUnits = false
			};

			CloseoutListProcessorClass proc = new CloseoutListProcessorClass();
			CloseoutListDO listDO = proc.Process(listSR);

			//If a closeout record already exists for this date, don't create a new one.
			if (listDO.CloseoutList.Count > 0)
			{
				return null;
			}

			//If a closeout record already exists after the requested date, don't create a new one.
			if ((listDO.SubsequentCloseout != null) && (listDO.SubsequentCloseout.CloseoutDate >= sr.InventoryDate))
			{
				return null;
			}

			CloseoutDO closeoutRecord;

			if (listDO.PriorCloseout != null)
			{
				closeoutRecord = listDO.PriorCloseout;
				closeoutRecord.CloseoutRecordFound = true;
				txStartDate = closeoutRecord.CloseoutDate;
				txStartDate = txStartDate.AddDays(1.0);
				closeoutRecord.CloseoutDate = sr.InventoryDate.Date;
			}
			else
			{
				closeoutRecord = new CloseoutDO();
				closeoutRecord.CloseoutRecordFound = false;
				closeoutRecord.SiteID = sr.Site;
				closeoutRecord.ManagerName = sr.ManagerName;
				closeoutRecord.ProductName = sr.ProductName;
				closeoutRecord.CloseoutDate = sr.InventoryDate.Date;
			}

			closeoutRecord.CloseoutDate = sr.InventoryDate.Date;

			// Retrieve all the transactions from the previous closeout to the current closeout inventory date.
			LedgerDO ledgerDO = this.RetrieveTransactionsForSelectedPeriod(txEndDate, sr);

			// Calculate a closeout (system) book inventory using the previous physical inventory as the 
			// beginning inventory.
			QuantityDO newBookInventory = this.CalculateBookInventory(ledgerDO, closeoutRecord, txEndDate);

			// Update the closeout record with the new book inventory for the closeout.
			closeoutRecord.BookInventory = newBookInventory;

			// Get the requested closeout date (current) physical inventory.
			this.SetPhysicalInventory(sr, closeoutRecord);

			// If there are no physical inventories then set the closeout record to
			// null indicating that the user cannot closeout.
			if (this.noPhysicalInventories)
			{
				closeoutRecord = null;
			}

			return closeoutRecord;
		}


		protected CloseoutDO ImportCalculateToCompare(CloseoutSR sr)
		{
			CloseoutDO closeoutRecord = new CloseoutDO();

			closeoutRecord.CloseoutRecordFound = false;
			closeoutRecord.SiteID = sr.Site;

			closeoutRecord.SiteGuid = this.GetSiteGuid(sr.Site, sr.Security);

			closeoutRecord.ManagerName = sr.ManagerName;
			closeoutRecord.ManagerGuid = this.GetManagerGuid(sr.ManagerName, sr.Security);

			closeoutRecord.ProductName = sr.ProductName;
			closeoutRecord.ProductGuid = this.GetProductMasterRecordGuid(sr.ProductName, sr.Security);

			closeoutRecord.CloseoutDate = sr.InventoryDate.Date;

			// Retrieve all the transactions from the previous closeout to the current closeout inventory date.
			LedgerDO ledgerDO = this.RetrieveTransactionsForSelectedPeriod(sr.InventoryDate, sr);

			// Calculate a closeout (system) book inventory using the previous physical inventory as the 
			// beginning inventory.
			QuantityDO newBookInventory = this.CalculateBookInventory(ledgerDO, closeoutRecord, sr.InventoryDate);

			// Update the closeout record with the new book inventory for the closeout.
			closeoutRecord.BookInventory = newBookInventory;

			// Get the requested closeout date (current) physical inventory.
			this.SetPhysicalInventory(sr, closeoutRecord);

			// If there are no physical inventories then set the closeout record to
			// null indicating that the user cannot closeout.
			if (this.noPhysicalInventories)
			{
				closeoutRecord = null;
			}
			else
			{
				// Now calculate a list of OwnerClose out records 
				closeoutRecord.lstOwnerCloseouts = this.CalculateAListOfAllOwnerCloseOutDOs(sr);
			}

			return closeoutRecord;
		}

		protected CloseoutDO CloseoutAllProducts(CloseoutSR sr)
		{
			DateTimeOffset closeoutDate = sr.InventoryDate;
			CompaniesClass comp = new CompaniesClass();
			CompanyClass manager = comp.Get(this.security, sr.ManagerCompanyGuid, false);

			sr.ManagerName = manager.ID;
			sr.CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP1;
			var step1Status = this.DoCloseoutsForAllProductsStep1(sr);
			if (step1Status.Closeouterror)
			{
				return step1Status;
			}

			sr.ManagerName = manager.ID;
			sr.CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP2;
			sr.AllProductsIrdoCollection = step1Status.Nonclosedproductsirdocollection;
			var step2Status = this.DoCloseoutsForAllProductsStep2(sr);
			if (step2Status.Closeouterror)
			{
				sr.CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP3;
				CloseoutDO step3StatusErrCond = this.DoCloseoutsForAllProductsStep3(sr);
				if (step3StatusErrCond.Closeouterror)
				{
					step2Status.Closeouterrtext += "\n" + step3StatusErrCond.Closeouterrtext;
				}
				return step2Status;
			}
			sr.CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP3;
			var step3Status = this.DoCloseoutsForAllProductsStep3(sr);
			return step3Status;
		}


		/// <summary>
		/// This method will find the book inventory in the ledger line item for
		/// the closeout date.
		/// </summary>
		/// <param name="ledgerDO"></param>
		/// <param name="closeoutDO"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		// ReSharper disable once UnusedParameter.Local
		private QuantityDO CalculateBookInventory(LedgerDO ledgerDO, CloseoutDO closeoutDO, DateTimeOffset endDate)
		{
			QuantityDO newBookInventory = new QuantityDO();

			int day = endDate.Day - 1;
			LedgerLineItemCollection lineItems = ledgerDO.LedgerLineItems;

			if ((day >= 0) && (day < lineItems.Count))
			{
				LedgerLineItemDO lineItem = (LedgerLineItemDO)lineItems[day];

				newBookInventory.Gross = lineItem.BookInventory.Gross;
				newBookInventory.Net = lineItem.BookInventory.Net;
				newBookInventory.Mass = lineItem.BookInventory.Mass;
			}
			else
			{
				throw new Exception("Invalid Ledger Data in CalculateBookInventory. Cannot close out.");
			}

			return newBookInventory;
		}

		/// <summary>
		/// This method will retrieve transactions for a given time period using the
		/// fm_Ledger store procedure. It will return an array that contains the physical inventory
		/// data object.
		/// </summary>
		/// <param name="inEndDate"></param>
		/// <param name="closeoutSR"></param>
		/// <returns></returns>
		private LedgerDO RetrieveTransactionsForSelectedPeriod(DateTimeOffset inEndDate, CloseoutSR closeoutSR)
		{
			LedgerSR ledgerSR = new LedgerSR
			{
				Security = closeoutSR.Security,
				Site = closeoutSR.Site,
				CurrentSiteGuid = closeoutSR.Security.SiteGuid,
				ManagerMasterGuid = closeoutSR.ManagerCompanyGuid,
				Manager = closeoutSR.ManagerName,
				Owner = null,
				Product = closeoutSR.ProductName,
				Month = DateEfficacy.ConvertToMonthAndYear(inEndDate)
			};
			//CSI4606

			ledgerSR.SetRequestType(LedgerSR.LedgerRequests.ManagerLedger);

			// Get the ledger data
			LedgerProcessorClass ledgerProcessor = new LedgerProcessorClass();
			var ledgerDO = ledgerProcessor.Process(ledgerSR);

			return ledgerDO;
		}

		/// <summary>
		/// This method will create a closeout record in the owner closeout table and the
		/// closeout inventory table. It will call other methods to set the closeout dates
		/// for the transactions and whether they are partially closed out.
		/// </summary>
		/// <param name="sr"></param>
		/// <returns></returns>
		protected CloseoutDO Create(CloseoutSR sr)
		{
			var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(sr.Security, sites.GetIdentityGuid(sr.Security, sr.Site));

			var products = new ProductsClass();
			ProductClass product = products.GetByProductAuthorizedCompanies(sr.Security, sr.ProductGuid, false);

			var lastCloseoutDO = new CloseoutDO();

			if (this.IsClosedOut(site, sr.ManagerCompanyGuid, sr.ProductGuid, sr.ManagerName, sr.ProductName, sr.InventoryDate, product.ProductType, lastCloseoutDO, sr.Security))
			{
				throw new AccountingServicesException(sr.Site + " " + sr.ManagerName + " " + sr.ProductName + " " +
														sr.InventoryDate.ToString("d") + " already closed out");
			}

			// For component, closeout associated blends
			if (product.ProductType == ProductType.ComponentProduct)
			{
				var productMaps = new ProductMapsClass();
                //Use the MasterRecordGuid to retrieve the list of Blends for which the Product is used as a Component. The Blend Component list and proportion is not subject to Record Versioning.
                ProductMapCollectionClass productMapCollection = productMaps.EnumerateByAssignedGuidAndType(sr.Security, product.MasterRecordGuid, PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP);

				foreach (ProductMapClass productMap in productMapCollection)
				{
					var lastBlendCloseoutDO = new CloseoutDO();

					if (this.IsClosedOut(
						 site,
						 sr.ManagerCompanyGuid,
						 sr.ProductGuid,
						 sr.ManagerName,
						 productMap.AssignedToID,
						 sr.InventoryDate,
						 ProductType.BlendProduct,
						 lastBlendCloseoutDO,
						 sr.Security))
					{
						continue;
					}

					// Get the InventoryReconciliation
					var inventoryReconciliationSR = new InventoryReconciliationSR
					{
						Security = sr.Security,
						Site = sr.Security.SiteID,
						Subrequest =
																		 InventoryReconciliationSR.RequestTypes
																		 .REFRESH,
						ManagerID = sr.ManagerName,
						ProductID = productMap.AssignedToID,
						Month =
																		 DateEfficacy.ConvertToMonthAndYear(
																			  sr.InventoryDate)
					};

					ProductClass blendProduct = products.GetByProductAuthorizedCompanies(sr.Security, productMap.AssignedToGuid, false);
					inventoryReconciliationSR.Tolerance = blendProduct.VarianceTolerance;

					InventoryReconciliationProcessorClass proc = new InventoryReconciliationProcessorClass();
					InventoryReconciliationDO inventoryReconciliation = proc.Process(inventoryReconciliationSR);
					InventoryReconciliationLineItemDO irLineItemDO = inventoryReconciliation.LineItems[sr.InventoryDate.Day - 1] as InventoryReconciliationLineItemDO;

					var blendCloseoutDO = new CloseoutDO
					{
						// ReSharper disable once PossibleNullReferenceException
						CloseoutDate = irLineItemDO.DtInventoryDate.Date,
						BookInventory = irLineItemDO.BookInventory,
						TotalPhysicalInventory = irLineItemDO.TotalPhysicalInventory,
						TotalVariance = irLineItemDO.TotalVariance,
						ManagerName = sr.ManagerName,
						ManagerGuid =
															this.GetCompanyMasterRecordGuid(sr.ManagerName, this.security),
						ProductName = productMap.AssignedToID,
						ProductGuid =
															this.GetProductMasterRecordGuid(
																 productMap.AssignedToID,
																 this.security),
						SiteID = sr.Security.SiteID,
						SiteGuid = sr.Security.SiteGuid
					};

					var blendCloseoutSR = new CloseoutSR
					{
						Closeout = blendCloseoutDO,
						CloseoutCommand = CloseoutSR.CloseoutType.CREATE,
						Security = sr.Security,
						Site = sr.Security.SiteID,
						CurrentSiteGuid = sr.Security.SiteGuid,
						InventoryDate = sr.InventoryDate,
						ManagerName = sr.ManagerName,
						ManagerCompanyGuid =
															this.GetCompanyMasterRecordGuid(sr.ManagerName, this.security),
						ProductName = productMap.AssignedToID,
						ProductGuid =
															this.GetProductMasterRecordGuid(
																 productMap.AssignedToID,
																 this.security)
					};

					this.CreateCloseoutRecordSaveSequence(blendCloseoutSR, new AccountingUnitConversion(site, blendProduct), lastBlendCloseoutDO);
				}
			}

			this.CreateCloseoutRecordSaveSequence(sr, new AccountingUnitConversion(site, product), lastCloseoutDO);

			this.SendEnterpriseCloseout(sr.Security, sr);

			return sr.Closeout;
		}

		/// <summary>
		/// This method determines if a product is closed out for a given inventory date 
		/// </summary>
		public bool IsClosedOut(SiteClass site, Guid managerGuid, Guid productGuid, string managerID, string productID,
			 DateTime inventoryDate, ProductType type, CloseoutDO closeoutDO, SecurityClass securityParam)
		{
			// Determine if blend is closed out
			LedgerSR ledgerSR = new LedgerSR
			{
				Manager = managerID,
				Product = productID,
				Month = DateEfficacy.ConvertToMonthAndYear(inventoryDate),
				Security = this.security
			};

			DataSet dataSet;

			using (SqlCommand cmd = new SqlCommand())
			{
				closeoutDO.GetLatestCloseoutDateSelectSQL(cmd, ledgerSR, site.ID);
				cmd.CommandTimeout = LedgerProcessorClass.timeout;
				dataSet = this.consolidatedDA.GetDataSet(cmd, securityParam);
			}

			if (dataSet != null)
			{
				closeoutDO.loadLatestCloseoutDate(dataSet);

				if (closeoutDO.CloseoutDate >= inventoryDate)
				{
					return true;
				}
			}

			if (type != ProductType.BlendProduct)
			{
				this.CheckBrokenBlend(site, managerGuid, productGuid, managerID, productID, closeoutDO.CloseoutDate, inventoryDate, securityParam);
			}

			return false;
		}

		/// <summary>
		/// This method determines if a broken blend exist in the closeout period 
		/// </summary>
		protected void CheckBrokenBlend(SiteClass site, Guid managerGuid, Guid productGuid, string managerID, string productID,
			 DateTime lastCloseoutDate, DateTime inventoryDate, SecurityClass securityParam)
		{
			// ReSharper disable once RedundantAssignment
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT DocumentNumber,InventoryDate FROM tblTransactions WITH(NOLOCK)" +
							" WHERE TransactionGuid IN (SELECT TransactionGuid" +
							" FROM tblTransactionSubLineItems WITH(NOLOCK)" +
							" WHERE TransactionGuid IN" +
							" (SELECT TransactionGuid" +
							" FROM tblTransactions WITH(NOLOCK)" +
							" WHERE InventoryDate > @LastCloseoutDate " +
							" AND InventoryDate <= @InventoryDate " +
									 " AND SiteGuid = @SiteGuid" +
									 " AND ManagerCompanyGuid = @ManagerCompanyGuid" +
							" AND (DeleteFlag = 0 OR DeleteFlag = NULL))" +
									 " AND ProductGuid = @ProductGuid" +
							" AND BrokenBlend = 1) ORDER BY InventoryDate ASC";

				cmd.Parameters.Add("@LastCloseoutDate", SqlDbType.Date);
				cmd.Parameters.Add("@InventoryDate", SqlDbType.Date);
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = site.IdentityGuid;
				cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier).Value = managerGuid;
				cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = productGuid;

				if (lastCloseoutDate.Year == 1)
				{
					cmd.Parameters["@LastCloseoutDate"].Value = TimeConverter.MinFMDate.Date;
				}
				else
				{
					cmd.Parameters["@LastCloseoutDate"].Value = lastCloseoutDate.Date;
				}

				cmd.Parameters["@InventoryDate"].Value = inventoryDate.Date;

				cmd.CommandTimeout = LedgerProcessorClass.timeout;

				dataSet = this.consolidatedDA.GetDataSet(cmd, securityParam);
			}

			if ((dataSet != null) &&
				(dataSet.Tables.Count != 0) &&
				(dataSet.Tables[0].Rows.Count != 0))
			{
				DateTime begin = DataObject.getValue<DateTime>(dataSet.Tables[0].Rows[0]["InventoryDate"], DateTime.Today);
				DateTime end = DataObject.getValue<DateTime>(dataSet.Tables[0].Rows[dataSet.Tables[0].Rows.Count - 1]["InventoryDate"], DateTime.Today);

				throw new AccountingServicesException(dataSet.Tables[0].Rows.Count + " Broken Blends for " +
														productID + " from " + begin.ToString("d", site.GetDateTimeFormatInfo()) +
														" to " + end.ToString("d", site.GetDateTimeFormatInfo()));
			}
		}



		/// <summary>
		/// This method will retrieve all the physical inventories for the date range, manager, 
		/// and product. If there is a physical inventory that matches today's date, then persist that
		/// inventory.
		/// </summary>
		/// <param name="sr"></param>
		/// <param name="closeoutRecord"></param>
		protected void SetPhysicalInventory(CloseoutSR sr, CloseoutDO closeoutRecord)
		{
			closeoutRecord.TotalPhysicalInventory = new QuantityDO(0, 0, 0, 0);

			PhysicalInventoryListSR inventorySR = new PhysicalInventoryListSR
			{
				Security = sr.Security,
				Site = sr.Site,
				InventoryDate = sr.InventoryDate,
				FirstDate = sr.InventoryDate,
				LastDate = sr.InventoryDate,
				Manager = sr.ManagerName,
				Product = sr.ProductName,
				CurrentSiteGuid = sr.CurrentSiteGuid
			};

			ProductsClass products = new ProductsClass();
			ProductClass product = products.GetByInfoAuthorizedCompanies(sr.Security, sr.ProductGuid, true, false);

			SitesClass sites = new SitesClass();
			SiteClass site = sites.Get(sr.Security, sr.CurrentSiteGuid, false, false, false);

			AccountingUnitConversion converter = new AccountingUnitConversion(site, product);
			PhysicalInventoryListProcessorClass proc = new PhysicalInventoryListProcessorClass();
			PhysicalInventoryListDO inventoryDO = proc.Process(inventorySR);

			this.noPhysicalInventories = true;

			// Check all items for a physical inventory. Only save the physical inventory for 
			// the current date. If there is not a physical inventory for the current date,
			// then it will be zero.
			foreach (PhysicalInventoryLineItemDO lineItem in inventoryDO.LineItems)
			{
				// The physical inventory date must match the current inventory date.
				if (lineItem.InventoryDate == sr.InventoryDate)
				{
					closeoutRecord.TotalPhysicalInventory.Gross = converter.ConvertVolumeFromSI(lineItem.GrossQuantity);
					closeoutRecord.TotalPhysicalInventory.Net = converter.ConvertVolumeFromSI(lineItem.NetQuantity);
					closeoutRecord.TotalPhysicalInventory.Mass = converter.ConvertMassFromSI(lineItem.MassQuantity);

					this.noPhysicalInventories = false;
					break;
				}
			}

			// Set no physical inventories flag to true if there are not any.
			if (inventoryDO.LineItems.Count < 1)
			{
				this.noPhysicalInventories = true;
			}
		}

		/// <summary>
		/// This method will create a database transaction to insert the close out record and update all the
		/// transactions in the database that have to be updated with the closeout date.
		/// </summary>
		/// <param name="closeoutRecordSQL"></param>
		/// <param name="sr"></param>
		private void CreateCloseoutRecordSaveSequence(CloseoutSR sr, AccountingUnitConversion converter, CloseoutDO lastCloseoutDO)
		{
			CloseoutDO closeoutDO = (CloseoutDO)sr.Closeout;

			// Convert value to SI.
			closeoutDO.BookInventory.GrossInventoryChange = converter.ConvertVolumeToSI(closeoutDO.BookInventory.GrossInventoryChange);
			closeoutDO.BookInventory.NetInventoryChange = converter.ConvertVolumeToSI(closeoutDO.BookInventory.NetInventoryChange);
			closeoutDO.BookInventory.MassInventoryChange = converter.ConvertMassToSI(closeoutDO.BookInventory.MassInventoryChange);
			closeoutDO.BookInventory.GrossPriceInventoryChange = converter.ConvertVolumeToSI(closeoutDO.BookInventory.GrossPriceInventoryChange);
			closeoutDO.BookInventory.NetPriceInventoryChange = converter.ConvertVolumeToSI(closeoutDO.BookInventory.NetPriceInventoryChange);
			closeoutDO.BookInventory.MassPriceInventoryChange = converter.ConvertMassToSI(closeoutDO.BookInventory.MassPriceInventoryChange);
			closeoutDO.TotalPhysicalInventory.GrossInventoryChange = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.GrossInventoryChange);
			closeoutDO.TotalPhysicalInventory.NetInventoryChange = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.NetInventoryChange);
			closeoutDO.TotalPhysicalInventory.MassInventoryChange = converter.ConvertMassToSI(closeoutDO.TotalPhysicalInventory.MassInventoryChange);
			closeoutDO.TotalPhysicalInventory.GrossPriceInventoryChange = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.GrossPriceInventoryChange);
			closeoutDO.TotalPhysicalInventory.NetPriceInventoryChange = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.NetPriceInventoryChange);
			closeoutDO.TotalPhysicalInventory.MassPriceInventoryChange = converter.ConvertMassToSI(closeoutDO.TotalPhysicalInventory.MassPriceInventoryChange);
			closeoutDO.TotalVariance.GrossInventoryChange = converter.ConvertVolumeToSI(closeoutDO.TotalVariance.GrossInventoryChange);
			closeoutDO.TotalVariance.NetInventoryChange = converter.ConvertVolumeToSI(closeoutDO.TotalVariance.NetInventoryChange);
			closeoutDO.TotalVariance.MassInventoryChange = converter.ConvertMassToSI(closeoutDO.TotalVariance.MassInventoryChange);

			List<OwnerCloseoutDAO> ownerCloseouts = new List<OwnerCloseoutDAO>();

			// Retrieve all the owners prior to the start of the transaction.
			var companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = companies.EnumerateByRole(sr.Security, COMPANY_ROLE.OWNER, false);

			var accountingSites = new AccountingSites();
			var accountingSite = accountingSites.LoadSiteInfoNoCompanies(sr.Security, sr.Security.SiteGuid);

			// Retrieve all the owner book inventory volumes and build a collections for
			// processing the them.
			foreach (CompanyClass company in companyCollection)
			{
				var ownerCloseoutDAO = new OwnerCloseoutDAO(sr.Security)
				{
					CompanyID = company.ID,
					CompanyGuid = company.MasterRecordGuid,
					SR = sr
				};
				ownerCloseoutDAO.BookInv = ownerCloseoutDAO.GetBookInventories(accountingSite);

				ownerCloseoutDAO.BookInv.GrossInventoryChange = converter.ConvertVolumeToSI(ownerCloseoutDAO.BookInv.GrossInventoryChange);
				ownerCloseoutDAO.BookInv.NetInventoryChange = converter.ConvertVolumeToSI(ownerCloseoutDAO.BookInv.NetInventoryChange);
				ownerCloseoutDAO.BookInv.MassInventoryChange = converter.ConvertMassToSI(ownerCloseoutDAO.BookInv.MassInventoryChange);
				ownerCloseoutDAO.BookInv.GrossPriceInventoryChange = converter.ConvertVolumeToSI(ownerCloseoutDAO.BookInv.GrossPriceInventoryChange);
				ownerCloseoutDAO.BookInv.NetPriceInventoryChange = converter.ConvertVolumeToSI(ownerCloseoutDAO.BookInv.NetPriceInventoryChange);
				ownerCloseoutDAO.BookInv.MassPriceInventoryChange = converter.ConvertMassToSI(ownerCloseoutDAO.BookInv.MassPriceInventoryChange);

				ownerCloseouts.Add(ownerCloseoutDAO);
			}

			this.SaveToDatabase(sr, closeoutDO, ownerCloseouts);

			// After committing Closeout Record que for export 
			this.InsertInChangeQueueToTrackCloseoutRec(sr);
		}

		private void SaveToDatabase(CloseoutSR sr, CloseoutDO closeoutDO, List<OwnerCloseoutDAO> ownerCloseouts)
		{
			try
			{
				using (SqlCommand sqlCommand = new SqlCommand())
				{
					closeoutDO.GetInsertCommand(sqlCommand, sr.Security.UserID);
					sqlCommand.CommandTimeout = LedgerProcessorClass.timeout;

					// Create the closeout records in the closeout table.
					this.consolidatedDA.ExecuteQuery(this.security, sqlCommand);
				}

				this.SaveOwnerCloseouts(ownerCloseouts);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error creating closeout record " + ex.Message);
			}
		}

		/// <summary>
		/// Save a list of owner closeout records
		/// </summary>
		/// <param name="ownerCloseouts">The owner closeouts to save</param>
		public void SaveOwnerCloseouts(List<OwnerCloseoutDAO> ownerCloseouts)
		{
			using (var ownerCloseoutSqlCommand = new SqlCommand())
			{
				ownerCloseoutSqlCommand.CommandTimeout = LedgerProcessorClass.timeout;
				ownerCloseoutSqlCommand.CommandType = CommandType.StoredProcedure;
				ownerCloseoutSqlCommand.CommandText = "usp_OwnerCloseoutInsert";

				SqlParameter tableValuedParameter = ownerCloseoutSqlCommand.Parameters.Add("@OwnerCloseouts", SqlDbType.Structured);
				tableValuedParameter.Value = CreateSqlDataRecordsForOwnerCloseoutInsert(ownerCloseouts);
				tableValuedParameter.TypeName = "dbo.OwnerCloseoutType";
				this.consolidatedDA.ExecuteQuery(this.security, ownerCloseoutSqlCommand);
			}
		}

		/// <summary>
		/// Create SqlDataRecords representing owner closeout records to insert
		/// </summary>
		/// <param name="ownerCloseouts">The owner closeout records to create SqlDataRecords for</param>
		/// <returns>SqlDataRecords representing owner closeout records to insert</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForOwnerCloseoutInsert(IEnumerable<OwnerCloseoutDAO> ownerCloseouts)
		{
			SqlMetaData[] metaData = new SqlMetaData[16];

			int i = 0;
			metaData[i++] = new SqlMetaData("Site", SqlDbType.NVarChar, 30);
			metaData[i++] = new SqlMetaData("ManagerName", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("OwnerName", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("ProductName", SqlDbType.NVarChar, 30);
			metaData[i++] = new SqlMetaData("CloseoutDate", SqlDbType.Date);
			metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("OwnerCompanyGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("ProductGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("GrossBookInventory", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("NetBookInventory", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("MassBookInventory", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("GrossBookPrice", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("NetBookPrice", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("MassBookPrice", SqlDbType.Float);
			// ReSharper disable once RedundantAssignment
			metaData[i++] = new SqlMetaData("CreatedBy", SqlDbType.NVarChar, 30);

			SqlDataRecord record = new SqlDataRecord(metaData);

			foreach (OwnerCloseoutDAO ownerCloseout in ownerCloseouts)
			{
				int j = 0;

				record.SetString(j++, ownerCloseout.SR.Site);
				record.SetString(j++, ownerCloseout.SR.ManagerName);
				record.SetString(j++, ownerCloseout.CompanyID);
				record.SetString(j++, ownerCloseout.SR.ProductName);
				record.SetDateTime(j++, ownerCloseout.SR.InventoryDate);
				record.SetNullableGuid(j++, ownerCloseout.SR.CurrentSiteGuid);
				record.SetNullableGuid(j++, ownerCloseout.SR.ManagerCompanyGuid);
				record.SetNullableGuid(j++, ownerCloseout.CompanyGuid);
				record.SetNullableGuid(j++, ownerCloseout.SR.ProductGuid);
				record.SetNullableDouble(j++, ownerCloseout.BookInv.GrossInventoryChange);
				record.SetNullableDouble(j++, ownerCloseout.BookInv.NetInventoryChange);
				record.SetNullableDouble(j++, ownerCloseout.BookInv.MassInventoryChange);
				record.SetNullableDouble(j++, ownerCloseout.BookInv.GrossPriceInventoryChange);
				record.SetNullableDouble(j++, ownerCloseout.BookInv.NetPriceInventoryChange);
				record.SetNullableDouble(j++, ownerCloseout.BookInv.MassPriceInventoryChange);
				// ReSharper disable once RedundantAssignment
				record.SetString(j++, ownerCloseout.SR.Security.UserID);

				yield return record;
			}
		}

		protected void InsertInChangeQueueToTrackCloseoutRec(CloseoutSR sr)
		{
			using (SqlCommand sqlCommandToGetLastID = new SqlCommand())
			{
				// Now get the CloseOutInventoryID record that was just inserted. 				
				CloseoutDO closeoutDO = new CloseoutDO();

				closeoutDO.GetLastCloseoutSelectSQL(sqlCommandToGetLastID);
				sqlCommandToGetLastID.CommandType = CommandType.Text;

				Guid closeOutInventoryGuid = Guid.Empty;
				string strProductName = "";

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommandToGetLastID, sr.Security);

				if (dataSet?.Tables != null && (dataSet.Tables.Count > 0))
				{
					DataTable table = dataSet.Tables[0];

					if (table != null)
					{
						if (table.Rows.Count > 0)
						{
							DataRow row = table.Rows[0];
							closeOutInventoryGuid = (Guid)row["CloseOutInventoryGuid"];
							strProductName = (string)row["ProductName"];

							// TODO: Temporary commented out so that QA does not test change queue features.
							// with the CloseOutInventoryID and the Product Name add a tracking record to the ChangeQueue table. 
							// ChangeQueueRecordsClass.ProcessChangeCloseOutQueueRecords(security, ChangeQueueEventType.Add, closeOutInventoryGuid, strProductName);
						}
					}
				}
			}
		}

		protected CloseoutDO ExportCloseoutDO(SecurityClass securityParam, CloseoutSR sr)
		{
			CloseoutDO clsout = new CloseoutDO();

			// GetThe record CloseoutInventory ID from Changes Queue				
			using (SqlCommand sqlCommand = new SqlCommand())
			{
				clsout.GetSQLForCloseOutRec(sqlCommand, sr.CloseoutInventoryGuid);

				DataSet cslOutInvntryDataSet = this.consolidatedDA.GetDataSet(sqlCommand, sr.Security);
				clsout.loadCloseoutUsingColumnName(cslOutInvntryDataSet);

				// Check to see if there are ownercloseouts
				this.AttachTheOwnerClosedOuts(ref clsout, sr);

				return clsout;
			}
		}

		protected void AttachTheOwnerClosedOuts(ref CloseoutDO clsout, CloseoutSR sr)
		{
			// get owner close out records. 
			if ((clsout.ManagerGuid == Guid.Empty) || (clsout.ProductGuid == Guid.Empty))
			{
				// need to check if the ManagerGuid is null and the ProductGuid is null. 
				return;
			}

			OwnerCloseoutDO ownrClsOut = new OwnerCloseoutDO();

			Guid managerGuid = clsout.ManagerGuid;

			Guid productGuid = clsout.ProductGuid;

			using (SqlCommand sqlCmmnd = new SqlCommand())
			{
				string sqlToGetAssociatedOwnerClsOutRecs = ownrClsOut.SQLToRetriveAssociatedOwnerCloseOutRecs(clsout.CloseoutDate, managerGuid, productGuid);

				sqlCmmnd.CommandText = sqlToGetAssociatedOwnerClsOutRecs;
				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCmmnd, sr.Security);

				// fill the list of owner close outs 
				if (dataSet?.Tables != null && (dataSet.Tables.Count > 0))
				{
					DataTable table = dataSet.Tables[0];

					if ((table.Rows != null) && (table.Rows.Count > 0))
					{
						int i = 0;

						while (i < table.Rows.Count)
						{
							DataRow row = table.Rows[i];
							OwnerCloseoutDO nextOwnrClsOut = new OwnerCloseoutDO();

							nextOwnrClsOut.loadCurrentOwnerCloseoutUsingColumnName(row);
							clsout.lstOwnerCloseouts.Add(nextOwnrClsOut);
							i++;
						}
					}
				}
			}
		}

		protected void ImportCloseoutDO(SecurityClass securityParam, CloseoutSR sr)
		{
			CloseoutDO closeoutDO = sr.Closeout;
			this.ImportCloseoutDO(closeoutDO, securityParam, sr.Force, sr.ConvertUnits, sr.Tolerance);
		}

		// The ImportCloseoutDO function was developed and needed to verify with FM 8.0. The code is under 
		// DataSynchronizationProcessor/DataSynchronizationProcessorClass.cs
		// There is a ImportCloseoutDO function in the project. See the code below: 
		private Guid GetSiteGuid(string siteID, SecurityClass securityParam)
		{
			//get site guid first because every other guids need siteguid
			var sites = new SitesClass();
			Guid siteGuid = sites.GetIdentityGuid(securityParam, siteID);

			if (siteGuid == Guid.Empty)
			{
				throw new Exception("Site " + siteID + " is not configured in the database.");
			}
			else
			{
				return siteGuid;
			}
		}

		//verify manager and product are configured and assigned to this site
		private Guid GetManagerGuid(string managerID, SecurityClass securityParam)
		{
			var managers = new CompaniesClass();
			Guid managerGuid = managers.GetMasterRecordGuid(securityParam, managerID);

			if (managerGuid == Guid.Empty)
			{
				throw new Exception("Manager " + managerID + " is not configured in the database.");
			}

			//check if this company has manager role
			CompanyClass manager = managers.Get(securityParam, managerGuid);

			foreach (CompanyRoleMapClass roleMap in manager.RoleCollection)
			{
				if (roleMap.Role == COMPANY_ROLE.MANAGER)
				{
					return managerGuid;
				}
			}

			//no manager role
			throw new Exception("Manager " + managerID + " is not assigned as Manager.");
		}

		private Guid GetCompanyMasterRecordGuid(string companyID, SecurityClass securityParam)
		{
			//verify company
			var companies = new CompaniesClass();
			Guid masterRecordGuid = companies.GetMasterRecordGuid(securityParam, companyID);

			if (masterRecordGuid == Guid.Empty)
			{
				throw new Exception("Company " + companyID + " is not configured in the database.");
			}
			else
			{
				return masterRecordGuid;
			}
		}

		private Guid GetProductMasterRecordGuid(string productID, SecurityClass securityParam)
		{
			//verify product
			var products = new ProductsClass();
			Guid masterRecordGuid = products.GetMasterRecordGuidFromID(securityParam, productID);

			if (masterRecordGuid == Guid.Empty)
			{
				throw new Exception("Product " + productID + " is not configured in the database.");
			}
			else
			{
				return masterRecordGuid;
			}
		}

		private void ConvertCloseoutToSI(AccountingUnitConversion converter, CloseoutDO closeoutDO)
		{

			if (closeoutDO.BookInventory.NullableGross.HasValue)
			{
				closeoutDO.BookInventory.Gross = converter.ConvertVolumeToSI(closeoutDO.BookInventory.Gross);
			}

			if (closeoutDO.BookInventory.NullableNet.HasValue)
			{
				closeoutDO.BookInventory.Net = converter.ConvertVolumeToSI(closeoutDO.BookInventory.Net);
			}

			if (closeoutDO.BookInventory.NullableMass.HasValue)
			{
				closeoutDO.BookInventory.Mass = converter.ConvertMassToSI(closeoutDO.BookInventory.Mass);
			}

			closeoutDO.BookInventory.GrossPrice = converter.ConvertVolumeToSI(closeoutDO.BookInventory.GrossPrice);
			closeoutDO.BookInventory.NetPrice = converter.ConvertVolumeToSI(closeoutDO.BookInventory.NetPrice);
			closeoutDO.BookInventory.MassPrice = converter.ConvertMassToSI(closeoutDO.BookInventory.MassPrice);

			if (closeoutDO.TotalPhysicalInventory.NullableGross.HasValue)
			{
				closeoutDO.TotalPhysicalInventory.Gross = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.Gross);
			}

			if (closeoutDO.TotalPhysicalInventory.NullableNet.HasValue)
			{
				closeoutDO.TotalPhysicalInventory.Net = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.Net);
			}

			if (closeoutDO.TotalPhysicalInventory.NullableMass.HasValue)
			{
				closeoutDO.TotalPhysicalInventory.Mass = converter.ConvertMassToSI(closeoutDO.TotalPhysicalInventory.Mass);
			}

			closeoutDO.TotalPhysicalInventory.GrossPrice = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.GrossPrice);
			closeoutDO.TotalPhysicalInventory.NetPrice = converter.ConvertVolumeToSI(closeoutDO.TotalPhysicalInventory.NetPrice);
			closeoutDO.TotalPhysicalInventory.MassPrice = converter.ConvertMassToSI(closeoutDO.TotalPhysicalInventory.MassPrice);

			if (closeoutDO.TotalVariance.NullableGross.HasValue)
			{
				closeoutDO.TotalVariance.Gross = converter.ConvertVolumeToSI(closeoutDO.TotalVariance.Gross);
			}

			if (closeoutDO.TotalVariance.NullableNet.HasValue)
			{
				closeoutDO.TotalVariance.Net = converter.ConvertVolumeToSI(closeoutDO.TotalVariance.Net);
			}

			if (closeoutDO.TotalVariance.NullableMass.HasValue)
			{
				closeoutDO.TotalVariance.Mass = converter.ConvertMassToSI(closeoutDO.TotalVariance.Mass);
			}

			foreach (OwnerCloseoutDO ownerCloseoutDO in closeoutDO.lstOwnerCloseouts)
			{
				if (ownerCloseoutDO.BookInventory.NullableGross.HasValue)
				{
					ownerCloseoutDO.BookInventory.GrossInventoryChange =
						 converter.ConvertVolumeToSI(ownerCloseoutDO.BookInventory.GrossInventoryChange);
				}

				if (ownerCloseoutDO.BookInventory.NullableNet.HasValue)
				{
					ownerCloseoutDO.BookInventory.NetInventoryChange =
						 converter.ConvertVolumeToSI(ownerCloseoutDO.BookInventory.NetInventoryChange);
				}

				if (ownerCloseoutDO.BookInventory.NullableMass.HasValue)
				{
					ownerCloseoutDO.BookInventory.MassInventoryChange =
						 converter.ConvertMassToSI(ownerCloseoutDO.BookInventory.MassInventoryChange);
				}

				ownerCloseoutDO.BookInventory.GrossPriceInventoryChange = converter.ConvertVolumeToSI(ownerCloseoutDO.BookInventory.GrossPriceInventoryChange);
				ownerCloseoutDO.BookInventory.NetPriceInventoryChange = converter.ConvertVolumeToSI(ownerCloseoutDO.BookInventory.NetPriceInventoryChange);
				ownerCloseoutDO.BookInventory.MassPriceInventoryChange = converter.ConvertMassToSI(ownerCloseoutDO.BookInventory.MassPriceInventoryChange);
			}
		}


		private void ConvertCloseoutFromSI(AccountingUnitConversion converter, CloseoutDO closeoutDO)
		{
			if (closeoutDO.BookInventory.NullableGross.HasValue)
			{
				closeoutDO.BookInventory.Gross = converter.ConvertVolumeFromSI(closeoutDO.BookInventory.Gross);
			}

			if (closeoutDO.BookInventory.NullableNet.HasValue)
			{
				closeoutDO.BookInventory.Net = converter.ConvertVolumeFromSI(closeoutDO.BookInventory.Net);
			}

			if (closeoutDO.BookInventory.NullableMass.HasValue)
			{
				closeoutDO.BookInventory.Mass = converter.ConvertMassFromSI(closeoutDO.BookInventory.Mass);
			}

			closeoutDO.BookInventory.GrossPrice = converter.ConvertVolumeFromSI(closeoutDO.BookInventory.GrossPrice);
			closeoutDO.BookInventory.NetPrice = converter.ConvertVolumeFromSI(closeoutDO.BookInventory.NetPrice);
			closeoutDO.BookInventory.MassPrice = converter.ConvertMassFromSI(closeoutDO.BookInventory.MassPrice);

			if (closeoutDO.TotalPhysicalInventory.NullableGross.HasValue)
			{
				closeoutDO.TotalPhysicalInventory.Gross =
					 converter.ConvertVolumeFromSI(closeoutDO.TotalPhysicalInventory.Gross);
			}

			if (closeoutDO.TotalPhysicalInventory.NullableNet.HasValue)
			{
				closeoutDO.TotalPhysicalInventory.Net = converter.ConvertVolumeFromSI(closeoutDO.TotalPhysicalInventory.Net);
			}

			if (closeoutDO.TotalPhysicalInventory.NullableMass.HasValue)
			{
				closeoutDO.TotalPhysicalInventory.Mass = converter.ConvertMassFromSI(closeoutDO.TotalPhysicalInventory.Mass);
			}

			closeoutDO.TotalPhysicalInventory.GrossPrice =
			converter.ConvertVolumeFromSI(closeoutDO.TotalPhysicalInventory.GrossPrice);
			closeoutDO.TotalPhysicalInventory.NetPrice = converter.ConvertVolumeFromSI(
				closeoutDO.TotalPhysicalInventory.NetPrice);
			closeoutDO.TotalPhysicalInventory.MassPrice = converter.ConvertMassFromSI(
				closeoutDO.TotalPhysicalInventory.MassPrice);

			if (closeoutDO.TotalVariance.NullableGross.HasValue)
			{
				closeoutDO.TotalVariance.Gross = converter.ConvertVolumeFromSI(closeoutDO.TotalVariance.Gross);
			}

			if (closeoutDO.TotalVariance.NullableNet.HasValue)
			{
				closeoutDO.TotalVariance.Net = converter.ConvertVolumeFromSI(closeoutDO.TotalVariance.Net);
			}

			if (closeoutDO.TotalVariance.NullableMass.HasValue)
			{
				closeoutDO.TotalVariance.Mass = converter.ConvertMassFromSI(closeoutDO.TotalVariance.Mass);
			}

			foreach (OwnerCloseoutDO ownerCloseoutDO in closeoutDO.lstOwnerCloseouts)
			{
				if (ownerCloseoutDO.BookInventory.NullableGross.HasValue)
				{
					ownerCloseoutDO.BookInventory.GrossInventoryChange =
						 converter.ConvertVolumeFromSI(ownerCloseoutDO.BookInventory.GrossInventoryChange);
				}

				if (ownerCloseoutDO.BookInventory.NullableNet.HasValue)
				{
					ownerCloseoutDO.BookInventory.NetInventoryChange =
						 converter.ConvertVolumeFromSI(ownerCloseoutDO.BookInventory.NetInventoryChange);
				}

				if (ownerCloseoutDO.BookInventory.NullableMass.HasValue)
				{
					ownerCloseoutDO.BookInventory.MassInventoryChange =
						 converter.ConvertMassFromSI(ownerCloseoutDO.BookInventory.MassInventoryChange);
				}

				ownerCloseoutDO.BookInventory.GrossPriceInventoryChange =
			  converter.ConvertVolumeFromSI(ownerCloseoutDO.BookInventory.GrossPriceInventoryChange);
				ownerCloseoutDO.BookInventory.NetPriceInventoryChange =
					converter.ConvertVolumeFromSI(ownerCloseoutDO.BookInventory.NetPriceInventoryChange);
				ownerCloseoutDO.BookInventory.MassPriceInventoryChange =
					converter.ConvertMassFromSI(ownerCloseoutDO.BookInventory.MassPriceInventoryChange);
			}
		}

		private void ImportCloseoutDO(CloseoutDO closeoutDOImport, SecurityClass securityParam, bool force = false, bool convertUnits = true, double tolerance = 0.0)
		{
			CloseoutSR closeoutSR = new CloseoutSR
			{
				Closeout = closeoutDOImport,
				Security = securityParam,
				Site = closeoutDOImport.SiteID,
				CurrentSiteGuid =
													  this.GetSiteGuid(closeoutDOImport.SiteID, securityParam),
				Force = force,
				ConvertUnits = convertUnits
			};


			//get site Guid first because every other Guid need siteGuid
			closeoutSR.Closeout.SiteGuid = closeoutSR.CurrentSiteGuid;


			//verify manager and product are configured and assigned to this site
			closeoutSR.ManagerCompanyGuid = this.GetManagerGuid(closeoutDOImport.ManagerName, securityParam);
			closeoutSR.ManagerName = closeoutDOImport.ManagerName;
			closeoutSR.Closeout.ManagerGuid = closeoutSR.ManagerCompanyGuid;

			//verify product
			closeoutSR.ProductName = closeoutDOImport.ProductName;
			closeoutSR.ProductGuid = this.GetProductMasterRecordGuid(closeoutDOImport.ProductName, securityParam);
			closeoutSR.Closeout.ProductGuid = closeoutSR.ProductGuid;
			closeoutSR.InventoryDate = closeoutDOImport.CloseoutDate;
			closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CALCULATE_FOR_IMPORT; //calculate first to verify if numbers match

			CloseoutProcessorClass closeoutProcessor = new CloseoutProcessorClass();
			CloseoutDO closeoutRecCalculated = closeoutProcessor.Process(closeoutSR);

			//verify numbers match the closeoutDO from local site
			if (force || closeoutRecCalculated != null)
			{
				var sites = new SitesClass();
				var site = sites.Get(securityParam, closeoutSR.CurrentSiteGuid, false, false, false);
				var products = new ProductsClass();
				var product = products.Get(securityParam, closeoutSR.ProductGuid);

				AccountingUnitConversion converter = new AccountingUnitConversion(site, product);

				if (convertUnits)
				{
					this.ConvertCloseoutFromSI(converter, closeoutDOImport);
				}

				bool bRecAreTheSame = force || this.CompareCloseoutRecs(closeoutDOImport, closeoutRecCalculated, tolerance);

				if (bRecAreTheSame)
				{
					// Compare the owner list of close out records.
					bool bOwerCloseoutLstAreTheSame = force || this.Compare2ListOfOwnerCloseoutRecs(closeoutDOImport.lstOwnerCloseouts,
																							closeoutRecCalculated.lstOwnerCloseouts, tolerance);

					if (bOwerCloseoutLstAreTheSame)
					{
						if (convertUnits)
						{
							this.ConvertCloseoutToSI(converter, closeoutDOImport);
						}

						List<OwnerCloseoutDAO> ownerCloseouts = new List<OwnerCloseoutDAO>();

						foreach (OwnerCloseoutDO ownerCloseoutDO in closeoutDOImport.lstOwnerCloseouts)
						{
							OwnerCloseoutDAO ownerCloseoutDAO = new OwnerCloseoutDAO(closeoutSR.Security)
							{
								CompanyID = ownerCloseoutDO.OwnerName,
								CompanyGuid = this.GetCompanyMasterRecordGuid
																					  (
																							ownerCloseoutDO.OwnerName,
																							securityParam),
								SR = closeoutSR,
								BookInv = ownerCloseoutDO.BookInventory
							};
							ownerCloseouts.Add(ownerCloseoutDAO);
						}

						this.SaveToDatabase(closeoutSR, closeoutDOImport, ownerCloseouts);
					}
					else
					{
						throw new Exception(" Fail to closeout because imported owner closeout record does not match calculated owner closeout record. ");
					}
				}
				else
				{
					throw new Exception(" Fail to closeout because imported closeout record does not match calculated closeout record. ");
				}
			}
			else
			{
				throw new Exception("Fail to closeout because insufficient data exists to calculate comparison closeout record");
			}
		}

		public bool CompareCloseoutRecs(CloseoutDO clsOutImported, CloseoutDO clsOutCalculated, double tolerance)
		{
			bool bSame = true;
			string strClsOutDate = clsOutImported.CloseoutDate.ToString("d");
			string msg =
				 $"Closeout record (Closeout date: {strClsOutDate}, Product: {clsOutImported.ProductName}) does not match the calculated result. ";

			if (clsOutImported.SiteID != clsOutCalculated.SiteID)
			{
				bSame = false;
				if ((clsOutImported.SiteID != null) && (clsOutCalculated.SiteID != null))
				{
					msg += $"Imported.SiteID = {clsOutImported.SiteID}, Calculated.SiteID = {clsOutCalculated.SiteID}. ";
				}
			}

			// if the site id matches there is no need to compare siteguids. 
			if (clsOutImported.CloseoutDate != clsOutCalculated.CloseoutDate)
			{
				bSame = false;
				msg +=
					 $"Imported.CloseoutDate = {clsOutImported.CloseoutDate}, Calculated.CloseoutDate = {clsOutCalculated.CloseoutDate}. ";
			}

			if (clsOutImported.ProductName != clsOutCalculated.ProductName)
			{
				bSame = false;
				if ((clsOutImported.ProductName != null) && (clsOutCalculated.ProductName != null))
				{
					msg +=
						 $"Imported.ProductName = {clsOutImported.ProductName}, Calculated.ProductName = {clsOutCalculated.ProductName}. ";
				}
			}

			if (clsOutImported.ManagerName != clsOutCalculated.ManagerName)
			{
				bSame = false;
				if ((clsOutImported.ManagerName != null) && (clsOutCalculated.ManagerName != null))
				{
					msg +=
						 $"Imported.ManagerName = {clsOutImported.ManagerName}, Calculated.ManagerName = {clsOutCalculated.ManagerName}. ";
				}
			}

			if (clsOutImported.BookInventory.NullableGross.HasValue)
			{
				if (Math.Abs((clsOutImported.BookInventory.Gross - clsOutCalculated.BookInventory.Gross) / clsOutCalculated.BookInventory.Gross) > tolerance)
				{
					bSame = false;
					msg += $"Imported.BookInventory.Gross = {clsOutImported.BookInventory.Gross}, Calculated.BookInventory.Gross = {clsOutCalculated.BookInventory.Gross}. ";
				}
			}

			if (clsOutImported.BookInventory.NullableNet.HasValue)
			{
				if (Math.Abs((clsOutImported.BookInventory.Net - clsOutCalculated.BookInventory.Net) / clsOutCalculated.BookInventory.Net) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.BookInventory.Net = {clsOutImported.BookInventory.Net}, Calculated.BookInventory.Net = {clsOutCalculated.BookInventory.Net}. ";
				}
			}

			if (clsOutImported.TotalPhysicalInventory.NullableGross.HasValue)
			{
				if (Math.Abs((clsOutImported.TotalPhysicalInventory.Gross - clsOutCalculated.TotalPhysicalInventory.Gross) / clsOutCalculated.TotalPhysicalInventory.Gross) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.TotalPhysicalInventory.Gross = {clsOutImported.TotalPhysicalInventory.Gross}, Calculated.TotalPhysicalInventory.Gross = {clsOutCalculated.TotalPhysicalInventory.Gross}. ";
				}
			}

			if (clsOutImported.TotalPhysicalInventory.NullableNet.HasValue)
			{
				if (Math.Abs((clsOutImported.TotalPhysicalInventory.Net - clsOutCalculated.TotalPhysicalInventory.Net) / clsOutCalculated.TotalPhysicalInventory.Net) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.TotalPhysicalInventory.Net = {clsOutImported.TotalPhysicalInventory.Net}, Calculated.TotalPhysicalInventory.Net = {clsOutCalculated.TotalPhysicalInventory.Net}. ";
				}
			}

			if (Math.Abs((clsOutImported.BookInventory.GrossPrice - clsOutCalculated.BookInventory.GrossPrice) / clsOutCalculated.BookInventory.GrossPrice) > tolerance)
			{
				bSame = false;
				msg +=
					 $"Imported.BookInventory.GrossPrice = {clsOutImported.BookInventory.GrossPrice}, Calculated.BookInventory.GrossPrice = {clsOutCalculated.BookInventory.GrossPrice}. ";
			}

			if (Math.Abs((clsOutImported.BookInventory.NetPrice - clsOutCalculated.BookInventory.NetPrice) / clsOutCalculated.BookInventory.NetPrice) > tolerance)
			{
				bSame = false;
				msg +=
					 $"Imported.BookInventory.NetPrice = {clsOutImported.BookInventory.NetPrice}, Calculated.BookInventory.NetPrice = {clsOutCalculated.BookInventory.NetPrice}. ";
			}

			if (clsOutImported.BookInventory.NullableMass.HasValue)
			{
				if (Math.Abs((clsOutImported.BookInventory.Mass - clsOutCalculated.BookInventory.Mass) / clsOutCalculated.BookInventory.Mass) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.BookInventory.Mass = {clsOutImported.BookInventory.Mass}, Calculated.BookInventory.Mass = {clsOutCalculated.BookInventory.Mass}. ";
				}
			}

			if (clsOutImported.TotalPhysicalInventory.NullableMass.HasValue)
			{
				if (Math.Abs((clsOutImported.TotalPhysicalInventory.Mass - clsOutCalculated.TotalPhysicalInventory.Mass) / clsOutCalculated.TotalPhysicalInventory.Mass) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.BookInventory.MassInventoryChange = {clsOutImported.TotalPhysicalInventory.Mass}, Calculated.BookInventory.MassInventoryChange = {clsOutCalculated.TotalPhysicalInventory.Mass}. ";
				}
			}

			if (bSame == false)
			{
				msg +=
					 $"This indicates the Enterprise server transactions for {clsOutImported.ProductName} does not match the transactions at the site {clsOutImported.SiteID}.";
				Exception ex = new Exception(msg);
				//EventLog eventLog = new EventLog("Application", ".", "CloseoutProcessor.cs, Function:CompareCloseoutRecs");
				//eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
				throw (ex);
			}

			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			return bSame;
		}


		public bool CompareOwnerCloseoutRecs(OwnerCloseoutDO ownrClsOutImported, OwnerCloseoutDO ownrClsOutCalculated, double tolerance)
		{
			bool bSame = true;
			string msg =
				 $"OwnerCloseout record (Closeout date: {ownrClsOutImported.CloseoutDate?.ToString("d")}, Product: {ownrClsOutImported.ProductName}) does not match the calculated result. ";

			// OwnerCloseoutID // skip
			// Site
			if (ownrClsOutImported.SiteName != ownrClsOutCalculated.SiteName)
			{
				//bSame = false;
				//if ((ownrClsOutImported.SiteName != null) && (ownrClsOutCalculated.SiteName != null))
				//{
				//	msg +=
				//	    $"Imported.SiteName = {ownrClsOutImported.SiteName}, Calculated.SiteName = {ownrClsOutCalculated.SiteName}. ";
				//}

				return false; // We're in a loop; this is not an immediate fail
			}

			// SiteGuid SiteName comparison so do not compare the SiteGuid
			// ManagerName
			if (ownrClsOutImported.ManagerName != ownrClsOutCalculated.ManagerName)
			{
				//bSame = false;
				//if ((ownrClsOutImported.ManagerName != null) && (ownrClsOutCalculated.ManagerName != null))
				//{
				//	msg +=
				//	    $"Imported.ManagerName = {ownrClsOutImported.ManagerName}, Calculated.ManagerName = {ownrClsOutCalculated.ManagerName}. ";
				//}

				return false; // We're in a loop; this is not an immediate fail
			}

			// ManagerGuid not comparing guids 
			// ProductName
			if (ownrClsOutImported.ProductName != ownrClsOutCalculated.ProductName)
			{
				//bSame = false;
				//if ((ownrClsOutImported.ProductName != null) && (ownrClsOutCalculated.ProductName != null))
				//{
				//	msg +=
				//	    $"Imported.ProductName = {ownrClsOutImported.ProductName}, Calculated.ProductName = {ownrClsOutCalculated.ProductName}. ";
				//}

				return false; // We're in a loop; this is not an immediate fail
			}

			// ProductGuid not comparing guids 
			// CloseoutDate
			if (ownrClsOutImported.CloseoutDate != ownrClsOutCalculated.CloseoutDate)
			{
				//bSame = false;
				//if ((ownrClsOutImported.CloseoutDate != null) && (ownrClsOutCalculated.CloseoutDate != null))
				//{
				//	msg +=
				//	    $"Imported.CloseoutDate = {ownrClsOutImported.CloseoutDate.ToString()}, Calculated.CloseoutDate = {ownrClsOutCalculated.CloseoutDate.ToString()}. ";
				//}

				return false; // We're in a loop; this is not an immediate fail
			}

			// OwnerName
			if (ownrClsOutImported.OwnerName != ownrClsOutCalculated.OwnerName)
			{
				//bSame = false;
				//if ((ownrClsOutImported.OwnerName != null) && (ownrClsOutCalculated.OwnerName != null))
				//{
				//	msg +=
				//	    $"Imported.OwnerName = {ownrClsOutImported.OwnerName}, Calculated.OwnerName = {ownrClsOutCalculated.OwnerName}. ";
				//}

				return false; // We're in a loop; this is not an immediate fail
			}

			// OwnerGuid not comparing 

			// GrossBookInventory
			if (ownrClsOutImported.BookInventory.NullableGross.HasValue)
			{
				if (Math.Abs((ownrClsOutImported.BookInventory.Gross - ownrClsOutCalculated.BookInventory.Gross) / ownrClsOutCalculated.BookInventory.Gross) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.BookInventory.Gross = {ownrClsOutImported.BookInventory.Gross}, Calculated.BookInventory.Gross = {ownrClsOutCalculated.BookInventory.Gross}. ";
				}
			}

			// NetBookInventory
			if (ownrClsOutImported.BookInventory.NullableNet.HasValue)
			{
				if (Math.Abs((ownrClsOutImported.BookInventory.Net - ownrClsOutCalculated.BookInventory.Net) / ownrClsOutCalculated.BookInventory.Net) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.BookInventory.Net = {ownrClsOutImported.BookInventory.Net}, Calculated.BookInventory.Net = {ownrClsOutCalculated.BookInventory.Net}. ";
				}
			}

			// CreatedDate not comparing 

			// CreatedBy not comparing 

			// UpdatedDate not comparing 

			// UpdatedBy not comparing 

			// GrossBookPrice
			if (Math.Abs((ownrClsOutImported.BookInventory.GrossPrice - ownrClsOutCalculated.BookInventory.GrossPrice) / ownrClsOutCalculated.BookInventory.GrossPrice) > tolerance)
			{
				bSame = false;
				msg +=
					 $"Imported.BookInventory.GrossPrice = {ownrClsOutImported.BookInventory.GrossPrice}, Calculated.BookInventory.GrossPrice = {ownrClsOutCalculated.BookInventory.GrossPrice}. ";
			}

			// NetBookPrice
			if (Math.Abs((ownrClsOutImported.BookInventory.NetPrice - ownrClsOutCalculated.BookInventory.NetPrice) / ownrClsOutCalculated.BookInventory.NetPrice) > tolerance)
			{
				bSame = false;
				msg +=
					 $"Imported.BookInventory.NetPrice = {ownrClsOutImported.BookInventory.NetPrice}, Calculated.BookInventory.NetPrice = {ownrClsOutCalculated.BookInventory.NetPrice}. ";
			}

			// MassBookInventory
			if (ownrClsOutImported.BookInventory.NullableMass.HasValue)
			{
				if (Math.Abs((ownrClsOutImported.BookInventory.Mass - ownrClsOutCalculated.BookInventory.Mass) / ownrClsOutCalculated.BookInventory.Mass) > tolerance)
				{
					bSame = false;
					msg +=
						 $"Imported.BookInventory.Mass = {ownrClsOutImported.BookInventory.Mass}, Calculated.BookInventory.Mass = {ownrClsOutCalculated.BookInventory.Mass}. ";
				}

			}
			// MassBookPrice
			if (Math.Abs((ownrClsOutImported.BookInventory.MassPrice - ownrClsOutCalculated.BookInventory.MassPrice) / ownrClsOutCalculated.BookInventory.MassPrice) > tolerance)
			{
				bSame = false;
				msg +=
					 $"Imported.BookInventory.MassPrice = {ownrClsOutImported.BookInventory.MassPrice}, Calculated.BookInventory.MassPrice = {ownrClsOutCalculated.BookInventory.MassPrice}. ";
			}

			if (bSame == false)
			{
				msg +=
					 $"This indicates the Enterprise server transactions for {ownrClsOutImported.ProductName} does not match the transactions at the site {ownrClsOutImported.SiteName}.";
				Exception ex = new Exception(msg);
				//EventLog eventLog = new EventLog("Application", ".", "CloseoutProcessor.cs, Function:CompareOwnerCloseoutRecs");
				//eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
				throw (ex);
			}

			return true;
		}


		public bool Compare2ListOfOwnerCloseoutRecs(List<OwnerCloseoutDO> lstOwnerCloseoutDOImported, List<OwnerCloseoutDO> lstOwnerCloseoutDOCalculated, double tolerance)
		{
			bool bFoundSameRecInList = false;

			if ((lstOwnerCloseoutDOImported == null) && (lstOwnerCloseoutDOCalculated == null))
			{
				return true;
			}

			if ((lstOwnerCloseoutDOImported?.Count ?? 0) != (lstOwnerCloseoutDOCalculated.Count))
			{
				string msg =
					 $"Number of Owner Closeout Records do not match: Imported Owner Closeouts count = {lstOwnerCloseoutDOImported?.Count ?? 0}, Calculated Owner Closeouts count = {lstOwnerCloseoutDOCalculated.Count}. ";
				Exception ex = new Exception(msg);

				using (EventLog eventLog = new EventLog("Application", ".", "CloseoutProcessor.cs, Function:CompareCloseoutRecs"))
				{
					eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
					throw (ex);
				}
			}

			if (lstOwnerCloseoutDOImported != null)
			{
				foreach (OwnerCloseoutDO importedRec in lstOwnerCloseoutDOImported)
				{
					foreach (OwnerCloseoutDO calculatedRec in lstOwnerCloseoutDOCalculated)
					{
						bFoundSameRecInList = this.CompareOwnerCloseoutRecs(importedRec, calculatedRec, tolerance);

						if (bFoundSameRecInList)
						{
							break;
						}
					}

					if (bFoundSameRecInList == false)
					{
						return false; // 
					}
				}
			}

			return bFoundSameRecInList; // if you get here it should be true; 
		}


		protected List<OwnerCloseoutDO> CalculateAListOfAllOwnerCloseOutDOs(CloseoutSR sr)
		{
			List<OwnerCloseoutDO> lstOwnerCloseoutDOs = new List<OwnerCloseoutDO>();

			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(sr.Security, sites.GetIdentityGuid(sr.Security, sr.Site));


			ProductsClass products = new ProductsClass();
			ProductClass product = products.GetByProductAuthorizedCompanies(sr.Security, sr.ProductGuid, false);

			AccountingUnitConversion converter = new AccountingUnitConversion(site, product);


			// Retrieve all the owners prior to the start of the transaction.
			CompaniesClass companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = companies.EnumerateByRole(sr.Security, COMPANY_ROLE.OWNER, false);

			// Retrieve all the owner book inventory volumes and build a collections for
			// processing the them.
			foreach (CompanyClass company in companyCollection)
			{
				OwnerCloseoutDAO ownrClsOutDAOs = new OwnerCloseoutDAO(sr.Security)
				{
					CompanyID = company.ID,
					CompanyGuid = company.MasterRecordGuid,
					SR = sr
				};
				ownrClsOutDAOs.BookInv = ownrClsOutDAOs.GetBookInventories();

				OwnerCloseoutDO nextRec = ownrClsOutDAOs.CreateOwnerCloseoutDO();

				lstOwnerCloseoutDOs.Add(nextRec);
			}

			return lstOwnerCloseoutDOs;
		}

		private CloseoutDO CheckForSuccess(ProductsClass products, CloseoutDO closeoutDO, CloseoutSR closeoutSR)
		{
			foreach (string key in closeoutSR.AllProductsIrdoCollection.Keys)
			{
				InventoryReconciliationDO invrecdo = closeoutSR.AllProductsIrdoCollection[key];
				var lineitem = invrecdo?.LineItems[closeoutSR.InventoryDate.Day - 1] as InventoryReconciliationLineItemDO;

				if (lineitem != null)
				{
					closeoutDO.CloseoutDate = lineitem.DtInventoryDate;
					closeoutDO.BookInventory = lineitem.BookInventory;
					closeoutDO.TotalPhysicalInventory = lineitem.TotalPhysicalInventory;
					closeoutDO.TotalVariance = lineitem.TotalVariance;

					closeoutDO.ProductName = key;
					closeoutDO.ProductGuid = products.GetMasterRecordGuidFromID(this.security, key);

					closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CREATE;
					closeoutSR.InventoryDate = lineitem.DtInventoryDate;
				}

				closeoutSR.ProductName = closeoutDO.ProductName;
				closeoutSR.ProductGuid = closeoutDO.ProductGuid;

				try
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.CloseoutProductStartEvent(closeoutSR.ProductName)));
					CloseoutProcessorClass closeoutProcessor = new CloseoutProcessorClass();
					closeoutProcessor.Process(closeoutSR);
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.CloseoutProductEndEvent(closeoutSR.ProductName)));
				}
				catch (Exception exception)
				{
					closeoutDO.Closeouterrtext += "[Error in creating a closeout transaction] - " + exception.Message + " \n";
					closeoutDO.Closeouterror = true;
				}
			}

			return closeoutDO;
		}

		protected CloseoutDO DoCloseoutsForAllProductsStep1(CloseoutSR closeoutSR)
		{
			CloseoutDO ret = new CloseoutDO
			{
				AllProductsIrdoCollection = new ProductIrdoCollectionClass(),
				Nonclosedproductsirdocollection = new ProductIrdoCollectionClass(),
				Closeouterrtext = string.Empty,
				Closeouterror = false,
				Confirmtext = string.Empty
			};

			var productbrokenblend = new ArrayList();
			var productnophysical = new ArrayList();
			var productUnpostedBols = new List<string>();

			var accountingSites = new AccountingSites();
			var accountingSite = accountingSites.LoadSiteInfoNoCompanies(this.security, this.security.SiteGuid);

			var sites = new SitesClass();
			var site = sites.Get(this.security, this.security.SiteGuid, false, false, false);
			bool blockCloseIfUnpostedBols = site.BlockCloseOnUnpostedBol;

			var sr = new InventoryReconciliationSR
			{
				Security = this.security,
				Site = this.security.SiteID,
				Subrequest = InventoryReconciliationSR.RequestTypes.REFRESH,
				ManagerID = closeoutSR.ManagerName,
				Month = DateEfficacy.ConvertToMonthAndYear(closeoutSR.InventoryDate)
			};

			// Process all products for the current manager
			var allproductscollection = new ProductCollectionClass();
			TanksClass tanks = new TanksClass();
			TankCollectionClass tankCollection = tanks.Enumerate(this.security);

			// clear the all collection
			ret.AllProductsIrdoCollection.Clear();

			ProductCollectionClass managerproducts;
			ProductsClass products = new ProductsClass();
			if (tankCollection.Count > 0)
			{
				managerproducts = products.EnumerateByManagerAndTanks(this.security, sr.ManagerID);
			}
			else
			{
				managerproducts = products.Enumerate(this.security);
			}

			// iterate through all products for the manager
			foreach (ProductClass managerproduct in managerproducts)
			{
				// don't add the product of the Inhibit accounting flag is set
				if (managerproduct.InhibitAccounting)
				{
					continue;
				}

				// don't add the product if it is an additive and the enable additive accounting is false
				if ((false == accountingSite.CurrentSite.EnableAdditiveAccounting)
					 && (ProductType.AdditiveProduct == managerproduct.ProductType))
				{
					continue;
				}

				allproductscollection.Add(managerproduct);
			}

			foreach (ProductClass product in allproductscollection)
			{
				sr.ProductID = product.ID;
				sr.Tolerance = product.VarianceTolerance;
				InventoryReconciliationProcessorClass inventoryProcessor = new InventoryReconciliationProcessorClass();
				//This can take up to 15 minutes
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.InventoryReconciliationStartEvent(sr.ProductID)));
				var perproductirdo = inventoryProcessor.Process(sr);
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.InventoryReconciliationEndEvent(sr.ProductID)));

				ret.AllProductsIrdoCollection.Add(product.ID, perproductirdo);

				// get the lineitem for the day to closeout
				var lineitem =
					 perproductirdo.LineItems[closeoutSR.InventoryDate.Day - 1] as InventoryReconciliationLineItemDO;

				// set error flags 
				if (lineitem != null && lineitem.Flags.CheckFlag(BaseLineItemDO.Status.BROKEN_BLENDS))
				{
					productbrokenblend.Add(product.ID);
					ret.Closeouterror = true;
				}

				if (lineitem != null && !lineitem.Flags.CheckFlag(BaseLineItemDO.Status.PHYS_INV_EXISTS))
				{
					productnophysical.Add(product.ID);
					ret.Closeouterror = true;
				}

				if (blockCloseIfUnpostedBols)
				{
					CompaniesClass companies = new CompaniesClass();
					// find last closeout for this product
					var closeoutListSR = new CloseoutListSR
					{
						ManagerGuid = companies.GetMasterRecordGuid(this.security, sr.ManagerID),
						ProductGuid = products.GetMasterRecordGuidFromID(this.security, product.ID),
						Site = this.security.SiteID,
						Security = this.security
					};

					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.GetPreviousCloseoutStartEvent(product.ID)));
					CloseoutListProcessorClass proc = new CloseoutListProcessorClass();
					CloseoutListDO closeoutListDO = proc.Process(closeoutListSR);
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.GetPreviousCloseoutEndEvent(product.ID)));

					// Now get BOLs between last closeout and current requested closeout, for this product
					var getSR = new GetTransactionSR
					{
						Request = GetTransactionRequest.SITE_MANAGER_PRODUCT_UNPOSTED_ISSUE,
						ManagerID = sr.ManagerID,
						Product = product.ID
					};
					if (closeoutListDO.PriorCloseout != null && closeoutListDO.PriorCloseout.CloseoutRecordFound)
					{
						getSR.BeginningDate = closeoutListDO.PriorCloseout.CloseoutDate;
					}
					else
					{
						getSR.BeginningDate = new DateTime(1900, 1, 1);
					}

					getSR.EndingDate = closeoutSR.InventoryDate.Add(TimeSpan.FromSeconds(86399.0)); // 86400 seconds in a day, advance to end of specified day
					getSR.Status = ((int)TransactionStatus.Completed).ToString(CultureInfo.InvariantCulture);
					getSR.Security = this.security;

					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.GetUnpostedBolsStartEvent(product.ID)));
					GetTransactionProcessorClass getTransactionProcessor = new GetTransactionProcessorClass();
					GetTransactionDO getDO = getTransactionProcessor.Process(getSR);
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.GetUnpostedBolsEndEvent(product.ID)));

					foreach (DataRow row in getDO.TransactionDataSet.Tables[0].Rows)
					{
						try
						{
							productUnpostedBols.Add(row["aliasname"] + " " + row["documentnumber"] + " for product " + product.ID);
						}
						catch (Exception)
						{
							// In case updated stored procedure wasn't installed.
							if (!productUnpostedBols.Contains(product.ID))
							{
								productUnpostedBols.Add(product.ID);
							}
						}
						ret.Closeouterror = true;
					}
				}
			}

			DataDictionariesClass dataDictionaries = new DataDictionariesClass();
			if (ret.Closeouterror)
			{
				if (productbrokenblend.Count > 0)
				{

					ret.Closeouterrtext += dataDictionaries.Get(this.security.SiteGuid, "Products containing broken blends") + ":\n";

					foreach (string productid in productbrokenblend)
					{
						ret.Closeouterrtext += productid + "\n";
					}

					ret.Closeouterrtext += "\n";
				}

				if (productnophysical.Count > 0)
				{
					ret.Closeouterrtext += dataDictionaries.Get(this.security.SiteGuid, "Products containing no physical inventory on the closeout date")
							  + ":\n";

					foreach (string productid in productnophysical)
					{
						ret.Closeouterrtext += productid + "\n";
					}

					ret.Closeouterrtext += "\n";
				}

				if (productUnpostedBols.Count > 0)
				{
					ret.Closeouterrtext += dataDictionaries.Get(this.security.SiteGuid, "Unpostable transactions") + ":\n";
					foreach (var productid in productUnpostedBols)
					{
						ret.Closeouterrtext += productid + "\n";
					}

					ret.Closeouterrtext += "\n";
				}

			}
			else
			{
				// show variances, and provide user a confirmation dialog


				var numberFormat = new NumberFormatInfo
				{
					NumberGroupSizes = accountingSite.CurrentSite.GetNumberGroupSizes(),
					NumberDecimalSeparator =
						  accountingSite.CurrentSite.NumberDecimalSeparator,
					NumberGroupSeparator =
						  accountingSite.CurrentSite.NumberGroupSeparator
				};

				foreach (string key in ret.AllProductsIrdoCollection.Keys)
				{
					var invrecdo = ret.AllProductsIrdoCollection[key] as InventoryReconciliationDO;
					if (invrecdo != null)
					{
						var lineitem = (InventoryReconciliationLineItemDO)invrecdo.LineItems[closeoutSR.InventoryDate.Day - 1];

						// only show variance for products not closed out
						if (!lineitem.Flags.CheckFlag(BaseLineItemDO.Status.CLOSED_OUT))
						{
							ProductClass product = managerproducts.Find(x => x.ID == key);

							byte volumeDecimalPlaces;
							byte massDecimalPlaces;

							if (product.ProductType == ProductType.AdditiveProduct)
							{
								if (product.VolumeUnits == 0)
								{
									volumeDecimalPlaces = accountingSite.CurrentSite._AdditiveVolumeDecimalPlaces;
								}
								else
								{
									volumeDecimalPlaces = product.VolumeDecimalPlaces;
								}
							}
							else
							{
								if (product.VolumeUnits == 0)
								{
									volumeDecimalPlaces = accountingSite.CurrentSite._VolumeDecimalPlaces;
								}
								else
								{
									volumeDecimalPlaces = product.VolumeDecimalPlaces;
								}
							}

							if (product.MassUnits == 0)
							{
								massDecimalPlaces = accountingSite.CurrentSite._MassDecimalPlaces;
							}
							else
							{
								massDecimalPlaces = product.MassDecimalPlaces;
							}

							ret.Confirmtext += dataDictionaries.Get(this.security.SiteGuid, "Closeout");
							ret.Confirmtext += " " + accountingSite.FormatDate(closeoutSR.InventoryDate);
							ret.Confirmtext += ", " + key;
							ret.Confirmtext += ", "
												+ dataDictionaries.Get(this.security.SiteGuid, "Total Variance");

							if (product.LoadByWeight)
							{
								numberFormat.NumberDecimalDigits = massDecimalPlaces;
								string massVariance = lineitem.TotalVariance.Mass.ToString("N", numberFormat);
								if ("0" != massVariance && lineitem.TotalVariance.MassInventoryChange < 0)
								{
									massVariance = "(" + massVariance + ")";
								}

								ret.Confirmtext += "(M) = " + massVariance;
							}
							else
							{
								numberFormat.NumberDecimalDigits = volumeDecimalPlaces;
								string grossVariance = lineitem.TotalVariance.Gross.ToString("N", numberFormat);
								if ("0" != grossVariance && lineitem.TotalVariance.GrossInventoryChange < 0)
								{
									grossVariance = "(" + grossVariance + ")";
								}

								ret.Confirmtext += "(G) = " + grossVariance;
								ret.Confirmtext += ", "
													+ dataDictionaries.Get(this.security.SiteGuid, "Total Variance");

								string netVariance = lineitem.TotalVariance.Net.ToString("N", numberFormat);
								if ("0" != netVariance && lineitem.TotalVariance.NetInventoryChange < 0)
								{
									netVariance = "(" + netVariance + ")";
								}

								ret.Confirmtext += "(N) = " + netVariance;
							}

							ret.Confirmtext += "?\n";

							// add new item to non-closed collection
							ret.Nonclosedproductsirdocollection.Add(key, invrecdo);
						}
					}
				}

				if (string.IsNullOrEmpty(ret.Confirmtext))
				{
					ret.Confirmtext = "All products are closed out for the date and manager selected";
				}
			}
			return ret;
		}

		protected CloseoutDO DoCloseoutsForAllProductsStep2(CloseoutSR closeoutSR)
		{
			CompaniesClass companies = new CompaniesClass();
			var closeoutDO = new CloseoutDO
			{
				ManagerName = closeoutSR.ManagerName,
				SiteID = this.security.SiteID,
				SiteGuid = this.security.SiteGuid,
				ManagerGuid = companies.GetMasterRecordGuid(this.security, closeoutSR.ManagerName),
			};
			closeoutSR.Closeout = closeoutDO;

			ProductsClass products = new ProductsClass();
			var ret = this.CheckForSuccess(products, closeoutDO, closeoutSR);
			return ret;
		}

		protected CloseoutDO DoCloseoutsForAllProductsStep3(CloseoutSR closeoutSR)
		{
			CompaniesClass companies = new CompaniesClass();
			CloseoutDO ret = new CloseoutDO
			{
				ManagerName = closeoutSR.ManagerName,
				SiteID = this.security.SiteID,
				SiteGuid = this.security.SiteGuid,
				ManagerGuid = companies.GetMasterRecordGuid(this.security, closeoutSR.ManagerName),
				AllProductsIrdoCollection = closeoutSR.AllProductsIrdoCollection,
				Closeouterror = false,
				Closeouterrtext = string.Empty,
				Confirmtext = string.Empty,
			};

			// Send Closeout All Complete
			closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_COMPLETE;

			try
			{
				CloseoutProcessorClass closeoutProcessor = new CloseoutProcessorClass();
				closeoutProcessor.Process(closeoutSR);
			}
			catch (Exception exception)
			{
				ret.Closeouterrtext = "[Error in signaling closeout all complete] - " + exception.Message;
				ret.Closeouterror = true;
			}
			return ret;
		}

		#endregion
	}
}
