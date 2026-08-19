using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class NoDataToSendException : Exception, ISerializable
	{

		public NoDataToSendException ( ) 
		{ 
		}

		public NoDataToSendException ( string message ) : base ( message ) 
		{ 
		}

		public NoDataToSendException ( string message, Exception innerException ) : base ( message, innerException ) 
		{ 
		}
	}
}
