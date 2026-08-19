// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionPopulaterBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Reads transaction data from XML and populates a transaction record with the data read
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XMLImport
{
	using System;
	using System.Xml.XPath;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System.Collections.Generic;

	internal class ExpandedFsrIsSupported : Dictionary<TransactionTypes, bool> { }

	public class TransactionPopulaterBase : TransactionDOM
	{
		#region Attributes

		protected TransactionDO transaction;
		protected XPathNavigator transactionNavigator;
		protected LineItemDO lineItem;
		protected XPathNavigator lineItemNavigator;
		protected XPathNavigator sublineItemNavigator;
		protected SubLineItemDO subLineItem;

		protected XMLImportProcessor importProcessor;

		ExpandedFsrIsSupported expandedFsrIsSupported = new ExpandedFsrIsSupported();
		readonly string[] expandedFsrCols = new string[] {
            //"DifferentialPressure",
            //"AcknowledgedDateTime",
            //"OnLocationTime",
            //"CompletionDateTime",
            "HydrantPressure",
			"MobileDeviceID",
			"MobileDeviceGuid",
			"TemperatureQualityStatus",
			"PartialFill",
			"FuelCompressionFactor",
			"DualFuelingModeFlag",
			"DualFuelingPrimaryFlag",
			"MeterStartObtainedAutomaticallyFlag",
			"MeterStopObtainedAutomaticallyFlag",
			"EngineRunTime",
			"FlowRate"
			};
		#endregion Attributes

		public void SetImportProcessor(XMLImportProcessor importProcessor)
		{
			this.importProcessor = importProcessor;
		}

		#region Public Helper Methods
		#region Transaction header

		virtual protected void SetSite()
		{
			this.transaction.Site = this.GetStringValue("Site", true, this.transactionNavigator);
			this.transaction.SiteGuid = this.importProcessor.GetSiteGuid(this.transaction.Site);

			if (this.transaction.SiteGuid == Guid.Empty)
			{
				this.transactionValidationResult.ErrorList.Add("Invalid Site \"" + this.transaction.Site + "\".");
			}
		}

		/// <summary>
		/// This method will set the alias name and guid in the transaction object.
		/// </summary>
		virtual protected void SetAlias()
		{
			this.transaction.Alias = this.GetStringValue("TransactionAlias", true, this.transactionNavigator);

			// We translate "24 Hr" to "24 Hour Closeout"
			if (string.Compare(this.transaction.Alias, "24 Hr", StringComparison.InvariantCulture) == 0)
			{
				this.transaction.Alias = "24 Hour Closeout";
			}

			this.transaction.TransactionAliasGuid = this.importProcessor.GetAliasGuid(this.transaction.Site, this.transaction.Alias);

			if (this.transaction.TransactionAliasGuid == Guid.Empty)
			{
				this.transactionValidationResult.ErrorList.Add("Invalid Transaction Alias \"" + this.transaction.Alias + "\" for Site \"" + this.transaction.Site + "\".");
			}
		}

		virtual protected void SetSubType()
		{ transaction.SubType = this.GetStringValue("SubType", false, this.transactionNavigator); }

		virtual protected void SetTransID()
		{ transaction.TransID = this.GetStringValue("ID", true, this.transactionNavigator); }

		virtual protected void SetInventoryDate()
		{ transaction.InventoryDate = this.GetDateValue("InventoryDate", true, this.transactionNavigator).Date; }

		virtual protected void SetTransactionDateTime()
		{ transaction.TransactionDateTime = this.GetNullableDateTime("TrxnDateTime", false, this.transactionNavigator); }

		virtual protected void SetTicketSource()
		{ transaction.TicketSource = this.GetStringValue("TicketSource", false, this.transactionNavigator); }

		virtual protected void SetTicketMode()
		{
			transaction.TicketMode = this.GetEnumValue("TicketMode", TicketModes.Unknown, false, this.transactionNavigator);
		}

		virtual protected void SetDocumentNumber()
		{
			this.transaction.DocumentNumber = this.GetStringValue("DocumentNumber", false, this.transactionNavigator);

			if (string.IsNullOrEmpty(this.transaction.DocumentNumber))
			{
				this.transaction.DocumentNumber = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetNextDocumentNumber(this.importProcessor.security, DOCUMENT_TYPE.TRANSACTION, this.importProcessor.security.SiteGuid).ToString()
																);
			}
		}

		virtual protected void SetShippingDocumentNumber()
		{
			this.transaction.ShippingDocumentNumber = this.GetStringValue("ShippingDocumentNumber", true, this.transactionNavigator);
		}

		virtual protected void SetShipmentNumber()
		{
			this.transaction.ShipmentNumber = this.GetStringValue("ShipmentNumber", true, this.transactionNavigator);
		}

		virtual protected void SetOwner(TransactionTypes TransTypeID)
		{
			string name, code;
			Guid companyGuid;

			if (TransactionTypes.T13_OwnerTransfer == TransTypeID)
			{
				OwnerTransferDO ownertransfer = this.transaction as OwnerTransferDO;

				GetCompany("FromOwner", out name, out code, out companyGuid);
				transaction.OwnerID = name;
				transaction.OwnerCode = code;
				transaction.OwnerCompanyGuid = companyGuid;

				GetCompany("ToOwner", out name, out code, out companyGuid);
				ownertransfer.ToOwnerID = name;
				ownertransfer.ToOwnerCode = code;
				ownertransfer.ToOwnerCompanyGuid = companyGuid;
			}
			// Do not set owner for physical inventory transactions. 
			else if (TransactionTypes.T14_PhysicalInventory != TransTypeID)
			{
				GetCompany("Owner", out name, out code, out companyGuid);
				transaction.OwnerID = name;
				transaction.OwnerCode = code;
				transaction.OwnerCompanyGuid = companyGuid;
			}
		}

		virtual protected void SetManager(TransactionTypes TransTypeID)
		{
			string name, code;
			Guid companyGuid;

			if (TransactionTypes.T13_OwnerTransfer == TransTypeID)
			{
				OwnerTransferDO ownertransfer = transaction as OwnerTransferDO;

				GetCompany("FromManager", out name, out code, out companyGuid);
				transaction.ManagerID = name;
				transaction.ManagerCode = code;
				transaction.ManagerCompanyGuid = companyGuid;

				GetCompany("ToManager", out name, out code, out companyGuid);
				ownertransfer.ToManagerID = name;
				ownertransfer.ToManagerCode = code;
				ownertransfer.ToManagerCompanyGuid = companyGuid;
			}
			else
			{
				GetCompany("Manager", out name, out code, out companyGuid);
				transaction.ManagerID = name;
				transaction.ManagerCode = code;
				transaction.ManagerCompanyGuid = companyGuid;
			}
		}

		virtual protected void SetShipTo()
		{
			string name, code;
			Guid companyGuid;
			GetCompany("ShipTo", out name, out code, out companyGuid);
			transaction.ShipToID = name;
			transaction.ShipToCode = code;
			transaction.ShipToCompanyGuid = companyGuid;
		}

		virtual protected void SetBillTo()
		{
			string name, code;
			Guid companyGuid;
			GetCompany("PaymentInfo/BillTo", false, out name, out code, out companyGuid);
			transaction.BillToID = name;
			transaction.BillToCode = code;
			transaction.BillToCompanyGuid = companyGuid;
		}

		virtual protected void SetShipper()
		{
			string name, code;
			Guid companyGuid;
			GetCompany("Shipper", false, out name, out code, out companyGuid);
			transaction.ShipperID = name;
			transaction.ShipperCode = code;
			transaction.ShipperCompanyGuid = companyGuid;
		}

		virtual protected void SetCarrier(TransactionTypes TransTypeID)
		{
			string name, code;
			Guid companyGuid;

			if (TransactionTypes.T13_OwnerTransfer == TransTypeID)
			{
				OwnerTransferDO ownertransfer = transaction as OwnerTransferDO;

				GetCompany("FromCarrier", false, out name, out code, out companyGuid);
				transaction.CarrierID = name;
				transaction.CarrierCode = code;
				transaction.CarrierCompanyGuid = companyGuid;
				transaction.SCACCode = this.GetStringValue("Carrier/SCAC", false, this.transactionNavigator);

				GetCompany("ToCarrier", false, out name, out code, out companyGuid);
				ownertransfer.ToCarrierID = name;
				ownertransfer.ToCarrierCode = code;
				ownertransfer.ToCarrierCompanyGuid = companyGuid;
				ownertransfer.SCACCode = this.GetStringValue("Carrier/SCAC", false, this.transactionNavigator);
			}
			else
			{
				GetCompany("Carrier", false, out name, out code, out companyGuid);
				transaction.CarrierID = name;
				transaction.CarrierCode = code;
				transaction.CarrierCompanyGuid = companyGuid;
				transaction.SCACCode = GetStringValue("Carrier/SCAC", false, this.transactionNavigator);
			}
		}

		virtual protected void SetSupplier()
		{
			string name, code;
			Guid companyGuid;
			GetCompany("Supplier", out name, out code, out companyGuid);
			transaction.SupplierID = name;
			transaction.SupplierCode = code;
			transaction.SupplierCompanyGuid = companyGuid;
		}

		virtual protected void SetReversedTransID()
		{
			transaction.ReversedTransID = GetStringValue("ReversedTransactionID", false, this.transactionNavigator);
		}

		virtual protected void SetReversalType()
		{
			this.transaction.ReversalType = this.GetStringValue("ReversalType", true, this.transactionNavigator);
		}

		virtual protected void SetLinkedDocumentNumber()
		{ transaction.LinkedDocumentNumber = GetStringValue("LinkedDocumentNumber", false, this.transactionNavigator); }

		virtual protected void SetTransRefID()
		{ transaction.TransRefID = GetStringValue("TransRefID", false, this.transactionNavigator); }

		virtual protected void SetNotes()
		{ transaction.Notes = GetStringValue("Notes", false, this.transactionNavigator); }

		virtual protected void SetPONumber()
		{ transaction.PONumber = GetStringValue("PONumber", false, this.transactionNavigator); }

		virtual protected void SetDriverIDNumber()
		{ transaction.DriverIDNumber = GetStringValue("DriverIDNumber", false, this.transactionNavigator); }

		virtual protected void SetTimeIn()
		{ transaction.TimeIn = this.GetNullableDateTime("TimeIn", false, this.transactionNavigator); }

		virtual protected void SetTimeOut()
		{ transaction.TimeOut = this.GetNullableDateTime("TimeOut", false, this.transactionNavigator); }

		virtual protected void SetTimeEnd()
		{ transaction.TimeEnd = this.GetNullableDateTime("TimeEnd", false, this.transactionNavigator); }

		virtual protected void SetConjoinedTransID()
		{ transaction.ConjoinedTransID = GetStringValue("ConjoinedTransID", true, this.transactionNavigator); }

		virtual protected void SetRequestedDeliveryDate()
		{ transaction.RequestedDeliveryDate = this.GetNullableDateTime("RequestedDeliveryDate", false, this.transactionNavigator); }

		virtual protected void SetLoadID()
		{ transaction.LoadID = GetStringValue("LoadID", false, this.transactionNavigator); }

		virtual protected void SetTransactionStatus()
		{ transaction.Status = this.GetEnumValue("TransactionStatus", TransactionStatus.Completed, false, this.transactionNavigator); }

		virtual protected void SetDeleteFlag()
		{ transaction.DeleteFlag = GetBoolValue("DeleteFlag", false, this.transactionNavigator); }

		virtual protected void SetOperatorID()
		{
			this.transaction.OperatorID = GetStringValue("Operator", false, this.transactionNavigator);
			Guid operatorPersonnelGuid = this.importProcessor.GetPersonMasterRecordGuid(this.transaction.Site, transaction.OperatorID);
			this.transaction.OperatorPersonnelGuid = operatorPersonnelGuid;
		}

		#endregion Transaction header

		#region LineItem

		virtual protected void SetLineItemSequenceNumber()
		{ this.lineItem.SequenceId = this.GetIntValue("SequenceNumber", true, this.lineItemNavigator); }

		virtual protected void SetLineItemProduct()
		{
			XPathNavigator productNavigator = this.lineItemNavigator.SelectSingleNode("ProductInfo");

			if (productNavigator != null)
			{
				string name, code, type;
				Guid productGuid;
				GetProduct(out name, out code, out type, out productGuid, productNavigator);
				lineItem.Product = name;
				lineItem.ProductCode = code;
				lineItem.ProductType = type;
				lineItem.ProductGuid = productGuid;

				lineItem.ProductPrice = this.GetNullableDouble("ProductPrice", false, productNavigator);
				lineItem.FreezePoint = this.GetNullableDoubleSIValue("FreezePoint", "FreezePointUnits", false, productNavigator);
			}
			else
			{
				this.transactionValidationResult.ErrorList.Add("ProductInfo must be provided");
			}
		}

		virtual protected void SetLineItemStatus()
		{
			lineItem.Status = this.GetEnumValue("Status", TransactionStatus.Completed, false, this.lineItemNavigator);
		}

		virtual protected void SetLineItemContractNumber()
		{ lineItem.ContractNumber = GetStringValue("ContractNumber", false, this.lineItemNavigator); }

		virtual protected void SetLineItemCLIN()
		{ lineItem.CLIN = GetStringValue("CLIN", false, this.lineItemNavigator); }

		virtual protected void SetLineItemArmNumber()
		{ this.lineItem.ArmNumber = this.GetNullableInt("ArmNumber", false, this.lineItemNavigator); }

		virtual protected void SetLineItemLineNumber()
		{ this.lineItem.LineNumber = this.GetNullableInt("LineNumber", false, this.lineItemNavigator); }

		virtual protected void SetLineItemOperatorID()
		{
			// OperatorName really is an ID
			lineItem.OperatorID = GetStringValue("OperatorName", false, this.lineItemNavigator);
			Guid operatorPersonnelGuid = this.importProcessor.GetPersonMasterRecordGuid(this.transaction.Site, lineItem.OperatorID);

			lineItem.OperatorPersonnelGuid = operatorPersonnelGuid;
		}

		virtual protected void SetLineItemBatchNumber()
		{ lineItem.BatchNumber = GetStringValue("BatchNumber", false, this.lineItemNavigator); }

		virtual protected void SetLineItemDocumentNumber()
		{ lineItem.DocumentNumber = GetStringValue("DocumentNumber", false, this.lineItemNavigator); }

		virtual protected void SetLineItemLineFill()
		{ lineItem.LineFill = this.GetNullableDoubleSIValue("LineFill", "LineFillUnits", false, this.lineItemNavigator); }

		virtual protected void SetLineItemBottomVolume()
		{ lineItem.BottomVolume = this.GetNullableDoubleSIValue("BottomVolume", "BottomVolumeUnits", false, this.lineItemNavigator); }

		virtual protected void SetLineItemNetCapacity()
		{ lineItem.NetCapacity = this.GetNullableDoubleSIValue("NetCapacity", "NetCapacityUnits", false, this.lineItemNavigator); }

		virtual protected void SetLineItemTankStatus()
		{ lineItem.TankStatus = GetStringValue("TankStatus", false, this.lineItemNavigator); }

		virtual protected void SetLineItemPit()
		{ lineItem.Pit = GetStringValue("Pit", false, this.lineItemNavigator); }

		virtual protected void SetLineItemRequestedDateTime()
		{ lineItem.RequestedDateTime = this.GetNullableDateTime("RequestedDateTime", false, this.lineItemNavigator); }

		virtual protected void SetLineItemDispatchedDateTime()
		{ lineItem.DispatchedDateTime = this.GetNullableDateTime("DispatchedDateTime", false, this.lineItemNavigator); }

		virtual protected void SetLineItemAcknowledgedDateTime()
		{ lineItem.AcknowledgedDateTime = this.GetNullableDateTime("AcknowledgedDateTime", false, this.lineItemNavigator); }

		virtual protected void SetLineItemOnLocationTime()
		{ lineItem.OnLocationTime = this.GetNullableDateTime("OnLocationTime", false, this.lineItemNavigator); }

		virtual protected void SetLineItemValidationDateTime()
		{ lineItem.ValidationDateTime = this.GetNullableDateTime("ValidationDateTime", false, this.lineItemNavigator); }

		virtual protected void SetLineItemCompletionDateTime()
		{ lineItem.CompletionDateTime = this.GetNullableDateTime("CompletionDateTime", false, this.lineItemNavigator); }

		virtual protected void SetLineItemReceiptVariance()
		{
			lineItem.LoadRackVariance = this.GetNullableDoubleSIValue("ReceiptVariance", "ReceiptVarianceUnits", false, this.lineItemNavigator);
		}

		virtual protected void SetLineItemDifferentialPressure()
		{
			lineItem.DifferentialPressure = this.GetNullableDoubleSIValue("DifferentialPressure", "DifferentialPressureUnits", false, this.lineItemNavigator);
		}

		virtual protected void SetLineItemLoadRackVariance()
		{
			lineItem.LoadRackVariance = this.GetNullableDoubleSIValue("LoadRackVariance", "LoadRackVarianceUnits", false, this.lineItemNavigator);
		}

		virtual protected void SetLineItemRequestedBy()
		{ lineItem.RequestedBy = GetStringValue("RequestedBy", false, this.lineItemNavigator); }

		virtual protected void SetLineItemTankID()
		{ lineItem.StorageLocationID = GetStringValue("StorageLocation", false, this.lineItemNavigator); }

		virtual protected void SetLineItemMeterID()
		{ lineItem.MeterID = GetStringValue("MeterID", false, this.lineItemNavigator); }

		virtual protected void SetLineItemAdditiveProfile()
		{ lineItem.AdditiveProfileID = GetStringValue("AdditiveProfile", false, this.lineItemNavigator); }

		virtual protected void SetLineItemPresetAmount()
		{ lineItem.PresetAmount = this.GetNullableDoubleSIValue("PresetAmount", "PresetAmountUnits", false, this.lineItemNavigator); }

		virtual protected void SetLineItemLocation()
		{
			lineItem.LoadingLocationID = GetStringValue("Location", false, this.transactionNavigator);
			Guid loadingLocationStationGuid = this.importProcessor.GetStationGuid(this.transaction.Site, lineItem.LoadingLocationID);

			if (loadingLocationStationGuid != Guid.Empty)
			{
				lineItem.LoadingLocationStationGuid = loadingLocationStationGuid;
			}
		}

		virtual protected void SetExpandedFsrValues()
		{
			if (UseExpandedFsrValues())
			{
				lineItem.MeterStartDateTime = this.GetNullableDateTime("MeterStartDateTime", false, this.lineItemNavigator);
				lineItem.MeterStopDateTime = this.GetNullableDateTime("MeterStopDateTime", false, this.lineItemNavigator);
				lineItem.MeterStartObtainedAutomaticallyFlag = this.GetNullableBool("MeterStartObtainedAutomaticallyFlag", false, this.lineItemNavigator);
				lineItem.MeterStopObtainedAutomaticallyFlag = this.GetNullableBool("MeterStopObtainedAutomaticallyFlag", false, this.lineItemNavigator);
				lineItem.DualFuelingModeFlag = this.GetNullableBool("DualFuelingModeFlag", false, this.lineItemNavigator);
				lineItem.FlowRate = this.GetNullableDouble("FlowRate", false, this.lineItemNavigator);
				lineItem.FuelCompressionFactor = this.GetNullableDouble("FuelCompressionFactor", false, this.lineItemNavigator);
				lineItem.EngineRunTime = this.GetNullableDouble("EngineRunTime", false, this.lineItemNavigator);
				lineItem.HydrantPressure = this.GetNullableDoubleSIValue("HydrantPressure", "DifferentialPressureUnits", false, this.lineItemNavigator);
				lineItem.MobileDeviceID = this.GetStringValue("MobileDeviceID", false, this.lineItemNavigator);
				lineItem.MobileDeviceGuid = this.GetNullableGuid("MobileDeviceGuid", false, this.lineItemNavigator);
				lineItem.DualFuelingPrimaryFlag = this.GetNullableBool("DualFuelingPrimaryFlag", false, this.lineItemNavigator);
				lineItem.TemperatureQualityStatus = this.GetStringValue("TemperatureQualityStatus", false, this.lineItemNavigator);
				lineItem.PartialFill = this.GetNullableBool("PartialFill", false, this.lineItemNavigator);
			}
		}

		protected bool UseExpandedFsrValues()
		{
			if (this.transaction.DeleteFlag || (TransactionTypes.T4_SecondaryDefuel != this.transaction.TransTypeID
				&& TransactionTypes.T5_PrimaryDisbursement != this.transaction.TransTypeID
				&& TransactionTypes.T7_FillStand != this.transaction.TransTypeID))
			{
				return false;
			}

			//Only add key once 
			if (!expandedFsrIsSupported.ContainsKey(this.transaction.TransTypeID))
			{
				expandedFsrIsSupported.Add(this.transaction.TransTypeID, false);
				//Check for specific Expanded Fsr fields 
				foreach (string col in expandedFsrCols)
				{
					if (!ColExists(col))
					{
						return false;
					}
				}
				expandedFsrIsSupported[this.transaction.TransTypeID] = true;
			}
			return expandedFsrIsSupported[this.transaction.TransTypeID];
		}

		protected bool ColExists(string name)
		{
			XPathNavigator col = this.lineItemNavigator.SelectSingleNode(name);
			if (col == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion LineItem

		#region SubLineItem

		virtual protected void SetSubLineItemProduct()
		{
			XPathNavigator productNavigator = this.sublineItemNavigator.SelectSingleNode("ProductInfo");

			if (productNavigator != null)
			{
				string name, code, type;
				Guid productGuid;
				GetProduct(out name, out code, out type, out productGuid, this.sublineItemNavigator);
				subLineItem.Product = name;
				subLineItem.ProductCode = code;
				subLineItem.ProductType = type;
				subLineItem.ProductGuid = productGuid;

				subLineItem.FreezePoint = this.GetNullableDoubleSIValue("FreezePoint", "FreezePointUnits", false, productNavigator);
			}
			else
			{
				this.transactionValidationResult.ErrorList.Add("ProductInfo must be provided");
			}
		}

		virtual protected void SetSubLineItemStatus()
		{
			subLineItem.Status = this.GetEnumValue("Status", TransactionStatus.Completed, false, this.sublineItemNavigator);
		}

		virtual protected void SetSubLineItemArmNumber()
		{ this.subLineItem.ArmNumber = this.GetNullableInt("ArmNumber", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemLineNumber()
		{ this.subLineItem.LineNumber = this.GetNullableInt("LineNumber", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemBatchNumber()
		{ subLineItem.BatchNumber = GetStringValue("BatchNumber", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemLineFill()
		{ subLineItem.LineFill = this.GetNullableDoubleSIValue("LineFill", "LineFillUnits", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemBottomVolume()
		{ subLineItem.BottomVolume = this.GetNullableDoubleSIValue("BottomVolume", "BottomVolumeUnits", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemNetCapacity()
		{ subLineItem.NetCapacity = this.GetNullableDoubleSIValue("NetCapacity", "NetCapacityUnits", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemTankStatus()
		{ subLineItem.TankStatus = GetStringValue("TankStatus", false, this.sublineItemNavigator); }


		virtual protected void SetSubLineItemDifferentialPressure()
		{
			subLineItem.DifferentialPressure = this.GetNullableDoubleSIValue("DifferentialPressure", "DifferentialPressureUnits", false, this.sublineItemNavigator);
		}

		virtual protected void SetSubLineItemDosageRate()
		{ subLineItem.DosageRate = this.GetNullableDouble("DosageRate", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemPresetAmount()
		{ subLineItem.PresetAmount = this.GetNullableDoubleSIValue("PresetAmount", "PresetAmountUnits", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemTankID()
		{ subLineItem.StorageLocationID = GetStringValue("StorageLocation", false, this.sublineItemNavigator); }

		virtual protected void SetSubLineItemMeterID()
		{ subLineItem.MeterID = GetStringValue("MeterID", false, this.sublineItemNavigator); }

		#endregion SubLineItem

		#endregion Public Helper Methods

		#region Protected Methods

		protected void GetCompany(string nodeName, out string name, out string code, out Guid companyGuid)
		{
			this.GetCompany(nodeName, true, out name, out code, out companyGuid);
		}

		protected void GetCompany(string nodeName, bool isRequired, out string name, out string code, out Guid companyGuid)
		{
			XPathNavigator companyNavigator = this.transactionNavigator.SelectSingleNode(nodeName);
			companyGuid = Guid.Empty;
			code = string.Empty;
			name = string.Empty;

			if (companyNavigator == null)
			{
				if (isRequired)
				{
					this.transactionValidationResult.ErrorList.Add(nodeName + " must be provided");
				}

				return;
			}

			string companyCode = GetStringValue("Code", false, companyNavigator);
			string companyName = GetStringValue("Name", false, companyNavigator);

			// If a translation has been defined for the name provided, use that value. 
			// Otherwise, continue processing the value as usual
			if (!string.IsNullOrEmpty(companyName))
			{
				Guid translatedGuid = this.importProcessor.GetTranslatedEntityGuid(companyName, FMAETranslationType.Company);

				if (translatedGuid != Guid.Empty)
				{
					CompanyClass company = this.importProcessor.GetCompanyByGuid(this.transaction.Site, translatedGuid);

					if (company != null)
					{
						code = company.Code;
						name = company.ID;
						companyGuid = translatedGuid;
						return;
					}
				}
			}

			name = companyName;
			code = companyCode;

			CompanyClass matchingCompany = null;

			if (!string.IsNullOrEmpty(companyName))
			{
				matchingCompany = this.importProcessor.GetCompanyByID(this.transaction.Site, companyName);

				if (matchingCompany != null)
				{
					code = matchingCompany.Code;

					// If the IDs match, we still want to use the one in the system
					// and not what was sent in the event that they differ in case (e.g. skytanking vs SkyTanking)
					name = matchingCompany.ID;
				}
			}
			else if (!string.IsNullOrEmpty(companyCode))
			{
				matchingCompany = this.importProcessor.GetCompanyByCode(this.transaction.Site, companyCode);

				if (matchingCompany != null)
				{
					name = matchingCompany.ID;
				}
			}

			if (matchingCompany != null)
			{
				companyGuid = matchingCompany.MasterRecordGuid;
			}

			if (isRequired && companyGuid == Guid.Empty)
			{
				this.transactionValidationResult.ErrorList.Add(nodeName + " not found. Code: \"" + companyCode + "\"  Name: \"" + companyName + "\".");
			}
		}

		protected void GetProduct(out string name, out string code, out string type, out Guid productGuid, XPathNavigator navigator)
		{
			this.GetProduct(true, out name, out code, out type, out productGuid, navigator);
		}

		protected void GetProduct(bool isRequired, out string name, out string code, out string type, out Guid productGuid, XPathNavigator navigator)
		{
			type = null;
			productGuid = Guid.Empty;

			string productCode = GetStringValue("ProductCode", false, navigator);
			string productName = GetStringValue("Product", false, navigator);

			// If a translation has been defined for the product provided, use that value. 
			// Otherwise, continue processing the value as usual
			if (!string.IsNullOrEmpty(productName))
			{
				Guid translatedGuid = this.importProcessor.GetTranslatedEntityGuid(productName, FMAETranslationType.Product);

				if (translatedGuid != Guid.Empty)
				{
					ProductClass product = this.importProcessor.GetProductByGuid(this.transaction.Site, translatedGuid);

					if (product != null)
					{
						code = product.Code;
						name = product.ID;
						productGuid = translatedGuid;
						type = ProductClass.ProductTypeID(product.ProductType);
						return;
					}
				}
			}

			ProductClass matchingProduct = null;
			name = productName;
			code = productCode;

			if (!string.IsNullOrEmpty(productName))
			{
				matchingProduct = this.importProcessor.GetProductByID(this.transaction.Site, productName);

				if (matchingProduct != null)
				{
					code = matchingProduct.Code;

					// If the IDs match, we still want to use the one in the system
					// and not what was sent in the event that they differ in case (e.g. ja vs JA)
					name = matchingProduct.ID;
				}
			}
			else if (!string.IsNullOrEmpty(productCode))
			{
				matchingProduct = this.importProcessor.GetProductByCode(this.transaction.Site, productCode);

				if (matchingProduct != null)
				{
					name = matchingProduct.ID;
				}
			}

			if (matchingProduct != null)
			{
				type = ProductClass.ProductTypeID(matchingProduct.ProductType);
				productGuid = matchingProduct.MasterRecordGuid;
			}

			if (isRequired && productGuid == Guid.Empty)
			{
				this.transactionValidationResult.ErrorList.Add("Invalid Product : Product Code: \"" + productCode + "\" Product Name: \"" + productName + "\".");
			}
		}

		#endregion Protected Methods
	}
}
