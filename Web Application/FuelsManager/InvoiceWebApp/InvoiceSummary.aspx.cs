// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvoiceSummary.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for Invoice Summary page
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.InvoiceWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.Exceptions;

	using FuelsManager.Accounting;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
    using FMCore;

	/// <summary>
	/// Code behind for Invoice Summary page
	/// </summary>
	public partial class InvoiceSummary : AccountingAutoSubmitWebFormView
	{
		#region Constants and Fields

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string InvoiceListGridSort = "InvoiceList.grid.sort";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string InvoiceListSortDirection = "InvoiceList.grid.sort.direction";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string InvoiceListSortExpression = "InvoiceList.SortExpression";

		/// <summary>
		/// The invoice summary all text with brackets
		/// </summary>
		private const string InvoiceSummaryAllText = "{All}";

		/// <summary>
		/// The invoice summary all text
		/// </summary>
		private const string InvoiceSummaryAllValue = "ALL";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string InvoiceSummaryContext = "InvoiceSummaryContext";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string InvoiceSummaryListDO = "InvoiceList.InvoiceListDO";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string InvoiceSummarySecurity = "Security";

		/// <summary>
		/// Session storage key
		/// </summary>
		private const string SessionInvoiceSummaryType = "InvoiceSummaryType";

		/// <summary>
		/// The accounting site object
		/// </summary>
		private AccountingSite accountingSite;

		/// <summary>
		/// The list view grid
		/// </summary>
		private ListViewDataSet listViewGrid;
	
		#endregion

		#region Methods

		/// <summary>
		/// Gets the type of the list view.
		/// </summary>
		/// <returns>A list view data set.</returns>
		/// <exception cref="System.ApplicationException">Unexpected list view type specified</exception>
		protected ListViewDataSet GetListViewType()
		{
			string invoiceType = this.GetInvoiceType();

			if (invoiceType == InvoiceListSR.INVOICE_PAYABLE)
			{
				return new ListViewDataSet(
					this.security, 
					LISTVIEW_TYPE.STANDARD, 
					ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.INVOICE), 
					this.accountingSite);
			}
			
			if (invoiceType == InvoiceListSR.INVOICE_RECEIVABLE)
			{
				return new ListViewDataSet(
					this.security, 
					LISTVIEW_TYPE.TRANSACTION_LIST, 
					new Guid("8D17D6B8-BCC0-43C3-85C4-27017B314C75"), //ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.INVOICE), 
					this.accountingSite);
			}

			throw new ApplicationException("Unexpected list view type specified");
		}

		/// <summary>
		/// This method handles the invoice number text change event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void InvoiceNumberTextBoxTextChanged(object sender, EventArgs e)
		{
			var invoiceSummaryContext = this.Session[InvoiceSummaryContext] as InvoiceSummaryContext;

			if (invoiceSummaryContext != null)
			{
				invoiceSummaryContext.InvoiceNumber = this.InvoiceNumberTB.Text;
				this.Session[InvoiceSummaryContext] = invoiceSummaryContext;
				this.LoadPersistedFilters();
			}
		}

		/// <summary>
		/// This method handles the on click for the Refresh button.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnClickRefreshBtn(object sender, EventArgs e)
		{
			try
			{
				this.PersistFilterCriteria();
				this.InvoiceDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
			this.Initialize();
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected virtual void Page_Load(object sender, EventArgs e)
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

		/// <summary>
		/// This function adds transID to the idList string collection if it does not already exist
		/// in the collection.
		/// </summary>
		/// <param name="idList">The ID list.</param>
		/// <param name="transID">The trans ID.</param>
		private void AddUniqueTransactionID(StringCollection idList, string transID)
		{
			foreach (string transactionID in idList)
			{
				if (transactionID.Equals(transID))
				{
					return;
				}
			}

			idList.Add(transID);
		}

		/// <summary>
		/// This method will apply the data dictionary to the list items in the invoice
		/// list data object.  Specifically for the "{All}" entry.
		/// </summary>
		/// <param name="invoiceListDO">The invoice list DO.</param>
		private void ApplyDataDictionaryToList(InvoiceListDO invoiceListDO)
		{
			if (invoiceListDO != null)
			{
				DropdownValuePairDO valuePair;

				if (invoiceListDO.ProductList != null && invoiceListDO.ProductList.Count > 0)
				{
					valuePair = (DropdownValuePairDO)invoiceListDO.ProductList[0];
					valuePair.Text = GetDataDictionaryValueByKey(this.security.SiteGuid, InvoiceSummaryAllText);
					invoiceListDO.ProductList[0] = valuePair;
				}

				if (invoiceListDO.AccountCodeList != null && invoiceListDO.AccountCodeList.Count > 0)
				{
					valuePair = (DropdownValuePairDO)invoiceListDO.AccountCodeList[0];
					valuePair.Text = GetDataDictionaryValueByKey(this.security.SiteGuid, InvoiceSummaryAllText);
					invoiceListDO.AccountCodeList[0] = valuePair;
				}

				if (invoiceListDO.CostCenterCodeList != null && invoiceListDO.CostCenterCodeList.Count > 0)
				{
					valuePair = (DropdownValuePairDO)invoiceListDO.CostCenterCodeList[0];
					valuePair.Text = GetDataDictionaryValueByKey(this.security.SiteGuid, InvoiceSummaryAllText);

					invoiceListDO.CostCenterCodeList[0] = valuePair;
				}
			}
		}

		/// <summary>
		/// This method will build a tool tip.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="address">The address.</param>
		/// <param name="city">The city.</param>
		/// <param name="state">The state.</param>
		/// <returns>
		/// The <see cref="string"/> tooltip.
		/// </returns>
		private string BuildToolTip(string name, string address, string city, string state)
		{
			string returnValue = string.Empty;

			var arrayList = new ArrayList();

			if (!string.IsNullOrEmpty(name))
			{
				arrayList.Add(name);
			}

			if (!string.IsNullOrEmpty(address))
			{
				arrayList.Add(address);
			}

			if (!string.IsNullOrEmpty(city))
			{
				arrayList.Add(city);
			}

			if (!string.IsNullOrEmpty(state))
			{
				arrayList.Add(state);
			}

			if (arrayList.Count > 0)
			{
				returnValue = arrayList[0] as string;

				for (int nextValue = 2; nextValue < arrayList.Count; ++nextValue)
				{
					returnValue += ", " + arrayList[nextValue];
				}
			}

			return returnValue;
		}

		/// <summary>
		///     This function is responsible for checking the current user's security access
		///     and responding appropriately including enforcing access and changing control
		///     availability.
		/// </summary>
		private void CheckUserSecurityAccess()
		{
			// Check security for this page
			bool viewInvoices = this.security.HasRight(RIGHT.VIEW_FINANCIAL_DATA);
			bool modifyInvoices = this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA);

			if ((viewInvoices == false) && (modifyInvoices == false))
			{
				throw new FMInsufficientRightsException();
			}
		}

		/// <summary>
		///     This method will remove all but the first two columns (edit and multiple select) so that
		///     the configured columns may be added.
		/// </summary>
		private void CleanUpGrid()
		{
			// Remove all but the first column which is Edit.
			while (this.InvoiceDataGrid.Columns.Count > 1)
			{
				this.InvoiceDataGrid.Columns.RemoveAt(1);
			}

			// Data dictionary the grid Edit column header if present.
			if (this.InvoiceDataGrid.Columns[0] != null)
			{
				string editText = GetDataDictionaryValueByKey(this.security.SiteGuid, "Edit");

				this.InvoiceDataGrid.Columns[0].HeaderText = editText;
			}
		}

		/// <summary>
		/// This method returns true if the site has the transaction alias to display invoice transactions.
		///     Otherwise, it will return false.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool DetermineIfInvoiceTransExist()
		{
			bool transAliasExist = false;

			var transAliasListSR = new TransactionAliasListSR
				{
					Security = this.security,
					CurrentSiteGuid = this.security.SiteGuid
				};

			try
			{
				var transAliasListDO =
					FMChannelHelper.MakeCall<ITransactionAliasListProcessor, TransactionAliasListDO>(
						proc => proc.Process(transAliasListSR));

				if (transAliasListDO != null)
				{
					IDictionaryEnumerator enumerator = transAliasListDO.aliasList.GetEnumerator();
					while (enumerator.MoveNext())
					{
						var transAlias = enumerator.Value as TransactionAliasClass;

						if (transAlias != null)
						{
							if ((transAlias._TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
							    || (transAlias._TransTypeID == TransactionTypes.T22_AccountReceivableInvoice))
							{
								transAliasExist = true;
								break;
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}

			return transAliasExist;
		}

		/// <summary>
		/// This method will return the invoice type that was stored in the session.
		/// </summary>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private string GetInvoiceType()
		{
			if (this.Session[SessionInvoiceSummaryType] == null)
			{
				return InvoiceListSR.INVOICE_NONE;
			}
			
			var invoiceType = this.Session[SessionInvoiceSummaryType] as string;

			if (string.IsNullOrEmpty(invoiceType))
			{
				return InvoiceListSR.INVOICE_NONE;
			}

			return invoiceType;
		}

		/// <summary>
		/// This method will return a tool tip for a line item control.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="index">The index.</param>
		/// <returns>
		/// The <see cref="string" /> tool tip.
		/// </returns>
		private string GetToolTip(string type, int index)
		{
			var invoiceListDO = this.Session[InvoiceSummaryListDO] as InvoiceListDO;

			string returnValue = string.Empty;

			if (invoiceListDO != null)
			{
				if (index < invoiceListDO.LineItems.Count)
				{
					var lineItem = invoiceListDO.LineItems[index] as InvoiceListLineItemDO;

					if (lineItem != null)
					{
						if (type == "Supplier")
						{
							returnValue = this.BuildToolTip(
								lineItem.SupplierName, lineItem.SupplierAddress, lineItem.SupplierCity, lineItem.SupplierState);
						}
						else if (type == "Ship-To")
						{
							returnValue = this.BuildToolTip(
								lineItem.ShipToName, lineItem.ShipToAddress, lineItem.ShipToCity, lineItem.ShipToState);
						}
					}
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method will get the dates from the controls and set the invoice list
		/// service request with the valid dates.
		/// </summary>
		/// <param name="invoiceListSR">The invoice list SR.</param>
		private void GetValidatedDates(InvoiceListSR invoiceListSR)
		{
			if (this.StartDate.Text != string.Empty)
			{
				invoiceListSR.StartDate = this.StartDate.CurrentValue;
			}

			// Get the end of the date range
			if (this.EndDate.Text != string.Empty)
			{
				invoiceListSR.EndDate = this.EndDate.CurrentValue;
			}
		}

		/// <summary>
		/// This method will return either an blank string if nothing has been selected
		///     or the actual selected value.
		/// </summary>
		/// <param name="dropdownList">
		/// The dropdown List.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private string GetValue(DropDownList dropdownList)
		{
			if (string.IsNullOrEmpty(dropdownList.SelectedValue))
			{
				return string.Empty;
			}

			if (dropdownList.SelectedValue.Equals(InvoiceSummaryAllValue))
			{
				return string.Empty;
			}

			return dropdownList.SelectedValue;
		}

		/// <summary>
		/// This method will return either a blank string for "{All}" or a selected company from
		/// selected control.
		/// </summary>
		/// <param name="companyTextBox">The company text box.</param>
		/// <returns>
		/// The <see cref="string"/> value of the text box.
		/// </returns>
		private string GetValue(FMCompanyTextBox companyTextBox)
		{
			if (companyTextBox.Text == GetDataDictionaryValueByKey(this.security.SiteGuid, InvoiceSummaryAllText))
			{
				return string.Empty;
			}

			return companyTextBox.Text;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.InvoiceDataGrid.ItemCommand += this.InvoiceDataGridItemCommand;
			this.InvoiceDataGrid.PageIndexChanged += this.InvoiceDataGridPageIndexChanged;
			this.InvoiceDataGrid.SortCommand += this.InvoiceDataGridSortCommand;
			this.InvoiceDataGrid.ItemDataBound += this.InvoiceDataGridItemDataBound;
		}

		/// <summary>
		/// This method handles the event of a line item being selected. It checks for the
		/// Edit command, if so, then it will send control to the transaction detail page for
		/// editing.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.Exception">InvoiceList.LineItemDataGrid_ItemCommand expected session to contain InvoiceListDO</exception>
		private void InvoiceDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
			// Is this the View link?
			if (e.CommandName == "Edit")
			{
				if (this.DetermineIfInvoiceTransExist() == false)
				{
					var noAliasDefined = new Exception("No Invoice type transaction configured");
					this.ErrorHandler(noAliasDefined);
				}
				else
				{
					// Get the data set
					var invoiceListDO = this.Session[InvoiceSummaryListDO] as InvoiceListDO;

					if (invoiceListDO == null)
					{
						// No object so we don't know under what context the button was pressed!
						throw new Exception("InvoiceList.LineItemDataGrid_ItemCommand expected session to contain InvoiceListDO");
					}

					// Create session object for TransactionDetail list of transactions.
					var detailList = new TransactionDetailList();

					// Find the real index from the grid paging.
					int realIndex = (this.InvoiceDataGrid.CurrentPageIndex * this.InvoiceDataGrid.PageSize) + e.Item.ItemIndex;

					// Build the list of transactions to display
					if (realIndex < invoiceListDO.LineItems.Count)
					{
						FMChannelHelper.MakeCall<IHardwareKey>(
							hardwareKey =>
								{
									foreach (InvoiceListLineItemDO lineItem in invoiceListDO.LineItems)
									{
										if (!hardwareKey.IsADFKey())
										{
											this.AddUniqueTransactionID(detailList.TransactionIDList, lineItem.TransID);
										}
										else
										{
											// in ADF transaction IDs are not unique in the summary list
											detailList.TransactionIDList.Add(lineItem.TransID);
										}
									}
								});

						// Indicate which transaction id in the list is the one to initially display.
						detailList.CurrentIndex = realIndex;

						// Indicate the return URL for when the TransactionDetail Close button is clicked.
						switch (this.GetInvoiceType())
						{
							case InvoiceListSR.INVOICE_PAYABLE:
								detailList.ReturnURL = "..\\InvoiceWebApp\\InvoiceSummary.aspx?invoicetype=payable";
								break;
							case InvoiceListSR.INVOICE_RECEIVABLE:
								detailList.ReturnURL = "..\\InvoiceWebApp\\InvoiceSummary.aspx?invoicetype=receivable";
								break;
							default:
								detailList.ReturnURL = "..\\InvoiceWebApp\\InvoiceSummary.aspx";
								break;
						}

						// Put the object into session and transfer to the TransactionDetail.
						this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

						// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
						string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

						this.Redirect("../" + transactionDetailUrl);
					}
				}
			}
		}

		/// <summary>
		/// This method handles the invoice data grid item data bound.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridItemEventArgs" /> instance containing the event data.</param>
		private void InvoiceDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex > -1)
			{
				if (listViewGrid != null)
				{
					for (int cellIndex = 2; cellIndex < e.Item.Cells.Count; ++cellIndex)
					{
						ListViewColumnDO columnDO = listViewGrid.listViewDO[cellIndex - 2];
						TableCell cell = e.Item.Cells[cellIndex];

						int realIndex = (this.InvoiceDataGrid.PageSize * this.InvoiceDataGrid.CurrentPageIndex) + e.Item.ItemIndex;
						cell.ToolTip = this.GetToolTip(columnDO.ColumnName, realIndex);
					}
				}
			}
		}

		/// <summary>
		/// This method handles the invoice data grid page index change.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridPageChangedEventArgs" /> instance containing the event data.</param>
		private void InvoiceDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.InvoiceDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.InvoiceDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		/// This method handles the invoice grid sort command event.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridSortCommandEventArgs" /> instance containing the event data.</param>
		private void InvoiceDataGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			try
			{
				// Find the Sort Data Path
				this.Session[InvoiceListGridSort] = e.SortExpression;

				bool sortDirection = false;
				if (this.Session[InvoiceListSortDirection] != null)
				{
					sortDirection = !((bool)this.Session[InvoiceListSortDirection]);
				}

				this.Session[InvoiceListSortDirection] = sortDirection;

				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}

		}

		/// <summary>
		/// This method will select the previous selected item in the dropdown
		/// list.
		/// </summary>
		/// <param name="dropDownList">The drop down list.</param>
		/// <param name="invalue">The invalue.</param>
		private void LoadControlValue(DropDownList dropDownList, string invalue)
		{
			if (invalue != string.Empty)
			{
				dropDownList.SelectedValue = invalue;
			}
		}

		/// <summary>
		///     This method retrieves the information for the dropdown list and binds
		///     the data to the dropdown controls.
		/// </summary>
		private void LoadDropDownBoxes()
		{
			// Request updated data from the database.
			// Create and populate our service request.
			var invoiceListSR = new InvoiceListSR
				{
					SubRequest = InvoiceListSR.RequestTypes.GET_HEADER_DATA,
					Security = this.security,
					AllText = GetDataDictionaryValueByKey(this.security.SiteGuid, InvoiceSummaryAllText),
					InvoiceType = this.GetInvoiceType()
				};

			// Process the service request
			InvoiceListDO invoiceListDO = FMChannelHelper.MakeCall<IInvoiceSummaryProcessor, InvoiceListDO>(proc => proc.Process(invoiceListSR));

			// Apply the data dictionary to the ALL in the data object list.
			this.ApplyDataDictionaryToList(invoiceListDO);

			// Bind control data
			this.ProductDropdown.DataSource = invoiceListDO.ProductList;
			this.ProductDropdown.DataTextField = "Text";
			this.ProductDropdown.DataValueField = "TextValue";
			this.ProductDropdown.DataBind();

			this.AccountCodeDropdown.DataSource = invoiceListDO.AccountCodeList;
			this.AccountCodeDropdown.DataTextField = "Text";
			this.AccountCodeDropdown.DataValueField = "TextValue";
			this.AccountCodeDropdown.DataBind();

			this.CostCenterDropdown.DataSource = invoiceListDO.CostCenterCodeList;
			this.CostCenterDropdown.DataTextField = "Text";
			this.CostCenterDropdown.DataValueField = "TextValue";
			this.CostCenterDropdown.DataBind();

			this.ShipToTextBox.Text = invoiceListSR.AllText;
			this.SupplierTextBox.Text = invoiceListSR.AllText;

			// Load any persisted context data
			this.LoadPersistedFilters();
		}

		/// <summary>
		///     This method will load the page with the invoice summary context values retrieved from
		///     session.
		/// </summary>
		private void LoadPersistedFilters()
		{
			var invoiceSummaryContext = this.Session[InvoiceSummaryContext] as InvoiceSummaryContext;

			if (invoiceSummaryContext != null)
			{
				// Load the controls
				this.LoadControlValue(this.ProductDropdown, invoiceSummaryContext.Product);
				this.LoadControlValue(this.AccountCodeDropdown, invoiceSummaryContext.AccountCode);
				this.LoadControlValue(this.CostCenterDropdown, invoiceSummaryContext.CostCentreCode);

				// Finish loading the controls
				this.StartDate.Text = this.accountingSite.FormatDate(invoiceSummaryContext.StartDate);
				this.EndDate.Text = this.accountingSite.FormatDate(invoiceSummaryContext.EndDate);

				this.ShipToTextBox.Text = invoiceSummaryContext.ShipTo;
				this.ShipToTextBox.ToolTip = invoiceSummaryContext.ShipToTip;

				this.SupplierTextBox.Text = invoiceSummaryContext.Supplier;
				this.SupplierTextBox.ToolTip = invoiceSummaryContext.SupplierTip;

				this.InvoiceNumberTB.Text = invoiceSummaryContext.InvoiceNumber;
			}
			else
			{
				DateTimeOffset today = TimeConverter.Today(this.accountingSite.CurrentSite);

				this.StartDate.Text = this.accountingSite.FormatDate(today);
				this.EndDate.Text = this.accountingSite.FormatDate(today.AddDays(1));
			}
		}

		/// <summary>
		///     This method handles the page load processing.
		/// </summary>
		private void PageLoadProcessing()
		{
			this.CleanUpGrid();

			// Get site information.
			this.accountingSite =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

			this.accountingSite.GetUserCompanies = false;

			if (this.IsPostBack == false)
			{
				// If the session is null, then remove all objects from the 
				// session and display the accounting error page.
				if (this.Session[InvoiceSummarySecurity] == null)
				{
					this.Session.RemoveAll();
					this.DisplayErrorPage();
					return;
				}

				// Set initial sort
				var invoiceListSort = this.Session[InvoiceListGridSort] as string;

				if (string.IsNullOrEmpty(invoiceListSort))
				{
					this.Session[InvoiceListGridSort] = "InventoryDate";
					this.Session[InvoiceListSortDirection] = false;
					this.Session[InvoiceListSortExpression] = "InventoryDate DESC";
				}

				// Check the user's security access
				this.CheckUserSecurityAccess();

				// Set the type of invoice summary specified by the request.
				this.SetInvoiceType();

				// Load the dropdown boxes
				this.LoadDropDownBoxes();

				// Save Filter Settings
				this.PersistFilterCriteria();

				// Set the page title to either Invoice Payable Summary or
				// Invoice Receivable Summary.  The default is Invoice Summary.
				this.SetPageTitle();

				// Refresh the data in the list view
				this.UpdateView();
			}

			// Set the page title to either Invoice Payable Summary or
			// Invoice Receivable Summary.  The default is Invoice Summary.
			this.SetPageTitle();
		}

		/// <summary>
		///     This method will persist all the filter data into session.
		/// </summary>
		private void PersistFilterCriteria()
		{
			// Save the filter criteria
			var invoiceSummaryContext = new InvoiceSummaryContext
				{
					Product = this.GetValue(this.ProductDropdown),
					AccountCode = this.GetValue(this.AccountCodeDropdown),
					CostCentreCode = this.GetValue(this.CostCenterDropdown),
					ShipTo = this.ShipToTextBox.Text,
					ShipToTip = this.ShipToTextBox.ToolTip,
					Supplier = this.SupplierTextBox.Text,
					SupplierTip = this.SupplierTextBox.ToolTip,
					InvoiceNumber = this.InvoiceNumberTB.Text
				};

			// Load the context object
			if (!string.IsNullOrEmpty(this.StartDate.Text))
			{
				invoiceSummaryContext.StartDate = this.StartDate.CurrentValue;
			}

			if (!string.IsNullOrEmpty(this.EndDate.Text))
			{
				invoiceSummaryContext.EndDate = this.EndDate.CurrentValue;
			}

			var sortExpression = this.Session[InvoiceListSortExpression] as string;
			if (sortExpression != null)
			{
				invoiceSummaryContext.SortExpression = sortExpression;
			}

			// Save it as a session variable
			this.Session[InvoiceSummaryContext] = invoiceSummaryContext;
		}

		/// <summary>
		///     This method will retrieve the invoice type (payable or receivable) from the request and
		///     save the value in session.
		/// </summary>
		private void SetInvoiceType()
		{
			if (string.IsNullOrEmpty(this.Page.Request.GetQueryOrFormValue("invoicetype")))
			{
				if (this.Page.Session[SessionInvoiceSummaryType] == null)
				{
					this.Page.Session.Add(SessionInvoiceSummaryType, InvoiceListSR.INVOICE_NONE);
				}
			}
			else
			{
				string typeOfInvoice = this.Page.Request.GetQueryOrFormValue("invoicetype");

				if (string.IsNullOrEmpty(typeOfInvoice) || (typeOfInvoice.Length <= 0))
				{
					this.Page.Session.Add(SessionInvoiceSummaryType, InvoiceListSR.INVOICE_NONE);
				}
				else
				{
					switch (typeOfInvoice)
					{
						case "payable":
							this.Page.Session.Add(SessionInvoiceSummaryType, InvoiceListSR.INVOICE_PAYABLE);
							break;
						case "receivable":
							this.Page.Session.Add(SessionInvoiceSummaryType, InvoiceListSR.INVOICE_RECEIVABLE);
							break;
						default:
							this.Page.Session.Add(SessionInvoiceSummaryType, InvoiceListSR.INVOICE_NONE);
							break;
					}
				}
			}
		}

		/// <summary>
		///     This method will set the page title depending on the type of summary page
		///     {Invoice Payable Summary or Invoice Receivable Summary).  In addition,
		///     the title is data dictionaried.
		/// </summary>
		private void SetPageTitle()
		{
			string invoiceType = this.GetInvoiceType();

			switch (invoiceType)
			{
				case InvoiceListSR.INVOICE_PAYABLE:
					this.PageTitle.Text = "Invoice Payable Summary";
					this.ShipToLabel.Visible = false;
					this.ShipToTextBox.Visible = false;
					this.SupplierLabel.Visible = true;
					this.SupplierTextBox.Visible = true;
					break;
				case InvoiceListSR.INVOICE_RECEIVABLE:
					this.PageTitle.Text = "Invoice Receivable Summary";
					this.ShipToLabel.Visible = true;
					this.ShipToTextBox.Visible = true;
					this.SupplierLabel.Visible = false;
					this.SupplierTextBox.Visible = false;
					break;
				default:
					this.PageTitle.Text = "Invoice Summary";
					this.ShipToLabel.Visible = false;
					this.ShipToTextBox.Visible = false;
					this.SupplierLabel.Visible = false;
					this.SupplierTextBox.Visible = false;
					break;
			}

			this.PageTitle.Text = GetDataDictionaryValueByKey(this.security.SiteGuid, this.PageTitle.Text);

		}

		/// <summary>
		///     This method updates the invoice data grid view with new data.
		/// </summary>
		private void UpdateView()
		{
			this.listViewGrid = this.GetListViewType();

			var sortDirection = false;
			if (this.Session[InvoiceListSortDirection] != null)
			{
				sortDirection = (bool)this.Session[InvoiceListSortDirection];
			}

			var sort = this.Session[InvoiceListGridSort] as string;
			string sortExpression = "TransactionDate DESC";
			if (sort != null)
			{
				sortExpression = this.listViewGrid.GetDataPath(sort);
				if (sortExpression == string.Empty)
				{
					sortExpression = sort;
				}

				this.listViewGrid.Sort = sort;
				this.listViewGrid.SortDirection = sortDirection;
				sortExpression += sortDirection ? string.Empty : " DESC";
			}

			this.Session[InvoiceListSortExpression] = sortExpression;

			this.listViewGrid.SetDataGrid(this.InvoiceDataGrid);

			// Set for sorting messages
			this.InvoiceDataGrid.AllowSorting = true;
			this.InvoiceDataGrid.SortCommand += this.InvoiceDataGridSortCommand;

			// Create and populate our service request
			var invoiceListSR = new InvoiceListSR
			{
				SubRequest = InvoiceListSR.RequestTypes.GET_DETAIL,
				Security = this.security,
				AccountingSite = this.accountingSite
			};

			// Set the request type and security

			// vthompson 10/30/2008
			// The view that will be queried against retrieves the product name and not
			// the product index.  This caused no records to be returned when the product index was passed.
			if (this.ProductDropdown.SelectedItem != null && this.ProductDropdown.SelectedValue != InvoiceSummaryAllValue)
			{
				invoiceListSR.ProductID = this.ProductDropdown.SelectedItem.Text.Trim();
			}
			else
			{
				invoiceListSR.ProductID = string.Empty;
			}

			invoiceListSR.AccountCode = this.GetValue(this.AccountCodeDropdown);
			invoiceListSR.CostCenterCode = this.GetValue(this.CostCenterDropdown);
			invoiceListSR.InvoiceType = this.GetInvoiceType();
			invoiceListSR.ShipToID = this.GetValue(this.ShipToTextBox);
			invoiceListSR.SupplierID = this.GetValue(this.SupplierTextBox);
			invoiceListSR.InvoiceNumber = this.InvoiceNumberTB.Text;

			invoiceListSR.SortExpression = (string)this.Session[InvoiceListSortExpression];

			this.GetValidatedDates(invoiceListSR);

			// Process the service request
			InvoiceListDO invoiceListDO =
				FMChannelHelper.MakeCall<IInvoiceSummaryProcessor, InvoiceListDO>(proc => proc.Process(invoiceListSR));

			this.Session[InvoiceSummaryListDO] = invoiceListDO;



			// Bind up the data
			this.listViewGrid.BindData(
				invoiceListDO.LineItems, 
				QuantityDisplay.NET, 
				this.accountingSite.CurrentSite._VolumeDecimalPlaces, 
				this.accountingSite.CurrentSite._MassDecimalPlaces, 
				false);

			// Do we need to warn about the result size?
			if (invoiceListDO.LineItems.Count >= 500)
			{
				// Issue the warning message
				this.WarningLabel.Text = "Results limited to first 500 records.  Use filters to narrow search.";
			}
		}

		#endregion
	}
}
