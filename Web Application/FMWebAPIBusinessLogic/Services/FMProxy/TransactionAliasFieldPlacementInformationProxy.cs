using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class TransactionAliasFieldPlacementInformationProxy : ITransactionAliasFieldPlacementInformationProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;
        public TransactionAliasFieldPlacementInformationProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid AddOrUpdate(TransactionAliasFieldPlacementInformationClass fieldPlacement)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliasFieldPlacementInformation, Guid>(
                    service => service.AddOrUpdate(currentSecurity, fieldPlacement));
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

        public TransactionAliasFieldPlacementInformationClass GetByTransactionAlias(Guid transactionAliasGuid)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ITransactionAliasFieldPlacementInformation, TransactionAliasFieldPlacementInformationClass>(
                    service => service.GetByTransactionAlias(currentSecurity, transactionAliasGuid));
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
