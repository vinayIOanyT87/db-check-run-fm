using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class Rotation24HourDestinationEquipmentResolver : IPipelineCommand
    {
        private readonly IEquipmentTypesProxy _equipmentTypesProxy;
        private readonly IEquipmentsProxy _equipmentsProxy;
        private readonly ICurrentRequestContext _currentRequestContext;
        public Rotation24HourDestinationEquipmentResolver(IEquipmentTypesProxy equipmentTypesProxy,
            IEquipmentsProxy equipmentsProxy, ICurrentRequestContext currentRequestContext)
        {
            _equipmentTypesProxy = equipmentTypesProxy;
            _equipmentsProxy = equipmentsProxy;
            _currentRequestContext = currentRequestContext;
        }

        /// <summary>
        /// For Rotations and 24 Hour Tickets, the Destination is the same as the Source
        /// </summary>
        /// <param name="trxDO">The inbound Transaction object</param>
        /// <param name="trxAlias">The Transaction Alias</param>
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (trxAlias.TransTypeID != TransactionTypes.T12_InventoryNotAffected)
            {
                return;
            }
            var userSite = _currentRequestContext.GetCurrentSite();
            foreach (LineItemDO lineItem in trxDO.LineItems)
            {
                //lookup equipment attached to this meter.  Populate transaction with equipment data if needed.
                var equipment = _equipmentsProxy.GetByMeterGuid(lineItem.MeterGuid);
                if (equipment != null)
                {
                    trxDO.DestinationEQ1.EquipmentGuid = equipment.MasterRecordGuid;
                    trxDO.DestinationEQ1.EquipmentTypeGuid = equipment.EquipmentTypeGuid;
                    trxDO.DestinationEQ1.RegistrationID = equipment.ID;
                    var equipmentType = _equipmentTypesProxy.Get(equipment.EquipmentTypeGuid);
                    trxDO.DestinationEQ1.EquipmentType = equipmentType.Description;

                }
            }
        }
    }
}
