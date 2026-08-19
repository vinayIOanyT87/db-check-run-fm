using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using System;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{

    public class TransactionTransferConverter : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T13_OwnerTransfer)
            {
                return;
            }
            trxDO.TransTypeID = trxAlias.TransTypeID;
            trxDO.SetVolumeSigns(false);
        }
    }

}