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
    public class ErrorTransactionSubmissionProxy : IErrorTransactionSubmissionProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;
        public ErrorTransactionSubmissionProxy(
            ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }
        public Guid Add(ErrorTransactionSubmissionClass errorTransactionSubmissionClass)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IErrorTransactionSubmission, Guid>(
                    service => service.Add(currentSecurity, errorTransactionSubmissionClass));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return errorTransactionSubmissionClass.IdentityGuid;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public IEnumerable<ErrorTransactionSubmissionClass> GetByCustomer(Guid customerGuid)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IErrorTransactionSubmission, IEnumerable<ErrorTransactionSubmissionClass>>(
                    service => service.GetByCustomer(currentSecurity, customerGuid));
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
