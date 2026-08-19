using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Exceptions
{
	public class CompanyRoleMapCollectionException : ApplicationException
	{
		public CompanyRoleMapCollectionException(string msg)
			: base(msg)
		{
		}

		public CompanyRoleMapCollectionException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}

}
