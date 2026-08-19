using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface ITransactionPipeline
    {
        IEnumerable<IPipelineCommand> Inbound();
        IEnumerable<IPipelineCommand> Outbound();
    }
}
