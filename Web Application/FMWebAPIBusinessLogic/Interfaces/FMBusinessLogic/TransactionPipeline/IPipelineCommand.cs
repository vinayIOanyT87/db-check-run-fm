using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline
{
    public interface IPipelineCommand
    {
        void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias);
    }
}
