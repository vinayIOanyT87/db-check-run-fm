using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{
	public class ProductClassException : ApplicationException
	{
		public ProductClassException(string msg)
			: base(msg)
		{
		}

		public ProductClassException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}

}
