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
    public class AssignEquipmentToMeterGuidResolver : IPipelineCommand
    {
        private readonly IEquipmentTypesProxy _equipmentTypesProxy;
        private readonly IEquipmentsProxy _equipmentsProxy;
        private readonly ICurrentRequestContext _currentRequestContext;
        public AssignEquipmentToMeterGuidResolver(IEquipmentTypesProxy equipmentTypesProxy,
            IEquipmentsProxy equipmentsProxy, ICurrentRequestContext currentRequestContext)
        {
            _equipmentTypesProxy = equipmentTypesProxy;
            _equipmentsProxy = equipmentsProxy;
            _currentRequestContext = currentRequestContext;
        }
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            //Until we come up with a fool-proof method for determining if the meter has been changed
            //we will have to look up attched equipment every time.

            foreach (LineItemDO lineItem in trxDO.LineItems)
            {
                //lookup equipment attached to this meter.  Populate transaction with equipment data if needed.
                var equipment = _equipmentsProxy.GetByMeterGuid(lineItem.MeterGuid);
                if (equipment != null)
                {
                    trxDO.SourceEQ1.EquipmentGuid = equipment.MasterRecordGuid;
                    trxDO.SourceEQ1.EquipmentTypeGuid = equipment.EquipmentTypeGuid;
                    trxDO.SourceEQ1.RegistrationID = equipment.ID;
                    var equipmentType = _equipmentTypesProxy.Get(equipment.EquipmentTypeGuid);
                    trxDO.SourceEQ1.EquipmentType = equipmentType.Description;

                }
            }
        }
    }
}
