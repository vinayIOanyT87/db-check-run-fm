using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    using FMCore.Interfaces;

    public class TransactionObjectTranslationService : ITransactionObjectTranslationService
    {
        private readonly ITransactionFieldsService _transactionFieldsService;
        private readonly ICurrentRequestContext _currentRequestContext;
        private readonly ITransactionAliasesProxy _transactionAliasProxy;

        private readonly IFMCustomLogger _logger;

        public TransactionObjectTranslationService(
            ITransactionFieldsService transactionFieldsService,
            ICurrentRequestContext currentRequestContext,
            ITransactionAliasesProxy transactionAliasProxy,
            IFMCustomLogger logger)
        {
            this._transactionFieldsService = transactionFieldsService;
            this._currentRequestContext = currentRequestContext;
            this._transactionAliasProxy = transactionAliasProxy;
            this._logger = logger;
        }


        public TransactionDO ApplyDictionaryToTransaction(TransactionDO transactionToBeAppliedTo, Dictionary<string, string> newTransactionUserValues, TransactionAliasClass transactionAlias)
        {
            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            var userSite = this._currentRequestContext.GetCurrentSite();

            var transactionFieldDefinitions = this._transactionFieldsService.GeTransactionFieldDefinitionsForUI(transactionAlias);
            foreach (var field in newTransactionUserValues)
            {
                var definition = transactionFieldDefinitions.FirstOrDefault(x => x.ID == field.Key);

                if (definition == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(definition.PropertyPath))
                {
                    this._logger.Debug("Did not receive a property path for the following value: {@PassedInParams}", new { field, definition });
                }

                if (definition.Type == TransactionFieldType.LineItem)
                {
                    var grandParent = typeof(LineItemDO);
                    var lineItem = transactionToBeAppliedTo.LineItems[0];

                    this.SetFieldParentChild(field.Value, definition.PropertyPath, grandParent, lineItem);

                }
                else //TransactionFieldType.Transaction and TransacitonFieldType.Notes and maybe whatever i did not catch
                {
                    var grandParent = typeof(TransactionDO);
                    var lineItem = transactionToBeAppliedTo;
                    this.SetFieldParentChild(field.Value, definition.PropertyPath, grandParent, lineItem);

                }
            }

            return transactionToBeAppliedTo;
        }

        private void SetFieldParentChild(string value, string propertyPath,
            Type grandParentType, object grandParent)
        {
            //MeterReading.MeterFactor
            if (propertyPath.Contains("."))
            {
                var parentName = propertyPath.Split('.')[0];
                var childName = propertyPath.Split('.')[1];
                var parentProperty = GetPropertyCaseInsensitive(parentName, grandParentType);
                var parentValue = parentProperty.GetValue(grandParent);
                if (parentValue == null)
                {
                    parentValue = Activator.CreateInstance(parentProperty.PropertyType);
                    parentProperty.SetValue(grandParent, parentValue);
                }
                var childProperty = GetPropertyCaseInsensitive(childName, parentProperty.PropertyType);
                SetField(value, parentValue, childProperty);
            }
            //VCF
            else
            {
                PropertyInfo property = GetPropertyCaseInsensitive(propertyPath, grandParentType);
                SetField(value, grandParent, property);
            }
        }

        private static void SetField(string value, object parent, PropertyInfo childProperty)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            object safeValue;
            switch (childProperty.PropertyType.FullName)
            {
                case "System.DateTime":
                    //special case for date times so we can parse in UTC
                    safeValue = DateTimeOffset.Parse(value).UtcDateTime;
                    break;
                default:
                    var converter = TypeDescriptor.GetConverter(childProperty.PropertyType);
                    safeValue = converter.ConvertFromString(value);
                    break;
            }
            childProperty.SetValue(parent, safeValue);
        }

        private static PropertyInfo GetPropertyCaseInsensitive(string fieldName, Type parent)
        {
            return parent
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(x => fieldName.ToLower() == x.Name.ToLower());
        }

        public Dictionary<string, string> CreateTransactionFromDataObject(TransactionDO transaction)
        {
            //get the transaciton alias from the transaction
            var transactionAlias = this._transactionAliasProxy.Get(transaction.TransactionAliasGuid, true);
            var transactionFieldDefinitions = this._transactionFieldsService.GeTransactionFieldDefinitionsForUI(transaction.TransactionAliasGuid);
            var result = new Dictionary<string, string>();
            //loop thru the fields for the transaciton alias
            foreach (var field in transactionFieldDefinitions)
            {
                KeyValuePair<string, string>? keyValueToAdd = null;
                if(field.Type == TransactionFieldType.Transaction)
                {
                    //add every fields value to the dictionary
                    keyValueToAdd = this.GrabKeyAndValueOfTransaction(field, transaction);
                }
                else if (field.Type == TransactionFieldType.LineItem)
                {
                    keyValueToAdd = this.GrabKeyAndValueOfTransaction(field, transaction.LineItems[0]);
                }
                else if (field.Type == TransactionFieldType.Note)
                {
                    keyValueToAdd = this.GrabKeyAndValueOfTransaction(field, transaction);
                }
                //if the key is already contained someone added the same field name from different transaction
                //sections ie: transaction.DocumentNumber = transactionlineitem.DocumentNumber
                if (keyValueToAdd.HasValue && !result.ContainsKey(keyValueToAdd.Value.Key))
                {
                    result.Add(keyValueToAdd.Value.Key, keyValueToAdd.Value.Value);
                }
            }
            return result;
        }

        private KeyValuePair<string,string> GrabKeyAndValueOfTransaction(TransactionAliasFieldClassWithColumn field, object grandparent)
        {
            var propName = field.PropertyPath;
            if (propName.Contains("."))
            {
                var parentName = propName.Split('.')[0];
                var parentProperty = GetPropertyCaseInsensitive(parentName, grandparent.GetType());
                var parent = parentProperty.GetValue(grandparent);
                var childName = propName.Split('.')[1];
                var childProperty = GetPropertyCaseInsensitive(childName, parent.GetType());
                var child = childProperty.GetValue(parent);

                var result = NullCheckForKeyValuePair(field, child);
                return result;
            }
            else
            {
                var reflectedPropertyInfo = GetPropertyCaseInsensitive(propName, grandparent.GetType());
                var value = reflectedPropertyInfo.GetValue(grandparent);
                var result = NullCheckForKeyValuePair(field, value);
                return result;
            }
        }

        private static KeyValuePair<string, string> NullCheckForKeyValuePair(TransactionAliasFieldClassWithColumn field, object child)
        {
            KeyValuePair<string, string> result;
            if (child == null)
            {
                result = new KeyValuePair<string, string>(field.ID, null);
            }
            else if (child.GetType() == typeof(DateTime))
            {
                result = new KeyValuePair<string, string>(field.ID, ((DateTime)child).ToString("s"));
            }
            else if (child.GetType() == typeof(DateTimeOffset))
            {
                result = new KeyValuePair<string, string>(field.ID, ((DateTimeOffset)child).ToString("s"));
            }
            else
            {
                result = new KeyValuePair<string, string>(field.ID, child.ToString());
            }

            return result;
        }
    }
}
