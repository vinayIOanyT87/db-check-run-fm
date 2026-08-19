using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class EquipmentGuidResolver : IPipelineCommand
    {
        private readonly IEquipmentsProxy _equipmentsProxy;
        private readonly ICurrentRequestContext _currentRequestContext;
        public EquipmentGuidResolver(IEquipmentsProxy equipmentsProxy, ICurrentRequestContext currentRequestContext)
        {
            _equipmentsProxy = equipmentsProxy;
            _currentRequestContext = currentRequestContext;
        }
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            Type t = trxDO.GetType();
            //Apparently what I am looking for are properties, not fields
            //FieldInfo[] fields = t.GetFields();
            //foreach (FieldInfo fi in fields)
            //{
            //    if (fi.GetType() == typeof(EquipmentDO))
            //    {
            //        var val = ((EquipmentDO)fi.GetValue(trxDO));
            //        LookupGuidFromRegistrationId(((EquipmentDO)fi.GetValue(trxDO)));
            //    }
            //}
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                if (pi.PropertyType == typeof(EquipmentDO))
                {
                    var val = (pi.GetValue(trxDO, null));
                    LookupGuidFromRegistrationId((EquipmentDO)(pi.GetValue(trxDO, null)));
                }
            }

        }

        private void LookupGuidFromRegistrationId(EquipmentDO eqDO)
        {
            if (eqDO != null && !string.IsNullOrEmpty(eqDO.RegistrationID))
            {
                var equipmentGuid = _equipmentsProxy.GetIdentityGuid(eqDO.RegistrationID);
                eqDO.EquipmentGuid = equipmentGuid;

            }
        }
    }
}
