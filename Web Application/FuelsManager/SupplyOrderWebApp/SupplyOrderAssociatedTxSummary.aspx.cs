namespace FuelsManager.SupplyOrderWebApp
{
	using System;
	using System.Configuration;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Accounting;

	/// <summary>
	/// Summary description for SupplyOrderAssociatedTxSummary.
	/// </summary>
	public partial class SupplyOrderAssociatedTxSummary : AccountingWebFormView
	{
		private AccountingSite accountingSite;
		private ListViewDataSet grid;

		//*************************************************************************
		// Web Form Designer generated code
		//*************************************************************************    

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);

			base.Initialize();

		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TransactionDataGrid.PageIndexChanged += this.TransactionDataGridPageIndexChanged;

		}
		#endregion

		//*************************************************************************
		// Member functions
		//*************************************************************************    

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.PageLoadProcessing();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}

		}


		private void PageLoadProcessing()
		{
			this.accountingSite =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

			this.CleanUpGrid();

			// Bind controls events
			this.BindControls();

			if (this.IsPostBack == false)
			{
				// If the session is null, then remove all objects from the 
				// session and display the accounting error page.
				if (this.Session["Security"] == null)
				{
					this.Session.RemoveAll();
					this.DisplayErrorPage();
					return;
				}

				// Check the user's security access
				this.CheckUserSecurityAccess();

				// Load the header controls
				this.LoadHeaderInformation();

				if (this.Session["SupplyOrderAssociatedTxSummary.CurrentPageIndex"] != null)
				{
					this.TransactionDataGrid.CurrentPageIndex = (int)this.Session["SupplyOrderAssociatedTxSummary.CurrentPageIndex"];
					this.Session.Remove("SupplyOrderAssociatedTxSummary.CurrentPageIndex");
				}

				// Refresh the data in the list view
				this.UpdateView();
			}
		}

		private void LoadHeaderInformation()
		{
			// Get the context object
			var supplyContext = this.Session["SupplyOrderAssociatedTxContext"] as SupplyOrderAssociatedTxContext;

			if (supplyContext != null)
			{
				this.OrderNumberTextBox.Text = supplyContext.OrderNumber;
				this.CustomerOrderNumberTextBox.Text = supplyContext.CustomerOrderNumber;
				this.LineNumberTextBox.Text = supplyContext.LineNumber;
				this.ProductTextBox.Text = supplyContext.Product;
				this.TransactionDateTextBox.Text = supplyContext.TransDate;
			}
		}

		private void CleanUpGrid()
		{
			// Remove all but the first column which is Edit
			while (this.TransactionDataGrid.Columns.Count > 1)
			{
				this.TransactionDataGrid.Columns.RemoveAt(1);
			}

			// Make sure Edit column has translated text
			if (this.Page.Session["UseDataDictionary"] == null
			|| (bool)this.Page.Session["UseDataDictionary"])
			{
				var editText = GetDataDictionaryValueByKey(this.accountingSite.CurrentSiteGuid, "Edit");

				if (this.TransactionDataGrid.Columns[0] != null)
				{
					this.TransactionDataGrid.Columns[0].HeaderText = editText;
				}
			}
		}

		private void BindControls()
		{
			this.TransactionDataGrid.ItemCommand += this.LineItemDataGridItemCommand;
			this.TransactionDataGrid.ItemDataBound += this.LineItemItemDataBound;
		}

		private void LineItemDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				// Get the data object
				var supplyorderAssociatedTxDO = this.Session["SupplyOrderAssociatedTxDO"] as SupplyOrderAssociatedTxDO;
				if (supplyorderAssociatedTxDO == null)
				{
					// No object so we don't know under what context the button was pressed!
					throw new Exception("SupplyOrderAssociatedTxSummary.LineItemDataGrid_ItemCommand expected session to contain SupplyOrderAssociatedTxDO");
				}

				// Create session object for TransactionDetail list of transactions.
				var detailList = new TransactionDetailList();

				int realIndex = (this.TransactionDataGrid.PageSize * this.TransactionDataGrid.CurrentPageIndex) + e.Item.ItemIndex;

				// Get the transaction ID of the currently selected item
				var lineItem = supplyorderAssociatedTxDO.Transactions[realIndex] as SupplyOrderAssociatedTxLineItemDO;
				string transID = lineItem.TransactionID;

				detailList.TransactionIDList.Add(transID);

				// Indicate which transaction id in the list is the one to initially display.
				detailList.CurrentIndex = 0;

				// Indicate the return URL for when the TransactionDetail Close button is clicked.
				detailList.ReturnURL = "..\\SupplyOrderWebApp\\SupplyOrderAssociatedTxSummary.aspx";

				// Put the object into session and transfer to the TransactionDetail.
				this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;
				this.Session["SupplyOrderAssociatedTxSummary.CurrentPageIndex"] = this.TransactionDataGrid.CurrentPageIndex;

				// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
				string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
				this.Redirect("../" + transactionDetailUrl);
			}
		}

		private void LineItemItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.accountingSite.CurrentSite.SiteGroup == false)
			{
				var supplyorderAssociatedTxDO = this.Session["SupplyOrderAssociatedTxDO"] as SupplyOrderAssociatedTxDO;
				if (supplyorderAssociatedTxDO == null)
				{
					// No object so we don't know under what context the button was pressed!
					throw new Exception("SupplyOrderAssociatedTxSummary.LineItemDataGrid_ItemCommand expected session to contain SupplyOrderAssociatedTxDO");
				}

				var editButton = e.Item.FindControl("EditButton") as LinkButton;

				if (editButton != null)
				{
					if (e.Item.ItemIndex > -1 && e.Item.ItemIndex < supplyorderAssociatedTxDO.Transactions.Count)
					{
						var lineItem = supplyorderAssociatedTxDO.Transactions[e.Item.ItemIndex] as SupplyOrderAssociatedTxLineItemDO;

						if (lineItem == null || lineItem.SiteID != this.accountingSite.CurrentSiteName)
						{
							editButton.Enabled = false;
						}

					}
				}
			}
		}


		private void CheckUserSecurityAccess()
		{
			if (!this.security.HasViewTransactionRightByTransTypeID(TransactionTypes.T18_SupplyOrder))
			{
				throw new FMInsufficientRightsException();
			}

		}


		private void UpdateView()
		{
			this.grid = new ListViewDataSet(this.security, LISTVIEW_TYPE.STANDARD,
													ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER_ASSOCIATED_TX), this.accountingSite);

			this.grid.SetDataGrid(this.TransactionDataGrid);

			var sortDirection = false;
			if (this.Session["SupplyOrderAssociatedTxSummary.grid.sort.direction"] != null)
			{
				sortDirection = (bool)this.Session["SupplyOrderAssociatedTxSummary.grid.sort.direction"];
			}

			var sort = this.Session["SupplyOrderAssociatedTxSummary.grid.sort"] as string;
			string sortExpression = "TransDateTime DESC";
			
			if (sort != null)
			{
				sortExpression = this.grid.GetDataPath(sort);
				if (sortExpression == string.Empty)
				{
					sortExpression = "TransDateTime";
				}

				this.grid.Sort = sort;
				this.grid.SortDirection = sortDirection;
				sortExpression += sortDirection ? string.Empty : " DESC";
			}

			this.Session["SupplyOrderAssociatedTxSummary.SortExpression"] = sortExpression;



			// Get the context object
			var context = this.Session["SupplyOrderAssociatedTxContext"] as SupplyOrderAssociatedTxContext;

			if (context == null)
			{
				// No object so we don't know under what context the button was pressed!
				throw new Exception("SupplyOrderAssociatedTxSummary.UpdateView expected session to contain SupplyOrderAssociatedTxContext");
			}

			var sr = new SupplyOrderAssociatedTxSR
			         {
				         SubRequest =
					         SupplyOrderAssociatedTxSR.RequestTypes.GET_ASSOCIATED_TRANSACTIONS,
				         Security = this.security,
				         TransactionLineItemGuid = context.TransactionLineItemGuid,
				         SortExpression =
					         (string)this.Session["SupplyOrderAssociatedTxSummary.SortExpression"]
			         };

			var supplyOrderAssociatedTx =
				FMChannelHelper.MakeCall<ISupplyOrderAssociatedTxProcessor, SupplyOrderAssociatedTxDO>(x => x.Process(sr));

			this.Session["SupplyOrderAssociatedTxDO"] = supplyOrderAssociatedTx;

			// Honor regional settings for date
			this.FormatForRegionalSettings(supplyOrderAssociatedTx);

			this.grid.BindData(supplyOrderAssociatedTx.Transactions, QuantityDisplay.NET, this.accountingSite.CurrentSite._VolumeDecimalPlaces, this.accountingSite.CurrentSite._MassDecimalPlaces, false);
		}

		private void FormatForRegionalSettings(SupplyOrderAssociatedTxDO supplyorderAssociatedTxDO)
		{
			foreach (SupplyOrderAssociatedTxLineItemDO item in supplyorderAssociatedTxDO.Transactions)
			{
				string formattedDate = this.accountingSite.FormatDate(item.TransactionDateTime);
				item.TransactionDate = formattedDate;
			}

		}

		protected void CloseBtnClick(object sender, EventArgs e)
		{
			// Get the context object
			var context = this.Session["SupplyOrderAssociatedTxContext"] as SupplyOrderAssociatedTxContext;

			string resultPage;

			if (context != null && context.ReturnURL != null && context.ReturnURL.Length > 0)
			{
				resultPage = context.ReturnURL;

				if (context.DetailList != null)
				{
					this.Session[TransactionDetailList.TransactionDetailListKey] = context.DetailList;
				}

			}
			else
			{
				resultPage = "..\\FMWebApp\\FuelsManagerForm.aspx";
			}
			this.Session.Remove("SupplyOrderAssociatedTxContext");
			this.Redirect(resultPage);
		}


		private void TransactionDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.TransactionDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.TransactionDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		/// Datas the grid sort command.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridSortCommandEventArgs"/> instance containing the event data.</param>
		private void DataGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			try
			{
				// Find the Sort Data Path
				this.Session["SupplyOrderAssociatedTxSummary.grid.sort"] = e.SortExpression;

				bool sortDirection = false;
				if (this.Session["SupplyOrderAssociatedTxSummary.grid.sort.direction"] != null)
				{
					sortDirection = !((bool)this.Session["SupplyOrderAssociatedTxSummary.grid.sort.direction"]);
				}

				this.Session["SupplyOrderAssociatedTxSummary.grid.sort.direction"] = sortDirection;

				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}
	}
}
