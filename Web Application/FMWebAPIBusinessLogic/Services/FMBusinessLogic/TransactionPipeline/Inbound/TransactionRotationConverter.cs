using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class TransactionRotationConverter : IPipelineCommand
    {
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            //if it is a meter closeout it is considered a 24Hr ticket
            //otherwise it is a rotation
            if (trxAlias.TransTypeID != TransactionTypes.T12_InventoryNotAffected &&
                trxAlias.MeterCloseout)
            {
                return;
            }
            trxDO.TransTypeID = trxAlias.TransTypeID;
            //Vendor = CarrierID 
            //ShipToID = Consumer
            //OwnerID = Owner
            //Consummer = ShipToID
            if (string.IsNullOrWhiteSpace(trxDO.CarrierID))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(trxDO.OwnerID))
            {
                trxDO.OwnerID = trxDO.CarrierID;
            }
            if (string.IsNullOrWhiteSpace(trxDO.ShipToID))
            {
                trxDO.ShipToID = trxDO.CarrierID;
            }
        }
    }
}
