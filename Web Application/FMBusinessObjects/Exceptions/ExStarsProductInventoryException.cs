using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{
	public class ExStarsProductInventoryException : ApplicationException
	{
		public ExStarsProductInventoryException(string msg)
			: base(msg)
		{
		}

		public ExStarsProductInventoryException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}
	}
}
