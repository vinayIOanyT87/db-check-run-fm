using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    public class TransactionPossibleActionsService : ITransactionPossibleActionsService
    {
        private readonly ICurrentRequestContext _currentRequestContext;
        private readonly ITransactionAliasesProxy _transactionAliasProxy;

        public TransactionPossibleActionsService(ICurrentRequestContext currentRequestContext,
            ITransactionAliasesProxy transactionAliasProxy)
        {
            this._currentRequestContext = currentRequestContext;
            this._transactionAliasProxy = transactionAliasProxy;
        }


        /// <summary>
        /// Funtionality copied from TransationDetail.aspx.cs IsTransactionEditable Line 135
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool CanTransactionBeEdited(TransactionDO transaction)
        {
            var result = true;

            if (transaction.CloseoutDate.HasValue || transaction.PartialCloseout ||
                transaction.SiteGuid.IsEmpty() || transaction.Status == TransactionStatus.Posted ||
                transaction.Status == TransactionStatus.Pending)
            {
                return false; //return immidiately to avoid network calls
            }

            //if (transaction.ReversalType == TransactionDO.ReversalWithUpdate || 
            //    transaction.ReversalType == TransactionDO.UpdateOriginal || 
            //    transaction.ReversalType == TransactionDO.Original || 
            //    transaction.ReversalType == TransactionDO.Reversal)
            if (transaction.ReversalType == TransactionDO.UpdateOriginal ||
                transaction.ReversalType == TransactionDO.Original)
            {
                return false;
            }

            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            if (!userSecurity.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
            {
                return false;
            }


            // If this is an Order type transaction, check the order security rights
            if (transaction.TransTypeID == TransactionTypes.T17_Order ||
                transaction.TransTypeID == TransactionTypes.T18_SupplyOrder)
            {
                if (!userSecurity.HasModifyTransactionRightByAliasName((transaction.Alias)))
                {
                    return false;
                }
            }

            // make sure the user has permissions to the alias - is this the same as above user security check?
            result = this._transactionAliasProxy.UserHasModifyPermissions(transaction.TransactionAliasGuid);

            return result;
        }

        public bool CanTransactionBeReversed(TransactionDO transaction)
        {
            var result = true;
            if (transaction.DeleteFlag ||
                transaction.TransTypeID == TransactionTypes.T17_Order ||
                transaction.TransTypeID == TransactionTypes.T18_SupplyOrder)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(transaction.ReversalType) &&
                transaction.ReversalType != TransactionDO.None &&
                transaction.ReversalType != TransactionDO.Update)
            {
                return false;
            }

            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            if (!userSecurity.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
            {
                return false;
            }


            // If this is an Order type transaction, check the order security rights
            if (transaction.TransTypeID == TransactionTypes.T17_Order ||
                transaction.TransTypeID == TransactionTypes.T18_SupplyOrder)
            {
                if (!userSecurity.HasModifyTransactionRightByAliasName((transaction.Alias)))
                {
                    return false;
                }
            }

            // make sure the user has permissions to the alias - is this the same as above user security check?
            result = this._transactionAliasProxy.UserHasModifyPermissions(transaction.TransactionAliasGuid);


            return result;
        }
    }
}
