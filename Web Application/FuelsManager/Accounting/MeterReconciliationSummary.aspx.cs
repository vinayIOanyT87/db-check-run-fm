namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

	using FMControls;

	using FMCore;

	/// <summary>
	/// The meter reconciliation summary page allows a user to view meter reconciliation information for all 
	/// meters in the system for a specified inventory date.
	/// </summary>
	public partial class MeterReconciliationSummary : AccountingWebFormView
	{
		/// <summary>
		/// These are the values available in the Tolerance filter drop down box
		/// </summary>
		private enum ToleranceValueFilter
		{
			AllMeters = 0, //Show all meters
			InTolerance = 1, //Show in tolerance meters only
			OutOfTolerance = 2 //Show out of tolerance meters only
		};

		private const string allText = "{All}";

		#region Page Properties

		private AccountingSite accountingSite = null;

		/// <summary>
		/// Defines the DateTimeFormatInfo for the inventory date display box
		/// </summary>
		protected DateTimeFormatInfo _dateFormat = DateTimeFormatInfo.CurrentInfo;

		private bool toleranceIsPercent = false;

		/// <summary>
		/// Indicates whether the tolerance value entered is a percent or a quantity
		/// Setting this will also hide or show a "%" label next to the tolerance value input text box
		/// </summary>
		protected bool ToleranceIsPercent
		{
			get
			{
				return this.toleranceIsPercent;
			}
			set
			{
				this.ToleranceIsPercentLabel.Visible = value;
				this.toleranceIsPercent = value;
			}
		}

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

		#endregion

		#region Session Properties

		/// <summary>
		/// Get or set the data bound to the grid, which is stored in session 
		/// </summary>
		private List<MeterReconciliationSummaryData> SessionSummaryData
		{
			get
			{
				if (this.Session["SummaryData"] != null && this.Session["SummaryData"] is List<MeterReconciliationSummaryData>)
				{
					return this.Session["SummaryData"] as List<MeterReconciliationSummaryData>;
				}
				else
				{
					return new List<MeterReconciliationSummaryData>();
				}
			}
			set
			{
				this.Session.Add("SummaryData", value);
			}
		}

		/// <summary>
		/// Get or set the inventory date search field value, which persists in session
		/// </summary>
		private DateTimeOffset SessionInventoryDate
		{
			get
			{
				// The inventory date provided persists in Session. If we find the inventory date in session, use it. 
				// If we don't find it, set it to the default value, which is yesterday.
				if (this.Session["MeterReconciliationInventoryDate"] != null && this.Session["MeterReconciliationInventoryDate"] is DateTimeOffset offset)
				{
					return offset;
				}
				else
				{
					return DateTimeOffset.Now.AddDays(-1);
				}
			}
			set
			{
				this.Session.Add("MeterReconciliationInventoryDate", value);
			}
		}


		/// <summary>
		/// Get or set the meter guid of the row the user clicked on in the grid. 
		/// This is used to highlight the row the user was just looking at.
		/// </summary>
		private Guid SessionSelectedRowMeterGuid
		{
			get
			{

				if (this.Session["MeterReconciliationSelectedRowMeterGuid"] != null && this.Session["MeterReconciliationSelectedRowMeterGuid"] is Guid guid)
				{
					return guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				this.Session.Add("MeterReconciliationSelectedRowMeterGuid", value);
			}
		}

		/// <summary>
		/// Get or set the asset guid of the asset selected in the asset select box
		/// This is used to filter the meters available when selecting a meter
		/// </summary>
		private Guid SessionMeterAssetGuid
		{
			get
			{
				if (this.Session["MeterAssetGuid"] != null && this.Session["MeterAssetGuid"] is Guid guid)
				{
					return guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				this.Session.Add("MeterAssetGuid", value);
			}
		}

		#endregion

		#region Page Event Handlers

		/// <summary>
		/// Fires when the page loads. We use this opportunity to do setup style things, 
		/// like populating the search fields with default parameters
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Get the site information, which we'll use to get the tolerance is percent and report name settings
				this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(accountingSites =>
						accountingSites.LoadSiteInfoNoCompanies(this.Security, this.Security.SiteGuid)
				);

				if (this.IsPostBack == false)
				{
					//Reset the session variable we use to remember the asset selected in the asset select box
					this.SessionMeterAssetGuid = Guid.Empty;

					this.PopulateSearchFields();

					// If the session is null, then remove all objects from the 
					// session and display the accounting error page.
					if (this.Session["Security"] == null)
					{
						this.Session.RemoveAll();
						base.DisplayErrorPage();
						return;
					}

					if (this.Request.GetQueryOrFormValue("Returning") == null || Convert.ToBoolean(this.Request.GetQueryOrFormValue("Returning")) == false)
					{
						this.SessionSelectedRowMeterGuid = Guid.Empty;
					}

					// Check the user's security access
					this.CheckUserSecurityAccess(base.Security);

					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the user clicks on the column header to sort data in the grid.
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">Contains the field the user wants to sort</param>
		protected void SummaryGrid_Sorting(object sender, GridViewSortEventArgs e)
		{
			try
			{
				List<MeterReconciliationSummaryData> summaryData = this.SessionSummaryData;

				var param = Expression.Parameter(typeof(MeterReconciliationSummaryData), e.SortExpression);
				var sortExpression = Expression.Lambda<Func<MeterReconciliationSummaryData, object>>(Expression.Convert(Expression.Property(param, e.SortExpression), typeof(object)), param);

				// Sort the currently bound data in ascending or descending order
				if (this.GridViewSortDirection == SortDirection.Ascending)
				{
					this.BindData(this.SummaryGrid.DataSource = summaryData.AsQueryable<MeterReconciliationSummaryData>().OrderBy(sortExpression));
					this.GridViewSortDirection = SortDirection.Descending;
				}
				else
				{
					this.BindData(summaryData.AsQueryable<MeterReconciliationSummaryData>().OrderByDescending(sortExpression));
					this.GridViewSortDirection = SortDirection.Ascending;
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

		}

		/// <summary>
		/// This event handles what happens when a user clicks on the view details button in a row.
		/// We gather some information and then send the user to the detail page to see the results 
		/// for the row they selected
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">contains the command argument, which is in this case the index of the row the user clicked on</param>
		protected void SummaryGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("ViewDetails", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);

					// You can't access the row's data here. We use the grid's datakeys property instead to store the meter guid.
					string meterGuid = this.SummaryGrid.DataKeys[rowIndex].Values["MeterGuid"].ToString();
					string assetGuid = this.SummaryGrid.DataKeys[rowIndex].Values["AssetGuid"].ToString();
					string assetID = this.SummaryGrid.DataKeys[rowIndex].Values["AssetID"].ToString();

					string inventoryDate = this.InventoryDate.CurrentValue.Date.ToString();

					this.SessionSelectedRowMeterGuid = Guid.Parse(meterGuid);

					this.Redirect("MeterReconciliationDetail.aspx?MeterGuid=" + meterGuid + "&AssetGuid=" + assetGuid + "&AssetID=" + assetID + "&InventoryDate=" + inventoryDate);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This event fires when a row is bound to the summary grid.
		/// We perform row-specific logic like highlighting rows that are out of tolerance
		/// and setting the error indicator
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">Contains the row being bound</param>
		protected void SummaryGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					// Set the command argument for the View details button.
					// We use this when the user presses the View details button to determine which record
					// they want to view details for.
					FMViewLinkButton view = (FMViewLinkButton)e.Row.FindControl("ViewDetailsButton");

					if (view != null)
					{
						view.CommandArgument = e.Row.RowIndex.ToString();
					}

					if (e.Row.DataItem != null && e.Row.DataItem is MeterReconciliationSummaryData)
					{
						MeterReconciliationSummaryData summaryData = e.Row.DataItem as MeterReconciliationSummaryData;

						// If this is a row the user has previously viewed details for, highlight it
						if (this.SessionSelectedRowMeterGuid != Guid.Empty)
						{
							Guid selectedRowMeterGuid = this.SessionSelectedRowMeterGuid;

							if (summaryData.MeterGuid == selectedRowMeterGuid)
							{
								e.Row.BackColor = Color.LightBlue;
								this.SessionSelectedRowMeterGuid = Guid.Empty;
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

						Label transactionMeterTotalLabel = (Label)e.Row.FindControl("TransactionMeterTotalGridColumn");
						if (transactionMeterTotalLabel != null)
						{
							transactionMeterTotalLabel.Text = summaryData.TransactionMeterTotal.ToString(this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
						}

						Label transactionVolumeTotalLabel = (Label)e.Row.FindControl("TransactionVolumeTotalGridColumn");
						if (transactionVolumeTotalLabel != null)
						{
							transactionVolumeTotalLabel.Text = summaryData.TransactionVolumeTotal.ToString(this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
						}

						Label meterVarianceLabel = (Label)e.Row.FindControl("MeterVarianceGridColumn");
						if (meterVarianceLabel != null)
						{
							meterVarianceLabel.Text = summaryData.MeterVariance.ToString(siteNumberFormat);
						}

						Label volumeVarianceLabel = (Label)e.Row.FindControl("VolumeVarianceGridColumn");
						if (volumeVarianceLabel != null)
						{
							volumeVarianceLabel.Text = summaryData.VolumeVariance.ToString(siteNumberFormat);
						}

						// Determine if the meter is out of tolerance. 
						// If it is, highlight it in red
						string toleranceValueString = this.ToleranceValueTextBox.Text;

						if (double.TryParse(toleranceValueString,
									 NumberStyles.Float | NumberStyles.AllowThousands,
									 siteNumberFormat,
									 out double toleranceValue))
						{
							bool outOfTolerance = false;

							if (this.ToleranceIsPercent)
							{
								double meterVariancePercent = (summaryData.MeterVariance / summaryData.MeterTotal) * 100;
								double volumeVariancePercent = (summaryData.VolumeVariance / summaryData.MeterTotal) * 100;
								outOfTolerance = (meterVariancePercent >= toleranceValue) || (volumeVariancePercent >= toleranceValue);
							}
							else
							{
								outOfTolerance = (summaryData.MeterVariance >= toleranceValue) || (summaryData.VolumeVariance >= toleranceValue);
							}

							e.Row.ForeColor = outOfTolerance ? Color.Red : Color.Black;
						}

						// If we were unable to determine the meter start or stop values for the meter in this row, 
						// Display an error image and set the tooltip text to display the error message(s)

						if (e.Row.FindControl("ErrorImage") is WebControl errorImage)
						{
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
		/// When the user clicks the refresh button, set the search criteria, get the data, 
		/// and refresh the screen
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
		/// When the user clicks the Generate Report button, display the meter reconciliation report on the report landing page
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void ReportButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.accountingSite != null && this.accountingSite.CurrentSite != null)
				{
					if (!string.IsNullOrEmpty(this.accountingSite.CurrentSite.MeterReconciliationReportName))
					{
						string configuredReportName = this.accountingSite.CurrentSite.MeterReconciliationReportName;
						string reportName = configuredReportName.Replace(" ", "+");

						string reportURL = "../FMReportWebMain/PopupReportLandingPage.aspx?ReportType=";

						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsAviationProduct()))
						{
							string aviation = ((int)ReportTypesClass.ReportTypes.AVIATION_RPT).ToString(CultureInfo.InvariantCulture);
							reportURL += aviation;
						}
						else
						{
							string oilAndGas = ((int)ReportTypesClass.ReportTypes.OIL_GAS_RPT).ToString(CultureInfo.InvariantCulture);
							reportURL += oilAndGas;
						}

						reportURL += "&ReportName=" + reportName;
						reportURL += "&" + this.Security.CSRFTokenWithParamName;

						string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" +
																 "window.open('" + reportURL + "', " +
																 "'Reports', " +
																 "'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=800, width=1000'" +
																 "); \n" +
																 "-->\n</script>";

						this.Response.Cookies.Add(new HttpCookie("Token", this.Session["Token"] as string));
						this.ClientScript.RegisterStartupScript(this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
					}
					else
					{
						this.ErrorHandler(new Exception("Meter Reconciliation Report has not been configured in the Site's System Settings"));
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
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
		/// Fill the search filter fields at the top of the page with default values.
		/// </summary>
		private void PopulateSearchFields()
		{
			// The default tolerance value is 5.
			this.ToleranceValueTextBox.Text = "5";

			this.ProductTextBox.Text = allText;
			this.ManagerTextBox.Text = allText;
			this.CarrierTextBox.Text = allText;
			this.MeterIDTextBox.Text = allText;
			this.AssetTextBox.Text = allText;

			// The inventory date persists in session. If it's not there, we use the default date, which is yesterday
			this.InventoryDate.CurrentValue = this.SessionInventoryDate;
		}

		/// <summary>
		/// Bind the parameter provided to the summary grid
		/// </summary>
		/// <param name="meterReconciliationSummaryCollection">The data to bind</param>
		private void BindData(object meterReconciliationSummaryCollection)
		{
			this.SummaryGrid.DataSource = meterReconciliationSummaryCollection;

			this.SummaryGrid.DataBind();
		}

		/// <summary>
		/// Resubmit the query and update the grid with the results
		/// </summary>
		private void UpdateView()
		{
			try
			{
				MeterReconciliationSR sr = new MeterReconciliationSR();

				// Set the search filtering criteria
				this.SetSearchCriteria(sr);

				// Process the service request
				List<MeterReconciliationSummaryData> meterReconciliationSummaryCollection =
					FMChannelHelper.MakeCall<IMeterReconciliationProcessor, List<MeterReconciliationSummaryData>>(meterReconciliationProcessor => meterReconciliationProcessor.GetSummary(sr));

				// Bind the search results to the grid
				this.SessionSummaryData = meterReconciliationSummaryCollection;
				this.BindData(meterReconciliationSummaryCollection);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Populate our request object with the search values the user specified
		/// </summary>
		/// <param name="serviceRequest">The request we will send to the server to get summary data</param>
		private void SetSearchCriteria(MeterReconciliationSR serviceRequest)
		{
			try
			{
				// if the inventory date is not in session, add it. If the inventory date is in session, set it.
				this.SessionInventoryDate = this.InventoryDate.CurrentValue;

				serviceRequest.Security = this.Security;
				serviceRequest.InventoryDate = this.InventoryDate.CurrentValue.Date;
				serviceRequest.CurrentSiteGuid = this.Security.SiteGuid;

				// Set the tolerance parameter on the request. Null = show all, false = show in tolerance only, true = show out of tolerance only
				if (this.ToleranceDropDownList.SelectedValue == ((int)ToleranceValueFilter.AllMeters).ToString())
				{
					serviceRequest.InOutOfTolerance = null;
				}
				else if (this.ToleranceDropDownList.SelectedValue == ((int)ToleranceValueFilter.InTolerance).ToString())
				{
					serviceRequest.InOutOfTolerance = false;
				}
				else if (this.ToleranceDropDownList.SelectedValue == ((int)ToleranceValueFilter.OutOfTolerance).ToString())
				{
					serviceRequest.InOutOfTolerance = true;
				}

				// Set the tolerance value

				serviceRequest.ToleranceValue = double.TryParse(this.ToleranceValueTextBox.Text,
						NumberStyles.Float | NumberStyles.AllowThousands,
						this.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT),
						out double toleranceValue)
					? toleranceValue
					: throw new ApplicationException("Tolerance value must be numeric");

				// Set the meter asset to filter results on
				if (!string.IsNullOrEmpty(this.AssetTextBox.Text) && this.AssetTextBox.Text != allText)
				{
					serviceRequest.AssetGuid = this.SessionMeterAssetGuid;
				}

				// Set the meter to filter results on
				if (!string.IsNullOrEmpty(this.MeterIDTextBox.Text) && this.MeterIDTextBox.Text != allText)
				{
					serviceRequest.MeterGuid = FMChannelHelper.MakeCall<IMeters, Guid>((meters) => meters.GetIdentityGuid(this.Security, this.MeterIDTextBox.Text));
				}

				// We might create a channel factory to get company search parameters.
				// Check to make sure that we will use the factory before creating it.
				bool hasManager = (!string.IsNullOrEmpty(this.ManagerTextBox.Text) && this.ManagerTextBox.Text != allText);
				bool hasCarrier = (!string.IsNullOrEmpty(this.CarrierTextBox.Text) && this.CarrierTextBox.Text != allText);

				if (hasManager || hasCarrier)
				{
					// Set the Manager to filter results on
					if (hasManager)
					{
						serviceRequest.ManagerCompanyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>((companies) => companies.GetIdentityGuid(this.Security, this.ManagerTextBox.Text));
					}

					// Set the Carrier to filter results on
					if (hasCarrier)
					{
						serviceRequest.CarrierCompanyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>((companies) => companies.GetIdentityGuid(this.Security, this.CarrierTextBox.Text));
					}
				}

				// Set the Product to filter results on
				if (!string.IsNullOrEmpty(this.ProductTextBox.Text) && this.ProductTextBox.Text != allText)
				{
					serviceRequest.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(products => products.GetMasterRecordGuidFromID(this.Security, this.ProductTextBox.Text));
				}

				// Set the parameter which indicates whether the tolerance value is a percent or a quantity
				if (this.accountingSite != null && this.accountingSite.CurrentSite.MeterReconciliationToleranceIsPercent)
				{
					this.ToleranceIsPercent = true;
					serviceRequest.ToleranceIsPercent = true;
				}
				else
				{
					this.ToleranceIsPercent = false;
					serviceRequest.ToleranceIsPercent = false;
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#endregion
	}
}