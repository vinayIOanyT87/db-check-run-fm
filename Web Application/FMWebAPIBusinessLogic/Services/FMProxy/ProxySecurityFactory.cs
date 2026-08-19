using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class ProxySecurityFactory : IProxySecurityFactory
    {
        private readonly ICurrentRequestContext _currentRequestContext;
        public ProxySecurityFactory(ICurrentRequestContext currentRequestContext)
        {
            this._currentRequestContext = currentRequestContext;
        }
        public SecurityClass GetSecurity()
        {
            return this._currentRequestContext.GetCurrentSecurityContext();
        }
    }
}
