using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    using FMBusinessObjects.DataObjects;

    using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;

    public class Transaction24HourConverter : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T12_InventoryNotAffected &&
                !trxAlias.MeterCloseout)
            {
                return;
            }
            trxDO.TransTypeID = trxAlias.TransTypeID;
            foreach (var lineItem in trxDO.LineItems)
            {
                if (!lineItem.MeterReading.MeterStart.HasValue && 
                    lineItem.MeterReading.MeterStop.HasValue)
                {
                    lineItem.MeterReading.MeterStart = lineItem.MeterReading.MeterStop;
                }
                else if (lineItem.MeterReading.MeterStart.HasValue && 
                         !lineItem.MeterReading.MeterStop.HasValue)
                {
                    lineItem.MeterReading.MeterStop = lineItem.MeterReading.MeterStart;
                }
            }
        }
    }
}
