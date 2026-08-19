using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{

    public class TransactionIssueConverter : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T5_PrimaryDisbursement)
            {
                return;
            }
            trxDO.TransTypeID = trxAlias.TransTypeID;
            trxDO.SetVolumeSigns(false);
        }
    }
}