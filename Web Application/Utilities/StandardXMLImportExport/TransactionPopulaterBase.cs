using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for TransactionPopulaterBase.
	/// </summary>
	public class TransactionPopulaterBase : TransactionDOM
	{
		#region Attributes
		protected TransactionDO transaction;
		protected LineItemDO lineItem;
		protected string lineItemPath;
		protected SubLineItemDO subLineItem;
		protected string subLineItemPath;
		#endregion Attributes

		public TransactionPopulaterBase()
		{
			
		}

		#region Public Helper Methods
		#region  Transaction header
		virtual protected void SetSite()
			{ transaction.Site = GetStringValue("Site"); }
		virtual protected void SetAlias()
			{ transaction.Alias = GetStringValue("TransactionAlias"); }
		virtual protected void SetTransID()
			{ transaction.TransID = GetStringValue("ID"); }
		virtual protected void SetInventoryDate()
			{ transaction.InventoryDate = GetDateValue("InventoryDate"); }
		virtual protected void SetTransactionDateTime()
			{ transaction.TransactionDateTime = GetDateValue("TrxnDateTime"); }
		virtual protected void SetTicketSource()
			{ transaction.TicketSource = GetStringValue("TicketSource"); }
		virtual protected void SetDocumentNumber()
		{ transaction.DocumentNumber = GetStringValue("DocumentNumber"); }
		virtual protected void SetShippingDocumentNumber()
		{ transaction.ShippingDocumentNumber = GetStringValue("ShippingDocumentNumber"); }
		virtual protected void SetShipmentNumber()
		{ transaction.ShipmentNumber = GetStringValue("ShipmentNumber"); }
		virtual protected void SetOwner()
			{ transaction.Owner = GetStringValue("Owner"); }
		virtual protected void SetManager()
			{ transaction.Manager= GetStringValue("Manager"); }
		virtual protected void SetShipTo()
			{ transaction.ShipTo = GetStringValue("ShipTo"); }
		virtual protected void SetBillTo()
			{ transaction.BillTo = GetStringValue("BillTo", false); }
		virtual protected void SetShipper()
			{ transaction.Shipper = GetStringValue("Shipper", false); }
		virtual protected void SetCarrier()
		{
			transaction.Carrier = GetStringValue("Carrier/Name", false);
			transaction.SCACCode = GetStringValue("Carrier/SCAC", false);
		}
		virtual protected void SetSupplier()
			{ transaction.Supplier = GetStringValue("Supplier"); }
		virtual protected void SetReversedTransID()
			{ transaction.ReversedTransID = GetStringValue("ReversedTransactionID", false); }
		virtual protected void SetReversalType()
			{ transaction.ReversalType = GetCharValue("ReversalType"); }
		virtual protected void SetCloseoutDateTime()
			{ transaction.CloseoutDateTime = GetDateValue("CloseOutDateTime", false); }
		virtual protected void SetLinkedDocumentNumber()
			{ transaction.LinkedDocumentNumber = GetStringValue("LinkedDocumentNumber", false); }
		virtual protected void SetTransRefID()
			{ transaction.TransRefID = GetStringValue("TransRefID", false); }
		virtual protected void SetNotes()
			{ transaction.Notes = GetStringValue("Notes", false); }
		virtual protected void SetPONumber()
			{ transaction.PONumber = GetStringValue("PONumber", false); }
		virtual protected void SetDriverIDNumber()
			{ transaction.DriverIDNumber = GetStringValue("DriverIDNumber", false); }
		virtual protected void SetLocation()
			{ transaction.Location = GetStringValue("Location", false); }
		virtual protected void SetTimeIn()
			{ transaction.TimeIn = GetStringValue("TimeIn", false); }
		virtual protected void SetTimeOut()
			{ transaction.TimeOut = GetStringValue("TimeOut", false); }
		virtual protected void SetTimeEnd()
			{ transaction.TimeEnd = GetStringValue("TimeEnd", false); }

		virtual protected void SetConjoinedTransID()
			{ transaction.ConjoinedTransID = GetStringValue("ConjoinedTransID", true); }
		virtual protected void SetRequestedDeliveryDate()
			{ transaction.RequestedDeliveryDate = GetOptionalDateTime("RequestedDeliveryDate"); }
		virtual protected void SetLoadID()
			{ transaction.LoadID = GetStringValue("LoadID", false); }
		virtual protected void SetDeleteFlag()
			{ transaction.DeleteFlag = GetBoolValue("DeleteFlag", false); }
		#endregion Transaction header
		#region LineItem
		virtual protected void SetLineItemSequenceNumber()
			{ lineItem.SequenceNumber = GetIntValue(lineItemPath + "SequenceNumber"); }
		virtual protected void SetLineItemProductCode()
			{ lineItem.ProductCode = GetStringValue(lineItemPath + "ProductInfo/ProductCode"); }
		virtual protected void SetLineItemProduct()
			{ lineItem.Product = GetStringValue(lineItemPath + "ProductInfo/Product"); }
		virtual protected void SetLineItemProductType()
			{ lineItem.ProductType = GetStringValue(lineItemPath + "ProductInfo/ProductType"); }
		virtual protected void SetLineItemProductPrice()
			{ lineItem.ProductPrice = GetOptionalDoubleValue(lineItemPath + "ProductInfo/ProductPrice"); }
		virtual protected void SetLineItemGrossQuantity()
		{
			lineItem.Volume.GrossInventoryChange = this.GetVolume(lineItemPath + "AccountingData/Quantity/Gross",
				lineItemPath + "AccountingData/Quantity/Units");
		}
		virtual protected void SetLineItemNetQuantity()
		{
			lineItem.Volume.NetInventoryChange = this.GetVolume(lineItemPath + "AccountingData/Quantity/Net",
				lineItemPath + "AccountingData/Quantity/Units");
		}
		virtual protected void SetLineItemVCF()
			{ lineItem.VCF = GetOptionalDoubleValue(lineItemPath + "AccountingData/VCF"); }
		virtual protected void SetLineItemTemperature()
		{
			lineItem.Temperature = GetOptionalTemperature(lineItemPath + "AccountingData/Temperature",
												  lineItemPath + "AccountingData/TemperatureUnits"); 
		}
		virtual protected void SetLineItemDensity()
		{
			lineItem.Density = GetOptionalDensity(lineItemPath + "AccountingData/Density",
				lineItemPath + "AccountingData/DensityUnits");
		}
		virtual protected void SetLineItemCustoms()
		{
//				lineItem.Customs = 
//			  (CustomsType) System.Enum.Parse(typeof(CustomsType), GetStringValue(lineItemPath + "AccountingData/Customs", false));
			lineItem.Customs = GetStringValue(lineItemPath + "AccountingData/Customs", false);
		}
		virtual protected void SetLineItemContractNumber()
			{ lineItem.ContractNumber = GetStringValue(lineItemPath + "ContractNumber", false); }
		virtual protected void SetLineItemCLIN()
			{ lineItem.CLIN = GetStringValue(lineItemPath + "CLIN", false); }
		virtual protected void SetLineItemArmNumber()
			{ lineItem.ArmNumber = GetStringValue(lineItemPath + "ArmNumber", false); }
		virtual protected void SetLineItemLineNumber()
			{ lineItem.LineNumber = GetStringValue(lineItemPath + "LineNumber", false); }
		virtual protected void SetLineItemOperatorID()
			{ lineItem.OperatorID = GetStringValue(lineItemPath + "Operator", false); }
		virtual protected void SetLineItemBatchNumber()
			{ lineItem.BatchNumber = GetStringValue(lineItemPath + "BatchNumber", false); }
		virtual protected void SetLineItemDocumentNumber()
			{ lineItem.DocumentNumber = GetStringValue(lineItemPath + "DocumentNumber", false); }
		virtual protected void SetLineItemLineFill()
			{ lineItem.LineFill = GetOptionalVolume(lineItemPath + "LineFill", lineItemPath + "LineFillUnits"); }
		virtual protected void SetLineItemBottomVolume()
			{ lineItem.BottomVolume = GetOptionalVolume(lineItemPath + "BottomVolume", lineItemPath + "BottomVolumeUnits"); }
		virtual protected void SetLineItemNetCapacity()
			{ lineItem.NetCapacity = GetOptionalVolume(lineItemPath + "NetCapacity", lineItemPath + "NetCapacityUnits"); }
		virtual protected void SetLineItemTankStatus()
		{
			string s = GetStringValue(lineItemPath + "TankStatus", false);
			if(s == null || s == "")
			{
				lineItem.TankStatus = '\0';
				return;
			}
			lineItem.TankStatus = s[0];
		}
		virtual protected void SetLineItemPit()
			{ lineItem.Pit = GetStringValue(lineItemPath + "Pit", false); }
		virtual protected void SetLineItemRequestedDateTime()
			{ lineItem.RequestedDateTime = GetOptionalDateTime(lineItemPath + "RequestedDateTime"); }
		virtual protected void SetLineItemDispatchedDateTime()
			{ lineItem.DispatchedDateTime = GetOptionalDateTime(lineItemPath + "DispatchedDateTime"); }
		virtual protected void SetLineItemAcknowledgedDateTime()
			{ lineItem.AcknowledgedDateTime = GetOptionalDateTime(lineItemPath + "AcknowledgedDateTime"); }
		virtual protected void SetLineItemOnLocationTime()
			{ lineItem.OnLocationTime = GetOptionalDateTime(lineItemPath + "OnLocationTime"); }
		virtual protected void SetLineItemValidationDateTime()
			{ lineItem.ValidationDateTime = GetOptionalDateTime(lineItemPath + "ValidationDateTime"); }
		virtual protected void SetLineItemCompletionDateTime()
			{ lineItem.CompletionDateTime = GetOptionalDateTime(lineItemPath + "CompletionDateTime"); }
		virtual protected void SetLineItemReceiptVariance()
		{
			lineItem.LoadRackVariance = GetOptionalVolume(lineItemPath + "ReceiptVariance",
														  lineItemPath + "ReceiptVarianceUnits");
		}
		virtual protected void SetLineItemDifferentialPressure()
		{
			lineItem.DifferentialPressure = GetOptionalVolume(lineItemPath + "DifferentialPressure", 
															  lineItemPath + "DifferentialPressureUnits");
		}
		virtual protected void SetLineItemLoadRackVariance()
		{
			lineItem.LoadRackVariance = GetOptionalVolume(lineItemPath + "LoadRackVariance",
				lineItemPath + "LoadRackVarianceUnits");
		}
		virtual protected void SetLineItemRequestedBy()
		{ lineItem.RequestedBy = GetStringValue(lineItemPath + "RequestedBy", false); }
		virtual protected void SetLineItemFreezePoint()
		{
			lineItem.FreezePoint = GetOptionalTemperature(lineItemPath + "ProductInfo/FreezePoint", 
														  lineItemPath + "ProductInfo/FreezePointUnits");
		}
		virtual protected void SetLineItemDestinationRegistrationID1()
			{ lineItem.DestinationEQ1.RegistrationID = GetStringValue(lineItemPath + "ToEquipment/Equipment1/RegistrationID", false); }
		virtual protected void SetLineItemDestinationRegistrationID2()
			{ lineItem.DestinationEQ2.RegistrationID = GetStringValue(lineItemPath + "ToEquipment/Equipment2/RegistrationID", false); }
		virtual protected void SetLineItemDestinationRegistrationID3()
			{ lineItem.DestinationEQ3.RegistrationID = GetStringValue(lineItemPath + "ToEquipment/Equipment3/RegistrationID", false); }
		virtual protected void SetLineItemDestinationSerialNumber1()
			{ lineItem.DestinationEQ1.SerialNumber = GetStringValue(lineItemPath + "ToEquipment/Equipment1/SerialNumber", false); }
		virtual protected void SetLineItemDestinationSerialNumber2()
			{ lineItem.DestinationEQ2.SerialNumber = GetStringValue(lineItemPath + "ToEquipment/Equipment2/SerialNumber", false); }
		virtual protected void SetLineItemDestinationSerialNumber3()
			{ lineItem.DestinationEQ3.SerialNumber = GetStringValue(lineItemPath + "ToEquipment/Equipment3/SerialNumber", false); }
		virtual protected void SetLineItemDestinationEquipmentType1()
			{ lineItem.DestinationEQ1.EquipmentType = GetStringValue(lineItemPath + "ToEquipment/Equipment1/EquipmentType", false); }
		virtual protected void SetLineItemDestinationEquipmentType2()
			{ lineItem.DestinationEQ2.EquipmentType = GetStringValue(lineItemPath + "ToEquipment/Equipment2/EquipmentType", false); }
		virtual protected void SetLineItemDestinationEquipmentType3()
			{ lineItem.DestinationEQ3.EquipmentType = GetStringValue(lineItemPath + "ToEquipment/Equipment3/EquipmentType", false); }
		virtual protected void SetLineItemDestinationEquipmentModel1()
			{ lineItem.DestinationEQ1.EquipmentModel = GetStringValue(lineItemPath + "ToEquipment/Equipment1/EquipmentModel", false); }
		virtual protected void SetLineItemDestinationEquipmentModel2()
			{ lineItem.DestinationEQ2.EquipmentModel = GetStringValue(lineItemPath + "ToEquipment/Equipment2/EquipmentModel", false); }
		virtual protected void SetLineItemDestinationEquipmentModel3()
			{ lineItem.DestinationEQ3.EquipmentModel = GetStringValue(lineItemPath + "ToEquipment/Equipment3/EquipmentModel", false); }
		virtual protected void SetLineItemSourceRegistrationID1()
			{ lineItem.SourceEQ1.RegistrationID = GetStringValue(lineItemPath + "FromEquipment/Equipment1/RegistrationID", false); }
		virtual protected void SetLineItemSourceRegistrationID2()
			{ lineItem.SourceEQ2.RegistrationID = GetStringValue(lineItemPath + "FromEquipment/Equipment2/RegistrationID", false); }
		virtual protected void SetLineItemSourceRegistrationID3()
			{ lineItem.SourceEQ3.RegistrationID = GetStringValue(lineItemPath + "FromEquipment/Equipment3/RegistrationID", false); }
		virtual protected void SetLineItemSourceSerialNumber1()
			{ lineItem.SourceEQ1.SerialNumber = GetStringValue(lineItemPath + "FromEquipment/Equipment1/SerialNumber", false); }
		virtual protected void SetLineItemSourceSerialNumber2()
			{ lineItem.SourceEQ2.SerialNumber = GetStringValue(lineItemPath + "FromEquipment/Equipment2/SerialNumber", false); }
		virtual protected void SetLineItemSourceSerialNumber3()
			{ lineItem.SourceEQ3.SerialNumber = GetStringValue(lineItemPath + "FromEquipment/Equipment3/SerialNumber", false); }
		virtual protected void SetLineItemSourceEquipmentType1()
			{ lineItem.SourceEQ1.EquipmentType = GetStringValue(lineItemPath + "FromEquipment/Equipment1/EquipmentType", false); }
		virtual protected void SetLineItemSourceEquipmentType2()
			{ lineItem.SourceEQ2.EquipmentType = GetStringValue(lineItemPath + "FromEquipment/Equipment2/EquipmentType", false); }
		virtual protected void SetLineItemSourceEquipmentType3()
			{ lineItem.SourceEQ3.EquipmentType = GetStringValue(lineItemPath + "FromEquipment/Equipment3/EquipmentType", false); }
		virtual protected void SetLineItemSourceEquipmentModel1()
			{ lineItem.SourceEQ1.EquipmentModel = GetStringValue(lineItemPath + "FromEquipment/Equipment1/EquipmentModel", false); }
		virtual protected void SetLineItemSourceEquipmentModel2()
			{ lineItem.SourceEQ2.EquipmentModel = GetStringValue(lineItemPath + "FromEquipment/Equipment2/EquipmentModel", false); }
		virtual protected void SetLineItemSourceEquipmentModel3()
			{ lineItem.SourceEQ3.EquipmentModel = GetStringValue(lineItemPath + "FromEquipment/Equipment3/EquipmentModel", false); }
		virtual protected void SetLineItemMeterFactor()
			{ lineItem.MeterReading.MeterFactor = GetOptionalDoubleValue(lineItemPath + "MeterReadings/MeterFactor"); }
		virtual protected void SetLineItemMeterStart()
			{ lineItem.MeterReading.MeterStart = GetOptionalDoubleValue(lineItemPath + "MeterReadings/MeterStart"); }
		virtual protected void SetLineItemMeterStop()
			{ lineItem.MeterReading.MeterStop = GetOptionalDoubleValue(lineItemPath + "MeterReadings/MeterStop"); }
		virtual protected void SetLineItemMeterStartDateTime()
		{ lineItem.MeterReading.StartDateTime =
				GetOptionalDateTime(lineItemPath + "MeterReadings/StartTime");
		}
		virtual protected void SetLineItemMeterStopDateTime()
		{ lineItem.MeterReading.StopDateTime =
			  GetOptionalDateTime(lineItemPath + "MeterReadings/StopTime");
		}
		#endregion LineItem

		#region SubLineItem
		virtual protected void SetSubLineItemProduct()
		{ subLineItem.Product = GetStringValue(subLineItemPath + "ProductInfo/Product"); }
		virtual protected void SetSubLineItemProductCode()
		{ subLineItem.ProductCode = GetStringValue(subLineItemPath + "ProductInfo/ProductCode"); }
		virtual protected void SetSubLineItemProductType()
		{ subLineItem.ProductType = GetStringValue(subLineItemPath + "ProductInfo/ProductType"); }
		virtual protected void SetSubLineItemGrossQuantity()
		{
			subLineItem.Volume.GrossInventoryChange = this.GetVolume(subLineItemPath + "AccountingData/Quantity/Gross",
				lineItemPath + "AccountingData/Quantity/Units");
		}
		virtual protected void SetSubLineItemNetQuantity()
		{
			subLineItem.Volume.NetInventoryChange = this.GetVolume(subLineItemPath + "AccountingData/Quantity/Net",
				lineItemPath + "AccountingData/Quantity/Units");
		}
		virtual protected void SetSubLineItemVCF()
		{ subLineItem.VCF = GetOptionalDoubleValue(subLineItemPath + "AccountingData/VCF"); }
		virtual protected void SetSubLineItemTemperature()
		{
			subLineItem.Temperature = GetOptionalTemperature(subLineItemPath + "AccountingData/Temperature",
				subLineItemPath + "AccountingData/TemperatureUnits"); 
		}
		virtual protected void SetSubLineItemDensity()
		{
			subLineItem.Density = GetOptionalDensity(subLineItemPath + "AccountingData/Density",
				subLineItemPath + "AccountingData/DensityUnits");
		}
		virtual protected void SetSubLineItemCustoms()
		{ subLineItem.Customs = GetStringValue(subLineItemPath + "AccountingData/Customs", false); }
		virtual protected void SetSubLineItemArmNumber()
		{ subLineItem.ArmNumber = GetOptionalInt(subLineItemPath + "ArmNumber"); }
		virtual protected void SetSubLineItemLineNumber()
		{ subLineItem.LineNumber = GetOptionalInt(subLineItemPath + "LineNumber"); }
		virtual protected void SetSubLineItemBatchNumber()
		{ subLineItem.BatchNumber = GetStringValue(subLineItemPath + "BatchNumber", false); }
		virtual protected void SetSubLineItemLineFill()
		{ subLineItem.LineFill = GetOptionalVolume(subLineItemPath + "LineFill", subLineItemPath + "LineFillUnits"); }
		virtual protected void SetSubLineItemBottomVolume()
		{ subLineItem.BottomVolume = GetOptionalVolume(subLineItemPath + "BottomVolume", subLineItemPath + "BottomVolumeUnits"); }
		virtual protected void SetSubLineItemNetCapacity()
		{ subLineItem.NetCapacity = GetOptionalVolume(subLineItemPath + "NetCapacity", subLineItemPath + "NetCapacityUnits"); }
		virtual protected void SetSubLineItemTankStatus()
		{
			string s = GetStringValue(subLineItemPath + "TankStatus", false);
			if(s == null || s == "")
			{
				subLineItem.TankStatus = '\0';
				return;
			}
			subLineItem.TankStatus = s[0];
		}
		virtual protected void SetSubLineItemAdditiveProfile()
		{ subLineItem.AdditiveProfile = GetStringValue(subLineItemPath + "AdditiveProfile"); }
		virtual protected void SetSubLineItemMeterFactor()
		{ subLineItem.MeterReading.MeterFactor = GetOptionalDoubleValue(subLineItemPath + "MeterReadings/MeterFactor"); }
		virtual protected void SetSubLineItemMeterStart()
		{ subLineItem.MeterReading.MeterStart = GetOptionalDoubleValue(subLineItemPath + "MeterReadings/MeterStart"); }
		virtual protected void SetSubLineItemMeterStop()
		{ subLineItem.MeterReading.MeterStop = GetOptionalDoubleValue(subLineItemPath + "MeterReadings/MeterStop"); }
		virtual protected void SetSubLineItemMeterStartDateTime()
		{
				subLineItem.MeterReading.StartDateTime =
			  GetOptionalDateTime(subLineItemPath + "MeterReadings/StartTime");
		}
		virtual protected void SetSubLineItemMeterStopDateTime()
		{
				subLineItem.MeterReading.StopDateTime =
			  GetOptionalDateTime(subLineItemPath + "MeterReadings/StopTime");
		}
		virtual protected void SetSubLineItemFreezePoint()
		{
			subLineItem.FreezePoint = GetOptionalTemperature(subLineItemPath + "ProductInfo/FreezePoint", 
				subLineItemPath + "ProductInfo/FreezePointUnits");
		}
		virtual protected void SetSubLineItemDifferentialPressure()
		{
			subLineItem.DifferentialPressure = GetOptionalVolume(subLineItemPath + "DifferentialPressure", 
				subLineItemPath + "DifferentialPressureUnits");
		}
		virtual protected void SetSubLineItemDosageRate()
		{ subLineItem.DosageRate = GetOptionalDoubleValue(subLineItemPath + "DosageRate"); }
		#endregion SubLineItem
		#endregion Public Helper Methods
	}
}
