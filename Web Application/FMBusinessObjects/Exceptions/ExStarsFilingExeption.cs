using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{
	public class ExStarsFilingException :  ApplicationException
	{
		public ExStarsFilingException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public ExStarsFilingException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}
	}
}
