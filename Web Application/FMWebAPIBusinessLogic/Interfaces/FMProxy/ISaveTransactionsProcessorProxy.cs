using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ISaveTransactionsProcessorProxy
    {
        SaveTransactionsResultDO SaveTransactions(SaveTransactionsSR sr);
        SaveTransmitTranListResultDO SaveTransmittedTransactions(TransmitTranListDO serviceRequestDataObject, SecurityClass securityObject);
    }
}