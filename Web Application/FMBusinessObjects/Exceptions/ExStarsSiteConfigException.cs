using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{
	public class ExStarsSiteConfigException : ApplicationException
	{
		public ExStarsSiteConfigException() : base() { }
		public ExStarsSiteConfigException(string msg) : base(msg) { }
		public ExStarsSiteConfigException(string fldName, int minLen, int maxLen)
			: base(string.Format("Field {0} must have a length between {1} and {2}", fldName, minLen, maxLen))
		{
		}

		public ExStarsSiteConfigException(string fmt, params object[] args)
			: base(string.Format(fmt, args))
		{
		}

		public ExStarsSiteConfigException(Exception innerException, string fmt, params object[] args)
			: base(string.Format(fmt, args), innerException)
		{
		}
	}
}
