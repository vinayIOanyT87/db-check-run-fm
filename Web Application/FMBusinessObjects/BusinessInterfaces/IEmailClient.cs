using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
    [ServiceContract]
    public interface IEmailClient
    {
        [OperationContract]
        bool EmailUserByGuid(SecurityClass security, string subjectText, string messageText, Guid fromUserGuid, Guid toUserGuid);

        [OperationContract]
        bool EmailUserById(SecurityClass security, string subjectText, string messageText, string fromUserId, string toUserId);

        [OperationContract]
        bool EmailUser(SecurityClass security, string subjectText, string messageText, string fromEmailAddress, string toEmailAddress);

        [OperationContract]
        void SendExpiredLicenceEmail();
    }
}
