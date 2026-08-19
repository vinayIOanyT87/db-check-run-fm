// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupplyOrderSummary.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SupplyOrderSummaryForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.SupplyOrderWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Web;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.LogClient;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMControls;

    using FuelsManager.Accounting;

    using TransactionFields;

    public partial class SupplyOrderSummaryForm : AccountingAutoSubmitWebFormView
	{
		#region Protected data members
		protected FMLabel Fmlabel3;
		protected FMLabel Fmlabel5;
		protected FMLabel Label1;
		#endregion

		#region Private data members
		private AccountingSite accountingSite;
		private ListViewDataSet grid;
		private bool bEnableEdit = true;
		private enum TimeSetting { Begin, End };
		private const string ErrorMsg001 = "Must have a Start Date.";
		private const string ErrorMsg002 = "Must have an End Date.";
		private const string ErrorMsg003 = "Invalid Start Date.";
		private const string ErrorMsg004 = "Invalid End Date.";
		private const string ErrorMsg005 = "Start Date must be before the End Date.";

		#endregion

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
			this.TransactionDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.LineItemDataGridPageIndexChanged);
			this.TransactionDataGrid.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGridSortCommand);
			this.TransactionDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridItemDataBound);

		}
		#endregion

		//*************************************************************************
		// Member functions
		//*************************************************************************    

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.PageLoadProcessing();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		private void PageLoadProcessing()
		{
			// Clear warning label
			this.WarningLabel.Text = "";

			this.CleanUpGrid();

			// Bind controls events
			this.BindControls();

			// Get site information.
			this.accountingSite =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

			this.accountingSite.GetUserCompanies = false;

			// this control is hidden on the web app. this is necessary to control the auto populate capability
			this.InhibitAutoLoadTextBox.Text = this.accountingSite.LoginSite.InhibitSupplyOrderSummaryAutoPopulate ? "true" : "false";

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
				var gridSort = this.Session["SupplyOrderList.grid.sort"] as String;
				if (string.IsNullOrEmpty(gridSort))
				{
					this.Session["SupplyOrderList.grid.sort"] = "Transaction Date";
					this.Session["SupplyOrderList.grid.sort.direction"] = false;
				}

				// Check the user's security access
				CheckUserSecurityAccess(this.security);

				// Load the dropdown boxes
				this.LoadDropDownBoxes();

				// Save Filter Settings
				this.PersistFilterCriteria();

				// check if there is a type 18 transaction available and set the edit button accordingly
				this.bEnableEdit = this.DetermineIfSupplyOrderTransExist();

				// Refresh the data in the list view
				this.UpdateView();

			}
		}


		private void BindControls()
		{
			this.TransactionDataGrid.ItemCommand += this.LineItemDataGridItemCommand;
		}


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


		private void LineItemDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
			// Is this the View link?
			if (e.CommandName == "Edit")
			{
				if (this.DetermineIfSupplyOrderTransExist() == false)
				{
					var noAliasDefined = new Exception("No Supply Order type transaction configured");
				    this.ErrorHandler(noAliasDefined);
				}
				else
				{
					// Get the data set
					var supplyorderListDO = this.Session["SupplyOrderList.SupplyOrderListDO"] as SupplyOrderListDO;
					if (supplyorderListDO == null)
					{
						// No object so we don't know under what context the button was pressed!
						throw new Exception("SupplyOrderList.LineItemDataGrid_ItemCommand expected session to contain SupplyOrderListDO");
					}

					//Create session object for TransactionDetail list of transactions.
					var detailList = new TransactionDetailList();

					// Determine which index to use
					int realIndex = (this.TransactionDataGrid.CurrentPageIndex * this.TransactionDataGrid.PageSize) + e.Item.ItemIndex;

					// Build the list of transactions to display
					if (realIndex < supplyorderListDO.LineItems.Count)
					{
						foreach (SupplyOrderListLineItemDO lineItem in supplyorderListDO.LineItems)
						{
							detailList.TransactionIDList.Add(lineItem.TransactionID);
						}

						//Indicate which transaction id in the list is the one to initially display.
						detailList.CurrentIndex = realIndex;

						//Indicate the return URL for when the TransactionDetail Close button is clicked.
						detailList.ReturnURL = "..\\SupplyOrderWebApp\\SupplyOrderSummary.aspx";

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
		/// This method returns true if the site has the transaction alias to display supply order transactions.
		/// Otherwise, it will return false.
		/// </summary>
		/// <returns></returns>
		private bool DetermineIfSupplyOrderTransExist()
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
					FMChannelHelper.MakeCall<ITransactionAliasListProcessor, TransactionAliasListDO>(x => x.Process(transAliasListSR));

				if (transAliasListDO != null)
				{
					var enumerator = transAliasListDO.aliasList.GetEnumerator();
					while (enumerator.MoveNext() )
					{
						var transAlias = enumerator.Value as TransactionAliasClass;

					    if (transAlias?.TransTypeID == TransactionTypes.T18_SupplyOrder)
					    {
					        transAliasExist = true;
					        break;
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
		/// Responsible for loading the dropdown boxes with appropriate selection choices
		/// </summary>
		private void LoadDropDownBoxes()
		{
			// Request updated data from the database.
			// Create and populate our service request.
			var sr = new SupplyOrderListSR
			         {
				        SubRequest = SupplyOrderListSR.RequestTypes.GET_HEADER_DATA,
						Security = this.security,
						AllText = GetDataDictionaryValueByKey(this.Security.SiteGuid, "{All}")
			};

			// Process the service request
			var supplyorderListDO =
				FMChannelHelper.MakeCall<ISupplyOrderListProcessor, SupplyOrderListDO>(x => x.Process(sr));

			// Bind control data
			this.ProductDropDown.DataSource = supplyorderListDO.ProductList;
			this.ProductDropDown.DataBind();

			this.OrderStatusDropDownList.DataSource = supplyorderListDO.OrderStatusList;
			this.OrderStatusDropDownList.DataBind();

			this.OrderTypeDropDown.DataSource = supplyorderListDO.OrderTypeList;
			this.OrderTypeDropDown.DataBind();

			this.ShipperTextBox.Text = sr.AllText;
			this.SupplierTextBox.Text = sr.AllText;
			this.OwnerTextBox.Text = sr.AllText;
			this.ManagerTextBox.Text = sr.AllText;

			// Load the date filter type drop down
			LoadDateFilterType(this.DateFilterTypeDropDown);
			SetDateFieldsAccessibility();

			// Load any persisted context data
			this.LoadPersistedFilters();
		}


		private static void LoadDateFilterType(FMDropDownList dateFilterTypeDropDown)
		{
			if (dateFilterTypeDropDown == null)
			{
				throw new ArgumentNullException();
			}

			var item = new ListItem("No filter",
				SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.NONE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("Estimated Delivery Date",
				SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.ESTIMATED_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("Required Delivery Date",
				SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.REQUIRED_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			item = new ListItem("Transaction Date",
				SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.TRANSACTION_DATE.ToString());
			dateFilterTypeDropDown.Items.Add(item);

			// Set the default date filter.
			dateFilterTypeDropDown.SelectByText("No filter");
		}


		/// <summary>
		/// This function is responsible for checking the current user's security access
		/// and responding appropriately including enforcing access and changing control
		/// availability.
		/// </summary>
		private static void CheckUserSecurityAccess(SecurityClass securityObject)
		{
			if (securityObject == null)
			{
				throw new ArgumentNullException();
			}
			if (!securityObject.HasViewTransactionRightByTransTypeID(TransactionTypes.T18_SupplyOrder))
			{
				throw new FMInsufficientRightsException();
			}

		}

		protected void RefreshButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.PersistFilterCriteria();
				// check if there is a type 18 transaction available and set the edit button accordingly
				this.bEnableEdit = this.DetermineIfSupplyOrderTransExist();
				this.TransactionDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}

		}


		private string GetValue(DropDownList dropDownList)
		{
			if (dropDownList.SelectedIndex == 0)
			{
				return "";
			}

			return dropDownList.SelectedValue;

		}

		private string GetValue(FMCompanyTextBox companyTextBox)
		{
			if (companyTextBox.Text == "{All}")
			{
				return "";
			}

			return companyTextBox.Text;

		}


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


		private void UpdateView()
		{
			var timer = new StopWatch(StopWatch.Appnames.SupplyOrderWebApp, "SupplyOrderSummary.UpdateView()");
			string errorMessage = "";


			// Ensure that the dates are valid.
			if (this.AreDatesValid(ref errorMessage) == false)
			{
				var except = new Exception(errorMessage);
			    this.ErrorHandler(except);
			}
			else
			{

				this.grid = new ListViewDataSet(this.security, LISTVIEW_TYPE.STANDARD,
														ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER), this.accountingSite);

				this.grid.SetDataGrid(this.TransactionDataGrid);

				var sortDirection = false;
				if (this.Session["SupplyOrderList.grid.sort.direction"] != null)
				{
					sortDirection = (bool)this.Session["SupplyOrderList.grid.sort.direction"];
				}

				var sort = this.Session["SupplyOrderList.grid.sort"] as string;
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

				// Create and populate our service request
				var sr = new SupplyOrderListSR
				         {
					         SubRequest = SupplyOrderListSR.RequestTypes.GET_DETAIL,
					         Security = this.security,
					         Criteria =
					         {
						         Security = this.security,
						         Product = this.GetValue(this.ProductDropDown),
						         Status = this.GetValue(this.OrderStatusDropDownList),
						         OrderType = this.GetValue(this.OrderTypeDropDown),
						         Manager = this.GetValue(this.ManagerTextBox),
						         Owner = this.GetValue(this.OwnerTextBox),
						         Shipper = this.GetValue(this.ShipperTextBox),
						         Supplier = this.GetValue(this.SupplierTextBox),
						         OrderNumber = this.OrderNumberTextBox.Text,
						         SortExpression = sortExpression
					         }
				         };

				this.GetValidatedDates(sr);

				// Process the service request
				SupplyOrderListDO supplyorderListDO =
					FMChannelHelper.MakeCall<ISupplyOrderListProcessor, SupplyOrderListDO>(x => x.Process(sr));

				this.Session["SupplyOrderList.SupplyOrderListDO"] = supplyorderListDO;

				// Honor regional settings for date
				this.FormatForRegionalSettings(supplyorderListDO);
	

				
				// Set the page size control
				this.OrderSummarySizeDropDown.SetPageSize(this.grid.dataGrid, supplyorderListDO.LineItems.Count);


				// Bind up the data
				this.grid.BindData(supplyorderListDO.LineItems, QuantityDisplay.NET, this.accountingSite.CurrentSite._VolumeDecimalPlaces, this.accountingSite.CurrentSite._MassDecimalPlaces, false);

				// Do we need to warn about the result size?
				if (supplyorderListDO.LineItems.Count >= 500)
				{
					// Issue the warning message
					this.WarningLabel.Text = "Results limited to first 500 records.  Use filters to narrow search.";
				}

				timer.Stop();
			}
		}

		/// <summary>
		/// This method will set the Service Request with the valid start and end
		/// dates. It will convert the date to the server date UTC.
		/// </summary>
		/// <param name="sr"></param>
		private void GetValidatedDates(SupplyOrderListSR sr)
		{
			if (this.StartDate.Text != "")
			{
				DateTimeOffset tempDateTime = this.SetTimeToBeginningOrEnd(this.StartDate.CurrentValue, TimeSetting.Begin);
				sr.Criteria.StartDate = TimeConverter.ToUTCTime(tempDateTime);
			}

			// Get the end of the date range
			if (this.EndDate.Text != "")
			{
				DateTimeOffset tempDateTime = this.SetTimeToBeginningOrEnd(this.EndDate.CurrentValue, TimeSetting.End);
				sr.Criteria.EndDate = TimeConverter.ToUTCTime(tempDateTime);
			}

			// Get the date filter type
			if ((this.StartDate.Text == "") && (this.EndDate.Text == ""))
			{
				sr.Criteria.DateFilterType = SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.NONE;
				this.DateFilterTypeDropDown.SelectedIndex = 0;
			}
			else
			{
				sr.Criteria.DateFilterType = (SupplyOrderListFilterCriteria.SupplyOrderDateFilterType)
					Enum.Parse(typeof(SupplyOrderListFilterCriteria.SupplyOrderDateFilterType), this.DateFilterTypeDropDown.SelectedValue);
			}
		}

		/// <summary>
		/// This method will return a date string with the time portion set to zeroes.
		/// </summary>
		/// <param name="inDate"></param>
		/// <param name="timeSetting"></param>
		/// <returns></returns>
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

		/// <summary>
		/// This method will persist all the filter information into a context object that is saved
		/// into session.
		/// </summary>
		private void PersistFilterCriteria()
		{
			var timer = new StopWatch(StopWatch.Appnames.SupplyOrderWebApp, "SupplyOrderSummary.PersistFilterCriteria()");

			// Save the filter criteria
			var context = new SupplyOrderSummaryContext
			              {
				              Product = this.GetValue(this.ProductDropDown),
				              OrderStatus = this.GetValue(this.OrderStatusDropDownList),
				              OrderType = this.GetValue(this.OrderTypeDropDown),
				              Shipper = this.ShipperTextBox.Text,
				              ShipperTip = this.ShipperTextBox.ToolTip,
				              Supplier = this.SupplierTextBox.Text,
				              SupplierTip = this.SupplierTextBox.ToolTip,
				              Owner = this.OwnerTextBox.Text,
				              OwnerTip = this.OwnerTextBox.ToolTip,
				              Manager = this.ManagerTextBox.Text,
				              ManagerTip = this.ManagerTextBox.ToolTip,
							  OrderNumber = this.OrderNumberTextBox.Text
			              };

			// Load the context object

			if (this.StartDate.Text != "")
			{
				context.StartDate = this.StartDate.CurrentValue;
			}

			if (this.EndDate.Text != "")
			{
				context.EndDate = this.EndDate.CurrentValue;
			}

			context.DateFilterType = this.DateFilterTypeDropDown.SelectedIndex;

			var sort = this.Session["SupplyOrderList.SortExpression"] as string;
			if (sort != null)
			{
				context.SortExpression = sort;
			}

			// Save it as a session variable
			this.Session["SupplyOrderSummaryContext"] = context;

			timer.Stop();
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


		private void LoadPersistedFilters()
		{
			var timer = new StopWatch(StopWatch.Appnames.SupplyOrderWebApp, "SupplyOrderSummary.LoadPersistedFilters()");

			var oContext = this.Session["SupplyOrderSummaryContext"] as SupplyOrderSummaryContext;

			if (oContext != null)
			{
				// Load the controls
				this.LoadControlValue(this.ProductDropDown, oContext.Product);
				this.LoadControlValue(this.OrderStatusDropDownList, oContext.OrderStatus);
				this.LoadControlValue(this.OrderTypeDropDown, oContext.OrderType);

				// Finish loading the controls
				this.StartDate.Text = this.accountingSite.FormatDate(oContext.StartDate);
				this.EndDate.Text = this.accountingSite.FormatDate(oContext.EndDate);

				this.ShipperTextBox.Text = oContext.Shipper;
				this.ShipperTextBox.ToolTip = oContext.ShipperTip;

				this.SupplierTextBox.Text = oContext.Supplier;
				this.SupplierTextBox.ToolTip = oContext.SupplierTip;

				this.OwnerTextBox.Text = oContext.Owner;
				this.OwnerTextBox.ToolTip = oContext.OwnerTip;

				this.ManagerTextBox.Text = oContext.Manager;
				this.ManagerTextBox.ToolTip = oContext.ManagerTip;

				this.DateFilterTypeDropDown.SelectedIndex = oContext.DateFilterType;

				this.OrderNumberTextBox.Text = oContext.OrderNumber;
			}
			else
			{
				DateTimeOffset today = TimeConverter.Today(this.accountingSite.CurrentSite);
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


		private void LoadControlValue(DropDownList dropDownList, string value)
		{
			if (value != "")
			{
				dropDownList.SelectedValue = value;
			}
		}


		private void DataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex > -1)
			{
				if (this.grid != null)
				{
					for (int cellIndex = 2; cellIndex < e.Item.Cells.Count; ++cellIndex)
					{
					    TableCell cell = e.Item.Cells[cellIndex];

						int realIndex = (this.TransactionDataGrid.PageSize * this.TransactionDataGrid.CurrentPageIndex) + e.Item.ItemIndex;

						cell.ToolTip = this.GetToolTip(realIndex);
					}
				}

				try
				{
					var editButton = (LinkButton)e.Item.FindControl("btnEdit");

					// Disable the edit button if there is no transaction type 18
					if (editButton != null)
					{
						editButton.Enabled = this.bEnableEdit;
					}
				}
				catch (Exception except)
				{
				    this.ErrorHandler(except);
				}
			}
		}


		private string GetToolTip(int index)
		{
			SupplyOrderListDO supplyOrderList = this.Session["SupplyOrderList.SupplyOrderListDO"] as SupplyOrderListDO;

			string returnValue = "";

		    if (index < supplyOrderList?.LineItems.Count)
		    {
		        SupplyOrderListLineItemDO lineItem = supplyOrderList.LineItems[index] as SupplyOrderListLineItemDO;

		        if (lineItem != null)
		        {
		        }
		    }

		    return returnValue;
		}


/*
		private string BuildToolTip(string Name, string Address, string City, string State)
		{
			string returnValue = "";

			ArrayList Values = new ArrayList();

			if (Name != null && Name != "")
			{
				Values.Add(Name);
			}

			if (Address != null && Address != "")
			{
				Values.Add(Address);
			}

			if (City != null && City != "")
			{
				Values.Add(City);
			}

			if (State != null && State != "")
			{
				Values.Add(State);
			}

			if (Values.Count > 0)
			{
				returnValue = Values[0] as string;

				for (int nLoop = 2; nLoop < Values.Count; ++nLoop)
				{
					returnValue += ", " + Values[nLoop] as string;
				}

			}

			return returnValue;

		}
*/


		private void DataGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			try
			{
				// Find the Sort Data Path
				this.Session["SupplyOrderList.grid.sort"] = e.SortExpression;

				bool sortDirection = false;
				if (this.Session["SupplyOrderList.grid.sort.direction"] != null)
				{
					sortDirection = !((bool) this.Session["SupplyOrderList.grid.sort.direction"]);
				}

				this.Session["SupplyOrderList.grid.sort.direction"] = sortDirection;

				this.UpdateView();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		private void FormatForRegionalSettings(SupplyOrderListDO supplyorderListDO)
		{
			foreach (SupplyOrderListLineItemDO item in supplyorderListDO.LineItems)
			{
				item.TransactionDate = this.accountingSite.FormatDate(item.TransactionDateTime);

				if (item.RequiredDeliveryDate != "")
				{
					item.RequiredDeliveryDate = this.accountingSite.FormatDateTime(item.RequiredDeliveryDateTime);
				}

				if (item.EstimatedDeliveryDateFrom != "")
				{
					item.EstimatedDeliveryDateFrom = this.accountingSite.FormatDate(item.EstimatedDeliveryDateFromTime);
				}

				if (item.EstimatedDeliveryDateTo != "")
				{
					item.EstimatedDeliveryDateTo = this.accountingSite.FormatDate(item.EstimatedDeliveryDateToTime);
				}

				if (item.InventoryDate != "")
				{
					item.InventoryDate = this.accountingSite.FormatDate(item.InventoryDateTime);
				}
			}
		}

/*
		private void OrderNumberTextBox_TextChanged(object sender, EventArgs e)
		{
			SupplyOrderSummaryContext oContext = this.Session["SupplyOrderSummaryContext"] as SupplyOrderSummaryContext;

			if (oContext != null)
			{
				oContext.OrderNumber = this.OrderNumberTextBox.Text;
			    this.Session["SupplyOrderSummaryContext"] = oContext;
			    this.LoadPersistedFilters();
			}
		}
*/

		protected Dictionary<string, string>
		GetSelectedTransactionAndAlises(ref SupplyOrderListDO orderList, ref TransactionDetailList detailList, ref TransactionDO trans)
		{
			var result = new Dictionary<string, string>();

			for (int index = 0; index < this.TransactionDataGrid.Items.Count; ++index)
			{
				try
				{
					var fmButton = (CheckBox)this.TransactionDataGrid.Items[index].Cells[1].FindControl("MultipleSelectCheckbox");

					if (fmButton.Checked )
					{
						// Determine which index to use
						int realIndex = (this.TransactionDataGrid.CurrentPageIndex * this.TransactionDataGrid.PageSize) + index;

						if (realIndex < orderList.LineItems.Count)
						{
							foreach (SupplyOrderListLineItemDO lineItem in orderList.LineItems)
							{
								detailList.TransactionIDList.Add(lineItem.TransactionID);
							}

							//Indicate which transaction id in the list is the one to initially display.
							detailList.CurrentIndex = realIndex;

							string transID = detailList.TransactionIDList[detailList.CurrentIndex];

							// accountingSite
							trans = this.LoadTransaction(transID);
							result.Add(transID, trans.Alias);
						}
					}
				}
				catch
				{
					break;
				}
			}

			return result;
		}

		protected void PrintSelectionClick(object sender, EventArgs e)
		{
			TransactionDO trans = null;
			
			var orderList = this.Session["SupplyOrderList.SupplyOrderListDO"] as SupplyOrderListDO;

			if (orderList == null)
			{
				// No order list, then leave.
				return;
			}

			//Create session object for TransactionDetail list of transactions.
			var detailList = new TransactionDetailList();

			Dictionary<string, string> selectedTransactionsAndAliases = this.GetSelectedTransactionAndAlises(ref orderList, ref detailList, ref trans);

			// Now we have a dictionary of the transaction ids and their aliases.
			// To print the new SupplyOrder report (allowing multiple Supply Orderss on a single report), all transactions must the same alias.
			// Also, one final check that we have any transactions
			string reportAlias = "";
			string transactionIdList = "";
			bool allSameAlias = true;
			if (selectedTransactionsAndAliases.Count == 0)
			{
				return;
			}

			foreach (KeyValuePair<string, string> transactionAndAliasPair in selectedTransactionsAndAliases)
			{
				if (string.IsNullOrEmpty(reportAlias) == false &&
					 reportAlias.Equals(transactionAndAliasPair.Value) == false)
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
				// No alias name found.  Should not happen.
				return;
			}

			if (allSameAlias == false)
			{
				return;
			}

			// reportAlias has our one and only involved alias.
			// get the associated report for this order
			bool dataDictionary = false;
			if (this.Session["UseDataDictionary"] != null)
			{
				dataDictionary = (bool)this.Session["UseDataDictionary"];
			}

			var transContext = new TransactionContext(this.security,this.accountingSite,reportAlias,TransactionContext.Mode.Edit,dataDictionary);

			transContext.GetTransactionContext();

			string supplyOrderRptType = ((int)ReportTypesClass.ReportTypes.BOL_RPT).ToString(CultureInfo.InvariantCulture);
			string stRptName = transContext.aliasClass.AssociatedReport;
            
            string rptUrl = "../FMReportWebMain/ReportLandingPage.aspx?ReportType=" + supplyOrderRptType;
			string reportName = HttpUtility.HtmlEncode(stRptName);//stRptName.Replace(" ", "+");

         // JS20100618 WI-14915 check there is a report associated with the selected transaction
         if (string.IsNullOrEmpty(reportName))
			{
			    this.ErrorHandler(new Exception("Cannot print selected transaction(s) because they have no reports associated."));
				return;
			}

			rptUrl = rptUrl + "&ReportName=" + reportName;
			rptUrl = rptUrl + "&SiteGuid=" + this.security.SiteGuid;
			rptUrl = rptUrl + "&TransID=" + transactionIdList;
			rptUrl += "&" + this.security.CSRFTokenWithParamName;

			string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" +
													 "window.open('" + rptUrl + "', " +
													 "'Reports', " +
													 "'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=800, width=1000'" +
													 "); \n" +
													 "-->\n</script>";

			this.Response.Cookies.Add(new HttpCookie("Token", this.Session["Token"] as String));
			this.ClientScript.RegisterStartupScript(this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
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

			return FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));
		}

		protected void OnSelectAll(object sender, EventArgs e)
		{
			for (int index = 0; index < this.TransactionDataGrid.Items.Count; ++index)
			{
				try
				{
					var checkBox = (CheckBox)this.TransactionDataGrid.Items[index].Cells[1].FindControl("MultipleSelectCheckbox");
					checkBox.Checked = true;
				}
				catch
				{
					break;
				}
			}
		}

		protected void UnSelectAll(object sender, EventArgs e)
		{
			for (int index = 0; index < this.TransactionDataGrid.Items.Count; ++index)
			{
				try
				{
					var checkBox = (CheckBox)this.TransactionDataGrid.Items[index].Cells[1].FindControl("MultipleSelectCheckbox");
					checkBox.Checked = false;
				}
				catch
				{
					break;
				}
			}
		}

        protected void DateFilterTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
			SetDateFieldsAccessibility();
		}
    }
}
