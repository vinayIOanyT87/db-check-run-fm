using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMBusinessObjects.DataObjects;
using System.ServiceModel;
using System.Data.SqlClient;

namespace FMBusinessObjects.BusinessInterfaces
{

   [ServiceContract]
   public interface ISiteCloseoutTimes
   {
      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      Guid Add(SecurityClass security, SiteCloseoutTimeClass siteCloseoutTime);

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      void Modify(SecurityClass security, SiteCloseoutTimeClass siteCloseoutTime);

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      void Purge(SecurityClass security, Guid siteCloseoutTimeGuid);

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      void PurgeBySiteGuid(SecurityClass security, Guid siteGuid);

      [OperationContract]
      SiteCloseoutTimeClass Get(SecurityClass security, Guid siteCloseoutTimeGuid);

      [OperationContract]
      SiteCloseoutTimeCollectionClass EnumerateBySiteGuid(SecurityClass security, Guid siteGuid);

      [OperationContract]
      SiteCloseoutTimeCollectionClass EnumerateBySiteGuidAndDate(SecurityClass security, Guid siteGuid, DateTimeOffset date);

      [OperationContract]
      void SetCloseoutTime(SecurityClass security, SiteCloseoutTimeClass siteCloseoutTime);

      [OperationContract]
      TimeSpan GetCloseoutTime(SecurityClass security, DateTimeOffset date);
   }
}
