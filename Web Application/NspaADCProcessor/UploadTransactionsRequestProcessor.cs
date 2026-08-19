// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTransactionsRequestProcessor.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UploadTransactionsRequestProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nspa
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.ServiceModel;

	using ADC.Nspa.General;


	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;
    using Varec.CommonComponents.EngineeringUnitsLibrary;
    using Varec.CommonComponents.VolumeCorrection;

	public class UploadTransactionsRequestProcessor : RequestProcessorGenericBase<UploadTransactionsRequest, UploadTransactionsResponse>
	{
		private SiteClass currentSite;

		private SiteTimeConverter dateTimeConverter;

		private TimeZoneInfo timeZoneInfo;

		private Dictionary<Guid, ProductClass> productDictionary;

		private Dictionary<Guid, TransactionAliasClass> transactionAliasDictionary;

		private Dictionary<Guid, ApplicationStringClass> companyTypeDictionary;

		private Guid thirdPartyTransactionAliasGuid;

		private TransactionAliasClass thirdPartyTransactionAlias;

		private CompanyClass manager;

		private CompanyClass owner;

		private bool isEnterprise;

		internal UploadTransactionsRequestProcessor()
			: base("uploading transactions")
		{
			
		}

		protected override void ProcessCore()
		{
			this.currentSite =
				FMChannelHelper.MakeCall<ISites, SiteClass>(siteService => siteService.Get(this.Security, this.Security.SiteGuid, false, false, false));

			dateTimeConverter = new SiteTimeConverter(currentSite);
			timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(currentSite.TimeZone);

			productDictionary = new Dictionary<Guid, ProductClass>();

			transactionAliasDictionary = new Dictionary<Guid, TransactionAliasClass>();

			companyTypeDictionary = new Dictionary<Guid, ApplicationStringClass>();

			this.thirdPartyTransactionAliasGuid =
				FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
					transactionAliasService => transactionAliasService.GetMasterRecordGuid(this.Security, "Third Party Sale"));

			if (this.thirdPartyTransactionAliasGuid == null
			|| this.thirdPartyTransactionAliasGuid.IsEmpty())
			{
                Helper.NspaADCEventLog.WriteEntry("No Third Party Sale configured for site.", EventLogEntryType.Warning);
				throw new Exception("No Third Party Sale configured for site.");
			}

			this.thirdPartyTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
					transactionAliasService => transactionAliasService.GetWithoutAliasFields(this.Security, this.thirdPartyTransactionAliasGuid));

			transactionAliasDictionary.Add(this.thirdPartyTransactionAliasGuid, thirdPartyTransactionAlias);

			ValidateExchangeUserId(this.Security.UserID);

			isEnterprise = this.IsEnterprise();

			this.ProcessAllTransactions(this.Request);
		}

		private void ProcessAllTransactions(UploadTransactionsRequest request)
		{
			foreach (var transData in request.TransactionList)
			{
				var transactionResponse = this.ProcessSingleTransaction(transData);

				this.Response.TransactionStatusList.Add(transactionResponse);
			}

		}

		private UploadTransactionResponse ProcessSingleTransaction(AdcTransactionDoGenerated transactionData)
		{
			var transactionResponse = new UploadTransactionResponse();

			try
			{
				transactionResponse.TransactionId = transactionData.TransID;

				transactionResponse = this.CreateNspaTransaction(transactionData);
			}
			catch (FaultException<SaveTransactionsException> ex)
			{
				SaveTransactionsException saveTransactionsException = ex.Detail;

				string formattedErrorList = "";
				foreach (TransactionValidationResult validationResult in saveTransactionsException.Results)
				{
					foreach (string errorMsg in validationResult.ErrorList)
					{
						formattedErrorList += "\r\n" + errorMsg;
					}
				}


				string message = transactionData.TransID + " : " + formattedErrorList;
				AddError(transactionResponse, "ProcessSingleTransaction", message);
			}
			catch (Exception ex)
			{
				string message = transactionData.TransID + " - " + ex.Message;
				AddError(transactionResponse, "ProcessSingleTransaction", message);
			}

			return transactionResponse;
		}

		private void SaveNspaTransaction(TransactionDO transactionDO, ref UploadTransactionResponse response)
		{
			var saveTransactionsSR = new SaveTransactionsSR
			                         {
				                         Security = this.Security,
				                         CurrentSiteGuid = this.Security.SiteGuid,
				                         ConvertUnits = true
			                         };

			saveTransactionsSR.Transactions.Add(transactionDO);

			var lineItem = transactionDO.LineItems[0];

		    SaveTransactionsResultDO resultDO = null;
		    try
		    {
		        resultDO =
		            FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
		                accountingService => accountingService.SaveTransactions(saveTransactionsSR));

		    }
		    catch (Exception ex)
		    {
		        response.ErrorList.Add(string.Format("TransId {0}: {1}", transactionDO.TransID, ex.Message));

                var expandedError = ex as System.ServiceModel.FaultException<FMBusinessObjects.Exceptions.SaveTransactionsException>;

		        if (expandedError != null)
		        {
		            var detailErrors = expandedError.Detail.Results[0].ErrorList;


		            if (detailErrors != null && detailErrors.Count > 0)
		            {
		                foreach (string detailMessage in detailErrors)
		                {
		                    response.ErrorList.Add(detailMessage);
		                }
		            }
		        }
		    }
		

			if (resultDO != null)
			{
				if (resultDO.Results.Count > 0)
				{
					foreach (TransactionValidationResult result in resultDO.Results)
					{
						foreach (string error in result.ErrorList)
						{
							response.ErrorList.Add(error);
						}
						foreach (string error in result.WarningList)
						{
							response.ErrorList.Add(error);
						}
					}					
				}
			}

            bool status = (response.ErrorList.Count == 0);

            // if there are errors, check to see if it is a duplicate transaction error, 
            // and if so, check to esee if that transaction is the same as this one.
            // If the transaction is the same, then clear the previous error and add an error
            // string so client can determine the status correctly
            // 
            if (status == false)
            {
                if (response.ErrorList[0].Contains("Cannot insert duplicate key"))
                {
                    bool isDuplicateTransactionReallyTheSame = CheckIfDuplicateTransIdHasSameDetails(transactionDO);
                    if (isDuplicateTransactionReallyTheSame)
                    {
                        // tell the processor that the transaction insert result was good
                        status = true;

                        // clear the existing error
                        response.ErrorList.Clear();

                        // add the custom error message that the handheld client will look for
                        response.ErrorList.Add("Duplicate transaction, safe to delete.");

                        // add AlarmAndEventLog entry to make note of what happened
                        string logMessage =
                            string.Format(
                                "Duplicate TransId '{0}' skipped.",
                                transactionDO.TransID);
                        Nspa.Helper.LogFmEventADCInformation(Security, BaseRequest.ClientHostName, logMessage);
                        Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Information);
                    }
                }
            }
		}


		#region CreateNewTransaction

		private DateTimeOffset ConvertToDateTimeOffset(DateTime? dateTime)
		{
			if (!dateTime.HasValue)
			{
				return dateTimeConverter.Now();
			}

			if (dateTime.Value.Kind == DateTimeKind.Utc)
			{
				return dateTimeConverter.ConvertToSiteTime(new DateTimeOffset(dateTime.Value, new TimeSpan(0)));
			}

			return new DateTimeOffset(dateTime.Value, timeZoneInfo.GetUtcOffset(dateTime.Value));

		}

		private DateTimeOffset? ConvertToDateAsNullableDateTimeOffset(DateTime? originalValue)
		{
			if (!originalValue.HasValue)
			{
				return null;
			}

			var tempValue = originalValue.Value;

			return new DateTimeOffset(tempValue.Year, tempValue.Month, tempValue.Day, 0, 0, 0, 0, TimeSpan.Zero);
		}


		private UploadTransactionResponse CreateNspaTransaction(AdcTransactionDoGenerated transactionData)
		{
			var response = new UploadTransactionResponse { TransactionId = transactionData.TransID };

		    this.manager = NspaADCProcessor.FindCompanyWithDefault(
		        this.Security,
		        COMPANY_ROLE.MANAGER,
		        this.currentSite.UserData[2]);

		    this.owner = NspaADCProcessor.FindCompanyWithDefault(
		        this.Security,
		        COMPANY_ROLE.OWNER,
		        this.currentSite.UserData[3]);

            var transactionDO = new TransactionDO();

			var lineItemDO = new LineItemDO();
			transactionDO.LineItems.Add(lineItemDO);

			transactionDO.Alias = transactionData.AliasName;
			transactionDO.TransactionAliasGuid = GetNullableGuid(transactionData.TransactionAliasGuid);

			TransactionAliasClass transactionAlias=null;
			if (transactionData.TransactionAliasGuid.HasValue
			&& !transactionAliasDictionary.TryGetValue(transactionData.TransactionAliasGuid.Value, out transactionAlias))
			{
				transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(transactionAliasService => transactionAliasService.GetWithoutAliasFields(this.Security, transactionData.TransactionAliasGuid.Value));
				transactionAliasDictionary.Add(transactionData.TransactionAliasGuid.Value, transactionAlias);
			}

			if (transactionAlias != null)
			{
				transactionDO.TransTypeID = transactionAlias.TransTypeID;
			}

			// Set the transaction origin to ADC uploaded at enterprise or ADC uploaded at base level.
			// The system configuration setting (NspaEnterprise = TRUE) determines if the system is enterprise or base level.
			// This is used in the NSPA Transaction Detail to determine if the reverse or reverse update buttons
			// should be enabled.
			transactionDO.OriginApplication = this.isEnterprise ? TransactionOrigin.AdcUploadedAtEnterpriseLevel : TransactionOrigin.AdcUploadedAtBaseLevel;

			transactionDO.TransID = transactionData.TransID;
			transactionDO.Site = transactionData.Site;
			transactionDO.SiteGuid = GetNullableGuid(transactionData.SiteGuid);

            var logMessage = string.Format("{0} saving TransID {1} from client device {2}", Helper.WindowsEventLogModuleName, transactionDO.TransID, BaseRequest.ClientHostName);
            Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Information);

            PersonClass person = ValidatePersonnel(transactionData.OperatorID);
            if (person != null)
            {
                transactionDO.OperatorName = person.FullName;
            }
            transactionDO.OperatorID = transactionData.OperatorID;
            transactionDO.OperatorPersonnelGuid = GetNullableGuid(transactionData.OperatorPersonnelGuid);
            
            transactionDO.CreatedBy = this.Security.UserID;
			transactionDO.CreatedDate = dateTimeConverter.Now();
			transactionDO.UpdatedBy = this.Security.UserID;
			transactionDO.UpdatedDate = dateTimeConverter.Now();

            transactionDO.ManagerID = manager.ID;
            transactionDO.ManagerCode = manager.Code;
            transactionDO.ManagerCompanyGuid = manager.MasterRecordGuid;
            if (transactionDO.TransTypeID != TransactionTypes.T14_PhysicalInventory)
            {
                transactionDO.OwnerID = owner.ID;
                transactionDO.OwnerCode = owner.Code;
                transactionDO.OwnerCompanyGuid = owner.MasterRecordGuid;
            }
            transactionDO.ShipperID = transactionData.ShipperID;
			transactionDO.ShipperCode = transactionData.ShipperCode;
			transactionDO.ShipperCompanyGuid = GetNullableGuid(transactionData.ShipperCompanyGuid);
			transactionDO.BillToID = transactionData.BillToID;
			transactionDO.BillToCode = transactionData.BillToCode;
			transactionDO.BillToCompanyGuid = GetNullableGuid(transactionData.BillToCompanyGuid);

			ApplicationStringClass applicationString = null;
			if (transactionData.BillToCompanyTypeGuid.HasValue
			&& !companyTypeDictionary.TryGetValue(transactionData.BillToCompanyTypeGuid.Value, out applicationString))
			{
				applicationString = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(applicationStringService => applicationStringService.Get(this.Security, transactionData.BillToCompanyTypeGuid.Value));
				companyTypeDictionary.Add(transactionData.BillToCompanyTypeGuid.Value, applicationString);
			}

			if ((applicationString!=null) 
			      && applicationString.ID.Equals("Third Party") 
			      && (transactionDO.TransTypeID.Equals(TransactionTypes.T5_PrimaryDisbursement) 
	 		          || transactionDO.TransTypeID.Equals(TransactionTypes.T6_SecondaryDisbursement)))
			{
				transactionDO.Alias = this.thirdPartyTransactionAlias.ID;
				transactionDO.TransactionAliasGuid = this.thirdPartyTransactionAliasGuid;
				transactionDO.TransTypeID = this.thirdPartyTransactionAlias.TransTypeID;
			}

            // TODO: see if this causes date format issue in document number to go away
			//transactionDO.DocumentNumber = this.GetDocumentNumber(transactionDO.TransTypeID);
			transactionDO.AssociatedDocumentNumber = transactionData.AssociatedDocNumber;

			transactionDO.ShipToID = transactionData.ShipToID;
			transactionDO.ShipToCode = transactionData.ShipToCode;
			transactionDO.ShipToCompanyGuid = GetNullableGuid(transactionData.ShipToCompanyGuid);
			transactionDO.SupplierID = transactionData.SupplierID;
			transactionDO.SupplierCode = transactionData.SupplierCode;
			transactionDO.SupplierCompanyGuid = GetNullableGuid(transactionData.SupplierCompanyGuid);
			transactionDO.CarrierID = transactionData.CarrierID;
			transactionDO.CarrierCode = transactionData.CarrierCode;
			transactionDO.CarrierCompanyGuid = GetNullableGuid(transactionData.CarrierCompanyGuid);

			transactionDO.FuelCardID = transactionData.FuelCardID;
			transactionDO.FuelCardGuid = GetNullableGuid(transactionData.FuelCardGuid);

			transactionDO.PaymentInfo.CreditCardNumber = transactionData.CardNumber;
		    DateTimeOffset? cardExpirationDateTimeOffset = this.ConvertToDateAsNullableDateTimeOffset(transactionData.CardExpiration);
		    if (cardExpirationDateTimeOffset != null)
		    {
		        transactionDO.PaymentInfo.CreditCardExpiration = TimeConverter.ToEndOfDay(cardExpirationDateTimeOffset.Value).LocalDateTime;
		    }
		    transactionDO.PaymentInfo.CreditCardType = transactionData.CardType;	

			transactionDO.GateID = transactionData.GateID;
			transactionDO.GateGuid = GetNullableGuid(transactionData.GateGuid);

			transactionDO.RouteInfo.FinalStationIATAID = transactionData.FinalStationIATAID;
			transactionDO.RouteInfo.FinalStationIATAGuid = GetNullableGuid(transactionData.FinalStationIATAGuid);

			transactionDO.InventoryDate = this.ConvertToDateTimeOffset(transactionData.InventoryDate).Date;

			transactionDO.SourceEQ1.RegistrationID = transactionData.SourceRegistrationID1;
			transactionDO.SourceEQ1.EquipmentGuid = GetNullableGuid(transactionData.Source1EquipmentGuid);
			transactionDO.SourceEQ1.EquipmentType = transactionData.SourceEquipmentType1;

			transactionDO.DestinationEQ1.RegistrationID = transactionData.DestinationRegistrationID1;
			transactionDO.DestinationEQ1.EquipmentGuid = GetNullableGuid(transactionData.Destination1EquipmentGuid);
			transactionDO.DestinationEQ1.EquipmentType = transactionData.DestinationEquipmentType1;

			this.HandleNewEquipment(transactionDO.DestinationEQ1, "Destination");
			this.HandleNewEquipment(transactionDO.SourceEQ1, "Source");

			FillLineItemEquiment(transactionDO, lineItemDO);

			transactionDO.TransactionDateTime = this.ConvertToDateTimeOffset(transactionData.TransDateTime);
			if (transactionDO.TransTypeID == TransactionTypes.T9_Request)
			{
				transactionDO.Date01 = TimeConverter.ToDate(transactionDO.TransactionDateTime.Value);
			}
			transactionDO.Date03 = transactionDO.TransactionDateTime;

			transactionDO.UserData1 = transactionData.UserData1;
			transactionDO.UserData2 = transactionData.UserData2;
			transactionDO.UserData4 = transactionData.UserData4;
			transactionDO.UserData5 = transactionData.UserData5;
			transactionDO.UserData7 = transactionData.UserData7;
			transactionDO.UserData8 = transactionData.UserData8;
			transactionDO.UserData9 = transactionData.UserData9;

            if (transactionDO.TransTypeID != TransactionTypes.T14_PhysicalInventory &&
                transactionDO.TransTypeID != TransactionTypes.T12_InventoryNotAffected)
            {
                transactionDO.Number02 = transactionData.Number02;
            }


			transactionDO.PONumber = transactionData.PONumber;
			transactionDO.ShippingDocumentNumber = transactionData.ShippingDocumentNumber;

            if (transactionDO.TransTypeID != TransactionTypes.T14_PhysicalInventory &&
                transactionDO.TransTypeID != TransactionTypes.T12_InventoryNotAffected)
            {
                transactionDO.Flag01 = (transactionData.Flag01.HasValue ? transactionData.Flag01.Value : false);
            }
            else
            {
                transactionDO.Flag01 = false;
            }

			transactionDO.Notes = transactionData.Note_Notes;

			ProductClass product=null;
			if (transactionData.Line_ProductGuid.HasValue
			&& !productDictionary.TryGetValue(transactionData.Line_ProductGuid.Value, out product))
			{
				product = FMChannelHelper.MakeCall<IProducts, ProductClass>(productService => productService.GetByInfoAuthorizedCompanies(this.Security, transactionData.Line_ProductGuid.Value,true,false));
				productDictionary.Add(transactionData.Line_ProductGuid.Value, product);
			}

			lineItemDO.Product = transactionData.Line_Product;
			lineItemDO.ProductGuid = GetNullableGuid(transactionData.Line_ProductGuid);

			if (product != null)
			{
				lineItemDO.ProductCode = product.Code;
				lineItemDO.ProductType = ProductClass.ProductTypeID(product.ProductType);
			}

		    lineItemDO.Density = transactionData.Line_Density;
		    lineItemDO.DensityUnits = (EngineeringUnit)transactionData.Line_DensityUnitsIndex;
		    lineItemDO.Quantity.GrossInventoryChange = GetNullableDouble(transactionData.Line_GrossQuantity);
		    lineItemDO.Quantity.NetInventoryChange = GetNullableDouble(transactionData.Line_NetQuantity);
		    lineItemDO.VCF = GetNullableDouble(transactionData.Line_Vcf);
		    lineItemDO.VolumeUnits = (EngineeringUnit)transactionData.Line_EngineeringUnitsIndex;
		    lineItemDO.Temperature = transactionData.Line_Temperature;
		    lineItemDO.TemperatureUnits = (EngineeringUnit)transactionData.Line_TemperatureUnitsIndex;

            if (transactionDO.TransTypeID == TransactionTypes.T14_PhysicalInventory ||
                transactionDO.TransTypeID == TransactionTypes.T12_InventoryNotAffected)
            {
                lineItemDO.BrokenBlend = false;
                lineItemDO.ImproperAdditization = false;
            }

            lineItemDO.StorageLocationID = transactionData.Line_StorageLocationID;
		    lineItemDO.StorageLocationTankGuid = GetNullableGuid(transactionData.Line_StorageLocationTankGuid);
		    lineItemDO.MeterID = transactionData.Line_MeterID;
		    lineItemDO.MeterGuid = GetNullableGuid(transactionData.Line_MeterGuid);
		    lineItemDO.Quantity.MassInventoryChange = GetNullableDouble(transactionData.Line_MassQuantity);
		    lineItemDO.MassUnits = (EngineeringUnit)transactionData.Line_MassUnitsIndex;
            if (transactionDO.TransTypeID == TransactionTypes.T12_InventoryNotAffected)
            {
                lineItemDO.MeterReading.MeterStop = transactionData.Number02;
            }
            else
            {
                lineItemDO.MeterReading.MeterStart = transactionData.Line_MeterStart;
                lineItemDO.MeterReading.MeterStop = transactionData.Line_MeterStop;
            }


			if (product != null)
			{
				if (lineItemDO.Density == null)
				{
					var dSiDensity = product._StandardDensity.SIValue;
					lineItemDO.Density = this.ConvertUnits(dSiDensity, EngineeringUnit.FmdKgM3, lineItemDO.DensityUnits);
				}
			}

			if (lineItemDO.VCF == 0)
			{
				if (product != null && lineItemDO.Temperature.HasValue)
				{
					lineItemDO.VCF = Vcf.CalculateVcf(
                        (ECorrectionTypeMajor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
                        (ECorrectionTypeMinor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
                        lineItemDO.Temperature.Value,
                        lineItemDO.TemperatureUnits,
                        product._VcfModuleSettings.BaseTemperature.Value,
                        ((EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? currentSite.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType),
                        lineItemDO.Density.Value,
                        lineItemDO.DensityUnits,
                        0.0,
                        currentSite.PressureUnits,
                        product._VcfModuleSettings.AlternateTemperature.Value,
                        ((EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? currentSite.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType),
                        product._VcfModuleSettings.AlternateBasePressure.Value,
                        ((EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? currentSite.PressureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType),
                        new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
                }
				else
				{
					lineItemDO.VCF = 1;
				}
			}

			if (lineItemDO.Quantity.NetInventoryChange == 0)
			{
				// The following is for the current situation.
				// When we work on Gross/Net Vol Correction, this needs to be updated most likely.
				if (transactionDO.TransTypeID == TransactionTypes.T9_Request)
				{
					if (lineItemDO.VCF == 0)
					{
						lineItemDO.VCF = 1;
					}
					lineItemDO.Quantity.GrossInventoryChange = lineItemDO.Quantity.NetInventoryChange / lineItemDO.VCF.Value;
				}
				else
				{
					lineItemDO.Quantity.NetInventoryChange = lineItemDO.VCF.Value * lineItemDO.Quantity.GrossInventoryChange;
				}
			}


			if (!(transactionDO.TransTypeID == TransactionTypes.T8_Receipt
				|| transactionDO.TransTypeID == TransactionTypes.T9_Request
				|| transactionDO.TransTypeID == TransactionTypes.T25_Shipment
				))
			{
				if (product != null)
					
					{
						SIDouble siDensity = new SIDouble();
						siDensity.Units = lineItemDO.DensityUnits;
						siDensity.Value = lineItemDO.Density.Value;

						SIDouble siNetVolume = new SIDouble();
						siNetVolume.Units = lineItemDO.VolumeUnits;
						siNetVolume.Value = lineItemDO.Quantity.NetInventoryChange;

						SIDouble siMass = new SIDouble();
						siMass.Units = currentSite.MassUnits;
						siMass.SIValue = siNetVolume.SIValue * siDensity.SIValue;

						lineItemDO.Quantity.MassInventoryChange = siMass.Value;
				}
				else
				{
					lineItemDO.Quantity.MassInventoryChange = 0.0;
				}
			}

			if (transactionDO.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
			    || transactionDO.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
				|| transactionDO.TransTypeID == TransactionTypes.T25_Shipment)
			{
				lineItemDO.Quantity.GrossInventoryChange *= -1;
				lineItemDO.Quantity.NetInventoryChange *= -1;
				lineItemDO.Quantity.MassInventoryChange *= -1;
			}

			this.SaveNspaTransaction(transactionDO, ref response);
            response.Success = (response.ErrorList.Count == 0);

		    if (response.Success == false)
		    {
                logMessage = string.Format("Errors saving TransID {0} from client device {1}:", transactionDO.TransID, BaseRequest.ClientHostName);
		        logMessage += string.Join(Environment.NewLine, response.ErrorList);
                Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Error);
		    }

			return response;
		}

        private bool CheckIfDuplicateTransIdHasSameDetails(TransactionDO importTransaction)
        {
            var sr = new TransactionSR();
            sr.Security = this.Security;
            sr.TransID = importTransaction.TransID;
            var existingTransaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));

            bool areTransactionsSame = (
                    importTransaction.OriginApplication == existingTransaction.OriginApplication &&
                    importTransaction.BillToID == existingTransaction.BillToID &&
                    importTransaction.LineItems[0].MeterStart == existingTransaction.LineItems[0].MeterStart &&
                    importTransaction.LineItems[0].MeterStop == existingTransaction.LineItems[0].MeterStop
                );

            return areTransactionsSame;
        }

	    private static Guid GetNullableGuid(Guid? sourceValue)
		{
			return sourceValue.HasValue ? sourceValue.Value : Guid.Empty;
		}

		private static double GetNullableDouble(Double? sourceValue)
		{
			return sourceValue.HasValue ? sourceValue.Value : 0.0;
		}

		private void FillLineItemEquiment(TransactionDO transaction, LineItemDO lineItem)
		{
			switch (transaction.TransTypeID)
			{
				case TransactionTypes.T5_PrimaryDisbursement:
				case TransactionTypes.T6_SecondaryDisbursement:
					CopyEquipment(transaction.SourceEQ1, lineItem.SourceEQ);
					break;
				case TransactionTypes.T4_SecondaryDefuel:
					CopyEquipment(transaction.DestinationEQ1, lineItem.DestinationEQ);
					break;
			}
		}

		/// <summary>
		/// Copies the equipment.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="destination">The destination.</param>
		private static void CopyEquipment(EquipmentDO source, EquipmentDO destination)
		{
			destination.RegistrationID = source.RegistrationID;
			destination.EquipmentGuid = source.EquipmentGuid;
			destination.EquipmentType = source.EquipmentType;
		}

        /// <summary>
        /// If the destination Equipment is a new equipment(guid=empty),
        /// that equipment could have been created since last sync, if so, let's look it up and back fill the Guid
        /// </summary>
        /// <param name="theEquipment"></param>
        /// <param name="equipmentPurpose"></param>
        private void HandleNewEquipment(EquipmentDO theEquipment, string equipmentPurpose)
		{
            // no equipment was specified, so do nothing. (eliminate useless log entry)
            if (string.IsNullOrEmpty(theEquipment.RegistrationID))
            {
                return;
            }

			// Is this a new equipment
			if (theEquipment.EquipmentGuid.Equals(Guid.Empty))
			{
				var logMessage = string.Format(
								"{0} equipment {1} with type {2} has blank Guid.",
                                equipmentPurpose,
								theEquipment.RegistrationID,
								theEquipment.EquipmentType);
                Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Information);

				var equipmentGuid = 
					FMChannelHelper.MakeCall<IEquipments, Guid>(
						equipmentService => equipmentService.GetIdentityGuid(this.Security, theEquipment.RegistrationID));

				// Found new equipment exists on the server?
				if (!equipmentGuid.Equals(Guid.Empty))
				{
					var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
							equipmentService => equipmentService.Get(this.Security, equipmentGuid));

					if (equipment == null)
					{
						Helper.NspaADCEventLog.WriteEntry(
							Helper.WindowsEventLogModuleName + " unexpected error, found equipment Guid " + equipmentGuid.ToString() + " but can't load the equipment",
							EventLogEntryType.Error);
					}
					else
					{
						var equipmentType = EquipmentTypeClass.TypeID(equipment.Type);

						// We should only compare with equipment.TypeClass
						// due to a bug in ADC version 25, we're comparing both original ID and data-dictionarized ID
						// so we can accomodate both SU5 and SU6.
						// This should be removed in SU7
						var dataDictionzarizedEquipmentType = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
							dataDictionaryService => dataDictionaryService.Get(this.Security.SiteGuid, equipmentType ));
						// verify they are the same equipment type
					    if (   string.Compare(equipmentType, theEquipment.EquipmentType, StringComparison.CurrentCultureIgnoreCase) == 0
					        || string.Compare(dataDictionzarizedEquipmentType,theEquipment.EquipmentType,StringComparison.CurrentCultureIgnoreCase) == 0)
					    {
					        theEquipment.EquipmentGuid = equipment.MasterRecordGuid;
					        theEquipment.EquipmentType = equipmentType;

					        logMessage =
					            string.Format(
					                "Destination equipment {0} with type {1} exists and ADC Processor has replaced the blank GUID with the new GUID {2}.",
					                theEquipment.RegistrationID,
					                equipmentType,
					                theEquipment.EquipmentGuid);
                            Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Information);
					    }
					    else
					    {
					        logMessage = string.Format(
					            "Found equipment {0} with type {1}, but it doesn't match the upload type {2}",
					            theEquipment.RegistrationID,
					            equipmentType,
					            theEquipment.EquipmentType);
                            Helper.NspaADCEventLog.WriteEntry(logMessage, EventLogEntryType.Error);
					    }
					}
				}
			}
		}

		private string GetDocumentNumber(TransactionTypes typeID)
		{
			var documentNumber = string.Empty;
			
			for (var retry = 0; retry < 3; retry++)
			{
				try
				{
					FMChannelHelper.MakeCall<ISites>(
						siteService => documentNumber = siteService.GetNextDocumentNumber(Security, DOCUMENT_TYPE.TRANSACTION, Security.SiteGuid));
					break;
				}
				catch (Exception ex /*SitesError*/)
				{
                    Helper.NspaADCEventLog.WriteEntry(
                        Helper.WindowsEventLogModuleName + " error in GetDocumnetNumber for type " + typeID.ToString() + " - "
						+ ex.Message,
						EventLogEntryType.Error);
					//EventLogger.WriteEntry(SitesError.Message, EventLogEntryType.Error);
					if (retry == 2)
					{
						throw new Exception("GetNextDocumentNumber failed after 3 attempts.");
					}
				}
			}

			return documentNumber;
		}

		/// <summary>
		/// This method will return true if the configuration is set to NSPA Enterprise.
		/// </summary>
		/// <returns>
		/// Returns true if set to enterprise, otherwise it return false.<see cref="bool"/>.
		/// </returns>
		private bool IsEnterprise()
		{
			return FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsNspaEnterpriseKey());
		} 

        private PersonClass ValidatePersonnel(string operatorId)
        {
            PersonClass person = null;
            {
                person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(personService => personService.GetByID(this.Security, operatorId));

                if (person == null || person.IdentityGuid == Guid.Empty)
                {
                    var errorMessage = string.Format("Personnel Id '{0}' is not configured.", operatorId);
                    throw new ArgumentOutOfRangeException(errorMessage);
                }

            }

            return person;
        }
		#endregion
	}
}
