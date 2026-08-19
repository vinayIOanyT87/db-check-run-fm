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
    public class TransactionAliasesProxy : ITransactionAliasesProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;

        public TransactionAliasesProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(TransactionAliasClass transactionAlias)
        {
            throw new NotImplementedException();
        }

        public TransactionAliasCollectionClass Enumerate()
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
                    service => service.Enumerate(currentSecurity));
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

        public TransactionAliasCollectionClass EnumerateByGroupMapsOnly()
        {
            throw new NotImplementedException();
        }

        public TransactionAliasCollectionClass EnumerateByTransTypeID(TransactionTypes transTypeID)
        {
            throw new NotImplementedException();
        }

        public TransactionAliasNameCollectionClass EnumerateDispatchAliasNames()
        {
            throw new NotImplementedException();
        }

        public List<string> EnumerateDispatchStatusCodes()
        {
            throw new NotImplementedException();
        }

        public TransactionAliasNameCollectionClass EnumerateNamesOnly(bool byUser)
        {
            throw new NotImplementedException();
        }

        public TransactionAliasCollectionClass EnumerateUndelegated()
        {
            throw new NotImplementedException();
        }

        public TransactionAliasClass Get(Guid identityGuid, bool byUser)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
                    service => service.Get(currentSecurity, identityGuid, byUser));
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

        public TransactionAliasClass GetBasicInfo(Guid transactionAliasClassGuid, Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdentityGuid(string ID)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
                    service => service.GetIdentityGuid(currentSecurity, ID));
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

        public Guid GetMasterRecordGuid(string id)
        {
            throw new NotImplementedException();
        }

        public TransactionAliasClass GetWithoutAliasFields(Guid aliasGuid)
        {
            throw new NotImplementedException();
        }

        public void Import(TransactionAliasClass alias)
        {
            throw new NotImplementedException();
        }

        public void Modify(TransactionAliasClass transactionAlias)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid transactionAliasGuid)
        {
            throw new NotImplementedException();
        }

        public bool UserHasModifyPermissions(Guid aliasGuid)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliases, bool>(
                    service => service.UserHasModifyPermissions(currentSecurity, aliasGuid));
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
    }
}
