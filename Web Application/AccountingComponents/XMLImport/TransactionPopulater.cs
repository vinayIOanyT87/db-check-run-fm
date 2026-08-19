// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionPopulater.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Reads transactionParam data from XML and populates a transactionParam record with the data read
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XMLImport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.XPath;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;
    using FMWebAPIBusinessLogic.Services.FMProxy;
    using FMWebAPIBusinessLogic.Interfaces.FMProxy;
    using FMDepedencyManager;
    using Unity;

    public abstract class TransactionPopulater : TransactionPopulaterBase
	{
        private readonly IMetersProxy _metersProxy;

        public TransactionPopulater() : base()
        {
            this._metersProxy = FMServiceLocator.Container.Resolve<IMetersProxy>();
        }
        #region Public methods

        /// <summary>
        /// This method starts the process of populating a transactionParam.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="transactionParam"></param>
        /// <param name="navigator"></param>
        /// <returns></returns>
        public TransactionValidationResult PopulateTransaction(SecurityClass security, TransactionDO transactionParam, XPathNavigator navigator)
		{
			this.transactionNavigator = navigator;
			this.transaction = transactionParam;
			this.transactionValidationResult = new TransactionValidationResult();

			try
			{
				if (this.PopulateCommon(security))
				{
					this.Populate();

                    if (this.transaction.LineItems.Count > 0)
                    {
                        // It was requested that we populate the header equipment data, but it seems that aviation stores equipment data only on the line item.
                        // So, copy the line item data to the header, even if it means potentially overwriting what was provided
                        this.transaction.DestinationEQ1 = this.transaction.LineItems[0].DestinationEQ;
                    }

                    // Make sure that the inventory date isn't before a closeout date. 
                    // This step is skipped for order transactions
                    if (this.transaction.TransTypeID != TransactionTypes.T17_Order
						&& this.transaction.TransTypeID != TransactionTypes.T18_SupplyOrder)
					{
						this.CheckCloseoutDates();
					}

					// Make sure that any conjoin or reversed transIds referenced actually exist in Cirrus
					// The save transactions processor does not do this at the moment.
					this.CheckAndSetTransactionIDReferences();

                    //Make sure the meter exists and is configured for this site
                    if (this.transaction.TransTypeID != TransactionTypes.T1_PrimaryAdjustment
                        && this.transaction.TransTypeID != TransactionTypes.T14_PhysicalInventory
                        && this.transaction.TransTypeID != TransactionTypes.T8_Receipt)
                    {
                        this.ValidateMeter(security);
                    }
				}
			}
			catch (Exception e)
			{
				this.transactionValidationResult.ErrorList.Add(e.Message);
			}
			finally
			{
				this.transactionNavigator = null;
				this.transaction = null;
			}

			return this.transactionValidationResult;
		}
		#endregion

		#region Abstract Methods

		abstract protected void PopulateLineItem();
		protected abstract void Populate();
		abstract protected TransactionTypes TransactionTypeID { get; }
		#endregion Abstract Methods

		#region Virtual protected methods
		virtual protected void PopulatePaymentInfo()
		{
			XPathNavigator navigator = this.transactionNavigator.SelectSingleNode("PaymentInfo");

			if (navigator == null)
			{
				return;
			}

		    this.transaction.PaymentInfo = new PaymentInfoDO();

			XPathNavigator cashNavigator = navigator.SelectSingleNode("Cash");

			if (cashNavigator != null)
			{
			    this.transaction.PaymentInfo.CashAmount = this.GetNullableDouble("Amount", false, cashNavigator);
			    this.transaction.PaymentInfo.CashCurrencyType = this.GetStringValue("CurrencyType", false, cashNavigator);
			}

			XPathNavigator creditCardNavigator = navigator.SelectSingleNode("CreditCard");

			if (creditCardNavigator != null)
			{
			    this.transaction.PaymentInfo.CreditCardAmount = this.GetNullableDouble("Amount", false, creditCardNavigator);
			    this.transaction.PaymentInfo.CreditCardCurrencyType = this.GetStringValue("CurrencyType", false, creditCardNavigator);
			    this.transaction.PaymentInfo.CreditCardName = this.GetStringValue("CardName", false, creditCardNavigator);
			    this.transaction.PaymentInfo.CreditCardNumber = this.GetStringValue("CardNumber", false, creditCardNavigator);
			    this.transaction.PaymentInfo.CreditCardType = this.GetStringValue("CardType", false, creditCardNavigator);

				string expiration = this.GetStringValue("Expiration", false, creditCardNavigator);

				if (!string.IsNullOrEmpty(expiration))
				{
					DateTimeOffset creditCardExpiration;
					string[] formats = { "MM/yy", "M/yy", "M/yyyy", "MM/yyyy" };

					if (DateTimeOffset.TryParse(expiration, out creditCardExpiration))
					{
					    this.transaction.PaymentInfo.CreditCardExpiration = creditCardExpiration;
					}
					else if (DateTimeOffset.TryParseExact(
						expiration, formats, null, DateTimeStyles.AllowWhiteSpaces, out creditCardExpiration))
					{
						// Set the day to the last day of the month by adding a month, then subtract a day.
						creditCardExpiration = creditCardExpiration.AddMonths(1);
						creditCardExpiration = creditCardExpiration.AddDays(-1);

					    this.transaction.PaymentInfo.CreditCardExpiration = creditCardExpiration;
					}
					else
					{
						this.transactionValidationResult.ErrorList.Add("Expiration = " + expiration + " : Invalid Credit Card Expiration");
					}
				}
			}
		}

		#endregion

		#region Protected methods
		/// <summary>
		/// This method populates the transactionParam header common fields.
		/// </summary>
		/// <param name="security">
		/// Contains security information
		/// </param>
		/// <returns>
		/// True to indicate that processing should continue. False to indicate a critical validation error
		/// </returns>
		protected bool PopulateCommon(SecurityClass security)
		{
			this.SetTransID();
			this.transactionValidationResult.TransID = this.transaction.TransID;
		    this.transaction.TransTypeID = this.TransactionTypeID;

			// Validate Site and TransAlias early because other data such as User Data depends on it.
			this.SetSite();
			this.SetAlias();

			// If the site or alias aren't validated, don't bother processing any further.
			// Other information depends on the site (nearly everything) and alias (user data)
			if (this.transactionValidationResult.ErrorList.Count > 0)
			{
				return false;
			}

			this.SetOwner(this.transaction.TransTypeID);
			this.SetManager(this.transaction.TransTypeID);
			this.SetCarrier(this.transaction.TransTypeID);
			this.SetBillTo();
			this.SetShipper();

            this.SetSourceEquipment();
			this.SetDestinationEquipment();

			this.SetOperatorID();

			this.SetSubType();
			this.SetInventoryDate();
			this.SetTransactionDateTime();
			this.SetTicketSource();
			this.SetTicketMode();
			this.SetDocumentNumber();
			this.SetLinkedDocumentNumber();
			this.SetReversalType();
			this.SetReversedTransID();
			this.SetTransRefID();
			this.SetNotes();
			this.SetPONumber();
			this.SetDriverIDNumber();
			this.SetTimeIn();
			this.SetTimeOut();
			this.SetTimeEnd();
			this.SetLoadID();
			this.SetTransactionStatus();
			this.SetDeleteFlag();

			this.EnsureReversalTypeIsSetCorrectly();

			this.PopulateTransactionUserData();

			this.PopulateLineItems();

			return true;
		}

		/// <summary>
		/// This method will set the Reversed Transaction Type to null if the transactionParam reversed ID is null
		/// or is empty. The reason is that if the transactionParam is not an original (meaning it had not been reversed)
		/// and the type is set to "O", then it will be read-only and it should not.  Therefore, if there is not
		/// a reversed transactionParam ID, the type must be set to null.
		/// </summary>
		private void EnsureReversalTypeIsSetCorrectly()
		{
			if (string.IsNullOrEmpty(this.transaction.ReversedTransID) && this.transaction.ReversalType == TransactionDO.Original)
			{
				this.transaction.ReversalType = TransactionDO.None;
			}
		}

		/// <summary>
		/// This method will start the process of populating the line items.
		/// </summary>
		protected void PopulateLineItems()
		{
			const string LineItemsPath = "/descendant::LineItems/LineItem";
			XPathNodeIterator nodeList = this.transactionNavigator.Select(LineItemsPath);

			if (nodeList.Count == 0)
			{
				this.transactionValidationResult.ErrorList.Add("Transaction does not contain any LineItem elements.");
			}

			foreach (XPathNavigator navigator in nodeList)
			{
				this.lineItemNavigator = navigator;

				this.lineItem = new LineItemDO();

			    this.transaction.LineItems.Add(this.lineItem);

				this.PopulateLineItemCommon();
				this.PopulateLineItem();

                this.lineItem = null;
				this.lineItemNavigator = null;
			}
		}

		/// <summary>
		/// This method will populate the common fields on the line item.
		/// </summary>
		protected void PopulateLineItemCommon()
		{
			this.SetLineItemSequenceNumber();
			this.SetLineItemProduct();
            this.SetLineItemSourceEquipment();
			this.SetLineItemDestinationEquipment();
			this.SetLineItemLocation();

			this.PopulateLineItemAccountingData();

			this.SetLineItemContractNumber();
			this.SetLineItemCLIN();
			this.SetLineItemArmNumber();
			this.SetLineItemLineNumber();
			this.SetLineItemOperatorID();
			this.SetLineItemPit();

			this.SetLineItemRequestedDateTime();
			this.SetLineItemDispatchedDateTime();
			this.SetLineItemAcknowledgedDateTime();
			this.SetLineItemOnLocationTime();
			this.SetLineItemValidationDateTime();
			this.SetLineItemCompletionDateTime();

			this.SetLineItemReceiptVariance();
			this.SetLineItemDifferentialPressure();
			this.SetLineItemLoadRackVariance();
			this.SetLineItemRequestedBy();

            //For Aviation imports, FromEquipment/Equipment1/RegistrationID is the meter ID.
            this.SetSourceMeter();

            this.lineItem.MeterReading = new MeterReadingDO();
			this.SetMeterReadings(this.lineItem.MeterReading, this.lineItemNavigator);

			this.SetLineItemDocumentNumber();
			this.SetLineItemStatus();
			this.SetLineItemPresetAmount();
			this.SetLineItemTankID();
			this.lineItem.StorageLocationTankGuid = this.importProcessor.GetTankGuid(this.transaction.Site, this.lineItem.StorageLocationID);
			this.SetLineItemAdditiveProfile();
			this.SetExpandedFsrValues();
			this.PopulateLineItemUserData();
		}

		protected void PopulateSubLineItems()
		{
			const string SubLineItemsPath = "/descendant::SubLineItems/SubLineItem";
			XPathNodeIterator nodeList = this.lineItemNavigator.Select(SubLineItemsPath);

			foreach (XPathNavigator navigator in nodeList)
			{
				this.sublineItemNavigator = navigator;
				this.subLineItem = new SubLineItemDO();
				this.lineItem.SubLineItems.Add(this.subLineItem);

				this.PopulateSubLineItem();

				this.subLineItem = null;
				this.sublineItemNavigator = null;
			}
		}

		protected void PopulateLineItemAccountingData()
		{
			XPathNavigator accountingInfoNavigator = this.lineItemNavigator.SelectSingleNode("AccountingData");

			if (accountingInfoNavigator != null)
			{
			    this.lineItem.Quantity.GrossInventoryChange = this.GetDoubleSIValue("Quantity/Gross", "Quantity/Units", true, accountingInfoNavigator);

				double? v = this.GetNullableDoubleSIValue("Quantity/Net", "Quantity/Units", false, accountingInfoNavigator);
			    this.lineItem.Quantity.NetInventoryChange = v != null ? v.Value : 0;

			    this.lineItem.Quantity.NetManualValueFlag = this.GetNullableBool("Quantity/NetManualValueFlag", false, accountingInfoNavigator);

			    this.lineItem.VCF = this.GetNullableDouble("VCF", false, accountingInfoNavigator);
			    this.lineItem.Temperature = this.GetNullableDoubleSIValue("Temperature", "TemperatureUnits", false, accountingInfoNavigator);
			    this.lineItem.Density = this.GetNullableDoubleSIValue("Density", "DensityUnits", false, accountingInfoNavigator);
			    this.lineItem.Customs = this.GetStringValue("Customs", false, accountingInfoNavigator);
			}
			else
			{
				this.transactionValidationResult.ErrorList.Add("LineItem AccountingData is required");
			}
		}

		protected void PopulateSubLineItemAccountingData()
		{
			XPathNavigator accountingInfoNavigator = this.sublineItemNavigator.SelectSingleNode("AccountingData");

			if (accountingInfoNavigator != null)
			{
			    this.subLineItem.Quantity.GrossInventoryChange = this.GetDoubleSIValue("Quantity/Gross", "Quantity/Units", true, accountingInfoNavigator);

				double? v = this.GetNullableDoubleSIValue("Quantity/Net", "Quantity/Units", false, accountingInfoNavigator);

			    this.subLineItem.Quantity.NetInventoryChange = v != null ? v.Value : 0;

			    this.subLineItem.VCF = this.GetNullableDouble("VCF", false, accountingInfoNavigator);
			    this.subLineItem.Temperature = this.GetNullableDoubleSIValue("Temperature", "TemperatureUnits", false, accountingInfoNavigator);
			    this.subLineItem.Density = this.GetNullableDoubleSIValue("Density", "DensityUnits", false, accountingInfoNavigator);
			    this.subLineItem.Customs = this.GetStringValue("Customs", false, accountingInfoNavigator);
			}
			else
			{
				this.transactionValidationResult.ErrorList.Add("SubLineItem AccountingData is required");
			}
		}

		protected void PopulateTransactionUserData()
		{
			XPathNodeIterator nodeList = this.transactionNavigator.Select("/descendant::UserFields/UserField");

			foreach (XPathNavigator navigator in nodeList)
			{
				string fieldName = this.GetStringValue("Name", true, navigator); 
				string fieldValue = this.GetStringValue("Value", false, navigator);
				this.SetTransactionUserData(this.transaction, fieldName, fieldValue);
			}
		}


		protected void PopulateLineItemUserData()
		{
			string fieldName;
			string fieldValue;
			string path = "UserFields/*";
			XPathNavigator userFieldsRoot = this.lineItemNavigator.SelectSingleNode(path);
			if (userFieldsRoot == null)
			{
				return;
			}

			do
			{
				XPathNavigator navigatorValue = userFieldsRoot.SelectSingleNode("child::text()");

				if (navigatorValue != null)
				{
					fieldValue = navigatorValue.Value;
					fieldName = userFieldsRoot.LocalName;
					this.SetLineItemUserData(this.transaction, this.lineItem, fieldName, fieldValue);
				}
			}
			while (userFieldsRoot.MoveToNext());

		}

		protected void PopulateRouteInfo()
		{
			RouteInfoDO routeInfo = new RouteInfoDO();
		    this.transaction.RouteInfo = routeInfo;

			XPathNavigator navigator = this.transactionNavigator.SelectSingleNode("RouteData");

			if (navigator != null)
			{
				routeInfo.RoutingID = this.GetStringValue("RouteID", false, navigator);
				routeInfo.RouteOriginationDate = this.GetNullableDateTime("RouteOriginationDate", false, navigator);
				routeInfo.InternationalRouteIndicator = this.GetBoolValue("InternationalRoute", false, navigator);
				routeInfo.PreviousRoutingID = this.GetStringValue("PreviousRoutingID", false, navigator);
				routeInfo.FinalStationIATAID = this.GetStringValue("FinalStation", false, navigator);
				routeInfo.PreviousStationIATAID = this.GetStringValue("PreviousStation", false, navigator);
				routeInfo.NextStationIATAID = this.GetStringValue("NextStation", false, navigator);
				routeInfo.OriginStationIATAID = this.GetStringValue("OriginStation", false, navigator);
			}
		}

		protected void PopulateRouteSchedule()
		{
			RouteScheduleDO schedule = new RouteScheduleDO();
		    this.transaction.RouteSchedule = schedule;

			XPathNavigator navigator = this.transactionNavigator.SelectSingleNode("RouteSchedule");

			if (navigator != null)
			{

				schedule.ETA = this.GetNullableDateTime("ETA", false, navigator);
				schedule.ETD = this.GetNullableDateTime("ETD", false, navigator);
				schedule.FST = this.GetNullableDateTime("FST", false, navigator);
				schedule.SFT = this.GetNullableDateTime("SFT", false, navigator);
				schedule.STA = this.GetNullableDateTime("STA", false, navigator);
				schedule.STD = this.GetNullableDateTime("STD", false, navigator);
			}
		}

		protected void SetDestinationEquipment()
		{
		    this.transaction.DestinationEQ1 = new EquipmentDO();
		    this.transaction.DestinationEQ2 = new EquipmentDO();
		    this.transaction.DestinationEQ3 = new EquipmentDO();

			XPathNavigator toEquipmentNavigator = this.transactionNavigator.SelectSingleNode("LineItems/LineItem/ToEquipment");

			if (toEquipmentNavigator != null)
			{
				XPathNavigator destination1Navigator = toEquipmentNavigator.SelectSingleNode("Equipment1");
				if (destination1Navigator != null)
				{
					this.SetEquipment(this.transaction.DestinationEQ1, destination1Navigator, false);
				}

				XPathNavigator destination2Navigator = toEquipmentNavigator.SelectSingleNode("Equipment2");
				if (destination2Navigator != null)
				{
					this.SetEquipment(this.transaction.DestinationEQ2, destination2Navigator, false);
				}

				XPathNavigator destination3Navigator = toEquipmentNavigator.SelectSingleNode("Equipment3");
				if (destination3Navigator != null)
				{
					this.SetEquipment(this.transaction.DestinationEQ3, destination3Navigator, false);
				}
			}
		}

		protected void SetSourceEquipment()
		{
		    this.transaction.SourceEQ1 = new EquipmentDO();
		    this.transaction.SourceEQ2 = new EquipmentDO();
		    this.transaction.SourceEQ3 = new EquipmentDO();

			XPathNavigator fromEquipmentNavigator = this.transactionNavigator.SelectSingleNode("LineItems/LineItem/FromEquipment");

			if (fromEquipmentNavigator != null)
			{
				XPathNavigator source1Navigator = fromEquipmentNavigator.SelectSingleNode("Equipment1");
				if (source1Navigator != null)
				{
					this.SetEquipment(this.transaction.SourceEQ1, source1Navigator, false);
				}

				XPathNavigator source2Navigator = fromEquipmentNavigator.SelectSingleNode("Equipment2");
				if (source2Navigator != null)
				{
					this.SetEquipment(this.transaction.SourceEQ2, source2Navigator, false);
				}

				XPathNavigator source3Navigator = fromEquipmentNavigator.SelectSingleNode("Equipment3");
				if (source3Navigator != null)
				{
					this.SetEquipment(this.transaction.SourceEQ3, source3Navigator, false);
				}
			}
		}

		protected void SetLineItemDestinationEquipment()
		{
		    this.lineItem.DestinationEQ = new EquipmentDO();

			XPathNavigator destinationEquipmentNavigator = this.lineItemNavigator.SelectSingleNode("ToEquipment/Equipment1");
			if (destinationEquipmentNavigator != null)
			{
				this.SetEquipment(this.lineItem.DestinationEQ, destinationEquipmentNavigator, false);
			    this.lineItem.DestinationCompartmentID = this.GetStringValue("CompartmentID", false, destinationEquipmentNavigator);
			}
		}

		protected void SetLineItemSourceEquipment()
		{
		    this.lineItem.SourceEQ = new EquipmentDO();

			XPathNavigator sourceEquipmentNavigator = this.lineItemNavigator.SelectSingleNode("FromEquipment/Equipment1");

			if (sourceEquipmentNavigator != null)
			{
				this.SetEquipment(this.lineItem.SourceEQ, sourceEquipmentNavigator, false);
			    this.lineItem.SourceCompartmentID = this.GetStringValue("CompartmentID", false, sourceEquipmentNavigator);
			}
		}

        protected void SetSourceMeter()
        {
            XPathNavigator fromEquipmentNavigator = this.transactionNavigator.SelectSingleNode("LineItems/LineItem/FromEquipment");

            if (fromEquipmentNavigator != null)
            {
                XPathNavigator source1Navigator = fromEquipmentNavigator.SelectSingleNode("Equipment1");
                if (source1Navigator != null)
                {
                    this.SetMeter(this.transaction.LineItems[0], source1Navigator, false);
                }

                XPathNavigator source2Navigator = fromEquipmentNavigator.SelectSingleNode("Equipment2");
                if (source2Navigator != null)
                {
                    this.SetMeter(this.transaction.LineItems[1], source2Navigator, false);
                }

                XPathNavigator source3Navigator = fromEquipmentNavigator.SelectSingleNode("Equipment3");
                if (source3Navigator != null)
                {
                    this.SetMeter(this.transaction.LineItems[2], source3Navigator, false);
                }
            }
        }

        protected void SetMeter(LineItemDO lineItem, XPathNavigator navigator, bool isRequired)
        {
            lineItem.MeterID = this.GetStringValue("RegistrationID", false, navigator);
        }

        protected void SetMeterReadings(MeterReadingDO meterReading, XPathNavigator navigator)
		{
			XPathNavigator meterNavigator = navigator.SelectSingleNode("MeterReadings");

			if (meterNavigator != null)
			{
				meterReading.MeterFactor = this.GetNullableDouble("MeterFactor", false, meterNavigator);
				meterReading.MeterStart = this.GetNullableDouble("MeterStart", false, meterNavigator);
				meterReading.MeterStop = this.GetNullableDouble("MeterStop", false, meterNavigator);
				meterReading.StartDateTime = this.GetNullableDateTime("StartDateTime", false, meterNavigator);
				meterReading.StopDateTime = this.GetNullableDateTime("StopDateTime", false, meterNavigator);
			}
		}

		protected void SetEquipment(EquipmentDO equipment, XPathNavigator navigator, bool isRequired)
		{
			equipment.RegistrationID = this.GetStringValue("RegistrationID", false, navigator);
			equipment.EquipmentGuid = this.importProcessor.GetEquipmentGuid(this.transaction.Site, equipment.RegistrationID);

			if (isRequired && !string.IsNullOrEmpty(equipment.RegistrationID) && equipment.EquipmentGuid == Guid.Empty)
			{
				this.transactionValidationResult.ErrorList.Add("Invalid equipment ID " + equipment.RegistrationID);
			}

			equipment.SerialNumber = this.GetStringValue("SerialNumber", false, navigator);
			equipment.EquipmentType = this.GetStringValue("EquipmentType", false, navigator);
			equipment.EquipmentModel = this.GetStringValue("EquipmentModel", false, navigator);
		}

		/// <summary>
		/// This method will start the process of populating the sub line items.
		/// </summary>
		protected void PopulateSubLineItem()
		{
			this.SetSubLineItemProduct();
			this.PopulateSubLineItemAccountingData();
			this.SetSubLineItemStatus();
			this.SetSubLineItemArmNumber();
			this.SetSubLineItemLineNumber();
			this.SetSubLineItemBatchNumber();
			this.SetSubLineItemLineFill();
			this.SetSubLineItemBottomVolume();
			this.SetSubLineItemNetCapacity();
			this.SetSubLineItemTankStatus();
			this.SetSubLineItemTankID();
			this.subLineItem.StorageLocationTankGuid = this.importProcessor.GetTankGuid(this.transaction.Site, this.subLineItem.StorageLocationID);

			this.SetSubLineItemMeterID();

			this.subLineItem.MeterReading = new MeterReadingDO();
			this.SetMeterReadings(this.subLineItem.MeterReading, this.sublineItemNavigator);

			this.SetSubLineItemDifferentialPressure();
			this.SetSubLineItemDosageRate();
			this.SetSubLineItemPresetAmount();
		}

		/// <summary>
		/// This method will populate the transactionParam user data fields.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		protected void SetTransactionUserData(TransactionDO transaction, string fieldName, string fieldValue)
		{
			string exceptionMsg = "";
			try
			{

				// If the name is empty, don't bother trying to add it to the transactionParam record
				if (string.IsNullOrEmpty(fieldName))
				{
					return;
				}

				// Is the name one of the default names like "UserData1" or "UserData24"?
				// If so, just go ahead and add it to the transactionParam, because we already know which 
				// user data field it belongs in
				if (this.importProcessor.IsDefaultUserDataFieldName(fieldName))
				{
					transaction.UserData.Add(fieldName, fieldValue);
					return;
				}

				// Perhaps the name is the display name of the user data field. We need to determine
				// Which user data field it corresponds to
				Dictionary<string, string> userFields = this.importProcessor.GetTransactionAliasUserDataFields(transaction.Site, transaction.TransactionAliasGuid);
				string userDataFieldDatabaseName;

				// @@@@@@@  ! TryGetValue
				if (userFields != null && !userFields.TryGetValue(fieldName, out userDataFieldDatabaseName))
				{
					transaction.UserData.Add(userDataFieldDatabaseName, fieldValue);
					return;
				}
			}
			catch (Exception exp)
			{
				exceptionMsg = exp.Message;
			}

			string msg = string.Format(
					"For Transaction {0}, Could not find User Data field \"{1}\" for Site \"{2}\" and Transaction Alias \"{3}\". Error={4}"
				, this.transaction.TransID, fieldName, transaction.Site, transaction.Alias, exceptionMsg);
			this.transactionValidationResult.ErrorList.Add(msg);
			Debug.WriteLine(msg + " " + exceptionMsg);
		}

		protected void SetLineItemUserData(TransactionDO transaction, LineItemDO trxlineItem, string fieldName, string fieldValue)
		{
			// If the name is empty, don't bother trying to add it to the lineItem record
			if (string.IsNullOrEmpty(fieldName))
			{
				return;
			}

			try
			{
				// The names are "UserData1" .. "UserData24"
				if (this.importProcessor.IsDefaultUserDataFieldName(fieldName))
				{
					trxlineItem.UserData.Add(fieldName, fieldValue);
					return;
				}
			}
			catch (Exception exp)
			{
				this.transactionValidationResult.ErrorList.Add(
				    $"For Transaction {this.transaction.TransID}, Could not find LineItem User Data field \"{fieldName}\" for Site \"{transaction.Site}\" and Transaction Alias \"{transaction.Alias}\". Error={exp.Message}");
				throw;
			}


		}

		/// <summary>
		/// This method will populate the transactionParam weight readings.
		/// </summary>
		protected void PopulateWeightReadings()
		{
			const string WeightReadingsPath = "/descendant::AviationGaugeReadings/Compartment";
			XPathNodeIterator nodeList = this.transactionNavigator.Select(WeightReadingsPath);

		    this.transaction.WeightReadings = new List<WeightReadingDO>();

			EngineeringUnit units = this.GetEnumValue("/descendant::AviationGaugeReadings/QuantityUnits", default(EngineeringUnit), false, this.transactionNavigator);

			foreach (XPathNavigator navigator in nodeList)
			{
				WeightReadingDO weightReading = new WeightReadingDO();
				weightReading.CompartmentName = this.GetStringValue("Name", false, navigator);

				if (string.IsNullOrEmpty(weightReading.CompartmentName))
				{
					weightReading.CompartmentName = string.Empty;
				}

				weightReading.BeginQuantity = this.GetNullableDoubleSIValue("BeginQuantity", units, false, navigator);
				weightReading.RequestedQuantity = this.GetNullableDoubleSIValue("RequestedQuantity", units, false, navigator);
				weightReading.FinalQuantity = this.GetNullableDoubleSIValue("FinalQuantity", units, false, navigator);
				weightReading.VolumetricTopOffFlag = this.GetNullableBool("VolumetricTopOffFlag", false, navigator);
			    this.transaction.WeightReadings.Add(weightReading);
			}
		}

		#endregion

		/// <summary>
		/// Check to make sure that the inventory date is after the closeout date for the products and manager associated with the transactionParam
		/// </summary>
		private void CheckCloseoutDates()
		{
			List<CloseoutDO> siteCloseouts = this.importProcessor.GetSiteCloseoutsForManager(this.transaction.Site, this.transaction.ManagerID);
			List<CloseoutDO> siteToManagerCloseouts = new List<CloseoutDO>();
			if(!string.IsNullOrWhiteSpace(this.transaction.ToManagerID))
			{
				siteToManagerCloseouts = this.importProcessor.GetSiteCloseoutsForManager(this.transaction.Site, this.transaction.ToManagerID);
            }

            GeneralConfigDO siteAccountingConfiguration = this.importProcessor.GetSiteAccountingConfiguration(this.transaction.Site);

			foreach (LineItemDO lineItemDO in this.transaction.LineItems)
			{
				string lineItemProduct = lineItemDO.Product;

				string errorMessage = FMChannelHelper.MakeCall<ITransactionValidator, string>(
						transactionValidator => transactionValidator.ValidateInventoryDate(lineItemProduct, this.transaction.Site, this.transaction.InventoryDate, this.transaction.CloseoutDate, siteCloseouts, siteToManagerCloseouts, siteAccountingConfiguration));

				if (!string.IsNullOrEmpty(errorMessage))
				{
					this.transactionValidationResult.ErrorList.Add(errorMessage);
				}

                if (this.transaction.TransTypeID == TransactionTypes.T15_PrimaryRegrade || this.transaction.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
                {
					var regradeLineItem = lineItemDO as RegradeLineItemDO;
					if(regradeLineItem != null)
					{
                        errorMessage = FMChannelHelper.MakeCall<ITransactionValidator, string>(
							transactionValidator => transactionValidator.ValidateInventoryDate(regradeLineItem.ToProduct, this.transaction.Site, this.transaction.InventoryDate, this.transaction.CloseoutDate, siteCloseouts, siteToManagerCloseouts, siteAccountingConfiguration));
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            this.transactionValidationResult.ErrorList.Add(errorMessage);
                        }
                    }
                }

                if (lineItemDO.SubLineItems != null)
				{
					foreach (SubLineItemDO subLineItemDO in lineItemDO.SubLineItems)
					{
						string subLineItemProduct = subLineItemDO.Product;

						errorMessage = FMChannelHelper.MakeCall<ITransactionValidator, string>(
							transactionValidator => transactionValidator.ValidateInventoryDate(subLineItemProduct, this.transaction.Site, this.transaction.InventoryDate, this.transaction.CloseoutDate, siteCloseouts, siteToManagerCloseouts, siteAccountingConfiguration));

						if (!string.IsNullOrEmpty(errorMessage))
						{
							this.transactionValidationResult.ErrorList.Add(errorMessage);
						}
					}
				}
			}
		}

		/// <summary>
		/// Make sure that any transactions referenced by the reversed or conjoined transID actually exist.
		/// This is not perfect due to the nature of the import process. It's possible that the transactionParam that is supposed to exist
		/// might be in the same batch or in a later batch of transactions. 
		/// In that case the import may need to be run again after the referenced transactionParam is created.
		/// </summary>
		private void CheckAndSetTransactionIDReferences()
		{
			List<string> transIdsToValidate = new List<string>();

			if (!string.IsNullOrEmpty(this.transaction.ReversedTransID))
			{
				transIdsToValidate.Add(this.transaction.ReversedTransID);
			}

			if (!string.IsNullOrEmpty(this.transaction.ConjoinedTransID))
			{
				transIdsToValidate.Add(this.transaction.ConjoinedTransID);
			}

			if (transIdsToValidate.Count > 0)
			{
				Dictionary<string, Guid> results = FMChannelHelper.MakeCall<ITransactionImportProcessor, Dictionary<string, Guid>>(
					transactionImportProcessor => transactionImportProcessor.GetTransactionGuidsForTransIDs(this.importProcessor.security, transIdsToValidate));

				if (!string.IsNullOrEmpty(this.transaction.ReversedTransID))
				{
					Guid reversedTransactionGuid;

					results.TryGetValue(this.transaction.ReversedTransID, out reversedTransactionGuid);

					if (reversedTransactionGuid == Guid.Empty)
					{
						this.transactionValidationResult.ErrorList.Add("Reversed Transaction ID " + this.transaction.ReversedTransID + " not found."
																	   + " If you believe that the referenced transactionParam was created by this import please try importing this transactionParam again.");
					}
				}

				if (!string.IsNullOrEmpty(this.transaction.ConjoinedTransID))
				{
					Guid conjoinedTransactionGuid;

					results.TryGetValue(this.transaction.ConjoinedTransID, out conjoinedTransactionGuid);

					if (conjoinedTransactionGuid == Guid.Empty)
					{
						this.transactionValidationResult.ErrorList.Add("Conjoined Transaction ID " + this.transaction.ConjoinedTransID + " not found."
							+ " If you believe that the referenced transactionParam was created by this import please try importing this transactionParam again.");
					}
					else
					{
						// Since we've already retrieved the conjoined Guid set it here to avoid having to retrieve it again
					    this.transaction.ConjoinedTransactionGuid = conjoinedTransactionGuid;
					}
				}
			}
		}

        private void ValidateMeter(SecurityClass sc)
        {
            if(string.IsNullOrEmpty(this.transaction.LineItems[0].MeterID))
            {
                throw new Exception("No meter specified for this transaction.");
            }

            var meterGuid = _metersProxy.GetIdentityGuid(this.transaction.LineItems[0].MeterID);
            if(meterGuid == Guid.Empty)
            {
                throw new Exception(string.Format("Meter {0} not configured for site {1}.", this.transaction.LineItems[0].MeterID, sc.SiteID));
            }
        }
	}
}
