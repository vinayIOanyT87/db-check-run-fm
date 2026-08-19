// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppointmentSummary.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.AppointmentWebApp
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	using FMControls;

	using FMWebApp;

	public partial class AppointmentSummary : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		private const string AppointmentGuid = "SelectedAppointmentGuid";
		public const string AppointmentMode = "AppointmentSummaryMode";
		protected DateTimeFormatInfo dateFormat = DateTimeFormatInfo.CurrentInfo;
		private string SessionSuffix { get { return (this.IsGetTestScheduleMode ? "Test" : ""); } }
		private string AppointmentAssetSelection { get { return "AppointmentSummaryAssetSelection" + this.SessionSuffix; } }
		private string AppointmentEndTime { get { return "AppointmentSummaryEndTime" + this.SessionSuffix; } }
		private string AppointmentSortDirection { get { return "AppointmentSummarySortDirection" + this.SessionSuffix; } }
		private string AppointmentSortExpresion { get { return "AppointmentSummarySortExpression" + this.SessionSuffix; } }
		private string AppointmentStartTime { get { return "AppointmentSummaryStartTime" + this.SessionSuffix; } }
		private string AppointmentTypeSelection { get { return "AppointmentSummaryTypeSelection" + this.SessionSuffix; } }
		private string PreviousAppointment { get { return "PreviousAppointment"; } } //used on another page so don't add the suffix

		#endregion

		#region Public Properties

		public DateTimeFormatInfo DateFormat
		{
			get
			{
				return this.dateFormat;
			}
		}

		#endregion

		#region Properties

		protected bool IsGetTestScheduleMode
		{
			get
			{
				return this.Request.GetQueryOrFormValue("MODE").DefaultIfNullOrEmpty("NORMAL").Equals("GETTEST");
			}
		}

		#endregion

		#region Public Methods and Operators

		public void AddResultButtonClick(object sender, EventArgs e)
		{
			string transferText;
			try
			{
				var button = (FMButton)sender;
				var row = (GridViewRow)button.NamingContainer;

				TableCell identityGuidCell = row.Cells[3];
				Guid appointmentGuid = Guid.Parse(identityGuidCell.Text);

				AppointmentClass appointment = FMChannelHelper.MakeCall<IAppointments, AppointmentClass>(x => x.Get(this.Security,appointmentGuid));
				
				string appointmentType = appointment.AssociatedType.Equals("TANKS", StringComparison.OrdinalIgnoreCase)
					                         ? "Tank"
					                         : appointment.AssociatedType;
				
				transferText =
					string.Format(
						this.TestSetResultFormUrl + "?MODE=ADD&TEST={0}&ENTITY={1}&ASSETTYPE={2}&APPOINTMENT={3}",
						appointment.TestSetDefinitionGuid,
						appointment.AssociatedTypeGuid,
						appointmentType,
						appointment.IdentityGuid);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			if (string.IsNullOrEmpty(transferText) == false)
			{
				this.Redirect(transferText);
			}
		}


		protected string TestSetResultFormUrl
		{
			get
			{
				string testSetResultFormUrl = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(Security, "TestSetResultFormURL"));

				if (string.IsNullOrEmpty(testSetResultFormUrl))
				{
					testSetResultFormUrl = "QualityControlWebApp/TestSetResultForm.aspx";
				}
				return "../" + testSetResultFormUrl;
			}
		}

		/// <summary>
		///    Override to distinguish how the page is being used
		/// </summary>
		/// <returns>Key for lookup into tblHelpMapping</returns>
		public override string GetHelpContextKey()
		{
			return base.GetHelpContextKey() + "|" + this.Request.GetQueryOrFormValue("MODE").DefaultIfNullOrEmpty("NORMAL");
		}

		#endregion


		#region Methods

		protected void AppointmentSummaryDataGridRowCommandReceived(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName == "Edit")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.AppointmentSummaryDataGrid.Rows[index];
					TableCell identityGuidCell = row.Cells[3];
					this.Session.Remove(AppointmentGuid);
					this.Session[AppointmentGuid] = identityGuidCell.Text;
					this.Redirect("AppointmentDetailPage.aspx");
				}
				else if (e.CommandName == "Delete")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.AppointmentSummaryDataGrid.Rows[index];
					TableCell identityGuidCell = row.Cells[3];
					// get the current appointment
					AppointmentClass appointment = FMChannelHelper.MakeCall<IAppointments, AppointmentClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(identityGuidCell.Text))
																);
					
					FMChannelHelper.MakeCall<IAppointments>(
																	 x =>
																	 x.Purge(this.Security, Guid.Parse(identityGuidCell.Text))
																);
					if (appointment != null)
					{
						if (appointment.TestSetDefinitionGuid != Guid.Empty
						    && appointment.AssociatedType == this.GetTranslatedText("Equipment"))
						{
							// get the equipment
							EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
												x => 
												x.Get(this.Security, appointment.AssociatedTypeGuid)
											);

							if (equipment != null)
							{
								// check the qc due date
								DateTimeOffset dateNow = TimeConverter.Today().AddMilliseconds(-1);
								DateTimeOffset qcdate = 
									FMChannelHelper.MakeCall<IAppointments, DateTimeOffset>(
										x =>
										x.GetNextQCDateForAsset(this.Security, appointment.AssociatedTypeGuid, 
											this.GetTranslatedText("Equipment"), dateNow)
									);

								if (qcdate >= dateNow)
								{
									equipment._QCDate.Value = qcdate;
									FMChannelHelper.MakeCall<IEquipments>(
										  x =>
										  x.Modify(this.Security, equipment)
									 );
								}
							}
						}
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void AppointmentSummaryDataGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			// we do this here because autocreatedcolumns do not exist as an object in the grid
			if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header
			    || e.Row.RowType == DataControlRowType.Footer)
			{
				if (this.IsGetTestScheduleMode == false)
				{
					e.Row.Cells[2].Visible = false;
				}

				//we first bind to an empty data grid, which only has a few columns. So check the 
				//count of the cells before accessing the collection
				if (e.Row.Cells.Count >= 4)
				{
					e.Row.Cells[3].Visible = false;
				}

				if (e.Row.Cells.Count >= 11)
				{
					e.Row.Cells[10].Visible = false; // Fueling Type
				}

				if (this.Security.HasRight(RIGHT.MODIFY_APPOINTMENTS) == false)
				{
					e.Row.Cells[0].Visible = false;
					e.Row.Cells[1].Visible = false;
					e.Row.Cells[2].Visible = false;
				}
			}

			// Set colors based on data and date
			if (this.Request.GetQueryOrFormValue("MODE").DefaultIfNullOrEmpty(string.Empty).Equals("GETTEST"))
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					TableCell dateCell = e.Row.Cells[5];
					DateTimeOffset date = DateTimeOffset.Parse(dateCell.Text);

					e.Row.ForeColor = FMColor.DarkRed;

					if (date.Date.Equals(this.StartDate.CurrentValue.Date))
					{
						if (e.Row.Cells[10].Text.Equals("1"))
						{
							e.Row.ForeColor = FMColor.DarkBlue;
						}
					}
					else
					{
						e.Row.ForeColor = FMColor.DarkGray;
					}
				}
			}
		}

		protected void AppointmentSummaryDataGridSorting(object sender, GridViewSortEventArgs e)
		{
			string selectSortDirection = this.getSortDirectionString(e.SortDirection);

			if (this.Session[this.AppointmentSortExpresion] != null && this.Session[this.AppointmentSortDirection] != null)
			{
				var lastSortedColumn = this.Session[this.AppointmentSortExpresion] as string;
				if (lastSortedColumn == e.SortExpression)
				{
					var lastSortDirection = this.Session[this.AppointmentSortDirection] as string;
					if (lastSortDirection == selectSortDirection && selectSortDirection == "ASC")
					{
						selectSortDirection = "DESC";
					}
					else
					{
						selectSortDirection = "ASC";
					}
				}
			}

			this.Session.Add(this.AppointmentSortExpresion, e.SortExpression);
			this.Session.Add(this.AppointmentSortDirection, selectSortDirection);

			this.UpdateView();
		}

		protected void AssetDropDown_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			this.AppointmentSummaryDataGrid.PageIndex = 0;
			this.Session.Remove(this.AppointmentAssetSelection);
			this.Session.Add(this.AppointmentAssetSelection, this.AssetDropDownList.SelectedItem.Text);
			this.UpdateSessionVariables();
			this.UpdateView();
		}

		protected void OnAddNewAppointment(object sender, EventArgs e)
		{
			// for an add clear the session and call the add appointment page
			this.Session.Remove(AppointmentGuid);
			this.Redirect("AppointmentDetailPage.aspx");
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InititializeComponents();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(
									this.Security,
									this.Security.SiteGuid,
									getMemberSites: true,
									getSchedulesAndProcessVariables: true,
									bGetAssociatedAliases: true)
								);

				DateTimeOffset siteTimeToday = TimeConverter.Today(site);

				if (!this.Page.IsPostBack)
				{
					this.Session.Remove(this.PreviousAppointment);
					this.Session.Remove(AppointmentGuid);
					this.Session.Remove(this.AppointmentSortDirection);
					this.Session.Remove(this.AppointmentSortExpresion);
					this.Session.Remove(AppointmentMode);
					this.Session.Add(AppointmentMode, this.Request.GetQueryOrFormValue("MODE"));

					// add the default sort
					this.Session.Add(this.AppointmentSortExpresion, this.GetTranslatedText("Due Date"));
					this.Session.Add(this.AppointmentSortDirection, "ASC");

					if (this.IsGetTestScheduleMode)
					{
						this.EndDateLabel.Visible = false;
						this.EndDate.Visible = false;
						this.PageTitle.Text = "Scheduled/Overdue Tests";
						this.StartDateLabel.Text = "Due Date:";
					}

					if (this.Session[this.AppointmentStartTime] != null)
					{
						var stStartTime = this.Session[this.AppointmentStartTime] as string;
						DateTimeOffset startTime = DateTimeOffset.Parse(stStartTime);
						this.StartDate.CurrentValue = startTime;
					}
					else
					{
						this.StartDate.CurrentValue = siteTimeToday;
						this.Session.Add(this.AppointmentStartTime, this.StartDate.CurrentValue.ToString());
					}

					if (this.Session[this.AppointmentEndTime] != null)
					{
						var stEndTime = this.Session[this.AppointmentEndTime] as string;
						DateTimeOffset endTime = DateTimeOffset.Parse(stEndTime);
						this.EndDate.CurrentValue = endTime;
					}
					else
					{
						this.EndDate.CurrentValue = siteTimeToday;
						this.Session.Add(this.AppointmentEndTime, this.EndDate.CurrentValue.ToString());
					}

					this.LoadTypeDropDown();

					this.UpdateView();

					if ((this.Security.HasRight(RIGHT.MODIFY_APPOINTMENTS)
						|| Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
						)&& this.TypeDropDownList.Items.Count > 1) //make sure it has items they can perform actions on
					{
						this.AddButton.Enabled = true;
						this.AddButton2.Enabled = true;
					}
					else
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void RefreshButtonOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(this.AppointmentSortDirection);
			this.Session.Remove(this.AppointmentSortExpresion);

			// add the default sort
			this.Session.Add(this.AppointmentSortExpresion, this.GetTranslatedText("Due Date"));
			this.Session.Add(this.AppointmentSortDirection, "ASC");

			this.UpdateSessionVariables();

			this.AppointmentSummaryDataGrid.PageIndex = 0;

			this.UpdateView();
		}

		protected void TypeDropDown_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			this.AppointmentSummaryDataGrid.PageIndex = 0;
			this.Session.Remove(this.AppointmentAssetSelection);
			this.UpdateSessionVariables();
			this.UpdateView();
		}

		private DataView EnumerateScheduledAppointments()
		{
			var timer = new StopWatch(StopWatch.Appnames.Accounting, "EnumerateScheduledAppointments");

			string selectAsset = string.Empty;

			string translatedPersonnel = GetTranslatedText("Personnel");
			string translatedEquipment = GetTranslatedText("Equipment");
			string translatedTranks = GetTranslatedText("Tanks");

			AppointmentCollectionClass appointmentCollection;
			var appointmentDataTable = new DataTable();

			
			// if the user has an invalid date selection just return
			if (this.StartDate.CurrentValue > this.EndDate.CurrentValue)
			{
				this.EndDate.CurrentValue = this.StartDate.CurrentValue;
			}
			// ensure the dates are at midnight
			this.StartDate.CurrentValue = this.StartDate.CurrentValue.AddHours(-this.StartDate.CurrentValue.Hour);
			this.StartDate.CurrentValue = this.StartDate.CurrentValue.AddMinutes(-this.StartDate.CurrentValue.Minute);
			this.StartDate.CurrentValue = this.StartDate.CurrentValue.AddSeconds(-this.StartDate.CurrentValue.Second);
			this.EndDate.CurrentValue = this.EndDate.CurrentValue.AddHours(-this.EndDate.CurrentValue.Hour);
			this.EndDate.CurrentValue = this.EndDate.CurrentValue.AddMinutes(-this.EndDate.CurrentValue.Minute);
			this.EndDate.CurrentValue = this.EndDate.CurrentValue.AddSeconds(-this.EndDate.CurrentValue.Second);

			appointmentDataTable.Columns.Add(this.GetTranslatedText("IdentityGuid"), typeof(Guid));
			appointmentDataTable.Columns.Add(this.GetTranslatedText("Asset"), typeof(string));
			appointmentDataTable.Columns.Add(this.GetTranslatedText("Due Date"), typeof(DateTimeOffset));
			appointmentDataTable.Columns.Add(this.GetTranslatedText("Description"), typeof(string));
			appointmentDataTable.Columns.Add(this.GetTranslatedText("Type"), typeof(string));
			appointmentDataTable.Columns.Add(this.GetTranslatedText("Category"), typeof(string));
			appointmentDataTable.Columns.Add(this.GetTranslatedText("Occurrence"), typeof(string));
			appointmentDataTable.Columns.Add(this.GetTranslatedText("Fueling Type"), typeof(string));

			ReloadDataInGrid:
			if (this.IsGetTestScheduleMode)
			{
				var timer2 = new StopWatch(StopWatch.Appnames.Accounting, "EnumerateScheduledAndOverdue BLL call");
				appointmentCollection = 
					FMChannelHelper.MakeCall<IAppointments, AppointmentCollectionClass>(
						x =>
						x.EnumerateScheduledAndOverdue(this.Security, this.StartDate.CurrentValue, 
							this.TypeDropDownList.SelectedItem.Text)
					);

				timer2.Stop();
			}
			else
			{
				appointmentCollection = 
					FMChannelHelper.MakeCall<IAppointments, AppointmentCollectionClass>(
							x =>
							x.EnumerateByStartStopTime(this.Security, this.TypeDropDownList.SelectedItem.Text, 
								this.StartDate.CurrentValue, this.EndDate.CurrentValue)
					);
			}

			this.AssetDropDownList.Items.Clear();
			this.AssetDropDownList.Items.Add(new ListItem("None", "0"));

			int iPosition = 1;
			if (this.Session[this.AppointmentAssetSelection] != null)
			{
				selectAsset = this.Session[this.AppointmentAssetSelection] as string;
			}

			var timer3 = new StopWatch(StopWatch.Appnames.Accounting, "EnumerateScheduledAppointments Data Loop");
			timer3.Info(string.Format("AppointmentCollection size is {0}", appointmentCollection.Count));

			foreach (AppointmentClass appointmentData in appointmentCollection)
			{
				//only persons with modify person training should have personnel records otherwise they would get permission error later
				if (!Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) && appointmentData.AssociatedType.Equals(translatedPersonnel, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				//only persons with execute quality tests should have tanks or equipment otherwise they would get permission error later
				if (!Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& (appointmentData.AssociatedType.Equals(translatedEquipment, StringComparison.OrdinalIgnoreCase)
					|| appointmentData.AssociatedType.Equals(translatedTranks, StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}

				// we will enforce the asset filter here since we need to ensure the dropdown list is always populated

				if (string.IsNullOrEmpty(selectAsset) || selectAsset.Equals("None") || selectAsset == appointmentData.AssetText)
				{
					DataRow appointmentDataRow = appointmentDataTable.NewRow();

					appointmentDataRow["IdentityGuid"] = appointmentData.IdentityGuid;
					appointmentDataRow["Asset"] = appointmentData.AssetText;
					appointmentDataRow["Due Date"] = appointmentData.DueDate; //.ToString(CurrentSite.GetDateTimeFormatInfo());
					appointmentDataRow["Description"] = appointmentData.Description;
					appointmentDataRow["Type"] = appointmentData.AssociatedType;
					appointmentDataRow["Category"] = appointmentData.AppointmentCategory;
					appointmentDataRow["Occurrence"] = appointmentData.AppointmentPeriodText;
					appointmentDataRow["Fueling Type"] = appointmentData.EquipmentFuelingType == FUELING_TYPES.REFUELER ? "1" : "0";

					appointmentDataTable.Rows.Add(appointmentDataRow);
				}
				// add the asset to the asset selected filter display
				if (this.AssetDropDownList.Items.FindByText(appointmentData.AssetText) == null)
				{
					this.AssetDropDownList.Items.Add(new ListItem(appointmentData.AssetText, iPosition.ToString(CultureInfo.InvariantCulture)));
					++iPosition;
				}
			}

			timer3.Stop();
			appointmentDataTable.ConvertDateTimeOffsetColumns();
			var appointmentDataView = new DataView(appointmentDataTable);

			// check if sorting is enabled
			if (this.Session[this.AppointmentSortDirection] != null && this.Session[this.AppointmentSortExpresion] != null)
			{
				var sortExpression = this.Session[this.AppointmentSortExpresion] as string;
				var sortDirection = this.Session[this.AppointmentSortDirection] as string;
				appointmentDataView.Sort = sortExpression + " " + sortDirection;
			}

			if (!string.IsNullOrEmpty(selectAsset))
			{
				if (this.AssetDropDownList.SelectByText(selectAsset) == false)
				{
					this.AssetDropDownList.SelectByText("None");
					this.Session.Remove(this.AppointmentAssetSelection);
					selectAsset = string.Empty;
					goto ReloadDataInGrid;
				}
			}
			else
			{
				this.AssetDropDownList.SelectByText("None");
			}

			timer.Stop();

			return appointmentDataView;
		}

		private void InititializeComponents()
		{
			this.AppointmentSummaryDataGrid.RowDataBound += this.AppointmentSummaryDataGridRowDataBound;
			this.AppointmentSummaryDataGrid.RowCommand += this.AppointmentSummaryDataGridRowCommandReceived;
			this.AppointmentSummaryDataGrid.Sorting += this.AppointmentSummaryDataGridSorting;
			this.PrintButton.Click += this.PrintButtonClick;
		}

		private void LoadTypeDropDown()
		{
			this.TypeDropDownList.Items.Clear();

			this.TypeDropDownList.Items.Add(new ListItem("All"));

			if (this.IsGetTestScheduleMode == false && Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
			{
				this.TypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Personnel")));
			}

			if (Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS))
			{
				this.TypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Equipment")));
				this.TypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Tanks")));
			}

			// restore previous selection if present or default to all
			var typeSelection = this.Session[this.AppointmentTypeSelection] as string;
			if (typeSelection != null)
			{
				this.TypeDropDownList.SelectedIndex =
					this.TypeDropDownList.Items.IndexOf(this.TypeDropDownList.Items.FindByText(typeSelection));
			}
			else
			{
				this.TypeDropDownList.SelectedIndex =
					this.TypeDropDownList.Items.IndexOf(this.TypeDropDownList.Items.FindByText("All"));
				this.Session.Add(this.AppointmentTypeSelection, "All");
			}
		}

		private void PrintButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (this.IsGetTestScheduleMode)
				{
					string rptType = ((int)ReportTypesClass.ReportTypes.OVRDUE_TST_RPRT).ToString(CultureInfo.InvariantCulture);
					string rptUrl = "../FMReportWebMain/ReportLandingPage.aspx?ReportType=" + rptType;
					const string ReportName = "BSM-E ScheduledOverdueTestReport";

					rptUrl = rptUrl + "&ReportName=" + ReportName.Replace(" ", "+");
					rptUrl = rptUrl + "&SiteGuidStr=" + this.Security.SiteGuid;
					rptUrl = rptUrl + "&LoginSiteGuidStr=" + this.Security.LoginSiteGuid;
					rptUrl = rptUrl + "&UserGuidStr=" + this.Security.UserGuid;
					rptUrl = rptUrl + "&StartDate=" + this.StartDate.CurrentValue.AddDays(1).ToString("d");
					rptUrl += "&" + this.Security.CSRFTokenWithParamName;
                    rptUrl = rptUrl + "&PopupDisplay=TRUE";

                    string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" + "window.open('" + rptUrl + "', "
					                               + "'Reports', "
					                               + "'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=700, width=750'"
					                               + "); \n" + "-->\n</script>";

					this.Response.Cookies.Add(new HttpCookie("Token", this.Session["Token"] as String));
					ScriptManager.RegisterClientScriptBlock(
						this.Page, this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
				}
				else
				{
					DataView dataView = this.EnumerateScheduledAppointments();
					dataView.Table.Columns.Remove("IdentityGuid");
				
					this.Session[PrinterFriendlyDataView.PRINTER_FRIENDLY_DATA_VIEW] = dataView.ToTable();
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						"RPT_NEW_BROWSER",
						string.Format("window.open('../FMWebApp/PrinterFriendlyDataView.aspx?Title=Appointment Summary&{0}');", this.Security.CSRFTokenWithParamName),
						true);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateSessionVariables()
		{
			this.Session.Remove(this.AppointmentStartTime);
			this.Session.Remove(this.AppointmentEndTime);
			this.Session.Remove(this.AppointmentTypeSelection);

			this.Session.Add(this.AppointmentStartTime, this.StartDate.CurrentValue.ToString());
			this.Session.Add(this.AppointmentEndTime, this.EndDate.CurrentValue.ToString());
			this.Session.Add(this.AppointmentTypeSelection, this.TypeDropDownList.SelectedItem.Text);
		}

		private void UpdateView()
		{
			try
			{
				var emptyDataView = new DataView();
				this.AppointmentSummaryDataGrid.DataSource = emptyDataView;
				this.AppointmentSummaryDataGrid.DataBind();
				this.AppointmentSummaryDataGrid.DataSource = this.EnumerateScheduledAppointments();
				this.AppointmentSummaryDataGrid.DataBind();
			}
			catch (Exception e)
			{
				ErrorHandler(e);
			}

		}

		private string getSortDirectionString(SortDirection sortDirection)
		{
			string newSortDirection;
			if (sortDirection == SortDirection.Ascending)
			{
				newSortDirection = "ASC";
			}
			else
			{
				newSortDirection = "DESC";
			}

			return newSortDirection;
		}

		#endregion
	}
}