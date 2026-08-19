using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{
	/// <summary>
	/// Use this exception to actually report useful messages, rather than hide them as the SqlException does
	/// </summary>
	public class ExStarsSqlException : ApplicationException
	{
		public ExStarsSqlException(string msg)
			: base(msg)
		{
		}
		public ExStarsSqlException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public ExStarsSqlException(Exception innerException, string fmt, params object[] args)
			: base(string.Format(fmt, args), innerException)
		{
		}

		public ExStarsSqlException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}
	
	}
}
