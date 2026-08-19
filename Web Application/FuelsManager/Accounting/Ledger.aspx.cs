// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ledger.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Ledger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;
	using System.Diagnostics;

	public partial class Ledger : AccountingWebFormView
	{
		#region Constants and Fields
		protected Image Image1;
		protected Label Label1;
		protected AccountingSite AccountingSite;
		protected ListViewDataSet Grid;

		private bool singleOwnerSystem;
		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the month selection event and set the
		///    selected item in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MonthSelectionChange(object sender, EventArgs e)
		{
			string selection = this.monthDropdown.SelectedValue;
			this.Session.Add(PageSessionKeyConstants.LEDGER_MONTH_SELECTION, selection);
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			this.CurrentSiteGuid = Guids.SiteAdminGuid;
			this.Initialize();
			base.OnInit(e);
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				if (this.Session[PageSessionKeyConstants.LEDGER_VIEW_COLLECTION] != null)
				{
					var listViewCollection = (ListViewCollectionClass)this.Session[PageSessionKeyConstants.LEDGER_VIEW_COLLECTION];

					if (listViewCollection != null)
					{
						this.BindViewDropDown(listViewCollection);

						if (this.Session[PageSessionKeyConstants.LEDGER_VIEW_SELECTION] != null)
						{
							var selectedGuid = (Guid)this.Session[PageSessionKeyConstants.LEDGER_VIEW_SELECTION];

							if (selectedGuid != Guid.Empty)
							{
								this.ViewDropDownList.SelectedValue = selectedGuid.ToString();
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method is the main entry point for the Ledger page. It will organize
		///    all the retrieving of data and binding.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// Remove from session as it effects ProductSelectForm behavior
				this.Session.Remove("TransactionDetailTransaction");

				// In accounting site, we do not need to get user company data.
				// Therefore, set the object to not retrieve it.
				this.AccountingSite =
					FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
						x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

				this.AccountingSite.GetUserCompanies = false;

				// If the session is null, then remove all objects from the 
				// session and display the accounting error page.
				if (this.Session["Security"] == null)
				{
					this.Session.RemoveAll();
					this.DisplayErrorPage();
					return;
				}

				// Determines if the system is a single or multiple owner system.
				this.SetSystemType();

				// Only initialize page if the request is not a post back request.
				if (this.Page.IsPostBack == false)
				{
					this.MovementButton.Visible = false;
					if (this.IsBsme && this.security.HasRight(RIGHT.VIEW_MOVEMENT, false))
					{
						SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
											x => x.Get(this.security, this.security.SiteGuid, false, false, false));

						if (site != null && site.SiteGroup == false)
						{
							this.MovementButton.Visible = true;
						    this.Session.Remove("MovementCallStack");
							var callStack = new Stack();
						    this.Session["MovementCallStack"] = callStack;
							callStack.Push(this.Request.RawUrl);
						}
					}

					var displayDateTypeControls = this.IsBsme && this.security.HasRight(RIGHT.ACCESS_RECONCILIATION_VIEWS, false);
					this.DisplayDateTypeControls(displayDateTypeControls);

					// Set initial GrossNetFlag setting
					if (this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] == null)
					{
						this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] = this.AccountingSite.CurrentSite.QuantityDisplayDefault;
					}

					if (this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION] == null)
					{
						const bool CheckedFlag = false;
						this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION] = CheckedFlag;
					}

					this.ApplyDataDictionary();
					this.RetrieveHeaderInfo();
					LedgerDO ledgerDO = this.RetrieveLedgerData();
					this.BuildListView(this.ledgerDataGrid, ledgerDO);
				}
				else if (this.Request.GetQueryOrFormValue("__EVENTTARGET") != null
						&& (this.Request.GetQueryOrFormValue("__EVENTTARGET").Contains("ucFMMenuBar$ibtnAddQuickLink")
						|| this.Request.GetQueryOrFormValue("__EVENTTARGET").Contains("ucFMMenuBar$lnkAddFavorite")))
				{
					// If the user added the page as a Favorite or Quick Link, we still need to build the list view controls, even
					// though this is a postback
					LedgerDO ledgerDO = this.RetrieveLedgerData();
					this.BuildListView(this.ledgerDataGrid, ledgerDO);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will handle the Quantity selection event and set the
		///    selected item in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void QuantityDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.Session.Add(PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION, this.QuantityDropDownList.SelectedIndex);
		}

		/// <summary>
		/// This method will handle the Date Type selection event and set the 
		/// selected value in session.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
 		protected void DateTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedValue = this.DateTypeDropDownList.SelectedValue;
		    this.Session.Add(PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION, selectedValue);
		}

		/// <summary>
		///    This method receives the request to refresh the ledger from the
		///    refresh button bring pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RefreshButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (this.singleOwnerSystem == false)
				{
					// Update the session owner information.
					if (!string.IsNullOrEmpty(this.OwnerTextBox.Text))
					{
						this.Session.Add(PageSessionKeyConstants.LEDGER_OWNER_SELECTION, this.OwnerTextBox.Text);
					}
				}

				// Update the session manager information.
				if (!string.IsNullOrEmpty(this.ManagerTextBox.Text))
				{
					this.Session.Add(PageSessionKeyConstants.LEDGER_MANAGER_SELECTION, this.ManagerTextBox.Text);
				}

				// Update the session product information.
				if (!string.IsNullOrEmpty(this.ProductTextBox.Text))
				{
					this.Session.Add(PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION, this.ProductTextBox.Text);
				}

				this.Session.Add(PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION, this.FinanceCheckBox.Checked);

				LedgerDO ledgerDO = this.RetrieveLedgerData();
				this.BuildListView(this.ledgerDataGrid, ledgerDO);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method receives the request to redirect to movement calendar from the 
		/// movement button bring pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MovementButtonClick(object sender, EventArgs e)
		{
			string[] monthYear = this.monthDropdown.SelectedValue.Split(' ');
			string product = this.ProductTextBox.Text;

		    this.Session["Movement_Month"] = monthYear[0];
		    this.Session["Movement_Year"] = monthYear[1];
		    this.Session["Movement_Product"] = product;

			this.Redirect("../BSMEWebApp/BSMEMovementCalendar.aspx?TrackReturn=true");
		    this.Context.ApplicationInstance.CompleteRequest();
		}


		/// <summary>
		///    This method applies the data dictionary to the labels and buttons.
		/// </summary>
		private void ApplyDataDictionary()
		{
			var colon = new char[1];
			colon[0] = ':';

			string newText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																x =>
																	x.Get(this.security.SiteGuid, this.managerLabel.Text.Trim(colon))
																);

			this.managerLabel.Text = newText + ":";

			newText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.security.SiteGuid, this.productLabel.Text.Trim(colon))
																);

			this.productLabel.Text = newText + ":";

			newText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.security.SiteGuid, this.ownerLabel.Text.Trim(colon))
																);

			this.ownerLabel.Text = newText + ":";

			newText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.security.SiteGuid, this.monthLabel.Text.Trim(colon))
																);

			this.monthLabel.Text = newText + ":";

			newText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.security.SiteGuid, this.refreshButton.Text)
																);

			this.refreshButton.Text = newText;

			newText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.security.SiteGuid, this.FinanceCheckBox.Text)
																);
			this.FinanceCheckBox.Text = newText;
		}

		/// <summary>
		///    This method binds the header information (manager, owner, and product) to the
		///    controls.
		/// </summary>
		/// <param name="ledgerPageDO"></param>
		private void BindHeaderInfo(LedgerPageDO ledgerPageDO)
		{
			if (ledgerPageDO != null)
			{
				// CompaniesClass companies = new CompaniesClass();
				// var ownerList = companies.EnumerateIDByRole(this.security, COMPANY_ROLE.OWNER, false, null);
				// Must retrieve info for a multi-owner system.  Otherwise, the info
				// is not needed.
				if (this.singleOwnerSystem)
				{
					if (ledgerPageDO.OwnerList.Count != 0)
					{
						this.OwnerTextBox.Text = ledgerPageDO.OwnerList[0] as string;
						this.Session.Add(PageSessionKeyConstants.LEDGER_OWNER_SELECTION, ledgerPageDO.OwnerList[0] as string);
					}
				}
				else
				{
					// Bind the owner list and if an item was previously selected that was in the 
					// session, then select it. Else set the selection to the 1st item.
					if ((ledgerPageDO.OwnerList != null) && (ledgerPageDO.OwnerList.Count > 0))
					{
						if (this.Session[PageSessionKeyConstants.LEDGER_OWNER_SELECTION] == null)
						{
							this.OwnerTextBox.Text = ledgerPageDO.OwnerList[0] as string;
							this.Session.Add(PageSessionKeyConstants.LEDGER_OWNER_SELECTION, ledgerPageDO.OwnerList[0] as string);
						}
						else
						{
							var selection = this.Session[PageSessionKeyConstants.LEDGER_OWNER_SELECTION] as string;
							if (string.IsNullOrEmpty(selection))
							{
								this.OwnerTextBox.Text = ledgerPageDO.OwnerList[0] as string;
								this.Session.Add(PageSessionKeyConstants.LEDGER_OWNER_SELECTION, ledgerPageDO.OwnerList[0] as string);
							}
							else
							{
								bool found = false;
								this.OwnerTextBox.Text = this.Session[PageSessionKeyConstants.LEDGER_OWNER_SELECTION] as string;
								foreach (string owner in ledgerPageDO.OwnerList)
								{
									if (owner == this.OwnerTextBox.Text)
									{
										found = true;
										break;
									}
								}

								if (!found)
								{
									this.OwnerTextBox.Text = ledgerPageDO.OwnerList[0] as string;
									this.Session.Add(PageSessionKeyConstants.LEDGER_OWNER_SELECTION, ledgerPageDO.OwnerList[0] as string);
								}
							}
						}
					}
				}

				// Bind the manager list and if an item was previously selected that was in the 
				// session, then select it. Else set the selection to the 1st item.
				if ((ledgerPageDO.ManagerList != null) && (ledgerPageDO.ManagerList.Count > 0))
				{
					if (this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] == null)
					{
						this.ManagerTextBox.Text = ledgerPageDO.ManagerList[0] as string;
						this.Session.Add(PageSessionKeyConstants.LEDGER_MANAGER_SELECTION, ledgerPageDO.ManagerList[0] as string);
					}
					else
					{
						var selection = this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] as string;

						if (string.IsNullOrEmpty(selection))
						{
							this.ManagerTextBox.Text = ledgerPageDO.ManagerList[0] as string;
							this.Session.Add(PageSessionKeyConstants.LEDGER_MANAGER_SELECTION, ledgerPageDO.ManagerList[0] as string);
						}
						else
						{
							bool found = false;
							this.ManagerTextBox.Text = this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] as string;
							foreach (string manager in ledgerPageDO.ManagerList)
							{
								if (manager == this.ManagerTextBox.Text)
								{
									found = true;
									break;
								}
							}

							if (!found)
							{
								this.ManagerTextBox.Text = ledgerPageDO.ManagerList[0] as string;
								this.Session.Add(PageSessionKeyConstants.LEDGER_MANAGER_SELECTION, ledgerPageDO.ManagerList[0] as string);
							}
						}
					}
				}

				// Bind the product list and if an item was previously selected that was in the 
				// session, then select it. Else set the selection to the 1st item.
				if (ledgerPageDO.ProductList != null && ledgerPageDO.ProductList.Count == 0)
				{
					throw new ApplicationException("No products configured for accounting use.");
				}

				if ((ledgerPageDO.ProductList != null) && (ledgerPageDO.ProductList.Count > 0))
				{
					if (this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] == null)
					{
						this.ProductTextBox.Text = ledgerPageDO.ProductList[0] as string;
						this.Session.Add(PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION, ledgerPageDO.ProductList[0] as string);
					}
					else
					{
						var selection = this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string;

						if (string.IsNullOrEmpty(selection))
						{
							this.ProductTextBox.Text = ledgerPageDO.ProductList[0] as String;
							this.Session.Add(PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION, ledgerPageDO.ProductList[0] as string);
						}
						else
						{
							if (ledgerPageDO.ProductList.IndexOf(selection) >= 0)
							{
								this.ProductTextBox.Text = selection;
							}
							else
							{
								this.ProductTextBox.Text = ledgerPageDO.ProductList[0] as string;
								this.Session.Add(PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION, ledgerPageDO.ProductList[0] as string);
							}
						}
					}
				}
			}

			// Reset the QuantityDropDownList to the correct state.
			if (this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] != null)
			{
				var quantityDisplay = (QuantityDisplay)this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION];

				this.QuantityDropDownList.SelectedIndex = (int)quantityDisplay;
			}

			// Disable the Finance checkbox if the user does not have the appropriate rights.
			if (this.security.HasRight(RIGHT.MODIFY_STANDING_OFFERS)
				|| this.security.HasRight(RIGHT.VIEW_STANDING_OFFERS)
				|| this.security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA)
				|| this.security.HasRight(RIGHT.VIEW_FINANCIAL_DATA))
			{
				this.FinanceCheckBox.Enabled = true;

				// Reset the Finance check box.
				if (this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION] != null)
				{
					this.FinanceCheckBox.Checked = (bool)this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION];
				}
			}
			else
			{
				if (this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION] != null)
				{
					this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION] = false;
				}

				this.FinanceCheckBox.Checked = false;
				this.FinanceCheckBox.Enabled = false;
			}
		}

		/// <summary>
		///    This method will bind the month/year to the dropdown control and data
		///    dictionary the name of the months
		/// </summary>
		/// <param name="monthYearDO"></param>
		private void BindMonthYearList(MonthYearDO monthYearDO)
		{
			DropdownItemCollectionClass monthYearList = monthYearDO.CombinedList;

			this.monthDropdown.DataSource = monthYearList;
			this.monthDropdown.DataTextField = "Text";
			this.monthDropdown.DataValueField = "TextValue";
			this.monthDropdown.DataBind();

			// Find previously selected item that was in the 
			// session, then select it. Else set the selection to the 1st item.
			if (this.Session[PageSessionKeyConstants.LEDGER_MONTH_SELECTION] == null)
			{
				this.monthDropdown.SelectedIndex = 0;
				int monthSelectIndex = 0;
				int monthNow = DateTimeOffset.Now.Month;
				int yearNow = DateTimeOffset.Now.Year;
				var converter = new DateTimeOffsetConverter();

				foreach (DropdownItem itemx in monthYearList)
				{
					string monthAndYear = itemx.TextValue;
					var convertFrom = converter.ConvertFrom(monthAndYear);

					if (convertFrom != null)
					{
						var dt = (DateTimeOffset)convertFrom;

						if (dt.Month == monthNow && dt.Year == yearNow)
						{
							this.monthDropdown.SelectedIndex = monthSelectIndex;

							break;
						}
					}

					monthSelectIndex++;
				}
				DropdownItem item = monthYearList[this.monthDropdown.SelectedIndex];
				this.Session.Add(PageSessionKeyConstants.LEDGER_MONTH_SELECTION, item.TextValue);
			}
			else
			{
				var selection = this.Session[PageSessionKeyConstants.LEDGER_MONTH_SELECTION] as string;

				if (string.IsNullOrEmpty(selection))
				{
					this.monthDropdown.SelectedIndex = 0;
					DropdownItem item = monthYearList[0];
					this.Session.Add(PageSessionKeyConstants.LEDGER_MONTH_SELECTION, item.TextValue);
				}
				else
				{
					for (int index = 0; index < monthYearList.Count; index++)
					{
						DropdownItem item = monthYearList[index];

						if (selection.Equals(item.TextValue))
						{
							this.monthDropdown.SelectedIndex = index;
							this.Session.Add(PageSessionKeyConstants.LEDGER_MONTH_SELECTION, item.TextValue);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		///    Binds the list view collection to the View drop down list
		/// </summary>
		/// <param name="listViewCollection"></param>
		private void BindViewDropDown(ListViewCollectionClass listViewCollection)
		{
			ListItem selectedListItem = null;
			if (this.ViewDropDownList.SelectedIndex >= 0)
			{
				selectedListItem = this.ViewDropDownList.SelectedItem;
			}

			this.ViewDropDownList.Items.Clear();
			this.ViewDropDownList.SelectedIndex = -1;
			this.ViewDropDownList.SelectedValue = null;
			this.ViewDropDownList.ClearSelection();

			if (listViewCollection != null && listViewCollection.Count > 0)
			{
				this.ViewDropDownList.DataTextField = "ID";
				this.ViewDropDownList.DataValueField = "IdentityGuid";
				this.ViewDropDownList.DataSource = listViewCollection;
				this.ViewDropDownList.DataBind();
			}

			if (selectedListItem != null)
			{
				int index = this.ViewDropDownList.Items.IndexOf(selectedListItem);

				if (index >= 0)
				{
					this.ViewDropDownList.SelectedIndex = index;
				}
			}
		}

		/// <summary>
		///    Disables links for all columns if the product is not active and for columns
		///    corresponding to transactions with missing modify and view security rights.
		/// </summary>
		private void DisableLinksBasedOnSecurityRights(bool activeProduct)
		{
			ListViewColumnDO columnDO;

			for (int index = 0; (columnDO = this.Grid.listViewDO.getListViewColumn(index)) != null; ++index)
			{
				if (columnDO.IsAggregateField)
				{
					columnDO.IsLink = false;
					LedgerAggregateColumnClass agg = this.GetByColumnID(this.security, columnDO.ColumnName);
					LedgerAggregateColumnMapCollectionClass mapColl = this.EnumerateLedgerAggColumns(this.security, agg.IdentityGuid);

					for (int i = 0; i < mapColl.Count && !columnDO.IsLink; i++)
					{
						LedgerAggregateColumnMapClass m = mapColl[i];
						columnDO.IsLink = activeProduct && this.security.HasViewTransactionRightByAliasName(m.AliasName);
					}
				}
				else
				{
					columnDO.IsLink = activeProduct && this.security.HasViewTransactionRightByAliasName(columnDO.ColumnName);
				}
			}
		}

		private LedgerAggregateColumnMapCollectionClass EnumerateLedgerAggColumns(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<ILedgerAggregateColumnMaps, LedgerAggregateColumnMapCollectionClass>(
																	 x =>
																	 x.Enumerate(securityClass, guid)
																);
		}

		private LedgerAggregateColumnClass GetByColumnID(SecurityClass securityClass, string columnName)
		{
			return FMChannelHelper.MakeCall<ILedgerAggregateColumns, LedgerAggregateColumnClass>(
																	 x =>
																	 x.GetByColumnID(securityClass, columnName)
																);
		}

		/// <summary>
		/// Sets the visibility of the owner controls.
		/// </summary>
		/// <param name="visible"></param>
		private void DisplayOwnerControls(bool visible)
		{
			this.ownerLabel.Visible = visible;
			this.OwnerTextBox.Visible = visible;
		}

		/// <summary>
		/// Sets the visibility of the date type controls.
		/// </summary>
		/// <param name="visible"></param>
		private void DisplayDateTypeControls(bool visible)
		{
			foreach (var item in this.DateTypeDropDownList.Items)
			{
				var listItem = item as ListItem;
				if (listItem != null)
				{
					var displayText = BsmeLedgerDateType.GetDisplayText(listItem.Value);
					listItem.Text = displayText;
				}
			}

			// Set selected value to session value or default to "Inventory Date"
			string selectedValue;
			if (this.Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION] == null)
			{
				selectedValue = BsmeLedgerDateType.GetDisplayValue(BsmeLedgerDateType.DateProcessTypes.ByInventoryDate);
				this.Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION] = selectedValue;
			}
			else
			{
				selectedValue = this.Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION].ToString();
			}

			this.dateTypeDropDownLabel.Visible = visible;
			this.DateTypeDropDownList.Visible = visible;
			this.DateTypeDropDownList.SelectedValue = selectedValue;
		}

		private void FormatForRegionalSettings(LedgerDO ledgerDO)
		{
			foreach (LedgerLineItemDO item in ledgerDO.LedgerLineItems)
			{
				if (item.InventoryDate.ToUpper().StartsWith("TOT") == false)
				{
					item.InventoryDate = this.AccountingSite.FormatDate(item.DtInventoryDate);
				}
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CurrentSiteGuid = Guids.SiteAdminGuid;
		}

		private bool ProductExistsInMap(ProductClass product, ProductMapCollectionClass productMapCollection)
		{
			foreach (ProductMapClass productMap in productMapCollection)
			{
				if (product.ID == productMap.AssignedID)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///    This method will retrieve the header information and call the binding methods.
		/// </summary>
		private void RetrieveHeaderInfo()
		{
			// Get month/year data and bind to the control.
			var monthYearSR = new MonthYearSR
			{
				Security = this.security,
				UseDataDictionary = true
			};

			MonthYearDO monthYearDO = FMChannelHelper.MakeCall<IMonthYearProcessor, MonthYearDO>(x => x.Process(monthYearSR));

			this.BindMonthYearList(monthYearDO);
			Stopwatch timer = new Stopwatch();
			timer.Start();
			// Get the product, manager, and owner data, then bind to the control.
			var ledgerPageSR = new LedgerPageSR
			{
				Security = this.security,
				Site = this.AccountingSite.CurrentSiteName,
				CurrentSiteGuid = this.CurrentSiteGuid
			};

			LedgerPageDO ledgerPageDO = FMChannelHelper.MakeCall<ILedgerPageProcessor, LedgerPageDO>(
													x => x.Process(ledgerPageSR, this.AccountingSite));
			timer.Stop();
			this.BindHeaderInfo(ledgerPageDO);
		}

		/// <summary>
		///    This method will retrieve the ledger data using the selected manager, owner,
		///    product, and month/year data.
		/// </summary>
		/// <returns></returns>
		private LedgerDO RetrieveLedgerData()
		{
			var ledgerSR = new LedgerSR
			{
				Security = this.Session["Security"] as SecurityClass
			};

			if (this.security == null)
			{
				throw new ArgumentNullException(nameof(this.security));
			}

			ledgerSR.Site = this.AccountingSite.CurrentSiteName;
			ledgerSR.CurrentSiteGuid = this.CurrentSiteGuid;
			ledgerSR.SetRequestType(LedgerSR.LedgerRequests.Refresh);
			ledgerSR.Manager = this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] as string;
			ledgerSR.Owner = this.Session[PageSessionKeyConstants.LEDGER_OWNER_SELECTION] as string;
			ledgerSR.Product = this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string;
			ledgerSR.Month = this.Session[PageSessionKeyConstants.LEDGER_MONTH_SELECTION] as string;
			ledgerSR.Units = (QuantityDisplay)this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION];
			ledgerSR.ShowCost = (bool)this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION];
			ledgerSR.DateType = (BsmeLedgerDateType.DateProcessTypes)Convert.ToInt32(this.Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION]);

			// Get the ledger data
			LedgerDO ledgerDO = FMChannelHelper.MakeCall<ILedgerProcessor, LedgerDO>(x => x.Process(ledgerSR, this.AccountingSite));
			return ledgerDO;
		}

		/// <summary>
		///    This method determines if the system is a single owner system or a multi-owner system.
		///    It will return true if it is a single owner system. Otherwise, it will return false.
		/// </summary>
		private void SetSystemType()
		{
			this.singleOwnerSystem = this.AccountingSite.CurrentSite.EnforceSingleOwner;

			if (this.AccountingSite.CurrentSite.EnforceSingleOwner)
			{
				this.DisplayOwnerControls(false);
			}
			else
			{
				this.DisplayOwnerControls(true);
			}
		}

		private bool UserExistsInMap(UserClass user, GroupLedgerViewMapCollectionClass groupLedgerViewMapCollectionClass)
		{
			foreach (GroupLedgerViewMapClass groupMap in groupLedgerViewMapCollectionClass)
			{
				if (user.UserGroupMapCollection != null)
				{
					foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
					{
						if (groupMap.GroupGuid == userGroupMap.GroupGuid)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private bool ViewCompareList(ListViewCollectionClass datasourceCollection, ListViewCollectionClass listViewCollection)
		{
			if (datasourceCollection == null || listViewCollection == null)
			{
				return true;
			}

			if (datasourceCollection.Count != listViewCollection.Count)
			{
				return true;
			}

			for (int index = 0; index < listViewCollection.Count; ++index)
			{
				if (datasourceCollection[index].IdentityGuid != listViewCollection[index].IdentityGuid)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///    This method will bind the ledger grid with the list view object that
		///    contains the list and statuses of the ledger data.
		/// </summary>
		private void BuildListView(DataGrid dataGrid, LedgerDO ledgerDO)
		{
			// EnumerateLedgerAggColumns the ledger list views available
			ListViewCollectionClass listViewCollection =
				FMChannelHelper.MakeCall<IListViews, ListViewCollectionClass>(
					x =>
					x.EnumerateByTypeAndTypeGuid(
						this.security, LISTVIEW_TYPE.STANDARD, ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER)));

			ProductClass product =
				FMChannelHelper.MakeCall<IProducts, ProductClass>(
					x => x.GetByID(this.security, this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string));

			UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.security, this.security.UserGuid));

			// Filter based on settings
			for (int index = listViewCollection.Count - 1; index >= 0; --index)
			{
				ListViewClass listView = listViewCollection[index];

				// Filter the views based on the product selected in the ledger view
				if ( !this.ProductExistsInMap(product, listView.ProductMapCollection))
				{
					listViewCollection.RemoveByIdentityGuid(listView);
				}

				// Filter the views based on the user groups configured for the view
				if ( !this.UserExistsInMap(user, listView.GroupMapCollection))
				{
					listViewCollection.RemoveByIdentityGuid(listView);
				}
			}

			if (listViewCollection.Count == 0)
			{
				this.ViewDropDownList.ClearSelection();
				this.ViewDropDownList.Items.Clear();
				throw new ApplicationException("No appropriate ledger list views defined.");
			}

			bool discardLastSelection = this.ViewCompareList(
				(ListViewCollectionClass)this.ViewDropDownList.DataSource, listViewCollection);

			// Make note of the currently selected LedgerView.
			var selectedView = Guid.Empty;

			if (discardLastSelection)
			{
				selectedView = Guid.Empty;
			}
			else
			{
				// Moved this logic to be conditional on the list changing.  If the list changed, the SelectedValue may no longer exist so
				// don't try and retrieve it.
				if (this.ViewDropDownList.SelectedIndex != -1)
				{
					selectedView = Guid.Parse(this.ViewDropDownList.SelectedValue);
				}
			}

			this.BindViewDropDown(listViewCollection);

			this.Session[PageSessionKeyConstants.LEDGER_VIEW_COLLECTION] = listViewCollection;

			// If the current ledger view list is the same as before, reselect the previous item.
			if (!discardLastSelection)
			{
				Guid checkValue = selectedView;
				selectedView = Guid.Empty;

				foreach (ListViewClass listView in listViewCollection)
				{
					if (checkValue == listView.IdentityGuid)
					{
						selectedView = listView.IdentityGuid;
						break;
					}
				}
			}

			this.ViewDropDownList.Style["visibility"] = "visible";
			this.ViewSelection.Style["visibility"] = "hidden";
			this.ViewSelection.Text = "";

			// Use the last LedgerView that they selected.  On the first pass, if there was only a single LedgerView,
			// this would fall through to the else condition where we would have forced the selection of the only entry.
			if (selectedView != Guid.Empty)
			{
				this.ViewDropDownList.SelectedValue = selectedView.ToString();
				this.Session[PageSessionKeyConstants.LEDGER_VIEW_SELECTION] = selectedView;
			}
			else
			{
				// If there's only one LedgerView available, autoselect it.
				// and set the dropdown to be hidden.  Show a simple label in its
				// place to show the current Ledger View
				if (listViewCollection.Count == 1)
				{
					selectedView = listViewCollection[0].IdentityGuid;
					this.ViewDropDownList.SelectedValue = selectedView.ToString();
					this.Session[PageSessionKeyConstants.LEDGER_VIEW_SELECTION] = selectedView;

					this.ViewSelection.Text = listViewCollection[0].ID;

					this.ViewDropDownList.Style["visibility"] = "hidden";
					this.ViewSelection.Style["visibility"] = "visible";
				}
			}

			// Setup column view.	
			Guid ledgerViewGuid = Guid.Parse(this.ViewDropDownList.SelectedValue);
			this.Grid = new ListViewDataSet(this.security, ledgerViewGuid, this.AccountingSite);
			this.Grid.SetDataGrid(dataGrid);
			this.Grid.SetNavigateURL("..\\Accounting\\TransactionList.aspx");

			byte volumeDecimalPlaces;
			byte massDecimalPlaces;

			if (product.ProductType == ProductType.AdditiveProduct)
			{
				if (product.VolumeUnits == 0)
				{
					volumeDecimalPlaces = this.AccountingSite.CurrentSite._AdditiveVolumeDecimalPlaces;
				}
				else
				{
					volumeDecimalPlaces = product.VolumeDecimalPlaces;
				}
			}
			else
			{
				if (product.VolumeUnits == 0)
				{
					volumeDecimalPlaces = this.AccountingSite.CurrentSite._VolumeDecimalPlaces;
				}
				else
				{
					volumeDecimalPlaces = product.VolumeDecimalPlaces;
				}
			}

			if (product.MassUnits == 0)
			{
				massDecimalPlaces = this.AccountingSite.CurrentSite._MassDecimalPlaces;
			}
			else
			{
				massDecimalPlaces = product.MassDecimalPlaces;
			}

			// Determine which of the Gross/Net radio buttons are set (Gross,
			// Net, or Both).
			var quantityDisplay = QuantityDisplay.GROSS;
			if (this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] != null)
			{
				quantityDisplay = (QuantityDisplay)this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION];
			}

			if (this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION] != null)
			{
				this.Grid.ShowCost = (bool)this.Session[PageSessionKeyConstants.LEDGER_SHOW_COST_SELECTION];
			}

			this.FormatForRegionalSettings(ledgerDO);

			// Disable links if BSM-E version and product is not active
			bool activeProduct = true;
			bool isBsme = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
			if (isBsme)
			{
				activeProduct = product.UserData6.ToUpper() == "YES";
				if (!activeProduct)
				{
					const string Message = "The ledger transaction links will not be editable since the selected product is not active.";

					string alertString = "<script type=\"text/javascript\">\r\n<!--\r\n alert(";
					alertString += HttpUtility.JavaScriptStringEncode(Message, true) + "); \r\n--></script>";

					ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "InactiveProductAlert", alertString, false);
				}
			}
			this.DisableLinksBasedOnSecurityRights(activeProduct);

			this.ChangeInventoryDateColumnLabel();  // Needs to be called before data bind.

			this.Grid.DateFormatInfo = this.AccountingSite.CurrentSite.GetDateTimeFormatInfo();
			this.Grid.BindData(
				ledgerDO.LedgerLineItems, quantityDisplay, volumeDecimalPlaces, massDecimalPlaces, product.LoadByWeight);

			var transactionListContext = new TransactionListContext
										 {
											 Site = this.security.SiteID,
											 Month = this.Session[PageSessionKeyConstants.LEDGER_MONTH_SELECTION] as string,
											 Manager = this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] as string,
											 Product = this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string,
											 ReturnURL = "Ledger.aspx",
											 TransactionListReturnURL = "Ledger.aspx",
											 Owner = this.Session[PageSessionKeyConstants.LEDGER_OWNER_SELECTION] as string
										 };

			this.Session["TransactionListContext"] = transactionListContext;
		}

		/// <summary>
		/// Coordinates the values in the Date Type dropdown list and the column heading for the Inventory Date column.
		/// </summary>
		private void ChangeInventoryDateColumnLabel()
		{
			if (this.Grid.Tables.Count <= 0)
			{
				return;
			}
			var translatedText = this.GetTranslatedText(BsmeLedgerDateType.InventoryDateType);
			var dateIndex = this.Grid.Tables[0].Columns.IndexOf(translatedText);
			if (dateIndex < 0)
			{
				return;
			}
			var displayText = BsmeLedgerDateType.GetDisplayText(this.Session[PageSessionKeyConstants.LEDGER_DATE_TYPE_SELECTION]);
			this.Grid.Tables[0].Columns[dateIndex].ColumnName = displayText;
		}
		#endregion
	}
}