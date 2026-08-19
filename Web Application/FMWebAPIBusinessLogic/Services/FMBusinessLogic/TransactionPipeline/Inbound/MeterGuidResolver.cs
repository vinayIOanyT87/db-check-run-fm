using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class MeterGuidResolver : IPipelineCommand
    {
        private readonly IMetersProxy _metersProxy;
        public MeterGuidResolver(IMetersProxy meterProxy)
        {
            _metersProxy = meterProxy;
        }

        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            foreach (LineItemDO lineItem in trxDO.LineItems)
            {
                if (!string.IsNullOrEmpty(lineItem.MeterID))
                {
                    var meterGuid = _metersProxy.GetIdentityGuid(lineItem.MeterID);
                    lineItem.MeterGuid = meterGuid;
                }
            }
        }
    }
}
