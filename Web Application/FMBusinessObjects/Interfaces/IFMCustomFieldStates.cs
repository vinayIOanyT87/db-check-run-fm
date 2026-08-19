using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.Interfaces
{
	public interface IFMCustomFieldStates
	{
		void SetTransactionFieldStates ( SecurityClass security, System.Web.UI.Page page );
		void SetTransactionFieldState ( SecurityClass security, System.Web.UI.WebControls.WebControl control );
	}
}
