using System;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
    [ServiceContract]
    public interface IIntoPlaneImport
    {
        [OperationContract]
        string ImportData(SecurityClass sec, string data, IntoPlaneImportParametersDO parameters);
    }
}
