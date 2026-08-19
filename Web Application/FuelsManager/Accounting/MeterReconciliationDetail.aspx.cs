namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using FMCore;

	/// <summary>
	/// The meter reconciliation detail screen allows a user to view all transactions which used a specified meter during 
	/// a specific inventory date
	/// </summary>
	public partial class MeterReconciliationDetail : AccountingWebFormView
	{
		#region Page Constants

		/// <summary>
		/// The default meter skip tolerance value to use on the page.
		/// </summary>
		private const string DefaultMeterSkipTolerance = "1";

		#endregion

		#region Page Properties

		/// <summary>
		/// Defines the DateTimeFormatInfo for the inventory date display box
		/// </summary>
		protected System.Globalization.DateTimeFormatInfo _dateFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo;

		/// <summary>
		/// Get or set the current sort order of the Summary Grid.
		/// </summary>
		protected SortDirection GridViewSortDirection
		{
			get
			{
				if (this.ViewState["sortDirection"] == null)
				{
					this.ViewState["sortDirection"] = SortDirection.Ascending;
				}

				return (SortDirection)this.ViewState["sortDirection"];
			}
			set
			{
				this.ViewState["sortDirection"] = value;
			}
		}

		private AccountingSite accountingSite = null;

		#endregion

		#region Session Properties

		/// <summary>
		/// Get or set the asset guid of the asset selected in the asset select box
		/// This is used to filter the meters available when selecting a meter
		/// </summary>
		private Guid SessionMeterAssetGuid
		{
			get
			{
				return this.Session["MeterAssetGuid"] != null && this.Session["MeterAssetGuid"] is Guid guid
				  ? guid
				  : Guid.Empty;
			}
			set
			{
				this.Session.Add("MeterAssetGuid", value);
			}
		}

		/// <summary>
		/// If we return to this page from the transaction detail page, we need to know the results the user was last looking at
		/// so we can display the same information. Most of the information we need is in the summary data, which is already stored in session.
		/// However, the inventory date is not. We store it so that we can use it.
		/// </summary>
		private DateTimeOffset SessionDetailInventoryDate
		{
			get
			{
				return this.Session["MeterReconciliationDetailInventoryDate"] != null && this.Session["MeterReconciliationDetailInventoryDate"] is DateTimeOffset offset
					? offset
					: DateTimeOffset.Now.AddDays(-1);
			}
			set
			{
				this.Session.Add("MeterReconciliationDetailInventoryDate", value);
			}
		}

		/// <summary>
		/// Get or set the data bound to the summary grid, which is stored in session 
		/// </summary>
		private List<MeterReconciliationSummaryData> SessionSummaryData
		{
			get
			{
				return this.Session["MeterReconciliationDetailSummaryData"] != null && this.Session["MeterReconciliationDetailSummaryData"] is List<MeterReconciliationSummaryData>
				  ? this.Session["MeterReconciliationDetailSummaryData"] as List<MeterReconciliationSummaryData>
				  : new List<MeterReconciliationSummaryData>();
			}
			set
			{
				this.Session.Add("MeterReconciliationDetailSummaryData", value);
			}
		}

		/// <summary>
		/// Get or set the data bound to the detail grid, which is stored in session 
		/// </summary>
		private List<MeterReconciliationDetailData> SessionDetailData
		{
			get
			{
				return this.Session["MeterReconciliationDetailData"] != null && this.Session["MeterReconciliationDetailData"] is List<MeterReconciliationDetailData>
					? this.Session["MeterReconciliationDetailData"] as List<MeterReconciliationDetailData>
					: new List<MeterReconciliationDetailData>();
			}
			set
			{
				this.Session.Add("MeterReconciliationDetailData", value);
			}
		}

		#endregion

		#region Page Methods

		/// <summary>
		/// This function is responsible for checking the current user's security access
		/// and responding appropriately including enforcing access and changing control
		/// availability.
		/// </summary>
		private void CheckUserSecurityAccess(SecurityClass security)
		{
			if (!security.HasRight(RIGHT.VIEW_METER_RECONCILIATION))
			{
				throw new FMInsufficientRightsException();
			}
		}

		/// <summary>
		/// Bind data to both the summary and detail grids
		/// </summary>
		/// <param name="summaryData">the summary data to bind to the summary grid</param>
		/// <param name="detailData">The detail data to bind to the detail grid</param>
		private void BindData(object summaryData, object detailData)
		{
			this.SummaryGrid.DataSource = summaryData;
			this.SummaryGrid.DataBind();

			this.DetailGrid.DataSource = detailData;
			this.DetailGrid.DataBind();
		}

		/// <summary>
		/// Submit a search request and refresh the summary and detail grids
		/// </summary>
		private void UpdateView()
		{
			try
			{
				// Create and populate our service request
				MeterReconciliationSR sr = new MeterReconciliationSR();

				// Set the filtering criteria
				this.SetSearchCriteria(sr);

				// Retrieve the summary and detail data based on the search parameters
				List<MeterReconciliationSummaryData> meterReconciliationSummaryCollection =
					FMChannelHelper.MakeCall<IMeterReconciliationProcessor, List<MeterReconciliationSummaryData>>((meterReconciliationProcessor) => meterReconciliationProcessor.GetSummary(sr));

				List<MeterReconciliationDetailData> meterReconciliationDetailCollection =
					FMChannelHelper.MakeCall<IMeterReconciliationProcessor, List<MeterReconciliationDetailData>>((meterReconciliationProcessor) => meterReconciliationProcessor.GetDetail(sr));

				// Bind the data retrieved to the grid
				this.SessionSummaryData = meterReconciliationSummaryCollection;
				this.SessionDetailData = meterReconciliationDetailCollection;

				this.SessionDetailInventoryDate = sr.InventoryDate;

				this.BindData(meterReconciliationSummaryCollection, meterReconciliationDetailCollection);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Set the search criteria based on the values provided by the user
		/// </summary>
		/// <param name="sr">A service request to populate</param>
		private void SetSearchCriteria(MeterReconciliationSR sr)
		{
			sr.Security = this.Security;
			sr.InventoryDate = this.InventoryDate.CurrentValue.Date;
			sr.CurrentSiteGuid = this.Security.SiteGuid;


			if (!double.TryParse(this.SkipToleranceValueTextBox.Text,
					 NumberStyles.Float | NumberStyles.AllowThousands,
					 this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT),
					 out double skipToleranceValue))
			{
				throw new ApplicationException("Skip tolerance value must be numeric");
			}

			// Set the meter guid based on the selection in the meter text box
			if (!string.IsNullOrEmpty(this.MeterIDTextBox.Text))
			{
				sr.MeterGuid = FMChannelHelper.MakeCall<IMeters, Guid>((meters) => meters.GetIdentityGuid(this.Security, this.MeterIDTextBox.Text));
			}
		}

		/// <summary>
		/// Fill the search filter fields at the top of the page with values provided from the item the user
		/// selected on the meter reconciliation summary screen, or the last values the user selected if we are returning from
		/// the transaction detail screen.
		/// </summary>
		private void PopulateSearchFields()
		{
			// Are we returning to this screen from the transaction detail screen? 
			// If so, we should populate the detail form with the search parameters that were last provided.
			if (this.Request.GetQueryOrFormValue("Returning") != null && Convert.ToBoolean(this.Request.GetQueryOrFormValue("Returning")) == true)
			{
				if (this.SessionSummaryData.Count > 0)
				{
					MeterReconciliationSummaryData lastBoundSummaryData = this.SessionSummaryData[0] as MeterReconciliationSummaryData;

					Guid meterGuid = lastBoundSummaryData.MeterGuid;
					MeterClass meter = FMChannelHelper.MakeCall<IMeters, MeterClass>((meters) => meters.Get(this.Security, meterGuid));

					this.MeterIDTextBox.Text = meter.ID;

					this.AssetTextBox.Text = lastBoundSummaryData.AssetID;

					this.InventoryDate.CurrentValue = this.SessionDetailInventoryDate;
				}
			}
			else
			{
				// We got here from the meter reconciliation summary screen, so we should pull the search parameters out of Request
				if (!string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("MeterGuid")))
				{
					Guid meterGuid = Guid.Parse(this.Request.GetQueryOrFormValue("MeterGuid"));
					MeterClass meter = FMChannelHelper.MakeCall<IMeters, MeterClass>((meters) => meters.Get(this.Security, meterGuid));

					this.MeterIDTextBox.Text = meter.ID;
				}

				if (!string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("AssetGuid")))
				{
					this.SessionMeterAssetGuid = Guid.Parse(this.Request.GetQueryOrFormValue("AssetGuid"));

					if (!string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("AssetID")))
					{
						this.AssetTextBox.Text = this.Request.GetQueryOrFormValue("AssetID");
					}
				}

				if (!string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("InventoryDate")))
				{
					this.InventoryDate.CurrentValue = DateTime.Parse(this.Request.GetQueryOrFormValue("InventoryDate"));
				}
			}

			// The default skip tolerance is 1
			this.SkipToleranceValueTextBox.Text = MeterReconciliationDetail.DefaultMeterSkipTolerance;
		}

		#endregion

		#region Page Event Handlers

		/// <summary>
		/// Fires when the page loads, which is a good time for us
		/// to do things like get the user's security information and populate the grids
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Get the site information, which we'll use to get the site's number formatting information to use when parsing the tolerance value
				this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(accountingSites =>
						  accountingSites.LoadSiteInfoNoCompanies(this.Security, this.Security.SiteGuid)
				);

				if (!this.Page.IsPostBack)
				{
					this.PopulateSearchFields();

					// If the session is null, then remove all objects from the 
					// session and display the accounting error page.
					if (this.Session["Security"] == null)
					{
						this.Session.RemoveAll();
						this.DisplayErrorPage();
						return;
					}

					// Check the user's security access
					this.CheckUserSecurityAccess(this.Security);

					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the user clicks on the view closeout button for a row in the summary grid. 
		/// We navigate the user to the transaction detail page for the transaction which corresponds
		/// to the current closeout for the inventory period specified
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">contains the command argument, which is in this case the index of the row the user clicked on</param>
		protected void SummaryGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			// Find the meter reconciliation transactionAlias
			TransactionAliasCollectionClass transactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(this.Security, TransactionTypes.T12_InventoryNotAffected)
																);

			TransactionAliasClass meterReadingTransactionAlias = null;

			foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
			{
				if (!transactionAlias.MeterCloseout)
				{
					continue;
				}

				meterReadingTransactionAlias = transactionAlias;
				break;
			}

			if (meterReadingTransactionAlias == null)
			{
				return;
			}

			// Create session object for TransactionDetail list of transactions.
			var detailList = new TransactionDetailList();

			// Put each transaction ID into the list for Previous/Next buttons.
			foreach (GridViewRow row in this.SummaryGrid.Rows)
			{
				var transId = ((Literal)row.FindControl("TransactionIDGridHidden")).Text;
				detailList.TransactionIDList.Add(transId);
			}

			// Indicate which transaction id in the list is the one to initially display.
			detailList.CurrentIndex = Convert.ToInt32(e.CommandArgument);

			var selectedTransactionGuid = ((Literal)this.SummaryGrid.Rows[detailList.CurrentIndex].FindControl("TransactionGuidText")).Text;
			var validatedTransactionGuid = selectedTransactionGuid == null ? "" : selectedTransactionGuid.ToString();
			if (Guid.TryParse(validatedTransactionGuid, out Guid parsedSelectedTransactionGuid))
			{
				detailList.SelectedTransactionGuid = parsedSelectedTransactionGuid;
			}

			detailList.SelectedTransactionAliasID = meterReadingTransactionAlias.ID;

			// Indicate the return URL for when the TransactionDetail Close button is clicked.
			detailList.ReturnURL = "../Accounting/MeterReconciliationDetail.aspx?Returning=true";

			// Put the object into session and transfer to the TransactionDetail.
			this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

			// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

			if (this.Request.GetQueryOrFormValue("TrackReturn") == "true")
			{
				if (transactionDetailUrl.IndexOf('?') > 0)
				{
					transactionDetailUrl += "&TrackReturn=true";
				}
				else
				{
					transactionDetailUrl += "?TrackReturn=true";
				}
			}

			this.Redirect("../" + transactionDetailUrl);
		}

		/// <summary>
		/// Fires when a row is bound to the summary grid. We do things like hide or show the error indicator based on the data
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">Contains information about the row being bound</param>
		protected void SummaryGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					if (e.Row.DataItem != null && e.Row.DataItem is MeterReconciliationSummaryData)
					{
						MeterReconciliationSummaryData summaryData = e.Row.DataItem as MeterReconciliationSummaryData;

						// If the closeout information had an error, don't show the edit closeout button
						FMEditLinkButton view = (FMEditLinkButton)e.Row.FindControl("ViewCloseoutButton");

						if (view != null)
						{
							if (summaryData.IsError)
							{
								view.Visible = false;
							}
							else
							{
								view.Visible = true;
								view.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
							}
						}

						// Display double values using the site's number formatting information
						NumberFormatInfo siteNumberFormat = this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);

						Label meterStartLabel = (Label)e.Row.FindControl("MeterStartGridColumn");
						if (meterStartLabel != null)
						{
							meterStartLabel.Text = summaryData.MeterStart.ToString(siteNumberFormat);
						}

						Label meterStopLabel = (Label)e.Row.FindControl("MeterStopGridColumn");
						if (meterStopLabel != null)
						{
							meterStopLabel.Text = summaryData.MeterStop.ToString(siteNumberFormat);
						}

						Label meterTotalLabel = (Label)e.Row.FindControl("MeterTotalGridColumn");
						if (meterTotalLabel != null)
						{
							meterTotalLabel.Text = summaryData.MeterTotal.ToString(siteNumberFormat);
						}

						Label transactionVolumeTotalLabel = (Label)e.Row.FindControl("TransactionVolumeTotalGridColumn");
						if (transactionVolumeTotalLabel != null)
						{
							transactionVolumeTotalLabel.Text = summaryData.TransactionVolumeTotal.ToString(this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
						}

						Label volumeVarianceLabel = (Label)e.Row.FindControl("VolumeVarianceGridColumn");
						if (volumeVarianceLabel != null)
						{
							volumeVarianceLabel.Text = summaryData.VolumeVariance.ToString(siteNumberFormat);
						}

						Label transactionMeterTotalLabel = (Label)e.Row.FindControl("TransactionMeterTotalGridColumn");
						if (transactionMeterTotalLabel != null)
						{
							transactionMeterTotalLabel.Text = summaryData.TransactionMeterTotal.ToString(this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
						}

						Label meterVarianceLabel = (Label)e.Row.FindControl("MeterVarianceGridColumn");
						if (meterVarianceLabel != null)
						{
							meterVarianceLabel.Text = summaryData.MeterVariance.ToString(siteNumberFormat);
						}

						// If the closeout information had an error, show the error indicator 
						if (e.Row.FindControl("ErrorImage") is WebControl)
						{
							var errorImage = e.Row.FindControl("ErrorImage") as WebControl;
							if (summaryData.IsError)
							{
								errorImage.Visible = true;
								errorImage.ToolTip = summaryData.GenerateErrorText(this.InventoryDate.CurrentValue);
							}
							else
							{
								errorImage.Visible = false;
								errorImage.ToolTip = string.Empty;
							}
						}

						// Set the color of the row to gray if there's an error
						if (summaryData.IsError)
						{
							e.Row.ForeColor = Color.Gray;
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
		/// When the user clicks on a column header, sort the detail grid
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">contains information about which field to sort on</param>
		protected void DetailGrid_Sorting(object sender, GridViewSortEventArgs e)
		{
			try
			{
				List<MeterReconciliationDetailData> detailData = this.SessionDetailData;

				var param = Expression.Parameter(typeof(MeterReconciliationDetailData), e.SortExpression);
				var sortExpression = Expression.Lambda<Func<MeterReconciliationDetailData, object>>(Expression.Convert(Expression.Property(param, e.SortExpression), typeof(object)), param);

				if (this.GridViewSortDirection == SortDirection.Ascending)
				{
					this.DetailGrid.DataSource = detailData.AsQueryable<MeterReconciliationDetailData>().OrderBy(sortExpression);
					this.GridViewSortDirection = SortDirection.Descending;
				}
				else
				{
					this.DetailGrid.DataSource = detailData.AsQueryable<MeterReconciliationDetailData>().OrderByDescending(sortExpression);
					this.GridViewSortDirection = SortDirection.Ascending;
				}

				this.DetailGrid.DataBind();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the user clicks on the view details button for a row in the grid. 
		/// We navigate the user to the transaction detail page for the transaction which corresponds
		/// to the row in the gird
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">contains the command argument, which is in this case the index of the row the user clicked on</param>
		protected void DetailGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			// Create session object for TransactionDetail list of transactions.
			var detailList = new TransactionDetailList();

			// Put each transaction ID into the list for Previous/Next buttons.
			foreach (GridViewRow row in this.DetailGrid.Rows)
			{
				var transId = ((Literal)row.FindControl("DetailTransactionIDGridHidden")).Text;
				detailList.TransactionIDList.Add(transId);
			}

			// Indicate which transaction id in the list is the one to initially display.
			detailList.CurrentIndex = Convert.ToInt32(e.CommandArgument);

			var selectedTransactionGuid = ((Literal)this.DetailGrid.Rows[detailList.CurrentIndex].FindControl("DetailTransactionGuidText")).Text;
			var validatedTransactionGuid = selectedTransactionGuid == null ? "" : selectedTransactionGuid.ToString();
			if (Guid.TryParse(validatedTransactionGuid, out Guid parsedSelectedTransactionGuid))
			{
				detailList.SelectedTransactionGuid = parsedSelectedTransactionGuid;
			}
			var possiblyNull = ((Literal)this.DetailGrid.Rows[detailList.CurrentIndex].FindControl("DetailTransactionAliasGridHidden")).Text;
			if (possiblyNull != null)
			{
				detailList.SelectedTransactionAliasID = possiblyNull.ToString();
			}

			//// Escape the alias name for any URL special characters (i.e. & ' / ? ! # $ * + , : ; = @ [ ])
			//string columnName = Uri.EscapeDataString(this.Request.GetQueryOrFormValue("Column"));

			// Indicate the return URL for when the TransactionDetail Close button is clicked.
			detailList.ReturnURL = "../Accounting/MeterReconciliationDetail.aspx?Returning=true";
			//MeterGuid =" + SessionMeterGuid + 
			//	"&AssetGuid=" + this.SessionMeterAssetGuid + "&AssetID=" + assetID + "&InventoryDate=" + this.SessionDetailInventoryDate;

			// Put the object into session and transfer to the TransactionDetail.
			this.Session[TransactionDetailList.TransactionDetailListKey] = detailList;

			// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

			if (this.Request.GetQueryOrFormValue("TrackReturn") == "true")
			{
				if (transactionDetailUrl.IndexOf('?') > 0)
				{
					transactionDetailUrl += "&TrackReturn=true";
				}
				else
				{
					transactionDetailUrl += "?TrackReturn=true";
				}
			}

			this.Redirect("../" + transactionDetailUrl);
		}

		/// <summary>
		/// Fires when a row is bound to the detail grid. This allows us to do things like color the row red if the meter skip value
		/// is out of tolerance
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">Contains information about the bound row</param>
		protected void DetailGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					// Set the command argument which will be used if a user tries to view the associated transaction record
					FMEditLinkButton view = (FMEditLinkButton)e.Row.FindControl("DetailViewTransactionButton");

					if (view != null)
					{
						view.CommandArgument = e.Row.RowIndex.ToString();
					}

					// If the meter skip is greater than the tolerance value, color it red
					if (e.Row.DataItem != null && e.Row.DataItem is MeterReconciliationDetailData data)
					{
						MeterReconciliationDetailData detailLine = data;

						NumberFormatInfo siteNumberFormat = this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);

						Label meterStartLabel = (Label)e.Row.FindControl("DetailMeterStartGridColumn");
						if (meterStartLabel != null)
						{
							meterStartLabel.Text = detailLine.MeterStart.ToString(siteNumberFormat);
						}

						Label meterStopLabel = (Label)e.Row.FindControl("DetailMeterStopGridColumn");
						if (meterStopLabel != null)
						{
							meterStopLabel.Text = detailLine.MeterStop.ToString(siteNumberFormat);
						}

						Label meterTotalLabel = (Label)e.Row.FindControl("DetailMeterTotalGridColumn");
						if (meterTotalLabel != null)
						{
							meterTotalLabel.Text = detailLine.MeterTotal.ToString(siteNumberFormat);
						}

						Label volumeLabel = (Label)e.Row.FindControl("DetailVolumeGridColumn");
						if (volumeLabel != null)
						{
							volumeLabel.Text = detailLine.GrossVolume.ToString(siteNumberFormat);
						}

						Label meterSkipLabel = (Label)e.Row.FindControl("DetailSkipGridLabel");
						if (meterSkipLabel != null)
						{
							meterSkipLabel.Text = detailLine.MeterSkip.ToString(siteNumberFormat);
						}

						string toleranceValueString = this.SkipToleranceValueTextBox.Text;

						if (double.TryParse(toleranceValueString,
									 NumberStyles.AllowThousands | NumberStyles.Float,
									 siteNumberFormat,
									 out double toleranceValue))
						{
							e.Row.ForeColor = detailLine.MeterSkip > toleranceValue ? Color.Red : Color.Black;
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
		/// When the user clicks the refresh button, resubmit the search and refresh the result grids
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the user clicks the close button, return to the meter reconciliation summary page
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void CloseButton_Click(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("MeterReconciliationDetailData");
				this.Session.Remove("MeterReconciliationDetailInventoryDate");
				this.Session.Remove("MeterAssetGuid");
				this.Session.Remove("MeterReconciliationDetailSummaryData");

				this.Redirect("MeterReconciliationSummary.aspx?Returning=true");
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#endregion
	}
}