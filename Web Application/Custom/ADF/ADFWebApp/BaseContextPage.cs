using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Reflection;

using Accounting;

namespace ADFWebApp
{
	public class BaseContextPage<T> : AccountingWebFormView, ICustomContextOperation<T> where T : ICustomContext
	{
		#region Attributes
		protected T context;
		#endregion // Attributes

		#region Constructor
		public BaseContextPage ( T inContext )
			: base ( )
		{
			this.context = inContext;
		}
		#endregion // Constructor

		#region Context Operations
		public virtual T GetContext ( )
		{
			ICustomContext existingContext = Session[context.GetKey ( )] as ICustomContext;
			if (null != existingContext)
			{
				context = (T) existingContext;
			}

			return context;
		}

		public virtual void StoreContext ( Object inContext )
		{
			context = (T) inContext;
			if (null == context)
			{
				throw new InvalidCastException ( "Could not cast " + inContext.ToString ( ) + " to " + context.ToString ( ) );
			}

			Session[context.GetKey ( )] = inContext;
			context = (T) inContext;
		}

		public virtual T LoadToContext ( ref Object inContext )
		{
			throw new NotImplementedException ( MethodBase.GetCurrentMethod ( ).ToString ( ) + " must be overriden" );
		}

		public virtual void LoadFromContext ( T inContext )
		{
			throw new NotImplementedException ( MethodBase.GetCurrentMethod ( ).ToString ( ) + " must be overriden" );
		}

		#endregion // Context Operations
	}
}
