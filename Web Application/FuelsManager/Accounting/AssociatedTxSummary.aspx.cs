namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Accounting;

    /// <summary>
	/// Summary description for InvoiceAssociatedTxSummary.
	/// </summary>
	public partial class AssociatedTxSummary : AccountingWebFormView
	{
		private AccountingSite accountingSite = null;
		private ListViewDataSet grid = null;
		private TransactionDO trans;
		private string transID;
		private LineItemDO lineItem;
		private BaseCollections associatedTransactions = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Initialize();

			// Initialize the accounting site
			this.accountingSite = 	FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

			// Get the context object
			var associatedTxContext = this.Session["AssociatedTxContext"] as AssociatedTxContext;

			// Grab the ine item index from the querystring
			if (associatedTxContext != null)
			{
				this.trans = associatedTxContext.transaction;
			}

            transID = this.trans.TransID;
            this.lineItem = this.GetLineItem();

			if (!this.IsPostBack)
			{
				this.CleanUpGrid();
				this.UpdateView();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgTransactions.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DgTransactionsItemCommand);

		}
		#endregion

		private void CleanUpGrid()
		{
			if (this.dgTransactions.Columns.Count > 1)
			{
				this.dgTransactions.Columns.RemoveAt(1);
			}

			if (this.dgTransactions.Columns[0] != null)
			{
				string editText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.security.SiteGuid, "Edit")
																);
				this.dgTransactions.Columns[0].HeaderText = editText;
			}
		}

		private void UpdateView()
		{
			if (this.lineItem == null)
				return;

			// Create and populate the request object
			var sr = new AssociatedTxSR
			         {
				         RequestType = AssociatedTxSR.RequestTypes.GetAssociatedTransactions,
				         TransID = this.trans.TransID,
				         TransactionLineItemGuid = this.lineItem.TransactionLineItemGuid,
				         Product = this.lineItem.Product,
				         TransactionAliasGuid = this.trans.TransactionAliasGuid,
				         Security = this.security
			         };

			// get the associated transaction ids and line item ids
			foreach (AssociatedTxDO txDo in this.lineItem.AssociatedTransactions)
			{
				if (txDo.Associated == 1)
				{
					sr.AssociatedTransactionIDs.Add(txDo);
				}
			}

			// Retrieve the list of associated transactions
			AssociatedTxListDO txList = FMChannelHelper.MakeCall<IAssociatedTxProcessor, AssociatedTxListDO>(x => x.Process(sr));

			Guid standardTypeGuid;

			if (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.RECOVERY_ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T9_Request)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder ||
						this.trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER_ASSOCIATED_TX);
			}
			else if (this.trans.TransTypeID == TransactionTypes.T17_Order)
			{
				standardTypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ORDER_ASSOCIATED_TX);
			}
			else
			{
				throw new ApplicationException("Unknown Associated Transaction Type ID Encountered");
			}

			this.grid = new ListViewDataSet(this.security, LISTVIEW_TYPE.STANDARD,
																		standardTypeGuid, this.accountingSite);

			this.grid.SetDataGrid(this.dgTransactions);

			// Iterate through the dataset and create a BaseCollections object that
			// can be used by ListViews
			this.associatedTransactions = new BaseCollections();
			this.Session["associatedTransactions"] = this.associatedTransactions;
			var transIDs = new StringCollection();

			if (txList.AssociatedTransactions.Tables.Count > 0)
			{
				foreach (DataRow dr in txList.AssociatedTransactions.Tables[0].Rows)
				{
					var txDo = new AssociatedTxDO(this.accountingSite.CurrentSite.GetDateTimeFormatInfo());
					txDo.Load(dr);

					if (!transIDs.Contains(txDo.TransID))
					{
						this.associatedTransactions.Add(txDo);
						transIDs.Add(txDo.TransID);
					}
				}
			}

			this.grid.BindData(	this.associatedTransactions, 
								QuantityDisplay.NET, 
								this.accountingSite.CurrentSite._VolumeDecimalPlaces, 
								this.accountingSite.CurrentSite._MassDecimalPlaces, 
								false);
		}

		private void DgTransactionsItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				this.associatedTransactions = this.Session["associatedTransactions"] as BaseCollections;

				if (this.associatedTransactions != null)
				{
					int realIndex = (this.dgTransactions.CurrentPageIndex * this.dgTransactions.PageCount) + e.Item.ItemIndex;
					string assocTransID = ((AssociatedTxDO)this.associatedTransactions[realIndex]).TransID;
					this.Session.Remove("associatedTransactions");

					// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
					string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
					this.Redirect("../" 
									+ transactionDetailUrl 
									+ "?KEEPASSOCCONTEXT=true&TransID=" 
									+ assocTransID 
									+ "&" 
									+ TransactionDetailBase.ModeKey 
									+ "=View");
				}
			}
		}

		private LineItemDO GetLineItem()
		{
			var associatedTxContext = this.Session["AssociatedTxContext"] as AssociatedTxContext;

			foreach (LineItemDO lineItemDO in this.trans.LineItems)
			{
				if (associatedTxContext != null 
					&& lineItemDO.TransactionLineItemGuid.ToString() == associatedTxContext.TransactionLineItemGuid)
				{
					return lineItemDO;
				}
			}

			return this.lineItem;
		}

		protected void BtnCloseClick(object sender, EventArgs e)
		{

			this.Session.Remove("associatedTransactions");
			// Get the context object
			var associatedTxContext = this.Session["AssociatedTxContext"] as AssociatedTxContext;

			string resultPage = string.Empty;

			if (associatedTxContext != null && associatedTxContext.ReturnURL != null && associatedTxContext.ReturnURL.Length > 0)
			{
				resultPage = associatedTxContext.ReturnURL;
			}

			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

			if (string.IsNullOrEmpty(transactionDetailUrl) == false)
			{
				if (transactionDetailUrl.ToUpper().Contains("ACCOUNTING") == false)
				{
					string[] urlSplit = transactionDetailUrl.Split('/');

					if (urlSplit.Length > 0)
					{
						string appName = urlSplit[0];
						resultPage = "../" + appName + "/" + resultPage;
					}
				}
			}

			this.Redirect(resultPage);
		}
	}
}
