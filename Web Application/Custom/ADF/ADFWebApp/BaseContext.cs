using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Accounting;
using FMBusinessObjects.DataObjects;

namespace ADFWebApp
{
	public class BaseContext : ICustomContext
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public AccountingSite AcctSite { get; set; }

		protected BaseContext ( )
		{
			ResetContextProperties ( );
		}

		protected void ResetDate ( )
		{
			this.StartDate = DateTime.UtcNow.AddDays ( -1.0 ); // default to seeing the current day only
			this.EndDate = DateTime.UtcNow;
		}

		protected void ResetAccounting ( )
		{
			this.AcctSite = new AccountingSite ( );
		}

		public virtual void ResetContextProperties ( )
		{
			ResetDate ( );
			ResetAccounting ( );
		}

		public virtual string GetKey ( )
		{
			throw new NotImplementedException ( );
		}
	}
}
