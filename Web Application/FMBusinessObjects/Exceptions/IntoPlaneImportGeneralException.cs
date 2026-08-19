using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.Exceptions
{
    [Serializable]
    public class IntoPlaneImportGeneralException : Exception
    {
        public IntoPlaneImportGeneralException()
            : base()
        {
        }

        public IntoPlaneImportGeneralException(string message)
            : base(message)
        {
        }

        public IntoPlaneImportGeneralException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IntoPlaneImportGeneralException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
