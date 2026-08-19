using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Diagnostics;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class SaveTransactionsProcessorProxy : ISaveTransactionsProcessorProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;

        public SaveTransactionsProcessorProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public SaveTransactionsResultDO SaveTransactions(SaveTransactionsSR sr)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
                    service => service.SaveTransactions(sr));
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

        public SaveTransmitTranListResultDO SaveTransmittedTransactions(TransmitTranListDO serviceRequestDataObject, SecurityClass securityObject)
        {
            throw new NotImplementedException();
        }
    }
}
