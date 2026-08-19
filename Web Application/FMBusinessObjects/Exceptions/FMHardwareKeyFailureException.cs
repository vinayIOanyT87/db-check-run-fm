namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FMHardwareKeyFailureException : ApplicationException
	{
		public const string ExceptionMessage = "Hardware Key Failure.";
		public FMHardwareKeyFailureException() : base( ExceptionMessage ) { }
		public FMHardwareKeyFailureException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
	}
}
