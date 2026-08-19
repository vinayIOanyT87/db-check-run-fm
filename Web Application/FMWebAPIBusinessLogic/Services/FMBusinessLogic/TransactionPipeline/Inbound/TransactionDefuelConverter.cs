using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class TransactionDefuelConverter : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T4_SecondaryDefuel)
            {
                return;
            }
            trxDO.TransTypeID = trxAlias.TransTypeID;
            foreach (var lineItem in trxDO.LineItems)
            {
                lineItem.Quantity.Net = Math.Abs(lineItem.Quantity.Net);
                lineItem.Quantity.Gross = Math.Abs(lineItem.Quantity.Gross);
                lineItem.Quantity.Mass = Math.Abs(lineItem.Quantity.Mass);
            }
        }
    }
}
