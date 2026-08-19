using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{
	public class ExStarsDataValidationException: ApplicationException
	{
		public ExStarsDataValidationException(string msg)
			: base(msg)
		{

		}

		public ExStarsDataValidationException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{

		}
	}
}
