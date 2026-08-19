using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class FMFileAlreadyImportedException : ApplicationException
	{
		public const string ExceptionMessage = "The selected file has already been imported.  Re-importing could cause undesired data changes.";

		public FMFileAlreadyImportedException ( ) : base ( ExceptionMessage ) { }

		public FMFileAlreadyImportedException ( SerializationInfo info, StreamingContext context ) : base ( info, context ) { }
	}
}
