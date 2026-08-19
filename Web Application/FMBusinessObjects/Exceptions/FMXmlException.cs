using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class FMXmlException : Exception
	{
		public const string ExceptionMessage = "An error occurred while reading the format of the file: {0}";

		public FMXmlException( XmlException exception )
			: base( string.Format( CultureInfo.CurrentCulture, ExceptionMessage, exception.Message ) ) { }

		public FMXmlException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
	}
}
