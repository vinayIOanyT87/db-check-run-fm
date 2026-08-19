using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    using FMCore.Interfaces;
    using FMWebAPIBusinessLogic.DTO.TransactionDTO;
    using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;

    public class TransactionActionsProcessorService : ITransactionActionsProcessorsService
    {
        private readonly ITransactionProcessorProxy _transactionProcessorProxy;
        private readonly ISaveTransactionsProcessorProxy _saveTransactionsProcessorProxy;
        private readonly ICurrentRequestContext _currentRequestContext;
        private readonly ITransactionPossibleActionsService _transactionPossibleActionsService;
        private readonly ITransactionAliasesProxy _transactionAliasProxy;
        private readonly ITransactionObjectTranslationService _createTransactionObjectService;
        private readonly ITransactionPipeline _transactionPipeline;
        private readonly IErrorTransactionSubmissionProxy _errorTransactionSubmissionProxy;
        private readonly IFMCustomLogger _logger;
        public TransactionActionsProcessorService(ITransactionProcessorProxy transactionProcessorProxy,
            ISaveTransactionsProcessorProxy saveTransactionsProcessorProxy,
            ICurrentRequestContext currentRequestContext,
            ITransactionPossibleActionsService transactionPossibleActionsService,
            ITransactionAliasesProxy transactionAliasProxy,
            ITransactionObjectTranslationService createTransactionObjectService,
            ITransactionPipeline transactionPipeline, 
            IErrorTransactionSubmissionProxy errorTransactionSubmissionProxy,
            IFMCustomLogger logger)
        {
            this._transactionProcessorProxy = transactionProcessorProxy;
            this._saveTransactionsProcessorProxy = saveTransactionsProcessorProxy;
            this._currentRequestContext = currentRequestContext;
            this._transactionPossibleActionsService = transactionPossibleActionsService;
            this._transactionAliasProxy = transactionAliasProxy;
            this._createTransactionObjectService = createTransactionObjectService;
            this._transactionPipeline = transactionPipeline;
            this._errorTransactionSubmissionProxy = errorTransactionSubmissionProxy;
            this._logger = logger;
        }

        public TransactionDO SubmitNewTransactionInDictionaryFormat(
            Dictionary<string, string> newTransactionUserValues,
            Guid transactionAliasGuid)
        {
            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            try
            {

                var submittedTransactionAlias = this._transactionAliasProxy.Get(transactionAliasGuid, true);
                var newTransaction = this.CreateNewAviationTransactionWithDefaults(submittedTransactionAlias);
                this._createTransactionObjectService.ApplyDictionaryToTransaction(newTransaction, newTransactionUserValues, submittedTransactionAlias);

                this.FixUpFields(newTransaction, newTransactionUserValues);

                var inboundPipeline = this._transactionPipeline.Inbound();
                foreach (var pipe in inboundPipeline)
                {
                    pipe.Execute(newTransaction, submittedTransactionAlias);
                }

                var saveRequest = new SaveTransactionsSR { CurrentSiteGuid = userSecurity.SiteGuid, Security = userSecurity };
                saveRequest.Transactions.Add(newTransaction);
                this._saveTransactionsProcessorProxy.SaveTransactions(saveRequest);

                return newTransaction;
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Something failed on creating a transaction.  Pased in transaction was {@passedInTransaction}", newTransactionUserValues);
                this._errorTransactionSubmissionProxy.Add(new ErrorTransactionSubmissionClass()
                {
                    TransactionSubmissionInformation = Newtonsoft.Json.JsonConvert.SerializeObject(newTransactionUserValues),
                    SubmittedSiteGuid = userSecurity.SiteGuid,
                    SubmittedUserGuid = userSecurity.UserGuid,
                    CreatedBy = userSecurity.UserID,
                    UpdatedBy = userSecurity.UserID

                });
                throw;
            }
        }

        public TransactionDO UpdateExistingTransactionInDictionaryFormat(
            Dictionary<string, string> newTransactionUserValues,
            Guid transactionAliasGuid,
            Guid transactionGuid)
        {
            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            try
            {
                var submittedTransactionAlias = this._transactionAliasProxy.Get(transactionAliasGuid, true);
                //Grab existing trx
                var trxSR = new TransactionSR();
                trxSR.TransactionGuid = transactionGuid;
                trxSR.Security = userSecurity;
                var existingTransaction = this._transactionProcessorProxy.Process(trxSR);
                this._createTransactionObjectService.ApplyDictionaryToTransaction(existingTransaction, newTransactionUserValues, submittedTransactionAlias);

                this.FixUpFields(existingTransaction, newTransactionUserValues);

                var inboundPipeline = this._transactionPipeline.Inbound();
                foreach (var pipe in inboundPipeline)
                {
                    pipe.Execute(existingTransaction, submittedTransactionAlias);
                }

                var saveRequest = new SaveTransactionsSR { CurrentSiteGuid = userSecurity.SiteGuid, Security = userSecurity };
                saveRequest.Transactions.Add(existingTransaction);
                this._saveTransactionsProcessorProxy.SaveTransactions(saveRequest);

                return existingTransaction;
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Something failed on creating a transaction.  Pased in transaction was {@passedInTransaction}", newTransactionUserValues);
                this._errorTransactionSubmissionProxy.Add(new ErrorTransactionSubmissionClass()
                {
                    TransactionSubmissionInformation = Newtonsoft.Json.JsonConvert.SerializeObject(newTransactionUserValues),
                    SubmittedSiteGuid = userSecurity.SiteGuid,
                    SubmittedUserGuid = userSecurity.UserGuid,
                    CreatedBy = userSecurity.UserID,
                    UpdatedBy = userSecurity.UserID

                });
                throw;
            }
        }


        private void FixUpFields(TransactionDO newTransaction, Dictionary<string, string> newTransactionUserValues)
        {
            //DocumentNumber definition might be on the LineItem, lets reassign it to the base item if it exist
            if (newTransactionUserValues.ContainsKey("DocumentNumber"))
            {
                newTransaction.DocumentNumber = newTransactionUserValues["DocumentNumber"];
            }

            //sometimes Status is on the line item, sometimes it is on the transaction.  
            TransactionStatus currentPassedInStatus = TransactionStatus.Completed;
            if (newTransactionUserValues.ContainsKey("LookupTransactionStatusIndex")
                && Enum.TryParse(newTransactionUserValues["LookupTransactionStatusIndex"], out currentPassedInStatus))
            {
                newTransaction.Status = currentPassedInStatus;
                foreach (var lineItem in newTransaction.LineItems)
                {
                    lineItem.Status = currentPassedInStatus;
                }
            }

            foreach (var lineItem in newTransaction.LineItems)
            {
                lineItem.DocumentNumber = newTransaction.DocumentNumber;
                lineItem.ProductCode = lineItem.Product;
                //grabs the last item set transaction status
                newTransaction.Status = lineItem.Status;

                //if dictionary has temp/density and it is null, replace existing fields with null
                if (newTransactionUserValues.ContainsKey("Temperature") &&
                    string.IsNullOrWhiteSpace(newTransactionUserValues["Temperature"]))
                {
                    lineItem.Temperature = null;
                }
                if (newTransactionUserValues.ContainsKey("Density") &&
                    string.IsNullOrWhiteSpace(newTransactionUserValues["Density"]))
                {
                    lineItem.Density = null;
                }
            }   
        }

        private TransactionDO CreateNewAviationTransactionWithDefaults(TransactionAliasClass transactionAlias)
        {
            // Create the transaction in memory.
            var newTransaction = new TransactionDO();
            if (transactionAlias.TransTypeID == TransactionTypes.T13_OwnerTransfer)
            {
                newTransaction = new OwnerTransferDO();
            }
            newTransaction.init();
            newTransaction.TransID = FuelsManagerId.NewId();
            newTransaction.InventoryDate = DateTime.Now.Date;
            var newLineItem = new LineItemDO();
            newTransaction.LineItems.Add(newLineItem);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_01, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_02, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_03, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_04, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_05, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_06, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_07, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_08, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_09, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_10, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_11, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_12, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_13, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_14, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_15, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_16, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_17, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_18, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_19, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_20, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_21, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_22, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_23, null);
            newTransaction.UserData.Add(TransactionDO.USER_DATA_KEY_24, null);
            newLineItem.UserData.Add("TALUD1", null);
            newLineItem.UserData.Add("TALUD2", null);
            newLineItem.UserData.Add("TALUD3", null);
            newLineItem.UserData.Add("TALUD4", null);
            newLineItem.UserData.Add("TALUD5", null);
            newLineItem.UserData.Add("TALUD6", null);
            newLineItem.UserData.Add("TALUD7", null);
            newLineItem.UserData.Add("TALUD8", null);
            newLineItem.UserData.Add("TALUD9", null);
            newLineItem.UserData.Add("TALUD10", null);
            newLineItem.UserData.Add("TALUD11", null);
            newLineItem.UserData.Add("TALUD12", null);
            newLineItem.UserData.Add("TALUD13", null);
            newLineItem.UserData.Add("TALUD14", null);
            newLineItem.UserData.Add("TALUD15", null);
            newLineItem.UserData.Add("TALUD16", null);
            newLineItem.UserData.Add("TALUD17", null);
            newLineItem.UserData.Add("TALUD18", null);
            newLineItem.UserData.Add("TALUD19", null);
            newLineItem.UserData.Add("TALUD20", null);
            newLineItem.UserData.Add("TALUD21", null);
            newLineItem.UserData.Add("TALUD22", null);
            newLineItem.UserData.Add("TALUD23", null);
            newLineItem.UserData.Add("TALUD24", null);

            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            var userSite = this._currentRequestContext.GetCurrentSite();
            newTransaction.Site = userSecurity.SiteID;
            newTransaction.SiteGuid = userSecurity.SiteGuid;
            newTransaction.TransactionDateTime = DateTime.Now;
            newTransaction.AdditiveVolumeUnits = userSite.AdditiveVolumeUnits;
            newTransaction.DensityUnits = userSite.DensityUnits;
            newTransaction.FlowUnits = userSite.FlowUnits;
            newTransaction.LevelUnits = userSite.LevelUnits;
            newTransaction.MassUnits = userSite.MassUnits;
            newTransaction.PressureUnits = userSite.PressureUnits;
            newTransaction.TemperatureUnits = userSite.TemperatureUnits;
            newTransaction.VolumeUnits = userSite.VolumeUnits;

            var lienItem = newTransaction.LineItems.First();
            lienItem.DensityUnits = userSite.DensityUnits;
            lienItem.FlowUnits = userSite.FlowUnits;
            lienItem.LevelUnits = userSite.LevelUnits;
            lienItem.MassUnits = userSite.MassUnits;
            lienItem.PressureUnits = userSite.PressureUnits;
            lienItem.TemperatureUnits = userSite.TemperatureUnits;
            lienItem.VolumeUnits = userSite.VolumeUnits;

            return newTransaction;
        }

        /// <summary>
        /// Reverse the transaction.
        /// </summary>
        /// <remarks>logic copied from TransactionDetail -> ReverseProcessing</remarks>
        /// <param name="transactionGuid"></param>
        public void ReverseTransaction(Guid transactionGuid)
        {
            try
            {
                //retrieve the transaction
                var retrieveRequest = new TransactionSR { TransactionGuid = transactionGuid };
                var transaction = this._transactionProcessorProxy.Process(retrieveRequest);

                //can the transaction be reversed?
                if (!this._transactionPossibleActionsService.CanTransactionBeReversed(transaction))
                {
                    throw new NotSupportedException("Cannot reverse this transaction");
                }

                ResetTransactionForReversalAndSetReversalType(transaction, TransactionDO.Reversal);

                //reverse the correct fields via the transactionReverdTransID flag
                transaction.SetVolumeSigns(false);


                //save
                var currentSecurityContext = this._currentRequestContext.GetCurrentSecurityContext();
                var currentSite = this._currentRequestContext.GetCurrentSite();
                transaction.UpdatedBy = currentSecurityContext.UserID;
                transaction.UpdatedDate = DateTimeOffset.Now;

                var saveRequest = new SaveTransactionsSR
                {
                    Security = currentSecurityContext,
                    CurrentSiteGuid = currentSecurityContext.SiteGuid,
                    Transactions = new List<TransactionDO>() { transaction }
                };

                //the save processor will grab the original transaction via the transaction.ReversedTransID and update 
                //the reverse type, we do not have to do it here (yay!)
                this._saveTransactionsProcessorProxy.SaveTransactions(saveRequest);
            }
            catch (Exception e)
            {
                this._logger.Fatal(e, "Could not reverse transaction");
                throw;
            }
        }

        private static void ResetTransactionForReversalAndSetReversalType(TransactionDO transaction, string reversalType)
        {
            //set reversal entries
            transaction.Status = TransactionStatus.Completed;
            transaction.ReversalType = reversalType;
            transaction.ReversedTransID = transaction.TransID;
            transaction.ConjoinReversedTransID = transaction.ConjoinedTransID;

            //reset transaction to act as a new transaction
            transaction.TransID = FuelsManagerId.NewId();
            transaction.TransactionGuid = Guid.Empty;
            transaction.ConjoinedTransactionGuid = Guid.Empty;
            transaction.TransactionNoteGuid = Guid.Empty;
            transaction.ConjoinedNotesGuid = Guid.Empty;
            transaction.TransactionSignatureGuid = Guid.Empty;
            transaction.ConjoinedSignatureGuid = Guid.Empty;
            transaction.TransactionUserDataGuid = Guid.Empty;
            transaction.ConjoinedUserDataGuid = Guid.Empty;

            if (string.IsNullOrEmpty(transaction.ConjoinedTransID) == false)
            {
                transaction.ConjoinedTransID = FuelsManagerId.NewId();
            }

            // Always default to current date
            transaction.InventoryDate = DateTime.Now;

            transaction.CloseoutDate = null;
            transaction.PartialCloseout = false;

            //fix up line items to be treated as new entries for the db
            foreach (LineItemDO lineItem in transaction.LineItems)
            {
                lineItem.TransactionLineItemGuid = Guid.Empty;
                lineItem.ConjoinedTransactionLineItemGuid = Guid.Empty;
                lineItem.TransactionLineItemUserDataGuid = Guid.Empty;
                lineItem.ConjoinedTransactionLineItemUserDataGuid = Guid.Empty;
                lineItem.CloseoutDate = null;
            }
        }

        public void ReverseUpdateTransactionInDictionaryFormat(
            Guid originalTransactionGuid,
            Dictionary<string, string> updatedTransactionUserValues)
        {
            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            try
            {
                //retrieve the transaction
                var retrieveRequest = new TransactionSR { TransactionGuid = originalTransactionGuid };
                var transaction = this._transactionProcessorProxy.Process(retrieveRequest);
                var currentAlias = this._transactionAliasProxy.Get(transaction.TransactionAliasGuid, false);

                //can the transaction be reversed?
                if (!this._transactionPossibleActionsService.CanTransactionBeReversed(transaction))
                {
                    throw new NotSupportedException("Cannot reverse this transaction");
                }

                ResetTransactionForReversalAndSetReversalType(transaction, TransactionDO.Update);

                this._createTransactionObjectService.ApplyDictionaryToTransaction(transaction, updatedTransactionUserValues, currentAlias);

                this.FixUpFields(transaction, updatedTransactionUserValues);

                var inboundPipeline = this._transactionPipeline.Inbound();
                foreach (var pipe in inboundPipeline)
                {
                    pipe.Execute(transaction, currentAlias);
                }

                var saveRequest = new SaveTransactionsSR { CurrentSiteGuid = userSecurity.SiteGuid, Security = userSecurity };
                saveRequest.Transactions.Add(transaction);
                this._saveTransactionsProcessorProxy.SaveTransactions(saveRequest);

            }
            catch (Exception e)
            {
                this._logger.Fatal(e, "Could not reverse transaction");
                throw;
            }
        }

        public void DeleteTransaction(Guid transactionGuid)
        {
            try
            {
                var retrieveRequest = new TransactionSR { TransactionGuid = transactionGuid };
                var transaction = this._transactionProcessorProxy.Process(retrieveRequest);

                //delete
                transaction.DeleteFlag = true;
                foreach (var lineItem in transaction.LineItems)
                {
                    lineItem.DeleteFlag = true;
                    foreach (var subLineItem in lineItem.SubLineItems)
                    {
                        subLineItem.DeleteFlag = true;
                    }
                }

                //save
                var currentSecurityContext = this._currentRequestContext.GetCurrentSecurityContext();
                var currentSite = this._currentRequestContext.GetCurrentSite();
                transaction.UpdatedBy = currentSecurityContext.UserID;
                transaction.UpdatedDate = DateTimeOffset.Now;

                var saveRequest = new SaveTransactionsSR
                                  {
                                      Security = currentSecurityContext,
                                      CurrentSiteGuid = currentSecurityContext.SiteGuid,
                                      Transactions = new List<TransactionDO>() { transaction }
                                  };
                this._saveTransactionsProcessorProxy.SaveTransactions(saveRequest);
            }
            catch (Exception e)
            {
                this._logger.Fatal(e, "Could not delete transaction");
                throw;
            }
        }
    }
}