using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    public class TransactionFieldsService : ITransactionFieldsService
    {
        ITransactionAliasFieldsProxy _transactionAliasFieldsProxy;
        ITransactionAliasesProxy _transactionAliasesProxy;
        IFMCustomLogger _logger;
        public TransactionFieldsService(ITransactionAliasFieldsProxy transactionAliasFieldsProxy,
            ITransactionAliasesProxy transactionAliasesProxy,
            IFMCustomLogger fmLogger)
        {
            this._transactionAliasFieldsProxy = transactionAliasFieldsProxy;
            this._logger = fmLogger;
            this._transactionAliasesProxy = transactionAliasesProxy;
        }

        public IEnumerable<TransactionAliasFieldClassWithColumn> GeTransactionFieldDefinitionsForUI(
            TransactionAliasClass currentAlias)
        {
            var currentFields = currentAlias.GetOrderedFields(TRANSACTION_SECTION_TYPE.BODY, false);

            var columnExtendedDetails = _transactionAliasFieldsProxy.GetColumnDefinitionsForTransactions();

            var results = new List<TransactionAliasFieldClassWithColumn>();
            foreach (var fieldAlias in currentFields)
            {
                var toAdd = new TransactionAliasFieldClassWithColumn();
                if (fieldAlias.GetType() == typeof(TransactionAliasFieldClass))
                {
                    toAdd.Copy(fieldAlias as TransactionAliasFieldClass);
                }
                else if (fieldAlias.GetType() == typeof(UserDataFieldClass))
                {
                    toAdd.Copy(fieldAlias as UserDataFieldClass);
                    toAdd.IsUserDataField = true;
                }
                else
                {
                    this._logger.Error("Cannot currently use this field type: {@type}", fieldAlias.GetType());
                    throw new ApplicationException("Unknown field type");
                }

                toAdd.ColumnDefinition = columnExtendedDetails.FirstOrDefault(x => x.ColumnName == fieldAlias.DbName);
                if (toAdd.ColumnDefinition == null)
                {
                    //There are a few properties that have attributes on TransactionDO and defined as
                    //transaction fields but do not have database equivalents.
                    //These are the ones I have found so far: FromManagerID, ToManagerID, FromOwnerID, ToOwnerID,
                    //FromCarrierID, ToCarrierID, FromProduct, ToProduct
                    //the bottom definition will catch all of the above custom cases
                    //IE:  fake it until you make it
                    toAdd.ColumnDefinition = new TransactionAliasFieldExtendedAttributes()
                                             {
                                                 TableName = "tblTransactions",
                                                 ColumnName = fieldAlias.DbName,
                                                 ColumnType = "string",
                                                 PropertyName =  fieldAlias.DbName
                                             };
                }

                //finding property path based on sql mapping
                if (toAdd.Type == TransactionFieldType.Transaction)
                {
                    toAdd.PropertyPath = TransactionDO.GetPropertyName((toAdd.ColumnDefinition.PropertyName));
                }
                else if (toAdd.Type == TransactionFieldType.LineItem)
                {
                    toAdd.PropertyPath = LineItemDO.GetPropertyName((toAdd.ColumnDefinition.PropertyName));
                }
                else
                {
                    toAdd.PropertyPath = toAdd.ColumnDefinition.PropertyName;
                }
                results.Add(toAdd);
            }
            return results;
        }

        /// <summary>
        /// packages up all the fields and their definitions for aviation transaction entries
        /// </summary>
        /// <param name="transactionAliasGuid"></param>
        /// <returns></returns>
        public IEnumerable<TransactionAliasFieldClassWithColumn> GeTransactionFieldDefinitionsForUI(Guid transactionAliasGuid)
        {
            var currentAlias = this._transactionAliasesProxy.Get(transactionAliasGuid, false);
            return this.GeTransactionFieldDefinitionsForUI(currentAlias);

        }
    }
}
