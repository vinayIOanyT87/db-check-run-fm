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
using System.Reflection;

using Accounting;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;

namespace ADFWebApp
{
    #region Context
    public class DemandSelectContext : BaseContext
    {
        protected static string CONTEXT_KEY = typeof(DemandSelectContext).ToString();

        public DemandSelectContext()
            : base()
        {
        }

        public override string GetKey()
        {
            return DemandSelectContext.CONTEXT_KEY;
        }
    }
    #endregion // Context

    public partial class DemandSelectForm : BaseContextPage<DemandSelectContext>
    {
        #region Constructor
        public DemandSelectForm()
            : base(new DemandSelectContext())
        {
        }
        #endregion // Constructor

        protected void Page_Load(object sender, EventArgs e)
        {
           if (Session["Security"] == null)
              base.ErrorHandler(new FMSessionInvalidException());
        }

        protected void UpdateView()
        {
        }

        protected TransactionDOCollection EnumerateByContext(DemandSelectContext a_context)
        {
            TransactionDOCollection result = new TransactionDOCollection();

            try
            {
                /*Common.EnumerateByContext(a_context, base.security, a_context.AcctSite,
                        new Common.FilterBuilderDelegate(FilterBuilder),
                        new Common.InlineFilterDelegate(FilterResults));*/
            }
            catch (Exception e)
            {
                base.ErrorHandler(e);
            }

            return result;
        }

        protected DataView BuildDataView(TransactionDOCollection a_collection)
        {
            return null; // placeholder
        }
    }
}
