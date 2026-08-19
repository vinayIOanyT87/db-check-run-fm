using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface IAutoDocumentNumberService
    {
        bool HasAutoDocumentNumberAvaliable(TransactionAliasClass transactionAlias, SiteClass currentSite);
    }
}