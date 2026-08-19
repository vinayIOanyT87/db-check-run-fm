using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FMNotificationBusinessObjects.BusinessInterfaces
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IEnterpriseExportError
    {

        [OperationContract]
        ReceiveErrorResponse ReceiveErrors(string site, string filename, string data);
    }

    [DataContract]
    public class ReceiveErrorResponse
    {
        bool operationSucessful;
        string operationErrorText;

        [DataMember]
        public bool OperationSuccessful
        {
            get { return operationSucessful; }
            set { operationSucessful = value; }
        }
        [DataMember]
        public string OperationErrorText
        {
            get { return operationErrorText; }
            set { operationErrorText = value; }
        }

        public ReceiveErrorResponse()
        {
            operationSucessful = true;
            operationErrorText = "OK";
        }
    }
}
