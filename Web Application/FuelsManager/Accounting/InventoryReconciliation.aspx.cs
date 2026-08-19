// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryReconciliation.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for InventoryReconciliation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Globalization;
	using System.Web;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
	using System.Diagnostics;

	/// <summary>
	/// The inventory reconciliation.
	/// </summary>
	public partial class InventoryReconciliation : AccountingWebFormView
	{
		#region Private Attributes
		private AccountingSite accountingSite;
		private InventoryReconciliationDO inventoryReconciliationDo;
		private bool dateChanged;
		private bool managerChanged;
		private bool productChanged;
		private bool tankChanged;
		private ProductClass product;
		private byte volumeDecimalPlaces;
		private byte massDecimalPlaces;
		private bool closeoutDisableFlag;
		#endregion

		/// <summary>
		/// The grid.
		/// </summary>
		protected ListViewDataSet Grid = null;

		#region Page Load
		/// <summary>
		/// The page load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Whether to disable the closeout buttons.
				this.GetCloseoutDisableFlag();

				// Remove from session as it effects ProductSelectForm behavior
				this.Session.Remove("TransactionDetailTransaction");

				this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

				if (this.Page.IsPostBack == false)
				{
					// Set initial GrossNetFlag setting
					if (this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] == null)
					{
						this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] = this.accountingSite.CurrentSite.QuantityDisplayDefault;
					}

					this.LoadDropdownList(null);
					this.InitializeDataGrid();

					if (this.accountingSite.CurrentSite.UseTankReconciliation == false)
					{
						this.tankTextBox.Visible = false;
						this.tankTextBox.Enabled = false;
						this.tankLabel.Visible = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will get the flag that makes the closeout buttons always disabled from the
		/// configuration settings in the database.
		/// </summary>
		private void GetCloseoutDisableFlag()
		{
			this.closeoutDisableFlag = false;

			string disableFlagStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_ForceCloseoutButtonDisable));

			if (string.IsNullOrEmpty(disableFlagStr) == false && disableFlagStr.ToUpper().Equals("TRUE"))
			{
				this.closeoutDisableFlag = true;
			}
		}

		/// <summary>
		/// This method will run through the list of months and process each month name
		/// through the data dictionary. It will build the month/year string (i.e. June 2004)
		/// and return the new list.
		/// </summary>
		/// <param name="monthList">
		/// The month list.
		/// </param>
		/// <param name="yearList">
		/// The year list.
		/// </param>
		/// <returns>
		/// The <see cref="ArrayList"/>.
		/// </returns>
		private ArrayList ApplyDictionaryToMonths(ArrayList monthList, ArrayList yearList)
		{
			var combined = new ArrayList();

			for (int nextMonth = 0; nextMonth < monthList.Count; nextMonth++)
			{
				var item = new DropdownItem();
			    var month = nextMonth;
				string monthStr = GetDataDictionaryValueByKey(this.security.SiteGuid, (string)monthList[nextMonth]);
				item.Text = monthStr + " " + yearList[nextMonth];
				item.TextValue = monthList[nextMonth] + " " + yearList[nextMonth];
				combined.Add(item);
			}

			return combined;
		}

		/// <summary>
		/// This method will load the manager, product, and transaction type dropdown lists for
		/// a given site and user.
		/// </summary>
		/// <param name="inventorySr">
		/// The inventory Service.
		/// </param>
		private void LoadDropdownList(InventoryReconciliationSR inventorySr)
		{
			InventoryReconciliationSR inventoryRecSr = inventorySr ?? new InventoryReconciliationSR { Security = this.security };

			inventoryRecSr.Subrequest = InventoryReconciliationSR.RequestTypes.GET_HEADER_DATA;
			inventoryRecSr.UseDataDictionary = this.useDataDictionary;

			InventoryReconciliationDO inventoryRecDO = FMChannelHelper.MakeCall<IInventoryReconciliationProcessor, InventoryReconciliationDO>(
																									x => x.Process(inventoryRecSr, this.accountingSite));

			var ledgerProductSelection = this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string;
			var ledgerManagerSelection = this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] as string;

			if (!string.IsNullOrEmpty(ledgerProductSelection) && inventoryRecDO.ProductList.IndexOf(ledgerProductSelection) >= 0)
			{
				this.productTextBox.Text = ledgerProductSelection;
			}
			else if (inventoryRecDO.ProductList.Count > 0)
			{
				this.productTextBox.Text = inventoryRecDO.ProductList[0] as string;
			}

			if (!string.IsNullOrEmpty(ledgerManagerSelection) && inventoryRecDO.ManagerList.IndexOf(ledgerManagerSelection) >= 0)
			{
				this.managerTextBox.Text = ledgerManagerSelection;
			}
			else if (inventoryRecDO.ManagerList.Count > 0)
			{
				this.managerTextBox.Text = inventoryRecDO.ManagerList[0] as string;
			}

			if (inventoryRecDO.TankList.Count > 0)
			{
				this.tankTextBox.Text = inventoryRecDO.TankList[0] as string;
			}

			// Apply the data dictionary to the name of the months and combine
			// the month and year.
			ArrayList monthYearList = this.ApplyDictionaryToMonths(inventoryRecDO.MonthList, inventoryRecDO.YearList);

			this.dateDropDownList.DataSource = monthYearList;
			this.dateDropDownList.DataTextField = "Text";
			this.dateDropDownList.DataValueField = "TextValue";
			this.dateDropDownList.DataBind();

			var context = this.Session[PageSessionKeyConstants.INVENTORY_RECONCILIATION_CONTEXT_KEY] as InventoryReconciliationContext;

			// Check the session before using the gross/net object (IGO 12-Apr-2007)
			if (this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION] != null)
			{
				this.QuantityDropDownList.SelectedIndex = (int)this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION];
			}

			if (context != null)
			{
				this.dateDropDownList.SelectedValue = context.Month;
			}
			else
			{
				var dateTimeOffsetConverter = new DateTimeOffsetConverter();
				this.dateDropDownList.SelectedIndex = 0;

				int selectionIndex = 0;
				int monthNow = DateTimeOffset.Now.Month;
				int yearNow = DateTimeOffset.Now.Year;
				
				foreach (DropdownItem monthItem in monthYearList)
				{
					string monthAndYear = monthItem.TextValue;
					var convertFrom = dateTimeOffsetConverter.ConvertFrom(monthAndYear);

					if (convertFrom != null)
					{
						var convertedDateTimeOffset = (DateTimeOffset)convertFrom;

						if ((convertedDateTimeOffset.Month == monthNow) && (convertedDateTimeOffset.Year == yearNow))
						{
							this.dateDropDownList.SelectedIndex = selectionIndex;
							break;
						}
					}

					selectionIndex++;
				}
			}
		}

		/// <summary>
		/// The initialize data grid.
		/// </summary>
		private void InitializeDataGrid()
		{
			var sr = new InventoryReconciliationSR
			{
				Security = this.security,
				Site = this.security.SiteID,
				Subrequest = InventoryReconciliationSR.RequestTypes.REFRESH,
				ManagerID = this.managerTextBox.Text,
				Month = this.dateDropDownList.SelectedValue
			};

			if (string.IsNullOrEmpty(this.tankTextBox.Text) || this.tankTextBox.Text == "{All}")
			{
				sr.TankId = string.Empty;
			}
			else
			{
				sr.TankId = this.tankTextBox.Text;
			}

			sr.ProductID = this.productTextBox.Text;

			// ListViews will need to know if the product is a Volume or AdditiveVolume, because they are formatted differently.
			//this.product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
			//					x => x.Get(this.security, x.GetIdentityGuid(this.security, sr.ProductID), false, false));

			this.product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
								x => x.GetMinimalProductData(this.security, x.GetIdentityGuid(this.security, sr.ProductID)));

			if (this.product.ProductType == ProductType.AdditiveProduct)
			{
				this.volumeDecimalPlaces = this.product.VolumeUnits == 0 ?
						this.accountingSite.CurrentSite._AdditiveVolumeDecimalPlaces : this.product.VolumeDecimalPlaces;
			}
			else
			{
				this.volumeDecimalPlaces = this.product.VolumeUnits == 0 ?
						this.accountingSite.CurrentSite._VolumeDecimalPlaces : this.product.VolumeDecimalPlaces;
			}

			this.massDecimalPlaces = this.product.MassUnits == 0 ?
						this.accountingSite.CurrentSite._MassDecimalPlaces : this.product.MassDecimalPlaces;
			this.ToleranceTextBox.Text =
				this.product.VarianceTolerance.ToString(this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
			this.ToleranceTextBox.MaxLength = this.ToleranceTextBox.Text.Length;
			this.inventoryReconciliationDo = FMChannelHelper.MakeCall<IInventoryReconciliationProcessor, InventoryReconciliationDO>(
																											x => x.Process(sr,this.accountingSite));

			this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] = sr.ProductID;
			this.Session[PageSessionKeyConstants.LEDGER_MANAGER_SELECTION] = sr.ManagerID;

			// build up inventory reconciliation context before binding data to the grid (IGO 28-Aug-2008)
			var inventoryReconciliationContext = new InventoryReconciliationContext
			{
				ManagerID = sr.ManagerID,
				ProductID = sr.ProductID,
				Month = sr.Month,
				inventoryReconciliationDO = this.inventoryReconciliationDo
			};

			this.Session[PageSessionKeyConstants.INVENTORY_RECONCILIATION_CONTEXT_KEY] = inventoryReconciliationContext;

			this.FormatForRegionalSettings(this.inventoryReconciliationDo);
			BaseCollections lineItems = this.inventoryReconciliationDo.LineItems;

			// Setup column view
			Guid typeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.INVENTORY_RECONCILIATION);
			this.Grid = new ListViewDataSet(this.security, LISTVIEW_TYPE.STANDARD, typeGuid, this.accountingSite);

			this.Grid.SetDataGrid(this.InventoryRecDataGrid);
			this.Grid.SetNavigateURL("../Accounting/TransactionList.aspx");

			// Suppress the Tolerance text box if TolerancePercentage is one of the columns in the list view
			for (int ndx = 0; this.Grid.listViewDO.getListViewColumn(ndx) != null; ndx++)
			{
				ListViewColumnDO currentColumn = this.Grid.listViewDO.getListViewColumn(ndx);

				if (string.Equals(currentColumn.DataPath, "Tolerance", StringComparison.InvariantCulture))
				{
					this.ToleranceTextBox.Visible = false;
					this.ToleranceLabel.Visible = false;
					break;
				}
			}

			// Bind data rows
			var quantityDisplay = (QuantityDisplay)this.Session[PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION];

			this.Grid.BindData(lineItems, quantityDisplay, this.volumeDecimalPlaces, this.massDecimalPlaces, this.product.LoadByWeight);

			var transactionListContext = new TransactionListContext
			{
				Site = this.security.SiteID,
				Month = sr.Month,
				Manager = sr.ManagerID,
				Owner = null,
				Product = sr.ProductID,
				ReturnURL = "InventoryReconciliation.aspx",
				TransactionListReturnURL = "InventoryReconciliation.aspx"
			};

			this.Session["TransactionListContext"] = transactionListContext;

			// for auto distribution, disable if there is no physical inv on last day
			int lastDayIndex = lineItems.Count - 2; // last line is the totals
			var lineItem = lineItems[lastDayIndex] as InventoryReconciliationLineItemDO;

			if (lineItem != null)
			{
				if (this.closeoutDisableFlag == false)
				{
					if (lineItem.Flags.CheckFlag(BaseLineItemDO.Status.CLOSED_OUT))
					{
						this.autoDistributionButton.Enabled = false;
					}
					else if (this.security.HasRight(RIGHT.PERFORM_AUTO_DISTRIBUTION))
					{
						this.autoDistributionButton.Enabled = lineItem.CheckFlag(BaseLineItemDO.Status.PHYS_INV_EXISTS);
					}
				}
				else
				{
					this.autoDistributionButton.Enabled = false;
				}
			}
		}

		/// <summary>
		/// The format for regional settings.
		/// </summary>
		/// <param name="inventoryListDo">
		/// The inventory list data object.
		/// </param>
		private void FormatForRegionalSettings(InventoryReconciliationDO inventoryListDo)
		{
			foreach (InventoryReconciliationLineItemDO item in inventoryListDo.LineItems)
			{
				if (item.InventoryDate.ToUpper().StartsWith("TOT") == false)
				{
					// Save the original date format as the original date.
					item.OriginalInventoryDate = item.DtInventoryDate;
					item.InventoryDate = this.accountingSite.FormatDate(item.DtInventoryDate);
				}
			}
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
			base.CurrentSiteGuid = Guids.SiteAdminGuid;
			base.Initialize();
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.InventoryRecDataGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.InventoryRecDataGridItemCommand);
			this.InventoryRecDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.InventoryRecDataGridItemDataBound);

		}
		#endregion

		/// <summary>
		/// The inventory reconciliation data grid item data bound.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void InventoryRecDataGridItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex > -1)
			{
				var closeoutButton = (FMButton)e.Item.FindControl("CloseoutButton");

				if (closeoutButton != null)
				{
					// Invisible on Total Line
					if (e.Item.ItemIndex == this.inventoryReconciliationDo.LineItems.Count - 1)
					{
						closeoutButton.Visible = false;
					}
					else
					{
						var lineItem = this.inventoryReconciliationDo.LineItems[e.Item.ItemIndex] as InventoryReconciliationLineItemDO;

						if (lineItem != null && (!this.security.HasRight(RIGHT.PERFORM_CLOSEOUT)
						                         || lineItem.Flags.CheckFlag(BaseLineItemDO.Status.CLOSED_OUT)
						                         || !lineItem.Flags.CheckFlag(BaseLineItemDO.Status.PHYS_INV_EXISTS)
						                         || !(string.IsNullOrEmpty(this.tankTextBox.Text) || this.tankTextBox.Text == "{All}")
												 || this.closeoutDisableFlag))
						{
							closeoutButton.Enabled = false;
						}
						else
						{
							string confirmText = GetDataDictionaryValueByKey(this.Security.SiteGuid, "Closeout");

							if (lineItem != null)
							{
								confirmText = confirmText + " " + this.accountingSite.FormatDate(lineItem.OriginalInventoryDate);
							}

							confirmText = confirmText + ", "
												+ FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.SiteGuid, "Total Variance"));

							var numberFormat = new NumberFormatInfo
							{
								NumberGroupSizes = this.accountingSite.CurrentSite.GetNumberGroupSizes(),
								NumberDecimalSeparator = this.accountingSite.CurrentSite.NumberDecimalSeparator,
								NumberGroupSeparator = this.accountingSite.CurrentSite.NumberGroupSeparator
							};

							if (this.product.LoadByWeight)
							{
								numberFormat.NumberDecimalDigits = this.massDecimalPlaces;

								if (lineItem != null)
								{
									string massVariance = lineItem.TotalVariance.Mass.ToString("N", numberFormat);

									if ("0" != massVariance && lineItem.TotalVariance.MassInventoryChange < 0)
									{
										massVariance = "(" + massVariance + ")";
									}

									confirmText = confirmText + "(M) = " + massVariance;
								}
							}
							else
							{
								numberFormat.NumberDecimalDigits = this.volumeDecimalPlaces;

								if (lineItem != null)
								{
									string grossVariance = lineItem.TotalVariance.Gross.ToString("N", numberFormat);

									if ("0" != grossVariance && lineItem.TotalVariance.GrossInventoryChange < 0)
									{
										grossVariance = "(" + grossVariance + ")";
									}

									confirmText = confirmText + "(G) = " + grossVariance;
								}

								confirmText = confirmText + ", "
												+ GetDataDictionaryValueByKey(this.Security.SiteGuid, "Total Variance");

								if (lineItem != null)
								{
									string netVariance = lineItem.TotalVariance.Net.ToString("N", numberFormat);

									if ("0" != netVariance && lineItem.TotalVariance.NetInventoryChange < 0)
									{
										netVariance = "(" + netVariance + ")";
									}

									confirmText = confirmText + "(N) = " + netVariance;
								}
							}

							confirmText = confirmText + "?";

							if (this.IsThereAdjustments(e.Item.ItemIndex) == false)
							{
								confirmText = confirmText + "  !!!WARNING - No adjustments have been created!!!";
							}

							closeoutButton.Attributes.Add("onClick", "if(disabled)return false; return confirm(" + HttpUtility.JavaScriptStringEncode(confirmText, true) + ");");
						}
					}
				}
			}
		}

		/// <summary>
		/// The refresh button click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void RefreshButtonClick(object sender, EventArgs e)
		{
			this.InitializeDataGrid();
		}

		/// <summary>
		/// The auto distribution button click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		/// <exception cref="ApplicationException">
		/// Default rule not found.
		/// </exception>
		protected void AutoDistributionButtonClick(object sender, EventArgs e)
		{
			string pageUrl = string.Empty;

			try
			{
				if (this.dateDropDownList.SelectedIndex > -1)
				{
					string monthAndYear = this.dateDropDownList.SelectedValue;
					
					// find total variances
					var context = this.Session[PageSessionKeyConstants.INVENTORY_RECONCILIATION_CONTEXT_KEY] as InventoryReconciliationContext;

					if (context != null)
					{
						int lastDayIndex = context.inventoryReconciliationDO.LineItems.Count - 2; // last line is the totals
						var lineItem = context.inventoryReconciliationDO.LineItems[lastDayIndex] as InventoryReconciliationLineItemDO;
					
						if (lineItem != null)
						{
							QuantityDO totalVariances = lineItem.TotalVariance;

							// get manager and product guid
							Guid managerGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetIdentityGuid(this.security, this.managerTextBox.Text));
							Guid productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(this.security, this.productTextBox.Text));

							// get rule guid
							Guid ruleGuid;
							string errorMessage;

							if (AutoDistributionOperationPage.FindDefaultRule(this.Security, managerGuid, productGuid, out ruleGuid, out errorMessage) == false)
							{
								throw new ApplicationException(errorMessage);
							}

							// prepare url parameters
							var queryParameters = new NameValueCollection 
											{
												{ AutoDistributionOperationPage.UrlParamOperationType, AutoDistributionOperationTypes.InventoryReconcilliation.ToString() },
												{ AutoDistributionOperationPage.UrlParamRuleGuid, ruleGuid.ToString() },
												{ AutoDistributionOperationPage.UrlParamManagerGuid, managerGuid.ToString() },
												{ AutoDistributionOperationPage.UrlParamProductGuid, productGuid.ToString() },
												{ AutoDistributionOperationPage.UrlParamGross, totalVariances.GrossInventoryChange.ToString(CultureInfo.InvariantCulture) },
												{ AutoDistributionOperationPage.UrlParamNet, totalVariances.NetInventoryChange.ToString(CultureInfo.InvariantCulture) },
												{ AutoDistributionOperationPage.UrlParamMass, totalVariances.MassInventoryChange.ToString(CultureInfo.InvariantCulture) },
												{ AutoDistributionOperationPage.UrlParamInventoryMonth, monthAndYear }
											};

							pageUrl = this.FMFormatUrl(AutoDistributionOperationPage.PageUrl, queryParameters);
						}
					}
				}
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}

			if (string.IsNullOrWhiteSpace(pageUrl) == false)
			{
				this.Redirect(pageUrl);
			}
		}

		/// <summary>
		/// The quantity dropdown list selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void QuantityDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.Session.Add(PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION, this.QuantityDropDownList.SelectedIndex);
		}

		/// <summary>
		/// The inventory reconciliation data grid item command.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void InventoryRecDataGridItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Closeout")
			{
				if (this.productChanged)
				{
					this.DisplayErrorDialog("Can't closeout because product changed");
				}
				else if (this.managerChanged)
				{
					this.DisplayErrorDialog("Can't closeout because manager changed");
				}
				else if (this.dateChanged)
				{
					this.DisplayErrorDialog("Can't Closeout because date changed");
				}
				else if (this.tankChanged)
				{
					this.DisplayErrorDialog("Can't Closeout because tank changed");
				}
				else
				{
					var context = this.Session[PageSessionKeyConstants.INVENTORY_RECONCILIATION_CONTEXT_KEY] as InventoryReconciliationContext;

				    var lineItem = context?.inventoryReconciliationDO.LineItems[e.Item.ItemIndex] as InventoryReconciliationLineItemDO;

				    if (lineItem != null)
				    {
				        // set error flags 
				        if (lineItem.Flags.CheckFlag(BaseLineItemDO.Status.BROKEN_BLENDS))
				        {
				            this.DisplayErrorDialog("Can't Closeout because broken blend detected");
				            return; // finally block will reinit data grid
				        }

				        SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.security, this.security.SiteGuid, false, false, false));

				        if (site.BlockCloseOnUnpostedBol)
				        {
				            // find last closeout for this product
				            CloseoutListSR closeoutListSR = new CloseoutListSR
				                                            {
				                                                ManagerGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetMasterRecordGuid(this.security, context.ManagerID)),
				                                                ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuidFromID(this.security, context.ProductID)),
				                                                Site = this.security.SiteID,
				                                                Security = this.security,
				                                                CurrentSiteGuid = this.security.SiteGuid
				                                            };

				            var closeoutListDO = FMChannelHelper.MakeCall<ICloseoutListProcessor, CloseoutListDO>(x => x.Process(closeoutListSR));

				            // Now get BOLs between last closeout and current requested closeout, for this product                              
				            var getSR = new GetTransactionSR
				                        {
				                            Request = GetTransactionRequest.SITE_MANAGER_PRODUCT_UNPOSTED_ISSUE,
				                            ManagerID = context.ManagerID,
				                            Product = context.ProductID
				                        };

				            if (closeoutListDO.PriorCloseout != null && closeoutListDO.PriorCloseout.CloseoutRecordFound)
				            {
				                getSR.BeginningDate = closeoutListDO.PriorCloseout.CloseoutDate;
				            }
				            else
				            {
				                getSR.BeginningDate = new DateTime(1900, 1, 1);
				            }
				            getSR.EndingDate = lineItem.OriginalInventoryDate.Date.Add(TimeSpan.FromSeconds(86399.0)); // 86400 seconds in a day, advance to end of specified day
				            getSR.Status = ((int)TransactionStatus.Completed).ToString(CultureInfo.InvariantCulture);
				            getSR.Security = this.security;
				            var getDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(x => x.Process(getSR));

				            if (getDO.TransactionDataSet.Tables[0].Rows.Count > 0)
				            {
				                this.DisplayErrorDialog("Can't Closeout because unposted BOLs detected");
				                return; // finally block will reinit data grid
				            }
				        }

				        var closeoutDO = new CloseoutDO
				                         {
				                             CloseoutDate			= lineItem.OriginalInventoryDate,
				                             BookInventory			= lineItem.BookInventory,
				                             TotalPhysicalInventory = lineItem.TotalPhysicalInventory,
				                             TotalVariance			= lineItem.TotalVariance,
				                             ManagerName			= this.managerTextBox.Text,
				                             ManagerGuid			= FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetMasterRecordGuid(this.security, this.managerTextBox.Text)),
				                             ProductName			= this.productTextBox.Text,
				                             ProductGuid			= FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuidFromID(this.security, this.productTextBox.Text)),
				                             SiteID					= this.security.SiteID,
				                             SiteGuid				= this.security.SiteGuid
				                         };

				        var closeoutSr = new CloseoutSR
				                         {
				                             Closeout			= closeoutDO,
				                             CloseoutCommand	= CloseoutSR.CloseoutType.CREATE,
				                             Security			= this.security,
				                             Site				= this.security.SiteID,
				                             CurrentSiteGuid	= this.security.SiteGuid,
				                             InventoryDate		= lineItem.OriginalInventoryDate,
				                             ManagerName		= this.managerTextBox.Text,
				                             ManagerCompanyGuid = closeoutDO.ManagerGuid,
				                             ProductName		= this.productTextBox.Text,
				                             ProductGuid		= closeoutDO.ProductGuid
				                         };

				        try
				        {
				            FMChannelHelper.MakeCall<ICloseoutProcessor, CloseoutDO>(x => x.Process(closeoutSr));
				        }
				        catch (Exception exception)
				        {
				            this.DisplayErrorDialog("[Error in creating a closeout transaction] - " + exception.Message + " \n");
				        }
				    }
				}
			}

			this.InitializeDataGrid();
		}

		#region Private Popup Error diaglog
		/// <summary>
		/// This method will display an error dialog informing the user of an error.
		/// </summary>
		/// <param name="errorMessage">
		/// The error Message.
		/// </param>
		private void DisplayErrorDialog(string errorMessage)
		{
			string errMsg = GetDataDictionaryValueByKey(this.Security.SiteGuid, errorMessage) + "!";
			this.RenderErrorMessage(errMsg);
		}

		/// <summary>
		/// This method will return true if there are adjustment transaction for the last day
		/// of the month or within the product variance tolerance.
		/// </summary>
		/// <param name="lineItemIndex">
		/// The line Item Index.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool IsThereAdjustments(int lineItemIndex)
		{
			bool hasAdjustments = false;

			var inventoryReconciliationContext = 
				this.Session[PageSessionKeyConstants.INVENTORY_RECONCILIATION_CONTEXT_KEY] as InventoryReconciliationContext;
			
			if (inventoryReconciliationContext != null)
			{
				var selectedLineItem = 
					inventoryReconciliationContext.inventoryReconciliationDO.LineItems[lineItemIndex] as InventoryReconciliationLineItemDO;
				BaseCollections lineItems = inventoryReconciliationContext.inventoryReconciliationDO.LineItems;

				// Find the last day of the month.
				int lastDayIndex = lineItems.Count - 2; // last line is the totals
				var lineItem = lineItems[lastDayIndex] as InventoryReconciliationLineItemDO;

				if (lineItem != null && selectedLineItem != null)
				{
					// If selected closeout line item date does not equal the end of
					// month date then there does not have to be adjustments. Therefore,
					// set has adjustments to true.
					if (selectedLineItem.InventoryDate != lineItem.InventoryDate)
					{
						// Indicates that there are adjustments.
						hasAdjustments = true;
					}
					else
					{
						// Selected closeout line item is the last date of the month.
						// check for adjustments.
						// If the variance is greater than zero (meaning there is a variance), then
						// check for adjustments.
						if (lineItem.Variance.Gross > 0.0 || lineItem.Variance.Net > 0.0)
						{
							var inventoryReconciliationSr = new InventoryReconciliationSR
							{
								Security = this.security,
								ManagerID = this.managerTextBox.Text,
								ProductID = this.productTextBox.Text,
								InventoryDate = DateTime.Parse(lineItem.InventoryDate, this.accountingSite.CurrentSite.GetDateTimeFormatInfo()).Date,
								Subrequest = InventoryReconciliationSR.RequestTypes.FindAdjustments
							};

							// Find adjustment transactions for this date.
							InventoryReconciliationDO invReconciliationDo =
								FMChannelHelper.MakeCall<IInventoryReconciliationProcessor, InventoryReconciliationDO>(
									x => x.Process(inventoryReconciliationSr));

							hasAdjustments = invReconciliationDo.HasAdjustments;
						}
						else
						{
							// Indicates that there are adjustments.
							hasAdjustments = true;
						}
					}
				}
			}

			return hasAdjustments;
		}
		#endregion

		/// <summary>
		/// The product text box text changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void ProductTextBoxTextChanged(object sender, EventArgs e)
		{
			this.productChanged = true;
		}

		/// <summary>
		/// The manager text box text changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void ManagerTextBoxTextChanged(object sender, EventArgs e)
		{
			this.managerChanged = true;
		}

		/// <summary>
		/// The date dropdown list selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void DateDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.dateChanged = true;
		}

		/// <summary>
		/// The tank text box text changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void TankTextBoxTextChanged(object sender, EventArgs e)
		{
			this.tankChanged = true;
		}
	}
}
