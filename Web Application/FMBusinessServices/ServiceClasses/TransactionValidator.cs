// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionValidator.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for TransactionValidator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using DataAccessLayer;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// Summary description for TransactionValidator.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TransactionValidatorClass : ITransactionValidator
	{
		#region Attributes
		/// <summary>
		/// The trans result.
		/// </summary>
		private TransactionValidationResult transResult;

		/// <summary>
		/// The trans.
		/// </summary>
		private TransactionDO trans;

		/// <summary>
		/// The site.
		/// </summary>
		private SiteClass site;

		/// <summary>
		/// The associated transaction alias GUID.
		/// </summary>
		private Guid associatedTransactionAliasGuid;

		/// <summary>
		/// The user company list.
		/// </summary>
		private List<Guid> userCompanyList;

		/// <summary>
		/// The closeout list.
		/// </summary>
		private List<CloseoutDO> fromManagerCloseoutList;

        /// <summary>
        /// The closeout list.
        /// </summary>
        private List<CloseoutDO> toManagerCloseoutList;

        /// <summary>
        /// The config.
        /// </summary>
        private GeneralConfigDO config;

		/// <summary>
		/// The validated products.
		/// </summary>
		private ProductCollectionClass validatedProducts;

		/// <summary>
		/// The security.
		/// </summary>
		private SecurityClass security;

		/// <summary>
		/// The allocation array.
		/// </summary>
		private AllocationClass[] allocationArray;
		
		/// <summary>
		/// The ship to company.
		/// </summary>
		private CompanyClass shipToCompany;

		/// <summary>
		/// The accounting site.
		/// </summary>
		private AccountingSite accountingSite;
		
		private Dictionary<string, Guid> aliasDictionary = new Dictionary<string, Guid>();
		private Dictionary<string, Guid> companyDictionary = new Dictionary<string, Guid>();
		private AllocationsClass allocations;
		private CompaniesClass companies;
		private ProductsClass products;
		private TransactionAliasesClass transactionAliases;

		#endregion Attributes

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionValidatorClass"/> class. 
		/// If this class doesn't have a default constructor, you get a 
		/// "The service type provided could not be loaded as a service because it does not have a default (parameter-less) constructor." error
		/// from the Azure Worker Role WorkerRole.cs
		/// </summary>
		public TransactionValidatorClass() :this(null)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionValidatorClass"/> class.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		public TransactionValidatorClass(SecurityClass security) : this(security, null)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionValidatorClass"/> class.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="accountingSite">
		/// The accounting site.
		/// </param>
		public TransactionValidatorClass(SecurityClass security, AccountingSite accountingSite)
		{
			this.security = security;
			this.transResult = new TransactionValidationResult( );
			this.accountingSite = accountingSite;
			this.Init( );
		}

		private void InitCompanyList()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@siteGuid, @userGuid)";
			cmd.Parameters.Add("@loginSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@siteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@userGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@loginSiteGuid"].Value = this.security.LoginSiteGuid;
			cmd.Parameters["@siteGuid"].Value = this.security.SiteGuid;
			cmd.Parameters["@userGuid"].Value = this.security.UserGuid;

			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set = consolidatedDA.GetDataSet(cmd, this.security);

			this.userCompanyList = new List<Guid>();

			if (set != null
			&& set.Tables.Count != 0
			&& set.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow row in set.Tables[0].Rows)
				{
					this.userCompanyList.Add(DataObject.getValue(row["CompanyGuid"], Guid.Empty));
				}
			}

		}

		/// <summary>
		/// This method will initialize values.
		/// </summary>
		protected void Init()
		{
			this.companies = new CompaniesClass();
			this.products = new ProductsClass();
			this.validatedProducts = new ProductCollectionClass();
			this.allocationArray = new AllocationClass[4];
			this.allocations = new AllocationsClass();
			this.transactionAliases = new TransactionAliasesClass();

			if (null != this.security)
			{
				this.InitCompanyList();
			}
		}

		/// <summary>
		/// The set transaction.
		/// </summary>
		/// <param name="inTrans">
		/// The in Trans.
		/// </param>
		public void SetTransaction(TransactionDO inTrans)
		{
			// Retrieve general configuration when Site changes
			if (this.trans != null
			&& this.trans.Site != inTrans.Site)
			{
				this.site = null;
				this.aliasDictionary.Clear();
				this.companyDictionary.Clear();
			}


			this.trans = inTrans;
			this.associatedTransactionAliasGuid = Guid.Empty;

			// We must create a new validation result because the validator may be reused when validating multiple transactions
			this.transResult = new TransactionValidationResult { TransID = this.trans.TransID, AliasName = this.trans.Alias };

			var productList = new List<Guid>();
			productList.AddRange(this.trans.LineItems.Select(lineItem => lineItem.ProductGuid));

            foreach (LineItemDO lineItem in this.trans.LineItems.Where(lineItem => lineItem.SubLineItems != null))
			{
				productList.AddRange(lineItem.SubLineItems.Select(subLineItem => subLineItem.ProductGuid));
			}

            if (this.trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade || this.trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
            {
                productList.AddRange(this.trans.LineItems.Select(lineItem => ((RegradeLineItemDO)lineItem).ToProductGuid));
            }

            productList = productList.Distinct().ToList();

            this.fromManagerCloseoutList =this.GetCloseoutDates(this.security, this.trans.Site, this.trans.SiteGuid, this.trans.ManagerCompanyGuid, productList);

			toManagerCloseoutList = new List<CloseoutDO>();
			if (TransactionTypes.T13_OwnerTransfer == this.trans.TransTypeID)
			{
				var ownertransfer = this.trans as OwnerTransferDO;

				if (ownertransfer != null && !string.IsNullOrEmpty(ownertransfer.ToManagerID)) {
                    this.toManagerCloseoutList = this.GetCloseoutDates(this.security, this.trans.Site, this.trans.SiteGuid, ownertransfer.ToManagerCompanyGuid, productList);
                }

			}
                    
            this.GetForcedCloseout(this.security, this.trans.Site, this.trans.SiteGuid);
		}

		/// <summary>
		/// The validate transaction.
		/// </summary>
		/// <param name="inTrans">
		/// The in Trans.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionValidationResult"/>.
		/// </returns>
		public TransactionValidationResult ValidateTransaction(TransactionDO inTrans)
		{
			this.SetTransaction(inTrans);
			
			bool resultFlag = this.ValidateSite(this.security, this.trans);

			if (resultFlag)
			{
				this.ValidateFieldsForApostrophe();
				this.ValidateUserIsAssociatedToCompanies( );
				
				this.ValidateTransactionAlias(this.security, this.trans);
				
				this.ValidateManager( );
				
				this.ValidateFuelCard( );

				bool validateDestinationEquipment = WillValidateDestinationEquipment();
				if (validateDestinationEquipment)
				{
					// Bryan Ponnwitz - TFS #60423
					// Destination equipment validation should not happen in Aviation.
					this.ValidateEquipment("Destination Equipment 1", this.trans.DestinationEQ1, this.trans.PermitNonReferenceData);
					this.ValidateEquipment("Destination Equipment 2", this.trans.DestinationEQ2, this.trans.PermitNonReferenceData);
					this.ValidateEquipment("Destination Equipment 3", this.trans.DestinationEQ3, this.trans.PermitNonReferenceData);
				}

				this.ValidateEquipment("Source Equipment 1", this.trans.SourceEQ1, this.trans.PermitNonReferenceData);
				this.ValidateEquipment("Source Equipment 2", this.trans.SourceEQ2, this.trans.PermitNonReferenceData);
				this.ValidateEquipment("Source Equipment 3", this.trans.SourceEQ3, this.trans.PermitNonReferenceData);

				if (TransactionTypes.T14_PhysicalInventory != this.trans.TransTypeID
					&& TransactionTypes.T12_InventoryNotAffected != this.trans.TransTypeID)
				{
					this.ValidateOwner( );
					
					if (TransactionTypes.T15_PrimaryRegrade != this.trans.TransTypeID &&
						 TransactionTypes.T16_SecondaryRegrade != this.trans.TransTypeID)
					{
						this.ValidateBillTo( );
					
						this.ValidateToBillTo( );
						
						this.ValidateShipTo( );
						
						this.ValidateToShipTo( );				
					}

					this.ValidateShipper( );
					this.ValidateCarrier();
					this.ValidateSupplier();
				}

				if (this.trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement ||
					this.trans.TransTypeID == TransactionTypes.T6_SecondaryDisbursement ||
					this.trans.TransTypeID == TransactionTypes.T25_Shipment)
				{
					this.GetTransactionAllocationsandCompany( );
				}

				foreach (LineItemDO currentLineItem in this.trans.LineItems)
				{
					if (validateDestinationEquipment)
					{
						this.ValidateEquipment("Line Item Destination", currentLineItem.DestinationEQ, this.trans.PermitNonReferenceData);
					}

					this.ValidateEquipment("Line Item Source", currentLineItem.SourceEQ, this.trans.PermitNonReferenceData);
					this.ValidateProduct(currentLineItem);

					// Check the inventory date for items that are not closedout.  If the transaction is
					// an order type, then skip the validation.
					if (this.trans.TransTypeID != TransactionTypes.T17_Order && this.trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
					{
						this.ValidateInventoryDate(currentLineItem.Product, this.trans.Site, this.trans.InventoryDate, currentLineItem.CloseoutDate, this.fromManagerCloseoutList, this.toManagerCloseoutList, this.config);
						
                        if (this.trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade || this.trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
                        {
                            this.ValidateInventoryDate( ((RegradeLineItemDO)currentLineItem).ToProduct, this.trans.Site, this.trans.InventoryDate, currentLineItem.CloseoutDate, this.fromManagerCloseoutList, this.toManagerCloseoutList, this.config);
                        }
                    }

					this.ValidateQuantities(currentLineItem.Quantity);
					
					if (this.trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement ||
						this.trans.TransTypeID == TransactionTypes.T6_SecondaryDisbursement ||
						this.trans.TransTypeID == TransactionTypes.T25_Shipment)
					{
						this.ValidateAllocations(currentLineItem);
					}

					// For Order and Supply Order that utilize AssociatedTransactionAliasGuid
					// verify there are no duplicate line items
					if ( this.associatedTransactionAliasGuid != Guid.Empty 
						&& (this.trans.TransTypeID == TransactionTypes.T17_Order || this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
						&& currentLineItem.ProductGuid != Guid.Empty)
					{
						foreach (LineItemDO lineItem in this.trans.LineItems)
						{
							if (lineItem == currentLineItem || lineItem.ProductGuid == Guid.Empty)
							{
								continue;
							}

							if (lineItem.ProductGuid == currentLineItem.ProductGuid)
							{
								this.transResult.ErrorList.Add("Duplicate Product : Product Code '" +
																currentLineItem.ProductCode + "'  Product ID  '" + currentLineItem.Product + "'.");
								return this.transResult;
							}
						}
					}

					if (currentLineItem.SubLineItems != null)
					{
						foreach (SubLineItemDO subLineItem in currentLineItem.SubLineItems)
						{
							this.ValidateProduct(subLineItem);
							this.ValidateInventoryDate(subLineItem.Product, this.trans.Site, this.trans.InventoryDate, subLineItem.CloseoutDate, this.fromManagerCloseoutList,this.toManagerCloseoutList, this.config);
							this.ValidateQuantities(subLineItem.Quantity);
						}
					}
				}

				this.ValidateWeightReadings( );
				this.ValidateRegradeTransactionNotOntoItself( );
			}

			return this.transResult;
		}

		protected void ValidateFieldsForApostrophe()
		{
			if (this.trans.Notes != null && this.trans.Notes.Contains("'"))
			{
			    this.transResult.ErrorList.Add("Transaction Notes cannot include Apostrophe.");
			}
		}

		/// <summary>
		/// Validate the provided transaction using the provided security credentials
		/// </summary>
		/// <param name="securityParam">Contains security information to be used when validating the transaction</param>
		/// <param name="inTrans">
		/// The transaction to validate
		/// </param>
		/// <returns>
		/// The <see cref="TransactionValidationResult"/>.
		/// </returns>
		public TransactionValidationResult ValidateTransaction(SecurityClass securityParam, TransactionDO inTrans)
		{
			this.security = securityParam;
			return this.ValidateTransaction(inTrans);
		}

		/// <summary>
		/// The validate weight readings.
		/// </summary>
		protected void ValidateWeightReadings()
		{
			// If there is more than one weight reading, the compartment id must be specified.
			if (this.trans.WeightReadings.Count > 1)
			{
				foreach (WeightReadingDO reading in this.trans.WeightReadings)
				{
					if (string.IsNullOrEmpty(reading.CompartmentName))
					{
						this.transResult.ErrorList.Add("Weight reading Compartment ID must be specified when using multiple weight readings.");
						break;
					}
				}
			}
		}

        /// <summary>
        /// The validate inventory date.
        /// </summary>
        /// <param name="productId">
        /// The product ID.
        /// </param>
        /// <param name="siteID">The site we're validating the inventory date for</param>
        /// <param name="inventoryDate">The inventory date to check</param>
        /// <param name="closeoutDate">
        /// The closeout date.
        /// </param>
        /// <param name="inCloseoutList">A list of closeouts for the site and manager</param>
        /// <param name="generalConfiguration">The general configuration of accounting for this site</param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public string ValidateInventoryDate(string productId, string siteID, DateTime inventoryDate, DateTime? closeoutDate, List<CloseoutDO> closeoutList, GeneralConfigDO generalConfiguration)
        {
            // Check Closeout
            if (closeoutDate == null)
            {
                foreach (CloseoutDO closeout in closeoutList)
                {
                    if (String.Compare(closeout.ProductName, productId, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Check that the Inventory Date is later than the most recent closeout date.
                        if (inventoryDate <= closeout.CloseoutDate)
                        {
                            return "Inventory Date " + inventoryDate.ToString("d") + " is closed out for manager:" + closeout.ManagerName+ "  product: " + productId +
                                   ".  The latest closeout date for this product was " + closeout.CloseoutDate.Date.ToString("d");
                        }

                        break;
                    }
                }
            }

            // Check if a Forced Closeout Setting exists for the site before attempting access the item in the collocation's.
            // This will prevent a null object exception.
            if (generalConfiguration == null)
            {
                return "No Forced Closeout Setting for Site: " + siteID;
            }

            // Check that the InventoryDate is prior to the Forced Closeout Date
            // (Most recent closeout date + configured Forced Closeout number of days)
            int forcedCloseoutDays = generalConfiguration.ForceCloseout;

            if (forcedCloseoutDays > 0)
            {
                foreach (CloseoutDO closeout in closeoutList)
                {
                    if (String.Compare(closeout.ProductName, productId, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        DateTimeOffset forcedCloseoutDate = closeout.CloseoutDate.AddDays(forcedCloseoutDays);

                        if (inventoryDate > forcedCloseoutDate)
                        {
                            return "Inventory Date " + inventoryDate.ToString("d") + " is later than the next forced closeout date.  " +
                                "The next forced closeout date is " + forcedCloseoutDate.ToString("d") + ".  This can be changed in Configuration -> General Configuration -> Force closeout after";
                        }

                        break;
                    }
                }
            }

            return string.Empty;
        }
        /// <summary>
        /// The validate inventory date.
        /// </summary>
        /// <param name="productId">
        /// The product ID.
        /// </param>
        /// <param name="siteID">The site we're validating the inventory date for</param>
        /// <param name="inventoryDate">The inventory date to check</param>
        /// <param name="closeoutDate">
        /// The closeout date.
        /// </param>
        /// <param name="inCloseoutList">A list of closeouts for the site and manager</param>
        /// <param name="generalConfiguration">The general configuration of accounting for this site</param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public string ValidateInventoryDate(string productId, string siteID, DateTime inventoryDate, DateTime? closeoutDate, List<CloseoutDO> fromManagerCloseoutList, List<CloseoutDO> toManagerCloseoutList, GeneralConfigDO generalConfiguration)
		{
			string result = ValidateInventoryDate(productId, siteID, inventoryDate, closeoutDate, fromManagerCloseoutList, generalConfiguration);
            if (!string.IsNullOrEmpty(result))
            {
                this.transResult.ErrorList.Add(result);
				return result;
            }
            result = ValidateInventoryDate(productId, siteID, inventoryDate, closeoutDate, toManagerCloseoutList, generalConfiguration);
            if (!string.IsNullOrEmpty(result))
            {
                this.transResult.ErrorList.Add(result);
            }
            return result;
        }

		/// <summary>
		/// The validate site.
		/// </summary>
		/// <param name="inSecurity">
		/// The in Security.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = false, TransactionAutoComplete = true)]
		public bool ValidateSite(SecurityClass inSecurity, TransactionDO transaction)
		{
			bool exists = false;
			string siteId = string.Empty;

			if (string.IsNullOrEmpty(transaction.Site) == false)
			{
				siteId = transaction.Site;

				if (transaction.SiteGuid != Guid.Empty )
				{
					if (this.accountingSite != null &&
						this.accountingSite.CurrentSite != null &&
						this.accountingSite.CurrentSite.SiteGuid == this.trans.SiteGuid )
					{
						this.site = this.accountingSite.CurrentSite;
					}
					else if ( this.accountingSite != null &&
							 this.accountingSite.LoginSite != null &&
							 this.accountingSite.LoginSite.SiteGuid == this.trans.SiteGuid )
					{
						this.site = this.accountingSite.LoginSite;
					}
					else
					{
						if (this.site == null)
						{
							var sites = new SitesClass();
							this.site = sites.GetBasic(inSecurity, transaction.SiteGuid);
						}
					}
					

					if (this.site != null && this.site.ID.ToUpper() == siteId.ToUpper() && (this.site.IdentityGuid == transaction.SiteGuid))
					{
						exists = true;
					}
				}
			}

			if (!exists)
			{
				this.transResult.ErrorList.Add("Site \"" + siteId + "\" is invalid.");
			}

			return exists;
		}

		/// <summary>
		/// The validate re-grade transaction not onto itself.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		protected bool ValidateRegradeTransactionNotOntoItself()
		{
			bool validated = true;

			if (this.trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade || this.trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
			{
				if (this.trans.LineItems.Count > 0)
				{
					LineItemDO lineItem = this.trans.LineItems[0];
					string strProductName = lineItem.Product;
					var regradeLnItm = (RegradeLineItemDO)lineItem;
					string strToPrdct = regradeLnItm.ToProduct;

					if (strProductName == strToPrdct)
					{
						string msg = string.Format("Cannot regrade product onto itself. Attempted to regrade {0} to {1}. ", strProductName, strToPrdct);
						this.transResult.ErrorList.Add(msg);
						validated = false;
					}
				}
			}

			return validated;
		}

		/// <summary>
		/// Validates the equipment.  Successful validation is when the ID is not specified or the Guids match.
		/// If Guid is empty, then Permit Non Reference data must be set but this is not validated
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="equipmentDO">The equipment do.</param>
		/// <param name="permitNonReferenceData"></param>
		/// <returns></returns>
		public bool ValidateEquipment(string equipment, EquipmentDO equipmentDO, bool? permitNonReferenceData)
		{
			if(equipmentDO == null || string.IsNullOrEmpty(equipmentDO.RegistrationID))
			{
				return true;
			}

			var equipments = new EquipmentsClass();
			var equipmentMasterRecordGuid = equipments.GetMasterRecordGuid(this.security, equipmentDO.RegistrationID);

			if (equipmentMasterRecordGuid == Guid.Empty)
			{
				if (permitNonReferenceData == null)
				{
					return true;
				}

				if (permitNonReferenceData.Value == false)
				{
					this.transResult.ErrorList.Add("Equipment \"" + equipmentDO.RegistrationID + "\" is invalid.");
					return false;
				}

				return true;
			}

			if (equipmentDO.EquipmentGuid != equipmentMasterRecordGuid)
			{
				this.transResult.ErrorList.Add("Equipment \"" + equipmentDO.RegistrationID + "\" is invalid.");
				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will valid to see if the fuel card ID exists in the 
		/// enterprise database.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ValidateFuelCard()
		{
			if (!string.IsNullOrEmpty(this.trans.FuelCardID) && this.trans.FuelCardGuid != Guid.Empty)
			{
				var fuelCards = new FuelCardsClass();
				var fuelCardGuid = fuelCards.GetIdentityGuid(this.security, this.trans.FuelCardID);
				if (fuelCardGuid == Guid.Empty || this.trans.FuelCardGuid != fuelCardGuid)
				{
					this.transResult.ErrorList.Add("FuelCard \"" + this.trans.FuelCardID + "\" is invalid.");
					return false;
				}

				var fuelCard = fuelCards.Get(this.security, fuelCardGuid, false);
				if (fuelCard.Status != FuelCardClass.Statuses.ACTIVE)
				{
					this.transResult.ErrorList.Add("FuelCard \"" + this.trans.FuelCardID + "\" not active.");
					return false;
				}

				var timeConverter = new SiteTimeConverter(this.site);

				if (fuelCard.ExpirationDate.HasValue && timeConverter.Now() > fuelCard.ExpirationDate.Value)
				{
					this.transResult.ErrorList.Add("FuelCard \"" + this.trans.FuelCardID + "\" is expired.");
					return false;
				}

			}

			return true;
		}

		/// <summary>
		/// This method will valid to see if the manager ID or Code exists in the 
		/// enterprise database. If they do not, then an error is returned.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ValidateManager()
		{
			bool fromManagerExists = false;
			bool toManagerExists = false;


			if (string.IsNullOrEmpty(this.trans.ManagerID) == false)
			{
				string fieldId;
				if (this.trans.ManagerCompanyGuid != Guid.Empty)
				{
					if (TransactionTypes.T13_OwnerTransfer == this.trans.TransTypeID)
					{
						fieldId = "From Manager";
					}
					else
					{
						fieldId = "Manager";
					}

					fromManagerExists = this.ValidateCompany(fieldId, this.trans.ManagerID, this.trans.ManagerCompanyGuid, false);
				}
			}

			// Added validity check for Owner Transfer transactions. 
			// This fixes CSI #3782. (IGO 17-Jan-2007)
			if (TransactionTypes.T13_OwnerTransfer == this.trans.TransTypeID)
			{
				var ownertransfer = this.trans as OwnerTransferDO;

				if (ownertransfer != null && !string.IsNullOrEmpty(ownertransfer.ToManagerID))
				{
					if (ownertransfer.ToManagerCompanyGuid != Guid.Empty)
					{
						toManagerExists = this.ValidateCompany("To Manager", ownertransfer.ToManagerID, ownertransfer.ToManagerCompanyGuid, false);
					}
				}
			}
			else
			{
				toManagerExists = true;
			}

			// Force Validate to return true. This fixes WI#25651. (TLH 2011-12-28)
			if (this.trans.TransTypeID == TransactionTypes.T19_EndOfDay ||
				this.trans.TransTypeID == TransactionTypes.T20_EndOfMonth)
			{
				fromManagerExists = true;
				toManagerExists = true;
			}

			return toManagerExists & fromManagerExists;
		}

		/// <summary>
		/// This method will valid to see if the owner ID or Code exists in the 
		/// enterprise database. If they do not, then an error is returned.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ValidateOwner()
		{
			bool fromOwnerExists = false;
			bool toOwnerExists = false;

			if (!string.IsNullOrEmpty(this.trans.OwnerID))
			{
				string fieldId;
				if (TransactionTypes.T13_OwnerTransfer == this.trans.TransTypeID)
				{
					fieldId = "From Owner";
				}
				else
				{
					fieldId = "Owner";
				}
				fromOwnerExists = this.ValidateCompany(fieldId, this.trans.OwnerID, this.trans.OwnerCompanyGuid, false);
			}


			// Added validity check for Owner Transfer transactions. 
			// This fixes CSI #3782. (IGO 17-Jan-2007)
			if (TransactionTypes.T13_OwnerTransfer == this.trans.TransTypeID)
			{
				var ownertransfer = this.trans as OwnerTransferDO;

				if (ownertransfer != null && (!string.IsNullOrEmpty(ownertransfer.ToOwnerID)))
				{
					if (ownertransfer.ToOwnerCompanyGuid != Guid.Empty)
					{
						toOwnerExists = this.ValidateCompany("To Owner", ownertransfer.ToManagerID, ownertransfer.ToManagerCompanyGuid);
					}
				}
			}
			else
			{
				toOwnerExists = true;
			}

			// Force Validate to return true. This fixes WI#25651. (TLH 2011-12-28)
			if (this.trans.TransTypeID == TransactionTypes.T19_EndOfDay ||
				this.trans.TransTypeID == TransactionTypes.T20_EndOfMonth)
			{
				fromOwnerExists = true;
				toOwnerExists = true;
			}

			return toOwnerExists & fromOwnerExists;
		}

		/// <summary>
		/// The validate product.
		/// </summary>
		/// <param name="item">
		/// The item.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ValidateProduct(object item)
		{
			bool exists = false;
			string productId = string.Empty;
			Guid productGuid = Guid.Empty;

			if (item is LineItemDO)
			{
				var lineItemDO = item as LineItemDO;

				if (string.IsNullOrEmpty(lineItemDO.Product) == false)
				{
					productId = lineItemDO.Product;

					try
					{
						ProductClass lineProduct = this.products.GetIdAndGuid(this.security, lineItemDO.ProductGuid, this.site);
						productGuid = lineProduct.MasterRecordGuid;

						foreach (ProductClass product in this.validatedProducts)
						{
							if (product.ID == lineProduct.ID && product.MasterRecordGuid == lineProduct.MasterRecordGuid)
							{
								exists = true;
								break;
							}
						}
					}
					catch (Exception)
					{
						exists = false;
					}

					if (!exists)
					{
						ProductClass product = this.products.GetByInfoAuthorizedCompanies(this.security, productGuid, true, false);

						if (product != null &&
							product.MasterRecordGuid == productGuid &&
							String.Compare(product.ID, productId, StringComparison.OrdinalIgnoreCase) == 0)
						{
							this.validatedProducts.Add(product);
							exists = true;
						}
					}
				}

				if (exists && item is RegradeLineItemDO)
				{
					productId = string.Empty;
					productGuid = Guid.Empty;
					exists = false;

					var regradeLineItemDo = item as RegradeLineItemDO;

					if (string.IsNullOrEmpty(regradeLineItemDo.ToProduct) == false)
					{
						productId = regradeLineItemDo.ToProduct;

						try
						{
							ProductClass lineProduct = this.products.GetIdAndGuid(this.security, regradeLineItemDo.ToProductGuid, this.site);
							productGuid = lineProduct.MasterRecordGuid;

							foreach (ProductClass product in this.validatedProducts)
							{
								if (product.ID == lineProduct.ID && product.MasterRecordGuid == lineProduct.MasterRecordGuid)
								{
									exists = true;
									break;
								}
							}
						}
						catch (Exception)
						{
							exists = false;
						}

						if (!exists)
						{
							ProductClass product = this.products.GetByInfoAuthorizedCompanies(this.security, productGuid, true, false);

							if (product != null &&
								product.MasterRecordGuid == productGuid &&
								String.Compare(product.ID, productId, StringComparison.OrdinalIgnoreCase) == 0)
							{
								this.validatedProducts.Add(product);
								exists = true;
							}
						}
					}
				}
			}
			else if (item is SubLineItemDO)
			{
				var subLineItemDO = item as SubLineItemDO;

				if (!string.IsNullOrEmpty(subLineItemDO.Product))
				{
					productId = subLineItemDO.Product;

					try
					{
						ProductClass lineProduct = this.products.GetIdAndGuid(this.security, subLineItemDO.ProductGuid, this.site);
						productGuid = lineProduct.MasterRecordGuid;

						foreach (ProductClass product in this.validatedProducts)
						{
							if (product.ID == lineProduct.ID && product.MasterRecordGuid == lineProduct.MasterRecordGuid)
							{
								exists = true;
								break;
							}
						}
					}
					catch (Exception)
					{
						exists = false;
					}

					if (!exists)
					{
						ProductClass product = this.products.GetByInfoAuthorizedCompanies(this.security, productGuid, true, false);

						if (product != null &&
							product.MasterRecordGuid == productGuid &&
							String.Compare(product.ID, productId, StringComparison.OrdinalIgnoreCase) == 0)
						{
							this.validatedProducts.Add(product);
							exists = true;
						}
					}
				}
			}


			if (exists == false)
			{
				this.transResult.ErrorList.Add("Product " + productId + " is invalid.");
			}

			return exists;
		}

		/// <summary>
		/// Validates the bill to.
		/// </summary>
		public void ValidateBillTo()
		{
			if (string.IsNullOrEmpty(this.trans.BillToID))
			{
				return;
			}

			if (this.trans.BillToCompanyGuid == Guid.Empty)
			{
				return;
			}

			this.ValidateCompany("BillTo", this.trans.BillToID, this.trans.BillToCompanyGuid);
		}

		/// <summary>
		/// Validates to bill to.
		/// </summary>
		public void ValidateToBillTo()
		{
			if (this.trans.TransTypeID != TransactionTypes.T11_ConsumerTransfer)
			{
				return;
			}

			var transDo = (ConsumerTransferDO)this.trans;

			if (string.IsNullOrEmpty(transDo.ToBillToID) && 
				transDo.ToBillToCompanyGuid == Guid.Empty)
			{
				return;
			}

			if (transDo.ToBillToCompanyGuid == Guid.Empty)
			{
				return;
			}

			this.ValidateCompany("ToBillTo", transDo.ToBillToID, transDo.BillToCompanyGuid);
		}


		/// <summary>
		/// Validates the ship to.
		/// </summary>
		public void ValidateShipTo()
		{
			if (string.IsNullOrEmpty(this.trans.ShipToID))
			{
				return;
			}

			if (this.trans.ShipToCompanyGuid == Guid.Empty)
			{
				return;
			}

			this.ValidateCompany("ShipTo", this.trans.ShipToID, this.trans.ShipToCompanyGuid);
		}

		/// <summary>
		/// Validates to Ship to.
		/// </summary>
		public void ValidateToShipTo()
		{
			if (this.trans.TransTypeID != TransactionTypes.T11_ConsumerTransfer)
			{
				return;
			}

			var transDo = (ConsumerTransferDO)this.trans;

			if (string.IsNullOrEmpty(transDo.ToShipToID) &&
				transDo.ToShipToCompanyGuid == Guid.Empty)
			{
				return;
			}

			if (transDo.ToShipToCompanyGuid == Guid.Empty)
			{
				return;
			}

			this.ValidateCompany("ToShipTo", transDo.ToShipToID, transDo.ToShipToCompanyGuid);
		}


		private bool ValidateCompany(string field, string id, Guid guid, bool bUseMasterGuid = true)
		{
			Guid companyGuid;

			if (!this.companyDictionary.TryGetValue(id.ToUpper(), out companyGuid))
			{
				if (bUseMasterGuid)
				{
					companyGuid = this.companies.GetMasterRecordGuid(this.security, id);
				}
				else
				{
					companyGuid = this.companies.GetIdentityGuid(this.security, id);
				}

				if (companyGuid != Guid.Empty)
				{
				    this.companyDictionary.Add(id.ToUpper(), guid);
					companyGuid = guid;
				}
			}

			if (companyGuid != guid)
			{
				this.transResult.ErrorList.Add(field + " " + id + " is invalid.");
				return false;
			}

			return true;
		}


		/// <summary>
		/// The validate user is associated to companies.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ValidateUserIsAssociatedToCompanies()
		{
			// UserGuid of Empty bypasses Associated Companies check
			if (this.security.UserGuid == Guid.Empty)
			{
				return true;
			}

			if (this.userCompanyList.Contains(this.trans.ManagerCompanyGuid))
			{
				return true;
			}

			if (this.userCompanyList.Contains(this.trans.OwnerCompanyGuid))
			{
				return true;
			}

			if (this.userCompanyList.Contains(this.trans.ShipToCompanyGuid))
			{
				return true;
			}

			if (this.userCompanyList.Contains(this.trans.BillToCompanyGuid))
			{
				return true;
			}

			if (this.userCompanyList.Contains(this.trans.SupplierCompanyGuid))
			{
				return true;
			}

			if (this.userCompanyList.Contains(this.trans.ShipperCompanyGuid))
			{
				return true;
			}

			if (this.userCompanyList.Contains(this.trans.CarrierCompanyGuid))
			{
				return true;
			}

			// Force Validate to return true. This fixes WI#25651. (TLH 2011-12-28)
			if ( this.trans.TransTypeID == TransactionTypes.T19_EndOfDay ||
				this.trans.TransTypeID == TransactionTypes.T20_EndOfMonth )
			{
				return true;
			}

			const string Msg = "User is not associated with company that is a party to the transaction.";
			this.transResult.ErrorList.Add(Msg);

			return false;
		}

		/// <summary>
		/// The validate shipper.
		/// </summary>
		public void ValidateShipper()
		{
			if (string.IsNullOrEmpty(this.trans.ShipperID))
			{
				return;
			}

			if (this.trans.ShipperCompanyGuid == Guid.Empty)
			{
				return;
			}

			this.ValidateCompany("Shipper ", this.trans.ShipperID, this.trans.ShipperCompanyGuid);

		}

		/// <summary>
		/// The validate Carrier.
		/// </summary>
		public void ValidateCarrier()
		{
			if (!string.IsNullOrEmpty(this.trans.CarrierID) && this.trans.CarrierCompanyGuid != Guid.Empty)
			{
			    this.ValidateCompany("CarrierID", this.trans.CarrierID, this.trans.CarrierCompanyGuid);
			}

		}

		/// <summary>
		/// The validate Supplier.
		/// </summary>
		public void ValidateSupplier()
		{
			if (!string.IsNullOrEmpty(this.trans.SupplierID) && this.trans.SupplierCompanyGuid != Guid.Empty)
			{
			    this.ValidateCompany("SupplierID", this.trans.SupplierID, this.trans.SupplierCompanyGuid);
			}
		}

		/// <summary>
		/// The validate transaction alias.
		/// </summary>
		/// <param name="inSecurity">
		/// The in Security.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = false, TransactionAutoComplete = true)]
		public bool ValidateTransactionAlias(SecurityClass inSecurity, TransactionDO transaction)
		{
			bool exists = false;
			string aliasId = string.Empty;

			if (!string.IsNullOrEmpty(transaction.Alias))
			{
				aliasId = transaction.Alias;

				if (transaction.TransactionAliasGuid != Guid.Empty && transaction.TransactionAliasGuid != Guid.Empty)
				{

					Guid aliasGuid;

					if (!this.aliasDictionary.TryGetValue(aliasId, out aliasGuid))
					{

						Guid originalSiteGuid = inSecurity.SiteGuid;
						inSecurity.SiteGuid = transaction.SiteGuid;

						aliasGuid = this.transactionAliases.GetMasterRecordGuid(inSecurity, aliasId);
						inSecurity.SiteGuid = originalSiteGuid;

						if (aliasGuid != Guid.Empty)
						{
						    this.aliasDictionary.Add(aliasId, aliasGuid);
						}
					}


					if (transaction.TransactionAliasGuid == aliasGuid)
					{
						if (transaction.TransTypeID == TransactionTypes.T17_Order
							|| transaction.TransTypeID == TransactionTypes.T18_SupplyOrder)
						{
							TransactionAliasClass transactionAlias = this.transactionAliases.GetWithoutAliasFields(inSecurity, aliasGuid);
							this.associatedTransactionAliasGuid = transactionAlias.AssociatedTransactionAliasGuid;
						}

						exists = true;
					}

					
				}
				else
				{
					// Force Validate to return true. This fixes WI#25651. (TLH 2011-12-28)
					if (transaction.TransTypeID == TransactionTypes.T19_EndOfDay || transaction.TransTypeID == TransactionTypes.T20_EndOfMonth)
					{
						exists = true;
					}
				}
			}

			if (!exists)
			{
				this.transResult.ErrorList.Add("Transaction Alias " + aliasId + " is invalid.");
			}

			return exists;
		}

		/// <summary>
		/// The validate quantities.
		/// </summary>
		/// <param name="quantity">
		/// The quantity.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		protected bool ValidateQuantities(QuantityDO quantity)
		{
			bool valid;

			short sign = 0;
			switch (this.trans.TransTypeID)
			{
				// These Transaction Types should always have a non-negative quantity
				// because they increase inventory.
				case TransactionTypes.T3_PrimaryDefuel:
				case TransactionTypes.T4_SecondaryDefuel:
				case TransactionTypes.T8_Receipt:

				// These Transactions Types should always have a non-negative quantity,
				// but they do not increase inventory.
				case TransactionTypes.T9_Request:	// Request is not a fuel movement.
				case TransactionTypes.T14_PhysicalInventory: // Physical Inventory can't be negative.

				case TransactionTypes.T7_FillStand: // FillStand and Unload move between
				case TransactionTypes.T10_Unload:	// primary and secondary storage.

					sign = 1;
					break;

				// These Transaction Types should always have a non-positive quantity
				// because they decrease inventory.
				case TransactionTypes.T5_PrimaryDisbursement:
				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T25_Shipment:

					sign = -1;
					break;

				// These Transaction Types may be any value.
				case TransactionTypes.T1_PrimaryAdjustment:   // Adjustments may increase or
				case TransactionTypes.T2_SecondaryAdjustment: // decrease inventory.

				case TransactionTypes.T11_ConsumerTransfer: // Transfers/Regrades are conjoined
				case TransactionTypes.T13_OwnerTransfer:	// transactions. One side will be positive,
				case TransactionTypes.T15_PrimaryRegrade:	// the other side will be negative.
				case TransactionTypes.T16_SecondaryRegrade:

				case TransactionTypes.T12_InventoryNotAffected:			// Type 12 transactions are loosely defined,
					// so they may contain any value.
					sign = 0;
					break;
			}

			// If the transaction is a Reverse, then the sign should be opposite the usual value.
			if (this.trans.ReversalType == TransactionDO.Reversal || this.trans.ReversalType == TransactionDO.ReversalWithUpdate)
			{
				sign *= -1;
			}

			double grossCheck = sign * quantity.GrossInventoryChange;
			double netCheck = sign * quantity.NetInventoryChange;
			double massCheck = sign * quantity.MassInventoryChange;

			if ((grossCheck < 0) || (netCheck < 0) || (massCheck < 0))
			{
				valid = false;
				string msg = "Net, Gross, and Mass Quantity must be ";
				msg += (sign == 1) ? "non-negative" : "non-positive";
				msg += " for Transaction Type " + TransactionAliasClass.TransactionTypeID(this.trans.TransTypeID);

				this.transResult.ErrorList.Add(msg);
			}
			else
			{
				valid = true;
			}

			return valid;
		}

		/// <summary>
		/// The get transaction allocations and company.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		protected bool GetTransactionAllocationsandCompany()
		{

			this.allocationArray[0] = null;
			this.allocationArray[1] = null;
			this.allocationArray[2] = null;
			this.allocationArray[3] = null;
			

			// Don't bother if the transaction will not affect inventory
			if (this.AllQuantitiesZero())
			{
				return true;
			}

			this.shipToCompany = null;

			if (string.IsNullOrEmpty(this.trans.ShipToID))
			{
				return false;
			}

			Guid shipToGuid = Guid.Empty;
			if (this.trans.ShipToCompanyGuid != Guid.Empty)
			{
				shipToGuid = this.trans.ShipToCompanyGuid;
			}

			// Load the shipto company
			this.shipToCompany = this.companies.Get(this.security, shipToGuid, false);

			if ( this.shipToCompany == null )
			{
				return false;
			}

			if (this.shipToCompany.DisableShipToAllocationsCheck &&
				this.shipToCompany.DisableBillToAllocationsCheck &&
				this.shipToCompany.DisableOwnerAllocationsCheck &&
				this.shipToCompany.DisableShipperAllocationsCheck)
			{
				return false;
			}

			Guid billToGuid = Guid.Empty;
			if (this.trans.BillToCompanyGuid != Guid.Empty)
			{
				billToGuid = this.trans.BillToCompanyGuid;
			}

			Guid shipperGuid = Guid.Empty;
			if (this.trans.ShipperCompanyGuid != Guid.Empty)
			{
				shipperGuid = this.trans.ShipperCompanyGuid;
			}

			Guid ownerGuid = Guid.Empty;
			if (this.trans.OwnerCompanyGuid != Guid.Empty)
			{
				ownerGuid = this.trans.OwnerCompanyGuid;
			}

			Guid managerGuid = Guid.Empty;
			if (this.trans.ManagerCompanyGuid != Guid.Empty)
			{
				managerGuid = this.trans.ManagerCompanyGuid;
			}

			Guid ownerManagerMapGuid = Guid.Empty;
			Guid shipperOwnerMapGuid = Guid.Empty;
			Guid billToShipperMapGuid = Guid.Empty;
			Guid shipToBillToMapGuid = Guid.Empty;

			// Retrieve the Company Hierarchy
			var maps = new CompanyMapsClass();

			if (managerGuid != Guid.Empty && ownerGuid != Guid.Empty)
			{
				ownerManagerMapGuid = maps.GetIdentityGuidByGuidsAndType(this.security, managerGuid, ownerGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);	
			}

			if (shipperGuid != Guid.Empty && ownerManagerMapGuid != Guid.Empty)
			{
				shipperOwnerMapGuid = maps.GetIdentityGuidByGuidsAndType(this.security, ownerManagerMapGuid, shipperGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);	
			}

			if (billToGuid != Guid.Empty && shipperOwnerMapGuid != Guid.Empty)
			{
				billToShipperMapGuid = maps.GetIdentityGuidByGuidsAndType(this.security, shipperOwnerMapGuid, billToGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
			}

			if (shipToGuid != Guid.Empty && billToShipperMapGuid != Guid.Empty)
			{
				shipToBillToMapGuid = maps.GetIdentityGuidByGuidsAndType(this.security, billToShipperMapGuid, shipToGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);	
			}
			
			var station = new StationClass { Type = STATION_TYPE.WEIGHT_SCALE };

			bool inThePast = (DateTimeOffset.Now - this.trans.InventoryDate) >= TimeSpan.FromDays(1);


			if (inThePast)
			{
				if (shipToBillToMapGuid != Guid.Empty)
				{
					Guid shipToAllocationGuid = this.allocations.GetIdentityGuid(this.security, shipToBillToMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
					if (shipToAllocationGuid != Guid.Empty)
					{
						this.allocationArray[0] = this.allocations.GetByInventoryDate(
																				this.security,
																				shipToAllocationGuid,
																				this.trans.SiteGuid,
																				station.Type,
																				this.trans.TransID,
																				this.trans.InventoryDate);
					}
				}

				if (billToShipperMapGuid != Guid.Empty)
				{
					Guid billToAllocationGuid = this.allocations.GetIdentityGuid(this.security, billToShipperMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
					if (billToAllocationGuid != Guid.Empty)
					{
						this.allocationArray[1] = this.allocations.GetByInventoryDate(
																				this.security,
																				billToAllocationGuid,
																				this.trans.SiteGuid,
																				station.Type,
																				this.trans.TransID,
																				this.trans.InventoryDate);
					}
				}

				if (shipperOwnerMapGuid != Guid.Empty)
				{
					Guid shipperAllocationGuid = this.allocations.GetIdentityGuid(this.security, shipperOwnerMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
					if (shipperAllocationGuid != Guid.Empty)
					{
						this.allocationArray[2] = this.allocations.GetByInventoryDate(
																				this.security,
																				shipperAllocationGuid,
																				this.trans.SiteGuid,
																				station.Type,
																				this.trans.TransID,
																				this.trans.InventoryDate);
					}
				}

				if (ownerManagerMapGuid != Guid.Empty)
				{
					Guid ownerAllocationGuid = this.allocations.GetIdentityGuid(this.security, ownerManagerMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
					if (ownerAllocationGuid != Guid.Empty)
					{
						this.allocationArray[3] = this.allocations.GetByInventoryDate(
																				this.security,
																				ownerAllocationGuid,
																				this.trans.SiteGuid,
																				station.Type,
																				this.trans.TransID,
																				this.trans.InventoryDate);
					}
				}
			}
			else
			{
				if (shipToBillToMapGuid != Guid.Empty)
				{
					Guid shipToAllocationGuid = this.allocations.GetIdentityGuid(this.security, shipToBillToMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
					if (shipToAllocationGuid != Guid.Empty)
					{
						this.allocationArray[0] = this.allocations.GetBySiteGuid(
																			this.security,
																			shipToAllocationGuid,
																			this.trans.SiteGuid,
																			station.Type,
																			this.trans.TransID);
					}
				}

				if (billToShipperMapGuid != Guid.Empty)
				{
					Guid billToAllocationGuid = this.allocations.GetIdentityGuid(this.security, billToShipperMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
					if (billToAllocationGuid != Guid.Empty)
					{
						this.allocationArray[1] = this.allocations.GetBySiteGuid(
																			this.security,
																			billToAllocationGuid,
																			this.trans.SiteGuid,
																			station.Type,
																			this.trans.TransID);
					}
				}

				if (shipperOwnerMapGuid != Guid.Empty)
				{
					Guid shipperAllocationGuid = this.allocations.GetIdentityGuid(this.security, shipperOwnerMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
					if (shipperAllocationGuid != Guid.Empty)
					{
						this.allocationArray[2] = this.allocations.GetBySiteGuid(
																			this.security,
																			shipperAllocationGuid,
																			this.trans.SiteGuid,
																			station.Type,
																			this.trans.TransID);
					}
				}

				if ( ownerManagerMapGuid != Guid.Empty )
				{
					Guid ownerAllocationGuid = this.allocations.GetIdentityGuid(this.security, ownerManagerMapGuid, this.trans.InventoryDate, this.trans.InventoryDate, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
					if (ownerAllocationGuid != Guid.Empty)
					{
						this.allocationArray[3] = this.allocations.GetBySiteGuid(
																			this.security,
																			ownerAllocationGuid,
																			this.trans.SiteGuid,
																			station.Type,
																			this.trans.TransID);
					}
				}
			}

			return true;
		}


		private bool AllQuantitiesZero()
		{
			foreach (LineItemDO lineItem in this.trans.LineItems)
			{
				if (lineItem.Quantity.Gross == 0.0)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// The validate allocations.
		/// </summary>
		/// <param name="currentLineItem">
		/// The current line item.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		[SecurityCritical]
		protected bool ValidateAllocations(LineItemDO currentLineItem)
		{

			// Don't bother if the transaction will not affect inventory
			if (this.AllQuantitiesZero())
			{
				return true;
			}

			if (string.IsNullOrEmpty(this.trans.ShipToID))
			{
				return false;
			}

			if ( this.shipToCompany == null )
			{
				return false;
			}

			if (this.shipToCompany.DisableShipToAllocationsCheck && 
				this.shipToCompany.DisableBillToAllocationsCheck && 
				this.shipToCompany.DisableOwnerAllocationsCheck && 
				this.shipToCompany.DisableShipperAllocationsCheck)
			{
				return false;
			}

			QuantityDO quantity = currentLineItem.Quantity;

			if ( this.shipToCompany != null )
			{
				var groups = new ProductGroupsClass();
				var logs = new AlarmAndEventLogsClass();

				// Process the first Allocation Denial, if no Allocation Denial process last Allocation Warning
				for (int allocationIndex = 0; allocationIndex < 4; allocationIndex++)
				{
					AllocationClass allocation = this.allocationArray[allocationIndex];

					if (allocationIndex == 0 && this.shipToCompany.DisableShipToAllocationsCheck)
					{
						continue;
					}

					if (allocationIndex == 1 && this.shipToCompany.DisableBillToAllocationsCheck)
					{
						continue;
					}

					if (allocationIndex == 2 && this.shipToCompany.DisableShipperAllocationsCheck)
					{
						continue;
					}

					if (allocationIndex == 3 && this.shipToCompany.DisableOwnerAllocationsCheck)
					{
						continue;
					}

					if (allocation == null)
					{
						continue;
					}

					foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
					{
						if (lineItem.Type == ALLOCATION_TYPE.PRODUCT_ALLOCATION && lineItem.AssignedGuid != currentLineItem.ProductGuid)
						{
							continue;
						}

						if (lineItem.Type == ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION)
						{
							ProductGroupClass productGroup = groups.Get(this.security, lineItem.AssignedGuid);

							if (!productGroup.IsProductInGroup(currentLineItem.ProductGuid))
							{
								continue;
							}
						}

						double totalValue = quantity.Gross + lineItem.Loaded.Value;

						if (lineItem.Limit.Value * allocation.LoadDenial / 100.0 <= totalValue)
						{
							string abbrevString = EngineeringUnits.GetUnitAbbreviation(lineItem.Limit.Units);

							string messageString = "Product: " + currentLineItem.Product + ", Site: " + this.security.SiteID + ", Company: " + this.trans.ShipToID;
							messageString = messageString + ", Amount: " + quantity.Gross + abbrevString + ", Allocation: " + lineItem.Limit.Value + abbrevString;
							messageString = messageString + ", Transacted Amount: " + totalValue + abbrevString;
							logs.Add(this.security, lineItem.AllocationDenialAlarm(allocation.ID, messageString));
							this.transResult.WarningList.Add("Issue Exceeds Allotted Allocation.");

							return true;
						}

						if (lineItem.Limit.Value * allocation.LoadWarning / 100.0 <= totalValue)
						{
							string abbrevString = EngineeringUnits.GetUnitAbbreviation(lineItem.Limit.Units);

							string messageString = "Product: " + currentLineItem.Product + ", Site: " + this.security.SiteID + ", Company: " + this.trans.ShipToID;
							messageString = messageString + ", Amount: " + quantity.Gross + abbrevString + ", Allocation: " + lineItem.Limit.Value + abbrevString;
							messageString = messageString + ", Transacted Amount: " + totalValue + abbrevString;
							logs.Add(this.security, lineItem.AllocationWarningAlarm(allocation.ID, messageString));
							this.transResult.WarningList.Add("Issue Exceeds Allotted Warning Allocation.");
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// The get closeout dates.
		/// </summary>
		public List<CloseoutDO> GetCloseoutDates(SecurityClass securityParameter, string siteID, Guid siteGuid, Guid managerGuid, List<Guid> productList)
		{
			var closeoutList = new List<CloseoutDO>();

			// Save away original security id/guid. This fixes CSI #5153. (10-Aug-2007 IGO)
			Guid originalSiteGuid = securityParameter.SiteGuid;
			string originalSiteId = securityParameter.SiteID;

			securityParameter.SiteID = siteID;
			securityParameter.SiteGuid = siteGuid;

			var sr = new CloseoutListSR
				         {
					         Security = securityParameter,
					         StartDate = null,
					         EndDate = null,
					         ConvertUnits = false,
					         GetPreviousAndSubsequentCloseouts = false,
					         Site = siteID,
                             CurrentSiteGuid = siteGuid,
                             ManagerGuid = managerGuid
				         };

			foreach (Guid productGuid in productList)
			{
                sr.ProductGuid = productGuid;            

				var closeoutListProcessor = new CloseoutListProcessorClass();
				CloseoutListDO closeoutListDO = closeoutListProcessor.Process(sr);

				if (closeoutListDO.CloseoutList.Count > 0)
				{
					var closeout = (CloseoutDO)closeoutListDO.CloseoutList[0]; // closeout list in desc order, latest is the first one on the list
					closeoutList.Add(closeout);
				}
			}

			securityParameter.SiteGuid = originalSiteGuid;
			securityParameter.SiteID = originalSiteId;

			return closeoutList;
		}

		/// <summary>
		/// The get forced closeout.
		/// </summary>
		public GeneralConfigDO GetForcedCloseout(SecurityClass securityParameter, string siteID, Guid siteGuid)
		{

			var sr = new GeneralConfigSR
						 {
							 Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION_EXCLUDE_ALIASES
						 };

			Guid originalSiteGuid = securityParameter.SiteGuid;
			string originalSiteId = securityParameter.SiteID;

			securityParameter.SiteID = siteID;
			securityParameter.SiteGuid = siteGuid;

			sr.Security = securityParameter;

			var proc = new GeneralConfigProcessorClass();
			this.config = proc.Get(sr);

			securityParameter.SiteGuid = originalSiteGuid;
			securityParameter.SiteID = originalSiteId;

			return this.config;
		}

		private bool WillValidateDestinationEquipment()
		{
			bool bWillValidateDestinationEquipment = false;


			string validateDestinationEquipment = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(this.security, ConfigurationSettingDOClass.Key_ValidateDestinationEquipment));

			if (string.IsNullOrEmpty(validateDestinationEquipment) == false && validateDestinationEquipment.ToUpper().Equals("TRUE"))
			{
				bWillValidateDestinationEquipment = true;
			}

			return bWillValidateDestinationEquipment;
		}
	}
}
