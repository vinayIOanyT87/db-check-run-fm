namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FMPasswordException : Exception
	{
		public const string ExceptionMessage = "Password Invalid.";
		public FMPasswordException() : base( ExceptionMessage ) { }

		public FMPasswordException( string message )
			: base( message )
		{
		}

		public FMPasswordException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
	}
}
