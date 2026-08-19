using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class TransactionAliasFieldsProxy : ITransactionAliasFieldsProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;

        public TransactionAliasFieldsProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(TransactionAliasFieldClass transactionAliasField)
        {
            throw new NotImplementedException();
        }
        
        public TransactionAliasFieldCollectionClass Enumerate(Guid transactionAliasGuid, TransactionFieldType type, bool dispatchFields, bool byUser)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliasFields, TransactionAliasFieldCollectionClass>(
                    service => service.Enumerate(currentSecurity, transactionAliasGuid, type, dispatchFields, byUser));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public TransactionAliasFieldCollectionClass EnumerateByAliasGuid(Guid aliasGuid, bool byUser)
        {
            throw new NotImplementedException();
        }

        public List<string> EnumerateFields(TransactionFieldType fieldType, TransactionTypes transType)
        {
            throw new NotImplementedException();
        }

        public TransactionAliasFieldClass Get(Guid identityGuid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransactionAliasFieldExtendedAttributes> GetColumnDefinitionsForTransactions()
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliasFields, IEnumerable<TransactionAliasFieldExtendedAttributes>>(
                    service => service.GetColumnDefinitionsForTransactions(currentSecurity));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public void Modify(TransactionAliasFieldClass transactionAliasField)
        {
            throw new NotImplementedException();
        }

        public void ModifyCollection(Guid transactionAliasGuid, string transactionAliasName, TransactionAliasFieldCollectionClass newFieldCollection, TransactionAliasFieldCollectionClass oldFieldCollection)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid identityGuid)
        {
            throw new NotImplementedException();
        }
    }
}
