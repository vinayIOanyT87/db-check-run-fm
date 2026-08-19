// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderAssociatedTxSummary.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.OrderEntryWebApp
{
	using System;
	using System.Configuration;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Accounting;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	///     Summary description for OrderAssociatedTxSummary.
	/// </summary>
	public partial class OrderAssociatedTxSummary : AccountingWebFormView
	{
		#region Constants and Fields

		private AccountingSite accountingSite;

		#endregion

		#region Methods

		protected void CloseBtnClick(object sender, EventArgs e)
		{
			// Get the context object
			var context = this.Session["OrderAssociatedTxContext"] as OrderAssociatedTxContext;

			string sResultPage;

			if (context != null && context.ReturnURL != null && context.ReturnURL.Length > 0)
			{
				sResultPage = context.ReturnURL;

				if (context.DetailList != null)
				{
					this.Session[TransactionDetailList.TransactionDetailListKey] = context.DetailList;
				}
            }
			else
			{
				sResultPage = "..\\OrderEntryWebApp\\OrderEntrySplash.aspx";
			}
            this.Session.Remove("OrderAssociatedTxContext");
            this.Redirect(sResultPage);
		}

		// *************************************************************************
		// Web Form Designer generated code
		// *************************************************************************    

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);

			this.Initialize();
		}

		// *************************************************************************
		// Member functions
		// *************************************************************************    

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

		private void BindControls()
		{
			this.TransactionDataGrid.ItemCommand += this.LineItemDataGridItemCommand;
			this.TransactionDataGrid.ItemDataBound += this.LineItemItemDataBound;
			this.TransactionDataGrid.SortCommand += this.DataGridSortCommand;
		}

		private void CheckUserSecurityAccess()
		{
			if (!this.security.HasViewTransactionRightByTransTypeID(TransactionTypes.T17_Order))
			{
				throw new FMInsufficientRightsException();
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
			if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
			{
				string editText = GetDataDictionaryValueByKey(this.accountingSite.CurrentSiteGuid, "Edit");

				if (this.TransactionDataGrid.Columns[0] != null)
				{
					this.TransactionDataGrid.Columns[0].HeaderText = editText;
				}
			}
		}

		private void FormatForRegionalSettings(OrderAssociatedTxDO orderAssociatedTxDO)
		{
			foreach (OrderAssociatedTxLineItemDO item in orderAssociatedTxDO.Transactions)
			{
				string formattedDate = this.accountingSite.FormatDate(item.TransactionDateTime);
				item.TransactionDate = formattedDate;
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TransactionDataGrid.PageIndexChanged += this.TransactionDataGridPageIndexChanged;
		}

		/// <summary>
		/// Lines the item data grid item command.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void LineItemDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				// Get the data object
				var orderAssociatedTxDO = this.Session["OrderAssociatedTxDO"] as OrderAssociatedTxDO;
				if (orderAssociatedTxDO == null)
				{
					// No object so we don't know under what context the button was pressed!
					throw new Exception(
						"OrderAssociatedTxSummary.LineItemDataGrid_ItemCommand expected session to contain OrderAssociatedTxDO");
				}

				// Create session object for TransactionDetail list of transactions.
				var detailList = new TransactionDetailList();

				int realIndex = (this.TransactionDataGrid.PageSize * this.TransactionDataGrid.CurrentPageIndex) + e.Item.ItemIndex;

				// Get the transaction ID of the currently selected item
				var lineItem = orderAssociatedTxDO.Transactions[realIndex] as OrderAssociatedTxLineItemDO;
				if (lineItem != null)
				{
					string transID = lineItem.TransactionID;
					detailList.TransactionIDList.Add(transID);
				}

				// Indicate which transaction id in the list is the one to initially display.
				detailList.CurrentIndex = 0;

				// Indicate the return URL for when the TransactionDetail Close button is clicked.
				detailList.ReturnURL = "..\\OrderEntryWebApp\\OrderAssociatedTxSummary.aspx";

				// Put the object into session and transfer to the TransactionDetail.
				this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;
				this.Session["OrderAssociatedTxSummary.CurrentPageIndex"] = this.TransactionDataGrid.CurrentPageIndex;

				// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
				string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

				this.Redirect("../" + transactionDetailUrl);
			}
		}

		/// <summary>
		/// Handles the ItemDataBound event of the LineItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
		private void LineItemItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.accountingSite.CurrentSite.SiteGroup == false)
			{
				var orderAssociatedTxDO = this.Session["OrderAssociatedTxDO"] as OrderAssociatedTxDO;
				if (orderAssociatedTxDO == null)
				{
					// No object so we don't know under what context the button was pressed!
					throw new Exception(
						"OrderAssociatedTxSummary.LineItemDataGrid_ItemCommand expected session to contain OrderAssociatedTxDO");
				}

				var editButton = e.Item.FindControl("EditButton") as LinkButton;

				if (editButton != null)
				{
					if (e.Item.ItemIndex > -1 && e.Item.ItemIndex < orderAssociatedTxDO.Transactions.Count)
					{
						var lineItem = orderAssociatedTxDO.Transactions[e.Item.ItemIndex] as OrderAssociatedTxLineItemDO;

						if (lineItem != null && lineItem.SiteID != this.accountingSite.CurrentSiteName)
						{
							editButton.Enabled = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Loads the header information.
		/// </summary>
		private void LoadHeaderInformation()
		{
			// Get the context object
			var context = this.Session["OrderAssociatedTxContext"] as OrderAssociatedTxContext;

			if (context != null)
			{
				this.OrderNumberTextBox.Text = context.OrderNumber;
				this.CustomerOrderNumberTextBox.Text = context.CustomerOrderNumber;
				this.LineNumberTextBox.Text = context.LineNumber;
				this.ProductTextBox.Text = context.Product;
				this.TransactionDateTextBox.Text = context.TransDate;
			}
		}

		private void PageLoadProcessing()
		{
			this.accountingSite =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

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

				if (this.Session["OrderAssociatedTxSummary.CurrentPageIndex"] != null)
				{
					this.TransactionDataGrid.CurrentPageIndex = (int)this.Session["OrderAssociatedTxSummary.CurrentPageIndex"];
					this.Session.Remove("OrderAssociatedTxSummary.CurrentPageIndex");
				}

				// Refresh the data in the list view
				this.UpdateView();
			}
		}

		/// <summary>
		/// Handles the PageIndexChanged event of the TransactionDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.</param>
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

		private void UpdateView()
		{
			// Get the context object
			var context = this.Session["OrderAssociatedTxContext"] as OrderAssociatedTxContext;

			var sr = new OrderAssociatedTxSR
				{
					SubRequest = OrderAssociatedTxSR.RequestTypes.GET_ASSOCIATED_TRANSACTIONS,
					Security = this.security
				};

			if (context != null)
			{
				sr.TransactionLineItemGuid = context.TransactionLineItemGuid;
			}
			sr.SortExpression = (string)this.Session["OrderAssociatedTxSummary.SortExpression"];

			var orderAssociatedTxDO =
				FMChannelHelper.MakeCall<IOrderAssociatedTxProcessor, OrderAssociatedTxDO>(x => x.Process(sr));

			this.Session["OrderAssociatedTxDO"] = orderAssociatedTxDO;

			// Honor regional settings for date
			this.FormatForRegionalSettings(orderAssociatedTxDO);

			var grid = new ListViewDataSet(
				this.security,
				LISTVIEW_TYPE.STANDARD,
				ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ORDER_ASSOCIATED_TX),
				this.accountingSite);

			grid.SetDataGrid(this.TransactionDataGrid);

			grid.BindData(
				orderAssociatedTxDO.Transactions,
				QuantityDisplay.NET,
				this.accountingSite.CurrentSite._VolumeDecimalPlaces,
				this.accountingSite.CurrentSite._MassDecimalPlaces,
				false);

			// Save the data object for later
			this.Session["OrderAssociatedTxSummary.ListView"] = grid;
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
				this.Session["OrderAssociatedTxSummary.grid.sort"] = e.SortExpression;

				bool sortDirection = false;
				if (this.Session["OrderAssociatedTxSummary.grid.sort.direction"] != null)
				{
					sortDirection = !((bool)this.Session["OrderAssociatedTxSummary.grid.sort.direction"]);
				}

				this.Session["OrderAssociatedTxSummary.grid.sort.direction"] = sortDirection;

				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		#endregion
	}
}