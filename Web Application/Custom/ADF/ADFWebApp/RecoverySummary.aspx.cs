using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace ADFWebApp
{
	using FuelsManager.InvoiceWebApp;

	public partial class RecoverySummary : InvoiceSummary
    {
        #region Attributes
        #region Constants
        public static string CUSTOM_REDIRECT_PARAM = "customRedirect";        
        #endregion // Constants

        protected string m_customRedirectUrl = "";

        #endregion // Attributes

        protected override void Page_Load(object sender, EventArgs e)
        {
            // TBC, I actually probably don't need this

            m_customRedirectUrl = Request.Params[RecoverySummary.CUSTOM_REDIRECT_PARAM];
            if (m_customRedirectUrl == Request.Params[RecoverySummary.CUSTOM_REDIRECT_PARAM])
            {
                base.ErrorHandler(new ArgumentNullException("cannot find custom redirection parameter"));
            }

            base.Page_Load(sender, e);
        }
    }
}
