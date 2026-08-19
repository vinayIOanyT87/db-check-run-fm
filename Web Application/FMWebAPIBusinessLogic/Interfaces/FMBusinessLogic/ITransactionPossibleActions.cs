using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface ITransactionPossibleActionsService
    {
        bool CanTransactionBeReversed(TransactionDO transaction);
        bool CanTransactionBeEdited(TransactionDO transaction);
    }
}