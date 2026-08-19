using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Outbound
{
    public class TransactionTransferConverter : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T13_OwnerTransfer)
            {
                return;
            }     
            //lets move over the transaction fields over to the from fields
            if (!string.IsNullOrWhiteSpace(trxDO.OwnerID) && string.IsNullOrWhiteSpace(trxDO.FromOwnerID))
            {
                trxDO.FromOwnerID = trxDO.OwnerID;
            }
            if (!string.IsNullOrWhiteSpace(trxDO.ManagerID) && string.IsNullOrWhiteSpace(trxDO.FromManagerID))
            {
                trxDO.FromManagerID = trxDO.ManagerID;
            }
            if (!string.IsNullOrWhiteSpace(trxDO.CarrierID) && string.IsNullOrWhiteSpace(trxDO.FromCarrierID))
            {
                trxDO.FromCarrierID = trxDO.CarrierID;
            }
        }
    }
}
