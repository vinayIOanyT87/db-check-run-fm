using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class FinanceObjectExistsException : Exception
	{
		public FinanceObjectExistsException ( ) : base ( )
		{
		}

		public FinanceObjectExistsException ( string message ) : base ( message )
		{ 
		}

		public FinanceObjectExistsException ( string message, Exception innerException ) : base ( message, innerException )
		{ 
		}

		public FinanceObjectExistsException ( SerializationInfo info, StreamingContext context ) : base ( info, context )
		{ 
		}
	}
}
