using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic.TransactionPipeline;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic.TransactionPipeline.Inbound
{
    public class PersonnelGuidResolver : IPipelineCommand
    {
        private readonly IPersonnelProxy _personnelProxy;
        public PersonnelGuidResolver(IPersonnelProxy personnelProxy)
        {
            _personnelProxy = personnelProxy;
        }
        public void Execute(TransactionDO trxDO, TransactionAliasClass trxAlias)
        {
            if (!string.IsNullOrEmpty(trxDO.OperatorID))
            {
                var operatorPerson = this._personnelProxy.GetByID(trxDO.OperatorID);
                if (operatorPerson != null)
                    trxDO.OperatorPersonnelGuid = operatorPerson.MasterRecordGuid;
            }
        }
    }
}
