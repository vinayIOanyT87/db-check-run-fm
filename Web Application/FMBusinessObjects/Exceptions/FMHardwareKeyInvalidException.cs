namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FMHardwareKeyInvalidException : ApplicationException
	{
		public const string ExceptionMessage = "Could not detect the hardware key.  Please check the key and restart the application.";
		public FMHardwareKeyInvalidException ( ) : base ( ExceptionMessage ) { }
		public FMHardwareKeyInvalidException ( SerializationInfo info, StreamingContext context ) : base ( info, context ) { }
	}
}
