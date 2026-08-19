using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    public class AutoDocumentNumberService : IAutoDocumentNumberService
    {
        public bool HasAutoDocumentNumberAvaliable(TransactionAliasClass transactionAlias, SiteClass currentSite)
        {
            int parsedCurrentNumber;
            var result = false;
            if (transactionAlias.TransTypeID == TransactionTypes.T5_PrimaryDisbursement ||
                transactionAlias.TransTypeID == TransactionTypes.T25_Shipment)
            {
                if (int.TryParse(currentSite.AutomaticBOLNextNumber, out parsedCurrentNumber) &&
                    parsedCurrentNumber > 0)
                {
                    result = true;
                }
                if (currentSite.SeparateManualBOLNumbering &&
                    int.TryParse(currentSite.ManualBOLNextNumber, out parsedCurrentNumber) &&
                    parsedCurrentNumber > 0)
                {
                    result = true;
                }
            }
            else if (transactionAlias.TransTypeID == TransactionTypes.T17_Order ||
                     transactionAlias.TransTypeID == TransactionTypes.T18_SupplyOrder)
            {
                if (int.TryParse(currentSite.OrderNextNumber, out parsedCurrentNumber) &&
                    parsedCurrentNumber > 0)
                {
                    result = true;
                }
            }
            else
            {
                if (int.TryParse(currentSite.TransactionNextNumber, out parsedCurrentNumber) &&
                    parsedCurrentNumber > 0)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
