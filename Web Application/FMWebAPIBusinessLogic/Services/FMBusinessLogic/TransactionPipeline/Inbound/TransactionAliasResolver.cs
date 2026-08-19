using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using System;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class TransactionAliasResolver : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            trxDO.Alias = trxAlias.ID;
            trxDO.TransactionAliasGuid = trxAlias.IdentityGuid;
            trxDO.TransTypeID= trxAlias.TransTypeID;
        }
    }
}
