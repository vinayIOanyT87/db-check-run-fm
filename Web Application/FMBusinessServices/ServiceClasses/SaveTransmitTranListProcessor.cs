// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveTransmitTranListProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation fo the ISaveTransmitTranListProcessor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Implementation of the ISaveTransmitTranListProcessor service interface.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SaveTransmitTranListProcessor : ISaveTransmitTranListProcessor
	{
		#region Fields

		private readonly SortedList appStringList;

		private readonly SortedList companyList;

		private readonly SortedList equipmentList;

		private readonly SortedList gateList;

		private readonly SortedList personList;

		private readonly SortedList productList;

		private readonly SortedList siteList;

		// Eric Simmons - 11/14/2007 @ 8:00 PM
		// Added to populate price 
		private readonly SortedList standingOfferParamList;

		private readonly SortedList stationList;

		private readonly SortedList tankList;

		private readonly SortedList transAliasList;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SaveTransmitTranListProcessor"/> class.
		/// </summary>
		public SaveTransmitTranListProcessor()
		{
			this.productList = new SortedList();
			this.equipmentList = new SortedList();
			this.companyList = new SortedList();
			this.transAliasList = new SortedList();
			this.siteList = new SortedList();
			this.gateList = new SortedList();
			this.stationList = new SortedList();
			this.tankList = new SortedList();
			this.personList = new SortedList();
			this.appStringList = new SortedList();

			// Eric Simmons - 11/14/2007 @ 8:00 PM
			// Added to populate price 
			this.standingOfferParamList = new SortedList();
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Processes the specified accounting service request.
		/// </summary>
		/// <param name="sr">The accounting SR.</param>
		/// <returns>An object containing the results of the import.</returns>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public SaveTransmitTranListResultDO Process( SaveTransmitTranListSR sr )
		{
			try
			{
				this.GetSites(sr.Security);
				this.GetProducts(sr.Security);
				this.GetCompanies(sr.Security);
				this.GetLocations(sr.Security);
				this.GetTanks(sr.Security);
				this.GetPersonnel(sr.Security);
				this.GetEquipment(sr.Security);
				this.GetTranAliases(sr.Security);
				this.GetApplicationString(sr.Security);
				this.PopulateTranHeadersWithIndexes(sr.Transactions);
				this.PopulateTranLineItemsWithIndexesAndPrices(sr.Transactions, sr.Security);
				this.PopulateTranSubLineItemsWithIndexes(sr.Transactions);

				SaveTransmitTranListResultDO result = this.SaveTransactions(sr.Transactions, sr.Security);

				return result;
			}
			catch ( IdentityGuidNotFoundException ex )
			{
				// 10-21-2007
				// Eric Simmons - Added IndexNotFoundException to show that reference data does not map.
				// Code before this was return a null SaveTransmitTransListResultDO object.  These
				// catch blocks ensure that a valid SaveTransmitTransListResultDO object is always returned.
				var result = new SaveTransmitTranListResultDO
					             {
						             ErrorMessage = ex.Message,
						             Status = SaveTransmitTranListResultDO.StatusEnum.FAIL
					             };

				return result;
			}
			catch (Exception ex)
			{
				var result = new SaveTransmitTranListResultDO
					             {
						             ErrorMessage =
							             ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source,
						             Status = SaveTransmitTranListResultDO.StatusEnum.FAIL
					             };

				return result;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the application string.
		/// </summary>
		/// <param name="security">The security object.</param>
		private void GetApplicationString(SecurityClass security)
		{
			var appstringbll = new ApplicationStringsClass();
			ApplicationStringCollectionClass appstrings = appstringbll.Enumerate(security);
			foreach (ApplicationStringClass appstring in appstrings)
			{
				if (this.appStringList.ContainsKey(appstring.ID) == false)
				{
					this.appStringList.Add(appstring.ID, appstring);
				}
			}
		}

		/// <summary>
		/// Initializes the company list.
		/// </summary>
		/// <param name="security">The security object.</param>
		private void GetCompanies(SecurityClass security)
		{
			var companybll = new CompaniesClass();
			CompanyCollectionClass companies = companybll.Enumerate(security);
			foreach (CompanyClass company in companies)
			{
				if (!this.companyList.ContainsKey(company.ID))
				{
					this.companyList.Add(company.ID, company);
				}
			}
		}

		/// <summary>
		/// Initializes the equipment list.
		/// </summary>
		/// <param name="security">The security.</param>
		private void GetEquipment(SecurityClass security)
		{
			var equipmentbll = new EquipmentsClass();
			EquipmentCollectionClass equipments = equipmentbll.Enumerate(security);
			foreach (EquipmentClass equipment in equipments)
			{
				if (!this.equipmentList.ContainsKey(equipment.ID))
				{
					this.equipmentList.Add(equipment.ID, equipment);
				}
			}
		}

		/// <summary>
		/// Initializes the locations list.
		/// </summary>
		/// <param name="security">The security object.</param>
		private void GetLocations(SecurityClass security)
		{
			var gatesbll = new GatesClass();
			var stationsbll = new StationsClass();
			GateCollectionClass gates = gatesbll.Enumerate(security);
			StationCollectionClass stations = stationsbll.Enumerate(security);

			foreach (GateClass gate in gates)
			{
				if (!this.gateList.ContainsKey(gate.ID))
				{
					this.gateList.Add(gate.ID, gate);
				}
			}

			foreach (StationClass station in stations)
			{
				if (!this.stationList.ContainsKey(station.ID))
				{
					this.stationList.Add(station.ID, station);
				}
			}
		}

		/// <summary>
		/// Initializes the personnel list.
		/// </summary>
		/// <param name="security">The security object.</param>
		private void GetPersonnel(SecurityClass security)
		{
			var personbll = new PersonnelClass();
			PersonCollectionClass persons = personbll.Enumerate(security);

			foreach (PersonClass person in persons)
			{
				if (this.personList.ContainsKey(person.ID) == false)
				{
					this.personList.Add(person.ID, person);
				}
			}
		}

		/// <summary>
		/// Gets the product price.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="standingOffer">The standing offer to use for calculations.</param>
		/// <returns>The price of the product.</returns>
		private double GetProductPrice(SecurityClass security, StandingOfferParameterClass standingOffer)
		{
			if (standingOffer == null)
			{
				// Eric Simmons
				// 11-15-2007
				// Updated to put null if no price is found
				// as per T. Archer's request on 11/15/2007 @ 12:10 PM
				return double.MaxValue;
			}

			var bll = new StandingOffersClass();

			// Eric Simmons 11/15/2007 @ 2:15 AM
			// Determine what to return if no standing offer can be found.
			double returnval;
			StandingOfferClass standingoffer;

			var standingOfferGuid = bll.GetIdentityGuidUsingPeriod(
				security, standingOffer.SupplierGuid, standingOffer.ProductGuid, standingOffer.InventoryDate);

			if (standingOfferGuid == Guid.Empty)
			{
				standingOfferGuid = bll.GetIdentityGuidUsingPeriod(security, Guid.Empty, standingOffer.ProductGuid, standingOffer.InventoryDate);
				standingoffer = bll.Get( security, standingOfferGuid );
			}
			else
			{
				standingoffer = bll.Get(security, standingOfferGuid);
			}

			if (standingoffer != null)
			{
				returnval = standingoffer.StandingOfferPrice;
			}
			else
			{
				// Eric Simmons
				// 11-15-2007
				// Updated to put null if no price is found
				// as per T. Archer's request on 11/15/2007 @ 12:10 PM
				returnval = double.MaxValue;
			}

			return returnval;
		}

		/// <summary>
		/// Initializes the products list.
		/// </summary>
		/// <param name="security">The security object.</param>
		private void GetProducts(SecurityClass security)
		{
			var productbll = new ProductsClass();
			ProductCollectionClass products = productbll.Enumerate(security);

			foreach (ProductClass product in products)
			{
				if (this.productList.ContainsKey(product.ID) == false)
				{
					this.productList.Add(product.ID, product);
				}
			}
		}

		/// <summary>
		/// Initializes the sites list.
		/// </summary>
		/// <param name="security">The security object.</param>
		private void GetSites(SecurityClass security)
		{
			var sitebll = new SitesClass();
			SiteCollectionClass sites = sitebll.Enumerate(security);
			foreach (SiteClass site in sites)
			{
				if (!this.siteList.ContainsKey(site.ID))
				{
					this.siteList.Add(site.ID, site);
				}
			}
		}

		/// <summary>
		/// Initializes the tanks list.
		/// </summary>
		/// <param name="security">The security.</param>
		private void GetTanks(SecurityClass security)
		{
			var tankbll = new TanksClass();
			TankCollectionClass tanks = tankbll.Enumerate(security);
			foreach (TankClass tank in tanks)
			{
				if (!this.tankList.ContainsKey(tank.ID))
				{
					this.tankList.Add(tank.ID, tank);
				}
			}
		}

		/// <summary>
		/// Gets the tran aliases list.
		/// </summary>
		/// <param name="security">The security object.</param>
		private void GetTranAliases(SecurityClass security)
		{
			var transaliasesbll = new TransactionAliasesClass();
			TransactionAliasCollectionClass aliases = transaliasesbll.Enumerate(security);
			foreach (TransactionAliasClass alias in aliases)
			{
				if (!this.transAliasList.ContainsKey(alias.ID))
				{
					this.transAliasList.Add(alias.ID, alias);
				}
			}
		}

		/// <summary>
		/// Populates the tran headers with indexes.
		/// </summary>
		/// <param name="dataobject">The dataobject.</param>
		private void PopulateTranHeadersWithIndexes(TransmitTranListDO dataobject)
		{
			foreach (DataRow row in dataobject.Headers.Tables[0].Rows)
			{
				// Eric Simmons - 11/14/2007
				// Added to populate price 
				StandingOfferParameterClass param;
				if (!this.standingOfferParamList.Contains(row["TransID"].ToString()))
				{
					param = new StandingOfferParameterClass
						        {
							        InventoryDate = (DateTime)row["InventoryDate"],
							        TransID = row["TransID"].ToString()
						        };

					this.standingOfferParamList.Add(row["TransID"].ToString(), param);
				}
				else
				{
					param = (StandingOfferParameterClass)this.standingOfferParamList[row["TransID"].ToString()];
				}

				string idFieldName = "AliasName";
				string guidFieldName = "TransactionAliasGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.transAliasList.ContainsKey(row[idFieldName].ToString()))
					{
						var alias = (TransactionAliasClass)this.transAliasList[row[idFieldName].ToString()];
						row[guidFieldName] = alias.IdentityGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "BillToID";
				guidFieldName = "BillToGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.companyList.ContainsKey(row[idFieldName].ToString()))
					{
						var company = (CompanyClass)this.companyList[row[idFieldName].ToString()];
						row[guidFieldName] = company.IdentityGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "CarrierID";
				guidFieldName = "CarrierGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.companyList.ContainsKey(row[idFieldName].ToString()))
					{
						var company = (CompanyClass)this.companyList[row[idFieldName].ToString()];
						row[guidFieldName] = company.IdentityGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "ManagerID";
				guidFieldName = "ManagerGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.companyList.ContainsKey(row[idFieldName].ToString()))
					{
						var company = (CompanyClass)this.companyList[row[idFieldName].ToString()];
						row[guidFieldName] = company.IdentityGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "OwnerID";
				guidFieldName = "OwnerGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.companyList.ContainsKey(row[idFieldName].ToString()))
					{
						var company = (CompanyClass)this.companyList[row[idFieldName].ToString()];
						row[guidFieldName] = company.IdentityGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "ShipperID";
				guidFieldName = "ShipperGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.companyList.ContainsKey(row[idFieldName].ToString()))
					{
						var company = (CompanyClass)this.companyList[row[idFieldName].ToString()];
						row[guidFieldName] = company.IdentityGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "ShipToID";
				guidFieldName = "ShipToGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.companyList.ContainsKey(row[idFieldName].ToString()))
					{
						var company = (CompanyClass)this.companyList[row[idFieldName].ToString()];
						row[guidFieldName] = company.IdentityGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "Site";
				guidFieldName = "SiteGuid";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.siteList.ContainsKey(row[idFieldName].ToString()))
					{
						var site = (SiteClass)this.siteList[row[idFieldName].ToString()];
						row[guidFieldName] = site.SiteGuid;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}

				idFieldName = "SupplierID";
				guidFieldName = "SupplierIndex";
				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.companyList.ContainsKey(row[idFieldName].ToString()))
					{
						var company = (CompanyClass)this.companyList[row[idFieldName].ToString()];
						row[guidFieldName] = company.IdentityGuid;

						// Eric Simmons - 11/14/2007
						// Added to populate price 
						param.SupplierGuid = company.IdentityGuid;
						param.SupplierID = company.ID;
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[guidFieldName] = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Populates the tran line items with indexes and prices.
		/// </summary>
		/// <param name="dataobject">The dataobject.</param>
		/// <param name="security">The security.</param>
		private void PopulateTranLineItemsWithIndexesAndPrices(TransmitTranListDO dataobject, SecurityClass security)
		{
			foreach (DataRow row in dataobject.LineItems.Tables[0].Rows)
			{
				// Eric Simmons - 11/14/2007
				// Added to populate price 
				StandingOfferParameterClass param = (StandingOfferParameterClass)this.standingOfferParamList[row["TransID"].ToString()];

				string idFieldName = "Product";
				const string IndexFieldName = "ProductIndex";

				if (row[idFieldName] != null && row[idFieldName] != DBNull.Value && row[idFieldName].ToString() != string.Empty)
				{
					if (this.productList.ContainsKey(row[idFieldName].ToString()))
					{
						var product = (ProductClass)this.productList[row[idFieldName].ToString()];
						row[IndexFieldName] = product.IdentityGuid;
						if (param != null)
						{
							// Eric Simmons - 11/14/2007
							// Added to populate price 
							param.ProductGuid = product.IdentityGuid;
							param.ProductID = product.ID;
						}
					}
					else
					{
						throw new IdentityGuidNotFoundException(
							idFieldName + " (" + row[idFieldName] + ") for Line Item associated with Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[IndexFieldName] = DBNull.Value;
				}

				idFieldName = "ProductPrice";
				if (row.Table.Columns.Contains(idFieldName))
				{
					// Eric Simmons
					// 11-15-2007
					// Updated to put null if no price is found
					// as per T. Archer's request on 11/15/2007 @ 12:10 PM
					double productprice = this.GetProductPrice(security, param);
					
					// ReSharper disable CompareOfFloatsByEqualityOperator
					if (productprice == double.MaxValue)
					{
						row[idFieldName] = DBNull.Value;
					}
					else
					{
						row[idFieldName] = productprice;
					}
					// ReSharper restore CompareOfFloatsByEqualityOperator
				}
			}
		}

		/// <summary>
		/// Populates the tran sub line items with indexes.
		/// </summary>
		/// <param name="dataobject">The dataobject.</param>
		private void PopulateTranSubLineItemsWithIndexes(TransmitTranListDO dataobject)
		{
			const string IDFieldName = "Product";
			const string IndexFieldName = "ProductGuid";

			foreach ( DataRow row in dataobject.SubLineItems.Tables[0].Rows )
			{
				if (row[IDFieldName] != null && row[IDFieldName] != DBNull.Value && row[IDFieldName].ToString() != string.Empty)
				{
					if (this.productList.ContainsKey(row[IDFieldName].ToString()))
					{
						var product = (ProductClass)this.productList[row[IDFieldName].ToString()];
						row[IndexFieldName] = product.IdentityGuid;
					}
					else
					{
						throw new IndexNotFoundException(
							IDFieldName + " (" + row[IDFieldName] + ") for Sub Line Item associated with Transaction ID [" + row["TransID"]
							+ "] was not found in target system.");
					}
				}
				else
				{
					row[IndexFieldName] = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Saves the transactions.
		/// </summary>
		/// <param name="dataObject">The dataobject.</param>
		/// <param name="security">The security.</param>
		/// <returns>An object describing the results of the save.</returns>
		private SaveTransmitTranListResultDO SaveTransactions(TransmitTranListDO dataObject, SecurityClass security)
		{
			var result = new SaveTransmitTranListResultDO();

			try
			{
				var processor = new SaveTransactionsProcessor();
				result = processor.SaveTransmittedTransactions( dataObject, security );
			}
			catch ( IdentityGuidNotFoundException ex )
			{
				result.Status = SaveTransmitTranListResultDO.StatusEnum.FAIL;
				result.ErrorMessage = "The transmission failed because " + "the reference data in the target "
				                      + "system does not match the reference " + "data in the source system.  See Explantion: "
				                      + ex.Message;
			}
			catch (Exception ex)
			{
				result.Status = SaveTransmitTranListResultDO.StatusEnum.FAIL;
				result.ErrorMessage = ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source;
			}

			return result;
		}

		#endregion
	}
}