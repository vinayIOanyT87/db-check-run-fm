/// <summary>
///   File name:	FinanceProcessor.cs
///   Purpose:	   To decipher the request to retrieve the finance
///				   data object.
///				   
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///   Date:			By:						Reason:
///   ----------  --------------------	----------------------------------------------------------
///   2009-03-03  Richard Panachida    Updated the standing offer and AUP methods to be the only
///                                    location for calculating these values. Defect 1696.
///   2009-06-19  A. Coker             Fixed Defect 3970. When searching for a standing offer
///                                    without a supplier preference, consider delivery location
///                                    when provided.
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

using FMBusinessObjects.LogClient;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class FinanceProcessorClass : IFinanceProcessor
	{
		#region Attributes
		private Logger logger;
		private SecurityClass security;
		private ConsolidatedDAClass consolidatedDA;

		private const string MSG001 = "Could not retrieve average unit price. ";
		private const string MSG002 = "Could not retrieve price list prices. ";
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the finance processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public FinanceProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.logger = new Logger("FinanceProcessorClass");
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method overrides the base class process. It will process the finance
		/// request and return a finance data object with the data.
		/// </summary>
		/// <param name="accountingSR"></param>
		/// <returns></returns>
		public FinanceDO Process(FinanceSR financeSR)
		{
			FinanceSR sr = financeSR;
			this.security = sr.Security;

			switch (sr.SubRequest)
			{
				case FinanceSR.SUB_REQUEST.AVERAGE_UNIT_PRICE:
					return this.GetAverageUnitPrice(sr);

				case FinanceSR.SUB_REQUEST.STANDING_OFFER_PRICE:
					return this.GetStandingOfferPrice(sr);
			}

			return null;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will return the finance data object containing the average unit price.
		/// </summary>
		/// <param name="sr"></param>
		/// <returns></returns>
		private FinanceDO GetAverageUnitPrice(FinanceSR sr)
		{
			FinanceDO financeDO = new FinanceDO();

			ProductGroupsClass productGroups = new ProductGroupsClass();
			ProductGroupCollectionClass collection = productGroups.Enumerate(sr.Security);

			string productGroupName = "";
			int numberOfMonthsToAverage = 0;
			DateTimeOffset inventoryDate = sr.StartDate;

			DateTimeOffset endDate = new DateTimeOffset(inventoryDate.Year, inventoryDate.Month, 1, 0, 0, 0, TimeSpan.Zero);
			endDate = endDate.AddDays(-1);
			DateTimeOffset startDate = endDate;

			//Use default number of days to average
			foreach (ProductGroupClass productGroupIter in collection)
			{
				ProductGroupClass productGroupCurrent = productGroups.Get(sr.Security, productGroupIter.IdentityGuid);
				if (productGroupCurrent == null)
				{
					continue;
				}

				if (productGroupCurrent.IsProductInGroup(sr.ProductGuid))
				{
					productGroupName = productGroupCurrent.ID;
					break;
				}
			}

			switch (productGroupName.ToUpper())
			{
				case "AVIATION":
					numberOfMonthsToAverage = 2;
					break;
				case "MARINE":
					numberOfMonthsToAverage = 11;
					break;
				case "GROUND":
					numberOfMonthsToAverage = 1;
					break;
				default:
					numberOfMonthsToAverage = -1;
					break;
			}

			startDate = endDate.AddMonths(-1 * numberOfMonthsToAverage);

			if (numberOfMonthsToAverage != -1)
			{
				try
				{
					DataSet dataSet = null;

					using (SqlCommand cmd = new SqlCommand())
					{
						financeDO.GetAverateUnitPriceExecuteSQL(cmd, sr, startDate, endDate);

						dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);
					}
					financeDO.loadAverateUnitPrice(dataSet);
				}
				catch (Exception ex)
				{
					throw new Exception(FinanceProcessorClass.MSG001 + ex.Message);
				}
			}
			else
			{
				financeDO.AverageGrossUnitPrice = 0.0;
				financeDO.ContainsAverageUnitPrice = false;
			}

			return financeDO;
		}

		/// <summary>
		/// This method will return the finance data object containing the current and most
		/// recent price list (aka standing offer) prices.
		/// </summary>
		/// <param name="sr"></param>
		/// <returns></returns>
		private FinanceDO GetStandingOfferPrice(FinanceSR sr)
		{
			FinanceDO financeDO = new FinanceDO();

			if (sr != null)
			{
				TransactionTypes type = sr.TransactionType;
				DateTimeOffset inventoryDate = sr.StartDate;
				Guid productGuid = sr.ProductGuid;
				Guid supplierGuid = sr.SupplierCompanyGuid;
				SecurityClass security = sr.Security;
				double? quantity = sr.Quantity;

				if (type != TransactionTypes.T14_PhysicalInventory)
				{
					try
					{
						bool mostRecent = false;
						StandingOffersClass standingOffers = new StandingOffersClass();
						Guid deliveryLocationGuid = this.GetDeliveryLocationGuid(sr.DeliveryLocation);

						// Retrieve price list entry (aka standing offer) based on supplier, product, delivery location,
						// date, and quantity.
						Guid standingOfferGuid = standingOffers.GetIdentityGuidUsingMostRecent(this.security,
																							supplierGuid,
																							productGuid,
																							deliveryLocationGuid,
																							inventoryDate,
																							quantity,
																							mostRecent);

						// Retrieve price list entry (aka standing offer) getting the most recent based on the
						// effective date.
						if (standingOfferGuid == Guid.Empty)
						{
							mostRecent = true;
							standingOfferGuid = standingOffers.GetIdentityGuidUsingMostRecent(this.security,
																							supplierGuid,
																							productGuid,
																							deliveryLocationGuid,
																							inventoryDate,
																							quantity,
																							mostRecent);
						}

						if (standingOfferGuid != Guid.Empty)
						{
							StandingOfferClass offer = standingOffers.Get(this.security, standingOfferGuid);

							if (offer != null)
							{
								financeDO.ContainsCurrentStandingOfferPrice = true;
								financeDO.CurrentStandingOfferPrice = offer.StandingOfferPrice;
								financeDO.StandingOfferID = offer.StandingOfferID;

								if (mostRecent == true)
								{
									financeDO.ClearInfoMessage();
									financeDO.InfoMessage = "No Price List Entry meets the criteria – an earlier Price List Entry has been chosen.";
								}
							}
							else
							{
								financeDO.ContainsCurrentStandingOfferPrice = false;
								financeDO.CurrentStandingOfferPrice = 0.0;

								financeDO.ClearInfoMessage();
								financeDO.InfoMessage = "No Price List Entries meet this criteria - please enter the fuel price.";
							}
						}
						else
						{
							financeDO.ContainsCurrentStandingOfferPrice = false;
							financeDO.CurrentStandingOfferPrice = 0.0;

							financeDO.ClearInfoMessage();
							financeDO.InfoMessage = "No Price List Entries meet this criteria - please enter the fuel price.";
						}
					}
					catch (Exception ex)
					{
						throw new Exception(FinanceProcessorClass.MSG002 + ex.Message);
					}
				}
			}

			return financeDO;
		}

		/// <summary>
		/// This method will retrieve the delivery location Guid for a given line item
		/// delivery location ID. It will return guid.empty if not found.
		/// </summary>
		/// <param name="deliveryLocation"></param>
		/// <returns></returns>
		private Guid GetDeliveryLocationGuid(string deliveryLocation)
		{
			Guid deliveryLocationGuid = Guid.Empty;

			if ((deliveryLocation != null) && (deliveryLocation.Length > 0))
			{
				IATACodesClass iataCodes = new IATACodesClass();
				deliveryLocationGuid = iataCodes.GetIdentityGuid(this.security, deliveryLocation);
			}

			return deliveryLocationGuid;
		}
		#endregion
	}
}
