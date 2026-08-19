// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderSummary.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for order summary page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.OrderEntryWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.Exceptions;

	using FuelsManager.Accounting;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using TransactionFields;

	/// <summary>
	///     The Order Listing Form is responsible for displaying a list of Orders and
	///     allowing the user to filter based on various criteria.
	/// </summary>
	public partial class OrderSummaryForm : AccountingAutoSubmitWebFormView
	{
		//*************************************************************************
		// Member variables
		//*************************************************************************    

		#region Constants and Fields
		private AccountingSite accountingSite;
		private bool bEnableEdit = true;
		private OrderListDO orderHeaderDO;
		private ListViewDataSet grid;
		private const string ErrorMsg001 = "Must have a Start Date.";
		private const string ErrorMsg002 = "Must have an End Date.";
		private const string ErrorMsg003 = "Invalid Start Date.";
		private const string ErrorMsg004 = "Invalid End Date.";
		private const string ErrorMsg005 = "Start Date must be before the End Date.";
		#endregion

		#region Enums

		private enum TimeSetting
		{
			Begin,
			End
		}

		#endregion

		#region Methods

		/// <summary>
		/// Handles the Click event of the ChangeSelected control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ChangeSelectedClick(object sender, EventArgs e)
		{
			var orderList = this.Session["OrderList.OrderListDO"] as OrderListDO;

			if (orderList != null)
			{
				//Create session object for TransactionDetail list of transactions.
				var detailList = new TransactionDetailList();

				string buttonStatus = this.GetValue(this.ChangeOrderStatusDropdownlist);

				if (!string.IsNullOrEmpty(buttonStatus))
				{
					for (int index = 0; index < this.TransactionDataGrid.Items.Count; ++index)
					{
						try
						{
							var fmButton = (CheckBox)this.TransactionDataGrid.Items[index].Cells[1].FindControl("MultipleSelectCheckbox");

							if (fmButton.Checked)
							{
								// Determine which index to use
								int realIndex = (this.TransactionDataGrid.CurrentPageIndex * this.TransactionDataGrid.PageSize) + index;

								if (realIndex < orderList.LineItems.Count)
								{
									foreach (OrderListLineItemDO lineItem in orderList.LineItems)
									{
										detailList.TransactionIDList.Add(lineItem.TransactionID);
									}

									//Indicate which transaction id in the list is the one to initially display.
									detailList.CurrentIndex = realIndex;

									string transID = detailList.TransactionIDList[detailList.CurrentIndex];

									// accountingSite
									TransactionDO trans = this.LoadTransaction(transID);

									trans.Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), buttonStatus, true);

									this.SaveTransaction(trans);
								}
							}
						}
						catch
						{
							break;
						}
					}
				}
			}
		}

		//*************************************************************************
		// Web Form Designer generated code
		//*************************************************************************    

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);

			this.Initialize();
		}

		protected void OnSelectAll(object sender, EventArgs e)
		{
			var orderList = this.Session["OrderList.OrderListDO"] as OrderListDO;

			if (orderList != null)
			{
				for (int index = 0; index < this.TransactionDataGrid.Items.Count; ++index)
				{
					try
					{
						var fmButton = (CheckBox)this.TransactionDataGrid.Items[index].Cells[1].FindControl("MultipleSelectCheckbox");

						if (fmButton != null && fmButton.Checked != true)
						{
							fmButton.Checked = true;
						}
					}
					catch
					{
						break;
					}
				}
			}
		}

		protected void OnUnselectAll(object sender, EventArgs e)
		{
			var orderList = this.Session["OrderList.OrderListDO"] as OrderListDO;

			if (orderList != null)
			{
				for (int index = 0; index < this.TransactionDataGrid.Items.Count; ++index)
				{
					try
					{
						var fmButton = (CheckBox)this.TransactionDataGrid.Items[index].Cells[1].FindControl("MultipleSelectCheckbox");

						if (fmButton.Checked)
						{
							fmButton.Checked = false;
						}
					}
					catch
					{
						break;
					}
				}
			}
		}

		//*************************************************************************
		// Member functions
		//*************************************************************************    

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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

		/// <summary>
		/// Handles the Click event of the PrintSelection control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void PrintSelectionClick(object sender, EventArgs e)
		{
			var selectedTransactionsAndAliases = new Dictionary<string, string>();

			var orderList = this.Session["OrderList.OrderListDO"] as OrderListDO;

			if (orderList == null)
			{
				// No order list, then leave.
				return;
			}

			//Create session object for TransactionDetail list of transactions.
			var detailList = new TransactionDetailList();

			for (int index = 0; index < this.TransactionDataGrid.Items.Count; ++index)
			{
				try
				{
					var fmButton = (CheckBox)this.TransactionDataGrid.Items[index].Cells[1].FindControl("MultipleSelectCheckbox");

					if (fmButton.Checked)
					{
						// Determine which index to use
						int realIndex = (this.TransactionDataGrid.CurrentPageIndex * this.TransactionDataGrid.PageSize) + index;

						if (realIndex < orderList.LineItems.Count)
						{
							foreach (OrderListLineItemDO lineItem in orderList.LineItems)
							{
								detailList.TransactionIDList.Add(lineItem.TransactionID);
							}

							//Indicate which transaction id in the list is the one to initially display.
							detailList.CurrentIndex = realIndex;

							string transID = detailList.TransactionIDList[detailList.CurrentIndex];

							// accountingSite
							TransactionDO trans = this.LoadTransaction(transID);
							selectedTransactionsAndAliases.Add(transID, trans.Alias);
						}
					}
				}
				catch
				{
					break;
				}
			}

			// Now we have a dictionary of the transaction ids and their aliases.
			// To print the new BOL report (allowing multiple BOLs on a single report), all transactions must the same alias.
			// Also, one final check that we have any transactions
			string reportAlias = "";
			string transactionIdList = "";
			bool allSameAlias = true;
			if (selectedTransactionsAndAliases.Count == 0)
			{
				const string WarningString = "\\r\\nWarning, Selected transactions  and aliases count is 0.";

				const string NotifyString = "<script language=\"JavaScript\">\r\n<!--\r\n" +
											"var result=alert(\"" + WarningString + "\");\r\n" + "-->\r\n</script>";
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					"TransactionAliasesCountIs0Script",
					NotifyString,
					false);

				return;
			}

			foreach (KeyValuePair<string, string> transactionAndAliasPair in selectedTransactionsAndAliases)
			{
				if (string.IsNullOrEmpty(reportAlias) == false && reportAlias.Equals(transactionAndAliasPair.Value) == false)
				{
					allSameAlias = false;
					break;
				}

				if (string.IsNullOrEmpty(reportAlias))
				{
					reportAlias = transactionAndAliasPair.Value;
				}

				transactionIdList = transactionIdList + transactionAndAliasPair.Key + ",";
			}
			transactionIdList = transactionIdList.Remove(transactionIdList.LastIndexOf(','));

			if (string.IsNullOrEmpty(reportAlias))
			{
				const string WarningString = "\\r\\nWarning, no transaction alias found.";

				const string NotifyString = "<script language=\"JavaScript\">\r\n<!--\r\n" +
											"var result=alert(\"" + WarningString + "\");\r\n" + "-->\r\n</script>";
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					"TransactionAliasesNotFoundScript",
					NotifyString,
					false);

				return;
			}

			if (allSameAlias == false)
			{
				const string WarningString = "\\r\\nWarning, transaction aliases must be the same to print.";

				const string NotifyString = "<script language=\"JavaScript\">\r\n<!--\r\n" +
											"var result=alert(\"" + WarningString + "\");\r\n" +
											"-->\r\n</script>";

				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					"TransactionAliasesNotAllTheSameScript",
					NotifyString,
					false);

				return;
			}

			// reportAlias has our one and only involved alias.
			// get the associated report for this order
			bool useDictionary = false;
			if (this.Session["UseDataDictionary"] != null)
			{
				useDictionary = (bool)this.Session["UseDataDictionary"];
			}

			var transContext = new TransactionContext(this.security, this.accountingSite, reportAlias, TransactionContext.Mode.Edit, useDictionary);

			transContext.GetTransactionContext();

			string bolRptType = ((int)ReportTypesClass.ReportTypes.BOL_RPT).ToString(CultureInfo.InvariantCulture);
			string stRptName = transContext.aliasClass.AssociatedReport;

			if (stRptName.Length <= 0)
			{
				string warningString = string.Format("\\r\\nWarning, No associated report configured for transaction alias: {0}.", reportAlias);

				string notifyString = "<script language=\"JavaScript\">\r\n<!--\r\n" +
											"var result=alert(\"" + warningString + "\");\r\n" + "-->\r\n</script>";
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					"NoTransactionAliasAssociatedReportScript",
					notifyString,
					false);

				return;
			}

			string rptUrl = "../FMReportWebMain/ReportLandingPage.aspx?ReportType=" + bolRptType;

			string reportName = HttpUtility.HtmlEncode(stRptName);//stRptName.Replace(" ", "+");
         rptUrl = rptUrl + "&ReportName=" + reportName;
			rptUrl = rptUrl + "&SiteGuid=" + this.security.SiteGuid.ToString();
			rptUrl = rptUrl + "&TransID=" + transactionIdList;
			rptUrl += "&" + this.security.CSRFTokenWithParamName;

			string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" + "window.open('" + rptUrl + "', "
										   + "'Reports', "
										   + "'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=800, width=1000'"
										   + "); \n" + "-->\n</script>";

			this.Response.Cookies.Add(new HttpCookie("Token", this.Session["Token"] as String));
			this.ClientScript.RegisterStartupScript(this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
		}

		/// <summary>
		/// Handles the Click event of the RefreshButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void RefreshButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.PersistFilterCriteria();
				this.TransactionDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		///     This function is responsible for checking the current user's security access
		///     and responding appropriately including enforcing access and changing control
		///     availability.
		/// </summary>
		private static void CheckUserSecurityAccess(SecurityClass securityObject)
		{
			if (securityObject == null)
			{
				throw new ArgumentNullException();
			}

			// Check security for this page
			if (!securityObject.HasViewTransactionRightByTransTypeID(TransactionTypes.T17_Order))
			{
				throw new FMInsufficientRightsException();
			}
		}

		/// <summary>
		/// Loads the type of the date filter.
		/// </summary>
		/// <param name="dateFilterTypeDropDown">The date filter type drop down.</param>
		private static void LoadDateFilterType(FMDropDownList dateFilterTypeDropDown)
		{
			var item = new ListItem("No filter", OrderListFilterCriteria.OrderDateFilterType.NONE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("Scheduled Date", OrderListFilterCriteria.OrderDateFilterType.SCHEDULED_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("Effective Date", OrderListFilterCriteria.OrderDateFilterType.EFFECTIVE_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("Expiration Date", OrderListFilterCriteria.OrderDateFilterType.EXPIRATION_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("Transaction Date", OrderListFilterCriteria.OrderDateFilterType.TRANSACTION_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("ETA", OrderListFilterCriteria.OrderDateFilterType.ETA.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem(
				"Requested Delivery Date", OrderListFilterCriteria.OrderDateFilterType.REQUESTED_DELIVERY_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			// Set the default date filter.
			dateFilterTypeDropDown.SelectByText("No filter");
		}

		/// <summary>
		/// Binds the controls.
		/// </summary>
		private void BindControls()
		{
			this.TransactionDataGrid.ItemCommand += this.LineItemDataGridItemCommand;
			this.TransactionDataGrid.ItemCreated += this.LineItemDataGridItemCreated;
			this.OrderNumberTextBox.TextChanged += this.OrderNumberTextBoxTextChanged;
		}

		/// <summary>
		/// Builds the tool tip.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="address">The address.</param>
		/// <param name="city">The city.</param>
		/// <param name="state">The state.</param>
		/// <returns>Tooltip string</returns>
		private string BuildToolTip(string name, string address, string city, string state)
		{
			string returnValue = "";

			var values = new ArrayList();

			if (!string.IsNullOrEmpty(name))
			{
				values.Add(name);
			}

			if (!string.IsNullOrEmpty(address))
			{
				values.Add(address);
			}

			if (!string.IsNullOrEmpty(city))
			{
				values.Add(city);
			}

			if (!string.IsNullOrEmpty(state))
			{
				values.Add(state);
			}

			if (values.Count > 0)
			{
				returnValue = values[0] as string;

				for (int nLoop = 2; nLoop < values.Count; ++nLoop)
				{
					returnValue += ", " + values[nLoop];
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Cleans up the grid.
		/// </summary>
		private void CleanUpGrid()
		{
			// Remove all but the first column which is Edit
			while (this.TransactionDataGrid.Columns.Count > 2)
			{
				this.TransactionDataGrid.Columns.RemoveAt(2);
			}

			// Make sure Edit column has translated text
			if (this.TransactionDataGrid.Columns[0] != null)
			{
				string editText = GetDataDictionaryValueByKey(this.Security.SiteGuid, "Edit");

				this.TransactionDataGrid.Columns[0].HeaderText = editText;
			}

			if (this.TransactionDataGrid.Columns[1] != null)
			{
				string editText = GetDataDictionaryValueByKey(this.Security.SiteGuid, "Multiple Select");
				this.TransactionDataGrid.Columns[1].HeaderText = editText;
			}
		}

		/// <summary>
		/// This method returns true if the site has the transaction alias to display order transactions.
		/// Otherwise, it will return false.
		/// </summary>
		/// <returns></returns>
		private bool DetermineIfOrderTransExist()
		{
			try
			{
				if (this.orderHeaderDO == null)
				{
					var aliasNames =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
							x => x.EnumerateNamesOnly(this.security, byUser: true));

					foreach (var alias in aliasNames)
					{
						if (alias.TransTypeID == TransactionTypes.T17_Order)
						{
							return true;
						}
					}
				}
				else
				{
					// The list contains an "all" setting even if there are none.
					return this.orderHeaderDO.OrderTypeList.Count > 1;
				}
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}

			return false;
		}

		/// <summary>
		/// Formats for regional settings.
		/// </summary>
		/// <param name="orderListDO">The order list DO.</param>
		private void FormatForRegionalSettings(OrderListDO orderListDO)
		{
			foreach (OrderListLineItemDO item in orderListDO.LineItems)
			{
				item.TransactionDate = this.accountingSite.FormatDate(item.TransactionDateTime);

				if (item.ScheduledDate != "")
				{
					item.ScheduledDate = this.accountingSite.FormatDateTime(item.ScheduledDateTime);
				}

				if (item.ExpirationDate != "")
				{
					item.ExpirationDate = this.accountingSite.FormatDate(item.ExpirationDateTime);
				}

				if (item.EffectiveDate != "")
				{
					item.EffectiveDate = this.accountingSite.FormatDate(item.EffectiveDateTime);
				}

				if (item.RequestedDeliveryDate != "")
				{
					item.RequestedDeliveryDate = this.accountingSite.FormatDateTime(item.RequestedDeliveryDateTime);
				}

				if (item.InventoryDate != "")
				{
					item.InventoryDate = this.accountingSite.FormatDate(item.InventoryDateTime);
				}
			}
		}

		private string GetToolTip(string type, int index)
		{
			var orderList = this.Session["OrderList.OrderListDO"] as OrderListDO;

			string returnValue = "";

			if (orderList != null)
			{
				if (index < orderList.LineItems.Count)
				{
					var lineItem = orderList.LineItems[index] as OrderListLineItemDO;

					if (lineItem != null)
					{
						if (type == "Bill-To")
						{
							returnValue = this.BuildToolTip(
								lineItem.BillToName, lineItem.BillToAddress, lineItem.BillToCity, lineItem.BillToState);
						}
						else if (type == "Ship-To")
						{
							returnValue = this.BuildToolTip(
								lineItem.ShipToName, lineItem.ShipToAddress, lineItem.ShipToCity, lineItem.ShipToState);
						}

						else if (type == "Carrier")
						{
							returnValue = this.BuildToolTip(
								lineItem.CarrierName, lineItem.CarrierAddress, lineItem.CarrierCity, lineItem.CarrierState);
						}
					}
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Gets the validated dates.
		/// </summary>
		/// <param name="sr">The SR.</param>
		private void GetValidatedDates(OrderListSR sr)
		{
			if (this.StartDate.Text != "")
			{
				sr.Criteria.StartDate = this.SetTimeToBeginningOrEnd(this.StartDate.CurrentValue, TimeSetting.Begin);
			}

			// Get the end of the date range
			if (this.EndDate.Text != "")
			{
				sr.Criteria.EndDate = this.SetTimeToBeginningOrEnd(this.EndDate.CurrentValue, TimeSetting.End);
			}

			// Get the date filter type
			if (this.StartDate.Text == "" && this.EndDate.Text == "")
			{
				sr.Criteria.DateFilterType = OrderListFilterCriteria.OrderDateFilterType.NONE;
				this.DateFilterTypeDropDown.SelectedIndex = 0;
			}
			else
			{
				sr.Criteria.DateFilterType =
					(OrderListFilterCriteria.OrderDateFilterType)
					Enum.Parse(typeof(OrderListFilterCriteria.OrderDateFilterType), this.DateFilterTypeDropDown.SelectedValue);
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="dropDownList">The drop down list.</param>
		/// <returns>Value of the specified drop down list.</returns>
		private string GetValue(DropDownList dropDownList)
		{
			if (dropDownList.SelectedIndex == 0)
			{
				return string.Empty;
			}

			return dropDownList.SelectedValue;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="companyTextBox">The company text box.</param>
		/// <returns>Company text box text.</returns>
		private string GetValue(FMCompanyTextBox companyTextBox)
		{
			if (companyTextBox.Text == "{All}")
			{
				return "";
			}

			return companyTextBox.Text;
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TransactionDataGrid.PageIndexChanged += this.LineItemDataGridPageIndexChanged;
			this.TransactionDataGrid.SortCommand += this.DataGridSortCommand;
			this.TransactionDataGrid.ItemDataBound += this.DataGridItemDataBound;
		}

		/// <summary>
		/// Lines the item data grid item command.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void LineItemDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
			// Is this the View link?
			if (e.CommandName == "Edit")
			{
				if (this.DetermineIfOrderTransExist() == false)
				{
					var noAliasDefined = new Exception("No Order type transaction configured");
					this.ErrorHandler(noAliasDefined);
				}
				else
				{
					// Get the data set
					var orderListDO = this.Session["OrderList.OrderListDO"] as OrderListDO;
					if (orderListDO == null)
					{
						// No object so we don't know under what context the button was pressed!
						throw new Exception("OrderList.LineItemDataGrid_ItemCommand expected session to contain OrderListDO");
					}

					//Create session object for TransactionDetail list of transactions.
					var detailList = new TransactionDetailList();

					// Determine which index to use
					int realIndex = (this.TransactionDataGrid.CurrentPageIndex * this.TransactionDataGrid.PageSize) + e.Item.ItemIndex;

					// Build the list of transactions to display
					if (realIndex < orderListDO.LineItems.Count)
					{
						foreach (OrderListLineItemDO lineItem in orderListDO.LineItems)
						{
							detailList.TransactionIDList.Add(lineItem.TransactionID);
						}

						//Indicate which transaction id in the list is the one to initially display.
						detailList.CurrentIndex = realIndex;

						//Indicate the return URL for when the TransactionDetail Close button is clicked.
						detailList.ReturnURL = "..\\OrderEntryWebApp\\OrderSummary.aspx";

						//Put the object into session and transfer to the TransactionDetail.
						this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

						// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
						string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

						this.Redirect("../" + transactionDetailUrl);
					}
				}
			}
		}

		/// <summary>
		/// Handles the PageIndexChanged event of the LineItemDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.</param>
		private void LineItemDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.TransactionDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.TransactionDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		/// Loads the control value.
		/// </summary>
		/// <param name="dropDownList">The drop down list.</param>
		/// <param name="value">The value.</param>
		private void LoadControlValue(DropDownList dropDownList, string value)
		{
			if (value != "")
			{
				dropDownList.SelectedValue = value;
			}
		}

		/// <summary>
		///     Responsible for loading the dropdown boxes with appropriate selection choices
		/// </summary>
		private void LoadDropDownBoxes()
		{
			// Request updated data from the database.
			// Create and populate our service request.
			var sr = new OrderListSR
			{
				SubRequest = OrderListSR.RequestTypes.GET_HEADER_DATA,
				Security = this.security,
				AllText = GetDataDictionaryValueByKey(this.Security.SiteGuid, "{All}")
			};

			// Process the service request
			this.orderHeaderDO = FMChannelHelper.MakeCall<IOrderListProcessor, OrderListDO>(x => x.Process(sr));

			// Bind control data
			this.ProductDropDown.DataSource = this.orderHeaderDO.ProductList;
			this.ProductDropDown.DataBind();

			this.OrderStatusDropDownList.DataSource = this.orderHeaderDO.OrderStatusList;
			this.OrderStatusDropDownList.DataBind();

			this.ChangeOrderStatusDropdownlist.DataSource = this.orderHeaderDO.OrderStatusList;
			this.ChangeOrderStatusDropdownlist.DataBind();

			this.OrderTypeDropDown.DataSource = this.orderHeaderDO.OrderTypeList;
			this.OrderTypeDropDown.DataBind();

			this.BillToTextBox.Text = sr.AllText;
			this.ShipToTextBox.Text = sr.AllText;
			this.ShipperTextBox.Text = sr.AllText;
			this.CarrierTextBox.Text = sr.AllText;
			this.OwnerTextBox.Text = sr.AllText;
			this.ManagerTextBox.Text = sr.AllText;

			// Load the date filter type drop down
			LoadDateFilterType(this.DateFilterTypeDropDown);
			SetDateFieldsAccessibility();

			// Load any persisted context data
			this.LoadPersistedFilters();
		}

		private void LoadPersistedFilters()
		{
			var timer = new StopWatch(StopWatch.Appnames.OrderEntry, "OrderSummary.LoadPersistedFilters()");

			var oContext = this.Session["OrderSummaryContext"] as OrderSummaryContext;

			if (oContext != null)
			{
				// Load the controls
				this.LoadControlValue(this.ProductDropDown, oContext.Product);
				this.LoadControlValue(this.OrderStatusDropDownList, oContext.OrderStatus);
				this.LoadControlValue(this.OrderTypeDropDown, oContext.OrderType);

				// Finish loading the controls
				this.StartDate.Text = this.accountingSite.FormatDate(oContext.StartDate);
				this.EndDate.Text = this.accountingSite.FormatDate(oContext.EndDate);

				this.BillToTextBox.Text = oContext.BillTo;
				this.BillToTextBox.ToolTip = oContext.BillToTip;

				this.ShipperTextBox.Text = oContext.Shipper;
				this.ShipperTextBox.ToolTip = oContext.ShipperTip;

				this.ShipToTextBox.Text = oContext.ShipTo;
				this.ShipperTextBox.ToolTip = oContext.ShipToTip;

				this.CarrierTextBox.Text = oContext.Carrier;
				this.CarrierTextBox.ToolTip = oContext.CarrierTip;

				this.OwnerTextBox.Text = oContext.Owner;
				this.OwnerTextBox.ToolTip = oContext.OwnerTip;

				this.ManagerTextBox.Text = oContext.Manager;
				this.ManagerTextBox.ToolTip = oContext.ManagerTip;

				this.DateFilterTypeDropDown.SelectedIndex = oContext.DateFilterType;

				this.OrderNumberTextBox.Text = oContext.OrderNumber;
			}
			else
			{
				DateTimeOffset today = TimeConverter.Today();
				this.StartDate.Text = this.accountingSite.FormatDate(today);
				this.EndDate.Text = this.accountingSite.FormatDate(today.AddDays(1));
			}

			timer.Stop();
		}

		private void SetDateFieldsAccessibility()
		{
			this.StartDate.Enabled = true;
			this.EndDate.Enabled = true;
			if (this.DateFilterTypeDropDown.SelectedValue == OrderListFilterCriteria.OrderDateFilterType.NONE.ToString())
			{
				this.StartDate.Enabled = false;
				this.EndDate.Enabled = false;
			}
		}

		private TransactionDO LoadTransaction(string transID)
		{
			var accountingSiteLocal =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

			var sr = new TransactionSR
			{
				Security = this.security,
				TransID = transID,
				AccountingSite = accountingSiteLocal
			};

			var transLocal = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));

			return transLocal;
		}

		/// <summary>
		/// Handles the TextChanged event of the OrderNumberTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OrderNumberTextBoxTextChanged(object sender, EventArgs e)
		{
			var oContext = this.Session["OrderSummaryContext"] as OrderSummaryContext;

			if (oContext != null)
			{
				oContext.OrderNumber = this.OrderNumberTextBox.Text;
				this.Session["OrderSummaryContext"] = oContext;
				this.LoadPersistedFilters();
			}
		}


		private void PageLoadProcessing()
		{
			// Clear warning label
			this.WarningLabel.Text = string.Empty;

			this.GetSecurity();

			this.CleanUpGrid();

			// Bind controls events
			this.BindControls();

			// Get site information.
			this.accountingSite =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

			this.accountingSite.GetUserCompanies = false;

			// this control is hidden on the web app. this is necessary to control the auto populate capability
			this.InhibitAutoLoadTextBox.Text = this.accountingSite.LoginSite.InhibitOrderSummaryAutoPopulate ? "true" : "false";

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

				// Set initial sort
				var gridSort = this.Session["OrderList.grid.sort"] as String;
				if (string.IsNullOrEmpty(gridSort))
				{
					this.Session["OrderList.grid.sort"] = "Transaction Date";
					this.Session["OrderList.grid.sort.direction"] = false;
					this.Session["OrderList.SortExpression"] = "TransactionDate DESC";
				}

				// Check the user's security access
				CheckUserSecurityAccess(this.security);

				// Load the dropdown boxes
				this.LoadDropDownBoxes();

				// Save Filter Settings
				this.PersistFilterCriteria();

				// check if there is a type 17 transaction available and set the edit button accordingly
				this.bEnableEdit = this.DetermineIfOrderTransExist();

				// Refresh the data in the list view
				this.UpdateView();
			}
		}

		private void PersistFilterCriteria()
		{
			// Save the filter criteria
			var context = new OrderSummaryContext
			{
				Product = this.GetValue(this.ProductDropDown),
				OrderStatus = this.GetValue(this.OrderStatusDropDownList),
				OrderType = this.GetValue(this.OrderTypeDropDown),
				BillTo = this.BillToTextBox.Text,
				BillToTip = this.BillToTextBox.ToolTip,
				ShipTo = this.ShipToTextBox.Text,
				ShipToTip = this.ShipperTextBox.ToolTip,
				Shipper = this.ShipperTextBox.Text,
				ShipperTip = this.ShipperTextBox.ToolTip,
				Carrier = this.CarrierTextBox.Text,
				CarrierTip = this.CarrierTextBox.ToolTip,
				Owner = this.OwnerTextBox.Text,
				OwnerTip = this.OwnerTextBox.ToolTip,
				Manager = this.ManagerTextBox.Text,
				ManagerTip = this.ManagerTextBox.ToolTip,
				OrderNumber = this.OrderNumberTextBox.Text
			};

			// Load the context object

			if (this.StartDate.Text != "")
			{
				// the start date needs to be at midnight of the selected date
				context.StartDate = this.SetTimeToBeginningOrEnd(this.StartDate.CurrentValue, TimeSetting.Begin);
			}
			if (this.EndDate.Text != "")
			{
				// the end date needs to be at 23:59:59 of the selected date
				context.EndDate = this.SetTimeToBeginningOrEnd(this.EndDate.CurrentValue, TimeSetting.End);
			}

			context.DateFilterType = this.DateFilterTypeDropDown.SelectedIndex;

			var sortExpression = this.Session["OrderList.SortExpression"] as string;
			if (sortExpression != null)
			{
				context.SortExpression = sortExpression;
			}

			// Save it as a session variable
			this.Session["OrderSummaryContext"] = context;
		}


		/// <summary>
		/// This method will return true if the start and end dates are valid. Valid
		/// dates means that the if the end date is populated, then the start date
		/// must be populated. The start date must be older than the end date.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		private bool AreDatesValid(ref string msg)
		{
			bool datesAreValid = true;
			DateTimeOffset dt;
			bool validStartDate = true;
			bool validEndDate = true;

			if (this.DateFilterTypeDropDown.SelectedValue == OrderListFilterCriteria.OrderDateFilterType.NONE.ToString())
			{
				return true;
			}

			var currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																x =>
																x.Get(security, security.SiteGuid, false, false, false));
			try
			{
				dt = new DateTimeOffset(DateTime.Parse(this.StartDate.Text, this.StartDate.FormatInfo, DateTimeStyles.None).Date, TimeConverter.Today(currentSite).Offset);
			}
			catch
			{
				validStartDate = false;
			}

			try
			{
				dt = new DateTimeOffset(DateTime.Parse(this.EndDate.Text, this.EndDate.FormatInfo, DateTimeStyles.None).Date, TimeConverter.Today(currentSite).Offset);
			}
			catch
			{
				validEndDate = false;
			}

			if (this.StartDate.Text == "")
			{
				datesAreValid = false;
				msg = ErrorMsg001;
			}
			if (this.EndDate.Text == "")
			{
				datesAreValid = false;
				msg = ErrorMsg002;
			}
			else if (!validStartDate)
			{
				datesAreValid = false;
				msg = ErrorMsg003;
			}
			else if (!validEndDate)
			{
				datesAreValid = false;
				msg = ErrorMsg004;
			}
			else if (this.StartDate.CurrentValue > this.EndDate.CurrentValue)
			{
				datesAreValid = false;
				msg = ErrorMsg005;
			}

			return datesAreValid;
		}


		private void SaveTransaction(TransactionDO trans)
		{
			var sr = new SaveTransactionsSR
			{
				Security = this.security,
				UseAutoComplete = true,
				CurrentSiteGuid = this.security.SiteGuid
			};

			sr.Transactions.Add(trans);

			try
			{
				FMChannelHelper.MakeCall<ISaveTransactionsProcessor>(x => x.SaveTransactions(sr));
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		private DateTimeOffset SetTimeToBeginningOrEnd(DateTimeOffset inDate, TimeSetting timeSetting)
		{
			DateTimeOffset outDate;

			switch (timeSetting)
			{
				case TimeSetting.Begin:
					outDate = TimeConverter.ToStartOfDay(inDate);
					break;
				case TimeSetting.End:
					outDate = TimeConverter.ToEndOfDay(inDate);
					break;
				default:
					outDate = TimeConverter.ToStartOfDay(inDate);
					break;
			}

			return outDate;
		}

		private void UpdateView()
		{
			var timer = new StopWatch(StopWatch.Appnames.OrderEntry, "OrderSummary.UpdateView()");
			string errorMessage = "";

			// Ensure that the dates are valid.
			if (this.AreDatesValid(ref errorMessage) == false)
			{
				var except = new Exception(errorMessage);
				this.ErrorHandler(except);
			}
			else
			{
				grid = new ListViewDataSet(
					this.security,
					LISTVIEW_TYPE.STANDARD,
					ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.ORDER),
					this.accountingSite);

				this.grid.SetDataGrid(this.TransactionDataGrid);

				var sortDirection = false;
				if (this.Session["OrderList.grid.sort.direction"] != null)
				{
					sortDirection = (bool)this.Session["OrderList.grid.sort.direction"];
				}

				var sort = this.Session["OrderList.grid.sort"] as string;
				string sortExpression = "TransactionDate DESC";
				if (sort != null)
				{
					sortExpression = this.grid.GetDataPath(sort);
					if (sortExpression == string.Empty)
					{
						sortExpression = "TransactionDate";
					}

					this.grid.Sort = sort;
					this.grid.SortDirection = sortDirection;
					sortExpression += sortDirection ? string.Empty : " DESC";
				}

				this.Session["OrderList.SortExpression"] = sortExpression;

				// Set for sorting messages
				this.TransactionDataGrid.AllowSorting = true;
				this.TransactionDataGrid.SortCommand += this.DataGridSortCommand;

				// Save the order list
				this.Session["OrderList.ListView"] = this.grid;

				// Create and populate our service request
				var sr = new OrderListSR
				{
					SubRequest = OrderListSR.RequestTypes.GET_DETAIL,
					Security = this.security,
					Criteria =
				{
					Security = this.security,
					Product = this.GetValue(this.ProductDropDown),
					Status = this.GetValue(this.OrderStatusDropDownList),
					OrderType = this.GetValue(this.OrderTypeDropDown),
					BillTo = this.GetValue(this.BillToTextBox),
					Carrier = this.GetValue(this.CarrierTextBox),
					Manager = this.GetValue(this.ManagerTextBox),
					Owner = this.GetValue(this.OwnerTextBox),
					Shipper = this.GetValue(this.ShipperTextBox),
					ShipTo = this.GetValue(this.ShipToTextBox),
					OrderNumber = this.OrderNumberTextBox.Text,
					SortExpression = sortExpression
				}
				};

				// Set the filtering criteria
				this.GetValidatedDates(sr);

				// Process the service request
				var orderListDO = FMChannelHelper.MakeCall<IOrderListProcessor, OrderListDO>(x => x.Process(sr));
				this.Session["OrderList.OrderListDO"] = orderListDO;

				// Honor regional settings for date
				this.FormatForRegionalSettings(orderListDO);

				// Bind up the data
				grid.BindData(
					orderListDO.LineItems,
					QuantityDisplay.NET,
					this.accountingSite.CurrentSite._VolumeDecimalPlaces,
					this.accountingSite.CurrentSite._MassDecimalPlaces,
					false);

				// Do we need to warn about the result size?
				if (orderListDO.LineItems.Count >= 500)
				{
					// Issue the warning message
					this.WarningLabel.Text = "Results limited to first 500 records.  Use filters to narrow search.";
				}

				timer.Stop();
			}
		}

		/// <summary>
		/// Handles the ItemDataBound event of the dataGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
		private void DataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex > -1)
			{
				if (grid != null)
				{
					for (int cellIndex = 2; cellIndex < e.Item.Cells.Count; ++cellIndex)
					{
						ListViewColumnDO columnDO = grid.listViewDO[cellIndex - 2];

						if (columnDO != null)
						{
							TableCell cell = e.Item.Cells[cellIndex];

							int realIndex = (this.TransactionDataGrid.PageSize * this.TransactionDataGrid.CurrentPageIndex) + e.Item.ItemIndex;

							cell.ToolTip = this.GetToolTip(columnDO.ColumnName, realIndex);
						}
						else
						{

						}
					}

					int iHiddenColCount = 0;
					if (grid.GetHiddenColumnList() != null)
							iHiddenColCount = grid.GetHiddenColumnList().Count;
					int iTotalColCount = e.Item.Cells.Count;
					//Hide the columns marked for hiding, using the fact that columns marked for hiding are appended at the end of the dataset (see ListViewDataSet)
					for (int i = 0; i < iHiddenColCount; i++)
					{
						e.Item.Cells[iTotalColCount - 1 - i].Visible = false;
					}

					try
					{
						var editButton = (FMEditLinkButton)e.Item.FindControl("btnEdit");

						// Disable the edit button if there is no transaction type 17
						if (editButton != null)
						{
							editButton.Enabled = this.bEnableEdit;
							var dataTable = (DataTable)this.TransactionDataGrid.DataSource;

							DataRow datarow = dataTable.Rows[e.Item.ItemIndex];
							if (!datarow.IsNull("DeleteFlag"))
							{
								bool deleteflag = DataObject.getValue(datarow["DeleteFlag"], false);

								if (deleteflag)
								{
									editButton.ShowDeleted = true;
								}
							}
						}
					}
					catch (Exception except)
					{
						this.ErrorHandler(except);
					}

				}
			}
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
				this.Session["OrderList.grid.sort"] = e.SortExpression;

				bool sortDirection = false;
				if (this.Session["OrderList.grid.sort.direction"] != null)
				{
					sortDirection = !((bool)this.Session["OrderList.grid.sort.direction"]);
				}

				this.Session["OrderList.grid.sort.direction"] = sortDirection;

				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		#endregion

		protected void DateFilterTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetDateFieldsAccessibility();
		}


		/// <summary>
		/// Handles an ItemCreated event on the data grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void LineItemDataGridItemCreated(Object sender, DataGridItemEventArgs e)
		{
			int iHiddenColCount = 0;
			if (this.TransactionDataGrid.DataSource != null)
			{
				System.Data.DataTable dt = (System.Data.DataTable)(this.TransactionDataGrid.DataSource);
				ListViewDataSet lvds = (ListViewDataSet)(dt.DataSet);
				if (lvds.GetHiddenColumnList() != null)
					iHiddenColCount = lvds.GetHiddenColumnList().Count;
			}
			int iTotalColCount = e.Item.Cells.Count;
			//Hide the columns marked for hiding, using the fact that columns marked for hiding are appended at the end of the dataset (see ListViewDataSet)
			for (int i = 0; i < iHiddenColCount; i++)
			{
				e.Item.Cells[iTotalColCount - 1 - i].Visible = false;
			}
		}
	}
}
