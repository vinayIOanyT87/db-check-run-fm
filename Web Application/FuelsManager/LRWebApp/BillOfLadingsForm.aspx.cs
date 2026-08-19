// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillOfLadingsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMAccountingWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
    using FMCore;
	using FMWebApp;

	using FuelsManager.FMWebApp;

	using TransactionFields;

	/// <summary>
	///     Summary description for BillOfLadingsForm.
	/// </summary>
	public partial class BillOfLadingsForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected Label BillToLabel;

		protected SiteClass CurrentSite;

		protected Label ManagerLabel;

		protected Label OwnerLabel;

		protected Label ShipToLabel;

		protected Label ShipperLabel;

		protected Label Status;

		protected string selectThisItemText = string.Empty;

		#endregion

		#region Public Properties

		public string SelectThisItemText
		{
			get
			{
				return this.selectThisItemText;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                // Depends Upon Load Rack
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING))
			{
				return null;
			}

			// CSI 3403 - Don't allow summary view if user does not have VIEW_LOAD_RACK_DATA security right.
			// CSI 4025 - Include VIEW_BILLS_OF_LADING
			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_BILLS_OF_LADING, 
						RootMenuName = "Operations", 
						CategoryName = "Load Rack", 
						ItemName = "Bills Of Lading", 
						NavigateUrl = "..\\LRWebApp\\BillOfLadingsForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		protected void BOLNumberTextBox_TextChanged(object sender, EventArgs e)
		{
			var bolContext = new BOLContext(this.CurrentSite);

			bolContext.BOL = this.BOLNumberTextBox.Text.Trim();

			if (this.Page.Request.GetQueryOrFormValue("Select") != null)
			{
				this.Session["CombineTransactionContext"] = bolContext;
			}
			else
			{
				this.Session["BOLSummaryContext"] = bolContext;
			}

			this.LoadPersistedFilters();
		}

		protected void DestinationSerialNumber1_TextChanged(object sender, EventArgs e)
		{
			this.PersistFilterCriteria();
		}

		protected void DestinationSerialNumber2_TextChanged(object sender, EventArgs e)
		{
			this.PersistFilterCriteria();
		}

		protected void DestinationSerialNumber3_TextChanged(object sender, EventArgs e)
		{
			this.PersistFilterCriteria();
		}

		protected void DisplayErrorDialog(string errorMessage)
		{
			string errMsg = "An Error has occurred!";
			

			if ((errorMessage != null) && (errorMessage.Length > 0))
			{
				errMsg = errorMessage;
			}

			string AlertString = "<script type='text/javascript'>\r\n<!--\r\nalert(\""
			                     + HttpUtility.JavaScriptStringEncode(errMsg) + "\");\r\n-->\r\n</script>";
			this.ClientScript.RegisterStartupScript(this.GetType(), "POPUP_ERROR_DIALOG", AlertString, false);
		}

		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
			this.GetSecurity();

			this.BillOfLadingsDataGrid.EditColumn = true;
			this.BillOfLadingsDataGrid.SelectColumn = false;
			this.BillOfLadingsDataGrid.PrintColumn = true;
			this.BillOfLadingsDataGrid.SecurityObject = base.Security;
			this.BillOfLadingsDataGrid.AssociatedDropdown = this.BOLFormPageSizeDropDown;
			this.BillOfLadingsDataGrid.Width = new Unit(1200, UnitType.Pixel);
			BillOfLadingsDataGrid.EditColumn = (Page.Request.GetQueryOrFormValue("Select") != null) ? false : true;
			BillOfLadingsDataGrid.SelectColumn = (Page.Request.GetQueryOrFormValue("Select") != null) ? true : false;
			BillOfLadingsDataGrid.PrintColumn = (Page.Request.GetQueryOrFormValue("Select") != null) ? false : true;
			PrintSelection.Visible = (Page.Request.GetQueryOrFormValue("Select") != null) ? false : true;
			SELECTALL.Visible = (Page.Request.GetQueryOrFormValue("Select") != null) ? false : true;
			DESELECTALL.Visible = (Page.Request.GetQueryOrFormValue("Select") != null) ? false : true;
			ucFMMenuBar.Visible = (Page.Request.GetQueryOrFormValue("Select") != null) ? false : true;

			if (Page.IsPostBack == true)
			{
				string productName = null;
				Guid typeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.BOL_SUMMARY);
				this.BillOfLadingsDataGrid.InitializeGridColumns(LISTVIEW_TYPE.STANDARD, typeGuid, productName);
			}

			if (Session["BillOfLadings.DataSet"] != null)
			{
				this.BillOfLadingsDataGrid.DataSet = (DataSet)Session["BillOfLadings.DataSet"];
			}
		}

		protected void OnPrintSelected_Click(object sender, EventArgs e)
		{
			TransactionDO trans = null;
			TransactionContext transContext;
			var selectedTransactionsAndAliases = new Dictionary<string, string>();

			AccountingSite accountingSiteLocal = null;

			// Create session object for TransactionDetail list of transactions.
			var detailList = new TransactionDetailList();
			var view = new DataView(((DataSet)this.Session["BillOfLadings.DataSet"]).Tables[0]);

			// Ensure this view instance is using the same sort as the grid.
			if ((this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_EXPRESSION] != null)
			    && (this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_DIRECTION] != null))
			{
				view.Sort = (string)this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_EXPRESSION] + " "
				            + (string)this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_DIRECTION];
			}

			// Put each transaction ID into the list for Previous/Next buttons.
			foreach (DataRowView row in view)
			{
				var transID = row[0] as string;
				detailList.TransactionIDList.Add(transID);
			}

			for (int Index = 0; Index < this.BillOfLadingsDataGrid.Items.Count; ++Index)
			{
				try
				{
					var fmButton = (CheckBox)this.BillOfLadingsDataGrid.Items[Index].Cells[4].FindControl("MultipleSelectCheckbox");
					if (fmButton.Checked)
					{
						detailList.CurrentIndex = Index
						                          + (this.BillOfLadingsDataGrid.CurrentPageIndex * this.BillOfLadingsDataGrid.PageSize);
						string TransID = detailList.TransactionIDList[detailList.CurrentIndex];

						trans = this.LoadTransaction(TransID);
						selectedTransactionsAndAliases.Add(trans.TransID, trans.Alias);
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
			string reportAlias = string.Empty;
			string transactionIdList = string.Empty;
			bool allSameAlias = true;
			if (selectedTransactionsAndAliases.Count == 0)
			{
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
				// No alias name found.  Should not happen.
				return;
			}

			if (allSameAlias == false)
			{
				// Register script to alert user that they can't do this.
				var messageBuilder = new StringBuilder();

				// messageBuilder;
				return;
			}

			// reportAlias has our one and only involved alias.
			// get the associated report for this order
			accountingSiteLocal =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x => x.LoadSiteInfo(this.Security, this.Security.SiteGuid));

			bool useDataDictionary = false;
			if (this.Session["UseDataDictionary"] != null)
			{
				useDataDictionary = (bool)this.Session["UseDataDictionary"];
			}

			transContext = new TransactionContext(base.Security, accountingSiteLocal, reportAlias, TransactionContext.Mode.Edit, useDataDictionary);

			transContext.GetTransactionContext();

			string bolRptType = ((int)ReportTypesClass.ReportTypes.BOL_RPT).ToString();
			string stRptName = transContext.aliasClass.AssociatedReport;

			// string rptURL = "../FMReporting/ReportLandingPage.aspx?ReportType=" + bolRptType;
			string rptURL = "../FMReportWebMain/ReportLandingPage.aspx?ReportType=" + bolRptType;
			string reportName = HttpUtility.HtmlEncode(stRptName);//.Replace(" ", "+"));
			rptURL = rptURL + "&ReportName=" + reportName;
			rptURL = rptURL + "&SiteGuid=" + base.Security.SiteGuid.ToString();
			rptURL = rptURL + "&TransID=" + transactionIdList;
			rptURL += "&" + this.Security.CSRFTokenWithParamName;

			string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" + "window.open('" + rptURL + "', "
			                               + "'Reports', "
			                               + "'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=950, width=850'"
			                               + "); \n" + "-->\n</script>";

			this.Response.Cookies.Add(new HttpCookie("Token", this.Session["Token"] as String));
			this.ClientScript.RegisterStartupScript(this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
		}

		protected void OnSelectAll(object sender, EventArgs e)
		{
			for (int Index = 0; Index < this.BillOfLadingsDataGrid.Items.Count; ++Index)
			{
				try
				{
					var fmButton = (CheckBox)this.BillOfLadingsDataGrid.Items[Index].Cells[4].FindControl("MultipleSelectCheckbox");
					if (fmButton.Checked != true)
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

		/// <summary>
		/// This method handles the page size dropdown selection change. It will
		///     update the grid with the data set and then update the grid view.
		/// </summary>
		/// <param name="source">
		/// </param>
		/// <param name="e">
		/// </param>
		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.BillOfLadingsDataGrid.DataSet = (DataSet)this.Session["BillOfLadings.DataSet"];
			this.BillOfLadingsDataGrid.UpdateView();
		}

		/// <summary>
		/// This event handles the loading of the page.
		/// </summary>
		/// <param name="sender">
		/// </param>
		/// <param name="e">
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																				this.Security, 
																				this.Security.SiteGuid, 
																				getMemberSites: true, 
																				getSchedulesAndProcessVariables: true, 
																				bGetAssociatedAliases: true)
																	);
				this.selectThisItemText = this.GetTranslatedText("Select This Item");

				// this control is hidden on the web app. this is necessary to control the auto populate capability
				this.InhibitAutoLoadTextBox.Text = this.CurrentSite.InhibitBOLSummaryAutoPopulate ? "true" : "false";

				if (this.Page.IsPostBack == false)
				{
					if (this.Page.Request.GetQueryOrFormValue("Select") != null)
					{
						var trans = this.Session["TransactionDetailTransaction"] as TransactionDO;
						if (trans == null)
						{
							throw new Exception("No Transaction in Session");
						}

						var context = new BOLContext(this.CurrentSite);
						context.Beginning = trans.InventoryDate;
						context.Ending = trans.InventoryDate.AddDays(1);
						context.Manager = trans.ManagerID;
						context.Owner = trans.OwnerID;
						context.Shipper = trans.ShipperID;
						context.BillTo = trans.BillToID;
						context.ShipTo = trans.ShipToID;
						context.carrier = trans.CarrierID;
						context.BOL = string.Empty;
						context.Product = "{All}";
						context.LocationID = "{All}";

						this.Session["CombineTransactionContext"] = context;
					}

					// Load the dropdown boxes
					this.LoadDropDownBoxes();
					this.SetFilterLabels();
					this.LoadPersistedFilters();
					this.RefreshButton_Command(null, null);
				}

				// Put user code to initialize the page here
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SetFilterLabels()
        {
			if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
			{
				if (this.Page.Session["SiteGuid"] != null)
				{
					var SiteGuid = (Guid)this.Page.Session["SiteGuid"];
					//load from db
					var localSiteDictionary = FMChannelHelper.MakeCall<IDataDictionariesClass, DataDictionaryCollectionClass>(x => x.EnumerateCached(SiteGuid));
					DestinationSerialNumber1Label.Text = GetDataDictionaryValue( localSiteDictionary,ListViewFieldClass.DESTINATION_SERIAL_NUMBER_1_DISPLAY, "Dest Serial #1");
					DestinationSerialNumber2Label.Text = GetDataDictionaryValue(localSiteDictionary, ListViewFieldClass.DESTINATION_SERIAL_NUMBER_2_DISPLAY, "Dest Serial #2");
					DestinationSerialNumber3Label.Text = GetDataDictionaryValue(localSiteDictionary, ListViewFieldClass.DESTINATION_SERIAL_NUMBER_3_DISPLAY, "Dest Serial #3");
				}
			}
		}

		private string GetDataDictionaryValue(DataDictionaryCollectionClass dic, string key, string defaultString)
        {
			if (dic.Contains(key))
			{
				return dic[key];
			}
			return defaultString;
		}

		protected void UnSelectAll(object sender, EventArgs e)
		{
			for (int Index = 0; Index < this.BillOfLadingsDataGrid.Items.Count; ++Index)
			{
				try
				{
					var fmButton = (CheckBox)this.BillOfLadingsDataGrid.Items[Index].Cells[4].FindControl("MultipleSelectCheckbox");
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

		/// <summary>
		/// This method will handle the edit icon event for the grid. This method is not implemented
		///     in the FMGrid base class, but must integrate with it.
		/// </summary>
		/// <param name="source">
		/// </param>
		/// <param name="e">
		/// </param>
		private void BillOfLadingsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			// Create session object for TransactionDetail list of transactions.
			var detailList = new TransactionDetailList();

			// First, sort the data set as it was displayed, so that the Previous/Next buttons operate correctly.
			var view = new DataView(((DataSet)this.Session["BillOfLadings.DataSet"]).Tables[0]);

			// Ensure this view instance is using the same sort as the grid.
			if ((this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_EXPRESSION] != null)
			    && (this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_DIRECTION] != null))
			{
				view.Sort = (string)this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_EXPRESSION] + " "
				            + (string)this.Page.Session[this.BillOfLadingsDataGrid.ID + FMGrid.SORT_DIRECTION];
			}

			// Put each transaction ID into the list for Previous/Next buttons.
			foreach (DataRowView row in view)
			{
				var transID = row[0] as string;
				detailList.TransactionIDList.Add(transID);
			}

			// Indicate which transaction id in the list is the one to initially display.
			detailList.CurrentIndex = e.Item.DataSetIndex;

			// Indicate the return URL for when the TransactionDetail Close button is clicked.
			detailList.ReturnURL = "..\\LRWebapp\\BillOfLadingsForm.aspx";

			// Put the object into session and transfer to the TransactionDetail.
			this.Session["BillOfLadingsPage.CurrentPageIndex"] = this.BillOfLadingsDataGrid.CurrentPageIndex;
			this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

			// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
			string transactionDetailURL;
			transactionDetailURL = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
			this.Redirect("../" + transactionDetailURL);
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.BillOfLadingsDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.BillOfLadingsDataGrid_EditCommand);
			this.BillOfLadingsDataGrid.SortCommand +=
				new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.BillOfLadingsDataGrid.DataGrid_SortCommand);
			this.BillOfLadingsDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.BillOfLadingsDataGrid.DataGrid_ItemDataBound);
			this.BillOfLadingsDataGrid.ItemCreated +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.BillOfLadingsDataGrid.DataGrid_ItemCreated);
			this.BillOfLadingsDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.BillOfLadingsDataGrid.DataGrid_PageIndexChanged);
			this.RefreshButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.RefreshButton_Command);
		}

		private void LoadDropDownBoxes()
		{
			var SR = new OrderListSR();

			SR.SubRequest = OrderListSR.RequestTypes.GET_HEADER_DATA;
			SR.Security = this.Security;
			SR.AllText = "{All}";

			// Process the service request
			OrderListDO orderListDO = FMChannelHelper.MakeCall<IOrderListProcessor, OrderListDO>(proc => proc.Process(SR));

			// Bind control data
			this.ProductDropDown.DataSource = orderListDO.ProductList;
			this.ProductDropDown.DataBind();

			this.LocationIDDropDown.DataSource = orderListDO.LocationList;
			this.LocationIDDropDown.DataBind();

			var Item = new ListItem("{All}", string.Empty);
			this.StatusDropDownList.Items.Add(Item);
			int NumberOfStatuses = Enum.GetValues(typeof(TransactionStatus)).Length;

			for (int nLoop = 0; nLoop < NumberOfStatuses; nLoop++)
			{
				string Value = Enum.GetName(typeof(TransactionStatus), nLoop);
				Item = new ListItem(Value, nLoop.ToString());
				this.StatusDropDownList.Items.Add(Item);
			}
		}

		private void LoadPersistedFilters()
		{
			BOLContext bolContext;
			if (this.Page.Request.GetQueryOrFormValue("Select") != null)
			{
				bolContext = this.Session["CombineTransactionContext"] as BOLContext;
			}
			else
			{
				bolContext = this.Session["BOLSummaryContext"] as BOLContext;
			}

			if (bolContext == null)
			{
				bolContext = new BOLContext(this.CurrentSite);
			}

			this.StatusDropDownList.SelectedValue = bolContext.Status;
			this.BOLNumberTextBox.Text = bolContext.BOL;
			this.BeginningDate.Text = bolContext.Beginning.ToString(this.CurrentSite.GetDateTimeFormatInfo());
			this.EndingDate.Text = bolContext.Ending.ToString(this.CurrentSite.GetDateTimeFormatInfo());

			this.ManagerTextBox.Text = bolContext.Manager;
			this.OwnerTextBox.Text = bolContext.Owner;
			this.ShipperTextBox.Text = bolContext.Shipper;
			this.BillToTextBox.Text = bolContext.BillTo;
			this.ShipToTextBox.Text = bolContext.ShipTo;
			this.CarrierTextBox.Text = bolContext.carrier;
			this.LocationIDDropDown.SelectedValue = bolContext.LocationID;
			this.ProductDropDown.SelectedValue = bolContext.Product;
			this.DestinationSerialNumber1TextBox.Text = bolContext.DestinationSerialNumber1;
			this.DestinationSerialNumber2TextBox.Text = bolContext.DestinationSerialNumber2;
			this.DestinationSerialNumber3TextBox.Text = bolContext.DestinationSerialNumber3;
		}

		private TransactionDO LoadTransaction(string transID)
		{
			AccountingSite accountingSiteLocal =
				FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
					x => x.LoadSiteInfo(this.Security, this.Security.SiteGuid));

			var sr = new TransactionSR
				{
					Security = this.Security, 
					TransID = transID, 
					AccountingSite = accountingSiteLocal
				};

			TransactionDO translocal = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(proc => proc.Process(sr));
			return translocal;
		}

		private void PersistFilterCriteria()
		{
			var Context = new BOLContext(this.CurrentSite);

			Context.Status = this.StatusDropDownList.SelectedValue;
			Context.Beginning = DateTimeOffset.Parse(this.BeginningDate.Text, this.CurrentSite.GetDateTimeFormatInfo());
			Context.Ending = DateTimeOffset.Parse(this.EndingDate.Text, this.CurrentSite.GetDateTimeFormatInfo());

			Context.Manager = this.ManagerTextBox.Text;
			Context.Owner = this.OwnerTextBox.Text;
			Context.Shipper = this.ShipperTextBox.Text;
			Context.BillTo = this.BillToTextBox.Text;
			Context.ShipTo = this.ShipToTextBox.Text;
			Context.carrier = this.CarrierTextBox.Text;
			Context.BOL = this.BOLNumberTextBox.Text.Trim();
			Context.Product = this.ProductDropDown.SelectedValue;
			Context.LocationID = this.LocationIDDropDown.SelectedValue;
			Context.DestinationSerialNumber1 = this.DestinationSerialNumber1TextBox.Text.Trim();
			Context.DestinationSerialNumber2 = this.DestinationSerialNumber2TextBox.Text.Trim();
			Context.DestinationSerialNumber3 = this.DestinationSerialNumber3TextBox.Text.Trim();

			if (this.Page.Request.GetQueryOrFormValue("Select") != null)
			{
				this.Session["CombineTransactionContext"] = Context;
			}
			else
			{
				this.Session["BOLSummaryContext"] = Context;
			}
		}

		/// <summary>
		/// This method will handle the refresh button event. It will retrieve the filter information and the
		///     retrieve the assoicated transaction data. It is also called by the Page Load method.
		/// </summary>
		/// <param name="sender">
		/// </param>
		/// <param name="e">
		/// </param>
		private void RefreshButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				// Validate user entered dates. This fixes CSI #4679. (IGO 22-Aug-2007)
				if (this.ValidateDates())
				{
					string[] statusString = Enum.GetNames(typeof(TransactionStatus));

					if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
					{
						if (this.Page.Session["SiteGuid"] != null)
						{
							var SiteGuid = (Guid)this.Page.Session["SiteGuid"];

							int Index = 0;
							FMChannelHelper.MakeCall<IDataDictionariesClass>(
								dict =>
									{
										foreach (string status in statusString)
										{
											statusString[Index++] = dict.Get(SiteGuid, status);
										}
									});
						}
					}

					this.PersistFilterCriteria();

					GetTransactionDO getTransactionDO = null;
					var getTransactionSR = new GetTransactionSR();

					getTransactionSR.Security = this.Security;
					getTransactionSR.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
					getTransactionSR.AliasName = string.Empty;

					if (this.BOLNumberTextBox.Text == string.Empty)
					{
						getTransactionSR.Status = this.StatusDropDownList.SelectedValue;
						getTransactionSR.BeginningDate = DateTimeOffset.Parse(
							this.BeginningDate.Text, this.CurrentSite.GetDateTimeFormatInfo());
						getTransactionSR.EndingDate = DateTimeOffset.Parse(this.EndingDate.Text, this.CurrentSite.GetDateTimeFormatInfo());
						getTransactionSR.ManagerID = this.ManagerTextBox.Text;
						getTransactionSR.Product = this.ProductDropDown.SelectedValue;
						getTransactionSR.LocationID = this.LocationIDDropDown.SelectedValue;

						if (getTransactionSR.ManagerID == "{All}")
						{
							getTransactionSR.ManagerID = string.Empty;
						}

						getTransactionSR.OwnerID = this.OwnerTextBox.Text;

						if (getTransactionSR.OwnerID == "{All}")
						{
							getTransactionSR.OwnerID = string.Empty;
						}

						getTransactionSR.ShipperID = this.ShipperTextBox.Text;

						if (getTransactionSR.ShipperID == "{All}")
						{
							getTransactionSR.ShipperID = string.Empty;
						}

						getTransactionSR.BillToID = this.BillToTextBox.Text;

						if (getTransactionSR.BillToID == "{All}")
						{
							getTransactionSR.BillToID = string.Empty;
						}

						getTransactionSR.ShipToID = this.ShipToTextBox.Text;

						if (getTransactionSR.ShipToID == "{All}")
						{
							getTransactionSR.ShipToID = string.Empty;
						}

						getTransactionSR.CarrierID = this.CarrierTextBox.Text;

						if (getTransactionSR.CarrierID == "{All}")
						{
							getTransactionSR.CarrierID = string.Empty;
						}

						if (getTransactionSR.Product == "{All}")
						{
							getTransactionSR.Product = string.Empty;
						}

						if (getTransactionSR.LocationID == "{All}")
						{
							getTransactionSR.LocationID = string.Empty;
						}

						getTransactionSR.DestinationSerialNumber1 = this.DestinationSerialNumber1TextBox.Text.Trim();
						getTransactionSR.DestinationSerialNumber2 = this.DestinationSerialNumber2TextBox.Text.Trim();
						getTransactionSR.DestinationSerialNumber3 = this.DestinationSerialNumber3TextBox.Text.Trim();
						getTransactionSR.Request = GetTransactionRequest.SITE_TYPEID_ALIAS_TRANSDATE_COMPANIES;
					}
					else
					{
						getTransactionSR.Request = GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER;
						getTransactionSR.DocumentNumber = this.BOLNumberTextBox.Text.Trim();
						getTransactionSR.Status = string.Empty;
					}

					getTransactionDO =
						FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(proc => proc.Process(getTransactionSR));

					getTransactionDO.TransactionDataSet.Tables[0].Columns["LookupTransactionStatusIndex"].ColumnName =
						"TransactionStatusInt";
					getTransactionDO.TransactionDataSet.Tables[0].Columns.Add("LookupTransactionStatusIndex");
					getTransactionDO.TransactionDataSet.Tables[0].Columns["LookupTransactionStatusIndex"].DataType =
						Type.GetType("System.String");

					foreach (DataRow Row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
					{
						Row["LookupTransactionStatusIndex"] = statusString[(int)Row["TransactionStatusInt"]];
					}

					if (this.Page.Request.GetQueryOrFormValue("Select") != null)
					{
						var trans = this.Session["TransactionDetailTransaction"] as TransactionDO;
						if (trans == null)
						{
							throw new Exception("No Transaction in Session");
						}

						for (int i = 0; i < getTransactionDO.TransactionDataSet.Tables[0].Rows.Count; i++)
						{
							DataRow tempRow = getTransactionDO.TransactionDataSet.Tables[0].Rows[i];

							var reversalType = tempRow["ReversalType"] as string;
							var transactionStatus = (int)tempRow["TransactionStatusInt"];

							if (tempRow["TransID"] as string == trans.TransID
								|| reversalType == "R"
								|| reversalType == "RU"
								|| reversalType == "O"
								|| reversalType == "UO"
								|| transactionStatus == (int)TransactionStatus.InProgress)
							{
								tempRow.Delete();
							}
						}

						getTransactionDO.TransactionDataSet.Tables[0].AcceptChanges();
					}

					this.BillOfLadingsDataGrid.CurrentPageIndex = 0;

					if (this.Session["BillOfLadingsPage.CurrentPageIndex"] != null)
					{
						this.BillOfLadingsDataGrid.CurrentPageIndex = (int)this.Session["BillOfLadingsPage.CurrentPageIndex"];
						this.Session.Remove("BillOfLadingsPage");
					}

					string productName = null;
					Guid typeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.BOL_SUMMARY);
					this.BillOfLadingsDataGrid.InitializeGridColumns(LISTVIEW_TYPE.STANDARD, typeGuid, productName);


					// Show destination serial number search boxes if columns are added to grid view
					DataGridColumnCollection cols = BillOfLadingsDataGrid.Columns;
					foreach (DataGridColumn col in cols)
					{
						if (col.HeaderText.Equals(ListViewFieldClass.DESTINATION_SERIAL_NUMBER_1_DISPLAY))
						{
							DestinationSerialNumber1.Style.Remove("display");
							continue;
						}
						if (col.HeaderText.Equals(ListViewFieldClass.DESTINATION_SERIAL_NUMBER_2_DISPLAY))
						{
							DestinationSerialNumber2.Style.Remove("display");
							continue;
						}
						if (col.HeaderText.Equals(ListViewFieldClass.DESTINATION_SERIAL_NUMBER_3_DISPLAY))
						{
							DestinationSerialNumber3.Style.Remove("display");
							continue;
						}
					}
				
					this.Session["BillOfLadings.DataSet"] = getTransactionDO.TransactionDataSet;
					this.BillOfLadingsDataGrid.DataSet = getTransactionDO.TransactionDataSet;
					this.BillOfLadingsDataGrid.UpdateView();

					this.LoadPersistedFilters();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private bool ValidateDates()
		{
			bool breturn = false;

			try
			{
				// Check for empty dates
				if (0 == this.BeginningDate.Text.Length || 0 == this.EndingDate.Text.Length)
				{
					this.DisplayErrorDialog("Cannot have empty date(s).");
				}
				else
				{
					// Gather and Validate Begin and End dates
					DateTimeOffset begindate;
					DateTimeOffset enddate;

					begindate = DateTimeOffset.Parse(this.BeginningDate.Text, this.CurrentSite.GetDateTimeFormatInfo());
					enddate = DateTimeOffset.Parse(this.EndingDate.Text, this.CurrentSite.GetDateTimeFormatInfo());
					if (begindate > enddate)
					{
						this.DisplayErrorDialog("Beginning date greater than ending date.");
					}
					else
					{
						breturn = true;
					}
				}
			}
			catch (FormatException fe)
			{
				this.DisplayErrorDialog(fe.Message);
			}

			return breturn;
		}

		#endregion
	}

	[Serializable]
	public class BOLContext
	{
		#region Constants and Fields

		public string BOL;

		public DateTimeOffset Beginning;

		public string BillTo;

		public DateTimeOffset Ending;

		public string LocationID;

		public string Manager;

		public string Owner;

		public string Product;

		public string ShipTo;

		public string Shipper;

		public string Status;

		public string carrier;

		public string DestinationSerialNumber1;

		public string DestinationSerialNumber2;

		public string DestinationSerialNumber3;

		#endregion

		#region Constructors and Destructors

		public BOLContext(SiteClass Site)
		{
			DateTimeOffset Today = TimeConverter.Today(Site);

			this.Status = string.Empty;
			this.Beginning = Today;
			this.Ending = Today.AddDays(1);
			this.Manager = "{All}";
			this.Owner = "{All}";
			this.Shipper = "{All}";
			this.BillTo = "{All}";
			this.ShipTo = "{All}";
			this.carrier = "{All}";
			this.BOL = string.Empty;
			this.Product = "{All}";
			this.LocationID = "{All}";
			this.DestinationSerialNumber1 = string.Empty;
			this.DestinationSerialNumber2 = string.Empty;
			this.DestinationSerialNumber3 = string.Empty;
		}

		#endregion
	}
}