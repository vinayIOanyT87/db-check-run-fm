using System;
using System.Runtime.Serialization;


namespace FMBusinessObjects.Exceptions
{

    [Serializable]
    public class FMLicenseException : ApplicationException
    {
        public const string ExceptionMessage = "Invalid License.";
        public const string LicenseHasExpired = "License has expired.";
        public const string LicenseInvalidOrNotInstalled = "License is invalid or not installed.";

        public FMLicenseException(string message = ExceptionMessage) : base(message) { }
        public FMLicenseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
