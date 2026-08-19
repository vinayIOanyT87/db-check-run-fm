using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Outbound
{
    public class IssueTransactionConverter : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T5_PrimaryDisbursement)
            {
                return;
            }
            trxDO.TransTypeID = trxAlias.TransTypeID;
            trxDO.SetVolumeSigns(true);
        }
    }
}
