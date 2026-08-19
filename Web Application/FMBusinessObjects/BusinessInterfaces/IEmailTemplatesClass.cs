using DocumentFormat.OpenXml.Spreadsheet;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.BusinessInterfaces
{
   [ServiceContract]
   public interface IEmailTemplatesClass
   {

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      Guid Add(SecurityClass security, EmailTemplateClass emailTemplate);

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      void Modify(SecurityClass security, EmailTemplateClass emailTemplate);

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      void Purge(SecurityClass security, Guid emailTemplateGuid);

      [OperationContract]
      EmailTemplateClass Get(SecurityClass security, Guid emailTemplateGuid);

      [OperationContract]
      EmailTemplateClass GetByAlarmAndEvent(SecurityClass security, Guid alarmAndEventGuid);

   }
}
