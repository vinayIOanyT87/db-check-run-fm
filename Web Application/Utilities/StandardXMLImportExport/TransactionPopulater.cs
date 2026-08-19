using System;

using FM7Accounting;


namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for TransactionPopulater.
	/// </summary>
	public abstract class TransactionPopulater : TransactionPopulaterBase
	{
		#region Attributes
		ImportAliasUserFieldsDO userFields;
		#endregion Attributes
		
		public TransactionPopulater()
		{
			GetTransactionAliases();
		}

		public TransactionDO PopulateTransaction(TransactionDO transaction, System.Xml.XmlDocument doc)
		{
			this.doc = doc;
			this.transaction = transaction;

			PopulateCommon();
			Populate();

			this.doc = null;
			this.transaction = null;
			return transaction;
		}

		#region Abstract Methods
//		abstract protected TransactionLineItemDO CreateLineItem();
		abstract protected void PopulateLineItem();
		protected abstract void Populate();
		#endregion Abstract Methods

		abstract protected string TransactionTypeID { get; }

		protected void PopulateCommon()
		{
			SetSite();
			SetAlias();
			transaction.TransTypeID = this.TransactionTypeID;
			SetTransID();
			SetInventoryDate();
			SetTransactionDateTime();
			SetTicketSource();
			SetDocumentNumber();
			SetOwner();
			SetManager();
			SetBillTo();
			SetLinkedDocumentNumber();
			SetReversalType();
			SetReversedTransID();
			SetTransRefID();
			SetCloseoutDateTime();
			SetNotes();
			SetPONumber();
			SetDriverIDNumber();
			SetLocation();
			SetTimeIn();
			SetTimeOut();
			SetTimeEnd();
			SetLoadID();
			SetDeleteFlag();

			PopulateUserData();

			PopulateLineItems();

		}

		protected void PopulateLineItems()
		{
			const string lineItemsPath = "/descendant::LineItems/LineItem";
			System.Xml.XmlNodeList nodeList = doc.SelectNodes(lineItemsPath);
			if((nodeList == null) || (nodeList.Count == 0))
			{
				throw new Exception("Transaction does not contain  any LineItem elements.");										   
			}
			for(int i=0; i < nodeList.Count; ++i)
			{
				System.Xml.XmlNode node = nodeList[i];
				lineItemPath = "LineItems/LineItem[position()=" + (i+1) + "]/";
				lineItem = new LineItemDO();
				transaction.LineItems.Add(lineItem);
				PopulateLineItemCommon();
				PopulateLineItem();

				lineItem = null;
				lineItemPath = null;
			}
		}

		protected void PopulateLineItemCommon()
		{
			SetLineItemSequenceNumber();
			PopulateProductInfo();
			PopulateAccountingInfo(transaction, lineItem, lineItemPath);
			SetLineItemContractNumber();
			SetLineItemCLIN();
			SetLineItemArmNumber();
			SetLineItemLineNumber();
			SetLineItemOperatorID();
			SetLineItemPit();

			SetLineItemRequestedDateTime();
			SetLineItemDispatchedDateTime();
			SetLineItemAcknowledgedDateTime();
			SetLineItemOnLocationTime();
			SetLineItemValidationDateTime();
			SetLineItemCompletionDateTime();

			SetLineItemReceiptVariance();
			SetLineItemDifferentialPressure();
			SetLineItemLoadRackVariance();
			SetLineItemRequestedBy();
			SetLineItemFreezePoint();

			SetLineItemDestinationEquipment();
			SetLineItemSourceEquipment();
			SetLineItemMeterReadings();
			
			SetLineItemDocumentNumber();
		}

		protected void PopulateSubLineItems()
		{
			string subLineItemsPath = "/descendant::" + lineItemPath + "SubLineItems/SubLineItem";
			System.Xml.XmlNodeList nodeList = doc.SelectNodes(subLineItemsPath);

			for(int i=0; i < nodeList.Count; ++i)
			{
				System.Xml.XmlNode node = nodeList[i];
				subLineItemPath = lineItemPath + "SubLineItems/SubLineItem[position()=" + (i+1) + "]/";

				subLineItem = new SubLineItemDO();
				lineItem.SubLineItems.Add(subLineItem);

				PopulateSubLineItem();

				subLineItem = null;
				subLineItemPath = null;
			}
		}

		protected void PopulateProductInfo()
		{
			SetLineItemProductCode();
			SetLineItemProduct();
			SetLineItemProductType();
			SetLineItemProductPrice();

			
		}

		protected void PopulateAccountingInfo(TransactionDO transaction, LineItemDO lineItem, 
			string lineItemPath)
		{
			SetLineItemGrossQuantity();
			SetLineItemNetQuantity();

			SetLineItemVCF();
			SetLineItemTemperature();
			SetLineItemDensity();

			SetLineItemCustoms();
		}

		protected void PopulateUserData()
		{
			System.Xml.XmlNodeList nodeList = doc.SelectNodes("/descendant::UserFields/UserField");
			if(nodeList != null)
			{
				for(int i=1; i <= nodeList.Count; ++i)
				{
					string path = "UserFields/UserField[position()=" + i + "]/";
					string fieldName = GetStringValue(path + "Name");
					string fieldValue = GetStringValue(path + "Value");

					SetUserData(transaction, fieldName, fieldValue);
				}
			}
		}

		virtual protected void PopulatePaymentInfo()
		{
			if(doc.SelectSingleNode("/"  + doc.DocumentElement.LocalName + "/PaymentInfo") == null)
			{
				return;
			}
			transaction.PaymentInfo = new PaymentInfoDO();

			transaction.PaymentInfo.CashAmount = GetStringValue("PaymentInfo/Cash/Amount", false);
			transaction.PaymentInfo.CashCurrencyType = GetStringValue("PaymentInfo/Cash/CurrencyType", false);
			transaction.PaymentInfo.CreditCardAmount = GetStringValue("PaymentInfo/CreditCard/Amount", false);
			transaction.PaymentInfo.CreditCardCurrencyType = GetStringValue("PaymentInfo/CreditCard/CurrencyType", false);
			transaction.PaymentInfo.CreditCardExpiration = GetStringValue("PaymentInfo/CreditCard/Expiration", false);
			transaction.PaymentInfo.CreditCardName = GetStringValue("PaymentInfo/CreditCard/CardName", false);
			transaction.PaymentInfo.CreditCardNumber = GetStringValue("PaymentInfo/CreditCard/CardNumber", false);
			transaction.PaymentInfo.CreditCardType = GetStringValue("PaymentInfo/CreditCard/CardType", false);
		}

		protected void PopulateFuelingData()
		{
			transaction.SimultaneousFueling = GetBoolValue("FuelingData/SimultaneousFueling");
			transaction.EstimatedFuelingDuration = 
				GetOptionalDuration("FuelingData/EstimatedFuelingDuration", "FuelingData/EstimatedFuelingDurationUnits");
//			transaction.FinishFuelingTime = GetDateTimeValue("FuelingData/FinishTime");
		}

		protected void PopulateRouteInfo()
		{
			RouteInfoDO routeInfo = new RouteInfoDO();
			transaction.RouteInfo = routeInfo;

			routeInfo.RoutingID = GetStringValue("RouteData/RouteID", false);
			routeInfo.RouteOriginationDate = GetStringValue("RouteData/RouteOriginationDate", false);
			routeInfo.InternationalRouteIndicator =
				GetBoolValue("RouteData/InternationalRoute", false);
			routeInfo.PreviousRoutingID = GetStringValue("RouteData/PreviousRouteID", false);
			routeInfo.FinalStation = GetStringValue("RouteData/FinalStation", false);
			routeInfo.PreviousStation = GetStringValue("RouteData/PreviousStation", false);
			routeInfo.NextStation = GetStringValue("RouteData/NextStation", false);
			routeInfo.OriginStation = GetStringValue("RouteData/OriginStation", false);
		}

		protected void PopulateRouteSchedule()
		{
			RouteScheduleDO schedule = new RouteScheduleDO();
			transaction.RouteSchedule = schedule;

			schedule.ETA = GetStringValue("ETA", false);
			schedule.ETD = GetStringValue("ETD", false);
			schedule.FST = GetStringValue("FST", false);
			schedule.SFT = GetStringValue("SFT", false);
			schedule.STA = GetStringValue("STA", false);
			schedule.STD = GetStringValue("STD", false);
		}

		protected void SetLineItemDestinationEquipment()
		{
			lineItem.DestinationEQ1 = new EquipmentDO();
			lineItem.DestinationEQ2 = new EquipmentDO();
			lineItem.DestinationEQ3 = new EquipmentDO();

			SetLineItemDestinationRegistrationID1();
			SetLineItemDestinationRegistrationID2();
			SetLineItemDestinationRegistrationID3();
			SetLineItemDestinationSerialNumber1();
			SetLineItemDestinationSerialNumber2();
			SetLineItemDestinationSerialNumber3();
			SetLineItemDestinationEquipmentType1();
			SetLineItemDestinationEquipmentType2();
			SetLineItemDestinationEquipmentType3();
			SetLineItemDestinationEquipmentModel1();
			SetLineItemDestinationEquipmentModel2();
			SetLineItemDestinationEquipmentModel3();
		}

		protected void SetLineItemSourceEquipment()
		{
			lineItem.SourceEQ1 = new EquipmentDO();
			lineItem.SourceEQ2 = new EquipmentDO();
			lineItem.SourceEQ3 = new EquipmentDO();
			
			SetLineItemSourceRegistrationID1();
			SetLineItemSourceRegistrationID2();
			SetLineItemSourceRegistrationID3();
			SetLineItemSourceSerialNumber1();
			SetLineItemSourceSerialNumber2();
			SetLineItemSourceSerialNumber3();
			SetLineItemSourceEquipmentType1();
			SetLineItemSourceEquipmentType2();
			SetLineItemSourceEquipmentType3();
			SetLineItemSourceEquipmentModel1();
			SetLineItemSourceEquipmentModel2();
			SetLineItemSourceEquipmentModel3();
		}

		protected void SetLineItemMeterReadings()
		{
			MeterReadingDO reading = new MeterReadingDO();
			lineItem.MeterReading = reading;

			SetLineItemMeterFactor();
			SetLineItemMeterStart();
			SetLineItemMeterStop();
			SetLineItemMeterStartDateTime();
			SetLineItemMeterStopDateTime();
		}

		protected void PopulateSubLineItem()
		{
			SetSubLineItemProduct();
			SetSubLineItemProductCode();
			SetSubLineItemProductType();
			SetSubLineItemGrossQuantity();
			SetSubLineItemNetQuantity();
			SetSubLineItemVCF();
			SetSubLineItemTemperature();
			SetSubLineItemDensity();
			SetSubLineItemCustoms();
			SetSubLineItemArmNumber();
			SetSubLineItemLineNumber();
			SetSubLineItemBatchNumber();
			SetSubLineItemLineFill();
			SetSubLineItemBottomVolume();
			SetSubLineItemNetCapacity();
			SetSubLineItemTankStatus();
			SetSubLineItemAdditiveProfile();
			SetSubLineItemMeterFactor();
			SetSubLineItemMeterStart();
			SetSubLineItemMeterStop();
			SetSubLineItemMeterStartDateTime();
			SetSubLineItemMeterStopDateTime();
			SetSubLineItemFreezePoint();
			SetSubLineItemDifferentialPressure();
			SetSubLineItemDosageRate();
		}

		protected void SetUserData(TransactionDO transaction, string fieldName, string fieldValue)
		{
			string dbName = userFields.GetDbName(transaction.Site, transaction.Alias, fieldName);
			transaction.UserData.Add(dbName, fieldValue);
		}

		protected void PopulateAviationGaugeReadings()
		{
			string gaugeReadingsPath = "/descendant::AviationGaugeReadings/Compartment";
			System.Xml.XmlNodeList nodeList = doc.SelectNodes(gaugeReadingsPath);
	
			transaction.AviationGaugeReadings = new System.Collections.ArrayList();

			for(int i=0; i < nodeList.Count; ++i)
			{
				System.Xml.XmlNode node = nodeList[i];
				string compartmentPath = "AviationGaugeReadings/Compartment[position()=" + (i+1) + "]/";
				AviationGaugeReadingDO gaugeReading = new AviationGaugeReadingDO();
				gaugeReading.CompartmentName = GetStringValue(compartmentPath + "Name");
				gaugeReading.BeginQuantity = GetOptionalVolume(compartmentPath + "BeginQuantity",
					"/descendant::AviationGaugeReadings/QuantityUnits");
				gaugeReading.RequestedQuantity = GetOptionalVolume(compartmentPath + "RequestedQuantity", 
					"/descendant::AviationGaugeReadings/QuantityUnits");
				gaugeReading.FinalQuantity = GetOptionalVolume(compartmentPath + "FinalQuantity", 
					"/descendant::AviationGaugeReadings/QuantityUnits");
				transaction.AviationGaugeReadings.Add(gaugeReading);
			}
		}

		protected void GetTransactionAliases()
		{
//			TransactionAliasUserDataSR sr = new TransactionAliasUserDataSR();
			ImportAliasUserFieldsSR sr = new ImportAliasUserFieldsSR();
			AccountingClient accountingClient = new AccountingClient();
			AccountingService server = accountingClient.connect();
			userFields = (ImportAliasUserFieldsDO) server.request(sr);
		}
	}
}
