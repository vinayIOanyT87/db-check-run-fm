using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
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
    public class TransactionProcessorProxy : ITransactionProcessorProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;
        public TransactionProcessorProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }
        public TransactionDO Process(TransactionSR sr)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                sr.Security = this._requestContext.GetCurrentSecurityContext();
                var resultTransaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return resultTransaction;
            }
            catch(Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }
    }
}
