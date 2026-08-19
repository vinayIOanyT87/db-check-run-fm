using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.DTO;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Services.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace FMWebAPI.Controllers
{
    [RoutePrefix("api/Transaction")]
    public class TransactionWebAPIController : ApiController
    {
        private readonly TransactionController _transactionController;
        private readonly IFMVCFService _vcfService;
        private readonly IFMCustomLogger _logger;

        public TransactionWebAPIController(TransactionController transactionController, IFMVCFService vcfService,  IFMCustomLogger logger)
        {
            this._transactionController = transactionController;
            this._vcfService = vcfService;
            this._logger = logger;

        }

        [Route("")]
        [HttpPost]
        public TransactionDO SubmitNewTransaction(TransactionInSimplifiedFormatDTO newTransactionUserValues)
        {
            try
            {
                if (newTransactionUserValues == null)
                {
                    throw new ArgumentNullException(nameof(newTransactionUserValues));
                }
                var newTransaction = this._transactionController.SubmitNewTransaction(newTransactionUserValues.TransactionPropertyValuePairs, newTransactionUserValues.TransactionAliasGuid);
                return newTransaction;
            }
            catch(Exception e)
            {
                _logger.Error(e, "Failed to save transaction: {@PassedInTransac}", newTransactionUserValues);
                throw;
            }
        }
        [Route("{transactionGuid}")]
        [HttpGet]
        public TransactionViewDTO GetTransaction(string transactionGuid)
        {
            try
            {
                Guid parsedTransactionGuid;
                if (!Guid.TryParse(transactionGuid, out parsedTransactionGuid))
                {
                    throw new NotSupportedException("Not a valid guid");
                }
                var result = this._transactionController.GetTransaction(parsedTransactionGuid);
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to get transaction: {@transactionGuid}", transactionGuid);
                throw;
            }
        }

        [Route("{transactionGuid}")]
        [HttpPost]
        public TransactionDO SubmitExistingTransaction(TransactionInSimplifiedFormatDTO updatedTransactionUserValues, string transactionGuid)
        {
            try
            {
                if (updatedTransactionUserValues == null)
                {
                    throw new ArgumentNullException(nameof(updatedTransactionUserValues));
                }
                if(string.IsNullOrWhiteSpace(transactionGuid))
                {
                    throw new ArgumentException(nameof(transactionGuid));
                }

                var newTransaction = this._transactionController.UpdateExistingTransaction(updatedTransactionUserValues.TransactionPropertyValuePairs, updatedTransactionUserValues.TransactionAliasGuid, transactionGuid);
                return newTransaction;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to save transaction: {@PassedInTransac}", updatedTransactionUserValues);
                throw;
            }
        }

        [Route("{transactionGuid}")]
        [HttpDelete]
        public void DeleteExistingTransaction(string transactionGuid)
        {

            try
            {
                this._transactionController.DeleteTransaction(transactionGuid);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to delete existing transaction: {@transactionGuid}", transactionGuid);
                throw;
            }
        }

        [Route("{transactionGuid}/Reverse")]
        [HttpPost]
        public void ReverseTransaction(string transactionGuid)
        {
            Guid parsedTransactionGuid;
            if (!Guid.TryParse(transactionGuid, out parsedTransactionGuid))
            {
                throw new NotSupportedException("Not a valid guid");
            }
            this._transactionController.ReverseTransaction(parsedTransactionGuid);
        }
        
        [Route("{originalTransactionGuid}/ReverseUpdate")]
        [HttpPost]
        public void ReverseUpdateTransaction(Dictionary<string, string> updatedTransactionUserValues, string originalTransactionGuid)
        {
            try
            {
                Guid parsedTransactionGuid;
                if (!Guid.TryParse(originalTransactionGuid, out parsedTransactionGuid))
                {
                    throw new NotSupportedException("Not a valid guid");
                }
                this._transactionController.ReverseUpdateTransaction(parsedTransactionGuid, updatedTransactionUserValues);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to save reverse update transaction: {@originalTransactionGuid}", originalTransactionGuid);
                throw;
            }
        }

        [Route("VCFCalculator/{productId}")]
        public double GetVCFForProduct(string productId, double temperature, double density)
        {
            try
            {
                var result = this._vcfService.GetVCFForProductBasedOnUserForAviation(productId, temperature, density);
                return result;
            }
            catch(Exception e)
            {
                _logger.Error(e, "Get VCF failed", new { productId, temperature, density });
                throw;
            }
        }

        [Route("TransactionDetails/{transactionAliasGuid}")]
        public TransactionDetailsDTO GetTransactionDetails(
            string transactionAliasGuid)
        {
            try
            {
                var results = this._transactionController.GetTransactionDetails(transactionAliasGuid);
                return results;
            }
            catch(Exception e)
            {
                this._logger.Error(e, "failed to get transaction details", transactionAliasGuid);
                throw;
            }
        }

        [Route("TransactionDetailAssociatedList/{transactionAliasGuid}")]
        [HttpGet]
        public IEnumerable<FieldWithAssociatedList> GetTransactionAssociatedLists(
            string transactionAliasGuid)
        {
            try
            {
                var result = this._transactionController.GetTransactionAssociatedLists(transactionAliasGuid);
                return result;
            }
            catch (Exception e)
            {
                this._logger.Error(e, "failed to get transaction details", transactionAliasGuid);
                throw;
            }
        }

        [Route("TransactionPlacementInformation/{transactionAliasGuid}")]
        public TransactionAliasFieldPlacementDTO GetPlacementInfo(string transactionAliasGuid)
        {
            try
            {
                var result = this._transactionController.GetPlacementInfo(transactionAliasGuid);
                return result;
            }
            catch (Exception e)
            {
                this._logger.Error(e, "failed to get transaction placement information", transactionAliasGuid);
                throw;
            }
        }
        [Route("TransactionPlacementInformation")]
        [HttpPost]
        public void SavePlacementInfo(TransactionAliasFieldPlacementDTO toSave)
        {
            try
            {
                this._transactionController.SavePlacementInfo(toSave);
            }
            catch (Exception e)
            {
                this._logger.Error(e, "failed to save transaction placement infomation", toSave);
                throw;
            }
        }
    }
}