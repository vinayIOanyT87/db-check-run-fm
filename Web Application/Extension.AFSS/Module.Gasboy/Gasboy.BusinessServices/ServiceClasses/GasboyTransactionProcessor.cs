// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyTransactionProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Implements operations to support database operations for External Stations
// like adding, modifying, or deleting a record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.ServiceModel;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
    using FMCore;
	using FuelsManager.Afss.BusinessObjects.Constants;
	using FuelsManager.Afss.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.BusinessServices.Repository;

	/// <summary>
	/// Implements operations to support database operations for External Stations
	/// like adding, modifying, or deleting a record.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GasboyTransactionProcessor : IGasboyTransactionProcessor
	{
		/// <summary>
		/// Allows database access.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		/// <summary>
		/// Used to transform raw external station transaction data into an object
		/// </summary>
		private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(GasboyStationTransaction));

		#region Transaction Processing Methods

		/// <summary>
		/// Attempt to process new transactions that have been received from a Gasboy Station
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="station">The station that originated the transactions</param>
		/// <param name="gasboyTransactions">The transactions to process</param>
		/// <returns>Any errors that may have been encountered when attempting to reprocess the transactions</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string ImportTransactions(SecurityClass security, GasboyStation station, List<GasboyStationTransaction> gasboyTransactions)
		{
			List<GasboyStationTransaction> gasboySourceTransactions = new List<GasboyStationTransaction>();
			GasboyEvents gasboyEvents = new GasboyEvents();

			string results = string.Empty;

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			var sr = new SaveTransactionsSR { Security = security, CurrentSiteGuid = security.SiteGuid, ConvertUnits = false, BypassValidation = true };

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(siteChannel => siteChannel.Get(security, security.SiteGuid, false, false, true));

			var gasboyStations = new GasboyStations();
			GasboyStationGeneralConfiguration generalConfigurationForSite = gasboyStations.GetGeneralConfigurationBySiteGuid(security, security.SiteGuid);

			if (generalConfigurationForSite.RetailSaleTransactionAliasGuid == null 
				|| generalConfigurationForSite.RetailSaleTransactionAliasGuid == Guid.Empty
				|| string.IsNullOrEmpty(generalConfigurationForSite.RetailSaleTransactionAliasName))
			{
				// LOG AN ERROR - UNABLE TO IDENTIFY / LOCATE TRANSACTION ALIAS
				throw new Exception("No transaction alias has been configured in the General Configuration for this site");
			}

			CompanyCollectionClass companyManagerList =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateByRole(security, COMPANY_ROLE.MANAGER, false, false));
			CompanyCollectionClass companyOwnerList =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateByRole(security, COMPANY_ROLE.OWNER, false, false));

			if ((companyManagerList.Count != 1) || (companyOwnerList.Count != 1))
			{
				throw new Exception("Could not find a Manager or Owner.");
			}
			else
			{
				string managerID = companyManagerList[0].ID;
				Guid managerGuid = companyManagerList[0].MasterRecordGuid;

				string ownerID = companyOwnerList[0].ID;
				Guid ownerGuid = companyOwnerList[0].MasterRecordGuid;

				List<GasboyStationTransaction> duplicateTransactions = this.FindDuplicateTransactions(
					security,
					station,
					gasboyTransactions);

				List<TransactionDO> transactionsToSave = new List<TransactionDO>();

				foreach (GasboyStationTransaction gasboyTransaction in gasboyTransactions)
				{
					var localTransaction = gasboyTransaction;

					if ((from t in duplicateTransactions where t.ID.ToLower().Equals(localTransaction.ID.ToLower()) select t).Any())
					{
						// Record duplicate transaction.

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
							alarmAndEventChannel =>
							{
								alarmAndEventChannel.Add(security
										,
										gasboyEvents.GasboyDuplicateTransactionRejectedEvent(Convert.ToString(gasboyTransaction.ID), station.ID));
							});

						continue;
					}

					GasboyStationProductMapping productMapping = this.GetMappedProduct(
						security,
						station,
						gasboyTransaction.ProductName);

					if (null == productMapping)
					{
						var error = new GasboyStationTransactionError
						{
							ErrorMessage = string.Format("Missing Gasboy Islander Product Mapping for {0}.", gasboyTransaction.ProductName),
							ExternalStationTransactionGuid = localTransaction.IdentityGuid
						};

						localTransaction.TransactionErrors.Add(error);

						localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
						localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.AutoRetry;
					}
					else
					{
						var productID = productMapping.FuelsManagerProductID;
						var productGuid = productMapping.FuelsManagerProductMasterRecordGuid;

						// Get the rest of the FuelsManager Product record since we'll need the Code.
						var product = FMChannelHelper.MakeCall<IProducts, ProductClass>(productChannel => productChannel.Get(security, productGuid));

						if (null == product)
						{
							var error = new GasboyStationTransactionError
							            {
								            ErrorMessage =
									            string.Format(
										            "Unable to load FuelsManager Product {0}.",
										            productID),
								            ExternalStationTransactionGuid = localTransaction.IdentityGuid
							            };

							localTransaction.TransactionErrors.Add(error);

							localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
							localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.AutoRetry;
						}
						else
						{
							var newFuelsManagerTransaction = new TransactionDO
							                                 {
																			TransID = FuelsManagerId.NewId(),
								                                 Alias =
									                                 generalConfigurationForSite.RetailSaleTransactionAliasName,
								                                 TransactionAliasGuid =
									                                 generalConfigurationForSite.RetailSaleTransactionAliasGuid
									                                 .GetValueOrDefault(),
								//DestinationRegistrationID1 = gasboyTransaction.MeanName,
								TransactionDateTime =TypeHelper.ConvertDateTimeOffset(gasboyTransaction.TransactionTimeStamp),
								//TransRefID = gasboyTransaction.ID,
								//GateID = station.ID,
																			InventoryDate =
																				TypeHelper.ConvertDateTimeNoNull(
																					gasboyTransaction.TransactionTimeStamp),
								TimeOut = TypeHelper.ConvertDateTimeNoNull(gasboyTransaction.TransactionTimeStamp),
								ManagerID = managerID,
								                                 ManagerCode = managerID,
								                                 ManagerCompanyGuid = managerGuid,
								                                 OwnerID = ownerID,
								                                 OwnerCode = ownerID,
								                                 OwnerCompanyGuid = ownerGuid,
								                                 SiteGuid = security.SiteGuid,
								                                 Site = security.SiteID,
								                                 SupplierID = station.BillingID,
								                                 SupplierCode = station.BillingID,
								                                 VolumeUnits = site.VolumeUnits,
																 //UserData1 = gasboyTransaction.ID,
								                                 //UserData2 = gasboyTransaction.TransactionType,
								                                 //UserData3 = gasboyTransaction.ExternalAuthorizationNumber,
								                                 UserData4 = Convert.ToString(Convert.ToInt64(gasboyTransaction.ID) - 300000000),
								                                 //UserData5 = gasboyTransaction.NozzleID,
								                                 //UserData6 = string.Empty, 
								                                 //UserData7 = gasboyTransaction.Pump,
								                                 //UserData8 = gasboyTransaction.PumpID,
								                                 //UserData9 = gasboyTransaction.ProxyDeviceID,
								         //                        UserData10 = gasboyTransaction.ShiftID,
								         //                        UserData11 = gasboyTransaction.Tag,
																 //UserData12 = gasboyTransaction.FuelingVehiclePlate,
								         //                        UserData14 = gasboyTransaction.MeanID,
								         //                        UserData15 = gasboyTransaction.HoseNumber,
								         //                        UserData16 = gasboyTransaction.FleetID,
								         //                        UserData17 = gasboyTransaction.DriverMeanID,
								         //                        UserData18 = gasboyTransaction.DriverPlate,
								         //                        UserData19 = gasboyTransaction.DriverTag,
							                                 };


							if (newFuelsManagerTransaction.InventoryDate <= site._AdministrativeLockDate.Value)
							{
								var error = new GasboyStationTransactionError
								            {
									            ErrorMessage =
										            "Inventory date must be after the Administrative Lock Date.",
									            ExternalStationTransactionGuid = localTransaction.IdentityGuid
								            };

								localTransaction.TransactionErrors.Add(error);

								localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
								localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.Pending;
							}

							if (!security.HasRight(RIGHT.CONFIGURE_ACCOUNTING) && !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
							    && newFuelsManagerTransaction.InventoryDate <= site._OperationalLockDate.Value)
							{
								var error = new GasboyStationTransactionError
								            {
									            ErrorMessage =
										            "Inventory date must be after the Operational Lock Date.",
									            ExternalStationTransactionGuid = localTransaction.IdentityGuid
								            };

								localTransaction.TransactionErrors.Add(error);

								localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
								localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.Pending;
							}

							newFuelsManagerTransaction.PaymentInfo = new PaymentInfoDO()
																	{
																		CreditCardNumber = gasboyTransaction.Tag
																	};

							LineItemDO newLineItem = new LineItemDO
							                         {
								                         TransactionLineItemGuid = Guid.NewGuid(),
								                         //Density = Convert.ToDouble(gasboyTransaction.Density),
								                         //EngineRunTime = Convert.ToDouble(gasboyTransaction.EngineHours),
								                         GrossQuantityReceived = Convert.ToDouble(gasboyTransaction.Quantity),
								                         NetQuantityReceived = Convert.ToDouble(gasboyTransaction.Quantity),
								                         Odometer = Convert.ToDouble(gasboyTransaction.Odometer),
								                         Product = productID,
								                         ProductGuid = productGuid,
														       ProductCode = product.Code,
																 UserData23 = gasboyTransaction.DriverName,
								                         //Number01 = Convert.ToDouble(gasboyTransaction.PricePerVolume),
								                         //StorageLocationID = gasboyTransaction.TankName,
								                         //Temperature = Convert.ToDouble(gasboyTransaction.Temperature)

							                         };

							newLineItem.Quantity.Gross = newLineItem.GrossQuantityReceived;
							newLineItem.Quantity.Net = newLineItem.NetQuantityReceived;

							newLineItem.VolumeUnits = site.VolumeUnits;

							newLineItem.DestinationEQ.RegistrationID = gasboyTransaction.MeanName;

							newFuelsManagerTransaction.LineItems.Add(newLineItem);

							transactionsToSave.Add(newFuelsManagerTransaction);
						}
					}

					//  Regardless of whether it validates or not, we need to record the original Gasboy transaction.
					gasboySourceTransactions.Add(localTransaction);
				}

				foreach (TransactionDO transactionToValidate in transactionsToSave)
				{
					TransactionDO validate = transactionToValidate;
					TransactionValidationResult validationResult;
					Guid securityUserGuid = security.UserGuid;

					try
					{
						// Set security.UserGuid = Guid.empty before validating to bypass the authorized companies check.
						security.UserGuid = Guid.Empty;
						validationResult =
							FMChannelHelper.MakeCall<ITransactionValidator, TransactionValidationResult>(
								transactionValidatorChannel => transactionValidatorChannel.ValidateTransaction(security, validate));
					}
					finally
					{
						security.UserGuid = securityUserGuid;
					}

					if (validationResult.IsValid)
					{
						sr.ConvertUnits = true;
						sr.Transactions.Add(transactionToValidate);
					}
					else
					{
						// Update the gasboy station transaction with any FuelsManager Transaction Validation errors.
						GasboyStationTransaction matchingGasboyStationTransaction =
							gasboyTransactions.Find(stationTransaction => stationTransaction.ID == transactionToValidate.TransRefID);

						if (matchingGasboyStationTransaction == null)
						{
							// We found the transaction the SaveTransactionsProcessor is complaining about in the list of transactions we tried to save,
							// but we can't find the corresponding GasboyTransaction.
							// This shouldn't ever happen.
							throw new Exception("Unable to find the Gasboy transaction with ID = " + transactionToValidate.TransRefID);
						}

						foreach (string error in validationResult.ErrorList)
						{
							matchingGasboyStationTransaction.TransactionErrors.Add(
								new GasboyStationTransactionError { ErrorMessage = error });
						}

						results += string.Join("\r\n", validationResult.ErrorList.Cast<string>().ToList());
					}
				}

				try
				{
					// Save the source transaction to the database.
					if (gasboySourceTransactions.Count > 0)
					{
						this.AddTransactions(security, gasboySourceTransactions);
					}

					// Save the FuelsManager Transaction to the database
					if (sr.Transactions.Count > 0)
					{
						FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
							saveTransactionsChannel => saveTransactionsChannel.SaveTransactions(sr));
					}

					results = "Success!";

				}
				catch (FaultException<SaveTransactionsException> fex)
				{
					// this error is unexpected, and because of the way the SaveTransactionsProcessor works, even if the error is only for one record the 
					// entire batch of transactions will not be saved.
					// At this point I think we don't want to notify the station that we received the transactions - we should let them be reprocessed.
					throw new Exception("Unexpected error when saving transactions: " + fex.Detail.Message);
				}
			}

			return results;
		}

		private List<GasboyStationTransaction> FindDuplicateTransactions(
			SecurityClass security,
			GasboyStation station, 
			List<GasboyStationTransaction> gasboyTransactions)
		{
			return GasboyStationTransactionDBI.GetDuplicateTransactions(security, station, gasboyTransactions);
		}

		#endregion Transaction Processing Methods

		#region Failed Transaction Methods

		/// <summary>
		/// Attempt to reprocess a transaction that has failed after being reviewed and possibly edited by a user
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="failedTransaction">The failed transaction to reprocess</param>
		/// <returns>Any errors that may have been encountered when attempting to reprocess the transaction</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string ProcessCorrectedTransaction(SecurityClass security, GasboyStationTransaction failedTransaction)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			// TODO: Send the transaction off to the external station service for processing. If successful, delete the failed transaction record         
			return this.PurgeFailedTransaction(security, failedTransaction);
			//return "Transaction succesfully processed";
		}

		private string PurgeFailedTransaction(SecurityClass security, GasboyStationTransaction transaction)
		{

			var localTransaction = transaction;
			var gasboyStations = new GasboyStations();
			GasboyStationGeneralConfiguration generalConfigurationForSite = gasboyStations.GetGeneralConfigurationBySiteGuid(security, localTransaction.SiteGuid); //security or local transaction?
			var station = gasboyStations.Get(security,localTransaction.ExternalStationGuid);

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(siteChannel => siteChannel.Get(security, localTransaction.SiteGuid, false, false, true));

			GasboyStationProductMapping productMapping = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStationProductMapping>(x => x.GetMappedProductByStationProductID(security, localTransaction.ExternalStationGuid, localTransaction.ProductName));

			CompanyCollectionClass companyManagerList =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateByRole(security, COMPANY_ROLE.MANAGER, false, false));
			CompanyCollectionClass companyOwnerList =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateByRole(security, COMPANY_ROLE.OWNER, false, false));

			if ((companyManagerList.Count != 1) || (companyOwnerList.Count != 1))
			{
				throw new Exception("Could not find a Manager or Owner.");
			}
				string managerID = companyManagerList[0].ID;
				Guid managerGuid = companyManagerList[0].MasterRecordGuid;

				string ownerID = companyOwnerList[0].ID;
				Guid ownerGuid = companyOwnerList[0].MasterRecordGuid;

			if (null == productMapping)
				{
					var error = new GasboyStationTransactionError
					{
						ErrorMessage = string.Format("Missing Gasboy Islander Product Mapping for {0}.", localTransaction.ProductName),
						ExternalStationTransactionGuid = localTransaction.IdentityGuid
					};

					localTransaction.TransactionErrors.Add(error);

					localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
					localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.AutoRetry;
				return string.Format("Missing Gasboy Islander Product Mapping for {0}.", localTransaction.ProductName);
			}
				else
				{
					var productID = productMapping.FuelsManagerProductID;
					var productGuid = productMapping.FuelsManagerProductMasterRecordGuid;

					// Get the rest of the FuelsManager Product record since we'll need the Code.
					var product = FMChannelHelper.MakeCall<IProducts, ProductClass>(productChannel => productChannel.Get(security, productGuid));

					if (null == product)
					{
						var error = new GasboyStationTransactionError
						{
							ErrorMessage =
												string.Format(
													"Unable to load FuelsManager Product {0}.",
													productID),
							ExternalStationTransactionGuid = localTransaction.IdentityGuid
						};

						localTransaction.TransactionErrors.Add(error);

						localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
						localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.AutoRetry;
					}
					else
					{
						var newFuelsManagerTransaction = new TransactionDO
						{
							TransID = FuelsManagerId.NewId(),
							Alias =
																			generalConfigurationForSite.RetailSaleTransactionAliasName,
							TransactionAliasGuid =
																			generalConfigurationForSite.RetailSaleTransactionAliasGuid
																			.GetValueOrDefault(),
							//DestinationRegistrationID1 = gasboyTransaction.MeanName,
							TransactionDateTime = TypeHelper.ConvertDateTimeOffset(localTransaction.TransactionTimeStamp),
							//TransRefID = gasboyTransaction.ID,
							//GateID = station.ID,
							InventoryDate =
																			TypeHelper.ConvertDateTimeNoNull(
																				localTransaction.TransactionTimeStamp),
							TimeOut = TypeHelper.ConvertDateTimeNoNull(localTransaction.TransactionTimeStamp),
							ManagerID = managerID,
							ManagerCode = managerID,
							ManagerCompanyGuid = managerGuid,
							OwnerID = ownerID,
							OwnerCode = ownerID,
							OwnerCompanyGuid = ownerGuid,
							SiteGuid = security.SiteGuid,
							Site = security.SiteID,
							SupplierID = station.BillingID,
							SupplierCode = station.BillingID,
							VolumeUnits = site.VolumeUnits,
							//UserData1 = gasboyTransaction.ID,
							//UserData2 = gasboyTransaction.TransactionType,
							//UserData3 = gasboyTransaction.ExternalAuthorizationNumber,
							UserData4 = Convert.ToString(Convert.ToInt64(localTransaction.ID) - 300000000),
							//UserData5 = gasboyTransaction.NozzleID,
							//UserData6 = string.Empty, 
							//UserData7 = gasboyTransaction.Pump,
							//UserData8 = gasboyTransaction.PumpID,
							//UserData9 = gasboyTransaction.ProxyDeviceID,
							//                        UserData10 = gasboyTransaction.ShiftID,
							//                        UserData11 = gasboyTransaction.Tag,
							//UserData12 = gasboyTransaction.FuelingVehiclePlate,
							//                        UserData14 = gasboyTransaction.MeanID,
							//                        UserData15 = gasboyTransaction.HoseNumber,
							//                        UserData16 = gasboyTransaction.FleetID,
							//                        UserData17 = gasboyTransaction.DriverMeanID,
							//                        UserData18 = gasboyTransaction.DriverPlate,
							//                        UserData19 = gasboyTransaction.DriverTag,
						};


						if (newFuelsManagerTransaction.InventoryDate <= site._AdministrativeLockDate.Value)
						{
							var error = new GasboyStationTransactionError
							{
								ErrorMessage =
													"Inventory date must be after the Administrative Lock Date.",
								ExternalStationTransactionGuid = localTransaction.IdentityGuid
							};

							localTransaction.TransactionErrors.Add(error);

							localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
							localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.Pending;
						}

						if (!security.HasRight(RIGHT.CONFIGURE_ACCOUNTING) && !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
							 && newFuelsManagerTransaction.InventoryDate <= site._OperationalLockDate.Value)
						{
							var error = new GasboyStationTransactionError
							{
								ErrorMessage =
													"Inventory date must be after the Operational Lock Date.",
								ExternalStationTransactionGuid = localTransaction.IdentityGuid
							};

							localTransaction.TransactionErrors.Add(error);

							localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Failed;
							localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.Pending;
						}

						newFuelsManagerTransaction.PaymentInfo = new PaymentInfoDO()
						{
							CreditCardNumber = localTransaction.Tag
						};

						LineItemDO newLineItem = new LineItemDO
						{
							TransactionLineItemGuid = Guid.NewGuid(),
							//Density = Convert.ToDouble(gasboyTransaction.Density),
							//EngineRunTime = Convert.ToDouble(gasboyTransaction.EngineHours),
							GrossQuantityReceived = Convert.ToDouble(localTransaction.Quantity),
							NetQuantityReceived = Convert.ToDouble(localTransaction.Quantity),
							Odometer = Convert.ToDouble(localTransaction.Odometer),
							Product = productID,
							ProductGuid = productGuid,
							ProductCode = product.Code,
							//Number01 = Convert.ToDouble(gasboyTransaction.PricePerVolume),
							//StorageLocationID = gasboyTransaction.TankName,
							//Temperature = Convert.ToDouble(gasboyTransaction.Temperature)

						};

						newLineItem.Quantity.Gross = newLineItem.GrossQuantityReceived;
						newLineItem.Quantity.Net = newLineItem.NetQuantityReceived;

						newLineItem.VolumeUnits = site.VolumeUnits;

						newLineItem.DestinationEQ.RegistrationID = localTransaction.MeanName;

						newFuelsManagerTransaction.LineItems.Add(newLineItem);

					//TransactionDO validate = transactionToValidate;
					TransactionValidationResult validationResult;
					Guid securityUserGuid = security.UserGuid;

					try
					{
						// Set security.UserGuid = Guid.empty before validating to bypass the authorized companies check.
						security.UserGuid = Guid.Empty;
						validationResult =
							FMChannelHelper.MakeCall<ITransactionValidator, TransactionValidationResult>(
								transactionValidatorChannel => transactionValidatorChannel.ValidateTransaction(security, newFuelsManagerTransaction));
					}
					finally
					{
						security.UserGuid = securityUserGuid;
					}

					if (validationResult.IsValid)
					{
						try
						{
							localTransaction.TransactionErrors.Clear();
							localTransaction.ExternalStationTransactionStatus = ExternalStationTransactionStatus.Completed;
							localTransaction.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.Processed;
							localTransaction.CreatedBy = security.UserID;
							var sr = new SaveTransactionsSR
							{
								Security = security,
								CurrentSiteGuid = security.SiteGuid,
								ConvertUnits = false,
								BypassValidation = true
							};
							sr.ConvertUnits = true;
							sr.Transactions.Add(newFuelsManagerTransaction);
							FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
								saveTransactionsChannel => saveTransactionsChannel.SaveTransactions(sr)); //Save the FuelsManager Transaction
							this.RemoveFailedTransactionErrors(security, localTransaction); //Clear out Failed Transaction Errors from this External Station Transaction
							List<GasboyStationTransaction> gasboySourceTransactions = new List<GasboyStationTransaction>()
																										 {
																											 localTransaction
																										 };
							this.UpdateTransactionStatus(security, localTransaction);
							this.UpdateTransactionFailedStatuses(security, gasboySourceTransactions); //Save the Completed and Processed Statuses 
							return "Gasboy Transaction succesfully converted to FuelsManager Transaction";

						}
						catch
						{
							throw new Exception("Error processing transaction");
						}
					}
					else
					{
						foreach (string error in validationResult.ErrorList)
						{
							localTransaction.TransactionErrors.Add(
								new GasboyStationTransactionError { ErrorMessage = error });
							List<GasboyStationTransaction> gasboySourceTransactions = new List<GasboyStationTransaction>()
																										 {
																											 localTransaction
																										 };

							this.UpdateTransactionFailedStatuses(security, gasboySourceTransactions);
						}
						return "Modified transaction failed validation";

					}
				}

					try
					{

						List<GasboyStationTransaction> gasboySourceTransactions = new List<GasboyStationTransaction>()
							                                                          {
								                                                          localTransaction
							                                                          };
					//this.AddTransactions(security, gasboySourceTransactions); transaction needs to be updated with new error status, not added
					return "Modified transaction could not be processed";


				}
					catch
					{

						throw new Exception("Error saving Gasboy transaction to database");

					}
				}
			}

		private void UpdateTransactionStatus(SecurityClass security, GasboyStationTransaction localTransaction)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			GasboyStationTransactionDBI.ModifyStatus(security, localTransaction);
		}

		/// <summary>
		/// Clears the Error list for a given external station transaction. Should be used when a transaction has previously failed, but been updated and then validated succesfully. 
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="localTransaction">The transaction from which to clear the errors from.</param>
		private void RemoveFailedTransactionErrors(SecurityClass security, GasboyStationTransaction localTransaction)
		{
			GasboyStationTransactionErrorDBI.Clear(security, localTransaction.IdentityGuid);
		}



		/// <summary>
		/// Add transactions that have failed to be converted to a real transaction to the database
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="transactions">The failed transactions to add to the database</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddTransactions(SecurityClass security, List<GasboyStationTransaction> transactions)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (transactions == null)
			{
				throw new ArgumentNullException("transactions");
			}

			var failedTransactionErrors = new List<GasboyStationTransactionError>();

			foreach (GasboyStationTransaction transaction in transactions)
			{
				transaction.IdentityGuid = Guid.NewGuid();
				transaction.SiteGuid = security.SiteGuid;
				transaction.CreatedBy = security.UserID;

				foreach (GasboyStationTransactionError failedTransactionError in transaction.TransactionErrors)
				{
					failedTransactionError.IdentityGuid = Guid.NewGuid();
					failedTransactionError.ExternalStationTransactionGuid = transaction.IdentityGuid;
					failedTransactionError.CreatedBy = security.UserID;
				}

				failedTransactionErrors.AddRange(transaction.TransactionErrors);

				XmlWriterSettings settings = new XmlWriterSettings
				{
					Encoding = new UnicodeEncoding(false, false),
					Indent = false,
					OmitXmlDeclaration = false
				};

				// Serialize the message
				using (TextWriter textWriter = new StringWriter())
				{
					using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
					{
						Serializer.Serialize(xmlWriter, transaction);
					}

					transaction.RawTransactionData = textWriter.ToString();
				}
			}

			GasboyStationTransactionDBI.Insert(security, transactions);

			if (failedTransactionErrors.Count > 0)
			{
				this.AddFailedTransactionErrors(security, failedTransactionErrors);
			}
		}

		/// <summary>
		/// Updates one or more failed transactions with their new failed status
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="failedTransactions"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateTransactionFailedStatuses(SecurityClass security, List<GasboyStationTransaction> failedTransactions)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			GasboyStationTransactionDBI.ModifyFailedStatus(security, failedTransactions);
		}

		/// <summary>Retrieve a failed transaction from the database</summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="externalStationTransactionGuid">Identifies the failed transaction to retrieve</param>
		/// <returns>The <see cref="GasboyStationTransaction"/> identified by the provided guid.</returns>
		public GasboyStationTransaction GetFailedTransaction(SecurityClass security, Guid externalStationTransactionGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (externalStationTransactionGuid == Guid.Empty)
			{
				throw new ArgumentException("externalStationTransactionGuid");
			}

			GasboyStationTransaction gasboyStationTransaction = GasboyStationTransactionDBI.Get(security, externalStationTransactionGuid, true);

			// Deserialize the Raw Transaction Data and copy over any fields that are recorded in the DB instead of in the RawTransactionData
			GasboyStationTransaction deserializedExternalStationTransaction;

			using (TextReader reader = new StringReader(gasboyStationTransaction.RawTransactionData))
			{
				deserializedExternalStationTransaction = Serializer.Deserialize(reader) as GasboyStationTransaction;
			}

			if (deserializedExternalStationTransaction != null)
			{
				deserializedExternalStationTransaction.IdentityGuid = gasboyStationTransaction.IdentityGuid;
				deserializedExternalStationTransaction.SiteGuid = gasboyStationTransaction.SiteGuid;
				deserializedExternalStationTransaction.ID = gasboyStationTransaction.ID;
				deserializedExternalStationTransaction.ExternalStationGuid = gasboyStationTransaction.ExternalStationGuid;
				deserializedExternalStationTransaction.ExternalStationID = gasboyStationTransaction.ExternalStationID;
				deserializedExternalStationTransaction.CreatedDate = gasboyStationTransaction.CreatedDate;
				deserializedExternalStationTransaction.CreatedBy = gasboyStationTransaction.CreatedBy;
				deserializedExternalStationTransaction.UpdatedDate = gasboyStationTransaction.UpdatedDate;
				deserializedExternalStationTransaction.UpdatedBy = gasboyStationTransaction.UpdatedBy;

				deserializedExternalStationTransaction.ExternalStationTransactionStatus =
					gasboyStationTransaction.ExternalStationTransactionStatus;
				deserializedExternalStationTransaction.ExternalStationTransactionFailedStatus =
					gasboyStationTransaction.ExternalStationTransactionFailedStatus;

				deserializedExternalStationTransaction.TransactionErrors = gasboyStationTransaction.TransactionErrors;
			}

			return deserializedExternalStationTransaction;
		}

		/// <summary>Get all failed transactions for a specific site from the database</summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="externalStationGuid">If not empty, identifies the external station to retrieve failed transactions for</param>
		/// <param name="beginDate">The beginning receive date of failed transactions to search for</param>
		/// <param name="endDate">The ending receive date of failed transactions to search for</param>
		/// <param name="transactionID">The external transaction ID to search for</param>
		/// <returns>All failed transactions for a specific site</returns>
		public List<GasboyStationTransaction> EnumerateFailedTransactions(SecurityClass security, Guid externalStationGuid, DateTimeOffset beginDate, DateTimeOffset endDate, string transactionID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			List<GasboyStationTransaction> failedTransactions = GasboyStationTransactionDBI.GetFailedList(security, security.SiteGuid, externalStationGuid, beginDate, endDate, transactionID);

			return failedTransactions;
		}

		#endregion Failed Transaction Methods

		#region Transaction Validation Error Methods
		/// <summary>
		/// Get all transaction validation errors from the database for a specified external station transaction
		/// </summary>
		/// <param name="security">Contains Security information</param>
		/// <param name="externalStationFailedTransactionGuid">Identifies the external station transaction to get errors for</param>
		/// <returns>All transaction validation errors from the database for a specified external station transaction</returns>
		private List<GasboyStationTransactionError> EnumerateFailedTransactionErrors(SecurityClass security, Guid externalStationFailedTransactionGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION) && !security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (externalStationFailedTransactionGuid == Guid.Empty)
			{
				throw new ArgumentException("externalStationFailedTransactionGuid");
			}

			List<GasboyStationTransactionError> errors = GasboyStationTransactionErrorDBI.GetList(security, externalStationFailedTransactionGuid);

			return errors;
		}

		/// <summary>
		/// Add transaction validation errors from fuelsmanager to the failed transaction error table
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="failedTransactionErrors">The failed transaction errors to add to the database</param>
		private void AddFailedTransactionErrors(SecurityClass security, List<GasboyStationTransactionError> failedTransactionErrors)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (failedTransactionErrors == null)
			{
				throw new ArgumentNullException("failedTransactionErrors");
			}

			GasboyStationTransactionErrorDBI.Insert(security, failedTransactionErrors);
		}

		#endregion Transaction Validation Error Methods

		#region Helpers (Move into utility class)

		private GasboyStationProductMapping GetMappedProduct(SecurityClass security, GasboyStation station, string externalProduct)
		{
			try
			{
				return GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStationProductMapping>(x => x.GetMappedProductByStationProductID(security, station.IdentityGuid, externalProduct));
			}
			catch (FaultException<SaveTransactionsException> fex)
			{
				throw new Exception(fex.Detail.Message);
			}

			return null;
		}

		private TransactionAliasClass GetTransactionAliasForSaleTransaction(SecurityClass security)
		{
			try
			{
				var configuration = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStationGeneralConfiguration>(x => x.GetGeneralConfigurationBySiteGuid(security, security.SiteGuid));

				if (null != configuration && configuration.RetailSaleTransactionAliasGuid.HasValue)
				{
					return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
						x => x.Get(security, configuration.RetailSaleTransactionAliasGuid.Value, false));
				}
				else
				{
					// Need to report a configuration error / Alarm and Events
				}
				// return GasboyChannelHelper.MakeCall<IGasboyStations, DefaultTransactionAliasForSale>(x => x.GetDefaultTransactionAliasForSale(security));
			}
			catch (FaultException<SaveTransactionsException> fex)
			{
				throw new Exception(fex.Detail.Message);
			}

			return null;
		}

		/// <summary>
		/// This method handles the process of setting the Line Item Equipment to the header equipment
		///    when the Line Item Source or Destination equipment is not configured in the transaction alias
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="alias">
		/// The alias.
		/// </param>
		/// <param name="trans">
		/// The trans.
		/// </param>
		/// <returns>
		/// </returns>
		private void SetLineItemEquipment(
			SecurityClass security,
			TransactionAliasClass alias,
			GasboyStationTransaction trans)
		{
			//if (alias.LineItemFieldCollection.Find("DestinationRegistrationID") == null)
			//{
			//    EquipmentDO equipmentDO = null;
			//    if (alias.TransactionFieldCollection.Find("DestinationRegistrationID1") != null
			//        && !string.IsNullOrEmpty(trans.MeanName)
			//        && EquipmentTypeClass.HasCompartments(
			//            EquipmentTypeClass.Type(trans.MeanName)))
			//    {
			//        EquipmentClass destEquip =
			//            FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
			//                x => x.Get(security, (Guid)trans.MeanName));

			//        equipmentDO = new EquipmentDO(destEquip);
			//    }
			//    else if (alias.TransactionFieldCollection.Find("DestinationRegistrationID2") != null
			//             && !string.IsNullOrEmpty(trans.transaction.DestinationRegistrationID2)
			//             && EquipmentTypeClass.HasCompartments(
			//                 EquipmentTypeClass.Type(trans.transaction.DestinationEquipmentType2)))
			//    {
			//        EquipmentClass destEquip =
			//            FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
			//                x => x.Get(security, (Guid)trans.transaction.Destination2EquipmentGuid));

			//        equipmentDO = new EquipmentDO(destEquip);
			//    }

			//    if (equipmentDO != null)
			//    {
			//        foreach (TransactionLineItemSelectionDO lineItemDO in trans.transactionLineItems)
			//        {
			//            lineItemDO.DestinationRegistrationID = equipmentDO.RegistrationID;
			//            lineItemDO.DestinationSerialNumber = equipmentDO.SerialNumber;
			//            lineItemDO.DestinationEquipmentModel = equipmentDO.EquipmentModel;
			//            lineItemDO.DestinationEquipmentType = equipmentDO.EquipmentType;

			//            if (equipmentDO.EquipmentGuid != Guid.Empty)
			//            {
			//                lineItemDO.DestinationEquipmentGuid = equipmentDO.EquipmentGuid;
			//            }
			//            else
			//            {
			//                lineItemDO.DestinationEquipmentGuid = Guid.Empty;
			//            }
			//        }
			//    }
			//}

			//if (alias.LineItemFieldCollection.Find("SourceRegistrationID") == null)
			//{
			//    EquipmentDO equipmentDO = null;
			//    if (alias.TransactionFieldCollection.Find("SourceRegistrationID1") != null
			//        && !string.IsNullOrEmpty(trans.transaction.SourceRegistrationID1)
			//        && EquipmentTypeClass.HasCompartments(
			//            EquipmentTypeClass.Type(trans.transaction.SourceEquipmentType1)))
			//    {
			//        EquipmentClass destEquip =
			//            FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
			//                x => x.Get(security, (Guid)trans.transaction.Source1EquipmentGuid));

			//        equipmentDO = new EquipmentDO(destEquip);
			//    }
			//    else if (alias.TransactionFieldCollection.Find("SourceRegistrationID2") != null
			//             && !string.IsNullOrEmpty(trans.transaction.SourceRegistrationID2)
			//             && EquipmentTypeClass.HasCompartments(
			//                 EquipmentTypeClass.Type(trans.transaction.SourceEquipmentType2)))
			//    {
			//        EquipmentClass destEquip =
			//            FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
			//                x => x.Get(security, (Guid)trans.transaction.Source2EquipmentGuid));

			//        equipmentDO = new EquipmentDO(destEquip);
			//    }

			//    if (equipmentDO != null)
			//    {
			//        foreach (TransactionLineItemSelectionDO lineItemDO in trans.transactionLineItems)
			//        {
			//            lineItemDO.SourceRegistrationID = equipmentDO.RegistrationID;
			//            lineItemDO.SourceSerialNumber = equipmentDO.SerialNumber;
			//            lineItemDO.SourceEquipmentModel = equipmentDO.EquipmentModel;
			//            lineItemDO.SourceEquipmentType = equipmentDO.EquipmentType;

			//            if (equipmentDO.EquipmentGuid != Guid.Empty)
			//            {
			//                lineItemDO.SourceEquipmentGuid = equipmentDO.EquipmentGuid;
			//            }
			//            else
			//            {
			//                lineItemDO.SourceEquipmentGuid = Guid.Empty;
			//            }
			//        }
			//    }
			//}
		}

		#endregion Helpers
	}
}