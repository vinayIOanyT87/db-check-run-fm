using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;


namespace FMBusinessObjects.BusinessInterfaces
{
   [ServiceContract]
   public interface IArchiveDataProcessor
   {
      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      string Process(ArchiveDataSR sr);
   }
}
