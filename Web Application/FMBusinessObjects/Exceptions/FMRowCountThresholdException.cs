using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class FMRowCountThresholdException : ApplicationException
	{
		public const string ExceptionMessage = "{0} has exceeded {1}% of alloted capacity.  Please contact the system administrator.";

		public FMRowCountThresholdException ( string tableName, string currentPercentage )
			: base ( string.Format ( CultureInfo.CurrentCulture, ExceptionMessage, tableName, currentPercentage ) ) { }

		public FMRowCountThresholdException ( SerializationInfo info, StreamingContext context ) : base ( info, context ) { }
	}
}
