// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryReconciliationProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the InventoryReconciliationProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMBusinessServices.DataAccessLayer;
	using System.Diagnostics;

	/// <summary>
	/// The inventory reconciliation processor class.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class InventoryReconciliationProcessorClass : IInventoryReconciliationProcessor
	{
		#region Private Attributes
		private InventoryReconciliationSR inventoryRecSr;
		private InventoryReconciliationDO inventoryRecDO;
		private readonly ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="InventoryReconciliationProcessorClass"/> class.
		/// This is the default constructor for the inventory reconciliation processor class.
		/// </summary>
		public InventoryReconciliationProcessorClass ( )
		{
			this.consolidatedDa = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// This method is an override method that all derived service request classes have
		/// to implement. It is the entry point for the inventory reconciliation business
		/// logic layer.
		/// </summary>
		/// <param name="inInventoryRecSR">
		/// The inventory reconciliation service request.
		/// </param>
		/// <returns>
		/// The <see cref="InventoryReconciliationDO"/>.
		/// </returns>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public InventoryReconciliationDO Process( InventoryReconciliationSR inInventoryRecSR, AccountingSite accountingSite = null)
		{
			this.inventoryRecSr = inInventoryRecSR;
			this.inventoryRecDO = new InventoryReconciliationDO ( );

			if (this.inventoryRecSr != null)
			{
				switch (this.inventoryRecSr.Subrequest)
				{
					case InventoryReconciliationSR.RequestTypes.GET_HEADER_DATA:
							this.GetHeaderData ( );
							break;
			
					case InventoryReconciliationSR.RequestTypes.REFRESH:
						this.GetLedger (accountingSite);
						break;

					case InventoryReconciliationSR.RequestTypes.FindAdjustments:
						this.FindAdjustments();
						break;
				}
			}

			return this.inventoryRecDO;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// The get ledger.
		/// </summary>
		private void GetLedger (AccountingSite accountingSite = null)
		{

			if (accountingSite == null)
			{
				var accountingSites = new AccountingSites();
				accountingSite = accountingSites.LoadSiteInfo(this.inventoryRecSr.Security, this.inventoryRecSr.Security.SiteGuid);
			}

			var ledgerSr = new LedgerSR { Security = this.inventoryRecSr.Security };
			ledgerSr.SetRequestType ( LedgerSR.LedgerRequests.ManagerLedger );

			ledgerSr.Manager				= this.inventoryRecSr.ManagerID;
			ledgerSr.Month					= this.inventoryRecSr.Month;
			ledgerSr.Product				= this.inventoryRecSr.ProductID;
			ledgerSr.CurrentSiteGuid	= this.inventoryRecSr.Security.SiteGuid;
			ledgerSr.Site					= this.inventoryRecSr.Security.SiteID;
			ledgerSr.SiteList				= accountingSite.SiteList;
			ledgerSr.TankId				= this.inventoryRecSr.TankId;

			var proc = new LedgerProcessorClass();
			LedgerDO ledgerDO = proc.Process ( ledgerSr, accountingSite );

			var aliases = new TransactionAliasesClass ( );
			TransactionAliasCollectionClass type14AliasCollection =
							aliases.EnumerateByTransTypeID ( this.inventoryRecSr.Security, TransactionTypes.T14_PhysicalInventory );

			foreach (LedgerLineItemDO ledgerLineItem in ledgerDO.LedgerLineItems)
			{
				var inventoryReconLineItem = new InventoryReconciliationLineItemDO
					                 {
						                 InventoryDate			= ledgerLineItem.InventoryDate,
						                 BeginInventory			= ledgerLineItem.BeginInventory,
						                 BookInventory			= ledgerLineItem.BookInventory,
						                 Variance				= ledgerLineItem.Variance,
						                 TotalVariance			= ledgerLineItem.TotalVariance,
						                 TotalPhysicalInventory = ledgerLineItem.TotalPhysicalInventory,
						                 TotalActivity			= ledgerLineItem.TotalActivity,
						                 TotalMovement			= ledgerLineItem.TotalMovement,
						                 Tolerance				= ledgerLineItem.Tolerance,
						                 AllowableGainLoss		= ledgerLineItem.AllowableGainLoss,
						                 VariancePercentage		= ledgerLineItem.VariancePercentage,
						                 Flags					= ledgerLineItem.Flags
					                 };

				foreach (string key in ledgerLineItem.GetCellFlags ( ).Keys)
				{
					inventoryReconLineItem.GetCellFlags ( ).Add ( key, ledgerLineItem.GetCellFlags ( key ) );
				}

				bool physicalInventoryExists = false;
				foreach (string key in ledgerLineItem.QuantityList.Keys)
				{
					QuantityDO aliasVolume = ledgerLineItem.QuantityList[key];
					inventoryReconLineItem.QuantityList.Add ( key, aliasVolume );

					foreach (TransactionAliasClass alias in type14AliasCollection)
					{
						if (string.Compare(alias.ID, key, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							if (ledgerLineItem.CheckFlag ( key, BaseLineItemDO.Status.NA ) == false)
							{
								physicalInventoryExists = true;
							}

							break;
						}
					}
				}

				// Set flags
				inventoryReconLineItem.Flags = ledgerLineItem.Flags;

				if (this.inventoryRecSr.Tolerance != null)
				{
					double variancePercentGross = 0.0;
					double variancePercentNet = 0.0;

					if (inventoryReconLineItem.TotalMovement.GrossInventoryChange != 0)
					{
						variancePercentGross = 100 * Math.Abs(inventoryReconLineItem.TotalVariance.GrossInventoryChange /
															inventoryReconLineItem.TotalMovement.GrossInventoryChange);
					}

					if (inventoryReconLineItem.TotalMovement.NetInventoryChange != 0)
					{
						variancePercentNet = 100 * Math.Abs(inventoryReconLineItem.TotalVariance.NetInventoryChange /
														inventoryReconLineItem.TotalMovement.NetInventoryChange);
					}

					if (physicalInventoryExists)
					{
						if (variancePercentGross > Math.Abs(this.inventoryRecSr.Tolerance.Value))
						{
							inventoryReconLineItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_GROSS;
						}

						if (variancePercentNet > Math.Abs(this.inventoryRecSr.Tolerance.Value))
						{
							inventoryReconLineItem.Flags |= BaseLineItemDO.Status.OUT_OF_TOLERANCE_NET;
						}
					}
				}

				if (physicalInventoryExists)
				{
					inventoryReconLineItem.Flags |= BaseLineItemDO.Status.PHYS_INV_EXISTS;
				}
				else
				{
					inventoryReconLineItem.SetCellFlag ( "Variance", BaseLineItemDO.Status.NA );
				}

				this.inventoryRecDO.LineItems.Add ( inventoryReconLineItem );
			}

			// Supress the total line for physical inventory aliases.
			var totalLine = this.inventoryRecDO.LineItems[this.inventoryRecDO.LineItems.Count - 1] as InventoryReconciliationLineItemDO;

			foreach (TransactionAliasClass alias in type14AliasCollection)
			{
				if (totalLine != null)
				{
					QuantityDO volume = totalLine.QuantityList[alias.ID];

					if (volume != null)
					{
						totalLine.SetCellFlag ( alias.ID, BaseLineItemDO.Status.SUPPRESS );
					}
				}
			}

			if (totalLine != null)
			{
				totalLine.SetCellFlag ( "Variance", BaseLineItemDO.Status.SUPPRESS );
			}
		}

		/// <summary>
		/// This method will retrieve the Manager, Product and Month lists to be
		/// displayed on the inventory reconciliation page.
		/// </summary>
		private void GetHeaderData()
		{
			// Get the list of companies that are associated to the site and the user.
			var companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = companies.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(
				this.inventoryRecSr.Security,
				new COMPANY_ROLE [] {COMPANY_ROLE.MANAGER});

			var managerList = new ArrayList ( );

			foreach (CompanyClass company in companyCollection)
			{
				managerList.Add ( company.ID );
			}

			// Get the list of products that are associated to the site and the user.
			var sites = new SitesClass ( );
			SiteClass site = sites.GetByMemberAndProcessVariables(this.inventoryRecSr.Security, this.inventoryRecSr.Security.SiteGuid, false, false);

			var products = new ProductsClass ( );
			ProductCollectionClass productCollection = products.Enumerate ( this.inventoryRecSr.Security,false,site );

			var productList = new ArrayList ( );

			foreach (ProductClass product in productCollection)
			{
				if (product.InhibitAccounting)
				{
					continue;
				}

				if (( site.EnableAdditiveAccounting == false ) && ( product.ProductType == ProductType.AdditiveProduct ))
				{
					continue;
				}

				if (( product.ProductType == ProductType.AdditizedProduct ) ||
					( product.ProductType == ProductType.BlendProduct ))
				{
					continue;
				}

				productList.Add ( product.ID );
			}

			this.inventoryRecDO.ManagerList = managerList;
			this.inventoryRecDO.ProductList = productList;

			var monthYearSr	= new MonthYearSR { Security = this.inventoryRecSr.Security };

			var proc = new MonthYearProcessor();
			MonthYearDO monthYearDO = proc.Process ( monthYearSr );

			this.inventoryRecDO.MonthList = monthYearDO.MonthList;
			this.inventoryRecDO.YearList	 = monthYearDO.YearList;

			if (site.UseTankReconciliation)
			{
				var tankListSr	= new TankListSR
					              	  {
						              	  Security = this.inventoryRecSr.Security,
						              	  ProductId = this.inventoryRecSr.ProductID
					              	  };

				var tankListProcessor = new TankListProcessorClass ( );
				TankListDO tankListDO = tankListProcessor.Process ( tankListSr );
				this.inventoryRecDO.TankList = tankListDO.TankList;
			}
			else
			{
				this.inventoryRecDO.TankList = new ArrayList { "{All}" };
			}
		}

		/// <summary>
		/// The find adjustments.
		/// </summary>
		private void FindAdjustments()
		{
			const string Select = "SELECT TOP(1) t.AliasName ";
			const string From   = "FROM tblTransactions t LEFT OUTER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid ";
			const string Where  = "WHERE t.InventoryDate = @InventoryDate " 
			                      + "AND t.ManagerID = @ManagerID " 
			                      + "AND t.SiteGuid = @SiteGuid "
			                      + "AND l.Product = @ProductID "
								  + "AND (t.LookupTransTypeIndex = @TransTypeID1 OR t.LookupTransTypeIndex = @TransTypeID2) ";

			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandText = Select + From + Where;

				var parm = new SqlParameter("@InventoryDate", SqlDbType.Date) { Value = this.inventoryRecSr.InventoryDate.HasValue ? this.inventoryRecSr.InventoryDate.Value.Date : DateTime.Today };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@ManagerID", SqlDbType.NVarChar, 50) { Value = this.inventoryRecSr.ManagerID };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@ProductID", SqlDbType.NVarChar, 50) { Value = this.inventoryRecSr.ProductID };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.inventoryRecSr.Security.SiteGuid };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@TransTypeID1", SqlDbType.Int) { Value = (int)TransactionTypes.T1_PrimaryAdjustment };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@TransTypeID2", SqlDbType.Int) { Value = (int)TransactionTypes.T2_SecondaryAdjustment };
				sqlCommand.Parameters.Add(parm);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, this.inventoryRecSr.Security);

				// Set has adjustments to false (no adjustments);
				this.inventoryRecDO.HasAdjustments = false;

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					// Set has adjustments to true;
					this.inventoryRecDO.HasAdjustments = true;
				}
			}
		}
		#endregion
	}
}