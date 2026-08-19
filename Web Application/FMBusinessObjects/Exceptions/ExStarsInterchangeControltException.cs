using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{

	public class ExStarsInterchangeControltException : ApplicationException
	{
		public ExStarsInterchangeControltException(string msg)
			: base(msg)
		{
		}

		public ExStarsInterchangeControltException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}
}
