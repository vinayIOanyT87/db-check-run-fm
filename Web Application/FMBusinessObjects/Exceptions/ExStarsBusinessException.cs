using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{

	public class ExStarsBusinessException : ApplicationException
	{
		public ExStarsBusinessException(string msg)
			: base(msg)
		{

		}

		public ExStarsBusinessException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}

		public ExStarsBusinessException(Exception innerException, string fmt, params object[] args)
			: base(string.Format(fmt, args), innerException)
		{
		}
	}
}
