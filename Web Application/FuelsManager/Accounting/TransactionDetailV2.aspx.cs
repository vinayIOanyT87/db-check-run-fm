using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMCore;
using FMDepedencyManager;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FuelsManager.FMWebApp;
using FuelsManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FuelsManager.Accounting
{
    using Microsoft.Ajax.Utilities;
    using Unity;
    using Unity.Attributes;

    public partial class TransactionDetailV2 : AccountingWebFormView
    {
        [Dependency]
        public ICurrentRequestContext CurrentRequestContext { get; set; }
        [Dependency]
        public AngularJavaScriptToPageService DynamicLoadJS { get; set; }

        public string TransactionAliasID { get; set; }
        public string ExistingTransactionGuid { get; set; }
        public bool ModifyTransaction { get; set; }
        public string PreviousUrl { get; set; }

        public bool ExtendedAddScenario { get; set; }

        public string Manager { get; set; }
        public string Owner { get; set; }
        public string Product { get; set; }
        public string InventoryDate { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.DynamicLoadJS.DynamicallyLoadJSAndCSSOntoCurrentPage(this, this.security);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            FMWebAPIServiceLocator.Container.BuildUp(this);
            this.Initialize();
            if (this.security == null)
            {
                throw new FMSessionInvalidException();
            }

            var details = ParseQueryString();
            this.TransactionAliasID = details.TransAlias;
            this.ExistingTransactionGuid = details.ExistingTransactionGuid;
            this.ModifyTransaction = details.ModifyTransaction;
            this.PreviousUrl = details.PreviousUrl;
            this.ExtendedAddScenario = details.ExtendedAddScenario;
            this.Manager = details.Manager;
            this.Owner = details.Owner;
            this.Product = details.Product;
            this.InventoryDate = details.InventoryDate;
        }

        private TransactionDetailsParameters ParseQueryString()
        {
            var results = new TransactionDetailsParameters();
            var ledgerInfo = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;
            string transMode = this.Request.GetQueryOrFormValue("TransactionDetailMode");
            if (transMode != null) { transMode = transMode.ToLower(); }
            if (transMode == "add")
            {
                results.TransactionDetailV2Mode = Request.QueryString.Get("TransactionDetailMode");
                results.TransAlias = Request.QueryString.Get("TransAlias");
                results.ExistingTransactionGuid = "";
                results.ModifyTransaction = false;

                if (ledgerInfo != null)
                {
                    results.ExtendedAddScenario = true;
                    results.PreviousUrl     =   ledgerInfo.ReturnURL;
                    results.Manager         =   Request.QueryString["Manager"];
                    results.Owner           =   Request.QueryString["Owner"];
                    results.Product         =   Request.QueryString["Product"];
                    results.InventoryDate   =   Request.QueryString["InventoryDate"];
                }
                else
                {
                    results.PreviousUrl = string.Format(
                        "{0}://{1}{2}", this.Request.Url.Scheme,
                        this.Request.Url.Authority, ResolveUrl( $"~/{this.InlineSessionID}/FMWebApp/FuelsManagerForm.aspx"));
                }

            }
            else
            {
                results.TransactionDetailV2Mode = "New";

                if (ledgerInfo != null)
                {
                    results.TransAlias = ledgerInfo.SelectedTransactionAliasID;
                    results.ExistingTransactionGuid = ledgerInfo.SelectedTransactionGuid.HasValue ? ledgerInfo.SelectedTransactionGuid.ToString() : "";
                    results.ModifyTransaction = true;
                    results.PreviousUrl = ledgerInfo.ReturnURL;
                }
                else
                {
                    results.PreviousUrl = string.Format(
                        "{0}://{1}{2}", this.Request.Url.Scheme,
                        this.Request.Url.Authority, ResolveUrl($"~/{this.InlineSessionID}/FMWebApp/FuelsManagerForm.aspx"));
                }
            }
            this.Session.Remove(TransactionDetailList.TransactionDetailListKey);
            return results;
        }

        public string InlineSessionID
        {
            get
            {
                if (HttpContext.Current.Session.IsCookieless)
                {
                    return "/(S(" + Session.SessionID + "))/";
                }
                return "";
            }
        }

        public class TransactionDetailsParameters
        {
            public string TransactionDetailV2Mode { get; set; }
            public string TransAlias { get; set; }
            public string ExistingTransactionGuid { get; set; }
            public bool ModifyTransaction { get; set; }
            public string PreviousUrl { get; set; }

            public bool ExtendedAddScenario { get; set; }
            public string Manager { get; set; }
            public string Owner { get; set; }
            public string Product { get; set; }
            public string InventoryDate { get; set; }
        }
    }
}