using FMBusinessObjects.DataObjects;
using FMDepedencyManager;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FuelsManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Attributes;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity;

namespace FuelsManager.FMWebApp
{
    public partial class TransactionAliasFieldPlacementPage : TransactionAliasPageBase
    {
        [Dependency]
        public ICurrentRequestContext CurrentRequestContext { get; set; }
        [Dependency]
        public AngularJavaScriptToPageService DynamicLoadJS { get; set; }

        public string TransactionAliasID { get; set; }
        public string[] Keys(SecurityClass security)
        {
            string[] keys = { };
            return keys;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FMWebAPIServiceLocator.Container.BuildUp(this);
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            this.TransactionAliasID = transactionAlias.ID;
            this.DynamicLoadJS.DynamicallyLoadJSAndCSSOntoCurrentPage(this, this.CurrentRequestContext.GetCurrentSecurityContext());
        }
    }
}