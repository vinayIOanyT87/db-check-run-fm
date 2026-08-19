using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.Exceptions
{
	[Serializable()]
	public class RetrieveException : Exception
	{
		public RetrieveException( string message )
			: base( message )
		{

		}
	}
}
