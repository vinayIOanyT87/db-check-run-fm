using System.Runtime.Serialization;
using System;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
    public class OrderQtyListSR : AccountingServiceRequest
    {
        [DataMember]
        public Guid TransactionGuid { get; set; }
    }
}
