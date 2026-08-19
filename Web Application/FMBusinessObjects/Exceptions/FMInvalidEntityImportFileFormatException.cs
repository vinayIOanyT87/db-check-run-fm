using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class FMInvalidEntityImportFileFormatException : FormatException
	{
		public const string ExceptionMessage = "Invalid import file format.  The selected file does not conform to the FuelsManager Entity Import/Export format.";

		public FMInvalidEntityImportFileFormatException ( ) : base ( ExceptionMessage ) { }

		public FMInvalidEntityImportFileFormatException ( SerializationInfo info, StreamingContext context ) : base ( info, context ) { }
	}
}
