namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FMSiteNotFoundException : ApplicationException
	{
		public const string ExceptionMessage = "Site Not Found.";
		public FMSiteNotFoundException() : base( ExceptionMessage ) { }

		public FMSiteNotFoundException( string message )
			: base( message )
		{
		}

		public FMSiteNotFoundException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
	}
}
