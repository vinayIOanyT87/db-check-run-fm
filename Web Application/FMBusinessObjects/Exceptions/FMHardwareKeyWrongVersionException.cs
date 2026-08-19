namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FMHardwareKeyWrongVersionException : ApplicationException
	{
		public const string ExceptionMessage = "Hardware Key Failure.";
		public FMHardwareKeyWrongVersionException() : base( ExceptionMessage ) { }

		public FMHardwareKeyWrongVersionException(string message)
			: base(message)
		{
		}

		public FMHardwareKeyWrongVersionException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
	}
}
