using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class FinanceObjectDoesNotExistException : Exception
	{
		public FinanceObjectDoesNotExistException ( ) : base ( )
		{
		}

		public FinanceObjectDoesNotExistException ( string message ) : base ( message )
		{ 
		}

		public FinanceObjectDoesNotExistException ( string message, Exception innerException ) : base ( message, innerException )
		{ 
		}

		public FinanceObjectDoesNotExistException ( SerializationInfo info, StreamingContext context ) : base ( info, context )
		{ 
		}
	}
}
